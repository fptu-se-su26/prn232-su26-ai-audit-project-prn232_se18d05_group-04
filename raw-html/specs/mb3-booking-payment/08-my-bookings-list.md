# VivuCar UI - My Bookings Tracking

## Mục tiêu trang
Khách hàng xem danh sách các đơn đặt xe, lọc theo trạng thái và theo dõi tiến trình thuê xe.

## File HTML gợi ý
`my-bookings.html`

## Layout
- Header user.
- Sidebar profile hoặc tab user account.
- Content danh sách đơn.
- Filter bar.
- Booking cards/table.
- Pagination.

## Header trang
- Title: `Đơn thuê của tôi`
- Subtitle: `Theo dõi các đơn xe đã đặt trên VivuCar.`

## Status tabs
Container:
- id: `bookingStatusTabs`

Tabs:
- `Tất cả`
  - value: `ALL`
- `Chờ thanh toán`
  - value: `PENDING_PAYMENT`
- `Chờ nhận xe`
  - value: `DEPOSIT_PAID`
- `Đang thuê`
  - value: `IN_PROGRESS`
- `Đã hoàn tất`
  - value: `COMPLETED`
- `Đã hủy`
  - value: `CANCELLED`

## Filter bar
Fields:
- Search:
  - id: `searchBookingInput`
  - placeholder: `Tìm theo mã đơn, tên xe, biển số`
- Date from:
  - id: `bookingDateFrom`
  - type: `date`
- Date to:
  - id: `bookingDateTo`
  - type: `date`
- Sort:
  - id: `bookingSort`
  - options:
    - `Mới nhất`
    - `Cũ nhất`
    - `Ngày nhận xe gần nhất`
    - `Giá trị cao nhất`
- Button:
  - id: `btnFilterBookings`
  - text: `Lọc`
- Button:
  - id: `btnResetBookingFilter`
  - text: `Đặt lại`

## Booking card
Mỗi card gồm:
- Ảnh xe.
- Mã đơn.
- Tên xe.
- Biển số rút gọn.
- Ngày giờ nhận xe.
- Ngày giờ trả xe.
- Địa điểm nhận xe.
- Tổng tiền.
- Tiền cọc đã thanh toán.
- Trạng thái.
- Progress step nhỏ:
  - Đặt xe.
  - Đã cọc.
  - Nhận xe.
  - Hoàn tất.
- Buttons:
  - `Xem chi tiết`
    - class: `btn-view-booking`
  - `Thanh toán cọc`
    - class: `btn-pay-deposit`
    - chỉ hiện nếu status `PENDING_PAYMENT`
  - `Hủy đơn`
    - class: `btn-cancel-booking`
    - chỉ hiện nếu còn được hủy
  - `Đánh giá`
    - class: `btn-review-booking`
    - chỉ hiện nếu status `COMPLETED`

## Badge trạng thái
- PENDING_PAYMENT: vàng.
- DEPOSIT_PAID: xanh dương.
- IN_PROGRESS: tím hoặc xanh đậm.
- COMPLETED: xanh.
- CANCELLED: đỏ/xám.

## Empty state
- Text: `Bạn chưa có đơn thuê xe nào`
- Button:
  - text: `Tìm xe ngay`
  - link: `search.html`

## JS state
```js
const bookingListState = {
  status: "ALL",
  keyword: "",
  dateFrom: "",
  dateTo: "",
  sort: "NEWEST",
  page: 1,
  pageSize: 10
};
```

## JS functions
- `fetchMyBookings()`
- `renderBookingCards(bookings)`
- `setActiveBookingStatus(status)`
- `renderBookingProgress(status)`
- `openCancelBookingModal(bookingId)`
- `goToBookingDetail(bookingId)`

## API gợi ý
```txt
GET /api/users/me/bookings?status=DEPOSIT_PAID&page=1&pageSize=10
```
