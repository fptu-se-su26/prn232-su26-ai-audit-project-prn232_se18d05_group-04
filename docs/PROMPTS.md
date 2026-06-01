# Prompt Log

## 1. Thông tin chung

| Thông tin | Nội dung |
|---|---|
| Môn học | Building Cross-Platform Back-End Application With .NET |
| Mã môn học | PRN232 |
| Lớp | SE18D05 |
| Học kỳ | 8 |
| Tên bài tập / Project | VivuCar |
| Tên sinh viên / Nhóm | Nhóm 4 |
| MSSV / Danh sách MSSV | DE180116 |
| Giảng viên hướng dẫn | QuangLTN3 |
| Ngày bắt đầu | 16/05/2026 |
| Ngày cập nhật gần nhất | 01/06/2026 |

---

## 2. Mục đích của file Prompt Log

File này dùng để ghi lại các prompt quan trọng đã sử dụng trong quá trình thực hiện bài tập, lab, assignment hoặc project.

Sinh viên/nhóm cần ghi lại:

- Đã hỏi AI điều gì.
- Mục đích sử dụng prompt.
- Công cụ AI đã sử dụng.
- AI đã trả lời hoặc gợi ý gì.
- Kết quả đó có được áp dụng vào bài hay không.
- Sinh viên/nhóm đã kiểm tra, chỉnh sửa hoặc cải tiến gì sau khi nhận kết quả từ AI.

---

## 3. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng.

- [ x ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ x ] Công cụ khác: Stitch

---

## 4. Bảng tổng hợp prompt đã sử dụng

| STT | Ngày | Công cụ AI | Mục đích | Prompt tóm tắt | Kết quả chính | Có sử dụng vào bài không? | Minh chứng |
|---:|---|---|---|---|---|---|---|
| 1 | 16/05/2026 | Stitch | Thiết kế layout UI Admin | Tạo dashboard quản trị responsive cho hệ thống thuê xe | Gợi ý 5 màn hình Admin | Có | `AI_AUDIT_LOG.md` - Lần sử dụng AI số 1 |
| 2 | 01/06/2026 | ChatGPT | Khởi tạo cấu trúc dự án | Tạo backend Web API và frontend Razor Pages | Tạo skeleton dự án, cấu hình dependency và kiểm tra build | Có | `AI_AUDIT_LOG.md` - Lần sử dụng AI số 2 |
| 3 |  |  |  |  |  | Có / Không |  |
| 4 |  |  |  |  |  | Có / Không |  |
| 5 |  |  |  |  |  | Có / Không |  |
| 6 |  |  |  |  |  | Có / Không |  |
| 7 |  |  |  |  |  | Có / Không |  |
| 8 |  |  |  |  |  | Có / Không |  |
| 9 |  |  |  |  |  | Có / Không |  |
| 10 |  |  |  |  |  | Có / Không |  |

---

## 5. Prompt chi tiết

> Sinh viên/nhóm có thể nhân bản mẫu “Prompt số...” nhiều lần tùy số lượng prompt thực tế đã sử dụng.

---

### Prompt số 1

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 16/05/2026 |
| Công cụ AI | Stitch |
| Mục đích | Thiết kế layout UI cho trang Admin |
| Phần việc liên quan | Design / Coding |
| Mức độ sử dụng | Hỏi ý tưởng / Hỏi sinh code |

#### 5.1. Prompt nguyên văn

```text
Bạn là Senior Frontend Developer chuyên thiết kế Admin Dashboard bằng HTML5 và
Tailwind CSS cho hệ thống thuê xe. Hãy tạo giao diện quản trị hiện đại,
responsive theo phong cách SaaS dashboard chuyên nghiệp như Uber Admin hoặc
Grab Merchant. Hệ thống bao gồm các chức năng: quản lý danh mục phương tiện với
CRUD xe, upload và preview hình ảnh, filter, pagination và block/unblock xe;
dashboard thống kê doanh thu với biểu đồ trực quan và chức năng export
Excel/PDF; quản lý voucher với tạo mã giảm giá, thiết lập điều kiện áp dụng và
theo dõi trạng thái, hiệu suất voucher. Giao diện cần có sidebar, navbar, cards,
tables, modal confirm, status badge, search/filter toolbar và sử dụng mock data
thực tế. Xuất code bằng HTML5 kết hợp Tailwind CSS CDN, code sạch, có comment,
hỗ trợ responsive cho mobile, tablet và desktop, sử dụng Heroicons hoặc
FontAwesome.
```

