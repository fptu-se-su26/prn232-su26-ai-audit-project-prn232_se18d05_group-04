/* ═══════════════════════════════════════════
   VivuCar – Register multi-step wizard
   ═══════════════════════════════════════════ */

const STEP_COUNT = 4;
const STEPPER_LABELS = ["Email", "Thông tin", "Mật khẩu", "Xác thực"];

let currentStep = 1;
let registeredEmail = "";
let registeredUserId = null;

document.addEventListener("DOMContentLoaded", () => {
    populateDobSelects();
    initPasswordToggle();
    initStepNavigation();
    initPasswordStrength();
    initOtpInputs();
    goToStep(1);
});

/* ── DOB selects ── */

function populateDobSelects() {
    const daySelect = document.getElementById("reg-dob-day");
    const monthSelect = document.getElementById("reg-dob-month");
    const yearSelect = document.getElementById("reg-dob-year");

    for (let d = 1; d <= 31; d++) {
        daySelect.add(new Option(d, d));
    }
    for (let m = 1; m <= 12; m++) {
        monthSelect.add(new Option(m, m));
    }
    const now = new Date();
    const currentYear = now.getFullYear();
    for (let y = currentYear - 100; y <= currentYear - 13; y++) {
        yearSelect.add(new Option(y, y));
    }
}

/* ── Password toggle ── */

function initPasswordToggle() {
    const passwordInput = document.getElementById("reg-password");
    const toggleBtn = document.getElementById("reg-password-toggle");
    const eyeOn = document.getElementById("reg-eye-on");
    const eyeOff = document.getElementById("reg-eye-off");

    toggleBtn.addEventListener("click", () => {
        const isPassword = passwordInput.type === "password";
        passwordInput.type = isPassword ? "text" : "password";
        eyeOn.hidden = isPassword;
        eyeOff.hidden = !isPassword;
        toggleBtn.setAttribute("aria-label",
            isPassword ? "Ẩn mật khẩu" : "Hiển thị mật khẩu");
    });
}

/* ── Step navigation ── */

function initStepNavigation() {
    document.querySelectorAll("[data-action]").forEach(btn => {
        btn.addEventListener("click", () => {
            const action = btn.dataset.action;
            if (action === "next") handleNext();
            else if (action === "back") handleBack();
            else if (action === "create") handleCreate();
            else if (action === "verify") handleVerify();
        });
    });

    // Enter key on step 1 and 2 fields triggers next
    ["reg-email", "reg-fullname", "reg-phone"].forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener("keydown", e => {
                if (e.key === "Enter") handleNext();
            });
        }
    });

    // Enter key on password field triggers create
    const pwd = document.getElementById("reg-password");
    if (pwd) {
        pwd.addEventListener("keydown", e => {
            if (e.key === "Enter") handleCreate();
        });
    }
}

function handleNext() {
    if (currentStep === 1 && !validateStep1()) return;
    if (currentStep === 2 && !validateStep2()) return;
    goToStep(currentStep + 1);
}

function handleBack() {
    goToStep(currentStep - 1);
}

async function handleCreate() {
    if (!validateStep3()) return;

    const btn = document.getElementById("reg-create-btn");
    const text = document.getElementById("reg-create-text");
    const spinner = document.getElementById("reg-create-spinner");

    btn.disabled = true;
    text.hidden = true;
    spinner.hidden = false;

    try {
        const payload = {
            email: document.getElementById("reg-email").value.trim(),
            password: document.getElementById("reg-password").value,
            fullName: document.getElementById("reg-fullname").value.trim(),
            phoneNumber: document.getElementById("reg-phone").value.trim(),
            dateOfBirth: getDobValue()
        };

        const response = await fetch("/api/auth/register", {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const result = await response.json();

        if (!response.ok) {
            showGlobalError(result.message || result.Message || "Đăng ký thất bại.");
            return;
        }

        registeredEmail = payload.email;
        registeredUserId = result.userId;
        document.getElementById("reg-otp-email").textContent = registeredEmail;
        hideGlobalError();
        goToStep(4);

        // Auto-focus first OTP input
        const firstOtp = document.querySelector(".reg-otp-input[data-otp='0']");
        if (firstOtp) firstOtp.focus();
    } catch (err) {
        showGlobalError("Lỗi kết nối. Vui lòng thử lại.");
    } finally {
        btn.disabled = false;
        text.hidden = false;
        spinner.hidden = true;
    }
}

async function handleVerify() {
    const code = getOtpCode();
    if (code.length !== 6) {
        showGlobalError("Vui lòng nhập đủ 6 chữ số.");
        return;
    }

    const btn = document.getElementById("reg-verify-btn");
    const text = document.getElementById("reg-verify-text");
    const spinner = document.getElementById("reg-verify-spinner");

    btn.disabled = true;
    text.hidden = true;
    spinner.hidden = false;
    hideGlobalError();

    try {
        const response = await fetch("/api/auth/verify-otp", {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: registeredEmail, code })
        });

        const result = await response.json();

        if (!response.ok) {
            showGlobalError(result.message || result.Message || "Xác thực thất bại.");
            return;
        }

        // Auto-login — store token and redirect
        if (result.accessToken) {
            localStorage.setItem("vivucar_token", result.accessToken);
            redirectByRole(result.user);
        }
    } catch (err) {
        showGlobalError("Lỗi kết nối. Vui lòng thử lại.");
    } finally {
        btn.disabled = false;
        text.hidden = false;
        spinner.hidden = true;
    }
}

