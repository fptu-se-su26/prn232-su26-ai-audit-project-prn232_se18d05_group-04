# VivuCar UI - Availability & Concurrency Check

## Mục tiêu component
Kiểm tra xe có bị đặt trùng lịch hay không khi khách chọn ngày giờ nhận/trả. Component này dùng trong checkout và có thể tái sử dụng ở trang chi tiết xe.

## File HTML gợi ý
`booking-availability-check.html`

## Vị trí sử dụng
- `booking-checkout.html`
- `car-detail.html`

## UI fields
- Ngày nhận xe:
  - id: `availabilityPickupDate`
- Giờ nhận xe:
  - id: `availabilityPickupTime`
- Ngày trả xe:
  - id: `availabilityReturnDate`
- Giờ trả xe:
  - id: `availabilityReturnTime`
- Button:
  - id: `btnCheckAvailability`
  - text: `Kiểm tra lịch trống`

## Result box
Container:
- id: `availabilityBox`

States:
1. Default:
   - `Chọn thời gian thuê để kiểm tra xe còn trống.`
2. Loading:
   - `Đang kiểm tra lịch xe...`
3. Available:
   - `Xe còn trống. Bạn có thể tiếp tục đặt xe.`
4. Conflict:
   - `Xe đã được đặt trong khung giờ này. Vui lòng chọn thời gian khác.`
5. Error:
   - `Không thể kiểm tra lịch xe. Vui lòng thử lại.`

## Gợi ý hiển thị khung giờ bị trùng
Nếu conflict, hiển thị:
- Từ ngày giờ.
- Đến ngày giờ.
- Trạng thái đơn đang giữ xe:
  - `Chờ thanh toán`
  - `Đã cọc`
  - `Đang thuê`

## JS state
```js
const availabilityState = {
  carId: null,
  pickupAt: null,
  returnAt: null,
  isAvailable: false,
  conflictBookings: []
};
```

## Hàm JS chính
```js
function buildDateTime(date, time) {
  if (!date || !time) return null;
  return `${date}T${time}:00`;
}
```

```js
function isValidBookingRange(pickupAt, returnAt) {
  return new Date(returnAt) > new Date(pickupAt);
}
```

## API gợi ý
```txt
POST /api/bookings/check-availability
```

Body:
```json
{
  "carId": "C001",
  "pickupAt": "2026-06-01T08:00:00",
  "returnAt": "2026-06-03T18:00:00"
}
```

Response available:
```json
{
  "available": true,
  "conflicts": []
}
```

Response conflict:
```json
{
  "available": false,
  "conflicts": [
    {
      "bookingId": "B001",
      "pickupAt": "2026-06-01T07:00:00",
      "returnAt": "2026-06-02T18:00:00",
      "status": "DEPOSIT_PAID"
    }
  ]
}
```

## Lưu ý logic backend
Codex chỉ dựng frontend, nhưng cần comment rõ nghiệp vụ:
- Backend phải dùng transaction hoặc lock để tránh 2 người đặt cùng lúc.
- Không chỉ kiểm tra ở frontend.
- Khi tạo booking phải kiểm tra lại lần nữa trong backend.
