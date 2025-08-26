@echo off
setlocal
set "PROJECT=%~dp0"
set "PS1=%~dp0Export-CsToTxtArchive.ps1"

if not exist "%PS1%" (
  echo [ERROR] ไม่พบไฟล์ PowerShell: "%PS1%"
  echo กรุณาวาง Export-CsToTxtArchive.ps1 ไว้ข้างๆ ไฟล์ .bat นี้
  pause
  exit /b 1
)

REM ค่าเริ่มต้น: จำกัดเฉพาะ Scripts\URTS_GPT และเพิ่มเวลาในชื่อโฟลเดอร์
REM ถ้าต้องการระบุระบบย่อยเฉพาะ ให้พิมพ์ต่อท้ายตอนรัน เช่น:
REM   ExportToTxt.bat -Systems SelectionSystem,UI
powershell -NoProfile -ExecutionPolicy Bypass -File "%PS1%" -ProjectPath "%PROJECT%" -OnlyUrts -IncludeTimeInFolder %*

if errorlevel 1 (
  echo.
  echo [FAIL] มีข้อผิดพลาดระหว่างส่งออก (.txt) -- ดูข้อความด้านบนหรือไฟล์ error.log ในปลายทาง
  pause
) else (
  echo.
  echo [OK] เสร็จแล้ว! กดปิดหน้าต่างนี้ได้เลย
  pause
)
endlocal