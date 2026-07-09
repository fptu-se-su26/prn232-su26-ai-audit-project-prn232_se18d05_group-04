# VivuCar UI - Voucher Status & Performance

## Mục tiêu trang
Admin quản lý danh sách voucher, theo dõi trạng thái, lượt sử dụng và hiệu quả chiến dịch.

## File HTML gợi ý
`admin-vouchers.html`

## Layout
- Sidebar Admin.
- Header.
- Content:
  - Title.
  - Summary cards.
  - Filter bar.
  - Voucher table.
  - Drawer / modal xem chi tiết hiệu suất.

## Header trang
- Title: `Quản lý khuyến mãi`
- Button:
  - id: `btnCreateVoucher`
  - text: `Tạo voucher`
  - chuyển sang `admin-voucher-form.html`

## Summary cards
- `Tổng voucher`
- `Đang hoạt động`
- `Đã hết hạn`
- `Đã dùng hết lượt`
- `Tổng lượt sử dụng`

## Filter bar
Fields:
- Search:
  - id: `searchVoucherInput`
  - placeholder: `Tìm theo mã voucher hoặc tên chiến dịch`
- Select trạng thái:
  - id: `voucherStatusFilter`
  - options:
    - `Tất cả trạng thái`
    - `ACTIVE`
    - `EXPIRED`
    - `USED_UP`
    - `INACTIVE`
- Select loại giảm:
  - id: `discountTypeFilter`
  - options:
    - `Tất cả loại`
    - `PERCENT`
    - `FIXED`
- Date from:
  - id: `voucherDateFrom`
  - type: `date`
- Date to:
  - id: `voucherDateTo`
  - type: `date`
- Button:
  - id: `btnFilterVoucher`
  - text: `Lọc`
- Button:
  - id: `btnResetVoucherFilter`
  - text: `Đặt lại`

## Bảng voucher
Columns:
- Mã voucher.
- Tên chiến dịch.
- Loại giảm.
- Giá trị giảm.
- Điều kiện tối thiểu.
- Thời gian áp dụng.
- Lượt dùng.
- Trạng thái.
- Hành động.

## Trạng thái voucher
- ACTIVE: xanh.
- EXPIRED: xám.
- USED_UP: vàng.
- INACTIVE: đỏ nhạt.

## Lượt dùng
Hiển thị dạng:
```txt
65 / 100
```
Có progress bar nhỏ dưới text.

## Hành động từng dòng
- `Xem hiệu suất`
  - class: `btn-view-performance`
- `Sửa`
  - class: `btn-edit-voucher`
- `Tạm dừng`
  - class: `btn-disable-voucher`
  - hiện khi ACTIVE.
- `Kích hoạt`
  - class: `btn-enable-voucher`
  - hiện khi INACTIVE.
- `Xóa`
  - class: `btn-delete-voucher`
  - chỉ cho xóa voucher chưa có lượt dùng.

## Modal / Drawer hiệu suất
- id: `voucherPerformanceDrawer`
- Title: `Hiệu suất voucher`
- Hiển thị:
  - Mã voucher.
  - Tổng lượt sử dụng.
  - Tổng doanh thu từ đơn có voucher.
  - Tổng số tiền đã giảm.
  - Tỷ lệ sử dụng.
  - Khách hàng sử dụng gần đây.

## Mini chart hiệu suất
Dùng div bar chart:
- id: `voucherUsageChart`
- Hiển thị lượt dùng theo ngày.

## Bảng khách hàng sử dụng gần đây
Columns:
- Khách hàng.
- Mã đơn.
- Ngày dùng.
- Giá trị đơn.
- Số tiền giảm.

## JS cần có
- Tự động tính trạng thái:
```js
function resolveVoucherStatus(voucher) {
  const now = new Date();

  if (!voucher.isActive) return "INACTIVE";
  if (new Date(voucher.endDate) < now) return "EXPIRED";
  if (voucher.usedCount >= voucher.usageLimit) return "USED_UP";

  return "ACTIVE";
}
```
- Render progress usage.
- Filter voucher.
- Open performance drawer.
- Enable/disable voucher.
- Delete voucher nếu `usedCount === 0`.

## API gợi ý
```txt
GET /api/admin/vouchers?page=1&pageSize=10&status=ACTIVE
PATCH /api/admin/vouchers/{id}/enable
PATCH /api/admin/vouchers/{id}/disable
DELETE /api/admin/vouchers/{id}
GET /api/admin/vouchers/{id}/performance
```

## Empty state
- Text: `Chưa có voucher nào phù hợp`
- Button: `Tạo voucher mới`
