import { fetchJson } from "../shared/api-client.js";
import { escapeHtml } from "../shared/dom.js";
import { formatVnd } from "../shared/utils.js";

const root = document.querySelector("[data-admin-dashboard-page]");
const byId = (id) => document.getElementById(id);
const presets = [
    ["YESTERDAY", "Hôm qua"],
    ["TODAY", "Hôm nay"],
    ["THIS_WEEK", "Tuần này"],
    ["THIS_MONTH", "Tháng này"],
    ["LAST_MONTH", "Tháng trước"],
    ["THIS_YEAR", "Năm nay"],
    ["LAST_YEAR", "Năm trước"],
    ["CUSTOM", "Tùy chỉnh"]
];
const state = {
    preset: "THIS_MONTH",
    from: "",
    to: "",
    page: 1,
    pageSize: 5
};

function iso(date) {
    const offset = date.getTimezoneOffset();
    return new Date(date.getTime() - offset * 60000).toISOString().slice(0, 10);
}

function rangeFor(key) {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    let start = new Date(today);
    let end = new Date(today);

    if (key === "YESTERDAY") {
        start.setDate(start.getDate() - 1);
        end = new Date(start);
    } else if (key === "THIS_WEEK") {
        const day = start.getDay() || 7;
        start.setDate(start.getDate() - day + 1);
    } else if (key === "THIS_MONTH") {
        start = new Date(today.getFullYear(), today.getMonth(), 1);
    } else if (key === "LAST_MONTH") {
        start = new Date(today.getFullYear(), today.getMonth() - 1, 1);
        end = new Date(today.getFullYear(), today.getMonth(), 0);
    } else if (key === "THIS_YEAR") {
        start = new Date(today.getFullYear(), 0, 1);
    } else if (key === "LAST_YEAR") {
        start = new Date(today.getFullYear() - 1, 0, 1);
        end = new Date(today.getFullYear() - 1, 11, 31);
    }

    return { from: iso(start), to: iso(end) };
}

function rangeLabel() {
    return (presets.find((item) => item[0] === state.preset) || ["", "Tùy chỉnh"])[1];
}

function renderPresets() {
    byId("revenueRangeButtons").innerHTML = presets.map((item) => {
        const active = item[0] === state.preset;
        const classes = active
            ? "border-emerald-600 bg-emerald-600 text-white hover:bg-emerald-700"
            : "border-zinc-300 bg-white text-zinc-700 hover:bg-zinc-100";
        const suffix = item[0] === "CUSTOM" ? '<span class="ml-1 text-xs">▾</span>' : "";
        return `<button class="rounded-lg border px-4 py-2 text-sm font-medium transition-all duration-200 ease-out ${classes}" type="button" data-range-preset="${item[0]}">${item[1]}${suffix}</button>`;
    }).join("");
    byId("customRangePanel").classList.toggle("hidden", state.preset !== "CUSTOM");
}

function badge(value, type) {
    const key = String(value || "pending").toLowerCase();
    const labels = type === "payment"
        ? { pending: "Chờ thanh toán", success: "Thành công", failed: "Thất bại", refunded: "Đã hoàn tiền" }
        : { pending: "Chờ xử lý", approved: "Đã xác nhận", rejected: "Đã từ chối", completed: "Hoàn tất", cancelled: "Đã hủy" };
    const tone = ["success", "completed", "approved"].includes(key)
        ? "success"
        : ["failed", "rejected"].includes(key)
            ? "danger"
            : ["cancelled", "refunded"].includes(key) ? "warning" : "info";
    return `<span class="status-badge status-${tone}">${escapeHtml(labels[key] || key)}</span>`;
}

