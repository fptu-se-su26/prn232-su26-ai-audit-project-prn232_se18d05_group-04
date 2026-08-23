# JWT Authentication Implementation Plan

## 1. Mục tiêu

Triển khai xác thực JWT cho VivuCar với các chức năng:

- Đăng nhập bằng email và password.
- Access token có thời hạn ngắn.
- Refresh token rotation.
- Logout thu hồi refresh token ngay; access token hết hiệu lực theo thời hạn hoặc cơ chế revoke tùy chọn.
- Phân quyền theo `Customer`, `CarOwner`, `Admin`.
- Từ chối truy cập khi tài khoản bị khóa.
- Thu hồi phiên đăng nhập khi logout, đổi password, logout-all hoặc Admin khóa tài khoản.

## 2. Quyết định kiến trúc

Project hiện dùng entity `AppUser` tự thiết kế, không dùng ASP.NET Core Identity.

Vì vậy:

- Không dùng `UserManager<AppUser>`.
- Dùng `VivuCarDbContext` hoặc repository để tìm user.
- Dùng `IPasswordHasher<AppUser>` để hash và verify password.
- Dùng trường `AppUser.Role` để tạo role claim.
- Role hợp lệ được khai báo tập trung bằng `AppRoles`: `Customer`, `CarOwner`, `Admin`.
- Access token lưu ở frontend memory.
- Refresh token lưu trong HttpOnly cookie.
- Database chỉ lưu hash của refresh token.
- Access token có thời hạn cố định 15 phút.
- `AppUser.TokenVersion` dùng để thu hồi toàn bộ token khi khóa user, đổi password hoặc logout-all.
- Access-token blacklist là tùy chọn, chỉ dùng khi logout phải vô hiệu hóa access token ngay.
- Nếu bật blacklist, chỉ lưu `jti` và ưu tiên cache bằng Redis hoặc `IMemoryCache`.

## 3. Phase 1 - Chuẩn hóa password

### Công việc backend

- Cài hoặc sử dụng package Identity Core có `IPasswordHasher<TUser>`.
- Đăng ký `IPasswordHasher<AppUser>` vào dependency injection.
- Bỏ user `HasData` có password placeholder.
- Runtime seed Admin, Customer và CarOwner mặc định từ biến môi trường.
- Không lưu plain password trong database, migration hoặc source code.
- Chuẩn hóa email bằng `Trim()` và `ToLowerInvariant()` trước khi tìm kiếm.
- Thêm `TokenVersion` vào `AppUser`, giá trị mặc định là `1`.
- Tạo constants dùng chung cho role, không hard-code role string rải rác.

### File dự kiến

```text
BusinessObjects/Security/AppRoles.cs
BusinessObjects/Models/AppUser.cs
API/Configurations/AuthenticationConfiguration.cs
API/HostedServices/UserSeedHostedService.cs
```

### Runtime seed tài khoản mặc định

Các biến môi trường:

```env
SEED_ADMIN_EMAIL=admin@vivucar.local
SEED_ADMIN_PASSWORD=change_this_password
SEED_ADMIN_FULL_NAME=System Admin
SEED_ADMIN_PHONE_NUMBER=0900000001

SEED_CUSTOMER_EMAIL=customer01@vivucar.local
SEED_CUSTOMER_PASSWORD=change_this_password
SEED_CUSTOMER_FULL_NAME=Default Customer
SEED_CUSTOMER_PHONE_NUMBER=0900000002

SEED_CAR_OWNER_EMAIL=owner01@vivucar.local
SEED_CAR_OWNER_PASSWORD=change_this_password
SEED_CAR_OWNER_FULL_NAME=Default Car Owner
SEED_CAR_OWNER_PHONE_NUMBER=0900000003
```

Hosted service chỉ tạo từng tài khoản nếu email chưa tồn tại. Password được hash bằng `PasswordHasher<AppUser>` trước khi lưu. App restart không reset password đã hợp lệ.

### Tiêu chí hoàn thành

- Password không xuất hiện dạng plain text trong bảng `Users`.
- `VerifyHashedPassword` trả về `Success` với tài khoản test.
- User sai password bị từ chối.
- User bị khóa không thể đăng nhập.
- Role authorization dùng `AppRoles`.
- Các runtime seed account có `TokenVersion` hợp lệ.

## 4. Phase 2 - JWT configuration và token generation

### Package cần thêm

```powershell
dotnet add API package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.27
```

### Biến môi trường cần thêm

```env
JWT_ISSUER=VivuCarServer
JWT_AUDIENCE=VivuCarClient
JWT_SECRET_KEY=replace_with_a_long_random_secret_key
JWT_ACCESS_TOKEN_MINUTES=15
JWT_REFRESH_TOKEN_DAYS=7
```

