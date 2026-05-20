# VivuCar UI - Edit User Profile

## Mục tiêu trang
Người dùng chỉnh sửa thông tin cá nhân: họ tên, số điện thoại, địa chỉ, ngày sinh, giới tính.

## File HTML gợi ý
`profile-edit.html`

## Layout
- Header user.
- Sidebar profile.
- Form chỉnh sửa hồ sơ.

## Header trang
- Title: `Chỉnh sửa hồ sơ`
- Subtitle: `Cập nhật thông tin cá nhân của bạn`

## Form fields
- Họ tên:
  - id: `fullName`
  - type: `text`
  - required
- Email:
  - id: `email`
  - type: `email`
  - disabled
  - note: `Email không thể thay đổi`
- Số điện thoại:
  - id: `phone`
  - type: `tel`
  - required
- Địa chỉ:
  - id: `address`
  - type: `text`
  - placeholder: `Nhập địa chỉ hiện tại`
- Ngày sinh:
  - id: `dateOfBirth`
  - type: `date`
- Giới tính:
  - id: `gender`
  - select
  - options:
    - `Không chọn`
    - `Nam`
    - `Nữ`
    - `Khác`

## Buttons
- Button:
  - id: `btnCancelEdit`
  - text: `Hủy`
  - quay về `profile.html`
- Button:
  - id: `btnSaveProfile`
  - text: `Lưu thay đổi`

## Alert
- id: `profileUpdateMessage`
- Success:
  - `Cập nhật hồ sơ thành công`
- Error:
  - `Không thể cập nhật hồ sơ. Vui lòng thử lại.`

## Validate
- Họ tên không rỗng.
- Số điện thoại đúng format Việt Nam cơ bản:
  - bắt đầu bằng 0.
  - 10 chữ số.
- Địa chỉ không quá 255 ký tự.
- Ngày sinh không được lớn hơn ngày hiện tại.

## JS cần có
- Load dữ liệu user hiện tại.
- Detect form dirty:
  - Nếu người dùng bấm hủy khi đã sửa, hiện confirm.
- Submit update.
- Sau khi thành công:
  - Cập nhật localStorage fullName nếu có.
  - Chuyển về `profile.html` sau khi hiển thị message.

## API gợi ý
```txt
PUT /api/users/me
```

Body:
```json
{
  "fullName": "Nguyen Van A",
  "phone": "0901234567",
  "address": "Hải Châu, Đà Nẵng",
  "dateOfBirth": "2001-01-01",
  "gender": "MALE"
}
```
