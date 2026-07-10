export function byId(id) {
    return document.getElementById(id);
}

export function requireElement(selector, root = document) {
    const element = root.querySelector(selector);

    if (!element) {
        throw new Error(`Required element not found: ${selector}`);
    }

    return element;
}

export function escapeHtml(value) {
    const element = document.createElement("div");
    element.textContent = String(value ?? "");
    return element.innerHTML;
}