# VivuCar UI - Featured Cars

## Mục tiêu trang
Hiển thị các xe nổi bật dựa trên đánh giá cao, nhiều lượt thuê hoặc chủ xe uy tín.

## File HTML gợi ý
`featured-cars.html`

## Layout
- Header user.
- Page title.
- Tabs phân loại xe nổi bật.
- Grid danh sách xe.
- Pagination.

## Header trang
Title:
- `Xe nổi bật tại Đà Nẵng`

Subtitle:
- `Những lựa chọn được khách hàng đánh giá cao trên VivuCar.`

## Tabs
Container:
- id: `featuredTabs`

Tabs:
- Button:
  - id: `tabTopRated`
  - text: `Đánh giá cao`
- Button:
  - id: `tabMostRented`
  - text: `Thuê nhiều`
- Button:
  - id: `tabTrustedOwner`
  - text: `Chủ xe uy tín`

## Sort select
Field:
- id: `featuredSort`
- options:
  - `Đề xuất`
  - `Giá thấp đến cao`
  - `Giá cao đến thấp`
  - `Rating cao nhất`
  - `Lượt thuê nhiều nhất`

## Featured car card
Mỗi card gồm:
- Ảnh xe.
- Badge nổi bật:
  - `Top Rated`
  - `Most Rented`
  - `Trusted Owner`
- Tên xe.
- Hãng xe.
- Địa điểm.
- Giá thuê/ngày.
- Rating.
- Lượt thuê.
- Tên chủ xe.
- Badge chủ xe:
  - `Đã xác minh`
- Button:
  - class: `btn-view-car`
  - text: `Xem chi tiết`

## Pagination
- id: `featuredPagination`
- Button `Trước`
- Số trang
- Button `Sau`

## JS state
```js
const featuredState = {
  tab: "TOP_RATED",
  sort: "RECOMMENDED",
  page: 1,
  pageSize: 8
};
```

## JS functions
- `fetchFeaturedCars()`
- `renderFeaturedCars(cars)`
- `setActiveTab(tab)`
- `sortFeaturedCars(cars, sort)`
- `renderPagination(totalItems)`

## API gợi ý
```txt
GET /api/cars/featured?type=TOP_RATED&page=1&pageSize=8&sort=RECOMMENDED
```

## Empty state
- Text: `Chưa có xe nổi bật trong nhóm này`
- Button: `Xem tất cả xe`
