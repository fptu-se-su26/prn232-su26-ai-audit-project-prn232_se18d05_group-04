import { apiFetch, fetchJson } from "../../shared/api-client.js";

export async function getUsers() {
    return fetchJson("api/admin/users");
}

async function ensureOk(response, fallbackMessage) {
    if (response.ok) return;

    let message = fallbackMessage;
    const text = await response.text();

    if (text) {
        try {
            const payload = JSON.parse(text);
            message = payload?.message ?? payload?.title ?? text;
        } catch {
            message = text;
        }
    }

    throw new Error(message);
}

export async function lockUser(userId) {
    const response = await apiFetch(`api/admin/users/${userId}/lock`, { method: "PATCH" });
    await ensureOk(response, "Unable to lock this user.");
}

export async function unlockUser(userId) {
    const response = await apiFetch(`api/admin/users/${userId}/unlock`, { method: "PATCH" });
    await ensureOk(response, "Unable to unlock this user.");
}

export async function softDeleteCustomer(userId) {
    const response = await apiFetch("api/admin/users/customers/" + userId, { method: "DELETE" });
    await ensureOk(response, "Unable to soft-delete this customer.");
}
