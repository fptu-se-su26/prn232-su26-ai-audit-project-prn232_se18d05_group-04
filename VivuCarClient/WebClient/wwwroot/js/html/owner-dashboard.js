(function () {
  const DB = window.VivuCarDB, U = window.VivuCarUtils, Auth = window.VivuCarAuth, C = window.VivuCarConstants;
  const owner = Auth.getCurrentUser();
  if (!owner) return;
  const root = U.byId('ownerDashboardRoot');

  function ownerCars() { return DB.cars.filter((c) => c.owner_id === owner.id); }
  function ownerBookings() {
    const ids = ownerCars().map((c) => c.id);
    return DB.bookings.filter((b) => ids.includes(b.car_id));
  }
  function ownerRevenue() {
    return DB.payments
      .filter((p) => p.status === 'success' && ownerBookings().some((b) => b.id === p.booking_id))
      .reduce((s, p) => s + Number(p.amount || 0), 0);
  }

  function recentBookings() {
    const ids = ownerCars().map((c) => c.id);
    return DB.bookings
      .filter((b) => ids.includes(b.car_id))
      .slice()
      .reverse()
      .slice(0, 5)
      .map((b) => ({
        booking: b,
        car: DB.cars.find((c) => c.id === b.car_id),
        user: DB.users.find((u) => u.id === b.user_id),
        payment: U.paymentForBooking(b.id),
        inspections: U.inspectionsForBooking(b.id)
      }))
      .map((r) => ({ ...r, ui: U.resolveBookingUiState(r.booking, r.payment, r.inspections) }));
  }

  function pendingCount() { return ownerBookings().filter((b) => b.status === 'pending').length; }
  function rentingCount() {
    return recentBookings().filter((r) => r.ui.key === 'renting').length;
  }

  function renderKpi() {
    const cars = ownerCars();
    const bookings = ownerBookings();
    const revenue = ownerRevenue();
    const pending = pendingCount();
    const kpis = [
      { label: 'Tổng xe', value: cars.length, color: 'text-zinc-900' },
      { label: 'Xe đang hoạt động', value: cars.filter((c) => c.status === 'available').length, color: 'text-emerald-700' },
      { label: 'Đang cho thuê', value: cars.filter((c) => c.status === 'rented').length, color: 'text-sky-700' },
      { label: 'Đơn chờ duyệt', value: pending, color: pending > 0 ? 'text-amber-700' : 'text-zinc-900' },
      { label: 'Tổng đơn', value: bookings.length, color: 'text-zinc-900' },
      { label: 'Hoàn thành', value: bookings.filter((b) => b.status === 'completed').length, color: 'text-emerald-700' },
      { label: 'Đã hủy', value: bookings.filter((b) => b.status === 'cancelled').length, color: 'text-red-600' },
      { label: 'Doanh thu (ước)', value: U.formatVnd(revenue), color: 'text-emerald-700', mono: true }
    ];
    return `<div class="kpi-grid max-sm:grid-cols-2 !grid-cols-4 md:!grid-cols-4 lg:!grid-cols-4">${kpis.map((k) =>
      `<div class="kpi-card"><span>${k.label}</span><strong class="${k.color}${k.mono ? ' font-mono text-xl' : ''}">${k.value}</strong></div>`
    ).join('')}</div>`;
  }

  function quickActions() {
    return `
      <section class="mb-5">
        <div class="page-heading"><div><h2 class="!text-xl !text-zinc-900 !m-0">Thao tác nhanh</h2></div></div>
        <div class="grid grid-cols-2 gap-3 sm:grid-cols-4">
          <a class="btn btn-secondary flex-col gap-2 !min-h-20 !rounded-2xl" href="owner-car-create.html">
            <svg viewBox="0 0 512 512" class="h-5 w-5 fill-current text-emerald-600"><path d="M256 512A256 256 0 1 0 256 0a256 256 0 1 0 0 512zM232 344V280H168c-13.3 0-24-10.7-24-24s10.7-24 24-24h64V168c0-13.3 10.7-24 24-24s24 10.7 24 24v64h64c13.3 0 24 10.7 24 24s-10.7 24-24 24H280v64c0 13.3-10.7 24-24 24s-24-10.7-24-24z"/></svg>
            <span class="text-xs font-semibold">Đăng xe mới</span>
          </a>
          <a class="btn btn-secondary flex-col gap-2 !min-h-20 !rounded-2xl" href="owner-cars.html">
            <svg viewBox="0 0 512 512" class="h-5 w-5 fill-current text-sky-600"><path d="M135.2 117.4 109.1 192h293.8l-26.1-74.6C372.3 104.6 360.2 96 346.6 96H165.4c-13.6 0-25.7 8.6-30.2 21.4zM39.6 196.8 74.8 96.3C88.3 57.8 124.6 32 165.4 32h181.2c40.8 0 77.1 25.8 90.6 64.3l35.2 100.5C495.6 207.6 512 231.1 512 258.5V400c0 26.5-21.5 48-48 48h-16v32c0 17.7-14.3 32-32 32h-32c-17.7 0-32-14.3-32-32v-32H160v32c0 17.7-14.3 32-32 32H96c-17.7 0-32-14.3-32-32v-32H48c-26.5 0-48-21.5-48-48V258.5c0-27.4 16.4-50.9 39.6-61.7zM128 352a48 48 0 1 0 0-96 48 48 0 1 0 0 96zm256 0a48 48 0 1 0 0-96 48 48 0 1 0 0 96z"/></svg>
            <span class="text-xs font-semibold">Quản lý xe</span>
          </a>
          <a class="btn btn-secondary flex-col gap-2 !min-h-20 !rounded-2xl" href="owner-booking-requests.html">
            <svg viewBox="0 0 448 512" class="h-5 w-5 fill-current text-amber-600"><path d="M152 24c0-13.3-10.7-24-24-24s-24 10.7-24 24V64H64C28.7 64 0 92.7 0 128v16 48V448c0 35.3 28.7 64 64 64H384c35.3 0 64-28.7 64-64V192 144 128c0-35.3-28.7-64-64-64H344V24c0-13.3-10.7-24-24-24s-24 10.7-24 24V64H152V24zM48 192H400V448c0 8.8-7.2 16-16 16H64c-8.8 0-16-7.2-16-16V192z"/></svg>
            <span class="text-xs font-semibold">Yêu cầu đặt xe</span>
          </a>
          <a class="btn btn-secondary flex-col gap-2 !min-h-20 !rounded-2xl" href="owner-handover-dashboard.html">
            <svg viewBox="0 0 512 512" class="h-5 w-5 fill-current text-violet-600"><path d="M32 96l320 0V32c0-12.9 7.8-24.6 19.8-29.6s25.7-2.2 34.9 6.9l96 96c6 6 9.4 14.1 9.4 22.6s-3.4 16.6-9.4 22.6l-96 96c-9.2 9.2-22.9 11.9-34.9 6.9s-19.8-16.6-19.8-29.6V160L32 160c-17.7 0-32-14.3-32-32s14.3-32 32-32zM480 352c17.7 0 32 14.3 32 32s-14.3 32-32 32H160v32c0 12.9-7.8 24.6-19.8 29.6s-25.7 2.2-34.9-6.9l-96-96c-6-6-9.4-14.1-9.4-22.6s3.4-16.6 9.4-22.6l96-96c9.2-9.2 22.9-11.9 34.9-6.9s19.8 16.6 19.8 29.6l0 32H480z"/></svg>
            <span class="text-xs font-semibold">Bàn giao & Trả xe</span>
          </a>
        </div>
      </section>`;
  }

  function renderRecentBookings() {
    const rows = recentBookings();
    if (!rows.length) return U.renderEmptyState({ title: 'Chưa có đơn đặt xe nào', description: 'Khi có khách đặt xe, đơn sẽ xuất hiện ở đây.' });
    return `<div class="owner-table-wrap"><table class="owner-table">
      <thead><tr><th>#</th><th>Khách thuê</th><th>Xe</th><th>Thời gian</th><th>Trạng thái</th><th>Hành động</th></tr></thead>
      <tbody>${rows.map((r) => `<tr>
        <td class="mono text-zinc-500">${r.booking.id}</td>
        <td>${r.user ? r.user.full_name : '—'}</td>
        <td>${r.car ? `${r.car.brand} ${r.car.model}` : '—'}</td>
        <td class="text-xs text-zinc-500">${U.formatDateTime(r.booking.pickup_datetime)}</td>
        <td>${U.statusBadge(r.ui.tone, r.ui.key, r.ui.label)}</td>
        <td><a class="btn btn-ghost btn-sm" href="owner-booking-request-detail.html?bookingId=${r.booking.id}">Xem</a></td>
      </tr>`).join('')}</tbody>
    </table></div>`;
  }

  function render() {
    root.innerHTML = `
      <div class="page-heading">
        <div>
          <span class="eyebrow">Owner Portal</span>
          <h1>Dashboard</h1>
          <p>Xin chào, <strong>${owner.full_name}</strong>. Đây là tổng quan hoạt động của bạn.</p>
        </div>
      </div>
      ${renderKpi()}
      ${quickActions()}
      <section>
        <div class="section-head"><h2>Đơn đặt xe gần đây</h2><a class="btn btn-ghost btn-sm" href="owner-booking-requests.html">Xem tất cả</a></div>
        ${renderRecentBookings()}
      </section>`;
  }

  render();
})();

