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
    const adminIcons = {
      chartLine: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M64 64c0-17.7-14.3-32-32-32S0 46.3 0 64v336c0 44.2 35.8 80 80 80h400c17.7 0 32-14.3 32-32s-14.3-32-32-32H80c-8.8 0-16-7.2-16-16V64zm375 111c9.4-9.4 9.4-24.6 0-33.9s-24.6-9.4-33.9 0l-87 87-39-39c-9.4-9.4-24.6-9.4-33.9 0l-96 96c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l79-79 39 39c9.4 9.4 24.6 9.4 33.9 0l104-104z"/></svg>',
      users: '<svg aria-hidden="true" viewBox="0 0 640 512" class="h-4 w-4 fill-current"><path d="M96 128a128 128 0 1 1 256 0A128 128 0 1 1 96 128zM0 482.3C0 383.8 79.8 304 178.3 304h91.4C368.2 304 448 383.8 448 482.3c0 16.4-13.3 29.7-29.7 29.7H29.7C13.3 512 0 498.7 0 482.3zM609.3 512H471.4c5.4-9.4 8.6-20.3 8.6-32.1C480 416.5 448.4 360.6 400.2 327c12.2-4.5 25.3-7 38.9-7h61.4C577.5 320 640 382.5 640 459.5c0 29-23.5 52.5-52.5 52.5zM432 256c-31 0-59-12.6-79.3-32.9C372.4 198.6 384 167.5 384 134.1c0-13.1-1.8-25.8-5.2-37.8C393.1 76.2 416.7 64 442.7 64C504.9 64 555.3 114.4 555.3 176.6S504.9 289.3 442.7 289.3c-3.6 0-7.2-.2-10.7-.5V256z"/></svg>',
      car: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M135.2 117.4 109.1 192h293.8l-26.1-74.6C372.3 104.6 360.2 96 346.6 96H165.4c-13.6 0-25.7 8.6-30.2 21.4zM39.6 196.8 74.8 96.3C88.3 57.8 124.6 32 165.4 32h181.2c40.8 0 77.1 25.8 90.6 64.3l35.2 100.5C495.6 207.6 512 231.1 512 258.5V400c0 26.5-21.5 48-48 48h-16v32c0 17.7-14.3 32-32 32h-32c-17.7 0-32-14.3-32-32v-32H160v32c0 17.7-14.3 32-32 32H96c-17.7 0-32-14.3-32-32v-32H48c-26.5 0-48-21.5-48-48V258.5c0-27.4 16.4-50.9 39.6-61.7zM128 352a48 48 0 1 0 0-96 48 48 0 1 0 0 96zm256 0a48 48 0 1 0 0-96 48 48 0 1 0 0 96z"/></svg>',
      ticket: '<svg aria-hidden="true" viewBox="0 0 576 512" class="h-4 w-4 fill-current"><path d="M64 64C28.7 64 0 92.7 0 128v80c0 8.8 7.4 15.7 15.7 18.6C34.5 233.1 48 251 48 272s-13.5 38.9-32.3 45.4C7.4 320.3 0 327.2 0 336v80c0 35.3 28.7 64 64 64h448c35.3 0 64-28.7 64-64v-80c0-8.8-7.4-15.7-15.7-18.6C541.5 310.9 528 293 528 272s13.5-38.9 32.3-45.4c8.3-2.9 15.7-9.8 15.7-18.6v-80c0-35.3-28.7-64-64-64H64zm64 112v192h320V176H128z"/></svg>',
      fileExport: '<svg aria-hidden="true" viewBox="0 0 576 512" class="h-4 w-4 fill-current"><path d="M0 64C0 28.7 28.7 0 64 0h224v128c0 17.7 14.3 32 32 32h128v96h-48c-26.5 0-48 21.5-48 48v32H160c-17.7 0-32 14.3-32 32s14.3 32 32 32h192v32c0 26.5 21.5 48 48 48h48c0 17.7-14.3 32-32 32H64c-35.3 0-64-28.7-64-64V64zm448 64H320V0l128 128zm-32 192v96c0 8.8 7.2 16 16 16h32V304h-32c-8.8 0-16 7.2-16 16zm112-32 43.3 43.3c6.2 6.2 6.2 16.4 0 22.6L528 397.3V368h-64v-32h64v-29.3z"/></svg>',
      logout: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M502.6 278.6c12.5-12.5 12.5-32.8 0-45.3l-128-128c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L402.7 224H192c-17.7 0-32 14.3-32 32s14.3 32 32 32h210.7l-73.4 73.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l128-128zM160 96c17.7 0 32-14.3 32-32s-14.3-32-32-32H96C43 32 0 75 0 128v256c0 53 43 96 96 96h64c17.7 0 32-14.3 32-32s-14.3-32-32-32H96c-17.7 0-32-14.3-32-32V128c0-17.7 14.3-32 32-32h64z"/></svg>'
    };
    const iconBox = (icon, active = false) => `<span class="admin-nav-icon ${active ? "admin-nav-icon-active" : ""}">${icon}</span>`;
    const activeIndicator = '<span class="admin-nav-indicator" aria-hidden="true"></span>';
    const items = [
      ["dashboard", "Dashboard doanh thu", "admin-dashboard.html", adminIcons.chartLine],
      ["users", "Người dùng", "admin-users.html", adminIcons.users],
      ["cars", "Phương tiện", "admin-cars.html", adminIcons.car],
      ["vouchers", "Voucher", "admin-vouchers.html", adminIcons.ticket],
      ["reports", "Xuất báo cáo", "admin-export-reports.html", adminIcons.fileExport]
    ];
    return `
      <aside class="admin-sidebar">
        <a class="sidebar-brand" href="admin-dashboard.html"><span class="brand-mark">${adminIcons.car}</span><span class="grid leading-tight"><span>VivuCar</span><span class="text-xs font-semibold text-zinc-500">Admin Console</span></span></a>
        <nav class="sidebar-nav" aria-label="Admin navigation">
          ${items.map(([key, label, href, icon]) => {
            const active = activeKey === key;
            return `<a class="group ${active ? "admin-nav-active" : ""}" href="${href}">${active ? activeIndicator : ""}${iconBox(icon, active)}${label}</a>`;
          }).join("")}
        </nav>
        <nav class="sidebar-nav mt-auto" aria-label="Admin account">
          <button class="group" type="button" data-logout-trigger><span class="admin-nav-icon admin-nav-icon-logout">${adminIcons.logout}</span>Đăng xuất</button>
        </nav>
      </aside>`;
  }
  function renderAdminHeader(currentUser) {
    const initials = currentUser.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    return `
      <header class="admin-header">
        <button class="icon-button menu-toggle" type="button" data-menu-toggle aria-label="Mở menu">Menu</button>
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
    const ownerAliases = {
      "owner-booking-detail": "owner-bookings",
      "owner-booking-tracking": "owner-bookings",
      "owner-car-create": "owner-cars",
      "owner-car-edit": "owner-cars",
      "owner-car-images": "owner-cars",
      "owner-car-status": "owner-cars",
      "owner-car-activity": "owner-cars",
      "owner-handover-condition": "owner-handover",
      "owner-return-condition": "owner-handover",
      "owner-return-inspection": "owner-handover"
    };
    const resolvedKey = ownerAliases[activeKey] || activeKey;
    const ownerIcons = {
      dashboard: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M0 256a256 256 0 1 1 512 0A256 256 0 1 1 0 256zm320 96c0-26.9-16.5-49.9-40-59.3V88c0-13.3-10.7-24-24-24s-24 10.7-24 24v204.7c-23.5 9.5-40 32.5-40 59.3c0 35.3 28.7 64 64 64s64-28.7 64-64zM144 176a32 32 0 1 0 0-64 32 32 0 1 0 0 64zm-16 80a32 32 0 1 0 -64 0 32 32 0 1 0 64 0zm288 32a32 32 0 1 0 0-64 32 32 0 1 0 0 64zM400 144a32 32 0 1 0 -64 0 32 32 0 1 0 64 0z"/></svg>',
      car: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M135.2 117.4 109.1 192h293.8l-26.1-74.6C372.3 104.6 360.2 96 346.6 96H165.4c-13.6 0-25.7 8.6-30.2 21.4zM39.6 196.8 74.8 96.3C88.3 57.8 124.6 32 165.4 32h181.2c40.8 0 77.1 25.8 90.6 64.3l35.2 100.5C495.6 207.6 512 231.1 512 258.5V400c0 26.5-21.5 48-48 48h-16v32c0 17.7-14.3 32-32 32h-32c-17.7 0-32-14.3-32-32v-32H160v32c0 17.7-14.3 32-32 32H96c-17.7 0-32-14.3-32-32v-32H48c-26.5 0-48-21.5-48-48V258.5c0-27.4 16.4-50.9 39.6-61.7zM128 352a48 48 0 1 0 0-96 48 48 0 1 0 0 96zm256 0a48 48 0 1 0 0-96 48 48 0 1 0 0 96z"/></svg>',
      booking: '<svg aria-hidden="true" viewBox="0 0 448 512" class="h-4 w-4 fill-current"><path d="M152 24c0-13.3-10.7-24-24-24s-24 10.7-24 24V64H64C28.7 64 0 92.7 0 128v16 48V448c0 35.3 28.7 64 64 64H384c35.3 0 64-28.7 64-64V192 144 128c0-35.3-28.7-64-64-64H344V24c0-13.3-10.7-24-24-24s-24 10.7-24 24V64H152V24zM48 192H400V448c0 8.8-7.2 16-16 16H64c-8.8 0-16-7.2-16-16V192zm176 40c-13.3 0-24 10.7-24 24v48H152c-13.3 0-24 10.7-24 24s10.7 24 24 24h48v48c0 13.3 10.7 24 24 24s24-10.7 24-24V352h48c13.3 0 24-10.7 24-24s-10.7-24-24-24H248V256c0-13.3-10.7-24-24-24z"/></svg>',
      handover: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M32 96l320 0V32c0-12.9 7.8-24.6 19.8-29.6s25.7-2.2 34.9 6.9l96 96c6 6 9.4 14.1 9.4 22.6s-3.4 16.6-9.4 22.6l-96 96c-9.2 9.2-22.9 11.9-34.9 6.9s-19.8-16.6-19.8-29.6V160L32 160c-17.7 0-32-14.3-32-32s14.3-32 32-32zM480 352c17.7 0 32 14.3 32 32s-14.3 32-32 32H160v32c0 12.9-7.8 24.6-19.8 29.6s-25.7 2.2-34.9-6.9l-96-96c-6-6-9.4-14.1-9.4-22.6s3.4-16.6 9.4-22.6l96-96c9.2-9.2 22.9-11.9 34.9-6.9s19.8 16.6 19.8 29.6l0 32H480z"/></svg>',
      support: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M256 48C141.1 48 48 141.1 48 256v40c0 13.3-10.7 24-24 24s-24-10.7-24-24V256C0 114.6 114.6 0 256 0S512 114.6 512 256V400.1c0 48.6-39.4 88-88.1 88L313.6 488c-8.3 14.3-23.8 24-41.6 24H240c-26.5 0-48-21.5-48-48s21.5-48 48-48h32c17.8 0 33.3 9.7 41.6 24l110.4 .1c22.1 0 40-17.9 40-40V256c0-114.9-93.1-208-208-208zM144 208a112 112 0 1 1 224 0 112 112 0 1 1 -224 0z"/></svg>',
      logout: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M502.6 278.6c12.5-12.5 12.5-32.8 0-45.3l-128-128c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L402.7 224H192c-17.7 0-32 14.3-32 32s14.3 32 32 32h210.7l-73.4 73.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l128-128zM160 96c17.7 0 32-14.3 32-32s-14.3-32-32-32H96C43 32 0 75 0 128v256c0 53 43 96 96 96h64c17.7 0 32-14.3 32-32s-14.3-32-32-32H96c-17.7 0-32-14.3-32-32V128c0-17.7 14.3-32 32-32h64z"/></svg>'
    };
    const iconBox = (icon, active = false) => `<span class="admin-nav-icon ${active ? 'admin-nav-icon-active' : ''}">${icon}</span>`;
    const activeIndicator = '<span class="admin-nav-indicator" aria-hidden="true"></span>';
    const items = [
      ["owner-dashboard", "Dashboard", "owner-dashboard.html", ownerIcons.dashboard],
      ["owner-cars", "Quản lý xe", "owner-cars.html", ownerIcons.car],
      ["owner-bookings", "Yêu cầu đặt xe", "owner-booking-requests.html", ownerIcons.booking],
      ["owner-handover", "Bàn giao & Trả xe", "owner-handover-dashboard.html", ownerIcons.handover],
      ["owner-support", "Hỗ trợ khách hàng", "owner-support-inbox.html", ownerIcons.support]
    ];
    return `
      <aside class="admin-sidebar owner-sidebar">
        <a class="sidebar-brand" href="owner-dashboard.html"><span class="brand-mark">VC</span><span class="grid leading-tight"><span>VivuCar</span><span class="text-xs font-semibold text-zinc-500">Owner Portal</span></span></a>
        <nav class="sidebar-nav" aria-label="Owner navigation">
          ${items.map(([key, label, href, icon]) => {
            const active = resolvedKey === key;
            return `<a class="group ${active ? 'admin-nav-active' : ''}" href="${href}">${active ? activeIndicator : ''}${iconBox(icon, active)}${label}</a>`;
          }).join('')}
        </nav>
        <nav class="sidebar-nav mt-auto" aria-label="Owner account">
          <button class="group" type="button" data-logout-trigger><span class="admin-nav-icon admin-nav-icon-logout">${ownerIcons.logout}</span>Đăng xuất</button>
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
    if (sidebarMount) sidebarMount.innerHTML = renderOwnerSidebar(page);
    if (headerMount) headerMount.innerHTML = renderOwnerHeader(user);
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
