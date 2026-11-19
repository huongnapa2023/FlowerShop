using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FlowerShop.Models
{
    /// <summary>
    /// Giải quyết lỗi "Unable to load the SQLUserInstance.dll" (error 56 / %1 is not a valid Win32 application)
    /// khi IIS Express chạy dạng ARM64 gốc nhưng LocalDB chỉ có DLL x64.
    /// Thay vì để SqlClient tự nạp SQLUserInstance.dll để phân giải "(localdb)\Instance",
    /// lớp này gọi SqlLocalDB.exe (tiến trình riêng) để khởi động instance và lấy tên named pipe,
    /// sau đó kết nối thẳng qua "np:\\.\pipe\LOCALDB#xxxx\tsql\query".
    /// </summary>
    public static class LocalDbConnection
    {
        private const string ConnectionName = "FlowerShopDbContext";
        private static readonly object SyncRoot = new object();
        private static string _cachedConnectionString;
        private static string _cachedPipe;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        public static string GetConnectionString()
        {
            string configured = ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
            var builder = new SqlConnectionStringBuilder(configured);
            string dataSource = builder.DataSource ?? string.Empty;

            if (!dataSource.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase))
                return configured;

            lock (SyncRoot)
            {
                if (_cachedConnectionString != null && PipeExists(_cachedPipe))
                    return _cachedConnectionString;

                int slash = dataSource.IndexOf('\\');
                string instance = slash >= 0 ? dataSource.Substring(slash + 1) : "MSSQLLocalDB";

                string pipe = ResolvePipe(instance);
                if (string.IsNullOrEmpty(pipe))
                    return configured; // fallback về cách kết nối mặc định

                builder.DataSource = pipe;
                _cachedPipe = pipe;
                _cachedConnectionString = builder.ConnectionString;
                return _cachedConnectionString;
            }
        }

        private static bool PipeExists(string npDataSource)
        {
            if (string.IsNullOrEmpty(npDataSource)) return false;
            string pipePath = npDataSource.StartsWith("np:", StringComparison.OrdinalIgnoreCase)
                ? npDataSource.Substring(3)
                : npDataSource;

            if (WaitNamedPipe(pipePath, 1)) return true;
            int err = Marshal.GetLastWin32Error();
            return err == 121; // ERROR_SEM_TIMEOUT: pipe tồn tại nhưng đang bận
        }

        private static string ResolvePipe(string instance)
        {
            string exe = FindSqlLocalDbExe();
            if (exe == null) return null;

            // 1. Kiểm tra trước xem instance đã đang chạy và có pipe sẵn chưa
            string info = RunSqlLocalDb(exe, "info \"" + instance + "\"");
            if (!string.IsNullOrEmpty(info))
            {
                var m = Regex.Match(info, @"np:\\\\\.\\pipe\\[^\s\r\n]+", RegexOptions.IgnoreCase);
                if (m.Success && !string.IsNullOrEmpty(m.Value))
                {
                    string existingPipe = m.Value.Trim();
                    if (PipeExists(existingPipe))
                        return existingPipe;
                }
            }

            // 2. Khởi động instance nếu chưa chạy
            RunSqlLocalDb(exe, "start \"" + instance + "\"");

            // 3. Vòng lặp chờ tối đa 10 giây (20 x 500ms) để SQL Server khởi tạo xong Named Pipe
            for (int i = 0; i < 20; i++)
            {
                info = RunSqlLocalDb(exe, "info \"" + instance + "\"");
                if (!string.IsNullOrEmpty(info))
                {
                    var match = Regex.Match(info, @"np:\\\\\.\\pipe\\[^\s\r\n]+", RegexOptions.IgnoreCase);
                    if (match.Success && !string.IsNullOrEmpty(match.Value))
                    {
                        string p = match.Value.Trim();
                        // Chờ thêm 200ms để SQL Server hoàn tất đăng ký pipe listener
                        System.Threading.Thread.Sleep(200);
                        return p;
                    }
                }
                System.Threading.Thread.Sleep(500);
            }

            return null;
        }

        private static string RunSqlLocalDb(string exe, string args)
        {
            try
            {
                var psi = new ProcessStartInfo(exe, args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(30000);
                    return output;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string FindSqlLocalDbExe()
        {
            // Các đường dẫn tiêu chuẩn phổ biến nhất
            string[] directPaths =
            {
                @"C:\Program Files\Microsoft SQL Server\170\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\150\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\140\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files (x86)\Microsoft SQL Server\170\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files (x86)\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files (x86)\Microsoft SQL Server\150\Tools\Binn\SqlLocalDB.exe"
            };

            foreach (string p in directPaths)
            {
                if (File.Exists(p)) return p;
            }

            string[] roots =
            {
                Environment.GetEnvironmentVariable("ProgramW6432"),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                @"C:\Program Files"
            };

            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;
                string sqlRoot = Path.Combine(root, "Microsoft SQL Server");
                if (!Directory.Exists(sqlRoot)) continue;

                string[] versions = Directory.GetDirectories(sqlRoot);
                Array.Sort(versions);
                Array.Reverse(versions); // ưu tiên phiên bản mới nhất (170, 160, 150...)
                foreach (string v in versions)
                {
                    string candidate = Path.Combine(v, @"Tools\Binn\SqlLocalDB.exe");
                    if (File.Exists(candidate)) return candidate;
                }
            }

            return null;
        }
    }
}
