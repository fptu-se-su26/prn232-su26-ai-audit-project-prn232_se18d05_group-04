# VivuCar UI - Block / Unblock Vehicle

## Mục tiêu trang / component
Component dùng trong trang quản lý xe để Admin tạm ngưng hoạt động hoặc mở lại xe. Khi xe bị block, xe không xuất hiện ở giao diện tìm kiếm của khách hàng.

## File HTML gợi ý
`admin-car-status-modal.html`

## Vị trí sử dụng
- Trong `admin-cars.html`
- Trong `admin-car-detail.html`

## Button trên từng xe
### Nếu xe đang hoạt động
- Button:
  - text: `Khóa xe`
  - class: `btn-block-car`
  - data-car-id

### Nếu xe đang bị khóa
- Button:
  - text: `Mở khóa`
  - class: `btn-unblock-car`
  - data-car-id

## Modal khóa xe
- id: `blockCarModal`
- Title: `Khóa phương tiện`
- Content:
  - `Xe này sẽ bị ẩn khỏi kết quả tìm kiếm của khách hàng.`
- Thông tin xe:
  - Biển số.
  - Hãng xe.
  - Dòng xe.
- Fields:
  - Select lý do:
    - id: `blockReasonType`
    - options:
      - `Bảo dưỡng định kỳ`
      - `Xe gặp sự cố`
      - `Vi phạm chính sách`
      - `Chủ xe yêu cầu tạm dừng`
      - `Khác`
  - Textarea ghi chú:
    - id: `blockNote`
    - placeholder: `Nhập ghi chú chi tiết`
- Buttons:
  - `Hủy`
    - id: `btnCancelBlockCar`
  - `Xác nhận khóa`
    - id: `btnConfirmBlockCar`

## Modal mở khóa xe
- id: `unblockCarModal`
- Title: `Mở khóa phương tiện`
- Content:
  - `Xe sẽ được hiển thị lại trên hệ thống nếu trạng thái là Đang rảnh.`
- Fields:
  - Textarea:
    - id: `unblockNote`
    - placeholder: `Ghi chú sau khi mở khóa`
- Buttons:
  - `Hủy`
    - id: `btnCancelUnblockCar`
  - `Xác nhận mở khóa`
    - id: `btnConfirmUnblockCar`

## Trạng thái sau thao tác
- Khi block:
  - Cập nhật status thành `BLOCKED`.
  - Dòng xe đổi badge đỏ.
  - Nút đổi thành `Mở khóa`.
- Khi unblock:
  - Cập nhật status thành `AVAILABLE` hoặc trạng thái trước đó.
  - Nút đổi thành `Khóa xe`.

## JS cần có
```js
let selectedCarId = null;

function openBlockCarModal(carId) {
  selectedCarId = carId;
  document.getElementById("blockCarModal").classList.add("show");
}

function closeBlockCarModal() {
  selectedCarId = null;
  document.getElementById("blockCarModal").classList.remove("show");
}
```

## API gợi ý
```txt
PATCH /api/admin/cars/{id}/block
PATCH /api/admin/cars/{id}/unblock
```

Body block:
```json
{
  "reasonType": "MAINTENANCE",
  "note": "Bảo dưỡng định kỳ"
}
```

## Validate
- Khi khóa xe bắt buộc chọn lý do.
- Nếu lý do là `Khác`, bắt buộc nhập ghi chú.
- Không cho khóa xe đang có đơn thuê đang diễn ra, hiển thị:
  - `Không thể khóa xe vì đang có đơn thuê chưa hoàn tất.`

## Toast
- `Đã khóa xe thành công`
- `Đã mở khóa xe thành công`
- `Không thể thao tác. Vui lòng thử lại.`
