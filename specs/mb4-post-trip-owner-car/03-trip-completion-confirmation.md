# VivuCar UI - Trip Completion Confirmation

## Mục tiêu component
Người cho thuê xác nhận hoàn tất chuyến đi sau khi kiểm tra xe không có tranh chấp. Hệ thống cập nhật booking thành `COMPLETED` và xe thành `AVAILABLE`.

## File HTML gợi ý
`trip-completion-confirmation.html`

## Vị trí sử dụng
- Modal trong `owner-return-inspection.html`
- Action trong `owner-booking-detail.html`

## Modal
- id: `completeTripModal`

## Header
Title:
- `Xác nhận hoàn tất chuyến đi`

## Content
Hiển thị:
- Mã đơn.
- Tên khách hàng.
- Xe.
- Biển số.
- Thời gian trả xe thực tế.
- Kết quả kiểm tra:
  - `Không phát sinh tranh chấp`
- Thông báo:
  - `Sau khi xác nhận, đơn sẽ chuyển sang Completed và xe sẽ được mở lại trạng thái Available.`

## Final checklist
Checkbox:
- id: `confirmVehicleReceived`
- label: `Tôi xác nhận đã nhận lại xe`
Checkbox:
- id: `confirmNoDispute`
- label: `Không có tranh chấp hoặc phụ phí chưa xử lý`
Checkbox:
- id: `confirmReleaseCar`
- label: `Cho phép hệ thống mở lại lịch thuê xe`

## Buttons
- `Hủy`
  - id: `btnCancelCompleteTrip`
- `Xác nhận hoàn tất`
  - id: `btnConfirmCompleteTrip`

## Success state
Sau khi hoàn tất:
- Toast:
  - `Đã hoàn tất chuyến đi`
- Badge booking:
  - `Completed`
- Badge car:
  - `Available`

## Validate
- Phải tick đủ 3 checkbox.
- Chỉ cho hoàn tất nếu booking status là `RETURN_PENDING`.
- Không cho hoàn tất nếu còn phụ phí/tranh chấp pending.

## JS flow
```js
function canCompleteTrip(booking) {
  return booking.status === "RETURN_PENDING" && !booking.hasOpenDispute && !booking.hasPendingExtraFee;
}
```

Submit:
```txt
POST /api/owner/bookings/{bookingId}/complete
```

Response:
```json
{
  "bookingId": "B001",
  "bookingStatus": "COMPLETED",
  "carId": "C001",
  "carStatus": "AVAILABLE"
}
```

## Backend note cho Codex comment
- Backend phải cập nhật booking và car availability trong cùng transaction.
- Nếu cập nhật booking thành Completed nhưng xe chưa Available sẽ gây lỗi nghiệp vụ.
