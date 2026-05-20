# VivuCar UI - Car Detail

## Mục tiêu trang
Hiển thị đầy đủ thông tin xe: hình ảnh, mô tả, giá thuê, thông số kỹ thuật, địa điểm, đánh giá khách hàng và nút đặt xe.

## File HTML gợi ý
`car-detail.html`

## Layout
- Header user.
- Breadcrumb.
- Gallery ảnh xe.
- Thông tin chính.
- Card đặt xe bên phải.
- Tabs chi tiết.
- Section đánh giá.

## Breadcrumb
- `Trang chủ / Xe / Toyota Vios 2022`

## Gallery
Container:
- id: `carGallery`

Thành phần:
- Ảnh chính:
  - id: `mainCarImage`
- Danh sách thumbnail:
  - id: `carThumbnailList`
- Click thumbnail đổi ảnh chính.

## Thông tin chính
Fields hiển thị:
- Tên xe.
- Hãng xe.
- Dòng xe.
- Rating trung bình.
- Số lượt thuê.
- Địa điểm nhận xe.
- Badge:
  - `Khả dụng`
  - `Chủ xe đã xác minh`

## Card đặt xe
Container:
- id: `bookingCard`

Hiển thị:
- Giá thuê/ngày.
- Phí dịch vụ nếu có.
- Tiền cọc.
- Date nhận xe:
  - id: `detailPickupDate`
- Date trả xe:
  - id: `detailReturnDate`
- Tổng số ngày:
  - id: `totalRentalDays`
- Tạm tính:
  - id: `estimatedPrice`
- Button:
  - id: `btnBookCar`
  - text: `Đặt xe`
- Button:
  - id: `btnContactOwner`
  - text: `Liên hệ chủ xe`

## Tabs
Tabs:
- `Mô tả`
- `Thông số kỹ thuật`
- `Điều kiện thuê`
- `Đánh giá`

### Tab mô tả
- id: `descriptionTab`
- Nội dung:
  - Mô tả xe.
  - Tiện nghi:
    - Camera hành trình.
    - Bluetooth.
    - Điều hòa.
    - Cảm biến lùi.
    - Bản đồ.

### Tab thông số
Fields:
- Hãng xe.
- Dòng xe.
- Năm sản xuất.
- Số chỗ.
- Hộp số.
- Nhiên liệu.
- Mức tiêu hao.
- Biển số rút gọn.

### Tab điều kiện thuê
Checklist:
- Có giấy phép lái xe hợp lệ.
- Căn cước công dân.
- Đặt cọc theo quy định.
- Không hút thuốc trong xe.
- Trả xe đúng thời gian.

### Tab đánh giá
- Rating summary.
- Danh sách review.
- Button:
  - id: `btnWriteReview`
  - text: `Viết đánh giá`
  - chỉ hiện nếu user đã thuê xe hoàn tất.

## Review item
Mỗi review gồm:
- Avatar khách.
- Tên khách.
- Rating sao.
- Ngày đánh giá.
- Nội dung bình luận.
- Hình ảnh review nếu có.

## JS cần có
- Lấy id xe từ URL.
- Render car detail.
- Gallery thumbnail.
- Tính tổng tiền thuê:
```js
function calculateRentalDays(startDate, endDate) {
  const start = new Date(startDate);
  const end = new Date(endDate);
  const diff = end - start;
  return Math.max(1, Math.ceil(diff / (1000 * 60 * 60 * 24)));
}
```
- Validate:
  - Ngày trả phải sau ngày nhận.
  - Xe phải AVAILABLE mới cho đặt.
- Click `Đặt xe`:
  - Nếu chưa login: chuyển `login.html`
  - Nếu đã login: chuyển `booking.html?carId=...`

## API gợi ý
```txt
GET /api/cars/{id}
GET /api/cars/{id}/reviews
```
