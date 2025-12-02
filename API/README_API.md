# API Documentation

## Архитектура

### Backend (API)
- **Handlers** - Обертки над бизнес-логикой (UsersHandler, RegionsHandler, NotificationsHandler)
  - Вся логика из Menu перенесена сюда
  - Используют существующие сервисы и функции (BD, NotificationService)
  - Простые методы без try/catch
- **Controllers** - HTTP endpoints с обработкой ошибок
  - Вызывают методы из Handlers
  - **try/catch с Ok() и BadRequest(ex.Message)**
  - Логируют ошибки через FileSystem.LogError
- **Services** - Бизнес-логика (NotificationService, JwtService)
- **SearchingInBD** - Функции поиска в БД (BD.SearchByCity, BD.SearchUser)
- **MailManager** - Отправка уведомлений через Telegram
- **Models** - Модели данных (User, BaseContext)
- **DTO** - Data Transfer Objects для API

### Frontend (FRONT)
- **handlers/api.js** - Все HTTP-запросы к API (без логики)
- **components** - React компоненты UI
- **App.js** - Главный компонент с роутингом

### Поток данных
```
Frontend (handlers/api.js) 
  → Backend (Controllers) 
    → Backend (Handlers) 
      → Services/BD/MailManager
```

## Swagger UI
После запуска API, Swagger доступен по адресу: **http://localhost:8080/swagger**

## API Endpoints

### Authentication
- `POST /api/auth/login` - Вход в систему
  ```json
  { "username": "string", "password": "string" }
  ```
- `POST /api/auth/register` - Регистрация нового пользователя
  ```json
  { "name": "string", "username": "string", "password": "string", "role": "string", "age": 0, "cityNow": "string", "cityLater": "string" }
  ```

### Users
- `GET /api/users` - Получить всех пользователей
- `GET /api/users/{id}` - Получить пользователя по ID
- `GET /api/users/city/{city}` - Получить пользователей по городу
- `POST /api/users` - Создать нового пользователя
  ```json
  { "firstName": "string", "username": "string", "city": "string", "age": 0 }
  ```

### Regions
- `GET /api/regions/statistics` - Получить статистику по городам
  ```json
  { "Москва": 10, "Санкт-Петербург": 5 }
  ```

### Notifications
- `POST /api/notifications/user` - Отправить сообщение пользователю
  ```json
  { "userId": 1, "message": "Текст сообщения" }
  ```
- `POST /api/notifications/city` - Отправить сообщение всем пользователям города
  ```json
  { "city": "Москва", "message": "Текст сообщения" }
  ```

## Frontend Handlers

Все запросы к API вынесены в `FRONT/src/handlers/api.js`

### Использование:
```javascript
import * as api from './handlers/api';

// Получить всех пользователей
const users = await api.getAllUsers();

// Получить пользователя по ID
const user = await api.getUserById(1);

// Получить пользователей города
const cityUsers = await api.getUsersByCity('Москва');

// Получить статистику по городам
const stats = await api.getCityStatistics();

// Отправить сообщение пользователю
await api.sendMessageToUser(userId, 'Привет!');

// Отправить сообщение в город
await api.sendMessageToCity('Москва', 'Внимание!');

// Авторизация
const response = await api.login('username', 'password');

// Регистрация
const response = await api.register({ name, username, password, ... });
```

## Запуск

### Backend
```bash
cd API
dotnet run
```
API будет доступен на http://localhost:8080
Swagger UI: http://localhost:8080/swagger

### Frontend
```bash
cd FRONT
npm install
npm start
```
Frontend будет доступен на http://localhost:3000

## CORS
CORS настроен для портов 3000 и 3001 (React dev server)
