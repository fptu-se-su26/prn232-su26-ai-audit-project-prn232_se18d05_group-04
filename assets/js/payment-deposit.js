(function () {
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const bookingId = Number(new URLSearchParams(location.search).get("bookingId"));
  const root = U.byId("paymentDepositRoot");

  let bookingData = null;

  async function fetchBooking() {
    try {
      const res = await Auth.fetchWithAuth(`${C.API_BASE_URL}/bookings/${bookingId}`);
      if (!res.ok) throw new Error("Không thể tải chi tiết đơn.");
      bookingData = await res.json();
      render();
    } catch (e) {
      root.innerHTML = U.renderEmptyState({ title: "Lỗi", text: e.message, href: "my-bookings.html", action: "Về danh sách đơn" });
    }
  }

  function render() {
    if (!bookingData) return;
    const b = bookingData;
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "my-bookings.html" },
      { label: `#${bookingId}`, href: `booking-detail.html?bookingId=${bookingId}` },
      { label: "Thanh toán cọc" }
    ]);
    
    // Nếu đơn không còn ở trạng thái PendingApproval, không cho phép thanh toán cọc nữa
    if (b.status !== "PendingApproval") {
        root.innerHTML = U.renderEmptyState({ title: "Không thể thanh toán", text: `Đơn thuê hiện đang ở trạng thái ${b.status}, không thể thực hiện thanh toán cọc.`, href: `booking-detail.html?bookingId=${bookingId}`, action: "Xem chi tiết" });
        return;
    }

    const uiLabel = "Chờ thanh toán cọc";
    const uiTone = "warning";

    root.innerHTML = `
      <div class="checkout-layout">
        <section class="checkout-main">
          <div class="checkout-section">
            <h1>Thanh toán tiền cọc</h1>
            <p class="muted">Đơn #${b.id} · ${b.carName} · ${uiLabel}</p>
            ${U.renderStatusBadge(uiTone, b.status)}
          </div>
          <div class="checkout-section">
            <h2>Chọn phương thức</h2>
            <div class="payment-method-grid">
              ${["vnpay"].map((method) => `<label class="method-card"><input type="radio" name="method" value="${method}" checked><strong>VNPay</strong><span class="muted">Thanh toán online qua cổng VNPay</span></label>`).join("")}
            </div>
            <button class="btn btn-primary btn-full" id="payNow" type="button">Tiếp tục thanh toán</button>
          </div>
        </section>
        <aside class="summary-card">
          <h2>Tóm tắt đơn</h2>
          <div class="summary-row"><span>Tổng tiền thuê</span><strong>${U.formatVnd(b.totalAmount)}</strong></div>
          <div class="summary-row"><span>Cần đặt cọc (30%)</span><strong>${U.formatVnd(b.depositAmount)}</strong></div>
          <div class="summary-row"><span>Trạng thái thanh toán</span><strong>Chờ thanh toán</strong></div>
          <div class="summary-total mt-4"><span>Số tiền cần thanh toán</span><strong class="text-blue-600">${U.formatVnd(b.depositAmount)}</strong></div>
        </aside>
      </div>`;
    U.byId("payNow").addEventListener("click", payNow);
  }

  async function payNow() {
    const method = document.querySelector("input[name='method']:checked").value;
    U.byId("payNow").disabled = true;
    U.byId("payNow").textContent = "Đang khởi tạo...";

    try {
        const returnUrl = window.location.origin + window.location.pathname.replace("payment-deposit.html", "payment-result.html");
        const res = await Auth.fetchWithAuth(`${C.API_BASE_URL}/payments/deposit/create`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                bookingId: bookingId,
                paymentMethod: method,
                returnUrl: returnUrl
            })
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.message || "Lỗi khởi tạo thanh toán");

        if (data.paymentUrl) {
            // Redirect to VNPay
            window.location.href = data.paymentUrl;
        } else {
            throw new Error("Không nhận được URL thanh toán từ hệ thống.");
        }
    } catch (e) {
        U.renderToast(e.message, "danger");
        U.byId("payNow").disabled = false;
        U.byId("payNow").textContent = "Tiếp tục thanh toán";
    }
  }

  root.innerHTML = "<p class='p-8 text-center text-gray-500'>Đang tải dữ liệu...</p>";
  fetchBooking();
})();
