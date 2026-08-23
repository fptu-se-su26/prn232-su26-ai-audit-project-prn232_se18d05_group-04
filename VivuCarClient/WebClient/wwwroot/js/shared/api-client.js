import { authService } from "./auth-service.js";

async function parseError(response) {
    const text = await response.text();

    if (!text) {
        return `Request failed with status ${response.status}.`;
    }

    try {
        const payload = JSON.parse(text);
        return payload?.message ?? payload?.title ?? text;
    } catch {
        return text;
    }
}

export async function apiFetch(path, options = {}) {
    return authService.apiFetch(path, options);
}

export async function fetchJson(path, options = {}) {
    const response = await apiFetch(path, {
        ...options,
        headers: {
            Accept: "application/json",
            ...(options.headers ?? {})
        }
    });

    if (!response.ok) {
        throw new Error(await parseError(response));
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

export async function sendJson(path, data, options = {}) {
    return fetchJson(path, {
        ...options,
        method: options.method ?? "POST",
        headers: {
            "Content-Type": "application/json",
            ...(options.headers ?? {})
        },
        body: JSON.stringify(data)
    });
}