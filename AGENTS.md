# AGENTS.md - VivuCar Frontend Rules

## Project overview

VivuCar is a self-drive car rental platform in Da Nang.

The frontend supports three main roles:

- `user`: customer who rents cars
- `car_owner`: partner/owner who lists cars and handles bookings
- `admin`: system administrator

The project must be implemented with:

- HTML
- CSS
- JavaScript

Do not use:

- React
- Vue
- Angular
- Bootstrap
- Tailwind
- jQuery
- UI component frameworks

Use plain browser APIs only.

---

## Required reading order

Before implementing or modifying any page, always read in this order:

1. `AGENTS.md`
2. `draw-db.sql`
3. Related files inside `specs/`
4. Related skill files in `.agents/skills/`

The database schema in `draw-db.sql` is the source of truth.

If a spec conflicts with `draw-db.sql`, follow `draw-db.sql`.

---

## Required skill usage

Before designing or coding UI, read and follow:

```txt
.agents/skills/gpt-taste/SKILL.md
.agents/skills/design-taste-frontend/SKILL.md
```

Use:

```txt
.agents/skills/imagegen-frontend-web/SKILL.md
```

only when the task explicitly asks for image-based design references, mockup images, landing page visual concepts, or screenshot-style output.

Do not use image generation skills for normal HTML/CSS/JS implementation.

If a skill conflicts with the database schema, the database schema wins.

---

## Specs directory rule

The `specs/` folder contains the UI requirements for all team members.

Expected structure:

```txt
specs/
├── mb1-admin/
├── mb2-user-app/
├── mb3-booking-payment/
├── mb4-post-trip-owner-car/
└── mb5-owner-support/
```

Do not implement from `.zip` files directly.

If `.zip` files still exist inside `specs/`, extract them first.

PowerShell unzip command:

```powershell
Get-ChildItem -Path .\specs -Recurse -Filter *.zip | ForEach-Object {
    $dest = $_.DirectoryName
    Expand-Archive -Path $_.FullName -DestinationPath $dest -Force
}
```

After extraction, use the `.md` files as the source of truth and ignore the `.zip` files.

---

## Module ownership rules

### Member 1 - Admin

Folder:

```txt
specs/mb1-admin
```

Controls:

- Login and role routing
- User account management
- Admin car management
- Revenue dashboard
- Excel/PDF export
- Voucher management

### Member 2 - User App/Web

Folder:

```txt
specs/mb2-user-app
```

Controls:

- Home page
- Car browsing
- Featured cars
- Car detail
- Search and filter
- User profile
- Avatar upload
- Driving license upload
- User reviews

### Member 3 - Booking & Payment

Folder:

```txt
specs/mb3-booking-payment
```

Controls:

- Booking checkout
- Availability check
- Driver information and documents
- Dynamic pricing
- Deposit payment
- Payment result
- Booking confirmation
- My bookings
- Booking detail
- Contract preview
- Cancellation

### Member 4 - Post-trip & Owner Car Management

Folder:

```txt
specs/mb4-post-trip-owner-car
```

Controls:

- Return car request
- Owner return inspection
- Trip completion
- Post-trip review
- Incident report
- Owner car list
- Owner create/edit car
- Car image management
- Car status and maintenance
- Car activity history

### Member 5 - Owner Orders & Support

Folder:

```txt
specs/mb5-owner-support
```

Controls:

- Owner booking requests
- Booking request detail
- Accept/decline booking
- Handover dashboard
- Handover condition
- Return condition
- Confirm vehicle return
- Booking status tracking
- AI chatbot widget
- Owner live chat
- Chat history

---

## Implementation order

When implementing the whole system, follow this order:

1. Shared foundation
   - `tokens.css`
   - `base.css`
   - `layout.css`
   - `components.css`
   - `pages.css`
   - `constants.js`
   - `utils.js`
   - `db-mock.js`
   - `auth.js`
   - `layout.js`

