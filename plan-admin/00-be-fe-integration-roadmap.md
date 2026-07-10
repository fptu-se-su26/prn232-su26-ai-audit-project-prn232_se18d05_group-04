# 00 - BE/FE Integration Roadmap for Admin Module

## Muc tieu moi
Tu bay gio moi phase admin phai giao duoc mot vertical slice chay that tu Frontend -> WebClient proxy -> API -> Service -> Repository -> Database. Khong de tinh trang FE mock xong roi moi noi backend o Phase 07.

## Kien truc ket noi hien tai
- FE static RAW HTML/JS duoc serve qua `VivuCarClient/WebClient`:
  - `/pages/...` -> thu muc `pages/`
  - `/js/...` va `/css/...` -> `VivuCarClient/WebClient/wwwroot/js` va `VivuCarClient/WebClient/wwwroot/css`
- FE goi API bang browser fetch:
  - `window.VivuCarConstants.API_BASE_URL = "/api/proxy/api"`
- WebClient proxy:
  - `VivuCarClient/WebClient/Controllers/ApiProxyController.cs`
  - route `/api/proxy/{**path}` forward den API server.
- API server base URL o WebClient:
  - `VivuCarClient/WebClient/appsettings.Development.json`
  - `ApiSettings:BaseUrl = "http://localhost:5119/"`
- Backend chinh:
  - Controllers: `VivuCarServer/API/Controllers`
  - Services: `VivuCarServer/Services/Interfaces`, `VivuCarServer/Services/Implementations`, `VivuCarServer/Services/Models`
  - Repositories: `VivuCarServer/Repositories/Interfaces`, `VivuCarServer/Repositories/Implementations`
  - EF models/db context: `VivuCarServer/BusinessObjects/Models`, `VivuCarServer/BusinessObjects/Data/VivuCarDbContext.cs`

## Nguyen tac thuc hien moi
1. Moi feature phai chot API contract truoc khi sua UI.
2. Backend endpoint phai ton tai va build pass truoc khi FE bo mock lam source chinh.
3. FE duoc giu mock fallback chi de demo khi API down, nhung DoD cua phase phai test API that.
4. Payload FE phai dung DB/schema/enums; khong gui field UI-only len backend.
5. WebClient proxy la duong ket noi chinh, khong goi thang API host tu browser.
6. Khong claim feature done neu chi co comment endpoint ma chua co controller/service/repository.

## Mau workflow bat buoc cho moi phase
1. Contract
   - Dinh nghia request/response DTO trong `VivuCarServer/Services/Models/Admin/...`.
   - Ghi ro endpoint, query, status code, validation error.

2. Backend
   - Them repository interface + implementation neu can query data moi.
   - Them service interface + implementation chua business rules.
   - Dang ky DI trong config hien co.
   - Them controller trong `VivuCarServer/API/Controllers`.
   - Build `VivuCarServer/API/API.csproj`.

3. Frontend
   - FE goi `${VivuCarConstants.API_BASE_URL}/...`.
   - Dung `Auth.fetchWithAuth` neu endpoint can auth.
   - Adapter FE map response backend ve view model, khong sua schema bang field gia.
   - Mock fallback neu co phai dat sau API call va log warning ro.

4. Integration verify
   - Chay API server.
   - Chay WebClient.
   - Test tren browser qua WebClient URL.
   - Mo DevTools Network de xac nhan request di qua `/api/proxy/api/...` va tra 2xx tu API.
   - Build ca hai project.

## Thu tu phase sau khi sua plan
1. Phase 01 Auth/session
   - Backend auth da co, FE login/logout/admin guard phai goi backend qua proxy/auth proxy.
2. Phase 02 Admin users
   - Backend admin users da co, FE phai dung endpoint that va bo mock lam source chinh.
3. Phase 03 Admin cars
   - Phai implement backend admin cars that truoc, vi hien chua co controller.
4. Phase 04 Revenue dashboard
   - Implement reports endpoint that dua tren bookings/payments/snapshots truoc khi render chart.
5. Phase 05 Export reports
   - Implement export_jobs lifecycle that hoac endpoint mock-backend co persisted status, khong chi Blob frontend.
6. Phase 06 Vouchers
   - Implement vouchers CRUD + performance endpoint that.
7. Phase 07 Hardening/QA
   - Chi lam cleanup/QA sau khi cac endpoint tren da noi that.

## Definition of Done moi cho moi phase
- API contract da ghi trong plan phase.
- Controller/service/repository/DTO da implement neu endpoint chua ton tai.
- FE goi endpoint that qua `/api/proxy/api`.
- Khong con duong chinh doc `VivuCarDB` neu backend endpoint da ton tai.
- Mock fallback, neu con, khong duoc che loi API trong QA; phai co cach tat fallback de test that.
- `dotnet build VivuCarServer/API/API.csproj --no-restore` pass.
- `dotnet build VivuCarClient/WebClient/WebClient.csproj --no-restore` pass.
- Manual browser test co Network request/response ro.

## Quy uoc endpoint admin
Tat ca endpoint admin dung prefix:

```txt
/api/admin/...
```

FE se goi qua WebClient proxy thanh:

```txt
/api/proxy/api/admin/...
```

Vi `API_BASE_URL = "/api/proxy/api"`, code FE nen viet:

```js
Auth.fetchWithAuth(`${C.API_BASE_URL}/admin/cars`)
```

Khong hard-code `http://localhost:5119` trong JS frontend.
