import { fetchJson } from "../../shared/api-client.js";
import { escapeHtml } from "../../shared/dom.js";
import { closeModal, openModal } from "../../shared/modal.js";
import { showToast } from "../../shared/toast.js";

const root = document.querySelector("[data-admin-licenses-page]");
const body = document.getElementById("licenseModerationTableBody");
const detailModal = document.getElementById("licenseDetailModal");
const imageModal = document.getElementById("licenseImageModal");
let items = [];
let activeLicenseId = null;

function formatDate(value) {
    return value ? new Date(value).toLocaleString("vi-VN") : "—";
}

function statusLabel(value) {
    return { pending: "Chờ duyệt", approved: "Đã duyệt", rejected: "Đã từ chối" }[value] || value;
}

function statusTone(value) {
    return value === "approved" ? "success" : value === "rejected" ? "danger" : "warning";
}

function imageButton(item) {
    return item.frontImageUrl
        ? '<button class="btn btn-ghost btn-sm" type="button" data-license-image-preview="' + item.id + '">Xem ảnh</button>'
        : '<span class="text-zinc-400">Chưa có ảnh</span>';
}

function renderSummary() {
    const rows = [
        ["Tổng hồ sơ", items.length],
        ["Chờ duyệt", items.filter(item => item.status === "pending").length],
        ["Đã duyệt", items.filter(item => item.status === "approved").length],
        ["Đã từ chối", items.filter(item => item.status === "rejected").length]
    ];
    document.getElementById("licenseModerationSummary").innerHTML = rows.map(row =>
        '<article class="summary-card"><span>' + row[0] + "</span><strong>" + row[1] + "</strong></article>"
    ).join("");
}

function render() {
    renderSummary();
    document.getElementById("licenseModerationEmpty").classList.toggle("hidden", items.length > 0);
    body.closest(".table-wrap").classList.toggle("hidden", items.length === 0);
    body.innerHTML = items.map(item => {
        const reject = item.status !== "rejected"
            ? '<button class="btn btn-danger btn-sm" type="button" data-license-action="reject" data-license-id="' + item.id + '">Từ chối</button>'
            : "";
        return '<tr><td><strong>' + escapeHtml(item.userName) + '</strong><small class="block text-zinc-500">' + escapeHtml(item.driverLicenseNumber) + "</small></td>" +
            "<td>" + escapeHtml(item.email) + "</td>" +
            '<td>' + imageButton(item) + "</td>" +
            "<td>" + escapeHtml(formatDate(item.createdAt)) + "</td>" +
            '<td><span class="status-badge status-' + statusTone(item.status) + '">' + escapeHtml(statusLabel(item.status)) + "</span></td>" +
            '<td><div class="flex gap-2"><button class="btn btn-ghost btn-sm" type="button" data-view-license="' + item.id + '">Xem</button>' + reject + "</div></td></tr>";
    }).join("");
}

function openLicenseImage(id) {
    const item = items.find(entry => entry.id === Number(id));
    if (!item?.frontImageUrl) return showToast("Hồ sơ chưa có ảnh GPLX.", "error");

    const hasBack = Boolean(item.backImageUrl);
    const card = document.getElementById("licenseFlipCard");
    document.getElementById("licenseImageTitle").textContent = "Ảnh GPLX · " + item.userName;
    document.getElementById("licenseImageFront").src = item.frontImageUrl;
    document.getElementById("licenseImageBack").src = item.backImageUrl || item.frontImageUrl;
    card.dataset.hasBack = String(hasBack);
    card.classList.remove("is-flipped");
    card.setAttribute("aria-label", hasBack ? "Lật sang mặt sau GPLX" : "Ảnh mặt trước GPLX");
    document.getElementById("btnFlipLicenseImage").classList.toggle("hidden", !hasBack);
    updateLicenseImageSide(false);
    openModal("licenseImageModal");
    window.requestAnimationFrame(() => imageModal.querySelector("[data-close-license-image]")?.focus());
}

