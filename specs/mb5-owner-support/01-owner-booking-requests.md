# VivuCar UI - Owner Booking Requests

## Mục tiêu trang
Người cho thuê xem danh sách các yêu cầu đặt xe được gửi đến từ khách thuê, lọc theo trạng thái và thao tác xác nhận/từ chối nhanh.

## File HTML gợi ý
`owner-booking-requests.html`

## Layout
- Owner header.
- Owner sidebar.
- Content:
  - Page title.
  - Summary cards.
  - Filter bar.
  - Booking request table/card list.
  - Pagination.

## Sidebar owner
Menu:
- Dashboard
- Yêu cầu đặt xe
- Bàn giao & trả xe
- Xe của tôi
- Hỗ trợ khách hàng
- Hồ sơ
- Đăng xuất

Item active:
- `Yêu cầu đặt xe`

## Header trang
- Title: `Yêu cầu đặt xe`
- Subtitle: `Xem và xử lý các yêu cầu thuê xe từ khách hàng.`

## Summary cards
- `Tất cả yêu cầu`
  - id: `totalRequests`
- `Chờ xác nhận`
  - id: `pendingRequests`
- `Đã xác nhận`
  - id: `acceptedRequests`
- `Đã từ chối`
  - id: `declinedRequests`
- `Sắp nhận xe`
  - id: `upcomingPickups`

## Filter bar
Fields:
- Search:
  - id: `bookingRequestSearch`
  - placeholder: `Tìm theo mã đơn, tên khách, tên xe`
- Status:
  - id: `requestStatusFilter`
  - options:
    - `Tất cả trạng thái`
    - `PENDING_OWNER_CONFIRMATION`
    - `ACCEPTED`
    - `DECLINED`
    - `CANCELLED`
- Car:
  - id: `carFilter`
  - options:
    - `Tất cả xe`
    - Danh sách xe của owner
- Date from:
  - id: `pickupDateFrom`
  - type: `date`
- Date to:
  - id: `pickupDateTo`
  - type: `date`
- Sort:
  - id: `requestSort`
  - options:
    - `Mới nhất`
    - `Cũ nhất`
    - `Ngày nhận xe gần nhất`
    - `Giá trị đơn cao nhất`
- Button:
  - id: `btnFilterRequests`
  - text: `Lọc`
- Button:
  - id: `btnResetRequestFilter`
  - text: `Đặt lại`

## Bảng yêu cầu
Columns:
- Mã đơn.
- Khách thuê.
- Xe.
- Thời gian nhận xe.
- Thời gian trả xe.
- Tổng tiền.
- Tiền cọc.
- Trạng thái.
- Hành động.

## Card mobile
Trên mobile hiển thị dạng card:
- Mã đơn.
- Tên khách.
- Xe.
- Thời gian.
- Trạng thái.
- Buttons.

## Badge trạng thái
- PENDING_OWNER_CONFIRMATION: vàng.
- ACCEPTED: xanh.
- DECLINED: đỏ.
- CANCELLED: xám.

## Hành động từng dòng
- `Xem chi tiết`
  - class: `btn-view-request`
- `Xác nhận`
  - class: `btn-accept-request`
  - chỉ hiện khi status `PENDING_OWNER_CONFIRMATION`
- `Từ chối`
  - class: `btn-decline-request`
  - chỉ hiện khi status `PENDING_OWNER_CONFIRMATION`
- `Chat với khách`
  - class: `btn-chat-customer`

## Pagination
- id: `requestPagination`
- Select page size:
  - 10
  - 20
  - 50
- Button:
  - `Trước`
  - số trang
  - `Sau`

## Empty state
- Text: `Chưa có yêu cầu đặt xe nào`
- Subtext: `Các yêu cầu mới từ khách thuê sẽ hiển thị tại đây.`

## JS state
```js
const bookingRequestState = {
  keyword: "",
  status: "ALL",
  carId: "",
  pickupDateFrom: "",
  pickupDateTo: "",
  sort: "NEWEST",
  page: 1,
  pageSize: 10
};
```

## JS cần có
- Render summary cards.
- Render list booking requests.
- Filter, sort, pagination.
- Click view detail:
  - `owner-booking-request-detail.html?bookingId=B001`
- Click accept/decline mở modal xác nhận.
- Click chat:
  - `owner-support-inbox.html?customerId=U001&bookingId=B001`

## API gợi ý
```txt
GET /api/owner/booking-requests?page=1&pageSize=10&status=PENDING_OWNER_CONFIRMATION
```
