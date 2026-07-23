import { fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";
import { closeModal, openModal } from "../../shared/modal.js";
import { showToast } from "../../shared/toast.js";

const root = document.querySelector("[data-admin-content-page]");
const body = document.getElementById("contentModerationTableBody");
let items = [];
const filters = { type: "", from: null, to: null };

function formatDate(value) {
    return value ? new Date(value).toLocaleString("vi-VN") : "—";
}

function filteredItems() {
    return items.filter(item => {
        const createdAt = new Date(item.createdAt);
        return (!filters.type || item.source === filters.type)
            && (!filters.from || createdAt >= filters.from)
            && (!filters.to || createdAt <= filters.to);
    });
}

function applyFilters() {
    const fromValue = document.getElementById("contentDateFrom").value;
    const toValue = document.getElementById("contentDateTo").value;
    const from = fromValue ? new Date(fromValue + "T00:00:00") : null;
    const to = toValue ? new Date(toValue + "T23:59:59.999") : null;

    if (from && to && from > to) {
        showToast("Ngày bắt đầu không được sau ngày kết thúc.", "error");
        return;
    }

    filters.type = document.getElementById("contentTypeFilter").value;
    filters.from = from;
    filters.to = to;
    render();
}

function resetFilters() {
    document.getElementById("contentTypeFilter").value = "";
    document.getElementById("contentDateFrom").value = "";
    document.getElementById("contentDateTo").value = "";
    filters.type = "";
    filters.from = null;
    filters.to = null;
    render();
}

function statusLabel(value) {
    return { published: "Đã đăng", reported: "Bị báo cáo", in_review: "Đang xem xét", resolved: "Đã xử lý", rejected: "Đã từ chối", closed: "Đã đóng" }[value] || value;
}

function statusTone(value) {
    if (["resolved", "published", "closed"].includes(value)) return "success";
    if (["reported", "rejected"].includes(value)) return "danger";
    return "warning";
}

function renderSummary() {
    const incidents = items.filter(item => item.source === "incident");
    const rows = [
        ["Tổng nội dung", items.length],
        ["Đánh giá", items.filter(item => item.source === "review").length],
        ["Báo cáo cần xử lý", incidents.filter(item => ["reported", "in_review"].includes(item.status)).length]
    ];
    document.getElementById("contentModerationSummary").innerHTML = rows.map(row =>
        '<article class="summary-card"><span>' + row[0] + '</span><strong>' + row[1] + '</strong></article>'
    ).join("");
}

function render() {
    const visibleItems = filteredItems();
    renderSummary();
    document.getElementById("contentModerationEmpty").classList.toggle("hidden", visibleItems.length > 0);
    body.closest(".table-wrap").classList.toggle("hidden", visibleItems.length === 0);
    document.getElementById("contentFilterResult").textContent = visibleItems.length + " nội dung";
    body.innerHTML = visibleItems.map(item =>
        '<tr><td>' + escapeHtml(item.type) + '</td>' +
        '<td><strong>' + escapeHtml(item.title) + '</strong><small class="block text-zinc-500">' + escapeHtml(item.content) + '</small></td>' +
        '<td>' + escapeHtml(item.reporterName) + '</td>' +
        '<td>' + escapeHtml(formatDate(item.createdAt)) + '</td>' +
        '<td><span class="status-badge status-' + statusTone(item.status) + '">' + escapeHtml(statusLabel(item.status)) + '</span></td>' +
        '<td><button class="btn btn-secondary btn-sm" type="button" data-view-content="' + item.source + ':' + item.id + '">Xem</button></td></tr>'
    ).join("");
}

function viewContent(token) {
    const parts = token.split(":");
    const item = items.find(entry => entry.source === parts[0] && entry.id === Number(parts[1]));
    if (!item) return;
    document.getElementById("contentModerationDetail").innerHTML =
        '<div class="grid gap-3 text-sm"><h3>' + escapeHtml(item.title) + '</h3>' +
        '<p>' + escapeHtml(item.content) + '</p>' +
        '<p><strong>Người gửi:</strong> ' + escapeHtml(item.reporterName) + '</p>' +
        '<p><strong>Trạng thái:</strong> ' + escapeHtml(statusLabel(item.status)) + '</p></div>';
    openModal("contentModerationDetailModal");
}

async function load() {
    body.innerHTML = '<tr><td colspan="6" class="empty-cell">Đang tải dữ liệu kiểm duyệt...</td></tr>';
    try {
        const data = await fetchJson("api/admin/moderation/content");
        items = Array.isArray(data) ? data : data.items ?? [];
        render();
    } catch (error) {
        body.innerHTML = '<tr><td colspan="6" class="empty-cell">' + escapeHtml(error.message) + '</td></tr>';
    }
}

if (root) {
    document.getElementById("btnFilterContent").addEventListener("click", applyFilters);
    document.getElementById("btnResetContentFilter").addEventListener("click", resetFilters);
    document.getElementById("btnResetEmptyContent").addEventListener("click", resetFilters);
    body.addEventListener("click", event => {
        const button = event.target.closest("[data-view-content]");
        if (button) viewContent(button.dataset.viewContent);
    });
    root.querySelectorAll("[data-close-modal]").forEach(button =>
        button.addEventListener("click", () => closeModal("contentModerationDetailModal"))
    );
    load();
}
