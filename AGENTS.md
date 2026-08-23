# AGENTS.md — VivuCar Frontend Guide

## Project overview

VivuCar is a self-drive car rental platform. Built with **ASP.NET Core 8 Razor Pages** + plain **CSS/JS**.

Tech stack:
- **Backend:** ASP.NET Core Web API (`VivuCarServer/API`)
- **Frontend:** Razor Pages with `Layout = null` (standalone) or `_UserLayout`
- **Styling:** Tailwind CSS via CDN + CSS files (`wwwroot/css/`) + inline `<style>`
- **JS:** Vanilla JS modules, no frameworks

---

## Required reading before any frontend work

1. `AGENTS.md` (this file)
2. `DESIGN.md` — design tokens, colors, spacing, components
3. Related CSS in `wwwroot/css/` — `booking.css`, `chat.css`, `auth.css`

---

## How pages are built

### Two layout modes

| Mode | File | Dùng cho |
|------|------|----------|
| `Layout = null` | Full CSHTML with `<html><head><body>` | Home, Search, Detail, Profile |
| `Layout = "_UserLayout"` | Content only, navbar + chatbot auto | MyBookings, Checkout, Payment |

### Standalone page template

```html
@page "/route"
@model ...
@{
    ViewData["Title"] = "...";
    Layout = null;
}
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>...</title>
    <!-- Tailwind config — xanh lá primary -->
    <script>tailwind = { config: { theme: { extend: { colors: { primary: { DEFAULT:'#16a34a',50:'#f0fdf4',100:'#dcfce7',700:'#15803d' } }, fontFamily: { sans:['Be Vietnam Pro','Segoe UI','Arial','sans-serif'] } } } } };</script>
    <script src="https://cdn.tailwindcss.com"></script>
    <link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <!-- CSS nếu cần -->
    <link rel="stylesheet" href="/css/chat.css" />
    <style>/* inline styles */</style>
</head>
<body>
    <partial name="_Navbar" />
    <!-- Nội dung -->
    <partial name="_ChatbotWidget" />
    <script>/* page logic */</script>
</body>
</html>
```

### _UserLayout page template

```html
@page "/route"
@model ...
@{
    ViewData["Title"] = "...";
    Layout = "_UserLayout";
}
<div class="booking-shell">
    <!-- nội dung, đã có navbar + chatbot sẵn -->
</div>
```

---

## Design system reference

**Always read `DESIGN.md`** before styling UI. Key rules:

| Rule | Detail |
|------|--------|
| Primary color | `#16a34a` (xanh lá), hover `#15803d` |
| Page bg | `#f7f7f5` |
| Card bg | `#ffffff` |
| Border | `1px solid #e6e4df` |
| Text | `#1f1f1f` |
| Muted | `#71717a` / `#a1a1aa` |
| Radius | 8px (input), 12px (button), 14-16px (card), 999px (badge) |
| Font | Be Vietnam Pro, 400-800 weights |

---

## CSS files — what to use

| File | Use when |
|------|----------|
| `booking.css` | Any user page with booking data (MyBookings, Checkout, Contract, Payment). Provides `.btn`, `.badge`, `.field`, `.booking-skeleton`, `.booking-card-item`, `.chip`, `.toast`, `.checkout-layout` |
| `chat.css` | Any page with chatbot (all public pages). Provides `.chatbot-panel`, `.chatbot-floating-btn`, `.chat-msg` |
| `auth.css` | Login / Register pages. Provides form styles, `.vc-*` CSS variables |
| `register.css` | Register page only. Stepper, OTP, password strength |

For pages that don't fit any CSS file, use **inline `<style>`** with `#16a34a` green theme.

---

## Tailwind CDN limitations

**CRITICAL:** Tailwind Play CDN cannot scan classes inside JS template literals. This means:

```js
// ❌ WILL NOT WORK — CDN can't scan JS strings
innerHTML = `<div class="text-green-600 font-bold">Hello</div>`;

// ✅ DO THIS — inline styles
innerHTML = `<div style="color:#16a34a;font-weight:700">Hello</div>`;

// ✅ OR — CSS class defined in <style> block
// <style>.my-class { color: #16a34a; font-weight: 700; }</style>
innerHTML = `<div class="my-class">Hello</div>`;
```

Rules:
- HTML attributes (in `.cshtml`) can use Tailwind classes safely
- JS template literals must use **inline styles** or **pre-defined CSS classes**
- For hover effects in JS: use `onmouseover`/`onmouseout` with inline style changes
- For active/press effects: use CSS class with `:active` pseudo-class

---

## Component sources

