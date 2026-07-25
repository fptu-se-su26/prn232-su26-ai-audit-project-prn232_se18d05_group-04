/**
 * payment-result.js
 * Hiển thị kết quả thanh toán sau khi gateway redirect về.
 * Dùng real API thay vì mock DB.
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
  const U    = window.VivuCarUtils;
  const root = document.getElementById('paymentResultRoot');

  // ── 1. Auth ──────────────────────────────────────────────────────────────
  const session = await authService.getValidSession();
  const currentUser = session?.user ?? authService.getUser();

  // Patch header for real auth (same pattern as checkout)
  function patchHeader(user) {
    if (!user) return;
    if (window.VivuCarLayout?.patchHeaderForRealUser) {
      window.VivuCarLayout.patchHeaderForRealUser(user);
      return;
    }
    const headerMount = document.getElementById('headerMount');
    if (!headerMount) return;
    const loginLink = headerMount.querySelector('a[href="/Login"], a[href*="login"]');
    if (loginLink) {
      loginLink.textContent = user.fullName || user.email || 'Tài khoản';
      loginLink.href = '/Profile';
      loginLink.classList.remove('btn-primary');
      loginLink.classList.add('btn-secondary');
    }
  }
  requestAnimationFrame(() => patchHeader(currentUser));
  setTimeout(() => patchHeader(currentUser), 300);

  if (!currentUser) {
    location.href = '/';
    return;
  }

  // ── 2. Đọc params từ URL ─────────────────────────────────────────────────
  const params     = new URLSearchParams(location.search);
  const bookingId  = Number(params.get('bookingId'));

  // Xác định kết quả từ gateway params (VNPay / MoMo / internal)
  // VNPay: vnp_ResponseCode=00 → success
  // MoMo:  resultCode=0 → success
  // Internal: status=success/failed
  function resolveGatewayStatus() {
    const vnpCode    = params.get('vnp_ResponseCode');
    const momoCode   = params.get('resultCode');
    const internalSt = params.get('status');

    if (vnpCode !== null) return vnpCode === '00' ? 'success' : 'failed';
    if (momoCode !== null) return momoCode === '0' ? 'success' : 'failed';
    if (internalSt === 'success' || internalSt === 'failed' || internalSt === 'pending') return internalSt;
    return 'pending';
  }

  const gatewayStatus = resolveGatewayStatus();

  // ── 3. Load booking từ API ────────────────────────────────────────────────
  if (!bookingId) {
    root.innerHTML = `
      <div style="text-align:center;padding:64px 24px">
        <div style="font-size:3rem;margin-bottom:16px">⚠️</div>
        <h2 style="font-size:1.25rem;font-weight:700;color:#1f1f1f;margin:0 0 8px">Không tìm thấy đơn</h2>
        <p style="color:#6b7280;margin:0 0 24px">Đường dẫn không hợp lệ.</p>
        <a href="/cars" style="background:#16a34a;color:#fff;padding:10px 20px;border-radius:8px;text-decoration:none;font-weight:600;font-size:.875rem">← Về trang chủ</a>
      </div>`;
    return;
  }

  let booking     = null;
  let payStatus   = null;

  try {
    const r = await authService.apiFetch(`bookings/${bookingId}`);
    if (r.ok) booking = await r.json();

    const pr = await authService.apiFetch(`payments/status/${bookingId}`);
    if (pr.ok) payStatus = await pr.json();
  } catch (e) {
    console.error('Error loading payment result data:', e);
  }

  // ── 4. Render ────────────────────────────────────────────────────────────
  // Sử dụng gatewayStatus hoặc paymentStatus từ API (ưu tiên API nếu có)
  const finalStatus = payStatus?.paymentStatus || gatewayStatus;

  const statusMap = {
    success: {
      tone : 'success',
      icon : '✓',
      color: '#16a34a',
      bg   : '#f0fdf4',
      title: 'Thanh toán thành công!',
      text : 'Đã ghi nhận cọc. Đơn đang chờ chủ xe xác nhận.',
      btnLabel: 'Tiến hành ký hợp đồng',
      btnHref : booking ? `/Booking/Contract?id=${booking.id}` : '/Booking/MyBookings'
    },
    failed: {
      tone : 'danger',
      icon : '✗',
      color: '#dc2626',
      bg   : '#fef2f2',
      title: 'Thanh toán thất bại',
      text : 'Giao dịch không thành công. Bạn có thể thử lại.',
      btnLabel: 'Thử lại',
      btnHref : booking ? `/Payment/Deposit?bookingId=${booking.id}` : '/Booking/MyBookings'
    },
    pending: {
      tone : 'warning',
      icon : '…',
      color: '#b45309',
      bg   : '#fffbeb',
      title: 'Đang xử lý',
      text : 'Giao dịch đang được xử lý. Vui lòng chờ hoặc kiểm tra lại sau.',
      btnLabel: 'Xem đơn của tôi',
      btnHref : '/Booking/MyBookings'
    }
  };

  const s = statusMap[finalStatus] || statusMap.pending;

  const txCode = params.get('vnp_TransactionNo') || params.get('transId') || payStatus?.transactionCode || '—';
  const amount = payStatus?.amount
    ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(payStatus.amount)
    : (booking?.depositAmount ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(booking.depositAmount) : '—');

  root.innerHTML = `
    <div style="max-width:480px;margin:48px auto;padding:0 16px">
      <div style="background:#fff;border:1px solid #e6e4df;border-radius:16px;padding:32px;text-align:center">

        <!-- Status icon -->
        <div style="width:72px;height:72px;border-radius:50%;background:${s.bg};border:2px solid ${s.color};display:flex;align-items:center;justify-content:center;font-size:2rem;font-weight:700;color:${s.color};margin:0 auto 20px">
          ${s.icon}
        </div>

        <h1 style="font-size:1.375rem;font-weight:800;color:#1f1f1f;margin:0 0 8px">${s.title}</h1>
        <p style="color:#6b7280;font-size:.9375rem;margin:0 0 24px;line-height:1.6">${s.text}</p>

        <!-- Summary rows -->
        ${booking ? `
        <div style="background:#f7f7f5;border-radius:12px;padding:16px;text-align:left;margin-bottom:24px">
          <div style="display:flex;justify-content:space-between;padding:6px 0;font-size:.875rem;border-bottom:1px solid #e6e4df">
            <span style="color:#6b7280">Mã đơn</span>
            <strong>#${booking.bookingCode || booking.id}</strong>
          </div>
          <div style="display:flex;justify-content:space-between;padding:6px 0;font-size:.875rem;border-bottom:1px solid #e6e4df">
            <span style="color:#6b7280">Xe thuê</span>
            <strong>${booking.carName || '—'}</strong>
          </div>
          <div style="display:flex;justify-content:space-between;padding:6px 0;font-size:.875rem;border-bottom:1px solid #e6e4df">
            <span style="color:#6b7280">Mã giao dịch</span>
            <strong>${txCode}</strong>
          </div>
          <div style="display:flex;justify-content:space-between;padding:8px 0 0;font-size:1rem">
            <span style="font-weight:700;color:#1f1f1f">Số tiền cọc</span>
            <strong style="color:${s.color}">${amount}</strong>
          </div>
        </div>` : ''}

        <!-- Action buttons -->
        <div style="display:flex;flex-direction:column;gap:10px">
          <a href="${s.btnHref}"
             style="display:block;background:#16a34a;color:#fff;padding:12px;border-radius:10px;text-decoration:none;font-weight:700;font-size:.9375rem;transition:background .2s"
             onmouseover="this.style.background='#15803d'"
             onmouseout="this.style.background='#16a34a'">
            ${s.btnLabel}
          </a>
          <a href="/Booking/MyBookings"
             style="display:block;border:1px solid #e6e4df;color:#374151;padding:12px;border-radius:10px;text-decoration:none;font-weight:600;font-size:.875rem;transition:background .2s"
             onmouseover="this.style.background='#f7f7f5'"
             onmouseout="this.style.background=''">
            Xem tất cả đơn thuê
          </a>
          <a href="/cars"
             style="color:#6b7280;font-size:.875rem;text-decoration:none">
            ← Về trang chủ
          </a>
        </div>

      </div>
    </div>`;
})();
