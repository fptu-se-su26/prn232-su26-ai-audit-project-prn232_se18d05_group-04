(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const state = { keyword: "", status: "", discount_type: "", from: "", to: "", tab: "all" };
  let voucherChartInstance = null;
  let forceDeleteVoucherId = null;

  function usageCount(voucherId) {
    return DB.voucher_usages.filter((usage) => usage.voucher_id === voucherId).length;
  }

  function activeVouchers() {
    return DB.vouchers.filter((voucher) => !voucher.deleted_at);
  }

  function trashVouchers() {
    return DB.vouchers.filter((voucher) => voucher.deleted_at);
  }

  function ensureTrashUi() {
    if (!document.getElementById("voucherForceDeleteModal")) {
      document.body.insertAdjacentHTML("beforeend", `
        <div class="modal-backdrop" id="voucherForceDeleteModal">
          <div class="modal">
            <div class="modal-header"><h2>Xóa vĩnh viễn voucher</h2><button class="icon-button" type="button" data-close-modal>×</button></div>
            <p class="muted">Hành động này không thể hoàn tác. Voucher sẽ bị xóa khỏi dữ liệu mock hiện tại.</p>
            <div class="modal-actions">
              <button class="btn btn-secondary" id="btnCancelForceDeleteVoucher" type="button">Hủy</button>
              <button class="btn btn-danger" id="btnConfirmForceDeleteVoucher" type="button">Xóa vĩnh viễn</button>
            </div>
          </div>
        </div>
      `);
    }
  }

  function resolveVoucherStatus(voucher) {
    if (voucher.expires_at && new Date(voucher.expires_at) < new Date("2026-05-19T00:00:00+07:00")) return "expired";
    if (usageCount(voucher.id) >= voucher.quantity) return "used_up";
    return "active";
  }

  function tabCount(key) {
    if (key === "trash") return trashVouchers().length;
    if (key === "active") return activeVouchers().filter((voucher) => resolveVoucherStatus(voucher) === "active").length;
    if (key === "hidden") return activeVouchers().filter((voucher) => ["expired", "used_up"].includes(resolveVoucherStatus(voucher))).length;
    return activeVouchers().length;
  }

  function renderTabs() {
    return tabCount("trash");
  }

  function filteredVouchers() {
    const source = state.tab === "trash" ? trashVouchers() : activeVouchers();
    return source.filter((voucher) => {
      const keyword = `${voucher.code} ${voucher.name}`.toLowerCase();
      const status = resolveVoucherStatus(voucher);
      const tabMatch = state.tab === "all" || state.tab === "trash"
        || (state.tab === "active" && status === "active")
        || (state.tab === "hidden" && ["expired", "used_up"].includes(status));
      return tabMatch
        && (!state.keyword || keyword.includes(state.keyword.toLowerCase()))
        && (!state.status || status === state.status)
        && (!state.discount_type || voucher.discount_type === state.discount_type)
        && (!state.from || !voucher.expires_at || voucher.expires_at.slice(0, 10) >= state.from)
        && (!state.to || !voucher.expires_at || voucher.expires_at.slice(0, 10) <= state.to);
    });
  }

  function renderSummary() {
    const vouchers = activeVouchers();
    const rows = [
      ["Tổng voucher", vouchers.length],
      ["Đang hoạt động", vouchers.filter((v) => resolveVoucherStatus(v) === "active").length],
      ["Đã hết hạn", vouchers.filter((v) => resolveVoucherStatus(v) === "expired").length],
      ["Đã dùng hết lượt", vouchers.filter((v) => resolveVoucherStatus(v) === "used_up").length],
      ["Tổng lượt sử dụng", DB.voucher_usages.length]
    ];
    document.getElementById("voucherSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderVouchers() {
    renderTabs();
    const vouchers = filteredVouchers();
    renderSummary();
    const empty = document.getElementById("voucherEmpty");
    empty.classList.toggle("hidden", vouchers.length > 0);
    empty.querySelector("h3").textContent = state.tab === "trash" ? "Thùng rác trống" : "Chưa có voucher phù hợp";
    empty.querySelector("p").textContent = state.tab === "trash"
      ? "Các đối tượng bị xoá mềm sẽ xuất hiện tại đây."
      : "Thử đổi bộ lọc hoặc tạo một voucher mới cho chiến dịch tiếp theo.";
    const emptyAction = empty.querySelector("a");
    if (emptyAction) emptyAction.classList.toggle("hidden", state.tab === "trash");
    document.querySelector(".table-wrap").classList.toggle("hidden", vouchers.length === 0);
    document.getElementById("voucherTableBody").innerHTML = vouchers.map((voucher) => {
      const used = usageCount(voucher.id);
      const percent = voucher.quantity ? Math.min(100, Math.round(used / voucher.quantity * 100)) : 100;
      const status = resolveVoucherStatus(voucher);
      const statusLabel = { active: "Đang hoạt động", expired: "Đã hết hạn", used_up: "Đã dùng hết lượt" }[status];
      const kind = status === "active" ? "success" : status === "used_up" ? "warning" : "neutral";
      const actions = state.tab === "trash"
        ? U.renderActionMenu([
          { label: "Khôi phục", attrs: { "data-restore-voucher": voucher.id } },
          { label: "Xóa vĩnh viễn", attrs: { "data-force-delete-voucher": voucher.id }, variant: "danger" }
        ])
        : U.renderActionMenu([
          { label: "Xem chi tiết", attrs: { "data-view-performance": voucher.id } },
          { label: "Chỉnh sửa", href: `admin-voucher-form.html?id=${voucher.id}` },
          { label: status === "active" ? "Tạm dừng" : "Kích hoạt", attrs: { "data-toggle-voucher": voucher.id } },
          { label: "Xóa mềm", attrs: { "data-soft-delete-voucher": voucher.id }, variant: "danger" }
        ]);
      return `<tr><td class="mono">${voucher.code}</td><td>${voucher.name}</td><td>${C.DISCOUNT_TYPE_LABELS[voucher.discount_type]}</td><td>${voucher.discount_type === "percentage" ? `${voucher.discount_value}%` : U.formatVnd(voucher.discount_value)}</td><td>${U.formatVnd(voucher.min_order_amount)}</td><td>${U.formatDate(voucher.expires_at)}</td><td>${used} / ${voucher.quantity}<div class="progress"><span style="width:${percent}%"></span></div></td><td>${U.statusBadge(kind, status, statusLabel)}</td><td class="actions">${actions}</td></tr>`;
    }).join("");
  }

  function applyFilter() {
    state.keyword = document.getElementById("searchVoucherInput").value.trim();
    state.status = document.getElementById("voucherStatusFilter").value;
    state.discount_type = document.getElementById("discountTypeFilter").value;
    state.from = document.getElementById("voucherDateFrom").value;
    state.to = document.getElementById("voucherDateTo").value;
    renderVouchers();
  }

  function softDeleteItem(id) {
    const voucher = DB.vouchers.find((item) => item.id === Number(id));
    if (!voucher) return;
    voucher.deleted_at = new Date().toISOString();
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast("Đã chuyển voucher vào thùng rác.");
    renderVouchers();
  }

  function restoreItem(id) {
    const voucher = DB.vouchers.find((item) => item.id === Number(id));
    if (!voucher) return;
    voucher.deleted_at = null;
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast("Đã khôi phục voucher.");
    renderVouchers();
  }

  function forceDeleteItem(id) {
    const voucherId = Number(id);
    DB.vouchers = DB.vouchers.filter((voucher) => voucher.id !== voucherId);
    DB.voucher_usages = DB.voucher_usages.filter((usage) => usage.voucher_id !== voucherId);
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.closeModal("voucherForceDeleteModal");
    U.showToast("Đã xóa vĩnh viễn voucher.");
    renderVouchers();
  }

  function openPerformance(id) {
    const voucher = DB.vouchers.find((item) => item.id === id);
    const usages = DB.voucher_usages.filter((item) => item.voucher_id === id);
    const relatedBookings = usages.map((usage) => DB.bookings.find((booking) => booking.id === usage.booking_id)).filter(Boolean);
    const revenue = relatedBookings.reduce((sum, booking) => sum + Number(booking.total_amount), 0);
    const discountTotal = usages.reduce((sum) => sum + Number(voucher.discount_type === "fixed" ? voucher.discount_value : Math.min(voucher.max_discount, 90000)), 0);
    const usageRate = voucher.quantity ? Math.round(usages.length / voucher.quantity * 100) : 100;
    document.getElementById("performanceContent").innerHTML = `
      <div class="summary-card"><span>Mã voucher</span><strong>${voucher.code}</strong></div>
      <div class="summary-card"><span>Tổng lượt sử dụng</span><strong>${usages.length}</strong></div>
      <div class="summary-card"><span>Doanh thu từ đơn có voucher</span><strong>${U.formatVnd(revenue)}</strong></div>
      <div class="summary-card"><span>Tổng tiền đã giảm</span><strong>${U.formatVnd(discountTotal)}</strong></div>
      <div class="summary-card"><span>Tỷ lệ sử dụng</span><strong>${usageRate}%</strong><div class="progress"><span style="width:${usageRate}%"></span></div></div>
      <div class="panel mt-3"><div class="panel-head"><h2>Lượt dùng theo ngày</h2></div><div class="chart-frame min-h-[210px]"><canvas id="voucherUsageChartCanvas" aria-label="Biểu đồ lượt dùng voucher"></canvas><div id="voucherUsageChart" class="mini-chart hidden">${[12, 35, 22, 48, 31, 60, 42].map((height) => `<span style="height:${height}%"></span>`).join("")}</div></div></div>
      <div class="table-wrap mt-3"><table><thead><tr><th>Khách hàng</th><th>Mã đơn</th><th>Ngày dùng</th><th>Giá trị đơn</th><th>Số tiền giảm</th></tr></thead><tbody>${usages.map((usage) => { const user = DB.users.find((item) => item.id === usage.user_id); const booking = DB.bookings.find((item) => item.id === usage.booking_id); return `<tr><td>${user?.full_name || ""}</td><td>${usage.booking_id}</td><td>${U.formatDateTime(usage.used_at)}</td><td>${U.formatVnd(booking?.total_amount || 0)}</td><td>${U.formatVnd(voucher.discount_type === "fixed" ? voucher.discount_value : Math.min(voucher.max_discount, 90000))}</td></tr>`; }).join("")}</tbody></table></div>`;
    window.VivuCarTailwindUI?.openDrawer(
      document.getElementById("voucherPerformanceDrawer"),
      document.getElementById("drawerOverlay")
    );
    renderVoucherUsageChart(usages);
  }

  function renderVoucherUsageChart(usages) {
    const canvas = document.getElementById("voucherUsageChartCanvas");
    const fallback = document.getElementById("voucherUsageChart");
    const labels = ["13/05", "14/05", "15/05", "16/05", "17/05", "18/05", "19/05"];
    const values = labels.map((_, index) => Math.max(0, usages.length - Math.abs(3 - index)));
    if (window.Chart && canvas) {
      fallback.classList.add("hidden");
      canvas.classList.remove("hidden");
      if (voucherChartInstance) voucherChartInstance.destroy();
      voucherChartInstance = new Chart(canvas, {
        type: "line",
        data: {
          labels,
          datasets: [{
            label: "Lượt dùng",
            data: values,
            borderColor: "#059669",
            backgroundColor: "rgba(5,150,105,.10)",
            fill: true,
            tension: .35,
            pointRadius: 3
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            x: { grid: { display: false }, ticks: { color: "#71717a" } },
            y: { beginAtZero: true, ticks: { precision: 0, color: "#71717a" }, grid: { color: "#e4e4e7" } }
          }
        }
      });
      return;
    }
    if (canvas) canvas.classList.add("hidden");
    fallback.classList.remove("hidden");
  }

  function closeDrawer() {
    window.VivuCarTailwindUI?.closeDrawer(
      document.getElementById("voucherPerformanceDrawer"),
      document.getElementById("drawerOverlay")
    );
  }

  ensureTrashUi();
  document.getElementById("btnFilterVoucher").addEventListener("click", applyFilter);
  document.getElementById("btnResetVoucherFilter").addEventListener("click", () => { document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = ""); applyFilter(); });
  document.getElementById("btnClosePerformance").addEventListener("click", closeDrawer);
  document.getElementById("drawerOverlay").addEventListener("click", closeDrawer);
  document.getElementById("btnCancelForceDeleteVoucher").addEventListener("click", () => U.closeModal("voucherForceDeleteModal"));
  document.getElementById("btnConfirmForceDeleteVoucher").addEventListener("click", () => forceDeleteItem(forceDeleteVoucherId));
  document.addEventListener("click", (event) => {
    const performance = event.target.closest("[data-view-performance]");
    const toggle = event.target.closest("[data-toggle-voucher]");
    const softDelete = event.target.closest("[data-soft-delete-voucher]");
    const restore = event.target.closest("[data-restore-voucher]");
    const forceDelete = event.target.closest("[data-force-delete-voucher]");
    if (performance) return openPerformance(Number(performance.dataset.viewPerformance));
    if (toggle) return U.showToast("Trạng thái kích hoạt voucher là UI-only trong schema hiện tại.");
    if (softDelete) return softDeleteItem(softDelete.dataset.softDeleteVoucher);
    if (restore) return restoreItem(restore.dataset.restoreVoucher);
    if (forceDelete) {
      forceDeleteVoucherId = Number(forceDelete.dataset.forceDeleteVoucher);
      return U.openModal("voucherForceDeleteModal");
    }
  });
  renderVouchers();
})();
