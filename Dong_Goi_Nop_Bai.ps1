param (
    [string]$DestinationPath = [System.IO.Path]::Combine([Environment]::GetFolderPath('Desktop'), 'CHAM_Flowers_DoAn_Va_CSDL.zip')
)

$src = 'C:\Users\andrew.ng_yn\.gemini\antigravity\scratch\CHAMFlowers_Framework'

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  CHAM FLOWERS - TỰ ĐỘNG ĐÓNG GÓI ĐỒ ÁN NỘP GOOGLE DRIVE" -ForegroundColor Yellow
Write-Host "========================================================" -ForegroundColor Cyan

# 1. Sao chép file SQL ra thư mục gốc để thầy cô mở xem ngay
$sqlSrc = Join-Path $src "FlowerShop\Database\CHAM_Flowers_SQLServer.sql"
$sqlDest = Join-Path $src "CHAM_Flowers_SQLServer.sql"
if (Test-Path $sqlSrc) {
    Copy-Item $sqlSrc $sqlDest -Force
    Write-Host "[OK] Đã đưa file script CHAM_Flowers_SQLServer.sql ra thư mục gốc." -ForegroundColor Green
}

# 2. Xóa file zip cũ nếu có
if (Test-Path $DestinationPath) {
    Remove-Item $DestinationPath -Force
}

# 3. Tạo thư mục tạm để nén sạch sẽ (bỏ qua .vs)
$tempDir = Join-Path ([System.IO.Path]::GetTempPath()) "CHAM_Upload_Package"
if (Test-Path $tempDir) {
    Remove-Item $tempDir -Recurse -Force
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

Write-Host "[...] Đang gom mã nguồn và thư viện packages..." -ForegroundColor Gray
& robocopy $src $tempDir /E /XD .vs /XF *.zip err_* > $null

Write-Host "[...] Đang nén file zip vào Desktop..." -ForegroundColor Gray
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, $DestinationPath)

# Dọn dẹp temp
Remove-Item $tempDir -Recurse -Force

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  ĐÓNG GÓI THÀNH CÔNG!" -ForegroundColor Green
Write-Host "  File nén đã được tạo tại màn hình Desktop:" -ForegroundColor White
Write-Host "  $DestinationPath" -ForegroundColor Yellow
Write-Host "========================================================" -ForegroundColor Cyan
