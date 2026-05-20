# VivuCar UI - Revenue Dashboard

## Mục tiêu trang
Admin xem thống kê doanh thu theo ngày, tuần, tháng, năm từ các đơn hàng đã hoàn tất thanh toán.

## File HTML gợi ý
`admin-dashboard.html`

## Layout
- Sidebar Admin.
- Header.
- Content:
  - Bộ lọc thời gian.
  - KPI cards.
  - Biểu đồ doanh thu.
  - Bảng đơn hàng gần đây.

## Header trang
- Title: `Dashboard doanh thu`
- Subtitle: `Theo dõi hiệu quả kinh doanh của VivuCar`

## Bộ lọc thời gian
Fields:
- Select khoảng thời gian:
  - id: `revenueRange`
  - options:
    - `Hôm nay`
    - `7 ngày gần đây`
    - `Tháng này`
    - `Năm nay`
    - `Tùy chỉnh`
- Date from:
  - id: `dateFrom`
  - type: `date`
  - chỉ hiện khi chọn `Tùy chỉnh`
- Date to:
  - id: `dateTo`
  - type: `date`
  - chỉ hiện khi chọn `Tùy chỉnh`
- Button:
  - id: `btnApplyRevenueFilter`
  - text: `Áp dụng`

## KPI cards
Cards:
- `Tổng doanh thu`
  - id: `totalRevenue`
- `Số đơn hoàn tất`
  - id: `completedOrders`
- `Giá trị đơn trung bình`
  - id: `averageOrderValue`
- `Tỷ lệ hủy đơn`
  - id: `cancelRate`

## Biểu đồ
Dùng HTML/CSS/JS thuần.

### Option đơn giản
Dùng bar chart tự dựng bằng div.

Container:
- id: `revenueChart`

Mỗi cột:
```html
<div class="bar-item">
  <div class="bar" style="height: 60%"></div>
  <span>Mon</span>
</div>
```

### Thành phần chart
- Title: `Doanh thu theo thời gian`
- Y-axis label đơn giản.
- Tooltip khi hover:
  - ngày.
  - doanh thu.
  - số đơn.

## Bảng đơn hàng gần đây
Columns:
- Mã đơn.
- Khách hàng.
- Xe.
- Ngày thuê.
- Tổng tiền.
- Trạng thái thanh toán.

## Status badge
- `PAID`: xanh.
- `PENDING`: vàng.
- `REFUNDED`: xám.

## JS state
```js
const dashboardState = {
  range: "THIS_MONTH",
  dateFrom: null,
  dateTo: null
};
```

## JS functions
- `fetchDashboardData()`
- `renderKpiCards(data)`
- `renderRevenueChart(chartData)`
- `renderRecentOrders(orders)`
- `formatCurrency(value)`

## API gợi ý
```txt
GET /api/admin/reports/revenue?range=THIS_MONTH
```

Response:
```json
{
  "totalRevenue": 125000000,
  "completedOrders": 182,
  "averageOrderValue": 686813,
  "cancelRate": 4.2,
  "chartData": [
    {
      "label": "01/05",
      "revenue": 4500000,
      "orders": 7
    }
  ],
  "recentOrders": []
}
```

## Empty state
Nếu chưa có dữ liệu:
- Text: `Chưa có dữ liệu doanh thu trong khoảng thời gian này.`
