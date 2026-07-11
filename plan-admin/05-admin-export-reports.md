# Phase 05 - Admin Export Reports

Trang thai: pending sau Phase 04. Trang Razor/JS da co khung, cac endpoint dang goi chua ton tai.

## Contract

- `GET /api/admin/reports/preview`
- `POST /api/admin/export-jobs`
- `GET /api/admin/export-jobs`
- `GET /api/admin/export-jobs/{id}`
- `GET /api/admin/export-jobs/{id}/download`

`export_jobs.status` chi dung `pending/processing/done/failed`.

## Backend

- [ ] Chot report type: payments, users, cars, revenue va format csv/pdf.
- [ ] Implement preview query dung filter DB enum.
- [ ] Implement create/list/detail job; `requested_by` lay tu authenticated admin.
- [ ] Persist params/status/file URL/error/timestamps.
- [ ] Download chi cho job `done` cua admin; validate file path an toan.
- [ ] CSV phai la file CSV that; PDF dung backend generator hoac print-friendly HTML da chot, khong gia duoi file.
- [ ] Them test lifecycle, authorization, invalid filter va failed job.

## Frontend

- [ ] Dung `Pages/Admin/Reports/Export.cshtml` va `wwwroot/js/admin/reports/export.js`.
- [ ] Sua route frontend cho khop contract tren; hien tai code dang goi `/reports/export/{format}`.
- [ ] Preview, create job, poll, history va authenticated download qua proxy.
- [ ] Loading/empty/error va disable action khi job dang xu ly.

## Definition of Done

- Khong con `mock://`, Blob gia Excel/PDF hoac job chi nam trong localStorage.
- Lifecycle persist sau reload; test/build/browser smoke test pass.
