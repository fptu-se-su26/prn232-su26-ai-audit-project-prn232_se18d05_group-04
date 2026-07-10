(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const booking = DB.bookings.find((item) => item.id === bookingId && item.user_id === currentUser.id);
  const payment = U.paymentForBooking(bookingId);
  const car = DB.cars.find((item) => item.id === booking?.car_id);
  const root = U.byId("paymentDepositRoot");

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "my-bookings.html" },
      { label: `#${bookingId}`, href: `booking-detail.html?bookingId=${bookingId}` },
      { label: "Thanh toán cọc" }
    ]);
    if (!booking || !car || !payment) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy thanh toán", text: "Đường dẫn không hợp lệ.", href: "my-bookings.html", action: "Về danh sách đơn" });
      return;
    }
    const state = U.resolveBookingUiState(booking, payment, U.inspectionsForBooking(booking.id));
    root.innerHTML = `
      <div class="checkout-layout">
        <section class="checkout-main">
          <div class="checkout-section">
            <h1>Thanh toán tiền cọc</h1>
            <p class="muted">Đơn #${booking.id} · ${U.carTitle(car)} · ${state.label}</p>
            ${U.renderStatusBadge(state.tone, booking.status)}
          </div>
          <div class="checkout-section">
            <h2>Chọn phương thức</h2>
            <div class="payment-method-grid">
              ${["vnpay", "momo", "cash"].map((method) => `<label class="method-card"><input type="radio" name="method" value="${method}" ${payment.method === method ? "checked" : ""}><strong>${method === "cash" ? "Tiền mặt" : method === "momo" ? "MoMo" : "VNPay"}</strong><span class="muted">${method === "cash" ? "Thanh toán khi nhận xe" : "Thanh toán online mock"}</span></label>`).join("")}
            </div>
            <button class="btn btn-primary btn-full" id="payNow" type="button">${payment.status === "success" ? "Đã thanh toán" : "Tiếp tục thanh toán"}</button>
          </div>
        </section>
        <aside class="summary-card">
          <h2>Tóm tắt đơn</h2>
          <div class="summary-row"><span>Tổng tiền thuê</span><strong>${U.formatVnd(booking.total_amount)}</strong></div>
          <div class="summary-row"><span>Tiền cọc</span><strong>${U.formatVnd(payment.amount)}</strong></div>
          <div class="summary-row"><span>Trạng thái payment</span><strong>${payment.status}</strong></div>
          <div class="summary-total"><span>Cần thanh toán</span><strong>${payment.status === "success" ? U.formatVnd(0) : U.formatVnd(payment.amount)}</strong></div>
        </aside>
      </div>`;
    U.byId("payNow").addEventListener("click", payNow);
  }

  function payNow() {
    if (payment.status === "success") return location.href = `booking-confirmation.html?bookingId=${booking.id}`;
    const method = document.querySelector("input[name='method']:checked").value;
    payment.method = method;
    payment.transaction_code = `${method.toUpperCase()}${Date.now().toString().slice(-6)}`;
    if (method === "cash") {
      payment.status = "pending";
      payment.paid_at = null;
      window.VivuCarSaveDB();
      U.renderToast("Đã chọn thanh toán tiền mặt khi nhận xe.", "success");
      setTimeout(() => location.href = `booking-detail.html?bookingId=${booking.id}`, 350);
      return;
    }
    window.VivuCarSaveDB();
    location.href = `payment-result.html?bookingId=${booking.id}&status=success`;
  }

  render();
})();
