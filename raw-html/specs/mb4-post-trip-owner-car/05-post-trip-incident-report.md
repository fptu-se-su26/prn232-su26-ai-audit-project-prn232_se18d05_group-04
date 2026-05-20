# VivuCar UI - Post Trip Incident Report

## Mục tiêu trang
Khách hàng gửi báo cáo sự cố sau chuyến đi nếu có tranh chấp, hư hỏng, phụ phí không hợp lý hoặc vấn đề liên quan đến phương tiện/dịch vụ.

## File HTML gợi ý
`post-trip-incident-report.html`

## URL gợi ý
```txt
post-trip-incident-report.html?bookingId=B001
```

## Layout
- Header user.
- Card thông tin đơn.
- Form báo cáo sự cố.
- Upload ảnh minh chứng.
- Confirmation modal.

## Booking summary
Hiển thị:
- Mã đơn.
- Xe.
- Chủ xe.
- Thời gian thuê.
- Trạng thái đơn.
- Trạng thái xử lý nếu đã gửi sự cố.

## Incident type
Select:
- id: `incidentType`
- options:
  - `Xe có hư hỏng trước khi nhận`
  - `Phụ phí không hợp lý`
  - `Tranh chấp nhiên liệu`
  - `Tranh chấp số km`
  - `Chủ xe không hỗ trợ`
  - `Vấn đề thanh toán`
  - `Khác`

## Severity
Radio:
- id: `severityLow`
  - value: `LOW`
  - label: `Nhẹ`
- id: `severityMedium`
  - value: `MEDIUM`
  - label: `Trung bình`
- id: `severityHigh`
  - value: `HIGH`
  - label: `Nghiêm trọng`

## Description
Textarea:
- id: `incidentDescription`
- placeholder: `Mô tả chi tiết sự cố, thời điểm xảy ra và mong muốn xử lý`
- maxlength: 2000

Counter:
- id: `incidentDescriptionCounter`

## Requested resolution
Select:
- id: `requestedResolution`
- options:
  - `Yêu cầu hoàn tiền`
  - `Yêu cầu xem xét phụ phí`
  - `Yêu cầu hỗ trợ từ Admin`
  - `Chỉ ghi nhận phản hồi`
  - `Khác`

## Evidence upload
Input:
- id: `incidentImages`
- type: `file`
- accept: `image/*`
- multiple

Preview:
- id: `incidentImagePreviewGrid`

Note:
- `Upload ảnh/video minh chứng nếu có. Tối đa 10 file ảnh.`

## Contact preference
Fields:
- Số điện thoại liên hệ:
  - id: `incidentContactPhone`
- Email:
  - id: `incidentContactEmail`
- Select:
  - id: `preferredContactMethod`
  - options:
    - `Điện thoại`
    - `Email`
    - `Tin nhắn trên hệ thống`

## Buttons
- `Hủy`
  - id: `btnCancelIncident`
- `Gửi báo cáo sự cố`
  - id: `btnSubmitIncidentReport`

## Confirmation modal
- id: `incidentConfirmModal`
- Text:
  - `Báo cáo sẽ được gửi đến VivuCar và người cho thuê để xử lý.`
- Buttons:
  - `Kiểm tra lại`
  - `Xác nhận gửi`

## Validate
- Bắt buộc chọn loại sự cố.
- Bắt buộc chọn mức độ.
- Mô tả tối thiểu 20 ký tự.
- Nếu yêu cầu hoàn tiền, nên có ít nhất 1 ảnh minh chứng.
- Số điện thoại hợp lệ.
- Tối đa 10 ảnh, mỗi ảnh tối đa 5MB.

## JS cần có
- Đếm ký tự mô tả.
- Preview ảnh.
- Validate form.
- Submit FormData.
- Sau khi gửi:
  - trạng thái incident: `OPEN`
  - redirect về booking detail.

## API gợi ý
```txt
POST /api/incidents
```

Body:
- multipart/form-data
- fields:
  - bookingId
  - incidentType
  - severity
  - incidentDescription
  - requestedResolution
  - contactPhone
  - contactEmail
  - preferredContactMethod
  - evidenceImages[]
