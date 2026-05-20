# VivuCar UI - Owner Booking Request Detail

## Mục tiêu trang
Người cho thuê xem chi tiết yêu cầu đặt xe: thông tin khách, xe, thời gian thuê, hồ sơ người lái, thanh toán cọc và điều kiện xác nhận.

## File HTML gợi ý
`owner-booking-request-detail.html`

## URL gợi ý
```txt
owner-booking-request-detail.html?bookingId=B001
```

## Layout
- Owner header.
- Owner sidebar.
- Breadcrumb.
- Status banner.
- Main content 2 cột:
  - Cột trái: chi tiết booking.
  - Cột phải: action panel.

## Breadcrumb
- `Yêu cầu đặt xe / Chi tiết yêu cầu`

## Status banner
Hiển thị:
- Mã đơn.
- Trạng thái hiện tại.
- Ngày tạo yêu cầu.
- Countdown xử lý nếu có:
  - `Còn 02:15:30 để phản hồi`

## Card thông tin khách thuê
Fields:
- Họ tên khách.
- Số điện thoại.
- Email.
- Rating khách nếu có.
- Số lần đã thuê.
- Trạng thái xác minh GPLX:
  - `Đã xác minh`
  - `Chờ xác minh`
- Button:
  - id: `btnOpenCustomerChat`
  - text: `Chat với khách`

## Card thông tin xe
Fields:
- Ảnh xe.
- Tên xe.
- Biển số.
- Địa điểm nhận xe.
- Trạng thái xe.
- Giá ngày thường.
- Giá cuối tuần.

## Card thời gian thuê
Fields:
- Ngày giờ nhận xe.
- Ngày giờ trả xe.
- Tổng số ngày.
- Có giao xe tận nơi:
  - Có/Không
- Địa chỉ giao xe nếu có.

## Card hồ sơ người lái
Fields:
- Người lái có phải người đặt không.
- Họ tên người lái.
- Số điện thoại.
- CCCD rút gọn.
- GPLX rút gọn.
- Hạng bằng.

## Document preview
Buttons:
- `Xem CCCD mặt trước`
- `Xem CCCD mặt sau`
- `Xem GPLX mặt trước`
- `Xem GPLX mặt sau`

Click mở modal:
- id: `documentPreviewModal`

## Card thanh toán
Fields:
- Tổng tiền thuê.
- Tiền cọc yêu cầu.
- Tiền cọc đã thanh toán.
- Số tiền còn lại.
- Trạng thái thanh toán:
  - `Chờ thanh toán`
  - `Đã cọc`

## Action panel
Buttons:
- `Xác nhận yêu cầu`
  - id: `btnAcceptBooking`
  - chỉ hiện nếu được phép xác nhận.
- `Từ chối yêu cầu`
  - id: `btnDeclineBooking`
- `Gửi tin nhắn cho khách`
  - id: `btnMessageCustomer`
- `Quay lại danh sách`
  - id: `btnBackToRequests`

## Điều kiện xác nhận hiển thị
Checklist:
- Xe không bị trùng lịch.
- Xe đang Available.
- Khách đã thanh toán cọc hoặc theo cấu hình hệ thống.
- Hồ sơ người lái đầy đủ.
- GPLX hợp lệ.

## Warning
Nếu có vấn đề:
- `Xe đang có lịch trùng. Không thể xác nhận.`
- `Khách chưa thanh toán cọc.`
- `Hồ sơ người lái chưa đầy đủ.`

## JS cần có
- Lấy bookingId từ URL.
- Load detail.
- Render checklist điều kiện.
- Disable accept nếu không đủ điều kiện.
- Mở modal preview giấy tờ.
- Mở accept/decline modal.

## API gợi ý
```txt
GET /api/owner/booking-requests/{bookingId}
GET /api/owner/booking-requests/{bookingId}/documents
```