function renderSummary(data) {
    const summary = data.summary || {};
    const pending = Math.max(
        0,
        (summary.totalBookings || 0) - (summary.completedBookings || 0) - (summary.cancelledBookings || 0)
    );

    byId("revenueCardRangeTitle").textContent = `Tổng doanh thu ${rangeLabel().toLowerCase()}`;
    byId("completedOrdersTitle").textContent = `Tổng đơn hoàn thành ${rangeLabel().toLowerCase()}`;
    byId("successfulRevenue").textContent = formatVnd(summary.grossRevenue || 0);
    byId("successfulOrderCount").textContent = `${summary.completedBookings || 0} đơn đã thanh toán`;
    byId("averageOrderValue").textContent = formatVnd(summary.averageOrderValue || 0);
    byId("netRevenue").textContent = formatVnd(summary.netRevenue || 0);
    byId("totalOrders").textContent = summary.totalBookings || 0;
    byId("revenueRangeBadge").textContent = rangeLabel();

    const stats = [
        ["#059669", "Đã thanh toán", summary.completedBookings || 0],
        ["#fbbf24", "Hủy", summary.cancelledBookings || 0],
        ["#38bdf8", "Chờ thanh toán", pending]
    ];
    let current = 0;
    const segments = stats.map((item) => {
        const start = summary.totalBookings ? current / summary.totalBookings * 100 : 0;
        current += item[2];
        const end = summary.totalBookings ? current / summary.totalBookings * 100 : 0;
        return `${item[0]} ${start}% ${end}%`;
    });
    byId("orderStatusDonut").style.background = summary.totalBookings
        ? `conic-gradient(${segments.join(",")})`
        : "conic-gradient(#e4e4e7 0 100%)";
    byId("orderStatusLegend").innerHTML = stats.map((item) =>
        `<span class="inline-flex items-center gap-2"><span class="h-2.5 w-2.5 rounded-full" style="background:${item[0]}"></span><span>${item[1]}</span><strong class="font-semibold text-zinc-900">${item[2]}</strong></span>`
    ).join("");
}

const DAY_IN_MILLISECONDS = 86_400_000;

function parseDate(value) {
    return new Date(`${value}T00:00:00`);
}

function dateKey(date) {
    return iso(date);
}

function addDays(date, numberOfDays) {
    const result = new Date(date);
    result.setDate(result.getDate() + numberOfDays);
    return result;
}

function inclusiveDayCount(from, to) {
    return Math.round((to.getTime() - from.getTime()) / DAY_IN_MILLISECONDS) + 1;
}

function resolveChartGrouping() {
    const from = parseDate(state.from);
    const to = parseDate(state.to);
    const numberOfDays = inclusiveDayCount(from, to);

    if (["THIS_YEAR", "LAST_YEAR"].includes(state.preset) || numberOfDays > 180) return "month";
    if (["THIS_MONTH", "LAST_MONTH"].includes(state.preset) || numberOfDays > 14) return "week";
    return "day";
}

function buildRevenueMap(items) {
    const revenueByDate = new Map();
    items.forEach((item) => {
        const key = String(item.date).slice(0, 10);
        revenueByDate.set(key, (revenueByDate.get(key) || 0) + Number(item.grossRevenue || 0));
    });
    return revenueByDate;
}

function sumRevenue(from, to, revenueByDate) {
    let total = 0;
    for (let cursor = new Date(from); cursor <= to; cursor = addDays(cursor, 1)) {
        total += revenueByDate.get(dateKey(cursor)) || 0;
    }
    return total;
}

function buildChartBuckets(items) {
    const grouping = resolveChartGrouping();
    const selectedStart = parseDate(state.from);
    const selectedEnd = parseDate(state.to);
    const isYearPreset = ["THIS_YEAR", "LAST_YEAR"].includes(state.preset);
    const rangeStart = isYearPreset
        ? new Date(selectedStart.getFullYear(), 0, 1)
        : selectedStart;
    const rangeEnd = isYearPreset
        ? new Date(selectedStart.getFullYear(), 11, 31)
        : selectedEnd;
    const revenueByDate = buildRevenueMap(items);
    const dayMonthFormatter = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit" });
    const fullDateFormatter = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
    const buckets = [];

    if (grouping === "month") {
        let cursor = new Date(rangeStart.getFullYear(), rangeStart.getMonth(), 1);
        while (cursor <= rangeEnd) {
            const bucketStart = cursor < rangeStart ? rangeStart : new Date(cursor);
            const lastDayOfMonth = new Date(cursor.getFullYear(), cursor.getMonth() + 1, 0);
            const bucketEnd = lastDayOfMonth > rangeEnd ? rangeEnd : lastDayOfMonth;
            const month = cursor.getMonth() + 1;
            buckets.push({
                label: `T${month}`,
                tooltipLabel: `Tháng ${month}/${cursor.getFullYear()}`,
                grossRevenue: sumRevenue(bucketStart, bucketEnd, revenueByDate)
            });
            cursor = new Date(cursor.getFullYear(), cursor.getMonth() + 1, 1);
        }
    } else if (grouping === "week") {
        let cursor = new Date(rangeStart);
        let weekNumber = 1;
        while (cursor <= rangeEnd) {
            const bucketEndCandidate = addDays(cursor, 6);
            const bucketEnd = bucketEndCandidate > rangeEnd ? rangeEnd : bucketEndCandidate;
            buckets.push({
                label: `Tuần ${weekNumber}`,
                tooltipLabel: `${fullDateFormatter.format(cursor)} – ${fullDateFormatter.format(bucketEnd)}`,
                grossRevenue: sumRevenue(cursor, bucketEnd, revenueByDate)
            });
            cursor = addDays(bucketEnd, 1);
            weekNumber++;
        }
    } else {
        for (let cursor = new Date(rangeStart); cursor <= rangeEnd; cursor = addDays(cursor, 1)) {
            buckets.push({
                label: dayMonthFormatter.format(cursor).replace("/", "-"),
                tooltipLabel: `Ngày ${fullDateFormatter.format(cursor)}`,
                grossRevenue: revenueByDate.get(dateKey(cursor)) || 0
            });
        }
    }

    return { grouping, buckets };
}

