window.VivuCarDB = {
  users: [
    { id: 1, google_id: null, email: "admin@vivucar.vn", full_name: "Nguyễn Minh Quân", avatar_url: "", role: "admin", is_blocked: false, created_at: "2026-05-01T08:00:00+07:00" },
    { id: 2, google_id: "g_102", email: "han.owner@vivucar.vn", full_name: "Trần Gia Hân", avatar_url: "", role: "car_owner", is_blocked: false, created_at: "2026-05-04T09:30:00+07:00" },
    { id: 3, google_id: "g_103", email: "lam.owner@vivucar.vn", full_name: "Lê Duy Lâm", avatar_url: "", role: "car_owner", is_blocked: false, created_at: "2026-05-07T14:20:00+07:00" },
    { id: 4, google_id: "g_104", email: "bao.tram@gmail.com", full_name: "Phạm Bảo Trâm", avatar_url: "", role: "user", is_blocked: false, created_at: "2026-05-10T11:00:00+07:00" },
    { id: 5, google_id: "g_105", email: "kiet.nguyen@gmail.com", full_name: "Nguyễn Tuấn Kiệt", avatar_url: "", role: "user", is_blocked: true, created_at: "2026-05-11T16:10:00+07:00" },
    { id: 6, google_id: "g_106", email: "mai.khanh@gmail.com", full_name: "Đỗ Mai Khanh", avatar_url: "", role: "user", is_blocked: false, created_at: "2026-05-14T10:15:00+07:00" }
  ],
  car_types: [
    { id: 1, name: "Sedan" }, { id: 2, name: "SUV" }, { id: 3, name: "MPV" }, { id: 4, name: "Electric" }
  ],
  cars: [
    { id: 1, owner_id: 2, brand: "Toyota", model: "Vios", type_id: 1, license_plate: "43A-12345", year: 2023, color: "Trắng", seats: 5, kilometers_driven: 21680, transmission: "automatic", fuel_type: "gasoline", price_per_day: 720000, price_per_hours: 95000, address: "Hải Châu, Đà Nẵng", description: "Xe sạch, tiết kiệm nhiên liệu.", status: "available", blocked_reason: null, created_at: "2026-05-02T08:00:00+07:00" },
    { id: 2, owner_id: 2, brand: "Mazda", model: "CX-5", type_id: 2, license_plate: "43A-66881", year: 2022, color: "Xám", seats: 5, kilometers_driven: 35240, transmission: "automatic", fuel_type: "gasoline", price_per_day: 980000, price_per_hours: 135000, address: "Sơn Trà, Đà Nẵng", description: "SUV phù hợp gia đình.", status: "rented", blocked_reason: null, created_at: "2026-05-03T08:00:00+07:00" },
    { id: 3, owner_id: 3, brand: "VinFast", model: "VF e34", type_id: 4, license_plate: "43A-90517", year: 2024, color: "Xanh", seats: 5, kilometers_driven: 8940, transmission: "cvt", fuel_type: "electric", price_per_day: 890000, price_per_hours: 120000, address: "Ngũ Hành Sơn, Đà Nẵng", description: "Xe điện, vận hành êm.", status: "maintenance", blocked_reason: "Bảo dưỡng định kỳ", created_at: "2026-05-05T08:00:00+07:00" },
    { id: 4, owner_id: 3, brand: "Kia", model: "Carnival", type_id: 3, license_plate: "43A-44120", year: 2021, color: "Đen", seats: 7, kilometers_driven: 48210, transmission: "automatic", fuel_type: "diesel", price_per_day: 1450000, price_per_hours: 190000, address: "Thanh Khê, Đà Nẵng", description: "MPV rộng, nhiều tiện nghi.", status: "blocked", blocked_reason: "Vi phạm chính sách hình ảnh", created_at: "2026-05-06T08:00:00+07:00" },
    { id: 5, owner_id: 2, brand: "Honda", model: "City", type_id: 1, license_plate: "43A-77128", year: 2022, color: "Đỏ", seats: 5, kilometers_driven: 27420, transmission: "manual", fuel_type: "gasoline", price_per_day: 690000, price_per_hours: 88000, address: "Cẩm Lệ, Đà Nẵng", description: "Sedan nhỏ gọn.", status: "available", blocked_reason: null, created_at: "2026-05-08T08:00:00+07:00" },
    { id: 6, owner_id: 3, brand: "Ford", model: "Everest", type_id: 2, license_plate: "43A-58392", year: 2020, color: "Bạc", seats: 7, kilometers_driven: 56610, transmission: "automatic", fuel_type: "diesel", price_per_day: 1320000, price_per_hours: 175000, address: "Liên Chiểu, Đà Nẵng", description: "SUV cao, phù hợp đi xa.", status: "available", blocked_reason: null, created_at: "2026-05-09T08:00:00+07:00" },
    { id: 7, owner_id: 2, brand: "Hyundai", model: "Accent", type_id: 1, license_plate: "43A-20477", year: 2021, color: "Xanh", seats: 5, kilometers_driven: 33480, transmission: "cvt", fuel_type: "gasoline", price_per_day: 640000, price_per_hours: 82000, address: "Hải Châu, Đà Nẵng", description: "Xe dễ lái, nội thất gọn gàng.", status: "available", blocked_reason: null, created_at: "2026-05-12T08:00:00+07:00" },
    { id: 8, owner_id: 3, brand: "Toyota", model: "Innova", type_id: 3, license_plate: "43A-81902", year: 2019, color: "Nâu", seats: 7, kilometers_driven: 73120, transmission: "manual", fuel_type: "gasoline", price_per_day: 920000, price_per_hours: 124000, address: "Ngũ Hành Sơn, Đà Nẵng", description: "Xe rộng, phù hợp nhóm gia đình.", status: "rented", blocked_reason: null, created_at: "2026-05-13T08:00:00+07:00" }
  ],
  car_images: [
    { id: 1, car_id: 1, image_url: "https://picsum.photos/seed/vivucar-vios/280/180", is_primary: true },
    { id: 2, car_id: 2, image_url: "https://picsum.photos/seed/vivucar-cx5/280/180", is_primary: true },
    { id: 3, car_id: 3, image_url: "https://picsum.photos/seed/vivucar-vfe34/280/180", is_primary: true },
    { id: 4, car_id: 4, image_url: "https://picsum.photos/seed/vivucar-carnival/280/180", is_primary: true },
    { id: 5, car_id: 5, image_url: "https://picsum.photos/seed/vivucar-city/280/180", is_primary: true },
    { id: 6, car_id: 6, image_url: "https://picsum.photos/seed/vivucar-everest/280/180", is_primary: true },
    { id: 7, car_id: 7, image_url: "https://picsum.photos/seed/vivucar-accent/280/180", is_primary: true },
    { id: 8, car_id: 8, image_url: "https://picsum.photos/seed/vivucar-innova/280/180", is_primary: true },
    { id: 9, car_id: 1, image_url: "https://picsum.photos/seed/vivucar-vios-side/280/180", is_primary: false },
    { id: 10, car_id: 1, image_url: "https://picsum.photos/seed/vivucar-vios-cabin/280/180", is_primary: false }
  ],
  user_documents: [
    { id: 1, user_id: 4, document_type: "avatar", file_name: "bao-tram.webp", file_url: "https://picsum.photos/seed/bao-tram/160/160", verified: true, created_at: "2026-05-11T09:00:00+07:00" },
    { id: 2, user_id: 4, document_type: "license_front", file_name: "license-front.webp", file_url: "https://picsum.photos/seed/license-front/420/260", verified: true, created_at: "2026-05-12T09:00:00+07:00" },
    { id: 3, user_id: 4, document_type: "license_back", file_name: "license-back.webp", file_url: "https://picsum.photos/seed/license-back/420/260", verified: true, created_at: "2026-05-12T09:02:00+07:00" },
    { id: 4, user_id: 6, document_type: "avatar", file_name: "mai-khanh.webp", file_url: "https://picsum.photos/seed/mai-khanh/160/160", verified: true, created_at: "2026-05-14T10:00:00+07:00" },
    { id: 5, user_id: 6, document_type: "license_front", file_name: "khanh-front.webp", file_url: "https://picsum.photos/seed/khanh-front/420/260", verified: false, created_at: "2026-05-15T10:00:00+07:00" }
  ],
  vouchers: [
    { id: 1, name: "Khuyến mãi hè Đà Nẵng", code: "SUMMER2026", discount_type: "percentage", discount_value: 12, min_order_amount: 500000, max_discount: 250000, quantity: 100, expires_at: "2026-06-30T23:59:00+07:00", created_at: "2026-05-01T08:00:00+07:00" },
    { id: 2, name: "Ưu đãi ngày thường", code: "WEEKDAY80", discount_type: "fixed", discount_value: 80000, min_order_amount: 400000, max_discount: 80000, quantity: 120, expires_at: "2026-05-10T23:59:00+07:00", created_at: "2026-04-01T08:00:00+07:00" },
    { id: 3, name: "Khách VIP", code: "VIPDRIVE", discount_type: "percentage", discount_value: 18, min_order_amount: 900000, max_discount: 420000, quantity: 40, expires_at: "2026-08-01T23:59:00+07:00", created_at: "2026-05-10T08:00:00+07:00" },
    { id: 4, name: "Khách mới", code: "NEWCAR50", discount_type: "fixed", discount_value: 50000, min_order_amount: 300000, max_discount: 50000, quantity: 80, expires_at: "2026-07-01T23:59:00+07:00", created_at: "2026-05-13T08:00:00+07:00" }
  ],
  voucher_usages: [
    { id: 1, voucher_id: 1, user_id: 4, booking_id: 1, used_at: "2026-05-18T11:00:00+07:00" },
    { id: 2, voucher_id: 1, user_id: 6, booking_id: 2, used_at: "2026-05-17T12:00:00+07:00" },
    { id: 3, voucher_id: 2, user_id: 4, booking_id: 3, used_at: "2026-05-04T08:00:00+07:00" },
    { id: 4, voucher_id: 3, user_id: 6, booking_id: 4, used_at: "2026-05-19T09:20:00+07:00" }
  ],
  bookings: [
    { id: 1, user_id: 4, car_id: 1, pickup_datetime: "2026-05-18T09:00:00+07:00", return_datetime: "2026-05-20T09:00:00+07:00", pickup_address: "Hải Châu", total_amount: 1420000, voucher_id: 1, status: "completed", created_at: "2026-05-16T08:00:00+07:00" },
    { id: 2, user_id: 6, car_id: 2, pickup_datetime: "2026-05-19T08:00:00+07:00", return_datetime: "2026-05-21T08:00:00+07:00", pickup_address: "Sơn Trà", total_amount: 1960000, voucher_id: 1, status: "approved", created_at: "2026-05-17T08:00:00+07:00" },
    { id: 3, user_id: 4, car_id: 5, pickup_datetime: "2026-05-05T10:00:00+07:00", return_datetime: "2026-05-06T10:00:00+07:00", pickup_address: "Cẩm Lệ", total_amount: 690000, voucher_id: 2, status: "cancelled", created_at: "2026-05-03T08:00:00+07:00" },
    { id: 4, user_id: 6, car_id: 4, pickup_datetime: "2026-05-22T09:00:00+07:00", return_datetime: "2026-05-24T09:00:00+07:00", pickup_address: "Thanh Khê", total_amount: 2900000, voucher_id: 3, status: "pending", created_at: "2026-05-19T08:00:00+07:00" }
  ],
  reviews: [
    { id: 1, booking_id: 1, reviewer_id: 4, car_id: 1, rating: 5, comment: "Xe sạch, giao nhận đúng giờ, chạy rất ổn trong nội thành.", created_at: "2026-05-20T10:00:00+07:00" },
    { id: 2, booking_id: 2, reviewer_id: 6, car_id: 2, rating: 4, comment: "Xe rộng và êm, chủ xe phản hồi nhanh. Nội thất cần vệ sinh kỹ hơn một chút.", created_at: "2026-05-21T14:00:00+07:00" },
    { id: 3, booking_id: 3, reviewer_id: 4, car_id: 5, rating: 4, comment: "Honda City tiết kiệm nhiên liệu, phù hợp đi công việc ngắn ngày.", created_at: "2026-05-07T14:00:00+07:00" }
  ],
  payments: [
    { id: 1, booking_id: 1, method: "vnpay", amount: 1420000, status: "success", transaction_code: "VNP2401", paid_at: "2026-05-16T08:30:00+07:00" },
    { id: 2, booking_id: 2, method: "momo", amount: 1960000, status: "pending", transaction_code: "MM2398", paid_at: null },
    { id: 3, booking_id: 3, method: "cash", amount: 690000, status: "refunded", transaction_code: "CS2386", paid_at: "2026-05-03T09:00:00+07:00" },
    { id: 4, booking_id: 4, method: "vnpay", amount: 2900000, status: "pending", transaction_code: "VNP2404", paid_at: null }
  ],
  handover_inspections: [
    { id: 1, booking_id: 1, inspection_type: "pre_rental", notes: "Xe sạch, nhiên liệu đầy, không ghi nhận hư hỏng mới.", confirmed_at: "2026-05-18T08:45:00+07:00", created_at: "2026-05-18T08:35:00+07:00" },
    { id: 2, booking_id: 1, inspection_type: "post_rental", notes: "Khách trả xe đúng giờ, tình trạng xe ổn định.", confirmed_at: "2026-05-20T09:15:00+07:00", created_at: "2026-05-20T09:05:00+07:00" },
    { id: 3, booking_id: 2, inspection_type: "pre_rental", notes: "Đã bàn giao xe và xác nhận ảnh hiện trạng.", confirmed_at: "2026-05-19T07:50:00+07:00", created_at: "2026-05-19T07:45:00+07:00" }
  ],
  daily_revenue_snapshots: [
    { id: 0, snapshot_date: "2026-05-13", total_bookings: 7, completed_bookings: 4, cancelled_bookings: 1, gross_revenue: 6900000, net_revenue: 6420000, deposit_collected: 1800000, generated_at: "2026-05-13T23:59:00+07:00" },
    { id: 1, snapshot_date: "2026-05-14", total_bookings: 8, completed_bookings: 5, cancelled_bookings: 1, gross_revenue: 8200000, net_revenue: 7740000, deposit_collected: 2100000, generated_at: "2026-05-14T23:59:00+07:00" },
    { id: 2, snapshot_date: "2026-05-15", total_bookings: 11, completed_bookings: 7, cancelled_bookings: 1, gross_revenue: 12400000, net_revenue: 11720000, deposit_collected: 3200000, generated_at: "2026-05-15T23:59:00+07:00" },
    { id: 3, snapshot_date: "2026-05-16", total_bookings: 9, completed_bookings: 6, cancelled_bookings: 2, gross_revenue: 9600000, net_revenue: 9040000, deposit_collected: 2700000, generated_at: "2026-05-16T23:59:00+07:00" },
    { id: 4, snapshot_date: "2026-05-17", total_bookings: 14, completed_bookings: 9, cancelled_bookings: 1, gross_revenue: 15100000, net_revenue: 14380000, deposit_collected: 4100000, generated_at: "2026-05-17T23:59:00+07:00" },
    { id: 5, snapshot_date: "2026-05-18", total_bookings: 13, completed_bookings: 8, cancelled_bookings: 1, gross_revenue: 13700000, net_revenue: 12960000, deposit_collected: 3900000, generated_at: "2026-05-18T23:59:00+07:00" },
    { id: 6, snapshot_date: "2026-05-19", total_bookings: 10, completed_bookings: 6, cancelled_bookings: 1, gross_revenue: 10800000, net_revenue: 10150000, deposit_collected: 3100000, generated_at: "2026-05-19T23:59:00+07:00" }
  ],
  export_jobs: []
};