#### 5.2. Bối cảnh khi viết prompt

Mô tả ngắn gọn vì sao sinh viên/nhóm cần dùng prompt này.

```text
Nhóm cần một bản phác thảo UI Admin để xác định bố cục và các thành phần giao
diện trước khi phát triển frontend chi tiết.
```

#### 5.3. Kết quả AI trả về

Tóm tắt nội dung AI đã trả lời hoặc gợi ý.

```text
Stitch gợi ý 5 màn hình Admin cho các trang quản lý và thống kê, sử dụng bố cục
dashboard responsive với sidebar, navbar, cards, bảng dữ liệu và bộ lọc.
```

#### 5.4. Kết quả đã áp dụng vào bài

Mô tả phần nào từ kết quả AI đã được sử dụng vào bài tập/project.

```text
Nhóm sử dụng bố cục tổng thể và danh sách thành phần giao diện làm định hướng
cho khu vực quản trị VivuCar.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

Mô tả sinh viên/nhóm đã thay đổi, kiểm tra, sửa lỗi hoặc cải tiến gì so với kết quả AI trả về.

```text
Nhóm rà soát lại các thành phần, chọn phần phù hợp với nghiệp vụ VivuCar và
tiếp tục điều chỉnh nội dung, dữ liệu mẫu, luồng thao tác theo yêu cầu thực tế.
```

#### 5.6. Đánh giá chất lượng prompt

Đánh dấu các nhận xét phù hợp.

- [ x ] Prompt rõ ràng
- [ x ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [ x ] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Chưa cập nhật |
| File liên quan | Giao diện Admin |
| Screenshot | Chưa cập nhật |
| Kết quả chạy/test | Đã rà soát layout và khả năng hiển thị responsive |
| Link tài liệu/báo cáo |  |
| Ghi chú khác | Người thực hiện: Nguyễn Minh Tuấn - DE180116 |

#### 5.8. Ghi chú thêm

```text
Kết quả được sử dụng làm định hướng UI, không áp dụng nguyên trạng.
```

---

### Prompt số 2

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng | 01/06/2026 |
| Công cụ AI | ChatGPT |
| Mục đích | Khởi tạo cấu trúc backend ASP.NET Core Web API và frontend Razor Pages |
| Phần việc liên quan | Design / Coding / Testing / Debug |
| Mức độ sử dụng | Hỏi sinh code / Hỏi debug |

#### 5.1. Prompt nguyên văn

```text
Thiết lập dự án VivuCar với backend sử dụng ASP.NET Core Web API và frontend
sử dụng Razor Pages. Backend gồm các project API, BusinessObjects, DataAccess,
Repositories và Services. Cài đặt OData, Swagger, Entity Framework Core,
SQL Server provider và migration support. Thiết lập project reference đúng
chiều phụ thuộc. Tạo thêm file cần thiết nhưng không xóa file đang có sẵn.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Repository chưa có source code ứng dụng. Nhóm cần khởi tạo nền tảng .NET 8
đúng cấu trúc để tiếp tục phát triển backend và frontend.
```

#### 5.3. Kết quả AI trả về

```text
ChatGPT tạo hai solution .NET 8; cài package cần thiết; thiết lập project
reference; cấu hình Swagger, OData, Entity Framework Core, SQL Server; tạo
VivuCarDbContext và API proxy để Razor Pages gọi backend.
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Nhóm sử dụng cấu trúc phân tầng backend, Razor Pages frontend, cấu hình DI,
Swagger, OData, DbContext SQL Server, API proxy và .gitignore cho bin/obj.
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Nhóm build và kiểm tra runtime sau khi tạo project. Khi phát sinh lỗi liên quan
đến UseSqlServer, nhóm chuyển phần đăng ký DbContext về tầng DataAccess. Nhóm
cũng đối chiếu launch profile và sửa URL backend thành https://localhost:7005/.
```

#### 5.6. Đánh giá chất lượng prompt

- [ x ] Prompt rõ ràng
- [ x ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [ x ] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ x ] Cần tự kiểm tra và chỉnh sửa nhiều
- [ x ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit | Chưa tạo commit |
| File liên quan | `VivuCarServer/`, `VivuCarClient/`, `.gitignore` |
| Screenshot |  |
| Kết quả chạy/test | Build backend và frontend thành công: 0 warning, 0 error. Swagger API và Razor Pages trả HTTP 200. |
| Link tài liệu/báo cáo |  |
| Ghi chú khác | Người thực hiện: Nguyễn Minh Tuấn - DE180116 |

#### 5.8. Ghi chú thêm

```text
Kết quả đã được kiểm tra bằng build và smoke test trước khi ghi nhận.
```

---

### Prompt số 3

| Nội dung | Thông tin |
|---|---|
| Ngày sử dụng |  |
| Công cụ AI | ChatGPT / Gemini / Claude / GitHub Copilot / Cursor / Antigravity / Khác |
| Mục đích |  |
| Phần việc liên quan | Requirement / Design / Database / Coding / Testing / Debug / Report / Presentation / Other |
| Mức độ sử dụng | Hỏi ý tưởng / Hỏi giải thích / Hỏi review / Hỏi debug / Hỏi sinh code / Hỏi tối ưu |

#### 5.1. Prompt nguyên văn

```text
Dán nguyên văn prompt đã hỏi AI tại đây.
```

#### 5.2. Bối cảnh khi viết prompt

```text
Viết tại đây...
```

#### 5.3. Kết quả AI trả về

```text
Viết tại đây...
```

#### 5.4. Kết quả đã áp dụng vào bài

```text
Viết tại đây...
```

#### 5.5. Phần sinh viên/nhóm đã chỉnh sửa hoặc cải tiến

```text
Viết tại đây...
```

#### 5.6. Đánh giá chất lượng prompt

- [ ] Prompt rõ ràng
- [ ] Prompt có đủ bối cảnh
- [ ] Prompt còn thiếu thông tin
- [ ] Prompt tạo ra kết quả tốt
- [ ] Prompt tạo ra kết quả chưa phù hợp
- [ ] Cần hỏi lại AI nhiều lần
- [ ] Cần tự kiểm tra và chỉnh sửa nhiều
- [ ] Kết quả AI có lỗi hoặc chưa chính xác

#### 5.7. Minh chứng liên quan

| Loại minh chứng | Nội dung |
|---|---|
| Link commit |  |
| File liên quan |  |
| Screenshot |  |
| Kết quả chạy/test |  |
| Link tài liệu/báo cáo |  |
| Ghi chú khác |  |

#### 5.8. Ghi chú thêm

```text
Viết tại đây...
```

---

## 6. Prompt quan trọng nhất

Chọn một prompt có ảnh hưởng lớn nhất đến bài tập/project.

### 6.1. Prompt được chọn

```text
Dán prompt quan trọng nhất tại đây.
```

### 6.2. Vì sao prompt này quan trọng?

```text
Viết tại đây...
```

### 6.3. Kết quả prompt này mang lại

```text
Viết tại đây...
```

### 6.4. Sinh viên/nhóm đã kiểm tra kết quả như thế nào?

```text
Viết tại đây...
```

### 6.5. Sinh viên/nhóm đã cải tiến gì từ kết quả AI?

```text
Viết tại đây...
```

---

## 7. Prompt chưa hiệu quả

Ghi lại ít nhất một prompt chưa tạo ra kết quả tốt hoặc chưa phù hợp.

### 7.1. Prompt chưa hiệu quả

```text
Dán prompt chưa hiệu quả tại đây.
```

### 7.2. Vì sao prompt này chưa hiệu quả?

```text
Viết tại đây...
```

Gợi ý nguyên nhân:

- Prompt quá ngắn.
- Thiếu bối cảnh bài toán.
- Không nêu rõ yêu cầu đầu ra.
- Không cung cấp ngôn ngữ lập trình/công nghệ đang dùng.
- Không đưa lỗi cụ thể.
- Không đưa ví dụ input/output.
- Không yêu cầu AI giải thích.
- Hỏi AI làm toàn bộ thay vì hỏi từng phần.

### 7.3. Cách cải thiện prompt

```text
Viết tại đây...
```

### 7.4. Prompt sau khi cải tiến

```text
Dán prompt đã được cải tiến tại đây.
```

### 7.5. Kết quả sau khi cải tiến prompt

```text
Viết tại đây...
```

---

## 8. Bài học về cách viết prompt

### 8.1. Khi viết prompt, em/nhóm cần cung cấp thông tin gì để AI trả lời tốt hơn?

```text
Viết tại đây...
```

Gợi ý:

- Mục tiêu cần đạt.
- Bối cảnh bài toán.
- Công nghệ/ngôn ngữ lập trình đang dùng.
- Input/output mong muốn.
- Ràng buộc của đề bài.
- Lỗi đang gặp.
- Format kết quả mong muốn.
- Yêu cầu AI giải thích từng bước.

### 8.2. Em/nhóm đã học được gì về cách đặt câu hỏi cho AI?

```text
Viết tại đây...
```

### 8.3. Lần sau em/nhóm sẽ cải thiện prompt như thế nào?

```text
Viết tại đây...
```

---

## 9. Phân loại prompt đã sử dụng

Đánh dấu số lượng prompt theo từng nhóm.

| Loại prompt | Số lượng | Ví dụ prompt tiêu biểu |
|---|---:|---|
| Prompt phân tích yêu cầu |  |  |
| Prompt giải thích kiến thức |  |  |
| Prompt thiết kế giải pháp |  |  |
| Prompt thiết kế database |  |  |
| Prompt sinh code mẫu |  |  |
| Prompt debug lỗi |  |  |
| Prompt viết test case |  |  |
| Prompt review code |  |  |
| Prompt tối ưu code |  |  |
| Prompt viết báo cáo |  |  |
| Prompt chuẩn bị thuyết trình |  |  |
| Prompt khác |  |  |

---

## 10. Checklist chất lượng prompt

Sinh viên/nhóm tự kiểm tra chất lượng prompt đã dùng.

| Tiêu chí | Đã đạt? | Ghi chú |
|---|:---:|---|
| Prompt có mục tiêu rõ ràng |  |  |
| Prompt có đủ bối cảnh |  |  |
| Prompt có nêu công nghệ/ngôn ngữ sử dụng |  |  |
| Prompt có nêu yêu cầu đầu ra |  |  |
| Prompt không yêu cầu AI làm toàn bộ bài một cách máy móc |  |  |
| Prompt có yêu cầu AI giải thích hoặc phân tích |  |  |
| Kết quả AI được kiểm tra lại |  |  |
| Kết quả AI được chỉnh sửa trước khi sử dụng |  |  |
| Prompt quan trọng được ghi lại đầy đủ |  |  |
| Prompt sai/chưa hiệu quả được rút kinh nghiệm |  |  |

---

## 11. Cam kết sử dụng prompt minh bạch

Sinh viên/nhóm cam kết rằng:

- Các prompt quan trọng đã được ghi lại trung thực.
- Không che giấu việc sử dụng AI trong các phần quan trọng của bài.
- Không nộp nguyên văn kết quả AI nếu chưa kiểm tra và chỉnh sửa.
- Có khả năng giải thích các phần đã sử dụng từ AI.
- Chịu trách nhiệm với sản phẩm cuối cùng.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|  |  |
