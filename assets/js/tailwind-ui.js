(function () {
  const classMap = {
    "app-shell": "min-h-[100dvh] grid grid-cols-[264px_minmax(0,1fr)] max-lg:grid-cols-1",
    "owner-grid": "grid min-h-[100dvh] grid-cols-[280px_minmax(0,1fr)] max-lg:grid-cols-1",
    "main-shell": "min-w-0",
    "admin-sidebar": "sticky top-0 z-40 flex h-[100dvh] flex-col gap-4 border-r border-[#e6e4df] bg-[#fbfbfa] p-3 max-lg:fixed max-lg:inset-y-0 max-lg:left-0 max-lg:w-[264px] max-lg:-translate-x-full max-lg:transition-transform",
    "owner-sidebar": "max-lg:static max-lg:h-auto max-lg:translate-x-0",
    "sidebar-brand": "flex items-center gap-2.5 px-2.5 py-2 font-extrabold text-neutral-900",
    "brand-mark": "grid h-8 w-8 place-items-center rounded-[9px] bg-neutral-900 text-sm font-extrabold text-white",
    "sidebar-nav": "grid gap-1 [&_a]:rounded-lg [&_a]:px-3 [&_a]:py-2 [&_a]:text-sm [&_a]:font-semibold [&_a]:text-neutral-600 [&_a:hover]:bg-[#efefed] [&_button]:rounded-lg [&_button]:px-3 [&_button]:py-2 [&_button]:text-left [&_button]:text-sm [&_button]:font-semibold [&_button]:text-neutral-600 [&_button:hover]:bg-[#efefed]",
    "active": "bg-[#efefed] text-neutral-950",
    "admin-header": "sticky top-0 z-30 flex h-16 items-center justify-between gap-4 border-b border-[#e6e4df] bg-[#f7f7f5]/90 px-6 backdrop-blur-xl max-sm:px-4",
    "user-header": "sticky top-0 z-30 flex min-h-[68px] items-center justify-between gap-4 border-b border-[#e6e4df] bg-[#f7f7f5]/90 px-7 backdrop-blur-xl max-md:flex-wrap max-sm:px-4 max-sm:py-3",
    "user-nav": "flex items-center gap-1 rounded-full border border-[#e6e4df] bg-white p-1 max-md:order-3 max-md:w-full max-md:overflow-x-auto [&_a]:whitespace-nowrap [&_a]:rounded-full [&_a]:px-3 [&_a]:py-2 [&_a]:text-sm [&_a]:font-semibold [&_a]:text-neutral-500 [&_a:hover]:bg-[#efefed]",
    "header-search": "w-[min(420px,44vw)] max-sm:hidden",
    "header-user": "flex items-center gap-2.5 text-sm text-neutral-500",
    "avatar": "grid h-8 w-8 place-items-center rounded-full border border-[#e6e4df] bg-white object-cover text-sm font-bold text-neutral-900",
    "page-shell": "mx-auto w-[min(1240px,100%)] p-7 max-sm:p-4",
    "content": "mx-auto w-[min(1360px,100%)] p-7 max-sm:p-4",
    "owner-content": "p-5 max-sm:p-4",
    "profile-layout": "grid grid-cols-[240px_minmax(0,1fr)] items-start gap-5 max-md:grid-cols-1",
    "profile-sidebar": "sticky top-24 grid gap-1 rounded-2xl border border-[#e6e4df] bg-white p-2 max-md:static max-md:flex max-md:overflow-x-auto [&_a]:rounded-lg [&_a]:px-3 [&_a]:py-2.5 [&_a]:text-sm [&_a]:font-semibold [&_a]:text-neutral-500 [&_button]:rounded-lg [&_button]:px-3 [&_button]:py-2.5 [&_button]:text-left [&_button]:text-sm [&_button]:font-semibold [&_button]:text-neutral-500",
    "page-heading": "mb-5 flex items-start justify-between gap-4 max-sm:grid [&_h1]:m-0 [&_h1]:text-3xl [&_h1]:font-bold [&_p]:mt-1.5 [&_p]:max-w-[68ch] [&_p]:text-neutral-500",
    "breadcrumb": "mb-2 text-sm text-neutral-500",
    "toolbar": "mb-4 grid gap-3 rounded-2xl border border-[#e6e4df] bg-white p-3 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "toolbar-admin-users": "grid-cols-[minmax(220px,2fr)_160px_160px_auto_auto]",
    "toolbar-cars": "grid-cols-[minmax(220px,2fr)_repeat(4,minmax(128px,1fr))_auto_auto]",
    "toolbar-dashboard": "grid-cols-[repeat(4,minmax(150px,1fr))_auto]",
    "toolbar-export": "grid-cols-[repeat(4,minmax(150px,1fr))_auto]",
    "toolbar-vouchers": "grid-cols-[minmax(220px,2fr)_150px_150px_150px_150px_auto_auto]",
    "menu-toggle": "hidden max-lg:inline-grid",
    "btn": "inline-flex min-h-9 items-center justify-center gap-2 rounded-lg border border-transparent px-3 text-sm font-semibold transition duration-200 active:translate-y-px disabled:cursor-not-allowed disabled:opacity-60",
    "btn-primary": "bg-neutral-900 text-white hover:bg-neutral-700",
    "btn-secondary": "border-[#e6e4df] bg-white text-neutral-900 hover:bg-[#f1f1ef]",
    "btn-danger": "border-red-200 bg-red-50 text-red-700 hover:bg-red-100",
    "btn-ghost": "bg-transparent text-neutral-500 hover:bg-[#efefed] hover:text-neutral-950",
    "btn-sm": "min-h-8 px-2.5 text-xs",
    "btn-full": "w-full",
    "icon-button": "inline-grid h-9 w-9 place-items-center rounded-lg border border-[#e6e4df] bg-white text-neutral-700 transition hover:bg-[#f1f1ef]",
    "field": "mb-3 grid gap-2 [&_label]:text-sm [&_label]:font-bold [&_small]:text-neutral-500",
    "check-row": "flex items-center gap-2 text-sm font-medium text-neutral-500 [&_input]:w-auto",
    "switch-row": "flex items-center gap-2 text-sm font-medium text-neutral-500 [&_input]:w-auto",
    "alert": "my-2 rounded-lg border border-[#e6e4df] bg-[#fbfbfa] px-3 py-2 text-sm text-neutral-500",
    "alert-error": "border-red-200 bg-red-50 text-red-700",
    "panel": "overflow-hidden rounded-2xl border border-[#e6e4df] bg-white shadow-sm",
    "card": "overflow-hidden rounded-2xl border border-[#e6e4df] bg-white shadow-sm",
    "panel-head": "flex items-center justify-between gap-3 border-b border-[#e6e4df] px-5 py-4 [&_h2]:m-0 [&_h2]:text-lg [&_h2]:font-bold",
    "table-wrap": "overflow-x-auto [&_table]:w-full [&_table]:border-collapse [&_th]:border-b [&_th]:border-[#e6e4df] [&_th]:bg-[#fbfbfa] [&_th]:p-3 [&_th]:text-left [&_th]:text-xs [&_th]:font-bold [&_th]:uppercase [&_th]:tracking-wide [&_th]:text-neutral-500 [&_td]:whitespace-nowrap [&_td]:border-b [&_td]:border-[#e6e4df] [&_td]:p-3 [&_td]:text-sm",
    "summary-grid": "mb-4 grid grid-cols-5 gap-3 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "kpi-grid": "mb-4 grid grid-cols-4 gap-3 max-lg:grid-cols-2 max-sm:grid-cols-1",
    "summary-card": "rounded-2xl border border-[#e6e4df] bg-white p-4 shadow-sm [&_span]:mb-2 [&_span]:block [&_span]:text-sm [&_span]:text-neutral-500 [&_strong]:font-mono [&_strong]:text-2xl",
    "kpi-card": "rounded-2xl border border-[#e6e4df] bg-white p-4 [&_span]:mb-2 [&_span]:block [&_span]:text-sm [&_span]:text-neutral-500 [&_strong]:font-mono [&_strong]:text-2xl",
    "badge": "inline-flex rounded-full border px-2.5 py-1 text-xs font-bold",
    "badge-success": "border-green-200 bg-green-50 text-green-700",
    "badge-warning": "border-amber-200 bg-amber-50 text-amber-700",
    "badge-danger": "border-red-200 bg-red-50 text-red-700",
    "badge-neutral": "border-[#e6e4df] bg-[#f0f0ee] text-neutral-600",
    "badge-info": "border-blue-200 bg-blue-50 text-blue-700",
    "actions": "flex flex-wrap gap-2",
    "pagination": "flex items-center justify-end gap-2 px-4 py-3 text-sm text-neutral-500 max-sm:flex-wrap",
    "modal-backdrop": "fixed inset-0 z-50 hidden items-center justify-center bg-neutral-950/35 p-5",
    "modal": "w-[min(520px,100%)] rounded-2xl border border-[#e6e4df] bg-white p-5 shadow-2xl",
    "modal-header": "mb-3 flex items-center justify-between gap-3 [&_h2]:m-0 [&_h2]:text-lg [&_h2]:font-bold",
    "modal-actions": "mt-4 flex justify-end gap-2 max-sm:flex-wrap",
    "drawer": "fixed inset-y-0 right-0 z-50 w-[min(540px,100%)] translate-x-full overflow-y-auto border-l border-[#e6e4df] bg-white p-5 shadow-2xl transition-transform duration-200",
    "drawer-overlay": "fixed inset-0 z-40 hidden bg-neutral-950/30",
    "toast-root": "fixed bottom-5 right-5 z-[70] grid gap-2",
    "toast": "animate-toast-in rounded-lg bg-neutral-900 px-3 py-2 text-sm text-white shadow-xl",
    "empty-state": "px-5 py-12 text-center text-neutral-500 [&_h3]:mb-1.5 [&_h3]:text-lg [&_h3]:font-bold [&_h3]:text-neutral-950",
    "skeleton": "animate-shimmer min-h-32 rounded-2xl bg-[linear-gradient(90deg,#f0f0ee,#fbfbfa,#f0f0ee)] bg-[length:220%_100%]",
    "login-page": "grid min-h-[100dvh] place-items-center bg-[#f7f7f5] p-6",
    "login-card": "w-[min(440px,100%)] rounded-[18px] border border-[#e6e4df] bg-white/90 p-7 shadow-xl backdrop-blur",
    "login-brand": "mb-5 [&_h1]:mt-3 [&_h1]:mb-1.5 [&_h1]:text-3xl [&_h1]:font-bold [&_p]:text-neutral-500",
    "demo-accounts": "my-4 grid grid-cols-3 gap-2 [&_button]:min-h-8 [&_button]:rounded-full [&_button]:border [&_button]:border-[#e6e4df] [&_button]:bg-[#fbfbfa] [&_button]:text-xs [&_button]:font-bold [&_button]:text-neutral-500",
    "form-stack": "grid gap-4",
    "form-card": "rounded-2xl border border-[#e6e4df] bg-white p-5 [&_h2]:mb-4 [&_h2]:text-lg [&_h2]:font-bold",
    "form-grid": "grid grid-cols-2 gap-x-4 gap-y-3 max-sm:grid-cols-1",
    "full": "col-span-full",
    "sticky-actions": "sticky bottom-0 flex justify-end gap-2 rounded-2xl border border-[#e6e4df] bg-[#f7f7f5]/90 p-3 backdrop-blur-xl",
    "upload-zone": "grid gap-2 rounded-xl border border-dashed border-[#d8d6d0] bg-[#fbfbfa] p-4",
    "image-preview-grid": "mt-3 grid grid-cols-[repeat(auto-fill,minmax(128px,1fr))] gap-2.5 [&_img]:aspect-[4/3] [&_img]:w-full [&_img]:rounded-lg [&_img]:border [&_img]:border-[#e6e4df] [&_img]:object-cover",
    "report-types": "mb-4 grid grid-cols-4 gap-2.5 max-sm:grid-cols-1",
    "radio-card": "flex items-center gap-2 rounded-2xl border border-[#e6e4df] bg-white p-4 [&_input]:w-auto",
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
    "progress": "mt-2 h-2 overflow-hidden rounded-full bg-[#ededeb] [&_span]:block [&_span]:h-full [&_span]:rounded-full [&_span]:bg-neutral-900",
    "chart-frame": "min-h-80 p-5 [&_canvas]:!h-[300px] [&_canvas]:!w-full",
    "bar-chart": "flex h-80 items-end gap-2 p-5",
    "bar-item": "relative grid h-full min-w-[34px] flex-1 grid-rows-[1fr_auto] gap-2 text-center text-xs text-neutral-500",
    "bar": "self-end rounded-t-lg bg-neutral-900",
    "mini-chart": "flex h-32 items-end gap-2 py-3 [&_span]:min-h-2 [&_span]:flex-1 [&_span]:rounded-t-md [&_span]:bg-neutral-900",
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

  const formControlClass = "w-full rounded-lg border border-[#e6e4df] bg-white px-3 py-2.5 text-sm text-neutral-900 outline-none transition focus:border-blue-600 focus:ring-4 focus:ring-blue-600/10 disabled:bg-neutral-100 disabled:text-neutral-500";

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
    document.body.classList.add("min-h-[100dvh]", "bg-[#f7f7f5]", "text-[#1f1f1f]", "font-sans", "antialiased", "overflow-x-hidden");
    applyTailwind(document);
    new MutationObserver((mutations) => {
      mutations.forEach((mutation) => mutation.addedNodes.forEach((node) => applyTailwind(node)));
    }).observe(document.body, { childList: true, subtree: true });
  });

  window.VivuCarTailwindUI = { applyTailwind, openModal, closeModal, openDrawer, closeDrawer };
})();
