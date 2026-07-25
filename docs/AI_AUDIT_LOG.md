# AI Audit Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Building Cross-Platform Back-End Application With .NET |
| Mã môn học | PRN232 |
| Lớp | SE18D05 |
| Học kỳ | 8 |
| Tên bài tập / Project | VivuCar |
| Tên sinh viên / Nhóm | Nhóm 4 |
| MSSV / Danh sách MSSV |  |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 16/5/2026 |
| Ngày hoàn thành |  |

---

## 2. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng trong quá trình thực hiện bài tập/project.

- [ x ] ChatGPT
- [ x ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ x ] Antigravity
- [ ] Perplexity
- [ ] Microsoft Copilot
- [ x ] Công cụ khác: Google Stitch

---

## 3. Mục tiêu sử dụng AI

Mô tả ngắn gọn sinh viên/nhóm đã sử dụng AI để hỗ trợ những công việc nào.

Ví dụ:

- Phân tích yêu cầu bài toán
- Gợi ý ý tưởng giải pháp
- Thiết kế database
- Thiết kế giao diện
- Viết code mẫu
- Debug lỗi
- Tối ưu code
- Viết test case
- Kiểm tra bảo mật
- Viết báo cáo
- Chuẩn bị slide thuyết trình
- Tìm hiểu công nghệ mới

### Mô tả mục tiêu sử dụng AI

```text
Nhóm sử dụng AI để hỗ trợ tăng tốc quá trình phát triển hệ thống, đặc biệt trong việc Phân tích yêu cầu bài toán, tối ưu logic xử lý, debug lỗi, thiết kế giao diện UI/UX, xây dựng cấu trúc database và tìm hiểu các công nghệ mới phục vụ cho dự án.
```

## 4. Nhật ký sử dụng AI chi tiết

> Mỗi lần sử dụng AI cho một phần quan trọng của bài tập/project, sinh viên cần ghi lại theo mẫu bên dưới.  
> Sinh viên/nhóm có thể nhân bản mẫu “Lần sử dụng AI” nhiều lần tùy theo số lần sử dụng AI thực tế.

---

### Lần sử dụng AI số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 16/5/2026 |
| MSSV | DE180117 |
| Công cụ AI | Stitch |
| Mục đích sử dụng | Thiết kế layout UI cho trang của Admin |
| Phần việc liên quan | Frontend |
| Mức độ sử dụng | Hỗ trợ ý tưởng |

#### 4.1. Prompt đã sử dụng

```text
Bạn là Senior Frontend Developer chuyên thiết kế Admin Dashboard bằng HTML5 và Tailwind CSS cho hệ thống thuê xe. Hãy tạo giao diện quản trị hiện đại, responsive theo phong cách SaaS dashboard chuyên nghiệp như Uber Admin hoặc Grab Merchant. Hệ thống bao gồm các chức năng: quản lý danh mục phương tiện với CRUD xe, upload và preview hình ảnh, filter, pagination và block/unblock xe; dashboard thống kê doanh thu với biểu đồ trực quan và chức năng export Excel/PDF; quản lý voucher với tạo mã giảm giá, thiết lập điều kiện áp dụng và theo dõi trạng thái, hiệu suất voucher. Giao diện cần có sidebar, navbar, cards, tables, modal confirm, status badge, search/filter toolbar và sử dụng mock data thực tế. Xuất code bằng HTML5 kết hợp Tailwind CSS CDN, code sạch, có comment, hỗ trợ responsive cho mobile, tablet và desktop, sử dụng Heroicons hoặc FontAwesome.
```

#### 4.2. Kết quả AI gợi ý

Tóm tắt nội dung AI đã trả lời hoặc gợi ý.

```text
Tạo ra 5 màn hình của Admin cho các trang quản lý, thông kê
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Mô tả rõ phần nào được sử dụng lại từ gợi ý của AI.

```text
Nhóm sử dụng bố cục tổng thể của dashboard Admin gồm sidebar, navbar, cards,
bảng dữ liệu, bộ lọc, phân trang, status badge và modal xác nhận làm định hướng
cho giao diện quản trị.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Mô tả sinh viên/nhóm đã thay đổi, kiểm tra, sửa lỗi hoặc cải tiến gì so với gợi ý ban đầu của AI.

