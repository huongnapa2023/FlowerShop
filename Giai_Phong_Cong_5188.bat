@echo off
chcp 65001 >nul
echo ========================================================
echo   CHAM FLOWERS - GIẢI PHÓNG CỔNG 5188 VÀ IIS EXPRESS
echo ========================================================
echo Đang tắt các tiến trình iisexpress.exe đang chạy ngầm...
taskkill /F /IM iisexpress.exe >nul 2>&1
echo.
echo Đang kiểm tra cổng 5188...
netstat -ano | findstr :5188
if %errorlevel% neq 0 (
    echo [THÀNH CÔNG] Cổng 5188 đã hoàn toàn được giải phóng!
) else (
    echo [LƯU Ý] Vẫn còn tiến trình chiếm cổng 5188.
)
echo.
echo Bạn có thể nhấn F5 trong Visual Studio để chạy lại dự án!
pause
