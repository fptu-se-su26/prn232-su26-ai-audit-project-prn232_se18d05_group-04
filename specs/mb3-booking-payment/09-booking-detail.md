# VivuCar UI - Booking Detail View

## Mục tiêu trang
Hiển thị chi tiết đơn đặt xe: xe, người lái, nhân viên giao xe, tiến độ thanh toán, phụ phí, hợp đồng và hành động hủy/đánh giá.

## File HTML gợi ý
`booking-detail.html`

## URL gợi ý
```txt
booking-detail.html?bookingId=B001
```

## Layout
- Header user.
- Breadcrumb.
- Booking status banner.
- Main content 2 cột:
  - Cột trái: thông tin chi tiết.
  - Cột phải: payment summary và actions.
- Timeline trạng thái đơn.

## Breadcrumb
- `Trang chủ / Đơn thuê của tôi / Chi tiết đơn`

## Status banner
Hiển thị:
- Mã đơn.
- Trạng thái hiện tại.
- Message theo trạng thái:
  - PENDING_PAYMENT: `Vui lòng thanh toán tiền cọc để giữ xe.`
  - DEPOSIT_PAID: `Đơn đã được xác nhận, chờ giao xe.`
  - IN_PROGRESS: `Bạn đang trong thời gian thuê xe.`
  - COMPLETED: `Đơn thuê đã hoàn tất.`
  - CANCELLED: `Đơn thuê đã bị hủy.`

## Timeline
Steps:
1. Tạo đơn.
2. Thanh toán cọc.
3. Nhận xe.
4. Trả xe.
5. Hoàn tất.

Mỗi step có:
- Title.
- Time.
- Status:
  - completed
  - active
  - pending

## Card thông tin xe
Fields:
- Ảnh xe.
- Tên xe.
- Hãng/dòng xe.
- Biển số.
- Năm sản xuất.
- Hộp số.
- Nhiên liệu.
- Địa điểm nhận xe.

## Card thời gian thuê
Fields:
- Ngày giờ nhận xe.
- Ngày giờ trả xe.
- Tổng số ngày.
- Giao xe tận nơi:
  - Có/Không
- Địa chỉ giao xe nếu có.

## Card người lái
Fields:
- Họ tên.
- Số điện thoại.
- Email.
- Số CCCD rút gọn.
- Số GPLX rút gọn.
- Hạng bằng.

## Card nhân viên giao xe
Fields:
- Tên nhân viên.
- Số điện thoại.
- Ghi chú giao xe.
Nếu chưa phân công:
- Text: `Chưa phân công nhân viên giao xe`

## Payment summary card
Rows:
- Tổng tiền thuê.
- Tiền cọc đã thanh toán.
- Phí bảo hiểm.
- Phí giao xe.
- Phụ phí phát sinh.
- Giảm giá.
- Cần thanh toán khi nhận xe.

## Extra fees section
Nếu có phụ phí:
- Tên phụ phí.
- Số tiền.
- Ghi chú.
Ví dụ:
- Trễ giờ trả xe.
- Vượt giới hạn km.
- Phí vệ sinh.
- Phí nhiên liệu.

## Actions
- `Thanh toán cọc`
  - id: `btnPayDeposit`
  - hiện nếu PENDING_PAYMENT.
- `Tải hợp đồng`
  - id: `btnDownloadContract`
- `Xem hợp đồng`
  - id: `btnPreviewContract`
- `Hủy đơn`
  - id: `btnCancelBooking`
  - hiện nếu còn được hủy.
- `Đánh giá xe`
  - id: `btnReviewCar`
  - hiện nếu COMPLETED.

## JS cần có
- Lấy bookingId từ URL.
- Load booking detail.
- Render status badge.
- Render timeline.
- Ẩn/hiện action theo trạng thái.
- Click hủy mở modal hủy đơn.
- Click hợp đồng chuyển `booking-contract.html?bookingId=B001`.

## API gợi ý
```txt
GET /api/bookings/{bookingId}
GET /api/bookings/{bookingId}/extra-fees
```
