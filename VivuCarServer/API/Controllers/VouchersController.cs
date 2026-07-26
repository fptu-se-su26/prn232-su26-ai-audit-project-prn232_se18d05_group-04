using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/vouchers")]
[AllowAnonymous]
public class VouchersController(IBookingRepository bookingRepo) : ControllerBase
{
    [HttpGet("check")]
    public async Task<ActionResult> Check(
        [FromQuery] string code,
        [FromQuery] decimal orderAmount = 0m,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Ok(new { valid = false, message = "Vui lòng nhập mã giảm giá." });

        var voucher = await bookingRepo.GetVoucherByCodeAsync(code.Trim(), cancellationToken);

        if (voucher == null)
            return Ok(new { valid = false, message = "Mã giảm giá không tồn tại." });

        if (voucher.ExpiresAt.HasValue && voucher.ExpiresAt.Value < DateTime.UtcNow)
            return Ok(new { valid = false, message = "Mã giảm giá đã hết hạn." });

        if (voucher.Quantity <= voucher.BookingVouchers.Count)
            return Ok(new { valid = false, message = "Mã giảm giá đã được sử dụng hết." });

        if (orderAmount > 0 && orderAmount < voucher.MinOrderAmount)
            return Ok(new { valid = false, message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0}₫ để sử dụng mã này." });

        string discountLabel;
        if (voucher.DiscountType == DiscountType.Percentage)
        {
            discountLabel = $"Giảm {voucher.DiscountValue}%";
            if (voucher.MaxDiscount > 0)
                discountLabel += $" (tối đa {voucher.MaxDiscount:N0}₫)";
        }
        else
        {
            discountLabel = $"Giảm {voucher.DiscountValue:N0}₫";
        }

        return Ok(new
        {
            valid = true,
            name = voucher.Name,
            code = voucher.Code,
            discountType = voucher.DiscountType.ToString(),
            discountValue = voucher.DiscountValue,
            maxDiscount = voucher.MaxDiscount,
            minOrderAmount = voucher.MinOrderAmount,
            discountLabel,
            message = "Mã giảm giá hợp lệ."
        });
    }
}
