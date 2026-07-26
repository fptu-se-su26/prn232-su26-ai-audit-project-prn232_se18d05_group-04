# SE AI Audit Project Template

## 1. Project Information

| Item | Description |
|---|---|
| Course | PRN232 |
| Class | SE18D05 |
| Semester | SU26 |
| Group | 4 |
| Topic | Ứng dụng cho thuê xe tự lái (VivuCar) |
| Repository | https://github.com/fptu-se-su26/prn232-su26-ai-audit-project-prn232_se18d05_group-04 |

---

## 2. Team Members

| No | Student ID | Full Name | GitHub Username | Role | Main Responsibility |
|---:|---|---|---|---|---|
| 1 | DE180116 | Nguyễn Minh Tuấn | winhtuan | Leader | Quản trị hệ thống, Xác thực, Admin, Voucher, Báo cáo |
| 3 | DE180093 | Phạm Hồng Quân | hongquandt | Member | Vận hành Chủ xe, Bàn giao, Trả xe, Sự cố |
| 4 | DE180117 | Ngô Sỹ Giá | Giahensum | Member | Booking, Tính giá, Thanh toán, Hoàn tiền |
| 5 | DE191106 | Nguyễn Lê Tiểu Long | longg1708 | Member | Tìm kiếm xe, Hồ sơ, Đánh giá, Chatbot |

---

## 3. Phân công nhiệm vụ

### Thành viên 1 — Nguyễn Minh Tuấn (Admin, Auth, Voucher, Báo cáo)

**Quản lý Định danh và Phân quyền**
- Đăng nhập, cấp token, phân quyền (Admin / Car Owner / Customer)
- Middleware bảo vệ API
- Quản lý tài khoản: xem, tìm kiếm, lọc, khóa/mở khóa
- Đăng xuất, thu hồi token

**Quản lý Danh mục Phương tiện phía Admin**
- Xem, chỉnh sửa xe toàn hệ thống
- Quản lý hãng xe, dòng xe, nhiên liệu, hộp số
- Phân trang, tìm kiếm, lọc theo trạng thái
- Khóa/mở khóa xe

**Báo cáo và Xuất dữ liệu**
- Dashboard thống kê (người dùng, xe, booking, doanh thu)
- Xuất Excel (giao dịch, booking, khách hàng, xe)
- Xuất PDF (doanh thu, booking)

**Voucher và Khuyến mãi**
- Tạo/cập nhật voucher, thiết lập điều kiện áp dụng
- Theo dõi số lượt sử dụng, tổng tiền đã giảm

### Thành viên 2 — Nguyễn Lê Tiểu Long (Trải nghiệm Khách hàng, Chatbot)

**Duyệt và Tìm kiếm xe**
- Xem danh sách xe khả dụng, xe nổi bật
- Chi tiết xe: thông số, giá, hình ảnh, chính sách, đánh giá

**Tìm kiếm và Bộ lọc**
- Tìm theo tên, hãng, dòng xe; gợi ý từ khóa
- Lọc: hãng, giá, khu vực, hộp số, nhiên liệu, số chỗ, đánh giá
- Sắp xếp: giá, đánh giá, lượt thuê, mới nhất

**Quản lý Hồ sơ Người dùng**
- Xem/cập nhật hồ sơ, upload giấy tờ (GPLX, CCCD)

**Đánh giá và Phản hồi**
- Chấm điểm 1-5 sao, bình luận sau booking hoàn tất
- Tổng hợp điểm trung bình, phân bố sao

**AI Chatbot và Hỗ trợ**
- Chatbot giải đáp quy trình, trạng thái booking, thanh toán
- Lịch sử hội thoại, chuyển hỗ trợ trực tiếp

### Thành viên 3 — Ngô Sỹ Giá (Booking, Payment)

**Booking Checkout**
- Kiểm tra khả dụng, chống xung đột thời gian
- Thông tin người thuê/người lái, kiểm tra giấy tờ
- Tính giá động (ngày thường/cuối tuần, bảo hiểm, phí dịch vụ, voucher)
- Tạo booking, sinh mã, lưu chi tiết giá

**Thanh toán Tiền cọc**
- Tích hợp VNPay/MoMo, tạo URL thanh toán
- Xử lý callback/webhook, cập nhật trạng thái
- Tạo biên lai, gửi email xác nhận

**Theo dõi Booking**
- Danh sách booking theo trạng thái
- Chi tiết booking, hợp đồng điện tử PDF

**Hủy Booking**
- Kiểm tra điều kiện hủy, chính sách hoàn tiền
- Xử lý hoàn tiền, theo dõi refund

### Thành viên 4 — Phạm Hồng Quân (Chủ xe, Bàn giao, Trả xe)

**Quản lý Xe của Chủ xe**
- Đăng xe mới, cập nhật thông tin, quản lý hình ảnh

**Trạng thái và Bảo trì Xe**
- Cập nhật trạng thái (Available, Maintenance, Unavailable...)
- Tạm ngưng/mở lại cho thuê, lịch sử hoạt động

**Quản lý Yêu cầu Booking (Car Owner)**
- Xem booking mới, chấp nhận/từ chối
- Gửi thông báo cho khách hàng

**Bàn giao Xe**
- Kiểm tra giấy tờ, ghi nhận thời gian, số km, nhiên liệu
- Biên bản bàn giao, xác nhận hai bên

**Trả xe và Kết thúc Chuyến đi**
- Xác nhận trả xe, kiểm tra sau chuyến đi
- Cập nhật trạng thái booking, giải phóng lịch xe