| Component | Where to find |
|-----------|---------------|
| Button (primary/secondary/ghost) | `booking.css` `.btn` + CSS variables |
| Badge (success/warning/danger) | `booking.css` `.badge-*` classes |
| Card | Inline border + radius pattern (`1px solid #e6e4df`, `border-radius:14-16px`) |
| Input | `booking.css` `.field input`, focus ring inline via JS |
| Chip / filter | `booking.css` `.chip`, `.chip-row`, `.chip.active` |
| Skeleton loading | `booking.css` `.booking-skeleton`, or inline `@keyframes shimmer` |
| Tabs | Custom `.tab-btn` + `.tab-btn.active` in `<style>` |
| Modal | `.modal-actions` in `booking.css`, custom wrapper |
| Toast | `.toast-root` + `.toast` in `booking.css` |
| Price estimate | Float animation pattern: `max-height` + `opacity` transition |
| Breadcrumb | Custom `.breadcrumb` in `booking.css` |
| Empty state | `booking.css` `.empty-state`, or `window.VivuCarUtils.renderEmptyState()` |

---

## JavaScript — shared modules

| Module | File | Provides |
|--------|------|----------|
| auth-service | `/js/shared/auth-service.js` (ES module) | `authService.refresh()`, `authService.apiFetch()` |
| navbar | `/js/shared/navbar.js` (ES module) | Active link, auth toggle, dropdown |
| utils | `/js/html/utils.js` (global) | `window.VivuCarUtils.getBookingApiUiState()`, `formatVnd()`, `renderEmptyState()`, `renderStatusBadge()`, `renderBookingStatusBadge()` |
| constants | `/js/html/constants.js` (global) | `window.VivuCarConstants`, status label maps |
| constants | `/js/html/layout.js` (global) | `window.VivuCarLayout.renderBreadcrumb()` |
| chatbot | `/js/chatbot.js` (global) | `window.VivuCarChatbot.toggleChat()`, `sendMessage()`, `closeChat()` |
| tailwind-ui | `/js/html/tailwind-ui.js` (global) | MutationObserver mapping custom classes → Tailwind classes |

### Import pattern

For `Layout = null` pages (ES module):
```js
<script type="module">
  import { authService } from '/js/shared/auth-service.js';
  // use authService directly
</script>
```

For `_UserLayout` pages (global scripts already loaded):
```js
// authService available via window.VivuCarAuthService (set by _UserLayout inline script)
// Or import as ES module
```

---

## Booking status resolution

Use `window.VivuCarUtils.getBookingApiUiState(booking, isPaid)` to derive display status.

Returns `{ key, label, tone }`:
- `key`: `payment_pending`, `handover_pending`, `renting`, `return_requested`, `completed`, `cancelled`, `rejected`
- `label`: Vietnamese display string
- `tone`: `success` / `warning` / `danger` / `neutral`

---

## API calls pattern

Use `authService.apiFetch()` for authenticated requests:

```js
import { authService } from '/js/shared/auth-service.js';

const session = await authService.refresh(); // ensures token is valid
const res = await authService.apiFetch('bookings/my-bookings?pageSize=100');
if (res.ok) {
  const data = await res.json();
  // render
}
```

For public endpoints, plain `fetch()` is fine.

---

## Responsive patterns

| Breakpoint | Effect |
|------------|--------|
| < 640px (mobile) | 1 column, smaller image (240px), bottom booking bar |
| < 768px | Nav links collapse |
| < 1024px (tablet) | Detail booking bar fixed bottom, 2 column grids become 1 |
| >= 1024px (desktop) | Full layout, sticky sidebar |

For mobile booking bar on `Layout = null` pages:
```css
@@media (max-width:1023px) { .mobile-booking-bar { display:flex; } }
```

---

## Dark mode / theme

No dark mode support. Always use light theme with `#f7f7f5` background.

---

## File naming convention

| Type | Convention | Example |
|------|------------|---------|
| Razor page | PascalCase | `Detail.cshtml`, `MyBookings.cshtml` |
| CSS | kebab-case | `booking.css`, `chat.css` |
| JS | kebab-case | `auth-service.js`, `my-bookings.js` |
| JS (old style) | kebab-case | `tailwind-ui.js`, `booking-cancellation.js` |

---

## Page quality checklist

Before finishing any frontend work, verify:

- [ ] Text in Vietnamese (except API labels)
- [ ] Primary color `#16a34a` used consistently
- [ ] Page bg `#f7f7f5` or white card on `#f7f7f5`
- [ ] Border `#e6e4df` on cards/inputs
- [ ] Loading skeleton state
- [ ] Error state
- [ ] Empty state
- [ ] Responsive (mobile + desktop)
- [ ] Chatbot included (`partial name="_ChatbotWidget"`)
- [ ] Navbar included (`partial name="_Navbar"`)
- [ ] Input focus ring green
- [ ] Active/press effect on cards/buttons (scale)
- [ ] JS template literals use inline styles (not Tailwind CDN classes)
