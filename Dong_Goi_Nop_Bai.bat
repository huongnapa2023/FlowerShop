@echo off
chcp 65001 >nul
echo ========================================================
echo   CHAM FLOWERS - TỰ ĐỘNG ĐÓNG GÓI ĐỒ ÁN NỘP GOOGLE DRIVE
echo ========================================================
echo.

cd /d "%~dp0"

echo 1. Đang sao chép file SQL ra thư mục gốc để thầy dễ thấy...
copy /y "FlowerShop\Database\CHAM_Flowers_SQLServer.sql" "CHAM_Flowers_SQLServer.sql" >nul

echo 2. Đang tạo file nén .ZIP sạch sẽ (loại bỏ cache .vs)...
set "ZIP_DEST=%USERPROFILE%\Desktop\CHAM_Flowers_DoAn_Va_CSDL.zip"

powershell -NoProfile -Command ^
    "$src = '%~dp0';" ^
    "$dest = [System.IO.Path]::Combine([Environment]::GetFolderPath('Desktop'), 'CHAM_Flowers_DoAn_Va_CSDL.zip');" ^
    "if (Test-Path $dest) { Remove-Item $dest -Force };" ^
    "$tmp = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), 'CHAM_Package');" ^
    "if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force };" ^
    "New-Item -ItemType Directory -Path $tmp | Out-Null;" ^
    "robocopy $src $tmp /E /XD .vs bin obj /XF *.zip err_* >nul;" ^
    "[System.IO.Compression.ZipFile]::CreateFromDirectory($tmp, $dest);" ^
    "Remove-Item $tmp -Recurse -Force;"

echo.
echo ========================================================
echo   ĐÃ ĐÓNG GÓI THÀNH CÔNG!
echo   File nén đã được lưu ra màn hình Desktop:
echo   CHAM_Flowers_DoAn_Va_CSDL.zip
echo ========================================================
echo.
pause
