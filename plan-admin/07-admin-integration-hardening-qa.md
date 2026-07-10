# Phase 07 - Admin Integration Hardening va QA

## Muc tieu
Phase 07 khong con la noi "chuan bi noi backend". Sau khi sua plan, cac phase 01-06 phai tu noi BE/FE ngay trong phase cua minh. Phase 07 chi dung de hardening, regression, cleanup, accessibility, responsive, encoding va xac nhan end-to-end lan cuoi.

## Nguon can doc
- `AGENTS.md`
- `draw-db.sql`
- `plan-admin/00-be-fe-integration-roadmap.md`
- Tat ca plan phase 01-06 sau khi da implement.
- `VivuCarClient/WebClient/Controllers/ApiProxyController.cs`
- `VivuCarClient/WebClient/appsettings.Development.json`
- `VivuCarClient/WebClient/wwwroot/js/constants.js`
- Cac controller admin trong `VivuCarServer/API/Controllers`
- Cac page admin trong `pages/`

## Phan bien thuc te
- Neu den Phase 07 ma endpoint van o trang thai `planned`, phase truoc do chua done.
- QA khong duoc chap nhan fallback mock nhu ket qua chinh.
- RAW HTML admin hien con Tailwind CDN/encoding mojibake o mot so file; cleanup co the lam sau khi luong API da on dinh.

## Pham vi phase 07
1. End-to-end verification
   - Chay API server va WebClient cung luc.
   - Test tat ca page admin qua WebClient URL.
   - DevTools Network phai thay request qua `/api/proxy/api/...` cho moi module.
   - Khong co request hard-code thang `localhost:5119` tu JS.

2. API matrix final
| Module | Endpoint | Yeu cau truoc Phase 07 |
| --- | --- | --- |
| Auth | `/api/auth/login`, `/api/auth/me`, `/api/auth/logout`, `/api/auth/refresh` | Existing va FE dang dung. |
| Users | `/api/admin/users`, lock/unlock | Existing va FE dang dung. |
| Cars | `/api/admin/cars`, detail, create, update, block, unblock, images | Phai implemented trong Phase 03. |
| Revenue | `/api/admin/reports/revenue` | Phai implemented trong Phase 04. |
| Export | `/api/admin/export-jobs...` | Phai implemented trong Phase 05. |
| Vouchers | `/api/admin/vouchers...` | Phai implemented trong Phase 06. |

3. Frontend hardening
   - Bo hoac gate mock fallback bang dev flag de QA bat loi API that.
   - Chuan hoa API error handling:
     - 400 validation.
     - 401/403 auth.
     - 404 not found.
     - 409 conflict.
     - 500 server error.
   - Loading/empty/error/success state cho moi page.
   - Khong gui field/enum ngoai DB.

4. UI consistency
   - Notion-style admin, table/form de scan.
   - Modal/button/input co focus visible.
   - Sidebar/menu active dung.
   - Responsive mobile/tablet khong overlap.

5. Accessibility
   - Button la `<button>` neu thao tac JS.
   - Input co label.
   - Modal co close button.
   - Escape dong modal.
   - Status co text label, khong chi mau.

6. Encoding/content cleanup
   - Sua mojibake tieng Viet neu file da duoc touch trong QA.
   - Dam bao file UTF-8.

7. Dependency cleanup
   - Must: khong them dependency UI moi.
   - Should: giam phu thuoc Tailwind CDN neu deadline cho phep.
   - Khong bien Tailwind cleanup thanh blocker neu BE/FE integration chua pass.

## Khong lam trong phase nay
- Khong implement endpoint moi bi bo sot; endpoint bi bo sot phai quay lai phase so huu.
- Khong refactor schema/migration lon.
- Khong them UI framework/chart/export library moi.

## Checklist QA theo page
- `pages/login.html`
  - Login admin thanh cong.
  - Login sai hien loi.
  - Token/session luu dung va logout xoa dung.

- `pages/admin-users.html`
  - Load tu `/api/proxy/api/admin/users`.
  - Lock/unlock qua API that.
  - 403 neu khong phai admin.

- `pages/admin-cars.html`
  - Load tu `/api/proxy/api/admin/cars`.
  - Filter/pagination query tren backend.
  - Block/unblock qua API that.
  - 409 khi xe co booking active hien message dung.

- `pages/admin-car-form.html`
  - Owner select tu API.
  - Type select tu API.
  - Create/update qua API that.
  - Image endpoint hoat dong hoac hien loi ro neu server chua support upload file.

- `pages/admin-dashboard.html`
  - Revenue/report data tu API.
  - Empty state khi khong co snapshot.

- `pages/admin-export-reports.html`
  - Create export job qua API.
  - Poll/list job qua API.
  - Download endpoint dung status `done`.

- `pages/admin-vouchers.html`, `pages/admin-voucher-form.html`
  - CRUD voucher qua API.
  - Payload khong co `is_active`, `start_date`, `usage_limit_per_user` neu DB chua co.

## Build/test bat buoc
```powershell
dotnet build VivuCarServer/API/API.csproj --no-restore
dotnet build VivuCarClient/WebClient/WebClient.csproj --no-restore
node --check VivuCarClient/WebClient/wwwroot/js/admin-users.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-cars.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-car-form.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-dashboard.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-export-reports.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-vouchers.js
node --check VivuCarClient/WebClient/wwwroot/js/admin-voucher-form.js
```

## Definition of Done phase 07
- Tat ca module admin goi API that qua WebClient proxy.
- Khong co endpoint admin nao con chi la `planned` trong feature da claim done.
- Build server/client pass.
- JS syntax checks pass.
- Manual browser QA pass tren desktop va mobile width 375px.
- Bao cao cuoi ghi ro:
  - endpoint nao da test.
  - page nao da test.
  - mock fallback nao con ton tai va cach tat khi QA.
  - known backend limitation neu con.
