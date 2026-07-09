# VivuCar UI - Payment Result Page

## Mục tiêu trang
Hiển thị kết quả thanh toán sau khi người dùng được redirect từ VNPay, MoMo hoặc ZaloPay về VivuCar.

## File HTML gợi ý
`payment-result.html`

## URL gợi ý
```txt
payment-result.html?bookingId=B001&status=success&transactionId=TXN001
```

## Layout
- Header user.
- Result card ở giữa màn hình.
- Thông tin giao dịch.
- Nút điều hướng.

## Result states

### Success
- Icon bằng CSS hoặc text: `✓`
- Title: `Thanh toán tiền cọc thành công`
- Message:
  - `Đơn đặt xe của bạn đã được xác nhận. VivuCar đã gửi email xác nhận cho bạn.`
- Badge status:
  - `Đã cọc - Chờ giao xe`

### Failed
- Icon: `!`
- Title: `Thanh toán thất bại`
- Message:
  - `Giao dịch chưa hoàn tất. Bạn có thể thử thanh toán lại.`
- Badge:
  - `Chờ thanh toán`

### Pending
- Title:
  - `Đang xác nhận thanh toán`
- Message:
  - `Hệ thống đang kiểm tra kết quả giao dịch. Vui lòng làm mới sau ít phút.`

## Transaction info
Fields:
- Mã đơn:
  - id: `bookingIdText`
- Mã giao dịch:
  - id: `transactionIdText`
- Cổng thanh toán:
  - id: `paymentProviderText`
- Số tiền cọc:
  - id: `paidAmountText`
- Thời gian thanh toán:
  - id: `paymentTimeText`

## Buttons
Success:
- `Xem chi tiết đơn`
  - id: `btnViewBookingDetail`
- `Về trang chủ`
  - id: `btnGoHome`

Failed:
- `Thử thanh toán lại`
  - id: `btnRetryPayment`
- `Liên hệ hỗ trợ`
  - id: `btnContactSupport`

Pending:
- `Kiểm tra lại`
  - id: `btnRefreshPaymentStatus`

## JS flow
- Đọc query params.
- Gọi API kiểm tra trạng thái thanh toán mới nhất.
- Render state theo response backend.

## API gợi ý
```txt
GET /api/payments/status?bookingId=B001&transactionId=TXN001
```

Response:
```json
{
  "bookingId": "B001",
  "transactionId": "TXN001",
  "provider": "VNPAY",
  "amount": 1500000,
  "status": "SUCCESS",
  "paidAt": "2026-06-01T10:30:00"
}
```

## Backend note cho Codex comment
- Callback/IPN từ payment gateway phải được xử lý ở backend.
- Backend cần kiểm tra checksum/signature.
- Frontend chỉ hiển thị kết quả, không tự set đơn thành đã thanh toán.
