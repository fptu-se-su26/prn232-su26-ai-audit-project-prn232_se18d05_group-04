# Command Guide

## 1. Tao file `.env`

Mo PowerShell tai thu muc `VivuCarServer`, sau do copy file mau:

```powershell
Copy-Item .env.example .env
```

Cap nhat cac gia tri trong `.env`:

```env
DB_SERVER=WINHTUAN\SQLEXPRESS
DB_DATABASE=VivuCarDb
DB_USER=your_db_user
DB_PASSWORD=your_db_password

SEED_ADMIN_EMAIL=admin@vivucar.local
SEED_ADMIN_PASSWORD=change_this_password
SEED_ADMIN_FULL_NAME=System Admin
SEED_ADMIN_PHONE_NUMBER=0900000001

SEED_CUSTOMER_EMAIL=customer01@vivucar.local
SEED_CUSTOMER_PASSWORD=change_this_password
SEED_CUSTOMER_FULL_NAME=Default Customer
SEED_CUSTOMER_PHONE_NUMBER=0900000002

SEED_CAR_OWNER_EMAIL=owner01@vivucar.local
SEED_CAR_OWNER_PASSWORD=change_this_password
SEED_CAR_OWNER_FULL_NAME=Default Car Owner
SEED_CAR_OWNER_PHONE_NUMBER=0900000003

JWT_ISSUER=VivuCarServer
JWT_AUDIENCE=VivuCarClient
JWT_SECRET_KEY=replace_with_at_least_32_random_characters
JWT_ACCESS_TOKEN_MINUTES=15
JWT_REFRESH_TOKEN_DAYS=7

AUTH_COOKIE_SAME_SITE=Lax
AUTH_COOKIE_SECURE=false
AUTH_TOKEN_CLEANUP_HOURS=24
AUTH_REVOKED_TOKEN_RETENTION_DAYS=7
```

Seed password chi duoc dung khi tao tai khoan lan dau va duoc hash truoc
khi luu. Ung dung khong reset password cua tai khoan da ton tai.

Trong production:

- Dung JWT secret ngau nhien, toi thieu 32 bytes.
- Dat `AUTH_COOKIE_SECURE=true`.
- Khong commit file `.env`.

## 2. Tao migration

Chay tai thu muc `VivuCarServer`:

```powershell
dotnet ef migrations add MigrationName `
  --project BusinessObjects `
  --startup-project API `
  --output-dir Migrations
```

## 3. Cap nhat database

```powershell
dotnet ef database update `
  --project BusinessObjects `
  --startup-project API
```

Xem danh sach migration:

```powershell
dotnet ef migrations list `
  --project BusinessObjects `
  --startup-project API
```

Kiem tra model co thay doi nhung chua co migration:

```powershell
dotnet ef migrations has-pending-model-changes `
  --project BusinessObjects `
  --startup-project API
```

## 4. Build va test

```powershell
dotnet build VivuCarServer.sln
dotnet test VivuCarServer.sln
```

## 5. Chay API

```powershell
dotnet run --project API
```

Swagger development:

```text
https://localhost:7005/swagger
```

## 6. Chay WebClient

Mo terminal khac tai thu muc root cua repository:

```powershell
dotnet run --project VivuCarClient\WebClient
```

WebClient mac dinh goi API tai `https://localhost:7005/`.
