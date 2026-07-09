# VivuCar UI - Record Car Condition After Return

## Mục tiêu trang
Người cho thuê ghi nhận tình trạng xe sau khi khách trả xe: nội/ngoại thất, nhiên liệu, số km, hư hỏng và phụ phí phát sinh.

## File HTML gợi ý
`owner-return-condition.html`

## URL gợi ý
```txt
owner-return-condition.html?bookingId=B001
```

## Layout
- Owner header.
- Owner sidebar.
- Booking return info.
- Compare before/after section.
- Return condition form.
- Damage and extra fee section.
- Image evidence.
- Action buttons.

## Booking return info
Hiển thị:
- Mã đơn.
- Khách hàng.
- Xe.
- Biển số.
- Thời gian trả dự kiến.
- Thời gian khách gửi yêu cầu trả thực tế.
- Địa điểm trả xe.
- Trạng thái:
  - `RETURN_PENDING`

## Compare before/after
Table:
- Chỉ số.
- Lúc bàn giao.
- Lúc trả xe.
- Chênh lệch.

Rows:
- Số km.
- Mức nhiên liệu.
- Ngoại thất.
- Nội thất.
- Ghi chú.

## Return condition fields
- Số km lúc nhận lại:
  - id: `returnOdometer`
  - type: `number`
- Mức nhiên liệu lúc nhận lại:
  - id: `returnFuelLevel`
  - select:
    - `Đầy bình`
    - `3/4 bình`
    - `1/2 bình`
    - `1/4 bình`
    - `Gần hết`
- Tình trạng ngoại thất:
  - id: `returnExteriorStatus`
  - select:
    - `Bình thường`
    - `Có trầy xước mới`
    - `Có móp/méo mới`
    - `Hư hỏng nặng`
- Tình trạng nội thất:
  - id: `returnInteriorStatus`
  - select:
    - `Bình thường`
    - `Cần vệ sinh`
    - `Có hư hỏng`
- Ghi chú nhận xe:
  - id: `returnConditionNote`
  - textarea

## Extra fee section
Toggle:
- id: `hasExtraFee`
- label: `Có phụ phí phát sinh`

Nếu bật, hiển thị danh sách phụ phí có thể thêm nhiều dòng.

### Extra fee item fields
- Loại phụ phí:
  - class: `extra-fee-type`
  - select:
    - `Trễ giờ trả xe`
    - `Thiếu nhiên liệu`
    - `Vượt giới hạn km`
    - `Vệ sinh xe`
    - `Hư hỏng xe`
    - `Khác`
- Số tiền:
  - class: `extra-fee-amount`
  - type: `number`
- Mô tả:
  - class: `extra-fee-description`
  - textarea

Button:
- id: `btnAddExtraFee`
- text: `Thêm phụ phí`

Total:
- id: `totalExtraFee`

## Upload ảnh minh chứng
Input:
- id: `returnEvidenceImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `returnEvidencePreviewGrid`

Yêu cầu:
- Nếu có phụ phí/hư hỏng, bắt buộc ít nhất 1 ảnh.
- Tối đa 12 ảnh.

## Buttons
- `Lưu nháp`
  - id: `btnSaveReturnDraft`
- `Xác nhận không phát sinh`
  - id: `btnConfirmReturnNoIssue`
- `Gửi phụ phí cho khách xác nhận`
  - id: `btnSubmitExtraFees`
- `Quay lại`
  - id: `btnBackOperations`

## Validate
- Số km lúc nhận lại >= số km lúc bàn giao.
- Nếu có phụ phí, mỗi dòng phụ phí phải có loại, số tiền > 0, mô tả.
- Nếu có hư hỏng hoặc phụ phí, bắt buộc ảnh minh chứng.
- Không cho xác nhận hoàn tất nếu còn phụ phí chưa được khách/admin xử lý.

## API gợi ý
```txt
POST /api/owner/bookings/{bookingId}/return-condition
POST /api/owner/bookings/{bookingId}/extra-fees
```
