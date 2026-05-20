(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const params = new URLSearchParams(location.search);
  const bookingId = Number(params.get("bookingId"));
  const result = params.get("status") || "pending";
  const booking = DB.bookings.find((item) => item.id === bookingId && item.user_id === currentUser.id);
  const payment = U.paymentForBooking(bookingId);
  const root = U.byId("paymentResultRoot");

  // Frontend only displays payment result. Backend must verify gateway signature/checksum before updating payments.status.
  if (booking && payment && ["success", "failed", "pending"].includes(result)) {
    payment.status = result;
    payment.paid_at = result === "success" ? new Date().toISOString() : null;
    window.VivuCarSaveDB();
  }

  function render() {
    if (!booking || !payment) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy kết quả", text: "Booking hoặc payment không tồn tại.", href: "my-bookings.html", action: "Về danh sách đơn" });
      return;
    }
    const map = {
      success: ["success", "✓", "Thanh toán thành công", "Đơn vẫn ở trạng thái pending để chờ chủ xe xác nhận."],
      failed: ["danger", "!", "Thanh toán thất bại", "Bạn có thể thử lại hoặc chọn phương thức khác."],
      pending: ["warning", "…", "Thanh toán đang xử lý", "Vui lòng kiểm tra lại sau ít phút."]
    };
    const [tone, mark, title, text] = map[payment.status] || map.pending;
    root.innerHTML = `
      <article class="result-card">
        <div class="result-mark ${tone}">${mark}</div>
        <h1>${title}</h1>
        <p class="muted">${text}</p>
        <div class="summary-row"><span>Mã đơn</span><strong>#${booking.id}</strong></div>
        <div class="summary-row"><span>Mã giao dịch</span><strong>${payment.transaction_code}</strong></div>
        <div class="summary-row"><span>Số tiền</span><strong>${U.formatVnd(payment.amount)}</strong></div>
        <div class="modal-actions">
          <a class="btn btn-primary" href="${payment.status === "success" ? `booking-confirmation.html?bookingId=${booking.id}` : `payment-deposit.html?bookingId=${booking.id}`}">${payment.status === "success" ? "Xem xác nhận" : "Thử thanh toán lại"}</a>
          <a class="btn btn-secondary" href="my-bookings.html">Về đơn thuê</a>
          <a class="btn btn-ghost" href="home.html">Trang chủ</a>
        </div>
      </article>`;
    U.renderToast("Đã cập nhật kết quả thanh toán.", tone);
  }

  render();
})();
