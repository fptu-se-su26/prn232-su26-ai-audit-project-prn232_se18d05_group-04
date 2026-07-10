import { fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";

const root = document.querySelector("[data-admin-licenses-page]");

function render(items) {
    const body = document.getElementById("licensesTableBody");
    if (!body) return;
    if (!items.length) {
        body.innerHTML = `<tr><td colspan="6" class="empty-cell">Chua có d? li?u gi?y phép.</td></tr>`;
        return;
    }
    body.innerHTML = items.map(item => `<tr><td>${escapeHtml(item.userName ?? item.fullName ?? "-")}</td><td>${escapeHtml(item.document_type ?? item.documentType ?? "-")}</td><td>${escapeHtml(item.file_name ?? item.fileName ?? "-")}</td><td>${escapeHtml(item.verified ? "Ðã xác minh" : "Chua xác minh")}</td><td>${escapeHtml(item.created_at ?? item.createdAt ?? "-")}</td><td></td></tr>`).join("");
}

async function loadLicenses() {
    const body = document.getElementById("licensesTableBody");
    try {
        const data = await fetchJson("api/admin/moderation/licenses");
        render(Array.isArray(data) ? data : data.items ?? []);
    } catch (error) {
        if (body) body.innerHTML = `<tr><td colspan="6" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
    }
}

if (root) loadLicenses();