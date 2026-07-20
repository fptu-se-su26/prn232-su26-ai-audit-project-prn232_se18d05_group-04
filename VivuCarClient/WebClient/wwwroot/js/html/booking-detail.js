(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const root = U.byId("bookingDetailRoot");

  function getRecord() {
    const booking = DB.bookings.find((item) => item.id === bookingId && item.user_id === currentUser.id);
    const car = DB.cars.find((item) => item.id === booking?.car_id);
    const user = DB.users.find((item) => item.id === booking?.user_id);
    const payment = U.paymentForBooking(bookingId);
    const inspections = U.inspectionsForBooking(bookingId);
    const voucher = DB.vouchers.find((item) => item.id === booking?.voucher_id);
    return { booking, car, user, payment, inspections, voucher, ui: U.resolveBookingUiState(booking, payment, inspections) };
  }

  function render() {
    const { booking, car, user, payment, inspections, voucher, ui } = getRecord();
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "/Booking/MyBookings" },
      { label: `#${bookingId}` }
    ]);
    if (!booking || !car) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy đơn", text: "Đơn không tồn tại hoặc không thuộc tài khoản hiện tại.", href: "/Booking/MyBookings", action: "Về danh sách đơn" });
      return;
    }
    root.innerHTML = `
      <div class="detail-document">
        <section class="checkout-main">
          <div class="checkout-section">
            <div class="booking-toolbar">
              <div>
                <span class="eyebrow">Booking #${booking.id}</span>
                <h1>${U.carTitle(car)}</h1>
                <p class="muted">${ui.label}</p>
              </div>
              <div class="chip-row">${U.statusBadge(ui.tone, ui.key, ui.label)}${U.renderStatusBadge("neutral", booking.status)}</div>
            </div>
            <div class="checkout-car">
              <img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}">
              <div class="info-grid">
                ${info("Biển số", car.license_plate)}
                ${info("Địa điểm nhận", booking.pickup_address)}
                ${info("Nhận xe", U.formatDateTime(booking.pickup_datetime))}
                ${info("Trả xe", U.formatDateTime(booking.return_datetime))}
                ${info("Tổng tiền", U.formatVnd(booking.total_amount))}
                ${info("Voucher", voucher ? voucher.code : "Không áp dụng")}
              </div>
            </div>
          </div>
          <div class="checkout-section">
            <h2>Payment</h2>
            <div class="info-grid">
              ${info("Method", payment?.method || "Chưa có")}
              ${info("Status", payment?.status || "Chưa có")}
              ${info("Amount", U.formatVnd(payment?.amount || 0))}
              ${info("Paid at", payment?.paid_at ? U.formatDateTime(payment.paid_at) : "Chưa thanh toán")}
            </div>
          </div>
          <div class="checkout-section">
            <h2>Inspection</h2>
            ${inspections.length ? inspections.map((item) => `<div class="summary-row"><span>${item.inspection_type}</span><strong>${U.formatDateTime(item.confirmed_at)}</strong></div><p class="muted">${item.notes}</p>`).join("") : `<p class="muted">Chưa có biên bản bàn giao.</p>`}
          </div>
        </section>
        <aside class="summary-card">
          <h2>Timeline</h2>
          <div class="timeline">${timeline(booking, payment, inspections)}</div>
          <div class="booking-actions mt-3.5">
            ${payment?.status !== "success" ? `<a class="btn btn-primary btn-sm" href="/Payment/Deposit?bookingId=${booking.id}">Thanh toán</a>` : ""}
            ${U.canCancelBooking(booking) ? `<button class="btn btn-danger btn-sm" type="button" id="cancelBooking">Hủy đơn</button>` : ""}
            ${booking.status === "approved" && inspections.some((item) => item.inspection_type === "pre_rental") && !inspections.some((item) => item.inspection_type === "post_rental") ? `<a class="btn btn-primary btn-sm" href="return-car-request.html?bookingId=${booking.id}">Trả xe</a>` : ""}
            <a class="btn btn-secondary btn-sm" href="booking-contract.html?bookingId=${booking.id}">Hợp đồng</a>
            ${booking.status === "completed" ? `<a class="btn btn-ghost btn-sm" href="post-trip-review.html?bookingId=${booking.id}&carId=${car.id}">Đánh giá sau chuyến</a>` : ""}
            <a class="btn btn-ghost btn-sm" href="post-trip-incident-report.html?bookingId=${booking.id}">Báo cáo sự cố</a>
          </div>
        </aside>
      </div>`;
    U.byId("cancelBooking")?.addEventListener("click", () => window.VivuCarBookingCancellation.openCancelBookingModal(booking.id, render));
  }

  function info(label, value) {
    return `<div class="info-cell"><span>${label}</span><strong>${value}</strong></div>`;
  }

  function timeline(booking, payment, inspections) {
    const items = [
      ["Tạo đơn", U.formatDateTime(booking.created_at)],
      payment?.paid_at ? ["Thanh toán", U.formatDateTime(payment.paid_at)] : null,
      ...inspections.map((item) => [item.inspection_type, U.formatDateTime(item.confirmed_at)]),
      booking.status === "completed" ? ["Hoàn tất", U.formatDateTime(booking.return_datetime)] : null,
      booking.status === "cancelled" ? ["Đã hủy", "Trạng thái booking.status = cancelled"] : null
    ].filter(Boolean);
    return items.map(([title, text]) => `<div class="timeline-item"><strong>${title}</strong><p>${text}</p></div>`).join("");
  }

  render();
})();