2. User flow
   - Member 2
   - Member 3

3. Owner flow
   - Member 5
   - Member 4

4. Admin flow
   - Member 1

Do not randomly implement isolated pages before creating shared layout, shared constants, and mock data.

---

## Recommended project structure

Use this structure:

```txt
RAW-HTML/
├── AGENTS.md
├── draw-db.sql
├── index.html
├── specs/
│   ├── mb1-admin/
│   ├── mb2-user-app/
│   ├── mb3-booking-payment/
│   ├── mb4-post-trip-owner-car/
│   └── mb5-owner-support/
├── pages/
│   ├── login.html
│   ├── home.html
│   ├── search.html
│   ├── car-detail.html
│   ├── booking-checkout.html
│   ├── my-bookings.html
│   ├── owner-booking-requests.html
│   ├── owner-handover-dashboard.html
│   ├── owner-support-inbox.html
│   └── admin-dashboard.html
└── assets/
    ├── css/
    │   ├── tokens.css
    │   ├── base.css
    │   ├── layout.css
    │   ├── components.css
    │   └── pages.css
    └── js/
        ├── constants.js
        ├── utils.js
        ├── db-mock.js
        ├── auth.js
        ├── layout.js
        └── page-specific-files.js
```

---

## Visual design direction

Design the site in a Notion-inspired modern style.

Use:

- Clean white and off-white backgrounds
- Soft gray borders
- Calm spacing
- Clear typography
- Document-like cards and panels
- Minimal shadows
- Rounded corners
- Restrained colors
- Modern SaaS dashboard layout
- Subtle transitions
- Readable tables and forms

Avoid:

- Heavy gradients
- Colorful generic admin-template UI
- Excessive icons
- Oversized shadows
- Dense forms without grouping
- Inconsistent spacing
- Inconsistent button styles
- Random colors

Suggested design tokens:

```css
:root {
  --bg: #f7f7f5;
  --surface: #ffffff;
  --surface-muted: #fbfbfa;
  --text: #1f1f1f;
  --text-muted: #6b6b6b;
  --border: #e6e4df;
  --border-strong: #d8d6d0;

  --primary: #2563eb;
  --primary-soft: #eff6ff;

  --success: #15803d;
  --success-soft: #ecfdf3;

  --warning: #b45309;
  --warning-soft: #fffbeb;

  --danger: #b91c1c;
  --danger-soft: #fef2f2;

  --radius-sm: 8px;
  --radius-md: 12px;
  --radius-lg: 16px;

  --shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.04);
  --shadow-md: 0 8px 24px rgba(0, 0, 0, 0.06);
}
```

---

## Database contract

Always follow `draw-db.sql`.

Do not invent database fields or enum values when the schema already defines them.

If a requested feature needs fields that are not present in the current schema, do one of these:

1. Implement it as frontend-only mock state and add this code comment:

```js
// UI-only field. Not present in current DB schema. Requires migration before backend integration.
```

2. Or propose a database migration separately if asked.

---

## Schema mapping rules

### users

Table:

```txt
users
```

Fields:

- `id`
- `google_id`
- `email`
- `full_name`
- `avatar_url`
- `role`
- `is_blocked`
- `created_at`

Allowed `role` values:

- `user`
- `car_owner`
- `admin`

Frontend labels:

```js
const USER_ROLE_LABELS = {
  user: "Khách thuê",
  car_owner: "Chủ xe",
  admin: "Quản trị viên"
};
```

Use `is_blocked` for account lock/block UI.

Do not create a fake `status` field for users.

---

### user_documents

Table:

```txt
user_documents
```

Fields:

- `id`
- `user_id`
- `document_type`
- `file_name`
- `file_url`
- `verified`
- `created_at`

Allowed `document_type` values:

- `avatar`
- `license_front`
- `license_back`

Use this table for avatar and driving license upload.

Current DB does not contain CCCD document types. If CCCD upload is required in UI, mark it as UI-only or propose migration.

