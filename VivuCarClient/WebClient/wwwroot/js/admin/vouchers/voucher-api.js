import { apiFetch, fetchJson, sendJson } from "../../shared/api-client.js";

export async function getVouchers(params = {}) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
        if (value) query.set(key, value);
    });
    return fetchJson(`api/admin/vouchers${query.size ? `?${query}` : ""}`);
}

export async function saveVoucher(data, id = null) {
    return sendJson(id ? `api/admin/vouchers/${id}` : "api/admin/vouchers", data, { method: id ? "PUT" : "POST" });
}

export async function deleteVoucher(id) {
    const response = await apiFetch(`api/admin/vouchers/${id}`, { method: "DELETE" });
    if (!response.ok) throw new Error(`Unable to delete voucher ${id}.`);
}