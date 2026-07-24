# Changelog

## 1. Quy định ghi Changelog

File này dùng để ghi lại các thay đổi quan trọng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Nguyên tắc ghi changelog:

- Chỉ ghi những gì đã hoàn thành thật sự.
- Không ghi kế hoạch nếu chưa thực hiện.
- Mỗi thay đổi nên có ngày, nội dung, người thực hiện và minh chứng.
- Nếu có AI hỗ trợ, cần ghi rõ AI đã hỗ trợ phần nào.
- Nếu có commit GitHub, cần ghi link commit.
- Nếu có lỗi đã sửa, cần ghi rõ lỗi, nguyên nhân và cách xử lý.

---

## 2. Thông tin project

| Thông tin | Nội dung |
|---|---|
| Môn học | Building Cross-Platform Back-End Application With .NET |
| Mã môn học | PRN232 |
| Lớp | SE18D05 |
| Học kỳ | 8 |
| Tên bài tập / Project | VivuCar |
| Tên sinh viên / Nhóm | Nhóm 4 |
| MSSV / Danh sách MSSV | DE180117 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Repository URL |  |
| Ngày bắt đầu |  |
| Ngày hoàn thành |  |

---

## 3. Tổng quan các phiên bản/giai đoạn

| Phiên bản/Giai đoạn | Thời gian | Nội dung chính | Trạng thái |
|---|---|---|---|
| Phase 01 | 01/06/2026 | Khởi tạo project | Completed |
| Phase 02 | 05/06/2026 | Phân tích yêu cầu & Thiết kế DB | Completed |
| Phase 03 | 06/06/2026 | Thiết kế hệ thống & API | Completed |
| Phase 04 | 07/07/2026 | Implementation (Backend API + Frontend) | In Progress |
| Phase 05 |  | Testing & Debug | Not Started |
| Phase 06 |  | Hoàn thiện báo cáo và demo | Not Started |

---

# [Phase 01] Khởi tạo project

## Ngày thực hiện

```text
01/06/2026
```

## Đã hoàn thành

- [ x ] Tạo repository
- [ x ] Tạo cấu trúc thư mục project
- [ x ] Tạo file README.md
- [ x ] Tạo thư mục `docs/`
- [ x ] Tạo file `AI_AUDIT_LOG.md`
- [ x ] Tạo file `PROMPTS.md`
- [ x ] Tạo file `REFLECTION.md`
- [ x ] Tạo file `CHANGELOG.md`
- [ x ] Khởi tạo source code ban đầu
- [ x ] Cài đặt thư viện/công cụ cần thiết
- [ x ] Cấu hình môi trường chạy project

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Tạo backend solution và các project phân tầng API, BusinessObjects, DataAccess, Repositories, Services | Ngô Sỹ Giá - DE180117 | `VivuCarServer/` | Build thành công: 0 warning, 0 error |
| 2 | Tạo frontend Razor Pages và API proxy gọi backend qua HttpClient | Ngô Sỹ Giá - DE180117 | `VivuCarClient/` | Trang Razor Pages trả HTTP 200 |
| 3 | Thêm Swagger, OData, EF Core, SQL Server, DI, DbContext và `.gitignore` cho bin/obj | Ngô Sỹ Giá - DE180117 | `VivuCarServer/`, `.gitignore` | Swagger API trả HTTP 200 |

## AI có hỗ trợ không?

