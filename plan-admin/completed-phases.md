# Completed Admin Phases

Cap nhat: 2026-07-11.

## Phase 01 - Auth/session

- API login, refresh, logout, logout-all, me va admin-check da co.
- Refresh token rotation va JWT da co test.
- WebClient co auth proxy, access-token refresh va admin authorization policy.
- Frontend admin dung auth service va API client chung.

## Phase 02 - Admin users

- Danh sach, lock/unlock da co controller, service va frontend API client.
- Self-lock duoc chan o backend.
- Mapping trang thai backend sang UI da co.

## Bang chung verify

- Server tests 18/18 pass ngay 2026-07-11.
- API va WebClient build pass.
## Phase 03 - Admin cars

- CRUD, block/unblock, image management, audit log va API client da hoan tat.
- Unit test va build pass.
- Browser smoke test duoc bo qua theo yeu cau ngay 2026-07-11.

## Phase 04 - Revenue dashboard

- Da them `DailyRevenueSnapshots` model, EF configuration va migration.
- Da them admin report repository, service, DTO va `GET /api/admin/reports/revenue`.
- KPI lay tu snapshot; snapshot rong tra so 0, khong suy dien so lieu.
- Recent bookings lay tu booking/payment hien tai va map status ve contract UI.
- Dashboard Razor dung API that, co range filter, chart HTML/CSS, loading, empty, error va retry.
- 4 test reporting moi; tong 22/22 test pass.
- Browser smoke test duoc bo qua theo yeu cau ngay 2026-07-11.
## Phase 05 - Export reports

- Da them `ExportJobs` entity, EF configuration va migration.
- Da them preview cho payments, users, cars va revenue.
- Da them lifecycle create/list/detail/download theo authenticated admin.
- CSV la file CSV UTF-8 that; PDF la file PDF backend that, khong gia duoi file.
- File download duoc doc qua file store co path validation.
- Razor page dung API that, co preview, history, loading, empty, error va download.
- 3 test export moi; tong 25/25 test pass.
- Migration `AddExportJobs` da apply; browser smoke test skipped by request.

## Phase 06 - Admin voucher management

- Voucher model va EF schema da can chinh theo `draw-db.sql`; migration bao toan du lieu cot cu.
- Da them CRUD, filter/pagination, validation, delete conflict va performance API cho admin.
- Usage count duoc tinh tu `BookingVouchers`, khong con `UsedCount` thu cong.
- Razor list/form goi API that, co loading, empty, error, pagination va performance drawer.
- 4 test voucher moi; tong 29/29 test pass.
- Migration `AlignVoucherSchemaAndAdminManagement` da apply; server/client build pass.
- Browser smoke test duoc bo qua theo yeu cau.

## Phase 07 - Admin hardening and QA

- Admin layout da bo GSAP, legacy layout scripts va `VivuCarDB`; Tailwind duoc tai qua CDN theo quyet dinh cua nhom, JavaScript van la module thuan.
- Navigation chi con cac module co backend that: dashboard, users, cars, vouchers va export reports.
- Admin Users API map role ve `user/car_owner/admin` va expose `is_blocked` dung schema.
- Users page da chuyen tu script legacy sang module API; modal co Escape, backdrop, focus va status text.
- Da sua mojibake tren layout, Users va export module; bo warning nullable/async.
- Contract PDF khong con anonymous; service kiem tra quyen booking truoc khi render.
- Server va WebClient build pass 0 warning; 30/30 tests pass; JavaScript syntax pass.
- Browser desktop/mobile va Network smoke test duoc bo qua theo yeu cau.
- Known limitation: moderation/trash prototype van ton tai ngoai navigation va chua co backend; khong duoc tinh la module admin da tich hop.


