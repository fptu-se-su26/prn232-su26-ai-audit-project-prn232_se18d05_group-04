# Phase 01 - Foundation, Auth va Admin Shell

## Muc tieu
Lam nen tang truoc khi dung tiep cac module admin. Phase nay khong nen nhay vao user/car/voucher ngay, vi FE hien co dang co lech role casing voi backend va dang dung Tailwind CDN, trai voi rule HTML/CSS/JS thuan trong `AGENTS.md`.

## Nguon da doc
- `AGENTS.md`
- `draw-db.sql`
- `CLAUDE.md`
- `pc.md`
- `specs/mb1-admin/01-login-role-routing.md`
- `specs/mb1-admin/03-logout-session-invalidation.md`
- `.agents/skills/gpt-taste/SKILL.md`
- `.agents/skills/design-taste-frontend/SKILL.md`
- `VivuCarClient/WebClient/wwwroot/js/auth.js`
- `VivuCarClient/WebClient/wwwroot/js/login.js`
- `VivuCarClient/WebClient/wwwroot/js/layout.js`
- `VivuCarServer/API/Controllers/AuthController.cs`
- `VivuCarServer/Services/Implementations/AuthService.cs`
- `VivuCarServer/Services/Implementations/JwtTokenService.cs`
- `VivuCarServer/BusinessObjects/Security/AppRoles.cs`
- `VivuCarServer/BusinessObjects/Enums/UserRole.cs`

## Phan bien thuc te
- Spec mau dung role uppercase `ADMIN`, `CAR_OWNER`, `CUSTOMER`, nhung `AGENTS.md` va FE mock dung `admin`, `car_owner`, `user`.
- Backend hien tai tra role enum `Admin`, `CarOwner`, `Customer`. Neu khong normalize, `Auth.roleRoutes[user.role]` co the sai.
- FE admin dang nap `https://cdn.tailwindcss.com` va `tailwind-ui.js`; neu bam chat rule du an thi phai thay bang CSS thuan.
- Backend auth dung refresh token HttpOnly cookie, FE chi luu access token trong localStorage. Plan FE khong duoc tu nhan da xu ly blacklist/refresh neu backend chua duoc goi dung.

## Pham vi lam trong phase
1. Chuan hoa auth client
   - Tao ham normalize role:
     - `Admin` -> `admin`
     - `CarOwner` -> `car_owner`
     - `Customer` -> `user`
   - Luu dung key hien co:
     - `vivucar_access_token`
     - `vivucar_current_user`
     - `vivucar_current_user_role`
   - Giu tuong thich voi mock data DB dang dung lowercase.

2. Login va role routing
   - `pages/login.html`
   - `VivuCarClient/WebClient/wwwroot/js/login.js`
   - `VivuCarClient/WebClient/wwwroot/js/auth.js`
   - Route:
     - `admin` -> `admin-dashboard.html`
     - `car_owner` -> `owner-booking-requests.html`
     - `user` -> `home.html`
   - Hien loading, error, disabled state.

3. Admin auth guard
   - `layout.js` phai chan admin page neu role khong phai `admin`.
   - Unauthorized nen redirect login hoac 403 rieng, nhung phai thong nhat mot cach.

4. Logout/session
   - Goi `POST /api/auth/logout` qua `fetchWithAuth`.
   - Xoa localStorage keys.
   - Dong/mo modal logout bang JS thuan, co Escape va backdrop.

5. Admin shell
   - Sidebar/header/breadcrumb/toast/modal dung lai tu `layout.js` va `utils.js`.
   - Giu active state theo `data-page`.
   - Neu sua UI, uu tien CSS thuan trong `VivuCarClient/WebClient/wwwroot/css/*`, khong mo rong Tailwind runtime.

## Khong lam trong phase nay
- Khong them CRUD user/car/voucher.
- Khong sua backend auth neu yeu cau hien tai chi la FE plan.
- Khong them thu vien UI moi.

## Tieu chi kiem tra
- Login bang admin redirect den `pages/admin-dashboard.html`.
- Login bang owner/user khong vao duoc admin page.
- Logout xoa token/user/role va ve login.
- Role tra ve tu backend dang `Admin/CarOwner/Customer` van chay dung sau normalize.
- Khong tao field/status moi ngoai schema.

