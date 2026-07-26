import { fetchJson, sendJson } from "./shared/api-client.js";

const ROLE_LABELS = {
    Admin: "Quản trị viên",
    Customer: "Khách hàng",
    CarOwner: "Chủ xe"
};

const DOC_STATUS = {
    0: { label: "Chưa upload", badge: "neutral" },
    1: { label: "Chờ xác minh", badge: "warning" },
    2: { label: "Đã duyệt", badge: "success" },
    3: { label: "Bị từ chối", badge: "danger" }
};

function formatDate(dateString) {
    if (!dateString) return "";
    // If it's only date without time, just return as is (YYYY-MM-DD) for input fields
    return dateString.split('T')[0];
}

function formatDateDisplay(dateString) {
    if (!dateString) return "Chưa cập nhật";
    return new Date(dateString).toLocaleDateString("vi-VN");
}

async function loadProfileData() {
    try {
        // Fetch Profile and Documents concurrently
        const [profile, docs] = await Promise.all([
            fetchJson("users/profile").catch(() => null),
            fetchJson("driver-documents/my").catch(() => null)
        ]);

        if (!profile) {
            if (window.showToast) window.showToast("Không tải được thông tin cá nhân", "error");
            return;
        }

        // 1. Populate Avatar & Overview
        if (profile.avatarUrl) {
            document.getElementById("profileAvatar").src = profile.avatarUrl;
            document.getElementById("profileAvatar").style.display = "block";
            document.getElementById("profileAvatarFallback").style.display = "none";
        } else {
            document.getElementById("profileAvatar").style.display = "none";
            document.getElementById("profileAvatarFallback").style.display = "flex";
            document.getElementById("profileAvatarFallback").textContent = (profile.fullName?.[0] || 'U').toUpperCase();
        }

        document.getElementById("profileName").textContent = profile.fullName;
        document.getElementById("profileEmail").textContent = profile.email;
        if (document.getElementById("profileRole")) {
            document.getElementById("profileRole").textContent = ROLE_LABELS[profile.role] || profile.role;
        }
        document.getElementById("profileJoined").textContent = formatDateDisplay(profile.createdAt);
        
        const status = DOC_STATUS[profile.driverDocumentStatus] || DOC_STATUS[0];
        const badgeEl = document.getElementById("driverDocBadge");
        badgeEl.textContent = status.label;
        badgeEl.className = `status-badge status-${status.badge}`;

        // 2. Populate Personal Info Form
        document.getElementById("fullName").value = profile.fullName || "";
        document.getElementById("phoneNumber").value = profile.phoneNumber || "";
        document.getElementById("dateOfBirth").value = formatDate(profile.dateOfBirth);
        document.getElementById("address").value = profile.address || "";

        // 3. Populate Documents Form
        if (docs) {
            document.getElementById("citizenIdNumber").value = docs.citizenIdNumber || "";
            if (docs.citizenIdFrontImageUrl) {
                document.getElementById("citizenIdFrontUrl").value = docs.citizenIdFrontImageUrl;
                document.getElementById("preview-citizenIdFront").classList.remove('hidden');
                document.getElementById("preview-citizenIdFront").querySelector('img').src = docs.citizenIdFrontImageUrl;
            }
            if (docs.citizenIdBackImageUrl) {
                document.getElementById("citizenIdBackUrl").value = docs.citizenIdBackImageUrl;
                document.getElementById("preview-citizenIdBack").classList.remove('hidden');
                document.getElementById("preview-citizenIdBack").querySelector('img').src = docs.citizenIdBackImageUrl;
            }
            
            document.getElementById("driverLicenseNumber").value = docs.driverLicenseNumber || "";
            if (docs.driverLicenseFrontImageUrl) {
                document.getElementById("driverLicenseFrontUrl").value = docs.driverLicenseFrontImageUrl;
                document.getElementById("preview-driverLicenseFront").classList.remove('hidden');
                document.getElementById("preview-driverLicenseFront").querySelector('img').src = docs.driverLicenseFrontImageUrl;
            }
            if (docs.driverLicenseBackImageUrl) {
                document.getElementById("driverLicenseBackUrl").value = docs.driverLicenseBackImageUrl;
                document.getElementById("preview-driverLicenseBack").classList.remove('hidden');
                document.getElementById("preview-driverLicenseBack").querySelector('img').src = docs.driverLicenseBackImageUrl;
            }

            // Disable submit button if already approved or pending
            if (docs.verificationStatus === 1 || docs.verificationStatus === 2) {
                const btn = document.getElementById("submitDocBtn");
                btn.disabled = true;
                btn.textContent = docs.verificationStatus === 1 ? "Hồ sơ đang chờ duyệt" : "Hồ sơ đã được duyệt";
                document.getElementById("citizenIdFront").disabled = true;
                document.getElementById("citizenIdBack").disabled = true;
                document.getElementById("driverLicenseFront").disabled = true;
                document.getElementById("driverLicenseBack").disabled = true;

                if (docs.verificationStatus === 1) {
                    const cancelBtn = document.getElementById("cancelDocBtn");
                    cancelBtn.classList.remove("hidden");
                }
            }
        }

    } catch (error) {
        console.error("Profile load error", error);
    }
}

