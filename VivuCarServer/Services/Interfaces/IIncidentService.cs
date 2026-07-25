using Services.Models.Owner;

namespace Services.Interfaces;

public interface IIncidentService
{
    Task<OwnerIncidentResponse> CreateIncidentAsync(
        int ownerId,
        int bookingId,
        CreateIncidentRequest request,
        CancellationToken cancellationToken = default
    );

    Task<PagedResult<OwnerIncidentResponse>> GetOwnerIncidentsAsync(
        int ownerId,
        OwnerIncidentFilter filter,
        CancellationToken cancellationToken = default
    );

    Task<OwnerIncidentResponse?> GetIncidentDetailAsync(
        int ownerId,
        int incidentId,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateIncidentStatusAsync(
        int ownerId,
        int incidentId,
        string status,
        CancellationToken cancellationToken = default
    );
}
