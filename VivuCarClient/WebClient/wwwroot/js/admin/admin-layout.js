import { authService } from "../shared/auth-service.js";

const sidebar = document.getElementById("adminSidebar");
const menuButton = document.getElementById("adminMenuButton");
const modal = document.getElementById("logoutModal");
const modalCard = modal?.querySelector(".modal");
let lastFocused = null;

function setSidebar(open) {
    sidebar?.classList.toggle("is-open", open);
    menuButton?.setAttribute("aria-expanded", String(open));
}
function openLogout() {
    lastFocused = document.activeElement;
    modal?.classList.add("is-open");
    modal?.setAttribute("aria-hidden", "false");
    modalCard?.focus();
}
function closeLogout() {
    modal?.classList.remove("is-open");
    modal?.setAttribute("aria-hidden", "true");
    lastFocused?.focus();
}
menuButton?.addEventListener("click", () => setSidebar(!sidebar.classList.contains("is-open")));
document.querySelector("[data-menu-toggle]")?.addEventListener("click", () => setSidebar(!sidebar.classList.contains("is-open")));
document.querySelector("[data-logout-trigger]")?.addEventListener("click", openLogout);
document.querySelectorAll("[data-close-logout]").forEach(button => button.addEventListener("click", closeLogout));
modal?.addEventListener("click", event => { if (event.target === modal) closeLogout(); });
document.addEventListener("keydown", event => { if (event.key === "Escape") { closeLogout(); setSidebar(false); } });
document.getElementById("confirmLogout")?.addEventListener("click", async () => {
    const button = document.getElementById("confirmLogout");
    button.disabled = true;
    try { await authService.logout(); window.location.assign("/"); }
    finally { button.disabled = false; }
});
document.getElementById("adminPageSearch")?.addEventListener("input", event => {
    document.dispatchEvent(new CustomEvent("vivucar:admin-search", { detail: event.target.value.trim() }));
});