async function setupEvents() {
    // Helper for validation
    function showError(id, message) {
        const el = document.getElementById(`err-${id}`);
        if (el) {
            el.textContent = message;
            el.classList.remove('hidden');
        }
    }
    function clearError(id) {
        const el = document.getElementById(`err-${id}`);
        if (el) {
            el.textContent = '';
            el.classList.add('hidden');
        }
    }

    // A. Personal Info Form Submit
    document.getElementById("personalInfoForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        
        let hasError = false;
        
        const fullName = document.getElementById("fullName").value.trim();
        if (!fullName) {
            showError("fullName", "Vui lòng nhập họ và tên");
            hasError = true;
        } else if (fullName.length < 2) {
            showError("fullName", "Họ và tên phải có ít nhất 2 ký tự");
            hasError = true;
        } else clearError("fullName");
        
        const phone = document.getElementById("phoneNumber").value.trim();
        if (phone && !/^(0|\+84)[3|5|7|8|9][0-9]{8}$/.test(phone)) {
            showError("phoneNumber", "Số điện thoại không hợp lệ (10 chữ số, bắt đầu bằng 0 hoặc +84)");
            hasError = true;
        } else clearError("phoneNumber");

        if (hasError) return;

        const btn = document.getElementById("saveProfileBtn");
        btn.disabled = true;
        btn.textContent = "Đang lưu...";
        
        try {
            await sendJson("users/profile", {
                fullName: document.getElementById("fullName").value,
                phoneNumber: phone,
                dateOfBirth: document.getElementById("dateOfBirth").value || null,
                address: document.getElementById("address").value
            }, { method: "PUT" });
            
            if (window.showToast) window.showToast("Lưu thông tin cá nhân thành công", "success");
            
            document.getElementById("profileName").textContent = document.getElementById("fullName").value;
        } catch (err) {
            if (window.showToast) window.showToast(err.message, "error");
        } finally {
            btn.disabled = false;
            btn.textContent = "Lưu thay đổi";
        }
    });

    // Helper for Document Upload
    async function handleDocumentUpload(fileInputId, hiddenInputId, previewId) {
        const fileInput = document.getElementById(fileInputId);
        fileInput.addEventListener("change", async (e) => {
            const file = e.target.files[0];
            if (!file) return;

            if (window.showToast) window.showToast("Đang tải ảnh lên...", "neutral");
            
            const formData = new FormData();
            formData.append("file", file);

            try {
                const { authService } = await import("./shared/auth-service.js");
                const headers = new Headers();
                if (localStorage.getItem("vivucar_token")) {
                    headers.set("Authorization", `Bearer ${localStorage.getItem("vivucar_token")}`);
                }
                
                const response = await fetch(`http://localhost:5119/api/uploads?folder=documents`, {
                    method: "POST",
                    headers: headers,
                    body: formData
                });

                if (!response.ok) {
                    throw new Error("Lỗi tải ảnh: " + await response.text());
                }
                
                const data = await response.json();
                document.getElementById(hiddenInputId).value = data.publicUrl;
                
                const preview = document.getElementById(`preview-${fileInputId}`);
                preview.querySelector('img').src = data.publicUrl;
                preview.classList.remove('hidden');
                
                if (window.showToast) window.showToast("Tải ảnh thành công", "success");
            } catch (err) {
                if (window.showToast) window.showToast(err.message, "error");
            }
        });
    }

    handleDocumentUpload("citizenIdFront", "citizenIdFrontUrl");
    handleDocumentUpload("citizenIdBack", "citizenIdBackUrl");
    handleDocumentUpload("driverLicenseFront", "driverLicenseFrontUrl");
    handleDocumentUpload("driverLicenseBack", "driverLicenseBackUrl");

    // Cancel pending document
    document.getElementById("cancelDocBtn").addEventListener("click", async () => {
        if (!confirm("Bạn có chắc chắn muốn hủy yêu cầu duyệt hồ sơ hiện tại?")) return;

        try {
            const { authService } = await import("./shared/auth-service.js");
            const headers = new Headers();
            if (localStorage.getItem("vivucar_token")) {
                headers.set("Authorization", `Bearer ${localStorage.getItem("vivucar_token")}`);
            }

            const response = await fetch("/api/proxy/driver-documents/my/cancel", {
                method: "POST",
                headers: headers
            });

            if (!response.ok) {
                const errText = await response.text();
                throw new Error("Lỗi khi hủy hồ sơ: " + errText);
            }

            if (window.showToast) window.showToast("Đã hủy chờ duyệt thành công", "success");
            setTimeout(() => window.location.reload(), 1000);
        } catch (err) {
            if (window.showToast) window.showToast(err.message, "error");
        }
    });

    // B. Driver Document Form Submit
    document.getElementById("documentForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        
        let hasError = false;
        
        const cccd = document.getElementById("citizenIdNumber").value;
        if (!/^[0-9]{12}$/.test(cccd)) {
            showError("citizenIdNumber", "CCCD phải bao gồm đúng 12 chữ số.");
            hasError = true;
        } else clearError("citizenIdNumber");

        const gplx = document.getElementById("driverLicenseNumber").value.trim();
        if (!/^[0-9]{12}$/.test(gplx)) {
            showError("driverLicenseNumber", "Số GPLX phải bao gồm đúng 12 chữ số.");
            hasError = true;
        } else clearError("driverLicenseNumber");

        const citizenIdFrontUrl = document.getElementById("citizenIdFrontUrl").value;
        const citizenIdBackUrl = document.getElementById("citizenIdBackUrl").value;
        const driverLicenseFrontUrl = document.getElementById("driverLicenseFrontUrl").value;
        const driverLicenseBackUrl = document.getElementById("driverLicenseBackUrl").value;

        if (!citizenIdFrontUrl || !citizenIdBackUrl || !driverLicenseFrontUrl || !driverLicenseBackUrl) {
            if (window.showToast) window.showToast("Vui lòng tải lên đầy đủ 4 ảnh giấy tờ", "error");
            hasError = true;
        }

        if (hasError) return;

        const btn = document.getElementById("submitDocBtn");
        btn.disabled = true;
        btn.textContent = "Đang gửi...";
        
        try {
            await sendJson("driver-documents/my/submit", {
                citizenIdNumber: cccd,
                citizenIdFrontImageUrl: document.getElementById("citizenIdFrontUrl").value,
                citizenIdBackImageUrl: document.getElementById("citizenIdBackUrl").value,
                driverLicenseNumber: gplx,
                driverLicenseFrontImageUrl: document.getElementById("driverLicenseFrontUrl").value,
                driverLicenseBackImageUrl: document.getElementById("driverLicenseBackUrl").value
            }, { method: "POST" });
            
            if (window.showToast) window.showToast("Gửi hồ sơ thành công", "success");
            setTimeout(() => window.location.reload(), 1500);
        } catch (err) {
            if (window.showToast) window.showToast(err.message, "error");
            btn.disabled = false;
            btn.textContent = "Gửi yêu cầu xác minh";
        }
    });

    // C. Avatar Upload Event
    document.getElementById("avatarFileInput").addEventListener("change", async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        // Show uploading toast
        if (window.showToast) window.showToast("Đang tải ảnh lên...", "neutral");
        
        const formData = new FormData();
        formData.append("file", file);

        try {
            // Import authService dynamically just for getting the token
            const { authService } = await import("./shared/auth-service.js");
            const headers = new Headers();
            if (localStorage.getItem("vivucar_token")) {
                headers.set("Authorization", `Bearer ${localStorage.getItem("vivucar_token")}`);
            }
            
            const response = await fetch("/api/proxy/users/profile/avatar", {
                method: "POST",
                headers: headers,
                body: formData
            });

            if (!response.ok) {
                const errText = await response.text();
                throw new Error("Lỗi tải ảnh: " + errText);
            }
            
            const data = await response.json();
            
            if (window.showToast) window.showToast("Cập nhật ảnh thành công", "success");
            
            // Update UI immediately
            document.getElementById("profileAvatar").src = data.avatarUrl;
            document.getElementById("profileAvatar").style.display = "block";
            document.getElementById("profileAvatarFallback").style.display = "none";
            
        } catch (err) {
            if (window.showToast) window.showToast(err.message, "error");
        }
    });
}

document.addEventListener("DOMContentLoaded", async () => {
    loadProfileData();
    setupEvents();
});