Secret key production phải đủ dài, ngẫu nhiên và không commit lên Git.

### Claims của access token

```text
sub   AppUser.Id
email AppUser.Email
role  Customer | CarOwner | Admin
token_version Phiên bản token của user
jti   ID duy nhất của access token
iat   Thời điểm phát hành
exp   Thời điểm hết hạn
iss   Issuer
aud   Audience
```

### Service cần tạo

```text
Services/Interfaces/ITokenService.cs
Services/Implementations/JwtTokenService.cs
Services/Models/Auth/AccessTokenResult.cs
API/Configurations/JwtConfiguration.cs
```

### Tiêu chí hoàn thành

- Tạo được access token có chữ ký hợp lệ.
- Token chứa đúng user ID, email và role.
- Token chứa `token_version` khớp với user.
- Token hết hạn theo cấu hình.
- Secret không nằm trực tiếp trong source code.

## 5. Phase 3 - Login endpoint

### DTO cần tạo

```text
Services/Models/Auth/LoginRequest.cs
Services/Models/Auth/LoginResponse.cs
Services/Models/Auth/AuthenticatedUserResponse.cs
```

### Endpoint

```http
POST /api/auth/login
```

Request:

```json
{
  "email": "customer01@vivucar.local",
  "password": "Customer123!"
}
```

Response:

```json
{
  "accessToken": "eyJhbGciOi...",
  "expiresAt": "2026-06-06T10:30:00Z",
  "user": {
    "id": 2,
    "email": "customer01@vivucar.local",
    "fullName": "Nguyen Van An",
    "role": "Customer"
  }
}
```

### Luồng xử lý

1. Validate request.
2. Normalize email.
3. Tìm user theo email.
4. Kiểm tra `UserStatus`.
5. Verify password hash.
6. Tạo access token.
7. Tạo refresh token.
8. Lưu hash refresh token vào DB.
9. Đặt refresh token vào HttpOnly cookie.
10. Trả access token và thông tin user.

### Quy tắc bảo mật

- Email không tồn tại và password sai đều trả cùng một response `401`.
- Không trả `PasswordHash`.
- Không ghi password hoặc token vào log.
- Có rate limit cho endpoint login.

### Tiêu chí hoàn thành

- User hợp lệ đăng nhập thành công.
- Password sai trả `401`.
- User không tồn tại trả `401`.
- User `Locked` bị từ chối.

## 6. Phase 4 - JWT validation và authorization

### Cấu hình backend

Đăng ký:

```csharp
services.AddAuthentication(...)
    .AddJwtBearer(...);
```

Validation bắt buộc:

- `ValidateIssuer`
- `ValidateAudience`
- `ValidateLifetime`
- `ValidateIssuerSigningKey`
- Kiểm tra signing key.
- Clock skew nhỏ, ví dụ 30 giây.

### Middleware

Thứ tự bắt buộc:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### Endpoint kiểm thử

```http
GET /api/auth/me
```

Phân quyền:

```csharp
[Authorize]
[Authorize(Roles = AppRoles.Customer)]
[Authorize(Roles = AppRoles.CarOwner)]
[Authorize(Roles = AppRoles.Admin)]
[Authorize(Roles = AppRoles.Admin + "," + AppRoles.CarOwner)]
```

### Tiêu chí hoàn thành

- Không có token trả `401`.
- Token sai hoặc hết hạn trả `401`.
- Token hợp lệ truy cập được `/api/auth/me`.
- Sai role trả `403`.
- `token_version` sai bị từ chối.

## 7. Phase 5 - Refresh token

### Entity RefreshToken

Thuộc tính đề xuất:

```text
Id
UserId
TokenHash
ExpiresAt
CreatedAt
RevokedAt
ReplacedByTokenHash
CreatedByIp
RevokedByIp
```

### Database rules

- Unique index cho `TokenHash`.
- Index cho `UserId`.
- Không lưu refresh token thô.
- Quan hệ `AppUser` 1-n `RefreshToken`.

### Endpoint

```http
POST /api/auth/refresh
```

### Cookie

```text
HttpOnly = true
Secure = true
Expires = refresh token expiration
```

Chọn `SameSite` theo cách deploy:

- Frontend và API cùng site: `Strict` hoặc `Lax`.
- Frontend và API cross-site: `None` và bắt buộc `Secure = true`.
- Nếu khác origin, frontend dùng `withCredentials: true` và backend cấu hình CORS credentials.

### Refresh token rotation

1. Đọc refresh token từ cookie.
2. Hash token.
3. Tìm token trong database.
4. Kiểm tra token chưa hết hạn và chưa revoke.
5. Kiểm tra user vẫn `Active`.
6. Revoke token cũ.
7. Tạo access token mới.
8. Tạo refresh token mới.
9. Lưu token mới và liên kết token thay thế.
10. Cập nhật cookie.

