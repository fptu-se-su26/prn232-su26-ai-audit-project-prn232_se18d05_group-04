(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const state = { keyword: "", status: "", discount_type: "", from: "", to: "" };
  let voucherChartInstance = null;

  function usageCount(voucherId) {
    return DB.voucher_usages.filter((usage) => usage.voucher_id === voucherId).length;
  }

  function resolveVoucherStatus(voucher) {
    if (voucher.expires_at && new Date(voucher.expires_at) < new Date("2026-05-19T00:00:00+07:00")) return "expired";
    if (usageCount(voucher.id) >= voucher.quantity) return "used_up";
    return "active";
  }

  function filteredVouchers() {
    return DB.vouchers.filter((voucher) => {
      const keyword = `${voucher.code} ${voucher.name}`.toLowerCase();
      const status = resolveVoucherStatus(voucher);
      return (!state.keyword || keyword.includes(state.keyword.toLowerCase()))
        && (!state.status || status === state.status)
        && (!state.discount_type || voucher.discount_type === state.discount_type)
        && (!state.from || !voucher.expires_at || voucher.expires_at.slice(0, 10) >= state.from)
        && (!state.to || !voucher.expires_at || voucher.expires_at.slice(0, 10) <= state.to);
    });
  }

  function renderSummary() {
    const rows = [
      ["Tổng voucher", DB.vouchers.length],
      ["Đang hoạt động", DB.vouchers.filter((v) => resolveVoucherStatus(v) === "active").length],
      ["Đã hết hạn", DB.vouchers.filter((v) => resolveVoucherStatus(v) === "expired").length],
      ["Đã dùng hết lượt", DB.vouchers.filter((v) => resolveVoucherStatus(v) === "used_up").length],
      ["Tổng lượt sử dụng", DB.voucher_usages.length]
    ];
    document.getElementById("voucherSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderVouchers() {
    const vouchers = filteredVouchers();
    renderSummary();
    document.getElementById("voucherEmpty").classList.toggle("hidden", vouchers.length > 0);
    document.getElementById("voucherTableBody").innerHTML = vouchers.map((voucher) => {
      const used = usageCount(voucher.id);
      const percent = voucher.quantity ? Math.min(100, Math.round(used / voucher.quantity * 100)) : 100;
      const status = resolveVoucherStatus(voucher);
      const statusLabel = { active: "Đang hoạt động", expired: "Đã hết hạn", used_up: "Đã dùng hết lượt" }[status];
      const kind = status === "active" ? "success" : status === "used_up" ? "warning" : "neutral";
      return `<tr><td class="mono">${voucher.code}</td><td>${voucher.name}</td><td>${C.DISCOUNT_TYPE_LABELS[voucher.discount_type]}</td><td>${voucher.discount_type === "percentage" ? `${voucher.discount_value}%` : U.formatVnd(voucher.discount_value)}</td><td>${U.formatVnd(voucher.min_order_amount)}</td><td>${U.formatDate(voucher.expires_at)}</td><td>${used} / ${voucher.quantity}<div class="progress"><span style="width:${percent}%"></span></div></td><td>${U.statusBadge(kind, status, statusLabel)}</td><td class="actions"><button class="btn btn-secondary btn-sm btn-view-performance" data-id="${voucher.id}">Xem hiệu suất</button><a class="btn btn-secondary btn-sm" href="admin-voucher-form.html?id=${voucher.id}">Sửa</a>${used === 0 ? `<button class="btn btn-danger btn-sm btn-delete-voucher" data-id="${voucher.id}">Xóa</button>` : ""}</td></tr>`;
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

  document.getElementById("btnFilterVoucher").addEventListener("click", applyFilter);
  document.getElementById("btnResetVoucherFilter").addEventListener("click", () => { document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = ""); applyFilter(); });
  document.getElementById("btnClosePerformance").addEventListener("click", closeDrawer);
  document.getElementById("drawerOverlay").addEventListener("click", closeDrawer);
  document.addEventListener("click", (event) => {
    const performance = event.target.closest(".btn-view-performance");
    const del = event.target.closest(".btn-delete-voucher");
    if (performance) openPerformance(Number(performance.dataset.id));
    if (del) {
      const id = Number(del.dataset.id);
      DB.vouchers = DB.vouchers.filter((voucher) => voucher.id !== id);
      U.showToast("Đã xóa voucher chưa có lượt dùng.");
      renderVouchers();
    }
  });
  renderVouchers();
})();