```text
Nhóm rà soát lại các màn hình do Stitch gợi ý, chọn các thành phần phù hợp với
nghiệp vụ VivuCar và tiếp tục điều chỉnh nội dung, dữ liệu mẫu, luồng thao tác
theo yêu cầu thực tế của dự án.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Chưa cập nhật |
| File liên quan | Giao diện Admin |
| Screenshot | Chưa cập nhật |
| Kết quả chạy/test | Đã rà soát layout và khả năng hiển thị responsive |
| Link video demo |  |
| Ghi chú khác | Người thực hiện: Ngô Sỹ Giá - DE180117 |

#### 4.6. Nhận xét cá nhân/nhóm

Sinh viên/nhóm học được gì sau lần sử dụng AI này?

```text
AI hỗ trợ tốt ở bước phác thảo giao diện và giúp nhóm hình dung nhanh các màn
hình quản trị cần có. Tuy nhiên, nhóm vẫn phải đối chiếu với nghiệp vụ thực tế,
chọn lọc thành phần phù hợp và tự hoàn thiện luồng sử dụng.
```

---

### Lần sử dụng AI số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 01/06/2026 |
| MSSV | DE180117 |
| Công cụ AI | ChatGPT |
| Mục đích sử dụng | Khởi tạo cấu trúc backend ASP.NET Core Web API và frontend Razor Pages |
| Phần việc liên quan | Backend / Frontend / Debug |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
Thiết lập dự án VivuCar với backend sử dụng ASP.NET Core Web API và frontend
sử dụng Razor Pages. Backend gồm các project API, BusinessObjects, DataAccess,
Repositories và Services. Cài đặt OData, Swagger, Entity Framework Core,
SQL Server provider và migration support. Thiết lập project reference đúng
chiều phụ thuộc. Tạo thêm file cần thiết nhưng không xóa file đang có sẵn.
```

#### 4.2. Kết quả AI gợi ý

```text
AI tạo hai solution .NET 8 cho backend và frontend; cài đặt package cần thiết;
thiết lập project reference; cấu hình Swagger, OData, Entity Framework Core,
SQL Server; tạo VivuCarDbContext và API proxy để Razor Pages gọi backend.
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Nhóm sử dụng cấu trúc phân tầng backend, Razor Pages frontend, cấu hình DI,
Swagger, OData, DbContext SQL Server, API proxy và .gitignore cho bin/obj.
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Nhóm build và kiểm tra runtime sau khi tạo project. Khi backend phát sinh lỗi
compile-time liên quan đến UseSqlServer, nhóm chuyển phần đăng ký DbContext về
tầng DataAccess để đúng trách nhiệm module. Nhóm cũng đối chiếu launch profile
và sửa URL backend của frontend thành https://localhost:7005/.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Chưa tạo commit |
| File liên quan | `VivuCarServer/`, `VivuCarClient/`, `.gitignore` |
| Screenshot |  |
| Kết quả chạy/test | Build backend và frontend thành công: 0 warning, 0 error. Swagger API và Razor Pages trả HTTP 200. |
| Link video demo |  |
| Ghi chú khác | Người thực hiện: Ngô Sỹ Giá - DE180117 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp giảm thời gian dựng skeleton dự án nhưng kết quả vẫn cần được kiểm tra
bằng build và smoke test. Nhóm nhận thấy dependency graph, vị trí đăng ký DI và
cổng chạy local phải được rà soát trước khi tiếp tục phát triển chức năng.
```

---

