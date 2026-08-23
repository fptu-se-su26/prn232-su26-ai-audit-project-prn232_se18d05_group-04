(function () {
  const DB = window.VivuCarDB;
  const U = window.VivuCarUtils;
  let selectedImages = [];

  function populateTypeOptions() {
    document.getElementById("type_id").innerHTML = DB.car_types.map((type) => `<option value="${type.id}">${type.name}</option>`).join("");
  }

  function fillEditData() {
    const id = Number(new URLSearchParams(location.search).get("id"));
    const car = DB.cars.find((item) => item.id === id);
    if (!car) return;
    document.getElementById("carFormTitle").textContent = "Cập nhật thông tin xe";
    Object.entries(car).forEach(([key, value]) => {
      const field = document.getElementById(key);
      if (field && value !== null) field.value = value;
    });
  }

  function renderPreview() {
    const grid = document.getElementById("imagePreviewGrid");
    grid.innerHTML = selectedImages.map((file) => `<img src="${URL.createObjectURL(file)}" alt="${file.name}">`).join("");
  }

  function validateFiles(files) {
    const valid = [];
    for (const file of files) {
      if (!file.type.startsWith("image/")) { U.showToast("Chỉ chấp nhận file ảnh."); continue; }
      if (file.size > 5 * 1024 * 1024) { U.showToast("Mỗi ảnh không quá 5MB."); continue; }
      valid.push(file);
    }
    return valid.slice(0, 8);
  }

  function compressImage(file, maxWidth = 1200, quality = 0.75) {
    return new Promise((resolve) => {
      const img = new Image();
      const reader = new FileReader();
      reader.onload = (e) => img.src = e.target.result;
      img.onload = () => {
        const scale = Math.min(1, maxWidth / img.width);
        const canvas = document.createElement("canvas");
        canvas.width = img.width * scale;
        canvas.height = img.height * scale;
        canvas.getContext("2d").drawImage(img, 0, 0, canvas.width, canvas.height);
        canvas.toBlob(resolve, "image/jpeg", quality);
      };
      reader.readAsDataURL(file);
    });
  }

  document.getElementById("car_images").addEventListener("change", (event) => {
    selectedImages = validateFiles(Array.from(event.target.files));
    renderPreview();
  });
  document.getElementById("btnClearImages").addEventListener("click", () => {
    selectedImages = [];
    document.getElementById("car_images").value = "";
    renderPreview();
  });
  document.getElementById("btnSaveDraft").addEventListener("click", () => U.showToast("Đã lưu nháp frontend."));
  document.getElementById("carForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const errorBox = document.getElementById("carFormError");
    const required = ["license_plate", "brand", "model", "address", "price_per_day", "price_per_hours"];
    const missing = required.find((id) => !document.getElementById(id).value.trim());
    if (missing) return showError("Vui lòng nhập đầy đủ biển số, hãng, dòng xe, địa chỉ và giá thuê.");
    if (Number(document.getElementById("price_per_day").value) <= 0 || Number(document.getElementById("price_per_hours").value) <= 0) return showError("Giá thuê phải lớn hơn 0.");
    if (Number(document.getElementById("year").value) > 2026) return showError("Năm sản xuất không được lớn hơn 2026.");
    if (!new URLSearchParams(location.search).get("id") && selectedImages.length === 0) return showError("Vui lòng chọn ít nhất 1 hình ảnh.");
    const formData = new FormData(event.target);
    await Promise.all(selectedImages.map((file) => compressImage(file)));
    console.log("POST/PUT /api/cars", Object.fromEntries(formData.entries()));
    errorBox.classList.add("hidden");
    U.showToast("Đã lưu xe thành công.");
  });

  function showError(message) {
    const errorBox = document.getElementById("carFormError");
    errorBox.textContent = message;
    errorBox.classList.remove("hidden");
  }

  populateTypeOptions();
  fillEditData();
})();

