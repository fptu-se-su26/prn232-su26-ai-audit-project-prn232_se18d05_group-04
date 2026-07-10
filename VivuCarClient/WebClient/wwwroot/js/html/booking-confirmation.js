(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const booking = DB.bookings.find((item) => item.id === bookingId && item.user_id === currentUser.id);
  const car = DB.cars.find((item) => item.id === booking?.car_id);
  const user = DB.users.find((item) => item.id === booking?.user_id);
  const payment = U.paymentForBooking(bookingId);
  const root = U.byId("bookingConfirmationRoot");

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "my-bookings.html" },
      { label: `#${bookingId}`, href: `booking-detail.html?bookingId=${bookingId}` },
      { label: "Xác nhận" }
    ]);
    if (!booking || !car || !user || !payment) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy biên lai", text: "Dữ liệu xác nhận chưa sẵn sàng.", href: "my-bookings.html", action: "Về danh sách đơn" });
      return;
    }
    root.innerHTML = `
      <div class="detail-document">
        <section class="checkout-section">
          <span class="eyebrow">Receipt</span>
          <h1>Xác nhận đơn thuê #${booking.id}</h1>
          <p class="muted">VivuCar đã ghi nhận payment ${payment.status}. Chủ xe sẽ xác nhận lịch thuê trên trạng thái booking gốc.</p>
          <div class="info-grid">
            <div class="info-cell"><span>Khách thuê</span><strong>${user.full_name}</strong></div>
            <div class="info-cell"><span>Email</span><strong>${user.email}</strong></div>
            <div class="info-cell"><span>Xe</span><strong>${U.carTitle(car)}</strong></div>
            <div class="info-cell"><span>Biển số</span><strong>${car.license_plate}</strong></div>
            <div class="info-cell"><span>Nhận xe</span><strong>${U.formatDateTime(booking.pickup_datetime)}</strong></div>
            <div class="info-cell"><span>Trả xe</span><strong>${U.formatDateTime(booking.return_datetime)}</strong></div>
          </div>
        </section>
        <aside class="summary-card">
          <h2>Biên lai cọc</h2>
          <div class="summary-row"><span>Method</span><strong>${payment.method}</strong></div>
          <div class="summary-row"><span>Amount</span><strong>${U.formatVnd(payment.amount)}</strong></div>
          <div class="summary-row"><span>Transaction</span><strong>${payment.transaction_code}</strong></div>
          <div class="summary-row"><span>Paid at</span><strong>${payment.paid_at ? U.formatDateTime(payment.paid_at) : "Chưa thanh toán"}</strong></div>
          <button class="btn btn-secondary btn-full" id="downloadReceipt">Tải biên lai TXT</button>
          <button class="btn btn-ghost btn-full" onclick="window.print()">In biên lai</button>
        </aside>
      </div>`;
    U.byId("downloadReceipt").addEventListener("click", downloadReceipt);
  }

  function downloadReceipt() {
    const text = `VivuCar receipt\nBooking #${booking.id}\nCustomer: ${user.full_name}\nCar: ${U.carTitle(car)}\nPayment: ${payment.method} ${payment.status}\nAmount: ${U.formatVnd(payment.amount)}\nTransaction: ${payment.transaction_code}`;
    U.downloadBlob(new Blob([text], { type: "text/plain;charset=utf-8" }), `vivucar-receipt-${booking.id}.txt`);
    U.renderToast("Đã tải biên lai mock.", "success");
  }

  render();
})();
