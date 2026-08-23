(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  // UI-only fields usage_limit_per_user and is_active are not present in current DB schema.
  // They require a migration before backend integration and are not included in FormData names.
  const inputs = ["code", "name", "discount_value", "max_discount", "min_order_amount", "quantity", "expires_at", "usage_limit_per_user", "is_active"];

  function discountType() {
    return document.querySelector("input[name='discount_type']:checked").value;
  }

  function updatePreview() {
    const code = document.getElementById("code").value.toUpperCase().replace(/\s/g, "");
    document.getElementById("code").value = code;
    const type = discountType();
    const value = Number(document.getElementById("discount_value").value || (type === "percentage" ? 10 : 100000));
    const discountText = type === "percentage" ? `${value}%` : U.formatVnd(value);
    document.getElementById("voucherPreview").innerHTML = `
      <div class="code">${code || "SUMMER2026"}</div>
      <p class="muted">${document.getElementById("name").value || "Khuyến mãi hè 2026"}</p>
      <div class="discount">${discountText}</div>
      <p>Đơn tối thiểu <strong>${U.formatVnd(document.getElementById("min_order_amount").value || 500000)}</strong></p>
      <p>Giảm tối đa <strong>${U.formatVnd(document.getElementById("max_discount").value || 250000)}</strong></p>
      <p>Số lượng <strong>${document.getElementById("quantity").value || 100}</strong></p>
      <p>Hết hạn: ${document.getElementById("expires_at").value ? U.formatDate(document.getElementById("expires_at").value) : "Chưa chọn"}</p>
      ${U.statusBadge(document.getElementById("is_active").checked ? "success" : "danger", document.getElementById("is_active").checked ? "active" : "inactive", document.getElementById("is_active").checked ? "Kích hoạt UI" : "Tạm dừng UI")}`;
  }

  function validate() {
    const code = document.getElementById("code").value;
    const type = discountType();
    const value = Number(document.getElementById("discount_value").value);
    if (!code || /\s/.test(code)) return "Mã voucher bắt buộc và không có dấu cách.";
    if (type === "percentage" && (value < 1 || value > 100)) return "Giảm phần trăm phải từ 1 đến 100.";
    if (type === "fixed" && value <= 0) return "Giảm cố định phải lớn hơn 0.";
    if (Number(document.getElementById("max_discount").value) <= 0) return "Giảm tối đa phải lớn hơn 0.";
    if (Number(document.getElementById("min_order_amount").value) < 0) return "Đơn tối thiểu phải lớn hơn hoặc bằng 0.";
    if (Number(document.getElementById("quantity").value) < 0) return "Số lượng voucher phải lớn hơn hoặc bằng 0.";
    return "";
  }

  inputs.forEach((id) => document.getElementById(id).addEventListener("input", updatePreview));
  document.querySelectorAll("input[name='discount_type']").forEach((input) => input.addEventListener("change", updatePreview));
  document.getElementById("btnSaveVoucherDraft").addEventListener("click", () => U.showToast("Đã lưu nháp frontend."));
  document.getElementById("voucherForm").addEventListener("submit", (event) => {
    event.preventDefault();
    const error = validate();
    const box = document.getElementById("voucherError");
    if (error) {
      box.textContent = error;
      box.classList.remove("hidden");
      return;
    }
    const data = Object.fromEntries(new FormData(event.target).entries());
    console.log("POST /api/admin/vouchers", data);
    box.classList.add("hidden");
    U.showToast("Đã tạo voucher thành công.");
  });
  updatePreview();
})();