---

### car_types

Table:

```txt
car_types
```

Fields:

- `id`
- `name`

Use this for vehicle category/type filters.

---

### cars

Table:

```txt
cars
```

Fields:

- `id`
- `owner_id`
- `brand`
- `model`
- `type_id`
- `license_plate`
- `year`
- `color`
- `seats`
- `kilometers_driven`
- `transmission`
- `fuel_type`
- `price_per_day`
- `price_per_hours`
- `address`
- `description`
- `status`
- `blocked_reason`
- `created_at`

Allowed `transmission` values:

- `manual`
- `automatic`
- `cvt`

Frontend labels:

```js
const TRANSMISSION_LABELS = {
  manual: "Số sàn",
  automatic: "Tự động",
  cvt: "CVT"
};
```

Allowed `fuel_type` values:

- `gasoline`
- `diesel`
- `electric`
- `hybrid`

Frontend labels:

```js
const FUEL_TYPE_LABELS = {
  gasoline: "Xăng",
  diesel: "Dầu",
  electric: "Điện",
  hybrid: "Hybrid"
};
```

Allowed `status` values:

- `available`
- `rented`
- `maintenance`
- `blocked`

Frontend labels:

```js
const CAR_STATUS_LABELS = {
  available: "Đang rảnh",
  rented: "Đang thuê",
  maintenance: "Bảo trì",
  blocked: "Đã khóa"
};
```

Do not use `unavailable` unless a migration is added.

If a spec says `Unavailable`, map it to `blocked` in the current schema.

---

### car_images

Table:

```txt
car_images
```

Fields:

- `id`
- `car_id`
- `image_url`
- `is_primary`

Use this for:

- car gallery
- primary car image
- car image management

---

### vouchers

Table:

```txt
vouchers
```

Fields:

- `id`
- `name`
- `code`
- `discount_type`
- `discount_value`
- `min_order_amount`
- `max_discount`
- `quantity`
- `expires_at`
- `created_at`

Allowed `discount_type` values:

- `percentage`
- `fixed`

Frontend labels:

```js
const DISCOUNT_TYPE_LABELS = {
  percentage: "Giảm theo phần trăm",
  fixed: "Giảm số tiền cố định"
};
```

Current DB does not have:

- `start_date`
- `is_active`
- `usage_limit_per_user`

Treat those as UI-only if needed.

---

### voucher_usages

Table:

```txt
voucher_usages
```

Fields:

- `id`
- `voucher_id`
- `user_id`
- `booking_id`
- `used_at`

Use this to calculate:

- voucher usage count
- voucher performance
- remaining quantity

---

### bookings

Table:

```txt
bookings
```

Fields:

- `id`
- `user_id`
- `car_id`
- `pickup_datetime`
- `return_datetime`
- `pickup_address`
- `total_amount`
- `voucher_id`
- `status`
- `created_at`

Allowed `status` values:

- `pending`
- `approved`
- `rejected`
- `completed`
- `cancelled`

Frontend labels:

```js
const BOOKING_STATUS_LABELS = {
  pending: "Chờ xử lý",
  approved: "Đã xác nhận",
  rejected: "Đã từ chối",
  completed: "Hoàn tất",
  cancelled: "Đã hủy"
};
```

Do not persist statuses such as:

- `ready_for_handover`
- `in_progress`
- `return_pending`
- `deposit_paid`
- `dispute_open`
- `pending_owner_confirmation`

If UI needs a more detailed workflow state, derive it from:

- `bookings.status`
- `payments.status`
- `handover_inspections.inspection_type`
- current date compared with `pickup_datetime` and `return_datetime`

Example:

