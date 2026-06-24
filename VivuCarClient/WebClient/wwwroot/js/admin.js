(function () {
  const page = document.body.dataset.adminPage;

  document.querySelectorAll(".admin-nav a").forEach((link) => {
    if (link.dataset.nav === page) {
      link.classList.add("is-active");
    }
  });

  document.querySelector("[data-admin-menu]")?.addEventListener("click", () => {
    document.querySelector(".admin-sidebar")?.classList.toggle("is-open");
  });

  function openModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;
    modal.classList.add("is-open");
    modal.setAttribute("aria-hidden", "false");
  }

  function closeModal(modal) {
    modal.classList.remove("is-open");
    modal.setAttribute("aria-hidden", "true");
  }

  document.addEventListener("click", (event) => {
    if (event.target.closest("[data-open-logout]")) {
      openModal("logoutModal");
    }

    if (event.target.matches("[data-close-modal]")) {
      const modal = event.target.closest(".admin-modal");
      if (modal) closeModal(modal);
    }

    if (event.target.classList.contains("admin-modal")) {
      closeModal(event.target);
    }
  });

  document.addEventListener("keydown", (event) => {
    if (event.key !== "Escape") return;
    document.querySelectorAll(".admin-modal.is-open").forEach(closeModal);
  });

  const voucherCode = document.getElementById("code");
  const voucherName = document.getElementById("name");
  const voucherValue = document.getElementById("discount_value");
  const voucherPreviewCode = document.getElementById("voucherPreviewCode");
  const voucherPreviewName = document.getElementById("voucherPreviewName");
  const voucherPreviewValue = document.getElementById("voucherPreviewValue");

  function syncVoucherPreview() {
    if (!voucherPreviewCode) return;
    voucherPreviewCode.textContent = (voucherCode?.value || "SUMMER2026").toUpperCase();
    voucherPreviewName.textContent = voucherName?.value || "Tên voucher";
    voucherPreviewValue.textContent = voucherValue?.value ? `Giảm ${voucherValue.value}` : "Giảm giá";
  }

  [voucherCode, voucherName, voucherValue].forEach((input) => {
    input?.addEventListener("input", syncVoucherPreview);
  });

  voucherCode?.addEventListener("input", () => {
    voucherCode.value = voucherCode.value.toUpperCase().replace(/\s+/g, "");
  });
})();
