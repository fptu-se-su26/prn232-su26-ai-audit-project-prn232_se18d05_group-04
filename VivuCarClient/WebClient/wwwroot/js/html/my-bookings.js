/**
 * my-bookings.js
 * Uses real API instead of window.VivuCarDB
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
  
  const root = U.byId("myBookingsRoot");
  const state = { filter: "all", keyword: "", sort: "newest" };
  
  let allBookings = [];
  let paymentStatuses = {};

  async function loadData() {
    root.innerHTML = `<div style="text-align:center;padding:48px;color:#6b7280">Đang tải danh sách đơn thuê...</div>`;
    try {
      const r = await authService.apiFetch('bookings/my-bookings?pageSize=100');
      if (r.ok) {
        allBookings = await r.json();
        
        // Fetch payment statuses for pending/approved bookings to determine UI state accurately
        const activeBookings = allBookings.filter(b => b.status === 'pending' || b.status === 'approved');
        await Promise.all(activeBookings.map(async (b) => {
          try {
            const pr = await authService.apiFetch(`payments/status/${b.id}`);
            if (pr.ok) {
              paymentStatuses[b.id] = await pr.json();
            }
          } catch (e) {
            console.warn("Could not fetch payment status for", b.id);
          }
        }));
      }
      render();
    } catch (err) {
      console.error(err);
      root.innerHTML = `<div style="text-align:center;padding:48px;color:#dc2626">Lỗi tải dữ liệu. Vui lòng tải lại trang.</div>`;
    }
  }

  function resolveUiState(booking) {
    const payment = paymentStatuses[booking.id];
    const paid = payment?.paymentStatus === 'success';
    
    let key = "unknown", label = "Không xác định", tone = "neutral";
    
    if (booking.status === "cancelled") { key = "cancelled"; label = "Đã hủy"; tone = "danger"; }
    else if (booking.status === "rejected") { key = "rejected"; label = "Đã từ chối"; tone = "danger"; }
    else if (booking.status === "completed") { key = "completed"; label = "Hoàn tất"; tone = "success"; }
    else if (booking.status === "pending") {
      if (paid) { key = "handover_pending"; label = "Đã cọc - chờ xác nhận"; tone = "primary"; }
      else { key = "payment_pending"; label = "Chờ thanh toán"; tone = "warning"; }
    }
    else if (booking.status === "approved") {
      // Missing inspection check for simplicity, default to renting/wait handover
      const pickupDate = new Date(booking.startDateTime);
      if (Date.now() < pickupDate.getTime()) { key = "handover_pending"; label = "Chờ bàn giao"; tone = "primary"; }
      else { key = "renting"; label = "Đang thuê"; tone = "primary"; }
    }
    return { key, label, tone };
  }

  function applyFilters(items) {
    const keyword = state.keyword.trim().toLowerCase();
    let output = items.map(booking => {
      const ui = resolveUiState(booking);
      return { booking, ui };
    }).filter(({ booking, ui }) => {
      const haystack = `${booking.bookingCode || booking.id} ${booking.carName} ${booking.licensePlate}`.toLowerCase();
      const filterOk = state.filter === "all" || ui.key === state.filter || booking.status === state.filter;
      return filterOk && (!keyword || haystack.includes(keyword));
    });
    
    output.sort((a, b) => {
      if (state.sort === "oldest") return new Date(a.booking.createdAt) - new Date(b.booking.createdAt);
      if (state.sort === "pickup") return new Date(a.booking.startDateTime) - new Date(b.booking.startDateTime);
      if (state.sort === "amount") return b.booking.totalAmount - a.booking.totalAmount;
      return new Date(b.booking.createdAt) - new Date(a.booking.createdAt); // newest
    });
    return output;
  }

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Trang chủ", href: "/cars" },
      { label: "Đơn thuê" }
    ]);
    
    const rows = applyFilters(allBookings);
    
    root.innerHTML = `
      <div class="booking-toolbar">
        <div class="chip-row" id="bookingTabs">
          ${[
            ["all", "Tất cả"],
            ["payment_pending", "Chờ xử lý"],
            ["handover_pending", "Đã xác nhận"],
            ["renting", "Đang thuê/chờ trả"],
            ["completed", "Hoàn tất"],
            ["cancelled", "Đã hủy"]
          ].map(([key, label]) => `<button class="chip ${state.filter === key ? "active" : ""}" data-filter="${key}" type="button">${label}</button>`).join("")}
        </div>
        <div class="form-grid max-w-[520px]">
          <input id="bookingSearch" placeholder="Tìm theo mã đơn, xe, biển số" value="${state.keyword}">
          <select id="bookingSort">
            <option value="newest" ${state.sort === "newest" ? "selected" : ""}>Mới nhất</option>
            <option value="oldest" ${state.sort === "oldest" ? "selected" : ""}>Cũ nhất</option>
            <option value="pickup" ${state.sort === "pickup" ? "selected" : ""}>Ngày nhận gần nhất</option>
            <option value="amount" ${state.sort === "amount" ? "selected" : ""}>Giá trị cao nhất</option>
          </select>
        </div>
      </div>
      <div class="booking-list">
        ${rows.length ? rows.map(card).join("") : U.renderEmptyState({ title: "Chưa có đơn phù hợp", text: "Thử đổi bộ lọc hoặc tìm xe để tạo booking mới.", href: "/cars/search", action: "Tìm xe" })}
      </div>`;
    bind();
  }

  function card({ booking, ui }) {
    const payment = paymentStatuses[booking.id];
    const paid = payment?.paymentStatus === "success";
    const canCancel = booking.status === "pending" || booking.status === "approved";
    
    return `
      <article class="booking-card-wide">
        <img src="${booking.carImageUrl || '/img/placeholder-car.png'}" alt="${booking.carName}" style="object-fit:cover">
        <div>
          <div class="car-meta"><span>#${booking.bookingCode || booking.id}</span>${U.renderStatusBadge(ui.tone, ui.label)}${U.renderStatusBadge("neutral", booking.status)}</div>
          <h2>${booking.carName}</h2>
          <p class="muted">${new Date(booking.startDateTime).toLocaleString('vi-VN')} - ${new Date(booking.endDateTime).toLocaleString('vi-VN')} · ${booking.licensePlate}</p>
          <strong>${U.formatVnd(booking.totalAmount)}</strong>
        </div>
        <div class="booking-actions">
          <a class="btn btn-secondary btn-sm" href="/Booking/Detail?id=${booking.id}">Chi tiết</a>
          ${booking.status === "pending" && !paid ? `<a class="btn btn-primary btn-sm" href="/Payment/Deposit?bookingId=${booking.id}">Thanh toán</a>` : ""}
          ${canCancel ? `<button class="btn btn-danger btn-sm" type="button" data-cancel="${booking.id}">Hủy đơn</button>` : ""}
          ${booking.status === "completed" ? `<a class="btn btn-ghost btn-sm" href="/Review/Create?bookingId=${booking.id}">Đánh giá</a>` : ""}
        </div>
      </article>`;
  }

  function bind() {
    document.querySelectorAll("[data-filter]").forEach((button) => button.addEventListener("click", () => {
      state.filter = button.dataset.filter;
      render();
    }));
    U.byId("bookingSearch")?.addEventListener("input", (event) => {
      state.keyword = event.target.value;
      render();
    });
    U.byId("bookingSort")?.addEventListener("change", (event) => {
      state.sort = event.target.value;
      render();
    });
    document.querySelectorAll("[data-cancel]").forEach((button) => button.addEventListener("click", () => {
      if(window.VivuCarBookingCancellation) {
        window.VivuCarBookingCancellation.openCancelBookingModal(button.dataset.cancel, loadData);
      } else {
        alert("Chức năng hủy đang được cập nhật.");
      }
    }));
  }

  loadData();
})();
