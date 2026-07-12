using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Owner;

namespace Services.Implementations;

public class IncidentService(VivuCarDbContext dbContext) : IIncidentService
{
    public async Task<OwnerIncidentResponse> CreateIncidentAsync(
        int ownerId,
        int bookingId,
        CreateIncidentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        // Verify booking belongs to owner
        var booking = await dbContext.Bookings
            .Include(b => b.Car)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.Car.OwnerId == ownerId, cancellationToken);

        if (booking == null)
            throw new InvalidOperationException("Không tìm thấy chuyến thuê hoặc bạn không có quyền truy cập.");

        var incident = new IncidentReport
        {
            BookingId   = bookingId,
            ReporterId  = ownerId,
            Title       = request.Title?.Trim() ?? "Sự cố không có tiêu đề",
            Description = request.Description.Trim(),
            Status      = IncidentStatus.Open,
            CreatedAt   = DateTime.UtcNow
        };

        dbContext.IncidentReports.Add(incident);
        await dbContext.SaveChangesAsync(cancellationToken);

        var reporter = await dbContext.Users.FindAsync([ownerId], cancellationToken);
        return MapToResponse(incident, booking.BookingCode, reporter?.FullName ?? "Chủ xe");
    }

    public async Task<PagedResult<OwnerIncidentResponse>> GetOwnerIncidentsAsync(
        int ownerId,
        OwnerIncidentFilter filter,
        CancellationToken cancellationToken = default
    )
    {
        // Show all incidents linked to bookings of cars owned by this owner
        var ownerCarIds = await dbContext.Cars
            .Where(c => c.OwnerId == ownerId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var query = dbContext.IncidentReports
            .Include(i => i.Booking)
            .Include(i => i.Reporter)
            .Where(i => ownerCarIds.Contains(i.Booking.CarId));

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            Enum.TryParse<IncidentStatus>(filter.Status, true, out var statusEnum))
        {
            query = query.Where(i => i.Status == statusEnum);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var page     = Math.Max(filter.Page, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var list = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = list.Select(i => MapToResponse(i, i.Booking?.BookingCode ?? "", i.Reporter?.FullName ?? "Không xác định")).ToList();
        
        return new PagedResult<OwnerIncidentResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OwnerIncidentResponse?> GetIncidentDetailAsync(
        int ownerId,
        int incidentId,
        CancellationToken cancellationToken = default
    )
    {
        var ownerCarIds = await dbContext.Cars
            .Where(c => c.OwnerId == ownerId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var incident = await dbContext.IncidentReports
            .Include(i => i.Booking)
            .Include(i => i.Reporter)
            .FirstOrDefaultAsync(i => i.Id == incidentId && ownerCarIds.Contains(i.Booking.CarId), cancellationToken);

        if (incident == null) return null;
        return MapToResponse(incident, incident.Booking?.BookingCode ?? "", incident.Reporter?.FullName ?? "Không xác định");
    }

    private static string MapStatus(IncidentStatus s) => s switch
    {
        IncidentStatus.Open     => "open",
        IncidentStatus.InReview => "in_review",
        IncidentStatus.Resolved => "resolved",
        IncidentStatus.Rejected => "rejected",
        IncidentStatus.Closed   => "closed",
        _                       => s.ToString().ToLowerInvariant()
    };

    private static OwnerIncidentResponse MapToResponse(IncidentReport i, string bookingCode, string reporterName) => new()
    {
        Id             = i.Id,
        BookingId      = i.BookingId,
        BookingCode    = bookingCode,
        Title          = i.Title,
        Description    = i.Description,
        ReportedByName = reporterName,
        PenaltyAmount  = i.PenaltyAmount,
        Status         = MapStatus(i.Status),
        CreatedAt      = i.CreatedAt,
        ResolvedAt     = i.ResolvedAt
    };
}
