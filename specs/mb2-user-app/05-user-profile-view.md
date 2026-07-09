# VivuCar UI - User Profile View

## Mục tiêu trang
Người dùng xem hồ sơ cá nhân gồm họ tên, email, số điện thoại, địa chỉ, ảnh đại diện và trạng thái xác minh giấy phép lái xe.

## File HTML gợi ý
`profile.html`

## Layout
- Header user.
- Sidebar profile bên trái.
- Content hồ sơ bên phải.

## Profile sidebar
Menu:
- `Thông tin cá nhân`
- `Chỉnh sửa hồ sơ`
- `Giấy phép lái xe`
- `Đánh giá của tôi`
- `Đơn thuê của tôi`
- `Đăng xuất`

Item active:
- `Thông tin cá nhân`

## Profile header card
Hiển thị:
- Avatar:
  - id: `profileAvatar`
- Họ tên:
  - id: `profileFullName`
- Email:
  - id: `profileEmail`
- Badge xác minh:
  - `Đã xác minh GPLX`
  - hoặc `Chưa xác minh GPLX`
- Button:
  - id: `btnEditProfile`
  - text: `Chỉnh sửa hồ sơ`
- Button:
  - id: `btnChangeAvatar`
  - text: `Đổi ảnh đại diện`

## Thông tin cá nhân
Card fields:
- Họ tên.
- Email.
- Số điện thoại.
- Địa chỉ.
- Ngày sinh.
- Giới tính.
- Ngày tham gia.

## Thông tin thuê xe
Cards nhỏ:
- Tổng số đơn đã thuê.
- Số đơn hoàn tất.
- Số đánh giá đã gửi.
- Tổng chi tiêu.

## Trạng thái giấy phép lái xe
Card:
- Số GPLX nếu đã có.
- Ngày hết hạn.
- Trạng thái:
  - `Chưa upload`
  - `Chờ xác minh`
  - `Đã xác minh`
  - `Bị từ chối`
- Button:
  - id: `btnUploadLicense`
  - text: `Upload giấy phép lái xe`

## JS cần có
- Kiểm tra login.
- Load user profile từ mock data hoặc API.
- Render badge theo license status.
- Click edit chuyển:
  - `profile-edit.html`
- Click upload license chuyển:
  - `driving-license.html`

## API gợi ý
```txt
GET /api/users/me
```

Response:
```json
{
  "id": "U001",
  "fullName": "Nguyen Van A",
  "email": "a@gmail.com",
  "phone": "0901234567",
  "address": "Hải Châu, Đà Nẵng",
  "avatarUrl": "../assets/images/avatar-default.png",
  "licenseStatus": "VERIFIED",
  "joinedAt": "2026-05-01"
}
```