### Phát hiện refresh token reuse

Nếu một refresh token đã bị revoke do rotation nhưng vẫn được gửi lại:

1. Coi đây là dấu hiệu token bị đánh cắp.
2. Revoke toàn bộ refresh token còn hoạt động của user.
3. Tăng `AppUser.TokenVersion`.
4. Ghi security log, nhưng không ghi token thô.
5. Xóa refresh-token cookie.
6. Trả `401` và yêu cầu đăng nhập lại.

### Tiêu chí hoàn thành

- Refresh token hợp lệ cấp access token mới.
- Refresh token cũ không dùng lại được.
- Refresh token reuse thu hồi toàn bộ phiên của user.
- Token hết hạn hoặc revoked trả `401`.
- User bị khóa không thể refresh.

## 8. Phase 6 - Logout, TokenVersion và blacklist tùy chọn

### Cơ chế mặc định

- Logout luôn revoke refresh token hiện tại.
- Frontend luôn xóa access token khỏi memory.
- Access token còn lại tự hết hạn sau 15 phút.
- Không query bảng blacklist trên mọi request.

### TokenVersion

Access token chứa claim `token_version`.

Tăng `AppUser.TokenVersion` khi:

- Admin khóa tài khoản.
- User đổi password.
- User chọn logout all devices.
- Phát hiện refresh token reuse.

Khi validate token, so sánh claim với phiên bản hiện tại của user. Để tránh query DB tốn kém trên mỗi request, cache trạng thái `{ UserId, Status, TokenVersion }` với TTL ngắn:

- Dùng `IMemoryCache` nếu API chỉ chạy một instance.
- Dùng Redis nếu API chạy nhiều instance.

### Blacklist access token tùy chọn

Chỉ bật nếu yêu cầu nghiệp vụ bắt buộc logout phải vô hiệu hóa access token hiện tại ngay lập tức.

### Entity RevokedToken

```text
Id
Jti
ExpiresAt
CreatedAt
```

Database rules:

- Unique index cho `Jti`.
- Index cho `ExpiresAt`.
- Không lưu toàn bộ JWT.
- Cache danh sách revoke bằng Redis hoặc `IMemoryCache`.

### Endpoint

```http
POST /api/auth/logout
```

Logout không bắt buộc access token còn hiệu lực. Backend ưu tiên đọc refresh-token cookie để revoke phiên hiện tại.

### Luồng logout

1. Đọc refresh token từ cookie.
2. Nếu tìm thấy, revoke refresh token hiện tại.
3. Nếu access token còn hợp lệ và blacklist được bật, lưu `jti` cùng `exp`.
4. Nếu access token đã hết hạn, logout vẫn tiếp tục bình thường.
5. Xóa refresh-token cookie.
6. Trả `204 No Content`.
7. Frontend xóa access token khỏi memory.

### Kiểm tra blacklist

Trong `JwtBearerEvents.OnTokenValidated`:

- Lấy `jti`.
- Kiểm tra cache trước, database khi cần.
- Nếu tồn tại thì gọi `context.Fail(...)`.
- Kiểm tra user vẫn `Active` và `token_version` còn khớp.

### Tiêu chí hoàn thành

- Refresh token vừa logout không dùng lại được.
- Logout vẫn thành công khi access token đã hết hạn.
- Nếu bật blacklist, access token vừa logout không dùng lại được.
- Logout nhiều lần không gây lỗi hệ thống.

## 9. Phase 7 - Admin khóa và mở khóa tài khoản

### Khi khóa tài khoản

1. Đổi `AppUser.Status` thành `Locked`.
2. Revoke tất cả refresh token của user.
3. Tăng `AppUser.TokenVersion`.
4. Invalidate cache trạng thái user.
5. Các request tiếp theo bị từ chối trong token validation.
6. User không thể login hoặc refresh.

### Khi mở khóa

1. Đổi trạng thái về `Active`.
2. Không khôi phục refresh token cũ.
3. User phải đăng nhập lại.

### Tiêu chí hoàn thành

- User bị khóa mất quyền truy cập ngay.
- User mở khóa phải login lại.
- API Admin được bảo vệ bằng `AppRoles.Admin`.

## 10. Phase 8 - Frontend integration

### Login

- Gửi email/password tới `/api/auth/login`.
- Lưu access token trong memory hoặc auth store.
- Không lưu refresh token bằng JavaScript.

### API requests

Thêm header:

```http
Authorization: Bearer <access-token>
```

Axios cần:

```javascript
withCredentials: true
```

nếu frontend và backend khác origin và dùng refresh-token cookie.

### Xử lý lỗi

