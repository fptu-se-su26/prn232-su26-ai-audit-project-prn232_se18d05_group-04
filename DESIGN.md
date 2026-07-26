# VivuCar Design System

## 1. Color Tokens

### Primary — Xanh lá
| Token | Giá trị | Dùng cho |
|-------|---------|----------|
| Primary | `#16a34a` | Button, link active, accent |
| Primary hover | `#15803d` | Button hover |
| Primary bg | `#f0fdf4` | Badge success, chip active |
| Primary text | `#065f46` | Badge success text |
| Focus ring | `rgba(22,163,74,.12)` | Input focus |

### Neutral
| Token | Giá trị | Dùng cho |
|-------|---------|----------|
| Page bg | `#f7f7f5` | `body` background |
| Surface | `#ffffff` | Card, modal, section |
| Border | `#e6e4df` | Card border, divider |
| Text | `#1f1f1f` | Heading, body chính |
| Text secondary | `#52525b` | Label, caption |
| Text muted | `#71717a` / `#a1a1aa` | Mô tả phụ, placeholder |
| Shimmer | `#f0f0f0` / `#e0e0e0` | Skeleton loading |

### Semantic
| Token | Giá trị | Dùng cho |
|-------|---------|----------|
| Success bg | `#f0fdf4` | Badge/xanh |
| Success text | `#065f46` | |
| Warning bg | `#fffbeb` | Badge/vàng |
| Warning text | `#92400e` | |
| Danger bg | `#fef2f2` | Badge/đỏ, error box |
| Danger text | `#dc2626` | |
| Star | `#f59e0b` / `#fbbf24` | Rating star |
| Overlay | `rgba(0,0,0,.45)` | Modal overlay |
| Overlay glass | `rgba(0,0,0,.55)` | Image counter badge |

---

## 2. Typography

### Font
`'Be Vietnam Pro', 'Segoe UI', Arial, sans-serif`

### Weights
| Weight | Dùng cho |
|--------|----------|
| 600 (semibold) | Button, label, nav link |
| 700 (bold) | Card title, section heading |
| 800 (extrabold) | Price, hero text, brand |

### Sizes
| Class | Rem | px | Dùng cho |
|-------|-----|----|----------|
| `.text-xs` | .75rem | 12 | Badge, caption, label form |
| `.text-sm` | .8125rem | 13 | Mô tả, button nhỏ, breadcrumb |
| `.text-sm` alt | .875rem | 14 | Button, body text |
| `.text-base` | .9375rem | 15 | Nav link |
| `.text-lg` | 1rem | 16 | Card title, section heading |
| `.text-xl` | 1.125rem-1.25rem | 18-20 | Page title |
| `.text-2xl` | 1.625rem | 26 | Price lớn |

### Line height
- Heading: `1.3`
- Body: `1.6` / `1.7`
- Button: `1.4`

---

## 3. Spacing

| Khoảng cách | rem | Dùng cho |
|-------------|-----|----------|
| 2px | | Separator line |
| 4px | | Icon spacing |
| 6px | | Badge padding |
| 8px | .5rem | Gap nhỏ |
| 10px | .625rem | |
| 12px | .75rem | Gap element |
| 14px | .875rem | |
| 16px | 1rem | Card padding |
| 18px | | |
| 20px | 1.25rem | Section padding |
| 24px | 1.5rem | Page padding, container gap |
| 28px | | |
| 32px | 2rem | Nav link gap |
| 48px | 3rem | Empty state |

### Container max-width: `1280px` (max-w-7xl)

---

## 4. Border & Radius

| Token | Giá trị | Dùng cho |
|-------|---------|----------|
| Border | `1px solid #e6e4df` | Card, input, divider |
| Radius sm | 6px | Input, small element |
| Radius md | 8px-10px | Button, field |
| Radius lg | 12px | Card, container |
| Radius xl | 14px-16px | Large card, modal |
| Radius pill | 999px/9999px | Badge, chip, avatar |

