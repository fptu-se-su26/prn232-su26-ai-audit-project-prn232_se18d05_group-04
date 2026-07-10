using Services.Models.Admin;

namespace Services.Interfaces;

public interface IAdminCarService
{
    Task<PagedResult<AdminCarResponse>> GetCarsAsync(
        AdminCarListQuery query,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarResponse?> GetCarAsync(
        int carId,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarResponse> CreateCarAsync(
        int adminUserId,
        AdminCarUpsertRequest request,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarResponse?> UpdateCarAsync(
        int adminUserId,
        int carId,
        AdminCarUpsertRequest request,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarResponse?> BlockCarAsync(
        int adminUserId,
        int carId,
        AdminCarBlockRequest request,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarResponse?> UnblockCarAsync(
        int adminUserId,
        int carId,
        AdminCarUnblockRequest request,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<AdminCarImageResponse>?> AddImagesAsync(
        int adminUserId,
        int carId,
        IReadOnlyList<AdminCarImageCreateRequest> images,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarImageResponse?> DeleteImageAsync(
        int adminUserId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    );

    Task<AdminCarImageResponse?> SetPrimaryImageAsync(
        int adminUserId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<CarTypeResponse>> GetCarTypesAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<CarModelResponse>> GetCarModelsAsync(
        CancellationToken cancellationToken = default
    );
}

public class AdminCarServiceException(int statusCode, string message, IReadOnlyDictionary<string, string[]>? errors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;
}