const storedVivuCarDB = localStorage.getItem("vivucar_mock_db");
if (storedVivuCarDB) {
  try {
    Object.assign(window.VivuCarDB, JSON.parse(storedVivuCarDB));
  } catch (error) {
    console.warn("Cannot restore mock DB", error);
  }
}
window.VivuCarSaveDB = function saveDB() {
  localStorage.setItem("vivucar_mock_db", JSON.stringify(window.VivuCarDB));
};
window.VivuCarDB.handover_inspections ||= [];
window.VivuCarDB.handover_inspections.forEach((inspection) => {
  inspection.inspected_by ||= 2;
  inspection.odometer_km ||= 0;
  inspection.damage_notes ||= "";
  inspection.note ||= inspection.notes || "";
  delete inspection.notes;
});
window.VivuCarDB.inspection_images ||= [
  { id: 1, inspection_id: 1, url: "https://picsum.photos/seed/pre-rental-vios/420/260", caption: "Ảnh trước thuê", uploaded_at: "2026-05-18T08:40:00+07:00" },
  { id: 2, inspection_id: 2, url: "https://picsum.photos/seed/post-rental-vios/420/260", caption: "Ảnh sau thuê", uploaded_at: "2026-05-20T09:10:00+07:00" }
];
window.VivuCarDB.incident_reports ||= [
  { id: 1, booking_id: 2, reported_by: 6, title: "Cần kiểm tra phụ phí vệ sinh", description: "Khách cần VivuCar kiểm tra lại ghi chú vệ sinh sau chuyến đi trước khi tất toán.", status: "in_review", created_at: "2026-05-21T15:00:00+07:00" }
];
window.VivuCarDB.incident_images ||= [
  { id: 1, incident_id: 1, url: "https://picsum.photos/seed/incident-proof-2/420/260", caption: "Minh chứng sự cố", uploaded_at: "2026-05-21T15:05:00+07:00" }
];
// UI-only field. Not present in current DB schema. Requires migration before backend integration.
window.VivuCarDB.return_requests ||= [];
// UI-only field. Not present in current DB schema. Requires migration before backend integration.
window.VivuCarDB.car_status_history ||= [];
// UI-only field. Not present in current DB schema. Requires migration before backend integration.
["users", "cars", "vouchers"].forEach((collectionName) => {
  window.VivuCarDB[collectionName]?.forEach((item) => {
    item.deleted_at ||= null;
  });
});
// UI-only admin metadata. Not present in current DB schema. Requires migration before backend integration.
window.VivuCarDB.users?.forEach((user) => {
  user.admin_note ||= "";
  user.lock_history ||= [];
  user.last_login_at ||= null;
});
// UI-only moderation fields. Current DB schema does not include content/document moderation status or rejection reason.
(window.VivuCarDB.reviews || []).forEach((review) => {
  review.moderation_status ||= review.rating <= 2 ? "pending" : "approved";
  review.reported ||= false;
  review.hidden ||= false;
  review.deleted_at ||= null;
});
(window.VivuCarDB.incident_reports || []).forEach((report) => {
  report.moderation_status ||= ["open", "in_review"].includes(report.status) ? "reported" : "approved";
  report.hidden ||= false;
  report.deleted_at ||= null;
});
(window.VivuCarDB.user_documents || []).forEach((document) => {
  if (["license_front", "license_back"].includes(document.document_type)) {
    document.moderation_status ||= document.verified ? "approved" : "pending";
    document.rejection_reason ||= "";
  }
});
window.VivuCarDB.chat_sessions ||= [
  { id: 1, user_id: 4, booking_id: 1, session_type: "ai", status: "escalated", assigned_to: null, escalated_at: "2026-05-20T10:20:00+07:00", closed_at: null, created_at: "2026-05-20T10:05:00+07:00" },
  { id: 2, user_id: 6, booking_id: 2, session_type: "live", status: "open", assigned_to: 2, escalated_at: "2026-05-21T09:10:00+07:00", closed_at: null, created_at: "2026-05-21T09:00:00+07:00" },
  { id: 3, user_id: 4, booking_id: 3, session_type: "ai", status: "closed", assigned_to: 2, escalated_at: null, closed_at: "2026-05-07T16:00:00+07:00", created_at: "2026-05-07T15:20:00+07:00" }
];
window.VivuCarDB.chat_messages ||= [
  { id: 1, session_id: 1, sender_id: 4, role: "user", content: "Tôi cần hỗ trợ về trả xe.", created_at: "2026-05-20T10:06:00+07:00" },
  { id: 2, session_id: 1, sender_id: null, role: "assistant", content: "Tôi đã ghi nhận. Bạn có thể gửi yêu cầu trả xe từ chi tiết đơn.", created_at: "2026-05-20T10:06:20+07:00" },
  { id: 3, session_id: 1, sender_id: null, role: "system", content: "Cuộc trò chuyện đã chuyển cho người hỗ trợ.", created_at: "2026-05-20T10:20:00+07:00" },
  { id: 4, session_id: 2, sender_id: 6, role: "user", content: "Chủ xe cho tôi hỏi thời gian nhận xe sáng mai.", created_at: "2026-05-21T09:01:00+07:00" },
  { id: 5, session_id: 2, sender_id: 2, role: "user", content: "Tôi sẽ chuẩn bị xe trước 7:45 tại Sơn Trà.", created_at: "2026-05-21T09:12:00+07:00" }
];

