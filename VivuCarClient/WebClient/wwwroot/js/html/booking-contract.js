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
  const root = U.byId("bookingContractRoot");

  function render() {
    if (!booking || !car || !user) {
      root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy hợp đồng", text: "Đơn không tồn tại hoặc không thuộc tài khoản hiện tại.", href: "/Booking/MyBookings", action: "Về danh sách đơn" });
      return;
    }
    root.innerHTML = `
      <div class="contract-toolbar">
        <a class="btn btn-secondary" href="/Booking/Detail?bookingId=${booking.id}">Quay lại chi tiết đơn</a>
        <div class="chip-row">
          <button class="btn btn-ghost" type="button" onclick="window.print()">In hợp đồng</button>
          <button class="btn btn-primary" id="downloadContract" type="button">Tải PDF mock</button>
        </div>
      </div>
      <div class="contract-scroll">
        <article class="contract-page" id="contractPage">
          <h1>HỢP ĐỒNG THUÊ XE TỰ LÁI</h1>
          <p class="text-center">Số: VC-${booking.id}-${new Date(booking.created_at).getFullYear()}</p>
          <h2>1. Thông tin bên thuê</h2>
          <p><strong>Họ tên:</strong> ${user.full_name}</p>
          <p><strong>Email:</strong> ${user.email}</p>
          <h2>2. Thông tin xe</h2>
          <p><strong>Xe:</strong> ${U.carTitle(car)} · ${car.color} · ${car.seats} chỗ</p>
          <p><strong>Biển số:</strong> ${car.license_plate}</p>
          <p><strong>Nhiên liệu/Hộp số:</strong> ${car.fuel_type} / ${car.transmission}</p>
          <h2>3. Thời gian và địa điểm thuê</h2>
          <p><strong>Nhận xe:</strong> ${U.formatDateTime(booking.pickup_datetime)}</p>
          <p><strong>Trả xe:</strong> ${U.formatDateTime(booking.return_datetime)}</p>
          <p><strong>Địa điểm nhận xe:</strong> ${booking.pickup_address}</p>
          <h2>4. Giá trị thanh toán</h2>
          <p><strong>Tổng giá trị booking:</strong> ${U.formatVnd(booking.total_amount)}</p>
          <p><strong>Payment:</strong> ${payment ? `${payment.method} · ${payment.status} · ${U.formatVnd(payment.amount)}` : "Chưa có payment"}</p>
          <h2>5. Cam kết</h2>
          <p>Bên thuê cam kết sử dụng xe đúng mục đích, hoàn trả xe đúng thời gian, giữ nguyên hiện trạng và chịu trách nhiệm với các phát sinh theo chính sách VivuCar.</p>
          <div class="signature-grid">
            <div><strong>BÊN CHO THUÊ</strong><p>(Ký và ghi rõ họ tên)</p></div>
            <div><strong>BÊN THUÊ</strong><p>${user.full_name}</p></div>
          </div>
        </article>
      </div>`;
    U.byId("downloadContract").addEventListener("click", downloadContract);
  }

  function downloadContract() {
    const html = document.getElementById("contractPage").innerText;
    U.downloadBlob(new Blob([html], { type: "application/pdf" }), `vivucar-contract-${booking.id}.pdf`);
    U.renderToast("Đã tải hợp đồng mock.", "success");
  }

  render();
})();


