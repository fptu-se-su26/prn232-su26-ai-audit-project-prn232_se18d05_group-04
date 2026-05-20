(function ContentModerationPage() {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  const state = { filter: "all" };
  const FILTERS = [
    ["all", "Tất cả"],
    ["pending", "Chờ duyệt"],
    ["approved", "Đã duyệt"],
    ["rejected", "Từ chối"],
    ["reported", "Bị báo cáo"]
  ];

  function authorName(id) {
    return DB.users.find((user) => user.id === id)?.full_name || "Không xác định";
  }

  function carName(id) {
    const car = DB.cars.find((item) => item.id === id);
    return car ? `${car.brand} ${car.model}` : "Không xác định";
  }

  function contentItems() {
    const reviews = (DB.reviews || []).filter((item) => !item.deleted_at).map((review) => ({
      id: review.id,
      source: "reviews",
      type: "Đánh giá",
      title: `${review.rating}/5 · ${carName(review.car_id)}`,
      body: review.comment,
      author: authorName(review.reviewer_id),
      created_at: review.created_at,
      status: review.moderation_status || "pending",
      hidden: review.hidden
    }));
    const reports = (DB.incident_reports || []).filter((item) => !item.deleted_at).map((report) => ({
      id: report.id,
      source: "incident_reports",
      type: "Báo cáo",
      title: report.title,
      body: report.description,
      author: authorName(report.reported_by),
      created_at: report.created_at,
      status: report.moderation_status || "reported",
      hidden: report.hidden
    }));
    return [...reviews, ...reports].sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
  }

  function filteredItems() {
    return contentItems().filter((item) => state.filter === "all" || item.status === state.filter);
  }

  function badge(status, hidden) {
    if (hidden) return U.statusBadge("neutral", "hidden", "Đã ẩn");
    const tone = status === "approved" ? "success" : status === "rejected" ? "danger" : status === "reported" ? "danger" : "warning";
    const label = { pending: "Chờ duyệt", approved: "Đã duyệt", rejected: "Từ chối", reported: "Bị báo cáo" }[status] || status;
    return U.statusBadge(tone, status, label);
  }

  function renderSummary() {
    const items = contentItems();
    const rows = [
      ["Tổng nội dung", items.length],
      ["Chờ duyệt", items.filter((item) => item.status === "pending").length],
      ["Bị báo cáo", items.filter((item) => item.status === "reported").length],
      ["Đã duyệt", items.filter((item) => item.status === "approved").length],
      ["Từ chối", items.filter((item) => item.status === "rejected").length]
    ];
    document.getElementById("contentModerationSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderFilters() {
    document.getElementById("contentModerationFilters").innerHTML = FILTERS.map(([key, label]) => {
      const active = state.filter === key;
      return `<button class="rounded-lg border px-4 py-2 text-sm font-medium transition ${active ? "border-emerald-600 bg-emerald-600 text-white" : "border-zinc-300 bg-white text-zinc-700 hover:bg-zinc-100"}" type="button" data-content-filter="${key}">${label}</button>`;
    }).join("");
  }

  function renderContentModeration() {
    const items = filteredItems();
    renderSummary();
    renderFilters();
    document.getElementById("contentModerationEmpty").classList.toggle("hidden", items.length > 0);
    document.querySelector(".table-wrap").classList.toggle("hidden", items.length === 0);
    document.getElementById("contentModerationTableBody").innerHTML = items.map((item) => `
      <tr>
        <td>${item.type}</td>
        <td><strong>${item.title}</strong><br><span class="muted">${item.body}</span></td>
        <td>${item.author}</td>
        <td>${U.formatDateTime(item.created_at)}</td>
        <td>${badge(item.status, item.hidden)}</td>
        <td class="actions">${U.renderActionMenu([
          { label: "Xem chi tiết", attrs: { "data-view-content": `${item.source}:${item.id}` } },
          { label: "Duyệt", attrs: { "data-approve-content": `${item.source}:${item.id}` } },
          { label: "Từ chối", attrs: { "data-reject-content": `${item.source}:${item.id}` }, variant: "danger" },
          { label: "Ẩn nội dung", attrs: { "data-hide-content": `${item.source}:${item.id}` } },
          { label: "Xóa mềm", attrs: { "data-soft-delete-content": `${item.source}:${item.id}` }, variant: "danger" }
        ])}</td>
      </tr>
    `).join("");
  }

  function findRecord(token) {
    const [source, id] = token.split(":");
    return { source, record: DB[source].find((item) => item.id === Number(id)) };
  }

  function saveAndRender(message) {
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast(message);
    renderContentModeration();
  }

  function viewDetail(token) {
    const { source, record } = findRecord(token);
    if (!record) return;
    const isReview = source === "reviews";
    document.getElementById("contentModerationDetail").innerHTML = `
      <div class="grid gap-3 text-sm text-zinc-700">
        <div><strong class="text-zinc-900">${isReview ? "Đánh giá" : record.title}</strong></div>
        <div>${isReview ? record.comment : record.description}</div>
        <div class="alert">Trạng thái: ${record.moderation_status || "pending"}</div>
      </div>
    `;
    U.openModal("contentModerationDetailModal");
  }

  function approveContent(token) {
    const { record } = findRecord(token);
    if (!record) return;
    record.moderation_status = "approved";
    record.hidden = false;
    saveAndRender("Đã duyệt nội dung.");
  }

  function rejectContent(token) {
    const { record } = findRecord(token);
    if (!record) return;
    record.moderation_status = "rejected";
    saveAndRender("Đã từ chối nội dung.");
  }

  function hideContent(token) {
    const { record } = findRecord(token);
    if (!record) return;
    record.hidden = true;
    saveAndRender("Đã ẩn nội dung.");
  }

  function softDeleteItem(token) {
    const { record } = findRecord(token);
    if (!record) return;
    record.deleted_at = new Date().toISOString();
    saveAndRender("Đã chuyển nội dung vào thùng rác.");
  }

  document.getElementById("contentModerationFilters").addEventListener("click", (event) => {
    const button = event.target.closest("[data-content-filter]");
    if (!button) return;
    state.filter = button.dataset.contentFilter;
    renderContentModeration();
  });
  document.getElementById("contentModerationTableBody").addEventListener("click", (event) => {
    const view = event.target.closest("[data-view-content]");
    const approve = event.target.closest("[data-approve-content]");
    const reject = event.target.closest("[data-reject-content]");
    const hide = event.target.closest("[data-hide-content]");
    const softDelete = event.target.closest("[data-soft-delete-content]");
    if (view) return viewDetail(view.dataset.viewContent);
    if (approve) return approveContent(approve.dataset.approveContent);
    if (reject) return rejectContent(reject.dataset.rejectContent);
    if (hide) return hideContent(hide.dataset.hideContent);
    if (softDelete) return softDeleteItem(softDelete.dataset.softDeleteContent);
  });

  window.ContentModerationPage = { renderContentModeration, softDeleteItem };
  renderContentModeration();
})();
