# Phase 04 - Admin Revenue Dashboard

## Muc tieu
Hoan thien dashboard doanh thu cho Admin dua tren `daily_revenue_snapshots`, `bookings`, `payments`, khong tinh toan trang thai tai chinh bang enum gia.

## Nguon da doc
- `draw-db.sql`: `daily_revenue_snapshots`, `bookings`, `payments`, `cars`, `users`
- `specs/mb1-admin/07-admin-revenue-dashboard.md`
- `pages/admin-dashboard.html`
- `VivuCarClient/WebClient/wwwroot/js/admin-dashboard.js`
- `VivuCarClient/WebClient/wwwroot/js/db-mock.js`

## Phan bien thuc te
- Spec dung status thanh toan `PAID/PENDING/REFUNDED`; schema dung `success/pending/failed/refunded`.
- Page hien co co `canvas` va fallback bar chart. JS co check `window.Chart`, nhung page khong nap Chart.js. Theo rule khong them lib, nen dung div/canvas tu viet bang JS thuan, hoac giu fallback.
- Doanh thu nen uu tien `daily_revenue_snapshots.net_revenue`, khong tu suy dien tuy tien tu booking neu snapshot co san.

## Pham vi lam trong phase
1. KPI
   - Tong doanh thu: sum `net_revenue`.
   - Don hoan tat: sum `completed_bookings`.
   - Gia tri don trung binh: `net_revenue / completed_bookings`.
   - Ty le huy: `cancelled_bookings / total_bookings`.

2. Filter thoi gian
   - Today.
   - 7 ngay gan day.
   - Thang nay.
   - Nam nay.
   - Custom date range.
   - Khi dung mock, filter tren `snapshot_date`.

3. Chart
   - Dung HTML/CSS/JS thuan.
   - Bar chart co label ngay, tooltip doanh thu va so don.
   - Khong phu thuoc Chart.js neu khong duoc phep them lib.

4. Recent orders
   - Join `bookings` voi:
     - `users.full_name`
     - `cars.brand/model`
     - `payments.status`
   - Payment label map:
     - `success` -> thanh cong
     - `pending` -> cho thanh toan
     - `failed` -> that bai
     - `refunded` -> da hoan tien

5. UI states
   - Loading state.
   - Empty state neu khong co snapshot.
   - Error state neu fetch fail.
   - Responsive KPI/chart/table.

## Khong lam trong phase nay
- Khong tao backend revenue API moi.
- Khong sua logic payment/refund cua Member 3.
- Khong dung fake status `PAID` lam persisted value.

## Tieu chi kiem tra
- Dashboard render du lieu tu mock DB.
- Doi filter thay doi KPI va chart.
- Khong crash khi khong co snapshot.
- Payment badge dung enum schema.

---

## Chi tiet bo sung - API endpoints

### Planned admin reporting endpoints
| Method | Endpoint | Auth | Muc dich | Query |
| --- | --- | --- | --- | --- |
| `GET` | `/api/admin/reports/revenue` | Admin | Lay KPI/chart/recent orders | `range=today/7days/month/year/custom`, `from`, `to`. |
| `GET` | `/api/admin/reports/revenue/snapshots` | Admin | Lay raw `daily_revenue_snapshots` | `from`, `to`. |
| `GET` | `/api/admin/bookings/recent` | Admin | Lay booking gan day cho dashboard | `limit=5`. |

### Response contract
```json
{
  "totalRevenue": 125000000,
  "completedOrders": 182,
  "averageOrderValue": 686813,
  "cancelRate": 4.2,
  "chartData": [
    {
      "snapshot_date": "2026-05-01",
      "total_bookings": 12,
      "completed_bookings": 8,
      "cancelled_bookings": 1,
      "gross_revenue": 5600000,
      "net_revenue": 5200000,
      "deposit_collected": 1800000
    }
  ],
  "recentOrders": [
    {
      "id": 12,
      "user": { "id": 3, "full_name": "Khach A" },
      "car": { "id": 5, "brand": "Toyota", "model": "Vios" },
      "pickup_datetime": "2026-05-10T08:00:00Z",
      "total_amount": 1600000,
      "booking_status": "completed",
      "payment_status": "success"
    }
  ]
}
```

