using System.Threading;
using System.Threading.Tasks;
using Services.Models.User;

namespace Services.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<string> UpdateAvatarUrlAsync(int userId, string avatarUrl, CancellationToken cancellationToken = default);
    Task<DriverDocumentDto> GetDriverDocumentAsync(int userId, CancellationToken cancellationToken = default);
    Task<DriverDocumentDto> SubmitDocumentForVerificationAsync(int userId, SubmitDocumentRequest request, CancellationToken cancellationToken = default);
    Task<DriverDocumentDto> CancelDocumentVerificationAsync(int userId, CancellationToken cancellationToken = default);
}