function niceScaleMaximum(value) {
    if (value <= 0) return 1_000_000;
    const magnitude = 10 ** Math.floor(Math.log10(value));
    const normalized = value / magnitude;
    const multiplier = normalized <= 1 ? 1 : normalized <= 2 ? 2 : normalized <= 5 ? 5 : 10;
    return multiplier * magnitude;
}

function renderChart(items) {
    const { grouping, buckets } = buildChartBuckets(items);
    const groupingMeta = {
        day: {
            title: "Doanh thu theo ngày",
            badge: "Theo ngày",
            description: `Hiển thị đủ ${buckets.length} ngày trong khoảng lọc, kể cả ngày không phát sinh doanh thu.`
        },
        week: {
            title: "Doanh thu theo tuần",
            badge: "Theo tuần",
            description: `Doanh thu được cộng theo ${buckets.length} tuần để biểu đồ dễ theo dõi.`
        },
        month: {
            title: "Doanh thu theo tháng",
            badge: "Theo tháng",
            description: `Doanh thu được cộng theo ${buckets.length} tháng; tháng không phát sinh được hiển thị 0 đ.`
        }
    }[grouping];

    byId("revenueChartTitle").textContent = groupingMeta.title;
    byId("chartGranularityBadge").textContent = groupingMeta.badge;
    byId("revenueChartDescription").textContent = groupingMeta.description;

    const maximumRevenue = Math.max(0, ...buckets.map((item) => item.grossRevenue));
    const scaleMaximum = niceScaleMaximum(maximumRevenue);
    const ticks = Array.from({ length: 6 }, (_, index) => Math.round(scaleMaximum - scaleMaximum / 5 * index));
    const denseDailyChart = grouping === "day" && buckets.length > 20;
    const columnWidth = denseDailyChart ? 28 : grouping === "week" ? 52 : 44;
    const columnGap = denseDailyChart ? 12 : grouping === "month" ? 24 : 20;
    const plotWidth = Math.max(520, buckets.length * (columnWidth + columnGap) + 40);

    const bars = buckets.map((item) => {
        const height = Math.min(100, item.grossRevenue / scaleMaximum * 100);
        const hasRevenue = item.grossRevenue > 0;
        const barClass = hasRevenue ? "bg-emerald-600 hover:bg-emerald-700" : "bg-zinc-200";
        const barStyle = hasRevenue ? `height:${Math.max(3, height)}%` : "height:2px";
        const accessibleLabel = `${item.tooltipLabel}: ${formatVnd(item.grossRevenue)}`;
        return `<div class="group grid h-full grid-rows-[1fr_auto] gap-3" role="img" aria-label="${escapeHtml(accessibleLabel)}">
            <div class="relative flex items-end justify-center">
                <span class="pointer-events-none absolute bottom-full z-20 mb-2 hidden min-w-max rounded-lg border border-zinc-200 bg-white px-3 py-2 text-center text-xs text-zinc-600 shadow-sm group-hover:block">
                    <span class="block font-medium">${escapeHtml(item.tooltipLabel)}</span>
                    <strong class="mt-0.5 block font-semibold text-zinc-900">${escapeHtml(formatVnd(item.grossRevenue))}</strong>
                </span>
                <div class="w-full rounded-t-lg transition-colors ${barClass}" style="${barStyle}"></div>
            </div>
            <span class="truncate text-center text-xs font-medium text-zinc-500" title="${escapeHtml(item.tooltipLabel)}">${escapeHtml(item.label)}</span>
        </div>`;
    }).join("");

    byId("monthlyRevenueChart").innerHTML = `<div class="grid min-h-[340px] grid-cols-[86px_minmax(0,1fr)] gap-4 max-sm:grid-cols-[72px_minmax(0,1fr)] max-sm:gap-2">
        <div class="grid h-[300px] grid-rows-6 text-right text-xs text-zinc-500">${ticks.map((tick) => `<div class="flex items-center justify-end">${escapeHtml(formatVnd(tick))}</div>`).join("")}</div>
        <div class="relative h-[340px] overflow-x-auto overflow-y-hidden border-l border-zinc-200 pb-2">
            <div class="relative h-[300px] min-w-full" style="width:${plotWidth}px">
                ${ticks.map((_, index) => `<div class="absolute left-0 right-0 border-t border-zinc-200" style="top:${index * 20}%"></div>`).join("")}
                <div class="relative z-10 grid h-full items-end justify-center px-5" style="grid-template-columns:repeat(${buckets.length},${columnWidth}px);column-gap:${columnGap}px">${bars}</div>
            </div>
        </div>
    </div>`;
}

