(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;

  const STATUS_COLORS = {
    paid: "#059669",
    cancelled: "#fbbf24",
    pending: "#38bdf8"
  };
  const RANGE_PRESETS = [
    { key: "YESTERDAY", label: "Hôm qua", suffix: "hôm qua" },
    { key: "TODAY", label: "Hôm nay", suffix: "hôm nay" },
    { key: "THIS_WEEK", label: "Tuần này", suffix: "tuần này" },
    { key: "THIS_MONTH", label: "Tháng này", suffix: "tháng này" },
    { key: "LAST_MONTH", label: "Tháng trước", suffix: "tháng trước" },
    { key: "THIS_YEAR", label: "Năm nay", suffix: "năm nay" },
    { key: "LAST_YEAR", label: "Năm trước", suffix: "năm trước" },
    { key: "CUSTOM", label: "Tùy chỉnh", suffix: "tùy chỉnh" }
  ];
  const state = {
    selectedRange: "THIS_MONTH",
    customStartDate: "",
    customEndDate: ""
  };

  function formatVnd(value) {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
      currencyDisplay: "code",
      maximumFractionDigits: 0
    }).format(Number(value || 0)).replace(/\s/g, " ");
  }

  function toDateInputValue(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  }

  function startOfDay(date) {
    const value = new Date(date);
    value.setHours(0, 0, 0, 0);
    return value;
  }

  function endOfDay(date) {
    const value = new Date(date);
    value.setHours(23, 59, 59, 999);
    return value;
  }

  function addDays(date, days) {
    const value = new Date(date);
    value.setDate(value.getDate() + days);
    return value;
  }

  function getDateRangeByPreset(preset) {
    const today = startOfDay(new Date());
    const currentYear = today.getFullYear();
    const currentMonth = today.getMonth();

    if (preset === "YESTERDAY") {
      const yesterday = addDays(today, -1);
      return { start: startOfDay(yesterday), end: endOfDay(yesterday) };
    }
    if (preset === "TODAY") return { start: startOfDay(today), end: endOfDay(today) };
    if (preset === "THIS_WEEK") {
      const day = today.getDay() || 7;
      const start = addDays(today, 1 - day);
      return { start: startOfDay(start), end: endOfDay(today) };
    }
    if (preset === "LAST_MONTH") {
      const start = new Date(currentYear, currentMonth - 1, 1);
      const end = new Date(currentYear, currentMonth, 0);
      return { start: startOfDay(start), end: endOfDay(end) };
    }
    if (preset === "THIS_YEAR") {
      return { start: startOfDay(new Date(currentYear, 0, 1)), end: endOfDay(today) };
    }
    if (preset === "LAST_YEAR") {
      return { start: startOfDay(new Date(currentYear - 1, 0, 1)), end: endOfDay(new Date(currentYear - 1, 11, 31)) };
    }
    if (preset === "CUSTOM") {
      const fallback = getDateRangeByPreset("THIS_MONTH");
      const rawStart = state.customStartDate ? new Date(`${state.customStartDate}T00:00:00`) : fallback.start;
      const rawEnd = state.customEndDate ? new Date(`${state.customEndDate}T00:00:00`) : fallback.end;
      const first = rawStart <= rawEnd ? rawStart : rawEnd;
      const last = rawStart <= rawEnd ? rawEnd : rawStart;
      return { start: startOfDay(first), end: endOfDay(last) };
    }
    return { start: startOfDay(new Date(currentYear, currentMonth, 1)), end: endOfDay(today) };
  }

  function isInRange(value, range) {
    if (!value) return false;
    const date = new Date(value);
    return date >= range.start && date <= range.end;
  }

  function daysBetween(start, end) {
    return Math.round((end - start) / 86400000) + 1;
  }

  function findUser(id) {
    return DB.users.find((user) => user.id === id);
  }

  function findCar(id) {
    return DB.cars.find((car) => car.id === id);
  }

  function paymentForBooking(bookingId) {
    return DB.payments.find((payment) => payment.booking_id === bookingId);
  }

  function successfulPaymentForBooking(bookingId) {
    return DB.payments.find((payment) => payment.booking_id === bookingId && payment.status === "success");
  }

  function getBookingStatusGroup(booking) {
    if (booking.status === "cancelled") return "cancelled";
    if (successfulPaymentForBooking(booking.id)) return "paid";
    return "pending";
  }

  function statusLabel(group) {
    return {
      paid: "Đã thanh toán",
      cancelled: "Hủy",
      pending: "Chờ thanh toán"
    }[group] || "Không xác định";
  }

  function statusBadge(group) {
    const tone = group === "paid" ? "success" : group === "cancelled" ? "warning" : "info";
    return U.statusBadge(tone, group, statusLabel(group));
  }

  function formatDateShort(value) {
    return new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" }).format(new Date(value));
  }

  function rangeSuffix() {
    return RANGE_PRESETS.find((item) => item.key === state.selectedRange)?.suffix || "khoảng đã chọn";
  }

  function buildChartBuckets(range, paidPayments) {
    const useDailyBuckets = daysBetween(range.start, range.end) <= 62;
    if (useDailyBuckets) {
      const buckets = [];
      for (let date = startOfDay(range.start); date <= range.end; date = addDays(date, 1)) {
        const key = toDateInputValue(date);
        buckets.push({ key, label: new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit" }).format(date), revenue: 0, orders: 0 });
      }
      paidPayments.forEach((payment) => {
        const key = toDateInputValue(new Date(payment.paid_at));
        const bucket = buckets.find((item) => item.key === key);
        if (!bucket) return;
        bucket.revenue += Number(payment.amount || 0);
        bucket.orders += 1;
      });
      return { mode: "day", items: buckets };
    }

    const buckets = [];
    for (let year = range.start.getFullYear(); year <= range.end.getFullYear(); year += 1) {
      const startMonth = year === range.start.getFullYear() ? range.start.getMonth() : 0;
      const endMonth = year === range.end.getFullYear() ? range.end.getMonth() : 11;
      for (let month = startMonth; month <= endMonth; month += 1) {
        buckets.push({ key: `${year}-${String(month + 1).padStart(2, "0")}`, label: `Th${String(month + 1).padStart(2, "0")}`, revenue: 0, orders: 0 });
      }
    }
    paidPayments.forEach((payment) => {
      const date = new Date(payment.paid_at);
      const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}`;
      const bucket = buckets.find((item) => item.key === key);
      if (!bucket) return;
      bucket.revenue += Number(payment.amount || 0);
      bucket.orders += 1;
    });
    return { mode: "month", items: buckets };
  }

  function calculateDashboardData() {
    const range = getDateRangeByPreset(state.selectedRange);
    const filteredBookings = DB.bookings.filter((booking) => isInRange(booking.created_at, range));
    const paidPayments = DB.payments.filter((payment) => payment.status === "success" && isInRange(payment.paid_at, range));
    const successfulRevenue = paidPayments.reduce((sum, payment) => sum + Number(payment.amount || 0), 0);
    const paidBookingIds = new Set(paidPayments.map((payment) => payment.booking_id));
    const paidCount = filteredBookings.filter((booking) => paidBookingIds.has(booking.id) || successfulPaymentForBooking(booking.id) && isInRange(successfulPaymentForBooking(booking.id).paid_at, range)).length;
    const cancelledCount = filteredBookings.filter((booking) => booking.status === "cancelled").length;
    const pendingCount = filteredBookings.filter((booking) => getBookingStatusGroup(booking) === "pending").length;
    const orderDetails = filteredBookings
      .map((booking) => ({ booking, group: getBookingStatusGroup(booking), payment: paymentForBooking(booking.id) }))
      .filter((item) => ["paid", "cancelled", "pending"].includes(item.group))
      .sort((a, b) => new Date(b.booking.created_at) - new Date(a.booking.created_at));

    return {
      range,
      successfulRevenue,
      successfulOrderCount: paidPayments.length,
      orderStatusStats: [
        { key: "paid", label: "Đã thanh toán", value: paidCount, color: STATUS_COLORS.paid },
        { key: "cancelled", label: "Hủy", value: cancelledCount, color: STATUS_COLORS.cancelled },
        { key: "pending", label: "Chờ thanh toán", value: pendingCount, color: STATUS_COLORS.pending }
      ],
      chartData: buildChartBuckets(range, paidPayments),
      totalOrders: filteredBookings.length,
      orderDetails
    };
  }

  function renderRangeButtons() {
    const root = document.getElementById("revenueRangeButtons");
    root.innerHTML = RANGE_PRESETS.map((preset) => {
      const active = state.selectedRange === preset.key;
      const icon = preset.key === "CUSTOM" ? '<span class="ml-1 text-xs">▾</span>' : "";
      const activeClass = "border-emerald-600 bg-emerald-600 text-white hover:bg-emerald-700";
      const inactiveClass = "border-zinc-300 bg-white text-zinc-700 hover:bg-zinc-100";
      return `<button class="rounded-lg border px-4 py-2 text-sm font-medium transition-all duration-200 ease-out ${active ? activeClass : inactiveClass}" type="button" data-range-preset="${preset.key}">${preset.label}${icon}</button>`;
    }).join("");
    document.getElementById("customRangePanel").classList.toggle("hidden", state.selectedRange !== "CUSTOM");
  }

  function renderSummary(data) {
    const suffix = rangeSuffix();
    document.getElementById("revenueCardRangeTitle").textContent = `Tổng doanh thu ${suffix}`;
    document.getElementById("completedOrdersTitle").textContent = `Tổng đơn hoàn thành ${suffix}`;
    document.getElementById("successfulRevenue").textContent = formatVnd(data.successfulRevenue);
    document.getElementById("successfulOrderCount").textContent = `${data.successfulOrderCount} đơn đã thanh toán`;
    document.getElementById("totalOrders").textContent = data.totalOrders;
    document.getElementById("revenueRangeBadge").textContent = RANGE_PRESETS.find((item) => item.key === state.selectedRange)?.label || "Tùy chỉnh";
  }

  function renderDonut(data) {
    const donut = document.getElementById("orderStatusDonut");
    const legend = document.getElementById("orderStatusLegend");
    let current = 0;
    const segments = data.orderStatusStats.map((item) => {
      const start = data.totalOrders ? current / data.totalOrders * 100 : 0;
      current += item.value;
      const end = data.totalOrders ? current / data.totalOrders * 100 : 0;
      return `${item.color} ${start}% ${end}%`;
    });
    donut.style.background = data.totalOrders
      ? `conic-gradient(${segments.join(", ")})`
      : "conic-gradient(#e4e4e7 0% 100%)";
    legend.innerHTML = data.orderStatusStats.map((item) => `
      <span class="inline-flex items-center gap-2">
        <span class="h-2.5 w-2.5 rounded-full" style="background:${item.color}"></span>
        <span>${item.label}</span>
        <strong class="font-semibold text-zinc-900">${item.value}</strong>
      </span>
    `).join("");
  }

  function chartScale(items) {
    const maxRevenue = Math.max(...items.map((item) => item.revenue), 0);
    const scaleMax = Math.max(1000000, Math.ceil(maxRevenue / 200000) * 200000);
    return {
      scaleMax,
      ticks: Array.from({ length: 6 }, (_, index) => Math.round(scaleMax - scaleMax / 5 * index))
    };
  }

  function renderRevenueChart(data) {
    const root = document.getElementById("monthlyRevenueChart");
    const chart = data.chartData;
    const { scaleMax, ticks } = chartScale(chart.items);
    document.getElementById("revenueChartTitle").textContent = chart.mode === "day" ? "Doanh thu theo ngày" : "Doanh thu theo tháng";

    root.innerHTML = `
      <div class="grid min-h-[380px] grid-cols-[86px_minmax(0,1fr)] gap-4">
        <div class="grid h-[320px] grid-rows-6 text-right text-xs text-zinc-500">
          ${ticks.map((tick) => `<div class="flex items-center justify-end">${formatVnd(tick)}</div>`).join("")}
        </div>
        <div class="relative h-[320px] overflow-x-auto border-l border-zinc-200">
          <div class="relative h-full min-w-[720px]">
            ${ticks.map((_, index) => `<div class="absolute left-0 right-0 border-t border-zinc-200" style="top:${index * 20}%"></div>`).join("")}
            <div class="relative z-10 grid h-full items-end gap-3 px-3" style="grid-template-columns:repeat(${Math.max(chart.items.length, 1)}, minmax(34px, 1fr))">
              ${chart.items.map((item) => {
      const height = Math.min(100, item.revenue / scaleMax * 100);
      const visualHeight = item.revenue > 0 ? Math.max(3, height) : 0;
      return `
                  <div class="group grid h-full grid-rows-[1fr_auto] gap-3">
                    <div class="relative flex items-end justify-center">
                      <div class="w-full max-w-10 rounded-t-xl bg-emerald-600 transition-all duration-200 ease-out group-hover:bg-emerald-700" style="height:${visualHeight}%"></div>
                      <div class="pointer-events-none absolute left-1/2 hidden -translate-x-1/2 whitespace-nowrap rounded-lg border border-zinc-200 bg-white px-2 py-1 text-xs font-medium text-zinc-700 shadow-sm group-hover:block" style="bottom:calc(${visualHeight}% + 8px)">${formatVnd(item.revenue)} · ${item.orders} đơn</div>
                    </div>
                    <span class="text-center text-xs font-medium text-zinc-500">${item.label}</span>
                  </div>
                `;
    }).join("")}
            </div>
          </div>
        </div>
      </div>
    `;
  }

  function renderOrderDetails(data) {
    const root = document.getElementById("orderDetailTable");
    document.getElementById("orderDetailCount").textContent = `${data.orderDetails.length} đơn`;
    if (!data.orderDetails.length) {
      root.innerHTML = U.renderEmptyState({
        title: "Chưa có đơn hàng trong khoảng thời gian này",
        text: "Chọn một mốc thời gian khác để xem danh sách chi tiết."
      });
      return;
    }

    root.innerHTML = `
      <div class="overflow-x-auto rounded-2xl border border-zinc-200 bg-white shadow-sm">
        <table class="min-w-full text-left text-sm">
          <thead class="bg-zinc-50 text-xs font-semibold uppercase tracking-wide text-zinc-500">
            <tr>
              <th class="px-5 py-3">Mã đơn</th>
              <th class="px-5 py-3">Khách hàng</th>
              <th class="px-5 py-3">Phương tiện</th>
              <th class="px-5 py-3">Ngày thuê</th>
              <th class="px-5 py-3 text-right">Tổng tiền</th>
              <th class="px-5 py-3">Trạng thái</th>
              <th class="px-5 py-3">Thanh toán</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-zinc-100">
            ${data.orderDetails.map(({ booking, group, payment }) => {
      const user = findUser(booking.user_id);
      const car = findCar(booking.car_id);
      return `
                <tr class="transition hover:bg-zinc-50">
                  <td class="px-5 py-4 font-semibold text-zinc-900">#${booking.id}</td>
                  <td class="px-5 py-4 text-zinc-700">${user?.full_name || "Không xác định"}</td>
                  <td class="px-5 py-4 text-zinc-700">${car ? `${car.brand} ${car.model}` : "Không xác định"}</td>
                  <td class="px-5 py-4 text-zinc-600">${formatDateShort(booking.pickup_datetime)}</td>
                  <td class="px-5 py-4 text-right font-semibold text-zinc-900">${formatVnd(booking.total_amount)}</td>
                  <td class="px-5 py-4">${statusBadge(group)}</td>
                  <td class="px-5 py-4 text-zinc-600">${payment ? U.renderStatusBadge("payment", payment.status) : U.statusBadge("warning", "pending", "Chờ thanh toán")}</td>
                </tr>
              `;
    }).join("")}
          </tbody>
        </table>
      </div>
    `;
  }

  function renderDashboard() {
    renderRangeButtons();
    const data = calculateDashboardData();
    renderSummary(data);
    renderDonut(data);
    renderRevenueChart(data);
    renderOrderDetails(data);
  }

  function bindEvents() {
    document.getElementById("revenueRangeButtons").addEventListener("click", (event) => {
      const button = event.target.closest("[data-range-preset]");
      if (!button) return;
      state.selectedRange = button.dataset.rangePreset;
      if (state.selectedRange === "CUSTOM" && (!state.customStartDate || !state.customEndDate)) {
        const range = getDateRangeByPreset("THIS_MONTH");
        state.customStartDate = toDateInputValue(range.start);
        state.customEndDate = toDateInputValue(range.end);
        document.getElementById("customStartDate").value = state.customStartDate;
        document.getElementById("customEndDate").value = state.customEndDate;
      }
      renderDashboard();
    });

    document.getElementById("applyCustomRange").addEventListener("click", () => {
      state.selectedRange = "CUSTOM";
      state.customStartDate = document.getElementById("customStartDate").value;
      state.customEndDate = document.getElementById("customEndDate").value;
      renderDashboard();
    });
  }

  window.VivuCarAdminDashboard = {
    state,
    getDateRangeByPreset,
    get successfulRevenue() {
      return calculateDashboardData().successfulRevenue;
    },
    get orderStatusStats() {
      return calculateDashboardData().orderStatusStats;
    },
    get monthlyRevenue() {
      return calculateDashboardData().chartData.items;
    },
    get totalOrders() {
      return calculateDashboardData().totalOrders;
    },
    renderDashboard
  };

  bindEvents();
  renderDashboard();
})();

