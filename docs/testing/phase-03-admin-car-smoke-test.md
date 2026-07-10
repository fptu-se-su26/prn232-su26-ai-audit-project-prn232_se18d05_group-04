# Phase 03 Admin Car Smoke Test

## Scope

Admin Car Management production-readiness checks for schema persistence, moderation, image upload, pagination, authorization, and audit logging.

## Preconditions

- SQL Server database is available and configured through `VivuCarServer/API/appsettings*.json` or environment variables.
- Backend migration `SyncCarSchemaPhase03` has been reviewed before applying.
- An admin account exists. Do not record real passwords in this file.
- At least one `CarOwner` user exists for the owner dropdown.
- WebClient `ApiSettings:BaseUrl` points to the running API.

## Run Backend

```powershell
dotnet run --project .\VivuCarServer\API\API.csproj
```

## Run Frontend

```powershell
dotnet run --project .\VivuCarClient\WebClient\WebClient.csproj
```

## Apply Migration

```powershell
dotnet ef database update --project .\VivuCarServer\BusinessObjects\BusinessObjects.csproj --startup-project .\VivuCarServer\API\API.csproj
```

## Test Cases

| ID | Case | Expected Result | Actual Result | Status |
| --- | --- | --- | --- | --- |
| 1 | Anonymous requests `GET /api/admin/cars` | HTTP 401 | Not run in this environment | Not run |
| 2 | Non-admin user requests `GET /api/admin/cars` | HTTP 403 | Not run in this environment | Not run |
| 3 | Admin logs in and opens `/admin/cars` | Cars load through `/api/admin/cars` | Not run in this environment | Not run |
| 4 | Search by license plate/brand/model/type/owner | Results update without full page reload | Not run in this environment | Not run |
| 5 | Filter by status/type/fuel/transmission | API query includes selected filters | Not run in this environment | Not run |
| 6 | Previous/Next pagination | Buttons disable at first/last page and page indicator updates | Not run in this environment | Not run |
| 7 | Create car with Year/Color/KilometersDriven/PricePerHour/CarType | Values persist in `Cars` table | Not run in this environment | Not run |
| 8 | Update car fields | Updated values persist after reload | Not run in this environment | Not run |
| 9 | Block car with reason | `Status = Blocked`, `BlockedReason` saved, previous status preserved | Not run in this environment | Not run |
| 10 | Reload blocked car | Blocked reason remains available from API | Not run in this environment | Not run |
| 11 | Unblock car to available | `BlockedReason` cleared and status becomes requested target | Not run in this environment | Not run |
| 12 | Upload valid JPEG/PNG/WebP below 5 MB | File saved in `API/wwwroot/uploads/cars/{carId}`, URL opens directly | Not run in this environment | Not run |
| 13 | Restart API and open uploaded image URL | Image still exists | Not run in this environment | Not run |
| 14 | Upload invalid extension/MIME/signature | HTTP 400 with `{ message, errors, traceId }` | Not run in this environment | Not run |
| 15 | Upload image above 5 MB | HTTP 400 with file size validation error | Not run in this environment | Not run |
| 16 | Create/update/block/unblock/upload/delete image | `AdminAuditLogs` contains matching action row | Not run in this environment | Not run |

## Automated Verification Performed

- `dotnet restore .\VivuCarServer\Tests\VivuCarServer.Tests.csproj` - passed
- `dotnet build .\VivuCarServer\API\API.csproj --no-restore` - passed
- `dotnet build .\VivuCarClient\WebClient\WebClient.csproj --no-restore` - passed
- `dotnet test .\VivuCarServer\Tests\VivuCarServer.Tests.csproj --no-restore` - passed, 18/18
- `node --check` for Admin Cars JavaScript modules - passed
- `dotnet ef migrations script --idempotent --project .\VivuCarServer\BusinessObjects\BusinessObjects.csproj --startup-project .\VivuCarServer\API\API.csproj --output .\docs\testing\phase-03-sync-car-schema.sql` - passed

## Pending Manual Runtime Verification

The full runtime smoke test requires a confirmed SQL Server target database plus API and WebClient processes. `dotnet ef database update` was not executed in this environment because applying a migration mutates the configured database and the target database was not explicitly verified. Review `docs/testing/phase-03-sync-car-schema.sql`, then apply it to the intended database before running the table above. Do not mark Phase 03 fully complete until the table above is executed against the target environment.

## Known Risks

- Applying migration changes persisted schema; review `20260710110153_SyncCarSchemaPhase03.cs` before running against shared databases.
- Existing rows receive default `KilometersDriven = 0` and `PricePerHour = 0`; update old data if business rules require positive hourly price.
- Upload cleanup rolls back files if metadata save fails during upload, but deleting existing image records now also attempts physical file deletion based on stored URL.

