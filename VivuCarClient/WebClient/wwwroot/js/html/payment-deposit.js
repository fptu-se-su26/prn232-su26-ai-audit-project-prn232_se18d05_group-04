/**
 * payment-deposit.js
 * Uses real API for deposit payment
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
  const U = window.VivuCarUtils;
  
  const session = await authService.getValidSession();
  const currentUser = session?.user ?? authService.getUser();

  // Patch header to show logged-in user (layout.js uses mock auth, so we bridge)
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
  const root = U.byId("paymentDepositRoot");

  let booking = null;
  let paymentStatus = null;

  async function loadData() {
    if (!bookingId) {
      renderError();
      return;
    }
    
    try {
      const r = await authService.apiFetch(`bookings/${bookingId}`);
      if (!r.ok) throw new Error('Booking not found');
      booking = await r.json();
      
      const pr = await authService.apiFetch(`payments/status/${bookingId}`);
      if (pr.ok) {
        paymentStatus = await pr.json();
      }
      
      render();
    } catch (e) {
      console.error(e);
      renderError();
    }
  }

  function renderError() {
    root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy thanh toán", text: "Đường dẫn không hợp lệ hoặc lỗi kết nối.", href: "/Booking/MyBookings", action: "Về danh sách đơn" });
  }

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "/Booking/MyBookings" },
      { label: `#${booking.bookingCode || booking.id}`, href: `/Booking/Detail?id=${booking.id}` },
      { label: "Thanh toán cọc" }
    ]);
    
    const paid = paymentStatus?.paymentStatus === 'success';
    const amountToPay = paid ? 0 : booking.depositAmount;
    
    root.innerHTML = `
      <div class="checkout-layout">
        <section class="checkout-main">
          <div class="checkout-section">
            <h1>Thanh toán tiền cọc</h1>
            <p class="muted">Đơn #${booking.bookingCode || booking.id} · ${booking.carName} · ${paid ? 'Đã thanh toán' : 'Chờ thanh toán'}</p>
            ${U.renderStatusBadge(paid ? "success" : "warning", paid ? "Đã thanh toán cọc" : "Chờ cọc")}
          </div>
          <div class="checkout-section">
            <h2>Chọn phương thức</h2>
            <div class="payment-method-grid">
              ${["payos"].map((method) => `
                <label class="method-card">
                  <input type="radio" name="method" value="${method}" checked>
                  <strong>Thanh toán qua PayOS</strong>
                  <span class="muted">Thanh toán qua mã QR</span>
                </label>
              `).join("")}
            </div>
            <button class="btn btn-primary btn-full" id="payNow" type="button" ${paid ? 'disabled' : ''}>${paid ? "Đã thanh toán" : "Tiếp tục thanh toán"}</button>
            ${!paid ? `<button class="btn btn-success btn-full mt-3" id="mockSuccessBtn" type="button" style="background-color: #10b981; border-color: #10b981;">[Test] Mô phỏng thanh toán thành công</button>` : ''}
          </div>
        </section>
        <aside class="summary-card">
          <h2>Tóm tắt đơn</h2>
          <div class="summary-row"><span>Tổng tiền thuê</span><strong>${U.formatVnd(booking.totalAmount)}</strong></div>
          <div class="summary-row"><span>Tiền cọc</span><strong>${U.formatVnd(booking.depositAmount)}</strong></div>
          <div class="summary-row"><span>Trạng thái payment</span><strong>${paymentStatus?.paymentStatus || 'pending'}</strong></div>
          <div class="summary-total"><span>Cần thanh toán</span><strong>${U.formatVnd(amountToPay)}</strong></div>
        </aside>
      </div>`;
      
    const payBtn = U.byId("payNow");
    if (payBtn) payBtn.addEventListener("click", payNow);
    
    const mockBtn = U.byId("mockSuccessBtn");
    if (mockBtn) mockBtn.addEventListener("click", mockSuccess);
  }

  async function mockSuccess() {
    const btn = U.byId("mockSuccessBtn");
    btn.disabled = true;
    btn.textContent = "Ä ang xá»­ lÃ½...";
    
    try {
      const r = await authService.apiFetch('payments/deposit/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          bookingId: booking.id,
          method: "payos",
          returnUrl: location.origin + "/Payment/Result"
        })
      });
      
      if (!r.ok) {
        U.renderToast('Lá»—i táº¡o thanh toÃ¡n', 'danger');
        btn.disabled = false;
        btn.textContent = "[Test] MÃ´ phỏng thanh toÃ¡n thÃ nh cÃ´ng";
        return;
      }
      
      const response = await r.json();
      if (response.transactionCode) {
        const callbackUrl = `/api/proxy/payments/callback?TransactionCode=${response.transactionCode}&Status=success&RedirectUrl=${encodeURIComponent(location.origin + "/Payment/Result")}`;
        window.location.href = callbackUrl;
      }
    } catch (e) {
      console.error(e);
      U.renderToast('Lá»—i káº¿t ná»‘i.', 'danger');
      btn.disabled = false;
      btn.textContent = "[Test] MÃ´ phỏng thanh toÃ¡n thÃ nh cÃ´ng";
    }
  }

  async function payNow() {
    const method = document.querySelector("input[name='method']:checked")?.value;
    if (!method) return;
    
    const btn = U.byId("payNow");
    btn.disabled = true;
    btn.textContent = "Đang xử lý...";
    
    try {
      const currentUrl = window.location.origin;
      const r = await authService.apiFetch('payments/deposit/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          bookingId: booking.id,
          method: method,
          returnUrl: `${currentUrl}/Payment/Result`
        })
      });
      
      if (!r.ok) {
        const err = await r.json().catch(()=>({}));
        U.renderToast(err.message || 'Lỗi tạo thanh toán', 'danger');
        btn.disabled = false;
        btn.textContent = "Tiếp tục thanh toán";
        return;
      }
      
      const response = await r.json();
      if (response.paymentUrl) {
        window.location.href = response.paymentUrl;
      } else {
        // If cash or no redirect url needed
        U.renderToast("Đã ghi nhận thanh toán.", "success");
        setTimeout(() => location.href = `/Booking/Detail?id=${booking.id}`, 1000);
      }
      
    } catch (e) {
      console.error(e);
      U.renderToast('Lỗi kết nối. Vui lòng thử lại.', 'danger');
      btn.disabled = false;
      btn.textContent = "Tiếp tục thanh toán";
    }
  }

  loadData();
})();
