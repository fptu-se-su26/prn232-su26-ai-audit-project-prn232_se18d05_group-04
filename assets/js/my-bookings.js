(function () {
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const root = U.byId("myBookingsRoot");
  const state = { filter: "all", keyword: "", sort: "newest" };
  
  let loadedBookings = [];

  async function fetchBookings() {
    try {
      const res = await Auth.fetchWithAuth(`${C.API_BASE_URL}/bookings/my-bookings?pageSize=100`);
      if (!res.ok) throw new Error("Lỗi khi tải dữ liệu");
      const data = await res.json();
      
      // Map API Response to UI format
      loadedBookings = data.map(b => ({
        id: b.id,
        carId: b.carId,
        carName: b.carName,
        carImage: b.carImageUrl || `https://ui-avatars.com/api/?name=${encodeURIComponent(b.carName)}&background=random`,
        licensePlate: b.licensePlate,
        pickupDatetime: b.startDateTime,
        returnDatetime: b.endDateTime,
        createdAt: b.createdAt,
        totalAmount: b.totalAmount,
        status: b.status,
        // Dựa vào status backend trả về để map giao diện (tạm thời)
        uiTone: getUiTone(b.status),
        uiLabel: getUiLabel(b.status),
        uiKey: getUiKey(b.status)
      }));
      renderList();
    } catch (e) {
      root.innerHTML = U.renderEmptyState({ title: "Lỗi tải dữ liệu", text: e.message, href: "home.html", action: "Quay lại" });
    }
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

  function applyFilters() {
    const keyword = state.keyword.trim().toLowerCase();
    let output = loadedBookings.filter((b) => {
      const haystack = `${b.id} ${b.carName} ${b.licensePlate}`.toLowerCase();
      const filterOk = state.filter === "all" || b.uiKey === state.filter || b.status.toLowerCase() === state.filter.toLowerCase();
      return filterOk && (!keyword || haystack.includes(keyword));
    });
    output.sort((a, b) => {
      if (state.sort === "oldest") return new Date(a.createdAt) - new Date(b.createdAt);
      if (state.sort === "pickup") return new Date(a.pickupDatetime) - new Date(b.pickupDatetime);
      if (state.sort === "amount") return b.totalAmount - a.totalAmount;
      return new Date(b.createdAt) - new Date(a.createdAt);
    });
    return output;
  }

  function renderList() {
    const rows = applyFilters();
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

  function card(b) {
    const isCompletedOrCancelled = b.status === "Completed" || b.status === "Cancelled" || b.status === "Rejected";
    return `
      <article class="booking-card-wide">
        <img src="${b.carImage}" alt="${b.carName}">
        <div>
          <div class="car-meta"><span>#${b.id}</span>${U.statusBadge(b.uiTone, b.uiKey, b.uiLabel)}</div>
          <h2>${b.carName}</h2>
          <p class="muted">${U.formatDateTime(b.pickupDatetime)} - ${U.formatDateTime(b.returnDatetime)} · ${b.licensePlate}</p>
          <strong>${U.formatVnd(b.totalAmount)}</strong>
        </div>
        <div class="booking-actions">
          <a class="btn btn-secondary btn-sm" href="booking-detail.html?bookingId=${b.id}">Chi tiết</a>
          ${b.status === "PendingApproval" ? `<a class="btn btn-primary btn-sm" href="payment-deposit.html?bookingId=${b.id}">Thanh toán</a>` : ""}
          ${!isCompletedOrCancelled ? `<button class="btn btn-danger btn-sm" type="button" data-cancel="${b.id}">Hủy đơn</button>` : ""}
          ${b.status === "Completed" ? `<a class="btn btn-ghost btn-sm" href="post-trip-review.html?bookingId=${b.id}&carId=${b.carId}">Đánh giá</a>` : ""}
        </div>
      </article>`;
  }

  function bind() {
    document.querySelectorAll("[data-filter]").forEach((button) => button.addEventListener("click", () => {
      state.filter = button.dataset.filter;
      renderList();
    }));
    U.byId("bookingSearch")?.addEventListener("input", (event) => {
      state.keyword = event.target.value;
      renderList();
    });
    U.byId("bookingSort")?.addEventListener("change", (event) => {
      state.sort = event.target.value;
      renderList();
    });
    document.querySelectorAll("[data-cancel]").forEach((button) => button.addEventListener("click", () => {
      window.VivuCarBookingCancellation.openCancelBookingModal(button.dataset.cancel, fetchBookings);
    }));
  }

  document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
    { label: "Trang chủ", href: "home.html" },
    { label: "Đơn thuê" }
  ]);
  
  root.innerHTML = "<p class='p-8 text-center text-gray-500'>Đang tải dữ liệu đơn thuê...</p>";
  fetchBookings();
})();
