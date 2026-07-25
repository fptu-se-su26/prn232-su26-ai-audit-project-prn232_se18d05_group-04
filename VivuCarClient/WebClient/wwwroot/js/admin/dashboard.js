import { fetchJson } from "../shared/api-client.js";
import { escapeHtml } from "../shared/dom.js";
import { debounce, formatVnd } from "../shared/utils.js";

const root = document.querySelector("[data-admin-dashboard-page]");
const byId = (id) => document.getElementById(id);

const primaryPresets = [
    { key: "TODAY", label: "Hôm nay" },
    { key: "THIS_WEEK", label: "Tuần này" },
    { key: "THIS_MONTH", label: "Tháng này" }
];
const secondaryPresets = [
    { key: "YESTERDAY", label: "Hôm qua" },
    { key: "LAST_MONTH", label: "Tháng trước" },
    { key: "THIS_YEAR", label: "Năm nay" },
    { key: "LAST_YEAR", label: "Năm trước" },
    { key: "CUSTOM", label: "Tùy chỉnh" }
];
const allPresets = [...primaryPresets, ...secondaryPresets];
const state = {
    preset: "THIS_MONTH",
    from: "",
    to: "",
    page: 1,
    pageSize: 5,
    search: "",
    bookingStatus: "",
    paymentStatus: ""
};
let loadSequence = 0;

function iso(date) {
    const offset = date.getTimezoneOffset();
    return new Date(date.getTime() - offset * 60_000).toISOString().slice(0, 10);
}

function parseDate(value) {
    return new Date(`${value}T00:00:00`);
}

function addDays(date, days) {
    const result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
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
        const weekday = start.getDay() || 7;
        start.setDate(start.getDate() - weekday + 1);
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
    return allPresets.find((item) => item.key === state.preset)?.label || "Tùy chỉnh";
}

function formattedRange() {
    if (!state.from || !state.to) return "";
    const formatter = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
    return `${formatter.format(parseDate(state.from))} – ${formatter.format(parseDate(state.to))}`;
}

function renderRangeControls() {
    byId("revenueRangeButtons").innerHTML = primaryPresets.map((item) => {
        const active = item.key === state.preset;
        return `<button class="revenue-range-button${active ? " is-active" : ""}" type="button" data-range-preset="${item.key}"${active ? ' aria-pressed="true"' : ' aria-pressed="false"'}>${item.label}</button>`;
    }).join("");

    const more = byId("revenueRangeMore");
    more.innerHTML = `<option value="">Khoảng khác</option>${secondaryPresets.map((item) => `<option value="${item.key}">${item.label}</option>`).join("")}`;
    more.value = secondaryPresets.some((item) => item.key === state.preset) ? state.preset : "";
    byId("customRangePanel").hidden = state.preset !== "CUSTOM";
    byId("selectedRangeText").textContent = `${rangeLabel()} · ${formattedRange()}`;
}

function paymentBadge(value) {
    const key = String(value || "pending").toLowerCase();
    const labels = {
        pending: "Chờ thanh toán",
        success: "Đã thanh toán",
        failed: "Thanh toán thất bại",
        refunded: "Đã hoàn tiền"
    };
    const tone = key === "success" ? "success" : key === "failed" ? "danger" : key === "refunded" ? "neutral" : "warning";
    return `<span class="status-badge status-${tone}">${escapeHtml(labels[key] || key)}</span>`;
}

function bookingBadge(value) {
    const key = String(value || "pending").toLowerCase();
    const labels = {
        pending: "Chờ xử lý",
        approved: "Đã xác nhận",
        rejected: "Đã từ chối",
        completed: "Hoàn tất",
        cancelled: "Đã hủy"
    };
    const tone = key === "completed" ? "success" : ["rejected", "cancelled"].includes(key) ? "danger" : key === "approved" ? "info" : "warning";
    return `<span class="status-badge status-${tone}">${escapeHtml(labels[key] || key)}</span>`;
}

function percentage(value, total) {
    return total > 0 ? value * 100 / total : 0;
}

