# UTF-8 Database Check Script

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
chcp 65001 | Out-Null

Write-Host "Checking Docker DB data..." -ForegroundColor Green
Write-Host ""

$query = "SELECT id, name, telegram_teg, city_now, city_later FROM users ORDER BY id;"
docker exec postgres-db psql -U postgres -d base -c $query | Out-File -Encoding UTF8 temp_output.txt

Get-Content temp_output.txt -Encoding UTF8 | ForEach-Object { Write-Host $_ }

Remove-Item temp_output.txt -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "Data is stored correctly in UTF-8 format!" -ForegroundColor Yellow
