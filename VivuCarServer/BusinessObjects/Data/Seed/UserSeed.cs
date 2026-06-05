using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace BusinessObjects.Data.Seed;

public static class UserSeed
{
    public static AppUser[] Users =>
        [
            new()
            {
                Id = 1,
                Email = "admin@vivucar.local",
                PasswordHash = "SeedPasswordHash_Admin123",
                FullName = "System Admin",
                PhoneNumber = "0900000001",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1990, 1, 1),
                Address = "Ho Chi Minh City",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 2,
                Email = "customer01@vivucar.local",
                PasswordHash = "SeedPasswordHash_Customer123",
                FullName = "Nguyen Van An",
                PhoneNumber = "0900000002",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1998, 3, 12),
                Address = "District 1, Ho Chi Minh City",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 3,
                Email = "customer02@vivucar.local",
                PasswordHash = "SeedPasswordHash_Customer123",
                FullName = "Tran Thi Binh",
                PhoneNumber = "0900000003",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1997, 7, 21),
                Address = "Thu Duc City, Ho Chi Minh City",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 4,
                Email = "customer03@vivucar.local",
                PasswordHash = "SeedPasswordHash_Customer123",
                FullName = "Le Minh Chau",
                PhoneNumber = "0900000004",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1995, 11, 5),
                Address = "Da Nang",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 5,
                Email = "customer04@vivucar.local",
                PasswordHash = "SeedPasswordHash_Customer123",
                FullName = "Pham Gia Huy",
                PhoneNumber = "0900000005",
                Role = UserRole.Customer,
                Status = UserStatus.Locked,
                DateOfBirth = new DateOnly(1996, 9, 18),
                Address = "Can Tho",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 6,
                Email = "owner01@vivucar.local",
                PasswordHash = "SeedPasswordHash_Owner123",
                FullName = "Vo Quoc Khanh",
                PhoneNumber = "0900000006",
                Role = UserRole.CarOwner,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1988, 4, 9),
                Address = "District 7, Ho Chi Minh City",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 7,
                Email = "owner02@vivucar.local",
                PasswordHash = "SeedPasswordHash_Owner123",
                FullName = "Dang Hoang Long",
                PhoneNumber = "0900000007",
                Role = UserRole.CarOwner,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1985, 12, 2),
                Address = "Binh Thanh, Ho Chi Minh City",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 8,
                Email = "owner03@vivucar.local",
                PasswordHash = "SeedPasswordHash_Owner123",
                FullName = "Hoang Bao Tram",
                PhoneNumber = "0900000008",
                Role = UserRole.CarOwner,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1992, 6, 30),
                Address = "Nha Trang",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 9,
                Email = "owner04@vivucar.local",
                PasswordHash = "SeedPasswordHash_Owner123",
                FullName = "Bui Thanh Son",
                PhoneNumber = "0900000009",
                Role = UserRole.CarOwner,
                Status = UserStatus.Locked,
                DateOfBirth = new DateOnly(1987, 8, 14),
                Address = "Ha Noi",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = 10,
                Email = "admin02@vivucar.local",
                PasswordHash = "SeedPasswordHash_Admin123",
                FullName = "Support Admin",
                PhoneNumber = "0900000010",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                DateOfBirth = new DateOnly(1991, 10, 25),
                Address = "Ho Chi Minh City",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
        ];
}
