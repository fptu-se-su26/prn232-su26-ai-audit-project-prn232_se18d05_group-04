# Admin BE/FE Integration Roadmap

Cap nhat: 2026-07-11.

## Trang thai hien tai

| Phase | Trang thai | Ghi chu |
| --- | --- | --- |
| 01 - Auth/session | Completed | Backend auth, refresh/logout, WebClient auth proxy va admin guard da co. |
| 02 - Admin users | Completed | List, lock/unlock va FE API client da noi backend. |
| 03 - Admin cars | Completed | Browser smoke test skipped by request. |
| 04 - Revenue dashboard | Completed | Snapshot model/migration, API, dashboard va tests da co. |
| 05 - Export reports | Completed | Preview, persisted jobs, CSV/PDF va download da co. |
| 06 - Vouchers | Completed | CRUD, validation, performance, migration va frontend API da hoan tat. |
| 07 - Hardening/QA | Next | San sang regression, accessibility, responsive va encoding cleanup. |

Phase 01-06 da duoc rut khoi backlog va luu tai `completed-phases.md`.

## Kien truc bat buoc

```text
Razor Pages/admin JS -> /api/proxy/api/... -> WebClient proxy -> API -> Service -> Repository/EF -> Database
```

- Khong hard-code API host trong JavaScript.
- Khong dung `VivuCarDB` lam happy path cho admin.
- UI-only field khong gui vao API.
- Khong claim completed neu endpoint frontend dang goi chua ton tai.

## Thu tu thuc hien

1. Phase 05: preview/export job lifecycle + download.
2. Phase 06: voucher CRUD + performance.
3. Phase 07: regression, accessibility, responsive, encoding va dependency cleanup.

## Definition of Done Phase 06

- Contract, DTO, controller, service va repository/query hoan tat.
- Endpoint co authorization Admin.
- Frontend goi API that qua proxy; co loading/empty/error/success.
- Test business rule quan trong va build server/client pass.
- Browser smoke test xac nhan request qua proxy.

## Verify 2026-07-11

- Server tests: 29 passed, 0 failed.
- API build pass; con 1 warning `CS1998` tai `BookingsController.cs`.
- WebClient build pass, 0 warning.
- Con 2 warning nullable `CS8602` trong `BookingRepository.cs` khi chay test.
- Browser E2E duoc bo qua theo yeu cau; Phase 03-05 dong o muc code/test/build.

