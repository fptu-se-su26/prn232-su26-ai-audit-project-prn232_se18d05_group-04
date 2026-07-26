(function () {
  const DB = window.VivuCarDB, U = window.VivuCarUtils, Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser(); if (!currentUser) return;
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const booking = DB.bookings.find((item) => item.id === bookingId && item.user_id === currentUser.id);
  const car = DB.cars.find((item) => item.id === booking?.car_id);
  const root = U.byId("returnCarRequestRoot");
  const images = [];

  function canReturn() {
    const inspections = U.inspectionsForBooking(bookingId);
    return String(booking?.status).toLowerCase() === "approved" && inspections.some((item) => item.inspection_type === "pre_rental") && !inspections.some((item) => item.inspection_type === "post_rental");
  }

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([{ label: "Đơn thuê", href: "/Booking/MyBookings" }, { label: "Trả xe" }]);
    if (!booking || !car) return root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy đơn", text: "Đơn không thuộc tài khoản hiện tại.", href: "/Booking/MyBookings", action: "Về đơn thuê" });
    if (!canReturn()) return root.innerHTML = U.renderEmptyState({ title: "Chưa thể gửi yêu cầu trả xe", text: "Yêu cầu cần booking approved, đã có pre_rental inspection và chưa có post_rental inspection.", href: `/Booking/Detail?bookingId=${bookingId}`, action: "Xem chi tiết" });
    root.innerHTML = `
      <div class="checkout-layout">
        <form class="checkout-main" id="returnForm">
          <section class="checkout-section"><h1>Gửi yêu cầu trả xe</h1><div class="checkout-car"><img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}"><div><h2>${U.carTitle(car)}</h2><p class="muted">#${booking.id} · ${car.license_plate}</p><p>${U.formatDateTime(booking.pickup_datetime)} - ${U.formatDateTime(booking.return_datetime)}</p></div></div></section>
          <section class="checkout-section"><h2>Thông tin trả xe</h2><div class="form-grid">
            <label>Thời gian trả thực tế<input id="actual_return_datetime" type="datetime-local" required></label>
            <label>Địa điểm trả xe<input id="return_location" value="${booking.pickup_address || car.address}" required></label>
            <label>Người bàn giao<input id="handover_person" placeholder="Tên người bàn giao"></label>
            <label>Số điện thoại liên hệ<input id="contact_phone" type="tel" placeholder="09xxxxxxxx" required></label>
            <label class="full">Ghi chú<textarea id="customer_return_note" rows="3"></textarea></label>
          </div><button class="chip" id="usePickup" type="button">Trả tại điểm nhận xe</button></section>
          <section class="checkout-section"><h2>Ảnh minh chứng</h2><input id="return_images" type="file" accept="image/*" multiple><div id="returnPreview" class="image-preview-grid"></div><p class="muted">Tối đa 8 ảnh, mỗi ảnh không quá 5MB.</p></section>
          <button class="btn btn-primary btn-full" type="submit">Gửi yêu cầu trả xe</button>
        </form>
        <aside class="summary-card"><h2>Lưu ý</h2><p class="return-note">Sau khi gửi, chủ xe sẽ kiểm tra tình trạng xe. Frontend chỉ lưu draft yêu cầu trả xe, không tạo booking.status mới.</p><a class="btn btn-secondary btn-full" href="/Booking/Detail?bookingId=${booking.id}">Quay lại chi tiết đơn</a></aside>
      </div>`;
    U.byId("actual_return_datetime").value = new Date().toISOString().slice(0, 16);
    U.byId("usePickup").onclick = () => U.byId("return_location").value = booking.pickup_address || car.address;
    U.byId("return_images").onchange = previewImages;
    U.byId("returnForm").onsubmit = submit;
  }

  function previewImages(event) {
    [...event.target.files].slice(0, 8).forEach((file) => {
      if (!file.type.startsWith("image/") || file.size > 5 * 1024 * 1024) return U.renderToast("Ảnh không hợp lệ.", "danger");
      const reader = new FileReader();
      reader.onload = () => { images.push({ url: reader.result, caption: file.name }); U.byId("returnPreview").insertAdjacentHTML("beforeend", `<img src="${reader.result}" alt="Ảnh trả xe">`); };
      reader.readAsDataURL(file);
    });
  }

  function submit(event) {
    event.preventDefault();
    if (!/^(0|\+84)[0-9]{9}$/.test(U.byId("contact_phone").value.trim())) return U.renderToast("Số điện thoại chưa hợp lệ.", "danger");
    if (new Date(U.byId("actual_return_datetime").value) < new Date(booking.pickup_datetime)) return U.renderToast("Thời gian trả không được trước lúc nhận xe.", "danger");
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    DB.return_requests.push({ id: Date.now(), booking_id: booking.id, actual_return_datetime: new Date(U.byId("actual_return_datetime").value).toISOString(), return_location: U.byId("return_location").value.trim(), handover_person: U.byId("handover_person").value.trim(), contact_phone: U.byId("contact_phone").value.trim(), note: U.byId("customer_return_note").value.trim(), images, created_at: new Date().toISOString() });
    window.VivuCarSaveDB();
    U.renderToast("Đã gửi yêu cầu trả xe.", "success");
    setTimeout(() => location.href = `/Booking/Detail?bookingId=${booking.id}`, 400);
  }

  render();
})();

