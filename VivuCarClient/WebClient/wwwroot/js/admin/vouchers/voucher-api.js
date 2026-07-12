import { apiFetch, fetchJson, sendJson } from "../../shared/api-client.js";

export function getVouchers(params = {}) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => { if (value !== "" && value != null) query.set(key, value); });
    return fetchJson(`api/admin/vouchers${query}`);
}
export function getVoucher(id) { return fetchJson(`api/admin/vouchers/${id}`); }
export function getVoucherPerformance(id) { return fetchJson(`api/admin/vouchers/${id}/performance`); }
export function saveVoucher(data, id) { return sendJson(id ? `api/admin/vouchers/${id}` : "api/admin/vouchers", data, { method: id ? "PUT" : "POST" }); }
export async function deleteVoucher(id) {
    const response = await apiFetch(`api/admin/vouchers/${id}`, { method: "DELETE" });
    if (!response.ok) { const payload = await response.json().catch(() => null); throw new Error(payload.message || "Khong th xa voucher."); }
}



