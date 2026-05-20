# VivuCar UI - Owner Handover & Return Dashboard

## Mục tiêu trang
Người cho thuê theo dõi các đơn cần bàn giao, đang thuê, chờ trả xe và đã hoàn tất. Đây là dashboard vận hành chính trong vòng đời chuyến đi.

## File HTML gợi ý
`owner-handover-dashboard.html`

## Layout
- Owner header.
- Owner sidebar.
- Page title.
- Status tabs.
- Operation cards/list.
- Calendar mini view.

## Header trang
- Title: `Bàn giao & trả xe`
- Subtitle: `Theo dõi các đơn cần xử lý trong quá trình nhận và trả xe.`

## Summary cards
- `Cần bàn giao hôm nay`
  - id: `todayHandovers`
- `Đang thuê`
  - id: `activeTrips`
- `Chờ xác nhận trả xe`
  - id: `pendingReturns`
- `Hoàn tất hôm nay`
  - id: `completedToday`

## Status tabs
- `Tất cả`
  - value: `ALL`
- `Đã xác nhận`
  - value: `ACCEPTED`
- `Chờ bàn giao`
  - value: `READY_FOR_HANDOVER`
- `Đang thuê`
  - value: `IN_PROGRESS`
- `Chờ xác nhận trả xe`
  - value: `RETURN_PENDING`
- `Hoàn thành`
  - value: `COMPLETED`
- `Đã hủy`
  - value: `CANCELLED`

## Filter bar
Fields:
- Search:
  - id: `handoverSearch`
  - placeholder: `Tìm theo mã đơn, khách, xe`
- Date:
  - id: `operationDate`
  - type: `date`
- Car:
  - id: `operationCarFilter`
- Button:
  - id: `btnFilterOperations`
  - text: `Lọc`

## Operation card
Mỗi card gồm:
- Mã đơn.
- Tên khách.
- Số điện thoại.
- Xe.
- Biển số.
- Thời gian nhận/trả.
- Địa điểm bàn giao/trả.
- Trạng thái.
- Nhãn ưu tiên:
  - `Sắp đến giờ`
  - `Trễ giờ`
  - `Cần xử lý`
- Buttons:
  - `Ghi nhận bàn giao`
    - class: `btn-record-handover`
    - hiện nếu READY_FOR_HANDOVER.
  - `Xác nhận khách đã nhận xe`
    - class: `btn-confirm-pickup`
  - `Ghi nhận trả xe`
    - class: `btn-record-return`
    - hiện nếu RETURN_PENDING.
  - `Xem chi tiết`
    - class: `btn-view-operation-detail`
  - `Chat`
    - class: `btn-chat-customer`

## Calendar mini view
Container:
- id: `handoverCalendar`

Hiển thị theo ngày:
- Số đơn nhận xe.
- Số đơn trả xe.
- Click ngày sẽ filter list.

## JS state
```js
const handoverState = {
  status: "ALL",
  keyword: "",
  date: "",
  carId: "",
  page: 1,
  pageSize: 10
};
```

## JS cần có
- Render summary.
- Filter theo status/date/car.
- Render operation cards.
- Highlight đơn sắp đến giờ trong 2 tiếng.
- Navigation:
  - `owner-handover-condition.html?bookingId=B001&type=pickup`
  - `owner-return-condition.html?bookingId=B001`
  - `owner-support-inbox.html?bookingId=B001`

## API gợi ý
```txt
GET /api/owner/operations/bookings?status=RETURN_PENDING&date=2026-06-01
```
