import { authService } from "../shared/auth-service.js";

const sidebar = document.getElementById("adminSidebar");
const shell = document.querySelector(".app-shell");
const toggle = document.getElementById("adminMenuButton");
const mobileToggle = document.querySelector("[data-menu-toggle]");
const modal = document.getElementById("logoutModal");
const accountShell = document.querySelector(".admin-account-shell");
const accountTrigger = document.getElementById("adminAccountTrigger");
const accountMenu = document.getElementById("adminAccountMenu");
const profileToggle = document.querySelector("[data-admin-profile-toggle]");
const profileSummary = document.getElementById("adminProfileSummary");
const themeToggle = document.querySelector("[data-admin-theme-toggle]");
const languageToggle = document.querySelector("[data-admin-language-toggle]");

// UI-only preferences. Theme and language are not fields in the current DB schema.
const THEME_STORAGE_KEY = "vivucar_admin_theme";
const LANGUAGE_STORAGE_KEY = "vivucar_admin_language";
const translations = {
    vi: {
        console: "Admin Console",
        dashboard: "Dashboard doanh thu",
        users: "Người dùng",
        cars: "Phương tiện",
        vouchers: "Voucher",
        moderation: "Kiểm duyệt",
        contentModeration: "Kiểm duyệt nội dung",
        licenseModeration: "Kiểm duyệt GPLX",
        reports: "Xuất báo cáo",
        trash: "Thùng rác",
        profile: "Hồ sơ",
        theme: "Giao diện",
        language: "Ngôn ngữ",
        logout: "Đăng xuất",
        role: "Quản trị viên",
        light: "Sáng",
        dark: "Tối",
        search: "Tìm kiếm...",
        accountOptions: "Tùy chọn tài khoản",
        accountTrigger: "Mở tùy chọn tài khoản",
        loggingOut: "Đang đăng xuất..."
    },
    en: {
        console: "Admin Console",
        dashboard: "Revenue dashboard",
        users: "Users",
        cars: "Vehicles",
        vouchers: "Vouchers",
        moderation: "Moderation",
        contentModeration: "Content moderation",
        licenseModeration: "License moderation",
        reports: "Export reports",
        trash: "Trash",
        profile: "Profile",
        theme: "Appearance",
        language: "Language",
        logout: "Sign out",
        role: "Administrator",
        light: "Light",
        dark: "Dark",
        search: "Search...",
        accountOptions: "Account options",
        accountTrigger: "Open account options",
        loggingOut: "Signing out..."
    }
};

