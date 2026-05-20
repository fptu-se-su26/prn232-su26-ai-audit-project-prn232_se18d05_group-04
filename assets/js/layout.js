(function () {
  const C = window.VivuCarConstants;
  const Auth = window.VivuCarAuth;
  const U = window.VivuCarUtils;
  const page = document.body.dataset.page;
  const layout = document.body.dataset.layout || "admin";
  const publicPage = document.body.dataset.public === "true";
  const appRoles = [C.USER_ROLES.USER, C.USER_ROLES.CAR_OWNER];
  const allowed = layout === "admin" ? [C.USER_ROLES.ADMIN] : layout === "owner" ? [C.USER_ROLES.CAR_OWNER] : appRoles;
  const user = publicPage ? Auth.getCurrentUser() : Auth.requireAuth(allowed);
  if (!publicPage && !user) return;

  function renderAdminSidebar(activeKey) {
    return `
      <aside class="admin-sidebar">
        <a class="sidebar-brand" href="admin-dashboard.html"><span class="brand-mark">VC</span><span>VivuCar Admin</span></a>
        <nav class="sidebar-nav">
          <a class="${activeKey === "dashboard" ? "active" : ""}" href="admin-dashboard.html">Dashboard doanh thu</a>
          <a class="${activeKey === "users" ? "active" : ""}" href="admin-users.html">Người dùng</a>
          <a class="${activeKey === "cars" ? "active" : ""}" href="admin-cars.html">Phương tiện</a>
          <a class="${activeKey === "vouchers" ? "active" : ""}" href="admin-vouchers.html">Voucher</a>
          <a class="${activeKey === "reports" ? "active" : ""}" href="admin-export-reports.html">Xuất báo cáo</a>
          <button type="button" data-logout-trigger>Đăng xuất</button>
        </nav>
      </aside>`;
  }

  function renderAdminHeader(currentUser) {
    const initials = currentUser.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    return `
      <header class="admin-header">
        <button class="icon-button menu-toggle" type="button" data-menu-toggle aria-label="Mở menu">☰</button>
        <div class="header-search"><input type="search" placeholder="Tìm xe, người dùng, voucher"></div>
        <div class="header-user">
          <span class="name">${currentUser.full_name}</span>
          <span class="avatar">${initials}</span>
          <button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>Đăng xuất</button>
        </div>
      </header>`;
  }

  function renderLogoutModal() {
    return `
      <div class="modal-backdrop" id="logoutModal" role="dialog" aria-modal="true">
        <div class="modal">
          <div class="modal-header"><h2>Xác nhận đăng xuất</h2><button class="icon-button" type="button" data-close-modal>×</button></div>
          <p class="muted">Bạn có chắc chắn muốn đăng xuất khỏi VivuCar?</p>
          <div class="modal-actions">
            <button class="btn btn-secondary" id="btnCancelLogout" type="button">Hủy</button>
            <button class="btn btn-danger" id="btnConfirmLogout" type="button">Đăng xuất</button>
          </div>
        </div>
      </div>`;
  }

  function renderUserHeader(activeKey) {
    const currentUser = Auth.getCurrentUser();
    const avatar = currentUser?.avatar_url || window.VivuCarDB.user_documents.find((doc) => doc.user_id === currentUser?.id && doc.document_type === "avatar")?.file_url;
    return `
      <header class="user-header">
        <a class="sidebar-brand" href="home.html"><span class="brand-mark">VC</span><span>VivuCar</span></a>
        <nav class="user-nav" aria-label="User navigation">
          <a class="${activeKey === "home" ? "active" : ""}" href="home.html">Trang chủ</a>
          <a class="${activeKey === "search" ? "active" : ""}" href="search.html">Tìm xe</a>
          <a class="${activeKey === "featured" ? "active" : ""}" href="featured-cars.html">Xe nổi bật</a>
          <a class="${activeKey === "bookings" ? "active" : ""}" href="my-bookings.html">Đơn thuê</a>
          <a class="${activeKey === "profile" ? "active" : ""}" href="profile.html">Hồ sơ</a>
        </nav>
        <div class="header-user">
          ${currentUser ? `<a class="btn btn-secondary btn-sm" href="profile.html">${avatar ? `<img class="avatar" src="${avatar}" alt="${currentUser.full_name}">` : `<span class="avatar">${currentUser.full_name[0]}</span>`}<span class="name">${currentUser.full_name}</span></a><button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>Đăng xuất</button>` : `<a class="btn btn-primary btn-sm" href="login.html">Đăng nhập</a>`}
        </div>
      </header>`;
  }

  function appNavItems(role) {
    if (role === C.USER_ROLES.CAR_OWNER) {
      return [
        ["rent", "Thuê xe", "home.html"],
        ["my-bookings", "Đơn thuê của tôi", "my-bookings.html"],
        ["owner-cars", "Xe của tôi", "owner-cars.html"],
        ["owner-bookings", "Đơn đặt xe", "owner-booking-requests.html"],
        ["owner-handover", "Bàn giao & trả xe", "owner-handover-dashboard.html"],
        ["owner-support", "Hỗ trợ", "owner-support-inbox.html"],
        ["profile", "Hồ sơ", "profile.html"]
      ];
    }
    return [
      ["rent", "Thuê xe", "home.html"],
      ["my-bookings", "Đơn thuê của tôi", "my-bookings.html"],
      ["my-reviews", "Đánh giá của tôi", "my-reviews.html"],
      ["profile", "Hồ sơ", "profile.html"]
    ];
  }

  function renderAppHeader(activeKey) {
    const role = localStorage.getItem("vivucar_current_user_role") || C.USER_ROLES.USER;
    const currentUser = Auth.getCurrentUser();
    const navItems = appNavItems(role);
    const activeAliases = {
      home: "rent",
      search: "rent",
      featured: "rent",
      bookings: "my-bookings",
      "profile-edit": "profile",
      "driving-license": "profile",
      "owner-booking-requests": "owner-bookings",
      "owner-activity": "owner-cars"
    };
    const normalizedActiveKey = activeAliases[activeKey] || activeKey;
    const avatar = currentUser?.avatar_url || window.VivuCarDB.user_documents.find((doc) => doc.user_id === currentUser?.id && doc.document_type === "avatar")?.file_url;
    const initials = currentUser?.full_name ? currentUser.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase() : "VC";
    const navMarkup = navItems.map(([key, label, href]) => {
      const active = normalizedActiveKey === key;
      const activeClass = active ? "bg-neutral-900 text-white shadow-sm" : "text-neutral-600 hover:bg-[#efefed] hover:text-neutral-950";
      return `<a class="rounded-full px-3 py-2 text-sm font-semibold whitespace-nowrap transition ${activeClass}" href="${href}">${label}</a>`;
    }).join("");
    return `
      <header class="sticky top-0 z-40 border-b border-[#e6e4df] bg-[#f7f7f5]/92 backdrop-blur-xl">
        <div class="mx-auto flex min-h-[68px] w-[min(1440px,100%)] items-center gap-3 px-5 max-sm:px-4">
          <a class="flex shrink-0 items-center gap-2.5 font-extrabold text-neutral-950" href="home.html">
            <span class="grid h-8 w-8 place-items-center rounded-[9px] bg-neutral-900 text-sm font-extrabold text-white">VC</span>
            <span>VivuCar</span>
          </a>
          <nav class="ml-2 flex flex-1 items-center gap-1 overflow-x-auto rounded-full border border-[#e6e4df] bg-white p-1 max-lg:hidden" aria-label="App navigation">
            ${navMarkup}
          </nav>
          <button class="icon-button ml-auto hidden max-lg:inline-grid" type="button" data-app-menu-toggle aria-label="Mở menu">☰</button>
          <div class="hidden items-center gap-2 text-sm text-neutral-500 lg:flex">
            ${currentUser ? `<a class="btn btn-secondary btn-sm" href="profile.html">${avatar ? `<img class="avatar" src="${avatar}" alt="${currentUser.full_name}">` : `<span class="avatar">${initials}</span>`}<span class="max-xl:hidden">${currentUser.full_name}</span></a><button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>Đăng xuất</button>` : `<a class="btn btn-primary btn-sm" href="login.html">Đăng nhập</a>`}
          </div>
        </div>
        <nav class="mx-auto hidden w-[min(1440px,100%)] px-5 pb-4 max-sm:px-4" id="appMobileNav" aria-label="Mobile navigation">
          <div class="grid gap-1 rounded-2xl border border-[#e6e4df] bg-white p-2 shadow-sm">
            ${navMarkup}
            ${currentUser ? `<button class="btn btn-ghost btn-full justify-start" type="button" data-logout-trigger>Đăng xuất</button>` : `<a class="btn btn-primary btn-full" href="login.html">Đăng nhập</a>`}
          </div>
        </nav>
      </header>`;
  }

  function renderProfileSidebar(activeKey) {
    const items = [
      ["profile", "Thông tin cá nhân", "profile.html"],
      ["profile-edit", "Chỉnh sửa hồ sơ", "profile-edit.html"],
      ["driving-license", "Giấy phép lái xe", "driving-license.html"],
      ["my-reviews", "Đánh giá của tôi", "my-reviews.html"],
      ["bookings", "Đơn thuê của tôi", "my-bookings.html"]
    ];
    return `<aside class="profile-sidebar">${items.map(([key, label, href]) => `<a class="${activeKey === key ? "active" : ""}" href="${href}">${label}</a>`).join("")}<button type="button" data-logout-trigger>Đăng xuất</button></aside>`;
  }

  function renderBreadcrumb(items) {
    return `<nav class="breadcrumb">${items.map((item, index) => item.href && index < items.length - 1 ? `<a href="${item.href}">${item.label}</a>` : `<span>${item.label}</span>`).join(" / ")}</nav>`;
  }

  function renderOwnerHeader(currentUser) {
    const initials = currentUser.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    return `
      <header class="admin-header">
        <button class="icon-button menu-toggle" type="button" data-menu-toggle aria-label="Mở menu">☰</button>
        <div class="header-search"><input type="search" placeholder="Tìm xe, đơn thuê, sự cố"></div>
        <div class="header-user">
          <span class="name">${currentUser.full_name}</span>
          <span class="avatar">${initials}</span>
          <button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>Đăng xuất</button>
        </div>
      </header>`;
  }

  function renderOwnerSidebar(activeKey) {
    const items = [
      ["owner-booking-requests", "Yêu cầu đặt xe", "owner-booking-requests.html"],
      ["owner-handover", "Bàn giao & trả xe", "owner-handover-dashboard.html"],
      ["owner-cars", "Xe của tôi", "owner-cars.html"],
      ["owner-support", "Hỗ trợ khách hàng", "owner-support-inbox.html"],
      ["owner-activity", "Lịch sử hoạt động", "owner-car-activity-history.html?carId=1"],
      ["profile", "Hồ sơ", "profile.html"]
    ];
    return `
      <aside class="admin-sidebar owner-sidebar">
        <a class="sidebar-brand" href="owner-cars.html"><span class="brand-mark">VC</span><span>VivuCar Owner</span></a>
        <nav class="sidebar-nav">
          ${items.map(([key, label, href]) => `<a class="${activeKey === key ? "active" : ""}" href="${href}">${label}</a>`).join("")}
          <button type="button" data-logout-trigger>Đăng xuất</button>
        </nav>
      </aside>`;
  }

  const sidebarMount = document.getElementById("sidebarMount");
  const headerMount = document.getElementById("headerMount");
  const modalMount = document.getElementById("modalMount");
  if (layout === "admin") {
    if (sidebarMount) sidebarMount.innerHTML = renderAdminSidebar(page);
    if (headerMount) headerMount.innerHTML = renderAdminHeader(user);
  } else if (layout === "owner") {
    if (headerMount) headerMount.innerHTML = renderAppHeader(page);
  } else {
    if (headerMount) headerMount.innerHTML = renderAppHeader(page);
    const profileMount = document.getElementById("profileSidebarMount");
    if (profileMount) profileMount.innerHTML = renderProfileSidebar(page);
  }
  if (modalMount) modalMount.insertAdjacentHTML("beforeend", renderLogoutModal());

  document.addEventListener("click", (event) => {
    if (event.target.matches("[data-menu-toggle]")) {
      const sidebar = document.querySelector(".admin-sidebar");
      sidebar?.classList.toggle("max-lg:-translate-x-full");
      sidebar?.classList.toggle("max-lg:translate-x-0");
    }
    if (event.target.matches("[data-app-menu-toggle]")) {
      document.getElementById("appMobileNav")?.classList.toggle("hidden");
    }
    if (event.target.matches("[data-logout-trigger]")) U.openModal("logoutModal");
    if (event.target.matches("#btnCancelLogout, [data-close-modal]") || event.target.classList.contains("modal-backdrop")) {
      const modal = event.target.closest(".modal-backdrop");
      if (modal) U.closeModal(modal.id);
    }
  });
  document.getElementById("btnConfirmLogout")?.addEventListener("click", () => {
    document.getElementById("btnConfirmLogout").textContent = "Đang đăng xuất...";
    setTimeout(Auth.logout, 250);
  });
  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") document.querySelectorAll(".modal-backdrop.show").forEach((modal) => U.closeModal(modal.id));
  });

  window.VivuCarLayout = { renderAdminSidebar, renderAdminHeader, renderOwnerHeader, renderOwnerSidebar, renderUserHeader, renderAppHeader, renderProfileSidebar, renderBreadcrumb };
})();
