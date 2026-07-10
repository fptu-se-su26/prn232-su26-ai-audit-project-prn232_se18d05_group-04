(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const state = { keyword: "", status: "", type_id: "", fuel_type: "", transmission: "", tab: "all", page: 1, pageSize: 10 };
  let selectedCarId = null;
  let forceDeleteCarId = null;

  function ownerName(id) { return DB.users.find((user) => user.id === id)?.full_name || "Không rõ"; }
  function typeName(id) { return DB.car_types.find((type) => type.id === id)?.name || "Chưa phân loại"; }
  function primaryImage(carId) { return DB.car_images.find((image) => image.car_id === carId && image.is_primary)?.image_url || "https://picsum.photos/seed/vivucar-car/280/180"; }
  function activeCars() { return DB.cars.filter((car) => !car.deleted_at); }
  function trashCars() { return DB.cars.filter((car) => car.deleted_at); }

  function ensureTrashUi() {
    if (!document.getElementById("carForceDeleteModal")) {
      document.body.insertAdjacentHTML("beforeend", `
        <div class="modal-backdrop" id="carForceDeleteModal">
          <div class="modal">
            <div class="modal-header"><h2>Xóa vĩnh viễn phương tiện</h2><button class="icon-button" type="button" data-close-modal>×</button></div>
            <p class="muted">Hành động này không thể hoàn tác. Phương tiện sẽ bị xóa khỏi dữ liệu mock hiện tại.</p>
            <div class="modal-actions">
              <button class="btn btn-secondary" id="btnCancelForceDeleteCar" type="button">Hủy</button>
              <button class="btn btn-danger" id="btnConfirmForceDeleteCar" type="button">Xóa vĩnh viễn</button>
            </div>
          </div>
        </div>
      `);
    }
  }

  function renderTypeOptions() {
    const typeFilter = document.getElementById("typeFilter");
    if (typeFilter.dataset.rendered === "true") return;
    typeFilter.insertAdjacentHTML("beforeend", DB.car_types.map((type) => `<option value="${type.id}">${type.name}</option>`).join(""));
    typeFilter.dataset.rendered = "true";
  }

  function tabCount(key) {
    if (key === "trash") return trashCars().length;
    if (key === "available") return activeCars().filter((car) => car.status === "available").length;
    if (key === "hidden") return activeCars().filter((car) => ["blocked", "maintenance"].includes(car.status)).length;
    return activeCars().length;
  }

  function renderTabs() {
    return tabCount("trash");
  }

  function filteredCars() {
    const source = state.tab === "trash" ? trashCars() : activeCars();
    return source.filter((car) => {
      const keyword = `${car.license_plate} ${car.brand} ${car.model}`.toLowerCase();
      const tabMatch = state.tab === "all" || state.tab === "trash"
        || (state.tab === "available" && car.status === "available")
        || (state.tab === "hidden" && ["blocked", "maintenance"].includes(car.status));
      return tabMatch
        && (!state.keyword || keyword.includes(state.keyword.toLowerCase()))
        && (!state.status || car.status === state.status)
        && (!state.type_id || String(car.type_id) === state.type_id)
        && (!state.fuel_type || car.fuel_type === state.fuel_type)
        && (!state.transmission || car.transmission === state.transmission);
    });
  }

  function renderSummary() {
    const cars = activeCars();
    const rows = [
      ["Tổng số xe", cars.length],
      ["Đang rảnh", cars.filter((c) => c.status === "available").length],
      ["Đang thuê", cars.filter((c) => c.status === "rented").length],
      ["Bảo trì", cars.filter((c) => c.status === "maintenance").length],
      ["Đã khóa", cars.filter((c) => c.status === "blocked").length]
    ];
    document.getElementById("carSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderCars() {
    renderTabs();
    const filtered = filteredCars();
    const page = U.paginate(filtered, state.page, state.pageSize);
    state.page = page.page;
    renderSummary();
    const empty = document.getElementById("carsEmpty");
    empty.classList.toggle("hidden", filtered.length > 0);
    empty.querySelector("h3").textContent = state.tab === "trash" ? "Thùng rác trống" : "Không tìm thấy phương tiện";
    empty.querySelector("p").textContent = state.tab === "trash"
      ? "Các đối tượng bị xoá mềm sẽ xuất hiện tại đây."
      : "Thử đặt lại bộ lọc hoặc kiểm tra trạng thái xe trong hệ thống.";
    document.querySelector(".table-wrap").classList.toggle("hidden", filtered.length === 0);
    document.querySelector(".pagination").classList.toggle("hidden", filtered.length === 0);
    document.getElementById("carsTableBody").innerHTML = page.items.map((car) => {
      const badgeKind = car.status === "available" ? "success" : car.status === "rented" ? "info" : car.status === "maintenance" ? "warning" : "danger";
      const actions = state.tab === "trash"
        ? U.renderActionMenu([
          { label: "Khôi phục", attrs: { "data-restore-car": car.id } },
          { label: "Xóa vĩnh viễn", attrs: { "data-force-delete-car": car.id }, variant: "danger" }
        ])
        : U.renderActionMenu([
          { label: "Xem chi tiết", href: `admin-car-form.html?id=${car.id}` },
          { label: "Chỉnh sửa", href: `admin-car-form.html?id=${car.id}` },
          car.status === "blocked"
            ? { label: "Mở khóa", attrs: { "data-unblock-car": car.id } }
            : { label: "Ẩn", attrs: { "data-block-car": car.id }, variant: "danger" },
          { label: "Xóa mềm", attrs: { "data-soft-delete-car": car.id }, variant: "danger" }
        ]);
      return `<tr><td><img class="car-thumb" src="${primaryImage(car.id)}" alt="${car.brand} ${car.model}"></td><td class="mono">${car.license_plate}</td><td><strong>${car.brand} ${car.model}</strong><br><span class="muted">${C.TRANSMISSION_LABELS[car.transmission]} · ${C.FUEL_TYPE_LABELS[car.fuel_type]}</span></td><td>${typeName(car.type_id)}</td><td>${car.year}</td><td>${car.seats}</td><td>${U.formatVnd(car.price_per_day)}</td><td>${U.statusBadge(badgeKind, car.status, C.CAR_STATUS_LABELS[car.status])}</td><td>${ownerName(car.owner_id)}</td><td class="actions">${actions}</td></tr>`;
    }).join("");
    document.getElementById("paginationInfo").textContent = `Hiển thị ${filtered.length ? page.start + 1 : 0}-${Math.min(page.start + state.pageSize, filtered.length)} trên ${filtered.length} xe`;
    document.getElementById("pageNumbers").innerHTML = Array.from({ length: page.totalPages }, (_, index) => `<button class="btn ${index + 1 === state.page ? "btn-primary" : "btn-secondary"} btn-sm" data-page="${index + 1}">${index + 1}</button>`).join("");
  }

  function applyFilters() {
    state.keyword = document.getElementById("searchCarInput").value.trim();
    state.status = document.getElementById("statusFilter").value;
    state.type_id = document.getElementById("typeFilter").value;
    state.fuel_type = document.getElementById("fuelFilter").value;
    state.transmission = document.getElementById("transmissionFilter").value;
    state.page = 1;
    renderCars();
  }

  function softDeleteItem(id) {
    const car = DB.cars.find((item) => item.id === Number(id));
    if (!car) return;
    const hasActiveBooking = DB.bookings.some((booking) => booking.car_id === car.id && ["pending", "approved"].includes(booking.status));
    if (hasActiveBooking) return U.showToast("Không thể xóa xe vì đang có đơn thuê chưa hoàn tất.");
    car.deleted_at = new Date().toISOString();
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast("Đã chuyển phương tiện vào thùng rác.");
    renderCars();
  }

  function restoreItem(id) {
    const car = DB.cars.find((item) => item.id === Number(id));
    if (!car) return;
    car.deleted_at = null;
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast("Đã khôi phục phương tiện.");
    renderCars();
  }

  function forceDeleteItem(id) {
    const carId = Number(id);
    DB.cars = DB.cars.filter((car) => car.id !== carId);
    DB.car_images = DB.car_images.filter((image) => image.car_id !== carId);
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.closeModal("carForceDeleteModal");
    U.showToast("Đã xóa vĩnh viễn phương tiện.");
    renderCars();
  }

  ensureTrashUi();
  document.getElementById("btnFilter").addEventListener("click", applyFilters);
  document.getElementById("btnReset").addEventListener("click", () => { document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = ""); applyFilters(); });
  document.getElementById("btnResetEmptyCars")?.addEventListener("click", () => { document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = ""); applyFilters(); });
  document.getElementById("pageSize").addEventListener("change", (event) => { state.pageSize = Number(event.target.value); state.page = 1; renderCars(); });
  document.getElementById("prevPage").addEventListener("click", () => { state.page -= 1; renderCars(); });
  document.getElementById("nextPage").addEventListener("click", () => { state.page += 1; renderCars(); });
  document.getElementById("pageNumbers").addEventListener("click", (event) => { const btn = event.target.closest("[data-page]"); if (btn) { state.page = Number(btn.dataset.page); renderCars(); } });
  document.addEventListener("click", (event) => {
    const block = event.target.closest("[data-block-car]");
    const unblock = event.target.closest("[data-unblock-car]");
    const softDelete = event.target.closest("[data-soft-delete-car]");
    const restore = event.target.closest("[data-restore-car]");
    const forceDelete = event.target.closest("[data-force-delete-car]");
    if (softDelete) return softDeleteItem(softDelete.dataset.softDeleteCar);
    if (restore) return restoreItem(restore.dataset.restoreCar);
    if (forceDelete) {
      forceDeleteCarId = Number(forceDelete.dataset.forceDeleteCar);
      return U.openModal("carForceDeleteModal");
    }
    if (block) {
      const car = DB.cars.find((item) => item.id === Number(block.dataset.blockCar));
      const hasActiveBooking = DB.bookings.some((booking) => booking.car_id === car.id && ["pending", "approved"].includes(booking.status));
      if (hasActiveBooking) return U.showToast("Không thể khóa xe vì đang có đơn thuê chưa hoàn tất.");
      selectedCarId = car.id;
      document.getElementById("blockCarInfo").textContent = `${car.license_plate} · ${car.brand} ${car.model}`;
      U.openModal("blockCarModal");
    }
    if (unblock) {
      selectedCarId = Number(unblock.dataset.unblockCar);
      U.openModal("unblockCarModal");
    }
  });
  document.getElementById("btnCancelBlockCar").addEventListener("click", () => U.closeModal("blockCarModal"));
  document.getElementById("btnCancelUnblockCar").addEventListener("click", () => U.closeModal("unblockCarModal"));
  document.getElementById("btnCancelForceDeleteCar").addEventListener("click", () => U.closeModal("carForceDeleteModal"));
  document.getElementById("btnConfirmForceDeleteCar").addEventListener("click", () => forceDeleteItem(forceDeleteCarId));
  document.getElementById("btnConfirmBlockCar").addEventListener("click", () => {
    const reason = document.getElementById("blockReasonType").value;
    const note = document.getElementById("blockNote").value.trim();
    if (!reason || (reason === "Khác" && !note)) return U.showToast("Vui lòng chọn lý do và nhập ghi chú nếu cần.");
    const car = DB.cars.find((item) => item.id === selectedCarId);
    car.status = "blocked";
    car.blocked_reason = note || reason;
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.closeModal("blockCarModal");
    U.showToast("Đã khóa xe thành công.");
    renderCars();
  });
  document.getElementById("btnConfirmUnblockCar").addEventListener("click", () => {
    const car = DB.cars.find((item) => item.id === selectedCarId);
    car.status = "available";
    car.blocked_reason = null;
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.closeModal("unblockCarModal");
    U.showToast("Đã mở khóa xe thành công.");
    renderCars();
  });
  renderTypeOptions();
  renderCars();
})();