## Chi tiet bo sung - Ham FE se tao/sua

### `VivuCarClient/WebClient/wwwroot/js/admin-dashboard.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `getRevenueFilters()` | none | object | Doc `revenueRange`, `dateFrom`, `dateTo`. |
| `validateRevenueFilters(filters)` | object | `{ valid, message }` | Custom range can from/to va from <= to. |
| `fetchDashboardData(filters)` | object | Promise dashboard data | Goi planned API hoac tinh tu mock snapshots. |
| `filterSnapshotsByRange(snapshots, filters)` | array, filters | array | Mock-only filter date. |
| `buildDashboardDataFromMock(snapshots)` | snapshots | dashboard data | Tinh KPI/recent orders. |
| `renderKpiCards(data)` | dashboard data | void | Set KPI text. |
| `renderRevenueChart(chartData)` | snapshots | void | Render div/canvas chart JS thuan. |
| `renderBarChart(container, items)` | element, data | void | Fallback chart khong dung Chart.js. |
| `renderRecentOrders(orders)` | array | void | Table recent orders. |
| `renderDashboardEmpty(isEmpty)` | boolean | void | Empty state. |
| `setDashboardLoading(isLoading)` | boolean | void | Skeleton/loading. |
| `showDashboardError(message)` | string | void | Error state/toast. |
| `applyRevenueFilter()` | none | Promise<void> | Validate, fetch, render. |

## Chi tiet bo sung - Cong thuc tinh
```js
totalRevenue = sum(snapshot.net_revenue)
completedOrders = sum(snapshot.completed_bookings)
cancelRate = sum(cancelled_bookings) / sum(total_bookings) * 100
averageOrderValue = completedOrders ? totalRevenue / completedOrders : 0
```

## Chi tiet bo sung - File thay doi du kien
| File | Loai thay doi |
| --- | --- |
| `pages/admin-dashboard.html` | Loading/error placeholders neu thieu. |
| `VivuCarClient/WebClient/wwwroot/js/admin-dashboard.js` | Tach data/filter/render functions, bo phu thuoc Chart.js. |
| `VivuCarClient/WebClient/wwwroot/css/pages.css` | Bar chart style neu chua co. |

---

## Review bo sung - viec can chot truoc khi implement

Plan 04 kha gon, nhung can lam ro nguon du lieu va fallback.

### 1. Snapshot la source uu tien
Neu `daily_revenue_snapshots` co du lieu, dashboard dung snapshot. Khong nen tu tinh revenue tu `payments` tru khi snapshot rong, vi co the lech logic net/gross/deposit.

### 2. Fallback khi snapshot rong
Neu muon demo dep hon khi snapshot rong, co the fallback tinh tu `bookings + payments`, nhung phai ghi ro trong code:
```js
// Mock fallback only. Revenue snapshots are the source of truth for admin reporting.
```
Khuyen nghi: phase 04 chi hien empty state khi snapshot rong, khong tu suy dien.

### 3. Date range va timezone
`current_date` la 2026-07-09, timezone Asia/Saigon. Khi filter `today/month/year`, dung local date helper thong nhat, tranh so sanh string UTC bi lech ngay.

### 4. Recent orders khong phai source KPI
Bang recent orders chi de hien thi. KPI khong tinh tu recent orders.

### 5. Chart dependency
Page dang co canvas va check `window.Chart`, nhung rule project khong them lib. Phase 04 nen bo nhanh dependency check hoac giu fallback div chart lam duong chinh. Khong can Chart.js.

### 6. Test cases can them
- Snapshot rong hien empty state va KPI ve 0.
- Custom date from > to hien loi.
- Filter 7 ngay/thang/nam khong crash voi snapshot ngoai range.
- Payment status badge chi dung `pending/success/failed/refunded`.

## Ket luan review phase 04
Co the implement sau user/car nen tang. Khong can backend moi trong phase nay; mock snapshot la du, mien la khong claim revenue API da co.
