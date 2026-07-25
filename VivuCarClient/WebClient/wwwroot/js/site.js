import { authService } from "./shared/auth-service.js";

document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("login-form");
    const sessionView = document.getElementById("session-view");

    if (!loginForm || !sessionView) {
        return;
    }

    const message = document.getElementById("auth-message");
    const loginButton = document.getElementById("login-button");

    /* ── Password show/hide toggle ── */
    const passwordInput = document.getElementById("password");
    const toggleBtn = document.getElementById("password-toggle");
    const eyeOn = document.getElementById("eye-on");
    const eyeOff = document.getElementById("eye-off");

    toggleBtn.addEventListener("click", () => {
        const isPassword = passwordInput.type === "password";
        passwordInput.type = isPassword ? "text" : "password";
        eyeOn.hidden = isPassword;
        eyeOff.hidden = !isPassword;
        toggleBtn.setAttribute("aria-label",
            isPassword ? "Ẩn mật khẩu" : "Hiển thị mật khẩu");
    });

    /* ── Render / session restore ── */
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

    /* ── Inline validation ── */
    function validateField(input, errorEl, validationFn) {
        const error = validationFn(input.value);
        errorEl.textContent = error ?? "";
        input.classList.toggle("input-error", Boolean(error));
        return !error;
    }

    function validateEmail(value) {
        if (!value.trim()) return "Vui lòng nhập email.";
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) return "Email không hợp lệ.";
        return null;
    }

    function validatePassword(value) {
        if (!value.trim()) return "Vui lòng nhập mật khẩu.";
        return null;
    }

    /* ── Submit handler ── */
    loginForm.addEventListener("submit", async event => {
        event.preventDefault();

        const email = document.getElementById("email");
        const password = document.getElementById("password");
        const emailError = document.getElementById("email-error");
        const passwordError = document.getElementById("password-error");
        const btnText = document.getElementById("btn-text");
        const btnSpinner = document.getElementById("btn-spinner");
        const btnLoadingText = document.getElementById("btn-loading-text");

        // Clear previous state
        message.textContent = "";
        email.classList.remove("input-error");
        password.classList.remove("input-error");
        emailError.textContent = "";
        passwordError.textContent = "";

        // Validate all fields
        const isEmailValid = validateField(email, emailError, validateEmail);
        const isPasswordValid = validateField(password, passwordError, validatePassword);

        if (!isEmailValid || !isPasswordValid) return;

        // Loading state
        loginButton.disabled = true;
        btnText.hidden = true;
        btnSpinner.hidden = false;
        btnLoadingText.hidden = false;

        try {
            const payload = await authService.login(email.value, password.value);
            loginForm.reset();
            redirectByRole(payload?.user);
        } catch (error) {
            message.textContent = error.message;
            password.classList.add("input-error");
        } finally {
            loginButton.disabled = false;
            btnText.hidden = false;
            btnSpinner.hidden = true;
            btnLoadingText.hidden = true;
        }
    });

    /* ── Logout ── */
    document.getElementById("logout-button").addEventListener("click", async () => {
        await authService.logout();
    });

    /* ── Restore session on load ── */
    authService.refresh().then(session => {
        const user = session?.user ?? null;
        render(user);
        redirectByRole(user);
    });
});

function redirectByRole(user) {
    if (!user) return;

    const roleRoutes = {
        Admin: "/admin/dashboard",
        CarOwner: "/owner/dashboard",
        Customer: "/cars"
    };

    const destination = roleRoutes[user.role];
    if (destination && window.location.pathname.toLowerCase() !== destination.toLowerCase()) {
        window.location.href = destination;
    }
}
