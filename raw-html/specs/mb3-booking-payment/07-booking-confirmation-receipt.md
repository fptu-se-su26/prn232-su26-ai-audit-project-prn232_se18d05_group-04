# VivuCar UI - Booking Confirmation & Deposit Receipt

## Mục tiêu trang
Sau khi thanh toán cọc thành công, hiển thị biên lai cọc và thông tin xác nhận đơn. Người dùng có thể tải biên lai hoặc xem chi tiết đơn.

## File HTML gợi ý
`booking-confirmation.html`

## URL gợi ý
```txt
booking-confirmation.html?bookingId=B001
```

## Layout
- Header user.
- Success banner.
- Booking confirmation card.
- Deposit receipt card.
- Next steps card.

## Success banner
- Title: `Đặt xe thành công`
- Message:
  - `Bạn đã thanh toán tiền cọc. Đơn thuê xe đang chờ giao xe.`
- Badge:
  - `Đã cọc - Chờ giao xe`

## Booking info card
Fields:
- Mã đơn.
- Tên xe.
- Biển số rút gọn.
- Ngày giờ nhận xe.
- Ngày giờ trả xe.
- Địa điểm nhận xe.
- Người lái.
- Số điện thoại người lái.
- Nhân viên giao xe nếu có.

## Deposit receipt card
Fields:
- Mã biên lai:
  - id: `receiptCode`
- Mã giao dịch:
  - id: `transactionCode`
- Cổng thanh toán:
  - id: `paymentProvider`
- Số tiền cọc:
  - id: `depositAmount`
- Tổng tiền thuê:
  - id: `totalAmount`
- Số tiền còn lại:
  - id: `remainingAmount`
- Thời gian thanh toán:
  - id: `paidAt`

## Buttons
- `Tải biên lai PDF`
  - id: `btnDownloadReceipt`
- `Xem chi tiết đơn`
  - id: `btnViewBookingDetail`
- `Về trang chủ`
  - id: `btnGoHome`

## Next steps
Hiển thị checklist:
- Kiểm tra email xác nhận.
- Chuẩn bị CCCD và GPLX bản gốc khi nhận xe.
- Thanh toán phần còn lại khi nhận xe.
- Kiểm tra tình trạng xe trước khi ký nhận.

## JS cần có
- Lấy bookingId từ URL.
- Load thông tin đơn và payment.
- Render receipt.
- Download receipt PDF giả lập hoặc gọi API.

## API gợi ý
```txt
GET /api/bookings/{bookingId}/confirmation
GET /api/bookings/{bookingId}/receipt/pdf
```

## Email/SMS note
Hiển thị text:
- `Email xác nhận đã được gửi đến địa chỉ email trong hồ sơ của bạn.`
