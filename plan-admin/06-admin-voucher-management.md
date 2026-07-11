# Phase 06 - Admin Voucher Management

Trang thai: pending sau Phase 05. Razor Pages va API client da co, backend voucher controller/service/repository chua co.

## Persisted fields

`name`, `code`, `discount_type`, `discount_value`, `min_order_amount`, `max_discount`, `quantity`, `expires_at`.

Khong persist `is_active`, `start_date`, `usage_limit_per_user`, `customer_group` khi chua migration.

## Contract

- `GET /api/admin/vouchers`
- `GET /api/admin/vouchers/{id}`
- `POST /api/admin/vouchers`
- `PUT /api/admin/vouchers/{id}`
- `DELETE /api/admin/vouchers/{id}`
- `GET /api/admin/vouchers/{id}/performance`

## Backend

- [ ] DTO, validation, repository, service, controller va DI.
- [ ] Normalize code uppercase; unique code.
- [ ] `discount_type` chi `percentage/fixed`; percentage trong range hop le.
- [ ] `quantity = 0` nghia la het luot; `expires_at = null` nghia la khong het han theo ngay.
- [ ] Delete bi chan neu voucher da co usage.
- [ ] Performance tinh tu voucher usages va bookings, ap dung `max_discount`.
- [ ] Them test CRUD, duplicate code, validation, delete conflict va performance.

## Frontend

- [ ] Dung `Pages/Admin/Vouchers/*` va `wwwroot/js/admin/vouchers/*`.
- [ ] Xoa/disable toggle persisted `is_active` va field usage limit khoi payload.
- [ ] Derive status `active/expired/used_up`; khong luu status gia.
- [ ] List/filter/pagination, form validation, performance drawer va API error states.

## Definition of Done

- CRUD/performance dung database that qua proxy; khong fallback mock trong QA.
- Test/build/browser smoke test pass.