function updateLicenseImageSide(isBack) {
    const card = document.getElementById("licenseFlipCard");
    const side = document.getElementById("licenseImageSide");
    const button = document.getElementById("btnFlipLicenseImage");
    card.classList.toggle("is-flipped", isBack);
    side.textContent = isBack ? "Mặt sau" : "Mặt trước";
    button.textContent = isBack ? "Xem mặt trước" : "Xem mặt sau";
    card.setAttribute("aria-label", isBack ? "Lật sang mặt trước GPLX" : "Lật sang mặt sau GPLX");
}

function toggleLicenseImage() {
    const card = document.getElementById("licenseFlipCard");
    if (card.dataset.hasBack !== "true") return;
    updateLicenseImageSide(!card.classList.contains("is-flipped"));
}

function closeLicenseImage() {
    closeModal("licenseImageModal");
    document.getElementById("licenseFlipCard").classList.remove("is-flipped");
}
function documentPreview(url, label) {
    if (!url) {
        return '<div class="license-document-missing"><strong>' + label + "</strong><span>Chưa có ảnh</span></div>";
    }
    return '<button class="license-document-preview" type="button" data-license-image="' + escapeHtml(url) + '">' +
        '<img src="' + escapeHtml(url) + '" alt="' + label + ' giấy phép lái xe" loading="lazy">' +
        '<span><strong>' + label + "</strong><small>Nhấn để xem ảnh lớn</small></span></button>";
}

function ocrField(label, value) {
    const hasValue = Boolean(value);
    return '<div class="license-ocr-field"><div><span>' + label + '</span><strong class="' + (hasValue ? "" : "is-missing") + '">' +
        escapeHtml(value || "Chưa quét được dữ liệu") + '</strong></div><button class="btn btn-ghost btn-sm" type="button" data-copy-license-value="' +
        escapeHtml(value || "") + '"' + (hasValue ? "" : " disabled") + ">Sao chép</button></div>";
}

function renderLicenseOcr(item) {
    const result = item.ocr;
    document.getElementById("licenseOcrFields").innerHTML = [
        ocrField("Số GPLX người dùng khai", item.driverLicenseNumber),
        ocrField("Họ và tên OCR", result?.fullName),
        ocrField("Số GPLX OCR", result?.licenseNumber),
        ocrField("Ngày sinh OCR", result?.dateOfBirth),
        ocrField("Hạng GPLX OCR", result?.licenseClass),
        ocrField("Ngày hết hạn OCR", result?.expiryDate)
    ].join("");

    const confidence = document.getElementById("licenseOcrConfidence");
    confidence.classList.toggle("hidden", !result);
    confidence.textContent = result ? Number(result.confidence).toLocaleString("vi-VN", { maximumFractionDigits: 1 }) + "% tin cậy" : "";
    document.getElementById("btnScanLicenseOcr").textContent = result ? "Quét lại OCR" : "Quét OCR";
}

function viewLicense(id) {
    const item = items.find(entry => entry.id === Number(id));
    if (!item) return;

    activeLicenseId = item.id;
    document.getElementById("licenseProfileMeta").textContent = item.userName + " · " + item.email + " · Gửi " + formatDate(item.createdAt);
    const status = document.getElementById("licenseReviewStatus");
    status.className = "status-badge status-" + statusTone(item.status);
    status.textContent = statusLabel(item.status);
    document.getElementById("licenseDocumentImages").innerHTML = documentPreview(item.frontImageUrl, "Mặt trước");
    renderLicenseOcr(item);
    document.getElementById("licenseAdminNote").value = "";
    document.getElementById("btnLicenseApprove").disabled = item.status === "approved";
    document.getElementById("btnLicenseMismatch").disabled = item.status === "rejected";
    openModal("licenseDetailModal");
    window.requestAnimationFrame(() => detailModal.querySelector("[data-close-modal]")?.focus());
}

async function scanLicenseOcr() {
    const item = items.find(entry => entry.id === activeLicenseId);
    if (!item) return;
    if (!item.frontImageUrl) return showToast("Hồ sơ chưa có ảnh GPLX mặt trước.", "error");

    const button = document.getElementById("btnScanLicenseOcr");
    button.disabled = true;
    button.textContent = "Đang quét OCR...";
    try {
        item.ocr = await fetchJson("admin/moderation/licenses/" + item.id + "/ocr", { method: "POST" });
        if (activeLicenseId === item.id) renderLicenseOcr(item);
        showToast("Đã quét OCR. Hãy đối chiếu lại với ảnh gốc.", "success");
    } catch (error) {
        showToast(error.message, "error");
    } finally {
        button.disabled = false;
        button.textContent = item.ocr ? "Quét lại OCR" : "Quét OCR";
    }
}
function setReviewButtonsDisabled(disabled) {
    ["btnLicenseApprove", "btnLicenseMismatch", "btnLicenseLookupUnavailable"].forEach(id => {
        document.getElementById(id).disabled = disabled;
    });
}

