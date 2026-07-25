# AI Learning Reflection

## 1. Thông tin chung

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
| Ngày hoàn thành reflection | 20/07/2026 |

---

## 2. Mục đích Reflection

File này dùng để sinh viên/nhóm tự đánh giá quá trình sử dụng AI trong học tập và thực hiện bài tập, lab, assignment hoặc project.

Reflection cần thể hiện:

- AI đã hỗ trợ gì trong quá trình học.
- Sinh viên/nhóm đã kiểm chứng kết quả AI như thế nào.
- Sinh viên/nhóm đã tự chỉnh sửa, cải tiến ra sao.
- Sinh viên/nhóm học được gì về môn học.
- Sinh viên/nhóm học được gì về cách sử dụng AI minh bạch và có trách nhiệm.

---

## 3. Tóm tắt quá trình sử dụng AI

Mô tả ngắn gọn quá trình sử dụng AI trong bài tập/project này.

```text
Nhóm đã sử dụng Stitch để phác thảo layout UI Admin, ChatGPT để khởi tạo
nền tảng backend ASP.NET Core Web API cùng frontend Razor Pages, và Antigravity
để tích hợp toàn bộ Backend API (Auth, Booking, Payment, Upload Cloudinary, User Profile, Reviews, AI Chatbot Gemini) 
vào Frontend HTML/JS, cũng như sửa DB. Kết quả AI được dùng làm điểm khởi đầu,
sau đó nhóm rà soát, build kiểm tra và xác nhận runtime trước khi áp dụng.
```

Gợi ý:

- Em/nhóm đã dùng AI ở giai đoạn nào?
- Dùng AI để hỗ trợ việc gì?
- Công cụ AI nào được sử dụng nhiều nhất?
- AI có giúp cải thiện chất lượng bài làm không?
- Có phần nào AI gợi ý nhưng em/nhóm không sử dụng không?

---

## 4. Công cụ AI đã sử dụng

Đánh dấu các công cụ AI đã sử dụng.

- [ x ] ChatGPT
- [ ] Gemini
- [ ] Claude
- [ ] GitHub Copilot
- [ ] Cursor
- [ x ] Antigravity
- [ ] Microsoft Copilot
- [ ] Perplexity
- [ x ] Công cụ khác: Stitch

### Công cụ được sử dụng nhiều nhất

```text
Antígravity được sử dụng nhiều nhất trong giai đoạn tích hợp API.
```

### Lý do sử dụng công cụ đó

```text
ChatGPT được sử dụng để dựng skeleton dự án, cài đặt dependency và hỗ trợ
debug lỗi build. Stitch được sử dụng ở giai đoạn phác thảo giao diện Admin.
Antigravity được sử dụng để tích hợp API Backend vào Frontend HTML/JS,
viết lại các file JS, sửa DB Model, tạo EF Migration và điền tài liệu.
```

---

## 5. AI đã hỗ trợ em/nhóm ở điểm nào?

Đánh dấu các nội dung phù hợp.

- [ ] Hiểu yêu cầu đề bài
- [ ] Phân tích bài toán
- [ x ] Tìm ý tưởng giải pháp
- [ ] Thiết kế database
- [ x ] Thiết kế giao diện
- [ x ] Thiết kế kiến trúc hệ thống
- [ x ] Viết code mẫu
- [ x ] Debug lỗi
- [ ] Viết test case
- [ x ] Review code
- [ ] Tối ưu code
- [ ] Kiểm tra bảo mật
- [ ] Viết báo cáo
- [ ] Chuẩn bị thuyết trình
- [ x ] Tìm hiểu công nghệ mới
- [ ] Khác: ....................................

### Mô tả chi tiết

```text
Stitch hỗ trợ hình dung nhanh các màn hình quản trị, bố cục dashboard và thành
phần UI cần thiết. ChatGPT hỗ trợ tạo cấu trúc solution, project reference,
Swagger, OData, EF Core, DbContext SQL Server, Razor Pages và API proxy.
```

---

## 6. AI có giúp em/nhóm học tốt hơn không?

### 6.1. Những điểm AI giúp em/nhóm học tốt hơn

```text
AI giúp nhóm hiểu rõ hơn cách tổ chức backend theo tầng và quy trình kiểm tra
một skeleton dự án .NET. Việc xử lý lỗi build giúp nhóm nhận thấy cấu hình DI
cần được đặt ở module sở hữu dependency tương ứng.
```

