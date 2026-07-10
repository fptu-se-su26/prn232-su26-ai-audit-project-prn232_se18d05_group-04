# Phase 03 - Admin Car Management BE/FE Integration

## Muc tieu
Hoan thien quan ly xe phia Admin theo vertical slice that: Frontend admin cars -> WebClient proxy -> API admin cars -> service -> repository -> database. Phase nay khong duoc dung mock lam ket qua chinh.

## Nguon can doc truoc khi code
- `AGENTS.md`
- `draw-db.sql`: `cars`, `car_types`, `car_images`, `bookings`, `users`
- `specs/mb1-admin/04-admin-car-form.md`
- `specs/mb1-admin/05-admin-car-list-filter-pagination.md`
- `specs/mb1-admin/06-admin-car-block-unblock.md`
- `plan-admin/00-be-fe-integration-roadmap.md`
- Backend patterns:
  - `VivuCarServer/API/Controllers/AdminUsersController.cs`
  - `VivuCarServer/Services/Interfaces/IAdminUserService.cs`
  - `VivuCarServer/Services/Implementations/AdminUserService.cs`
  - `VivuCarServer/Repositories/Interfaces/IUserRepository.cs`
  - `VivuCarServer/Repositories/Implementations/UserRepository.cs`
- Frontend/proxy:
  - `VivuCarClient/WebClient/Controllers/ApiProxyController.cs`
  - `VivuCarClient/WebClient/appsettings.Development.json`
  - `VivuCarClient/WebClient/wwwroot/js/constants.js`
  - `VivuCarClient/WebClient/wwwroot/js/auth.js`
  - `pages/admin-cars.html`
  - `pages/admin-car-form.html`

## Phan bien thuc te
- Backend hien co chua co `AdminCarsController`, `IAdminCarService`, `ICarRepository` cho admin cars. Vi vay viec dau tien cua phase la BE, khong phai UI.
- FE hien dang co mock `VivuCarDB`. Sau phase nay mock chi duoc la fallback dev, khong la happy path.
- Spec dung camelCase va field ngoai schema (`licensePlate`, `dailyPrice`, `depositAmount`, `fuelConsumption`). Backend/FE payload phai dung field schema hoac DTO map ro rang.
- Schema admin car theo `draw-db.sql` dung `price_per_hours`, khong tu sua thanh `price_per_hour` neu chua migration.
- DB chi co status `available`, `rented`, `maintenance`, `blocked`. Khong tao `reserved`, `unavailable`.

## Contract bat buoc
### Endpoints
| Method | Endpoint | FE call qua proxy | Muc dich |
| --- | --- | --- | --- |
| `GET` | `/api/admin/cars` | `/api/proxy/api/admin/cars` | List/filter/pagination. |
| `GET` | `/api/admin/cars/{id}` | `/api/proxy/api/admin/cars/{id}` | Lay detail de edit. |
| `POST` | `/api/admin/cars` | `/api/proxy/api/admin/cars` | Tao xe admin-level voi `owner_id`. |
| `PUT` | `/api/admin/cars/{id}` | `/api/proxy/api/admin/cars/{id}` | Cap nhat xe. |
| `PATCH` | `/api/admin/cars/{id}/block` | `/api/proxy/api/admin/cars/{id}/block` | Khoa xe. |
| `PATCH` | `/api/admin/cars/{id}/unblock` | `/api/proxy/api/admin/cars/{id}/unblock` | Mo khoa xe ve `available`. |
| `POST` | `/api/admin/cars/{id}/images` | `/api/proxy/api/admin/cars/{id}/images` | Upload/add image URL. |
| `DELETE` | `/api/admin/cars/{id}/images/{imageId}` | `/api/proxy/api/admin/cars/{id}/images/{imageId}` | Xoa anh. |
| `PATCH` | `/api/admin/cars/{id}/images/{imageId}/primary` | `/api/proxy/api/admin/cars/{id}/images/{imageId}/primary` | Dat anh chinh. |
| `GET` | `/api/car-types` | `/api/proxy/api/car-types` | Select/filter type. |
| `GET` | `/api/admin/users?role=car_owner` | `/api/proxy/api/admin/users?role=car_owner` | Select owner. |

