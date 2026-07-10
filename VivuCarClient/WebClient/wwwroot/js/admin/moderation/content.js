import { fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";

const root = document.querySelector("[data-admin-content-page]");

function render(items) {
    const body = document.getElementById("contentModerationBody");
    if (!body) return;
    if (!items.length) {
        body.innerHTML = `<tr><td colspan="6" class="empty-cell">Chua có d? li?u ki?m duy?t.</td></tr>`;
        return;
    }
    body.innerHTML = items.map(item => `<tr><td>${escapeHtml(item.type)}</td><td>${escapeHtml(item.reporterName ?? item.userName ?? "-")}</td><td>${escapeHtml(item.content ?? item.title ?? "-")}</td><td>${escapeHtml(item.status)}</td><td>${escapeHtml(item.createdAt ?? item.created_at ?? "-")}</td><td></td></tr>`).join("");
}

async function loadContent() {
    const body = document.getElementById("contentModerationBody");
    try {
        const data = await fetchJson("api/admin/moderation/content");
        render(Array.isArray(data) ? data : data.items ?? []);
    } catch (error) {
        if (body) body.innerHTML = `<tr><td colspan="6" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
    }
}

if (root) loadContent();