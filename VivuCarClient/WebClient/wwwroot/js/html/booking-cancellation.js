/**
 * booking-cancellation.js
 * Uses API for cancelling bookings
 */
import { authService } from '/js/shared/auth-service.js';

(function () {
  const U = window.VivuCarUtils;

  function ensureModal() {
    if (document.getElementById("cancelBookingModal")) return;
    document.body.insertAdjacentHTML("beforeend", `
      <div class="modal-backdrop" id="cancelBookingModal" role="dialog" aria-modal="true">
        <div class="modal">
          <div class="modal-header"><h2>Hủy đơn thuê</h2><button class="icon-button" type="button" data-cancel-close>×</button></div>
          <p class="muted" id="cancelBookingText"></p>
          <label>Lý do hủy</label>
          <textarea id="cancelReason" rows="3" placeholder="Nhập lý do để chủ xe nắm thông tin"></textarea>
          <p class="muted">Nếu đơn đã thanh toán thành công, hoàn tiền sẽ được xử lý theo chính sách của VivuCar.</p>
          <div class="modal-actions">
            <button class="btn btn-secondary" type="button" data-cancel-close>Giữ đơn</button>
            <button class="btn btn-danger" type="button" id="confirmCancelBooking">Xác nhận hủy</button>
          </div>
        </div>
      </div>`);
    document.addEventListener("click", (event) => {
      if (event.target.matches("[data-cancel-close]") || event.target.id === "cancelBookingModal") U.closeModal("cancelBookingModal");
    });
  }

  async function cancelBooking(bookingId) {
    try {
      const reason = document.getElementById("cancelReason")?.value?.trim() || "Người dùng không cung cấp lý do";
      
      const r = await authService.apiFetch(`bookings/${bookingId}/cancel`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ reason: reason, note: "", confirmed: true })
      });
      
      if (!r.ok) {
        const err = await r.json().catch(()=>({}));
        return { ok: false, message: err.message || "Không thể hủy đơn. Vui lòng thử lại sau." };
      }
      
      return { ok: true, message: "Đã hủy đơn thuê." };
    } catch (e) {
      console.error(e);
      return { ok: false, message: "Lỗi kết nối. Vui lòng thử lại sau." };
    }
  }

  async function openCancelBookingModal(bookingId, onDone) {
    ensureModal();
    const btn = document.getElementById("confirmCancelBooking");
    document.getElementById("cancelBookingText").textContent = `Bạn đang thực hiện hủy đơn #${bookingId}. Hành động này không thể hoàn tác.`;
    
    btn.onclick = async () => {
      btn.disabled = true;
      btn.textContent = "Đang xử lý...";
      
      const result = await cancelBooking(bookingId);
      
      btn.disabled = false;
      btn.textContent = "Xác nhận hủy";
      
      U.renderToast(result.message, result.ok ? "success" : "danger");
      if (result.ok) {
        U.closeModal("cancelBookingModal");
        if (typeof onDone === "function") onDone();
      }
    };
    U.openModal("cancelBookingModal");
  }

  window.VivuCarBookingCancellation = { openCancelBookingModal };
})();