```js
function resolveBookingUiState(booking, payment, inspections) {
  if (booking.status === "cancelled") return "Đã hủy";
  if (booking.status === "rejected") return "Đã từ chối";
  if (booking.status === "completed") return "Hoàn tất";

  if (booking.status === "pending") {
    return payment?.status === "success"
      ? "Đã cọc - chờ chủ xe xác nhận"
      : "Chờ thanh toán hoặc xác nhận";
  }

  if (booking.status === "approved") {
    const hasPreRental = inspections.some(
      item => item.inspection_type === "pre_rental"
    );

    const hasPostRental = inspections.some(
      item => item.inspection_type === "post_rental"
    );

    if (!hasPreRental) return "Chờ bàn giao";
    if (!hasPostRental) return "Đang thuê hoặc chờ trả xe";

    return "Chờ hoàn tất";
  }

  return "Không xác định";
}
```

---

### payments

Table:

```txt
payments
```

Fields:

- `id`
- `booking_id`
- `method`
- `amount`
- `status`
- `transaction_code`
- `paid_at`

Allowed `method` values:

- `vnpay`
- `momo`
- `cash`

Allowed `status` values:

- `pending`
- `success`
- `failed`
- `refunded`

Frontend labels:

```js
const PAYMENT_METHOD_LABELS = {
  vnpay: "VNPay",
  momo: "MoMo",
  cash: "Tiền mặt"
};

const PAYMENT_STATUS_LABELS = {
  pending: "Chờ thanh toán",
  success: "Thành công",
  failed: "Thất bại",
  refunded: "Đã hoàn tiền"
};
```

Do not use `zalopay` unless a migration is added.

Current DB does not separate deposit and remaining amount. If needed, derive in UI or mark UI-only.

---

### notifications

Table:

```txt
notifications
```

Fields:

- `id`
- `user_id`
- `title`
- `content`
- `is_read`
- `created_at`

Use for:

- notification dropdown
- unread badge
- system messages

---

### handover_inspections

Table:

```txt
handover_inspections
```

Fields:

- `id`
- `booking_id`
- `inspected_by`
- `inspection_type`
- `odometer_km`
- `damage_notes`
- `note`
- `confirmed_at`

Allowed `inspection_type` values:

- `pre_rental`
- `post_rental`

Use:

- `pre_rental`: inspection before customer receives car
- `post_rental`: inspection after customer returns car

Current DB does not have:

- `fuel_level`
- `interior_status`
- `exterior_status`
- `extra_fee`

Treat these as UI-only if needed.

---

### inspection_images

Table:

```txt
inspection_images
```

Fields:

- `id`
- `inspection_id`
- `url`
- `caption`
- `uploaded_at`

Use for handover and return proof images.

---

### reviews

Table:

```txt
reviews
```

Fields:

- `id`
- `booking_id`
- `reviewer_id`
- `car_id`
- `rating`
- `comment`
- `created_at`

Allowed `rating`: 1 to 5.

Current DB supports:

- one rating
- one comment

Do not persist multi-category rating unless a migration is added.

---

### incident_reports

Table:

```txt
incident_reports
```

Fields:

- `id`
- `booking_id`
- `reported_by`
- `title`
- `description`
- `status`
- `created_at`

Allowed `status` values:

- `open`
- `in_review`
- `resolved`
- `closed`

Use for post-trip issue reporting.

---

### incident_images

Table:

```txt
incident_images
```

Fields:

- `id`
- `incident_id`
- `url`
- `caption`
- `uploaded_at`

Use for incident proof images.

---

### chat_sessions

Table:

```txt
chat_sessions
```

Fields:

- `id`
- `user_id`
- `booking_id`
- `session_type`
- `status`
- `assigned_to`
- `escalated_at`
- `closed_at`
- `created_at`

Allowed `session_type` values:

- `ai`
- `live`

Allowed `status` values:

- `open`
- `escalated`
- `closed`

Use `assigned_to` for owner/admin taking over chat.

Do not use fake statuses like `WAITING_OWNER`, `AI_ONLY`, or `OWNER_JOINED` as stored DB values.

Map them as UI labels if needed:

