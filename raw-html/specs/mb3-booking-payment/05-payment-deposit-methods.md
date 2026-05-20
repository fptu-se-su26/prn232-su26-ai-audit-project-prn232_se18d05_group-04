# VivuCar UI - Deposit Payment Integration

## Mục tiêu trang
Khởi tạo phiên thanh toán tiền cọc và chuyển người dùng sang cổng thanh toán VNPay, MoMo hoặc ZaloPay.

## File HTML gợi ý
`payment-deposit.html`

## URL gợi ý
```txt
payment-deposit.html?bookingId=B001
```

## Layout
- Header user.
- Card thông tin đơn.
- Card chọn phương thức thanh toán.
- Card tổng tiền cọc.
- Button thanh toán.

## Booking payment summary
Hiển thị:
- Mã đơn.
- Tên xe.
- Thời gian thuê.
- Tổng tiền thuê.
- Tiền cọc cần thanh toán.
- Trạng thái:
  - `Chờ thanh toán`

## Payment methods
Radio cards:
- VNPay
  - id: `methodVNPay`
  - value: `VNPAY`
  - mô tả: `Thanh toán qua thẻ ATM, Visa, QR`
- MoMo
  - id: `methodMoMo`
  - value: `MOMO`
  - mô tả: `Thanh toán bằng ví MoMo`
- ZaloPay
  - id: `methodZaloPay`
  - value: `ZALOPAY`
  - mô tả: `Thanh toán bằng ví ZaloPay`

## Buttons
- `Quay lại đơn đặt`
  - id: `btnBackToBooking`
- `Thanh toán tiền cọc`
  - id: `btnPayDeposit`

## Loading state
- Khi click thanh toán:
  - disable button.
  - text: `Đang tạo phiên thanh toán...`
  - show spinner.

## JS flow
1. Lấy bookingId từ URL.
2. Load booking payment info.
3. Người dùng chọn cổng thanh toán.
4. Click thanh toán.
5. Gọi API tạo payment session.
6. API trả về `paymentUrl`.
7. Redirect:
```js
window.location.href = paymentUrl;
```

## API gợi ý
```txt
POST /api/payments/deposit/create
```

Body:
```json
{
  "bookingId": "B001",
  "paymentProvider": "VNPAY",
  "amount": 1500000,
  "returnUrl": "https://vivucar.vn/payment-result.html"
}
```

Response:
```json
{
  "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/...",
  "transactionId": "TXN001"
}
```

## Validate
- Bắt buộc chọn phương thức thanh toán.
- Chỉ cho thanh toán nếu booking status là `PENDING_PAYMENT`.
- Nếu đơn đã thanh toán, hiển thị:
  - `Đơn này đã thanh toán tiền cọc`
  - Button `Xem chi tiết đơn`

## Security note cho Codex comment
- Frontend không tự quyết định trạng thái thanh toán.
- Trạng thái chỉ cập nhật sau khi backend nhận callback/IPN hợp lệ từ payment gateway.
