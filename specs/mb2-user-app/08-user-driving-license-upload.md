# VivuCar UI - Driving License Upload

## Mục tiêu trang
Người dùng upload giấy phép lái xe để xác minh điều kiện thuê xe.

## File HTML gợi ý
`driving-license.html`

## Layout
- Header user.
- Sidebar profile.
- Card trạng thái xác minh.
- Form upload giấy phép lái xe.
- Preview ảnh mặt trước/mặt sau.

## Header trang
- Title: `Giấy phép lái xe`
- Subtitle: `Upload giấy phép lái xe để được xác minh trước khi thuê xe.`

## Status card
Hiển thị:
- Trạng thái:
  - `Chưa upload`
  - `Chờ xác minh`
  - `Đã xác minh`
  - `Bị từ chối`
- Ngày gửi xác minh.
- Ghi chú từ Admin nếu bị từ chối.
- Badge màu:
  - Pending: vàng.
  - Verified: xanh.
  - Rejected: đỏ.
  - Not Uploaded: xám.

## Form fields
- Số giấy phép lái xe:
  - id: `licenseNumber`
  - placeholder: `Nhập số GPLX`
  - required
- Hạng bằng lái:
  - id: `licenseClass`
  - select
  - options:
    - `B1`
    - `B2`
    - `C`
    - `D`
    - `E`
- Ngày cấp:
  - id: `issuedDate`
  - type: `date`
- Ngày hết hạn:
  - id: `expiredDate`
  - type: `date`

## Upload mặt trước
- Input:
  - id: `licenseFrontImage`
  - type: `file`
  - accept: `image/*`
- Preview:
  - id: `licenseFrontPreview`
- Button:
  - id: `btnRemoveFrontImage`
  - text: `Xóa ảnh mặt trước`

## Upload mặt sau
- Input:
  - id: `licenseBackImage`
  - type: `file`
  - accept: `image/*`
- Preview:
  - id: `licenseBackPreview`
- Button:
  - id: `btnRemoveBackImage`
  - text: `Xóa ảnh mặt sau`

## Hướng dẫn upload
Box info:
- Ảnh rõ nét, không bị mờ.
- Không che thông tin trên giấy phép.
- Dung lượng mỗi ảnh không quá 5MB.
- Chấp nhận JPG, PNG, WEBP.

## Buttons
- `Hủy`
  - id: `btnCancelLicense`
- `Gửi xác minh`
  - id: `btnSubmitLicense`

## Validate
- Số GPLX không rỗng.
- Chọn hạng bằng lái.
- Ngày hết hạn phải sau ngày cấp.
- Bắt buộc upload mặt trước.
- Bắt buộc upload mặt sau.
- File ảnh không vượt quá 5MB.

## JS cần có
- Preview ảnh trước khi upload.
- Validate form.
- Submit bằng `FormData`.
- Sau khi gửi:
  - đổi trạng thái thành `Chờ xác minh`.
  - disable form nếu đang pending hoặc verified.

## API gợi ý
```txt
POST /api/users/me/driving-license
```

Body:
- `multipart/form-data`
- fields:
  - `licenseNumber`
  - `licenseClass`
  - `issuedDate`
  - `expiredDate`
  - `frontImage`
  - `backImage`

## Message
- Success:
  - `Đã gửi giấy phép lái xe để xác minh`
- Error:
  - `Không thể gửi xác minh. Vui lòng thử lại.`