let collapsed = localStorage.getItem("vivucar_admin_sidebar_collapsed") === "true";
let currentTheme = localStorage.getItem(THEME_STORAGE_KEY)
    || (matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
let currentLanguage = localStorage.getItem(LANGUAGE_STORAGE_KEY) === "en" ? "en" : "vi";
let lastFocused = null;

function desktop() {
    return matchMedia("(min-width:1024px)").matches;
}

function applySidebar() {
    if (!sidebar || !shell) return;
    const compact = desktop() && collapsed;
    const width = compact ? "76px" : "300px";
    sidebar.style.width = width;
    shell.style.paddingLeft = desktop() ? width : "";
    document.body.style.setProperty("--admin-transition-sidebar-offset", desktop() ? width : "0px");
    sidebar.querySelectorAll(".admin-nav-label,.admin-sidebar-title").forEach((element) => {
        element.style.opacity = compact ? "0" : "1";
        element.style.width = compact ? "0" : "";
        element.style.overflow = "hidden";
        element.style.pointerEvents = compact ? "none" : "";
    });
    sidebar.querySelector(".admin-sidebar-search").style.display = compact ? "none" : "";
    sidebar.querySelectorAll(".sidebar-nav a,.sidebar-nav button").forEach((element) => {
        element.style.justifyContent = compact ? "center" : "";
    });
    sidebar.querySelectorAll(".sidebar-nav .ml-auto").forEach((badge) => {
        badge.style.display = compact ? "none" : "";
    });
    if (accountTrigger) accountTrigger.style.justifyContent = compact ? "center" : "";
    document.querySelector(".admin-account-trigger-chevron")?.toggleAttribute("hidden", compact);
    toggle.style.display = compact ? "none" : "inline-grid";
    toggle.setAttribute("aria-label", compact ? "Mở rộng sidebar" : "Thu gọn sidebar");
}

function toggleSidebar() {
    closeAccountMenu();
    if (desktop()) {
        collapsed = !collapsed;
        localStorage.setItem("vivucar_admin_sidebar_collapsed", String(collapsed));
        applySidebar();
    } else {
        sidebar?.classList.toggle("is-open");
    }
}

function setAccountMenu(open, focusFirst = false) {
    if (!accountMenu || !accountTrigger) return;
    accountMenu.hidden = !open;
    accountMenu.classList.toggle("is-open", open);
    accountTrigger.setAttribute("aria-expanded", String(open));
    if (open && focusFirst) accountMenu.querySelector("button")?.focus();
}

function openAccountMenu() {
    if (desktop() && collapsed) {
        collapsed = false;
        localStorage.setItem("vivucar_admin_sidebar_collapsed", "false");
        applySidebar();
    }
    setAccountMenu(true);
}

function closeAccountMenu({ restoreFocus = false } = {}) {
    const wasOpen = accountTrigger?.getAttribute("aria-expanded") === "true";
    setAccountMenu(false);
    if (restoreFocus && wasOpen) accountTrigger?.focus();
}

function toggleAccountMenu() {
    const isOpen = accountTrigger?.getAttribute("aria-expanded") === "true";
    if (isOpen) closeAccountMenu();
    else openAccountMenu();
}

function openLogout() {
    closeAccountMenu();
    lastFocused = document.activeElement;
    modal?.classList.add("is-open");
    modal?.setAttribute("aria-hidden", "false");
    modal?.querySelector(".modal")?.focus();
}

function closeLogout() {
    modal?.classList.remove("is-open");
    modal?.setAttribute("aria-hidden", "true");
    lastFocused?.focus();
}

function applyTheme(theme, persist = true) {
    currentTheme = theme === "dark" ? "dark" : "light";
    document.documentElement.dataset.adminTheme = currentTheme;
    if (persist) localStorage.setItem(THEME_STORAGE_KEY, currentTheme);
    themeToggle?.setAttribute("aria-pressed", String(currentTheme === "dark"));
    const value = document.querySelector("[data-admin-theme-value]");
    if (value) value.textContent = translations[currentLanguage][currentTheme];
}

function applyLanguage(language, persist = true) {
    currentLanguage = language === "en" ? "en" : "vi";
    const copy = translations[currentLanguage];
    document.documentElement.lang = currentLanguage;
    if (persist) localStorage.setItem(LANGUAGE_STORAGE_KEY, currentLanguage);
    document.querySelectorAll("[data-admin-copy]").forEach((element) => {
        const value = copy[element.dataset.adminCopy];
        if (value) element.textContent = value;
    });
    const search = document.getElementById("adminPageSearch");
    if (search) search.placeholder = copy.search;
    accountMenu?.setAttribute("aria-label", copy.accountOptions);
    accountTrigger?.setAttribute("aria-label", copy.accountTrigger);
    const languageValue = document.querySelector("[data-admin-language-value]");
    if (languageValue) languageValue.textContent = currentLanguage.toUpperCase();
    applyTheme(currentTheme, false);
}

function toggleProfileSummary() {
    if (!profileSummary || !profileToggle) return;
    const willOpen = profileSummary.hidden;
    profileSummary.hidden = !willOpen;
    profileToggle.setAttribute("aria-expanded", String(willOpen));
    profileToggle.querySelector(".admin-account-item-chevron")?.classList.toggle("is-open", willOpen);
}

async function verifyAdminSession() {
    try {
        const response = await authService.apiFetch("auth/me");
        if (!response.ok) return null;
        const user = await response.json();
        return String(user?.role || "").toLowerCase() === "admin" ? user : null;
    } catch {
        return null;
    }
}

toggle?.addEventListener("click", toggleSidebar);
mobileToggle?.addEventListener("click", toggleSidebar);
window.addEventListener("resize", () => {
    closeAccountMenu();
    applySidebar();
});
document.querySelector("[data-sidebar-logo]")?.addEventListener("click", (event) => {
    if (desktop() && collapsed) {
        event.preventDefault();
        collapsed = false;
        localStorage.setItem("vivucar_admin_sidebar_collapsed", "false");
        applySidebar();
    }
});
accountTrigger?.addEventListener("click", toggleAccountMenu);
accountTrigger?.addEventListener("keydown", (event) => {
    if (event.key === "ArrowUp" || event.key === "ArrowDown") {
        event.preventDefault();
        openAccountMenu();
        accountMenu?.querySelector("button")?.focus();
    }
});
profileToggle?.addEventListener("click", toggleProfileSummary);
themeToggle?.addEventListener("click", () => applyTheme(currentTheme === "dark" ? "light" : "dark"));
languageToggle?.addEventListener("click", () => applyLanguage(currentLanguage === "vi" ? "en" : "vi"));
document.querySelectorAll("[data-logout-trigger]").forEach((button) => button.addEventListener("click", openLogout));
document.querySelectorAll("[data-close-logout]").forEach((button) => button.addEventListener("click", closeLogout));
modal?.addEventListener("click", (event) => {
    if (event.target === modal) closeLogout();
});
document.addEventListener("click", (event) => {
    if (accountShell && !accountShell.contains(event.target)) closeAccountMenu();
});
document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") return;
    if (modal?.classList.contains("is-open")) closeLogout();
    else closeAccountMenu({ restoreFocus: true });
    sidebar?.classList.remove("is-open");
});
document.getElementById("confirmLogout")?.addEventListener("click", async () => {
    const button = document.getElementById("confirmLogout");
    const previousText = button.textContent;
    button.disabled = true;
    button.textContent = translations[currentLanguage].loggingOut;
    try {
        await authService.logout();
        location.assign("/");
    } finally {
        button.disabled = false;
        button.textContent = previousText;
    }
});
document.getElementById("adminPageSearch")?.addEventListener("input", (event) => {
    document.dispatchEvent(new CustomEvent("vivucar:admin-search", { detail: event.target.value.trim() }));
});

window.VivuCarAdminAuthReady = verifyAdminSession().then((user) => {
    if (!user) {
        localStorage.removeItem("vivucar_token");
        location.replace("/");
        return null;
    }
    const name = user.fullName || user.full_name || user.email;
    const initials = name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    const panel = document.querySelector(".admin-account-shell");
    if (panel) {
        panel.querySelector(".avatar").textContent = initials;
        panel.querySelector(".truncate").textContent = name;
        panel.querySelector("[data-admin-profile-name]").textContent = name;
        panel.querySelector("[data-admin-profile-email]").textContent = user.email || "—";
    }
    return user;
});

applyTheme(currentTheme, false);
applyLanguage(currentLanguage, false);
applySidebar();