### Box shadow
- Card hover: `0 4px 16px rgba(0,0,0,.06)`
- Dropdown: `0 12px 32px rgba(0,0,0,.1)`
- Chatbot btn: `0 8px 24px rgba(22,163,74,.3)`

---

## 5. Navbar (`_Navbar.cshtml`)

- Sticky top, z-50
- Background: `rgba(255,255,255,.92)` + `backdrop-filter: blur(12px)`
- Height: 64px
- Border bottom: `1px solid rgba(0,0,0,.06)`
- Link: `.9375rem`, weight 600, `#52525b` → hover `#1f1f1f`
- Active: `#16a34a`
- Logo: 32px height
- Brand text: `1.1rem`, weight 800
- Login button: `#16a34a` bg, 10px radius
- Dropdown: 180px width, 12px radius, `#fff` bg
- Mobile: nav links hidden (`display:none`), visible at `>=768px` (`.md-flex`)

**Auth state:** `#navGuestState` (hidden by default) / `#navUserState` (hidden by default). JS toggles via `classList.remove('hidden')`.

---

## 6. Components

### Button
```
.btn                — base: inline-flex, 9px 18px padding, 8px radius, .875rem, 600 weight
.btn-primary        — bg #16a34a, #fff text, border #16a34a
.btn-secondary      — bg #fff, #1f1f1f text, border #e6e4df
.btn-ghost          — transparent, #a1a1aa text
.btn-sm             — 6px 12px padding, .8125rem
.btn-full           — width: 100%
.btn:disabled       — opacity .55, cursor not-allowed
```

### Badge
```
.badge              — inline-flex, 3px 10px padding, 99px radius, .75rem, 700 weight
.badge-success      — bg #ecfdf5, #065f46 text
.badge-warning      — bg #fffbeb, #92400e text
.badge-danger       — bg #fef2f2, #991b1b text
.badge-info         — bg #eff6ff, #1d4ed8 text
.badge-neutral      — bg #f3f4f6, #374151 text
```

### Card
- Bg: `#ffffff`
- Border: `1px solid #e6e4df`
- Radius: 14px-16px
- Padding: 16px-24px
- Transition hover: `transform .2s, box-shadow .2s`
- Active press: `transform: scale(0.97-0.99)`

### Input
- Border: `1px solid #e6e4df`, radius 8px-10px
- Padding: `9px 12px`
- Focus: border `#16a34a` + box-shadow `0 0 0 3px rgba(22,163,74,.12)`
- Error: border `#dc2626`
- Disabled: bg `#f9fafb`, cursor `not-allowed`

### Skeleton / Loading
```css
.skeleton {
  background: linear-gradient(90deg,#f0f0f0 25%,#e0e0e0 50%,#f0f0f0 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: 12px;
}
@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
```

### Tabs
- Flex row, border-bottom `#e6e4df`
- Tab button: padding 10-12px 18-20px, .8125rem, 600 weight, `#71717a`
- Active tab: `#16a34a` + `border-bottom-color: #16a34a`

### Chip / Filter
```
.chip               — border-radius 999px, padding 4px 14px, font .8125rem weight 600
.chip.active        — bg #16a34a, text #fff
.chip-row           — display flex, gap 8px, flex-wrap wrap
```

### Breadcrumb
- Flex, gap 6px, `.8125rem`, `#a1a1aa`
- Separator: `›` (`#d4d4d8`)
- Current: `#1f1f1f`, weight 600

### Stepper (register page)
- Dots: 32px circle, `--vc-green-700` active, `--vc-green-500` done
- Connector: 48px x 2px line, `--vc-green-500` done
- Label: centered, `.7rem`, weight 600

### Price estimate panel
- Float animation: `max-height 0 → 300px`, `opacity 0 → 1` over `.35s`

---

## 7. Layout Patterns

### App layout (public pages)
```
┌─────────────────────────────────┐
│         Navbar (sticky)         │
├─────────────────────────────────┤
│                                 │
│      Page content (container)   │
│      max-width: 1280px          │
│      padding: 24px 20px        │
│                                 │
├─────────────────────────────────┤
│         Footer (optional)       │
└─────────────────────────────────┘
┌─────────────────────────────────┐
│    Chatbot (fixed bottom-right) │
└─────────────────────────────────┘
```

