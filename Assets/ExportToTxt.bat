:: ExportToTxt.bat — fixed
@echo off
setlocal

:: ใช้ UTF-8 กันข้อความไทยเพี้ยน
chcp 65001 >nul

:: โฟลเดอร์ที่ไฟล์ .bat นี้อยู่
set "HERE=%~dp0"

:: คำนวณรากโปรเจกต์:
:: - ถ้า .bat อยู่ในโฟลเดอร์ชื่อ "Assets" => รากโปรเจกต์ = โฟลเดอร์แม่ของ Assets
:: - ถ้าไม่ใช่ => ใช้โฟลเดอร์ปัจจุบันเป็นรากโปรเจกต์
for %%I in ("%HERE%") do set "CURFOLDER=%%~nxI"
set "PROJECT=%HERE%"
if /I "%CURFOLDER%"=="Assets" (
  for %%J in ("%HERE%..\") do set "PROJECT=%%~fJ"
)

:: ไฟล์ PowerShell ที่จะเรียก (ต้องวางไว้โฟลเดอร์เดียวกับ .bat)
set "PS1=%HERE%Export-CsToTxtArchive.ps1"
if not exist "%PS1%" (
  echo [ERROR] ไม่พบไฟล์ PowerShell: "%PS1%"
  echo กรุณาวาง Export-CsToTxtArchive.ps1 ไว้โฟลเดอร์เดียวกับไฟล์ .bat
  pause & exit /b 1
)

echo [INFO] Project root  : %PROJECT%
echo [INFO] PowerShell PS1: %PS1%
echo.

:: เรียกสคริปต์ด้วย -File (ปลอดภัยกว่ามาก)
powershell -NoLogo -NoProfile -ExecutionPolicy Bypass ^
  -File "%PS1%" -ProjectPath "%PROJECT%" -OnlyUrts -IncludeTimeInFolder %*

if errorlevel 1 (
  echo.
  echo [FAIL] มีข้อผิดพลาดระหว่างส่งออก (.txt) -- ดูข้อความด้านบนหรือ error.log ในปลายทาง
  pause
) else (
  echo.
  echo [OK] เสร็จแล้ว! ปิดหน้าต่างนี้ได้เลย
  pause
)
endlocal