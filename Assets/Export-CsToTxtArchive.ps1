# บังคับเอาต์พุตคอนโซลเป็น UTF-8 (แก้ไทยเพี้ยนใน CMD)
[Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($false)

# ทำให้ ProjectPath เป็นพาธสมบูรณ์และตัดเครื่องหมายคำพูด/สแลชส่วนเกิน
param(
  [string]$ProjectPath = (Get-Location).Path,
  [string]$SourcePath  = $null,
  [string]$OutRoot     = $null,
  [switch]$IncludeTimeInFolder,
  [switch]$OnlyUrts,
  [string[]]$Systems = @()
)

$ProjectPath = [IO.Path]::GetFullPath($ProjectPath.Trim('"'))

param(
  [string]$ProjectPath = (Get-Location).Path,
  [string]$SourcePath  = $null,
  [string]$OutRoot     = $null,
  [switch]$IncludeTimeInFolder
)

# กำหนดค่าเริ่มต้นอัตโนมัติ
if (-not $SourcePath) { $SourcePath = Join-Path $ProjectPath 'Assets\Scripts' }
if (-not (Test-Path $SourcePath)) {
  $alt = Join-Path $ProjectPath 'Scripts'
  if (Test-Path $alt) { $SourcePath = $alt } else { throw "ไม่พบโฟลเดอร์ Scripts: $SourcePath" }
}
if (-not $OutRoot)    { $OutRoot = Join-Path $ProjectPath 'CodeTxtArchive' }

# สร้างโฟลเดอร์ปลายทางตามวัน (และเวลา ถ้าระบุ)
$stampDate = Get-Date -Format 'yyyy-MM-dd'
$stampTime = Get-Date -Format 'HH-mm'
$dayFolder = if ($IncludeTimeInFolder) { "$stampDate`_$stampTime" } else { $stampDate }
$destRoot  = Join-Path $OutRoot $dayFolder
New-Item -ItemType Directory -Force -Path $destRoot | Out-Null

Write-Host "แปลงจาก: $SourcePath" -ForegroundColor Cyan
Write-Host "ปลายทาง: $destRoot"    -ForegroundColor Cyan

# หาไฟล์ .cs ทั้งหมด
$files = Get-ChildItem -Path $SourcePath -Recurse -Include *.cs -File
if ($files.Count -eq 0) { Write-Warning "ไม่พบไฟล์ .cs ใต้ $SourcePath"; return }

$converted = 0
$errors = @()

foreach ($f in $files) {
  try {
    # สร้างเส้นทางปลายทางให้คงโครงสร้างย่อยเดิม
    $rel    = $f.FullName.Substring($SourcePath.Length).TrimStart('\','/')
    $relDir = Split-Path $rel -Parent
    $outDir = if ($relDir) { Join-Path $destRoot $relDir } else { $destRoot }
    New-Item -ItemType Directory -Force -Path $outDir | Out-Null

    # ชื่อไฟล์ .txt ปลายทาง
    $outFile = Join-Path $outDir ( [IO.Path]::GetFileNameWithoutExtension($f.Name) + '.txt' )

    # อ่านโค้ดแบบ Raw เพื่อคงรูปแบบเดิม และเติมส่วนหัว
    $code   = Get-Content -LiteralPath $f.FullName -Raw -ErrorAction Stop
    $header = @(
      "// Exported from: $($f.FullName)",
      "// Export time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
      "// ProjectPath : $ProjectPath",
      "// SourceRoot  : $SourcePath",
      "// NOTE: ใช้งานในแชท (ประหยัด Token) — ใช้ใน Unity ต้องไฟล์ .cs ต้นฉบับ"
    ) -join "`r`n"

    $content = $header + "`r`n`r`n" + $code
    Set-Content -LiteralPath $outFile -Value $content -Encoding UTF8 -NoNewline

    $converted++
  }
  catch {
    $errors += "แปลงไม่ได้: $($f.FullName) — $($_.Exception.Message)"
  }
}

Write-Host "สำเร็จ: แปลง $converted ไฟล์ → $destRoot" -ForegroundColor Green
if ($errors.Count -gt 0) {
  Write-Warning "มีข้อผิดพลาด $($errors.Count) รายการ — บันทึก error.log ที่ปลายทาง"
  $errors | Set-Content -LiteralPath (Join-Path $destRoot 'error.log') -Encoding UTF8
}