## Ghi chu tich hop
Backend `AdminUsersController` dang authorize bang role `Admin`, trong khi FE noi bo dung `admin`. Can co layer normalize o FE, khong doi label DB mock tuy tien.

---

## Chi tiet bo sung - API endpoints

### Existing backend endpoints
| Method | Endpoint | Auth | Muc dich | Ghi chu |
| --- | --- | --- | --- | --- |
| `POST` | `/api/auth/login` | Public | Dang nhap email/password | Da co trong `AuthController`. Tra `accessToken`, `expiresAt`, `user`. |
| `POST` | `/api/auth/refresh` | Refresh cookie | Lay access token moi | Backend dung HttpOnly cookie `refreshToken`. Khi FE tich hop that can xu ly cookie/credential. |
| `POST` | `/api/auth/logout` | Optional access token + refresh cookie | Thu hoi refresh token hien tai | FE van xoa localStorage ke ca API fail. |
| `POST` | `/api/auth/logout-all` | Bearer token | Dang xuat tat ca phien | Chi dung neu UI co nut logout all. |
| `GET` | `/api/auth/me` | Bearer token | Kiem tra user hien tai | Huu ich cho admin guard/sync session. |
| `GET` | `/api/auth/admin-check` | Bearer token role Admin | Test quyen admin | Dung de verify guard neu can. |

## Chi tiet bo sung - Ham FE se tao/sua

### `VivuCarClient/WebClient/wwwroot/js/auth.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `normalizeRole(role)` | `string` | `user/car_owner/admin/null` | Map `Admin`, `CarOwner`, `Customer`, `ADMIN`, `CAR_OWNER`, `CUSTOMER` ve role FE lowercase. |
| `normalizeUser(rawUser)` | backend/mock user | user FE | Chuan hoa `id`, `email`, `full_name`, `role`, `avatar_url`. |
| `getCurrentUser()` | none | user/null | Doc `vivucar_current_user`, parse an toan. |
| `setAuthSession(data)` | login response | user | Luu `vivucar_access_token`, `vivucar_current_user`, `vivucar_current_user_role`. |
| `clearAuthSession()` | none | void | Xoa token/current user/role. |
| `login(email, password)` | string, string | normalized user | Goi `POST /api/auth/login`, normalize response, throw error co message. |
| `logout()` | none | Promise<void> | Goi `/api/auth/logout`, sau do clear session va redirect login. |
| `refreshSession()` | none | Promise<user/null> | Planned: goi `/api/auth/refresh` khi token het han hoac page load. |
| `requireAuth(allowedRoles)` | array role | user/null | Chan route, normalize role truoc khi compare. |
| `fetchWithAuth(url, options)` | url/options | Response | Gan bearer token, xu ly 401 bang clear session. |

### `VivuCarClient/WebClient/wwwroot/js/login.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `validateLoginForm(email, password)` | string, string | `{ valid, message }` | Check email format, password >= 6. |
| `showError(message)` | string | void | Hien error box. |
| `setLoginLoading(isLoading)` | boolean | void | Disable button, hien loading text. |
| `handleLoginSubmit(event)` | submit event | Promise<void> | Validate, goi `Auth.login`, redirect theo `Auth.roleRoutes`. |
| `fillDemoAccount(email)` | string | void | Neu giu demo account, dien email/password mock. |

### `VivuCarClient/WebClient/wwwroot/js/layout.js`
| Function | Input | Output | Trach nhiem |
| --- | --- | --- | --- |
| `renderAdminSidebar(activeKey)` | page key | HTML string | Sidebar admin. |
| `renderAdminHeader(currentUser)` | user | HTML string | Header admin, search, logout. |
| `renderLogoutModal()` | none | HTML string | Modal logout dung chung. |
| `bindLogoutHandlers()` | none | void | Mo/dong modal, Escape, backdrop. Nen tach de de test. |
| `initializeLayout()` | none | void | Determine layout, auth guard, mount sidebar/header/modal. |

## Chi tiet bo sung - Data mapping

### Login response backend hien tai
```json
{
  "accessToken": "jwt",
  "expiresAt": "2026-07-09T00:00:00Z",
  "user": {
    "id": 1,
    "email": "admin@vivucar.vn",
    "fullName": "Admin",
    "role": "Admin"
  }
}
```

