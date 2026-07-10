import { getUsers, lockUser, unlockUser } from "./user-api.js";
import { escapeHtml } from "../../shared/dom.js";
import { paginate } from "../../shared/pagination.js";
import { showToast } from "../../shared/toast.js";

const state = {
    users: [],
    keyword: "",
    role: "",
    isBlocked: "",
    page: 1,
    pageSize: 10,
    selectedUserId: null,
    selectedAction: null
};

const elements = {};

function cacheElements() {
    elements.root = document.querySelector("[data-admin-users-page]");
    if (!elements.root) return false;

    elements.search = document.getElementById("searchUserInput");
    elements.role = document.getElementById("roleFilter");
    elements.blocked = document.getElementById("blockedFilter");
    elements.apply = document.getElementById("btnApplyFilter");
    elements.reset = document.getElementById("btnResetFilter");
    elements.body = document.getElementById("usersTableBody");
    elements.info = document.getElementById("userPaginationInfo");
    elements.prev = document.getElementById("userPrevPage");
    elements.next = document.getElementById("userNextPage");
    elements.modal = document.getElementById("userStatusModal");
    elements.confirm = document.getElementById("btnConfirmUserStatus");
    return true;
}

function normalizeUser(item) {
    return {
        id: item.id ?? item.Id,
        email: item.email ?? item.Email ?? "",
        fullName: item.fullName ?? item.full_name ?? item.FullName ?? "",
        role: item.role ?? item.Role ?? "user",
        isBlocked: item.isBlocked ?? item.is_blocked ?? item.IsBlocked ?? false,
        createdAt: item.createdAt ?? item.created_at ?? item.CreatedAt ?? ""
    };
}

function filteredUsers() {
    const keyword = state.keyword.toLowerCase();

    return state.users.filter(user => {
        const matchesKeyword = !keyword
            || user.fullName.toLowerCase().includes(keyword)
            || user.email.toLowerCase().includes(keyword);
        const matchesRole = !state.role || user.role === state.role;
        const matchesBlocked = !state.isBlocked || String(user.isBlocked) === state.isBlocked;
        return matchesKeyword && matchesRole && matchesBlocked;
    });
}

function statusBadge(user) {
    const label = user.isBlocked ? "Ðã khóa" : "Ðang ho?t d?ng";
    const tone = user.isBlocked ? "danger" : "success";
    return `<span class="status-badge status-${tone}">${label}</span>`;
}

function renderRow(user) {
    const action = user.isBlocked ? "unlock" : "lock";
    const label = user.isBlocked ? "M? khóa" : "Khóa";

    return `<tr>
        <td><input type="checkbox" aria-label="Ch?n ${escapeHtml(user.fullName)}" /></td>
        <td>${escapeHtml(user.id)}</td>
        <td>${escapeHtml(user.fullName)}</td>
        <td>${escapeHtml(user.email)}</td>
        <td>${escapeHtml(user.role)}</td>
        <td>${statusBadge(user)}</td>
        <td>${escapeHtml(user.createdAt || "-")}</td>
        <td><button class="button button-secondary" type="button" data-user-action="${action}" data-user-id="${escapeHtml(user.id)}">${label}</button></td>
    </tr>`;
}

function render() {
    const filtered = filteredUsers();
    const page = paginate(filtered, state.page, state.pageSize);
    state.page = page.page;

    elements.info.textContent = `Hi?n th? ${page.start}-${page.end} trên ${page.totalItems} ngu?i dùng`;
    elements.prev.disabled = page.page <= 1;
    elements.next.disabled = page.page >= page.totalPages;

    if (!page.items.length) {
        elements.body.innerHTML = `<tr><td colspan="8" class="empty-cell">Không tìm th?y ngu?i dùng phù h?p.</td></tr>`;
        return;
    }

    elements.body.innerHTML = page.items.map(renderRow).join("");
}

function collectFilters() {
    state.keyword = elements.search?.value.trim() ?? "";
    state.role = elements.role?.value ?? "";
    state.isBlocked = elements.blocked?.value ?? "";
    state.page = 1;
}

async function loadUsers() {
    elements.body.innerHTML = `<tr><td colspan="8" class="empty-cell">Ðang t?i d? li?u ngu?i dùng...</td></tr>`;

    try {
        const users = await getUsers();
        state.users = (Array.isArray(users) ? users : []).map(normalizeUser);
        render();
    } catch (error) {
        elements.body.innerHTML = `<tr><td colspan="8" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
    }
}

function openUserStatusModal(userId, action) {
    state.selectedUserId = userId;
    state.selectedAction = action;
    elements.modal?.classList.add("is-open");
    elements.modal?.setAttribute("aria-hidden", "false");
}

async function confirmUserStatusChange() {
    if (!state.selectedUserId || !state.selectedAction) return;
    elements.confirm.disabled = true;

    try {
        if (state.selectedAction === "lock") await lockUser(state.selectedUserId);
        else await unlockUser(state.selectedUserId);
        showToast(state.selectedAction === "lock" ? "Ðã khóa tài kho?n thành công." : "Ðã m? khóa tài kho?n thành công.", "success");
        await loadUsers();
    } catch (error) {
        showToast(error.message, "error");
    } finally {
        elements.confirm.disabled = false;
        state.selectedUserId = null;
        state.selectedAction = null;
        elements.modal?.classList.remove("is-open");
        elements.modal?.setAttribute("aria-hidden", "true");
    }
}

function bindEvents() {
    elements.apply.addEventListener("click", () => {
        collectFilters();
        render();
    });

    elements.reset.addEventListener("click", () => {
        elements.search.value = "";
        elements.role.value = "";
        elements.blocked.value = "";
        collectFilters();
        render();
    });

    elements.prev.addEventListener("click", () => {
        state.page -= 1;
        render();
    });

    elements.next.addEventListener("click", () => {
        state.page += 1;
        render();
    });

    elements.body.addEventListener("click", event => {
        const button = event.target.closest("[data-user-action]");
        if (!button) return;
        openUserStatusModal(button.dataset.userId, button.dataset.userAction);
    });

    elements.confirm?.addEventListener("click", confirmUserStatusChange);
}

if (cacheElements()) {
    bindEvents();
    loadUsers();
}