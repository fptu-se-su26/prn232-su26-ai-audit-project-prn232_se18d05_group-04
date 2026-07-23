let accessToken = localStorage.getItem("vivucar_token");
let currentUser = null;
let refreshPromise = null;

async function readJson(response) {
    const text = await response.text();
    return text ? JSON.parse(text) : null;
}

function setSession(session) {
    accessToken = session?.accessToken ?? null;
    currentUser = session?.user ?? null;
    
    if (accessToken) {
        localStorage.setItem('vivucar_token', accessToken);
    } else {
        localStorage.removeItem('vivucar_token');
    }

    document.dispatchEvent(
        new CustomEvent("vivucar:auth-changed", {
            detail: currentUser
        })
    );
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
                    setSession(null);
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

async function apiFetch(path, options = {}, allowRefresh = true) {
    const headers = new Headers(options.headers ?? {});

    if (accessToken) {
        headers.set("Authorization", `Bearer ${accessToken}`);
    }

    const response = await fetch(`/api/proxy/${path.replace(/^\/+/, "")}`, {
        ...options,
        headers,
        credentials: "same-origin"
    });

    if (response.status !== 401 || !allowRefresh) {
        return response;
    }

    const session = await refresh();

    if (!session) {
        return response;
    }

    return apiFetch(path, options, false);
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
    apiFetch,
    logout,
    getUser: () => currentUser
};