import { openModal, bindGlobalModalEvents } from "../shared/modal.js";

function setActiveNavigation() {
    const page = document.body.dataset.adminPage;

    document.querySelectorAll(".admin-nav a").forEach(link => {
        link.classList.toggle("is-active", link.dataset.nav === page);
    });
}

function bindSidebarToggle() {
    document.querySelector("[data-admin-menu]")?.addEventListener("click", () => {
        document.querySelector(".admin-sidebar")?.classList.toggle("is-open");
    });
}

function bindLogoutModal() {
    document.addEventListener("click", event => {
        if (event.target.closest("[data-open-logout]")) {
            openModal("logoutModal");
        }
    });
}

function init() {
    setActiveNavigation();
    bindSidebarToggle();
    bindLogoutModal();
    bindGlobalModalEvents();
}

init();