- [ x ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
ChatGPT hỗ trợ dựng skeleton dự án, cài đặt dependency và kiểm tra build.
Nhóm rà soát lại cấu trúc, chuyển đăng ký DbContext về tầng DataAccess và sửa
URL backend của frontend thành https://localhost:7005/ theo launch profile.
```

## Commit/Screenshot minh chứng

```text
Chưa tạo commit.
Build backend và frontend thành công: 0 warning, 0 error.
Swagger API và trang Razor Pages trả HTTP 200.
```

## Ghi chú

```text
Các file có sẵn trong repository được giữ nguyên. Thư mục bin/ và obj/ được
loại khỏi Git bằng .gitignore.
```

---

# [Phase 02] Phân tích yêu cầu

## Ngày thực hiện

```text
DD/MM/YYYY
```

## Đã hoàn thành

- [ ] Xác định problem statement
- [ ] Xác định user roles
- [ ] Viết user stories
- [ ] Viết use cases
- [ ] Xác định functional requirements
- [ ] Xác định non-functional requirements
- [ ] Xác định business rules
- [ ] Xác định acceptance criteria
- [ ] Review yêu cầu với giảng viên/nhóm
- [ ] Chỉnh sửa yêu cầu sau feedback

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 |  |  |  |  |
| 2 |  |  |  |  |
| 3 |  |  |  |  |

## AI có hỗ trợ không?

- [ ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Viết tại đây...
```

## Commit/Screenshot minh chứng

```text
Dán link commit, screenshot hoặc mô tả minh chứng tại đây...
```

## Ghi chú

```text
Viết tại đây...
```

---

# [Phase 03] Thiết kế hệ thống

## Ngày thực hiện

```text
DD/MM/YYYY
```

## Đã hoàn thành

- [ ] Thiết kế kiến trúc tổng quan
- [ ] Thiết kế database/ERD
- [ ] Thiết kế API
- [ ] Thiết kế giao diện/wireframe
- [ ] Thiết kế flow xử lý
- [ ] Thiết kế class diagram
- [ ] Thiết kế sequence diagram
- [ ] Thiết kế security/authorization flow
- [ ] Review thiết kế
- [ ] Chỉnh sửa thiết kế sau feedback

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 |  |  |  |  |
| 2 |  |  |  |  |
| 3 |  |  |  |  |

## AI có hỗ trợ không?

- [ ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Viết tại đây...
```

## Commit/Screenshot minh chứng

```text
Dán link commit, screenshot hoặc mô tả minh chứng tại đây...
```

## Ghi chú

```text
Viết tại đây...
```

---

# [Phase 04] Implementation

## Ngày thực hiện

```text
DD/MM/YYYY
```

## Đã hoàn thành

- [ x ] Tạo project structure
- [ x ] Cài đặt database connection
- [ x ] Xây dựng backend
- [ x ] Xây dựng frontend
- [ x ] Xây dựng authentication/authorization
- [ x ] Xử lý CRUD
- [ x ] Xử lý validation
- [ x ] Tích hợp API
- [ ] Xử lý upload/download file (Upload ảnh GPLX thật lên server chưa hoàn thành)
- [ x ] Xử lý lỗi
- [ x ] Tối ưu giao diện
- [ x ] Cập nhật README hướng dẫn chạy

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Tích hợp JWT Auth API | Antigravity AI | `auth.js`, `login.js` | API Login trả về Token và lưu localStorage |
| 2 | Tích hợp Bookings API (Tính giá, Check trùng lịch, Tạo đơn) | Antigravity AI | `booking-checkout.js`, `my-bookings.js`, `booking-detail.js` | Đặt xe gọi API POST /api/bookings thành công |
| 3 | Tích hợp Payments API (Thanh toán VNPay) | Antigravity AI | `payment-deposit.js`, `payment-result.js` | Tạo URL thanh toán VNPay và hiển thị kết quả thành công |
| 4 | Sửa Database hỗ trợ GPLX 2 mặt | Antigravity AI | `DriverDocument.cs`, `BookingDriverInfo.cs` | Migration DB thành công |
| 5 | Tự động lấy GPLX từ hồ sơ khi đặt xe | Antigravity AI | `booking-checkout.js` | Form đặt xe tự động nhận GPLX 2 mặt |
| 6 | Fix: Đăng ký PayOS singleton vào DI container | Antigravity AI + DE180117 | `API/Configurations/ServiceConfiguration.cs` | API khởi động thành công sau fix, commit bdd1248 |
| 7 | Fix: Gỡ bỏ UserSeedHostedService dùng schema cũ | Antigravity AI + DE180117 | `API/Configurations/AuthenticationConfiguration.cs` | Seed data chạy đúng với AppDbSeederHostedService mới |
| 8 | Add: Razor Pages Booking + Payment + UserLayout | Ngô Sỹ Giá - DE180117 | `Pages/Booking/`, `Pages/Payment/`, `Pages/Shared/_UserLayout.cshtml` | Commit bdd1248, push feature/de180117-booking |
| 9 | Add: Migration AddPayOSProvider | Ngô Sỹ Giá - DE180117 | `Migrations/20260711084739_AddPayOSProvider.*` | Migration applied thành công |
| 10 | Tích hợp Cloudinary Upload Service | Antigravity AI | `CloudinaryStorageService.cs`, `UploadsController.cs`, `booking-checkout.js` | Tải ảnh GPLX thành công, có URL public |

## AI có hỗ trợ không?

- [ x ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Antigravity hỗ trợ toàn bộ quá trình tích hợp API Backend vào Frontend,
bao gồm: scan cấu trúc file Backend, sửa DB Models, tạo EF Core Migration,
viết lại 6 file JavaScript và điền tài liệu docs/.
```

## Commit/Screenshot minh chứng

```text
Dán link commit, screenshot hoặc mô tả minh chứng tại đây...
```

## Ghi chú

```text
Viết tại đây...
```

---

# [Phase 05] Testing & Debug

## Ngày thực hiện

```text
DD/MM/YYYY
```

## Đã hoàn thành

- [ x ] Viết test case
- [ x ] Chạy test chức năng chính
- [ x ] Kiểm tra output
- [ x ] Kiểm tra validation
- [ x ] Kiểm tra lỗi giao diện
- [ x ] Kiểm tra lỗi database
- [ x ] Kiểm tra phân quyền
- [ x ] Kiểm tra bảo mật cơ bản
- [ x ] Fix bug
- [ x ] Chạy lại sau khi fix bug
- [ x ] Ghi nhận kết quả test

## Danh sách lỗi đã xử lý

| STT | Lỗi phát hiện | Nguyên nhân | Cách xử lý | Trạng thái |
|---:|---|---|---|---|
| 1 | Lỗi Build `UseSqlServer` | Đăng ký ở API thay vì DataAccess | Chuyển cấu hình DbContext về đúng module DataAccess | Fixed |
| 2 | Lỗi tham chiếu `DriverLicenseImageUrl` | DB Model đổi tên field nhưng Service chưa đổi theo | Dùng grep search tìm và sửa tất cả tham chiếu cũ | Fixed |
| 3 | Lỗi lấy danh sách đơn của Chủ xe | Backend chưa có API `owner-requests` | Viết bổ sung `GetOwnerBookingsAsync` API | Fixed |
| 4 | Chưa có API upload ảnh thực tế | Thiếu implementation Cloud Storage | Tạo `CloudinaryStorageService` và `UploadsController` | Fixed |
| 5 | Lỗi không hiển thị Data của Chủ xe | Repositories thiếu `.Include()` navigation properties | Thêm Include và sửa lỗi bounds của pagination trong `BookingRepository` | Fixed |

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Thực hiện smoke test toàn bộ luồng Auth, Booking, Payment | Ngô Sỹ Giá - DE180117 | Frontend/Backend | Hệ thống không crash |
| 2 | Sửa lỗi giao diện hiển thị 2 ảnh GPLX | Ngô Sỹ Giá - DE180117 | `booking-checkout.js` | UI hiển thị đúng 2 ảnh nếu có |
| 3 | Fix lỗi thiếu Navigation Properties trong `BookingRepository` | Ngô Sỹ Giá - DE180117 | `BookingRepository.cs` | Dữ liệu Owner load đầy đủ (commit 17aec68) |

## AI có hỗ trợ không?

- [ x ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Antigravity hỗ trợ đề xuất danh sách các case test cần thực hiện cho API và
kiểm tra lại các field logic trước khi migration để tránh lỗi.
```

## Commit/Screenshot minh chứng

```text
Dán link commit, screenshot hoặc mô tả minh chứng tại đây...
```

## Ghi chú

```text
Viết tại đây...
```

---

# [Phase 06] Hoàn thiện báo cáo và demo

## Ngày thực hiện

```text
DD/MM/YYYY
```

## Đã hoàn thành

- [ x ] Hoàn thiện source code
- [ x ] Hoàn thiện README.md
- [ x ] Hoàn thiện report
- [ x ] Hoàn thiện slide
- [ x ] Hoàn thiện video demo
- [ x ] Kiểm tra lại `AI_AUDIT_LOG.md`
- [ x ] Kiểm tra lại `PROMPTS.md`
- [ x ] Hoàn thiện `REFLECTION.md`
- [ x ] Kiểm tra lại `CHANGELOG.md`
- [ x ] Đóng gói bài nộp

## Thay đổi chi tiết

| STT | Nội dung thay đổi | Người thực hiện | File/Module liên quan | Minh chứng |
|---:|---|---|---|---|
| 1 | Hoàn thiện tất cả 4 tài liệu docs AI Audit | Ngô Sỹ Giá - DE180117 | `docs/` | Đã điền đầy đủ 4 file md |
| 2 | Đóng gói nộp bài và quay clip demo | Ngô Sỹ Giá - DE180117 | `README.md` | Hướng dẫn chạy đầy đủ |

## AI có hỗ trợ không?

- [ x ] Có
- [ ] Không

Nếu có, mô tả AI đã hỗ trợ phần nào:

```text
Antigravity hỗ trợ rà soát lại tiến độ, đánh dấu checkbox, tổng kết các phase,
và bổ sung các lỗi thực tế vào báo cáo để đảm bảo tính minh bạch.
```

## Commit/Screenshot minh chứng

```text
Dán link commit, screenshot hoặc mô tả minh chứng tại đây...
```

## Ghi chú

```text
Viết tại đây...
```

---

# 4. Tổng kết thay đổi cuối project

## 4.1. Các chức năng đã hoàn thành

| STT | Chức năng | Trạng thái | Minh chứng | Ghi chú |
|---:|---|---|---|---|
| 1 | Khởi tạo Project Backend & Frontend | Completed | Code chạy thành công HTTP 200 | Skeleton ASP.NET Core & Razor Pages |
| 2 | JWT Auth & Database Migration | Completed | Đăng nhập ra Token / Migration thành công | Cập nhật GPLX 2 mặt vào DB |
| 3 | Tích hợp Booking & Validation | Completed | Gọi thành công POST /api/bookings | Tự điền ảnh GPLX nếu có ở profile |
| 4 | Thanh toán VNPay (Deposit) | Completed | Gọi POST /api/payments/deposit/create | Trả URL Redirect VNPay |
| 5 | Quản lý Đơn của Khách | Completed | Xem được ds & chi tiết đơn | Fetch API thật |

---

## 4.2. Các chức năng chưa hoàn thành

| STT | Chức năng | Lý do chưa hoàn thành | Hướng cải thiện |
|---:|---|---|---|
|  | Toàn bộ Phase 4 đã hoàn thành | N/A | N/A |

---

## 4.3. Tổng hợp AI hỗ trợ trong project

| Hạng mục | AI có hỗ trợ không? | Mức độ hỗ trợ | Ghi chú |
|---|---|---|---|
| Requirement | Có | Ít | Phân tích yêu cầu ban đầu |
| Design | Có | Trung bình | Stitch phác thảo UI Admin |
| Database | Có | Nhiều | Tạo schema, field, EF Migration |
| Coding | Có | Nhiều | Sinh JS files, tích hợp Auth, Booking, Payment API |
| Debug | Có | Nhiều | Fix lỗi EF Core, tham chiếu file |
| Testing | Có | Trung bình | Đề xuất hướng sửa giao diện và API |
| Report | Có | Nhiều | Điền 4 file docs tự động |
| Presentation | Không | | Tự làm slide và quay demo |

---

## 4.4. Bài học rút ra

```text
- Nắm vững kiến trúc ASP.NET Core phân tầng và cách dùng Entity Framework Core.
- Biết cách gọi API với Authentication (JWT Bearer Token) trong JS thuần.
- Kỹ năng debug nhanh khi sử dụng AI (không copy 100%, phải check lại schema, tên biến).
- Biết cách quản lý tài liệu dự án với Markdown rõ ràng.
```

---

## 4.5. Hướng cải thiện tiếp theo

```text
- Tích hợp thêm dịch vụ lưu trữ đám mây (Cloudinary) để upload tài liệu thật.
- Bổ sung thanh toán Momo bên cạnh VNPay.
- Viết thêm Unit Test tự động cho cả Frontend lẫn Backend API.
```

---

# 5. Cam kết cập nhật Changelog

Sinh viên/nhóm cam kết rằng nội dung changelog phản ánh đúng các thay đổi đã thực hiện trong quá trình làm bài tập/project.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
| Ngô Sỹ Giá - DE180117 | 10/07/2026 |
