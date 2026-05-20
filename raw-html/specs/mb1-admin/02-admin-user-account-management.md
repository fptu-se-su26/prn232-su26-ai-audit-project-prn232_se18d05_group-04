# VivuCar UI - Admin Account Management & Blocking

## Mục tiêu trang
Admin xem danh sách người dùng, lọc theo role/trạng thái, khóa hoặc mở khóa tài khoản.

## File HTML gợi ý
`admin-users.html`

## Layout
- Sidebar Admin bên trái.
- Header trên cùng.
- Nội dung chính gồm:
  - Tiêu đề trang.
  - Bộ lọc.
  - Bảng danh sách user.
  - Modal xác nhận khóa/mở khóa.

## Sidebar
Menu:
- Dashboard
- Quản lý người dùng
- Quản lý xe
- Đơn thuê xe
- Voucher
- Báo cáo
- Đăng xuất

Item active: `Quản lý người dùng`

## Header
- Search global nhỏ.
- Tên admin.
- Avatar hình tròn.
- Button `Đăng xuất`

## Bộ lọc
Fields:
- Ô tìm kiếm:
  - id: `searchUserInput`
  - placeholder: `Tìm theo tên, email, số điện thoại`
- Select role:
  - id: `roleFilter`
  - options:
    - `Tất cả vai trò`
    - `Admin`
    - `Car Owner`
    - `Customer`
- Select trạng thái:
  - id: `statusFilter`
  - options:
    - `Tất cả trạng thái`
    - `Active`
    - `Blocked`
- Button:
  - id: `btnApplyFilter`
  - text: `Lọc`
- Button:
  - id: `btnResetFilter`
  - text: `Đặt lại`

## Bảng user
Columns:
- Checkbox chọn user.
- Mã user.
- Họ tên.
- Email.
- Số điện thoại.
- Vai trò.
- Trạng thái.
- Ngày tạo.
- Hành động.

## Badge trạng thái
- `Active`: nền xanh nhạt.
- `Blocked`: nền đỏ nhạt.

## Nút hành động từng dòng
- `Xem`
  - class: `btn-view-user`
- `Khóa`
  - class: `btn-block-user`
  - chỉ hiện khi user Active.
- `Mở khóa`
  - class: `btn-unblock-user`
  - chỉ hiện khi user Blocked.

## Modal xác nhận
### Modal khóa tài khoản
Fields:
- Text: `Bạn chắc chắn muốn khóa tài khoản này?`
- Textarea:
  - id: `blockReason`
  - placeholder: `Nhập lý do khóa tài khoản`
- Button:
  - `Hủy`
  - `Xác nhận khóa`

### Modal mở khóa
- Text: `Mở khóa tài khoản này?`
- Button:
  - `Hủy`
  - `Xác nhận mở khóa`

## Pagination
- Text: `Hiển thị 1-10 trên 120 người dùng`
- Button:
  - `Trước`
  - Số trang
  - `Sau`
- Select page size:
  - 10
  - 20
  - 50

## JS cần có
- Render users từ mảng data.
- Filter theo keyword, role, status.
- Click khóa/mở khóa cập nhật status.
- Không cho block chính tài khoản admin đang đăng nhập.
- Có toast sau thao tác:
  - `Đã khóa tài khoản thành công`
  - `Đã mở khóa tài khoản thành công`

## Gợi ý dữ liệu mẫu
```js
const users = [
  {
    id: "U001",
    fullName: "Nguyen Van A",
    email: "a@gmail.com",
    phone: "0901234567",
    role: "CUSTOMER",
    status: "ACTIVE",
    createdAt: "2026-05-10"
  }
];
```
