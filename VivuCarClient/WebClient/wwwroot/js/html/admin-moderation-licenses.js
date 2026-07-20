(function LicenseModerationPage() {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const state = { filter: "all" };
  let selectedUserId = null;
  const FILTERS = [
    ["all", "Tất cả"],
    ["pending", "Chờ duyệt"],
    ["approved", "Đã duyệt"],
    ["rejected", "Từ chối"]
  ];

  function licenseDocuments() {
    return (DB.user_documents || []).filter((doc) => ["license_front", "license_back"].includes(doc.document_type));
  }

  function userLicenseGroups() {
    const userIds = [...new Set(licenseDocuments().map((doc) => doc.user_id))];
    return userIds.map((userId) => {
      const user = DB.users.find((item) => item.id === userId);
      const docs = licenseDocuments().filter((doc) => doc.user_id === userId);
      const front = docs.find((doc) => doc.document_type === "license_front");
      const back = docs.find((doc) => doc.document_type === "license_back");
      const statuses = docs.map((doc) => doc.moderation_status || (doc.verified ? "approved" : "pending"));
      const status = statuses.includes("rejected") ? "rejected" : statuses.every((value) => value === "approved") && docs.length >= 2 ? "approved" : "pending";
      const createdAt = docs.map((doc) => doc.created_at).sort()[0];
      return { userId, user, docs, front, back, status, created_at: createdAt };
    }).filter((group) => group.user);
  }

  function filteredGroups() {
    return userLicenseGroups()
      .filter((group) => state.filter === "all" || group.status === state.filter)
      .sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
  }

  function statusBadge(status) {
    const tone = status === "approved" ? "success" : status === "rejected" ? "danger" : "warning";
    const label = { pending: "Chờ duyệt", approved: "Đã duyệt", rejected: "Từ chối" }[status];
    return U.statusBadge(tone, status, label);
  }

  function renderSummary() {
    const groups = userLicenseGroups();
    const rows = [
      ["Tổng hồ sơ", groups.length],
      ["Chờ duyệt", groups.filter((item) => item.status === "pending").length],
      ["Đã duyệt", groups.filter((item) => item.status === "approved").length],
      ["Từ chối", groups.filter((item) => item.status === "rejected").length]
    ];
    document.getElementById("licenseModerationSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderFilters() {
    document.getElementById("licenseModerationFilters").innerHTML = FILTERS.map(([key, label]) => {
      const active = state.filter === key;
      return `<button class="rounded-lg border px-4 py-2 text-sm font-medium transition ${active ? "border-emerald-600 bg-emerald-600 text-white" : "border-zinc-300 bg-white text-zinc-700 hover:bg-zinc-100"}" type="button" data-license-filter="${key}">${label}</button>`;
    }).join("");
  }

  function licenseThumb(doc, label) {
    if (!doc) return `<span class="rounded-lg border border-zinc-200 bg-zinc-50 px-3 py-2 text-xs font-semibold text-zinc-400">${label} thiếu</span>`;
    return `<a href="${doc.file_url}" target="_blank" class="inline-flex items-center gap-2 rounded-lg border border-zinc-200 bg-white px-2 py-1 text-xs font-semibold text-zinc-600 hover:bg-zinc-50"><img class="h-12 w-16 rounded-md object-cover" src="${doc.file_url}" alt="${label}"> ${label}</a>`;
  }

  function renderLicenseModeration() {
    const groups = filteredGroups();
    renderSummary();
    renderFilters();
    document.getElementById("licenseModerationEmpty").classList.toggle("hidden", groups.length > 0);
    document.querySelector(".table-wrap").classList.toggle("hidden", groups.length === 0);
    document.getElementById("licenseModerationTableBody").innerHTML = groups.map((group) => `
      <tr>
        <td><strong>${group.user.full_name}</strong><br><span class="muted">#${group.user.id}</span></td>
        <td>${group.user.email}</td>
        <td><div class="flex flex-wrap gap-2">${licenseThumb(group.front, "Mặt trước")}${licenseThumb(group.back, "Mặt sau")}</div></td>
        <td>${U.formatDateTime(group.created_at)}</td>
        <td>${statusBadge(group.status)}</td>
        <td class="actions">${U.renderActionMenu([
          { label: "Xem chi tiết", attrs: { "data-view-license": group.userId } },
          { label: "Duyệt GPLX", attrs: { "data-approve-license": group.userId } },
          { label: "Từ chối GPLX", attrs: { "data-reject-license": group.userId }, variant: "danger" }
        ])}</td>
      </tr>
    `).join("");
  }

  function docsForUser(userId) {
    return licenseDocuments().filter((doc) => doc.user_id === Number(userId));
  }

  function saveAndRender(message) {
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast(message);
    renderLicenseModeration();
  }

  function viewLicense(userId) {
    const group = userLicenseGroups().find((item) => item.userId === Number(userId));
    if (!group) return;
    document.getElementById("licenseDetailContent").innerHTML = `
      <div class="grid gap-4 text-sm text-zinc-700">
        <div><strong class="text-zinc-900">${group.user.full_name}</strong><br>${group.user.email}</div>
        <div class="grid grid-cols-2 gap-3 max-sm:grid-cols-1">
          ${group.front ? `<img class="w-full rounded-xl border border-zinc-200 object-cover" src="${group.front.file_url}" alt="GPLX mặt trước">` : `<div class="empty-state rounded-xl border border-zinc-200">Thiếu ảnh mặt trước</div>`}
          ${group.back ? `<img class="w-full rounded-xl border border-zinc-200 object-cover" src="${group.back.file_url}" alt="GPLX mặt sau">` : `<div class="empty-state rounded-xl border border-zinc-200">Thiếu ảnh mặt sau</div>`}
        </div>
        ${group.docs.find((doc) => doc.rejection_reason) ? `<div class="alert alert-error">${group.docs.find((doc) => doc.rejection_reason).rejection_reason}</div>` : ""}
      </div>
    `;
    U.openModal("licenseDetailModal");
  }

  function approveLicense(userId) {
    docsForUser(userId).forEach((doc) => {
      doc.verified = true;
      doc.moderation_status = "approved";
      doc.rejection_reason = "";
    });
    saveAndRender("Đã duyệt GPLX.");
  }

  function openRejectLicense(userId) {
    selectedUserId = Number(userId);
    document.getElementById("licenseRejectReason").value = "";
    U.openModal("licenseRejectModal");
  }

  function rejectLicense() {
    const reason = document.getElementById("licenseRejectReason").value.trim();
    if (!reason) return U.showToast("Vui lòng nhập lý do từ chối.");
    docsForUser(selectedUserId).forEach((doc) => {
      doc.verified = false;
      doc.moderation_status = "rejected";
      doc.rejection_reason = reason;
    });
    U.closeModal("licenseRejectModal");
    saveAndRender("Đã từ chối GPLX.");
  }

  document.getElementById("licenseModerationFilters").addEventListener("click", (event) => {
    const button = event.target.closest("[data-license-filter]");
    if (!button) return;
    state.filter = button.dataset.licenseFilter;
    renderLicenseModeration();
  });
  document.getElementById("licenseModerationTableBody").addEventListener("click", (event) => {
    const view = event.target.closest("[data-view-license]");
    const approve = event.target.closest("[data-approve-license]");
    const reject = event.target.closest("[data-reject-license]");
    if (view) return viewLicense(view.dataset.viewLicense);
    if (approve) return approveLicense(approve.dataset.approveLicense);
    if (reject) return openRejectLicense(reject.dataset.rejectLicense);
  });
  document.getElementById("btnCancelLicenseReject").addEventListener("click", () => U.closeModal("licenseRejectModal"));
  document.getElementById("btnConfirmLicenseReject").addEventListener("click", rejectLicense);

  window.LicenseModerationPage = { renderLicenseModeration, approveLicense, rejectLicense };
  renderLicenseModeration();
})();

