# VivuCar UI - Excel Export & PDF Export

## Mục tiêu trang
Admin xuất báo cáo giao dịch, danh sách khách hàng, danh sách xe sang Excel hoặc PDF.

## File HTML gợi ý
`admin-export-reports.html`

## Layout
- Sidebar Admin.
- Header.
- Content:
  - Tiêu đề.
  - Card chọn loại báo cáo.
  - Bộ lọc dữ liệu.
  - Preview dữ liệu.
  - Nút export.

## Header trang
- Title: `Xuất báo cáo`
- Subtitle: `Tải xuống dữ liệu phục vụ đối soát và lưu trữ`

## Card chọn loại báo cáo
Radio cards:
- id: `reportTypeTransactions`
  - value: `TRANSACTIONS`
  - label: `Giao dịch`
- id: `reportTypeCustomers`
  - value: `CUSTOMERS`
  - label: `Khách hàng`
- id: `reportTypeCars`
  - value: `CARS`
  - label: `Danh sách xe`
- id: `reportTypeRevenue`
  - value: `REVENUE`
  - label: `Doanh thu`

## Bộ lọc
Fields:
- Từ ngày
  - id: `exportDateFrom`
  - type: `date`
- Đến ngày
  - id: `exportDateTo`
  - type: `date`
- Trạng thái thanh toán
  - id: `paymentStatusFilter`
  - select
  - options:
    - `Tất cả`
    - `PAID`
    - `PENDING`
    - `REFUNDED`
- Trạng thái đơn
  - id: `orderStatusFilter`
  - select
  - options:
    - `Tất cả`
    - `COMPLETED`
    - `CANCELLED`
    - `ONGOING`
- Button:
  - id: `btnPreviewReport`
  - text: `Xem trước`

## Preview table
Container:
- id: `reportPreview`

Columns thay đổi theo report type.

### Giao dịch
- Mã giao dịch.
- Mã đơn.
- Khách hàng.
- Ngày thanh toán.
- Số tiền.
- Phương thức.
- Trạng thái.

### Khách hàng
- Mã khách hàng.
- Họ tên.
- Email.
- SĐT.
- Số đơn đã thuê.
- Tổng chi tiêu.
- Trạng thái.

### Xe
- Biển số.
- Hãng xe.
- Dòng xe.
- Chủ xe.
- Giá/ngày.
- Trạng thái.

### Doanh thu
- Ngày.
- Số đơn.
- Doanh thu.
- Hoàn tiền.
- Doanh thu thực nhận.

## Export buttons
- Button:
  - id: `btnExportExcel`
  - text: `Xuất Excel`
- Button:
  - id: `btnExportPdf`
  - text: `Xuất PDF`

## Modal export PDF
- id: `pdfExportModal`
- Fields:
  - Tiêu đề báo cáo:
    - id: `pdfTitle`
  - Checkbox:
    - id: `includeLogo`
    - label: `Hiển thị logo công ty`
  - Checkbox:
    - id: `includeFooter`
    - label: `Hiển thị footer`
- Buttons:
  - `Hủy`
  - `Tạo PDF`

## JS cần có
- Thu thập filter.
- Gọi API preview.
- Render bảng preview.
- Khi export Excel:
  - gọi endpoint backend trả file.
- Khi export PDF:
  - gọi endpoint backend trả file.

## API gợi ý
```txt
GET /api/admin/reports/preview?type=TRANSACTIONS&from=2026-05-01&to=2026-05-31
GET /api/admin/reports/export/excel?type=TRANSACTIONS&from=2026-05-01&to=2026-05-31
GET /api/admin/reports/export/pdf?type=REVENUE&from=2026-05-01&to=2026-05-31
```

## JS tải file
```js
function downloadBlob(blob, fileName) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");

  link.href = url;
  link.download = fileName;
  link.click();

  window.URL.revokeObjectURL(url);
}
```

## Loading state
- Disable nút export khi đang xử lý.
- Text:
  - `Đang tạo file...`
- Toast:
  - `Xuất file thành công`
  - `Không thể xuất file`
