(function () {
  const DB = window.VivuCarDB, C = window.VivuCarConstants, U = window.VivuCarUtils, Auth = window.VivuCarAuth;
  const carId = Number(new URLSearchParams(location.search).get("carId") || new URLSearchParams(location.search).get("id"));
  const car = DB.cars.find((c) => c.id === carId);
  if (!car) { document.getElementById("carDetailRoot").innerHTML = U.renderEmptyState({ title: "Không tìm thấy xe", href: "search.html", action: "Quay lại tìm xe" }); return; }
  const images = DB.car_images.filter((img) => img.car_id === car.id);
  const reviews = DB.reviews.filter((r) => r.car_id === car.id);
  document.getElementById("detailBreadcrumb").innerHTML = VivuCarLayout.renderBreadcrumb([{ label: "Trang chủ", href: "home.html" }, { label: "Xe", href: "search.html" }, { label: U.carTitle(car) }]);
  function calculateRentalDays(startDate, endDate) { const diff = new Date(endDate) - new Date(startDate); return Math.max(1, Math.ceil(diff / 86400000)); }
  function render() {
    const tomorrow = new Date(Date.now() + 864e5);
    const afterTwo = new Date(Date.now() + 3 * 864e5);
    const toDatetimeLocal = d => {
        // Set time to 08:00
        d.setHours(8, 0, 0, 0);
        return d.toISOString().slice(0, 16);
    };
    const defaultPickup = toDatetimeLocal(tomorrow);
    
    const afterTwoDate = new Date(tomorrow.getTime() + 2 * 864e5);
    afterTwoDate.setHours(20, 0, 0, 0);
    const defaultReturn = toDatetimeLocal(afterTwoDate);

    document.getElementById("carDetailRoot").innerHTML = `<div class="detail-layout"><section><img id="mainCarImage" class="gallery-main" src="${images[0]?.image_url || U.carImage(car.id)}" alt="${U.carTitle(car)}"><div id="carThumbnailList" class="thumb-row">${images.map((img) => `<img src="${img.image_url}" alt="Ảnh ${U.carTitle(car)}">`).join("")}</div><div class="tabs" id="detailTabs"><button class="active" data-tab="description">Mô tả</button><button data-tab="specs">Thông số</button><button data-tab="rules">Điều kiện thuê</button><button data-tab="reviews">Đánh giá</button></div><div id="tabContent" class="card p-[18px] mt-3.5"></div></section><aside class="booking-card card"><h1>${U.carTitle(car)}</h1><p class="muted">${car.address}</p><p>${U.statusBadge(String(car.status).toLowerCase() === "available" ? "success" : "neutral", car.status, C.CAR_STATUS_LABELS[car.status])}</p><div class="price-line"><strong>${U.formatVnd(car.price_per_day)}</strong><span>/ngày</span></div><div class="field"><label>Thời gian nhận</label><input id="detailPickupDate" type="text" value="${defaultPickup}" placeholder="Chọn thời gian"></div><div class="field"><label>Thời gian trả</label><input id="detailReturnDate" type="text" value="${defaultReturn}" placeholder="Chọn thời gian"></div><p>Tổng ngày: <strong id="totalRentalDays">1</strong></p><p>Tạm tính: <strong id="estimatedPrice">${U.formatVnd(car.price_per_day)}</strong></p><button class="btn btn-primary btn-full" id="btnBookCar" ${car.status !== "available" ? "disabled" : ""}>Đặt xe</button><button class="btn btn-secondary btn-full mt-2" id="btnContactOwner">Liên hệ chủ xe</button></aside></div>`;
    bind();
    renderTab("description");
  }
  
  function bind() {
    document.getElementById("carThumbnailList").addEventListener("click", (e) => { if (e.target.tagName === "IMG") document.getElementById("mainCarImage").src = e.target.src; });
    document.getElementById("detailTabs").addEventListener("click", (e) => { const b = e.target.closest("[data-tab]"); if (!b) return; document.querySelectorAll("#detailTabs button").forEach((x) => x.classList.toggle("active", x === b)); renderTab(b.dataset.tab); });
    
    const fpConfig = {
      enableTime: true,
      dateFormat: "Y-m-d\\TH:i",
      altInput: true,
      altFormat: "d/m/Y H:i",
      time_24hr: true,
      minDate: "today",
      minuteIncrement: 30,
      minTime: "07:00",
      maxTime: "22:00",
      onChange: function(selectedDates, dateStr, instance) {
        if (instance.element.id === "detailPickupDate" && selectedDates[0]) {
          returnPicker.set('minDate', selectedDates[0]);
        }
        updatePrice();
      },
      onDayCreate: function(dObj, dStr, fp, dayElem) {
        if (dayElem.classList.contains("flatpickr-disabled")) {
          dayElem.title = "Ngày này đã được thuê";
        }
      }
    };
    
    const pickupPicker = flatpickr("#detailPickupDate", fpConfig);
    const returnPicker = flatpickr("#detailReturnDate", fpConfig);

    // Fetch real availability to disable dates
    fetch(`/api/cars/${car.id}`)
      .then(r => r.json())
      .then(data => {
        if (data && data.availabilityBlocks) {
          const blockedRanges = data.availabilityBlocks.map(b => ({
            from: b.startDateTime.split('T')[0],
            to: b.endDateTime.split('T')[0]
          }));
          pickupPicker.set('disable', blockedRanges);
          returnPicker.set('disable', blockedRanges);
        }
      })
      .catch(e => console.warn("Could not fetch real blocked dates", e));

    document.getElementById("btnBookCar").addEventListener("click", () => {
      const p = document.getElementById("detailPickupDate").value;
      const r = document.getElementById("detailReturnDate").value;
      if (!p || !r) {
        U.renderToast("Vui lòng chọn thời gian nhận và trả xe", "danger");
        return;
      }
      if (!Auth.getCurrentUser()) location.href = "/Login"; 
      else location.href = `/Booking/Checkout?carId=${car.id}&pickup=${p}&return=${r}`; 
    });
  }
  
  function updatePrice() { 
      const s = document.getElementById("detailPickupDate").value;
      const e = document.getElementById("detailReturnDate").value; 
      if (!s || !e) return;
      const days = calculateRentalDays(s, e); 
      document.getElementById("totalRentalDays").textContent = days; 
      document.getElementById("estimatedPrice").textContent = U.formatVnd(days * car.price_per_day); 
  }
  
  function renderTab(tab) {
    const content = document.getElementById("tabContent");
    if (tab === "description") content.innerHTML = `<h2>Mô tả</h2><p>${car.description || "Chưa có mô tả."}</p><p class="muted">Tiện nghi: Bluetooth, điều hòa, cảm biến lùi, bản đồ.</p>`;
    if (tab === "specs") content.innerHTML = `<h2>Thông số</h2><table><tbody><tr><td>Hãng</td><td>${car.brand}</td></tr><tr><td>Dòng</td><td>${car.model}</td></tr><tr><td>Năm</td><td>${car.year}</td></tr><tr><td>Số chỗ</td><td>${car.seats}</td></tr><tr><td>Hộp số</td><td>${C.TRANSMISSION_LABELS[car.transmission]}</td></tr><tr><td>Nhiên liệu</td><td>${C.FUEL_TYPE_LABELS[car.fuel_type]}</td></tr><tr><td>Km đã đi</td><td>${car.kilometers_driven.toLocaleString("vi-VN")} km</td></tr></tbody></table>`;
    if (tab === "rules") content.innerHTML = `<h2>Điều kiện thuê</h2><ul><li>Có giấy phép lái xe hợp lệ.</li><li>Đặt cọc theo quy định.</li><li>Không hút thuốc trong xe.</li><li>Trả xe đúng thời gian.</li></ul>`;
    if (tab === "reviews") content.innerHTML = `<h2>Đánh giá</h2><p>${U.stars(U.carRating(car.id))} ${U.carRating(car.id).toFixed(1)} từ ${reviews.length} đánh giá</p>${reviews.map((r) => { const u = DB.users.find((x) => x.id === r.reviewer_id); return `<div class="card p-3 mt-2.5"><strong>${u?.full_name || "Khách thuê"}</strong><p>${U.stars(r.rating)}</p><p>${r.comment}</p><small class="muted">${U.formatDate(r.created_at)}</small></div>`; }).join("") || U.renderEmptyState({ title: "Chưa có đánh giá" })}`;
  }
  render();
})();