/* ── Step transition ── */

function goToStep(step) {
    if (step < 1 || step > STEP_COUNT) return;

    const direction = step > currentStep ? "forward" : "back";
    currentStep = step;

    document.querySelectorAll(".register-step").forEach(el => {
        el.classList.remove("is-active", "is-slide-forward", "is-slide-back");
    });

    const activeEl = document.querySelector(`.register-step[data-step="${step}"]`);
    if (activeEl) {
        activeEl.classList.add("is-active");
        activeEl.classList.add(direction === "forward" ? "is-slide-forward" : "is-slide-back");
    }

    updateStepper(step);
}

function updateStepper(step) {
    document.querySelectorAll(".reg-step-dot").forEach(dot => {
        const dotStep = parseInt(dot.dataset.step);
        dot.classList.remove("is-active", "is-done");
        if (dotStep === step) dot.classList.add("is-active");
        else if (dotStep < step) dot.classList.add("is-done");
        dot.textContent = dotStep < step ? "✓" : dotStep;
    });

    document.querySelectorAll(".reg-step-connector").forEach(conn => {
        const connStep = parseInt(conn.dataset.connector);
        conn.classList.toggle("is-done", connStep < step);
    });
}

/* ── Validation per step ── */

function showFieldError(inputId, errorId, message) {
    const input = document.getElementById(inputId);
    const error = document.getElementById(errorId);
    input.classList.add("input-error");
    error.textContent = message;
}

function clearFieldError(inputId, errorId) {
    const input = document.getElementById(inputId);
    const error = document.getElementById(errorId);
    input.classList.remove("input-error");
    error.textContent = "";
}

function validateStep1() {
    const email = document.getElementById("reg-email").value.trim();
    clearFieldError("reg-email", "reg-email-error");

    if (!email) {
        showFieldError("reg-email", "reg-email-error", "Vui lòng nhập email.");
        return false;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
        showFieldError("reg-email", "reg-email-error", "Email không hợp lệ.");
        return false;
    }
    return true;
}

function validateStep2() {
    const fullname = document.getElementById("reg-fullname").value.trim();
    const phone = document.getElementById("reg-phone").value.trim();

    clearFieldError("reg-fullname", "reg-fullname-error");
    clearFieldError("reg-phone", "reg-phone-error");
    clearFieldError("reg-dob-day", "reg-dob-error");

    let valid = true;
    if (!fullname) {
        showFieldError("reg-fullname", "reg-fullname-error", "Vui lòng nhập họ tên.");
        valid = false;
    }
    if (!phone) {
        showFieldError("reg-phone", "reg-phone-error", "Vui lòng nhập số điện thoại.");
        valid = false;
    } else if (!/^0\d{8,10}$/.test(phone.replace(/\s/g, ""))) {
        showFieldError("reg-phone", "reg-phone-error", "Số điện thoại không hợp lệ (bắt đầu bằng 0, 9-11 số).");
        valid = false;
    }

    return valid;
}