**Báo cáo Sự cố và Tranh chấp**
- Phụ phí (trễ, nhiên liệu, vệ sinh, hư hỏng)
- Chuyển tranh chấp cho Admin

---

### Ma trận phân chia nghiệp vụ

| Nghiệp vụ | Phụ trách |
|-----------|-----------|
| Authentication & Authorization | Tuấn |
| Quản lý tài khoản | Tuấn |
| Quản trị xe toàn hệ thống | Tuấn |
| Báo cáo và xuất dữ liệu | Tuấn |
| Voucher | Tuấn |
| Tìm kiếm và xem xe | Long |
| Hồ sơ khách hàng | Long |
| Review và Rating | Long |
| AI Chatbot & Customer Support | Long |
| Availability và tạo Booking | Giá |
| Dynamic Pricing | Giá |
| Payment, Webhook và Refund | Giá |
| My Bookings và Cancellation | Giá |
| Đăng xe của Car Owner | Quân |
| Trạng thái và bảo trì xe | Quân |
| Accept/Decline Booking | Quân |
| Bàn giao và trả xe | Quân |
| Sự cố và phụ phí sau chuyến đi | Quân |

### Quy tắc phối hợp

1. **Tuấn** sở hữu User, Role, Permission, Voucher và các API Admin.
2. **Long** sở hữu giao diện Customer Experience, Profile, Review và Chat Support.
3. **Giá** sở hữu Booking, Booking Price, Payment và Refund.
4. **Quân** sở hữu Car Owner, Vehicle Operation, Handover và Return.
5. Chỉ **Giá** được thay đổi dữ liệu tài chính của booking.
6. Chỉ **Quân** được xác nhận giao xe, nhận xe và hoàn tất chuyến đi.
7. **Tuấn** có thể khóa xe hoặc tài khoản, nhưng không xử lý nghiệp vụ bàn giao và trả xe.
8. Điểm đánh giá chỉ được tạo sau khi **Quân** xác nhận booking Completed.
9. Trạng thái xe phải kiểm tra lịch thuê, bảo trì và khóa trước khi xác định Available.

---

## 4. Project Structure

```text
VivuCarClient/
├── WebClient/                    # ASP.NET Core 8 Web App (Razor Pages)
│   ├── Pages/
│   │   ├── Admin/                # Giao diện Admin
│   │   ├── Booking/              # Checkout, MyBookings, Detail, Contract
│   │   ├── Cars/                 # Tìm kiếm, Chi tiết xe
│   │   ├── Owner/                # Giao diện Car Owner
│   │   ├── Payment/              # Deposit, Final, Result
│   │   ├── Shared/               # Layout, Navbar, Chatbot
│   │   └── User/                 # Profile
│   ├── wwwroot/
│   │   ├── css/                  # booking.css, auth.css, chat.css, register.css
│   │   ├── js/
│   │   │   ├── html/             # UI logic (utils, constants, tailwind-ui, auth...)
│   │   │   └── shared/           # auth-service.js, navbar.js
│   │   └── images/
│   └── WebClient.csproj
│
VivuCarServer/
├── API/                          # ASP.NET Core Web API
│   ├── Controllers/
│   └── API.csproj
├── BusinessObjects/              # Entity Framework models, Migrations
├── Repositories/                 # Data access layer
├── Services/                     # Business logic layer
└── Tests/                        # Unit tests
```

---

## 5. Required AI Audit Documents

Each group must maintain the following documents:

```text
docs/AI_AUDIT_LOG.md
docs/PROMPTS.md
docs/REFLECTION.md
docs/CHANGELOG.md
```

---

## 6. Workflow

Students must follow this workflow:

```text
Issue → Branch → Commit → Pull Request → Review → Merge
```

Direct push to the `main` branch should be avoided.

---

## 7. Branch Naming Convention

```text
feature/studentid-task-name
bugfix/studentid-error-name
docs/studentid-update-audit-log
test/studentid-test-case-name
```

Example:

```text
feature/se123456-login-page
bugfix/se123456-login-validation
docs/se123456-update-ai-audit-log
```

---

## 8. Commit Message Convention

```text
[StudentID] type: short description
```

Examples:

```text
[SE123456] feat: add login page
[SE123456] fix: fix login validation
[SE123456] docs: update AI audit log
[SE123456] test: add login test cases
```

Common types:

```text
feat, fix, docs, test, refactor, style, chore
```

---

## 9. How to Run

### Yêu cầu
- .NET 8 SDK
- SQL Server (LocalDB hoặc remote)
- Node.js (optional, cho frontend build)

### Khởi động Backend (API)

```bash
cd VivuCarServer/API
# Cấu hình connection string trong appsettings.json
dotnet restore
dotnet run
# API chạy tại https://localhost:5001
```

### Khởi động Frontend (WebClient)

```bash
cd VivuCarClient/WebClient
dotnet restore
dotnet run
# Web chạy tại https://localhost:5162
```

### Migration (nếu cần cập nhật DB)

```bash
cd VivuCarServer/API
dotnet ef database update
```

### Chạy Test

```bash
cd VivuCarServer/Tests
dotnet test
```

### Port mặc định
| Service | URL |
|---------|-----|
| Web Client | http://localhost:5162 |
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |

---

## 10. AI Usage Rule

Students are allowed to use AI tools such as ChatGPT, Gemini, Claude, GitHub Copilot, Cursor, Antigravity, or similar tools.

However, all important AI usage must be recorded in:

```text
docs/AI_AUDIT_LOG.md
docs/PROMPTS.md
docs/CHANGELOG.md
docs/REFLECTION.md
```

Students must be able to explain, verify, and defend all submitted work.
