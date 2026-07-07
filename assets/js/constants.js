window.VivuCarConstants = {
  API_BASE_URL: "https://localhost:7005/api",
  USER_ROLES: { USER: "user", CAR_OWNER: "car_owner", ADMIN: "admin" },
  USER_ROLE_LABELS: { user: "Khách thuê", car_owner: "Chủ xe", admin: "Quản trị viên" },
  TRANSMISSION_LABELS: { manual: "Số sàn", automatic: "Tự động", cvt: "CVT" },
  FUEL_TYPE_LABELS: { gasoline: "Xăng", diesel: "Dầu", electric: "Điện", hybrid: "Hybrid" },
  CAR_STATUS_LABELS: { available: "Đang rảnh", rented: "Đang thuê", maintenance: "Bảo trì", blocked: "Đã khóa" },
  BOOKING_STATUS_LABELS: { pending: "Chờ xử lý", approved: "Đã xác nhận", rejected: "Đã từ chối", completed: "Hoàn tất", cancelled: "Đã hủy" },
  PAYMENT_METHOD_LABELS: { vnpay: "VNPay", momo: "MoMo", cash: "Tiền mặt" },
  PAYMENT_STATUS_LABELS: { pending: "Chờ thanh toán", success: "Thành công", failed: "Thất bại", refunded: "Đã hoàn tiền" },
  DISCOUNT_TYPE_LABELS: { percentage: "Giảm theo phần trăm", fixed: "Giảm số tiền cố định" },
  EXPORT_STATUS_LABELS: { pending: "Đang chờ", processing: "Đang xử lý", done: "Hoàn tất", failed: "Thất bại" }
};
