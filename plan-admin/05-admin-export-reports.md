# Phase 05 - Admin Export Reports

## Muc tieu
Hoan thien cum xuat bao cao Excel/PDF o muc FE mock/API-ready. Cum nay phai dung `export_jobs` de the hien workflow export, khong nhan backend export da xong neu chua co endpoint that.

## Nguon da doc
- `draw-db.sql`: `export_jobs`, `payments`, `bookings`, `users`, `cars`, `daily_revenue_snapshots`
- `specs/mb1-admin/08-admin-export-excel-pdf.md`
- `pages/admin-export-reports.html`
- `VivuCarClient/WebClient/wwwroot/js/admin-export-reports.js`
- `VivuCarClient/WebClient/wwwroot/js/utils.js`

## Phan bien thuc te
- Spec noi Excel/PDF backend tra file, nhung FE hien tai dang tao Blob mock. Phai ghi ro la frontend mock/export demo.
- Spec dung `TRANSACTIONS/CUSTOMERS/CARS/REVENUE`; code hien tai dung `payments/users/cars/revenue`. Nen thong nhat report type noi bo va label hien thi.
- Filter `ONGOING/COMPLETED/PAID` khong khop DB. Neu can filter thi map ve `bookings.status` va `payments.status`.

## Pham vi lam trong phase
1. Report type
   - Payments/transactions tu `payments`.
   - Users/customers tu `users`.
   - Cars tu `cars`.
   - Revenue tu `daily_revenue_snapshots`.

2. Filters
   - Date from/to.
   - Payment status:
     - `pending`
     - `success`
     - `failed`
     - `refunded`
   - Booking status:
     - `pending`
     - `approved`
     - `rejected`
     - `completed`
     - `cancelled`

3. Preview
   - Render table theo report type.
   - Empty state khi khong co dong nao.
   - Khong dua field ngoai schema vao preview neu khong UI-only.

4. Export job
   - Khi click export:
     - tao row mock trong `export_jobs`.
     - `status = "processing"`.
     - sau delay chuyen `done` hoac `failed`.
   - Field theo schema:
     - `requested_by`
     - `export_type`
     - `params`
     - `status`
     - `file_url`
     - `error_message`
     - `created_at`
     - `completed_at`

5. PDF
   - Neu FE-only: dung print-friendly HTML hoac Blob mock.
   - Khong them third-party PDF lib neu chua duoc phep.

## Khong lam trong phase nay
- Khong tich hop Excel/PDF backend that neu API chua co.
- Khong them thu vien export moi.
- Khong sua payment/refund logic.

## Tieu chi kiem tra
- Preview doi theo report type.
- Export tao job va cap nhat status.
- File mock download duoc.
- `export_jobs.status` chi dung `pending/processing/done/failed`.

---

## Chi tiet bo sung - API endpoints

### Planned export endpoints
| Method | Endpoint | Auth | Muc dich | Query/Body |
| --- | --- | --- | --- | --- |
| `GET` | `/api/admin/reports/preview` | Admin | Preview report data | `type`, `from`, `to`, `paymentStatus`, `bookingStatus`. |
| `POST` | `/api/admin/export-jobs` | Admin | Tao export job | JSON `{ export_type, params }`. |
| `GET` | `/api/admin/export-jobs` | Admin | Lay lich su export jobs | `page`, `pageSize`, `status`. |
| `GET` | `/api/admin/export-jobs/{id}` | Admin | Lay chi tiet job | Path id. |
| `GET` | `/api/admin/export-jobs/{id}/download` | Admin | Tai file da tao | Chi cho `status=done`. |

### FE-only fallback endpoint comments
Neu backend export chua co, JS chi tao Blob mock va van comment endpoint gan function:
```js
// POST /api/admin/export-jobs
// GET /api/admin/export-jobs/:id/download
```

## Chi tiet bo sung - Request contract
```json
{
  "export_type": "payments_excel",
  "params": {
    "type": "payments",
    "from": "2026-05-01",
    "to": "2026-05-31",
    "paymentStatus": "success",
    "bookingStatus": "completed"
  }
}
```

## Chi tiet bo sung - Export job schema
```js
{
  id: 1,
  requested_by: 1,
  export_type: "payments_excel",
  params: {},
  status: "processing",
  file_url: null,
  error_message: null,
  created_at: "2026-07-09T00:00:00Z",
  completed_at: null
}
```

