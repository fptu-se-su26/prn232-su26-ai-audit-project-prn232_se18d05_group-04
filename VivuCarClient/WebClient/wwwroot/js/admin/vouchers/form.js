import { saveVoucher } from "./voucher-api.js";
import { showToast } from "../../shared/toast.js";

const form = document.querySelector("[data-admin-voucher-form]");

function collectVoucher() {
    const formData = new FormData(form);
    return {
        name: String(formData.get("name") ?? "").trim(),
        code: String(formData.get("code") ?? "").trim().toUpperCase(),
        discount_type: String(formData.get("discount_type") ?? "percentage"),
        discount_value: Number(formData.get("discount_value") ?? 0),
        min_order_amount: Number(formData.get("min_order_amount") ?? 0),
        max_discount: Number(formData.get("max_discount") ?? 0),
        quantity: Number(formData.get("quantity") ?? 0),
        expires_at: String(formData.get("expires_at") ?? "") || null
    };
}

function initVoucherPreview() {
    const voucherCode = document.getElementById("code");
    const voucherName = document.getElementById("name");
    const voucherValue = document.getElementById("discount_value");
    const voucherPreviewCode = document.getElementById("voucherPreviewCode");
    const voucherPreviewName = document.getElementById("voucherPreviewName");
    const voucherPreviewValue = document.getElementById("voucherPreviewValue");

    function syncVoucherPreview() {
        voucherPreviewCode.textContent = (voucherCode?.value || "SUMMER2026").toUpperCase();
        voucherPreviewName.textContent = voucherName?.value || "Voucher name";
        voucherPreviewValue.textContent = voucherValue?.value ? `Discount ${voucherValue.value}` : "Discount";
    }

    [voucherCode, voucherName, voucherValue].forEach(input => input?.addEventListener("input", syncVoucherPreview));
    voucherCode?.addEventListener("input", () => {
        voucherCode.value = voucherCode.value.toUpperCase().replace(/\s+/g, "");
    });
    syncVoucherPreview();
}

async function submitVoucher(event) {
    event.preventDefault();
    const button = document.getElementById("btnCreateVoucher");
    button.disabled = true;

    try {
        await saveVoucher(collectVoucher(), form.dataset.voucherId || null);
        showToast("Ðã luu voucher thành công.", "success");
        window.location.href = "/admin/vouchers";
    } catch (error) {
        showToast(error.message, "error");
    } finally {
        button.disabled = false;
    }
}

if (form) {
    initVoucherPreview();
    form.addEventListener("submit", submitVoucher);
}