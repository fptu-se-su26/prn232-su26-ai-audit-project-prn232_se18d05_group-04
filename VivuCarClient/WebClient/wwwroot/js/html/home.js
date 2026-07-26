(async function () {
  const U = window.VivuCarUtils;
  const fmt = n => (n || 0).toLocaleString('vi-VN') + '₫';
  const stars = r => { const f = Math.round(r || 0); return `<span style="color:#fbbf24">${'★'.repeat(f)}${'☆'.repeat(5 - f)}</span>`; };
  const fuelMap = { Gasoline: 'Xăng', Electric: 'Điện', Diesel: 'Dầu', Hybrid: 'Hybrid' };

  function carCard(car) {
    const img = car.primaryImageUrl || car.carImageUrl
      ? `<img src="${car.primaryImageUrl || car.carImageUrl}" alt="${car.name}" loading="lazy">`
      : `<div style="height:100%;background:#f3f4f6;display:flex;align-items:center;justify-content:center;font-size:2.5rem;color:#d1d5db">🚗</div>`;
    const badge = String(car.status || '').toLowerCase() === 'available'
      ? `<span style="position:absolute;top:10px;left:10px;background:#dcfce7;color:#16a34a;font-size:.75rem;font-weight:700;padding:4px 10px;border-radius:999px;box-shadow:0 1px 4px rgba(0,0,0,.1)">Khả dụng</span>`
      : '';
    return `<article class="car-card">
      <div style="position:relative;overflow:hidden">${img}${badge}</div>
      <div class="car-card-body">
        <p style="font-size:.75rem;color:#9ca3af;font-weight:500;margin:0">${car.brandName || ''}</p>
        <h3>${car.name}</h3>
        <div class="car-meta">
          <span>📍 ${car.location || 'Đà Nẵng'}</span>
          <span>🪑 ${car.seatCount} chỗ</span>
          <span>⚙️ ${car.transmission === 'Automatic' ? 'Tự động' : 'Số sàn'}</span>
          <span>⛽ ${fuelMap[car.fuel] || car.fuel || ''}</span>
        </div>
        <div style="display:flex;align-items:center;gap:6px;font-size:.8125rem;margin-top:4px">
          ${stars(car.averageRating)}
          <span style="color:#6b7280">${(car.averageRating || 0).toFixed(1)} · ${car.totalBookings || 0} lượt thuê</span>
        </div>
        <div class="price-line" style="margin-top:auto">
          <strong>${fmt(car.dailyPrice)}</strong><span class="muted">/ngày</span>
        </div>
        <div class="actions" style="margin-top:8px">
          <a class="btn btn-secondary btn-sm" href="/cars/detail?id=${car.id}">Xem chi tiết</a>
          <a class="btn btn-primary btn-sm" href="/Booking/Checkout?carId=${car.id}">Đặt xe</a>
        </div>
      </div>
    </article>`;
  }

  async function loadData() {
    try {
      const r = await fetch('/api/proxy/cars?pageSize=100&status=available');
      if (!r.ok) throw new Error('Failed');
      const data = await r.json();
      const cars = data.items || [];
      render(cars);
    } catch {
      document.getElementById('featuredCarsGrid').innerHTML = '<p style="padding:20px;text-align:center;color:#6b7280">Không thể tải dữ liệu.</p>';
      document.getElementById('availableCarsGrid').innerHTML = '';
    }
  }

  function render(cars) {
    renderStats(cars);
    renderAreas(cars);
    renderTypes(cars);
    // Featured: sort by rating + bookings
    const featured = cars.slice().sort((a, b) =>
      ((b.averageRating || 0) + (b.totalBookings || 0) / 10) - ((a.averageRating || 0) + (a.totalBookings || 0) / 10)
    ).slice(0, 4);
    document.getElementById('featuredCarsGrid').innerHTML = featured.length
      ? featured.map(carCard).join('')
      : '<p style="padding:20px;text-align:center;color:#6b7280">Chưa có xe nổi bật</p>';
    document.getElementById('availableCarsGrid').innerHTML = cars.length
      ? cars.map(carCard).join('')
      : '<p style="padding:20px;text-align:center;color:#6b7280">Không tìm thấy xe khả dụng</p>';
  }

  function renderStats(cars) {
    const locations = new Set(cars.map(c => (c.location || '').split(',')[0].trim()).filter(Boolean));
    const avg = cars.reduce((s, c) => s + Number(c.dailyPrice || 0), 0) / Math.max(cars.length, 1);
    document.getElementById('homeStats').innerHTML = [
      [`${cars.length}`, 'xe đang rảnh'],
      [`${locations.size}`, 'khu vực nhận xe'],
      [fmt(avg), 'giá trung bình/ngày']
    ].map(([v, l]) => `<div class="home-stat"><strong>${v}</strong><span>${l}</span></div>`).join('');
  }

  function renderAreas(cars) {
    const areas = [...new Set(cars.map(c => (c.location || '').split(',')[0].trim()).filter(Boolean))];
    document.getElementById('areaChips').innerHTML = areas.map(a =>
      `<a class="chip" href="/cars?location=${encodeURIComponent(a)}">${a}</a>`
    ).join('');
    // Populate location select
    const sel = document.getElementById('homeLocationSelect');
    if (sel && areas.length) {
      sel.innerHTML = '<option value="">Tất cả khu vực</option>' +
        areas.map(a => `<option>${a}</option>`).join('');
    }
  }

  function renderTypes() {
    const types = [
      { key: '4', label: '4 chỗ', icon: '🚗', desc: 'Xe nhỏ gọn, dễ đỗ' },
      { key: '5', label: '5 chỗ', icon: '🚙', desc: 'Phù hợp gia đình nhỏ' },
      { key: '7', label: '7 chỗ', icon: '🚐', desc: 'Xe gia đình đông người' },
      { key: '9', label: '9 chỗ', icon: '🚌', desc: 'Du lịch nhóm đông' }
    ];
    document.getElementById('typeChips').innerHTML = types.map(t =>
      `<a class="type-card" href="/cars?seatCount=${t.key}" style="text-decoration:none">
        <span style="font-size:2rem">${t.icon}</span>
        <strong style="display:block;font-size:1rem;margin-top:8px">${t.label}</strong>
        <span style="font-size:.8125rem;opacity:.8">${t.desc}</span>
      </a>`
    ).join('');
  }

  document.getElementById('btnQuickSearch')?.addEventListener('click', () => {
    const keyword = encodeURIComponent(document.getElementById('homeSearchInput').value.trim());
    const location = encodeURIComponent(document.getElementById('homeLocationSelect').value);
    const seats = encodeURIComponent(document.getElementById('seatQuickSelect').value);
    const params = new URLSearchParams();
    if (keyword) params.set('searchTerm', keyword);
    if (location) params.set('location', location);
    if (seats) params.set('seatCount', seats);
    window.location.href = `/cars?${params.toString()}`;
  });

  loadData();
})();
