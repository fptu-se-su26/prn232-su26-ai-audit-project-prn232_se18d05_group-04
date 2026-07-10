import { fetchJson } from "../shared/api-client.js";
import { escapeHtml } from "../shared/dom.js";
import { formatVnd } from "../shared/utils.js";

const root = document.querySelector("[data-admin-dashboard-page]");

function setText(id, value) {
    const element = document.getElementById(id);
    if (element) element.textContent = value;
}

function renderChart(items = []) {
    const chart = document.getElementById("revenueChart");
    if (!chart) return;

    if (!items.length) {
        chart.textContent = "Chua có d? li?u doanh thu trong kho?ng th?i gian này.";
        return;
    }

    const max = Math.max(...items.map(item => Number(item.revenue ?? 0)), 1);
    chart.innerHTML = items.map(item => {
        const percent = Math.max(6, Math.round((Number(item.revenue ?? 0) / max) * 100));
        return `<div class="bar-item"><div class="bar" style="height:${percent}%"></div><span>${escapeHtml(item.label)}</span></div>`;
    }).join("");
}

function renderOrders(orders = []) {
    const body = document.getElementById("recentOrdersBody");
    if (!body) return;

    if (!orders.length) {
        body.innerHTML = `<tr><td colspan="6" class="empty-cell">Chua có d? li?u.</td></tr>`;
        return;
    }

    body.innerHTML = orders.map(order => `<tr>
        <td>${escapeHtml(order.id ?? order.bookingId ?? "-")}</td>
        <td>${escapeHtml(order.customerName ?? order.customer ?? "-")}</td>
        <td>${escapeHtml(order.carName ?? order.car ?? "-")}</td>
        <td>${escapeHtml(order.rentalDate ?? order.pickupDate ?? "-")}</td>
        <td>${escapeHtml(formatVnd(order.totalAmount ?? order.total_amount))}</td>
        <td>${escapeHtml(order.paymentStatus ?? order.payment_status ?? "-")}</td>
    </tr>`).join("");
}

async function loadDashboard() {
    const range = document.getElementById("revenueRange")?.value ?? "this_month";

    try {
        const data = await fetchJson(`api/admin/reports/revenue?range=${encodeURIComponent(range)}`);
        setText("totalRevenue", formatVnd(data.totalRevenue ?? data.total_revenue));
        setText("completedOrders", String(data.completedOrders ?? data.completed_orders ?? 0));
        setText("averageOrderValue", formatVnd(data.averageOrderValue ?? data.average_order_value));
        setText("cancelRate", `${data.cancelRate ?? data.cancel_rate ?? 0}%`);
        renderChart(data.chartData ?? data.chart_data ?? []);
        renderOrders(data.recentOrders ?? data.recent_orders ?? []);
    } catch (error) {
        renderChart([]);
        renderOrders([]);
        const chart = document.getElementById("revenueChart");
        if (chart) chart.textContent = error.message;
    }
}

if (root) {
    document.getElementById("btnApplyRevenueFilter")?.addEventListener("click", loadDashboard);
    loadDashboard();
}