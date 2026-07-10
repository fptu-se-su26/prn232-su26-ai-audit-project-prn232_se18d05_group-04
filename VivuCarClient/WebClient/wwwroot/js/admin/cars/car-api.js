import { fetchJson, apiFetch, sendJson } from "../../shared/api-client.js";
import { mapCarListResponse, mapCarResponse } from "./car-mapper.js";

async function ensureOk(response, fallbackMessage) {
    if (response.ok) return;

    let message = fallbackMessage;
    const text = await response.text();

    if (text) {
        try {
            const payload = JSON.parse(text);
            const fieldErrors = payload?.errors
                ? Object.values(payload.errors).flat().join(" ")
                : "";
            message = [payload?.message, fieldErrors].filter(Boolean).join(" ") || payload?.title || payload?.detail || text;
        } catch {
            message = text;
        }
    }

    throw new Error(message);
}

export async function getCars(params = {}) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== "") query.set(key, value);
    });

    const payload = await fetchJson(`api/admin/cars${query.size ? `?${query}` : ""}`);
    return mapCarListResponse(payload);
}

export async function getCarById(id) {
    const payload = await fetchJson(`api/admin/cars/${id}`);
    return mapCarResponse(payload);
}

export async function createCar(data) {
    const payload = await sendJson("api/admin/cars", data);
    return mapCarResponse(payload);
}

export async function updateCar(id, data) {
    const payload = await sendJson(`api/admin/cars/${id}`, data, { method: "PUT" });
    return mapCarResponse(payload);
}

export async function blockCar(id, blocked_reason) {
    const payload = await sendJson(`api/admin/cars/${id}/block`, { blocked_reason }, { method: "PATCH" });
    return mapCarResponse(payload);
}

export async function unblockCar(id, target_status = "available") {
    const payload = await sendJson(`api/admin/cars/${id}/unblock`, { target_status }, { method: "PATCH" });
    return mapCarResponse(payload);
}

export async function uploadCarImages(id, files) {
    if (!files.length) return [];

    const formData = new FormData();
    files.forEach(file => formData.append("files", file, file.name));
    formData.append("isPrimary", "false");

    const response = await apiFetch(`api/admin/cars/${id}/images`, {
        method: "POST",
        body: formData
    });
    await ensureOk(response, "Unable to upload car images.");
    return response.status === 204 ? [] : response.json();
}

export async function getCarTypes() {
    return fetchJson("api/car-types");
}

export async function getCarModels() {
    return fetchJson("api/car-models");
}

export async function getCarOwners() {
    const users = await fetchJson("api/admin/users?role=car_owner");
    return users.map(user => ({
        id: user.id ?? user.Id,
        full_name: user.full_name ?? user.fullName ?? user.FullName ?? user.email ?? user.Email ?? "Owner",
        email: user.email ?? user.Email ?? ""
    }));
}
