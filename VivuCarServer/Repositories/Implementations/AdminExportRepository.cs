using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations;

public class AdminExportRepository(VivuCarDbContext db) : IAdminExportRepository
{
    public Task AddJobAsync(ExportJob job, CancellationToken ct = default) => db.ExportJobs.AddAsync(job, ct).AsTask();
    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    public Task<ExportJob?> GetJobAsync(int id, int adminId, CancellationToken ct = default) => db.ExportJobs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.RequestedBy == adminId, ct);
    public async Task<IReadOnlyList<ExportJob>> GetJobsAsync(int adminId, int limit, CancellationToken ct = default) => await db.ExportJobs.AsNoTracking().Where(x => x.RequestedBy == adminId).OrderByDescending(x => x.CreatedAt).Take(limit).ToListAsync(ct);

    public async Task<AdminReportPreviewResponse> GetPreviewAsync(AdminReportFilter f, int limit, CancellationToken ct = default) => f.Type switch
    {
        "users" => await Users(f, limit, ct),
        "cars" => await Cars(f, limit, ct),
        "revenue" => await Revenue(f, limit, ct),
        _ => await Payments(f, limit, ct)
    };

    private async Task<AdminReportPreviewResponse> Payments(AdminReportFilter f, int limit, CancellationToken ct)
    {
        var start = f.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc); var end = f.To.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var query = db.PaymentTransactions.AsNoTracking().Include(x => x.Booking).ThenInclude(x => x.Customer).Where(x => x.CreatedAt >= start && x.CreatedAt < end);
        if (string.Equals(f.PaymentStatus,"refunded",StringComparison.OrdinalIgnoreCase)) query = query.Where(_ => false);
        else if (MapPayment(f.PaymentStatus) is { } ps) query = query.Where(x => x.Status == ps);
        if (MapBookings(f.BookingStatus) is { } bs) query = query.Where(x => bs.Contains(x.Booking.Status));
        var total = await query.CountAsync(ct); var items = await query.OrderByDescending(x => x.CreatedAt).Take(limit).ToListAsync(ct);
        return Result(["Giao dịch", "Mã đơn", "Khách hàng", "Ngày thanh toán", "Số tiền", "Phương thức", "Trạng thái"], items.Select(x => (IReadOnlyList<string>)[x.TransactionCode, x.Booking.BookingCode, x.Booking.Customer.FullName, (x.PaidAt ?? x.CreatedAt).ToString("dd/MM/yyyy HH:mm"), x.Amount.ToString("N0"), x.PaymentProvider.ToString(), PaymentLabel(x.Status)]).ToList(), total);
    }

    private async Task<AdminReportPreviewResponse> Users(AdminReportFilter f, int limit, CancellationToken ct)
    {
        var start=f.From.ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc); var end=f.To.AddDays(1).ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc);
        var query=db.Users.AsNoTracking().Include(x=>x.Bookings).Where(x=>x.Role==UserRole.Customer&&x.CreatedAt>=start&&x.CreatedAt<end);
        var total=await query.CountAsync(ct); var items=await query.OrderByDescending(x=>x.CreatedAt).Take(limit).ToListAsync(ct);
        return Result(["Mã khách", "Họ tên", "Email", "SĐT", "Số đơn", "Tổng chi tiêu", "Trạng thái"], items.Select(x=>(IReadOnlyList<string>)[x.Id.ToString(),x.FullName,x.Email,x.PhoneNumber,x.Bookings.Count.ToString(),x.Bookings.Where(b=>b.Status==BookingStatus.Completed).Sum(b=>b.TotalAmount).ToString("N0"),x.Status.ToString()]).ToList(),total);
    }

    private async Task<AdminReportPreviewResponse> Cars(AdminReportFilter f,int limit,CancellationToken ct)
    {
        var start=f.From.ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc); var end=f.To.AddDays(1).ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc);
        var query=db.Cars.AsNoTracking().Include(x=>x.CarBrand).Include(x=>x.CarModel).Include(x=>x.Owner).Where(x=>x.CreatedAt>=start&&x.CreatedAt<end);
        var total=await query.CountAsync(ct); var items=await query.OrderByDescending(x=>x.CreatedAt).Take(limit).ToListAsync(ct);
        return Result(["Biển số","Hãng xe","Dòng xe","Chủ xe","Giá/ngày","Trạng thái"],items.Select(x=>(IReadOnlyList<string>)[x.LicensePlate,x.CarBrand.Name,x.CarModel.Name,x.Owner.FullName,x.DailyPrice.ToString("N0"),x.Status.ToString()]).ToList(),total);
    }

    private async Task<AdminReportPreviewResponse> Revenue(AdminReportFilter f,int limit,CancellationToken ct)
    {
        var query=db.DailyRevenueSnapshots.AsNoTracking().Where(x=>x.SnapshotDate>=f.From&&x.SnapshotDate<=f.To); var total=await query.CountAsync(ct); var items=await query.OrderByDescending(x=>x.SnapshotDate).Take(limit).ToListAsync(ct);
        return Result(["Ngày","Tổng đơn","Hoàn tất","Đã hủy","Doanh thu","Doanh thu thuần","Tiền cọc"],items.Select(x=>(IReadOnlyList<string>)[x.SnapshotDate.ToString("dd/MM/yyyy"),x.TotalBookings.ToString(),x.CompletedBookings.ToString(),x.CancelledBookings.ToString(),x.GrossRevenue.ToString("N0"),x.NetRevenue.ToString("N0"),x.DepositCollected.ToString("N0")]).ToList(),total);
    }

    private static AdminReportPreviewResponse Result(IReadOnlyList<string> h,IReadOnlyList<IReadOnlyList<string>> r,int total)=>new(){Headers=h,Rows=r,TotalRows=total};
    private static PaymentStatus? MapPayment(string? v)=>v?.ToLowerInvariant() switch{"success"=>PaymentStatus.Success,"pending"=>PaymentStatus.Pending,"failed"=>PaymentStatus.Failed,_=>null};
    private static BookingStatus[]? MapBookings(string? v)=>v?.ToLowerInvariant() switch{"completed"=>[BookingStatus.Completed],"cancelled"=>[BookingStatus.Cancelled,BookingStatus.Expired],"approved"=>[BookingStatus.WaitingPickup,BookingStatus.InProgress,BookingStatus.ReturnRequested],"pending"=>[BookingStatus.PendingApproval,BookingStatus.WaitingDeposit],"rejected"=>[BookingStatus.Rejected],_=>null};
    private static string PaymentLabel(PaymentStatus value)=>value switch{PaymentStatus.Success=>"success",PaymentStatus.Failed=>"failed",PaymentStatus.Cancelled=>"failed",_=>"pending"};
}


