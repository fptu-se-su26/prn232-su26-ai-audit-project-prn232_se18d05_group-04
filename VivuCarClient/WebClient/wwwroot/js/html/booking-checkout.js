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
    // Show friendly message instead of silent redirect
    root.innerHTML = `
      <div style="text-align:center;padding:60px 24px;max-width:400px;margin:0 auto">
        <div style="font-size:3rem;margin-bottom:16px">🔐</div>
        <h2 style="font-size:1.25rem;font-weight:700;color:#1f1f1f;margin:0 0 8px">Bạn chưa đăng nhập</h2>
        <p style="color:#6b7280;font-size:.875rem;margin:0 0 24px">Vui lòng đăng nhập để tiếp tục đặt xe. Sau khi đăng nhập bạn sẽ được đưa trở lại trang này.</p>
        <a href="/?redirect=${encodeURIComponent(location.pathname + location.search)}"
           style="display:inline-block;background:#16a34a;color:#fff;font-weight:700;padding:12px 28px;border-radius:12px;text-decoration:none;font-size:.9375rem">
          Đăng nhập ngay
        </a>
      </div>`;
    return;
  }

  // Update header to show logged-in user (since layout.js uses mock auth, we patch it here)
  function patchHeaderForRealAuth(user) {
    // Use the exposed layout function if available
    if (window.VivuCarLayout?.patchHeaderForRealUser) {
      window.VivuCarLayout.patchHeaderForRealUser(user);
      return;
    }
    // Fallback: directly patch the header
    const headerMount = document.getElementById('headerMount');
    if (!headerMount) return;
    const loginLink = headerMount.querySelector('a[href="/"], a[href="/Login"], a[href*="login"]');
    if (loginLink) {
      loginLink.textContent = user.fullName || user.email || 'Tài khoản';
      loginLink.href = '/Profile';
      loginLink.classList.remove('btn-primary');
      loginLink.classList.add('btn-secondary');
    }
  }
  // Wait for layout.js to finish rendering, then patch
  requestAnimationFrame(() => patchHeaderForRealAuth(currentUser));
  // Also patch after a short delay in case layout is async
  setTimeout(() => patchHeaderForRealAuth(currentUser), 300);

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

  function formatUrlDate(val, defaultTime) {
    if (!val) return null;
    if (val.length === 10) return `${val}T${defaultTime}`;
    if (val.length > 16) return val.slice(0, 16);
    return val;
  }
  const rawPickup = params.get('pickup') || params.get('pickupDate');
  const rawReturn = params.get('return') || params.get('returnDate');
  const urlPickup  = formatUrlDate(rawPickup, '08:00') || toDatetimeLocal(tomorrow);
  const urlReturn  = formatUrlDate(rawReturn, '18:00') || toDatetimeLocal(afterTwo);

  const DB = window.VivuCarDB;
  const docFront = DB?.user_documents?.find(d => d.user_id === currentUser.id && d.document_type === 'license_front');
  const docBack = DB?.user_documents?.find(d => d.user_id === currentUser.id && d.document_type === 'license_back');

  // ── 4. Render form ────────────────────────────────────────────────────────
  const primaryImg = car.images?.find(i => i.isPrimary)?.imageUrl || car.images?.[0]?.imageUrl || '';

  root.innerHTML = `
    <form class="checkout-main" id="checkoutForm" style="display:contents">

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
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Số điện thoại
              <input id="driverPhone" type="tel" required placeholder="09xxxx"
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151">
              Số CCCD/CMND
              <input id="driverNationalId" type="text" required placeholder="12 số"
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151;grid-column:1/-1">
              Số giấy phép lái xe
              <input id="driverLicense" type="text" required placeholder="Số GPLX"
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;outline:none">
            </label>
            <label style="display:flex;flex-direction:column;gap:4px;font-size:.875rem;font-weight:600;color:#374151;grid-column:1/-1">
              Ghi chú bàn giao
              <textarea id="driverNote" rows="3" placeholder="Thời điểm thuận tiện, yêu cầu giao xe..."
                style="border:1px solid #e6e4df;border-radius:8px;padding:8px 12px;font-size:.875rem;color:#1f1f1f;resize:vertical;outline:none"></textarea>
            </label>
          </div>
        </div>

        <!-- GPLX Upload -->
        <div style="background:#fff;border:1px solid #e6e4df;border-radius:12px;padding:20px">
          <h2 style="font-size:1rem;font-weight:700;color:#1f1f1f;margin:0 0 16px">Giấy phép lái xe (GPLX)</h2>
          <p style="font-size:.875rem;color:#6b7280;margin:0 0 16px">Để rút ngắn thời gian bàn giao xe, bạn có thể tải lên ảnh GPLX ngay bây giờ.</p>
          <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px">
            <div>
              <p style="font-size:.875rem;font-weight:600;color:#374151;margin:0 0 8px">Mặt trước</p>
              ${docFront ? `<img src="${docFront.file_url}" style="width:100%;height:120px;object-fit:cover;border-radius:8px;border:1px solid #e6e4df">` : `<input id="licenseFrontInput" type="file" accept="image/*" style="width:100%;font-size:.875rem">`}
            </div>
            <div>
              <p style="font-size:.875rem;font-weight:600;color:#374151;margin:0 0 8px">Mặt sau</p>
              ${docBack ? `<img src="${docBack.file_url}" style="width:100%;height:120px;object-fit:cover;border-radius:8px;border:1px solid #e6e4df">` : `<input id="licenseBackInput" type="file" accept="image/*" style="width:100%;font-size:.875rem">`}
            </div>
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
    const rentalCost = (p.weekdayCost || 0) + (p.weekendCost || 0);
    const hasWeekend = p.weekendCount > 0;
    summaryEl.innerHTML = `
      <h2 style="font-size:1rem;font-weight:700;color:#1f1f1f;margin:0 0 12px">Tóm tắt chi phí</h2>
      <p style="color:#6b7280;font-size:.875rem;margin:0 0 12px">${car.name} · ${p.rentalDays ?? '?'} ngày</p>
      <div style="display:flex;flex-direction:column;gap:8px;font-size:.875rem">
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Ngày thường (${p.weekdayCount || 0} ngày × ${fmt(p.weekdayPrice || 0)})</span><strong>${fmt(p.weekdayCost || 0)}</strong></div>
        ${hasWeekend ? `<div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Cuối tuần (${p.weekendCount} ngày × ${fmt(p.weekendPrice || 0)})</span><strong>${fmt(p.weekendCost || 0)}</strong></div>` : ''}
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Giá thuê</span><strong>${fmt(rentalCost)}</strong></div>
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Bảo hiểm</span><strong>${fmt(p.insuranceFee)}</strong></div>
        <div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Giao xe</span><strong>${fmt(p.deliveryFee ?? 0)}</strong></div>
        ${p.discountAmount > 0 ? `<div style="display:flex;justify-content:space-between"><span style="color:#6b7280">Voucher</span><strong style="color:#16a34a">-${fmt(p.discountAmount)}</strong></div>` : ''}
        <div style="display:flex;justify-content:space-between;padding-top:10px;border-top:1px solid #e6e4df;font-size:1rem"><span style="font-weight:700;color:#1f1f1f">Tổng tiền</span><strong style="color:#16a34a">${fmt(p.totalAmount)}</strong></div>
        <div style="display:flex;justify-content:space-between;background:#fefce8;border-radius:8px;padding:8px 12px"><span style="color:#92400e;font-weight:600">Tiền cọc (30%)</span><strong style="color:#92400e">${fmt(p.depositAmount)}</strong></div>
      </div>`;
  }

  pickupEl.addEventListener('change', () => { returnEl.min = pickupEl.value; updatePricingAndAvailability(); });
  returnEl.addEventListener('change', updatePricingAndAvailability);

  // File preview logic
  function bindPreview(inputId) {
    const input = document.getElementById(inputId);
    if (!input) return;
    input.addEventListener('change', (e) => {
      const file = e.target.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = (ev) => {
        let img = input.previousElementSibling;
        if (img && img.tagName === 'IMG') {
          img.src = ev.target.result;
        } else {
          img = document.createElement('img');
          img.src = ev.target.result;
          img.style.cssText = 'width:100%;height:120px;object-fit:cover;border-radius:8px;border:1px solid #e6e4df;margin-bottom:8px';
          input.parentNode.insertBefore(img, input);
        }
      };
      reader.readAsDataURL(file);
    });
  }
  bindPreview('licenseFrontInput');
  bindPreview('licenseBackInput');

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

    // Validate: return must be after pickup by at least 1 hour
    const pickupDt = new Date(pickup);
    const returnDt = new Date(ret);
    if (returnDt <= pickupDt) {
      showToast('Thời gian trả xe phải sau thời gian nhận xe.', 'danger');
      returnEl.focus();
      return;
    }
    if ((returnDt - pickupDt) < 3600000) {
      showToast('Thời gian thuê tối thiểu là 1 giờ.', 'danger');
      returnEl.focus();
      return;
    }
    // Pickup must be at least 30 min from now
    if (pickupDt < new Date(Date.now() - 30 * 60000)) {
      showToast('Thời gian nhận xe không thể ở trong quá khứ.', 'danger');
      pickupEl.focus();
      return;
    }

    setLoading(btn, true);

    try {
      // 1. Upload GPLX images if selected
      const frontInput = document.getElementById('licenseFrontInput');
      const backInput = document.getElementById('licenseBackInput');
      let frontUrl = docFront?.file_url;
      let backUrl = docBack?.file_url;

      if (frontInput && frontInput.files[0]) {
        const fd = new FormData();
        fd.append('file', frontInput.files[0]);
        fd.append('folder', 'user_documents');
        try {
          const res = await authService.apiFetch('uploads', { method: 'POST', body: fd });
          if (res.ok) frontUrl = (await res.json()).url;
        } catch (e) { console.error('Lỗi upload mặt trước', e); }
      }

      if (backInput && backInput.files[0]) {
        const fd = new FormData();
        fd.append('file', backInput.files[0]);
        fd.append('folder', 'user_documents');
        try {
          const res = await authService.apiFetch('uploads', { method: 'POST', body: fd });
          if (res.ok) backUrl = (await res.json()).url;
        } catch (e) { console.error('Lỗi upload mặt sau', e); }
      }

      // Save uploaded URLs to mock DB so they persist in the UI for this user
      if (frontUrl && !docFront && window.VivuCarDB) {
        window.VivuCarDB.user_documents = window.VivuCarDB.user_documents || [];
        window.VivuCarDB.user_documents.push({ id: window.VivuCarDB.user_documents.length + 1, user_id: currentUser.id, document_type: 'license_front', file_name: 'front.jpg', file_url: frontUrl, verified: false, created_at: new Date().toISOString() });
      }
      if (backUrl && !docBack && window.VivuCarDB) {
        window.VivuCarDB.user_documents = window.VivuCarDB.user_documents || [];
        window.VivuCarDB.user_documents.push({ id: window.VivuCarDB.user_documents.length + 1, user_id: currentUser.id, document_type: 'license_back', file_name: 'back.jpg', file_url: backUrl, verified: false, created_at: new Date().toISOString() });
      }
      window.VivuCarSaveDB?.();

      // 2. Submit booking
      const r = await authService.apiFetch('bookings', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          carId,
          startDateTime: pickup,
          endDateTime: ret,
          pickupLocation: address,
          returnLocation: address,
          voucherCode: null,
          driverInfo: {
            fullName: document.getElementById('driverName').value.trim(),
            phoneNumber: document.getElementById('driverPhone').value.trim(),
            citizenIdNumber: document.getElementById('driverNationalId').value.trim(),
            driverLicenseNumber: document.getElementById('driverLicense').value.trim()
          }
        })
      });

      if (!r.ok) {
        const err = await r.json().catch(() => ({}));
        if (r.status === 401) {
          showToast('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.', 'danger');
          // Clear stale tokens, then redirect back here after re-login
          await authService.logout().catch(() => {});
          setTimeout(() => location.href = `/?redirect=${encodeURIComponent(location.pathname + location.search)}`, 1500);
        } else {
          showToast(err.message || 'Không thể tạo đơn thuê. Vui lòng thử lại.', 'danger');
          setLoading(btn, false);
        }
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
