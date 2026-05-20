# VivuCar UI - Create / Edit Review & Rating

## Mục tiêu trang / modal
Người dùng gửi rating và bình luận sau khi hoàn tất thuê xe. Có thể chỉnh sửa review cá nhân.

## File HTML gợi ý
`review-form.html`

## Vị trí sử dụng
- Modal trong trang `car-detail.html`.
- Trang riêng sau khi đơn thuê hoàn tất:
  - `review-form.html?bookingId=B001&carId=C001`

## Điều kiện hiển thị
Chỉ cho review nếu:
- Người dùng đã đăng nhập.
- Có đơn thuê xe đã hoàn tất.
- Chưa review hoặc đang chỉnh sửa review của chính mình.

## Layout
- Header user nếu là trang riêng.
- Card thông tin xe/đơn thuê.
- Form đánh giá.

## Card thông tin đơn thuê
Hiển thị:
- Ảnh xe.
- Tên xe.
- Biển số rút gọn.
- Ngày nhận xe.
- Ngày trả xe.
- Mã đơn thuê.

## Rating field
- Label: `Bạn đánh giá xe này như thế nào?`
- 5 sao clickable:
  - id: `ratingStars`
- Hidden input:
  - id: `ratingValue`
  - value từ 1 đến 5.
- Text rating:
  - 1: `Rất không hài lòng`
  - 2: `Không hài lòng`
  - 3: `Bình thường`
  - 4: `Hài lòng`
  - 5: `Rất hài lòng`

## Comment field
- Textarea:
  - id: `reviewComment`
  - placeholder: `Chia sẻ trải nghiệm thuê xe của bạn...`
  - maxlength: 1000
- Counter:
  - id: `commentCounter`
  - format: `0/1000`

## Upload ảnh trải nghiệm
- Input:
  - id: `reviewImages`
  - type: `file`
  - accept: `image/*`
  - multiple
- Preview grid:
  - id: `reviewImagePreview`
- Note:
  - `Tối đa 5 ảnh`

## Buttons
- `Hủy`
  - id: `btnCancelReview`
- `Gửi đánh giá`
  - id: `btnSubmitReview`
- Nếu edit:
  - text button: `Cập nhật đánh giá`

## Validate
- Rating bắt buộc từ 1 đến 5.
- Comment tối thiểu 10 ký tự.
- Tối đa 5 ảnh.
- Mỗi ảnh tối đa 5MB.

## JS cần có
- Click sao để set rating.
- Hover sao để preview rating.
- Đếm ký tự comment.
- Preview ảnh.
- Remove từng ảnh đã chọn.
- Submit review.
- Nếu có query `reviewId`, load review cũ để edit.

## API gợi ý
```txt
POST /api/reviews
PUT /api/reviews/{id}
```

Body:
```json
{
  "bookingId": "B001",
  "carId": "C001",
  "rating": 5,
  "comment": "Xe sạch, chủ xe hỗ trợ tốt."
}
```

## Toast
- `Gửi đánh giá thành công`
- `Cập nhật đánh giá thành công`
- `Vui lòng chọn số sao đánh giá`
