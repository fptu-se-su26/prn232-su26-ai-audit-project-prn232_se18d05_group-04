# VivuCar UI - Add / Update Car Profile

## Mục tiêu trang
Admin thêm mới hoặc chỉnh sửa hồ sơ xe, bao gồm thông tin xe, giá thuê, trạng thái và hình ảnh.

## File HTML gợi ý
`admin-car-form.html`

## Layout
- Sidebar Admin.
- Header.
- Content chính dạng form.
- Form chia thành các card:
  1. Thông tin cơ bản.
  2. Thông số xe.
  3. Giá thuê và trạng thái.
  4. Hình ảnh xe.
  5. Nút lưu.

## Header trang
- Title khi thêm: `Thêm xe mới`
- Title khi sửa: `Cập nhật thông tin xe`
- Breadcrumb:
  - `Admin / Quản lý xe / Thêm xe`

## Card 1: Thông tin cơ bản
Fields:
- Biển số xe
  - id: `licensePlate`
  - required
  - placeholder: `43A-12345`
- Hãng xe
  - id: `brand`
  - select
  - options: Toyota, Honda, Mazda, Kia, Hyundai, Ford, VinFast
- Dòng xe
  - id: `model`
  - placeholder: `Vios, City, CX-5...`
- Năm sản xuất
  - id: `year`
  - type: `number`
  - min: 2000
  - max: 2026
- Số chỗ
  - id: `seatCount`
  - select
  - options: 4, 5, 7, 9

## Card 2: Thông số xe
Fields:
- Hộp số
  - id: `transmission`
  - select
  - options:
    - `Tự động`
    - `Số sàn`
- Nhiên liệu
  - id: `fuelType`
  - select
  - options:
    - `Xăng`
    - `Dầu`
    - `Điện`
    - `Hybrid`
- Mức tiêu hao nhiên liệu
  - id: `fuelConsumption`
  - placeholder: `7L/100km`
- Địa điểm nhận xe
  - id: `pickupLocation`
  - placeholder: `Hải Châu, Đà Nẵng`
- Mô tả xe
  - id: `description`
  - textarea
  - placeholder: `Mô tả tiện nghi, tình trạng xe...`

## Card 3: Giá thuê và trạng thái
Fields:
- Giá thuê theo ngày
  - id: `dailyPrice`
  - type: `number`
  - placeholder: `700000`
- Tiền cọc
  - id: `depositAmount`
  - type: `number`
  - placeholder: `3000000`
- Trạng thái xe
  - id: `carStatus`
  - select
  - options:
    - `AVAILABLE`
    - `RENTED`
    - `MAINTENANCE`
    - `BLOCKED`

## Card 4: Upload hình ảnh
Fields:
- Input file:
  - id: `carImages`
  - type: `file`
  - accept: `image/*`
  - multiple
- Preview grid:
  - id: `imagePreviewGrid`
- Text hướng dẫn:
  - `Tối đa 8 ảnh, mỗi ảnh không quá 5MB.`
- Button:
  - `Xóa tất cả ảnh`
  - id: `btnClearImages`

## Nút cuối form
- `Hủy`
  - id: `btnCancel`
  - quay lại `admin-cars.html`
- `Lưu nháp`
  - id: `btnSaveDraft`
- `Lưu xe`
  - id: `btnSaveCar`

## Validate
- Biển số không rỗng.
- Hãng xe không rỗng.
- Dòng xe không rỗng.
- Giá thuê > 0.
- Ít nhất 1 hình ảnh.
- Năm sản xuất không lớn hơn năm hiện tại.

## JS cần có
- Preview ảnh trước khi upload.
- Nén ảnh phía client bằng canvas trước khi gửi.
- Nếu URL có query `?id=C001`, load dữ liệu xe để edit.
- Submit bằng `FormData`.

## Gợi ý hàm nén ảnh
```js
function compressImage(file, maxWidth = 1200, quality = 0.75) {
  return new Promise((resolve) => {
    const img = new Image();
    const reader = new FileReader();

    reader.onload = (e) => {
      img.src = e.target.result;
    };

    img.onload = () => {
      const canvas = document.createElement("canvas");
      const scale = maxWidth / img.width;
      canvas.width = maxWidth;
      canvas.height = img.height * scale;

      const ctx = canvas.getContext("2d");
      ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

      canvas.toBlob(resolve, "image/jpeg", quality);
    };

    reader.readAsDataURL(file);
  });
}
```
