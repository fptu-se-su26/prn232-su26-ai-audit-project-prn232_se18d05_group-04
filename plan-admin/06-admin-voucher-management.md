# Phase 06 - Admin Voucher Management

## Muc tieu
Hoan thien cum tao/sua voucher, danh sach voucher va performance drawer dua tren `vouchers` va `voucher_usages`.

## Nguon da doc
- `draw-db.sql`: `vouchers`, `voucher_usages`, `bookings`, `users`
- `specs/mb1-admin/09-admin-voucher-form.md`
- `specs/mb1-admin/10-admin-voucher-list-performance.md`
- `pages/admin-vouchers.html`
- `pages/admin-voucher-form.html`
- `VivuCarClient/WebClient/wwwroot/js/admin-vouchers.js`
- `VivuCarClient/WebClient/wwwroot/js/admin-voucher-form.js`

## Phan bien thuc te
- Spec co `startDate`, `isActive`, `usageLimitPerUser`, `customerGroup`, `description`; schema khong co.
- Schema chi co `expires_at`, khong co start date hoac active flag. Trang thai voucher phai derive tu `expires_at`, `quantity`, `voucher_usages`.
- Spec dung `PERCENT/FIXED`, schema dung `percentage/fixed`.

## Pham vi lam trong phase
1. Voucher form
   - Field persisted:
     - `name`
     - `code`
     - `discount_type`
     - `discount_value`
     - `min_order_amount`
     - `max_discount`
     - `quantity`
     - `expires_at`
   - `discount_type` chi dung:
     - `percentage`
     - `fixed`
   - Uppercase `code`, khong co dau cach.

2. UI-only fields neu giu tren form
   - `is_active`
   - `usage_limit_per_user`
   - `start_date`
   - `customer_group`
   - Bat buoc comment gan logic:
     - `// UI-only field. Not present in current DB schema. Requires migration before backend integration.`

3. Voucher list
   - Search theo `code`, `name`.
   - Filter `discount_type`.
   - Filter date dua tren `expires_at`.
   - Status derive:
     - expired: `expires_at < now`.
     - used_up: usage count >= `quantity`.
     - active: con han va con luot.

4. Performance drawer
   - Usage count tu `voucher_usages`.
   - Booking da dung voucher tu `voucher_usages.booking_id`.
   - Tong doanh thu tu `bookings.total_amount`.
   - Tong tien giam:
     - fixed: `discount_value`.
     - percentage: tinh theo booking amount va cap `max_discount`.
   - Mini chart bang div/canvas JS thuan.

5. Actions
   - Edit voucher.
   - Delete chi khi chua co usage.
   - Enable/disable chi la UI-only neu khong co `is_active` trong schema.

## Khong lam trong phase nay
- Khong them `is_active` vao persisted data.
- Khong them `usage_limit_per_user` neu chua migration.
- Khong dung `PERCENT/FIXED` trong payload DB.

## Tieu chi kiem tra
- Form submit payload khop schema.
- Status voucher derive dung.
- Voucher used count tinh tu `voucher_usages`.
- Delete bi chan khi voucher da co usage.

---

## Chi tiet bo sung - API endpoints

### Planned voucher endpoints
| Method | Endpoint | Auth | Muc dich | Query/Body |
| --- | --- | --- | --- | --- |
| `GET` | `/api/admin/vouchers` | Admin | Danh sach voucher | `page`, `pageSize`, `keyword`, `discountType`, `status`, `from`, `to`. |
| `GET` | `/api/admin/vouchers/{id}` | Admin | Lay chi tiet voucher | Path id. |
| `POST` | `/api/admin/vouchers` | Admin | Tao voucher | DB-aligned voucher body. |
| `PUT` | `/api/admin/vouchers/{id}` | Admin | Cap nhat voucher | DB-aligned voucher body. |
| `DELETE` | `/api/admin/vouchers/{id}` | Admin | Xoa voucher chua co usage | Backend phai chan neu co `voucher_usages`. |
| `GET` | `/api/admin/vouchers/{id}/performance` | Admin | Hieu suat voucher | Usage, revenue, discount total, recent users. |

### Khong nen them neu chua migration
| Endpoint | Ly do |
| --- | --- |
| `PATCH /api/admin/vouchers/{id}/enable` | DB khong co `is_active`. |
| `PATCH /api/admin/vouchers/{id}/disable` | DB khong co `is_active`. |

## Chi tiet bo sung - Request contract

### `POST/PUT /api/admin/vouchers`
```json
{
  "name": "Khuyen mai he 2026",
  "code": "SUMMER2026",
  "discount_type": "percentage",
  "discount_value": 10,
  "min_order_amount": 500000,
  "max_discount": 250000,
  "quantity": 100,
  "expires_at": "2026-06-30T23:59:59Z"
}
```

### `GET /api/admin/vouchers/{id}/performance`
```json
{
  "voucher": {
    "id": 1,
    "code": "SUMMER2026",
    "name": "Khuyen mai he 2026"
  },
  "usageCount": 65,
  "quantity": 100,
  "usageRate": 65,
  "grossRevenue": 82000000,
  "discountTotal": 7200000,
  "dailyUsage": [
    { "date": "2026-06-01", "count": 4 }
  ],
  "recentUsages": [
    {
      "user_id": 3,
      "full_name": "Khach A",
      "booking_id": 10,
      "used_at": "2026-06-01T09:00:00Z",
      "order_amount": 1500000,
      "discount_amount": 150000
    }
  ]
}
```