- `401`: thử refresh token một lần.
- Refresh thành công: gọi lại request ban đầu.
- Refresh thất bại: xóa auth state và chuyển về login.
- `403`: hiển thị thông báo không đủ quyền.

### Logout

1. Gọi `/api/auth/logout`.
2. Xóa access token khỏi memory.
3. Xóa thông tin user trong auth store.
4. Chuyển về màn hình login.

### Tiêu chí hoàn thành

- Refresh request không chạy đồng thời nhiều lần.
- Không tạo vòng lặp refresh vô hạn.
- Reload trang có thể gọi refresh để lấy access token mới.

## 11. Phase 9 - Cleanup và background job

### Dữ liệu cần dọn

- Revoked token đã hết hạn nếu bật blacklist.
- Refresh token đã hết hạn.
- Refresh token revoked quá lâu.

### Cách triển khai

Tạo `BackgroundService` chạy định kỳ, ví dụ mỗi 24 giờ.

Pseudo query:

```sql
-- Chỉ dùng nếu bật access-token blacklist.
DELETE FROM RevokedTokens
WHERE ExpiresAt <= GETUTCDATE();

DELETE FROM RefreshTokens
WHERE ExpiresAt <= GETUTCDATE()
   OR RevokedAt IS NOT NULL;
```

### Tiêu chí hoàn thành

- Bảng token không tăng vô hạn.
- Cleanup không xóa token đang hoạt động.

## 12. Phase 10 - Testing

### Unit tests

- Hash và verify password.
- Tạo access token.
- Claims đúng.
- Refresh token hash.
- Refresh token rotation.
- Kiểm tra user locked.

### Integration tests

- Login thành công.
- Login sai password.
- Login user locked.
- Truy cập endpoint không có token.
- Truy cập đúng/sai role.
- Refresh token thành công.
- Refresh token reuse bị từ chối và revoke toàn bộ phiên.
- Logout và dùng lại access token.
- Logout khi access token đã hết hạn.
- Admin khóa user đang đăng nhập.
- Token có `token_version` cũ.

### Security tests

- JWT signature sai.
- Issuer sai.
- Audience sai.
- Token hết hạn.
- Token thiếu `jti`.
- Cookie không gửi qua HTTP production.
- Không lộ password, token hoặc secret trong log/response.

## 13. Thứ tự migration

### Migration 1

```text
AddAuthenticationTokens
```

Bao gồm:

- Bảng `RefreshTokens`.
- Cột `AppUser.TokenVersion`.
- Navigation property và index.
- Bảng `RevokedTokens` chỉ thêm nếu bật blacklist.

Lệnh:

```powershell
cd BusinessObjects
dotnet ef migrations add AddAuthenticationTokens --project . --startup-project ..\API --output-dir Migrations
dotnet ef database update --project . --startup-project ..\API
```

## 14. Cấu trúc file cuối cùng

```text
API
  Configurations
    AuthenticationConfiguration.cs
    JwtConfiguration.cs
  Controllers
    AuthController.cs
  HostedServices
    UserSeedHostedService.cs

BusinessObjects
  Security
    AppRoles.cs
  Models
    AppUser.cs
    RefreshToken.cs
    RevokedToken.cs (optional)
  Configurations
    RefreshTokenConfiguration.cs
    RevokedTokenConfiguration.cs (optional)
Repositories
  Interfaces
    IUserRepository.cs
    IRefreshTokenRepository.cs
    IRevokedTokenRepository.cs (optional)
  Implementations
    UserRepository.cs
    RefreshTokenRepository.cs
    RevokedTokenRepository.cs (optional)

Services
  Interfaces
    IAuthService.cs
    ITokenService.cs
  Implementations
    AuthService.cs
    JwtTokenService.cs
  Models
    Auth
      LoginRequest.cs
      LoginResponse.cs
      AccessTokenResult.cs
      AuthenticatedUserResponse.cs
```

## 15. Definition of Done

- Build thành công, không warning/error liên quan authentication.
- Migration áp dụng thành công lên SQL Server.
- Admin, Customer và CarOwner runtime seed lưu password hash hợp lệ.
- Login, refresh, logout và `/me` hoạt động.
- Logout revoke refresh token kể cả khi access token đã hết hạn.
- Refresh token rotation hoạt động.
- Refresh token reuse revoke toàn bộ phiên và tăng `TokenVersion`.
- Lock, đổi password và logout-all tăng `TokenVersion`.
- Role authorization trả đúng `401` và `403`.
- User bị khóa mất quyền truy cập ngay.
- Cookie `SameSite` phù hợp với mô hình deploy frontend/backend.
- Nếu bật blacklist, lookup được cache và access token logout bị vô hiệu hóa ngay.
- Frontend không lưu refresh token trong local storage.
- `.env` không được commit.
- JWT secret không xuất hiện trong source code hoặc log.