Gợi ý:

- Hiểu bài nhanh hơn.
- Có thêm ví dụ minh họa.
- Biết cách debug lỗi.
- Biết cách cải thiện báo cáo hoặc slide.

### 6.2. Những điểm AI chưa giúp tốt hoặc gây khó khăn

```text
AI hỗ trợ nhiều nhất ở 3 giai đoạn: (1) Phác thảo UI Admin qua Stitch,
(2) Dựng skeleton Backend + Frontend thông qua ChatGPT, (3) Tích hợp toàn bộ
API và sửa DB qua Antigravity. AI giúp tiết kiệm thời gian phát triển đáng kể.

Kết quả AI ban đầu chưa hoàn toàn phù hợp: đăng ký DbContext đặt ở API khiến
extension UseSqlServer không khả dụng tại compile time. URL backend frontend
cũng chưa khớp launch profile. Nhóm phải tự build, kiểm tra và chỉnh sửa.
```

Gợi ý:

- AI trả lời sai.
- AI sinh code không chạy.
- AI hiểu sai yêu cầu đề bài.
- AI đưa giải pháp quá phức tạp.
- AI thiếu ngữ cảnh môn học.
- AI trả lời chung chung.
- AI khiến em/nhóm dễ phụ thuộc.

### 6.3. Em/nhóm có bị phụ thuộc vào AI không?

- [ ] Không phụ thuộc
- [ x ] Phụ thuộc ít
- [ ] Phụ thuộc trung bình
- [ ] Phụ thuộc nhiều

Giải thích:

```text
Nhóm sử dụng AI để tăng tốc bước phác thảo và khởi tạo, nhưng vẫn tự kiểm tra
dependency, sửa lỗi cấu hình và xác nhận kết quả bằng build cùng smoke test.
```

---

## 7. Em/nhóm đã kiểm tra kết quả AI như thế nào?

Đánh dấu các cách đã sử dụng.

- [ x ] Chạy thử chương trình
- [ x ] Kiểm tra output
- [ ] Viết test case
- [ x ] So sánh với yêu cầu đề bài
- [ ] Đối chiếu với tài liệu môn học
- [ x ] Review code
- [ ] Hỏi lại giảng viên
- [ ] Tra cứu tài liệu chính thống
- [ ] Thảo luận với thành viên nhóm
- [ ] Kiểm tra bằng dữ liệu mẫu
- [ ] So sánh trước và sau khi dùng AI
- [ ] Khác: ....................................

### Mô tả quá trình kiểm chứng

```text
Nhóm build backend và frontend để phát hiện lỗi compile-time. Sau khi sửa,
nhóm chạy lại hai solution và kiểm tra HTTP endpoint: Swagger API trả 200 và
trang Razor Pages trả 200. Cấu hình URL cũng được đối chiếu với launch profile.
```

### Ví dụ cụ thể về một lần kiểm chứng

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Đăng ký DbContext và SQL Server trong bootstrap backend |
| Em/nhóm đã kiểm tra bằng cách nào? | Build backend solution |
| Kết quả kiểm tra | Cần chỉnh sửa |
| Em/nhóm đã xử lý tiếp như thế nào? | Chuyển đăng ký DbContext về tầng DataAccess và build lại thành công |

---

## 8. Ví dụ AI gợi ý sai hoặc chưa phù hợp

Ghi lại ít nhất một ví dụ nếu có.

| Nội dung | Mô tả |
|---|---|
| AI đã gợi ý gì? | Đặt phần cấu hình UseSqlServer tại API |
| Vì sao gợi ý đó sai/chưa phù hợp? | API chưa có dependency compile-time phù hợp và cấu hình này thuộc trách nhiệm DataAccess |
| Em/nhóm phát hiện bằng cách nào? | Build backend báo lỗi không tìm thấy namespace EntityFrameworkCore |
| Em/nhóm đã sửa như thế nào? | Tạo extension cấu hình trong DataAccess và để API chỉ gọi extension DI |
| Bài học rút ra | Cần kiểm tra dependency graph và ownership của từng tầng |

Nếu không có trường hợp AI gợi ý sai, hãy ghi rõ:

```text
Trong quá trình thực hiện, em/nhóm chưa ghi nhận trường hợp AI gợi ý sai nghiêm trọng. Tuy nhiên, em/nhóm vẫn kiểm tra lại kết quả AI trước khi sử dụng.
```

---

