(function () {
  const blockingStatuses = ["pending", "approved"];

  function overlaps(startA, endA, startB, endB) {
    return new Date(startA).getTime() < new Date(endB).getTime() && new Date(endA).getTime() > new Date(startB).getTime();
  }

  function isValidBookingRange(pickup, returned) {
    const start = new Date(pickup).getTime();
    const end = new Date(returned).getTime();
    return Number.isFinite(start) && Number.isFinite(end) && end > start;
  }

  function checkAvailability(carId, pickup, returned, ignoreBookingId) {
    if (!isValidBookingRange(pickup, returned)) {
      return { available: false, reason: "Vui lòng chọn thời gian nhận và trả xe hợp lệ.", conflicts: [] };
    }
    // Backend must repeat this availability check inside a transaction/lock to avoid overselling.
    const conflicts = window.VivuCarDB.bookings.filter((booking) => {
      if (booking.car_id !== Number(carId) || booking.id === Number(ignoreBookingId)) return false;
      if (!blockingStatuses.includes(booking.status)) return false;
      return overlaps(pickup, returned, booking.pickup_datetime, booking.return_datetime);
    });
    return {
      available: conflicts.length === 0,
      reason: conflicts.length ? "Xe đã có người đặt trong khung giờ này." : "Xe còn trống trong khung giờ đã chọn.",
      conflicts
    };
  }

  window.VivuCarBookingAvailability = { checkAvailability, isValidBookingRange, overlaps };
})();

