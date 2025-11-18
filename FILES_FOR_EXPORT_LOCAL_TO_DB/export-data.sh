#!/bin/bash
# Скрипт для экспорта данных с localhost
# Использование: ./export-data.sh

echo "Экспорт данных из localhost БД..."

# Экспортируем только данные (без структуры)
pg_dump -h localhost -U postgres -d base \
  --data-only \
  --table=users \
  --column-inserts \
  > exported-users-data.sql

echo "Данные экспортированы в exported-users-data.sql"
echo "Теперь можно импортировать их в Docker БД:"
echo "docker-compose exec -T postgres psql -U postgres -d base < exported-users-data.sql"

