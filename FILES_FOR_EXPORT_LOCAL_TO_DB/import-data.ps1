# PowerShell скрипт для импорта данных в Docker БД
# Использование: .\import-data.ps1 [файл.sql]

param(
    [string]$SqlFile = "init-data.sql"
)

Write-Host "Импорт данных из $SqlFile в Docker БД..." -ForegroundColor Green

Get-Content $SqlFile | docker-compose exec -T postgres psql -U postgres -d base

Write-Host "Данные импортированы!" -ForegroundColor Green

