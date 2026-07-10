# Phase 02 - Admin User Account Management

## Muc tieu
Hoan thien cum quan ly tai khoan admin sau khi auth guard da on dinh. Cum nay gom danh sach user, filter, pagination, xem trang thai khoa/mo khoa va thao tac lock/unlock.

## Nguon da doc
- `draw-db.sql`: bang `users`
- `specs/mb1-admin/02-admin-user-account-management.md`
- `VivuCarClient/WebClient/wwwroot/js/admin-users.js`
- `pages/admin-users.html`
- `VivuCarClient/WebClient/wwwroot/js/db-mock.js`
- `VivuCarServer/API/Controllers/AdminUsersController.cs`
- `VivuCarServer/Services/Implementations/AdminUserService.cs`

## Phan bien thuc te
- Spec co cot so dien thoai, nhung `draw-db.sql.users` khong co `phone`. Khong dua vao persisted UI neu khong danh dau UI-only.
- Spec dung `status: ACTIVE/BLOCKED`, nhung schema dung `is_blocked` boolean. FE hien tai da dung `is_blocked`, nen giu cach nay.
- Backend code-first hien tai dung `UserStatus.Active/Locked`, khac `draw-db.sql`. Khi FE noi backend, can map `Locked` -> `is_blocked = true`.

## Pham vi lam trong phase
1. Data contract
   - Dung cac field DB:
     - `id`
     - `email`
     - `full_name`
     - `role`
     - `is_blocked`
     - `created_at`
   - Role hop le:
     - `user`
     - `car_owner`
     - `admin`

2. Danh sach va filter
   - Search theo `full_name`, `email`.
   - Filter role theo enum DB.
   - Filter trang thai theo `is_blocked`.
   - Pagination client-side voi page size 10/20/50 khi dung mock.

3. Lock/unlock
   - Khong cho admin khoa tai khoan dang dang nhap.
   - Khi lock:
     - set `is_blocked = true` trong mock.
     - backend tuong ung: `PATCH /api/admin/users/{userId}/lock`.
   - Khi unlock:
     - set `is_blocked = false` trong mock.
     - backend tuong ung: `PATCH /api/admin/users/{userId}/unlock`.
   - Hien toast thanh cong/that bai.

4. UI states
   - Loading state khi fetch.
   - Empty state khi filter khong co ket qua.
   - Error state khi API fail.
   - Modal confirm co close button, Escape, backdrop.

## Khong lam trong phase nay
- Khong them phone/address vao bang users.
- Khong lam chuc nang booking restriction cho blocked user o cac module khac; chi ghi dependency de phase booking/user xu ly.
- Khong lam bulk action neu spec khong yeu cau ro.

## Tieu chi kiem tra
- Filter role/trang thai dung voi mock data.
- Khong hien/cap nhat fake `status` persisted tren user.
- Lock/unlock cap nhat table va badge ngay.
- Tu khoa chinh admin dang dang nhap bi chan.
- Khi backend tra `Active/Locked`, adapter map ve UI dung.

---

## Chi tiet bo sung - API endpoints

### Existing backend endpoints
| Method | Endpoint | Auth | Muc dich | Response/Body |
| --- | --- | --- | --- | --- |
| `GET` | `/api/admin/users` | Admin | Lay danh sach user | Backend hien tai tra list `AdminUserResponse`. |
| `PATCH` | `/api/admin/users/{userId}/lock` | Admin | Khoa tai khoan | `204 NoContent`; `400` neu khoa chinh minh; `404` neu khong thay. |
| `PATCH` | `/api/admin/users/{userId}/unlock` | Admin | Mo khoa tai khoan | `204 NoContent`; `404` neu khong thay. |

### Planned endpoints neu can nang cap
| Method | Endpoint | Status | Muc dich |
| --- | --- | --- | --- |
| `GET` | `/api/admin/users?page=1&pageSize=10&role=admin&isBlocked=false&keyword=a` | planned | Server-side pagination/filter. Hien backend moi co get all. |
| `GET` | `/api/admin/users/{userId}` | planned | Xem chi tiet user. |

## Chi tiet bo sung - API adapter contract

### Backend hien tai
```json
{
  "id": 1,
  "email": "admin@vivucar.vn",
  "fullName": "Admin",
  "role": "Admin",
  "status": "Active",
  "createdAt": "2026-06-01T00:00:00Z"
}
```

