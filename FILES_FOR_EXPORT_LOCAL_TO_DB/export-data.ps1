# PowerShell скрипт для экспорта данных с localhost
# Использование: .\export-data.ps1

Write-Host "Экспорт данных из localhost БД..." -ForegroundColor Green

# Параметры подключения к localhost БД
$host = "localhost"
$port = "5432"
$database = "base"
$user = "postgres"
$password = "55455901"

# Экспортируем только данные (без структуры)
$env:PGPASSWORD = $password
pg_dump -h $host -p $port -U $user -d $database `
  --data-only `
  --table=users `
  --column-inserts `
  -f exported-users-data.sql

Write-Host "Данные экспортированы в exported-users-data.sql" -ForegroundColor Green
Write-Host "Теперь можно импортировать их в Docker БД:" -ForegroundColor Yellow
Write-Host "docker-compose exec -T postgres psql -U postgres -d base < exported-users-data.sql" -ForegroundColor Cyan

