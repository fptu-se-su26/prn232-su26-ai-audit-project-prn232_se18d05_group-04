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
    existingImages: [],
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
    elements.existingGallery = document.getElementById("imagePreviewGrid");
    elements.existingCount = document.getElementById("existingImageCount");
    elements.selectedSection = document.getElementById("selectedImageSection");
    elements.selectedPreview = document.getElementById("selectedImagePreviewGrid");
    elements.selectedCount = document.getElementById("selectedImageCount");
    elements.clearImages = document.getElementById("btnClearImages");
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

function normalizeExistingImages(images) {
    return (images ?? [])
        .map(image => ({
            id: image.id ?? image.Id,
            imageUrl: image.image_url ?? image.imageUrl ?? image.ImageUrl ?? "",
            isPrimary: Boolean(image.is_primary ?? image.isPrimary ?? image.IsPrimary),
            displayOrder: Number(image.display_order ?? image.displayOrder ?? image.DisplayOrder ?? 0)
        }))
        .filter(image => image.imageUrl)
        .sort((left, right) => Number(right.isPrimary) - Number(left.isPrimary) || left.displayOrder - right.displayOrder);
}

function renderExistingImages(images) {
    if (images === null) {
        elements.existingCount.textContent = "Đang tải";
        elements.existingGallery.innerHTML = Array.from({ length: 3 }, () => '<div class="skeleton min-h-32"></div>').join("");
        return;
    }

    const normalizedImages = normalizeExistingImages(images);
    state.existingImages = normalizedImages;
    elements.existingCount.textContent = `${normalizedImages.length} ảnh hiện có`;

    if (!normalizedImages.length) {
        elements.existingGallery.innerHTML = `
            <div class="col-span-full rounded-xl border border-dashed border-zinc-300 bg-zinc-50 px-5 py-10 text-center">
                <strong class="block text-sm text-zinc-800">Xe chưa có hình ảnh</strong>
                <span class="mt-1 block text-sm text-zinc-500">Chọn ảnh bên dưới để bổ sung gallery cho xe.</span>
            </div>
        `;
        return;
    }

    elements.existingGallery.innerHTML = normalizedImages.map((image, index) => `
        <figure class="group relative overflow-hidden rounded-xl border border-zinc-200 bg-zinc-100 ${index === 0 ? "sm:col-span-2" : ""}">
            <img
                src="${escapeHtml(image.imageUrl)}"
                alt="Ảnh xe ${index + 1}"
                class="${index === 0 ? "aspect-[16/9]" : "aspect-[4/3]"} h-full w-full object-cover transition duration-300 group-hover:scale-[1.02]"
                loading="${index === 0 ? "eager" : "lazy"}"
            />
            <figcaption class="absolute inset-x-0 bottom-0 flex items-center justify-between gap-2 bg-zinc-950/70 px-3 py-2 text-xs font-medium text-white backdrop-blur-sm">
                <span>${image.isPrimary ? "Ảnh chính" : `Ảnh ${index + 1}`}</span>
                <span class="font-mono text-white/70">${index + 1}/${normalizedImages.length}</span>
            </figcaption>
        </figure>
    `).join("");
}

function renderImagePreview(files) {
    revokeAllPreviewUrls();
    elements.selectedPreview.innerHTML = "";
    elements.selectedSection.classList.toggle("hidden", files.length === 0);
    elements.clearImages.classList.toggle("hidden", files.length === 0);
    elements.selectedCount.textContent = files.length ? `${files.length} ảnh mới` : "";

    files.forEach(file => {
        const image = document.createElement("img");
        image.src = createPreviewUrl(file);
        image.alt = file.name;
        elements.selectedPreview.appendChild(image);
    });
}

async function handleImageChange() {
    const maxImageCount = 8;
    const availableSlots = Math.max(0, maxImageCount - state.existingImages.length);
    const files = Array.from(elements.images.files ?? []);
    const limitedFiles = files.slice(0, availableSlots);
    const result = validateSelectedImages(limitedFiles);

    if (files.length > availableSlots) {
        result.errors.push(`Xe chỉ được có tối đa ${maxImageCount} ảnh; bạn còn ${availableSlots} vị trí trống.`);
    }

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
        if (!state.carId && owners.length) elements.owner.value = String(owners[0].id);
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
    if (!state.carId) {
        renderExistingImages([]);
        return;
    }

    renderExistingImages(null);
    try {
        const car = await getCarById(state.carId);
        document.getElementById("carFormTitle").textContent = "Chỉnh sửa phương tiện";
        populateForm(car);
        renderExistingImages(car.images);
    } catch (error) {
        elements.existingCount.textContent = "Không tải được";
        elements.existingGallery.innerHTML = `<div class="col-span-full alert alert-error">${escapeHtml(error.message)}</div>`;
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

function clearSelectedImages() {
    state.selectedImages = [];
    state.compressedImages = [];
    elements.images.value = "";
    renderImagePreview([]);
}

function bindEvents() {
    elements.images?.addEventListener("change", handleImageChange);
    elements.carModel?.addEventListener("change", syncBrandModelFromModel);
    elements.form.addEventListener("submit", handleSubmit);
    elements.clearImages?.addEventListener("click", clearSelectedImages);
    document.getElementById("btnSaveDraft")?.addEventListener("click", () => showToast("Bản nháp chỉ được giữ trên UI vì DB chưa có trạng thái draft.", "info"));
}

async function init() {
    bindEvents();
    await loadLookups();
    await loadExistingCar();
}

if (cacheElements()) {
    init();
}
