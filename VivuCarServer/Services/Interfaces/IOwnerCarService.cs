using Services.Models.Owner;

namespace Services.Interfaces;

public interface IOwnerCarService
{
    Task<PagedResult<OwnerCarResponse>> GetOwnerCarsAsync(
        int ownerId,
        OwnerCarListQuery query,
        CancellationToken cancellationToken = default
    );

    Task<OwnerCarDetailResponse?> GetOwnerCarDetailAsync(
        int ownerId,
        int carId,
        CancellationToken cancellationToken = default
    );

    Task<OwnerCarDetailResponse> CreateOwnerCarAsync(
        int ownerId,
        OwnerCarUpsertRequest request,
        CancellationToken cancellationToken = default
    );

    Task<OwnerCarDetailResponse?> UpdateOwnerCarAsync(
        int ownerId,
        int carId,
        OwnerCarUpsertRequest request,
        CancellationToken cancellationToken = default
    );

    Task<OwnerCarDetailResponse?> UpdateCarStatusAsync(
        int ownerId,
        int carId,
        string status,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<OwnerCarImageResponse>?> AddCarImagesAsync(
        int ownerId,
        int carId,
        IReadOnlyList<OwnerCarImageCreateRequest> images,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteCarImageAsync(
        int ownerId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    );

    Task<bool> SetPrimaryImageAsync(
        int ownerId,
        int carId,
        int imageId,
        CancellationToken cancellationToken = default
    );

    Task<OwnerCarActivityResponse?> GetCarActivityAsync(
        int ownerId,
        int carId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<CarTypeResponse>> GetCarTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarModelResponse>> GetCarModelsAsync(CancellationToken cancellationToken = default);
}

public class OwnerCarServiceException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
