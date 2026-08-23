(function () {
  const DB = window.VivuCarDB,
    U = window.VivuCarUtils,
    Auth = window.VivuCarAuth;
  const owner = Auth.getCurrentUser();
  if (!owner) return;
  const carId = Number(new URLSearchParams(location.search).get("carId"));
  const car = DB.cars.find((x) => x.id === carId && x.owner_id === owner.id);
  const root = U.byId("ownerCarImagesRoot");
  const uploads = [];
  function images() {
    return DB.car_images.filter((x) => x.car_id === carId);
  }
  function render() {
    if (!car)
      return (root.innerHTML = U.renderEmptyState({
        title: "Không tìm thấy xe",
        href: "owner-cars.html",
        action: "Về danh sách",
      }));
    const list = images();
    root.innerHTML = `<section class="owner-hero"><div><span class="eyebrow">Gallery</span><h1>Ảnh xe ${U.carTitle(car)}</h1><p class="muted">${car.license_plate} · ${list.length} ảnh</p></div><a class="btn btn-secondary" href="owner-cars.html">Quay lại</a></section><section class="checkout-section"><h2>Gallery hiện tại</h2><div class="owner-image-grid">${list.map(card).join("")}</div></section><section class="checkout-section"><h2>Upload ảnh mới</h2><input id="newCarImages" type="file" accept="image/*" multiple><div id="newImagePreviewGrid" class="image-preview-grid"></div><button class="btn btn-primary" id="uploadImages" type="button">Upload ảnh mới</button><p class="muted">Drag/drop sort_order là UI-only vì DB car_images chưa có sort_order.</p><!-- UI-only field. Not present in current DB schema. Requires migration before backend integration. --></section>`;
    document
      .querySelectorAll("[data-main]")
      .forEach((b) => (b.onclick = () => setMain(Number(b.dataset.main))));
    document
      .querySelectorAll("[data-delete]")
      .forEach((b) => (b.onclick = () => del(Number(b.dataset.delete))));
    U.byId("newCarImages").onchange = preview;
    U.byId("uploadImages").onclick = upload;
  }
  function card(img) {
    return `<article class="owner-image-card"><img src="${img.image_url}" alt="Ảnh xe">${img.is_primary ? U.statusBadge("success", "available", "Ảnh đại diện") : ""}<button class="btn btn-secondary btn-sm" data-main="${img.id}" type="button">Đặt đại diện</button><button class="btn btn-danger btn-sm" data-delete="${img.id}" type="button">Xóa</button></article>`;
  }
  function setMain(id) {
    images().forEach((img) => (img.is_primary = img.id === id));
    window.VivuCarSaveDB();
    U.renderToast("Đã cập nhật ảnh đại diện.", "success");
    render();
  }
  function del(id) {
    if (images().length <= 1)
      return U.renderToast("Không thể xóa ảnh cuối cùng.", "danger");
    const idx = DB.car_images.findIndex((img) => img.id === id);
    DB.car_images.splice(idx, 1);
    if (!images().some((img) => img.is_primary)) images()[0].is_primary = true;
    window.VivuCarSaveDB();
    U.renderToast("Đã xóa ảnh.", "success");
    render();
  }
  function preview(e) {
    [...e.target.files].forEach((file) => {
      if (!file.type.startsWith("image/") || file.size > 5 * 1024 * 1024)
        return U.renderToast("Ảnh không hợp lệ.", "danger");
      const r = new FileReader();
      r.onload = () => {
        uploads.push(r.result);
        U.byId("newImagePreviewGrid").insertAdjacentHTML(
          "beforeend",
          `<img src="${r.result}" alt="Ảnh mới">`,
        );
      };
      r.readAsDataURL(file);
    });
  }
  function upload() {
    if (images().length + uploads.length > 12)
      return U.renderToast("Tối đa 12 ảnh.", "danger");
    uploads
      .splice(0)
      .forEach((url) =>
        DB.car_images.push({
          id: Math.max(0, ...DB.car_images.map((x) => x.id)) + 1,
          car_id: car.id,
          image_url: url,
          is_primary: !images().length,
        }),
      );
    window.VivuCarSaveDB();
    U.renderToast("Đã upload ảnh.", "success");
    render();
  }
  render();
})();

