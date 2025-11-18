# Инструкция по переносу данных с localhost в Docker

## Вариант 1: Экспорт/Импорт через pg_dump (Рекомендуется)

### Шаг 1: Экспорт данных с localhost

**Windows (PowerShell):**
```powershell
.\export-data.ps1
```

**Linux/Mac:**
```bash
chmod +x export-data.sh
./export-data.sh
```

**Или вручную:**
```bash
pg_dump -h localhost -U postgres -d base --data-only --table=users --column-inserts > exported-users-data.sql
```

### Шаг 2: Импорт данных в Docker БД

**Windows (PowerShell):**
```powershell
Get-Content exported-users-data.sql | docker-compose exec -T postgres psql -U postgres -d base
```

**Linux/Mac:**
```bash
docker-compose exec -T postgres psql -U postgres -d base < exported-users-data.sql
```

## Вариант 2: Ручное создание SQL скрипта

1. Получите данные из localhost БД:
```sql
SELECT * FROM users;
```

2. Создайте файл `init-data.sql` с INSERT запросами:
```sql
INSERT INTO users (name, age, telegram_teg, city_now, city_later, password, role) VALUES
('Имя Фамилия', 25, '1234567890', 'Москва', 'Москва', NULL, 'guest'),
('Другой Пользователь', 30, '0987654321', 'СПб', 'СПб', NULL, 'guest');
```

3. Импортируйте:
```bash
docker-compose exec -T postgres psql -U postgres -d base < init-data.sql
```

## Вариант 3: Использование init.sql (только для новой БД)

Если нужно создать БД с нуля с данными:

1. Создайте `init.sql` с данными
2. Добавьте в `docker-compose.yml`:
```yaml
volumes:
  - postgres_data:/var/lib/postgresql/data
  - ./init.sql:/docker-entrypoint-initdb.d/init.sql
```

3. Удалите volume и пересоздайте:
```bash
docker-compose down -v
docker-compose up
```

**Важно:** init.sql выполняется только при первом создании БД!