function paginationItems(current, total) {
    if (total <= 7) return Array.from({ length: total }, (_, index) => index + 1);
    const pages = new Set([1, total, current - 1, current, current + 1]);
    const valid = [...pages].filter((page) => page >= 1 && page <= total).sort((a, b) => a - b);
    const result = [];
    valid.forEach((page, index) => {
        if (index > 0 && page - valid[index - 1] > 1) result.push("ellipsis");
        result.push(page);
    });
    return result;
}

function renderPagination(pagination) {
    const nav = byId("orderPagination");
    const totalItems = pagination.totalItems || 0;
    const totalPages = Math.max(1, pagination.totalPages || 1);
    const page = Math.min(Math.max(1, pagination.page || 1), totalPages);
    const pageSize = pagination.pageSize || state.pageSize;
    const start = totalItems ? (page - 1) * pageSize + 1 : 0;
    const end = Math.min(page * pageSize, totalItems);

    nav.innerHTML = `<p class="m-0 text-sm text-zinc-500">Hiển thị <strong class="font-semibold text-zinc-800">${start}–${end}</strong> trong ${totalItems} đơn</p>
        <div class="flex items-center gap-1.5">
            <button class="rounded-lg border border-zinc-200 bg-white px-3 py-2 text-sm font-medium text-zinc-700 transition hover:bg-zinc-50 disabled:cursor-not-allowed disabled:opacity-40" type="button" data-order-page="${page - 1}" ${page <= 1 ? "disabled" : ""} aria-label="Trang trước">Trước</button>
            ${paginationItems(page, totalPages).map((item) => item === "ellipsis"
                ? '<span class="px-1 text-zinc-400">…</span>'
                : `<button class="h-9 min-w-9 rounded-lg border px-2 text-sm font-semibold transition ${item === page ? "border-emerald-600 bg-emerald-600 text-white" : "border-zinc-200 bg-white text-zinc-700 hover:bg-zinc-50"}" type="button" data-order-page="${item}" ${item === page ? 'aria-current="page"' : ""}>${item}</button>`
            ).join("")}
            <button class="rounded-lg border border-zinc-200 bg-white px-3 py-2 text-sm font-medium text-zinc-700 transition hover:bg-zinc-50 disabled:cursor-not-allowed disabled:opacity-40" type="button" data-order-page="${page + 1}" ${page >= totalPages ? "disabled" : ""} aria-label="Trang sau">Sau</button>
        </div>`;
}

