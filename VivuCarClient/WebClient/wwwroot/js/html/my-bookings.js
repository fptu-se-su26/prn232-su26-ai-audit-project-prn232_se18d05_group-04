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
    root.innerHTML = `<div style="display:grid;gap:12px">${Array(3).fill('<div class="booking-skeleton"></div>').join('')}</div>`;
    try {
      const r = await authService.apiFetch('bookings/my-bookings?pageSize=100');
      if (r.ok) {
        allBookings = await r.json();
        
        // Fetch payment statuses for pending/approved bookings to determine UI state accurately
        const activeBookings = allBookings.filter(b => String(b.status).toLowerCase() === 'pending' || String(b.status).toLowerCase() === 'approved');
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
      root.innerHTML = `<div class="booking-error">Lỗi tải dữ liệu. Vui lòng tải lại trang.</div>`;
    }
  }

  function resolveUiState(booking) {
    const payment = paymentStatuses[booking.id];
    const paid = payment?.paymentStatus === 'success';
    
    const ui = window.VivuCarUtils.getBookingApiUiState(booking, paid);
    let { key, label, tone } = ui;
    const s = String(booking.status || "").toLowerCase();
    return { key, label, tone };
  }

  function applyFilters(items) {
    const keyword = state.keyword.trim().toLowerCase();
    let output = items.map(booking => {
      const ui = resolveUiState(booking);
      return { booking, ui };
    }).filter(({ booking, ui }) => {
      const haystack = `${booking.bookingCode || booking.id} ${booking.carName} ${booking.licensePlate}`.toLowerCase();
      
      let filterOk = false;
      if (state.filter === "all") {
        filterOk = true;
      } else if (state.filter === "renting") {
        filterOk = ui.key === "renting" || ui.key === "return_requested";
      } else {
        filterOk = ui.key === state.filter || booking.status === state.filter;
      }

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

    // Stats summary
    const total = allBookings.length;
    const active = allBookings.filter(b => {
      const s = String(b.status).toLowerCase();
      return s === 'pending' || s === 'approved' || s === 'inprogress' || s === 'waitingpickup';
    }).length;
    const completed = allBookings.filter(b => String(b.status).toLowerCase() === 'completed').length;

    root.innerHTML = `
      <div style="display:flex;gap:12px;margin-bottom:20px;flex-wrap:wrap">
        <div style="display:flex;align-items:center;gap:16px;flex-wrap:wrap;flex:1">
          <span style="font-size:13px;font-weight:600;color:#a1a1aa;background:#f4f4f5;padding:4px 14px;border-radius:999px">${total} đơn</span>
          <span style="font-size:13px;font-weight:600;color:#16a34a;background:#f0fdf4;padding:4px 14px;border-radius:999px">${active} đang xử lý</span>
          <span style="font-size:13px;font-weight:600;color:#065f46;background:#ecfdf5;padding:4px 14px;border-radius:999px">${completed} hoàn tất</span>
        </div>
        <div style="display:flex;align-items:center;gap:8px">
          <input id="bookingSearch" placeholder="Tìm mã đơn, xe..." value="${state.keyword}"
                 style="border:1px solid #e6e4df;border-radius:10px;padding:8px 12px;font-size:.8125rem;outline:none;width:200px;transition:border-color .2s;background:#fff" onfocus="this.style.borderColor='#16a34a'" onblur="this.style.borderColor='#e6e4df'">
          <select id="bookingSort" style="border:1px solid #e6e4df;border-radius:10px;padding:8px 12px;font-size:.8125rem;outline:none;background:#fff;color:#18181b;-webkit-appearance:none;appearance:none;cursor:pointer;transition:border-color .2s" onfocus="this.style.borderColor='#16a34a'" onblur="this.style.borderColor='#e6e4df'">
            <option value="newest" ${state.sort === "newest" ? "selected" : ""}>Mới nhất</option>
            <option value="oldest" ${state.sort === "oldest" ? "selected" : ""}>Cũ nhất</option>
            <option value="pickup" ${state.sort === "pickup" ? "selected" : ""}>Ngày nhận</option>
            <option value="amount" ${state.sort === "amount" ? "selected" : ""}>Giá trị</option>
          </select>
        </div>
      </div>
      <div class="chip-row" id="bookingTabs" style="margin-bottom:16px">
        ${[
          ["all", "Tất cả"],
          ["payment_pending", "Chờ xử lý"],
          ["handover_pending", "Đã xác nhận"],
          ["renting", "Đang thuê"],
          ["completed", "Hoàn tất"],
          ["cancelled", "Đã hủy"]
        ].map(([key, label]) => `<button class="chip ${state.filter === key ? "active" : ""}" data-filter="${key}" type="button">${label}${key !== 'all' ? ` (${allBookings.filter(b => {const ui=resolveUiState(b);if(key==='renting')return ui.key==='renting'||ui.key==='return_requested';return ui.key===key||b.status===key;}).length})` : ''}</button>`).join("")}
      </div>
      <div class="booking-list">
        ${rows.length ? rows.map(card).join("") : U.renderEmptyState({ title: "Chưa có đơn phù hợp", text: "Thử đổi bộ lọc hoặc tìm xe để tạo booking mới.", href: "/cars/search", action: "Tìm xe" })}
      </div>`;
    bind();
  }

  function card({ booking, ui }) {
    const payment = paymentStatuses[booking.id];
    const paid = payment?.paymentStatus === "success";
    const s = String(booking.status).toLowerCase();
    const canCancel = (s === "pending" || s === "pendingapproval" || s === "pendinggplx" || s === "waitingdeposit") && !paid;
    const dateStr = new Date(booking.startDateTime).toLocaleString('vi-VN', {dateStyle:'medium'}) + ' - ' + new Date(booking.endDateTime).toLocaleString('vi-VN', {dateStyle:'medium'});
    const days = booking.rentalDays || 0;
    const hours = booking.rentalHours || 0;

    return `
      <article class="booking-card-item" style="display:grid;grid-template-columns:150px 1fr;gap:16px;align-items:start;border-radius:18px;border:1px solid #e6e4df;background:#fff;padding:16px;transition:transform .2s,box-shadow .2s;margin-bottom:12px" onmouseover="this.style.boxShadow='0 4px 16px rgba(0,0,0,.06)'" onmouseout="this.style.boxShadow='none'">
        <div style="overflow:hidden;border-radius:12px;aspect-ratio:16/10;width:100%"><img src="${booking.carImageUrl || '/img/placeholder-car.png'}" alt="${booking.carName}" style="width:100%;height:100%;object-fit:cover;display:block;transition:transform .4s ease" onmouseover="this.style.transform='scale(1.06)'" onmouseout="this.style.transform='scale(1)'"></div>
        <div>
          <div style="display:flex;align-items:center;gap:8px;margin-bottom:6px;flex-wrap:wrap">
            <span style="font-size:11px;font-weight:600;color:#a1a1aa">#${booking.bookingCode || booking.id}</span>
            ${U.renderStatusBadge(ui.tone, ui.label)}
          </div>
          <h3 style="font-size:1rem;font-weight:700;color:#18181b;margin:0 0 6px;line-height:1.3">${booking.carName}</h3>
          <p style="font-size:.8125rem;color:#71717a;margin:0 0 8px;line-height:1.4">${dateStr} · ${days} ngày${hours ? ` ${hours} giờ` : ''}</p>
          <div style="display:flex;align-items:center;justify-content:space-between;flex-wrap:wrap;gap:8px">
            <span style="font-size:1.1rem;font-weight:800;color:#18181b">${U.formatVnd(booking.totalAmount)}</span>
            <div style="display:flex;gap:6px;flex-wrap:wrap">
              <a class="btn btn-secondary btn-sm" href="/Booking/Detail?id=${booking.id}">Chi tiết</a>
              ${booking.canPayDeposit && !paid ? `<a class="btn btn-primary btn-sm" href="/Payment/Deposit?bookingId=${booking.id}">Thanh toán</a>` : ""}
              ${ui.key === "handover_pending" ? `<a class="btn btn-primary btn-sm" href="/Booking/Contract?id=${booking.id}">Ký hợp đồng</a>` : ""}
              ${ui.key === "renting" ? `<a class="btn btn-primary btn-sm" href="/Booking/Detail?id=${booking.id}">Trả xe</a>` : ""}
              ${canCancel ? `<button class="btn btn-danger btn-sm" type="button" data-cancel="${booking.id}">Hủy</button>` : ""}
              ${String(booking.status).toLowerCase() === "completed" ? `<a class="btn btn-ghost btn-sm" href="/Review/Create?bookingId=${booking.id}">Đánh giá</a>` : ""}
            </div>
          </div>
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
