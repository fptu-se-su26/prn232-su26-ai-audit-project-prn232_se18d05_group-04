import { getVoucher, saveVoucher } from "./voucher-api.js";
import { formatVnd } from "../../shared/utils.js";
import { showToast } from "../../shared/toast.js";

const form = document.querySelector("[data-admin-voucher-form]");
const byId = id => document.getElementById(id);
const fields = ["name", "code", "discount_value", "min_order_amount", "max_discount", "quantity", "expires_at"];

function payload() {
    const data = new FormData(form);
    return { name: data.get("name").trim(), code: data.get("code").trim().toUpperCase(), discount_type: data.get("discount_type"), discount_value: Number(data.get("discount_value")), min_order_amount: Number(data.get("min_order_amount")), max_discount: Number(data.get("max_discount")), quantity: Number(data.get("quantity")), expires_at: data.get("expires_at") ? new Date(data.get("expires_at")).toISOString() : null };
}
function syncPreview() {
    const data = payload();
    byId("voucherPreviewCode").textContent = data.code || "SUMMER2026"; byId("voucherPreviewName").textContent = data.name || "Tn chuong trnh";
    byId("voucherPreviewValue").textContent = data.discount_type === "fixed" ? `Gim ${formatVnd(data.discount_value || 0)}` : `Gim ${data.discount_value || 0}%`;
    byId("voucherPreviewCondition").textContent = `on ti thiu ${formatVnd(data.min_order_amount || 0)}`;
}
function showError(message) { const box = byId("voucherError"); box.textContent = message; box.hidden = !message; }
async function hydrate(id) {
    try { const item = await getVoucher(id); fields.forEach(key => { const input = byId(key); if (!input) return; if (key === "expires_at") input.value = item.expires_at ? new Date(item.expires_at).toISOString().slice(0, 16) : ""; else input.value = item[key]; }); form.querySelector(`[name="discount_type"][value="${item.discount_type}"]`).checked = true; syncPreview(); }
    catch (error) { showError(error.message); }
}
async function submit(event) {
    event.preventDefault(); showError(""); if (!form.reportValidity()) return;
    const button = byId("btnSuaveVoucher"); button.disabled = true; button.textContent = "ang luu";
    try { await saveVoucher(payload(), form.dataset.voucherId || null); showToast(" luu voucher.", "success"); window.location.assign("/admin/vouchers"); }
    catch (error) { showError(error.message); showToast(error.message, "error"); }
    finally { button.disabled = false; button.textContent = "Luu voucher"; }
}

if (form) {
    byId("code").addEventListener("input", event => { event.target.value = event.target.value.toUpperCase().replace(/[^A-Z0-9_-]/g, ""); });
    form.addEventListener("input", syncPreview); form.addEventListener("change", syncPreview); form.addEventListener("submit", submit); syncPreview();
    if (form.dataset.voucherId) hydrate(form.dataset.voucherId);
}



