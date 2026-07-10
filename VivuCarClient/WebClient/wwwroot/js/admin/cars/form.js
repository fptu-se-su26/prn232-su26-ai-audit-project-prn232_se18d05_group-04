import { createCar, getCarById, getCarModels, getCarOwners, getCarTypes, updateCar, uploadCarImages } from "./car-api.js";
import { escapeHtml } from "../../shared/dom.js";
import { toNumber } from "../../shared/utils.js";
import { showToast } from "../../shared/toast.js";
import { validateCarForm } from "./car-validation.js";
import { compressImages, createPreviewUrl, revokeAllPreviewUrls, validateSelectedImages } from "./image-service.js";

const state = {
    carId: null,
    selectedImages: [],
    compressedImages: [],
    submitting: false
};

const elements = {};

function cacheElements() {
    elements.root = document.querySelector("[data-admin-car-form]");
    if (!elements.root) return false;

    elements.form = document.getElementById("carForm");
    elements.owner = document.getElementById("owner_id");
    elements.type = document.getElementById("type_id");
    elements.carModel = document.getElementById("car_model_id");
    elements.brand = document.getElementById("brand");
    elements.model = document.getElementById("model");
    elements.images = document.getElementById("carImages");
    elements.preview = document.getElementById("imagePreviewGrid");
    elements.submit = document.getElementById("btnSaveCar");
    state.carId = elements.root.dataset.carId || null;
    return Boolean(elements.form);
}

function collectFormData() {
    const formData = new FormData(elements.form);

    return {
        owner_id: toNumber(formData.get("owner_id")),
        type_id: toNumber(formData.get("type_id")) || null,
        car_model_id: toNumber(formData.get("car_model_id")) || null,
        license_plate: String(formData.get("license_plate") ?? "").trim(),
        brand: String(formData.get("brand") ?? "").trim(),
        model: String(formData.get("model") ?? "").trim(),
        year: toNumber(formData.get("year")) || null,
        color: String(formData.get("color") ?? "").trim(),
        seats: toNumber(formData.get("seats")),
        kilometers_driven: toNumber(formData.get("kilometers_driven")) || 0,
        transmission: String(formData.get("transmission") ?? "automatic"),
        fuel_type: String(formData.get("fuel_type") ?? "gasoline"),
        price_per_day: toNumber(formData.get("price_per_day")),
        price_per_hours: toNumber(formData.get("price_per_hours")),
        status: String(formData.get("status") ?? "available"),
        address: String(formData.get("address") ?? "").trim(),
        description: String(formData.get("description") ?? "").trim()
    };
}

function setFieldValue(name, value) {
    const field = elements.form.elements.namedItem(name);
    if (field) field.value = value ?? "";
}

function populateForm(car) {
    Object.entries(car).forEach(([name, value]) => setFieldValue(name, value));
}

function renderErrors(errors) {
    const message = Object.values(errors).join(" ");
    showToast(message || "Please review the form.", "error");
}

function renderOwnerOptions(owners) {
    elements.owner.innerHTML = [
        '<option value="">Select an owner</option>',
        ...owners.map(owner => `<option value="${escapeHtml(owner.id)}">${escapeHtml(owner.full_name)}${owner.email ? ` (${escapeHtml(owner.email)})` : ""}</option>`)
    ].join("");
}

function renderTypeOptions(types) {
    elements.type.innerHTML = [
        '<option value="">Select a car type</option>',
        ...types.map(type => `<option value="${escapeHtml(type.id)}">${escapeHtml(type.name)}</option>`)
    ].join("");
}

function renderModelOptions(models) {
    elements.carModel.innerHTML = [
        '<option value="">Create from brand/model text</option>',
        ...models.map(model => `<option value="${escapeHtml(model.id)}" data-brand="${escapeHtml(model.brand_name)}" data-model="${escapeHtml(model.name)}">${escapeHtml(model.brand_name)} ${escapeHtml(model.name)}</option>`)
    ].join("");
}

function syncBrandModelFromModel() {
    const selected = elements.carModel.selectedOptions[0];
    if (!selected || !selected.value) return;
    elements.brand.value = selected.dataset.brand ?? "";
    elements.model.value = selected.dataset.model ?? "";
}

function renderImagePreview(files) {
    revokeAllPreviewUrls();
    elements.preview.innerHTML = "";

    files.forEach(file => {
        const image = document.createElement("img");
        image.src = createPreviewUrl(file);
        image.alt = file.name;
        elements.preview.appendChild(image);
    });
}

async function handleImageChange() {
    const files = Array.from(elements.images.files ?? []);
    const result = validateSelectedImages(files);

    if (result.errors.length) {
        showToast(result.errors.join(" "), "error");
    }

    state.selectedImages = result.validFiles;
    state.compressedImages = [];
    renderImagePreview(state.selectedImages);
}

async function loadLookups() {
    try {
        const [owners, types, models] = await Promise.all([getCarOwners(), getCarTypes(), getCarModels()]);
        renderOwnerOptions(owners);
        renderTypeOptions(types);
        renderModelOptions(models);
    } catch (error) {
        showToast(error.message, "error");
        renderOwnerOptions([]);
        renderTypeOptions([]);
        renderModelOptions([]);
    }
}

async function loadExistingCar() {
    if (!state.carId) return;

    try {
        const car = await getCarById(state.carId);
        populateForm(car);
    } catch (error) {
        showToast(error.message, "error");
    }
}

async function handleSubmit(event) {
    event.preventDefault();
    if (state.submitting) return;

    const data = collectFormData();
    const validation = validateCarForm(data, {
        isEdit: Boolean(state.carId),
        imageCount: state.selectedImages.length
    });

    if (!validation.valid) {
        renderErrors(validation.errors);
        return;
    }

    state.submitting = true;
    elements.submit.disabled = true;

    try {
        const savedCar = state.carId ? await updateCar(state.carId, data) : await createCar(data);

        if (state.selectedImages.length) {
            state.compressedImages = await compressImages(state.selectedImages);
            await uploadCarImages(savedCar.id ?? state.carId, state.compressedImages);
        }

        revokeAllPreviewUrls();
        showToast("Car saved successfully.", "success");
        window.location.href = "/admin/cars";
    } catch (error) {
        showToast(escapeHtml(error.message), "error");
    } finally {
        state.submitting = false;
        elements.submit.disabled = false;
    }
}

function bindEvents() {
    elements.images?.addEventListener("change", handleImageChange);
    elements.carModel?.addEventListener("change", syncBrandModelFromModel);
    elements.form.addEventListener("submit", handleSubmit);
}

async function init() {
    bindEvents();
    await loadLookups();
    await loadExistingCar();
}

if (cacheElements()) {
    init();
}
