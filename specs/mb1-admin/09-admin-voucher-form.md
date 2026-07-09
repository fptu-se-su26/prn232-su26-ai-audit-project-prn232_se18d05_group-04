# VivuCar UI - Create Voucher

## Mục tiêu trang
Admin tạo mã giảm giá, thiết lập loại giảm giá, thời gian áp dụng và điều kiện sử dụng.

## File HTML gợi ý
`admin-voucher-form.html`

## Layout
- Sidebar Admin.
- Header.
- Content chính dạng form 2 cột:
  - Cột trái: thông tin voucher.
  - Cột phải: preview voucher.

## Header trang
- Title: `Tạo mã giảm giá`
- Breadcrumb:
  - `Admin / Khuyến mãi / Tạo voucher`

## Form fields

### Thông tin cơ bản
- Mã voucher
  - id: `voucherCode`
  - placeholder: `SUMMER2026`
  - uppercase tự động.
- Tên chiến dịch
  - id: `campaignName`
  - placeholder: `Khuyến mãi hè 2026`
- Mô tả
  - id: `voucherDescription`
  - textarea
  - placeholder: `Mô tả ngắn về chương trình`

### Loại giảm giá
- Radio:
  - id: `discountTypePercent`
  - value: `PERCENT`
  - label: `Giảm theo phần trăm`
- Radio:
  - id: `discountTypeFixed`
  - value: `FIXED`
  - label: `Giảm số tiền cố định`

### Giá trị giảm
- Nếu chọn phần trăm:
  - field id: `discountPercent`
  - type: `number`
  - placeholder: `10`
  - suffix: `%`
- Nếu chọn cố định:
  - field id: `discountAmount`
  - type: `number`
  - placeholder: `100000`
  - suffix: `VND`

### Thời gian áp dụng
- Ngày bắt đầu
  - id: `startDate`
  - type: `datetime-local`
- Ngày kết thúc
  - id: `endDate`
  - type: `datetime-local`

### Điều kiện áp dụng
- Giá trị đơn tối thiểu
  - id: `minimumOrderAmount`
  - type: `number`
  - placeholder: `500000`
- Số lượt sử dụng tối đa
  - id: `usageLimit`
  - type: `number`
  - placeholder: `100`
- Số lượt mỗi khách
  - id: `usageLimitPerUser`
  - type: `number`
  - placeholder: `1`
- Nhóm khách hàng
  - id: `customerGroup`
  - select
  - options:
    - `Tất cả khách hàng`
    - `Khách hàng mới`
    - `Khách hàng cũ`
    - `Khách VIP`

### Trạng thái
- Toggle:
  - id: `isActive`
  - label: `Kích hoạt ngay`

## Preview voucher
Card bên phải gồm:
- Code lớn: `SUMMER2026`
- Tên chiến dịch.
- Giá trị giảm.
- Điều kiện đơn tối thiểu.
- Thời gian áp dụng.
- Trạng thái.

## Buttons
- `Hủy`
  - id: `btnCancelVoucher`
- `Lưu nháp`
  - id: `btnSaveVoucherDraft`
- `Tạo voucher`
  - id: `btnCreateVoucher`

## Validate
- Mã voucher bắt buộc, không dấu cách.
- Mã voucher tự động uppercase.
- Discount percent từ 1 đến 100.
- Discount fixed > 0.
- Ngày kết thúc phải sau ngày bắt đầu.
- Usage limit > 0.
- Minimum order amount >= 0.

## JS cần có
- Toggle field theo discount type.
- Preview voucher cập nhật realtime.
- Validate trước khi submit.
- Submit tạo voucher.

## API gợi ý
```txt
POST /api/admin/vouchers
```

Body:
```json
{
  "code": "SUMMER2026",
  "campaignName": "Khuyến mãi hè 2026",
  "discountType": "PERCENT",
  "discountValue": 10,
  "startDate": "2026-06-01T00:00:00",
  "endDate": "2026-06-30T23:59:59",
  "minimumOrderAmount": 500000,
  "usageLimit": 100,
  "usageLimitPerUser": 1,
  "customerGroup": "NEW_CUSTOMER",
  "isActive": true
}
```
