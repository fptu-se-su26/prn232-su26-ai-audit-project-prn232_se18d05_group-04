import { fetchJson } from "../shared/api-client.js";
import { escapeHtml } from "../shared/dom.js";
import { formatVnd } from "../shared/utils.js";

const root = document.querySelector("[data-admin-dashboard-page]");
const state = { from: null, to: null };
const fullDate = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
const shortDate = new Intl.DateTimeFormat("vi-VN", { day: "2-digit", month: "2-digit" });

function isoDate(date) { const offset = date.getTimezoneOffset(); return new Date(date.getTime() - offset * 60000).toISOString().slice(0, 10); }
function rangeFor(key) {
  const now = new Date(); const end = new Date(now.getFullYear(), now.getMonth(), now.getDate()); let start = new Date(end);
  if (key === "7days") start.setDate(end.getDate() - 6);
  if (key === "month") start = new Date(end.getFullYear(), end.getMonth(), 1);
  if (key === "year") start = new Date(end.getFullYear(), 0, 1);
  return { from: isoDate(start), to: isoDate(end) };
}
function setText(id, value) { const el = document.getElementById(id); if (el) el.textContent = value; }
function setLoading(value) {
  root?.classList.toggle("is-loading", value);
  document.getElementById("chartSkeleton").hidden = !value;
  if (value) document.getElementById("revenueChart").hidden = true;
  document.querySelectorAll("#revenuePresets button, #customRangeForm button").forEach(button => button.disabled = value);
}
function showError(error) { setText("dashboardErrorMessage", error?.message || "Đã xảy ra lỗi không xác định."); document.getElementById("dashboardError").hidden = false; }
function renderSummary(summary = {}) {
  setText("grossRevenue", formatVnd(summary.grossRevenue ?? 0)); setText("netRevenue", formatVnd(summary.netRevenue ?? 0));
  setText("depositCollected", formatVnd(summary.depositCollected ?? 0)); setText("averageOrderValue", formatVnd(summary.averageOrderValue ?? 0));
  setText("completedSummary", `${summary.completedBookings ?? 0} đơn hoàn tất`); setText("totalBookingSummary", `${summary.totalBookings ?? 0} đơn trong kỳ`);
  setText("cancelRateSummary", `Tỷ lệ hủy ${Number(summary.cancelRate ?? 0).toLocaleString("vi-VN")}%`);
}
function renderChart(items = []) {
  const chart = document.getElementById("revenueChart"); const empty = document.getElementById("chartEmpty");
  setText("snapshotCount", `${items.length} ngày dữ liệu`); chart.hidden = !items.length; empty.hidden = !!items.length;
  if (!items.length) { chart.replaceChildren(); return; }
  const max = Math.max(...items.map(item => Number(item.grossRevenue || 0)), 1);
  chart.innerHTML = items.map(item => {
    const amount = Number(item.grossRevenue || 0); const scale = amount === 0 ? 0.02 : Math.max(0.08, amount / max); const date = new Date(`${item.date}T00:00:00`);
    return `<div class="revenue-bar-item" tabindex="0" aria-label="${escapeHtml(fullDate.format(date))}: ${escapeHtml(formatVnd(amount))}"><div class="revenue-bar-value">${escapeHtml(formatVnd(amount))}</div><div class="revenue-bar-track"><span style="--bar-scale:${scale}"></span></div><small>${escapeHtml(shortDate.format(date))}</small></div>`;
  }).join("");
}
function badge(value, type) {
  const key = String(value || "pending").toLowerCase();
  const labels = type === "payment" ? { pending:"Chờ thanh toán", success:"Thành công", failed:"Thất bại", refunded:"Đã hoàn tiền" } : { pending:"Chờ xử lý", approved:"Đã xác nhận", rejected:"Đã từ chối", completed:"Hoàn tất", cancelled:"Đã hủy" };
  return `<span class="revenue-badge revenue-badge-${escapeHtml(key)}">${escapeHtml(labels[key] || key)}</span>`;
}
function renderBookings(items = []) {
  const body = document.getElementById("recentBookingsBody"); const wrap = body.closest(".revenue-table-wrap"); const empty = document.getElementById("recentEmpty");
  setText("recentCount", `${items.length} đơn`); wrap.hidden = !items.length; empty.hidden = !!items.length;
  body.innerHTML = items.map(item => `<tr><td><strong>${escapeHtml(item.bookingCode || `#${item.id}`)}</strong></td><td>${escapeHtml(item.customerName)}</td><td>${escapeHtml(item.carName)}</td><td>${escapeHtml(fullDate.format(new Date(item.pickupDate)))}</td><td class="revenue-money">${escapeHtml(formatVnd(item.totalAmount))}</td><td>${badge(item.bookingStatus,"booking")}</td><td>${badge(item.paymentStatus,"payment")}</td></tr>`).join("");
}
async function loadDashboard() {
  setLoading(true); document.getElementById("dashboardError").hidden = true;
  setText("selectedPeriod", `${fullDate.format(new Date(`${state.from}T00:00:00`))} – ${fullDate.format(new Date(`${state.to}T00:00:00`))}`);
  try { const data = await fetchJson(`api/admin/reports/revenue?${new URLSearchParams(state)}`); renderSummary(data.summary); renderChart(data.daily); renderBookings(data.recentBookings); }
  catch (error) { showError(error); renderSummary(); renderChart([]); renderBookings([]); }
  finally { setLoading(false); }
}
function choosePreset(button) {
  document.querySelectorAll("#revenuePresets button").forEach(item => item.classList.toggle("is-active", item === button));
  const key = button.dataset.range; document.getElementById("customRangeForm").hidden = key !== "custom";
  if (key !== "custom") { Object.assign(state, rangeFor(key)); loadDashboard(); }
}
if (root) {
  Object.assign(state, rangeFor("month"));
  document.getElementById("revenuePresets").addEventListener("click", event => { const button = event.target.closest("button[data-range]"); if (button) choosePreset(button); });
  document.getElementById("customRangeForm").addEventListener("submit", event => { event.preventDefault(); const from = document.getElementById("dateFrom").value; const to = document.getElementById("dateTo").value; if (!from || !to || from > to) return showError(new Error("Khoảng ngày không hợp lệ.")); Object.assign(state,{from,to}); loadDashboard(); });
  document.getElementById("retryDashboard").addEventListener("click", loadDashboard); loadDashboard();
}
