/**
 * payment-final.js
 * Final payment page after owner return inspection.
 * Customer pays RemainingAmount + ExtraFee + OverdueFee via PayOS.
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
  const U = window.VivuCarUtils;
  
  const session = await authService.getValidSession();
  const currentUser = session?.user ?? authService.getUser();

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
  
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const root = U.byId("paymentFinalRoot");

  let booking = null;

  async function loadData() {
    if (!bookingId) {
      renderError();
      return;
    }
    
    try {
      const r = await authService.apiFetch(`bookings/${bookingId}`);
      if (!r.ok) throw new Error('Booking not found');
      booking = await r.json();
      
      render();
    } catch (e) {
      console.error(e);
      renderError();
    }
  }

  function renderError() {
    root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy đơn", text: "Đường dẫn không hợp lệ hoặc lỗi kết nối.", href: "/Booking/MyBookings", action: "Về danh sách đơn" });
  }

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "/Booking/MyBookings" },
      { label: `#${booking.bookingCode || booking.id}`, href: `/Booking/Detail?id=${booking.id}` },
      { label: "Thanh toán cuối chuyến" }
    ]);
    
    const canPay = booking.canPayFinal;
    const finalAmount = booking.finalPaymentAmount || 0;
    
    root.innerHTML = `
      <div class="checkout-layout">
        <section class="checkout-main">
          <div class="checkout-section">
            <h1>Thanh toán cuối chuyến</h1>
            <p class="muted">Đơn #${booking.bookingCode || booking.id} · ${booking.carName}</p>
            ${canPay 
              ? U.renderStatusBadge("warning", "Chờ thanh toán cuối") 
              : U.renderStatusBadge("success", "Đã hoàn tất")}
          </div>
          ${canPay ? `
          <div class="checkout-section">
            <h2>Chọn phương thức</h2>
            <div class="payment-method-grid">
              <label class="method-card">
                <input type="radio" name="method" value="payos" checked>
                <strong>Thanh toán qua PayOS</strong>
                <span class="muted">Thanh toán qua mã QR</span>
              </label>
            </div>
            <button class="btn btn-primary btn-full" id="payFinalBtn" type="button">Thanh toán ${U.formatVnd(finalAmount)}</button>
            <button class="btn btn-success btn-full mt-3" id="mockFinalBtn" type="button" style="background-color: #10b981; border-color: #10b981;">[Test] Mô phỏng thanh toán thành công</button>
          </div>
          ` : `
          <div class="checkout-section">
            <div style="padding:16px;background:#ecfdf5;border:1px solid #a7f3d0;color:#065f46;border-radius:8px;font-size:14px;">
              ✅ <strong>Đã thanh toán thành công!</strong> Chuyến xe đã hoàn tất.
            </div>
            <a class="btn btn-primary btn-full mt-3" href="/Booking/Detail?id=${booking.id}">Xem chi tiết đơn</a>
          </div>
          `}
        </section>
        <aside class="summary-card">
          <h2>Chi tiết thanh toán</h2>
          <div class="summary-row"><span>Tổng tiền thuê</span><strong>${U.formatVnd(booking.totalAmount)}</strong></div>
          <div class="summary-row"><span>Đã cọc</span><strong style="color:#15803d;">- ${U.formatVnd(booking.depositAmount)}</strong></div>
          <div class="summary-row"><span>Tiền còn lại</span><strong>${U.formatVnd(booking.remainingAmount)}</strong></div>
          ${booking.extraFee > 0 ? `<div class="summary-row"><span>Phụ phí phát sinh</span><strong style="color:#ea580c;">+ ${U.formatVnd(booking.extraFee)}</strong></div>` : ""}
          ${(booking.overdueFee || 0) > 0 ? `<div class="summary-row"><span>Phí trả trễ</span><strong style="color:#ea580c;">+ ${U.formatVnd(booking.overdueFee)}</strong></div>` : ""}
          <div class="summary-total"><span>Cần thanh toán</span><strong style="color:#dc2626;">${U.formatVnd(finalAmount)}</strong></div>
        </aside>
      </div>`;
      
    const payBtn = U.byId("payFinalBtn");
    if (payBtn) payBtn.addEventListener("click", payFinal);
    
    const mockBtn = U.byId("mockFinalBtn");
    if (mockBtn) mockBtn.addEventListener("click", mockFinalSuccess);
  }

  async function mockFinalSuccess() {
    const btn = U.byId("mockFinalBtn");
    btn.disabled = true;
    btn.textContent = "Đang xử lý...";
    
    try {
      const r = await authService.apiFetch('payments/final/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          bookingId: booking.id,
          method: "payos",
          returnUrl: location.origin + "/Payment/Result"
        })
      });
      
      if (!r.ok) {
        const err = await r.json().catch(() => ({}));
        U.showToast(err.message || 'Lỗi tạo thanh toán', 'danger');
        btn.disabled = false;
        btn.textContent = "[Test] Mô phỏng thanh toán thành công";
        return;
      }
      
      const response = await r.json();
      if (response.transactionCode) {
        const callbackUrl = `/api/proxy/payments/callback?TransactionCode=${response.transactionCode}&Status=success&RedirectUrl=${encodeURIComponent(location.origin + "/Payment/Result")}`;
        window.location.href = callbackUrl;
      } else if (!response.paymentUrl) {
        // finalAmount was 0, auto-completed
        U.showToast("Chuyến xe đã hoàn tất!", "success");
        setTimeout(() => location.href = `/Booking/Detail?id=${booking.id}`, 1000);
      }
    } catch (e) {
      console.error(e);
      U.showToast('Lỗi kết nối.', 'danger');
      btn.disabled = false;
      btn.textContent = "[Test] Mô phỏng thanh toán thành công";
    }
  }

  async function payFinal() {
    const method = document.querySelector("input[name='method']:checked")?.value;
    if (!method) return;
    
    const btn = U.byId("payFinalBtn");
    btn.disabled = true;
    btn.textContent = "Đang xử lý...";
    
    try {
      const r = await authService.apiFetch('payments/final/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          bookingId: booking.id,
          method: method,
          returnUrl: `${location.origin}/Payment/Result`
        })
      });
      
      if (!r.ok) {
        const err = await r.json().catch(() => ({}));
        U.showToast(err.message || 'Lỗi tạo thanh toán', 'danger');
        btn.disabled = false;
        btn.textContent = `Thanh toán ${U.formatVnd(booking.finalPaymentAmount)}`;
        return;
      }
      
      const response = await r.json();
      if (response.paymentUrl) {
        window.location.href = response.paymentUrl;
      } else if (!response.transactionCode) {
        // finalAmount was 0, auto-completed
        U.showToast("Chuyến xe đã hoàn tất!", "success");
        setTimeout(() => location.href = `/Booking/Detail?id=${booking.id}`, 1000);
      }
      
    } catch (e) {
      console.error(e);
      U.showToast('Lỗi kết nối. Vui lòng thử lại.', 'danger');
      btn.disabled = false;
      btn.textContent = `Thanh toán ${U.formatVnd(booking.finalPaymentAmount)}`;
    }
  }

  loadData();
})();
