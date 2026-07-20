(function () {
  const classMap = {
    "app-shell": "min-h-[100dvh] bg-zinc-50 transition-all duration-300 ease-in-out lg:pl-[300px]",
    "owner-grid": "grid min-h-[100dvh] grid-cols-[280px_minmax(0,1fr)] max-lg:grid-cols-1",
    "main-shell": "min-w-0 bg-zinc-50",
    "admin-sidebar": "fixed inset-y-0 left-0 z-40 flex h-[100dvh] w-[300px] flex-col gap-5 overflow-hidden border-r border-zinc-200 bg-white p-4 shadow-sm transition-all duration-300 ease-in-out max-lg:-translate-x-full",
    "owner-sidebar": "max-lg:static max-lg:h-auto max-lg:translate-x-0",
    "sidebar-brand": "flex min-w-0 items-center gap-3 rounded-2xl px-2 py-2 font-extrabold tracking-tight text-zinc-900",
    "brand-mark": "grid h-10 w-10 min-h-[2.5rem] min-w-[2.5rem] shrink-0 aspect-square place-items-center rounded-xl bg-emerald-600 text-sm font-extrabold text-white shadow-sm object-contain [&_svg]:h-4 [&_svg]:w-4 [&_svg]:shrink-0",
    "sidebar-nav": "grid gap-1.5 [&_a]:relative [&_a]:flex [&_a]:items-center [&_a]:gap-3 [&_a]:rounded-xl [&_a]:px-3 [&_a]:py-2.5 [&_a]:text-sm [&_a]:font-medium [&_a]:text-zinc-600 [&_a]:transition-all [&_a]:duration-200 [&_a]:ease-out [&_a:hover]:bg-zinc-100 [&_a:hover]:text-zinc-900 [&_button]:relative [&_button]:flex [&_button]:w-full [&_button]:items-center [&_button]:gap-3 [&_button]:rounded-xl [&_button]:px-3 [&_button]:py-2.5 [&_button]:text-left [&_button]:text-sm [&_button]:font-medium [&_button]:text-zinc-600 [&_button]:transition-all [&_button]:duration-200 [&_button]:ease-out [&_button:hover]:bg-red-50 [&_button:hover]:text-red-600",
    "active": "bg-[#efefed] text-neutral-950",
    "admin-nav-active": "border border-emerald-100 bg-emerald-50 text-emerald-700 shadow-sm animate-admin-nav-in",
    "admin-nav-indicator": "absolute left-0 top-1/2 h-6 w-1 -translate-y-1/2 rounded-r-full bg-emerald-600 transition-all duration-300 ease-out",
    "admin-nav-icon": "flex h-8 w-8 shrink-0 items-center justify-center rounded-lg border border-zinc-200 bg-white text-zinc-500 transition-all duration-200 ease-out group-hover:border-emerald-100 group-hover:text-emerald-600",
    "admin-nav-icon-active": "border-emerald-100 bg-white text-emerald-600 shadow-sm group-hover:text-emerald-600",
    "admin-nav-icon-logout": "group-hover:border-red-100 group-hover:text-red-500",
    "admin-sidebar-title": "grid min-w-0 leading-tight transition-all duration-200 ease-out",
    "admin-nav-label": "min-w-0 flex-1 overflow-hidden whitespace-nowrap transition-all duration-200 ease-out",
    "admin-sidebar-search": "relative transition-all duration-200 ease-out",
    "admin-search-icon": "pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-zinc-400",
    "admin-sidebar-toggle": "ml-auto inline-grid h-8 w-8 shrink-0 place-items-center rounded-lg border border-zinc-200 bg-white text-zinc-600 transition hover:bg-zinc-100 hover:text-zinc-900",
    "admin-user-panel": "mt-auto border-t border-zinc-200 pt-4",
    "admin-header": "sticky top-0 z-30 flex min-h-16 items-center justify-between gap-4 border-b border-zinc-200 bg-white/95 px-6 backdrop-blur-xl max-sm:px-4",
    "user-header": "sticky top-0 z-30 flex min-h-[68px] items-center justify-between gap-4 border-b border-[#e6e4df] bg-[#f7f7f5]/90 px-7 backdrop-blur-xl max-md:flex-wrap max-sm:px-4 max-sm:py-3",
    "user-nav": "flex items-center gap-1 rounded-full border border-[#e6e4df] bg-white p-1 max-md:order-3 max-md:w-full max-md:overflow-x-auto [&_a]:whitespace-nowrap [&_a]:rounded-full [&_a]:px-3 [&_a]:py-2 [&_a]:text-sm [&_a]:font-semibold [&_a]:text-neutral-500 [&_a:hover]:bg-[#efefed]",
    "header-search": "w-[min(460px,44vw)] max-sm:hidden",
    "header-user": "flex items-center gap-2.5 text-sm text-zinc-600",
    "avatar": "grid h-8 w-8 min-h-[2rem] min-w-[2rem] shrink-0 aspect-square place-items-center rounded-full border border-zinc-200 bg-zinc-50 object-cover text-sm font-bold text-zinc-900",
    "page-shell": "mx-auto w-[min(1240px,100%)] p-7 max-sm:p-4",
    "content": "mx-auto w-[min(1360px,100%)] p-7 text-zinc-900 max-sm:p-4 admin-page-enter",
    "owner-content": "p-5 max-sm:p-4",
    "profile-layout": "grid grid-cols-[240px_minmax(0,1fr)] items-start gap-5 max-md:grid-cols-1",
    "profile-sidebar": "sticky top-24 grid gap-1 rounded-2xl border border-[#e6e4df] bg-white p-2 max-md:static max-md:flex max-md:overflow-x-auto [&_a]:rounded-lg [&_a]:px-3 [&_a]:py-2.5 [&_a]:text-sm [&_a]:font-semibold [&_a]:text-neutral-500 [&_button]:rounded-lg [&_button]:px-3 [&_button]:py-2.5 [&_button]:text-left [&_button]:text-sm [&_button]:font-semibold [&_button]:text-neutral-500",
    "page-heading": "mb-6 flex items-start justify-between gap-4 max-sm:grid [&_h1]:m-0 [&_h1]:text-3xl [&_h1]:font-bold [&_h1]:tracking-tight [&_h1]:text-zinc-900 [&_p]:mt-1.5 [&_p]:max-w-[72ch] [&_p]:text-sm [&_p]:leading-6 [&_p]:text-zinc-600",
    "breadcrumb": "mb-2 text-sm font-medium text-zinc-500",
    "toolbar": "mb-5 grid gap-3 rounded-2xl border border-zinc-200 bg-white p-4 shadow-sm max-lg:grid-cols-2 max-sm:grid-cols-1",
    "toolbar-admin-users": "grid-cols-[minmax(220px,2fr)_150px_160px_160px_auto_auto]",
    "toolbar-cars": "grid-cols-[minmax(220px,2fr)_repeat(4,minmax(128px,1fr))_auto_auto]",
    "toolbar-dashboard": "grid-cols-[repeat(4,minmax(150px,1fr))_auto]",
    "toolbar-export": "grid-cols-[repeat(4,minmax(150px,1fr))_auto]",
    "toolbar-vouchers": "grid-cols-[minmax(220px,2fr)_150px_150px_150px_150px_auto_auto]",
    "toolbar-trash": "grid-cols-[minmax(220px,1fr)_190px_auto_auto]",
    "menu-toggle": "hidden max-lg:inline-grid",
    "btn": "inline-flex min-h-10 items-center justify-center gap-2 rounded-xl border border-transparent px-4 text-sm font-semibold transition duration-200 active:translate-y-px disabled:cursor-not-allowed disabled:opacity-60",
    "btn-primary": "bg-emerald-600 text-white shadow-sm hover:bg-emerald-700 focus:outline-none focus:ring-4 focus:ring-emerald-100",
    "btn-secondary": "border-zinc-200 bg-white text-zinc-700 shadow-sm hover:bg-zinc-50 focus:outline-none focus:ring-4 focus:ring-emerald-100",
    "btn-danger": "border-red-600 bg-red-600 text-white shadow-sm hover:bg-red-700 focus:outline-none focus:ring-4 focus:ring-red-100",
    "btn-ghost": "bg-transparent text-zinc-600 hover:bg-zinc-100 hover:text-zinc-900",
    "btn-sm": "min-h-8 rounded-lg px-3 text-xs",
    "btn-full": "w-full",
    "icon-button": "inline-grid h-9 w-9 place-items-center rounded-xl border border-zinc-200 bg-white text-zinc-700 transition hover:bg-zinc-50 focus:outline-none focus:ring-4 focus:ring-emerald-100",
    "field": "mb-3 grid gap-2 [&_label]:text-sm [&_label]:font-medium [&_label]:text-zinc-700 [&_small]:text-sm [&_small]:text-zinc-500",
    "check-row": "flex items-center gap-2 text-sm font-medium text-neutral-500 [&_input]:w-auto",
    "switch-row": "flex items-center gap-2 text-sm font-medium text-neutral-500 [&_input]:w-auto",
    "alert": "my-2 rounded-xl border border-zinc-200 bg-zinc-50 px-3 py-2 text-sm text-zinc-600",
    "alert-error": "border-red-200 bg-red-50 text-red-700",
    "panel": "overflow-visible rounded-2xl border border-zinc-200 bg-white shadow-sm",
    "card": "overflow-hidden rounded-2xl border border-zinc-200 bg-white shadow-sm",
    "panel-head": "flex items-center justify-between gap-3 border-b border-zinc-200 px-5 py-4 [&_h2]:m-0 [&_h2]:text-lg [&_h2]:font-semibold [&_h2]:tracking-tight [&_h2]:text-zinc-900",
    "table-wrap": "overflow-x-auto overflow-y-visible rounded-2xl border border-zinc-200 bg-white shadow-sm [&_table]:min-w-[860px] [&_table]:w-full [&_table]:border-collapse [&_thead]:bg-zinc-50 [&_tbody]:divide-y [&_tbody]:divide-zinc-100 [&_tr]:transition [&_tbody_tr:hover]:bg-zinc-50 [&_th]:border-b [&_th]:border-zinc-200 [&_th]:px-5 [&_th]:py-3.5 [&_th]:text-left [&_th]:text-xs [&_th]:font-semibold [&_th]:uppercase [&_th]:tracking-wide [&_th]:text-zinc-500 [&_td]:whitespace-nowrap [&_td]:px-5 [&_td]:py-4 [&_td]:text-sm [&_td]:text-zinc-700",
    "summary-grid": "mb-5 grid grid-cols-5 gap-3 max-xl:grid-cols-3 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "kpi-grid": "mb-5 grid grid-cols-4 gap-3 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "summary-card": "rounded-2xl border border-zinc-200 bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md [&_span]:mb-2 [&_span]:block [&_span]:text-sm [&_span]:font-medium [&_span]:text-zinc-500 [&_strong]:font-mono [&_strong]:text-2xl [&_strong]:text-zinc-900",
    "kpi-card": "rounded-2xl border border-zinc-200 bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md [&_span]:mb-2 [&_span]:block [&_span]:text-sm [&_span]:font-medium [&_span]:text-zinc-500 [&_strong]:font-mono [&_strong]:text-2xl [&_strong]:text-zinc-900",
    "badge": "inline-flex rounded-full border px-2.5 py-1 text-xs font-semibold",
    "badge-success": "border-emerald-100 bg-emerald-50 text-emerald-700",
    "badge-warning": "border-amber-100 bg-amber-50 text-amber-700",
    "badge-danger": "border-red-100 bg-red-50 text-red-700",
    "badge-neutral": "border-zinc-200 bg-zinc-100 text-zinc-600",
    "badge-info": "border-sky-100 bg-sky-50 text-sky-700",
    "actions": "relative flex flex-wrap gap-2",
    "action-menu": "relative inline-block text-left [&_summary]:list-none [&_summary]:inline-grid [&_summary]:h-9 [&_summary]:w-9 [&_summary]:cursor-pointer [&_summary]:place-items-center [&_summary]:rounded-lg [&_summary]:border [&_summary]:border-zinc-200 [&_summary]:bg-white [&_summary]:text-zinc-700 [&_summary]:transition [&_summary:hover]:bg-zinc-100",
    "action-menu-panel": "absolute right-0 z-50 mt-2 grid w-48 gap-1 rounded-xl border border-zinc-200 bg-white p-1.5 shadow-lg",
    "action-menu-item": "rounded-lg px-3 py-2 text-left text-sm text-zinc-700 transition hover:bg-zinc-100 disabled:pointer-events-none disabled:opacity-50",
    "action-menu-danger": "rounded-lg px-3 py-2 text-left text-sm text-red-600 transition hover:bg-red-50 disabled:pointer-events-none disabled:opacity-50",
    "page-buttons": "flex flex-wrap gap-1.5",
    "pagination": "flex items-center justify-end gap-2 px-5 py-4 text-sm text-zinc-500 max-sm:flex-wrap",
    "modal-backdrop": "fixed inset-0 z-50 hidden items-center justify-center bg-neutral-950/35 p-5",
    "modal": "w-[min(520px,100%)] rounded-2xl border border-zinc-200 bg-white p-5 shadow-2xl [&_label]:mb-3 [&_label]:grid [&_label]:gap-2 [&_label]:text-sm [&_label]:font-medium [&_label]:text-zinc-700",
    "modal-header": "mb-3 flex items-center justify-between gap-3 [&_h2]:m-0 [&_h2]:text-lg [&_h2]:font-bold",
    "modal-actions": "mt-4 flex justify-end gap-2 max-sm:flex-wrap",
    "drawer": "fixed inset-y-0 right-0 z-50 w-[min(560px,100%)] translate-x-full overflow-y-auto border-l border-zinc-200 bg-white p-5 shadow-2xl transition-transform duration-200",
    "drawer-head": "mb-5 flex items-center justify-between gap-3 border-b border-zinc-200 pb-4 [&_h2]:m-0 [&_h2]:text-lg [&_h2]:font-semibold [&_h2]:text-zinc-900",
    "drawer-overlay": "fixed inset-0 z-40 hidden bg-neutral-950/30",
    "toast-root": "fixed bottom-5 right-5 z-[70] grid gap-2",
    "toast": "animate-toast-in rounded-xl bg-zinc-900 px-3 py-2 text-sm text-white shadow-xl",
    "empty-state": "px-5 py-14 text-center text-zinc-500 [&_h3]:mb-1.5 [&_h3]:text-lg [&_h3]:font-semibold [&_h3]:text-zinc-900 [&_p]:mx-auto [&_p]:max-w-md",
    "skeleton": "animate-shimmer min-h-32 rounded-2xl bg-[linear-gradient(90deg,#f0f0ee,#fbfbfa,#f0f0ee)] bg-[length:220%_100%]",
    "login-page": "grid min-h-[100dvh] place-items-center bg-zinc-50 p-6",
    "login-card": "w-[min(460px,100%)] rounded-2xl border border-zinc-200 bg-white p-8 shadow-sm",
    "login-brand": "mb-6 [&_h1]:mt-3 [&_h1]:mb-1.5 [&_h1]:text-3xl [&_h1]:font-bold [&_h1]:tracking-tight [&_h1]:text-zinc-900 [&_p]:text-zinc-600",
    "demo-accounts": "my-4 grid grid-cols-3 gap-2 [&_button]:min-h-9 [&_button]:rounded-xl [&_button]:border [&_button]:border-zinc-200 [&_button]:bg-white [&_button]:text-xs [&_button]:font-bold [&_button]:text-zinc-500 [&_button]:transition [&_button:hover]:bg-zinc-50 [&_button:hover]:text-zinc-900",
    "form-stack": "grid gap-4",
    "form-card": "rounded-2xl border border-zinc-200 bg-white p-5 shadow-sm [&_h2]:mb-4 [&_h2]:text-lg [&_h2]:font-semibold [&_h2]:tracking-tight [&_h2]:text-zinc-900",
    "form-grid": "grid grid-cols-2 gap-x-4 gap-y-3 max-sm:grid-cols-1",
    "full": "col-span-full",
    "sticky-actions": "sticky bottom-0 flex justify-end gap-2 rounded-2xl border border-zinc-200 bg-white/95 p-3 shadow-sm backdrop-blur-xl",
    "upload-zone": "grid gap-2 rounded-xl border border-dashed border-zinc-300 bg-zinc-50 p-4",
    "image-preview-grid": "mt-3 grid grid-cols-[repeat(auto-fill,minmax(128px,1fr))] gap-2.5 [&_img]:aspect-[4/3] [&_img]:w-full [&_img]:rounded-lg [&_img]:border [&_img]:border-[#e6e4df] [&_img]:object-cover",
    "car-thumb": "h-14 w-20 rounded-xl border border-zinc-200 object-cover",
    "voucher-preview": "sticky top-24 rounded-2xl border border-zinc-200 bg-white p-6 shadow-sm max-lg:static [&_.code]:mb-3 [&_.code]:font-mono [&_.code]:text-2xl [&_.code]:font-bold [&_.code]:text-zinc-900 [&_.discount]:my-5 [&_.discount]:text-4xl [&_.discount]:font-extrabold [&_.discount]:tracking-tight [&_.discount]:text-emerald-700 [&_p]:mb-2 [&_p]:text-sm [&_p]:text-zinc-600",
    "report-types": "mb-4 grid grid-cols-4 gap-2.5 max-sm:grid-cols-1",
    "radio-card": "flex items-center gap-2 rounded-2xl border border-zinc-200 bg-white p-4 shadow-sm transition hover:border-emerald-200 hover:bg-emerald-50/40 [&_input]:w-auto",
    "two-column": "grid grid-cols-[minmax(0,1.45fr)_minmax(320px,.8fr)] items-start gap-4 max-lg:grid-cols-1",
    "home-hero-pro": "grid grid-cols-[minmax(0,1.2fr)_minmax(360px,.72fr)] items-stretch gap-5 max-lg:grid-cols-1",
    "home-hero-copy": "flex min-h-[520px] flex-col justify-between overflow-hidden rounded-[28px] border border-[#e6e4df] bg-cover bg-center p-[clamp(26px,5vw,58px)] max-sm:min-h-[460px]",
    "home-search-card": "self-end rounded-[28px] border border-[#e6e4df] bg-white p-6 shadow-xl [&_h2]:mb-4 [&_h2]:text-2xl [&_h2]:font-bold",
    "home-search-grid": "grid grid-cols-2 gap-3 max-sm:grid-cols-1",
    "home-strip": "my-5 grid grid-cols-[280px_minmax(0,1fr)] items-center gap-4 rounded-[22px] border border-[#e6e4df] bg-white p-5 max-lg:grid-cols-1",
    "eyebrow": "inline-block text-xs font-extrabold uppercase tracking-[.08em] text-neutral-500",
    "car-grid": "grid grid-cols-4 gap-4 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "car-card": "group overflow-hidden rounded-2xl border border-[#e6e4df] bg-white shadow-sm transition duration-200 hover:-translate-y-0.5 hover:shadow-xl [&_img]:aspect-[16/10] [&_img]:w-full [&_img]:object-cover",
    "car-card-body": "grid gap-2.5 p-4 [&_h3]:m-0 [&_h3]:text-lg [&_h3]:font-bold",
    "car-meta": "flex flex-wrap gap-2 text-sm text-neutral-500",
    "price-line": "flex items-baseline justify-between gap-2 font-extrabold [&_strong]:font-mono",
    "stars": "whitespace-nowrap tracking-wider text-stone-300",
    "is-filled": "text-amber-700",
    "search-box": "mt-5 grid grid-cols-[1.5fr_1fr_1fr_1fr_auto] gap-2.5 rounded-2xl border border-[#e6e4df] bg-white p-3 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "section-head": "my-4 flex items-end justify-between gap-4 [&_h2]:m-0 [&_h2]:text-2xl [&_h2]:font-bold",
    "tabs": "mt-5 flex gap-1.5 border-b border-[#e6e4df] [&_button]:px-3.5 [&_button]:py-3 [&_button]:font-bold [&_button]:text-neutral-500",
    "home-stat-row": "grid max-w-[680px] grid-cols-3 gap-2.5 max-sm:grid-cols-1",
    "home-stat": "rounded-[18px] border border-[#e6e4df]/80 bg-white/80 p-3.5 backdrop-blur [&_strong]:block [&_strong]:font-mono [&_strong]:text-2xl [&_span]:text-sm [&_span]:text-neutral-500",
    "home-section": "mt-8",
    "home-browse-band": "grid grid-cols-[minmax(260px,.7fr)_minmax(0,1.3fr)] gap-4 rounded-[28px] bg-neutral-900 p-7 text-white max-lg:grid-cols-1",
    "home-type-grid": "grid grid-cols-4 gap-2.5 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "type-card": "flex min-h-[150px] flex-col justify-between rounded-[18px] border border-white/15 bg-white/10 p-4 text-white",
    "steps-grid": "grid grid-cols-3 gap-3 max-sm:grid-cols-1",
    "step-card": "rounded-[20px] border border-[#e6e4df] bg-white p-5",
    "flow-steps": "my-5 grid grid-cols-4 gap-2 max-sm:grid-cols-1",
    "flow-step": "rounded-full border border-[#e6e4df] bg-white px-3 py-2.5 text-center text-sm font-bold text-neutral-500",
    "checkout-car": "grid grid-cols-[168px_minmax(0,1fr)] items-center gap-4 max-sm:grid-cols-1 [&_img]:aspect-[16/10] [&_img]:w-[168px] [&_img]:rounded-2xl [&_img]:border [&_img]:border-[#e6e4df] [&_img]:object-cover max-sm:[&_img]:w-full",
    "payment-method-grid": "grid grid-cols-3 gap-2.5 max-sm:grid-cols-1",
    "method-card": "grid cursor-pointer gap-2 rounded-[18px] border border-[#e6e4df] bg-white p-4 transition hover:-translate-y-0.5 hover:border-[#d8d6d0] [&_input]:w-auto",
    "result-card": "mx-auto my-9 max-w-[760px] rounded-[26px] border border-[#e6e4df] bg-white p-[clamp(24px,5vw,44px)] text-center shadow-xl",
    "result-mark": "mx-auto mb-5 grid h-[72px] w-[72px] place-items-center rounded-full text-3xl font-black",
    "booking-list": "grid gap-3",
    "booking-actions": "flex flex-wrap justify-end gap-2 max-sm:justify-start",
    "detail-layout": "grid grid-cols-[minmax(0,1.35fr)_360px] items-start gap-5 max-lg:grid-cols-1",
    "gallery-main": "aspect-[16/10] w-full rounded-[20px] border border-[#e6e4df] object-cover",
    "thumb-row": "mt-2.5 flex gap-2.5 overflow-x-auto [&_img]:h-16 [&_img]:w-24 [&_img]:cursor-pointer [&_img]:rounded-lg [&_img]:border [&_img]:border-[#e6e4df] [&_img]:object-cover",
    "booking-card": "sticky top-24 p-5 max-lg:static",
    "profile-hero": "grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-4 p-5 max-sm:grid-cols-1",
    "profile-avatar": "grid h-[86px] w-[86px] place-items-center rounded-full border border-[#e6e4df] bg-[#fbfbfa] object-cover text-3xl font-extrabold",
    "stats-grid": "my-4 grid grid-cols-4 gap-3 max-sm:grid-cols-1",
    "rating-picker": "flex gap-2 text-4xl text-stone-300",
    "review-card": "mb-3 grid grid-cols-[120px_minmax(0,1fr)] gap-4 rounded-2xl border border-[#e6e4df] bg-white p-4 max-sm:grid-cols-1 [&_img]:h-[82px] [&_img]:w-[120px] [&_img]:rounded-lg [&_img]:object-cover",
    "detail-document": "grid grid-cols-[minmax(0,1fr)_330px] items-start gap-5 max-lg:grid-cols-1",
    "info-grid": "grid grid-cols-2 gap-2.5 max-sm:grid-cols-1",
    "info-cell": "border-t border-[#e6e4df] pt-2.5 [&_span]:mb-1 [&_span]:block [&_span]:text-xs [&_span]:text-neutral-500 [&_strong]:text-sm",
    "timeline": "grid",
    "timeline-item": "relative border-l border-[#e6e4df] pb-5 pl-6 before:absolute before:-left-[5px] before:top-0.5 before:h-2.5 before:w-2.5 before:rounded-full before:bg-blue-700 before:ring-4 before:ring-white [&_p]:mt-1 [&_p]:text-neutral-500",
    "contract-toolbar": "sticky top-[72px] z-10 mb-4 flex justify-between gap-2.5 rounded-[18px] border border-[#e6e4df] bg-[#f7f7f5]/90 p-3 backdrop-blur-xl",
    "contract-scroll": "overflow-x-auto pb-5",
    "contract-page": "mx-auto min-h-[1123px] w-[794px] border border-[#e6e4df] bg-white p-[58px] text-sm leading-relaxed text-neutral-900 shadow-xl",
    "signature-grid": "mt-12 grid grid-cols-2 gap-20 text-center",
    "progress": "mt-2 h-2 overflow-hidden rounded-full bg-zinc-100 [&_span]:block [&_span]:h-full [&_span]:rounded-full [&_span]:bg-emerald-600",
    "chart-frame": "min-h-80 p-5 [&_canvas]:!h-[300px] [&_canvas]:!w-full",
    "bar-chart": "flex h-80 items-end gap-2 p-5",
    "bar-item": "relative grid h-full min-w-[34px] flex-1 grid-rows-[1fr_auto] gap-2 text-center text-xs text-neutral-500",
    "bar": "self-end rounded-t-lg bg-emerald-600",
    "mini-chart": "flex h-32 items-end gap-2 py-3 [&_span]:min-h-2 [&_span]:flex-1 [&_span]:rounded-t-md [&_span]:bg-emerald-600",
    "owner-form-layout": "grid grid-cols-[minmax(0,1fr)_340px] items-start gap-4 max-lg:grid-cols-1",
    "owner-image-grid": "grid grid-cols-[repeat(auto-fill,minmax(180px,1fr))] gap-3",
    "owner-image-card": "grid gap-2 rounded-[18px] border border-[#e6e4df] bg-white p-2.5 [&_img]:aspect-[4/3] [&_img]:w-full [&_img]:rounded-xl [&_img]:object-cover",
    "owner-chart": "flex h-60 items-end gap-2.5 rounded-[18px] border border-[#e6e4df] bg-white p-5",
    "owner-chart-bar": "grid h-full flex-1 grid-rows-[1fr_auto] gap-2 text-center text-xs text-neutral-500 [&_span:first-child]:self-end [&_span:first-child]:rounded-t-lg [&_span:first-child]:bg-blue-600",
    "return-note": "rounded-2xl border border-blue-200 bg-blue-50 p-4 text-blue-900",
    "support-layout": "grid min-h-[680px] grid-cols-[280px_minmax(0,1fr)_300px] gap-3 max-lg:grid-cols-1",
    "conversation-list": "grid grid-rows-[auto_1fr] overflow-hidden rounded-[20px] border border-[#e6e4df] bg-white",
    "chat-panel": "grid grid-rows-[auto_1fr_auto] overflow-hidden rounded-[20px] border border-[#e6e4df] bg-white",
    "context-panel": "overflow-hidden rounded-[20px] border border-[#e6e4df] bg-white p-4",
    "conversation-item": "grid w-full gap-1.5 border-b border-[#e6e4df] bg-transparent p-4 text-left hover:bg-[#fbfbfa]",
    "chat-header": "p-4",
    "chat-composer": "p-4",
    "chat-messages": "grid content-start gap-2.5 overflow-y-auto bg-[#fbfbfa] p-4",
    "message": "max-w-[78%] rounded-2xl border border-[#e6e4df] bg-white px-3 py-2 text-sm",
    "me": "ml-auto border-blue-700 bg-blue-700 text-white",
    "system": "mx-auto max-w-[90%] bg-orange-50 text-orange-800",
    "chatbot-button": "fixed bottom-5 right-5 z-20 rounded-full shadow-xl",
    "chatbot-window": "fixed bottom-[78px] right-5 z-20 hidden h-[540px] w-[min(380px,calc(100vw-24px))] grid-rows-[auto_1fr_auto] overflow-hidden rounded-[22px] border border-[#e6e4df] bg-white shadow-xl max-sm:inset-x-2 max-sm:bottom-2 max-sm:h-[78dvh] max-sm:w-auto",
    "chatbot-messages": "grid content-start gap-2.5 overflow-y-auto bg-[#fbfbfa] p-4",
    "listing-layout": "grid grid-cols-[280px_minmax(0,1fr)] items-start gap-5 max-lg:grid-cols-1",
    "filter-panel": "sticky top-24 rounded-2xl border border-[#e6e4df] bg-white p-4 max-lg:static",
    "filter-group": "mt-4 grid gap-2 border-t border-[#e6e4df] pt-4 [&_h3]:m-0 [&_h3]:text-sm [&_h3]:font-bold",
    "chip-row": "flex flex-wrap gap-2",
    "chip": "rounded-full border border-[#e6e4df] bg-[#fbfbfa] px-3 py-1.5 text-sm transition hover:bg-[#efefed]",
    "detail-layout": "grid grid-cols-[minmax(0,1.35fr)_360px] items-start gap-5 max-lg:grid-cols-1",
    "checkout-layout": "grid grid-cols-[minmax(0,1fr)_370px] items-start gap-5 max-lg:grid-cols-1",
    "checkout-main": "grid gap-4",
    "checkout-section": "rounded-[20px] border border-[#e6e4df] bg-white p-5 shadow-sm [&_h2]:mb-4 [&_h2]:text-xl [&_h2]:font-bold",
    "booking-card-wide": "grid grid-cols-[132px_minmax(0,1fr)_auto] items-center gap-4 rounded-[20px] border border-[#e6e4df] bg-white p-4 max-sm:grid-cols-1 [&_img]:aspect-[16/10] [&_img]:w-[132px] [&_img]:rounded-xl [&_img]:border [&_img]:border-[#e6e4df] [&_img]:object-cover max-sm:[&_img]:w-full",
    "owner-hero": "mb-4 flex items-end justify-between gap-4 rounded-3xl border border-[#e6e4df] bg-white p-6 max-sm:grid",
    "owner-kpi-grid": "my-4 grid grid-cols-5 gap-2.5 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "owner-kpi": "rounded-[18px] border border-[#e6e4df] bg-white p-4 [&_strong]:mt-2 [&_strong]:block [&_strong]:font-mono [&_strong]:text-2xl",
    "owner-table-wrap": "overflow-x-auto rounded-[20px] border border-[#e6e4df] bg-white",
    "owner-table": "min-w-[820px] w-full border-collapse [&_td]:border-b [&_td]:border-[#e6e4df] [&_td]:p-3 [&_td]:align-top [&_th]:border-b [&_th]:border-[#e6e4df] [&_th]:p-3 [&_th]:text-left [&_th]:text-xs [&_th]:font-bold [&_th]:uppercase [&_th]:tracking-wide [&_th]:text-neutral-500",
    "muted": "text-neutral-500",
    "mono": "font-mono"
  };

  const formControlClass = "w-full rounded-xl border border-zinc-200 bg-white px-3.5 py-2.5 text-sm text-zinc-900 placeholder:text-zinc-400 outline-none transition focus:border-emerald-500 focus:ring-4 focus:ring-emerald-100 disabled:bg-zinc-100 disabled:text-zinc-500";

  function applyTailwind(root) {
    const scope = root?.nodeType === 1 ? root : document;
    const elements = scope.querySelectorAll ? [scope, ...scope.querySelectorAll("*")] : [];
    elements.forEach((element) => {
      if (!element.classList) return;
      Array.from(element.classList).forEach((token) => {
        const utilities = classMap[token];
        if (utilities) element.classList.add(...utilities.split(/\s+/).filter(Boolean));
      });
      if (["INPUT", "SELECT", "TEXTAREA"].includes(element.tagName)) {
        element.classList.add(...formControlClass.split(/\s+/));
      }
      if (element.tagName === "BUTTON") element.classList.add("cursor-pointer");
    });
  }

  function openModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;
    applyTailwind(modal);
    modal.classList.remove("hidden");
    modal.classList.add("flex", "show");
  }

  function closeModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;
    modal.classList.add("hidden");
    modal.classList.remove("flex", "show");
  }

  function openDrawer(drawer, overlay) {
    drawer?.classList.remove("translate-x-full");
    drawer?.classList.add("translate-x-0", "show");
    overlay?.classList.remove("hidden");
    overlay?.classList.add("block", "show");
  }

  function closeDrawer(drawer, overlay) {
    drawer?.classList.add("translate-x-full");
    drawer?.classList.remove("translate-x-0", "show");
    overlay?.classList.add("hidden");
    overlay?.classList.remove("block", "show");
  }

  document.addEventListener("DOMContentLoaded", () => {
    document.body.classList.add("min-h-[100dvh]", "bg-zinc-50", "text-zinc-900", "font-sans", "antialiased", "overflow-x-hidden");
    applyTailwind(document);
    new MutationObserver((mutations) => {
      mutations.forEach((mutation) => mutation.addedNodes.forEach((node) => applyTailwind(node)));
    }).observe(document.body, { childList: true, subtree: true });
  });

  window.VivuCarTailwindUI = { applyTailwind, openModal, closeModal, openDrawer, closeDrawer };
})();

