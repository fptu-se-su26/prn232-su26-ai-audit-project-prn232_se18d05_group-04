# Code First Guide - BusinessObjects

## 1. Tạo migration mới

```powershell
dotnet ef migrations add <MigrationName> --project . --startup-project ..\API --output-dir Migrations
```

## 2. Update database

```powershell
dotnet ef database update --project . --startup-project ..\API
```

## 3. Kiểm tra có thay đổi model chưa tạo migration

```powershell
dotnet ef migrations has-pending-model-changes --project . --startup-project ..\API
```

Nếu kết quả là:

```text
No changes have been made to the model since the last migration.
```

thì model hiện tại đã khớp migration mới nhất.