```js
function resolveChatUiState(session) {
  if (session.status === "closed") return "Đã xử lý";
  if (session.status === "escalated") return "Chờ người hỗ trợ";
  if (session.session_type === "ai") return "AI đang hỗ trợ";
  if (session.session_type === "live") return "Live chat";
  return "Đang mở";
}
```

---

### chat_messages

Table:

```txt
chat_messages
```

Fields:

- `id`
- `session_id`
- `sender_id`
- `role`
- `content`
- `created_at`

Allowed `role` values:

- `user`
- `assistant`
- `system`

Use:

- `user`: customer or owner typed messages
- `assistant`: AI chatbot replies
- `system`: system notices

Current DB does not distinguish owner role in `chat_messages.role`.

To display owner messages, use `sender_id` joined with `users.role`.

---

### daily_revenue_snapshots

Table:

```txt
daily_revenue_snapshots
```

Fields:

- `id`
- `snapshot_date`
- `total_bookings`
- `completed_bookings`
- `cancelled_bookings`
- `gross_revenue`
- `net_revenue`
- `deposit_collected`
- `generated_at`

Use this for admin revenue dashboard.

---

### export_jobs

Table:

```txt
export_jobs
```

Fields:

- `id`
- `requested_by`
- `export_type`
- `params`
- `status`
- `file_url`
- `error_message`
- `created_at`
- `completed_at`

Allowed `status` values:

- `pending`
- `processing`
- `done`
- `failed`

Use this for Excel/PDF export UI.

---

## Conflict resolution rules

If two specs describe the same feature differently, resolve conflicts in this order:

1. `draw-db.sql`
2. `AGENTS.md`
3. The most specific member spec
4. Shared UI consistency

Examples:

- If a spec uses `DEPOSIT_PAID` but DB only has `payments.status = success`, do not create a new booking status. Derive the label from payment data.
- If a spec uses `UNAVAILABLE` but DB only has `cars.status = blocked`, use `blocked`.
- If a spec uses `WAITING_OWNER` but DB only has `chat_sessions.status = escalated`, use `escalated`.
- If a spec uses `ZaloPay` but DB only has `vnpay`, `momo`, `cash`, do not implement `ZaloPay` as persisted payment method.

---

## Shared constants rule

Create a shared constants file.

If using browser global scripts, attach constants to `window.VivuCarConstants`.

Example:

```js
window.VivuCarConstants = {
  USER_ROLES: {
    USER: "user",
    CAR_OWNER: "car_owner",
    ADMIN: "admin"
  },

  CAR_STATUS: {
    AVAILABLE: "available",
    RENTED: "rented",
    MAINTENANCE: "maintenance",
    BLOCKED: "blocked"
  },

  BOOKING_STATUS: {
    PENDING: "pending",
    APPROVED: "approved",
    REJECTED: "rejected",
    COMPLETED: "completed",
    CANCELLED: "cancelled"
  },

  PAYMENT_METHOD: {
    VNPAY: "vnpay",
    MOMO: "momo",
    CASH: "cash"
  },

  PAYMENT_STATUS: {
    PENDING: "pending",
    SUCCESS: "success",
    FAILED: "failed",
    REFUNDED: "refunded"
  },

  CHAT_SESSION_STATUS: {
    OPEN: "open",
    ESCALATED: "escalated",
    CLOSED: "closed"
  }
};
```

---

## Mock data rules

Mock data must follow database table shape first.

Good:

```js
const cars = [
  {
    id: 1,
    owner_id: 2,
    brand: "Toyota",
    model: "Vios",
    type_id: 1,
    license_plate: "43A-12345",
    year: 2022,
    color: "Trắng",
    seats: 5,
    kilometers_driven: 28000,
    transmission: "automatic",
    fuel_type: "gasoline",
    price_per_day: 650000,
    price_per_hours: 90000,
    address: "Hải Châu, Đà Nẵng",
    description: "Xe sạch, vận hành ổn định.",
    status: "available",
    blocked_reason: null,
    created_at: "2026-05-01T08:00:00"
  }
];
```