### Layout modes
| Mode | Structure | Ví dụ |
|------|-----------|-------|
| Standalone (Layout=null) | Self-contained CSHTML + inline `<style>` + inline `<script>` | Home, Search, Detail, Profile |
| User layout | `_UserLayout` → navbar + chatbot + CSS + JS | MyBookings, Checkout, Payment |
| Admin layout | `_AdminLayout` | Dashboard, Users, Cars |
| Auth layout | `_AuthLayout` | Login, Register |

### Grid patterns
- **2-column search:** `grid-template-columns: 280px 1fr` (filter sidebar + results)
- **3-column car grid:** `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5`
- **Detail page:** `grid-template-columns: 1fr 380px` (main + sidebar)
- **Checkout:** `grid-template-columns: 1fr 360px`
- **2-field form:** `grid-template-columns: 1fr 1fr`

---

## 8. CSS Files

| File | Scope | Nội dung |
|------|-------|----------|
| `booking.css` | User pages | Design tokens, card, button, badge, form, stepper, modal, toast, skeleton |
| `chat.css` | Global | Chatbot widget panel, floating button, messages |
| `auth.css` | Auth pages | Login/Register form style, step indicator |
| `register.css` | Register | Register stepper, OTP input, password strength |
| `admin.css` | Admin panel | Admin layout tokens, sidebar, table, stats |
| `animations.css` | User pages | Float animation, page transition |

### Cần link CSS nào ở trang nào
| Trang | CSS |
|-------|-----|
| Home (/), Search (/cars), Detail (/cars/detail) | chat.css (nếu có chatbot) |
| MyBookings, Checkout, Contract, Payment | booking.css + chat.css + animations.css |
| Login, Register | auth.css + register.css |
| Admin pages | admin.css |
| _UserLayout | booking.css + chat.css + animations.css (tự động) |

---

## 9. Shared Partials

| Partial | File | Dùng ở |
|---------|------|--------|
| Navbar | `_Navbar.cshtml` | All public pages |
| Chatbot | `_ChatbotWidget.cshtml` | All public pages |
| User layout | `_UserLayout.cshtml` | Booking, Payment |
| Auth layout | `_AuthLayout.cshtml` | Login, Register |
| Admin layout | `/Admin/Shared/_AdminLayout.cshtml` | Admin pages |
| Owner layout | `/Owner/Shared/_OwnerLayout.cshtml` | Owner pages |

### Script dependencies (`_UserLayout`)
```
auth-service.js     → API auth
tailwind-ui.js      → MutationObserver class mapper
constants.js        → Window.VivuCarConstants
utils.js            → Window.VivuCarUtils
auth.js             → Window.VivuCarAuth (legacy)
layout.js           → Window.VivuCarLayout (breadcrumb)
chatbot.js          → Window.VivuCarChatbot
```

For **standalone pages** (Layout=null), import only what needed:
```html
<script type="module">
  import { authService } from '/js/shared/auth-service.js';
  // ...
</script>
```

---

## 10. Responsive Breakpoints

| Breakpoint | Width | Effect |
|------------|-------|--------|
| Mobile | < 640px | 1 column, smaller padding, bottom booking bar |
| Tablet | 640px-1023px | 2 columns, medium padding |
| Desktop | >= 1024px | Full layout, sticky sidebar |
| Nav collapse | < 768px | Nav links hidden |

### Mobile adaptations
- Search: filter sidebar ẩn, mobile search bar hiện (`block md:hidden`)
- Detail: bottom fixed booking bar (`@media max-width:1023px`)
- Card grid: `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3`
- Booking card detail: image 240px height

---

## 11. Animations

### Float (home page CTA)
```css
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-6px); }
}
.float-anim { animation: float 3s ease-in-out infinite; }
```

