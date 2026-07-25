import { apiFetch, fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";
import { showToast } from "../../shared/toast.js";

const root = document.querySelector("[data-admin-trash-page]");
const state = {
  items: [],
  keyword: ""
};

function byId(id) {
  return document.getElementById(id);
}

function formatDate(value) {
  if (!value) return "—";

  const date = new Date(value);
  return Number.isNaN(date.getTime())
    ? "—"
    : date.toLocaleString("vi-VN");
}

function getVisibleItems() {
  const keyword = state.keyword.trim().toLocaleLowerCase("vi-VN");
  if (!keyword) return state.items;

  return state.items.filter((item) =>
    `${item.name ?? ""} ${item.info ?? ""}`
      .toLocaleLowerCase("vi-VN")
      .includes(keyword)
  );
}

function renderSummary() {
  byId("trashSummary").innerHTML = `
    <article class="summary-card">
      <span>Tài khoản đã xóa</span>
      <strong>${state.items.length}</strong>
      <small>Khách thuê có thể khôi phục</small>
    </article>
  `;
}

function render() {
  const body = byId("trashTableBody");
  const empty = byId("trashEmpty");
  const tableWrap = body.closest(".table-wrap");
  const items = getVisibleItems();

  empty.classList.toggle("hidden", items.length > 0);
  tableWrap.classList.toggle("hidden", items.length === 0);

  if (!items.length) {
    body.innerHTML = "";
    return;
  }

  body.innerHTML = items.map((item) => `
    <tr>
      <td>Khách thuê</td>
      <td><strong>${escapeHtml(item.name ?? "—")}</strong></td>
      <td>${escapeHtml(item.info ?? "—")}</td>
      <td>${escapeHtml(formatDate(item.created_at))}</td>
      <td>${escapeHtml(formatDate(item.deleted_at))}</td>
      <td>
        <button
          class="btn btn-secondary btn-sm"
          type="button"
          data-restore-user="${Number(item.id)}"
        >Khôi phục</button>
      </td>
    </tr>
  `).join("");

  window.VivuCarTailwindUI?.applyTailwind(body);
}

async function ensureSuccess(response) {
  if (response.ok) return;

  const payload = await response.json().catch(() => null);
  throw new Error(payload?.message || `Request failed with status ${response.status}.`);
}

async function load() {
  const body = byId("trashTableBody");
  byId("trashEmpty").classList.add("hidden");
  body.closest(".table-wrap").classList.remove("hidden");
  body.innerHTML = '<tr><td colspan="6" class="empty-cell">Đang tải dữ liệu...</td></tr>';

  try {
    const data = await fetchJson("admin/trash");
    state.items = Array.isArray(data) ? data : data.items ?? [];
    renderSummary();
    render();
  } catch (error) {
    state.items = [];
    renderSummary();
    body.innerHTML = `<tr><td colspan="6" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
  }
}

async function restoreCustomer(userId, button) {
  button.disabled = true;

  try {
    const response = await apiFetch(
      `api/admin/trash/users/${userId}/restore`,
      { method: "PATCH" }
    );
    await ensureSuccess(response);
    showToast("Đã khôi phục tài khoản khách thuê.", "success");
    await load();
  } catch (error) {
    showToast(error.message, "error");
    button.disabled = false;
  }
}

if (root) {
  byId("btnFilterTrash").addEventListener("click", () => {
    state.keyword = byId("trashSearchInput").value;
    render();
  });

  byId("trashSearchInput").addEventListener("keydown", (event) => {
    if (event.key !== "Enter") return;
    state.keyword = event.currentTarget.value;
    render();
  });

  byId("btnResetTrash").addEventListener("click", () => {
    byId("trashSearchInput").value = "";
    state.keyword = "";
    render();
  });

  byId("trashTableBody").addEventListener("click", (event) => {
    const button = event.target.closest("[data-restore-user]");
    if (!button) return;
    restoreCustomer(Number(button.dataset.restoreUser), button);
  });

  load();
}
