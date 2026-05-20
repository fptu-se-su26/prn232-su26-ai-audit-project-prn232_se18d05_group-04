(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const root = U.byId("myBookingsRoot");
  const state = { filter: "all", keyword: "", sort: "newest" };

  function records() {
    return DB.bookings
      .filter((booking) => booking.user_id === currentUser.id)
      .map((booking) => {
        const car = DB.cars.find((item) => item.id === booking.car_id);
        const payment = U.paymentForBooking(booking.id);
        const inspections = U.inspectionsForBooking(booking.id);
        return { booking, car, payment, inspections, ui: U.resolveBookingUiState(booking, payment, inspections) };
      });
  }

  function applyFilters(items) {
    const keyword = state.keyword.trim().toLowerCase();
    let output = items.filter(({ booking, car, ui }) => {
      const haystack = `${booking.id} ${car?.brand} ${car?.model} ${car?.license_plate}`.toLowerCase();
      const filterOk = state.filter === "all" || ui.key === state.filter || booking.status === state.filter;
      return filterOk && (!keyword || haystack.includes(keyword));
    });
    output.sort((a, b) => {
      if (state.sort === "oldest") return new Date(a.booking.created_at) - new Date(b.booking.created_at);
      if (state.sort === "pickup") return new Date(a.booking.pickup_datetime) - new Date(b.booking.pickup_datetime);
      if (state.sort === "amount") return b.booking.total_amount - a.booking.total_amount;
      return new Date(b.booking.created_at) - new Date(a.booking.created_at);
    });
    return output;
  }

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Trang chủ", href: "home.html" },
      { label: "Đơn thuê" }
    ]);
    const rows = applyFilters(records());
    root.innerHTML = `
      <div class="booking-toolbar">
        <div class="chip-row" id="bookingTabs">
          ${[
            ["all", "Tất cả"],
            ["payment_pending", "Chờ xử lý"],
            ["handover_pending", "Đã xác nhận"],
            ["renting", "Đang thuê/chờ trả"],
            ["completed", "Hoàn tất"],
            ["cancelled", "Đã hủy"]
          ].map(([key, label]) => `<button class="chip ${state.filter === key ? "active" : ""}" data-filter="${key}" type="button">${label}</button>`).join("")}
        </div>
        <div class="form-grid max-w-[520px]">
          <input id="bookingSearch" placeholder="Tìm theo mã đơn, xe, biển số" value="${state.keyword}">
          <select id="bookingSort">
            <option value="newest" ${state.sort === "newest" ? "selected" : ""}>Mới nhất</option>
            <option value="oldest" ${state.sort === "oldest" ? "selected" : ""}>Cũ nhất</option>
            <option value="pickup" ${state.sort === "pickup" ? "selected" : ""}>Ngày nhận gần nhất</option>
            <option value="amount" ${state.sort === "amount" ? "selected" : ""}>Giá trị cao nhất</option>
          </select>
        </div>
      </div>
      <div class="booking-list">
        ${rows.length ? rows.map(card).join("") : U.renderEmptyState({ title: "Chưa có đơn phù hợp", text: "Thử đổi bộ lọc hoặc tìm xe để tạo booking mới.", href: "search.html", action: "Tìm xe" })}
      </div>`;
    bind();
  }

  function card({ booking, car, payment, ui }) {
    const paid = payment?.status === "success";
    return `
      <article class="booking-card-wide">
        <img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}">
        <div>
          <div class="car-meta"><span>#${booking.id}</span>${U.statusBadge(ui.tone, ui.key, ui.label)}${U.renderStatusBadge("neutral", booking.status)}</div>
          <h2>${U.carTitle(car)}</h2>
          <p class="muted">${U.formatDateTime(booking.pickup_datetime)} - ${U.formatDateTime(booking.return_datetime)} · ${car.license_plate}</p>
          <strong>${U.formatVnd(booking.total_amount)}</strong>
        </div>
        <div class="booking-actions">
          <a class="btn btn-secondary btn-sm" href="booking-detail.html?bookingId=${booking.id}">Chi tiết</a>
          ${!paid ? `<a class="btn btn-primary btn-sm" href="payment-deposit.html?bookingId=${booking.id}">Thanh toán</a>` : ""}
          ${U.canCancelBooking(booking) ? `<button class="btn btn-danger btn-sm" type="button" data-cancel="${booking.id}">Hủy đơn</button>` : ""}
          ${booking.status === "completed" ? `<a class="btn btn-ghost btn-sm" href="post-trip-review.html?bookingId=${booking.id}&carId=${car.id}">Đánh giá</a>` : ""}
        </div>
      </article>`;
  }

  function bind() {
    document.querySelectorAll("[data-filter]").forEach((button) => button.addEventListener("click", () => {
      state.filter = button.dataset.filter;
      render();
    }));
    U.byId("bookingSearch")?.addEventListener("input", (event) => {
      state.keyword = event.target.value;
      render();
    });
    U.byId("bookingSort")?.addEventListener("change", (event) => {
      state.sort = event.target.value;
      render();
    });
    document.querySelectorAll("[data-cancel]").forEach((button) => button.addEventListener("click", () => {
      window.VivuCarBookingCancellation.openCancelBookingModal(button.dataset.cancel, render);
    }));
  }

  render();
})();

