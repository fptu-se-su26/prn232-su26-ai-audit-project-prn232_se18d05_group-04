let globalEventsBound = false;

export function openModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;

    modal.classList.add("is-open");
    modal.setAttribute("aria-hidden", "false");
}

export function closeModal(idOrElement) {
    const modal = typeof idOrElement === "string"
        ? document.getElementById(idOrElement)
        : idOrElement;

    if (!modal) return;

    modal.classList.remove("is-open");
    modal.setAttribute("aria-hidden", "true");
}

export function bindGlobalModalEvents() {
    if (globalEventsBound) return;
    globalEventsBound = true;

    document.addEventListener("click", event => {
        if (event.target.matches("[data-close-modal]")) {
            closeModal(event.target.closest(".admin-modal"));
        }

        if (event.target.classList.contains("admin-modal")) {
            closeModal(event.target);
        }
    });

    document.addEventListener("keydown", event => {
        if (event.key !== "Escape") return;
        document.querySelectorAll(".admin-modal.is-open").forEach(closeModal);
    });
}