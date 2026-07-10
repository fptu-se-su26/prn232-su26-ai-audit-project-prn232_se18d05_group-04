import { apiFetch, fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";
import { showToast } from "../../shared/toast.js";

const root = document.querySelector("[data-admin-reports-page]");

function selectedType() {
    return document.querySelector('input[name="export_type"]:checked')?.value ?? "transactions";
}

function queryString() {
    const query = new URLSearchParams({ type: selectedType() });
    const from = document.getElementById("exportDateFrom")?.value;
    const to = document.getElementById("exportDateTo")?.value;
    const paymentStatus = document.getElementById("paymentStatusFilter")?.value;
    if (from) query.set("from", from);
    if (to) query.set("to", to);
    if (paymentStatus) query.set("paymentStatus", paymentStatus);
    return query;
}

function renderPreview(data) {
    const preview = document.getElementById("reportPreview");
    const rows = Array.isArray(data) ? data : data.rows ?? data.Rows ?? [];
    if (!preview) return;

    if (!rows.length) {
        preview.textContent = "Không có d? li?u preview.";
        return;
    }

    const headers = Object.keys(rows[0]);
    preview.innerHTML = `<div class="table-wrap"><table><thead><tr>${headers.map(header => `<th>${escapeHtml(header)}</th>`).join("")}</tr></thead><tbody>${rows.map(row => `<tr>${headers.map(header => `<td>${escapeHtml(row[header])}</td>`).join("")}</tr>`).join("")}</tbody></table></div>`;
}

async function previewReport() {
    const preview = document.getElementById("reportPreview");
    if (preview) preview.textContent = "Ðang t?i preview...";

    try {
        renderPreview(await fetchJson(`api/admin/reports/preview?${queryString()}`));
    } catch (error) {
        if (preview) preview.textContent = error.message;
    }
}

async function exportReport(format) {
    const button = document.getElementById(format === "excel" ? "btnExportExcel" : "btnExportPdf");
    button.disabled = true;

    try {
        const response = await apiFetch(`api/admin/reports/export/${format}?${queryString()}`);
        if (!response.ok) throw new Error(`Unable to export ${format}.`);
        const blob = await response.blob();
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = `vivucar-${selectedType()}-${Date.now()}.${format === "excel" ? "xlsx" : "pdf"}`;
        link.click();
        URL.revokeObjectURL(url);
        showToast("Xu?t file thành công.", "success");
    } catch (error) {
        showToast(error.message, "error");
    } finally {
        button.disabled = false;
    }
}

if (root) {
    document.getElementById("btnPreviewReport")?.addEventListener("click", previewReport);
    document.getElementById("btnExportExcel")?.addEventListener("click", () => exportReport("excel"));
    document.getElementById("btnExportPdf")?.addEventListener("click", () => exportReport("pdf"));
}