## Chi tiet bo sung - Ham FE se tao/sua

### `VivuCarClient/WebClient/wwwroot/js/admin-voucher-form.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `getVoucherIdFromUrl()` | none | number/null | Edit mode. |
| `discountType()` | none | `percentage/fixed` | Lay radio checked. |
| `normalizeVoucherCode(value)` | string | string | Uppercase, remove spaces. |
| `collectVoucherFormData()` | none | object | Payload DB-aligned. |
| `validateVoucherForm(data)` | object | `{ valid, message }` | Code, type, value, max, min, quantity, expires. |
| `loadVoucherForEdit(id)` | id | Promise voucher | API/mock load. |
| `fillVoucherForm(voucher)` | voucher | void | Set fields. |
| `updateVoucherPreview()` | none | void | Preview realtime. |
| `submitVoucherForm(event)` | event | Promise<void> | POST/PUT API hoac mock console. |
| `saveVoucherDraft()` | none | void | UI-only draft. |
| `markVoucherUiOnlyFields()` | none | void/comment | Dam bao UI-only fields khong vao payload. |

### `VivuCarClient/WebClient/wwwroot/js/admin-vouchers.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `usageCount(voucherId)` | id | number | Count tu `voucher_usages`. |
| `calculateDiscountAmount(voucher, booking)` | voucher, booking | number | Fixed/percentage cap `max_discount`. |
| `resolveVoucherStatus(voucher)` | voucher | `active/expired/used_up` | Derive tu `expires_at`, `quantity`, usages. |
| `fetchVouchers(params)` | filters | Promise paged result | API/mock. |
| `filteredVouchers()` | none | array | Mock-only filter. |
| `renderVoucherSummary(vouchers)` | array | void | Summary cards. |
| `renderVouchers(vouchers)` | array | void | Table + empty. |
| `renderVoucherRow(voucher)` | voucher | HTML string | Row, usage progress, actions. |
| `applyVoucherFilters()` | none | void | Doc toolbar, render. |
| `resetVoucherFilters()` | none | void | Clear filters. |
| `openPerformanceDrawer(voucherId)` | id | Promise<void> | Fetch/build performance. |
| `buildVoucherPerformance(voucherId)` | id | object | Mock-only join usages/bookings/users. |
| `renderPerformanceDrawer(data)` | object | void | KPI, mini chart, recent users. |
| `renderVoucherUsageChart(dailyUsage)` | array | void | Div/canvas chart thuan. |
| `deleteVoucher(voucherId)` | id | Promise<void> | Chan neu usage > 0. |
| `setVoucherLoading(isLoading)` | boolean | void | Loading state. |

## Chi tiet bo sung - File thay doi du kien
| File | Loai thay doi |
| --- | --- |
| `pages/admin-voucher-form.html` | Dam bao name/id dung schema, UI-only fields co ghi chu trong JS. |
| `VivuCarClient/WebClient/wwwroot/js/admin-voucher-form.js` | Tach validate/collect/preview/submit. |
| `pages/admin-vouchers.html` | Filter enum dung schema; performance drawer accessible. |
| `VivuCarClient/WebClient/wwwroot/js/admin-vouchers.js` | Status derive, performance calculation, no fake persisted active flag. |

---

## Review bo sung - viec can chot truoc khi implement

Plan 06 da bam schema tot, nhung voucher co nhieu field spec khong ton tai nen can rat nghiem.

### 1. Khong implement enable/disable backend
Vi DB khong co `is_active`, phase 06 khong nen co nut `Tam dung/Kich hoat` nhu mot hanh dong persisted. Neu UI van can, chi de disabled/hidden hoac ghi UI-only ro rang.

### 2. `quantity = 0` ambiguity
Schema cho `quantity >= 0`. Can chot y nghia:
- `0` = het luot ngay tu dau, hoac
- `0` = khong gioi han?
AGENTS.md noi quantity dung de tinh remaining, nen nen coi `0` la het luot/khong con. Neu muon unlimited, can migration/field rieng.

### 3. `expires_at = null`
Schema cho nullable. Can chot status:
- `expires_at = null` => khong het han theo ngay, van active neu quantity con.
Khuyen nghi them logic nay vao `resolveVoucherStatus`.

### 4. Discount percentage calculation
Performance khong duoc hardcode `90000` nhu code hien co. Phai tinh:
```js
const raw = booking.total_amount * voucher.discount_value / 100;
const discount = Math.min(raw, voucher.max_discount);
```
Fixed thi:
```js
const discount = Math.min(voucher.discount_value, booking.total_amount);
```

### 5. Date filter
Filter date nen dua tren `expires_at`, khong co `start_date`. Neu UI co tu/ngay den ngay, label nen noi ro la "Han su dung" thay vi "Thoi gian ap dung" neu chua co start date.

### 6. Test cases can them
- `discount_type = percentage/fixed` dung lowercase.
- `expires_at = null` khong crash.
- `quantity = 0` status `used_up`.
- Voucher co usage khong xoa duoc.
- Performance fixed discount khong vuot order amount.
- Percentage discount co cap `max_discount`.

## Ket luan review phase 06
Co the implement, nhung nen bo enable/disable persisted action. Neu giu UI toggle `is_active` trong form thi chi la UI-only va khong gui backend.
