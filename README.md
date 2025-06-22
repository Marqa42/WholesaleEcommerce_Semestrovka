# Wholesale E-commerce Platform

Современная платформа для оптовой электронной коммерции, построенная с использованием Clean Architecture и .NET 8.

## Архитектура

Проект использует Clean Architecture с четким разделением на слои:

### Backend (.NET 8)

- **Domain Layer** - Бизнес-логика и сущности
- **Application Layer** - Сервисы и DTOs
- **Infrastructure Layer** - Реализация репозиториев и внешних сервисов
- **API Layer** - ASP.NET Core Web API контроллеры

### Frontend (React + TypeScript)

- **React 18** с TypeScript
- **Vite** для сборки
- **Tailwind CSS** для стилизации
- **React Router** для маршрутизации
- **Zustand** для управления состоянием

## Технологии

### Backend
- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **BCrypt** для хеширования паролей

### Frontend
- **React 18**
- **TypeScript**
- **Vite**
- **Tailwind CSS**
- **React Router DOM**
- **Zustand**
- **Axios**

## Структура проекта

```
├── backend/                          # Backend на C# (.NET 8)
│   ├── src/
│   │   ├── WholesaleEcommerce.API/           # Web API
│   │   ├── WholesaleEcommerce.Application/   # Application Layer
│   │   ├── WholesaleEcommerce.Domain/        # Domain Layer
│   │   └── WholesaleEcommerce.Infrastructure/ # Infrastructure Layer
│   └── WholesaleEcommerce.sln
├── frontend/                         # Frontend на React
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   ├── stores/
│   │   └── types/
│   └── package.json
└── README.md
```

## Установка и запуск

### Предварительные требования

- **.NET 8 SDK**
- **SQL Server** (LocalDB для разработки)
- **Node.js 18+**
- **npm** или **yarn**

### Backend

1. **Клонируйте репозиторий:**
   ```bash
   git clone <repository-url>
   cd Dro[/backend
   ```

2. **Настройте базу данных:**
   - Убедитесь, что SQL Server запущен
   - Обновите строку подключения в `src/WholesaleEcommerce.API/appsettings.Development.json`

3. **Запустите backend:**
   ```bash
   cd src/WholesaleEcommerce.API
   dotnet run
   ```

4. **API будет доступен по адресу:**
   - Swagger UI: `https://localhost:5000`
   - API: `https://localhost:5000/api`

### Frontend

1. **Установите зависимости:**
   ```bash
   cd frontend
   npm install
   ```

2. **Запустите frontend:**
   ```bash
   npm run dev
   ```

3. **Frontend будет доступен по адресу:**
   - `http://localhost:3001`

## API Endpoints

### Аутентификация
- `POST /api/auth/login` - Вход в систему
- `POST /api/auth/register` - Регистрация
- `POST /api/auth/refresh` - Обновление токена
- `POST /api/auth/logout` - Выход из системы
- `GET /api/auth/profile` - Профиль пользователя
- `PUT /api/auth/profile` - Обновление профиля

### Пользователи (Admin)
- `GET /api/users` - Список пользователей
- `GET /api/users/{id}` - Пользователь по ID
- `POST /api/users` - Создание пользователя
- `PUT /api/users/{id}` - Обновление пользователя
- `DELETE /api/users/{id}` - Удаление пользователя
- `GET /api/users/count` - Количество пользователей

### Товары
- `GET /api/products` - Список товаров с фильтрацией
- `GET /api/products/{id}` - Товар по ID
- `GET /api/products/handle/{handle}` - Товар по handle
- `GET /api/products/category/{category}` - Товары по категории
- `GET /api/products/featured` - Рекомендуемые товары
- `GET /api/products/{id}/related` - Похожие товары
- `GET /api/products/count` - Количество товаров

### Товары (Admin)
- `POST /api/products` - Создание товара
- `PUT /api/products/{id}` - Обновление товара
- `DELETE /api/products/{id}` - Удаление товара
- `PUT /api/products/{id}/inventory` - Обновление остатков

## Аутентификация

API использует JWT токены для аутентификации:

1. **Получите токен через `/api/auth/login`**
2. **Добавьте заголовок:** `Authorization: Bearer <token>`
3. **Используйте refresh token для обновления:** `/api/auth/refresh`

## Роли пользователей

- **customer** - Обычный покупатель
- **admin** - Администратор системы

## База данных

### Основные сущности

- **Users** - Пользователи системы
- **Products** - Товары
- **ProductVariants** - Варианты товаров
- **ProductOptions** - Опции товаров
- **Orders** - Заказы
- **OrderItems** - Позиции заказов
- **Addresses** - Адреса доставки

### Миграции

Для создания базы данных:

```bash
cd backend/src/WholesaleEcommerce.API
dotnet ef database update
```

## Разработка

### Структура Clean Architecture

#### Domain Layer
- **Entities** - Бизнес-сущности
- **Repositories** - Интерфейсы репозиториев
- **Value Objects** - Объекты-значения

#### Application Layer
- **Services** - Бизнес-логика
- **DTOs** - Объекты передачи данных
- **Interfaces** - Интерфейсы сервисов

#### Infrastructure Layer
- **Repositories** - Реализация репозиториев
- **Services** - Внешние сервисы (JWT, Email)
- **Data** - Entity Framework контекст

#### API Layer
- **Controllers** - Web API контроллеры
- **Middleware** - Промежуточное ПО
- **Configuration** - Настройки приложения

### Добавление новых функций

1. **Создайте сущность в Domain Layer**
2. **Добавьте интерфейс репозитория**
3. **Создайте DTOs в Application Layer**
4. **Реализуйте сервис**
5. **Создайте репозиторий в Infrastructure Layer**
6. **Добавьте контроллер в API Layer**

## Тестирование

### Backend тесты
```bash
cd backend
dotnet test
```

### Frontend тесты
```bash
cd frontend
npm test
```

## Развертывание

### Backend
```bash
cd backend/src/WholesaleEcommerce.API
dotnet publish -c Release
```

### Frontend
```bash
cd frontend
npm run build
```

## Безопасность

- **JWT токены** для аутентификации
- **BCrypt** для хеширования паролей
- **CORS** настройки
- **Валидация** входных данных
- **Авторизация** на основе ролей

## Лицензия

MIT License

## Поддержка

Для вопросов и предложений создавайте Issues в репозитории. 