Bad:

```js
const cars = [
  {
    carName: "Toyota Vios",
    dailyPrice: 650000,
    status: "AVAILABLE"
  }
];
```

If display-specific data is needed, create mapper functions.

Example:

```js
function mapCarToCard(car) {
  return {
    title: `${car.brand} ${car.model}`,
    priceText: formatVnd(car.price_per_day),
    statusLabel: CAR_STATUS_LABELS[car.status]
  };
}
```

---

## Form field naming rules

Use database-aligned names in form fields whenever possible.

Good:

```html
<input id="license_plate" name="license_plate" />
<input id="price_per_day" name="price_per_day" />
<input id="price_per_hours" name="price_per_hours" />
```

Avoid:

```html
<input id="licensePlate" />
<input id="dailyPrice" />
```

View-only computed fields can use UI names, but persisted fields should match DB columns.

---

## API comment rules

When writing fetch calls or mock API wrappers, use endpoint comments based on DB entities.

Examples:

```js
// GET /api/cars?status=available
// GET /api/cars/:id
// POST /api/bookings
// GET /api/bookings/:id
// POST /api/payments
// GET /api/reviews?car_id=1
```

Frontend must not pretend to update fields that are not in the current DB schema.

---

## JavaScript rules

Use plain JavaScript only.

Allowed:

```js
document.querySelector()
document.querySelectorAll()
addEventListener()
localStorage
sessionStorage
URLSearchParams
FormData
FileReader
Blob
fetch
setTimeout
setInterval
```

Not allowed:

```js
React
Vue
Angular
jQuery
Bootstrap JS
Tailwind runtime
```

Prefer clear, small functions:

```js
function renderCars(cars) {}
function applyFilters() {}
function openModal(id) {}
function closeModal(id) {}
function formatVnd(value) {}
function showToast(message, type) {}
```

Avoid huge anonymous scripts that are difficult to maintain.

---

## Shared layout rules

Create reusable layout helpers with plain JavaScript:

```js
renderUserHeader()
renderOwnerHeader()
renderAdminHeader()
renderOwnerSidebar(activeKey)
renderAdminSidebar(activeKey)
renderProfileSidebar(activeKey)
renderBreadcrumb(items)
renderStatusBadge(type, value)
renderEmptyState(config)
renderToast(message, type)
```

Do not duplicate the same header/sidebar HTML manually in every page unless absolutely necessary.

---

## File naming rules

Use kebab-case.

Good:

```txt
owner-booking-requests.html
booking-checkout.html
car-detail.html
admin-dashboard.html
owner-support-inbox.html
```

Avoid:

```txt
OwnerBookingRequests.html
bookingCheckout.html
carDetailPage.html
```

CSS files:

```txt
tokens.css
base.css
layout.css
components.css
pages.css
```

JS files:

```txt
constants.js
utils.js
db-mock.js
layout.js
auth.js
booking-checkout.js
owner-booking-requests.js
```

---

## UI component rules

Use consistent components:

- button
- input
- select
- textarea
- checkbox
- radio card
- status badge
- card
- table
- modal
- drawer
- toast
- tabs
- pagination
- empty state
- loading skeleton
- breadcrumb
- sidebar
- header
- file upload preview
- image gallery
- simple bar chart

Every component should have:

- default state
- hover state
- focus state
- disabled state where relevant
- responsive behavior

---

## Page quality checklist

Every completed page must include:

- Working navigation
- Correct active sidebar/menu state
- Loading state
- Empty state
- Error state where relevant
- Responsive layout
- Form validation
- Toast or inline feedback after actions
- DB-aligned field names
- DB-aligned enum values
- No fake backend fields without `UI-only` comments
- Accessible modal close behavior
- Meaningful button labels
- Clear primary action
- Clear secondary action

---

## Authentication and routing rules

Use mock authentication with `localStorage`.

Expected keys:

