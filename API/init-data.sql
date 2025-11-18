-- Скрипт для импорта данных в существующую БД
-- Этот скрипт можно использовать для добавления данных в уже существующую БД
-- Выполните: docker-compose exec -T postgres psql -U postgres -d base < init-data.sql

-- Очищаем существующие данные (опционально, раскомментируйте если нужно)
-- TRUNCATE TABLE users RESTART IDENTITY CASCADE;

-- Вставляем данные пользователей с сохранением оригинальных ID
INSERT INTO users (id, name, age, telegram_teg, city_now, city_later, password, role) VALUES
(1, 'Anton Nekhlopochin', 18, '@quasaaarx', 'Voronezsh', 'Voronezsh', '55455901', 'guest'),
(2, 'Anton Nekhlopochin', 18, '@quasaaarx', 'Voronezh', 'Voronezh', NULL, NULL),
(3, 'Anton Nekhlopochin', 19, '@quasaaarx', 'Elets', 'Elets', NULL, NULL),
(4, 'Anton Nekhlopochin', 25, '2108637904', 'Voronezsh', 'Voronezsh', NULL, 'guest'),
(5, 'Vyacheslav', 18, '1331310743', 'Воронеж', 'Воронеж', NULL, 'guest'),
(6, 'Владимир Кузнецов', 18, '1869441303', 'Воронеж', 'Воронеж', NULL, 'guest'),
(7, 'Роман', 95, '2116208057', 'Магадан', 'Магадан', NULL, 'guest')
ON CONFLICT (id) DO UPDATE SET
    name = EXCLUDED.name,
    age = EXCLUDED.age,
    telegram_teg = EXCLUDED.telegram_teg,
    city_now = EXCLUDED.city_now,
    city_later = EXCLUDED.city_later,
    password = EXCLUDED.password,
    role = EXCLUDED.role;