### Query `GET /api/admin/cars`
```txt
page=1&pageSize=10&keyword=&status=&type_id=&fuel_type=&transmission=
```

### Response `GET /api/admin/cars`
```json
{
  "items": [
    {
      "id": 1,
      "owner_id": 2,
      "owner_name": "Chu xe A",
      "brand": "Toyota",
      "model": "Vios",
      "type_id": 1,
      "type_name": "Sedan",
      "license_plate": "43A-12345",
      "year": 2022,
      "color": "Trang",
      "seats": 5,
      "kilometers_driven": 28000,
      "transmission": "automatic",
      "fuel_type": "gasoline",
      "price_per_day": 650000,
      "price_per_hours": 90000,
      "address": "Hai Chau, Da Nang",
      "description": "Xe sach.",
      "status": "available",
      "blocked_reason": null,
      "created_at": "2026-05-01T08:00:00Z",
      "primary_image_url": "https://..."
    }
  ],
  "totalItems": 1,
  "currentPage": 1,
  "pageSize": 10
}
```

### Request `POST/PUT /api/admin/cars`
```json
{
  "owner_id": 2,
  "brand": "Toyota",
  "model": "Vios",
  "type_id": 1,
  "license_plate": "43A-12345",
  "year": 2022,
  "color": "Trang",
  "seats": 5,
  "kilometers_driven": 28000,
  "transmission": "automatic",
  "fuel_type": "gasoline",
  "price_per_day": 650000,
  "price_per_hours": 90000,
  "address": "Hai Chau, Da Nang",
  "description": "Xe sach.",
  "status": "available",
  "blocked_reason": null
}
```

### Request block/unblock
```json
{ "blocked_reason": "Bao duong dinh ky" }
```

```json
{ "target_status": "available" }
```

### Status code
- `200`: update/list/detail success.
- `201`: create success.
- `204`: delete image success.
- `400`: validation error/enum sai.
- `401/403`: chua login/khong phai admin.
- `404`: car/image/type/owner khong ton tai.
- `409`: bien so trung, hoac xe co booking `pending/approved` nen khong block/unblock.

## Backend tasks
1. DTOs trong `VivuCarServer/Services/Models/Admin`
   - `AdminCarListQuery`
   - `AdminCarListItemResponse`
   - `AdminCarDetailResponse`
   - `AdminCarUpsertRequest`
   - `AdminCarStatusRequest`
   - `AdminCarImageResponse`
   - `PagedResult<T>` neu chua co shared model.

2. Repository
   - Tao `Repositories/Interfaces/ICarRepository.cs` neu chua co.
   - Tao `Repositories/Implementations/CarRepository.cs`.
   - Can methods:
     - `GetAdminCarsAsync(query)`
     - `GetByIdWithAdminDetailsAsync(id)`
     - `ExistsLicensePlateAsync(licensePlate, excludeCarId)`
     - `HasActiveBookingAsync(carId)` voi status `pending/approved`
     - `AddAsync(car)`
     - `UpdateAsync(car)`
     - `AddImageAsync(carImage)`
     - `DeleteImageAsync(carId, imageId)`
     - `SetPrimaryImageAsync(carId, imageId)`

3. Service
   - Tao `Services/Interfaces/IAdminCarService.cs`.
   - Tao `Services/Implementations/AdminCarService.cs`.
   - Business rules:
     - Chi admin duoc thao tac.
     - `owner_id` phai la user role `car_owner` va khong blocked.
     - Validate enums theo DB.
     - Validate number: price > 0, seats > 0, kilometers_driven > 0, year >= 1900 va <= current year.
     - License plate unique.
     - Block/unblock bi chan neu co booking `pending/approved`.
     - Unblock set `status = available`, clear `blocked_reason`.

