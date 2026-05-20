# VivuCar UI - Owner Booking Status Tracking

## Mục tiêu trang
Người cho thuê theo dõi và cập nhật trạng thái thực tế của đơn hàng xuyên suốt quá trình: đang xử lý, đã xác nhận, chờ bàn giao, đang thuê, chờ trả xe, hoàn thành, đã hủy.

## File HTML gợi ý
`owner-booking-status-tracking.html`

## URL gợi ý
```txt
owner-booking-status-tracking.html?bookingId=B001
```

## Layout
- Owner header.
- Owner sidebar.
- Booking status banner.
- Timeline trạng thái.
- Status update panel.
- Activity log.

## Booking banner
Hiển thị:
- Mã đơn.
- Khách hàng.
- Xe.
- Trạng thái hiện tại.
- Ngày tạo.
- Tổng tiền.
- Trạng thái thanh toán.

## Timeline
Steps:
1. `Yêu cầu đặt xe`
   - PENDING_OWNER_CONFIRMATION
2. `Đã xác nhận`
   - ACCEPTED
3. `Chờ bàn giao`
   - READY_FOR_HANDOVER
4. `Đang thuê`
   - IN_PROGRESS
5. `Chờ xác nhận trả xe`
   - RETURN_PENDING
6. `Hoàn thành`
   - COMPLETED

Trạng thái đặc biệt:
- DECLINED
- CANCELLED
- DISPUTE_OPEN

## Status update panel
Select:
- id: `nextStatus`
- options render theo status hiện tại, không cho nhảy trạng thái sai.

Textarea:
- id: `statusUpdateNote`
- placeholder: `Nhập ghi chú cập nhật trạng thái`

Button:
- id: `btnUpdateBookingStatus`
- text: `Cập nhật trạng thái`

## Logic chuyển trạng thái hợp lệ
```js
const allowedTransitions = {
  PENDING_OWNER_CONFIRMATION: ["ACCEPTED", "DECLINED"],
  ACCEPTED: ["READY_FOR_HANDOVER", "CANCELLED"],
  READY_FOR_HANDOVER: ["IN_PROGRESS", "CANCELLED"],
  IN_PROGRESS: ["RETURN_PENDING"],
  RETURN_PENDING: ["COMPLETED", "DISPUTE_OPEN"],
  DISPUTE_OPEN: ["COMPLETED"],
  COMPLETED: [],
  CANCELLED: [],
  DECLINED: []
};
```

## Quick actions
Tùy status:
- PENDING_OWNER_CONFIRMATION:
  - `Xác nhận`
  - `Từ chối`
- READY_FOR_HANDOVER:
  - `Ghi nhận bàn giao`
- IN_PROGRESS:
  - `Chat với khách`
- RETURN_PENDING:
  - `Ghi nhận trả xe`
  - `Xác nhận hoàn tất`
- DISPUTE_OPEN:
  - `Xem sự cố`

## Activity log table
Columns:
- Thời gian.
- Trạng thái cũ.
- Trạng thái mới.
- Người cập nhật.
- Ghi chú.

## Validate
- Không cho update nếu nextStatus không nằm trong allowedTransitions.
- Một số trạng thái cần form nghiệp vụ riêng:
  - IN_PROGRESS cần ghi nhận bàn giao.
  - COMPLETED cần ghi nhận trả xe.
- Nếu cố update sai, hiển thị:
  - `Không thể chuyển trạng thái trực tiếp. Vui lòng thực hiện đúng bước nghiệp vụ.`

## JS cần có
- Load booking.
- Render timeline.
- Render allowed next statuses.
- Render quick actions.
- Render activity log.
- Submit status update nếu hợp lệ.

## API gợi ý
```txt
GET /api/owner/bookings/{bookingId}/status
PATCH /api/owner/bookings/{bookingId}/status
GET /api/owner/bookings/{bookingId}/status-history
```
