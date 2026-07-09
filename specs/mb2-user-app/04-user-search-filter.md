# VivuCar UI - Search & Filter Cars

## Mục tiêu trang
Người dùng tìm xe theo tên, hãng xe, dòng xe và lọc theo hãng, giá thuê, khu vực.

## File HTML gợi ý
`search.html`

## Layout
- Header user.
- Search bar lớn.
- Sidebar filter bên trái.
- Grid kết quả bên phải.
- Sort + pagination.

## Search bar
Fields:
- Input:
  - id: `searchKeyword`
  - placeholder: `Tìm theo tên xe, hãng xe hoặc dòng xe`
- Suggestions box:
  - id: `searchSuggestions`
  - Ẩn mặc định.
- Button:
  - id: `btnSearch`
  - text: `Tìm kiếm`

## Gợi ý tìm kiếm
Khi người dùng nhập từ khóa:
- Hiển thị tối đa 5 gợi ý.
- Gợi ý theo:
  - Tên xe.
  - Hãng xe.
  - Dòng xe.
- Click gợi ý sẽ set keyword và tìm kiếm.

## Sidebar filter

### Lọc theo hãng xe
Checkbox group:
- id prefix: `brand_`
- Options:
  - Toyota
  - Honda
  - Hyundai
  - Mazda
  - Kia
  - Ford
  - VinFast

### Lọc theo giá thuê
Fields:
- Min price:
  - id: `minPrice`
  - type: `number`
  - placeholder: `Từ`
- Max price:
  - id: `maxPrice`
  - type: `number`
  - placeholder: `Đến`

Quick price chips:
- `Dưới 500k`
- `500k - 800k`
- `800k - 1.2 triệu`
- `Trên 1.2 triệu`

### Lọc theo khu vực
Checkbox group:
- Hải Châu
- Thanh Khê
- Sơn Trà
- Ngũ Hành Sơn
- Liên Chiểu
- Cẩm Lệ

### Lọc khác
- Số chỗ:
  - 4
  - 5
  - 7
  - 9
- Hộp số:
  - Tự động
  - Số sàn
- Nhiên liệu:
  - Xăng
  - Dầu
  - Điện
  - Hybrid

## Filter buttons
- Button:
  - id: `btnApplyFilter`
  - text: `Áp dụng bộ lọc`
- Button:
  - id: `btnResetFilter`
  - text: `Xóa bộ lọc`

## Result header
- Text:
  - id: `resultCount`
  - `Tìm thấy 24 xe`
- Sort:
  - id: `sortCars`
  - options:
    - `Phù hợp nhất`
    - `Giá thấp đến cao`
    - `Giá cao đến thấp`
    - `Rating cao nhất`
    - `Lượt thuê nhiều nhất`

## Result grid
Container:
- id: `searchResultGrid`

Card xe giống trang home:
- Ảnh.
- Tên xe.
- Hãng/dòng xe.
- Khu vực.
- Giá/ngày.
- Rating.
- Button `Xem chi tiết`.

## Pagination
- id: `searchPagination`

## JS state
```js
const searchState = {
  keyword: "",
  brands: [],
  locations: [],
  minPrice: null,
  maxPrice: null,
  seats: [],
  transmissions: [],
  fuelTypes: [],
  sort: "RELEVANT",
  page: 1,
  pageSize: 12
};
```

## JS functions
- `readQueryParams()`
- `renderSuggestions(keyword)`
- `applySearch()`
- `applyFilters()`
- `resetFilters()`
- `renderResults(cars)`
- `renderPagination(totalItems)`

## API gợi ý
```txt
GET /api/cars/search?keyword=vios&brands=Toyota,Honda&minPrice=500000&maxPrice=1000000&location=HaiChau
```

## Validate
- Min price không âm.
- Max price phải lớn hơn min price.
- Nếu filter không hợp lệ, hiện message dưới field.
