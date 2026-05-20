# VivuCar UI - My Reviews Management

## Mục tiêu trang
Người dùng xem, chỉnh sửa hoặc xóa các bình luận/đánh giá của cá nhân.

## File HTML gợi ý
`my-reviews.html`

## Layout
- Header user.
- Sidebar profile.
- Content danh sách đánh giá.
- Modal xác nhận xóa.

## Header trang
- Title: `Đánh giá của tôi`
- Subtitle: `Quản lý các đánh giá bạn đã gửi sau khi thuê xe.`

## Filter bar
Fields:
- Search:
  - id: `searchMyReview`
  - placeholder: `Tìm theo tên xe hoặc nội dung đánh giá`
- Select rating:
  - id: `ratingFilter`
  - options:
    - `Tất cả đánh giá`
    - `5 sao`
    - `4 sao`
    - `3 sao`
    - `2 sao`
    - `1 sao`
- Sort:
  - id: `reviewSort`
  - options:
    - `Mới nhất`
    - `Cũ nhất`
    - `Rating cao nhất`
    - `Rating thấp nhất`
- Button:
  - id: `btnFilterReview`
  - text: `Lọc`

## Review list
Container:
- id: `myReviewList`

## Review card
Mỗi card gồm:
- Ảnh xe.
- Tên xe.
- Mã đơn thuê.
- Ngày thuê.
- Rating sao.
- Nội dung bình luận.
- Ảnh review nếu có.
- Ngày tạo review.
- Ngày cập nhật nếu có.
- Buttons:
  - class: `btn-edit-review`
  - text: `Chỉnh sửa`
  - class: `btn-delete-review`
  - text: `Xóa`

## Modal xóa review
- id: `deleteReviewModal`
- Title: `Xóa đánh giá`
- Content:
  - `Bạn chắc chắn muốn xóa đánh giá này? Hành động này không thể hoàn tác.`
- Buttons:
  - id: `btnCancelDeleteReview`
  - text: `Hủy`
  - id: `btnConfirmDeleteReview`
  - text: `Xóa đánh giá`

## Empty state
Nếu chưa có review:
- Text: `Bạn chưa có đánh giá nào`
- Subtext: `Sau khi hoàn tất thuê xe, bạn có thể gửi đánh giá tại đây.`
- Button:
  - text: `Tìm xe để thuê`

## JS state
```js
const myReviewState = {
  keyword: "",
  rating: "",
  sort: "NEWEST",
  page: 1,
  pageSize: 10
};
```

## JS functions
- `fetchMyReviews()`
- `renderMyReviews(reviews)`
- `openDeleteReviewModal(reviewId)`
- `deleteReview(reviewId)`
- `editReview(reviewId)`

## API gợi ý
```txt
GET /api/users/me/reviews
DELETE /api/reviews/{id}
```

## Luồng edit
- Click `Chỉnh sửa`.
- Chuyển sang:
  - `review-form.html?reviewId=R001`

## Toast
- `Đã xóa đánh giá`
- `Không thể xóa đánh giá`
