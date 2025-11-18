#!/bin/bash
# Скрипт для импорта данных в Docker БД
# Использование: ./import-data.sh [файл.sql]

SQL_FILE=${1:-init-data.sql}

echo "Импорт данных из $SQL_FILE в Docker БД..."

docker-compose exec -T postgres psql -U postgres -d base < "$SQL_FILE"

echo "Данные импортированы!"

