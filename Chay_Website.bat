@echo off
chcp 65001 >nul
echo ========================================================
echo        KHỞI ĐỘNG HỆ THỐNG CHAM FLOWERS (ASP.NET MVC 5)
echo ========================================================
echo.
echo 1. Đang kích hoạt máy chủ cơ sở dữ liệu SQL Server LocalDB...
sqllocaldb start MSSQLLocalDB >nul 2>&1
echo    -> Cơ sở dữ liệu LocalDB sẵn sàng!
echo.
echo 2. Đang khởi chạy máy chủ web IIS Express tại http://localhost:5188/ ...
echo    (Vui lòng giữ cửa sổ này để website hoạt động liên tục)
echo    Bấm phím 'Q' trong cửa sổ này nếu bạn muốn dừng máy chủ.
echo.
start http://localhost:5188/
"C:\Program Files\IIS Express\iisexpress.exe" /config:"%~dp0.vs\FlowerShop\config\applicationhost.config" /site:FlowerShop
