/**
 * booking-detail.js
 * Uses real API for booking details
 */
import { authService } from '/js/shared/auth-service.js';

(async function () {
  const U = window.VivuCarUtils;
  
  const session = await authService.refresh();
  const currentUser = session?.user ?? null;
  if (!currentUser) {
    location.href = '/';
    return;
  }
  
  const bookingId = Number(new URLSearchParams(location.search).get("id")) || Number(new URLSearchParams(location.search).get("bookingId"));
  const root = U.byId("bookingDetailRoot");

  let booking = null;
  let paymentStatus = null;
  let currentReview = null;

  async function loadData() {
    if (!bookingId) {
      renderError();
      return;
    }
    
    root.innerHTML = `<div style="text-align:center;padding:48px;color:#6b7280">Đang tải thông tin đơn...</div>`;
    try {
      const r = await authService.apiFetch(`bookings/${bookingId}`);
      if (!r.ok) throw new Error('Booking not found');
      booking = await r.json();
      
      try {
        const pr = await authService.apiFetch(`payments/status/${bookingId}`);
        if (pr.ok) {
          paymentStatus = await pr.json();
        }
      } catch (e) {
        console.warn("Could not fetch payment status for", bookingId);
      }
      
      const normalizedStatus = (booking.status || "").toLowerCase();
      if (normalizedStatus === "completed") {
        try {
          const rr = await authService.apiFetch(`reviews/booking/${bookingId}`);
          if (rr.ok) {
            currentReview = await rr.json();
          }
        } catch(e) {
          console.warn("Could not fetch review for", bookingId);
        }
      }
      
      render();
    } catch (e) {
      console.error(e);
      renderError();
    }
  }

  function renderError() {
    root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy đơn", text: "Đơn không tồn tại hoặc lỗi kết nối.", href: "/Booking/MyBookings", action: "Về danh sách đơn" });
  }

  function resolveUiState() {
    const paid = paymentStatus?.paymentStatus === 'success';
    let key = "unknown", label = "Không xác định", tone = "neutral";
    
    const s = (booking.status || "").toLowerCase();
    if (s === "cancelled") { key = "cancelled"; label = "Đã hủy"; tone = "danger"; }
    else if (s === "rejected") { key = "rejected"; label = "Đã từ chối"; tone = "danger"; }
    else if (s === "completed") { key = "completed"; label = "Hoàn tất"; tone = "success"; }
    else if (s === "pending" || s === "pendingapproval") {
      if (paid) { key = "handover_pending"; label = "Đã cọc - chờ xác nhận"; tone = "primary"; }
      else { key = "payment_pending"; label = "Chờ thanh toán"; tone = "warning"; }
    }
    else if (s === "returnrequested") { key = "return_requested"; label = "Đang chờ trả xe"; tone = "warning"; }
    else if (s === "approved" || s === "waitingdeposit" || s === "waitingpickup" || s === "inprogress") {
      const pickupDate = new Date(booking.startDateTime);
      if (Date.now() < pickupDate.getTime()) { key = "handover_pending"; label = "Chờ bàn giao"; tone = "primary"; }
      else { key = "renting"; label = "Đang thuê"; tone = "primary"; }
    }
    return { key, label, tone };
  }

  function render() {
    const ui = resolveUiState();
    
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Đơn thuê", href: "/Booking/MyBookings" },
      { label: `#${booking.bookingCode || booking.id}` }
    ]);
    
    const paid = paymentStatus?.paymentStatus === 'success';
    const s = (booking.status || "").toLowerCase();
    const canCancel = (s === "pending" || s === "pendingapproval" || s === "waitingdeposit") && !paid;
    
    root.innerHTML = `
      <div class="detail-document">
        <section class="checkout-main">
          <div class="checkout-section">
            <div class="booking-toolbar">
              <div>
                <span class="eyebrow">Booking #${booking.bookingCode || booking.id}</span>
                <h1>${booking.carName}</h1>
                <p class="muted">${ui.label}</p>
              </div>
              <div class="chip-row">${U.renderStatusBadge(ui.tone, ui.label)}</div>
            </div>
            <div class="checkout-car">
              <img src="${booking.carImageUrl || '/img/placeholder-car.png'}" alt="${booking.carName}" style="object-fit:cover">
              <div class="info-grid">
                ${info("Biển số", booking.licensePlate)}
                ${info("Địa điểm nhận", booking.pickupLocation)}
                ${info("Nhận xe", new Date(booking.startDateTime).toLocaleString('vi-VN'))}
                ${info("Trả xe", new Date(booking.endDateTime).toLocaleString('vi-VN'))}
                ${info("Thời gian thuê", `${booking.rentalDays || 0} ngày ${booking.rentalHours ? `và ${booking.rentalHours} giờ` : ''}`)}
                ${info("Tổng tiền", U.formatVnd(booking.totalAmount))}
                ${info("Voucher", booking.voucherCode || "Không áp dụng")}
              </div>
            </div>
          </div>
          <div class="checkout-section">
            <h2>Thanh toán tiền cọc</h2>
            <div class="info-grid">
              ${info("Trạng thái", paymentStatus?.paymentStatus || (paid ? "Thành công" : "Chờ thanh toán"))}
              ${info("Mã giao dịch", paymentStatus?.transactionCode || "—")}
              ${info("Cần cọc", U.formatVnd(booking.depositAmount))}
            </div>
          </div>
        </section>
        <aside class="summary-card">
          <h2>Thời gian biểu</h2>
          <div class="timeline">${timeline()}</div>
          <div class="booking-actions mt-3.5">
            ${s === "returnrequested" ? `<div style="padding:12px;background:#fffbe6;border:1px solid #ffe58f;color:#873800;border-radius:8px;font-size:13px;line-height:1.5;font-weight:500;">⏳ <strong>Đã gửi yêu cầu trả xe:</strong> Vui lòng chờ chủ xe kiểm tra xe và xác nhận hoàn tất thủ tục bàn giao lại.</div>` : ""}
            ${s === "pending" && !paid ? `<a class="btn btn-primary btn-sm" href="/Payment/Deposit?bookingId=${booking.id}">Thanh toán cọc</a>` : ""}
            ${canCancel ? `<button class="btn btn-danger btn-sm" type="button" id="cancelBookingBtn">Hủy đơn</button>` : ""}
            ${ui.key === "renting" ? `<button class="btn btn-primary btn-sm" type="button" id="requestReturnBtn">Yêu cầu trả xe</button>` : ""}
            ${booking.contractPdfUrl && !booking.contractPdfUrl.includes('sig=') ? `<a class="btn btn-primary btn-sm" href="/Booking/Contract?id=${booking.id}">Ký hợp đồng</a>` : ""}
            ${booking.contractPdfUrl ? `<a class="btn btn-secondary btn-sm" href="${booking.contractPdfUrl}" target="_blank">Xem hợp đồng</a>` : ""}
            ${s === "completed" ? (
              currentReview 
                ? `<button class="btn btn-outline btn-sm" type="button" id="reviewBtn">Sửa Đánh Giá</button>`
                : `<button class="btn btn-primary btn-sm" type="button" id="reviewBtn">Đánh giá xe</button>`
            ) : ""}
          </div>
        </aside>
      </div>`;
      
    const cancelBtn = U.byId("cancelBookingBtn");
    if (cancelBtn) {
      cancelBtn.addEventListener("click", () => {
        if(window.VivuCarBookingCancellation) {
          window.VivuCarBookingCancellation.openCancelBookingModal(booking.id, loadData);
        } else {
          U.showToast("Chức năng hủy đang được cập nhật.", "info");
        }
      });
    }

    const requestReturnBtn = U.byId("requestReturnBtn");
    if (requestReturnBtn) {
      requestReturnBtn.addEventListener("click", async () => {
        if (!confirm("Bạn có chắc chắn muốn trả xe lúc này?")) return;
        requestReturnBtn.disabled = true;
        requestReturnBtn.textContent = "Đang xử lý...";
        try {
          const res = await authService.apiFetch(`bookings/${booking.id}/request-return`, { method: "POST" });
          if (!res.ok) {
            const err = await res.json();
            throw new Error(err.message || "Lỗi khi yêu cầu trả xe.");
          }
          U.showToast("Đã gửi yêu cầu trả xe thành công.", "success");
          loadData();
        } catch (e) {
          U.showToast(e.message, "error");
          requestReturnBtn.disabled = false;
          requestReturnBtn.textContent = "Yêu cầu trả xe";
        }
      });
    }

    const reviewBtn = U.byId("reviewBtn");
    if (reviewBtn) {
      reviewBtn.addEventListener("click", () => {
        openReviewModal();
      });
    }
  }

  function openReviewModal() {
    let modal = document.getElementById("reviewModal");
    if (modal) {
      modal.remove(); // Remove old modal to re-render fresh
    }

    document.body.insertAdjacentHTML("beforeend", `
      <div id="reviewModal" class="fixed inset-0 z-[100] flex items-center justify-center bg-black/50">
        <div class="bg-white rounded-xl shadow-2xl w-full max-w-[500px] p-6 mx-4 relative">
          <h3 class="font-bold text-lg mb-4 text-zinc-900">${currentReview ? 'Sửa Đánh Giá' : 'Đánh giá chuyến đi'}</h3>
          <form id="reviewForm" class="space-y-4">
            <div>
              <label class="block text-sm font-semibold text-zinc-700 mb-2">Đánh giá sao (1-5)</label>
              <div class="flex gap-2" id="starContainer">
                ${[1, 2, 3, 4, 5].map(i => `
                  <button type="button" class="star-btn text-3xl ${currentReview && currentReview.rating >= i ? 'text-amber-400' : 'text-zinc-300'}" data-val="${i}">★</button>
                `).join("")}
              </div>
              <input type="hidden" id="ratingValue" value="${currentReview ? currentReview.rating : 5}" />
            </div>
            <div>
              <label class="block text-sm font-semibold text-zinc-700 mb-1">Nhận xét (Tùy chọn)</label>
              <textarea id="reviewComment" class="w-full border border-zinc-300 rounded-lg p-3 text-sm focus:ring-2 focus:ring-primary focus:outline-none" rows="4" placeholder="Chia sẻ trải nghiệm của bạn...">${currentReview ? currentReview.comment : ''}</textarea>
            </div>
            <div class="flex justify-end gap-3 mt-6">
              ${currentReview ? `<button type="button" id="deleteReviewBtn" class="px-4 py-2 text-sm font-semibold text-red-600 bg-red-50 hover:bg-red-100 rounded-lg">Xóa</button>` : ""}
              <button type="button" id="closeReviewModal" class="px-4 py-2 text-sm font-semibold text-zinc-700 bg-zinc-100 hover:bg-zinc-200 rounded-lg">Đóng</button>
              <button type="submit" class="px-4 py-2 text-sm font-semibold text-white bg-green-600 hover:bg-green-700 rounded-lg">${currentReview ? 'Cập nhật' : 'Gửi đánh giá'}</button>
            </div>
          </form>
        </div>
      </div>
    `);

    modal = document.getElementById("reviewModal");
      
      // Star click logic
      modal.querySelectorAll(".star-btn").forEach(btn => {
        btn.addEventListener("click", (e) => {
          const val = parseInt(e.target.dataset.val, 10);
          document.getElementById("ratingValue").value = val;
          modal.querySelectorAll(".star-btn").forEach((s, idx) => {
            if (idx < val) {
              s.classList.remove("text-zinc-300");
              s.classList.add("text-amber-400");
            } else {
              s.classList.remove("text-amber-400");
              s.classList.add("text-zinc-300");
            }
          });
        });
      });

      document.getElementById("closeReviewModal").addEventListener("click", () => {
        modal.classList.add("hidden");
      });

        const btnDelete = document.getElementById("deleteReviewBtn");
        if (btnDelete) {
          btnDelete.addEventListener("click", () => {
            // Create a simple custom confirm modal
            const confirmHtml = `
              <div id="confirmDeleteModal" class="fixed inset-0 z-[110] flex items-center justify-center bg-black/50">
                <div class="bg-white rounded-xl shadow-2xl max-w-sm w-full p-6 mx-4">
                  <h3 class="font-bold text-lg mb-2">Xác nhận xóa</h3>
                  <p class="text-zinc-600 mb-6">Bạn có chắc chắn muốn xóa đánh giá này không?</p>
                  <div class="flex justify-end gap-3">
                    <button type="button" id="cancelDeleteBtn" class="px-4 py-2 text-sm font-semibold text-zinc-700 bg-zinc-100 hover:bg-zinc-200 rounded-lg">Hủy</button>
                    <button type="button" id="confirmDeleteBtn" class="px-4 py-2 text-sm font-semibold text-white bg-red-600 hover:bg-red-700 rounded-lg">Xóa</button>
                  </div>
                </div>
              </div>
            `;
            document.body.insertAdjacentHTML("beforeend", confirmHtml);
            
            document.getElementById("cancelDeleteBtn").addEventListener("click", () => {
              document.getElementById("confirmDeleteModal").remove();
            });
            
            document.getElementById("confirmDeleteBtn").addEventListener("click", async () => {
              document.getElementById("confirmDeleteModal").remove();
              document.getElementById("deleteReviewBtn").disabled = true;
              document.getElementById("deleteReviewBtn").textContent = "Đang xóa...";
              
              try {
                const r = await authService.apiFetch(`reviews/${currentReview.id}`, { method: "DELETE" });
                if (!r.ok) throw new Error("Could not delete");
                currentReview = null;
                U.showToast("Đã xóa đánh giá thành công.", "success");
                modal.classList.add("hidden");
                render();
              } catch (e) {
                U.showToast("Có lỗi xảy ra: " + e.message, "error");
                document.getElementById("deleteReviewBtn").disabled = false;
                document.getElementById("deleteReviewBtn").textContent = "Xóa";
              }
            });
          });
        }

      document.getElementById("reviewForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const rating = parseInt(document.getElementById("ratingValue").value, 10);
        const comment = document.getElementById("reviewComment").value.trim();
        
        try {
          let r;
          if (currentReview) {
            r = await authService.apiFetch(`reviews/${currentReview.id}`, {
              method: "PUT",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify({ rating, comment })
            });
          } else {
            r = await authService.apiFetch("reviews", {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify({ bookingId: booking.id, rating, comment })
            });
          }
          if (!r.ok) {
            const err = await r.json();
            throw new Error(err.message || "Failed to submit review");
          }
          currentReview = await r.json();
          U.showToast("Đã lưu đánh giá thành công.", "success");
          modal.classList.add("hidden");
          render();
        } catch (err) {
          U.showToast("Lỗi: " + err.message, "error");
        }
      });
    if (currentReview) {
      document.getElementById("reviewComment").value = currentReview.comment || "";
    } else {
      document.getElementById("reviewComment").value = "";
    }
    
    modal.classList.remove("hidden");
  }

  function info(label, value) {
    return `<div class="info-cell"><span>${label}</span><strong>${value}</strong></div>`;
  }

  function timeline() {
    const s = (booking.status || "").toLowerCase();
    const items = [
      ["Tạo đơn", new Date(booking.createdAt).toLocaleString('vi-VN')],
      s === "completed" ? ["Hoàn tất", new Date(booking.endDateTime).toLocaleString('vi-VN')] : null,
      s === "cancelled" ? ["Đã hủy", booking.cancellationReason || "Đơn đã bị hủy"] : null
    ].filter(Boolean);
    return items.map(([title, text]) => `<div class="timeline-item"><strong>${title}</strong><p>${text}</p></div>`).join("");
  }

  loadData();
})();
