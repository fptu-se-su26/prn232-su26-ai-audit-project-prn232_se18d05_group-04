# VivuCar UI - Owner Car Image Management

## Mục tiêu trang
Người cho thuê thêm mới, thay đổi, xóa và sắp xếp hình ảnh xe đã đăng tải.

## File HTML gợi ý
`owner-car-images.html`

## URL gợi ý
```txt
owner-car-images.html?carId=C001
```

## Layout
- Owner header.
- Owner sidebar.
- Car info header.
- Main image section.
- Gallery management grid.
- Upload new images.
- Save order button.

## Car info header
Hiển thị:
- Tên xe.
- Biển số.
- Trạng thái.
- Số lượng ảnh hiện tại.
- Button:
  - `Quay lại xe của tôi`

## Main image
Section:
- Title: `Ảnh đại diện`
- Hiển thị ảnh chính hiện tại.
- Button:
  - `Đặt làm ảnh đại diện`
  - xuất hiện trên từng ảnh trong grid.

## Gallery grid
Container:
- id: `carImageManageGrid`

Mỗi image item:
- Ảnh.
- Badge:
  - `Ảnh đại diện` nếu là main.
- Buttons:
  - `Đặt đại diện`
    - class: `btn-set-main-image`
  - `Xóa`
    - class: `btn-delete-car-image`
- Drag handle:
  - text: `Kéo để sắp xếp`

## Upload new images
Input:
- id: `newCarImages`
- type: `file`
- accept: `image/*`
- multiple

Dropzone:
- id: `imageUploadDropzone`
- text: `Kéo thả ảnh xe hoặc bấm để chọn`

Preview new images:
- id: `newImagePreviewGrid`

## Image requirements
Box hướng dẫn:
- Tối thiểu 4 ảnh cho mỗi xe.
- Ảnh rõ nét, đúng xe thực tế.
- Nên có ảnh trước, sau, hai bên, nội thất, đồng hồ km.
- Không dùng ảnh chứa watermark hoặc thông tin liên hệ ngoài hệ thống.

## Buttons
- `Hủy`
  - id: `btnCancelImageManage`
- `Lưu thay đổi`
  - id: `btnSaveImageChanges`
- `Upload ảnh mới`
  - id: `btnUploadNewImages`

## Delete image modal
- id: `deleteImageModal`
- Title: `Xóa ảnh xe`
- Content:
  - `Bạn chắc chắn muốn xóa ảnh này?`
- Buttons:
  - `Hủy`
  - `Xóa ảnh`

## Validate
- Không cho xóa nếu sau khi xóa còn dưới 4 ảnh.
- Tối đa 12 ảnh.
- Mỗi ảnh tối đa 5MB.
- File phải là JPG, PNG hoặc WEBP.
- Luôn phải có 1 ảnh đại diện.

## JS cần có
- Load ảnh xe.
- Preview ảnh mới.
- Delete ảnh.
- Set main image.
- Sắp xếp ảnh bằng drag/drop HTML5.
- Save image order.
- Upload ảnh bằng FormData.

## API gợi ý
```txt
GET /api/owner/cars/{carId}/images
POST /api/owner/cars/{carId}/images
PATCH /api/owner/cars/{carId}/images/main
PATCH /api/owner/cars/{carId}/images/order
DELETE /api/owner/cars/{carId}/images/{imageId}
```