### Lần sử dụng AI số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 07/07/2026 |
| MSSV | DE180117 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Tích hợp Backend API (Booking, Payment, Auth) vào Frontend HTML/JS; sửa DB thêm GPLX 2 mặt |
| Phần việc liên quan | Database / Frontend / Backend / Debug |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
xem backend đã có gì rồi để tích hợp lên giao diện thì làm luôn một thể.
hiện tôi đang muốn nếu người dùng đó đã có ảnh bằng lái trong profile rồi thì
lấy ảnh bằng lái trong profile. còn nếu chưa có thì trong trang đặt xe thêm cái
upload ảnh gplx lên 2 mặt để người dùng khỏi phải qua trang profile upload
```

#### 4.2. Kết quả AI gợi ý

Tóm tắt nội dung AI đã trả lời hoặc gợi ý.

```text
AI phân tích toàn bộ Backend (Controllers, Services, DTOs), xác định API
cần tích hợp, sau đó thực hiện:
- Sửa DB Models (DriverDocument.cs, BookingDriverInfo.cs) thêm DriverLicenseFrontImageUrl/BackImageUrl
- Cập nhật Configuration và DTO tương ứng
- Tạo EF Core Migration AddDriverLicenseBackImage và update DB
- Sửa auth.js: thêm fetchWithAuth() gọi API Login thật thay Mock
- Sửa booking-checkout.js: gọi price-preview, check-availability API và auto-fill GPLX từ profile
- Sửa my-bookings.js, booking-detail.js: gọi API danh sách và chi tiết đơn
- Sửa payment-deposit.js: tạo request VNPay qua API thật
- Sửa payment-result.js: kiểm tra trạng thái thanh toán qua API
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

Mô tả rõ phần nào được sử dụng lại từ gợi ý của AI.

```text
Nhóm áp dụng toàn bộ logic tích hợp API mà AI tạo ra, bao gồm:
- Hàm fetchWithAuth() dùng chung để đính kèm JWT Bearer Token
- Luồng kiểm tra GPLX trong profile trước khi hiển thị form Upload
- Cấu trúc payload gửi lên POST /api/bookings với DriverInfo đầy đủ 2 ảnh GPLX
- Migration DB AddDriverLicenseBackImage chạy thành công
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

Mô tả sinh viên/nhóm đã thay đổi, kiểm tra, sửa lỗi hoặc cải tiến gì so với gợi ý ban đầu của AI.

```text
Nhóm rà soát lại tên field DTO (DriverLicenseFrontImageUrl vs DriverLicenseImageUrl)
và kiểm tra toàn bộ file BookingService.cs có còn tham chiếu tên cũ không.
Nhóm build Backend để đảm bảo Migration không lỗi compile trước khi apply.
Nhóm xác nhận lại API endpoint URL (/api/bookings/check-availability) khớp với
cấu hình Route trong BookingsController.cs.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Chưa tạo commit |
| File liên quan | `auth.js`, `booking-checkout.js`, `my-bookings.js`, `booking-detail.js`, `payment-deposit.js`, `payment-result.js`, `DriverDocument.cs`, `BookingDriverInfo.cs` |
| Screenshot |  |
| Kết quả chạy/test | Migration AddDriverLicenseBackImage: Build succeeded. Backend Build: 0 error. |
| Link video demo |  |
| Ghi chú khác | Người thực hiện: Ngô Sỹ Giá - DE180117 |

#### 4.6. Nhận xét cá nhân/nhóm

Sinh viên/nhóm học được gì sau lần sử dụng AI này?

```text
AI giúp tăng tốc đáng kể việc tích hợp API phức tạp (nhiều endpoint, nhiều file).
Nhóm nhận ra tầm quan trọng của việc đối chiếu tên field giữa DB Model, DTO và
Frontend payload trước khi chạy Migration. Việc để AI tự scan grep toàn bộ
codebase để tìm tham chiếu cũ là rất hiệu quả cho việc refactor an toàn.
```

---

### Lần sử dụng AI số 4

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 20/07/2026 |
| MSSV | DE180117 |
| Công cụ AI | Antigravity |
| Mục đích sử dụng | Pull code từ dev, debug lỗi DI + migration, push code lên nhánh |
| Phần việc liên quan | Backend / Debug |
| Mức độ sử dụng | Hỗ trợ nhiều |

#### 4.1. Prompt đã sử dụng

```text
do có code mới trên git nhánh develop hãy pull về cho tôi nếu có lỗi hãy báo
giờ cần làm gì tiếp theo để chạy được project
```

#### 4.2. Kết quả AI gợi ý

```text
AI thực hiện toàn bộ:
- Fetch và merge origin/dev vào feature/de180117-booking
- Phát hiện PayOS chưa được đăng ký vào DI container → fix ServiceConfiguration.cs
- Phát hiện UserSeedHostedService dùng column schema cũ (BlockedReason, CarTypeId, Color, Year) → remove khỏi AuthenticationConfiguration.cs
- Drop và recreate database sạch để sync toàn bộ 6 migration từ dev
- Xác nhận API khởi động thành công tại http://localhost:5119
- Commit và push code lên feature/de180117-booking theo đúng convention
```

