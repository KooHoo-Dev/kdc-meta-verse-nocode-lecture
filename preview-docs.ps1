# ------------------------------------------------------------------
#  교안을 로컬 HTML 로 빌드하고 브라우저로 열기
#
#    .\preview-docs.ps1
#
#  배포(GitHub Actions)를 기다리지 않고 바로 확인할 때 사용합니다.
# ------------------------------------------------------------------

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# mkdocs 설치 확인
python -m mkdocs --version *> $null
if (-not $?) {
    Write-Host ""
    Write-Host "  mkdocs 가 설치되어 있지 않습니다. 아래 명령으로 먼저 설치해주세요." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "    pip install -r requirements-docs.txt" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

Write-Host "교안을 빌드하는 중..." -ForegroundColor Cyan
python -m mkdocs build -f mkdocs.offline.yml

$entry = Join-Path $PSScriptRoot 'site-offline\index.html'
Write-Host ""
Write-Host "완료: $entry" -ForegroundColor Green
Start-Process $entry