### FE/schema-aligned model
```js
{
  id: 1,
  email: "admin@vivucar.vn",
  full_name: "Admin",
  role: "admin",
  is_blocked: false,
  created_at: "2026-06-01T00:00:00Z"
}
```

## Chi tiet bo sung - Ham FE se tao/sua

### `VivuCarClient/WebClient/wwwroot/js/admin-users.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `mapAdminUserResponse(item)` | backend/mock item | schema-aligned user | Map `fullName/full_name`, `Role`, `Status/Locked` ve `role/is_blocked`. |
| `fetchUsers()` | none | Promise<user[]> | Goi `/api/admin/users`; fallback mock `DB.users` neu chua tich hop. |
| `getFilteredUsers(users, state)` | users, state | user[] | Filter keyword, role, blocked. |
| `renderUsers(users)` | user[] | void | Render table, empty state, pagination info. |
| `renderUserRow(user)` | user | HTML string | Tao row, badge, action buttons. |
| `renderUserPagination(totalItems)` | number | void | Render page numbers, prev/next disabled state. |
| `applyUserFilters()` | none | void | Doc toolbar, reset page=1, render. |
| `resetUserFilters()` | none | void | Clear inputs/selects, render lai. |
| `openUserStatusModal(userId, action)` | id, `lock/unlock` | void | Set selected user, title/body, reason visibility. |
| `closeUserStatusModal()` | none | void | Reset selected user va dong modal. |
| `confirmUserStatusChange()` | none | Promise<void> | Goi lock/unlock endpoint hoac cap nhat mock. |
| `canLockUser(user)` | user | boolean | Chan khoa chinh current admin. |
| `setUsersLoading(isLoading)` | boolean | void | Loading skeleton/disabled filter. |
| `showUsersError(message)` | string | void | Hien inline error hoac toast. |

## Chi tiet bo sung - State de dung
```js
const state = {
  users: [],
  keyword: "",
  role: "",
  blocked: "",
  page: 1,
  pageSize: 10,
  loading: false,
  error: "",
  selectedUserId: null,
  selectedAction: null
};
```

## Chi tiet bo sung - File thay doi du kien
| File | Loai thay doi |
| --- | --- |
| `pages/admin-users.html` | Bo sung loading/error region neu thieu; dam bao label input. |
| `VivuCarClient/WebClient/wwwroot/js/admin-users.js` | Tach function, them API adapter, normalize response. |
| `VivuCarClient/WebClient/wwwroot/js/constants.js` | Neu can them map backend role/status, nhung khong them enum DB moi. |

---

## Review bo sung - viec can chot truoc khi implement

Plan 02 da du de code phan danh sach va lock/unlock user, nhung can lam ro cac diem sau:

### 1. Ly do khoa tai khoan
Spec co textarea ly do khoa, nhung `draw-db.sql.users` khong co `blocked_reason`, backend `AdminUserService.LockAsync` cung khong nhan reason. Vi vay:
- Co the hien textarea UI-only, khong gui backend.
- Hoac bo textarea de dung schema.
- Neu giu textarea, them comment trong JS:
```js
// UI-only field. Lock reason is not present in current users schema. Requires migration before backend integration.
```
Khuyen nghi: giu modal confirm don gian, bo reason khoi payload trong phase 02.

### 2. Response adapter phai xu ly casing
`AdminUserResponse` co the serialize thanh camelCase. Adapter nen chap nhan:
- `fullName`, `FullName`, `full_name`
- `createdAt`, `CreatedAt`, `created_at`
- `status`, `Status`
- `role`, `Role`

### 3. Self-lock check
Khong chi check FE. Khi goi backend, backend da tra `400` cho self-lock. FE can hien thong bao rieng cho `400`, khong gom chung thanh loi server.

### 4. Pagination source
Backend hien tai `GET /api/admin/users` tra toan bo list, chua server pagination. Phase 02 nen lam client-side pagination truoc. Endpoint co query pagination chi la planned.

### 5. Test cases can them
- User response `status = "Locked"` map thanh `is_blocked = true`.
- User response `role = "CarOwner"` map thanh `car_owner`.
- Self-lock bi chan truoc khi mo modal hoac khi confirm.
- API lock tra `204` thi refetch hoac update local state.
- API lock tra `400` hien message "Khong the khoa tai khoan dang dang nhap".

## Ket luan review phase 02
Co the implement sau phase 01. Viec can chot duy nhat: co giu textarea ly do khoa tai khoan hay bo de khop schema. Khuyen nghi bo payload reason, neu UI van hien thi thi danh dau UI-only.