### FE normalized user
```js
{
  id: 1,
  email: "admin@vivucar.vn",
  full_name: "Admin",
  role: "admin",
  avatar_url: null
}
```

## Chi tiet bo sung - File thay doi du kien
| File | Loai thay doi |
| --- | --- |
| `VivuCarClient/WebClient/wwwroot/js/auth.js` | Sua normalize role/user, session helpers, fetch auth. |
| `VivuCarClient/WebClient/wwwroot/js/login.js` | Tach validate/loading/submit functions. |
| `VivuCarClient/WebClient/wwwroot/js/layout.js` | Dam bao guard dung role normalized; tach bind logout neu can. |
| `pages/login.html` | Chi sua neu id/label/loading state thieu. |
| `VivuCarClient/WebClient/wwwroot/css/*.css` | Neu remove Tailwind CDN thi bo sung style thuan. |

---

## Review bo sung - viec can chot truoc khi implement

Plan 01 da du de bat dau, nhung can bo sung/lam ro cac diem sau de tranh loi tich hop:

### 1. Cookie refresh token va fetch credentials
Backend set refresh token bang HttpOnly cookie `refreshToken` voi `Path = /api/auth`. Neu FE chay khac origin voi API, cac request auth can xem xet:
```js
fetch(url, {
  credentials: "include"
});
```
Can kiem tra CORS backend co `AllowCredentials` va origin FE hop le hay khong. Neu khong, `/api/auth/refresh` va `/api/auth/logout` co the khong nhan cookie refresh token.

### 2. Auth response casing
Backend ASP.NET thuong serialize PascalCase thanh camelCase neu dung default web JSON options. FE adapter phai chap nhan ca hai dang:
- `accessToken` va `AccessToken`
- `expiresAt` va `ExpiresAt`
- `user.fullName` va `user.full_name` va `user.FullName`
- `user.role` va `user.Role`

### 3. Role route fallback
`Auth.roleRoutes[user.role]` can co fallback an toan:
```js
const route = roleRoutes[normalizedRole];
if (!route) throw new Error("Vai tro khong duoc ho tro.");
```
Khong redirect ve trang mac dinh neu role khong hop le, vi nhu vay che giau loi auth/permission.

### 4. Unauthorized behavior
Chot mot cach dung thong nhat:
- Chua dang nhap: redirect `login.html`.
- Da dang nhap nhung sai role: hien/redirect `403.html`.

Neu chua co `403.html`, trong phase 01 nen tao trang don gian hoac doi plan thanh redirect login co toast. Khuyen nghi tao `pages/403.html` de dung dung nghia phan quyen.

### 5. Tailwind CDN decision
Phase 01 khong nhat thiet phai remove Tailwind khoi toan bo admin ngay, vi se phong to scope. Tuy nhien can chot ro:
- Option A: lam nho, chi sua auth/session va de Tailwind cleanup sang Phase 07.
- Option B: remove Tailwind CDN cho login/admin shell ngay, nhung scope se lon hon.

Khuyen nghi Option A de phase 01 khong bi lan sang UI migration. Ghi technical debt trong Phase 07 la du.

### 6. Refresh-session behavior
Khong nen tu dong refresh phuc tap ngay neu backend/CORS cookie chua duoc test. Phase 01 nen lam toi thieu:
- Login luu access token/user.
- `fetchWithAuth` gap 401 thi clear session va ve login.
- `refreshSession()` de planned/stub, chi bat khi da verify cookie/CORS.

### 7. Test cases can them vao phase 01
- Backend tra role `Admin`, `CarOwner`, `Customer` deu normalize dung.
- Backend tra role uppercase `ADMIN`, `CAR_OWNER`, `CUSTOMER` van normalize dung.
- Backend tra response camelCase/PascalCase deu doc duoc.
- User da login role `user` mo `admin-dashboard.html` thi den `403.html` hoac login theo quy uoc da chot.
- `POST /api/auth/logout` fail van clear local session.
- `fetchWithAuth` gap `401` clear session va khong loop redirect.

## Ket luan review phase 01
Can thuc hien them truoc khi code: chot unauthorized behavior va chot co tao `403.html` trong phase 01 hay khong. Cac diem con lai co the implement truc tiep trong `auth.js`, `login.js`, `layout.js` ma khong can doi backend.
