(function () {
  const DB = window.VivuCarDB; // Vẫn dùng DB cho xe và voucher
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser();
  if (!currentUser) return;
  const params = new URLSearchParams(location.search);
  const carId = Number(params.get("carId"));
  const car = DB.cars.find((item) => item.id === carId);
  const root = U.byId("checkoutRoot");
  let appliedVoucher = null;
  const licenseUploads = {};
  
  // Tái sử dụng ảnh GPLX từ Profile (Mock bằng DB)
  const userDocs = DB.user_documents.filter(d => d.user_id === currentUser.id);
  const profileFront = userDocs.find(d => d.document_type === "license_front")?.file_url;
  const profileBack = userDocs.find(d => d.document_type === "license_back")?.file_url;
  const hasProfileLicense = !!(profileFront && profileBack);

  function renderError(message) {
    root.innerHTML = U.renderEmptyState({ title: "Không thể đặt xe", text: message, href: "search.html", action: "Tìm xe khác" });
  }

  function render() {
    if (!car) return renderError("Không tìm thấy xe theo đường dẫn hiện tại.");
    if (car.status !== "available") return renderError("Xe này hiện không ở trạng thái available.");
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([
      { label: "Trang chủ", href: "home.html" },
      { label: U.carTitle(car), href: `car-detail.html?carId=${car.id}` },
      { label: "Đặt xe" }
    ]);
    const tomorrow = new Date(Date.now() + 864e5).toISOString().slice(0, 16);
    const after = new Date(Date.now() + 3 * 864e5).toISOString().slice(0, 16);
    
    let licenseSection = "";
    if (hasProfileLicense) {
      licenseSection = `
        <div class="p-4 rounded-xl border border-blue-200 bg-blue-50 text-blue-800 text-sm mb-4">
          Hệ thống đã tự động lấy ảnh Giấy phép lái xe từ hồ sơ cá nhân của bạn.
        </div>
      `;
    } else {
      licenseSection = `
        <div class="form-grid">
          <label>GPLX mặt trước<input id="licenseFront" type="file" accept="image/png,image/jpeg,image/webp" required></label>
          <label>GPLX mặt sau<input id="licenseBack" type="file" accept="image/png,image/jpeg,image/webp" required></label>
        </div>
        <div class="image-preview-grid" id="licensePreview"></div>
      `;
    }

    root.innerHTML = `
      <form class="checkout-main" id="checkoutForm">
        <section class="checkout-section">
          <h2>Xe đã chọn</h2>
          <div class="checkout-car">
            <img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}">
            <div>
              <h1>${U.carTitle(car)}</h1>
              <p class="muted">${car.address}</p>
              <div class="car-meta">
                <span>${car.seats} chỗ</span><span>${car.transmission}</span><span>${car.fuel_type}</span>
              </div>
            </div>
          </div>
        </section>
        <section class="checkout-section">
          <h2>Thông tin người lái</h2>
          <div class="form-grid">
            <label>Họ tên<input id="driverName" value="${currentUser.full_name}" required></label>
            <label>Số điện thoại<input id="driverPhone" value="0901234567" required></label>
            <label>CCCD/CMND<input id="citizenId" value="048099123456" required></label>
            <label>Số GPLX<input id="licenseNumber" value="790123456789" required></label>
          </div>
        </section>
        <section class="checkout-section">
          <h2>Hồ sơ lái xe</h2>
          ${licenseSection}
        </section>
        <button class="btn btn-primary btn-full" type="submit" id="submitBtn">Tạo đơn và tiếp tục thanh toán</button>
      </form>
      <aside class="flex flex-col gap-4 sticky top-24" id="checkoutSidebar">
        <section class="checkout-section">
          <h2>Lịch thuê</h2>
          <div class="form-grid">
            <label class="full">Nhận xe<input id="pickupDatetime" name="pickup_datetime" type="datetime-local" value="${tomorrow}" form="checkoutForm" required></label>
            <label class="full">Trả xe<input id="returnDatetime" name="return_datetime" type="datetime-local" value="${after}" form="checkoutForm" required></label>
            <label class="full">Địa điểm nhận xe<input id="pickupAddress" name="pickup_address" value="${car.address}" form="checkoutForm" required></label>
            <label class="full">Địa điểm trả xe<input id="returnAddress" name="return_address" value="${car.address}" form="checkoutForm" required></label>
            <label class="full">Khoảng cách Giao/Nhận (km)<input id="distanceKm" type="number" min="0" value="0" form="checkoutForm" required></label>
          </div>
          <div id="availabilityNote" class="availability-note">Đang kiểm tra lịch xe...</div>
        </section>
        <section class="checkout-section">
          <h2>Voucher ưu đãi</h2>
          <div class="voucher-inline flex gap-2">
            <input id="voucherCode" placeholder="Nhập mã voucher" form="checkoutForm" class="flex-1">
            <button class="btn btn-secondary shrink-0" id="applyVoucher" type="button">Áp dụng</button>
          </div>
          <div class="field mt-3">
            <label for="voucherSelect" class="text-xs font-semibold text-neutral-500 mb-1 block">Hoặc chọn ưu đãi có sẵn:</label>
            <select id="voucherSelect" class="w-full rounded-xl border border-zinc-200 bg-white px-3.5 py-2.5 text-sm">
              <option value="">-- Chọn voucher --</option>
              ${DB.vouchers.map(v => {
                const desc = v.discount_type === "percentage" ? `${v.discount_value}%` : `${U.formatVnd(v.discount_value)}`;
                return `<option value="${v.code}">${v.code} (${v.name} - Giảm ${desc})</option>`;
              }).join("")}
            </select>
          </div>
        </section>
        <div class="summary-card" id="summaryCard"></div>
      </aside>`;
    bind();
    updateAll();
  }

  function bind() {
    ["pickupDatetime", "returnDatetime", "distanceKm"].forEach((id) => U.byId(id).addEventListener("change", updateAll));
    U.byId("applyVoucher").addEventListener("click", applyVoucher);
    U.byId("voucherSelect").addEventListener("change", (e) => {
      const code = e.target.value;
      U.byId("voucherCode").value = code;
      applyVoucher();
    });
    if (!hasProfileLicense) {
      ["licenseFront", "licenseBack"].forEach((id) => U.byId(id).addEventListener("change", previewLicense));
    }
    U.byId("checkoutForm").addEventListener("submit", submitBooking);
  }

  async function updateAll() {
    const pickup = new Date(U.byId("pickupDatetime").value).toISOString();
    const returned = new Date(U.byId("returnDatetime").value).toISOString();
    
    // Check Availability API
    try {
      const availRes = await Auth.fetchWithAuth(`${C.API_BASE_URL}/bookings/check-availability`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ carId: car.id, startDateTime: pickup, endDateTime: returned })
      });
      const availData = await availRes.json();
      U.byId("availabilityNote").className = \`availability-note \${availData.available ? "success" : "danger"}\`;
      U.byId("availabilityNote").textContent = availData.available ? "Xe có sẵn trong thời gian này." : "Lịch xe bị trùng, vui lòng chọn thời gian khác.";
      
      if (!availData.available) {
          U.byId("submitBtn").disabled = true;
          return;
      }
      U.byId("submitBtn").disabled = false;
    } catch (e) {
      U.byId("availabilityNote").textContent = "Lỗi khi kiểm tra lịch trống.";
    }

    // Check Price Preview API
    try {
      const priceRes = await Auth.fetchWithAuth(`${C.API_BASE_URL}/bookings/price-preview`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            carId: car.id,
            startDateTime: pickup,
            endDateTime: returned,
            hasInsurance: true,
            hasDelivery: Number(U.byId("distanceKm").value) > 0,
            distanceKm: Number(U.byId("distanceKm").value),
            voucherCode: appliedVoucher?.code || null
        })
      });
      const pricing = await priceRes.json();
      
      U.byId("summaryCard").innerHTML = \`
        <h2>Tóm tắt chi phí</h2>
        <p class="muted">\${U.carTitle(car)} · \${pricing.rentalDays} ngày</p>
        <div class="summary-row"><span>Giá thuê gốc</span><strong>\${U.formatVnd(pricing.weekdayCost + pricing.weekendCost)}</strong></div>
        <div class="summary-row"><span>Bảo hiểm thuê xe</span><strong>\${U.formatVnd(pricing.insuranceFee)}</strong></div>
        <div class="summary-row"><span>Phí Giao/Nhận xe</span><strong>\${U.formatVnd(pricing.deliveryFee)}</strong></div>
        \${pricing.discountAmount > 0 ? \`<div class="summary-row text-green-600"><span>Voucher giảm giá</span><strong>-\${U.formatVnd(pricing.discountAmount)}</strong></div>\` : ""}
        <div class="summary-total mt-4"><span>Tổng thanh toán</span><strong>\${U.formatVnd(pricing.totalAmount)}</strong></div>
        <p class="muted font-medium mt-2">Cần đặt cọc trước: <span class="text-blue-600">\${U.formatVnd(pricing.depositAmount)}</span></p>\`;
    } catch (e) {
        U.byId("summaryCard").innerHTML = "<p class='text-red-500'>Không thể tính giá. Vui lòng thử lại.</p>";
    }
  }

  function applyVoucher() {
    const code = U.byId("voucherCode").value.trim().toUpperCase();
    if (!code) {
      appliedVoucher = null;
      U.renderToast("Đã hủy áp dụng voucher.", "neutral");
      if (U.byId("voucherSelect")) U.byId("voucherSelect").value = "";
      updateAll();
      return;
    }
    const voucher = DB.vouchers.find((item) => item.code.toUpperCase() === code);
    if (!voucher) {
      appliedVoucher = null;
      U.renderToast("Không tìm thấy voucher.", "danger");
      if (U.byId("voucherSelect")) U.byId("voucherSelect").value = "";
      updateAll();
      return;
    }
    appliedVoucher = voucher;
    U.renderToast("Đã áp dụng voucher.", "success");
    if (U.byId("voucherSelect")) U.byId("voucherSelect").value = code;
    updateAll();
  }

  function previewLicense(event) {
    const file = event.target.files[0];
    if (!file) return;
    if (!["image/jpeg", "image/png", "image/webp"].includes(file.type) || file.size > 5 * 1024 * 1024) {
      event.target.value = "";
      U.renderToast("Ảnh phải là JPG/PNG/WEBP và không quá 5MB.", "danger");
      return;
    }
    const reader = new FileReader();
    reader.onload = () => {
      const type = event.target.id === "licenseFront" ? "license_front" : "license_back";
      licenseUploads[type] = { file_name: file.name, file_url: reader.result };
      U.byId("licensePreview").insertAdjacentHTML("beforeend", \`<img src="\${reader.result}" alt="Preview GPLX">\`);
    };
    reader.readAsDataURL(file);
  }

  async function submitBooking(event) {
    event.preventDefault();
    const pickup = new Date(U.byId("pickupDatetime").value).toISOString();
    const returned = new Date(U.byId("returnDatetime").value).toISOString();

    let frontUrl = profileFront;
    let backUrl = profileBack;

    if (!hasProfileLicense) {
        if (!licenseUploads["license_front"] || !licenseUploads["license_back"]) {
            return U.renderToast("Vui lòng tải lên ảnh 2 mặt GPLX.", "danger");
        }
        // Giả lập API Upload ảnh -> trả về dummy URL
        frontUrl = "https://dummyimage.com/600x400/2563eb/fff&text=GPLX+Front";
        backUrl = "https://dummyimage.com/600x400/2563eb/fff&text=GPLX+Back";
    }

    U.byId("submitBtn").disabled = true;
    U.byId("submitBtn").textContent = "Đang tạo đơn...";

    const payload = {
        carId: car.id,
        startDateTime: pickup,
        endDateTime: returned,
        pickupLocation: U.byId("pickupAddress").value.trim(),
        returnLocation: U.byId("returnAddress").value.trim(),
        hasInsurance: true,
        hasDelivery: Number(U.byId("distanceKm").value) > 0,
        distanceKm: Number(U.byId("distanceKm").value),
        voucherCode: appliedVoucher?.code || null,
        driverInfo: {
            fullName: U.byId("driverName").value.trim(),
            phoneNumber: U.byId("driverPhone").value.trim(),
            citizenIdNumber: U.byId("citizenId").value.trim(),
            citizenIdFrontImageUrl: null,
            citizenIdBackImageUrl: null,
            driverLicenseNumber: U.byId("licenseNumber").value.trim(),
            driverLicenseFrontImageUrl: frontUrl,
            driverLicenseBackImageUrl: backUrl
        }
    };

    try {
        const res = await Auth.fetchWithAuth(`${C.API_BASE_URL}/bookings`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        
        if (!res.ok) throw new Error(data.message || "Tạo đơn thất bại");

        U.renderToast("Đã tạo đơn thuê thành công.", "success");
        setTimeout(() => location.href = \`payment-deposit.html?bookingId=\${data.id}\`, 350);
    } catch (e) {
        U.renderToast(e.message, "danger");
        U.byId("submitBtn").disabled = false;
        U.byId("submitBtn").textContent = "Tạo đơn và tiếp tục thanh toán";
    }
  }

  render();
})();
