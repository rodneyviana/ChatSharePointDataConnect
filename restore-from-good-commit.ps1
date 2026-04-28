# Download and restore README files with proper UTF-8 encoding from GitHub
$ErrorActionPreference = "Stop"

Write-Host "Downloading README files from GitHub with proper encoding..." -ForegroundColor Cyan

$repoPath = "C:\Users\rviana\source\repos\SPGraphConnector"
Set-Location $repoPath

# The commit before "Fixing README encoding" attempt
$goodCommit = "da71aa5"  # "Quick fix of type" - before 3f82c0b which may have had issues

Write-Host "`nRestoring files from commit $goodCommit..." -ForegroundColor Yellow

# Restore each file
git checkout $goodCommit -- README.md 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] README.md restored" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Could not restore README.md" -ForegroundColor Red
}

git checkout $goodCommit -- securitybot/README.md 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] securitybot/README.md restored" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Could not restore securitybot/README.md" -ForegroundColor Red
}

Write-Host "`nChecking file encoding..." -ForegroundColor Cyan

# Read first line of README to check encoding
$firstLine = Get-Content "README.md" -TotalCount 1 -Encoding UTF8
Write-Host "First line: $firstLine"

Write-Host "`nREADME files restored from commit $goodCommit" -ForegroundColor Green
Write-Host "This commit was from July 26, before any encoding changes." -ForegroundColor Green
Write-Host "`nTo commit these changes:" -ForegroundColor Cyan
Write-Host "  git add README.md securitybot/README.md" -ForegroundColor White
Write-Host "  git commit -m 'Restore README files with proper UTF-8 encoding'" -ForegroundColor White
Write-Host "  git push" -ForegroundColor White

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
