# VivuCar UI - User Home & Car Browsing

## Mục tiêu trang
Trang chủ dành cho khách hàng, hiển thị danh sách xe đang khả dụng tại Đà Nẵng. Người dùng có thể xem xe, tìm kiếm nhanh và chuyển sang trang chi tiết.

## File HTML gợi ý
`home.html`

## Layout
- Header user.
- Hero search section.
- Section xe nổi bật.
- Section tất cả xe khả dụng.
- Footer.

## Header
Menu:
- Logo: `VivuCar`
- Link: `Trang chủ`
- Link: `Tìm xe`
- Link: `Xe nổi bật`
- Link: `Hồ sơ`
- Link: `Đơn thuê của tôi`
- Button:
  - id: `btnLogin`
  - text: `Đăng nhập`
- Button:
  - id: `btnProfile`
  - text: tên người dùng nếu đã đăng nhập.

## Hero section
Title:
- `Thuê xe tự lái dễ dàng tại Đà Nẵng`

Subtitle:
- `Tìm xe phù hợp cho chuyến đi cá nhân, công tác hoặc du lịch.`

Fields:
- Search input:
  - id: `homeSearchInput`
  - placeholder: `Tìm Toyota, Honda, Mazda...`
- Select khu vực:
  - id: `homeLocationSelect`
  - options:
    - `Tất cả khu vực`
    - `Hải Châu`
    - `Thanh Khê`
    - `Sơn Trà`
    - `Ngũ Hành Sơn`
    - `Liên Chiểu`
    - `Cẩm Lệ`
- Date nhận xe:
  - id: `pickupDate`
  - type: `date`
- Date trả xe:
  - id: `returnDate`
  - type: `date`
- Button:
  - id: `btnQuickSearch`
  - text: `Tìm xe ngay`

## Section xe nổi bật
- Title: `Xe nổi bật`
- Container:
  - id: `featuredCarsGrid`
- Button:
  - id: `btnViewAllFeatured`
  - text: `Xem tất cả xe nổi bật`

## Section tất cả xe khả dụng
- Title: `Xe đang khả dụng`
- Container:
  - id: `availableCarsGrid`

## Card xe
Mỗi card gồm:
- Hình ảnh xe.
- Badge:
  - `Khả dụng`
  - `Nổi bật` nếu có.
- Tên xe.
- Hãng xe.
- Địa điểm.
- Giá thuê/ngày.
- Rating.
- Số lượt thuê.
- Button:
  - class: `btn-view-detail`
  - text: `Xem chi tiết`
- Button:
  - class: `btn-book-now`
  - text: `Đặt xe`

## Empty state
Khi không có xe:
- Text: `Không tìm thấy xe khả dụng`
- Button: `Xóa bộ lọc`

## JS cần có
- Render danh sách xe từ mock data.
- Click `Xem chi tiết` chuyển sang:
  - `car-detail.html?id=CAR_ID`
- Click `Đặt xe` cũng chuyển sang chi tiết xe hoặc booking page sau này.
- Quick search redirect sang:
  - `search.html?keyword=...&location=...`

## Mock data gợi ý
```js
const cars = [
  {
    id: "C001",
    name: "Toyota Vios 2022",
    brand: "Toyota",
    model: "Vios",
    image: "../assets/images/car-vios.jpg",
    location: "Hải Châu, Đà Nẵng",
    dailyPrice: 650000,
    rating: 4.8,
    rentalCount: 125,
    status: "AVAILABLE",
    isFeatured: true
  }
];
```

## CSS gợi ý
- Grid card: desktop 4 cột, tablet 2 cột, mobile 1 cột.
- Card bo góc 16px.
- Ảnh xe tỷ lệ 16:9.
- Giá thuê màu xanh dương đậm.
