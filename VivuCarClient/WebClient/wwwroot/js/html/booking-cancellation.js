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

  function cancelBooking(bookingId) {
    const booking = window.VivuCarDB.bookings.find((item) => item.id === Number(bookingId));
    if (!U.canCancelBooking(booking)) return { ok: false, message: "Đơn này không còn đủ điều kiện hủy." };
    booking.status = "cancelled";
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    booking.cancellation_reason = document.getElementById("cancelReason")?.value?.trim() || "";
    window.VivuCarSaveDB();
    return { ok: true, message: "Đã hủy đơn thuê." };
  }

  function openCancelBookingModal(bookingId, onDone) {
    ensureModal();
    const booking = window.VivuCarDB.bookings.find((item) => item.id === Number(bookingId));
    document.getElementById("cancelBookingText").textContent = `Bạn đang hủy đơn #${bookingId}. Trạng thái gốc hiện tại: ${booking?.status || "unknown"}.`;
    document.getElementById("confirmCancelBooking").onclick = () => {
      const result = cancelBooking(bookingId);
      U.renderToast(result.message, result.ok ? "success" : "danger");
      U.closeModal("cancelBookingModal");
      if (result.ok && typeof onDone === "function") onDone();
    };
    U.openModal("cancelBookingModal");
  }

  window.VivuCarBookingCancellation = { openCancelBookingModal, cancelBooking };
})();

