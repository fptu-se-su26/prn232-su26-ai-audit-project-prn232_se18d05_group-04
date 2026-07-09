# VivuCar UI - Owner Car Activity History

## Mục tiêu trang
Người cho thuê theo dõi lịch sử hoạt động xe: số lượt thuê, thời gian hoạt động, doanh thu, trạng thái, lịch bảo trì và hiệu suất khai thác.

## File HTML gợi ý
`owner-car-activity-history.html`

## URL gợi ý
```txt
owner-car-activity-history.html?carId=C001
```

## Layout
- Owner header.
- Owner sidebar.
- Car header.
- KPI cards.
- Filter range.
- Activity timeline.
- Rental history table.
- Maintenance history table.

## Car header
Hiển thị:
- Ảnh xe.
- Tên xe.
- Biển số.
- Trạng thái hiện tại.
- Rating.
- Tổng lượt thuê.

## KPI cards
- `Tổng lượt thuê`
  - id: `totalRentals`
- `Tổng doanh thu`
  - id: `totalRevenue`
- `Số ngày hoạt động`
  - id: `activeDays`
- `Tỷ lệ khai thác`
  - id: `utilizationRate`
- `Số lần bảo trì`
  - id: `maintenanceCount`

## Filter range
Fields:
- Select:
  - id: `historyRange`
  - options:
    - `30 ngày gần đây`
    - `3 tháng gần đây`
    - `Năm nay`
    - `Tùy chỉnh`
- Date from:
  - id: `historyDateFrom`
  - type: `date`
- Date to:
  - id: `historyDateTo`
  - type: `date`
- Button:
  - id: `btnApplyHistoryFilter`
  - text: `Áp dụng`

## Activity chart
Dùng HTML/CSS/JS thuần:
- Container:
  - id: `activityChart`
- Hiển thị bar chart:
  - số ngày được thuê theo tuần/tháng.
- Tooltip:
  - thời gian.
  - số đơn.
  - doanh thu.

## Activity timeline
Container:
- id: `carActivityTimeline`

Timeline item:
- Thời gian.
- Loại sự kiện:
  - `Booking Created`
  - `Deposit Paid`
  - `Trip Started`
  - `Return Requested`
  - `Completed`
  - `Maintenance Started`
  - `Status Changed`
- Nội dung.
- Link đơn nếu có.

## Rental history table
Columns:
- Mã đơn.
- Khách hàng.
- Thời gian thuê.
- Số ngày.
- Tổng tiền.
- Trạng thái.
- Đánh giá sau chuyến.
- Hành động:
  - `Xem đơn`

## Maintenance history table
Columns:
- Mã bảo trì.
- Lý do.
- Ngày bắt đầu.
- Ngày kết thúc.
- Ghi chú.
- Trạng thái.

## Export buttons
- `Xuất lịch sử CSV`
  - id: `btnExportHistoryCsv`
- `Xuất báo cáo PDF`
  - id: `btnExportHistoryPdf`

## JS state
```js
const activityHistoryState = {
  carId: null,
  range: "LAST_30_DAYS",
  dateFrom: "",
  dateTo: ""
};
```

## JS cần có
- Lấy carId từ URL.
- Load KPI.
- Render chart bằng div.
- Render timeline.
- Render rental history.
- Render maintenance history.
- Export CSV frontend giả lập bằng Blob.
- Button PDF có thể gọi API hoặc dùng window.print.

## API gợi ý
```txt
GET /api/owner/cars/{carId}/activity?range=LAST_30_DAYS
GET /api/owner/cars/{carId}/rentals
GET /api/owner/cars/{carId}/maintenance-history
GET /api/owner/cars/{carId}/activity/export/pdf
```

## Empty state
Nếu chưa có dữ liệu:
- Text: `Xe chưa có lịch sử hoạt động trong khoảng thời gian này.`
