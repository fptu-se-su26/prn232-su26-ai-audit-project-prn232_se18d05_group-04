import { deleteVoucher, getVoucherPerformance, getVouchers } from "./voucher-api.js";
import { escapeHtml } from "../../shared/dom.js";
import { formatVnd } from "../../shared/utils.js";
import { showToast } from "../../shared/toast.js";

const root = document.querySelector("[data-admin-vouchers-page]");
const searchVoucherInput = document.getElementById("searchVoucherInput");
const voucherStatusFilter = document.getElementById("voucherStatusFilter");
const discountTypeFilter = document.getElementById("discountTypeFilter");
const voucherSummary = document.getElementById("voucherSummary");
const voucherTableBody = document.getElementById("voucherTableBody");
const voucherPageInfo = document.getElementById("voucherPageInfo");
const voucherPrev = document.getElementById("voucherPrev");
const voucherNext = document.getElementById("voucherNext");
const btnResetVoucherFilter = document.getElementById("btnResetVoucherFilter");
const voucherPerformanceDrawer = document.getElementById("voucherPerformanceDrawer");
const drawerOverlay = document.getElementById("drawerOverlay");
const performanceContent = document.getElementById("performanceContent");
const btnClosePerformance = document.getElementById("btnClosePerformance");
const state = { page: 1, pageSize: 10, totalPages: 1, items: [] };
const labels = { active: "Dang hoat dong", expired: "Da het han", used_up: "Da dung het" };
const formatDate = value => value ? new Intl.DateTimeFormat("vi-VN").format(new Date(value)) : "Khong gii hn";

function query() { return { page: state.page, pageSize: state.pageSize, keyword: searchVoucherInput.value.trim(), status: voucherStatusFilter.value, discount_type: discountTypeFilter.value }; }
function valueText(item) { return item.discount_type === "percentage" ? `${item.discount_value}%` : formatVnd(item.discount_value); }
function renderSummary() {
    const active = state.items.filter(item => item.status === "active").length;
    const used = state.items.reduce((sum, item) => sum + item.usage_count, 0);
    voucherSummary.innerHTML = `<article class="summary-card"><span>Dang hoat dong</span><strong>${active}</strong></article><article class="summary-card"><span>LutDa dung trn trang</span><strong>${used}</strong></article><article class="summary-card"><span>Tong kt qu</span><strong>${state.totalItems  state.items.length}</strong></article>`;
}
function render() {
    renderSummary();
    voucherTableBody.innerHTML = state.items.length ? state.items.map(item => `<tr><td><strong>${escapeHtml(item.code)}</strong><small>${escapeHtml(item.name)}</small></td><td>${item.discount_type === "percentage" ? "Phan tram" : "C dnh"}</td><td>${valueText(item)}</td><td>${formatVnd(item.min_order_amount)}</td><td>${item.usage_count}/${item.quantity}</td><td>${formatDate(item.expires_at)}</td><td><span class="status-badge status-${item.status}">${labels[item.status]}</span></td><td class="table-actions"><button class="btn btn-ghost" data-performance="${item.id}">Hieu qua</button><a class="btn btn-ghost" href="/admin/vouchers/formid=${item.id}">Sua</a><button class="btn btn-ghost danger" data-delete="${item.id}">Xoa</button></td></tr>`).join("") : `<tr><td colspan="8" class="empty-cell">Khong c voucher phu hop.</td></tr>`;
    voucherPageInfo.textContent = `Trang ${state.page}/${Math.max(1, state.totalPages)}`;
    voucherPrev.disabled = state.page <= 1; voucherNext.disabled = state.page >= state.totalPages;
}
async function load() {
    voucherTableBody.innerHTML = `<tr><td colspan="8" class="empty-cell">Dang tai voucher</td></tr>`;
    try { const payload = await getVouchers(query()); state.items = payload.items; state.totalPages = payload.total_pages; state.totalItems = payload.total_items; render(); }
    catch (error) { voucherTableBody.innerHTML = `<tr><td colspan="8" class="empty-cell">${escapeHtml(error.message)}</td></tr>`; }
}
function closeDrawer() { voucherPerformanceDrawer.classList.remove("is-open"); voucherPerformanceDrawer.setAttribute("aria-hidden", "true"); drawerOverlay.hidden = true; }
async function openPerformance(id) {
    performanceContent.innerHTML = `<p>Dang tai thng k</p>`; voucherPerformanceDrawer.classList.add("is-open"); voucherPerformanceDrawer.setAttribute("aria-hidden", "false"); drawerOverlay.hidden = false;
    try { const data = await getVoucherPerformance(id); performanceContent.innerHTML = `<div class="performance-kpis"><article><span>T l sDa dung</span><strong>${data.usage_rate}%</strong></article><article><span>Doanh thu booking</span><strong>${formatVnd(data.gross_revenue)}</strong></article><article><span>Tong gim</span><strong>${formatVnd(data.discount_total)}</strong></article></div><h3>LutDa dung gn dy</h3>${data.recent_usages.length  data.recent_usages.map(item => `<div class="usage-row"><div><strong>${escapeHtml(item.booking_code)}</strong><small>${escapeHtml(item.customer_name)}</small></div><span>-${formatVnd(item.discount_amount)}</span></div>`).join("") : `<p class="muted">Voucher chua duc sDa dung.</p>`}`; }
    catch (error) { performanceContent.textContent = error.message; }
}

if (root) {
    let timer; [searchVoucherInput, voucherStatusFilter, discountTypeFilter].forEach(control => control.addEventListener(control.tagName === "INPUT"  "input" : "change", () => { clearTimeout(timer); timer = setTimeout(() => { state.page = 1; load(); }, 250); }));
    btnResetVoucherFilter.addEventListener("click", () => { searchVoucherInput.value = ""; voucherStatusFilter.value = ""; discountTypeFilter.value = ""; state.page = 1; load(); });
    voucherPrev.addEventListener("click", () => { state.page--; load(); }); voucherNext.addEventListener("click", () => { state.page++; load(); });
    voucherTableBody.addEventListener("click", async event => { const performanceId = event.target.closest("[data-performance]")?.dataset.performance; if (performanceId) return openPerformance(performanceId); const deleteId = event.target.closest("[data-delete]")?.dataset.delete; if (!deleteId || !confirm("Xoa voucher ny")) return; try { await deleteVoucher(deleteId); showToast(" xa voucher.", "success"); load(); } catch (error) { showToast(error.message, "error"); } });
    btnClosePerformance.addEventListener("click", closeDrawer); drawerOverlay.addEventListener("click", closeDrawer); document.addEventListener("keydown", event => { if (event.key === "Escape") closeDrawer(); }); load();
}