## Chi tiet bo sung - Ham FE se tao/sua

### `VivuCarClient/WebClient/wwwroot/js/admin-export-reports.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `activeType()` | none | string | Lay report type dang chon: `payments/users/cars/revenue`. |
| `getExportFilters()` | none | object | Doc from/to/paymentStatus/bookingStatus. |
| `validateExportFilters(filters)` | object | `{ valid, message }` | Date range hop le. |
| `fetchReportPreview(type, filters)` | string, object | Promise table data | Goi planned API hoac mock `previewRows`. |
| `buildPreviewRows(type, filters)` | string, object | `{ headers, rows }` | Mock-only tao data tu DB. |
| `renderPreviewTable(data)` | table data | void | Render table, empty state. |
| `renderPreviewEmpty(isEmpty)` | boolean | void | Empty state. |
| `createExportJob(format)` | `excel/pdf` | Promise job | Goi API hoac tao mock `export_jobs`. |
| `pollExportJob(jobId)` | id | Promise job | Planned: poll den `done/failed`. |
| `renderExportJobs()` | none | void | Render table job history. |
| `downloadExportJob(job)` | job | Promise<void> | Goi download endpoint hoac Blob mock. |
| `openPdfModal()` | none | void | Mo modal PDF options. |
| `createPdfFromModal()` | none | Promise<void> | Dong modal, tao job pdf. |
| `setExportLoading(action, isLoading)` | string, bool | void | Disable button/export state. |
| `showExportError(message)` | string | void | Error/toast. |

## Chi tiet bo sung - Report type mapping
| UI label | Internal type | Data source |
| --- | --- | --- |
| Giao dich | `payments` | `payments` + `bookings` + `users` |
| Khach hang | `users` | `users` |
| Danh sach xe | `cars` | `cars` + `users` |
| Doanh thu | `revenue` | `daily_revenue_snapshots` |

## Chi tiet bo sung - File thay doi du kien
| File | Loai thay doi |
| --- | --- |
| `pages/admin-export-reports.html` | Them filter enum dung DB, empty/error/loading state. |
| `VivuCarClient/WebClient/wwwroot/js/admin-export-reports.js` | Tach preview/export/job functions, API-ready comments. |

---

## Review bo sung - viec can chot truoc khi implement

Plan 05 dung huong, nhung can ro hon ve file export mock va schema `export_jobs`.

### 1. Export type la string nhung can convention
Schema chi noi `export_type VARCHAR(30)`, khong co enum. Nen chot convention FE:
- `payments_excel`
- `payments_pdf`
- `users_excel`
- `users_pdf`
- `cars_excel`
- `cars_pdf`
- `revenue_excel`
- `revenue_pdf`
Khong dung lung tung `TRANSACTIONS`, `CUSTOMERS` trong payload DB-backed.

### 2. File mock khong phai Excel/PDF that
Neu dung Blob text/plain ma dat duoi `.xls`/`.pdf`, de demo thi duoc nhung de gay hieu nham. Nen:
- CSV export FE-only: tao CSV Blob that voi `.csv`.
- PDF FE-only: dung `window.print()` hoac tao HTML printable.
- Neu van mock `.xls/.pdf`, label UI phai la mock/demo.

Khuyen nghi phase 05: lam CSV frontend thay cho Excel neu chua backend, va PDF print-friendly. Endpoint Excel/PDF de planned.

### 3. `requested_by` phai lay tu current admin
Dung `Auth.getCurrentUser().id`. Neu null, redirect login thay vi tao job voi id rong.

### 4. Download security
`file_url` tu backend sau nay khong nen mo truc tiep neu can auth. Dung endpoint download co bearer token se an toan hon.

### 5. Test cases can them
- Tao export job khi chua login bi chan.
- `status` chi la `pending/processing/done/failed`.
- Filter status booking/payment dung enum DB.
- Preview rong hien empty state.
- CSV/print mock khong claim backend export.

## Ket luan review phase 05
Nen doi wording tu Excel mock sang CSV/print-friendly PDF neu backend chua co. Neu van giu Excel/PDF label, phai ghi ro la export demo.
