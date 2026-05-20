# VivuCar UI - Owner Return Inspection

## Mục tiêu trang
Người cho thuê kiểm tra tình trạng xe sau khi khách gửi yêu cầu trả xe. Ghi nhận ngoại thất, nhiên liệu, số km, hư hỏng, phụ phí và hình ảnh minh chứng.

## File HTML gợi ý
`owner-return-inspection.html`

## URL gợi ý
```txt
owner-return-inspection.html?bookingId=B001
```

## Layout
- Owner header.
- Owner sidebar.
- Content chính gồm:
  - Thông tin đơn trả xe.
  - Checklist kiểm tra.
  - Form tình trạng xe.
  - Upload ảnh minh chứng.
  - Phụ phí phát sinh.
  - Buttons xử lý.

## Sidebar owner
Menu:
- Dashboard
- Xe của tôi
- Đơn thuê
- Yêu cầu trả xe
- Lịch sử hoạt động
- Hồ sơ
- Đăng xuất

Item active:
- `Yêu cầu trả xe`

## Booking info card
Hiển thị:
- Mã đơn.
- Tên khách hàng.
- Số điện thoại khách.
- Xe.
- Biển số.
- Thời gian thuê.
- Thời gian trả thực tế.
- Địa điểm trả xe.
- Trạng thái:
  - `Đang chờ xác nhận trả xe`

## Inspection checklist
Checkbox group:
- id: `checkExterior`
  - label: `Ngoại thất bình thường`
- id: `checkInterior`
  - label: `Nội thất bình thường`
- id: `checkFuel`
  - label: `Nhiên liệu đúng thỏa thuận`
- id: `checkOdometer`
  - label: `Số km hợp lệ`
- id: `checkDocuments`
  - label: `Giấy tờ/thiết bị đầy đủ`
- id: `checkNoDamage`
  - label: `Không có hư hỏng phát sinh`

## Vehicle condition fields
- Số km hiện tại:
  - id: `currentOdometer`
  - type: `number`
  - required
- Mức nhiên liệu:
  - id: `fuelLevel`
  - select
  - options:
    - `Đầy bình`
    - `3/4 bình`
    - `1/2 bình`
    - `1/4 bình`
    - `Gần hết`
- Tình trạng ngoại thất:
  - id: `exteriorStatus`
  - select:
    - `Bình thường`
    - `Có trầy xước nhẹ`
    - `Có móp/méo`
    - `Hư hỏng nặng`
- Tình trạng nội thất:
  - id: `interiorStatus`
  - select:
    - `Sạch sẽ`
    - `Cần vệ sinh`
    - `Có hư hỏng`
- Ghi chú kiểm tra:
  - id: `inspectionNote`
  - textarea
  - placeholder: `Nhập ghi chú kiểm tra thực tế`

## Damage section
Toggle:
- id: `hasDamage`
- label: `Có hư hỏng/phụ phí phát sinh`

Nếu bật, hiển thị:
- Loại hư hỏng:
  - id: `damageType`
  - select:
    - `Trầy xước`
    - `Móp xe`
    - `Nội thất bẩn/hư`
    - `Thiếu nhiên liệu`
    - `Trả xe trễ`
    - `Vượt số km`
    - `Khác`
- Mô tả hư hỏng:
  - id: `damageDescription`
  - textarea
- Chi phí đề xuất:
  - id: `proposedExtraFee`
  - type: `number`

## Upload ảnh minh chứng
Input:
- id: `inspectionImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `inspectionImagePreviewGrid`

Text:
- `Upload ảnh xe sau chuyến đi để đối chiếu. Tối đa 12 ảnh.`

## Buttons
- `Lưu nháp kiểm tra`
  - id: `btnSaveInspectionDraft`
- `Xác nhận không phát sinh`
  - id: `btnConfirmNoIssue`
- `Gửi yêu cầu xử lý phụ phí`
  - id: `btnSubmitExtraFee`
- `Quay lại`
  - id: `btnBackReturnList`

## Validate
- Số km hiện tại bắt buộc và lớn hơn số km lúc giao xe.
- Nếu có hư hỏng, bắt buộc nhập loại hư hỏng, mô tả và ít nhất 1 ảnh.
- Chi phí đề xuất không âm.
- Tối đa 12 ảnh, mỗi ảnh tối đa 5MB.

## JS cần có
- Load booking return request.
- Toggle damage section.
- Preview ảnh.
- Tính số km đã đi:
  - currentOdometer - startOdometer.
- Nếu không phát sinh:
  - chuyển sang modal xác nhận hoàn tất chuyến đi.
- Nếu có phụ phí:
  - submit inspection status `EXTRA_FEE_PENDING`.

## API gợi ý
```txt
GET /api/owner/bookings/{bookingId}/return-inspection
POST /api/owner/bookings/{bookingId}/inspection
```
