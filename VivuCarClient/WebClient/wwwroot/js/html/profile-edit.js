(function () {
  const Auth = window.VivuCarAuth, U = window.VivuCarUtils, C = window.VivuCarConstants;
  const user = Auth.getCurrentUser(); let dirty = false;
  document.getElementById("full_name").value = user.full_name; document.getElementById("email").value = user.email; document.getElementById("role").value = C.USER_ROLE_LABELS[user.role];
  document.getElementById("profileEditForm").addEventListener("input", () => dirty = true);
  document.getElementById("btnCancelEdit").addEventListener("click", (e) => { if (dirty && !confirm("Bạn có thay đổi chưa lưu. Hủy chỉnh sửa?")) e.preventDefault(); });
  document.getElementById("profileEditForm").addEventListener("submit", (e) => { e.preventDefault(); const name = document.getElementById("full_name").value.trim(); const msg = document.getElementById("profileUpdateMessage"); if (!name) { msg.textContent = "Họ tên không được trống."; msg.className = "alert alert-error"; return; } user.full_name = name; window.VivuCarSaveDB?.(); localStorage.setItem("vivucar_current_user_role", user.role); msg.textContent = "Cập nhật hồ sơ thành công."; msg.className = "alert"; U.showToast("Cập nhật hồ sơ thành công."); setTimeout(() => location.href = "profile.html", 650); });
})();

