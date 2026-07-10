import { getVouchers } from "./voucher-api.js";
import { escapeHtml } from "../../shared/dom.js";
import { formatVnd } from "../../shared/utils.js";

const root = document.querySelector("[data-admin-vouchers-page]");
const state = { vouchers: [] };

function normalize(item) {
    return {
        id: item.id ?? item.Id,
        code: item.code ?? item.Code ?? "",
        name: item.name ?? item.Name ?? "",
        discount_type: item.discount_type ?? item.discountType ?? item.DiscountType ?? "",
        discount_value: item.discount_value ?? item.discountValue ?? item.DiscountValue ?? 0,
        min_order_amount: item.min_order_amount ?? item.minOrderAmount ?? item.MinOrderAmount ?? 0,
        quantity: item.quantity ?? item.Quantity ?? 0,
        expires_at: item.expires_at ?? item.expiresAt ?? item.ExpiresAt ?? ""
    };
}

function filtered() {
    const keyword = document.getElementById("searchVoucherInput")?.value.trim().toLowerCase() ?? "";
    const type = document.getElementById("discountTypeFilter")?.value ?? "";
    return state.vouchers.filter(voucher => {
        const matchesKeyword = !keyword || voucher.code.toLowerCase().includes(keyword) || voucher.name.toLowerCase().includes(keyword);
        const matchesType = !type || voucher.discount_type === type;
        return matchesKeyword && matchesType;
    });
}

function render() {
    const body = document.getElementById("vouchersTableBody");
    const vouchers = filtered();
    if (!body) return;

    if (!vouchers.length) {
        body.innerHTML = `<tr><td colspan="8" class="empty-cell">Chua có voucher phù h?p.</td></tr>`;
        return;
    }

    body.innerHTML = vouchers.map(voucher => `<tr>
        <td>${escapeHtml(voucher.code)}</td>
        <td>${escapeHtml(voucher.name)}</td>
        <td>${escapeHtml(voucher.discount_type)}</td>
        <td>${escapeHtml(voucher.discount_type === "fixed" ? formatVnd(voucher.discount_value) : `${voucher.discount_value}%`)}</td>
        <td>${escapeHtml(formatVnd(voucher.min_order_amount))}</td>
        <td>${escapeHtml(voucher.quantity)}</td>
        <td>${escapeHtml(voucher.expires_at || "-")}</td>
        <td><a class="button button-secondary" href="/admin/voucher-form/${encodeURIComponent(voucher.id)}">S?a</a></td>
    </tr>`).join("");
}

async function loadVouchers() {
    const body = document.getElementById("vouchersTableBody");
    if (body) body.innerHTML = `<tr><td colspan="8" class="empty-cell">Ðang t?i voucher...</td></tr>`;

    try {
        const payload = await getVouchers();
        const items = Array.isArray(payload) ? payload : payload.items ?? payload.Items ?? [];
        state.vouchers = items.map(normalize);
        render();
    } catch (error) {
        if (body) body.innerHTML = `<tr><td colspan="8" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
    }
}

if (root) {
    document.getElementById("btnFilterVoucher")?.addEventListener("click", render);
    document.getElementById("btnResetVoucherFilter")?.addEventListener("click", () => {
        document.getElementById("searchVoucherInput").value = "";
        document.getElementById("discountTypeFilter").value = "";
        render();
    });
    loadVouchers();
}