async function updateStatus(id, action) {
    const label = action === "approve" ? "xác nhận hợp lệ" : "đánh dấu thông tin không khớp";
    if (!window.confirm("Xác nhận " + label + " cho GPLX này?")) return;
    setReviewButtonsDisabled(true);
    try {
        await fetchJson("admin/moderation/licenses/" + id + "/" + action, { method: "PATCH" });
        showToast(action === "approve" ? "Đã duyệt GPLX." : "Đã từ chối GPLX.", "success");
        closeModal("licenseDetailModal");
        await load();
    } catch (error) {
        showToast(error.message, "error");
    } finally {
        setReviewButtonsDisabled(false);
    }
}

async function copyLicenseValue(button) {
    const value = button.dataset.copyLicenseValue;
    if (!value) return;
    try {
        await navigator.clipboard.writeText(value);
        const previous = button.textContent;
        button.textContent = "Đã sao chép";
        window.setTimeout(() => { button.textContent = previous; }, 1200);
    } catch {
        showToast("Không thể sao chép. Hãy sao chép thủ công.", "error");
    }
}

async function load() {
    body.innerHTML = '<tr><td colspan="6" class="empty-cell">Đang tải hồ sơ GPLX...</td></tr>';
    try {
        const data = await fetchJson("admin/moderation/licenses");
        items = Array.isArray(data) ? data : data.items ?? [];
        render();
    } catch (error) {
        body.innerHTML = '<tr><td colspan="6" class="empty-cell">' + escapeHtml(error.message) + "</td></tr>";
    }
}

if (root) {
    document.body.append(detailModal, imageModal);

    body.addEventListener("click", event => {
        const preview = event.target.closest("[data-license-image-preview]");
        if (preview) return openLicenseImage(preview.dataset.licenseImagePreview);
        const view = event.target.closest("[data-view-license]");
        if (view) return viewLicense(view.dataset.viewLicense);
        const action = event.target.closest("[data-license-action]");
        if (action) updateStatus(action.dataset.licenseId, action.dataset.licenseAction);
    });

    detailModal.addEventListener("click", event => {
        const close = event.target.closest("[data-close-modal]");
        if (close || event.target === detailModal) return closeModal("licenseDetailModal");
        const image = event.target.closest("[data-license-image]");
        if (image) return openLicenseImage(activeLicenseId);
        const copy = event.target.closest("[data-copy-license-value]");
        if (copy) return copyLicenseValue(copy);
    });

    imageModal.addEventListener("click", event => {
        if (event.target === imageModal || event.target.closest("[data-close-license-image]")) return closeLicenseImage();
        if (event.target.closest("#licenseFlipCard")) toggleLicenseImage();
    });
    document.getElementById("btnFlipLicenseImage").addEventListener("click", toggleLicenseImage);
    document.getElementById("btnScanLicenseOcr").addEventListener("click", scanLicenseOcr);
    document.getElementById("btnLicenseApprove").addEventListener("click", () => activeLicenseId && updateStatus(activeLicenseId, "approve"));
    document.getElementById("btnLicenseMismatch").addEventListener("click", () => activeLicenseId && updateStatus(activeLicenseId, "reject"));
    document.getElementById("btnLicenseLookupUnavailable").addEventListener("click", () => {
        // UI-only state. Not present in the current DB schema. Requires migration before backend integration.
        showToast("API hiện tại chưa hỗ trợ lưu trạng thái không tra cứu được.", "error");
    });
    document.addEventListener("keydown", event => {
        if (event.key !== "Escape") return;
        if (imageModal.classList.contains("is-open")) return closeLicenseImage();
        if (detailModal.classList.contains("is-open")) closeModal("licenseDetailModal");
    });
    load();
}