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
      <div id="cancelBookingModal" class="fixed inset-0 z-[100] hidden items-center justify-center bg-black/50" role="dialog" aria-modal="true">
        <div class="bg-white rounded-xl shadow-2xl w-full max-w-[500px] p-6 mx-4 relative">
          <div class="flex items-center justify-between mb-4">
            <h2 class="font-bold text-lg text-zinc-900 m-0">Hủy đơn thuê</h2>
            <button class="text-zinc-500 hover:text-zinc-700 text-xl font-bold cursor-pointer border-none bg-transparent" type="button" data-cancel-close>&times;</button>
          </div>
          <p class="text-zinc-600 text-sm mb-4" id="cancelBookingText"></p>
          <label class="block text-sm font-semibold text-zinc-700 mb-2">Lý do hủy</label>
          <textarea id="cancelReason" rows="3" class="w-full border border-zinc-300 rounded-lg p-3 text-sm focus:ring-2 focus:ring-primary focus:outline-none mb-3" placeholder="Nhập lý do để chủ xe nắm thông tin"></textarea>
          <p class="text-zinc-500 text-xs italic mb-6">Nếu đơn đã thanh toán thành công, hoàn tiền sẽ được xử lý theo chính sách của VivuCar.</p>
          <div class="flex justify-end gap-3">
            <button class="px-4 py-2 text-sm font-semibold text-zinc-700 bg-zinc-100 hover:bg-zinc-200 rounded-lg border-none cursor-pointer" type="button" data-cancel-close>Giữ đơn</button>
            <button class="px-4 py-2 text-sm font-semibold text-white bg-red-600 hover:bg-red-700 rounded-lg border-none cursor-pointer" type="button" id="confirmCancelBooking">Xác nhận hủy</button>
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
