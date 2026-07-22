using API.Models;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Admin;

namespace API.Controllers;

[ApiController]
[Route("api/admin/vouchers")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminVouchersController(IAdminVoucherService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminVoucherResponse>>> GetList(
        [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? keyword,
        [FromQuery(Name = "discount_type")] string? discountType, [FromQuery] string? status,
        [FromQuery] DateOnly? from, [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        if (from.HasValue && to.HasValue && from > to)
            return ApiErrorFactory.Error(HttpContext, 400, "Ngày bắt đầu không được sau ngày kết thúc.");

        return Ok(await service.GetListAsync(new AdminVoucherListQuery
        {
            Page = page <= 0 ? 1 : page,
            PageSize = pageSize <= 0 ? 10 : pageSize,
            Keyword = keyword,
            DiscountType = discountType,
            Status = status,
            From = from,
            To = to
        }, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminVoucherResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var voucher = await service.GetByIdAsync(id, cancellationToken);
        return voucher is null ? NotFoundError() : Ok(voucher);
    }

    [HttpPost]
    public async Task<ActionResult<AdminVoucherResponse>> Create(AdminVoucherUpsertRequest request, CancellationToken cancellationToken)
    {
        try { var voucher = await service.CreateAsync(request, cancellationToken); return CreatedAtAction(nameof(GetById), new { id = voucher.Id }, voucher); }
        catch (AdminVoucherServiceException ex) { return ServiceError(ex); }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AdminVoucherResponse>> Update(int id, AdminVoucherUpsertRequest request, CancellationToken cancellationToken)
    {
        try { var voucher = await service.UpdateAsync(id, request, cancellationToken); return voucher is null ? NotFoundError() : Ok(voucher); }
        catch (AdminVoucherServiceException ex) { return ServiceError(ex); }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try { return await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFoundError(); }
        catch (AdminVoucherServiceException ex) { return ServiceError(ex); }
    }

    [HttpGet("{id:int}/performance")]
    public async Task<ActionResult<AdminVoucherPerformanceResponse>> GetPerformance(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetPerformanceAsync(id, cancellationToken);
        return result is null ? NotFoundError() : Ok(result);
    }

    private ObjectResult NotFoundError() => ApiErrorFactory.Error(HttpContext, 404, "Không tìm thấy voucher.");
    private ObjectResult ServiceError(AdminVoucherServiceException ex) => ApiErrorFactory.Error(HttpContext, ex.StatusCode, ex.Message, ex.Errors);
}