```js
localStorage.setItem("vivucar_current_user_id", user.id);
localStorage.setItem("vivucar_current_user_role", user.role);
```

Role routing:

```js
const roleRoutes = {
  user: "pages/home.html",
  car_owner: "pages/owner-booking-requests.html",
  admin: "pages/admin-dashboard.html"
};
```

Auth guard:

- Admin pages require `role === "admin"`
- Owner pages require `role === "car_owner"`
- User profile/booking pages require `role === "user"`

If unauthorized:

- redirect to `login.html`
- or show a simple `403` page

---

## Booking workflow rules

Use DB booking statuses only:

```txt
pending
approved
rejected
completed
cancelled
```

For detailed UI workflow, derive labels from related data.

Examples:

- `pending` + no successful payment => `Chờ thanh toán hoặc xác nhận`
- `pending` + successful payment => `Đã cọc - chờ chủ xe xác nhận`
- `approved` + no `pre_rental` inspection => `Chờ bàn giao`
- `approved` + has `pre_rental` + no `post_rental` => `Đang thuê hoặc chờ trả xe`
- `approved` + has `post_rental` => `Chờ hoàn tất`
- `completed` => `Hoàn tất`
- `cancelled` => `Đã hủy`

Never store derived workflow labels as booking statuses.

---

## Payment rules

Allowed payment methods:

```txt
vnpay
momo
cash
```

Do not implement `zalopay` as a DB-backed payment method unless a migration exists.

Payment status:

```txt
pending
success
failed
refunded
```

Frontend can show payment result pages, but final payment status must come from backend/IPN logic.

Add this comment near payment-result logic:

```js
// Frontend only displays payment result.
// Backend must verify gateway signature/checksum before updating payments.status.
```

---

## Chatbot and live chat rules

Use:

- `chat_sessions`
- `chat_messages`

DB-backed session types:

```txt
ai
live
```

DB-backed session statuses:

```txt
open
escalated
closed
```

DB-backed message roles:

```txt
user
assistant
system
```

For owner live chat, display sender label by joining `chat_messages.sender_id` with `users.role`.

Do not create a fake `owner` role inside `chat_messages.role`.

---

## Upload rules

Use `FileReader` for preview.

Use `FormData` for upload mocks.

Validate:

- file type
- file size
- max file count
- required images

For images not represented in DB, mark fields UI-only.

Examples:

```js
// UI-only field. CCCD image is not present in user_documents.document_type.
```

---

## Export rules

For CSV/TXT export, frontend can use `Blob`.

For PDF export:

- use print-friendly HTML with `window.print()`
- or mock backend API
- do not add third-party PDF libraries unless explicitly allowed

Use `export_jobs` for admin export workflow if implementing persisted export status.

---

## Accessibility rules

Minimum requirements:

- Buttons must be real `<button>` elements.
- Inputs must have labels.
- Modals must have close buttons.
- Escape key should close modals where practical.
- Backdrop click can close modal if safe.
- Focus states must be visible.
- Do not use color alone to communicate status; include text labels.

---

## Responsive rules

Support:

- desktop
- tablet
- mobile

Behavior:

- sidebars can collapse on smaller screens
- tables can become cards or horizontally scroll
- forms should become one column on mobile
- chatbot should become full-width bottom sheet on mobile
- sticky summary cards should become normal flow on mobile

---

## Performance rules

Keep implementation lightweight:

- No framework
- No large libraries
- Avoid unnecessary DOM re-rendering
- Use event delegation for table/list actions where useful
- Keep mock data in one shared file
- Reuse layout helpers

---

## Final reporting rule

After completing a task, always report:

1. Files created
2. Files modified
3. Pages implemented
4. Known UI-only fields not present in DB
5. How to run/test

Do not claim backend integration is complete unless backend code exists.

---

## Final quality bar

The result must feel like a polished Notion-style SaaS product, not a generic student admin template.

The code must be clean, plain HTML/CSS/JS, and database-aligned.