function renderOrders(items, pagination) {
    byId("orderDetailCount").textContent = `${pagination.totalItems || 0} đơn`;
    if (!items.length) {
        byId("orderDetailTable").innerHTML = '<div class="empty-state"><h3>Chưa có đơn hàng trong khoảng thời gian này</h3><p>Chọn một mốc thời gian khác để xem danh sách chi tiết.</p></div>';
        renderPagination(pagination);
        return;
    }

    const dateFormatter = new Intl.DateTimeFormat("vi-VN");
    byId("orderDetailTable").innerHTML = `<div class="overflow-x-auto rounded-2xl border border-zinc-200 bg-white shadow-sm"><table class="min-w-full text-left text-sm">
        <thead class="bg-zinc-50 text-xs font-semibold uppercase tracking-wide text-zinc-500"><tr><th class="px-5 py-3">Mã đơn</th><th class="px-5 py-3">Khách hàng</th><th class="px-5 py-3">Phương tiện</th><th class="px-5 py-3">Ngày thuê</th><th class="px-5 py-3 text-right">Tổng tiền</th><th class="px-5 py-3">Trạng thái</th><th class="px-5 py-3">Thanh toán</th></tr></thead>
        <tbody class="divide-y divide-zinc-100">${items.map((item) => `<tr class="transition hover:bg-zinc-50"><td class="px-5 py-4 font-semibold">#${escapeHtml(item.bookingCode || item.id)}</td><td class="px-5 py-4">${escapeHtml(item.customerName)}</td><td class="px-5 py-4">${escapeHtml(item.carName)}</td><td class="px-5 py-4">${escapeHtml(dateFormatter.format(new Date(item.pickupDate)))}</td><td class="px-5 py-4 text-right font-semibold">${escapeHtml(formatVnd(item.totalAmount))}</td><td class="px-5 py-4">${badge(item.bookingStatus, "booking")}</td><td class="px-5 py-4">${badge(item.paymentStatus, "payment")}</td></tr>`).join("")}</tbody>
    </table></div>`;
    renderPagination(pagination);
}

async function load() {
    byId("monthlyRevenueChart").innerHTML = '<div class="skeleton h-[340px] rounded-xl"></div>';
    byId("orderDetailTable").setAttribute("aria-busy", "true");
    try {
        const query = new URLSearchParams({
            from: state.from,
            to: state.to,
            page: String(state.page),
            pageSize: String(state.pageSize)
        });
        const data = await fetchJson(`api/admin/reports/revenue?${query}`);
        state.page = data.pagination?.page || 1;
        renderSummary(data);
        renderChart(data.daily || []);
        renderOrders(data.recentBookings || [], data.pagination || { page: 1, pageSize: state.pageSize, totalItems: 0, totalPages: 1 });
    } catch (error) {
        byId("monthlyRevenueChart").innerHTML = `<div class="empty-state"><h3>Không tải được báo cáo</h3><p>${escapeHtml(error.message)}</p><button class="btn btn-primary" id="retryDashboard" type="button">Thử lại</button></div>`;
        byId("retryDashboard")?.addEventListener("click", load);
        renderSummary({ summary: {} });
        renderOrders([], { page: 1, pageSize: state.pageSize, totalItems: 0, totalPages: 1 });
    } finally {
        byId("orderDetailTable").removeAttribute("aria-busy");
    }
}

function selectPreset(key) {
    state.preset = key;
    state.page = 1;
    if (key !== "CUSTOM") {
        Object.assign(state, rangeFor(key));
    } else {
        if (!state.from) Object.assign(state, rangeFor("THIS_MONTH"));
        byId("customStartDate").value = state.from;
        byId("customEndDate").value = state.to;
    }
    renderPresets();
    if (key !== "CUSTOM") load();
}

if (root) {
    byId("revenueRangeButtons").addEventListener("click", (event) => {
        const button = event.target.closest("[data-range-preset]");
        if (button) selectPreset(button.dataset.rangePreset);
    });
    byId("applyCustomRange").addEventListener("click", () => {
        const from = byId("customStartDate").value;
        const to = byId("customEndDate").value;
        if (!from || !to || from > to) return;
        state.preset = "CUSTOM";
        state.from = from;
        state.to = to;
        state.page = 1;
        renderPresets();
        load();
    });
    byId("orderPagination").addEventListener("click", (event) => {
        const button = event.target.closest("[data-order-page]");
        if (!button || button.disabled) return;
        state.page = Number(button.dataset.orderPage);
        load();
    });

    Object.assign(state, rangeFor("THIS_MONTH"));
    renderPresets();
    load();
}