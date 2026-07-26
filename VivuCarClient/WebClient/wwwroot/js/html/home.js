(function () {
  const DB = window.VivuCarDB, U = window.VivuCarUtils;
  function card(car) {
    return `<article class="car-card"><img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}"><div class="car-card-body"><div>${U.statusBadge("success", "available", "Khả dụng")}</div><h3>${U.carTitle(car)}</h3><div class="car-meta"><span>${car.address}</span><span>${U.stars(U.carRating(car.id))} ${U.carRating(car.id).toFixed(1)}</span><span>${U.rentalCount(car.id)} lượt thuê</span></div><div class="price-line"><strong>${U.formatVnd(car.price_per_day)}</strong><span class="muted">/ngày</span></div><div class="actions"><a class="btn btn-secondary btn-sm" href="car-detail.html?carId=${car.id}">Xem chi tiết</a><a class="btn btn-primary btn-sm" href="/Booking/Checkout?carId=${car.id}">Đặt xe</a></div></div></article>`;
  }
  function featuredCars(cars) {
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    return cars.slice().sort((a, b) => (U.carRating(b.id) + U.rentalCount(b.id)) - (U.carRating(a.id) + U.rentalCount(a.id))).slice(0, 4);
  }
  function render() {
    const available = DB.cars.filter((car) => String(car.status).toLowerCase() === "available");
    renderStats(available);
    renderAreas();
    renderTypes();
    document.getElementById("featuredCarsGrid").innerHTML = featuredCars(available).map(card).join("") || U.renderEmptyState({ title: "Chưa có xe nổi bật" });
    document.getElementById("availableCarsGrid").innerHTML = available.map(card).join("") || U.renderEmptyState({ title: "Không tìm thấy xe khả dụng", href: "search.html", action: "Xóa bộ lọc" });
  }
  function renderStats(available) {
    const areas = new Set(available.map((car) => car.address.split(",")[0].trim()));
    const avg = available.reduce((sum, car) => sum + Number(car.price_per_day), 0) / Math.max(available.length, 1);
    document.getElementById("homeStats").innerHTML = [
      [`${available.length}`, "xe đang rảnh"],
      [`${areas.size}`, "khu vực nhận xe"],
      [U.formatVnd(avg), "giá trung bình/ngày"]
    ].map(([value, label]) => `<div class="home-stat"><strong>${value}</strong><span>${label}</span></div>`).join("");
  }
  function renderAreas() {
    const areas = [...new Set(DB.cars.filter((car) => String(car.status).toLowerCase() === "available").map((car) => car.address.split(",")[0].trim()))];
    document.getElementById("areaChips").innerHTML = areas.map((area) => `<a class="chip" href="search.html?location=${encodeURIComponent(area)}">${area}</a>`).join("");
  }
  function renderTypes() {
    document.getElementById("typeChips").innerHTML = DB.car_types.map((type) => {
      const count = DB.cars.filter((car) => String(car.status).toLowerCase() === "available" && car.type_id === type.id).length;
      return `<a class="type-card" href="search.html?type_id=${type.id}"><strong>${type.name}</strong><span>${count} xe khả dụng</span></a>`;
    }).join("");
  }
  document.getElementById("btnQuickSearch").addEventListener("click", () => {
    const keyword = encodeURIComponent(document.getElementById("homeSearchInput").value.trim());
    const area = encodeURIComponent(document.getElementById("homeLocationSelect").value);
    const seats = encodeURIComponent(document.getElementById("seatQuickSelect").value);
    window.location.href = `search.html?keyword=${keyword}&location=${area}&seats=${seats}`;
  });
  setTimeout(render, 180);
})();

