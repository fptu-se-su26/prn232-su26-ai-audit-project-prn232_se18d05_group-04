(function () {
  const DB = window.VivuCarDB, U = window.VivuCarUtils, Auth = window.VivuCarAuth;
  const currentUser = Auth.getCurrentUser(); if (!currentUser) return;
  const params = new URLSearchParams(location.search);
  const bookingId = Number(params.get("bookingId"));
  const booking = DB.bookings.find((item) => item.id === bookingId && item.user_id === currentUser.id);
  const car = DB.cars.find((item) => item.id === (Number(params.get("carId")) || booking?.car_id));
  const root = U.byId("postTripReviewRoot");
  let rating = 0;

  function render() {
    document.getElementById("breadcrumbMount").innerHTML = window.VivuCarLayout.renderBreadcrumb([{ label: "Đơn thuê", href: "/Booking/MyBookings" }, { label: "Đánh giá" }]);
    if (!booking || !car) return root.innerHTML = U.renderEmptyState({ title: "Không tìm thấy chuyến đi", href: "/Booking/MyBookings", action: "Về đơn thuê" });
    if (booking.status !== "completed") return root.innerHTML = U.renderEmptyState({ title: "Chưa thể đánh giá", text: "Chỉ đánh giá khi booking.status = completed.", href: `/Booking/Detail?bookingId=${booking.id}`, action: "Xem đơn" });
    if (DB.reviews.some((item) => item.booking_id === booking.id && item.reviewer_id === currentUser.id)) return root.innerHTML = U.renderEmptyState({ title: "Bạn đã đánh giá chuyến này", href: "my-reviews.html", action: "Quản lý đánh giá" });
    root.innerHTML = `<div class="checkout-layout"><form class="checkout-main" id="reviewForm"><section class="checkout-section"><h1>Đánh giá sau chuyến</h1><div class="checkout-car"><img src="${U.carImage(car.id)}" alt="${U.carTitle(car)}"><div><h2>${U.carTitle(car)}</h2><p class="muted">#${booking.id} · ${U.formatDateTime(booking.return_datetime)}</p></div></div></section><section class="checkout-section"><h2>Rating</h2><div class="rating-picker" id="ratingPicker">${[1,2,3,4,5].map((n)=>`<button type="button" data-rate="${n}">★</button>`).join("")}</div><!-- UI-only field. Not present in current DB schema. Requires migration before backend integration. --><p class="muted">Tiêu chí phụ như cleanliness/performance chỉ hiển thị UI, DB reviews chỉ lưu rating/comment.</p></section><section class="checkout-section"><h2>Nhận xét</h2><textarea id="comment" rows="5" placeholder="Xe có sạch, vận hành tốt, đúng mô tả không?"></textarea><input id="reviewImages" type="file" accept="image/*" multiple><div class="image-preview-grid" id="reviewPreview"></div></section><button class="btn btn-primary btn-full" type="submit">Gửi đánh giá</button></form><aside class="summary-card"><h2>Điều kiện</h2><p class="muted">Rating từ 1 đến 5. Không cho review trùng booking.</p></aside></div>`;
    document.querySelectorAll("[data-rate]").forEach((button) => button.onclick = () => { rating = Number(button.dataset.rate); paintStars(); });
    U.byId("reviewImages").onchange = preview;
    U.byId("reviewForm").onsubmit = submit;
  }
  function paintStars(){ document.querySelectorAll("[data-rate]").forEach((button)=>button.classList.toggle("active", Number(button.dataset.rate) <= rating)); }
  function preview(event){ [...event.target.files].slice(0,5).forEach((file)=>{ const reader = new FileReader(); reader.onload=()=>U.byId("reviewPreview").insertAdjacentHTML("beforeend", `<img src="${reader.result}" alt="Ảnh review">`); reader.readAsDataURL(file); }); }
  function submit(event){ event.preventDefault(); const comment = U.byId("comment").value.trim(); if (!rating) return U.renderToast("Vui lòng chọn rating.", "danger"); if (comment && comment.length < 10) return U.renderToast("Nhận xét tối thiểu 10 ký tự.", "danger"); DB.reviews.push({ id: Math.max(0,...DB.reviews.map((item)=>item.id))+1, booking_id: booking.id, reviewer_id: currentUser.id, car_id: car.id, rating, comment, created_at: new Date().toISOString() }); window.VivuCarSaveDB(); U.renderToast("Đã gửi đánh giá.", "success"); setTimeout(()=>location.href=`/Booking/Detail?bookingId=${booking.id}`, 400); }
  render();
})();

