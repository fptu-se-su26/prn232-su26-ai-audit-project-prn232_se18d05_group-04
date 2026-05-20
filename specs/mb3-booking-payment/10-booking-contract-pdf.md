# VivuCar UI - Electronic Rental Contract PDF

## Mục tiêu trang
Cho phép khách hàng xem trước và tải xuống hợp đồng thuê xe tự lái điện tử đã merge thông tin cá nhân, thông tin xe và điều khoản.

## File HTML gợi ý
`booking-contract.html`

## URL gợi ý
```txt
booking-contract.html?bookingId=B001
```

## Layout
- Header user.
- Toolbar trên cùng.
- Contract preview dạng trang A4.
- Buttons tải PDF/in.

## Toolbar
- Button:
  - id: `btnBackToBookingDetail`
  - text: `Quay lại chi tiết đơn`
- Button:
  - id: `btnDownloadContractPdf`
  - text: `Tải PDF`
- Button:
  - id: `btnPrintContract`
  - text: `In hợp đồng`

## Contract preview container
- id: `contractPreview`
- Style giống giấy A4:
  - width khoảng 794px.
  - nền trắng.
  - shadow nhẹ.
  - padding lớn.

## Header hợp đồng
- Logo text: `VivuCar`
- Title:
  - `HỢP ĐỒNG THUÊ XE Ô TÔ TỰ LÁI`
- Mã hợp đồng.
- Ngày lập hợp đồng.

## Section 1: Bên cho thuê
Fields:
- Tên đơn vị/chủ xe.
- Số điện thoại.
- Địa chỉ.
- Email.

## Section 2: Bên thuê
Fields:
- Họ tên người thuê.
- Số CCCD.
- Số điện thoại.
- Địa chỉ.
- Số GPLX.
- Hạng bằng.

## Section 3: Thông tin xe
Fields:
- Hãng xe.
- Dòng xe.
- Biển số.
- Năm sản xuất.
- Màu xe.
- Số chỗ.
- Hộp số.
- Nhiên liệu.

## Section 4: Thời gian và địa điểm thuê
Fields:
- Ngày giờ nhận xe.
- Ngày giờ trả xe.
- Địa điểm nhận xe.
- Địa điểm trả xe.
- Giao xe tận nơi nếu có.

## Section 5: Chi phí
Table:
- Tiền thuê xe.
- Phí bảo hiểm.
- Phí giao xe.
- Voucher giảm giá.
- Tổng tiền.
- Tiền cọc đã thanh toán.
- Còn lại khi nhận xe.

## Section 6: Điều khoản
Danh sách điều khoản:
1. Bên thuê có trách nhiệm cung cấp thông tin chính xác.
2. Bên thuê phải xuất trình CCCD và GPLX bản gốc khi nhận xe.
3. Bên thuê chịu trách nhiệm bảo quản xe trong thời gian thuê.
4. Mọi vi phạm giao thông trong thời gian thuê do bên thuê chịu trách nhiệm.
5. Xe phải được hoàn trả đúng thời gian và tình trạng đã thỏa thuận.
6. Phụ phí phát sinh được tính theo chính sách VivuCar.

## Section 7: Chữ ký
Hai cột:
- `Bên cho thuê`
- `Bên thuê`

Hiển thị:
- Họ tên.
- Vị trí ký.
- Text: `Ký và ghi rõ họ tên`

## JS cần có
- Lấy bookingId từ URL.
- Load dữ liệu contract.
- Merge dữ liệu vào template.
- Download PDF:
  - Nếu chưa có backend: dùng `window.print()` hoặc tạo file HTML in được.
  - Nếu có backend: gọi API lấy PDF.

## API gợi ý
```txt
GET /api/bookings/{bookingId}/contract
GET /api/bookings/{bookingId}/contract/pdf
```

## CSS print
Cần có:
```css
@media print {
  .app-header,
  .contract-toolbar {
    display: none;
  }

  body {
    background: white;
  }

  #contractPreview {
    box-shadow: none;
    width: 100%;
  }
}
```
