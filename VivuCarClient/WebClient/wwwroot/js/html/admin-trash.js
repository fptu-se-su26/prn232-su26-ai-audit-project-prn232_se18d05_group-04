(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const state = { keyword: "", type: "" };
  let forceDeleteTarget = null;

  const TYPE_LABELS = {
    users: "Người dùng",
    cars: "Phương tiện",
    vouchers: "Voucher",
    reviews: "Đánh giá",
    incident_reports: "Báo cáo"
  };

  function userTitle(user) {
    return user.full_name;
  }

  function carTitle(car) {
    return `${car.brand} ${car.model}`;
  }

  function voucherTitle(voucher) {
    return voucher.code;
  }

  function mapTrashItem(type, item) {
    if (type === "users") {
      return {
        type,
        id: item.id,
        title: userTitle(item),
        info: `${item.email} · ${C.USER_ROLE_LABELS[item.role] || item.role}`,
        created_at: item.created_at,
        deleted_at: item.deleted_at
      };
    }
    if (type === "cars") {
      return {
        type,
        id: item.id,
        title: carTitle(item),
        info: `${item.license_plate} · ${C.CAR_STATUS_LABELS[item.status] || item.status}`,
        created_at: item.created_at,
        deleted_at: item.deleted_at
      };
    }
    if (type === "reviews") {
      return {
        type,
        id: item.id,
        title: `Đánh giá #${item.id}`,
        info: item.comment || "--",
        created_at: item.created_at,
        deleted_at: item.deleted_at
      };
    }
    if (type === "incident_reports") {
      return {
        type,
        id: item.id,
        title: item.title,
        info: item.description || "--",
        created_at: item.created_at,
        deleted_at: item.deleted_at
      };
    }
    return {
      type,
      id: item.id,
      title: voucherTitle(item),
      info: `${item.name} · ${C.DISCOUNT_TYPE_LABELS[item.discount_type] || item.discount_type}`,
      created_at: item.created_at,
      deleted_at: item.deleted_at
    };
  }

  function trashItems() {
    return ["users", "cars", "vouchers", "reviews", "incident_reports"].flatMap((type) => {
      return (DB[type] || [])
        .filter((item) => item.deleted_at)
        .map((item) => mapTrashItem(type, item));
    });
  }

  function filteredTrashItems() {
    return trashItems()
      .filter((item) => !state.type || item.type === state.type)
      .filter((item) => {
        const keyword = `${item.title} ${item.info} ${TYPE_LABELS[item.type]}`.toLowerCase();
        return !state.keyword || keyword.includes(state.keyword.toLowerCase());
      })
      .sort((a, b) => new Date(b.deleted_at) - new Date(a.deleted_at));
  }

  function renderSummary() {
    const rows = [
      ["Tổng trong thùng rác", trashItems().length],
      ["Người dùng", (DB.users || []).filter((item) => item.deleted_at).length],
      ["Phương tiện", (DB.cars || []).filter((item) => item.deleted_at).length],
      ["Voucher", (DB.vouchers || []).filter((item) => item.deleted_at).length],
      ["Nội dung", [...(DB.reviews || []), ...(DB.incident_reports || [])].filter((item) => item.deleted_at).length]
    ];
    document.getElementById("trashSummary").innerHTML = rows.map(([label, value]) => `<article class="summary-card"><span>${label}</span><strong>${value}</strong></article>`).join("");
  }

  function renderTrash() {
    const items = filteredTrashItems();
    renderSummary();
    document.getElementById("trashEmpty").classList.toggle("hidden", items.length > 0);
    document.querySelector(".table-wrap").classList.toggle("hidden", items.length === 0);
    document.getElementById("trashTableBody").innerHTML = items.map((item) => `
      <tr>
        <td>${U.statusBadge(item.type === "cars" ? "info" : item.type === "vouchers" ? "warning" : "neutral", item.type, TYPE_LABELS[item.type])}</td>
        <td><strong>${item.title}</strong><br><span class="muted">#${item.id}</span></td>
        <td>${item.info}</td>
        <td>${U.formatDate(item.created_at)}</td>
        <td>${U.formatDateTime(item.deleted_at)}</td>
        <td class="actions">${U.renderActionMenu([
          { label: "Khôi phục", attrs: { "data-restore-type": item.type, "data-id": item.id } },
          { label: "Xóa vĩnh viễn", attrs: { "data-force-delete-type": item.type, "data-id": item.id, "data-title": item.title }, variant: "danger" }
        ])}</td>
      </tr>
    `).join("");
  }

  function softDeleteItem() {
    // Intentionally unused on this page. Soft delete is triggered from object management screens.
  }

  function restoreItem(type, id) {
    const item = (DB[type] || []).find((record) => record.id === Number(id));
    if (!item) return;
    item.deleted_at = null;
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.showToast(`Đã khôi phục ${TYPE_LABELS[type].toLowerCase()}.`);
    renderTrash();
  }

  function forceDeleteItem(type, id) {
    const numericId = Number(id);
    if (type === "users") {
      DB.users = DB.users.filter((item) => item.id !== numericId);
    }
    if (type === "cars") {
      DB.cars = DB.cars.filter((item) => item.id !== numericId);
      DB.car_images = DB.car_images.filter((image) => image.car_id !== numericId);
    }
    if (type === "vouchers") {
      DB.vouchers = DB.vouchers.filter((item) => item.id !== numericId);
      DB.voucher_usages = DB.voucher_usages.filter((usage) => usage.voucher_id !== numericId);
    }
    if (type === "reviews") {
      DB.reviews = DB.reviews.filter((item) => item.id !== numericId);
    }
    if (type === "incident_reports") {
      DB.incident_reports = DB.incident_reports.filter((item) => item.id !== numericId);
    }
    window.VivuCarSaveDB?.();
    window.VivuCarLayout?.refreshAdminSidebar?.();
    U.closeModal("trashForceDeleteModal");
    U.showToast(`Đã xóa vĩnh viễn ${TYPE_LABELS[type].toLowerCase()}.`);
    renderTrash();
  }

  function applyFilters() {
    state.keyword = document.getElementById("trashSearchInput").value.trim();
    state.type = document.getElementById("trashTypeFilter").value;
    renderTrash();
  }

  document.getElementById("btnFilterTrash").addEventListener("click", applyFilters);
  document.getElementById("btnResetTrash").addEventListener("click", () => {
    document.getElementById("trashSearchInput").value = "";
    document.getElementById("trashTypeFilter").value = "";
    applyFilters();
  });
  document.getElementById("trashTableBody").addEventListener("click", (event) => {
    const restore = event.target.closest("[data-restore-type]");
    const forceDelete = event.target.closest("[data-force-delete-type]");
    if (restore) return restoreItem(restore.dataset.restoreType, restore.dataset.id);
    if (!forceDelete) return;
    forceDeleteTarget = {
      type: forceDelete.dataset.forceDeleteType,
      id: forceDelete.dataset.id
    };
    document.getElementById("trashForceDeleteInfo").textContent = `${TYPE_LABELS[forceDeleteTarget.type]} · ${forceDelete.dataset.title}`;
    U.openModal("trashForceDeleteModal");
  });
  document.getElementById("btnCancelTrashForceDelete").addEventListener("click", () => U.closeModal("trashForceDeleteModal"));
  document.getElementById("btnConfirmTrashForceDelete").addEventListener("click", () => {
    if (!forceDeleteTarget) return;
    forceDeleteItem(forceDeleteTarget.type, forceDeleteTarget.id);
  });

  window.VivuCarAdminTrash = { softDeleteItem, restoreItem, forceDeleteItem, renderTrash };
  renderTrash();
})();
