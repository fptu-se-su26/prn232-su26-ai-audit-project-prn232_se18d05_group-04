import { blockCar, getCarTypes, getCars, unblockCar } from "./car-api.js";
import { CAR_STATUS_LABELS } from "../../shared/constants.js";
import { escapeHtml } from "../../shared/dom.js";
import { formatVnd } from "../../shared/utils.js";
import { showToast } from "../../shared/toast.js";

const state = {
    keyword: "",
    status: "",
    type_id: "",
    fuel_type: "",
    transmission: "",
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0,
    cars: []
};

const elements = {};

function cacheElements() {
    elements.root = document.querySelector("[data-admin-cars-page]");
    if (!elements.root) return false;

    elements.search = document.getElementById("searchCarInput");
    elements.status = document.getElementById("statusFilter");
    elements.type = document.getElementById("typeFilter");
    elements.fuel = document.getElementById("fuelFilter");
    elements.transmission = document.getElementById("transmissionFilter");
    elements.filter = document.getElementById("btnFilter");
    elements.reset = document.getElementById("btnReset");
    elements.body = document.getElementById("carsTableBody");
    elements.summaryCards = document.querySelectorAll(".admin-grid-kpi .admin-card strong");
    elements.paginationInfo = document.getElementById("paginationInfo");
    elements.pageSize = document.getElementById("pageSize");
    elements.prevPage = document.getElementById("prevPage");
    elements.nextPage = document.getElementById("nextPage");
    return true;
}

function renderStatus(status) {
    return `<span class="status-badge status-${escapeHtml(status)}">${escapeHtml(CAR_STATUS_LABELS[status] ?? status)}</span>`;
}

function renderRow(car) {
    const canBlock = car.status !== "blocked";
    return `<tr>
        <td>${car.primary_image_url ? `<img src="${escapeHtml(car.primary_image_url)}" alt="${escapeHtml(car.brand)} ${escapeHtml(car.model)}" class="car-thumb">` : "-"}</td>
        <td>${escapeHtml(car.license_plate)}</td>
        <td><strong>${escapeHtml(car.brand)} ${escapeHtml(car.model)}</strong></td>
        <td>${escapeHtml(car.type_name || "-")}</td>
        <td>${escapeHtml(car.year ?? "-")}</td>
        <td>${escapeHtml(car.seats ?? "-")}</td>
        <td>${escapeHtml(formatVnd(car.price_per_day))}</td>
        <td>${renderStatus(car.status)}</td>
        <td>${escapeHtml(car.owner_name)}</td>
        <td>
            <a class="button button-secondary" href="/admin/car-form/${encodeURIComponent(car.id)}">Edit</a>
            <button class="button ${canBlock ? "button-danger" : "button-secondary"}" type="button" data-car-action="${canBlock ? "block" : "unblock"}" data-car-id="${escapeHtml(car.id)}">
                ${canBlock ? "Block" : "Unblock"}
            </button>
        </td>
    </tr>`;
}

function renderSummary() {
    if (elements.summaryCards.length < 4) return;
    elements.summaryCards[0].textContent = String(state.totalItems);
    elements.summaryCards[1].textContent = String(state.cars.filter(car => car.status === "available").length);
    elements.summaryCards[2].textContent = String(state.cars.filter(car => car.status === "maintenance").length);
    elements.summaryCards[3].textContent = String(state.cars.filter(car => car.status === "blocked").length);
}

function renderPagination() {
    const currentPage = state.totalPages === 0 ? 0 : state.page;
    if (elements.paginationInfo) {
        elements.paginationInfo.textContent = `Page ${currentPage} / ${state.totalPages} · ${state.totalItems} cars`;
    }
    if (elements.prevPage) elements.prevPage.disabled = state.page <= 1;
    if (elements.nextPage) elements.nextPage.disabled = state.totalPages === 0 || state.page >= state.totalPages;
    if (elements.pageSize) elements.pageSize.value = String(state.pageSize);
}

function render() {
    renderSummary();
    renderPagination();

    if (!state.cars.length) {
        elements.body.innerHTML = `<tr><td colspan="10" class="empty-cell">No cars matched your filters.</td></tr>`;
        return;
    }

    elements.body.innerHTML = state.cars.map(renderRow).join("");
}

function collectFilters() {
    state.keyword = elements.search?.value.trim() ?? "";
    state.status = elements.status?.value ?? "";
    state.type_id = elements.type?.value ?? "";
    state.fuel_type = elements.fuel?.value ?? "";
    state.transmission = elements.transmission?.value ?? "";
    state.page = 1;
}

async function loadTypes() {
    if (!elements.type) return;
    try {
        const types = await getCarTypes();
        elements.type.innerHTML = [
            '<option value="">All types</option>',
            ...types.map(type => `<option value="${escapeHtml(type.id)}">${escapeHtml(type.name)}</option>`)
        ].join("");
    } catch (error) {
        showToast(error.message, "error");
    }
}

async function loadCars() {
    elements.body.innerHTML = `<tr><td colspan="10" class="empty-cell">Loading cars...</td></tr>`;

    try {
        const result = await getCars({
            page: state.page,
            pageSize: state.pageSize,
            keyword: state.keyword,
            status: state.status,
            type_id: state.type_id,
            fuel_type: state.fuel_type,
            transmission: state.transmission
        });

        state.cars = result.items;
        state.page = result.page;
        state.pageSize = result.pageSize || state.pageSize;
        state.totalItems = result.totalItems;
        state.totalPages = result.totalPages;
        render();
    } catch (error) {
        elements.body.innerHTML = `<tr><td colspan="10" class="empty-cell">${escapeHtml(error.message)}</td></tr>`;
        renderPagination();
    }
}

async function handleTableAction(event) {
    const button = event.target.closest("[data-car-action]");
    if (!button) return;

    const id = button.dataset.carId;
    const action = button.dataset.carAction;
    button.disabled = true;

    try {
        if (action === "block") await blockCar(id, "Blocked by admin moderation.");
        else await unblockCar(id, "available");
        showToast(action === "block" ? "Car blocked successfully." : "Car unblocked successfully.", "success");
        await loadCars();
    } catch (error) {
        showToast(error.message, "error");
    } finally {
        button.disabled = false;
    }
}

function bindEvents() {
    elements.filter?.addEventListener("click", async () => {
        collectFilters();
        await loadCars();
    });

    elements.reset?.addEventListener("click", async () => {
        [elements.search, elements.status, elements.type, elements.fuel, elements.transmission].forEach(element => {
            if (element) element.value = "";
        });
        collectFilters();
        await loadCars();
    });

    elements.prevPage?.addEventListener("click", async () => {
        if (state.page <= 1) return;
        state.page -= 1;
        await loadCars();
    });

    elements.nextPage?.addEventListener("click", async () => {
        if (state.totalPages === 0 || state.page >= state.totalPages) return;
        state.page += 1;
        await loadCars();
    });

    elements.pageSize?.addEventListener("change", async () => {
        state.pageSize = Number(elements.pageSize.value) || 10;
        state.page = 1;
        await loadCars();
    });

    elements.body.addEventListener("click", handleTableAction);
}

async function init() {
    bindEvents();
    await loadTypes();
    await loadCars();
}

if (cacheElements()) {
    init();
}