function renderSummary(data) {
    const summary = data.summary || {};
    const total = Number(summary.totalBookings || 0);
    const paid = Number(summary.paidBookings ?? summary.completedBookings ?? 0);
    const cancelled = Number(summary.cancelledWithoutPaymentBookings ?? summary.cancelledBookings ?? 0);
    const pending = Number(summary.pendingPaymentBookings ?? Math.max(0, total - paid - cancelled));
    const paidAverage = paid > 0 ? Number(summary.grossRevenue || 0) / paid : 0;

    byId("successfulRevenue").textContent = formatVnd(summary.grossRevenue || 0);
    byId("successfulOrderCount").textContent = `${paid.toLocaleString("vi-VN")} đơn đã thanh toán`;
    byId("completedOrders").textContent = `${Number(summary.completedBookings || 0).toLocaleString("vi-VN")} đơn`;
    byId("completedOrdersHint").textContent = `${rangeLabel()} · ${formattedRange()}`;
    byId("averageOrderValue").textContent = formatVnd(paidAverage);
    byId("totalOrders").textContent = total.toLocaleString("vi-VN");
    byId("revenueRangeBadge").textContent = rangeLabel();

    const stats = [
        { color: "#059669", label: "Đã thu", count: paid, className: "is-paid" },
        { color: "#94a3b8", label: "Chờ thanh toán", count: pending, className: "is-pending" },
        { color: "#d96c4f", label: "Đã hủy chưa thu", count: cancelled, className: "is-cancelled" }
    ];
    let current = 0;
    const segments = stats.map((item) => {
        const start = percentage(current, total);
        current += item.count;
        const end = percentage(current, total);
        return `${item.color} ${start}% ${end}%`;
    });
    const donut = byId("orderStatusDonut");
    donut.style.background = total > 0 ? `conic-gradient(${segments.join(",")})` : "#e4e4e7";
    donut.setAttribute("aria-label", total > 0
        ? `Tổng ${total} đơn: ${paid} đã thu, ${pending} chờ thanh toán, ${cancelled} đã hủy chưa thu.`
        : "Chưa có đơn hàng trong khoảng thời gian này.");
    byId("orderStatusLegend").innerHTML = stats.map((item) => {
        const ratio = percentage(item.count, total);
        return `<div class="revenue-donut-row${item.count === 0 ? " is-zero" : ""}">
            <i class="revenue-donut-dot ${item.className}"></i>
            <span>${item.label}</span>
            <strong>${item.count.toLocaleString("vi-VN")} · ${ratio.toLocaleString("vi-VN", { maximumFractionDigits: 1 })}%</strong>
        </div>`;
    }).join("");
}

const DAY_IN_MILLISECONDS = 86_400_000;

function inclusiveDayCount(from, to) {
    return Math.round((to.getTime() - from.getTime()) / DAY_IN_MILLISECONDS) + 1;
}

function resolveChartGrouping() {
    const dayCount = inclusiveDayCount(parseDate(state.from), parseDate(state.to));
    if (["THIS_YEAR", "LAST_YEAR"].includes(state.preset) || dayCount > 180) return "month";
    if (["THIS_MONTH", "LAST_MONTH"].includes(state.preset) || dayCount > 14) return "week";
    return "day";
}

function emptyRevenueStatus() {
    return {
        paidAmount: 0,
        pendingPaymentAmount: 0,
        cancelledAmount: 0,
        totalBookings: 0,
        paidBookings: 0,
        pendingPaymentBookings: 0,
        completedBookings: 0,
        cancelledBookings: 0
    };
}

function buildRevenueStatusMap(items) {
    const statusByDate = new Map();
    items.forEach((item) => {
        const key = String(item.date).slice(0, 10);
        const current = statusByDate.get(key) || emptyRevenueStatus();
        current.paidAmount += Number(item.grossRevenue || 0);
        current.pendingPaymentAmount += Number(item.pendingPaymentAmount || 0);
        current.cancelledAmount += Number(item.cancelledAmount || 0);
        current.totalBookings += Number(item.totalBookings || 0);
        current.paidBookings += Number(item.paidBookings || 0);
        current.pendingPaymentBookings += Number(item.pendingPaymentBookings || 0);
        current.completedBookings += Number(item.completedBookings || 0);
        current.cancelledBookings += Number(item.cancelledBookings || 0);
        statusByDate.set(key, current);
    });
    return statusByDate;
}

