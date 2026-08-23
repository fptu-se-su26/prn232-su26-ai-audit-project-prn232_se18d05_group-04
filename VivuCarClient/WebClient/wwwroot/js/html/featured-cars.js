(function () {
  const DB = window.VivuCarDB, U = window.VivuCarUtils;
  const state = { tab: "top_rated", sort: "recommended", page: 1, pageSize: 8 };
  function card(car, badge) { return `<article class="car-card"><img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}"><div class="car-card-body"><div>${U.statusBadge("info", badge, badge)}</div><h3>${U.carTitle(car)}</h3><div class="car-meta"><span>${car.address}</span><span>${U.stars(U.carRating(car.id))} ${U.carRating(car.id).toFixed(1)}</span><span>${U.rentalCount(car.id)} lượt thuê</span></div><div class="price-line"><strong>${U.formatVnd(car.price_per_day)}</strong><span>Chủ xe: ${DB.users.find((u) => u.id === car.owner_id)?.full_name}</span></div><a class="btn btn-primary btn-sm" href="car-detail.html?carId=${car.id}">Xem chi tiết</a></div></article>`; }
  function fetchFeaturedCars() {
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    let cars = DB.cars.filter((car) => String(car.status).toLowerCase() === "available");
    if (state.tab === "top_rated") cars.sort((a, b) => U.carRating(b.id) - U.carRating(a.id));
    if (state.tab === "most_rented") cars.sort((a, b) => U.rentalCount(b.id) - U.rentalCount(a.id));
    if (state.tab === "trusted_owner") cars.sort((a, b) => DB.cars.filter((c) => c.owner_id === b.owner_id).length - DB.cars.filter((c) => c.owner_id === a.owner_id).length);
    if (state.sort === "price_asc") cars.sort((a, b) => a.price_per_day - b.price_per_day);
    if (state.sort === "price_desc") cars.sort((a, b) => b.price_per_day - a.price_per_day);
    if (state.sort === "rating_desc") cars.sort((a, b) => U.carRating(b.id) - U.carRating(a.id));
    if (state.sort === "rental_desc") cars.sort((a, b) => U.rentalCount(b.id) - U.rentalCount(a.id));
    return cars;
  }
  function render() {
    const badge = { top_rated: "Top Rated", most_rented: "Most Rented", trusted_owner: "Trusted Owner" }[state.tab];
    const page = U.paginate(fetchFeaturedCars(), state.page, state.pageSize);
    document.getElementById("featuredGrid").innerHTML = page.items.map((car) => card(car, badge)).join("") || U.renderEmptyState({ title: "Chưa có xe nổi bật trong nhóm này", href: "search.html", action: "Xem tất cả xe" });
    document.getElementById("featuredPagination").innerHTML = Array.from({ length: page.totalPages }, (_, i) => `<button class="btn ${i + 1 === page.page ? "btn-primary" : "btn-secondary"} btn-sm" data-page="${i + 1}">${i + 1}</button>`).join("");
  }
  document.getElementById("featuredTabs").addEventListener("click", (e) => { const b = e.target.closest("[data-tab]"); if (!b) return; state.tab = b.dataset.tab; state.page = 1; document.querySelectorAll("#featuredTabs button").forEach((x) => x.classList.toggle("active", x === b)); render(); });
  document.getElementById("featuredSort").addEventListener("change", (e) => { state.sort = e.target.value; render(); });
  document.getElementById("featuredPagination").addEventListener("click", (e) => { const b = e.target.closest("[data-page]"); if (b) { state.page = Number(b.dataset.page); render(); } });
  setTimeout(render, 180);
})();