#### 4.3. Phần sinh viên/nhóm đã sử dụng từ AI

```text
Áp dụng toàn bộ fix đề xuất của AI:
- Thêm đăng ký PayOS singleton vào ServiceConfiguration.cs
- Gỡ bỏ UserSeedHostedService khỏi AuthenticationConfiguration.cs
- Chạy drop database + dotnet ef database update để reset DB sạch
```

#### 4.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Sinh viên xác nhận lại API đang chạy (log "Now listening on: http://localhost:5119"
và "Application started") trước khi commit. Sinh viên tự quyết định drop DB
thay vì cố patch từng migration bị lệch schema.
```

#### 4.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | bdd1248 — feature/de180117-booking |
| File liên quan | `ServiceConfiguration.cs`, `AuthenticationConfiguration.cs`, `Pages/Booking/`, `Pages/Payment/`, `Pages/Shared/_UserLayout.cshtml`, `Migrations/20260711084739_AddPayOSProvider.*` |
| Screenshot | API log: `Application started. Now listening on: http://localhost:5119` |
| Kết quả chạy/test | dotnet build: 0 errors. dotnet run: API started. Seed 181 xe thành công. |
| Link video demo |  |
| Ghi chú khác | Người thực hiện: Ngô Sỹ Giá - DE180117 |

#### 4.6. Nhận xét cá nhân/nhóm

```text
AI giúp chẩn đoán nhanh lỗi DI (PayOS chưa register) và schema drift (UserSeedHostedService
dùng column cũ) sau khi merge từ dev. Việc drop và recreate DB là giải pháp
sạch nhất khi migration bị conflict do nhánh song song. Cần chú ý đồng bộ
DI registration khi thêm service mới có dependency bên ngoài (như PayOS SDK).
```

---

### Lần sử dụng AI số 5: Tích hợp Cloudinary Storage Service

#### 5.1. Mô tả vấn đề hoặc yêu cầu

```text
Chức năng upload ảnh thực tế (đặc biệt là Bằng lái xe) trong quá trình Đặt xe
chưa được hoàn thiện, vẫn đang lưu tạm trong mảng mock. Cần tích hợp
thư viện CloudinaryDotNet vào Backend để thay thế cơ chế Local File Upload.
```

#### 5.2. Các prompt đã sử dụng

```text
"Cloudinary__CloudName=dtm5a4bwr Cloudinary__ApiKey=826725167493146
Cloudinary__ApiSecret=MZ_VzHZ0nwKPDaruPDRRKFf8ccI
Cloudinary__SignedUrlExpiresInSeconds=300
Cloudinary__ResourceType=image Cloudinary__DeliveryType=upload
Cloudinary__FolderName=Vivucar Cloudinary__DevelopmentMediaRoot=./uploads
bổ sung cái cloudary này vào file env và apply chỗ upload ảnh cho tôi"
```

#### 5.3. Kết quả do AI sinh ra

```text
- AI đề xuất bản Kế hoạch (Implementation Plan) gồm 5 bước: Cài package, 
tạo CloudinaryStorageService, đổi DI, tạo UploadsController, và update Frontend.
- Code sinh ra cho `CloudinaryStorageService.cs` có đầy đủ validation file size (< 5MB),
loại file (JPG/PNG/WEBP) và dùng `CloudinaryDotNet` SDK để đẩy lên thư mục `Vivucar/`.
- Code sinh ra cho `booking-checkout.js` sửa đổi event change của input file,
dùng `fetchWithAuth` để post `FormData` lên `/api/uploads`.
```

