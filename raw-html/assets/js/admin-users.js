(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const state = { keyword: "", role: "", blocked: "", page: 1, pageSize: 10 };
  let selectedUserId = null;

  function getFilteredUsers() {
    return DB.users.filter((user) => {
      const keyword = `${user.full_name} ${user.email}`.toLowerCase();
      return (!state.keyword || keyword.includes(state.keyword.toLowerCase()))
        && (!state.role || user.role === state.role)
        && (state.blocked === "" || String(user.is_blocked) === state.blocked);
    });
  }

  function renderUsers() {
    const filtered = getFilteredUsers();
    const page = U.paginate(filtered, state.page, state.pageSize);
    state.page = page.page;
    document.getElementById("usersEmpty").classList.toggle("hidden", filtered.length > 0);
    document.getElementById("usersTableBody").innerHTML = page.items.map((user) => {
      const status = user.is_blocked ? U.statusBadge("danger", "blocked", "Đã khóa") : U.statusBadge("success", "active", "Đang hoạt động");
      return `<tr><td><input type="checkbox"></td><td class="mono">${user.id}</td><td>${user.full_name}</td><td>${user.email}</td><td>${U.statusBadge("", user.role, C.USER_ROLE_LABELS[user.role])}</td><td>${status}</td><td>${U.formatDate(user.created_at)}</td><td class="actions"><button class="btn btn-secondary btn-sm btn-view-user">Xem</button>${user.is_blocked ? `<button class="btn btn-primary btn-sm btn-unblock-user" data-id="${user.id}">Mở khóa</button>` : `<button class="btn btn-danger btn-sm btn-block-user" data-id="${user.id}">Khóa</button>`}</td></tr>`;
    }).join("");
    document.getElementById("userPaginationInfo").textContent = `Hiển thị ${filtered.length ? page.start + 1 : 0}-${Math.min(page.start + state.pageSize, filtered.length)} trên ${filtered.length} người dùng`;
    document.getElementById("userPageNumbers").innerHTML = Array.from({ length: page.totalPages }, (_, index) => `<button class="btn ${index + 1 === state.page ? "btn-primary" : "btn-secondary"} btn-sm" data-page="${index + 1}">${index + 1}</button>`).join("");
  }

  function applyFilters() {
    state.keyword = document.getElementById("searchUserInput").value.trim();
    state.role = document.getElementById("roleFilter").value;
    state.blocked = document.getElementById("blockedFilter").value;
    state.page = 1;
    renderUsers();
  }

  document.getElementById("btnApplyFilter").addEventListener("click", applyFilters);
  document.getElementById("btnResetFilter").addEventListener("click", () => {
    document.querySelectorAll(".toolbar input, .toolbar select").forEach((el) => el.value = "");
    applyFilters();
  });
  document.getElementById("userPageSize").addEventListener("change", (event) => {
    state.pageSize = Number(event.target.value);
    state.page = 1;
    renderUsers();
  });
  document.getElementById("userPrevPage").addEventListener("click", () => { state.page -= 1; renderUsers(); });
  document.getElementById("userNextPage").addEventListener("click", () => { state.page += 1; renderUsers(); });
  document.getElementById("userPageNumbers").addEventListener("click", (event) => {
    const btn = event.target.closest("[data-page]");
    if (btn) { state.page = Number(btn.dataset.page); renderUsers(); }
  });
  document.addEventListener("click", (event) => {
    const action = event.target.closest(".btn-block-user, .btn-unblock-user");
    if (!action) return;
    selectedUserId = Number(action.dataset.id);
    const user = DB.users.find((item) => item.id === selectedUserId);
    if (user.id === Auth.getCurrentUser().id) return U.showToast("Không thể khóa chính tài khoản admin đang đăng nhập.");
    const isBlock = action.classList.contains("btn-block-user");
    document.getElementById("userStatusTitle").textContent = isBlock ? "Khóa tài khoản" : "Mở khóa tài khoản";
    document.getElementById("userStatusText").textContent = isBlock ? `Khóa tài khoản ${user.full_name}?` : `Mở khóa tài khoản ${user.full_name}?`;
    document.getElementById("blockReasonWrap").classList.toggle("hidden", !isBlock);
    U.openModal("userStatusModal");
  });
  document.getElementById("btnCancelUserStatus").addEventListener("click", () => U.closeModal("userStatusModal"));
  document.getElementById("btnConfirmUserStatus").addEventListener("click", () => {
    const user = DB.users.find((item) => item.id === selectedUserId);
    user.is_blocked = !user.is_blocked;
    U.closeModal("userStatusModal");
    U.showToast(user.is_blocked ? "Đã khóa tài khoản thành công." : "Đã mở khóa tài khoản thành công.");
    renderUsers();
  });
  renderUsers();
})();
