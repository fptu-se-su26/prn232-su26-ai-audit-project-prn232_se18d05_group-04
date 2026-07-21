(function () {
  const U = window.VivuCarUtils;

  function rentalDays(pickup, returned) {
    const hours = Math.max(1, (new Date(returned).getTime() - new Date(pickup).getTime()) / 36e5);
    return Math.max(1, Math.ceil(hours / 24));
  }

  function rentalHours(pickup, returned) {
    return Math.max(1, Math.ceil((new Date(returned).getTime() - new Date(pickup).getTime()) / 36e5));
  }

  function usedCount(voucherId) {
    return window.VivuCarDB.voucher_usages.filter((usage) => usage.voucher_id === Number(voucherId)).length;
  }

  function calculateVoucher(voucher, subtotal) {
    if (!voucher) return { discount: 0, error: "" };
    if (new Date(voucher.expires_at).getTime() < Date.now()) return { discount: 0, error: "Voucher đã hết hạn." };
    if (usedCount(voucher.id) >= voucher.quantity) return { discount: 0, error: "Voucher đã hết lượt sử dụng." };
    if (subtotal < voucher.min_order_amount) return { discount: 0, error: `Đơn tối thiểu ${U.formatVnd(voucher.min_order_amount)}.` };
    const raw = voucher.discount_type === "percentage" ? subtotal * voucher.discount_value / 100 : voucher.discount_value;
    return { discount: Math.min(raw, voucher.max_discount || raw), error: "" };
  }

  function calculate(car, pickup, returned, voucher) {
    const hours = rentalHours(pickup, returned);
    const days = rentalDays(pickup, returned);
    const subtotal = hours <= 8 ? hours * Number(car.price_per_hours || 0) : days * Number(car.price_per_day || 0);
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    const insuranceFee = Math.round(subtotal * 0.06);
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    const deliveryFee = 0;
    const voucherResult = calculateVoucher(voucher, subtotal + insuranceFee + deliveryFee);
    const total = Math.max(0, subtotal + insuranceFee + deliveryFee - voucherResult.discount);
    return { hours, days, subtotal, insuranceFee, deliveryFee, discount: voucherResult.discount, voucherError: voucherResult.error, total };
  }

  window.VivuCarBookingPricing = { calculate, calculateVoucher, rentalDays, rentalHours, usedCount };
})();

