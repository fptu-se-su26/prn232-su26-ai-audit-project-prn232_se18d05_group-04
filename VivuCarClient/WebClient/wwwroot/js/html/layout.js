(function () {
  const C = window.VivuCarConstants;
  const Auth = window.VivuCarAuth;
  const U = window.VivuCarUtils;
  const page = document.body.dataset.page;
  const layout = document.body.dataset.layout || "admin";
  const publicPage = document.body.dataset.public === "true";
  const appRoles = [C.USER_ROLES.USER, C.USER_ROLES.CAR_OWNER];
  const allowed = layout === "admin" ? [C.USER_ROLES.ADMIN] : layout === "owner" ? [C.USER_ROLES.CAR_OWNER] : appRoles;
  const user = (typeof Auth !== "undefined" ? Auth.getCurrentUser() : null) || { full_name: "Administrator" }; // Bypassed for Razor Pages auth
  const expandedSidebarWidth = "300px";
  const collapsedSidebarWidth = "76px";
  let isSidebarCollapsed = localStorage.getItem("vivucar_admin_sidebar_collapsed") === "true";

  function renderAdminSidebar(activeKey) {
    const adminIcons = {
      chartLine: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M64 64c0-17.7-14.3-32-32-32S0 46.3 0 64v336c0 44.2 35.8 80 80 80h400c17.7 0 32-14.3 32-32s-14.3-32-32-32H80c-8.8 0-16-7.2-16-16V64zm375 111c9.4-9.4 9.4-24.6 0-33.9s-24.6-9.4-33.9 0l-87 87-39-39c-9.4-9.4-24.6-9.4-33.9 0l-96 96c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l79-79 39 39c9.4 9.4 24.6 9.4 33.9 0l104-104z"/></svg>',
      users: '<svg aria-hidden="true" viewBox="0 0 640 512" class="h-4 w-4 fill-current"><path d="M96 128a128 128 0 1 1 256 0A128 128 0 1 1 96 128zM0 482.3C0 383.8 79.8 304 178.3 304h91.4C368.2 304 448 383.8 448 482.3c0 16.4-13.3 29.7-29.7 29.7H29.7C13.3 512 0 498.7 0 482.3zM609.3 512H471.4c5.4-9.4 8.6-20.3 8.6-32.1C480 416.5 448.4 360.6 400.2 327c12.2-4.5 25.3-7 38.9-7h61.4C577.5 320 640 382.5 640 459.5c0 29-23.5 52.5-52.5 52.5zM432 256c-31 0-59-12.6-79.3-32.9C372.4 198.6 384 167.5 384 134.1c0-13.1-1.8-25.8-5.2-37.8C393.1 76.2 416.7 64 442.7 64C504.9 64 555.3 114.4 555.3 176.6S504.9 289.3 442.7 289.3c-3.6 0-7.2-.2-10.7-.5V256z"/></svg>',
      car: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M135.2 117.4 109.1 192h293.8l-26.1-74.6C372.3 104.6 360.2 96 346.6 96H165.4c-13.6 0-25.7 8.6-30.2 21.4zM39.6 196.8 74.8 96.3C88.3 57.8 124.6 32 165.4 32h181.2c40.8 0 77.1 25.8 90.6 64.3l35.2 100.5C495.6 207.6 512 231.1 512 258.5V400c0 26.5-21.5 48-48 48h-16v32c0 17.7-14.3 32-32 32h-32c-17.7 0-32-14.3-32-32v-32H160v32c0 17.7-14.3 32-32 32H96c-17.7 0-32-14.3-32-32v-32H48c-26.5 0-48-21.5-48-48V258.5c0-27.4 16.4-50.9 39.6-61.7zM128 352a48 48 0 1 0 0-96 48 48 0 1 0 0 96zm256 0a48 48 0 1 0 0-96 48 48 0 1 0 0 96z"/></svg>',
      ticket: '<svg aria-hidden="true" viewBox="0 0 576 512" class="h-4 w-4 fill-current"><path d="M64 64C28.7 64 0 92.7 0 128v80c0 8.8 7.4 15.7 15.7 18.6C34.5 233.1 48 251 48 272s-13.5 38.9-32.3 45.4C7.4 320.3 0 327.2 0 336v80c0 35.3 28.7 64 64 64h448c35.3 0 64-28.7 64-64v-80c0-8.8-7.4-15.7-15.7-18.6C541.5 310.9 528 293 528 272s13.5-38.9 32.3-45.4c8.3-2.9 15.7-9.8 15.7-18.6v-80c0-35.3-28.7-64-64-64H64zm64 112v192h320V176H128z"/></svg>',
      fileExport: '<svg aria-hidden="true" viewBox="0 0 576 512" class="h-4 w-4 fill-current"><path d="M0 64C0 28.7 28.7 0 64 0h224v128c0 17.7 14.3 32 32 32h128v96h-48c-26.5 0-48 21.5-48 48v32H160c-17.7 0-32 14.3-32 32s14.3 32 32 32h192v32c0 26.5 21.5 48 48 48h48c0 17.7-14.3 32-32 32H64c-35.3 0-64-28.7-64-64V64zm448 64H320V0l128 128zm-32 192v96c0 8.8 7.2 16 16 16h32V304h-32c-8.8 0-16 7.2-16 16zm112-32 43.3 43.3c6.2 6.2 6.2 16.4 0 22.6L528 397.3V368h-64v-32h64v-29.3z"/></svg>',
      trash: '<svg aria-hidden="true" viewBox="0 0 448 512" class="h-4 w-4 fill-current"><path d="M135.2 17.7C140.6 6.8 151.7 0 163.8 0h120.4c12.1 0 23.2 6.8 28.6 17.7L328 48h88c17.7 0 32 14.3 32 32s-14.3 32-32 32H32C14.3 112 0 97.7 0 80s14.3-32 32-32h88l15.2-30.3zM32 144h384l-21.2 339.3C393.2 499.4 379.8 512 363.6 512H84.4c-16.2 0-29.6-12.6-31.2-28.7L32 144zm96 64c-8.8 0-16 7.2-16 16v208c0 8.8 7.2 16 16 16s16-7.2 16-16V224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16v208c0 8.8 7.2 16 16 16s16-7.2 16-16V224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16v208c0 8.8 7.2 16 16 16s16-7.2 16-16V224c0-8.8-7.2-16-16-16z"/></svg>',
      shieldCheck: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M256 0c4.6 0 9.2 1 13.4 2.9l192 80C473.5 88 481 99.8 480 112.9c-7.5 98.1-48.4 188.8-110.2 257.6C309.9 437.2 247.1 474.6 265.7 487.9c-5.8 4.1-13.6 4.1-19.4 0C171 474.6 108.1 437.2 48.2 370.5C-13.6 301.7-54.5 211-62 112.9C-63 99.8-55.5 88 56.6 82.9l192-80C246.8 1 251.4 0 256 0zm116.7 180.7c6.2-6.2 6.2-16.4 0-22.6s-16.4-6.2-22.6 0L224 284.1l-62.1-62.1c-6.2-6.2-16.4-6.2-22.6 0s-6.2 16.4 0 22.6l73.4 73.4c6.2 6.2 16.4 6.2 22.6 0l137.4-137.4z"/></svg>',
      contentWarning: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M64 32C28.7 32 0 60.7 0 96v256c0 35.3 28.7 64 64 64h96v80c0 6.1 3.4 11.6 8.8 14.3s11.9 2.1 16.8-1.5L309.3 416H448c35.3 0 64-28.7 64-64V96c0-35.3-28.7-64-64-64H64zm192 96c8.8 0 16 7.2 16 16v96c0 8.8-7.2 16-16 16s-16-7.2-16-16v-96c0-8.8 7.2-16 16-16zm-20 180a20 20 0 1 1 40 0 20 20 0 1 1-40 0z"/></svg>',
      idCard: '<svg aria-hidden="true" viewBox="0 0 576 512" class="h-4 w-4 fill-current"><path d="M0 96C0 60.7 28.7 32 64 32h448c35.3 0 64 28.7 64 64v320c0 35.3-28.7 64-64 64H64c-35.3 0-64-28.7-64-64V96zm96 80v64h160v-64H96zm0 112v32h256v-32H96zm0 64v32h192v-32H96zm320-176a64 64 0 1 0 0 128 64 64 0 1 0 0-128zm-96 224h192c0-44.2-43-80-96-80s-96 35.8-96 80z"/></svg>',
      logout: '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M502.6 278.6c12.5-12.5 12.5-32.8 0-45.3l-128-128c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L402.7 224H192c-17.7 0-32 14.3-32 32s14.3 32 32 32h210.7l-73.4 73.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l128-128zM160 96c17.7 0 32-14.3 32-32s-14.3-32-32-32H96C43 32 0 75 0 128v256c0 53 43 96 96 96h64c17.7 0 32-14.3 32-32s-14.3-32-32-32H96c-17.7 0-32-14.3-32-32V128c0-17.7 14.3-32 32-32h64z"/></svg>'
    };
    const contentPendingCount = (window.VivuCarDB?.reviews || []).filter((item) => !item.deleted_at && ["pending", "reported"].includes(item.moderation_status)).length
      + (window.VivuCarDB?.incident_reports || []).filter((item) => !item.deleted_at && ["pending", "reported"].includes(item.moderation_status)).length;
    const licensePendingUsers = new Set((window.VivuCarDB?.user_documents || []).filter((item) => ["license_front", "license_back"].includes(item.document_type) && item.moderation_status === "pending").map((item) => item.user_id));
    const licensePendingCount = licensePendingUsers.size;
    const trashCount = ["users", "cars", "vouchers", "reviews", "incident_reports"].reduce((sum, key) => sum + (window.VivuCarDB?.[key] || []).filter((item) => item.deleted_at).length, 0);
    const searchIcon = '<svg aria-hidden="true" viewBox="0 0 512 512" class="h-4 w-4 fill-current"><path d="M416 208c0 45.9-14.9 88.3-40 122.7L502.6 457.4c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0L330.7 376c-34.4 25.2-76.8 40-122.7 40C93.1 416 0 322.9 0 208S93.1 0 208 0s208 93.1 208 208zM208 352a144 144 0 1 0 0-288 144 144 0 1 0 0 288z"/></svg>';
    const chevronLeft = '<svg aria-hidden="true" viewBox="0 0 320 512" class="h-3.5 w-3.5 fill-current"><path d="M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l192 192c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L77.3 256 246.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-192 192z"/></svg>';
    const chevronRight = '<svg aria-hidden="true" viewBox="0 0 320 512" class="h-3.5 w-3.5 fill-current"><path d="M310.6 233.4c12.5 12.5 12.5 32.8 0 45.3l-192 192c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3L242.7 256 73.4 86.6c-12.5-12.5-12.5-32.8 0-45.3s32.8-12.5 45.3 0l192 192z"/></svg>';
    const iconBox = (icon, active = false) => `<span class="admin-nav-icon ${active ? "admin-nav-icon-active" : ""}">${icon}</span>`;
    const initials = user.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    const activeIndicator = '<span class="admin-nav-indicator" aria-hidden="true"></span>';
    const items = [
      ["dashboard", "Dashboard doanh thu", "admin-dashboard.html", adminIcons.chartLine],
      ["users", "NgÆ°á»i dÃ¹ng", "admin-users.html", adminIcons.users],
      ["cars", "PhÆ°Æ¡ng tiá»‡n", "admin-cars.html", adminIcons.car],
      ["vouchers", "Voucher", "admin-vouchers.html", adminIcons.ticket],
      ["reports", "Xuáº¥t bÃ¡o cÃ¡o", "admin-export-reports.html", adminIcons.fileExport],
      ["trash", "ThÃ¹ng rÃ¡c", "admin-trash.html", adminIcons.trash, trashCount]
    ];
    const moderationItems = [
      ["moderation-content", "Kiá»ƒm duyá»‡t ná»™i dung", "admin-moderation-content.html", adminIcons.contentWarning, contentPendingCount],
      ["moderation-licenses", "Kiá»ƒm duyá»‡t GPLX", "admin-moderation-licenses.html", adminIcons.idCard, licensePendingCount]
    ];
    const renderSidebarItem = ([key, label, href, icon, count], isChild = false) => {
      const active = activeKey === key;
      const badge = Number(count) > 0 ? `<span class="ml-auto rounded-full ${Number(count) > 3 ? "bg-red-100 text-red-700" : "bg-amber-100 text-amber-700"} px-2 py-0.5 text-xs font-semibold">${count}</span>` : "";
      return `<a class="group ${active ? "admin-nav-active" : ""} ${isChild ? "admin-nav-child" : ""}" href="${href}" title="${label}">${active ? activeIndicator : ""}${iconBox(icon, active)}<span class="admin-nav-label">${label}</span>${badge}</a>`;
    };
    return `
      <aside class="admin-sidebar" data-admin-sidebar>
        <div class="flex items-center gap-2">
          <a class="sidebar-brand min-w-0 flex-1" href="admin-dashboard.html" title="VivuCar Admin" data-sidebar-logo>
            <span class="brand-mark">${adminIcons.car}</span>
            <span class="admin-sidebar-title"><span>VivuCar</span><span class="text-xs font-semibold text-zinc-500">Admin Console</span></span>
          </a>
          <button class="admin-sidebar-toggle" type="button" data-sidebar-collapse-toggle aria-label="Thu gá»n sidebar" title="Thu gá»n sidebar">${chevronLeft}</button>
        </div>
        <div class="admin-sidebar-search">
          <span class="admin-search-icon">${searchIcon}</span>
          <input class="pl-9" type="search" placeholder="TÃ¬m kiáº¿m..." aria-label="TÃ¬m kiáº¿m trong admin">
        </div>
        <nav class="sidebar-nav" aria-label="Admin navigation">
          ${items.slice(0, 4).map((item) => renderSidebarItem(item)).join("")}
          <div class="admin-nav-label mt-3 px-3 text-xs font-semibold uppercase tracking-wide text-zinc-400">Kiá»ƒm duyá»‡t</div>
          ${moderationItems.map((item) => renderSidebarItem(item, true)).join("")}
          ${items.slice(4).map((item) => renderSidebarItem(item)).join("")}
        </nav>
        <nav class="sidebar-nav admin-user-panel" aria-label="Admin account">
          <div class="flex items-center gap-3 px-2">
            <span class="avatar shrink-0">${initials}</span>
            <div class="admin-nav-label min-w-0 flex-1">
              <div class="truncate text-sm font-semibold text-zinc-900">${user.full_name}</div>
              <div class="text-xs text-zinc-500">Admin</div>
            </div>
          </div>
          <button class="group" type="button" data-logout-trigger title="ÄÄƒng xuáº¥t"><span class="admin-nav-icon admin-nav-icon-logout">${adminIcons.logout}</span><span class="admin-nav-label">ÄÄƒng xuáº¥t</span></button>
        </nav>
      </aside>`;
  }
  function renderAdminHeader(currentUser) {
    const initials = currentUser.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    return `
      <header class="admin-header">
        <button class="icon-button menu-toggle" type="button" data-menu-toggle aria-label="Má»Ÿ menu">Menu</button>
        <div></div>
        <div class="header-user">
          <span class="name">${currentUser.full_name}</span>
          <span class="avatar">${initials}</span>
        </div>
      </header>`;
  }
  function renderLogoutModal() {
    return `
      <div class="modal-backdrop" id="logoutModal" role="dialog" aria-modal="true">
        <div class="modal">
          <div class="modal-header"><h2>XÃ¡c nháº­n Ä‘Äƒng xuáº¥t</h2><button class="icon-button" type="button" data-close-modal>Ã—</button></div>
          <p class="muted">Báº¡n cÃ³ cháº¯c cháº¯n muá»‘n Ä‘Äƒng xuáº¥t khá»i VivuCar?</p>
          <div class="modal-actions">
            <button class="btn btn-secondary" id="btnCancelLogout" type="button">Há»§y</button>
            <button class="btn btn-danger" id="btnConfirmLogout" type="button">ÄÄƒng xuáº¥t</button>
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
          <a class="${activeKey === "home" ? "active" : ""}" href="home.html">Trang chá»§</a>
          <a class="${activeKey === "search" ? "active" : ""}" href="search.html">TÃ¬m xe</a>
          <a class="${activeKey === "featured" ? "active" : ""}" href="featured-cars.html">Xe ná»•i báº­t</a>
          <a class="${activeKey === "bookings" ? "active" : ""}" href="my-bookings.html">ÄÆ¡n thuÃª</a>
          <a class="${activeKey === "profile" ? "active" : ""}" href="profile.html">Há»“ sÆ¡</a>
        </nav>
        <div class="header-user">
          ${currentUser ? `<a class="btn btn-secondary btn-sm" href="profile.html">${avatar ? `<img class="avatar" src="${avatar}" alt="${currentUser.full_name}">` : `<span class="avatar">${currentUser.full_name[0]}</span>`}<span class="name">${currentUser.full_name}</span></a><button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>ÄÄƒng xuáº¥t</button>` : `<a class="btn btn-primary btn-sm" href="login.html">ÄÄƒng nháº­p</a>`}
        </div>
      </header>`;
  }

  function appNavItems(role) {
    if (role === C.USER_ROLES.CAR_OWNER) {
      return [
        ["rent", "ThuÃª xe", "home.html"],
        ["my-bookings", "ÄÆ¡n thuÃª cá»§a tÃ´i", "my-bookings.html"],
        ["owner-cars", "Xe cá»§a tÃ´i", "owner-cars.html"],
        ["owner-bookings", "ÄÆ¡n Ä‘áº·t xe", "owner-booking-requests.html"],
        ["owner-handover", "BÃ n giao & tráº£ xe", "owner-handover-dashboard.html"],
        ["owner-support", "Há»— trá»£", "owner-support-inbox.html"],
        ["profile", "Há»“ sÆ¡", "profile.html"]
      ];
    }
    return [
      ["rent", "ThuÃª xe", "home.html"],
      ["my-bookings", "ÄÆ¡n thuÃª cá»§a tÃ´i", "my-bookings.html"],
      ["my-reviews", "ÄÃ¡nh giÃ¡ cá»§a tÃ´i", "my-reviews.html"],
      ["profile", "Há»“ sÆ¡", "profile.html"]
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
          <button class="icon-button ml-auto hidden max-lg:inline-grid" type="button" data-app-menu-toggle aria-label="Má»Ÿ menu">â˜°</button>
          <div class="hidden items-center gap-2 text-sm text-neutral-500 lg:flex">
            ${currentUser ? `<a class="btn btn-secondary btn-sm" href="profile.html">${avatar ? `<img class="avatar" src="${avatar}" alt="${currentUser.full_name}">` : `<span class="avatar">${initials}</span>`}<span class="max-xl:hidden">${currentUser.full_name}</span></a><button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>ÄÄƒng xuáº¥t</button>` : `<a class="btn btn-primary btn-sm" href="login.html">ÄÄƒng nháº­p</a>`}
          </div>
        </div>
        <nav class="mx-auto hidden w-[min(1440px,100%)] px-5 pb-4 max-sm:px-4" id="appMobileNav" aria-label="Mobile navigation">
          <div class="grid gap-1 rounded-2xl border border-[#e6e4df] bg-white p-2 shadow-sm">
            ${navMarkup}
            ${currentUser ? `<button class="btn btn-ghost btn-full justify-start" type="button" data-logout-trigger>ÄÄƒng xuáº¥t</button>` : `<a class="btn btn-primary btn-full" href="login.html">ÄÄƒng nháº­p</a>`}
          </div>
        </nav>
      </header>`;
  }

  function renderProfileSidebar(activeKey) {
    const items = [
      ["profile", "ThÃ´ng tin cÃ¡ nhÃ¢n", "profile.html"],
      ["profile-edit", "Chá»‰nh sá»­a há»“ sÆ¡", "profile-edit.html"],
      ["driving-license", "Giáº¥y phÃ©p lÃ¡i xe", "driving-license.html"],
      ["my-reviews", "ÄÃ¡nh giÃ¡ cá»§a tÃ´i", "my-reviews.html"],
      ["bookings", "ÄÆ¡n thuÃª cá»§a tÃ´i", "my-bookings.html"]
    ];
    return `<aside class="profile-sidebar">${items.map(([key, label, href]) => `<a class="${activeKey === key ? "active" : ""}" href="${href}">${label}</a>`).join("")}<button type="button" data-logout-trigger>ÄÄƒng xuáº¥t</button></aside>`;
  }

  function renderBreadcrumb(items) {
    return `<nav class="breadcrumb">${items.map((item, index) => item.href && index < items.length - 1 ? `<a href="${item.href}">${item.label}</a>` : `<span>${item.label}</span>`).join(" / ")}</nav>`;
  }

  function renderOwnerHeader(currentUser) {
    const initials = currentUser.full_name.split(" ").slice(-2).map((part) => part[0]).join("").toUpperCase();
    return `
      <header class="admin-header">
        <button class="icon-button menu-toggle" type="button" data-menu-toggle aria-label="Má»Ÿ menu">â˜°</button>
        <div class="header-search"><input type="search" placeholder="TÃ¬m xe, Ä‘Æ¡n thuÃª, sá»± cá»‘"></div>
        <div class="header-user">
          <span class="name">${currentUser.full_name}</span>
          <span class="avatar">${initials}</span>
          <button class="btn btn-ghost btn-sm" type="button" data-logout-trigger>ÄÄƒng xuáº¥t</button>
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
      ["owner-cars", "Quáº£n lÃ½ xe", "owner-cars.html", ownerIcons.car],
      ["owner-bookings", "YÃªu cáº§u Ä‘áº·t xe", "owner-booking-requests.html", ownerIcons.booking],
      ["owner-handover", "BÃ n giao & Tráº£ xe", "owner-handover-dashboard.html", ownerIcons.handover],
      ["owner-support", "Há»— trá»£ khÃ¡ch hÃ ng", "owner-support-inbox.html", ownerIcons.support]
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
          <button class="group" type="button" data-logout-trigger><span class="admin-nav-icon admin-nav-icon-logout">${ownerIcons.logout}</span>ÄÄƒng xuáº¥t</button>
        </nav>
      </aside>`;
  }

  const sidebarMount = document.getElementById("sidebarMount");
  const headerMount = document.getElementById("headerMount");
  const modalMount = document.getElementById("modalMount");
  if (layout === "admin") {
    if (sidebarMount) sidebarMount.innerHTML = renderAdminSidebar(page);
    if (headerMount) headerMount.innerHTML = "";
  } else if (layout === "owner") {
    if (sidebarMount) sidebarMount.innerHTML = renderOwnerSidebar(page);
    if (headerMount) headerMount.innerHTML = renderOwnerHeader(user);
  } else {
    if (headerMount) headerMount.innerHTML = renderAppHeader(page);
    const profileMount = document.getElementById("profileSidebarMount");
    if (profileMount) profileMount.innerHTML = renderProfileSidebar(page);
  }
  if (modalMount) modalMount.insertAdjacentHTML("beforeend", renderLogoutModal());

  function applyAdminSidebarState() {
    if (layout !== "admin") return;
    const sidebar = document.querySelector("[data-admin-sidebar]");
    const appShell = document.querySelector(".app-shell");
    if (!sidebar || !appShell) return;
    const isDesktop = window.matchMedia("(min-width: 1024px)").matches;
    const width = isDesktop && isSidebarCollapsed ? collapsedSidebarWidth : expandedSidebarWidth;
    sidebar.style.width = width;
    appShell.style.paddingLeft = isDesktop ? width : "";
    sidebar.querySelectorAll(".admin-nav-label, .admin-sidebar-title").forEach((element) => {
      element.style.opacity = isDesktop && isSidebarCollapsed ? "0" : "1";
      element.style.width = isDesktop && isSidebarCollapsed ? "0px" : "";
      element.style.overflow = "hidden";
      element.style.pointerEvents = isDesktop && isSidebarCollapsed ? "none" : "";
    });
    sidebar.querySelectorAll(".sidebar-nav a, .sidebar-nav button").forEach((item) => {
      item.style.justifyContent = isDesktop && isSidebarCollapsed ? "center" : "";
      item.style.paddingLeft = isDesktop && isSidebarCollapsed ? "0.625rem" : "";
      item.style.paddingRight = isDesktop && isSidebarCollapsed ? "0.625rem" : "";
    });
    sidebar.querySelectorAll(".sidebar-nav .ml-auto").forEach((badge) => {
      badge.style.display = isDesktop && isSidebarCollapsed ? "none" : "";
    });
    const search = sidebar.querySelector(".admin-sidebar-search");
    if (search) search.style.display = isDesktop && isSidebarCollapsed ? "none" : "";
    const brand = sidebar.querySelector("[data-sidebar-logo]");
    if (brand) {
      brand.style.justifyContent = isDesktop && isSidebarCollapsed ? "center" : "";
      brand.style.flex = isDesktop && isSidebarCollapsed ? "0 0 auto" : "";
      brand.style.width = isDesktop && isSidebarCollapsed ? "100%" : "";
      brand.style.paddingLeft = isDesktop && isSidebarCollapsed ? "0" : "";
      brand.style.paddingRight = isDesktop && isSidebarCollapsed ? "0" : "";
    }
    sidebar.querySelectorAll(".brand-mark").forEach((mark) => {
      mark.style.width = "2.5rem";
      mark.style.height = "2.5rem";
      mark.style.minWidth = "2.5rem";
      mark.style.minHeight = "2.5rem";
    });
    const toggle = sidebar.querySelector("[data-sidebar-collapse-toggle]");
    if (toggle) {
      toggle.style.display = isDesktop && isSidebarCollapsed ? "none" : "inline-grid";
      toggle.innerHTML = isDesktop && isSidebarCollapsed
        ? '<svg aria-hidden="true" viewBox="0 0 320 512" class="h-3.5 w-3.5 fill-current"><path d="M310.6 233.4c12.5 12.5 12.5 32.8 0 45.3l-192 192c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3L242.7 256 73.4 86.6c-12.5-12.5-12.5-32.8 0-45.3s32.8-12.5 45.3 0l192 192z"/></svg>'
        : '<svg aria-hidden="true" viewBox="0 0 320 512" class="h-3.5 w-3.5 fill-current"><path d="M9.4 233.4c-12.5 12.5-12.5 32.8 0 45.3l192 192c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L77.3 256 246.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-192 192z"/></svg>';
      toggle.setAttribute("aria-label", isDesktop && isSidebarCollapsed ? "Má»Ÿ rá»™ng sidebar" : "Thu gá»n sidebar");
      toggle.setAttribute("title", isDesktop && isSidebarCollapsed ? "Má»Ÿ rá»™ng sidebar" : "Thu gá»n sidebar");
    }
  }

  function refreshAdminSidebar() {
    if (layout !== "admin" || !sidebarMount) return;
    sidebarMount.innerHTML = renderAdminSidebar(page);
    window.VivuCarTailwindUI?.applyTailwind(sidebarMount);
    applyAdminSidebarState();
  }

  document.addEventListener("click", (event) => {
    const logo = event.target.closest("[data-sidebar-logo]");
    if (logo && layout === "admin" && isSidebarCollapsed && window.matchMedia("(min-width: 1024px)").matches) {
      event.preventDefault();
      isSidebarCollapsed = false;
      localStorage.setItem("vivucar_admin_sidebar_collapsed", "false");
      applyAdminSidebarState();
      return;
    }
    if (event.target.closest("[data-sidebar-collapse-toggle]")) {
      isSidebarCollapsed = !isSidebarCollapsed;
      localStorage.setItem("vivucar_admin_sidebar_collapsed", String(isSidebarCollapsed));
      applyAdminSidebarState();
    }
    const menu = event.target.closest(".action-menu");
    document.querySelectorAll(".action-menu[open]").forEach((item) => {
      if (item !== menu) item.removeAttribute("open");
    });
    if (menu && !event.target.closest("summary")) {
      setTimeout(() => menu.removeAttribute("open"), 0);
    }
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
  window.addEventListener("resize", applyAdminSidebarState);
  applyAdminSidebarState();
  document.getElementById("btnConfirmLogout")?.addEventListener("click", () => {
    document.getElementById("btnConfirmLogout").textContent = "Äang Ä‘Äƒng xuáº¥t...";
    setTimeout(Auth.logout, 250);
  });
  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") document.querySelectorAll(".modal-backdrop.show").forEach((modal) => U.closeModal(modal.id));
  });

  window.VivuCarLayout = { renderAdminSidebar, renderAdminHeader, renderOwnerHeader, renderOwnerSidebar, renderUserHeader, renderAppHeader, renderProfileSidebar, renderBreadcrumb, refreshAdminSidebar };
})();


