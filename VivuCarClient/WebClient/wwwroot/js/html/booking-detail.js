/**
 * booking-detail.js
 * Uses real API for booking details
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
  const U = window.VivuCarUtils;
  
  const session = await authService.refresh();
  const currentUser = session?.user ?? null;
  if (!currentUser) {
    location.href = '/';
    return;
  }
  
  const bookingId = Number(new URLSearchParams(location.search).get("id")) || Number(new URLSearchParams(location.search).get("bookingId"));
  const root = U.byId("bookingDetailRoot");

  let booking = null;
  let paymentStatus = null;

  async function loadData() {
    if (!bookingId) {
      renderError();
      return;
    }
    
    root.innerHTML = `<div style="text-align:center;padding:48px;color:#6b7280">Đang tải thông tin đơn...</div>`;
    try {
      const r = await authService.apiFetch(`bookings/${bookingId}`);
      if (!r.ok) throw new Error('Booking not found');
      booking = await r.json();
      
      try {
        const pr = await authService.apiFetch(`payments/status/${bookingId}`);
        if (pr.ok) {
          paymentStatus = await pr.json();
        }
      } catch (e) {
        console.warn("Could not fetch payment status for", bookingId);
      }
      
      render();
    } catch (e) {
      console.error(e);
      renderError();
    }
  }

  function renderError() {
    root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy đơn", text: "Đơn không tồn tại hoặc lỗi kết nối.", href: "/Booking/MyBookings", action: "Về danh sách đơn" });
  }

  function resolveUiState() {
    const paid = paymentStatus?.paymentStatus === 'success';
    let key = "unknown", label = "Không xác định", tone = "neutral";
    
    if (booking.status === "cancelled") { key = "cancelled"; label = "Đã hủy"; tone = "danger"; }
    else if (booking.status === "rejected") { key = "rejected"; label = "Đã từ chối"; tone = "danger"; }
    else if (booking.status === "completed") { key = "completed"; label = "Hoàn tất"; tone = "success"; }
    else if (booking.status === "pending") {
      if (paid) { key = "handover_pending"; label = "Đã cọc - chờ xác nhận"; tone = "primary"; }
      else { key = "payment_pending"; label = "Chờ thanh toán"; tone = "warning"; }
    }
    else if (booking.status === "approved") {
      const pickupDate = new Date(booking.startDateTime);
      if (Date.now() < pickupDate.getTime()) { key = "handover_pending"; label = "Chờ bàn giao"; tone = "primary"; }
      else { key = "renting"; label = "Đang thuê"; tone = "primary"; }
    }
    return { key, label, tone };
  }

  function render() {
    const ui = resolveUiState();
    
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "/Booking/MyBookings" },
      { label: `#${booking.bookingCode || booking.id}` }
    ]);
    
    const paid = paymentStatus?.paymentStatus === 'success';
    const canCancel = booking.status === "pending" || booking.status === "approved";
    
    root.innerHTML = `
      <div class="detail-document">
        <section class="checkout-main">
          <div class="checkout-section">
            <div class="booking-toolbar">
              <div>
                <span class="eyebrow">Booking #${booking.bookingCode || booking.id}</span>
                <h1>${booking.carName}</h1>
                <p class="muted">${ui.label}</p>
              </div>
              <div class="chip-row">${U.renderStatusBadge(ui.tone, ui.label)}${U.renderStatusBadge("neutral", booking.status)}</div>
            </div>
            <div class="checkout-car">
              <img src="${booking.carImageUrl || '/img/placeholder-car.png'}" alt="${booking.carName}" style="object-fit:cover">
              <div class="info-grid">
                ${info("Biển số", booking.licensePlate)}
                ${info("Địa điểm nhận", booking.pickupLocation)}
                ${info("Nhận xe", new Date(booking.startDateTime).toLocaleString('vi-VN'))}
                ${info("Trả xe", new Date(booking.endDateTime).toLocaleString('vi-VN'))}
                ${info("Tổng tiền", U.formatVnd(booking.totalAmount))}
                ${info("Voucher", booking.voucherCode || "Không áp dụng")}
              </div>
            </div>
          </div>
          <div class="checkout-section">
            <h2>Thanh toán tiền cọc</h2>
            <div class="info-grid">
              ${info("Trạng thái", paymentStatus?.paymentStatus || (paid ? "Thành công" : "Chờ thanh toán"))}
              ${info("Mã giao dịch", paymentStatus?.transactionCode || "—")}
              ${info("Cần cọc", U.formatVnd(booking.depositAmount))}
            </div>
          </div>
        </section>
        <aside class="summary-card">
          <h2>Thời gian biểu</h2>
          <div class="timeline">${timeline()}</div>
          <div class="booking-actions mt-3.5">
            ${booking.status === "pending" && !paid ? `<a class="btn btn-primary btn-sm" href="/Payment/Deposit?bookingId=${booking.id}">Thanh toán cọc</a>` : ""}
            ${canCancel ? `<button class="btn btn-danger btn-sm" type="button" id="cancelBookingBtn">Hủy đơn</button>` : ""}
            ${booking.contractPdfUrl ? `<a class="btn btn-secondary btn-sm" href="${booking.contractPdfUrl}" target="_blank">Xem hợp đồng</a>` : ""}
            ${booking.status === "completed" ? `<a class="btn btn-ghost btn-sm" href="/Review/Create?bookingId=${booking.id}">Đánh giá sau chuyến</a>` : ""}
          </div>
        </aside>
      </div>`;
      
    const cancelBtn = U.byId("cancelBookingBtn");
    if (cancelBtn) {
      cancelBtn.addEventListener("click", () => {
        if(window.VivuCarBookingCancellation) {
          window.VivuCarBookingCancellation.openCancelBookingModal(booking.id, loadData);
        } else {
          alert("Chức năng hủy đang được cập nhật.");
        }
      });
    }
  }

  function info(label, value) {
    return `<div class="info-cell"><span>${label}</span><strong>${value}</strong></div>`;
  }

  function timeline() {
    const items = [
      ["Tạo đơn", new Date(booking.createdAt).toLocaleString('vi-VN')],
      booking.status === "completed" ? ["Hoàn tất", new Date(booking.endDateTime).toLocaleString('vi-VN')] : null,
      booking.status === "cancelled" ? ["Đã hủy", booking.cancellationReason || "Đơn đã bị hủy"] : null
    ].filter(Boolean);
    return items.map(([title, text]) => `<div class="timeline-item"><strong>${title}</strong><p>${text}</p></div>`).join("");
  }

  loadData();
})();
