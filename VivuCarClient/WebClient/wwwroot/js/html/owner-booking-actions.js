(function () {
  const U = window.VivuCarUtils;
  function ownerBooking(bookingId) {
    const DB = window.VivuCarDB, owner = window.VivuCarAuth.getCurrentUser();
    const booking = DB.bookings.find((item) => item.id === Number(bookingId));
    const car = DB.cars.find((item) => item.id === booking?.car_id && item.owner_id === owner?.id);
    return car ? { booking, car, user: DB.users.find((item) => item.id === booking.user_id), payment: U.paymentForBooking(booking.id) } : null;
  }
  function ensureModals(onDone) {
    if (document.getElementById("acceptBookingModal")) return;
    document.body.insertAdjacentHTML("beforeend", `<div class="modal-backdrop" id="acceptBookingModal"><div class="modal"><div class="modal-header"><h2>Xác nhận yêu cầu đặt xe</h2><button class="icon-button" data-action-close>×</button></div><div id="acceptBody"></div><label><input type="checkbox" id="confirmCarReady"> Xe sẵn sàng cho thuê</label><label><input type="checkbox" id="confirmScheduleAvailable"> Lịch xe không bị trùng</label><label><input type="checkbox" id="confirmHandoverPlan"> Sẽ chuẩn bị bàn giao đúng giờ</label><textarea id="acceptNote" placeholder="Ghi chú cho khách"></textarea><div class="modal-actions"><button class="btn btn-secondary" data-action-close>Hủy</button><button class="btn btn-primary" id="btnConfirmAcceptBooking">Xác nhận đơn</button></div></div></div><div class="modal-backdrop" id="declineBookingModal"><div class="modal"><div class="modal-header"><h2>Từ chối yêu cầu</h2><button class="icon-button" data-action-close>×</button></div><select id="declineReason"><option>Xe không sẵn sàng</option><option>Trùng lịch sử dụng</option><option>Cần bảo trì xe</option><option>Khác</option></select><textarea id="declineNote" placeholder="Lý do chi tiết"></textarea><p class="muted">Decline reason là UI-only vì bookings không có decline_reason.</p><div class="modal-actions"><button class="btn btn-secondary" data-action-close>Hủy</button><button class="btn btn-danger" id="btnConfirmDeclineBooking">Xác nhận từ chối</button></div></div></div>`);
    document.addEventListener("click", (event) => {
      if (event.target.matches("[data-action-close]") || event.target.classList.contains("modal-backdrop")) {
        const modal = event.target.closest(".modal-backdrop");
        if (modal) U.closeModal(modal.id);
      }
    });
  }
  function openAcceptBookingModal(bookingId, onDone) {
    ensureModals(onDone);
    const record = ownerBooking(bookingId);
    if (!record || record.booking.status !== "pending") return U.renderToast("Không thể xác nhận yêu cầu này.", "danger");
    document.getElementById("acceptBody").innerHTML = `<p class="muted">#${record.booking.id} · ${record.user.full_name} · ${U.carTitle(record.car)} · ${U.formatVnd(record.booking.total_amount)}</p>`;
    document.getElementById("btnConfirmAcceptBooking").onclick = () => {
      const ok = ["confirmCarReady", "confirmScheduleAvailable", "confirmHandoverPlan"].every((id) => document.getElementById(id).checked);
      if (!ok) return U.renderToast("Vui lòng xác nhận đủ checklist.", "danger");
      record.booking.status = "approved";
      if (new Date(record.booking.pickup_datetime) <= new Date() && new Date(record.booking.return_datetime) >= new Date()) record.car.status = "rented";
      window.VivuCarSaveDB(); U.closeModal("acceptBookingModal"); U.renderToast("Đã xác nhận yêu cầu đặt xe.", "success"); if (onDone) onDone();
    };
    U.openModal("acceptBookingModal");
  }
  function openDeclineBookingModal(bookingId, onDone) {
    ensureModals(onDone);
    const record = ownerBooking(bookingId);
    if (!record || record.booking.status !== "pending") return U.renderToast("Không thể từ chối yêu cầu này.", "danger");
    document.getElementById("btnConfirmDeclineBooking").onclick = () => {
      if (document.getElementById("declineReason").value === "Khác" && document.getElementById("declineNote").value.trim().length < 10) return U.renderToast("Lý do khác tối thiểu 10 ký tự.", "danger");
      record.booking.status = "rejected";
      // UI-only field. Not present in current DB schema. Requires migration before backend integration.
      record.booking.decline_reason = document.getElementById("declineNote").value.trim();
      window.VivuCarSaveDB(); U.closeModal("declineBookingModal"); U.renderToast("Đã từ chối yêu cầu đặt xe.", "success"); if (onDone) onDone();
    };
    U.openModal("declineBookingModal");
  }
  function openCustomerChat(userId, bookingId) {
    const DB = window.VivuCarDB;
    let session = DB.chat_sessions.find((item) => item.user_id === Number(userId) && item.booking_id === Number(bookingId) && item.status !== "closed");
    if (!session) {
      session = { id: Math.max(0, ...DB.chat_sessions.map((item) => item.id)) + 1, user_id: Number(userId), booking_id: Number(bookingId), session_type: "live", status: "open", assigned_to: window.VivuCarAuth.getCurrentUser().id, escalated_at: null, closed_at: null, created_at: new Date().toISOString() };
      DB.chat_sessions.push(session); window.VivuCarSaveDB();
    }
    location.href = `owner-support-inbox.html?conversationId=${session.id}`;
  }
  window.VivuCarOwnerBookingActions = { ownerBooking, openAcceptBookingModal, openDeclineBookingModal, openCustomerChat };
})();

