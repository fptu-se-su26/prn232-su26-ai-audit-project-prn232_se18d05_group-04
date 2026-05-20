# VivuCar UI - Owner Edit Car

## Mục tiêu trang
Người cho thuê cập nhật thông tin xe như giá thuê, mô tả, địa điểm nhận xe, giấy tờ liên quan và tiện nghi.

## File HTML gợi ý
`owner-car-edit.html`

## URL gợi ý
```txt
owner-car-edit.html?carId=C001
```

## Layout
- Owner header.
- Owner sidebar.
- Form chỉnh sửa xe.
- Sidebar preview trạng thái xe.

## Header trang
- Title: `Cập nhật thông tin xe`
- Subtitle: `Đảm bảo thông tin xe luôn chính xác trước khi khách đặt.`

## Form fields
Tương tự trang đăng xe mới, nhưng load dữ liệu hiện tại.

### Fields được sửa
- Tên xe.
- Giá ngày thường.
- Giá cuối tuần.
- Tiền cọc.
- Địa điểm nhận xe.
- Khu vực.
- Mô tả.
- Tiện nghi.
- Mức tiêu hao nhiên liệu.
- Số km hiện tại.
- Giấy tờ xe nếu cần cập nhật.

### Fields nên hạn chế sửa
- Biển số.
- Hãng xe.
- Dòng xe.
- Năm sản xuất.

Nếu cho sửa các field này, nên hiện warning:
- `Thay đổi thông tin định danh xe có thể cần Admin duyệt lại.`

## Status preview card
Hiển thị:
- Trạng thái hiện tại.
- Lượt thuê.
- Rating.
- Lần cập nhật gần nhất.
- Trạng thái duyệt:
  - `Đã duyệt`
  - `Chờ duyệt lại`

## Change warning
Nếu sửa giá hoặc địa điểm:
- Hiển thị box:
  - `Thay đổi này chỉ áp dụng cho các đơn đặt mới, không ảnh hưởng đơn đã xác nhận.`

## Buttons
- `Hủy`
  - id: `btnCancelEditCar`
- `Lưu thay đổi`
  - id: `btnSaveCarChanges`
- `Lưu và gửi duyệt lại`
  - id: `btnSaveAndRequestReview`
  - hiện nếu thay đổi thông tin nhạy cảm.

## Validate
- Giá ngày thường > 0.
- Giá cuối tuần >= giá ngày thường.
- Địa điểm nhận xe không rỗng.
- Mô tả tối thiểu 30 ký tự.
- Số km hiện tại không nhỏ hơn số km đã lưu gần nhất.

## JS cần có
- Lấy carId từ URL.
- Load dữ liệu xe.
- Detect dirty fields.
- Nếu người dùng hủy khi đã sửa, hiện confirm.
- Submit PATCH.
- Nếu có giấy tờ mới, submit FormData.
- Sau khi lưu, hiển thị toast:
  - `Cập nhật xe thành công`

## API gợi ý
```txt
GET /api/owner/cars/{carId}
PATCH /api/owner/cars/{carId}
```
