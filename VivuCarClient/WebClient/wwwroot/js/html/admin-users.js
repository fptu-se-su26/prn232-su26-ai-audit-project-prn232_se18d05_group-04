(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const state = { keyword: "", role: "", blocked: "", verification: "", quickTab: "all", page: 1, pageSize: 10, selectedIds: new Set() };
  let selectedUserId = null;
  let pendingStatusAction = null;
  let pendingSoftDeleteIds = [];

  const QUICK_TABS = [
    ["all", "Tất cả"],
    ["admin", "Quản trị viên"],
    ["car_owner", "Chủ xe"],
    ["user", "Khách thuê"],
    ["blocked", "Bị khóa"],
    ["pending", "Chờ xác minh"]
  ];

  function activeUsers() {
    return DB.users.filter((user) => !user.deleted_at);
  }

  function userDocs(userId) {
    return (DB.user_documents || []).filter((doc) => doc.user_id === userId);
  }

  function licenseDocs(userId) {
    return userDocs(userId).filter((doc) => ["license_front", "license_back"].includes(doc.document_type));
  }

  function verificationState(user) {
    const docs = licenseDocs(user.id);
    if (!docs.length) return { key: "unverified", label: "Chưa xác minh", tone: "neutral" };
    const statuses = docs.map((doc) => doc.moderation_status || (doc.verified ? "approved" : "pending"));
    if (statuses.includes("rejected")) return { key: "rejected", label: "Từ chối GPLX", tone: "danger" };
    if (docs.length >= 2 && statuses.every((status) => status === "approved")) return { key: "verified", label: "Đã xác minh", tone: "success" };
    return { key: "pending", label: "Chờ GPLX", tone: "warning" };
  }

  function orderCount(userId) {
    return DB.bookings.filter((booking) => booking.user_id === userId).length;
  }

  function carCount(userId) {
    return DB.cars.filter((car) => car.owner_id === userId && !car.deleted_at).length;
  }

  function phoneText() {
    return "--";
  }

  function lastLoginText(user) {
    return user.last_login_at ? U.formatDateTime(user.last_login_at) : "--";
  }

  function monthStart() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
  }

  function statIcon(path) {
    return `<span class="grid h-10 w-10 place-items-center rounded-xl bg-emerald-50 text-emerald-600"><svg aria-hidden="true" class="h-4 w-4 fill-current" viewBox="0 0 512 512"><path d="${path}"/></svg></span>`;
  }

  function renderStats() {
    const users = activeUsers();
    const rows = [
      ["Tổng người dùng", users.length, "M256 256A128 128 0 1 0 256 0a128 128 0 1 0 0 256zm-45.7 48C93.8 304 0 397.8 0 514.3c0 16.4 13.3 29.7 29.7 29.7h452.6c16.4 0 29.7-13.3 29.7-29.7C512 397.8 418.2 304 301.7 304h-91.4z"],
      ["Quản trị viên", users.filter((user) => user.role === "admin").length, "M256 0l192 80v144c0 123.7-79.1 237.3-192 288C143.1 461.3 64 347.7 64 224V80L256 0z"],
      ["Chủ xe", users.filter((user) => user.role === "car_owner").length, "M135.2 117.4 109.1 192h293.8l-26.1-74.6C372.3 104.6 360.2 96 346.6 96H165.4c-13.6 0-25.7 8.6-30.2 21.4zM39.6 196.8C16.4 207.6 0 231.1 0 258.5V400c0 26.5 21.5 48 48 48h16v32h64v-32h224v32h64v-32h48c26.5 0 48-21.5 48-48V258.5c0-27.4-16.4-50.9-39.6-61.7L437.2 96.3C423.7 57.8 387.4 32 346.6 32H165.4c-40.8 0-77.1 25.8-90.6 64.3L39.6 196.8z"],
      ["Khách thuê", users.filter((user) => user.role === "user").length, "M96 128a128 128 0 1 1 256 0A128 128 0 1 1 96 128zM0 482.3C0 383.8 79.8 304 178.3 304h91.4C368.2 304 448 383.8 448 482.3c0 16.4-13.3 29.7-29.7 29.7H29.7C13.3 512 0 498.7 0 482.3z"],
      ["Tài khoản bị khóa", users.filter((user) => user.is_blocked).length, "M144 144v48h224v-48c0-61.9-50.1-112-112-112S144 82.1 144 144zM80 192v-48C80 46 158 0 256 0s176 46 176 144v48h16c35.3 0 64 28.7 64 64v192c0 35.3-28.7 64-64 64H64c-35.3 0-64-28.7-64-64V256c0-35.3 28.7-64 64-64h16z"],
      ["Người dùng mới trong tháng", users.filter((user) => new Date(user.created_at) >= monthStart()).length, "M256 48a208 208 0 1 1 0 416 208 208 0 1 1 0-416zm0 80c-8.8 0-16 7.2-16 16v96h-96c-8.8 0-16 7.2-16 16s7.2 16 16 16h96v96c0 8.8 7.2 16 16 16s16-7.2 16-16v-96h96c8.8 0 16-7.2 16-16s-7.2-16-16-16h-96v-96c0-8.8-7.2-16-16-16z"]
    ];
    document.getElementById("userStats").innerHTML = rows.map(([label, value, icon]) => `
      <article class="rounded-2xl border border-zinc-200 bg-white p-4 shadow-sm">
        <div class="flex items-center gap-3">
          ${statIcon(icon)}
          <div><strong class="block font-mono text-2xl text-zinc-900">${value}</strong><span class="text-sm font-medium text-zinc-500">${label}</span></div>
        </div>
      </article>
    `).join("");
  }

  function quickTabCount(key) {
    const users = activeUsers();
    if (key === "blocked") return users.filter((user) => user.is_blocked).length;
    if (key === "pending") return users.filter((user) => verificationState(user).key === "pending").length;
    if (["admin", "car_owner", "user"].includes(key)) return users.filter((user) => user.role === key).length;
    return users.length;
  }

  function renderQuickTabs() {
    document.getElementById("userQuickTabs").innerHTML = QUICK_TABS.map(([key, label]) => {
      const active = state.quickTab === key;
      return `<button class="rounded-lg border px-4 py-2 text-sm font-medium transition ${active ? "border-emerald-600 bg-emerald-600 text-white" : "border-zinc-200 bg-white text-zinc-600 hover:bg-zinc-100"}" type="button" data-user-tab="${key}">${label}<span class="ml-2 rounded-full ${active ? "bg-white/20" : "bg-zinc-100"} px-2 py-0.5 text-xs">${quickTabCount(key)}</span></button>`;
    }).join("");
  }

  function getFilteredUsers() {
    return activeUsers().filter((user) => {
      const verification = verificationState(user).key;
      const keyword = `${user.full_name} ${user.email} ${phoneText(user)}`.toLowerCase();
      const quickMatch = state.quickTab === "all"
        || user.role === state.quickTab
        || (state.quickTab === "blocked" && user.is_blocked)
        || (state.quickTab === "pending" && verification === "pending");
      return quickMatch
        && (!state.keyword || keyword.includes(state.keyword.toLowerCase()))
        && (!state.role || user.role === state.role)
        && (state.blocked === "" || String(user.is_blocked) === state.blocked)
        && (!state.verification || verification === state.verification);
    });
  }

  function accountBadge(user) {
    return user.is_blocked ? U.statusBadge("danger", "blocked", "Đã khóa") : U.statusBadge("success", "active", "Đang hoạt động");
  }

  function verificationBadge(user) {
    const status = verificationState(user);
    return U.statusBadge(status.tone, status.key, status.label);
  }

  function actionMenu(user) {
    const lockLabel = user.is_blocked ? "Mở khóa tài khoản" : "Khóa tài khoản";
    return U.renderActionMenu([
      { label: "Xem chi tiết", attrs: { "data-view-user": user.id } },
      { label: "Chỉnh sửa", attrs: { "data-edit-user": user.id } },
      { label: lockLabel, attrs: { "data-toggle-lock-user": user.id }, variant: user.is_blocked ? "default" : "danger" },
      { label: "Xem lịch sử đơn", attrs: { "data-view-orders": user.id } },
      { label: "Xem GPLX", attrs: { "data-view-license": user.id } },
      { label: "Đặt lại mật khẩu", attrs: { "data-reset-password": user.id } },
      { label: "Xóa mềm", attrs: { "data-soft-delete-user": user.id }, variant: "danger" }
    ]);
  }

  function renderBulkBar() {
    const count = state.selectedIds.size;
    const bar = document.getElementById("userBulkBar");
    bar.classList.toggle("hidden", count === 0);
    bar.classList.toggle("flex", count > 0);
    document.getElementById("userBulkCount").textContent = `Đã chọn ${count} người dùng`;
  }

  function renderUsers() {
    renderStats();
    renderQuickTabs();
    const filtered = getFilteredUsers();
    const page = U.paginate(filtered, state.page, state.pageSize);
    state.page = page.page;
    const empty = document.getElementById("usersEmpty");
    empty.classList.toggle("hidden", filtered.length > 0);
    document.querySelector(".table-wrap").classList.toggle("hidden", filtered.length === 0);
    document.querySelector(".pagination").classList.toggle("hidden", filtered.length === 0);
    document.getElementById("usersTableBody").innerHTML = page.items.map((user) => `
      <tr>
        <td><input class="user-row-check" type="checkbox" data-id="${user.id}" ${state.selectedIds.has(user.id) ? "checked" : ""}></td>
        <td><strong>${user.full_name}</strong><br><span class="muted">${user.email}</span><br><span class="text-xs text-zinc-400">${phoneText(user)}</span></td>
        <td>${U.statusBadge("", user.role, C.USER_ROLE_LABELS[user.role])}</td>
        <td>${accountBadge(user)}</td>
        <td>${verificationBadge(user)}</td>
        <td>${lastLoginText(user)}</td>
        <td class="font-mono">${orderCount(user.id)}</td>
        <td class="font-mono">${carCount(user.id)}</td>
        <td>${U.formatDate(user.created_at)}</td>
        <td>${actionMenu(user)}</td>
      </tr>
    `).join("");
    document.getElementById("userSelectAll").checked = page.items.length > 0 && page.items.every((user) => state.selectedIds.has(user.id));
    document.getElementById("userPaginationInfo").textContent = `Hiển thị ${filtered.length ? page.start + 1 : 0}-${Math.min(page.start + state.pageSize, filtered.length)} trên ${filtered.length} người dùng`;
    document.getElementById("userPageNumbers").innerHTML = Array.from({ length: page.totalPages }, (_, index) => `<button class="btn ${index + 1 === state.page ? "btn-primary" : "btn-secondary"} btn-sm" data-page="${index + 1}">${index + 1}</button>`).join("");
    renderBulkBar();
  }

  function applyFilters() {
    state.keyword = document.getElementById("searchUserInput").value.trim();
    state.role = document.getElementById("roleFilter").value;
    state.blocked = document.getElementById("blockedFilter").value;
    state.verification = document.getElementById("verificationFilter").value;
    state.page = 1;
    renderUsers();
  }

  function selectedUsers() {
    return [...state.selectedIds].map((id) => DB.users.find((user) => user.id === id)).filter(Boolean);
  }

  function openStatusModal(ids, action) {
    const users = ids.map((id) => DB.users.find((user) => user.id === Number(id))).filter(Boolean);
    if (users.some((user) => user.id === Auth.getCurrentUser().id) && action === "block") return U.showToast("Không thể khóa chính tài khoản admin đang đăng nhập.");
    pendingStatusAction = { ids: users.map((user) => user.id), action };
    const isBlock = action === "block";
    document.getElementById("userStatusTitle").textContent = isBlock ? "Khóa tài khoản" : "Mở khóa tài khoản";
    document.getElementById("userStatusText").textContent = users.length === 1 ? `${isBlock ? "Khóa" : "Mở khóa"} tài khoản ${users[0].full_name}?` : `${isBlock ? "Khóa" : "Mở khóa"} ${users.length} tài khoản đã chọn?`;
    document.getElementById("blockReasonWrap").classList.toggle("hidden", !isBlock);
    U.openModal("userStatusModal");
  }

  function confirmStatusAction() {
    if (!pendingStatusAction) return;
    const isBlock = pendingStatusAction.action === "block";
    pendingStatusAction.ids.forEach((id) => {
      const user = DB.users.find((item) => item.id === id);
      if (!user || user.id === Auth.getCurrentUser().id && isBlock) return;
      user.is_blocked = isBlock;
      user.lock_history ||= [];
      user.lock_history.push({ action: isBlock ? "blocked" : "unblocked", at: new Date().toISOString(), note: isBlock ? document.getElementById("blockReason").value.trim() : "" });
    });
    window.VivuCarSaveDB?.();
    U.closeModal("userStatusModal");
    U.showToast(isBlock ? "Đã khóa tài khoản thành công." : "Đã mở khóa tài khoản thành công.");
    pendingStatusAction = null;
    renderUsers();
  }

  function openSoftDeleteModal(ids) {
    const users = ids.map((id) => DB.users.find((user) => user.id === Number(id))).filter(Boolean);
    if (users.some((user) => user.id === Auth.getCurrentUser().id)) return U.showToast("Không thể xóa chính tài khoản admin đang đăng nhập.");
    pendingSoftDeleteIds = users.map((user) => user.id);
    document.getElementById("userSoftDeleteInfo").textContent = users.length === 1 ? users[0].full_name : `${users.length} người dùng đã chọn`;
    U.openModal("userSoftDeleteModal");
  }

  function softDeleteItem(ids) {
    ids.forEach((id) => {
      const user = DB.users.find((item) => item.id === Number(id));
      if (!user || user.id === Auth.getCurrentUser().id) return;
      user.deleted_at = new Date().toISOString();
    });
    state.selectedIds.clear();
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.closeModal("userSoftDeleteModal");
    U.showToast("Đã chuyển người dùng vào thùng rác.");
    renderUsers();
  }

  function userDetailHtml(user) {
    const verification = verificationState(user);
    const docs = licenseDocs(user.id);
    const history = user.lock_history || [];
    return `
      <div class="grid gap-4">
        <section class="rounded-2xl border border-zinc-200 bg-white p-4">
          <h3 class="mb-3 text-lg font-semibold text-zinc-900">${user.full_name}</h3>
          <div class="grid gap-2 text-sm text-zinc-600">
            <div>Email: <strong class="text-zinc-900">${user.email}</strong></div>
            <div>Số điện thoại: <strong class="text-zinc-900">${phoneText(user)}</strong></div>
            <div>Vai trò: ${U.statusBadge("", user.role, C.USER_ROLE_LABELS[user.role])}</div>
            <div>Trạng thái tài khoản: ${accountBadge(user)}</div>
            <div>Trạng thái xác minh GPLX: ${U.statusBadge(verification.tone, verification.key, verification.label)}</div>
            <div>Ngày tạo: <strong class="text-zinc-900">${U.formatDateTime(user.created_at)}</strong></div>
            <div>Lần đăng nhập cuối: <strong class="text-zinc-900">${lastLoginText(user)}</strong></div>
            <div>Tổng số đơn: <strong class="font-mono text-zinc-900">${orderCount(user.id)}</strong></div>
            <div>Tổng số xe: <strong class="font-mono text-zinc-900">${carCount(user.id)}</strong></div>
          </div>
        </section>
        <section class="rounded-2xl border border-zinc-200 bg-white p-4">
          <h3 class="mb-3 text-sm font-semibold text-zinc-900">GPLX</h3>
          <div class="grid grid-cols-2 gap-2 max-sm:grid-cols-1">
            ${docs.length ? docs.map((doc) => `<a href="${doc.file_url}" target="_blank"><img class="aspect-[4/3] w-full rounded-xl border border-zinc-200 object-cover" src="${doc.file_url}" alt="${doc.file_name}"></a>`).join("") : `<p class="muted">Chưa có dữ liệu</p>`}
          </div>
        </section>
        <section class="rounded-2xl border border-zinc-200 bg-white p-4">
          <h3 class="mb-3 text-sm font-semibold text-zinc-900">Ghi chú nội bộ</h3>
          <textarea id="userAdminNote" class="min-h-24" placeholder="Nhập ghi chú nội bộ cho admin">${user.admin_note || ""}</textarea>
          <button class="btn btn-primary btn-sm mt-3" type="button" data-save-note="${user.id}">Lưu ghi chú</button>
        </section>
        <section class="rounded-2xl border border-zinc-200 bg-white p-4">
          <h3 class="mb-3 text-sm font-semibold text-zinc-900">Lịch sử khóa/mở khóa</h3>
          ${history.length ? history.map((item) => `<div class="border-t border-zinc-100 py-2 text-sm"><strong>${item.action === "blocked" ? "Khóa" : "Mở khóa"}</strong> · ${U.formatDateTime(item.at)}<br><span class="muted">${item.note || "--"}</span></div>`).join("") : `<p class="muted">Chưa có dữ liệu</p>`}
        </section>
      </div>
    `;
  }

  function openUserDrawer(id) {
    const user = DB.users.find((item) => item.id === Number(id));
    if (!user) return;
    document.getElementById("userDetailContent").innerHTML = userDetailHtml(user);
    window.VivuCarTailwindUI?.applyTailwind(document.getElementById("userDetailContent"));
    window.VivuCarTailwindUI?.openDrawer(document.getElementById("userDetailDrawer"), document.getElementById("userDrawerOverlay"));
  }

  function closeUserDrawer() {
    window.VivuCarTailwindUI?.closeDrawer(document.getElementById("userDetailDrawer"), document.getElementById("userDrawerOverlay"));
  }

  function resetPassword(id) {
    const user = DB.users.find((item) => item.id === Number(id));
    U.showToast(`Đã tạo yêu cầu đặt lại mật khẩu cho ${user?.full_name || "người dùng"}.`);
  }

  document.getElementById("userQuickTabs").addEventListener("click", (event) => {
    const btn = event.target.closest("[data-user-tab]");
    if (!btn) return;
    state.quickTab = btn.dataset.userTab;
    state.page = 1;
    renderUsers();
  });
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
  document.getElementById("userSelectAll").addEventListener("change", (event) => {
    document.querySelectorAll(".user-row-check").forEach((checkbox) => {
      checkbox.checked = event.target.checked;
      if (checkbox.checked) state.selectedIds.add(Number(checkbox.dataset.id));
      else state.selectedIds.delete(Number(checkbox.dataset.id));
    });
    renderBulkBar();
  });
  document.getElementById("usersTableBody").addEventListener("change", (event) => {
    const checkbox = event.target.closest(".user-row-check");
    if (!checkbox) return;
    if (checkbox.checked) state.selectedIds.add(Number(checkbox.dataset.id));
    else state.selectedIds.delete(Number(checkbox.dataset.id));
    renderBulkBar();
  });
  document.addEventListener("click", (event) => {
    const view = event.target.closest("[data-view-user]");
    const edit = event.target.closest("[data-edit-user]");
    const toggleLock = event.target.closest("[data-toggle-lock-user]");
    const viewOrders = event.target.closest("[data-view-orders]");
    const viewLicense = event.target.closest("[data-view-license]");
    const reset = event.target.closest("[data-reset-password]");
    const softDelete = event.target.closest("[data-soft-delete-user]");
    const saveNote = event.target.closest("[data-save-note]");
    if (view) return openUserDrawer(view.dataset.viewUser);
    if (edit) return openUserDrawer(edit.dataset.editUser);
    if (toggleLock) {
      const user = DB.users.find((item) => item.id === Number(toggleLock.dataset.toggleLockUser));
      return openStatusModal([user.id], user.is_blocked ? "unblock" : "block");
    }
    if (viewOrders) return U.showToast(`Người dùng có ${orderCount(Number(viewOrders.dataset.viewOrders))} đơn.`);
    if (viewLicense) return openUserDrawer(viewLicense.dataset.viewLicense);
    if (reset) return resetPassword(reset.dataset.resetPassword);
    if (softDelete) return openSoftDeleteModal([softDelete.dataset.softDeleteUser]);
    if (saveNote) {
      const user = DB.users.find((item) => item.id === Number(saveNote.dataset.saveNote));
      user.admin_note = document.getElementById("userAdminNote").value.trim();
      window.VivuCarSaveDB?.();
      return U.showToast("Đã lưu ghi chú nội bộ.");
    }
  });
  document.getElementById("btnCancelUserStatus").addEventListener("click", () => U.closeModal("userStatusModal"));
  document.getElementById("btnConfirmUserStatus").addEventListener("click", confirmStatusAction);
  document.getElementById("btnCancelSoftDeleteUser").addEventListener("click", () => U.closeModal("userSoftDeleteModal"));
  document.getElementById("btnConfirmSoftDeleteUser").addEventListener("click", () => softDeleteItem(pendingSoftDeleteIds));
  document.getElementById("btnCloseUserDrawer").addEventListener("click", closeUserDrawer);
  document.getElementById("userDrawerOverlay").addEventListener("click", closeUserDrawer);
  document.getElementById("btnBulkBlock").addEventListener("click", () => openStatusModal([...state.selectedIds], "block"));
  document.getElementById("btnBulkUnblock").addEventListener("click", () => openStatusModal([...state.selectedIds], "unblock"));
  document.getElementById("btnBulkSoftDelete").addEventListener("click", () => openSoftDeleteModal([...state.selectedIds]));
  document.getElementById("btnBulkClear").addEventListener("click", () => { state.selectedIds.clear(); renderUsers(); });

  renderUsers();
})();

