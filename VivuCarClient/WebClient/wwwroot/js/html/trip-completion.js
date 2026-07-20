(function () {
  const U = window.VivuCarUtils;
  function canCompleteTrip(booking, car) {
    const inspections = U.inspectionsForBooking(booking?.id);
    const incidents = (window.VivuCarDB.incident_reports || []).filter((item) => item.booking_id === booking?.id && ["open", "in_review"].includes(item.status));
    return Boolean(booking && car && booking.status === "approved" && inspections.some((item) => item.inspection_type === "pre_rental") && inspections.some((item) => item.inspection_type === "post_rental") && !incidents.length);
  }
  function openCompleteTripModal(bookingId, onDone) {
    const DB = window.VivuCarDB;
    const booking = DB.bookings.find((item) => item.id === Number(bookingId));
    const car = DB.cars.find((item) => item.id === booking?.car_id);
    if (!document.getElementById("completeTripModal")) {
      document.body.insertAdjacentHTML("beforeend", `<div class="modal-backdrop" id="completeTripModal" role="dialog" aria-modal="true"><div class="modal"><div class="modal-header"><h2>Xác nhận hoàn tất chuyến đi</h2><button class="icon-button" type="button" data-complete-close>×</button></div><div id="completeTripBody"></div><label><input type="checkbox" id="confirmVehicleReceived"> Tôi xác nhận đã nhận lại xe</label><label><input type="checkbox" id="confirmNoDispute"> Không có tranh chấp hoặc phụ phí chưa xử lý</label><label><input type="checkbox" id="confirmReleaseCar"> Cho phép mở lại lịch thuê xe</label><div class="modal-actions"><button class="btn btn-secondary" type="button" data-complete-close>Hủy</button><button class="btn btn-primary" type="button" id="btnConfirmCompleteTrip">Xác nhận hoàn tất</button></div></div></div>`);
      document.addEventListener("click", (event) => { if (event.target.matches("[data-complete-close]") || event.target.id === "completeTripModal") U.closeModal("completeTripModal"); });
    }
    document.getElementById("completeTripBody").innerHTML = `<p class="muted">Đơn #${booking?.id} · ${car ? U.carTitle(car) : ""}. Sau khi xác nhận, booking sẽ completed và xe available.</p>`;
    document.getElementById("btnConfirmCompleteTrip").onclick = () => {
      const ok = ["confirmVehicleReceived", "confirmNoDispute", "confirmReleaseCar"].every((id) => document.getElementById(id).checked);
      if (!ok) return U.renderToast("Vui lòng tick đủ checklist.", "danger");
      if (!canCompleteTrip(booking, car)) return U.renderToast("Chưa đủ điều kiện hoàn tất chuyến.", "danger");
      // Backend must update booking.status and cars.status in one transaction.
      booking.status = "completed";
      car.status = "available";
      window.VivuCarSaveDB();
      U.closeModal("completeTripModal");
      U.renderToast("Đã hoàn tất chuyến đi.", "success");
      if (typeof onDone === "function") onDone();
    };
    U.openModal("completeTripModal");
  }
  window.VivuCarTripCompletion = { canCompleteTrip, openCompleteTripModal };
})();

