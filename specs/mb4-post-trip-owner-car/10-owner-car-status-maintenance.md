# VivuCar UI - Owner Car Status & Maintenance

## Mục tiêu trang
Người cho thuê cập nhật trạng thái xe, tạm ngưng hoặc mở lại cho thuê, ghi nhận lý do bảo trì và đồng bộ trạng thái xe trên nền tảng.

## File HTML gợi ý
`owner-car-status.html`

## URL gợi ý
```txt
owner-car-status.html?carId=C001
```

## Layout
- Owner header.
- Owner sidebar.
- Card thông tin xe.
- Status control panel.
- Maintenance schedule.
- Status history.

## Car info card
Hiển thị:
- Ảnh xe.
- Tên xe.
- Biển số.
- Trạng thái hiện tại.
- Địa điểm nhận xe.
- Đơn thuê đang diễn ra nếu có.

## Current status badge
Status:
- AVAILABLE
- RENTED
- MAINTENANCE
- UNAVAILABLE

## Status control
Radio cards:
- Available:
  - id: `statusAvailable`
  - label: `Available`
  - mô tả: `Xe sẵn sàng cho thuê`
- Maintenance:
  - id: `statusMaintenance`
  - label: `Maintenance`
  - mô tả: `Xe đang bảo trì, không hiển thị cho khách`
- Unavailable:
  - id: `statusUnavailable`
  - label: `Unavailable`
  - mô tả: `Tạm ngưng cho thuê theo yêu cầu chủ xe`

Không cho chọn `Rented` thủ công nếu không có booking đang diễn ra.

## Maintenance fields
Hiển thị khi chọn `Maintenance`:
- Lý do bảo trì:
  - id: `maintenanceReason`
  - select:
    - `Bảo dưỡng định kỳ`
    - `Sửa chữa hư hỏng`
    - `Vệ sinh xe`
    - `Kiểm tra giấy tờ`
    - `Khác`
- Ngày bắt đầu:
  - id: `maintenanceStartDate`
  - type: `date`
- Ngày dự kiến mở lại:
  - id: `maintenanceEndDate`
  - type: `date`
- Ghi chú:
  - id: `maintenanceNote`
  - textarea

## Unavailable fields
Hiển thị khi chọn `Unavailable`:
- Lý do tạm ngưng:
  - id: `unavailableReason`
  - select:
    - `Chủ xe tạm ngưng kinh doanh`
    - `Xe không sẵn sàng`
    - `Điều chỉnh thông tin xe`
    - `Khác`
- Ghi chú:
  - id: `unavailableNote`

## Buttons
- `Hủy`
  - id: `btnCancelStatusChange`
- `Cập nhật trạng thái`
  - id: `btnUpdateCarStatus`

## Warning rules
- Nếu xe đang có đơn `IN_PROGRESS`, không cho chuyển sang Available/Unavailable.
- Nếu xe có booking sắp tới, hiển thị:
  - `Xe đang có đơn đặt trong tương lai. Thay đổi trạng thái có thể ảnh hưởng đến khách hàng.`
- Khi chuyển sang Maintenance/Unavailable:
  - xe sẽ bị ẩn khỏi tìm kiếm.

## Status history
Table:
- Thời gian.
- Trạng thái cũ.
- Trạng thái mới.
- Lý do.
- Người cập nhật.

## Validate
- Maintenance bắt buộc có lý do và ngày bắt đầu.
- Ngày kết thúc bảo trì phải sau ngày bắt đầu.
- Unavailable bắt buộc có lý do.
- Không cho update nếu trạng thái mới giống trạng thái hiện tại.

## JS cần có
- Load car status.
- Toggle form theo status.
- Validate nghiệp vụ.
- Confirm modal trước khi update.
- Sau khi update:
  - cập nhật badge.
  - thêm dòng vào status history.
  - toast `Cập nhật trạng thái xe thành công`.

## API gợi ý
```txt
GET /api/owner/cars/{carId}/status
PATCH /api/owner/cars/{carId}/status
GET /api/owner/cars/{carId}/status-history
```
