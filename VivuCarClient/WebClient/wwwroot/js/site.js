window.vivuCarAuth = (() => {
    let accessToken = null;
    let currentUser = null;
    let refreshPromise = null;

    async function readJson(response) {
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    }

    function setSession(session) {
        accessToken = session?.accessToken ?? null;
        currentUser = session?.user ?? null;
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
            throw new Error(payload?.message ?? "Đăng nhập không thành công.");
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

    return {
        login,
        refresh,
        apiFetch,
        logout,
        getUser: () => currentUser
    };
})();

document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("login-form");
    const sessionView = document.getElementById("session-view");

    if (!loginForm || !sessionView) {
        return;
    }

    const message = document.getElementById("auth-message");
    const loginButton = document.getElementById("login-button");

    function render(user) {
        loginForm.hidden = Boolean(user);
        sessionView.hidden = !user;

        if (user) {
            document.getElementById("user-name").textContent = user.fullName;
            document.getElementById("user-email").textContent = user.email;
            document.getElementById("user-role").textContent = user.role;
        }
    }

    document.addEventListener("vivucar:auth-changed", event => {
        render(event.detail);
    });

    loginForm.addEventListener("submit", async event => {
        event.preventDefault();
        loginButton.disabled = true;
        message.textContent = "";

        try {
            const formData = new FormData(loginForm);
            await window.vivuCarAuth.login(
                formData.get("email"),
                formData.get("password")
            );
            loginForm.reset();
        } catch (error) {
            message.textContent = error.message;
        } finally {
            loginButton.disabled = false;
        }
    });

    document.getElementById("logout-button").addEventListener("click", async () => {
        await window.vivuCarAuth.logout();
    });

    window.vivuCarAuth.refresh().then(session => render(session?.user ?? null));
});
