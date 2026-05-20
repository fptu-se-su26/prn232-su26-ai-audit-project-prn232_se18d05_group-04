# VivuCar UI - Login & Role Routing

## Mục tiêu trang
Trang đăng nhập cho Admin, Car Owner, Customer. Sau khi đăng nhập thành công, JS kiểm tra role từ response và điều hướng đúng màn hình.

## File HTML gợi ý
`login.html`

## Layout
- Nền sáng, căn giữa màn hình.
- Card đăng nhập rộng khoảng 420px.
- Logo chữ: `VivuCar`
- Subtitle: `Nền tảng thuê xe tự lái tại Đà Nẵng`
- Form đăng nhập.
- Khu vực hiển thị lỗi.

## Thành phần UI

### Header trong card
- Text logo: `VivuCar`
- Text mô tả ngắn.
- Có thể thêm ảnh/logo bằng thẻ `<div class="brand-logo">VC</div>`

### Form fields
- `Email`
  - type: `email`
  - id: `email`
  - placeholder: `admin@vivucar.vn`
- `Mật khẩu`
  - type: `password`
  - id: `password`
  - placeholder: `Nhập mật khẩu`
- Checkbox:
  - id: `rememberMe`
  - label: `Ghi nhớ đăng nhập`

### Buttons
- Button chính:
  - text: `Đăng nhập`
  - id: `btnLogin`
  - type: `submit`
- Link phụ:
  - text: `Quên mật khẩu?`
  - id: `forgotPasswordLink`

### Alert / Message
- `div#errorMessage`
  - Ẩn mặc định.
  - Hiển thị khi sai email/mật khẩu hoặc tài khoản bị khóa.
- `div#loadingState`
  - Hiển thị text `Đang xác thực...`

## Trạng thái UI
- Default: form trống.
- Loading: disable button, đổi text thành `Đang đăng nhập...`
- Error: border đỏ field lỗi, hiện thông báo.
- Success: lưu token vào `localStorage`.

## Luồng JS cần có
```js
const roleRedirectMap = {
  ADMIN: "admin-dashboard.html",
  CAR_OWNER: "owner-dashboard.html",
  CUSTOMER: "home.html"
};
```

### Validate phía client
- Email không được rỗng.
- Email đúng format.
- Password tối thiểu 6 ký tự.

### Submit flow
1. Người dùng nhập email/password.
2. Click `Đăng nhập`.
3. JS gọi API giả lập hoặc thật:
   - `POST /api/auth/login`
4. Response mẫu:
```json
{
  "token": "jwt-token",
  "role": "ADMIN",
  "status": "ACTIVE",
  "fullName": "Nguyen Van A"
}
```
5. Nếu `status === "BLOCKED"`:
   - Không đăng nhập.
   - Hiển thị: `Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.`
6. Nếu hợp lệ:
   - Lưu `token`, `role`, `fullName`.
   - Điều hướng theo role.

## Gợi ý CSS
- Card bo góc 16px.
- Button chính màu xanh dương.
- Error màu đỏ nhạt.
- Responsive mobile: card chiếm 90% width.

## Không dùng
- Không React.
- Không Bootstrap bắt buộc.
- Không framework JS.