4. Controller
   - Tao `VivuCarServer/API/Controllers/AdminCarsController.cs`.
   - Prefix `[Route("api/admin/cars")]`.
   - Dung service, khong query DbContext truc tiep trong controller.
   - Tra status code dung contract.

5. DI/config
   - Dang ky repository/service trong config hien co:
     - `RepositoryConfiguration.cs`
     - `ServiceConfiguration.cs`
   - Neu project dang auto-scan service thi verify da nhan class moi.

6. Supporting endpoint
   - Neu chua co `GET /api/car-types`, them controller/service/repository nho de FE select `type_id` that.
   - Neu `GET /api/admin/users?role=car_owner` chua support query role, cap nhat AdminUsers endpoint hoac them endpoint owner-select.

## Frontend tasks
1. `VivuCarClient/WebClient/wwwroot/js/admin-cars.js`
   - Happy path bat buoc goi:
     - `GET ${C.API_BASE_URL}/admin/cars?...`
     - `PATCH ${C.API_BASE_URL}/admin/cars/{id}/block`
     - `PATCH ${C.API_BASE_URL}/admin/cars/{id}/unblock`
   - Render list tu API response, khong tinh pagination tren mock khi API thanh cong.
   - Chi fallback mock khi API fail trong dev, co `console.warn` va UI hint neu can.

2. `VivuCarClient/WebClient/wwwroot/js/admin-car-form.js`
   - Load edit tu `GET /admin/cars/{id}`.
   - Save create/update tu `POST/PUT /admin/cars`.
   - Load owner select tu API admin users/owner endpoint.
   - Load type select tu `/api/car-types`.
   - Upload images qua endpoint image sau khi car saved.
   - Khong gui `depositAmount`, `fuelConsumption`, `pickupLocation`, `reserved`, `unavailable`.

3. HTML
   - Form fields persisted dung `name` DB-aligned:
     - `owner_id`, `license_plate`, `price_per_day`, `price_per_hours`, `kilometers_driven`, ...
   - Status select chi gom `available/rented/maintenance/blocked`.

## Khong lam trong phase nay
- Khong lam owner car management flow.
- Khong them migration cho deposit/fuel consumption.
- Khong lam revenue/export/voucher.
- Khong thay doi schema neu chua co yeu cau rieng.

## Integration test bat buoc
1. Start API:
```powershell
dotnet run --project VivuCarServer/API/API.csproj
```

2. Start WebClient:
```powershell
dotnet run --project VivuCarClient/WebClient/WebClient.csproj
```

3. Browser qua WebClient:
- `/pages/admin-cars.html`
- `/pages/admin-car-form.html`
- `/pages/admin-car-form.html?id=1`

4. DevTools Network phai thay:
- `GET /api/proxy/api/admin/cars` -> 200.
- `GET /api/proxy/api/admin/cars/1` -> 200 neu edit.
- `POST /api/proxy/api/admin/cars` -> 201 khi create.
- `PUT /api/proxy/api/admin/cars/1` -> 200 khi edit.
- `PATCH /api/proxy/api/admin/cars/1/block` -> 200 hoac 409 neu active booking.

5. Build:
```powershell
dotnet build VivuCarServer/API/API.csproj --no-restore
dotnet build VivuCarClient/WebClient/WebClient.csproj --no-restore
node --check VivuCarClient/WebClient/wwwroot/js/admin-cars.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-car-form.js
```

## Definition of Done phase 03
- `AdminCarsController` ton tai va build pass.
- Service/repository admin cars ton tai va duoc DI.
- FE list/form goi API that qua proxy.
- Mock khong con la source chinh.
- Payload FE/BE dung schema `draw-db.sql`.
- Manual integration test pass theo danh sach tren.
