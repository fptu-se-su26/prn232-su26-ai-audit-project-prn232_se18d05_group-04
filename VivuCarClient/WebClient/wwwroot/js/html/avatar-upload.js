(function () {
  const DB = window.VivuCarDB, Auth = window.VivuCarAuth, U = window.VivuCarUtils; const user = Auth.getCurrentUser(); let selected = null;
  const current = DB.user_documents.find((d) => d.user_id === user.id && d.document_type === "avatar")?.file_url || user.avatar_url || "";
  document.getElementById("currentAvatar").src = current || "https://picsum.photos/seed/avatar-default/160/160";
  function validate(file) { if (!file) return false; if (!["image/jpeg", "image/png", "image/webp"].includes(file.type)) { U.showToast("Ảnh không hợp lệ."); return false; } if (file.size > 5 * 1024 * 1024) { U.showToast("Dung lượng ảnh không được vượt quá 5MB."); return false; } return true; }
  function preview(file) { const reader = new FileReader(); reader.onload = (e) => { document.getElementById("avatarPreview").src = e.target.result; document.getElementById("avatarPreview").classList.remove("hidden"); }; reader.readAsDataURL(file); }
  document.getElementById("avatarInput").addEventListener("change", (e) => { const file = e.target.files[0]; if (validate(file)) { selected = file; preview(file); } });
  document.getElementById("avatarDropzone").addEventListener("click", () => document.getElementById("avatarInput").click());
  document.getElementById("avatarDropzone").addEventListener("drop", (e) => { e.preventDefault(); const file = e.dataTransfer.files[0]; if (validate(file)) { selected = file; preview(file); } });
  document.getElementById("avatarDropzone").addEventListener("dragover", (e) => e.preventDefault());
  document.getElementById("btnRemoveAvatar").addEventListener("click", () => { selected = null; document.getElementById("avatarPreview").classList.add("hidden"); });
  document.getElementById("btnSaveAvatar").addEventListener("click", () => { if (!selected) return U.showToast("Vui lòng chọn ảnh."); const url = URL.createObjectURL(selected); DB.user_documents.push({ id: DB.user_documents.length + 1, user_id: user.id, document_type: "avatar", file_name: selected.name.slice(0, 50), file_url: url, verified: true, created_at: new Date().toISOString() }); user.avatar_url = url; new FormData().append("avatar", selected); window.VivuCarSaveDB?.(); U.showToast("Cập nhật ảnh đại diện thành công."); });
})();
