# VivuCar UI - Return Car Request

## Mục tiêu trang
Khách hàng gửi yêu cầu trả xe khi kết thúc thời gian thuê. Hệ thống ghi nhận thời gian, địa điểm trả xe thực tế và chuyển trạng thái đơn sang `RETURN_PENDING`.

## File HTML gợi ý
`return-car-request.html`

## URL gợi ý
```txt
return-car-request.html?bookingId=B001
```

## Layout
- Header user.
- Breadcrumb.
- Card thông tin đơn thuê.
- Form gửi yêu cầu trả xe.
- Card lưu ý.
- Modal xác nhận.

## Breadcrumb
- `Trang chủ / Đơn thuê của tôi / Trả xe`

## Booking summary card
Hiển thị:
- Mã đơn thuê.
- Tên xe.
- Biển số xe.
- Ảnh xe.
- Thời gian nhận xe dự kiến.
- Thời gian trả xe dự kiến.
- Địa điểm nhận xe.
- Trạng thái hiện tại:
  - `Đang thuê`

## Return form
Fields:
- Thời gian trả xe thực tế:
  - id: `actualReturnDate`
  - type: `date`
  - required
- Giờ trả xe thực tế:
  - id: `actualReturnTime`
  - type: `time`
  - required
- Địa điểm trả xe:
  - id: `returnLocation`
  - type: `text`
  - placeholder: `Nhập địa điểm trả xe thực tế`
  - required
- Người bàn giao:
  - id: `handoverPerson`
  - placeholder: `Tên người bàn giao xe`
- Số điện thoại liên hệ:
  - id: `contactPhone`
  - type: `tel`
  - required
- Ghi chú của khách hàng:
  - id: `customerReturnNote`
  - textarea
  - placeholder: `Ví dụ: Xe đã được đổ xăng, có trầy nhẹ bên hông...`

## Optional quick location
Buttons/chips:
- `Trả tại điểm nhận xe`
  - id: `btnUsePickupLocation`
- `Nhập địa điểm khác`
  - id: `btnCustomReturnLocation`

## Upload ảnh khi trả xe
Input:
- id: `returnCarImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `returnImagePreviewGrid`

Text hướng dẫn:
- `Có thể upload ảnh tình trạng xe khi trả để làm minh chứng. Tối đa 8 ảnh.`

## Notice card
Nội dung:
- Sau khi gửi yêu cầu, chủ xe sẽ kiểm tra tình trạng xe.
- Đơn sẽ chuyển sang trạng thái `Đang chờ xác nhận trả xe`.
- Nếu có phụ phí hoặc tranh chấp, VivuCar sẽ ghi nhận để xử lý.

## Buttons
- `Quay lại chi tiết đơn`
  - id: `btnBackToBookingDetail`
- `Gửi yêu cầu trả xe`
  - id: `btnSubmitReturnRequest`

## Confirmation modal
- id: `confirmReturnModal`
- Title: `Xác nhận gửi yêu cầu trả xe`
- Content:
  - `Bạn chắc chắn muốn gửi yêu cầu trả xe cho đơn này?`
- Buttons:
  - `Hủy`
  - `Xác nhận gửi`

## Validate
- Chỉ cho gửi nếu booking status là `IN_PROGRESS`.
- Thời gian trả thực tế không được trước thời gian nhận xe.
- Địa điểm trả xe không rỗng.
- Số điện thoại phải đúng format Việt Nam 10 số.
- Tối đa 8 ảnh, mỗi ảnh không quá 5MB.

## JS cần có
- Lấy `bookingId` từ URL.
- Load thông tin booking.
- Auto fill địa điểm trả xe bằng địa điểm nhận nếu bấm chip.
- Preview ảnh upload.
- Validate form.
- Sau khi submit thành công:
  - cập nhật trạng thái UI thành `RETURN_PENDING`.
  - chuyển sang `booking-detail.html?bookingId=B001`.

## API gợi ý
```txt
POST /api/bookings/{bookingId}/return-request
```

Body:
- multipart/form-data
- fields:
  - actualReturnAt
  - returnLocation
  - handoverPerson
  - contactPhone
  - customerReturnNote
  - returnImages[]
