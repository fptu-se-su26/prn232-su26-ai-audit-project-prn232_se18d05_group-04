# VivuCar UI - Booking Cancellation

## Mục tiêu component
Cho phép khách hàng hủy đơn khi còn được phép hủy. Sau khi hủy, đơn chuyển sang `CANCELLED` và lịch xe được giải phóng để khách khác đặt.

## File HTML gợi ý
`booking-cancellation-modal.html`

## Vị trí sử dụng
- `my-bookings.html`
- `booking-detail.html`

## Điều kiện được hủy
Frontend chỉ hiển thị nút hủy nếu:
- Booking status là:
  - `PENDING_PAYMENT`
  - `DEPOSIT_PAID`
- Chưa đến thời gian nhận xe.
- Không phải đơn `IN_PROGRESS`, `COMPLETED`, `CANCELLED`.

## Button mở modal
- class: `btn-cancel-booking`
- text: `Hủy đơn`

## Modal hủy đơn
- id: `cancelBookingModal`

Header:
- Title: `Hủy đơn thuê xe`

Content:
- Warning:
  - `Sau khi hủy, đơn sẽ không thể khôi phục. Xe sẽ được mở lịch cho khách hàng khác.`
- Thông tin đơn:
  - Mã đơn.
  - Tên xe.
  - Thời gian nhận/trả.
  - Tiền cọc đã thanh toán.

## Reason field
Select:
- id: `cancelReason`
- options:
  - `Thay đổi kế hoạch`
  - `Tìm được xe khác phù hợp hơn`
  - `Nhập sai thời gian thuê`
  - `Không còn nhu cầu thuê`
  - `Khác`

Textarea:
- id: `cancelNote`
- placeholder: `Nhập ghi chú thêm nếu có`

## Refund info
Box:
- id: `refundPolicyBox`

Hiển thị:
- Nếu chưa thanh toán cọc:
  - `Bạn chưa thanh toán cọc, đơn sẽ được hủy ngay.`
- Nếu đã thanh toán cọc:
  - `Tiền cọc có thể được hoàn theo chính sách hủy của VivuCar.`
  - `Số tiền dự kiến hoàn: ...`

## Buttons
- `Đóng`
  - id: `btnCloseCancelModal`
- `Xác nhận hủy đơn`
  - id: `btnConfirmCancelBooking`

## Confirm safety
Trước khi submit, checkbox:
- id: `confirmCancelCheckbox`
- label: `Tôi hiểu rằng đơn sẽ bị hủy và không thể khôi phục.`

Nút xác nhận chỉ enable khi checkbox được tick.

## JS flow
1. Click `Hủy đơn`.
2. Load booking cần hủy vào modal.
3. Người dùng chọn lý do.
4. Tick xác nhận.
5. Click xác nhận.
6. Gọi API hủy đơn.
7. Cập nhật UI:
   - status thành `CANCELLED`.
   - ẩn button hủy.
   - show toast.
8. Trong comment code nhắc rõ backend phải giải phóng lịch xe.

## API gợi ý
```txt
POST /api/bookings/{bookingId}/cancel
```

Body:
```json
{
  "reason": "CHANGE_PLAN",
  "note": "Tôi thay đổi lịch trình",
  "confirmed": true
}
```

Response:
```json
{
  "bookingId": "B001",
  "status": "CANCELLED",
  "carAvailabilityReleased": true,
  "refundAmount": 1000000
}
```

## Validate
- Bắt buộc chọn lý do.
- Nếu chọn `Khác`, bắt buộc nhập ghi chú.
- Bắt buộc tick xác nhận.
- Không cho hủy nếu booking không còn hợp lệ.

## Toast
- `Đã hủy đơn thành công`
- `Không thể hủy đơn này`
- `Vui lòng chọn lý do hủy`