## 9. Phần đóng góp thật sự của sinh viên/nhóm

| Thành viên | MSSV | Nhiệm vụ chính | Có sử dụng AI không? | Minh chứng đóng góp |
|---|---|---|---|---|
| Ngô Sỹ Giá | DE180117 | Backend (Auth, Booking, Payment, Cloudinary), Frontend tich hợp API, Tài liệu | Có (Stitch, ChatGPT, Antigravity) | Build thành công, Tích hợp Cloudinary thành công |
|  |  |  | Có / Không |  |
|  |  |  | Có / Không |  |
|  |  |  | Có / Không |  |

---

## 10. So sánh trước và sau khi dùng AI

| Nội dung | Trước khi dùng AI | Sau khi dùng AI | Cải thiện đạt được |
|---|---|---|---|
| Hiểu yêu cầu | Chỉ đọc spec text | Có AI đối chiếu DB schema | Rõ hơn về mapping field và enum |
| Phân tích bài toán | Thủ công, mất thời gian | AI scan toàn bộ file hiện tại | Nhanh hơn, ít sai sót hơn |
| Thiết kế giải pháp | Cần nhiều thời gian phác thảo | AI gợi ý skeleton nhanh | Tăng tốc giai đoạn kick-off |
| Code/Implementation | Tự viết toàn bộ từ đầu | AI sinh code; nhóm review và kiểm tra | Giảm tải viết boilerplate |
| Debug/Testing | Tự đọc lỗi, mất thời gian | AI gợi ý hướng sửa, nhóm xác nhận | Phat hiện lỗi nhanh hơn |
| Báo cáo/Thuyết trình | Viết thủ công | AI điền mẫu, nhóm chỉnh nội dung | Tiết kiệm thời gian ghi chép |

---

## 11. Bài học về môn học

Sau bài tập/project này, em/nhóm học được gì về kiến thức môn học?

```text
Nhóm hiểu rõ hơn cách cấu hình ASP.NET Core theo tầng, cách đăng ký DI đúng
Module, EF Core Migration workflow và cách Frontend gọi JWT API qua Bearer Token.
Nhóm cũng thực hành thiết kế UX/UI: kiểm tra dữ liệu profile trước khi hiển
form upload, giúp tăng trải nghiệm người dùng rõ rệt.
```

Gợi ý:

- Kiến thức kỹ thuật đã hiểu rõ hơn.
- Kỹ năng lập trình đã cải thiện.
- Cách thiết kế hệ thống.
- Cách kiểm thử.
- Cách phân tích yêu cầu.
- Cách làm việc nhóm.
- Cách giải quyết lỗi.
- Cách trình bày sản phẩm.
- Cách đọc và hiểu tài liệu kỹ thuật.

---

## 12. Bài học về sử dụng AI có trách nhiệm

Sau bài tập/project này, em/nhóm học được gì về việc sử dụng AI một cách minh bạch, có trách nhiệm?

```text
AI cần được sử dụng như công cụ hỗ trợ. Mọi kết quả quan trọng phải được ghi
nhận, kiểm tra và chỉnh sửa trước khi áp dụng. Nhóm cần hiểu được cấu trúc code
và chịu trách nhiệm với sản phẩm cuối cùng.
```

Gợi ý:

- Không nên copy nguyên kết quả AI.
- Cần kiểm tra lại mọi kết quả AI.
- Cần hiểu nội dung trước khi nộp.
- Cần ghi nhận việc sử dụng AI.
- Cần biết AI có thể sai.
- Cần tự chịu trách nhiệm với sản phẩm cuối cùng.
- Cần dùng AI như công cụ hỗ trợ học tập, không thay thế hoàn toàn việc học.

---

## 13. Điều em/nhóm sẽ không làm khi sử dụng AI

Đánh dấu các cam kết phù hợp.

- [ x ] Không dùng AI để làm toàn bộ bài mà không hiểu nội dung.
- [ x ] Không nộp nguyên văn kết quả AI nếu chưa kiểm tra.
- [ x ] Không che giấu việc sử dụng AI trong các phần quan trọng.
- [ x ] Không dùng AI để tạo nội dung sai lệch hoặc gian lận.
- [ x ] Không dùng AI thay thế hoàn toàn quá trình học.
- [ x ] Không bỏ qua yêu cầu, rubric hoặc hướng dẫn của giảng viên.

### Giải thích thêm nếu có

