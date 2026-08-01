<#
.SYNOPSIS
    Local "CD" for the EMS stack: pulls latest code (optional), rebuilds
    edu-api/edu-worker images, and brings up the full docker-compose stack.

.PARAMETER Pull
    Run `git pull` on the current branch before building.

.PARAMETER Down
    Tear down the stack (`docker compose down`) instead of deploying.

.EXAMPLE
    ./scripts/deploy-local.ps1
    ./scripts/deploy-local.ps1 -Pull
    ./scripts/deploy-local.ps1 -Down
#>
param(
    [switch]$Pull,
    [switch]$Down
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

if ($Down) {
    Write-Host "Tearing down the stack..." -ForegroundColor Yellow
    docker compose down
    exit $LASTEXITCODE
}

if (-not (Test-Path ".env")) {
    Write-Host ".env not found - copying from .env.example. Review it before relying on these credentials." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env"
}

if ($Pull) {
    Write-Host "Pulling latest changes on $(git rev-parse --abbrev-ref HEAD)..." -ForegroundColor Cyan
    git pull
}

Write-Host "Building edu-api and edu-worker images..." -ForegroundColor Cyan
docker compose build edu-api edu-worker
if ($LASTEXITCODE -ne 0) { throw "docker compose build failed" }

Write-Host "Starting the stack (postgres, redis, rabbitmq, edu-api, edu-worker)..." -ForegroundColor Cyan
docker compose up -d
if ($LASTEXITCODE -ne 0) { throw "docker compose up failed" }

Write-Host "Waiting for edu-api to respond on /health..." -ForegroundColor Cyan
$maxAttempts = 30
for ($i = 1; $i -le $maxAttempts; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:8080/health" -UseBasicParsing -TimeoutSec 3
        if ($response.StatusCode -eq 200) {
            Write-Host "edu-api is healthy." -ForegroundColor Green
            break
        }
    } catch {
        # not ready yet
    }
    if ($i -eq $maxAttempts) {
        Write-Host "edu-api did not become healthy in time. Check logs with: docker compose logs edu-api" -ForegroundColor Red
        exit 1
    }
    Start-Sleep -Seconds 2
}

docker compose ps
Write-Host "Deploy complete. Api: http://localhost:8080  RabbitMQ UI: http://localhost:15672" -ForegroundColor Green