### Card hover
```css
.card-hover {
  transition: transform .2s, box-shadow .2s;
}
.card-hover:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0,0,0,.06);
}
.card-hover:active {
  transform: scale(0.98);
}
```

### Image zoom (gallery / booking card)
```css
.img-zoom-wrap { overflow: hidden; border-radius: 12px; }
.img-zoom-wrap img {
  transition: transform .4s ease;
}
.img-zoom-wrap:hover img {
  transform: scale(1.05);
}
```

### Slide panel (chatbot)
```css
.chatbot-panel {
  transform: translateX(100%);
  transition: transform .35s cubic-bezier(.4,0,.2,1);
}
.chatbot-panel.active { transform: translateX(0); }
```

### Register step transition
```css
@keyframes slide-fwd {
  from { opacity: 0; transform: translateX(24px); }
  to   { opacity: 1; transform: translateX(0); }
}
```

---

## 12. Icons

Dùng inline SVG (Lucide icons). Pattern:
```html
<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
     stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
  <path d="..."/>
</svg>
```
- Icon trong button: `width=18-20`, `stroke-width=2`
- Icon nhỏ trong text: `width=14-16`
- Icon trong badge: `width=16`
- Decorative lớn: `font-size` emoji fallback

---

## 13. How to add a new page

```html
@page "/your-route"
@model YourModel
@{
    ViewData["Title"] = "Tiêu đề";
    Layout = null;  // standalone
    // hoặc Layout = "_UserLayout";  // nếu có navbar + chatbot tự động
}
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>...</title>
    <!-- Tailwind CDN -->
    <script>
        tailwind = { config: { theme: { extend: { colors: { primary: { DEFAULT:'#16a34a',50:'#f0fdf4',100:'#dcfce7',700:'#15803d' } }, fontFamily: { sans:['Be Vietnam Pro',...] } } } } };
    </script>
    <script src="https://cdn.tailwindcss.com"></script>
    <link href="..." rel="stylesheet" />
    <!-- CSS nếu cần: chat.css, booking.css, ... -->
    <style>
        /* Inline critical styles */
        @keyframes shimmer { ... }
    </style>
</head>
<body>
    <partial name="_Navbar" />
    <!-- Nội dung -->
    <partial name="_ChatbotWidget" />
    <script type="module">
        import { authService } from '/js/shared/auth-service.js';
    </script>
    <script>
        // Page logic
    </script>
</body>
</html>
```

### Layout = "_UserLayout" (có navbar + chatbot sẵn)
```html
@page "/your-route"
@model ...
@{
    ViewData["Title"] = "...";
    Layout = "_UserLayout";
}
<!-- Không cần html/head/body, chỉ nội dung -->
<div class="booking-shell">
    ...
</div>
```

---

## 14. Quick Reference

### CSS class cho booking page
```
.booking-shell    — max-width 1000px, padding
.booking-skeleton — skeleton loading card
.booking-error    — red error card
.booking-card-item — booking card wrapper
.checkout-layout  — 2-column checkout
.checkout-section — section trong checkout
.summary-card     — sidebar summary
.flow-steps       — progress stepper
.field            — form field group
.toast            — toast notification
.result-card      — success/error result
```

### CSS variables booking.css
```
--bk-bg            : #f7f7f5
--bk-surface       : #ffffff
--bk-border        : #e6e4df
--bk-border-strong : #d8d6d0
--bk-text          : #1f1f1f
--bk-muted         : #6b7280
--bk-primary       : #16a34a
--bk-primary-h     : #15803d
--bk-danger        : #dc2626
--bk-radius        : 12px
--bk-radius-lg     : 16px
--bk-shadow        : 0 1px 3px rgba(0,0,0,.06)
```

### CSS variables auth.css
```
--vc-green-{900..100}
--vc-gray-{900..50}
--vc-bg, --vc-surface
--vc-error, --vc-error-bg, --vc-error-border
--vc-focus-ring
--vc-input-bg, --vc-input-border
--vc-radius-sm, --vc-radius-md
```
