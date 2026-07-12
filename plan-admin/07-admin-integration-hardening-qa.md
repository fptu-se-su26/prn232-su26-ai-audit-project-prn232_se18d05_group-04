# Phase 07 - Admin Hardening and QA

Trang thai: chua bat dau. Khong dung phase nay de bu endpoint cua Phase 04-06.

## Dieu kien bat dau

- [x] Phase 03 da dong; browser smoke test skipped by request.
- [x] Revenue endpoint va dashboard da hoan tat; browser smoke test skipped by request.
- [x] Export job lifecycle da hoan tat; browser smoke test skipped by request.
- [x] Voucher CRUD/performance da hoan tat; browser smoke test skipped by request.

## QA

- [ ] Tat mock fallback bang QA flag va test tat ca admin page qua WebClient proxy.
- [ ] Kiem tra 400/401/403/404/409/500 va message phu hop.
- [ ] Regression auth, users, cars, revenue, export, vouchers.
- [ ] Responsive desktop/tablet/mobile 375px.
- [ ] Keyboard, focus, label, modal Escape/backdrop va status text.
- [ ] Sua mojibake va bao dam UTF-8.
- [ ] Loai code/frontend asset khong dung; danh gia Bootstrap/jQuery/Tailwind xung dot `AGENTS.md`.
- [ ] Sua warning nullable trong `BookingRepository.cs` va async warning trong `BookingsController.cs` neu con.
- [ ] Chay toan bo test/build va ghi ket qua browser Network.

## Definition of Done

- Tat ca module admin dung API that qua proxy.
- Khong con endpoint frontend goi ma backend khong co.
- Test/build pass, browser QA desktop/mobile pass.
- Bao cao cuoi liet ke endpoint, page, known limitation va mock con lai.