function validateStep3() {
    const password = document.getElementById("reg-password").value;
    clearFieldError("reg-password", "reg-password-error");

    if (!password) {
        showFieldError("reg-password", "reg-password-error", "Vui lòng nhập mật khẩu.");
        return false;
    }
    if (password.length < 8) {
        showFieldError("reg-password", "reg-password-error", "Mật khẩu phải có ít nhất 8 ký tự.");
        return false;
    }
    if (!evaluatePassword(password).allPassed) {
        showFieldError("reg-password", "reg-password-error", "Mật khẩu chưa đáp ứng tất cả yêu cầu.");
        return false;
    }
    return true;
}

/* ── Global error ── */

function showGlobalError(msg) {
    const el = document.getElementById("reg-global-error");
    el.textContent = msg;
    el.hidden = false;
}

function hideGlobalError() {
    const el = document.getElementById("reg-global-error");
    el.hidden = true;
    el.textContent = "";
}

/* ── DOB helpers ── */

function getDobValue() {
    const day = document.getElementById("reg-dob-day").value;
    const month = document.getElementById("reg-dob-month").value;
    const year = document.getElementById("reg-dob-year").value;
    if (!day || !month || !year) return null;
    const monthPadded = month.padStart(2, "0");
    const dayPadded = day.padStart(2, "0");
    return `${year}-${monthPadded}-${dayPadded}`;
}

/* ── Password strength ── */

function initPasswordStrength() {
    const pwdInput = document.getElementById("reg-password");
    pwdInput.addEventListener("input", () => {
        const result = evaluatePassword(pwdInput.value);
        updateStrengthUI(result);
    });
}

function evaluatePassword(password) {
    const checks = {
        length: password.length >= 8,
        upper: /[A-Z]/.test(password),
        lower: /[a-z]/.test(password),
        digit: /\d/.test(password),
        special: /[^A-Za-z0-9]/.test(password)
    };
    const score = Object.values(checks).filter(Boolean).length;
    const allPassed = score === 5;

    const labels = ["", "Rất yếu", "Yếu", "Trung bình", "Mạnh", "Rất mạnh"];
    return { checks, score, allPassed, label: labels[score] };
}

function updateStrengthUI(result) {
    document.querySelectorAll(".reg-strength-bar").forEach((bar, i) => {
        bar.classList.toggle("is-filled", i < result.score);
        bar.classList.remove("is-score-1", "is-score-2", "is-score-3", "is-score-4", "is-score-5");
        if (i < result.score) {
            bar.classList.add(`is-score-${result.score}`);
        }
    });

    const label = document.getElementById("reg-strength-label");
    label.textContent = result.label;
    label.style.color = ["", "#ef4444", "#f97316", "#eab308", "#22c55e", "#16a34a"][result.score];

    document.querySelectorAll(".reg-strength-check").forEach(check => {
        const key = check.dataset.check;
        if (result.checks[key]) {
            check.classList.add("is-pass");
            check.innerHTML = "&#10003; " + check.textContent.replace("● ", "");
        } else {
            check.classList.remove("is-pass");
            check.innerHTML = "&#9679; " + check.textContent.replace("✓ ", "").replace("● ", "");
        }
    });
}

/* ── OTP inputs ── */

function initOtpInputs() {
    const inputs = document.querySelectorAll(".reg-otp-input");

    inputs.forEach((input, idx) => {
        input.addEventListener("input", (e) => {
            const value = e.target.value;

            // Allow only single digit
            if (value && /^\d$/.test(value)) {
                input.classList.add("is-filled");
                // Auto-advance
                if (idx < inputs.length - 1) {
                    inputs[idx + 1].focus();
                }
            } else if (value) {
                // Non-digit, clear
                input.value = "";
                input.classList.remove("is-filled");
            }
        });

        input.addEventListener("keydown", (e) => {
            if (e.key === "Backspace" && !input.value && idx > 0) {
                inputs[idx - 1].focus();
                inputs[idx - 1].classList.remove("is-filled");
            }
        });

        input.addEventListener("paste", (e) => {
            e.preventDefault();
            const pasteData = (e.clipboardData || window.clipboardData).getData("text").trim();
            if (/^\d{6}$/.test(pasteData)) {
                [...pasteData].forEach((char, i) => {
                    if (inputs[i]) {
                        inputs[i].value = char;
                        inputs[i].classList.add("is-filled");
                    }
                });
                inputs[Math.min(5, pasteData.length - 1)].focus();
            }
        });
    });
}

function getOtpCode() {
    return Array.from(document.querySelectorAll(".reg-otp-input"))
        .map(input => input.value)
        .join("");
}

/* ── Redirect after auto-login ── */

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
