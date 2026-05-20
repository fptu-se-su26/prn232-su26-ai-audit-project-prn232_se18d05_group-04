# VivuCar UI - Booking Checkout

## Mục tiêu trang
Trang checkout sau khi khách hàng bấm `Đặt xe` từ trang chi tiết xe. Trang này gom các bước: chọn thời gian thuê, kiểm tra xe trống, nhập thông tin người lái, chọn dịch vụ thêm, áp dụng voucher và thanh toán tiền cọc.

## File HTML gợi ý
`booking-checkout.html`

## URL gợi ý
```txt
booking-checkout.html?carId=C001
```

## Layout
- Header user.
- Stepper quy trình đặt xe.
- Cột trái: form booking.
- Cột phải: booking summary cố định khi scroll.
- Footer đơn giản.

## Stepper
Container:
- id: `bookingStepper`

Steps:
1. `Thời gian thuê`
2. `Hồ sơ người lái`
3. `Chi phí`
4. `Thanh toán cọc`

Trạng thái step:
- `active`
- `completed`
- `disabled`

## Card thông tin xe
Hiển thị:
- Ảnh xe.
- Tên xe.
- Hãng xe.
- Địa điểm nhận xe.
- Giá ngày thường.
- Giá cuối tuần.
- Rating.
- Badge: `Khả dụng`

## Section 1: Thời gian thuê
Fields:
- Ngày nhận xe:
  - id: `pickupDate`
  - type: `date`
- Giờ nhận xe:
  - id: `pickupTime`
  - type: `time`
- Ngày trả xe:
  - id: `returnDate`
  - type: `date`
- Giờ trả xe:
  - id: `returnTime`
  - type: `time`
- Button:
  - id: `btnCheckAvailability`
  - text: `Kiểm tra xe trống`

## Availability result
Container:
- id: `availabilityResult`

Trạng thái:
- Success:
  - text: `Xe còn trống trong thời gian bạn chọn`
  - màu xanh.
- Error:
  - text: `Xe đã có người đặt trong khung giờ này`
  - màu đỏ.
- Loading:
  - text: `Đang kiểm tra lịch xe...`

## Section 2: Hồ sơ người lái
Có checkbox:
- id: `sameAsAccount`
- label: `Người lái là tôi`

Nếu bỏ chọn, cho nhập thông tin người lái khác.

Fields:
- Họ tên người lái:
  - id: `driverFullName`
- Số điện thoại:
  - id: `driverPhone`
- Email:
  - id: `driverEmail`
- Số CCCD:
  - id: `driverIdentityNumber`
- Số GPLX:
  - id: `driverLicenseNumber`
- Hạng bằng:
  - id: `driverLicenseClass`
  - select:
    - B1
    - B2
    - C

## Section 3: Dịch vụ thêm
Checkbox:
- id: `insuranceOption`
- label: `Mua bảo hiểm vật chất xe`
- mô tả: `Giảm rủi ro chi phí phát sinh khi có va chạm`

Checkbox:
- id: `deliveryOption`
- label: `Giao xe tận nơi`

Nếu chọn giao xe tận nơi, hiển thị:
- Địa chỉ giao xe:
  - id: `deliveryAddress`
- Khoảng cách ước tính km:
  - id: `deliveryDistanceKm`
  - type: `number`
- Phí giao xe:
  - id: `deliveryFeePreview`

## Section 4: Voucher
Fields:
- Input:
  - id: `voucherCode`
  - placeholder: `Nhập mã giảm giá`
- Button:
  - id: `btnApplyVoucher`
  - text: `Áp dụng`
- Message:
  - id: `voucherMessage`

## Section 5: Thanh toán
Chọn cổng thanh toán:
- Radio:
  - id: `paymentVNPay`
  - value: `VNPAY`
  - label: `VNPay`
- Radio:
  - id: `paymentMoMo`
  - value: `MOMO`
  - label: `MoMo`
- Radio:
  - id: `paymentZaloPay`
  - value: `ZALOPAY`
  - label: `ZaloPay`

Button chính:
- id: `btnCreateBooking`
- text: `Tiếp tục thanh toán cọc`

## Booking summary bên phải
Container:
- id: `bookingSummary`

Hiển thị:
- Tên xe.
- Thời gian thuê.
- Số ngày thuê.
- Giá thuê ngày thường.
- Giá thuê cuối tuần.
- Phí bảo hiểm.
- Phí giao xe.
- Giảm giá voucher.
- Tổng tiền thuê.
- Tiền cọc cần thanh toán.
- Số tiền còn lại khi nhận xe.

## Buttons cuối form
- `Quay lại`
  - id: `btnBackToCarDetail`
- `Tiếp tục thanh toán cọc`
  - id: `btnProceedPayment`

## Validate
- Ngày giờ trả xe phải sau ngày giờ nhận xe.
- Phải kiểm tra availability thành công trước khi thanh toán.
- Họ tên người lái không rỗng.
- Số điện thoại hợp lệ.
- CCCD không rỗng.
- GPLX không rỗng.
- Phải chọn cổng thanh toán.

## JS cần có
- Lấy `carId` từ URL.
- Load thông tin xe.
- Kiểm tra availability.
- Tính lại chi phí mỗi khi thay đổi ngày, bảo hiểm, giao xe, voucher.
- Disable nút thanh toán nếu chưa kiểm tra xe trống.
- Tạo booking trạng thái `PENDING_PAYMENT`.
- Chuyển sang payment URL giả lập.

## API gợi ý
```txt
GET /api/cars/{carId}
POST /api/bookings/check-availability
POST /api/bookings
POST /api/payments/deposit/create
```
