let accessToken = localStorage.getItem("vivucar_token");
let tokenExpiresAt = null;
try {
    const stored = localStorage.getItem("vivucar_token_exp");
    if (stored) tokenExpiresAt = new Date(stored);
} catch {}
let currentUser = null;
let refreshPromise = null;

async function readJson(response) {
    const text = await response.text();
    return text ? JSON.parse(text) : null;
}

function decodeJwtPayload(token) {
    let base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    while (base64.length % 4) base64 += "=";
    
    // Properly decode UTF-8 to avoid JSON.parse throwing exceptions on Vietnamese names
    const binaryString = atob(base64);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    const decoder = new TextDecoder('utf-8');
    const jsonString = decoder.decode(bytes);
    
    return JSON.parse(jsonString);
}

function restoreSessionFromStorage() {
    if (!accessToken) return;

    try {
        const payload = decodeJwtPayload(accessToken);
        tokenExpiresAt = new Date(payload.exp * 1000);
        localStorage.setItem("vivucar_token_exp", tokenExpiresAt.toISOString());

        if (!currentUser) {
            currentUser = {
                id: Number.parseInt(payload.sub, 10),
                email: payload.email ?? "",
                fullName: payload.fullName ?? payload.name ?? "",
                role: payload.role
                    ?? payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
                    ?? ""
            };
        }
    } catch {}
}

restoreSessionFromStorage();

function setSession(session) {
    accessToken = session?.accessToken ?? null;
    currentUser = session?.user ?? null;
    tokenExpiresAt = session?.expiresAt ? new Date(session.expiresAt) : null;

    if (accessToken) {
        localStorage.setItem("vivucar_token", accessToken);
        try {
            const payload = decodeJwtPayload(accessToken);
            tokenExpiresAt = new Date(payload.exp * 1000);
            localStorage.setItem("vivucar_token_exp", tokenExpiresAt.toISOString());
        } catch {}
    } else {
        localStorage.removeItem("vivucar_token");
        localStorage.removeItem("vivucar_token_exp");
    }

    document.dispatchEvent(
        new CustomEvent("vivucar:auth-changed", {
            detail: currentUser
        })
    );
}

/** Returns true if the current access token is missing or within 60s of expiry */
function isTokenExpiredOrNearExpiry() {
    if (!accessToken) return true;
    if (!tokenExpiresAt) return false;
    return tokenExpiresAt.getTime() - Date.now() < 60_000;
}

async function login(email, password) {
    const response = await fetch("/api/auth/login", {
        method: "POST",
        credentials: "same-origin",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });
    const payload = await readJson(response);

    if (!response.ok) {
        throw new Error(payload?.message ?? "Sign-in failed.");
    }

    setSession(payload);
    return payload;
}

async function refresh() {
    if (!refreshPromise) {
        refreshPromise = fetch("/api/auth/refresh", {
            method: "POST",
            credentials: "same-origin"
        })
            .then(async response => {
                const payload = await readJson(response);

                if (!response.ok) {
                    if (isTokenExpiredOrNearExpiry()) {
                        setSession(null);
                    }
                    return null;
                }

                setSession(payload);
                return payload;
            })
            .finally(() => {
                refreshPromise = null;
            });
    }

    return refreshPromise;
}

/** Only refresh when token is expired or near expiry; otherwise return current session */
async function getValidSession() {
    restoreSessionFromStorage();

    if (!accessToken) {
        return refresh();
    }

    if (!isTokenExpiredOrNearExpiry()) {
        return { user: currentUser, accessToken };
    }

    return refresh();
}

/** Dummy Response for navigation-aborted fetches — .ok = false, .status = 0. */
function cancelledResponse() {
    return { ok: false, status: 0, statusText: "Cancelled", headers: new Headers(), redirected: false, type: "basic", url: "", json: () => Promise.reject("aborted"), text: () => Promise.resolve(""), blob: () => Promise.reject("aborted"), arrayBuffer: () => Promise.reject("aborted"), clone() { return this; } };
}

async function apiFetch(path, options = {}, allowRefresh = true) {
    restoreSessionFromStorage();

    const isFormData = typeof FormData !== "undefined" && options.body instanceof FormData;
    const headers = new Headers(options.headers ?? {});

    if (accessToken) {
        headers.set("Authorization", `Bearer ${accessToken}`);
    }

    // Browser must set multipart boundary; a manual Content-Type breaks uploads.
    if (isFormData) {
        headers.delete("Content-Type");
    }

    let response;
    try {
        response = await fetch(`/api/proxy/${path.replace(/^\/+/, "")}`, {
            ...options,
            headers,
            credentials: "same-origin"
        });
    } catch (err) {
        // Navigation aborts in-flight fetches (AbortError / TypeError).
        // Return cancelled response so callers don't crash on .ok / .status.
        if (err instanceof DOMException && err.name === "AbortError") return cancelledResponse();
        if (err instanceof TypeError && err.message?.includes("abort")) return cancelledResponse();
        throw err;
    }

    // FormData streams cannot be replayed after a 401 refresh retry.
    if (response.status !== 401 || !allowRefresh || isFormData) {
        return patchResponse(response);
    }

    const session = await refresh();

    if (!session) {
        return patchResponse(response);
    }

    return apiFetch(path, options, false);
}

function patchResponse(response) {
    if (!response || !response.json) return response;
    const originalJson = response.json.bind(response);
    response.json = async function () {
        const obj = await originalJson();
        normalizeStatus(obj);
        return obj;
    };
    return response;
}

function normalizeStatus(o) {
    if (Array.isArray(o)) {
        for (let i = 0; i < o.length; i++) normalizeStatus(o[i]);
    } else if (o && typeof o === 'object') {
        if (typeof o.status === 'string') {
            const ls = o.status.toLowerCase();
            if (ls === "pendingapproval" || ls === "waitingdeposit") o.status = "pending";
            else if (ls === "waitingpickup" || ls === "inprogress" || ls === "returnrequested") o.status = "approved";
            else if (ls === "completed") o.status = "completed";
            else if (ls === "cancelled" || ls === "expired") o.status = "cancelled";
            else if (ls === "rejected") o.status = "rejected";
            else o.status = ls;
        }
        for (const key in o) {
            if (Object.prototype.hasOwnProperty.call(o, key)) {
                normalizeStatus(o[key]);
            }
        }
    }
}

async function logout() {
    try {
        await fetch("/api/auth/logout", {
            method: "POST",
            credentials: "same-origin"
        });
    } finally {
        setSession(null);
    }
}

export const authService = {
    login,
    refresh,
    getValidSession,
    apiFetch,
    logout,
    getUser: () => currentUser
};
