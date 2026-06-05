# Command Guide

## 1. Copy `.env.example` sang `.env`

Mo terminal tai thu muc root:

```powershell
S:\PRN232\prn232-su26-ai-audit-project-prn232_se18d05_group-04\VivuCarServer
```

Copy file mau:

```powershell
Copy-Item .env.example .env
```

Sau do mo `.env` va sua thong tin database theo may cua ban.

Vi du dung SQL Server Express:

```env
DB_SERVER=WINHTUAN\SQLEXPRESS
DB_DATABASE=VivuCarDb
DB_USER=your_db_user
DB_PASSWORD=your_db_password
```
