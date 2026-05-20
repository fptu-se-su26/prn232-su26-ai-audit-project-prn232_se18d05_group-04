# VivuCar UI - Avatar Upload

## Mục tiêu trang / component
Cho phép người dùng upload, preview, crop đơn giản và cập nhật ảnh đại diện.

## File HTML gợi ý
`avatar-upload.html`

## Có thể dùng như
- Trang riêng.
- Modal trong `profile.html`.

## Layout
- Header user.
- Card upload avatar.
- Preview ảnh.
- Nút lưu.

## Thành phần UI

### Current avatar
- id: `currentAvatar`
- Hiển thị ảnh đại diện hiện tại.

### Upload area
- Input file:
  - id: `avatarInput`
  - type: `file`
  - accept: `image/*`
- Dropzone:
  - id: `avatarDropzone`
  - text: `Kéo thả ảnh vào đây hoặc bấm để chọn ảnh`

### Preview
- id: `avatarPreview`
- Hiển thị ảnh sau khi chọn.
- Shape: hình tròn.

### Controls
- Button:
  - id: `btnRemoveAvatar`
  - text: `Xóa ảnh đã chọn`
- Button:
  - id: `btnCancelAvatar`
  - text: `Hủy`
- Button:
  - id: `btnSaveAvatar`
  - text: `Lưu ảnh đại diện`

## Validate
- File phải là ảnh.
- Dung lượng tối đa 5MB.
- Chỉ chấp nhận:
  - JPG
  - PNG
  - WEBP

## JS cần có
- Click dropzone mở file picker.
- Drag/drop file vào dropzone.
- Preview ảnh bằng `FileReader`.
- Nén ảnh bằng canvas trước khi upload.
- Upload bằng `FormData`.

## Hàm preview
```js
function previewAvatar(file) {
  const reader = new FileReader();

  reader.onload = function(event) {
    document.getElementById("avatarPreview").src = event.target.result;
  };

  reader.readAsDataURL(file);
}
```

## API gợi ý
```txt
POST /api/users/me/avatar
```

Body:
- `multipart/form-data`
- field: `avatar`

## Toast
- `Cập nhật ảnh đại diện thành công`
- `Ảnh không hợp lệ`
- `Dung lượng ảnh không được vượt quá 5MB`
