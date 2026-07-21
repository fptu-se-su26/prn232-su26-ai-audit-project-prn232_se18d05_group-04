(function () {
  const DB = window.VivuCarDB, C = window.VivuCarConstants, U = window.VivuCarUtils, Auth = window.VivuCarAuth;
  const user = Auth.getCurrentUser();
  const avatar = DB.user_documents.find((d) => d.user_id === user.id && d.document_type === "avatar")?.file_url || user.avatar_url;
  const front = DB.user_documents.find((d) => d.user_id === user.id && d.document_type === "license_front");
  const back = DB.user_documents.find((d) => d.user_id === user.id && d.document_type === "license_back");
  const verified = front?.verified && back?.verified;
  const bookings = DB.bookings.filter((b) => b.user_id === user.id);
  const completed = bookings.filter((b) => b.status === "completed");
  const reviews = DB.reviews.filter((r) => r.reviewer_id === user.id);
  document.getElementById("profileRoot").innerHTML = `<section class="panel"><div class="profile-hero"><div class="profile-avatar">${avatar ? `<img class="profile-avatar" src="${avatar}" alt="${user.full_name}">` : user.full_name[0]}</div><div><h1>${user.full_name}</h1><p class="muted">${user.email}</p>${verified ? U.statusBadge("success", "verified", "Đã xác minh GPLX") : U.statusBadge("warning", "pending", "Chưa xác minh GPLX")}</div><div class="actions"><a class="btn btn-secondary" href="avatar-upload.html">Đổi ảnh đại diện</a><a class="btn btn-primary" href="profile-edit.html">Chỉnh sửa hồ sơ</a></div></div></section><section class="stats-grid"><article class="summary-card"><span>Tổng đơn</span><strong>${bookings.length}</strong></article><article class="summary-card"><span>Hoàn tất</span><strong>${completed.length}</strong></article><article class="summary-card"><span>Đánh giá</span><strong>${reviews.length}</strong></article><article class="summary-card"><span>Tổng chi tiêu</span><strong>${U.formatVnd(bookings.reduce((s,b)=>s+Number(b.total_amount),0))}</strong></article></section><section class="panel"><div class="panel-head"><h2>Thông tin cá nhân</h2></div><div class="form-card"><p><strong>Họ tên:</strong> ${user.full_name}</p><p><strong>Email:</strong> ${user.email}</p><p><strong>Vai trò:</strong> ${C.USER_ROLE_LABELS[user.role]}</p><p><strong>Ngày tham gia:</strong> ${U.formatDate(user.created_at)}</p><p class="muted">Phone/address/date_of_birth không có trong bảng users hiện tại.</p></div></section><section class="panel mt-3.5"><div class="panel-head"><h2>Giấy phép lái xe</h2><a class="btn btn-secondary btn-sm" href="driving-license.html">Upload GPLX</a></div><div class="form-card"><p>Trạng thái: ${verified ? U.statusBadge("success", "verified", "Đã xác minh") : front || back ? U.statusBadge("warning", "pending", "Chờ xác minh") : U.statusBadge("neutral", "none", "Chưa upload")}</p></div></section>`;
})();


