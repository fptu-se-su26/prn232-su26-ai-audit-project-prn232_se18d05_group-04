# VivuCar UI - Logout & Session Invalidation

## Mục tiêu trang / component
Tạo component đăng xuất dùng chung cho Admin, Car Owner, Customer. Xóa token client và hiển thị modal xác nhận trước khi đăng xuất.

## File HTML gợi ý
`logout-component.html`

## Vị trí sử dụng
- Header admin.
- Sidebar admin.
- Header user homepage.
- Owner dashboard.

## Thành phần UI

### Button đăng xuất
- id: `btnLogout`
- text: `Đăng xuất`
- Có icon text đơn giản nếu muốn: `← Đăng xuất`
- Style:
  - border nhẹ.
  - hover nền đỏ nhạt.

### Modal xác nhận đăng xuất
- id: `logoutModal`
- Title: `Xác nhận đăng xuất`
- Content:
  - `Bạn có chắc chắn muốn đăng xuất khỏi VivuCar?`
- Buttons:
  - `Hủy`
    - id: `btnCancelLogout`
  - `Đăng xuất`
    - id: `btnConfirmLogout`

### Loading state
- Khi xác nhận:
  - disable button.
  - đổi text thành `Đang đăng xuất...`

## JS cần có

### Hàm mở modal
```js
function openLogoutModal() {
  document.getElementById("logoutModal").classList.add("show");
}
```

### Hàm đóng modal
```js
function closeLogoutModal() {
  document.getElementById("logoutModal").classList.remove("show");
}
```

### Hàm logout
```js
async function logout() {
  const token = localStorage.getItem("token");

  try {
    // Nếu backend hỗ trợ blacklist token
    await fetch("/api/auth/logout", {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });
  } catch (error) {
    console.log("Logout API failed, continue client logout");
  }

  localStorage.removeItem("token");
  localStorage.removeItem("role");
  localStorage.removeItem("fullName");

  window.location.href = "login.html";
}
```

## Guard kiểm tra token
Tạo file `auth-guard.js` dùng cho các trang cần đăng nhập.

```js
function requireAuth(allowedRoles = []) {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  if (!token) {
    window.location.href = "login.html";
    return;
  }

  if (allowedRoles.length && !allowedRoles.includes(role)) {
    window.location.href = "403.html";
  }
}
```

## Trang 403
Có thể tạo thêm section đơn giản:
- Title: `Bạn không có quyền truy cập`
- Text: `Vui lòng đăng nhập bằng tài khoản phù hợp.`
- Button: `Về trang đăng nhập`

## Trạng thái UI
- Modal đóng mặc định.
- Click ngoài modal sẽ đóng.
- Nhấn ESC sẽ đóng.
- Sau logout thành công chuyển về login.

## Không dùng
- Không thư viện modal.
- Không framework.
