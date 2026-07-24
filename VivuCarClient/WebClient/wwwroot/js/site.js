import { authService } from "./shared/auth-service.js";

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
            const payload = await authService.login(
                formData.get("email"),
                formData.get("password")
            );
            loginForm.reset();
            redirectByRole(payload?.user);
        } catch (error) {
            message.textContent = error.message;
        } finally {
            loginButton.disabled = false;
        }
    });

    document.getElementById("logout-button").addEventListener("click", async () => {
        await authService.logout();
    });

    authService.refresh().then(session => {
        const user = session?.user ?? null;
        render(user);
        redirectByRole(user);
    });
});

function redirectByRole(user) {
    if (!user) return;

    // Honor explicit redirect param first (e.g. from checkout when session expired)
    const redirectParam = new URLSearchParams(window.location.search).get('redirect');
    if (redirectParam && redirectParam.startsWith('/')) {
        window.location.href = redirectParam;
        return;
    }

    const roleRoutes = {
        Admin: "/admin/dashboard",
        CarOwner: "/owner/dashboard",
        Customer: "/home"
    };

    const destination = roleRoutes[user.role];
    if (destination && window.location.pathname.toLowerCase() !== destination.toLowerCase()) {
        window.location.href = destination;
    }
}