```text
Nhóm cam kết sử dụng AI minh bạch, có kiểm chứng và chỉ xem AI là công cụ hỗ
trợ. Thành viên thực hiện phải hiểu và giải thích được nội dung đã áp dụng.
```

---

## 14. Kế hoạch cải thiện lần sau

Lần sau em/nhóm sẽ sử dụng AI tốt hơn bằng cách nào?

```text
Nhóm sẽ cung cấp context cụ thể hơn, ghi log ngay sau mỗi lần sử dụng AI và
liên kết minh chứng với commit hoặc screenshot. Các gợi ý kỹ thuật sẽ tiếp tục
được kiểm tra bằng build, test và tài liệu chính thức.
```

Gợi ý:

- Viết prompt rõ hơn.
- Cung cấp nhiều ngữ cảnh hơn cho AI.
- Không hỏi AI làm toàn bộ bài.
- Tập trung hỏi AI giải thích, gợi ý, review.
- Tự kiểm tra kỹ hơn.
- Ghi log thường xuyên hơn.
- Liên kết log với commit/screenshot rõ hơn.
- Thảo luận với nhóm trước khi áp dụng kết quả AI.
- Đối chiếu kết quả AI với tài liệu môn học.

---

## 15. Tự đánh giá mức độ hoàn thành

Sinh viên/nhóm tự đánh giá theo thang 1-5.

| Tiêu chí | Điểm tự đánh giá 1-5 | Ghi chú |
|---|:---:|---|
| Ghi nhận việc dùng AI trung thực | 5 | Đã ghi lại năm lần sử dụng AI |
| Prompt có mục tiêu rõ ràng | 4 | Có nêu công nghệ, cấu trúc và đầu ra mong muốn |
| Kiểm chứng kết quả AI | 4 | Đã build và smoke test; chưa có test tự động |
| Tự chỉnh sửa/cải tiến | 4 | Đã sửa dependency và URL backend |
| Hiểu nội dung đã nộp | 4 | Đã rà soát cấu trúc và trách nhiệm từng tầng |
| Reflection có chiều sâu | 4 | Đã ghi nhận lỗi, cách sửa và bài học |
| Sử dụng AI có trách nhiệm | 5 | Không áp dụng nguyên trạng kết quả AI |

---

## 16. Câu hỏi tự vấn cuối bài

Trả lời ngắn gọn các câu hỏi sau.

### 16.1. Nếu giảng viên hỏi về phần AI đã hỗ trợ, em/nhóm có giải thích lại được không?

```text
Chúc. Nhóm có thể giải thích: fetchWithAuth() hoạt động như thế nào, tại sao
DriverLicenseImageUrl phải đổi thành 2 field, EF Core Migration thay đổi DB
vật lý như thế nào, và lý do Backend chưa tích hợp API Owner Requests.
```

### 16.2. Nếu không có AI, em/nhóm có thể tự làm lại phần quan trọng nhất không?

```text
Có. AI giúp tăng tốc bước khởi tạo nhưng nhóm đã tự build, đọc lỗi, điều chỉnh
dependency và xác nhận kết quả runtime nên có thể thực hiện lại theo quy trình.
```

### 16.3. Phần nào trong bài thể hiện rõ nhất năng lực thật sự của em/nhóm?

```text
Khả năng rà soát cấu trúc dự án, sửa lỗi dependency và kiểm chứng kết quả bằng
build cùng smoke test thể hiện rõ nhất năng lực thực tế của nhóm.
```

### 16.4. Em/nhóm muốn cải thiện kỹ năng nào sau bài này?

```text
Nhóm muốn cải thiện kỹ năng thiết kế database, viết test tự động, kiểm tra bảo
mật và liên kết minh chứng AI với commit hoặc screenshot đầy đủ hơn.
```

---

## 17. Cam kết Reflection

Em/nhóm cam kết rằng nội dung reflection này phản ánh trung thực quá trình sử dụng AI và quá trình học tập trong bài tập/project.

Sinh viên/nhóm hiểu rằng:

- AI là công cụ hỗ trợ học tập, không thay thế hoàn toàn năng lực cá nhân.
- Mọi kết quả AI gợi ý cần được kiểm tra trước khi sử dụng.
- Sinh viên/nhóm chịu trách nhiệm với sản phẩm cuối cùng.
- Sinh viên/nhóm cần giải thích được các phần đã nộp.

| Đại diện sinh viên/nhóm | Ngày xác nhận |
|---|---|
|  |  |
