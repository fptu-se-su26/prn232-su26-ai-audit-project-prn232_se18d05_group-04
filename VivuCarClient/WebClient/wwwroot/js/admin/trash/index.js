import { fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";

const root = document.querySelector("[data-admin-trash-page]");

function render(items) {
    const body = document.getElementById("trashTableBody");
    if (!body) return;
    if (!items.length) {
        body.innerHTML = `<tr><td colspan="5" class="empty-cell">Chua có d? li?u thùng rác.</td></tr>`;
        return;
    }
    body.innerHTML = items.map(item => `<tr><td>${escapeHtml(item.type)}</td><td>${escapeHtml(item.name ?? item.title ?? item.id)}</td><td>${escapeHtml(item.deletedAt ?? item.deleted_at ?? "-")}</td><td>${escapeHtml(item.deletedBy ?? item.deleted_by ?? "-")}</td><td></td></tr>`).join("");
}

async function loadTrash() {
    const body = document.getElementById("trashTableBody");
    try {
        const data = await fetchJson("api/admin/trash");
        render(Array.isArray(data) ? data : data.items ?? []);
    } catch (error) {
        if (body) body.innerHTML = `<tr><td colspan="5" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
    }
}

if (root) loadTrash();