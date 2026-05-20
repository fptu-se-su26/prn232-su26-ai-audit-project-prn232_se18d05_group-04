# VivuCar UI - Record Car Condition Before Handover

## Mục tiêu trang
Người cho thuê ghi nhận tình trạng xe trước khi bàn giao cho khách: ngoại thất, nội thất, mức nhiên liệu, số km và ảnh minh chứng.

## File HTML gợi ý
`owner-handover-condition.html`

## URL gợi ý
```txt
owner-handover-condition.html?bookingId=B001&type=pickup
```

## Layout
- Owner header.
- Owner sidebar.
- Booking info card.
- Condition form.
- Image upload.
- Customer confirmation section.
- Buttons.

## Booking info card
Hiển thị:
- Mã đơn.
- Khách hàng.
- Số điện thoại.
- Xe.
- Biển số.
- Thời gian bàn giao.
- Địa điểm bàn giao.
- Trạng thái:
  - `READY_FOR_HANDOVER`

## Checklist trước bàn giao
Checkbox:
- id: `checkCleanExterior`
  - label: `Ngoại thất sạch và bình thường`
- id: `checkCleanInterior`
  - label: `Nội thất sạch và đầy đủ`
- id: `checkFuelReady`
  - label: `Mức nhiên liệu đúng thỏa thuận`
- id: `checkDocumentsReady`
  - label: `Giấy tờ/thiết bị đi kèm đầy đủ`
- id: `checkNoWarningLight`
  - label: `Xe không báo lỗi vận hành`

## Condition fields
- Số km lúc bàn giao:
  - id: `handoverOdometer`
  - type: `number`
  - required
- Mức nhiên liệu:
  - id: `handoverFuelLevel`
  - select:
    - `Đầy bình`
    - `3/4 bình`
    - `1/2 bình`
    - `1/4 bình`
- Tình trạng ngoại thất:
  - id: `handoverExteriorStatus`
  - select:
    - `Bình thường`
    - `Có vết trầy sẵn`
    - `Có móp sẵn`
- Tình trạng nội thất:
  - id: `handoverInteriorStatus`
  - select:
    - `Bình thường`
    - `Có ghi chú`
- Ghi chú bàn giao:
  - id: `handoverNote`
  - textarea
  - placeholder: `Ghi chú tình trạng xe trước khi giao`

## Existing damage note
Nếu có vết trầy/hư hỏng có sẵn:
- Toggle:
  - id: `hasExistingDamage`
- Fields:
  - id: `existingDamageDescription`
  - textarea
  - placeholder: `Mô tả vết trầy/hư hỏng có sẵn`

## Upload ảnh minh chứng
Input:
- id: `handoverImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `handoverImagePreviewGrid`

Yêu cầu:
- Tối thiểu 4 ảnh.
- Nên có ảnh 4 góc xe, nội thất, đồng hồ km, mức nhiên liệu.

## Customer confirmation
Fields:
- Checkbox:
  - id: `customerConfirmedCondition`
  - label: `Khách đã kiểm tra và đồng ý tình trạng xe`
- Tên người nhận xe:
  - id: `receiverName`
- Chữ ký xác nhận dạng text/canvas đơn giản:
  - id: `receiverSignature`
  - placeholder: `Nhập tên xác nhận`

## Buttons
- `Lưu nháp`
  - id: `btnSaveHandoverDraft`
- `Xác nhận khách đã nhận xe`
  - id: `btnConfirmPickup`
- `Quay lại`
  - id: `btnBackHandoverDashboard`

## Validate
- Số km bắt buộc.
- Mức nhiên liệu bắt buộc.
- Tối thiểu 4 ảnh.
- Phải tick khách đã xác nhận.
- Receiver name không rỗng.
- Signature không rỗng.

## Trạng thái sau submit
- Booking status chuyển từ `READY_FOR_HANDOVER` sang `IN_PROGRESS`.
- Lưu condition snapshot để đối chiếu khi trả xe.

## API gợi ý
```txt
POST /api/owner/bookings/{bookingId}/handover-condition
POST /api/owner/bookings/{bookingId}/confirm-pickup
```
