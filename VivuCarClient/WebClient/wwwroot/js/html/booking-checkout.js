/**
 * booking-checkout.js
 * Dùng API thật thay vì mock DB.
 * Requires: /js/shared/auth-service.js (ES module, loaded via Checkout.cshtml)
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
  // ── 0. Helpers ────────────────────────────────────────────────────────────
  const params     = new URLSearchParams(location.search);
  const carId      = Number(params.get('carId'));
  const root       = document.getElementById('checkoutRoot');

  function fmt(n) {
    return (n || 0).toLocaleString('vi-VN') + '₫';
  }

  function showToast(msg, type = 'success') {
    const colors = { success: '#16a34a', danger: '#dc2626', neutral: '#6b7280' };
    const toast = document.createElement('div');
    toast.style.cssText = `position:fixed;bottom:24px;right:24px;z-index:9999;background:${colors[type] || colors.neutral};color:#fff;padding:12px 20px;border-radius:12px;font-size:.875rem;font-weight:600;box-shadow:0 4px 16px rgba(0,0,0,.15);transition:opacity .3s`;
    toast.textContent = msg;
    document.body.appendChild(toast);
    setTimeout(() => { toast.style.opacity = '0'; setTimeout(() => toast.remove(), 300); }, 3000);
  }

  function setLoading(el, loading) {
    if (loading) { el.disabled = true; el.dataset.orig = el.textContent; el.textContent = 'Đang xử lý...'; }
    else { el.disabled = false; el.textContent = el.dataset.orig || el.textContent; }
  }

  // ── 1. Auth check ─────────────────────────────────────────────────────────
  const session = await authService.refresh();
  const currentUser = session?.user ?? null;
  if (!currentUser) {
    location.href = '/';
    return;
  }

  if (!carId) {
    root.innerHTML = `<div style="text-align:center;padding:48px;color:#6b7280"><h2>Không tìm thấy xe</h2><a href="/cars/search" style="color:#16a34a">← Tìm xe khác</a></div>`;
    return;
  }

  // ── 2. Load car from API ──────────────────────────────────────────────────
  let car = null;
  try {
    const r = await authService.apiFetch(`cars/${carId}`);
    if (!r.ok) throw new Error('Car not found');
    car = await r.json();
  } catch {
    root.innerHTML = `<div style="text-align:center;padding:48px;color:#6b7280"><h2>Không thể tải thông tin xe</h2><a href="/cars/search" style="color:#16a34a">← Tìm xe khác</a></div>`;
    return;
  }

  if (car.status !== 'Available') {
    root.innerHTML = `<div style="text-align:center;padding:48px;color:#6b7280"><h2>Xe này hiện không khả dụng</h2><p>Trạng thái: ${car.status}</p><a href="/cars/search" style="color:#16a34a">← Tìm xe khác</a></div>`;
    return;
  }

  // ── 3. Default dates (from URL params hoặc tomorrow/+2days) ───────────────
  const tomorrow   = new Date(Date.now() + 864e5);
  const afterTwo   = new Date(Date.now() + 3 * 864e5);
  const toDatetimeLocal = d => d.toISOString().slice(0, 16);

  const urlPickup  = params.get('pickupDate')  ? `${params.get('pickupDate')}T08:00`  : toDatetimeLocal(tomorrow);
  const urlReturn  = params.get('returnDate')  ? `${params.get('returnDate')}T18:00`  : toDatetimeLocal(afterTwo);

  // ── 4. Render form ────────────────────────────────────────────────────────
  const primaryImg = car.images?.find(i => i.isPrimary)?.imageUrl || car.images?.[0]?.imageUrl || '';

  root.innerHTML = `
    <form class="checkout-main" id="checkoutForm" style="display:grid;grid-template-columns:1fr 380px;gap:24px;align-items:start">

      <!-- LEFT COLUMN -->
      <div style="display:flex;flex-direction:column;gap:20px">

        <!-- Car card -->
        <div style="background:#fff;border:1px solid #e6e4df;border-radius:12px;padding:20px;display:flex;gap:16px;align-items:center">
          ${primaryImg ? `<img src="${primaryImg}" alt="${car.name}" style="width:120px;height:80px;object-fit:cover;border-radius:8px;flex-shrink:0">` : `<div style="width:120px;height:80px;background:#f3f4f6;border-radius:8px;display:flex;align-items:center;justify-content:center;font-size:2rem;flex-shrink:0">🚗</div>`}
          <div>
            <p style="font-size:.75rem;color:#6b7280;font-weight:600;text-transform:uppercase;letter-spacing:.05em;margin:0 0 4px">${car.brandName || ''}</p>
            <h1 style="font-size:1.125rem;font-weight:700;color:#1f1f1f;margin:0 0 4px">${car.name}</h1>
            <p style="font-size:.875rem;color:#6b7280;margin:0">📍 ${car.location || 'Đà Nẵng'}</p>
            <p style="font-size:.875rem;color:#16a34a;font-weight:700;margin:4px 0 0">${fmt(car.dailyPrice)}<span style="color:#9ca3af;font-weight:400">/ngày</span></p>
          </div>
        </div>

        <!-- Thông tin người thuê -->
        <div style="background:#fff;border:1px solid #e6e4df;border-radius:12px;padding:20px">
          <h2 style="font-size:1rem;font-weight:700;color:#1f1f1f;margin:0 0 16px">Thông tin người thuê</h2>
          <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px">
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Họ tên
              <input id="driverName" value="${currentUser.fullName || ''}" required
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Email
              <input value="${currentUser.email || ''}" disabled
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#6b7280;background:#f9fafb;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151;grid-column:1/-1">
              Ghi chú bàn giao
              <textarea id="driverNote" rows="3" placeholder="Thời điểm thuận tiện, yêu cầu giao xe..."
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;resize:vertical;outline:none"></textarea>
            </label>
          </div>
        </div>

        <!-- Submit -->
        <button type="submit" id="btnSubmit"
          style="width:100%;background:#16a34a;color:#fff;font-weight:700;padding:14px;border:none;border-radius:12px;font-size:1rem;cursor:pointer;transition:background .2s"
          onmouseover="this.style.background='#15803d'" onmouseout="this.style.background='#16a34a'">
          Tạo đơn và tiếp tục thanh toán
        </button>
      </div>

      <!-- RIGHT COLUMN (sidebar) -->
      <div style="display:flex;flex-direction:column;gap:16px;position:sticky;top:24px">

        <!-- Lịch thuê -->
        <div style="background:#fff;border:1px solid #e6e4df;border-radius:12px;padding:20px">
          <h2 style="font-size:1rem;font-weight:700;color:#1f1f1f;margin:0 0 16px">Lịch thuê xe</h2>
          <div style="display:flex;flex-direction:column;gap:12px">
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Thời gian nhận xe
              <input id="pickupDatetime" name="pickup_datetime" type="datetime-local" value="${urlPickup}" required
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Thời gian trả xe
              <input id="returnDatetime" name="return_datetime" type="datetime-local" value="${urlReturn}" required
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Địa điểm nhận xe
              <input id="pickupAddress" name="pickup_address" value="${car.location || ''}" required
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
          </div>
          <div id="availabilityNote" style="margin-top:12px;padding:8px 12px;border-radius:8px;font-size:.8125rem;font-weight:600;background:#f0fdf4;color:#16a34a">
            Đang kiểm tra lịch xe...
          </div>
        </div>

        <!-- Tóm tắt chi phí -->
        <div id="summaryCard" style="background:#fff;border:1px solid #e6e4df;border-radius:12px;padding:20px">
          <h2 style="font-size:1rem;font-weight:700;color:#1f1f1f;margin:0 0 12px">Tóm tắt chi phí</h2>
          <p style="color:#9ca3af;font-size:.875rem">Đang tính toán...</p>
        </div>

      </div>
    </form>`;

  // ── 5. Bind events ────────────────────────────────────────────────────────
  const pickupEl  = document.getElementById('pickupDatetime');
  const returnEl  = document.getElementById('returnDatetime');
  const noteEl    = document.getElementById('availabilityNote');
  const summaryEl = document.getElementById('summaryCard');
  const form      = document.getElementById('checkoutForm');

  let lastPreview = null;

  async function updatePricingAndAvailability() {
    const pickup  = pickupEl.value;
    const ret     = returnEl.value;
    if (!pickup || !ret) return;

    // Availability
    try {
      const r = await authService.apiFetch('bookings/check-availability', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ carId, startDateTime: pickup, endDateTime: ret })
      });
      const data = await r.json();
      if (data.available) {
        noteEl.style.cssText = 'margin-top:12px;padding:8px 12px;border-radius:8px;font-size:.8125rem;font-weight:600;background:#f0fdf4;color:#16a34a';
        noteEl.textContent = '✓ Xe còn trống trong khung giờ đã chọn';
      } else {
        noteEl.style.cssText = 'margin-top:12px;padding:8px 12px;border-radius:8px;font-size:.8125rem;font-weight:600;background:#fef2f2;color:#dc2626';
        noteEl.textContent = '✗ Xe đã có người đặt trong khung giờ này';
      }
    } catch {
      noteEl.style.cssText = 'margin-top:12px;padding:8px 12px;border-radius:8px;font-size:.8125rem;font-weight:600;background:#fffbeb;color:#b45309';
      noteEl.textContent = '⚠ Không thể kiểm tra lịch xe';
    }

    // Price preview
    try {
      const r = await authService.apiFetch('bookings/price-preview', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ carId, startDateTime: pickup, endDateTime: ret, voucherCode: null })
      });
      if (r.ok) {
        lastPreview = await r.json();
        renderSummary(lastPreview);
      }
    } catch {
      // Keep previous summary if error
    }
  }

  function renderSummary(p) {
    if (!p) return;
    summaryEl.innerHTML = `
      <h2 style="font-size:1rem;font-weight:700;color:#1f1f1f;margin:0 0 12px">Tóm tắt chi phí</h2>
      <p style="color:#6b7280;font-size:.875rem;margin:0 0 12px">${car.name} · ${p.rentalDays ?? '?'} ngày</p>
      <div style="display:flex;flex-direction:column;gap:8px;font-size:.875rem">
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Giá thuê</span><strong>${fmt(p.basePrice)}</strong></div>
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Bảo hiểm</span><strong>${fmt(p.insuranceFee)}</strong></div>
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Giao xe</span><strong>${fmt(p.deliveryFee ?? 0)}</strong></div>
        ${p.discountAmount > 0 ? `<div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Voucher</span><strong style="color:#16a34a">-${fmt(p.discountAmount)}</strong></div>` : ''}
        <div style="display:flex;justify-content:space-between;padding-top:10px;border-top:1px solid #e6e4df;font-size:1rem"><span style="font-weight:700;color:#1f1f1f">Tổng tiền</span><strong style="color:#16a34a">${fmt(p.totalAmount)}</strong></div>
        <div style="display:flex;justify-content:space-between;background:#fefce8;border-radius:8px;padding:8px 12px"><span style="color:#92400e;font-weight:600">Tiền cọc (30%)</span><strong style="color:#92400e">${fmt(p.depositAmount)}</strong></div>
      </div>`;
  }

  pickupEl.addEventListener('change', () => { returnEl.min = pickupEl.value; updatePricingAndAvailability(); });
  returnEl.addEventListener('change', updatePricingAndAvailability);

  // Initial load
  await updatePricingAndAvailability();

  // ── 6. Submit booking ─────────────────────────────────────────────────────
  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const btn = document.getElementById('btnSubmit');
    const pickup  = pickupEl.value;
    const ret     = returnEl.value;
    const address = document.getElementById('pickupAddress').value.trim();

    if (!pickup || !ret || !address) {
      showToast('Vui lòng điền đầy đủ thông tin.', 'danger');
      return;
    }

    setLoading(btn, true);
    try {
      const r = await authService.apiFetch('bookings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          carId,
          startDateTime: pickup,
          endDateTime: ret,
          pickupLocation: address,
          returnLocation: address,
          voucherCode: null
        })
      });

      if (!r.ok) {
        const err = await r.json().catch(() => ({}));
        showToast(err.message || 'Không thể tạo đơn thuê. Vui lòng thử lại.', 'danger');
        setLoading(btn, false);
        return;
      }

      const booking = await r.json();
      showToast('Đã tạo đơn thuê thành công!', 'success');
      setTimeout(() => location.href = `/Payment/Deposit?bookingId=${booking.id}`, 800);
    } catch (err) {
      console.error(err);
      showToast('Lỗi kết nối. Vui lòng thử lại.', 'danger');
      setLoading(btn, false);
    }
  });

})();
