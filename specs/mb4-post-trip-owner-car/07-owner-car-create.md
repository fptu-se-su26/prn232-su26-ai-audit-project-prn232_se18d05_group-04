# VivuCar UI - Owner Create Car

## Mục tiêu trang
Người cho thuê đăng xe mới lên VivuCar bằng cách nhập thông tin xe, giá thuê, địa điểm và upload hình ảnh thực tế.

## File HTML gợi ý
`owner-car-create.html`

## Layout
- Owner header.
- Owner sidebar.
- Multi-section form:
  1. Thông tin cơ bản.
  2. Thông số kỹ thuật.
  3. Giá thuê & địa điểm.
  4. Giấy tờ xe.
  5. Hình ảnh xe.
  6. Preview & submit.

## Header trang
- Title: `Đăng xe mới`
- Subtitle: `Cung cấp thông tin chính xác để khách hàng dễ dàng đặt xe.`

## Section 1: Thông tin cơ bản
Fields:
- Tên xe hiển thị:
  - id: `carName`
  - placeholder: `Toyota Vios 2022`
- Hãng xe:
  - id: `brand`
  - select:
    - Toyota
    - Honda
    - Hyundai
    - Mazda
    - Kia
    - Ford
    - VinFast
- Dòng xe:
  - id: `model`
- Biển số:
  - id: `licensePlate`
- Năm sản xuất:
  - id: `manufactureYear`
  - type: `number`
- Loại xe:
  - id: `carType`
  - select:
    - Sedan
    - SUV
    - Hatchback
    - MPV
    - Pickup

## Section 2: Thông số kỹ thuật
Fields:
- Số chỗ:
  - id: `seatCount`
  - select: 4, 5, 7, 9
- Hộp số:
  - id: `transmission`
  - select:
    - Tự động
    - Số sàn
- Nhiên liệu:
  - id: `fuelType`
  - select:
    - Xăng
    - Dầu
    - Điện
    - Hybrid
- Mức tiêu hao nhiên liệu:
  - id: `fuelConsumption`
- Số km hiện tại:
  - id: `currentOdometer`
  - type: `number`
- Màu xe:
  - id: `carColor`

## Section 3: Giá thuê & địa điểm
Fields:
- Giá thuê ngày thường:
  - id: `weekdayPrice`
  - type: `number`
- Giá thuê cuối tuần:
  - id: `weekendPrice`
  - type: `number`
- Tiền cọc:
  - id: `depositAmount`
  - type: `number`
- Địa điểm nhận xe:
  - id: `pickupLocation`
- Khu vực:
  - id: `district`
  - select:
    - Hải Châu
    - Thanh Khê
    - Sơn Trà
    - Ngũ Hành Sơn
    - Liên Chiểu
    - Cẩm Lệ
- Có hỗ trợ giao xe tận nơi:
  - id: `supportDelivery`
  - type: `checkbox`

## Section 4: Mô tả & tiện nghi
Fields:
- Mô tả chi tiết:
  - id: `description`
  - textarea
- Tiện nghi checkbox:
  - Camera hành trình
  - Bluetooth
  - Cảm biến lùi
  - Bản đồ
  - Ghế trẻ em
  - Sạc USB
  - Cửa sổ trời

## Section 5: Giấy tờ xe
Upload:
- Đăng ký xe:
  - id: `vehicleRegistrationImage`
- Đăng kiểm:
  - id: `inspectionCertificateImage`
- Bảo hiểm xe:
  - id: `insuranceImage`

## Section 6: Hình ảnh xe
Input:
- id: `carImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `carImagePreviewGrid`

Yêu cầu:
- Tối thiểu 4 ảnh.
- Tối đa 12 ảnh.
- Nên có ảnh ngoại thất, nội thất, đồng hồ km.

## Buttons
- `Hủy`
  - id: `btnCancelCreateCar`
- `Lưu nháp`
  - id: `btnSaveCarDraft`
- `Gửi duyệt xe`
  - id: `btnSubmitCar`

## Validate
- Tên xe, hãng, dòng, biển số bắt buộc.
- Giá thuê ngày thường > 0.
- Giá cuối tuần >= giá ngày thường.
- Tiền cọc >= 0.
- Số km không âm.
- Tối thiểu 4 ảnh xe.
- Bắt buộc upload đăng ký xe và đăng kiểm.

## JS cần có
- Preview ảnh xe.
- Preview giấy tờ.
- Validate form.
- Submit FormData.
- Sau khi gửi:
  - status xe: `PENDING_APPROVAL`
  - redirect `owner-cars.html`.

## API gợi ý
```txt
POST /api/owner/cars
```
