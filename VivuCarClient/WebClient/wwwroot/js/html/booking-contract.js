/**
 * booking-contract.js
 * Logic for Contract Signature page
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
    const U = window.VivuCarUtils;
    const session = await authService.getValidSession();
    const currentUser = session?.user ?? authService.getUser();

    // Patch header
    function patchHeader(user) {
        if (!user) return;
        if (window.VivuCarLayout?.patchHeaderForRealUser) {
            window.VivuCarLayout.patchHeaderForRealUser(user);
            return;
        }
        const headerMount = document.getElementById('headerMount');
        if (!headerMount) return;
        const loginLink = headerMount.querySelector('a[href="/Login"], a[href*="login"]');
        if (loginLink) {
            loginLink.textContent = user.fullName || user.email || 'Tài khoản';
            loginLink.href = '/Profile';
            loginLink.classList.remove('btn-primary');
            loginLink.classList.add('btn-secondary');
        }
    }
    requestAnimationFrame(() => patchHeader(currentUser));
    setTimeout(() => patchHeader(currentUser), 300);

    if (!currentUser) {
        location.href = '/';
        return;
    }

    const bookingId = Number(new URLSearchParams(location.search).get("id"));
    const root = U.byId("contractRoot");

    if (!bookingId) {
        root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy", text: "Thiếu ID đơn thuê.", href: "/Booking/MyBookings", action: "Về danh sách" });
        return;
    }

    let booking = null;

    try {
        const r = await authService.apiFetch(`bookings/${bookingId}`);
        if (!r.ok) throw new Error("Failed to fetch");
        booking = await r.json();
        render(booking);
    } catch (e) {
        root.innerHTML = U.renderEmptyState({ title: "Lỗi", text: "Không thể tải đơn thuê.", href: "/Booking/MyBookings", action: "Về danh sách" });
    }

    function render(b) {
        // We use the pdfUrl from booking.contractPdfUrl if exists, or fallback
        let contractUrl = b.contractPdfUrl || `/api/bookings/${bookingId}/contract/pdf`;
        if (contractUrl.startsWith('/api')) {
            contractUrl = (window.API_BASE_URL || 'http://localhost:5246') + contractUrl;
        }

        const isSigned = !!b.contractPdfUrl?.includes('sig=');

        // Render iframe and signature pad
        root.innerHTML = `
            <div style="margin-bottom: 24px;">
                <h1 style="font-size: 1.5rem; font-weight: 700; color: #1f1f1f; margin-bottom: 8px;">Ký Hợp Đồng Thuê Xe</h1>
                <p style="color: #6b7280; font-size: 0.875rem;">Đơn #${b.bookingCode || b.id} · Xe: ${b.carName}</p>
            </div>

            <div style="border: 1px solid #e5e7eb; border-radius: 8px; overflow: hidden; height: 400px; margin-bottom: 24px;">
                <iframe src="${contractUrl}" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>

            ${isSigned ? `
                <div style="text-align: center; padding: 20px; background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 8px;">
                    <p style="color: #16a34a; font-weight: 600;">Hợp đồng này đã được ký.</p>
                    <a href="/Booking/Detail?id=${bookingId}" class="btn btn-primary" style="margin-top: 12px; display: inline-block;">Quay về đơn hàng</a>
                </div>
            ` : `
                <div class="signature-pad-wrapper">
                    <h3 style="font-size: 1rem; font-weight: 600; color: #374151; margin-bottom: 8px;">Chữ ký của bạn (Bên B)</h3>
                    <p style="font-size: 0.8125rem; color: #6b7280; margin-bottom: 12px;">Dùng chuột hoặc ngón tay để ký vào khung bên dưới</p>
                    <canvas id="signatureCanvas" class="signature-pad" width="600" height="200"></canvas>
                    <div style="margin-top: 12px; display: flex; gap: 12px; justify-content: center;">
                        <button type="button" id="btnClear" class="btn btn-secondary">Xóa chữ ký</button>
                        <button type="button" id="btnSubmit" class="btn btn-primary">Ký & Hoàn tất</button>
                    </div>
                </div>
            `}
        `;

        if (!isSigned) {
            initSignaturePad();
        }
    }

    function initSignaturePad() {
        const canvas = document.getElementById("signatureCanvas");
        const ctx = canvas.getContext("2d");
        const btnClear = document.getElementById("btnClear");
        const btnSubmit = document.getElementById("btnSubmit");

        let isDrawing = false;
        let lastX = 0;
        let lastY = 0;

        // Clear canvas with white background
        function clearCanvas() {
            ctx.fillStyle = "#ffffff";
            ctx.fillRect(0, 0, canvas.width, canvas.height);
        }
        clearCanvas();

        function getMousePos(canvas, evt) {
            const rect = canvas.getBoundingClientRect();
            // Scale mouse coordinates to canvas coordinates
            const scaleX = canvas.width / rect.width;
            const scaleY = canvas.height / rect.height;
            return {
                x: (evt.clientX - rect.left) * scaleX,
                y: (evt.clientY - rect.top) * scaleY
            };
        }

        function getTouchPos(canvas, evt) {
            const rect = canvas.getBoundingClientRect();
            const scaleX = canvas.width / rect.width;
            const scaleY = canvas.height / rect.height;
            return {
                x: (evt.touches[0].clientX - rect.left) * scaleX,
                y: (evt.touches[0].clientY - rect.top) * scaleY
            };
        }

        function startDrawing(x, y) {
            isDrawing = true;
            lastX = x;
            lastY = y;
        }

        function draw(x, y) {
            if (!isDrawing) return;
            ctx.beginPath();
            ctx.moveTo(lastX, lastY);
            ctx.lineTo(x, y);
            ctx.strokeStyle = "#1f1f1f";
            ctx.lineWidth = 3;
            ctx.lineCap = "round";
            ctx.stroke();
            lastX = x;
            lastY = y;
        }

        function stopDrawing() {
            isDrawing = false;
        }

        // Mouse events
        canvas.addEventListener("mousedown", (e) => {
            const pos = getMousePos(canvas, e);
            startDrawing(pos.x, pos.y);
        });
        canvas.addEventListener("mousemove", (e) => {
            const pos = getMousePos(canvas, e);
            draw(pos.x, pos.y);
        });
        canvas.addEventListener("mouseup", stopDrawing);
        canvas.addEventListener("mouseout", stopDrawing);

        // Touch events
        canvas.addEventListener("touchstart", (e) => {
            e.preventDefault();
            const pos = getTouchPos(canvas, e);
            startDrawing(pos.x, pos.y);
        }, { passive: false });
        canvas.addEventListener("touchmove", (e) => {
            e.preventDefault();
            const pos = getTouchPos(canvas, e);
            draw(pos.x, pos.y);
        }, { passive: false });
        canvas.addEventListener("touchend", stopDrawing);
        canvas.addEventListener("touchcancel", stopDrawing);

        btnClear.addEventListener("click", clearCanvas);

        btnSubmit.addEventListener("click", async () => {
            btnSubmit.disabled = true;
            btnSubmit.textContent = "Đang xử lý...";

            try {
                // Convert to Blob
                const blob = await new Promise(resolve => canvas.toBlob(resolve, 'image/png'));
                
                // Check if canvas is actually empty (optional heuristic, here we just assume user drew something)

                // Upload to Cloudinary via our backend API
                const formData = new FormData();
                formData.append('file', blob, 'signature.png');

                const uploadRes = await authService.apiFetch('uploads?folder=signatures', {
                    method: 'POST',
                    body: formData
                });

                if (!uploadRes.ok) {
                    throw new Error("Không thể upload chữ ký.");
                }

                const uploadData = await uploadRes.json();
                const signatureUrl = uploadData.url;

                // Call sign contract API
                const signRes = await authService.apiFetch(`bookings/${bookingId}/contract/sign`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ signatureUrl: signatureUrl })
                });

                if (!signRes.ok) {
                    throw new Error("Ký hợp đồng thất bại.");
                }

                U.renderToast("Hợp đồng đã được ký thành công!", "success");
                setTimeout(() => {
                    location.href = `/Booking/Detail?id=${bookingId}`;
                }, 1500);

            } catch (err) {
                U.renderToast(err.message || "Đã xảy ra lỗi, vui lòng thử lại.", "danger");
                btnSubmit.disabled = false;
                btnSubmit.textContent = "Ký & Hoàn tất";
            }
        });
    }
})();
