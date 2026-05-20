# VivuCar UI - Driver Information & Required Documents

## Mục tiêu trang/component
Cho phép khách nhập thông tin người lái và upload hồ sơ bắt buộc: CCCD, giấy phép lái xe B1/B2.

## File HTML gợi ý
`booking-driver-documents.html`

## Vị trí sử dụng
- Step 2 trong `booking-checkout.html`

## Layout
- Card thông tin người lái.
- Card upload CCCD.
- Card upload GPLX.
- Preview hồ sơ đã upload.

## Checkbox
- id: `sameAsLoggedInUser`
- label: `Dùng thông tin tài khoản của tôi`

Nếu checked:
- Tự fill họ tên, email, số điện thoại.
- Vẫn cho sửa nếu cần.

## Driver fields
- Họ tên người lái:
  - id: `driverFullName`
  - required
- Số điện thoại:
  - id: `driverPhone`
  - required
- Email:
  - id: `driverEmail`
- Ngày sinh:
  - id: `driverDob`
  - type: `date`
- Số CCCD:
  - id: `identityNumber`
  - required
- Địa chỉ:
  - id: `driverAddress`
  - textarea

## License fields
- Số giấy phép lái xe:
  - id: `licenseNumber`
  - required
- Hạng bằng:
  - id: `licenseClass`
  - select:
    - B1
    - B2
    - C
- Ngày hết hạn GPLX:
  - id: `licenseExpiredDate`
  - type: `date`

## Upload CCCD
### Mặt trước CCCD
- Input:
  - id: `identityFrontImage`
  - type: `file`
  - accept: `image/*`
- Preview:
  - id: `identityFrontPreview`

### Mặt sau CCCD
- Input:
  - id: `identityBackImage`
  - type: `file`
  - accept: `image/*`
- Preview:
  - id: `identityBackPreview`

## Upload GPLX
### Mặt trước GPLX
- Input:
  - id: `licenseFrontImage`
  - type: `file`
  - accept: `image/*`
- Preview:
  - id: `licenseFrontPreview`

### Mặt sau GPLX
- Input:
  - id: `licenseBackImage`
  - type: `file`
  - accept: `image/*`
- Preview:
  - id: `licenseBackPreview`

## Upload requirements box
Hiển thị:
- Ảnh rõ nét.
- Không bị che thông tin.
- Định dạng JPG, PNG, WEBP.
- Mỗi ảnh tối đa 5MB.
- Thông tin phải trùng với người lái.

## Buttons
- `Lưu hồ sơ`
  - id: `btnSaveDriverProfile`
- `Tiếp tục`
  - id: `btnContinueToPricing`

## Validate
- Họ tên bắt buộc.
- Số điện thoại Việt Nam 10 số.
- CCCD từ 9 đến 12 số.
- Hạng bằng phải là B1/B2/C.
- Ngày hết hạn GPLX phải lớn hơn ngày hiện tại.
- Bắt buộc upload đủ 4 ảnh:
  - CCCD trước.
  - CCCD sau.
  - GPLX trước.
  - GPLX sau.

## JS cần có
- Preview từng ảnh.
- Remove từng ảnh.
- Validate file type và file size.
- Lưu tạm vào `bookingDraft`.
- Submit bằng `FormData` khi tạo booking.

## API gợi ý
```txt
POST /api/bookings/{bookingId}/driver-documents
```

Body:
- `multipart/form-data`
- fields:
  - driverFullName
  - driverPhone
  - identityNumber
  - licenseNumber
  - licenseClass
  - identityFrontImage
  - identityBackImage
  - licenseFrontImage
  - licenseBackImage
