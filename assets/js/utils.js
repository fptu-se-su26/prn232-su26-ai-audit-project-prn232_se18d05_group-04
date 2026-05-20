window.VivuCarUtils = {
  formatVnd(value) {
    return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND", maximumFractionDigits: 0 }).format(Number(value || 0));
  },
  formatDate(value) {
    if (!value) return "Không giới hạn";
    return new Intl.DateTimeFormat("vi-VN", { dateStyle: "medium" }).format(new Date(value));
  },
  formatDateTime(value) {
    return new Intl.DateTimeFormat("vi-VN", { dateStyle: "short", timeStyle: "short" }).format(new Date(value));
  },
  byId(id) {
    return document.getElementById(id);
  },
  showToast(message) {
    let root = document.querySelector(".toast-root");
    if (!root) {
      root = document.createElement("div");
      root.className = "toast-root";
      document.body.appendChild(root);
    }
    const toast = document.createElement("div");
    toast.className = "toast";
    toast.textContent = message;
    root.appendChild(toast);
    setTimeout(() => toast.remove(), 2800);
  },
  statusBadge(kind, value, label) {
    const groups = {
      success: ["available", "success", "completed", "done", "active"],
      warning: ["pending", "processing", "maintenance", "approved", "used_up"],
      danger: ["blocked", "failed", "rejected", "cancelled", "inactive"],
      info: ["rented", "car_owner", "admin"],
      neutral: ["refunded", "expired", "user"]
    };
    const bucket = Object.entries(groups).find(([, values]) => values.includes(value))?.[0] || kind || "neutral";
    return `<span class="badge badge-${bucket}">${label || value}</span>`;
  },
  renderStatusBadge(type, value) {
    const C = window.VivuCarConstants || {};
    const labels = {
      ...(C.BOOKING_STATUS_LABELS || {}),
      ...(C.PAYMENT_STATUS_LABELS || {}),
      ...(C.CAR_STATUS_LABELS || {}),
      vnpay: "VNPay",
      momo: "MoMo",
      cash: "Tiền mặt",
      pre_rental: "Trước thuê",
      post_rental: "Sau thuê"
    };
    return this.statusBadge(type, value, labels[value] || value);
  },
  renderToast(message, type) {
    this.showToast(message, type);
  },
  renderEmptyState(config) {
    const action = config.href ? `<a class="btn btn-primary" href="${config.href}">${config.action || "Tiếp tục"}</a>` : "";
    return `<div class="empty-state"><h3>${config.title}</h3><p>${config.text || ""}</p>${action}</div>`;
  },
  carTitle(car) {
    return `${car.brand} ${car.model} ${car.year}`;
  },
  carImage(carId) {
    return window.VivuCarDB.car_images.find((image) => image.car_id === carId && image.is_primary)?.image_url || "https://picsum.photos/seed/vivucar-car/640/420";
  },
  carRating(carId) {
    const reviews = window.VivuCarDB.reviews.filter((review) => review.car_id === carId);
    if (!reviews.length) return 0;
    return reviews.reduce((sum, review) => sum + review.rating, 0) / reviews.length;
  },
  rentalCount(carId) {
    return window.VivuCarDB.bookings.filter((booking) => booking.car_id === carId).length;
  },
  stars(value) {
    const rating = Math.round(Number(value || 0));
    return `<span class="stars">${Array.from({ length: 5 }, (_, index) => `<span class="${index < rating ? "is-filled" : ""}">★</span>`).join("")}</span>`;
  },
  paginate(items, page, pageSize) {
    const totalPages = Math.max(1, Math.ceil(items.length / pageSize));
    const safePage = Math.min(Math.max(1, page), totalPages);
    const start = (safePage - 1) * pageSize;
    return { items: items.slice(start, start + pageSize), totalPages, page: safePage, start };
  },
  downloadBlob(blob, fileName) {
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(url);
  },
  openModal(id) {
    if (window.VivuCarTailwindUI?.openModal) {
      window.VivuCarTailwindUI.openModal(id);
      return;
    }
    const modal = document.getElementById(id);
    modal?.classList.remove("hidden");
    modal?.classList.add("flex", "show");
  },
  closeModal(id) {
    if (window.VivuCarTailwindUI?.closeModal) {
      window.VivuCarTailwindUI.closeModal(id);
      return;
    }
    const modal = document.getElementById(id);
    modal?.classList.add("hidden");
    modal?.classList.remove("flex", "show");
  },
  resolveBookingUiState(booking, payment, inspections = []) {
    if (!booking) return { key: "unknown", label: "Không xác định", tone: "neutral" };
    if (booking.status === "cancelled") return { key: "cancelled", label: "Đã hủy", tone: "danger" };
    if (booking.status === "rejected") return { key: "rejected", label: "Bị từ chối", tone: "danger" };
    if (booking.status === "completed") return { key: "completed", label: "Hoàn tất", tone: "success" };
    if (booking.status === "pending") {
      return payment?.status === "success"
        ? { key: "paid_pending", label: "Đã cọc - chờ chủ xe xác nhận", tone: "warning" }
        : { key: "payment_pending", label: "Chờ thanh toán hoặc xác nhận", tone: "warning" };
    }
    if (booking.status === "approved") {
      const hasPre = inspections.some((item) => item.inspection_type === "pre_rental");
      const hasPost = inspections.some((item) => item.inspection_type === "post_rental");
      if (!hasPre) return { key: "handover_pending", label: "Chờ bàn giao", tone: "info" };
      if (!hasPost) return { key: "renting", label: "Đang thuê hoặc chờ trả xe", tone: "info" };
      return { key: "completion_pending", label: "Chờ hoàn tất", tone: "warning" };
    }
    return { key: booking.status, label: booking.status, tone: "neutral" };
  },
  canCancelBooking(booking) {
    if (!booking || !["pending", "approved"].includes(booking.status)) return false;
    return new Date(booking.pickup_datetime).getTime() > Date.now();
  },
  paymentForBooking(bookingId) {
    return window.VivuCarDB.payments.find((payment) => payment.booking_id === Number(bookingId));
  },
  inspectionsForBooking(bookingId) {
    return (window.VivuCarDB.handover_inspections || []).filter((inspection) => inspection.booking_id === Number(bookingId));
  },
  depositAmount(booking) {
    return Math.round(Number(booking?.total_amount || 0) * 0.3);
  },
  resolveChatUiState(session) {
    if (!session) return { key: "unknown", label: "Không xác định", tone: "neutral" };
    if (session.status === "closed") return { key: "closed", label: "Đã xử lý", tone: "success" };
    if (session.status === "escalated") return { key: "escalated", label: "Chờ người hỗ trợ", tone: "warning" };
    if (session.status === "open" && session.session_type === "live") return { key: "live", label: "Live chat", tone: "info" };
    if (session.status === "open" && session.session_type === "ai") return { key: "ai", label: "AI đang hỗ trợ", tone: "neutral" };
    return { key: session.status, label: session.status, tone: "neutral" };
  }
};
