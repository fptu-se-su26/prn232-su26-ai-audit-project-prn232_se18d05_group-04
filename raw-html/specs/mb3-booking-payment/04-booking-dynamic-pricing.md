# VivuCar UI - Dynamic Pricing Engine

## Mục tiêu component
Hiển thị và tính toán chi phí thuê xe động dựa trên ngày thường/cuối tuần, bảo hiểm, giao xe tận nơi, voucher và tiền cọc.

## File HTML gợi ý
`booking-pricing-summary.html`

## Vị trí sử dụng
- Cột phải trong `booking-checkout.html`
- Có thể dùng riêng để test công thức tính tiền.

## Input cần lấy từ checkout
- Giá ngày thường.
- Giá cuối tuần.
- Ngày giờ nhận xe.
- Ngày giờ trả xe.
- Có mua bảo hiểm không.
- Có giao xe tận nơi không.
- Khoảng cách giao xe.
- Voucher nếu có.
- Tỷ lệ cọc.

## UI summary
Container:
- id: `pricingSummary`

Rows:
- `Số ngày thuê`
  - id: `rentalDays`
- `Ngày thường`
  - id: `weekdayCount`
- `Cuối tuần`
  - id: `weekendCount`
- `Tiền thuê ngày thường`
  - id: `weekdayCost`
- `Tiền thuê cuối tuần`
  - id: `weekendCost`
- `Phí bảo hiểm`
  - id: `insuranceFee`
- `Phí giao xe`
  - id: `deliveryFee`
- `Giảm giá`
  - id: `voucherDiscount`
- `Tổng tiền thuê`
  - id: `totalRentalCost`
- `Tiền cọc cần thanh toán`
  - id: `depositAmount`
- `Còn lại khi nhận xe`
  - id: `remainingAmount`

## Voucher section
- Input:
  - id: `pricingVoucherCode`
- Button:
  - id: `btnApplyPricingVoucher`
- Message:
  - id: `pricingVoucherMessage`

## Công thức gợi ý frontend
```js
const pricingConfig = {
  weekdayPrice: 650000,
  weekendPrice: 800000,
  insuranceFeePerDay: 80000,
  deliveryFeePerKm: 15000,
  depositRate: 0.3
};
```

```js
function isWeekend(date) {
  const day = date.getDay();
  return day === 0 || day === 6;
}
```

```js
function calculateRentalBreakdown(startDate, endDate) {
  const start = new Date(startDate);
  const end = new Date(endDate);

  let weekdayCount = 0;
  let weekendCount = 0;

  const cursor = new Date(start);

  while (cursor < end) {
    if (isWeekend(cursor)) {
      weekendCount++;
    } else {
      weekdayCount++;
    }

    cursor.setDate(cursor.getDate() + 1);
  }

  return {
    weekdayCount,
    weekendCount,
    totalDays: Math.max(1, weekdayCount + weekendCount)
  };
}
```

## Voucher rules mock
- `SUMMER2026`
  - giảm 10%
  - đơn tối thiểu 1.000.000
- `DANANG100K`
  - giảm 100.000
  - đơn tối thiểu 700.000

## Validate
- Voucher không hợp lệ hiển thị:
  - `Mã giảm giá không hợp lệ`
- Không đủ điều kiện:
  - `Đơn hàng chưa đạt giá trị tối thiểu để dùng mã`
- Thành công:
  - `Áp dụng mã giảm giá thành công`

## JS cần có
- Recalculate realtime khi thay đổi input.
- Format VND.
- Tách rõ:
  - `calculateBaseRentalCost`
  - `calculateInsuranceFee`
  - `calculateDeliveryFee`
  - `calculateVoucherDiscount`
  - `calculateDeposit`
