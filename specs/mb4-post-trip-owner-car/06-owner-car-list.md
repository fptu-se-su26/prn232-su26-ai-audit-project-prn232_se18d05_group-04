# VivuCar UI - Owner Car Inventory List

## Mục tiêu trang
Người cho thuê quản lý danh sách xe đã đăng, xem trạng thái, hiệu suất cơ bản và thao tác chỉnh sửa/tạm ngưng/mở lại.

## File HTML gợi ý
`owner-cars.html`

## Layout
- Owner header.
- Owner sidebar.
- Content:
  - Title.
  - Summary cards.
  - Filter bar.
  - Car table/grid.
  - Pagination.

## Header trang
- Title: `Xe của tôi`
- Button:
  - id: `btnCreateCar`
  - text: `Đăng xe mới`
  - link: `owner-car-create.html`

## Summary cards
- `Tổng xe`
- `Đang cho thuê`
- `Đang rảnh`
- `Bảo trì`
- `Tạm ngưng`

## Filter bar
Fields:
- Search:
  - id: `ownerCarSearch`
  - placeholder: `Tìm theo tên xe, biển số`
- Status:
  - id: `ownerCarStatusFilter`
  - options:
    - `Tất cả trạng thái`
    - `AVAILABLE`
    - `RENTED`
    - `MAINTENANCE`
    - `UNAVAILABLE`
- Brand:
  - id: `ownerCarBrandFilter`
- Sort:
  - id: `ownerCarSort`
  - options:
    - `Mới nhất`
    - `Lượt thuê nhiều nhất`
    - `Doanh thu cao nhất`
    - `Giá thấp đến cao`
    - `Giá cao đến thấp`
- Button:
  - id: `btnFilterOwnerCars`
  - text: `Lọc`
- Button:
  - id: `btnResetOwnerCarFilter`
  - text: `Đặt lại`

## Car card/table
Mỗi item gồm:
- Ảnh đại diện xe.
- Tên xe.
- Biển số.
- Giá thuê/ngày.
- Địa điểm nhận xe.
- Trạng thái.
- Số lượt thuê.
- Rating trung bình.
- Doanh thu tháng này.
- Buttons:
  - `Xem`
    - class: `btn-view-owner-car`
  - `Sửa`
    - class: `btn-edit-owner-car`
  - `Ảnh xe`
    - class: `btn-manage-car-images`
  - `Trạng thái`
    - class: `btn-manage-car-status`

## Badge trạng thái
- AVAILABLE: xanh.
- RENTED: xanh dương.
- MAINTENANCE: vàng.
- UNAVAILABLE: đỏ/xám.

## Empty state
- Text: `Bạn chưa đăng xe nào`
- Button:
  - `Đăng xe đầu tiên`

## JS state
```js
const ownerCarState = {
  keyword: "",
  status: "ALL",
  brand: "",
  sort: "NEWEST",
  page: 1,
  pageSize: 10
};
```

## JS cần có
- Render danh sách xe của owner.
- Filter, sort, pagination.
- Điều hướng:
  - create: `owner-car-create.html`
  - edit: `owner-car-edit.html?carId=C001`
  - images: `owner-car-images.html?carId=C001`
  - status: `owner-car-status.html?carId=C001`

## API gợi ý
```txt
GET /api/owner/cars?page=1&pageSize=10&status=AVAILABLE
```
