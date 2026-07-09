(function () {
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const root = U.byId("bookingDetailRoot");
  let bookingData = null;

  async function fetchBookingDetail() {
    try {
      const res = await Auth.fetchWithAuth(`${C.API_BASE_URL}/bookings/${bookingId}`);
      if (!res.ok) throw new Error("Không thể tải chi tiết đơn thuê.");
      bookingData = await res.json();
      render();
    } catch (e) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy đơn", text: "Đơn không tồn tại hoặc không thuộc tài khoản hiện tại.", href: "my-bookings.html", action: "Về danh sách đơn" });
    }
  }

  function render() {
    if (!bookingData) return;
    const b = bookingData;
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "my-bookings.html" },
      { label: `#${bookingId}` }
    ]);
    
    // Tạm thời map các status của BE sang UI Tone/Label (Tương lai có thể dùng hàm dùng chung)
    const uiTone = getUiTone(b.status);
    const uiLabel = getUiLabel(b.status);
    const uiKey = getUiKey(b.status);
    
    const carImage = b.carImageUrl || `https://ui-avatars.com/api/?name=${encodeURIComponent(b.carName)}&background=random`;

    root.innerHTML = `
      <div class="detail-document">
        <section class="checkout-main">
          <div class="checkout-section">
            <div class="booking-toolbar">
              <div>
                <span class="eyebrow">Booking #${b.id} ${b.bookingCode ? `(${b.bookingCode})` : ""}</span>
                <h1>${b.carName}</h1>
                <p class="muted">${uiLabel}</p>
              </div>
              <div class="chip-row">${U.statusBadge(uiTone, uiKey, uiLabel)}</div>
            </div>
            <div class="checkout-car">
              <img src="${carImage}" alt="${b.carName}">
              <div class="info-grid">
                ${info("Biển số", b.licensePlate)}
                ${info("Địa điểm nhận", b.pickupLocation)}
                ${info("Địa điểm trả", b.returnLocation)}
                ${info("Nhận xe", U.formatDateTime(b.startDateTime))}
                ${info("Trả xe", U.formatDateTime(b.endDateTime))}
                ${info("Voucher", b.voucherCode || "Không áp dụng")}
              </div>
            </div>
            <div class="info-grid mt-4">
                ${info("Giá gốc", U.formatVnd(b.basePrice))}
                ${info("Bảo hiểm", U.formatVnd(b.insuranceFee))}
                ${info("Giao nhận", U.formatVnd(b.deliveryFee))}
                ${info("Giảm giá", "-" + U.formatVnd(b.discountAmount))}
                <div class="info-cell col-span-2"><span>Tổng tiền thanh toán</span><strong class="text-lg text-primary">${U.formatVnd(b.totalAmount)}</strong></div>
                ${info("Đã cọc", U.formatVnd(b.depositAmount))}
                ${info("Còn lại", U.formatVnd(b.remainingAmount))}
            </div>
          </div>
          
          ${b.driverInfo ? `
          <div class="checkout-section">
            <h2>Thông tin người lái</h2>
            <div class="info-grid">
              ${info("Họ tên", b.driverInfo.fullName)}
              ${info("Số điện thoại", b.driverInfo.phoneNumber)}
              ${info("CCCD/CMND", b.driverInfo.citizenIdNumber)}
              ${info("Số GPLX", b.driverInfo.driverLicenseNumber)}
            </div>
          </div>
          ` : ""}
        </section>
        <aside class="summary-card">
          <h2>Timeline</h2>
          <div class="timeline">${timeline(b)}</div>
          <div class="booking-actions mt-3.5">
            ${b.status === "PendingApproval" ? `<a class="btn btn-primary btn-sm" href="payment-deposit.html?bookingId=${b.id}">Thanh toán cọc</a>` : ""}
            ${b.status === "PendingApproval" || b.status === "Approved" ? `<button class="btn btn-danger btn-sm" type="button" id="cancelBooking">Hủy đơn</button>` : ""}
            ${b.status === "Approved" ? `<a class="btn btn-primary btn-sm" href="return-car-request.html?bookingId=${b.id}">Trả xe</a>` : ""}
            ${b.contractPdfUrl ? `<a class="btn btn-secondary btn-sm" href="${b.contractPdfUrl}" target="_blank">Xem hợp đồng</a>` : ""}
            ${b.status === "Completed" ? `<a class="btn btn-ghost btn-sm" href="post-trip-review.html?bookingId=${b.id}&carId=${b.carId}">Đánh giá sau chuyến</a>` : ""}
            <a class="btn btn-ghost btn-sm" href="post-trip-incident-report.html?bookingId=${b.id}">Báo cáo sự cố</a>
          </div>
        </aside>
      </div>`;
    U.byId("cancelBooking")?.addEventListener("click", () => window.VivuCarBookingCancellation.openCancelBookingModal(bookingId, fetchBookingDetail));
  }

  function getUiTone(status) {
    if (status === "PendingApproval") return "warning";
    if (status === "Approved") return "info";
    if (status === "Completed") return "success";
    if (status === "Cancelled" || status === "Rejected") return "danger";
    return "neutral";
  }

  function getUiLabel(status) {
    if (status === "PendingApproval") return "Chờ xử lý";
    if (status === "Approved") return "Đã xác nhận";
    if (status === "Completed") return "Hoàn tất";
    if (status === "Cancelled") return "Đã hủy";
    if (status === "Rejected") return "Bị từ chối";
    return status;
  }

  function getUiKey(status) {
    if (status === "PendingApproval") return "payment_pending";
    if (status === "Approved") return "handover_pending";
    if (status === "Completed") return "completed";
    if (status === "Cancelled" || status === "Rejected") return "cancelled";
    return "all";
  }

  function info(label, value) {
    return `<div class="info-cell"><span>${label}</span><strong>${value}</strong></div>`;
  }

  function timeline(b) {
    const items = [
      ["Tạo đơn", U.formatDateTime(b.createdAt)],
      b.cancelledAt ? ["Đã hủy", U.formatDateTime(b.cancelledAt)] : null,
      b.cancellationReason ? ["Lý do", b.cancellationReason] : null
    ].filter(Boolean);
    return items.map(([title, text]) => `<div class="timeline-item"><strong>${title}</strong><p>${text}</p></div>`).join("");
  }

  root.innerHTML = "<p class='p-8 text-center text-gray-500'>Đang tải thông tin đơn thuê...</p>";
  fetchBookingDetail();
})();