#### 5.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Sinh viên rà soát file `.env` xác nhận các Config Key của Cloudinary đã được
khai báo đúng như yêu cầu của SDK. Cho phép AI tự động thực thi các file thay vì code tay.
```

#### 5.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | 33091bd — feature/de180117-booking |
| File liên quan | `API.csproj`, `CloudinaryStorageService.cs`, `ServiceConfiguration.cs`, `UploadsController.cs`, `booking-checkout.js` |
| Screenshot | Đã đẩy file lên thành công qua API |
| Kết quả chạy/test | dotnet build: 0 errors. Chạy UI upload GPLX -> response trả về url res.cloudinary.com |
| Link video demo |  |
| Ghi chú khác | Người thực hiện: Ngô Sỹ Giá - DE180117 |

#### 5.6. Nhận xét cá nhân/nhóm

```text
AI hiểu nhanh ngữ cảnh kiến trúc Repository/Service/Controller hiện tại để
implement IFileStorageService mà không phá vỡ logic cũ. Chuyển đổi Local Storage
sang Cloud Storage trơn tru. Quản lý tác vụ rất tốt với Implementation Plan và Tasks.
```

---

### Lần sử dụng AI số 6: Xây dựng trang User Profile & Sửa lỗi Upload Cloudinary

#### 6.1. Mô tả vấn đề hoặc yêu cầu

```text
Người dùng cần tạo giao diện Profile, hiển thị Avatar, Dropdown menu, Form cập nhật thông tin cá nhân và upload GPLX 2 mặt. Cần tích hợp nút Hủy chờ duyệt và sửa các lỗi liên quan đến proxy API khi upload ảnh lên Cloudinary.
```

#### 6.2. Các prompt đã sử dụng

```text
- ở trang profile thì còn đang lỗi font chữ khá nhiều, phần hồ sơ chưa load được data hiện tại...
- thêm nút hủy chờ duyệt để có thể upload lại ảnh khác
- cả hôm nay làm từ đầu phần user profile bạn tìm những file cần điền và điền cho đúng
```

#### 6.3. Kết quả do AI sinh ra

```text
- Sinh code UI cho Profile.cshtml và logic Javascript trong profile.js
- Sửa cấu hình CORS trong Program.cs và loại bỏ proxy trong fetch để giải quyết lỗi 400 Bad Request
- Thêm cache-buster v=3 vào html để khắc phục lỗi trình duyệt cache file cũ (gây ra lỗi lưu chữ "undefined" vào DB)
- Bổ sung nút "Hủy chờ duyệt" ở Frontend và API endpoint my/cancel tương ứng ở Backend
- Sửa lỗi Enum DocumentVerificationStatus thiếu trạng thái Unverified = 0 gây lỗi build
```

#### 6.4. Phần sinh viên/nhóm tự chỉnh sửa hoặc cải tiến

```text
Sinh viên tự chạy lại lệnh dotnet run, khởi động lại Backend/Frontend và hard-reload trình duyệt (Ctrl + F5) theo hướng dẫn của AI để cập nhật giao diện và API mới nhất.
```

#### 6.5. Minh chứng

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Chưa tạo commit |
| File liên quan | `Profile.cshtml`, `profile.js`, `UploadsController.cs`, `DriverDocumentsController.cs`, `Program.cs`, `UserService.cs`, `DocumentVerificationStatus.cs` |
| Screenshot | Đã Hủy duyệt và Upload thành công không lỗi undefined |
| Kết quả chạy/test | Trang Profile hoạt động tốt, Upload thành công không lỗi undefined. |
| Link video demo |  |
| Ghi chú khác | Người thực hiện: Ngô Sỹ Giá - DE180117 |

#### 6.6. Nhận xét cá nhân/nhóm

```text
AI giải quyết dứt điểm các lỗi khó liên quan đến cơ chế cache của trình duyệt và Model Binding của HttpClient proxy rất xuất sắc. Xử lý UI tốt và đồng bộ hoàn hảo với logic Backend C#.
```

---

## 5. Bảng tổng hợp mức độ sử dụng AI

Đánh dấu mức độ AI hỗ trợ ở từng hạng mục.

| Hạng mục | Không dùng AI | AI hỗ trợ ít | AI hỗ trợ nhiều | AI sinh chính | Ghi chú |
|---|:---:|:---:|:---:|:---:|---|
| Phân tích yêu cầu |  | x |  |  | Nhóm phân tích spec, AI hỗ trợ đối chiếu DB schema |
| Viết user story/use case | x |  |  |  |  |
| Thiết kế database |  |  | x |  | AI hỗ trợ tạo schema ban đầu và migration |
| Thiết kế kiến trúc hệ thống |  |  | x |  | AI hỗ trợ cấu trúc phân tầng ASP.NET Core |
| Thiết kế giao diện |  |  |  | x | Stitch phác thảo UI Admin; AI sinh HTML/CSS/JS |
| Code frontend |  |  |  | x | AI sinh 56 file JS, HTML pages và CSS |
| Code backend |  |  |  | x | AI sinh Controllers, Services, Repositories, DTOs |
| Debug lỗi |  |  | x |  | AI hỗ trợ tìm lỗi dependency và tham chiếu field sai |
| Viết test case |  | x |  |  | AI hỗ trợ viết BookingServiceTests.cs |
| Kiểm thử sản phẩm |  | x |  |  | Nhóm tự chạy build và smoke test |
| Tối ưu code |  | x |  |  |  |
| Viết báo cáo |  | x |  |  | AI hỗ trợ điền doc |
| Làm slide thuyết trình | x |  |  |  |  |

---

## 6. Các lỗi hoặc hạn chế từ AI

Ghi lại các trường hợp AI trả lời sai, thiếu, chưa phù hợp hoặc sinh code không chạy.

| STT | Lỗi/hạn chế từ AI | Cách phát hiện | Cách xử lý/cải tiến |
|---:|---|---|---|
| 1 | AI đặt UseSqlServer ở sai tầng API | Build thất bại: namespace không khả dụng | Chuyển đăng ký DbContext về DataAccess |
| 2 | AI dùng tên field DriverLicenseImageUrl trong BookingService nhưng Model đã đổi thành DriverLicenseFrontImageUrl | grep search phát hiện 3 chỗ tham chiếu cũ | Cập nhật toàn bộ tham chiếu trong BookingService.cs |
| 3 | Backend chưa có API GET /api/bookings/owner-requests (lấy đơn theo Chủ xe) | Rà soát IBookingService interface | Tạm thời giữ Mock Data cho trang owner-booking-requests; ghi nhận Open Issue để BE bổ sung sau |

---

## 7. Kiểm chứng kết quả AI

Mô tả cách sinh viên/nhóm kiểm tra lại kết quả do AI gợi ý.

Có thể bao gồm:

- Chạy thử chương trình
- Viết test case
- So sánh với yêu cầu đề bài
- Kiểm tra output
- Đối chiếu tài liệu môn học
- Hỏi lại giảng viên
- Review cùng thành viên nhóm
- Kiểm tra lỗi bảo mật
- Kiểm tra bằng dữ liệu mẫu
- So sánh trước và sau khi dùng AI

### Nội dung kiểm chứng

```text
Nhóm build backend bằng dotnet ef migrations add để kiểm tra lỗi compile-time
trước khi update DB. Sau khi migration thành công, nhóm kiểm tra log output
"Build succeeded. Done. To undo this action, use ef migrations remove".
Nhóm sử dụng grep search để đảm bảo không còn tham chiếu tên field cũ
(DriverLicenseImageUrl) trong codebase. Kết quả Backend build: 0 error.
```

---

## 8. Đóng góp cá nhân hoặc đóng góp nhóm

### 8.1. Đối với bài cá nhân

Mô tả phần sinh viên tự làm, phần AI hỗ trợ và phần đã tự cải tiến.

```text
Viết tại đây...
```

### 8.2. Đối với bài nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
|  |  |  | Có / Không |  |
|  |  |  | Có / Không |  |
|  |  |  | Có / Không |  |
|  |  |  | Có / Không |  |

---

## 9. Reflection cuối bài

### 9.1. AI đã hỗ trợ em/nhóm ở điểm nào?

```text
Viết tại đây...
```

### 9.2. Phần nào em/nhóm không sử dụng theo gợi ý của AI? Vì sao?

```text
Viết tại đây...
```

### 9.3. Em/nhóm đã kiểm tra tính đúng đắn của kết quả AI như thế nào?

```text
Viết tại đây...
```

### 9.4. Nếu không có AI, phần nào sẽ khó khăn nhất?

```text
Viết tại đây...
```

### 9.5. Sau bài tập/project này, em/nhóm học được gì về môn học?

```text
Viết tại đây...
```

### 9.6. Sau bài tập/project này, em/nhóm học được gì về cách sử dụng AI có trách nhiệm?

```text
Viết tại đây...
```

---

## 10. Cam kết học thuật

Sinh viên/nhóm cam kết rằng:

- Nội dung AI hỗ trợ đã được ghi nhận trung thực.
- Không nộp nguyên văn kết quả AI mà không kiểm tra.
- Có khả năng giải thích các phần đã nộp.
- Chịu trách nhiệm về tính đúng đắn của sản phẩm cuối cùng.
- Hiểu rằng việc sử dụng AI không khai báo có thể ảnh hưởng đến kết quả đánh giá.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|  |  |
 
 - - -  
  
 # # #   L � � � n   s � � �   d � � � n g   A I   s � �    7 :   H o � � n   t h i � � ! n   l u � �  n g   � � � � � t   x e   ( B o o k i n g ) ,   T � � c h   h � � � p   P a y O S   v � �   K � �   h � � � p   �  � �  n g   t r � � � c   t u y � � � n  
  
 # # # #   7 . 1 .   M � �   t � � �   v � � � n   �  � � �   h o � � � c   y � � u   c � � � u  
  
 ` ` ` t e x t  
 H o � � n   t h i � � ! n   l u � �  n g   B o o k i n g   t � � �   b � � � � : c   t � � � o   R e q u e s t ,   k i � � �m   t r a   �  i � � � u   k i � � ! n   t � � i   l i � � ! u   G P L X ,   t h a n h   t o � � n   c � � � c   q u a   P a y O S   v � �   c u � �  i   c � � n g   l � �   K � �   h � � � p   �  � �  n g   c h � � �   k � �   s � �    ( C a n v a s ) .   � � � �  n g   t h � � � i   y � � u   c � � � u   t � � � o   s c r i p t   I n t e g r a t i o n   T e s t   c h � � � y   q u a   A P I   t h � � � t   ( k h � � n g   d � � n g   t r � � n h   d u y � � ! t   t � � �   �  � � "!n g )   �  � � �  v e r i f y   t o � � n   b � � "!  l u � �  n g .  
 ` ` `  
  
 # # # #   7 . 2 .   C � � c   p r o m p t   �  � �   s � � �   d � � � n g  
  
 ` ` ` t e x t  
 -   g i � � �   b o o k i n g   �  a n g   c � � � n   � � x  c h � �    l � � n   l � � 9 c h   b o o k i n g   u p l o a d   � � � n h   v � �   �  i � � � n   c � � c   t h � � n g   t i n . . .  
 -   g i � � �   c � �   m � � "!t   c h � � t   t h � � n g   t i n   l u � �  n g   �  i   n h � �   s a u :   t � � � o   r e q u e s t   b o o k i n g   - >   d u y � � ! t   G P L X   - >   c � � � c   P a y O S   - >   k � �   h � � � p   �  � �  n g .  
 -   t e s t   v � �   d e b u g   c h o   t � � i   �  i   v � �   � � x  c h � �    b o o k i n g . . .  
 -   t e s t   x e m   p a y o s   �  � �   h o � � � t   �  � � "!n g   c � � � c   �  � � � � � c   c h � � a   � � x  g i a o   d i � � ! n   t h a n h   t o � � n   c � � � c . . .  
 -   T a s k :   R u n t i m e   I n t e g r a t i o n   T e s t   c h o   l u � �  n g   B o o k i n g . . .  
 ` ` `  
  
 # # # #   7 . 3 .   K � � � t   q u � � �   d o   A I   s i n h   r a  
  
 ` ` ` t e x t  
 -   C � � � p   n h � � � t   l o g i c   B a c k e n d   ( B o o k i n g S e r v i c e ) :   T � � �   �  � � "!n g   �  � � � t   t r � � � n g   t h � � i   W a i t i n g D e p o s i t   n � � � u   t � � i   l i � � ! u   G P L X   �  � �   A p p r o v e d .  
 -   T � � c h   h � � � p   t � � n h   n � �n g   k � �   h � � � p   �  � �  n g   b � � � n g   t h � � �   < c a n v a s > ,   l � � u   U R L   c h � � �   k � �   s � �  .  
 -   T � � � o   e n d p o i n t   A P I   P O S T   / c o n t r a c t / s i g n   �  � � �  l � � u   v � � � t   h � � � p   �  � �  n g .  
 -   T � � � o   P o w e r S h e l l   s c r i p t   c h � � � y   I n t e g r a t i o n   T e s t   1 0   b � � � � : c   q u a   c � � c   A P I   t h � � � c   t � � �   c � � � a   h � � !   t h � �  n g .  
 -   X � � �   l � �   l � �  i   4 0 0   B a d   R e q u e s t   c � � � a   P a y O S   b � � � n g   c � � c h   c � � � t   �  � � "!  d � � i   c h u � �  i   d e s c r i p t i o n   t � �  i   �  a   2 5   k � �   t � � � .  
 ` ` `  
  
 # # # #   7 . 4 .   P h � � � n   s i n h   v i � � n / n h � � m   t � � �   c h � � 0 n h   s � � � a   h o � � � c   c � � � i   t i � � � n  
  
 ` ` ` t e x t  
 S i n h   v i � � n   �  � �  i   c h i � � � u   l � � � i   c � � � u   t r � � c   D a t a b a s e   h i � � ! n   t � � � i   �  � � �  � � n h   x � � �   �  � � n g   D o c u m e n t V e r i f i c a t i o n S t a t u s   ( A p p r o v e d   =   2 ) ,   t � � �   c h � � � y   c � � c   H T T P   R e q u e s t   �  � � �  b � � � t   n g u y � � n   n h � � n   l � �  i   P a y O S   4 0 0   v � �   r � �   s o � � t   l � � � i   c � � c   U I   c o m p o n e n t .  
 ` ` `  
  
 # # # #   7 . 5 .   M i n h   c h � � � n g  
  
 |   L o � � � i   m i n h   c h � � � n g   |   N � � "!i   d u n g   |  
 | - - - | - - - |  
 |   L i n k   c o m m i t   |   C h � � a   c � � � p   n h � � � t   |  
 |   F i l e   l i � � n   q u a n   |   ` B o o k i n g S e r v i c e . c s ` ,   ` P a y m e n t S e r v i c e . c s ` ,   ` C o n t r a c t . c s h t m l ` ,   ` b o o k i n g - c o n t r a c t . j s ` ,   ` p a y m e n t - d e p o s i t . j s `   |  
 |   S c r e e n s h o t   |   S c r i p t   t e s t   R u n t i m e   P a s s   1 0 0 %   |  
 |   K � � � t   q u � � �   c h � � � y / t e s t   |   I n t e g r a t i o n   t e s t   ( P o w e r S h e l l   s c r i p t )   c h � � � y   t h � � n h   c � � n g   t o � � n   b � � "!  f l o w   t � � � o   �  � � n   - >   P a y O S   - >   k � �   h � � � p   �  � �  n g   |  
 |   L i n k   v i d e o   d e m o   |     |  
 |   G h i   c h � �   k h � � c   |   N g � � � � � i   t h � � � c   h i � � ! n :   N g � �   S � � �   G i � �   -   D E 1 8 0 1 1 7   |  
  
 # # # #   7 . 6 .   N h � � � n   x � � t   c � �   n h � � n / n h � � m  
  
 ` ` ` t e x t  
 V i � � ! c   s � � �   d � � � n g   P o w e r S h e l l   s c r i p t   �  � � �  m o c k   A P I   t e s t i n g   l � �   p h � � � � n g   p h � � p   r � � � t   h i � � ! u   q u � � �   �  � � �  k i � � �m   t r a   c � � c   l u � �  n g   n g h i � � ! p   v � � �   d � � i   m � �   k h � � n g   c � � � n   c h � � �   t � � c h   h � � � p   �  � � � y   �  � � �   g i a o   d i � � ! n   h o � � � c   c � � n g   c � � �   a u t o m a t i o n   c � �  n g   k � � � n h .   T � � m   r a   g i � � : i   h � � � n   2 5   k � �   t � � �   c � � � a   P a y O S   n h � � �   p h � � n   t � � c h   r e s p o n s e   l o g   c h i   t i � � � t .  
 ` ` `  
 
