(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const Pricing = window.VivuCarBookingPricing;
  const Availability = window.VivuCarBookingAvailability;
  const params = new URLSearchParams(location.search);
  const carId = Number(params.get("carId"));
  const car = DB.cars.find((item) => item.id === carId);
  const root = U.byId("checkoutRoot");
  let appliedVoucher = null;
  const licenseUploads = {};

  function renderError(message) {
    root.innerHTML = U.renderEmptyState({ title: "Không thể đặt xe", text: message, href: "search.html", action: "Tìm xe khác" });
  }

  function render() {
    if (!car) return renderError("Không tìm thấy xe theo đường dẫn hiện tại.");
    if (car.status !== "available") return renderError("Xe này hiện không ở trạng thái available.");
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Trang chủ", href: "home.html" },
      { label: U.carTitle(car), href: `car-detail.html?carId=${car.id}` },
      { label: "Đặt xe" }
    ]);
    const tomorrow = new Date(Date.now() + 864e5).toISOString().slice(0, 16);
    const after = new Date(Date.now() + 3 * 864e5).toISOString().slice(0, 16);
    root.innerHTML = `
      <form class="checkout-main" id="checkoutForm">
        <section class="checkout-section">
          <h2>Xe đã chọn</h2>
          <div class="checkout-car">
            <img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}">
            <div>
              <h1>${U.carTitle(car)}</h1>
              <p class="muted">${car.address}</p>
              <div class="car-meta">
                <span>${car.seats} chỗ</span><span>${car.transmission}</span><span>${car.fuel_type}</span>
              </div>
            </div>
          </div>
        </section>
        <section class="checkout-section">
          <h2>Lịch thuê</h2>
          <div class="form-grid">
            <label>Nhận xe<input id="pickupDatetime" name="pickup_datetime" type="datetime-local" value="${tomorrow}" required></label>
            <label>Trả xe<input id="returnDatetime" name="return_datetime" type="datetime-local" value="${after}" required></label>
            <label class="full">Địa điểm nhận xe<input id="pickupAddress" name="pickup_address" value="${car.address}" required></label>
          </div>
          <div id="availabilityNote" class="availability-note">Đang kiểm tra lịch xe...</div>
        </section>
        <section class="checkout-section">
          <h2>Thông tin người lái</h2>
          <div class="form-grid">
            <label>Họ tên<input id="driverName" value="${currentUser.full_name}" required></label>
            <label>Email<input value="${currentUser.email}" disabled></label>
            <label class="full">Ghi chú bàn giao<textarea id="driverNote" rows="3" placeholder="Thời điểm thuận tiện, yêu cầu giao xe..."></textarea></label>
          </div>
          <!-- UI-only field. Not present in current DB schema. Requires migration before backend integration. -->
          <p class="muted">Nếu người lái khác người đặt, backend cần bổ sung driver profile riêng.</p>
        </section>
        <section class="checkout-section">
          <h2>Hồ sơ lái xe</h2>
          <div class="form-grid">
            <label>GPLX mặt trước<input id="licenseFront" type="file" accept="image/png,image/jpeg,image/webp"></label>
            <label>GPLX mặt sau<input id="licenseBack" type="file" accept="image/png,image/jpeg,image/webp"></label>
          </div>
          <div class="image-preview-grid" id="licensePreview"></div>
        </section>
        <section class="checkout-section">
          <h2>Voucher và thanh toán</h2>
          <div class="voucher-inline">
            <input id="voucherCode" placeholder="Nhập mã voucher">
            <button class="btn btn-secondary" id="applyVoucher" type="button">Áp dụng</button>
          </div>
          <div class="payment-method-grid mt-3">
            ${["vnpay", "momo", "cash"].map((method, index) => `<label class="method-card"><input type="radio" name="method" value="${method}" ${index === 0 ? "checked" : ""}><strong>${method === "cash" ? "Tiền mặt" : method === "momo" ? "MoMo" : "VNPay"}</strong><span class="muted">${method === "cash" ? "Thanh toán khi nhận xe" : "Chuyển sang trang kết quả mock"}</span></label>`).join("")}
          </div>
        </section>
        <button class="btn btn-primary btn-full" type="submit">Tạo đơn và tiếp tục thanh toán</button>
      </form>
      <aside class="summary-card" id="summaryCard"></aside>`;
    bind();
    updateAll();
  }

  function bind() {
    ["pickupDatetime", "returnDatetime"].forEach((id) => U.byId(id).addEventListener("change", updateAll));
    U.byId("applyVoucher").addEventListener("click", applyVoucher);
    ["licenseFront", "licenseBack"].forEach((id) => U.byId(id).addEventListener("change", previewLicense));
    U.byId("checkoutForm").addEventListener("submit", submitBooking);
  }

  function currentPricing() {
    return Pricing.calculate(car, U.byId("pickupDatetime").value, U.byId("returnDatetime").value, appliedVoucher);
  }

  function updateAll() {
    const pickup = U.byId("pickupDatetime").value;
    const returned = U.byId("returnDatetime").value;
    const availability = Availability.checkAvailability(car.id, pickup, returned);
    U.byId("availabilityNote").className = `availability-note ${availability.available ? "success" : "danger"}`;
    U.byId("availabilityNote").textContent = availability.reason;
    const pricing = currentPricing();
    U.byId("summaryCard").innerHTML = `
      <h2>Tóm tắt chi phí</h2>
      <p class="muted">${U.carTitle(car)} · ${Pricing.rentalDays(pickup, returned)} ngày</p>
      <div class="summary-row"><span>Giá thuê</span><strong>${U.formatVnd(pricing.subtotal)}</strong></div>
      <div class="summary-row"><span>Bảo hiểm thuê xe</span><strong>${U.formatVnd(pricing.insuranceFee)}</strong></div>
      <div class="summary-row"><span>Giao xe</span><strong>${U.formatVnd(pricing.deliveryFee)}</strong></div>
      <div class="summary-row"><span>Voucher</span><strong>-${U.formatVnd(pricing.discount)}</strong></div>
      <div class="summary-total"><span>Tổng thanh toán</span><strong>${U.formatVnd(pricing.total)}</strong></div>
      <p class="muted">Tiền cọc mock: ${U.formatVnd(Math.round(pricing.total * 0.3))}</p>`;
  }

  function applyVoucher() {
    const code = U.byId("voucherCode").value.trim().toUpperCase();
    const voucher = DB.vouchers.find((item) => item.code.toUpperCase() === code);
    if (!voucher) {
      appliedVoucher = null;
      U.renderToast("Không tìm thấy voucher.", "danger");
      updateAll();
      return;
    }
    const result = Pricing.calculateVoucher(voucher, currentPricing().subtotal);
    if (result.error) {
      U.renderToast(result.error, "danger");
      return;
    }
    appliedVoucher = voucher;
    U.renderToast("Đã áp dụng voucher.", "success");
    updateAll();
  }

  function previewLicense(event) {
    const file = event.target.files[0];
    if (!file) return;
    if (!["image/jpeg", "image/png", "image/webp"].includes(file.type) || file.size > 5 * 1024 * 1024) {
      event.target.value = "";
      U.renderToast("Ảnh phải là JPG/PNG/WEBP và không quá 5MB.", "danger");
      return;
    }
    const reader = new FileReader();
    reader.onload = () => {
      const type = event.target.id === "licenseFront" ? "license_front" : "license_back";
      licenseUploads[type] = { file_name: file.name, file_url: reader.result };
      U.byId("licensePreview").insertAdjacentHTML("beforeend", `<img src="${reader.result}" alt="Preview GPLX">`);
    };
    reader.readAsDataURL(file);
  }

  function submitBooking(event) {
    event.preventDefault();
    const pickup = U.byId("pickupDatetime").value;
    const returned = U.byId("returnDatetime").value;
    const availability = Availability.checkAvailability(car.id, pickup, returned);
    if (!availability.available) return U.renderToast("Không thể tạo booking vì lịch bị trùng.", "danger");
    const pricing = currentPricing();
    const id = Math.max(0, ...DB.bookings.map((item) => item.id)) + 1;
    const booking = {
      id,
      user_id: currentUser.id,
      car_id: car.id,
      pickup_datetime: new Date(pickup).toISOString(),
      return_datetime: new Date(returned).toISOString(),
      pickup_address: U.byId("pickupAddress").value.trim(),
      total_amount: pricing.total,
      voucher_id: appliedVoucher?.id || null,
      status: "pending",
      created_at: new Date().toISOString()
    };
    DB.bookings.push(booking);
    ["license_front", "license_back"].forEach((documentType) => {
      if (!licenseUploads[documentType]) return;
      const existing = DB.user_documents.find((doc) => doc.user_id === booking.user_id && doc.document_type === documentType);
      if (existing) {
        existing.file_name = licenseUploads[documentType].file_name;
        existing.file_url = licenseUploads[documentType].file_url;
        existing.verified = false;
        existing.created_at = new Date().toISOString();
        return;
      }
      DB.user_documents.push({ id: Math.max(0, ...DB.user_documents.map((item) => item.id)) + 1, user_id: booking.user_id, document_type: documentType, file_name: licenseUploads[documentType].file_name, file_url: licenseUploads[documentType].file_url, verified: false, created_at: new Date().toISOString() });
    });
    if (appliedVoucher) {
      DB.voucher_usages.push({ id: Math.max(0, ...DB.voucher_usages.map((item) => item.id)) + 1, voucher_id: appliedVoucher.id, user_id: booking.user_id, booking_id: id, used_at: new Date().toISOString() });
    }
    const method = document.querySelector("input[name='method']:checked").value;
    // UI-only interpretation. payments.amount is used as deposit amount in current frontend mock.
    DB.payments.push({ id: Math.max(0, ...DB.payments.map((item) => item.id)) + 1, booking_id: id, method, amount: Math.round(pricing.total * 0.3), status: "pending", transaction_code: `PAY${Date.now()}`, paid_at: null });
    window.VivuCarSaveDB();
    U.renderToast("Đã tạo đơn thuê.", "success");
    setTimeout(() => location.href = `payment-deposit.html?bookingId=${id}`, 350);
  }

  render();
})();

