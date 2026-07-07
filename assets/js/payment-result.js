(function () {
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const params = new URLSearchParams(location.search);
  const bookingId = Number(params.get("bookingId"));
  const resultStatus = params.get("status") || "pending";
  const root = U.byId("paymentResultRoot");

  let paymentData = null;

  async function fetchPaymentStatus() {
    try {
      const res = await Auth.fetchWithAuth(`${C.API_BASE_URL}/payments/status/${bookingId}`);
      if (!res.ok) throw new Error("Không thể tải trạng thái thanh toán.");
      paymentData = await res.json();
      render();
    } catch (e) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy kết quả", text: "Booking hoặc payment không tồn tại.", href: "my-bookings.html", action: "Về danh sách đơn" });
    }
  }

  function render() {
    if (!paymentData) return;
    const p = paymentData;
    // Backend process the callback and update DB, so we just use the status returned or from URL
    const finalStatus = resultStatus !== "pending" ? resultStatus : p.status.toLowerCase();

    const map = {
      success: ["success", "✓", "Thanh toán thành công", "Đơn đã được xác nhận và chờ bàn giao xe."],
      failed: ["danger", "!", "Thanh toán thất bại", "Bạn có thể thử lại hoặc chọn phương thức khác."],
      pending: ["warning", "…", "Thanh toán đang xử lý", "Vui lòng kiểm tra lại sau ít phút."]
    };
    const [tone, mark, title, text] = map[finalStatus] || map.pending;
    
    root.innerHTML = `
      <article class="result-card">
        <div class="result-mark ${tone}">${mark}</div>
        <h1>${title}</h1>
        <p class="muted">${text}</p>
        <div class="summary-row"><span>Mã đơn</span><strong>#${bookingId}</strong></div>
        <div class="summary-row"><span>Mã giao dịch</span><strong>${p.transactionCode || "Đang cập nhật"}</strong></div>
        <div class="summary-row"><span>Số tiền cọc</span><strong>${U.formatVnd(p.amount)}</strong></div>
        <div class="modal-actions">
          <a class="btn btn-primary" href="${finalStatus === "success" ? `booking-confirmation.html?bookingId=${bookingId}` : `payment-deposit.html?bookingId=${bookingId}`}">${finalStatus === "success" ? "Xem xác nhận" : "Thử thanh toán lại"}</a>
          <a class="btn btn-secondary" href="my-bookings.html">Về đơn thuê</a>
          <a class="btn btn-ghost" href="home.html">Trang chủ</a>
        </div>
      </article>`;
    U.renderToast("Đã cập nhật kết quả thanh toán.", tone);
  }

  root.innerHTML = "<p class='p-8 text-center text-gray-500'>Đang kiểm tra giao dịch...</p>";
  fetchPaymentStatus();
})();
