using System;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.User;

namespace Services.Implementations;

public class UserService(VivuCarDbContext dbContext) : IUserService
{
    public async Task<UserProfileDto> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .Include(u => u.DriverDocument)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new Exception("User not found");

        return MapToDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .Include(u => u.DriverDocument)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new Exception("User not found");

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth;
        user.Address = request.Address;
        user.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<string> UpdateAvatarUrlAsync(int userId, string avatarUrl, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FindAsync([userId], cancellationToken);
        if (user == null)
            throw new Exception("User not found");

        user.AvatarUrl = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return user.AvatarUrl;
    }

    public async Task<DriverDocumentDto> GetDriverDocumentAsync(int userId, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.DriverDocuments
            .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);
            
        if (document == null)
        {
            document = new DriverDocument 
            { 
                UserId = userId, 
                VerificationStatus = DocumentVerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.DriverDocuments.Add(document);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return MapToDocumentDto(document);
    }

    public async Task<DriverDocumentDto> SubmitDocumentForVerificationAsync(int userId, SubmitDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.DriverDocuments
            .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);

        if (document == null)
        {
            document = new DriverDocument 
            { 
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.DriverDocuments.Add(document);
        }

        document.CitizenIdNumber = request.CitizenIdNumber;
        if (request.CitizenIdFrontImageUrl != null) document.CitizenIdFrontImageUrl = request.CitizenIdFrontImageUrl;
        if (request.CitizenIdBackImageUrl != null) document.CitizenIdBackImageUrl = request.CitizenIdBackImageUrl;
        document.DriverLicenseNumber = request.DriverLicenseNumber;
        document.DriverLicenseFrontImageUrl = request.DriverLicenseFrontImageUrl;
        document.DriverLicenseBackImageUrl = request.DriverLicenseBackImageUrl;
        
        document.VerificationStatus = DocumentVerificationStatus.Pending;
        document.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDocumentDto(document);
    }

    public async Task<DriverDocumentDto> CancelDocumentVerificationAsync(int userId, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.DriverDocuments
            .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);

        if (document == null)
        {
            throw new Exception("Document not found");
        }

        if (document.VerificationStatus != DocumentVerificationStatus.Pending)
        {
            throw new Exception("Can only cancel pending documents");
        }

        document.VerificationStatus = DocumentVerificationStatus.Unverified;
        document.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDocumentDto(document);
    }

    private static UserProfileDto MapToDto(AppUser user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            CreatedAt = user.CreatedAt,
            DriverDocumentStatus = user.DriverDocument?.VerificationStatus ?? DocumentVerificationStatus.Pending
        };
    }

    private static DriverDocumentDto MapToDocumentDto(DriverDocument document)
    {
        return new DriverDocumentDto
        {
            Id = document.Id,
            UserId = document.UserId,
            CitizenIdNumber = document.CitizenIdNumber,
            CitizenIdFrontImageUrl = document.CitizenIdFrontImageUrl,
            CitizenIdBackImageUrl = document.CitizenIdBackImageUrl,
            DriverLicenseNumber = document.DriverLicenseNumber,
            DriverLicenseFrontImageUrl = document.DriverLicenseFrontImageUrl,
            DriverLicenseBackImageUrl = document.DriverLicenseBackImageUrl,
            VerificationStatus = document.VerificationStatus,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