function dateKey(date) {
    return iso(date);
}

function sumRevenueStatus(from, to, statusByDate) {
    const total = emptyRevenueStatus();
    for (let cursor = new Date(from); cursor <= to; cursor = addDays(cursor, 1)) {
        const item = statusByDate.get(dateKey(cursor)) || emptyRevenueStatus();
        Object.keys(total).forEach((key) => { total[key] += item[key]; });
    }
    return total;
}

function buildChartBuckets(items) {
    const grouping = resolveChartGrouping();
    const selectedStart = parseDate(state.from);
    const selectedEnd = parseDate(state.to);
    const isYearPreset = ["THIS_YEAR", "LAST_YEAR"].includes(state.preset);
    const rangeStart = isYearPreset ? new Date(selectedStart.getFullYear(), 0, 1) : selectedStart;
    const rangeEnd = isYearPreset ? new Date(selectedStart.getFullYear(), 11, 31) : selectedEnd;
    const statusByDate = buildRevenueStatusMap(items);
    const dayMonthFormatter = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit" });
    const fullDateFormatter = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
    const buckets = [];

    if (grouping === "month") {
        for (let cursor = new Date(rangeStart.getFullYear(), rangeStart.getMonth(), 1); cursor <= rangeEnd; cursor = new Date(cursor.getFullYear(), cursor.getMonth() + 1, 1)) {
            const bucketStart = cursor < rangeStart ? rangeStart : new Date(cursor);
            const monthEnd = new Date(cursor.getFullYear(), cursor.getMonth() + 1, 0);
            const bucketEnd = monthEnd > rangeEnd ? rangeEnd : monthEnd;
            const month = cursor.getMonth() + 1;
            buckets.push({ label: `T${month}`, tooltipLabel: `Tháng ${month}/${cursor.getFullYear()}`, ...sumRevenueStatus(bucketStart, bucketEnd, statusByDate) });
        }
    } else if (grouping === "week") {
        let cursor = new Date(rangeStart);
        let week = 1;
        while (cursor <= rangeEnd) {
            const candidate = addDays(cursor, 6);
            const bucketEnd = candidate > rangeEnd ? rangeEnd : candidate;
            buckets.push({ label: `Tuần ${week}`, tooltipLabel: `${fullDateFormatter.format(cursor)} – ${fullDateFormatter.format(bucketEnd)}`, ...sumRevenueStatus(cursor, bucketEnd, statusByDate) });
            cursor = addDays(bucketEnd, 1);
            week += 1;
        }
    } else {
        for (let cursor = new Date(rangeStart); cursor <= rangeEnd; cursor = addDays(cursor, 1)) {
            buckets.push({ label: dayMonthFormatter.format(cursor).replace("/", "-"), tooltipLabel: `Ngày ${fullDateFormatter.format(cursor)}`, ...(statusByDate.get(dateKey(cursor)) || emptyRevenueStatus()) });
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

function formatChartValue(value) {
    const amount = Number(value || 0);
    if (amount >= 1_000_000_000) return `${(amount / 1_000_000_000).toLocaleString("vi-VN", { maximumFractionDigits: 1 })} tỷ`;
    if (amount >= 1_000_000) return `${(amount / 1_000_000).toLocaleString("vi-VN", { maximumFractionDigits: 1 })} tr`;
    if (amount >= 1_000) return `${Math.round(amount / 1_000).toLocaleString("vi-VN")}k`;
    return amount.toLocaleString("vi-VN");
}

function renderHourlyLineChart(items) {
    const hourlyByHour = new Map((items || []).map((item) => [Number(item.hour), item]));
    const hours = Array.from({ length: 24 }, (_, hour) => ({
        hour,
        grossRevenue: Number(hourlyByHour.get(hour)?.grossRevenue || 0),
        paidTransactions: Number(hourlyByHour.get(hour)?.paidTransactions || 0)
    }));
    const maximumValue = Math.max(0, ...hours.map((item) => item.grossRevenue));
    const scaleMaximum = niceScaleMaximum(maximumValue);
    const ticks = Array.from({ length: 6 }, (_, index) => Math.round(scaleMaximum - scaleMaximum / 5 * index));
    const width = 960;
    const height = 286;
    const plot = { left: 64, right: 18, top: 22, bottom: 42 };
    const plotWidth = width - plot.left - plot.right;
    const plotHeight = height - plot.top - plot.bottom;
    const pointFor = (item) => ({
        x: plot.left + item.hour / 23 * plotWidth,
        y: plot.top + (1 - item.grossRevenue / scaleMaximum) * plotHeight
    });
    const points = hours.map(pointFor);
    const selectedDate = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" })
        .format(parseDate(state.from));

    byId("revenueChartTitle").textContent = "Doanh thu đã thu theo giờ";
    byId("chartGranularityBadge").textContent = "Theo giờ";
    byId("revenueChartDescription").textContent = `24 giờ ngày ${selectedDate}. Mỗi điểm thể hiện tiền thanh toán thành công trong một giờ.`;
    byId("revenueChartLegend").innerHTML = '<span><i class="is-paid"></i>Doanh thu đã thu</span>';

    const highest = hours.reduce((best, item) => item.grossRevenue > best.grossRevenue ? item : best, hours[0]);
    byId("chartTextSummary").textContent = maximumValue > 0
        ? `Doanh thu theo giờ ngày ${selectedDate}. Khung ${String(highest.hour).padStart(2, "0")}:00 đạt cao nhất với ${formatVnd(highest.grossRevenue)}.`
        : `Chưa có thanh toán thành công trong ngày ${selectedDate}.`;

    const grid = ticks.map((tick, index) => {
        const y = plot.top + index / 5 * plotHeight;
        return `<line class="revenue-line-grid" x1="${plot.left}" y1="${y}" x2="${width - plot.right}" y2="${y}"></line>
            <text class="revenue-line-axis-value" x="${plot.left - 12}" y="${y + 4}" text-anchor="end">${escapeHtml(formatChartValue(tick))}</text>`;
    }).join("");
    const xLabels = hours.filter((item) => item.hour % 2 === 0 || item.hour === 23).map((item) => {
        const point = pointFor(item);
        return `<text class="revenue-line-hour" x="${point.x}" y="${height - 12}" text-anchor="middle">${String(item.hour).padStart(2, "0")}:00</text>`;
    }).join("");
    const markers = hours.map((item, index) => {
        const point = points[index];
        const labelY = Math.max(13, point.y - 11);
        const label = item.grossRevenue > 0
            ? `<text class="revenue-line-value" x="${point.x}" y="${labelY}" text-anchor="middle">${escapeHtml(formatChartValue(item.grossRevenue))}</text>`
            : "";
        const accessibleLabel = `${String(item.hour).padStart(2, "0")}:00–${String((item.hour + 1) % 24).padStart(2, "0")}:00: ${formatVnd(item.grossRevenue)}, ${item.paidTransactions} giao dịch.`;
        return `<g class="revenue-line-point-group" tabindex="0" role="img" aria-label="${escapeHtml(accessibleLabel)}">
            ${label}
            <circle class="revenue-line-point${item.grossRevenue <= 0 ? " is-zero" : ""}" cx="${point.x}" cy="${point.y}" r="${item.grossRevenue > 0 ? 5 : 3}"></circle>
            <title>${escapeHtml(accessibleLabel)}</title>
        </g>`;
    }).join("");
    const polyline = points.map((point) => `${point.x},${point.y}`).join(" ");

    byId("monthlyRevenueChart").innerHTML = `<div class="revenue-line-viewport">
        <svg class="revenue-line-chart" viewBox="0 0 ${width} ${height}" role="img" aria-labelledby="hourlyLineTitle hourlyLineDescription">
            <title id="hourlyLineTitle">Doanh thu đã thu theo từng giờ</title>
            <desc id="hourlyLineDescription">Biểu đồ đường gồm 24 điểm từ 00:00 đến 23:00 ngày ${escapeHtml(selectedDate)}.</desc>
            ${grid}
            <polyline class="revenue-line-path" points="${polyline}"></polyline>
            ${markers}
            ${xLabels}
        </svg>
        ${maximumValue <= 0 ? '<div class="revenue-zero-note revenue-line-zero-note">Chưa có thanh toán thành công trong ngày này.</div>' : ""}
    </div>`;
}

function renderChart(items, hourlyItems = []) {
    if (state.from === state.to) {
        renderHourlyLineChart(hourlyItems);
        return;
    }

    byId("revenueChartLegend").innerHTML = '<span><i class="is-paid"></i>Đã thu</span><span><i class="is-pending"></i>Chờ thanh toán</span><span><i class="is-cancelled"></i>Giá trị đơn đã hủy</span>';
    const { grouping, buckets } = buildChartBuckets(items);
    const meta = {
        day: ["Cơ cấu giá trị đơn theo ngày", "Theo ngày", `${buckets.length} ngày trong khoảng đang chọn.`],
        week: ["Cơ cấu giá trị đơn theo tuần", "Theo tuần", `${buckets.length} tuần trong khoảng đang chọn.`],
        month: ["Cơ cấu giá trị đơn theo tháng", "Theo tháng", "Hiển thị đủ 12 tháng, kể cả tháng không phát sinh dữ liệu."]
    }[grouping];
    byId("revenueChartTitle").textContent = meta[0];
    byId("chartGranularityBadge").textContent = meta[1];
    byId("revenueChartDescription").textContent = `${meta[2]} Phần “Đã thu” luôn khớp KPI doanh thu.`;

    const bucketTotal = (item) => item.paidAmount + item.pendingPaymentAmount + item.cancelledAmount;
    const groupedChart = grouping === "week";
    const seriesFor = (item) => [
        ["paid", item.paidAmount],
        ["pending", item.pendingPaymentAmount],
        ["cancelled", item.cancelledAmount]
    ];
    const maximumValue = groupedChart
        ? Math.max(0, ...buckets.flatMap((item) => seriesFor(item).map((series) => series[1])))
        : Math.max(0, ...buckets.map(bucketTotal));
    const scaleMaximum = niceScaleMaximum(maximumValue);
    const ticks = Array.from({ length: 6 }, (_, index) => Math.round(scaleMaximum - scaleMaximum / 5 * index));
    const denseDailyChart = grouping === "day" && buckets.length > 20;
    const periodChart = grouping === "week" || grouping === "month";
    const distributedChart = grouping !== "month" && buckets.length >= 4 && buckets.length <= 7;
    const distributionClass = `${distributedChart ? ` is-distributed is-${grouping}` : ""}${groupedChart ? " is-grouped" : ""}`;
    const columnWidth = denseDailyChart ? 34 : groupedChart ? 120 : periodChart ? 48 : 56;
    const columnGap = denseDailyChart ? 10 : periodChart ? 12 : 14;
    const plotWidth = distributedChart ? 0 : Math.max(520, buckets.length * (columnWidth + columnGap) + 32);

    const bars = buckets.map((item, index) => {
        const totalValue = bucketTotal(item);
        const height = totalValue > 0 ? Math.max(3, Math.min(100, totalValue / scaleMaximum * 100)) : 0;
        const series = seriesFor(item);
        const segments = groupedChart
            ? series.map((segment, seriesIndex) => {
                const seriesHeight = segment[1] > 0 ? Math.max(3, Math.min(100, segment[1] / scaleMaximum * 100)) : 0;
                const seriesLabelPlacement = segment[1] <= 0 ? "is-zero" : seriesHeight >= 15 ? "is-inside" : "is-outside";
                return `<span class="revenue-bar-segment revenue-series-bar is-${segment[0]}${segment[1] <= 0 ? " is-zero" : ""}" style="--series-height:${seriesHeight > 0 ? seriesHeight : 0.8}%;--series-index:${seriesIndex}" aria-hidden="true"><span class="revenue-series-value ${seriesLabelPlacement}">${escapeHtml(formatChartValue(segment[1]))}</span></span>`;
            }).join("")
            : totalValue > 0
            ? series.filter((segment) => segment[1] > 0).map((segment) => `<span class="revenue-bar-segment is-${segment[0]}" style="flex-basis:${segment[1] / totalValue * 100}%" aria-hidden="true"></span>`).join("")
            : '<span class="revenue-bar-segment is-empty" aria-hidden="true"></span>';
        const accessibleLabel = `${item.tooltipLabel}: đã thu ${formatVnd(item.paidAmount)}, chờ thanh toán ${formatVnd(item.pendingPaymentAmount)}, giá trị đơn đã hủy ${formatVnd(item.cancelledAmount)}, tổng giá trị đơn ${formatVnd(totalValue)}.`;
        const labelPlacement = totalValue <= 0 ? "is-zero" : height >= 15 ? "is-inside" : "is-outside";
        return `<div class="revenue-chart-column${groupedChart ? " is-grouped" : ""}" tabindex="0" role="img" aria-label="${escapeHtml(accessibleLabel)}" style="--bar-height:${groupedChart ? 100 : height > 0 ? height : 0.8}%;--bar-index:${index}">
            <div class="revenue-chart-bar-area">
                <div class="revenue-chart-tooltip" role="tooltip">
                    <strong>${escapeHtml(item.tooltipLabel)}</strong>
                    <span><i class="is-paid"></i>Đã thu <b>${escapeHtml(formatVnd(item.paidAmount))}</b></span>
                    <span><i class="is-pending"></i>Chờ thanh toán <b>${escapeHtml(formatVnd(item.pendingPaymentAmount))}</b></span>
                    <span><i class="is-cancelled"></i>Giá trị đơn đã hủy <b>${escapeHtml(formatVnd(item.cancelledAmount))}</b></span>
                    <small>Tổng giá trị đơn: ${escapeHtml(formatVnd(totalValue))}</small>
                </div>
                <div class="revenue-bar-stack${groupedChart ? " is-grouped" : ""}">${segments}${groupedChart ? "" : `<span class="revenue-bar-total ${labelPlacement}">${escapeHtml(formatChartValue(totalValue))}</span>`}</div>
            </div>
            <span class="revenue-chart-label" title="${escapeHtml(item.tooltipLabel)}">${escapeHtml(item.label)}</span>
        </div>`;
    }).join("");

    const highest = buckets.reduce((best, item) => bucketTotal(item) > bucketTotal(best) ? item : best, buckets[0] || emptyRevenueStatus());
    byId("chartTextSummary").textContent = maximumValue > 0
        ? `${meta[0]}. ${highest.tooltipLabel} có tổng giá trị đơn cao nhất là ${formatVnd(bucketTotal(highest))}.`
        : `${meta[0]}. Chưa có dữ liệu trong khoảng thời gian này; các kỳ được giữ với giá trị 0.`;

    byId("monthlyRevenueChart").innerHTML = `<div class="revenue-chart-shell">
        <div class="revenue-chart-axis" aria-hidden="true">${ticks.map((tick) => `<span>${escapeHtml(formatChartValue(tick))}</span>`).join("")}</div>
        <div class="revenue-chart-viewport">
            <div class="revenue-chart-plot${distributionClass}"${plotWidth ? ` style="width:${plotWidth}px"` : ""}>
                <div class="revenue-chart-gridlines" aria-hidden="true">
                    ${ticks.map((_, index) => `<span class="revenue-chart-gridline" style="top:${index * 20}%"></span>`).join("")}
                </div>
                ${maximumValue <= 0 ? '<div class="revenue-zero-note">Chưa có đơn hàng trong khoảng thời gian này. Các kỳ vẫn được giữ để dễ đối chiếu.</div>' : ""}
                <div class="revenue-chart-columns${distributionClass}" style="--bucket-count:${buckets.length};${distributedChart ? "" : `grid-template-columns:repeat(${buckets.length},${columnWidth}px);column-gap:${columnGap}px`}">${bars}</div>
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
    const totalItems = Number(pagination.totalItems || 0);
    const totalPages = Math.max(1, Number(pagination.totalPages || 1));
    const page = Math.min(Math.max(1, Number(pagination.page || 1)), totalPages);
    const pageSize = Number(pagination.pageSize || state.pageSize);
    const start = totalItems ? (page - 1) * pageSize + 1 : 0;
    const end = Math.min(page * pageSize, totalItems);
    byId("orderPagination").innerHTML = `<p>Hiển thị <strong>${start}–${end}</strong> trong ${totalItems.toLocaleString("vi-VN")} đơn</p>
        <div class="revenue-page-buttons">
            <button class="revenue-page-button" type="button" data-order-page="${page - 1}" ${page <= 1 ? "disabled" : ""} aria-label="Trang trước">Trước</button>
            ${paginationItems(page, totalPages).map((item) => item === "ellipsis"
                ? '<span aria-hidden="true">…</span>'
                : `<button class="revenue-page-button${item === page ? " is-current" : ""}" type="button" data-order-page="${item}" ${item === page ? 'aria-current="page"' : ""}>${item}</button>`).join("")}
            <button class="revenue-page-button" type="button" data-order-page="${page + 1}" ${page >= totalPages ? "disabled" : ""} aria-label="Trang sau">Sau</button>
        </div>`;
}

function renderOrders(items, pagination) {
    byId("orderDetailCount").textContent = `${Number(pagination.totalItems || 0).toLocaleString("vi-VN")} đơn`;
    if (!items.length) {
        byId("orderDetailTable").innerHTML = '<div class="revenue-empty-state"><h3>Chưa có đơn hàng phù hợp</h3><p>Thử đổi khoảng thời gian, từ khóa hoặc bộ lọc trạng thái.</p></div>';
        renderPagination(pagination);
        return;
    }

    const dateFormatter = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
    byId("orderDetailTable").innerHTML = `<div class="revenue-table-wrap"><table class="revenue-table">
        <thead><tr><th>Mã đơn</th><th>Khách hàng</th><th>Phương tiện</th><th>Ngày thuê</th><th style="text-align:right">Tổng tiền</th><th>Trạng thái đơn</th><th>Thanh toán</th></tr></thead>
        <tbody>${items.map((item) => `<tr>
            <td data-label="Mã đơn"><span class="revenue-order-code">${escapeHtml(item.bookingCode || `BK-${item.id}`)}</span></td>
            <td data-label="Khách hàng">${escapeHtml(item.customerName || "—")}</td>
            <td data-label="Phương tiện"><span class="revenue-vehicle-name">${escapeHtml(item.carName || "—")}</span></td>
            <td data-label="Ngày thuê">${escapeHtml(dateFormatter.format(new Date(item.pickupDate)))}</td>
            <td data-label="Tổng tiền" class="revenue-money">${escapeHtml(formatVnd(item.totalAmount))}</td>
            <td data-label="Trạng thái đơn">${bookingBadge(item.bookingStatus)}</td>
            <td data-label="Thanh toán">${paymentBadge(item.paymentStatus)}</td>
        </tr>`).join("")}</tbody>
    </table></div>`;
    renderPagination(pagination);
}

function renderLoading() {
    root.classList.add("is-loading");
    byId("monthlyRevenueChart").innerHTML = '<div class="revenue-chart-skeleton" aria-label="Đang tải biểu đồ"></div>';
    byId("orderDetailTable").innerHTML = `<div class="revenue-table-skeleton" aria-label="Đang tải danh sách đơn hàng">${Array.from({ length: 5 }, () => '<div class="revenue-skeleton-line"></div>').join("")}</div>`;
    byId("orderDetailTable").setAttribute("aria-busy", "true");
}

function updateExportLink() {
    const query = new URLSearchParams({ type: "revenue", from: state.from, to: state.to });
    if (state.bookingStatus) query.set("bookingStatus", state.bookingStatus);
    if (state.paymentStatus) query.set("paymentStatus", state.paymentStatus);
    byId("exportRevenueReport").href = `/admin/reports/export?${query}`;
}

async function load() {
    const currentLoad = ++loadSequence;
    renderLoading();
    updateExportLink();
    try {
        const query = new URLSearchParams({
            from: state.from,
            to: state.to,
            page: String(state.page),
            pageSize: String(state.pageSize)
        });
        if (state.search) query.set("search", state.search);
        if (state.bookingStatus) query.set("bookingStatus", state.bookingStatus);
        if (state.paymentStatus) query.set("paymentStatus", state.paymentStatus);
        const data = await fetchJson(`api/admin/reports/revenue?${query}`);
        if (currentLoad !== loadSequence) return;
        state.page = data.pagination?.page || 1;
        renderSummary(data);
        renderChart(data.daily || [], data.hourly || []);
        renderOrders(data.recentBookings || [], data.pagination || { page: 1, pageSize: state.pageSize, totalItems: 0, totalPages: 1 });
    } catch (error) {
        if (currentLoad !== loadSequence) return;
        renderSummary({ summary: {} });
        byId("monthlyRevenueChart").innerHTML = `<div class="revenue-error-state"><h3>Không thể tải dữ liệu biểu đồ</h3><p>${escapeHtml(error.message)}</p><button class="revenue-primary-button" data-retry-dashboard type="button">Thử lại</button></div>`;
        byId("orderDetailTable").innerHTML = `<div class="revenue-error-state"><h3>Không thể tải danh sách đơn hàng</h3><p>Kiểm tra kết nối và thử tải lại báo cáo.</p><button class="revenue-primary-button" data-retry-dashboard type="button">Thử lại</button></div>`;
        byId("orderPagination").innerHTML = "";
    } finally {
        if (currentLoad === loadSequence) {
            root.classList.remove("is-loading");
            byId("orderDetailTable").removeAttribute("aria-busy");
        }
    }
}

function selectPreset(key) {
    state.preset = key;
    state.page = 1;
    if (key !== "CUSTOM") {
        Object.assign(state, rangeFor(key));
        renderRangeControls();
        load();
        return;
    }
    byId("customStartDate").value = state.from;
    byId("customEndDate").value = state.to;
    renderRangeControls();
    byId("customStartDate").focus();
}

function applyCustomRange() {
    const from = byId("customStartDate").value;
    const to = byId("customEndDate").value;
    const error = byId("customRangeError");
    let message = "";
    if (!from || !to) message = "Vui lòng chọn đủ ngày bắt đầu và ngày kết thúc.";
    else if (from > to) message = "Ngày bắt đầu không được sau ngày kết thúc.";
    else if (inclusiveDayCount(parseDate(from), parseDate(to)) > 366) message = "Khoảng báo cáo không được vượt quá 366 ngày.";
    error.textContent = message;
    error.hidden = !message;
    if (message) return;
    state.preset = "CUSTOM";
    state.from = from;
    state.to = to;
    state.page = 1;
    renderRangeControls();
    load();
}

function bindEvents() {
    byId("revenueRangeButtons").addEventListener("click", (event) => {
        const button = event.target.closest("[data-range-preset]");
        if (button) selectPreset(button.dataset.rangePreset);
    });
    byId("revenueRangeMore").addEventListener("change", (event) => {
        if (event.target.value) selectPreset(event.target.value);
    });
    byId("applyCustomRange").addEventListener("click", applyCustomRange);
    byId("orderPagination").addEventListener("click", (event) => {
        const button = event.target.closest("[data-order-page]");
        if (!button || button.disabled) return;
        state.page = Number(button.dataset.orderPage);
        load();
    });
    byId("orderBookingStatus").addEventListener("change", (event) => {
        state.bookingStatus = event.target.value;
        state.page = 1;
        load();
    });
    byId("orderPaymentStatus").addEventListener("change", (event) => {
        state.paymentStatus = event.target.value;
        state.page = 1;
        load();
    });
    byId("orderSearch").addEventListener("input", debounce((event) => {
        state.search = event.target.value.trim();
        state.page = 1;
        load();
    }, 300));
    root.addEventListener("click", (event) => {
        if (event.target.closest("[data-retry-dashboard]")) load();
    });
}

if (root) {
    Object.assign(state, rangeFor("THIS_MONTH"));
    renderRangeControls();
    bindEvents();
    const adminUser = await (window.VivuCarAdminAuthReady ?? Promise.resolve(true));
    if (adminUser) load();
}