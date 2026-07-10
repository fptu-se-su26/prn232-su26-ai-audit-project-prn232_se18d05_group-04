(function () {
  const DB = window.VivuCarDB,
    U = window.VivuCarUtils,
    Auth = window.VivuCarAuth,
    C = window.VivuCarConstants;
  const owner = Auth.getCurrentUser();
  if (!owner) return;
  const carId = Number(new URLSearchParams(location.search).get("carId"));
  const car = DB.cars.find((x) => x.id === carId && x.owner_id === owner.id);
  const root = U.byId("ownerCarStatusRoot");
  function upcoming() {
    return DB.bookings.filter(
      (b) =>
        b.car_id === carId &&
        b.status === "approved" &&
        new Date(b.pickup_datetime) > new Date(),
    );
  }
  function render() {
    if (!car)
      return (root.innerHTML = U.renderEmptyState({
        title: "Không tìm thấy xe",
        href: "owner-cars.html",
        action: "Về danh sách",
      }));
    root.innerHTML = `<section class="owner-hero"><div><span class="eyebrow">Car status</span><h1>Trạng thái ${U.carTitle(car)}</h1><p class="muted">${car.license_plate} · ${car.address}</p></div>${U.renderStatusBadge("neutral", car.status)}</section>${upcoming().length ? `<p class="return-note">Xe đang có ${upcoming().length} booking approved trong tương lai. Thay đổi trạng thái có thể ảnh hưởng khách hàng.</p>` : ""}<div class="owner-form-layout"><form class="checkout-section" id="statusForm"><h2>Cập nhật trạng thái</h2><div class="payment-method-grid">${["available", "maintenance", "blocked"].map((s) => `<label class="method-card"><input type="radio" name="status" value="${s}" ${car.status === s ? "checked" : ""}><strong>${C.CAR_STATUS_LABELS[s]}</strong><span class="muted">${s === "blocked" ? "Tạm ngưng cho thuê theo owner request" : s === "maintenance" ? "Xe đang bảo trì" : "Sẵn sàng cho thuê"}</span></label>`).join("")}</div><div id="reasonBox" class="form-grid mt-3.5"><label class="full">Lý do / ghi chú<input id="blocked_reason" value="${car.blocked_reason || ""}"></label><label>Ngày bắt đầu<input id="maintenance_start_date" type="date"></label><label>Ngày mở lại dự kiến<input id="maintenance_end_date" type="date"></label></div><!-- UI-only field. Not present in current DB schema. Requires migration before backend integration. --><p class="muted">maintenance_start_date/end_date không có trong bảng cars.</p><button class="btn btn-primary btn-full" type="submit">Cập nhật trạng thái xe</button></form><aside class="summary-card"><h2>History</h2><div class="timeline">${
      (DB.car_status_history || [])
        .filter((h) => h.car_id === car.id)
        .slice(-6)
        .reverse()
        .map(
          (h) =>
            `<div class="timeline-item"><strong>${h.old_status} → ${h.new_status}</strong><p>${U.formatDateTime(h.created_at)} · ${h.reason || ""}</p></div>`,
        )
        .join("") || "<p class='muted'>Chưa có lịch sử trạng thái.</p>"
    }</div></aside></div>`;
    U.byId("statusForm").onsubmit = submit;
  }
  function submit(e) {
    e.preventDefault();
    const next = document.querySelector("input[name='status']:checked").value;
    const reason = U.byId("blocked_reason").value.trim();
    if (next === car.status)
      return U.renderToast(
        "Trạng thái mới trùng trạng thái hiện tại.",
        "danger",
      );
    if (["maintenance", "blocked"].includes(next) && !reason)
      return U.renderToast("Vui lòng nhập lý do.", "danger");
    if (car.status === "rented" && next !== "maintenance")
      return U.renderToast(
        "Không cho chuyển xe đang rented thủ công nếu chưa có xử lý booking.",
        "danger",
      );
    DB.car_status_history.push({
      id: Date.now(),
      car_id: car.id,
      old_status: car.status,
      new_status: next,
      reason,
      updated_by: owner.id,
      created_at: new Date().toISOString(),
    });
    car.status = next;
    car.blocked_reason = ["maintenance", "blocked"].includes(next)
      ? reason
      : null;
    window.VivuCarSaveDB();
    U.renderToast("Cập nhật trạng thái xe thành công.", "success");
    render();
  }
  render();
})();

