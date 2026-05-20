# VivuCar UI - Car Fleet List, Filtering & Pagination

## Mục tiêu trang
Admin quản lý danh sách xe, tìm kiếm, lọc theo trạng thái, phân trang và thao tác nhanh với từng xe.

## File HTML gợi ý
`admin-cars.html`

## Layout
- Sidebar Admin.
- Header.
- Content:
  - Page title.
  - Summary cards.
  - Filter bar.
  - Data table.
  - Pagination.

## Header trang
- Title: `Quản lý phương tiện`
- Button chính:
  - id: `btnAddCar`
  - text: `Thêm xe mới`
  - chuyển sang `admin-car-form.html`

## Summary cards
4 card nhỏ:
- `Tổng số xe`
- `Đang rảnh`
- `Đang thuê`
- `Bảo trì`
- `Đã khóa`

Mỗi card có:
- Label.
- Number.
- Màu khác nhau theo trạng thái.

## Filter bar
Fields:
- Search:
  - id: `searchCarInput`
  - placeholder: `Tìm theo biển số, hãng xe, dòng xe`
- Select trạng thái:
  - id: `statusFilter`
  - options:
    - `Tất cả trạng thái`
    - `AVAILABLE`
    - `RENTED`
    - `MAINTENANCE`
    - `BLOCKED`
- Select hãng xe:
  - id: `brandFilter`
  - options:
    - `Tất cả hãng`
    - Toyota
    - Honda
    - Mazda
    - Kia
    - Hyundai
    - Ford
    - VinFast
- Select loại nhiên liệu:
  - id: `fuelFilter`
  - options:
    - `Tất cả nhiên liệu`
    - Xăng
    - Dầu
    - Điện
    - Hybrid
- Select hộp số:
  - id: `transmissionFilter`
  - options:
    - `Tất cả hộp số`
    - Tự động
    - Số sàn
- Button:
  - id: `btnFilter`
  - text: `Lọc`
- Button:
  - id: `btnReset`
  - text: `Đặt lại`

## Bảng danh sách xe
Columns:
- Ảnh xe.
- Biển số.
- Hãng / Dòng xe.
- Năm.
- Số chỗ.
- Giá/ngày.
- Trạng thái.
- Chủ xe.
- Hành động.

## Hành động từng dòng
- `Xem`
  - class: `btn-view-car`
- `Sửa`
  - class: `btn-edit-car`
- `Khóa`
  - class: `btn-block-car`
  - chỉ hiện nếu xe chưa bị khóa.
- `Mở khóa`
  - class: `btn-unblock-car`
  - chỉ hiện nếu xe đang bị khóa.

## Badge trạng thái
- AVAILABLE: xanh.
- RENTED: xanh dương.
- MAINTENANCE: vàng.
- BLOCKED: đỏ.

## Pagination
- Select page size:
  - id: `pageSize`
  - options: 10, 20, 50
- Button:
  - id: `prevPage`
  - text: `Trước`
- Container số trang:
  - id: `pageNumbers`
- Button:
  - id: `nextPage`
  - text: `Sau`
- Text:
  - id: `paginationInfo`
  - ví dụ: `Hiển thị 1-10 trên 235 xe`

## Empty state
Khi không có xe:
- Text: `Không tìm thấy xe phù hợp`
- Button: `Đặt lại bộ lọc`

## JS cần có
- State:
```js
const state = {
  keyword: "",
  status: "",
  brand: "",
  fuelType: "",
  transmission: "",
  page: 1,
  pageSize: 10
};
```
- Hàm:
  - `fetchCars()`
  - `renderCars(cars)`
  - `renderPagination(totalItems)`
  - `applyFilters()`
  - `resetFilters()`

## API gợi ý
```txt
GET /api/admin/cars?page=1&pageSize=10&status=AVAILABLE&brand=Toyota
```

Response:
```json
{
  "items": [],
  "totalItems": 235,
  "currentPage": 1,
  "pageSize": 10
}
```
