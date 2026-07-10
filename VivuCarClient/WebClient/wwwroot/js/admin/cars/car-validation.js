const ALLOWED_IMAGE_TYPES = new Set(["image/jpeg", "image/png", "image/webp"]);
const MAX_IMAGE_SIZE = 5 * 1024 * 1024;

export function validateCarForm(data, options = {}) {
    const errors = {};
    const currentYear = new Date().getFullYear();

    if (!data.owner_id) errors.owner_id = "Owner is required.";
    if (!data.license_plate) errors.license_plate = "License plate is required.";
    if (!data.car_model_id && !data.brand) errors.brand = "Brand is required.";
    if (!data.car_model_id && !data.model) errors.model = "Model is required.";
    if (!data.address) errors.address = "Pickup address is required.";
    if (!data.seats || Number(data.seats) <= 0) errors.seats = "Seats must be greater than 0.";
    if (Number(data.price_per_day) <= 0) errors.price_per_day = "Daily price must be greater than 0.";
    if (Number(data.price_per_hours) <= 0) errors.price_per_hours = "Hourly price must be greater than 0.";
    if (data.year && (Number(data.year) < 1900 || Number(data.year) > currentYear + 1)) errors.year = "Year is outside the allowed range.";
    if (data.color && data.color.length > 50) errors.color = "Color must be 50 characters or fewer.";
    if (Number(data.kilometers_driven) < 0) errors.kilometers_driven = "Kilometers driven cannot be negative.";
    if (!options.isEdit && !options.imageCount) errors.car_images = "At least one image is required.";

    return {
        valid: Object.keys(errors).length === 0,
        errors
    };
}

export function validateCarImage(file) {
    if (!ALLOWED_IMAGE_TYPES.has(file.type)) {
        return "Only JPEG, PNG, or WebP images are allowed.";
    }

    if (file.size > MAX_IMAGE_SIZE) {
        return "Each image must be 5MB or smaller.";
    }

    return null;
}
