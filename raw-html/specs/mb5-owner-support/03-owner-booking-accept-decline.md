# VivuCar UI - Accept / Decline Booking Request

## Mục tiêu component
Người cho thuê xác nhận hoặc từ chối yêu cầu đặt xe. Component dùng dạng modal trong danh sách và trang chi tiết.

## File HTML gợi ý
`owner-booking-accept-decline-modal.html`

## Vị trí sử dụng
- `owner-booking-requests.html`
- `owner-booking-request-detail.html`

## Modal xác nhận yêu cầu

### Modal
- id: `acceptBookingModal`

### Header
- Title: `Xác nhận yêu cầu đặt xe`

### Content
Hiển thị:
- Mã đơn.
- Tên khách.
- Xe.
- Thời gian nhận/trả.
- Tổng tiền.
- Tiền cọc.

### Checklist
Checkbox bắt buộc:
- id: `confirmCarReady`
  - label: `Tôi xác nhận xe sẵn sàng cho thuê`
- id: `confirmScheduleAvailable`
  - label: `Tôi đã kiểm tra lịch xe không bị trùng`
- id: `confirmHandoverPlan`
  - label: `Tôi sẽ chuẩn bị bàn giao xe đúng thời gian`

### Optional note
Textarea:
- id: `acceptNote`
- placeholder: `Gửi ghi chú cho khách nếu cần`

### Buttons
- `Hủy`
  - id: `btnCancelAccept`
- `Xác nhận đơn`
  - id: `btnConfirmAcceptBooking`

## Modal từ chối yêu cầu

### Modal
- id: `declineBookingModal`

### Header
- Title: `Từ chối yêu cầu đặt xe`

### Reason field
Select:
- id: `declineReason`
- options:
  - `Xe không sẵn sàng`
  - `Trùng lịch sử dụng`
  - `Cần bảo trì xe`
  - `Thông tin khách chưa phù hợp`
  - `Không thể bàn giao theo thời gian yêu cầu`
  - `Khác`

Textarea:
- id: `declineNote`
- placeholder: `Nhập lý do chi tiết để khách hiểu rõ hơn`

### Buttons
- `Hủy`
  - id: `btnCancelDecline`
- `Xác nhận từ chối`
  - id: `btnConfirmDeclineBooking`

## Trạng thái sau thao tác
Accept:
- Booking status: `ACCEPTED`
- Car schedule được giữ.
- Gửi notification cho khách.
- Nếu cần, chuyển booking sang `READY_FOR_HANDOVER`.

Decline:
- Booking status: `DECLINED`
- Giải phóng lịch xe.
- Gửi notification cho khách.
- Nếu đã cọc, backend xử lý hoàn tiền theo chính sách.

## Validate
Accept:
- Phải tick đủ checklist.
- Chỉ accept nếu booking status là `PENDING_OWNER_CONFIRMATION`.

Decline:
- Bắt buộc chọn lý do.
- Nếu chọn `Khác`, bắt buộc nhập ghi chú tối thiểu 10 ký tự.

## JS cần có
```js
let selectedBookingId = null;

function openAcceptBookingModal(bookingId) {
  selectedBookingId = bookingId;
  document.getElementById("acceptBookingModal").classList.add("show");
}

function openDeclineBookingModal(bookingId) {
  selectedBookingId = bookingId;
  document.getElementById("declineBookingModal").classList.add("show");
}
```

## API gợi ý
```txt
POST /api/owner/booking-requests/{bookingId}/accept
POST /api/owner/booking-requests/{bookingId}/decline
```

Accept body:
```json
{
  "note": "Tôi sẽ chuẩn bị xe đúng giờ.",
  "confirmed": true
}
```

Decline body:
```json
{
  "reason": "CAR_NOT_AVAILABLE",
  "note": "Xe cần bảo trì trong thời gian này."
}
```

## Toast
- `Đã xác nhận yêu cầu đặt xe`
- `Đã từ chối yêu cầu đặt xe`
- `Không thể xử lý yêu cầu này`
