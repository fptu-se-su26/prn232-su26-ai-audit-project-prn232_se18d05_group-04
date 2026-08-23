import { validateCarImage } from "./car-validation.js";

const previewUrls = new Set();

export function createPreviewUrl(file) {
    const url = URL.createObjectURL(file);
    previewUrls.add(url);
    return url;
}

export function revokePreviewUrl(url) {
    if (!url) return;
    URL.revokeObjectURL(url);
    previewUrls.delete(url);
}

export function revokeAllPreviewUrls() {
    previewUrls.forEach(url => URL.revokeObjectURL(url));
    previewUrls.clear();
}

export function validateSelectedImages(files) {
    const errors = [];
    const validFiles = [];

    files.forEach(file => {
        const error = validateCarImage(file);
        if (error) errors.push(`${file.name}: ${error}`);
        else validFiles.push(file);
    });

    return { validFiles, errors };
}

export function compressImage(file, maxWidth = 1200, quality = 0.75) {
    return new Promise((resolve, reject) => {
        const image = new Image();
        const reader = new FileReader();

        reader.onerror = () => reject(new Error("Unable to read image."));
        reader.onload = event => {
            image.src = event.target.result;
        };

        image.onerror = () => reject(new Error("Unable to load image."));
        image.onload = () => {
            const scale = image.width > maxWidth ? maxWidth / image.width : 1;
            const canvas = document.createElement("canvas");
            canvas.width = Math.round(image.width * scale);
            canvas.height = Math.round(image.height * scale);

            const context = canvas.getContext("2d");
            context.drawImage(image, 0, 0, canvas.width, canvas.height);

            canvas.toBlob(blob => {
                if (!blob) {
                    reject(new Error("Unable to compress image."));
                    return;
                }

                const compressed = new File([blob], file.name.replace(/\.[^.]+$/, ".jpg"), {
                    type: "image/jpeg",
                    lastModified: Date.now()
                });
                resolve(compressed);
            }, "image/jpeg", quality);
        };

        reader.readAsDataURL(file);
    });
}

export async function compressImages(files) {
    const output = [];

    for (const file of files) {
        try {
            output.push(await compressImage(file));
        } catch {
            output.push(file);
        }
    }

    return output;
}

window.addEventListener("beforeunload", revokeAllPreviewUrls);