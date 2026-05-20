# VivuCar UI - Post Trip Review & Feedback

## Mục tiêu trang
Khách hàng đánh giá chất lượng xe và gửi nhận xét dịch vụ sau khi chuyến đi đã hoàn tất.

## File HTML gợi ý
`post-trip-review.html`

## URL gợi ý
```txt
post-trip-review.html?bookingId=B001&carId=C001
```

## Layout
- Header user.
- Card thông tin chuyến đi.
- Form đánh giá nhiều tiêu chí.
- Bình luận dịch vụ.
- Upload ảnh trải nghiệm.
- Button gửi đánh giá.

## Trip summary card
Hiển thị:
- Mã đơn.
- Tên xe.
- Ảnh xe.
- Chủ xe.
- Thời gian thuê.
- Trạng thái:
  - `Completed`

## Rating categories
Dùng rating sao 1-5 cho từng tiêu chí.

### Chất lượng xe
- id: `vehicleQualityRating`
- Label: `Chất lượng xe`

### Độ sạch sẽ
- id: `cleanlinessRating`
- Label: `Độ sạch sẽ`

### Đúng mô tả
- id: `accuracyRating`
- Label: `Đúng như mô tả`

### Vận hành
- id: `performanceRating`
- Label: `Khả năng vận hành`

### Hỗ trợ của chủ xe
- id: `ownerSupportRating`
- Label: `Hỗ trợ của chủ xe`

## Overall rating
- id: `overallRating`
- Tự tính trung bình từ các tiêu chí.
- Cho phép hiển thị:
  - `Đánh giá tổng: 4.8/5`

## Comment fields
- Nhận xét về xe:
  - id: `vehicleComment`
  - textarea
  - placeholder: `Xe có sạch, vận hành tốt, đúng mô tả không?`
- Phản hồi dịch vụ:
  - id: `serviceFeedback`
  - textarea
  - placeholder: `Chủ xe hỗ trợ thế nào? Quy trình nhận/trả xe ra sao?`

## Upload ảnh review
Input:
- id: `reviewImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `reviewImagePreviewGrid`

Text:
- `Tối đa 5 ảnh. Không upload ảnh chứa thông tin nhạy cảm.`

## Buttons
- `Bỏ qua`
  - id: `btnSkipReview`
- `Gửi đánh giá`
  - id: `btnSubmitReview`

## Validate
- Ít nhất phải có rating tổng hoặc các rating tiêu chí.
- Nếu bình luận có nội dung thì tối thiểu 10 ký tự.
- Tối đa 5 ảnh.
- Chỉ cho đánh giá nếu booking status `COMPLETED`.
- Không cho gửi lại nếu đã review trước đó, chỉ cho chỉnh sửa ở trang quản lý review.

## JS cần có
- Click sao để set rating.
- Tự tính overall rating.
- Preview ảnh upload.
- Submit FormData.
- Sau khi gửi thành công:
  - chuyển về `booking-detail.html?bookingId=B001`
  - hoặc hiển thị success modal.

## API gợi ý
```txt
POST /api/reviews/post-trip
```

Body:
```json
{
  "bookingId": "B001",
  "carId": "C001",
  "vehicleQualityRating": 5,
  "cleanlinessRating": 4,
  "accuracyRating": 5,
  "performanceRating": 5,
  "ownerSupportRating": 4,
  "overallRating": 4.6,
  "vehicleComment": "Xe sạch, chạy ổn định.",
  "serviceFeedback": "Chủ xe hỗ trợ nhanh."
}
```
