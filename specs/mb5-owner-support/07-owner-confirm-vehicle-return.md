# VivuCar UI - Confirm Vehicle Return

## Mục tiêu component
Người cho thuê xác nhận khách đã trả xe thành công sau khi ghi nhận tình trạng xe. Nếu không có phát sinh, booking chuyển sang `COMPLETED`, xe chuyển về `AVAILABLE`.

## File HTML gợi ý
`owner-confirm-vehicle-return-modal.html`

## Vị trí sử dụng
- `owner-return-condition.html`
- `owner-handover-dashboard.html`

## Modal
- id: `confirmVehicleReturnModal`

## Header
Title:
- `Xác nhận khách đã trả xe`

## Content
Hiển thị:
- Mã đơn.
- Khách hàng.
- Xe.
- Biển số.
- Thời gian trả thực tế.
- Số km đã đi.
- Phụ phí:
  - `Không có`
  - hoặc tổng phụ phí.

## Điều kiện hoàn tất
Checklist:
- id: `confirmReceivedVehicle`
  - label: `Tôi đã nhận lại xe`
- id: `confirmCheckedCondition`
  - label: `Tôi đã kiểm tra tình trạng xe`
- id: `confirmNoPendingIssue`
  - label: `Không còn phụ phí hoặc tranh chấp chưa xử lý`
- id: `confirmReleaseAvailability`
  - label: `Cho phép hệ thống mở lại xe cho khách khác đặt`

## Note
Textarea:
- id: `returnConfirmNote`
- placeholder: `Ghi chú hoàn tất nếu có`

## Buttons
- `Hủy`
  - id: `btnCancelReturnConfirm`
- `Xác nhận hoàn tất`
  - id: `btnConfirmVehicleReturn`

## Trạng thái sau xác nhận
- Booking status: `COMPLETED`
- Car status: `AVAILABLE`
- Tạo event trong activity history.
- Gửi thông báo cho khách:
  - `Chuyến đi đã hoàn tất. Bạn có thể đánh giá xe.`

## Validate
- Tick đủ checklist.
- Không cho confirm nếu:
  - booking không ở trạng thái `RETURN_PENDING`.
  - có phụ phí đang chờ khách xác nhận.
  - có dispute đang mở.
- Nếu có phụ phí đã được xử lý, vẫn cho hoàn tất.

## JS cần có
```js
function canConfirmReturn(booking) {
  return booking.status === "RETURN_PENDING"
    && !booking.hasPendingExtraFee
    && !booking.hasOpenDispute;
}
```

## API gợi ý
```txt
POST /api/owner/bookings/{bookingId}/confirm-return
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
- Backend cần cập nhật booking và car availability trong cùng transaction.
- Đây là bước quan trọng để giải phóng xe cho các booking tiếp theo.
