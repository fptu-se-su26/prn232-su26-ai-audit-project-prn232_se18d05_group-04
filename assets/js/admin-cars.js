(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const state = { keyword: "", status: "", type_id: "", fuel_type: "", transmission: "", page: 1, pageSize: 10 };
  let selectedCarId = null;

  function ownerName(id) { return DB.users.find((user) => user.id === id)?.full_name || "Không rõ"; }
  function typeName(id) { return DB.car_types.find((type) => type.id === id)?.name || "Chưa phân loại"; }
  function primaryImage(carId) { return DB.car_images.find((image) => image.car_id === carId && image.is_primary)?.image_url || "https://picsum.photos/seed/vivucar-car/280/180"; }

  function renderTypeOptions() {
    document.getElementById("typeFilter").insertAdjacentHTML("beforeend", DB.car_types.map((type) => `<option value="${type.id}">${type.name}</option>`).join(""));
  }

  function filteredCars() {
    return DB.cars.filter((car) => {
      const keyword = `${car.license_plate} ${car.brand} ${car.model}`.toLowerCase();
      return (!state.keyword || keyword.includes(state.keyword.toLowerCase()))
        && (!state.status || car.status === state.status)
        && (!state.type_id || String(car.type_id) === state.type_id)
        && (!state.fuel_type || car.fuel_type === state.fuel_type)
        && (!state.transmission || car.transmission === state.transmission);
    });
  }

  function renderSummary() {
    const rows = [
      ["Tổng số xe", DB.cars.length],
      ["Đang rảnh", DB.cars.filter((c) => c.status === "available").length],
      ["Đang thuê", DB.cars.filter((c) => c.status === "rented").length],
      ["Bảo trì", DB.cars.filter((c) => c.status === "maintenance").length],
      ["Đã khóa", DB.cars.filter((c) => c.status === "blocked").length]
    ];
    document.getElementById("carSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderCars() {
    const filtered = filteredCars();
    const page = U.paginate(filtered, state.page, state.pageSize);
    state.page = page.page;
    renderSummary();
    document.getElementById("carsEmpty").classList.toggle("hidden", filtered.length > 0);
    document.getElementById("carsTableBody").innerHTML = page.items.map((car) => {
      const badgeKind = car.status === "available" ? "success" : car.status === "rented" ? "info" : car.status === "maintenance" ? "warning" : "danger";
      return `<tr><td><img class="car-thumb" src="${primaryImage(car.id)}" alt="${car.brand} ${car.model}"></td><td class="mono">${car.license_plate}</td><td><strong>${car.brand} ${car.model}</strong><br><span class="muted">${C.TRANSMISSION_LABELS[car.transmission]} · ${C.FUEL_TYPE_LABELS[car.fuel_type]}</span></td><td>${typeName(car.type_id)}</td><td>${car.year}</td><td>${car.seats}</td><td>${U.formatVnd(car.price_per_day)}</td><td>${U.statusBadge(badgeKind, car.status, C.CAR_STATUS_LABELS[car.status])}</td><td>${ownerName(car.owner_id)}</td><td class="actions"><a class="btn btn-secondary btn-sm" href="admin-car-form.html?id=${car.id}">Sửa</a>${car.status === "blocked" ? `<button class="btn btn-primary btn-sm btn-unblock-car" data-id="${car.id}">Mở khóa</button>` : `<button class="btn btn-danger btn-sm btn-block-car" data-id="${car.id}">Khóa</button>`}</td></tr>`;
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

  document.getElementById("btnFilter").addEventListener("click", applyFilters);
  document.getElementById("btnReset").addEventListener("click", () => { document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = ""); applyFilters(); });
  document.getElementById("btnResetEmptyCars")?.addEventListener("click", () => { document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = ""); applyFilters(); });
  document.getElementById("pageSize").addEventListener("change", (event) => { state.pageSize = Number(event.target.value); state.page = 1; renderCars(); });
  document.getElementById("prevPage").addEventListener("click", () => { state.page -= 1; renderCars(); });
  document.getElementById("nextPage").addEventListener("click", () => { state.page += 1; renderCars(); });
  document.getElementById("pageNumbers").addEventListener("click", (event) => { const btn = event.target.closest("[data-page]"); if (btn) { state.page = Number(btn.dataset.page); renderCars(); } });
  document.addEventListener("click", (event) => {
    const block = event.target.closest(".btn-block-car");
    const unblock = event.target.closest(".btn-unblock-car");
    if (block) {
      const car = DB.cars.find((item) => item.id === Number(block.dataset.id));
      const hasActiveBooking = DB.bookings.some((booking) => booking.car_id === car.id && ["pending", "approved"].includes(booking.status));
      if (hasActiveBooking) return U.showToast("Không thể khóa xe vì đang có đơn thuê chưa hoàn tất.");
      selectedCarId = car.id;
      document.getElementById("blockCarInfo").textContent = `${car.license_plate} · ${car.brand} ${car.model}`;
      U.openModal("blockCarModal");
    }
    if (unblock) {
      selectedCarId = Number(unblock.dataset.id);
      U.openModal("unblockCarModal");
    }
  });
  document.getElementById("btnCancelBlockCar").addEventListener("click", () => U.closeModal("blockCarModal"));
  document.getElementById("btnCancelUnblockCar").addEventListener("click", () => U.closeModal("unblockCarModal"));
  document.getElementById("btnConfirmBlockCar").addEventListener("click", () => {
    const reason = document.getElementById("blockReasonType").value;
    const note = document.getElementById("blockNote").value.trim();
    if (!reason || (reason === "Khác" && !note)) return U.showToast("Vui lòng chọn lý do và nhập ghi chú nếu cần.");
    const car = DB.cars.find((item) => item.id === selectedCarId);
    car.status = "blocked";
    car.blocked_reason = note || reason;
    U.closeModal("blockCarModal");
    U.showToast("Đã khóa xe thành công.");
    renderCars();
  });
  document.getElementById("btnConfirmUnblockCar").addEventListener("click", () => {
    const car = DB.cars.find((item) => item.id === selectedCarId);
    car.status = "available";
    car.blocked_reason = null;
    U.closeModal("unblockCarModal");
    U.showToast("Đã mở khóa xe thành công.");
    renderCars();
  });
  renderTypeOptions();
  renderCars();
})();
