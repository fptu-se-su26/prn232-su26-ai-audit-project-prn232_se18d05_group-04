IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [CarBrands] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_CarBrands] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(256) NOT NULL,
        [PasswordHash] nvarchar(512) NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [AvatarUrl] nvarchar(500) NULL,
        [Role] nvarchar(32) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [DateOfBirth] date NULL,
        [Address] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [Vouchers] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [DiscountType] nvarchar(32) NOT NULL,
        [DiscountValue] decimal(18,2) NOT NULL,
        [MaxDiscountAmount] decimal(18,2) NULL,
        [MinOrderAmount] decimal(18,2) NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [UsageLimit] int NULL,
        [UsedCount] int NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Vouchers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [CarModels] (
        [Id] int NOT NULL IDENTITY,
        [CarBrandId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_CarModels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarModels_CarBrands_CarBrandId] FOREIGN KEY ([CarBrandId]) REFERENCES [CarBrands] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [DriverDocuments] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [CitizenIdNumber] nvarchar(30) NOT NULL,
        [CitizenIdFrontImageUrl] nvarchar(500) NULL,
        [CitizenIdBackImageUrl] nvarchar(500) NULL,
        [DriverLicenseNumber] nvarchar(30) NOT NULL,
        [DriverLicenseImageUrl] nvarchar(500) NULL,
        [VerificationStatus] nvarchar(32) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_DriverDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DriverDocuments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [Cars] (
        [Id] int NOT NULL IDENTITY,
        [OwnerId] int NOT NULL,
        [CarBrandId] int NOT NULL,
        [CarModelId] int NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [LicensePlate] nvarchar(20) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Location] nvarchar(300) NOT NULL,
        [DailyPrice] decimal(18,2) NOT NULL,
        [InsuranceFeePerDay] decimal(18,2) NOT NULL,
        [DeliveryFee] decimal(18,2) NOT NULL,
        [DepositAmount] decimal(18,2) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [SeatCount] int NOT NULL,
        [TransmissionType] nvarchar(32) NOT NULL,
        [FuelType] nvarchar(32) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Cars] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cars_CarBrands_CarBrandId] FOREIGN KEY ([CarBrandId]) REFERENCES [CarBrands] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cars_CarModels_CarModelId] FOREIGN KEY ([CarModelId]) REFERENCES [CarModels] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cars_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [Bookings] (
        [Id] int NOT NULL IDENTITY,
        [BookingCode] nvarchar(30) NOT NULL,
        [CustomerId] int NOT NULL,
        [CarId] int NOT NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [PickupLocation] nvarchar(300) NOT NULL,
        [ReturnLocation] nvarchar(300) NOT NULL,
        [BasePrice] decimal(18,2) NOT NULL,
        [InsuranceFee] decimal(18,2) NOT NULL,
        [DeliveryFee] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [DepositAmount] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [RemainingAmount] decimal(18,2) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [CancellationReason] nvarchar(500) NULL,
        [CancelledAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bookings_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [CarImages] (
        [Id] int NOT NULL IDENTITY,
        [CarId] int NOT NULL,
        [ImageUrl] nvarchar(500) NOT NULL,
        [IsPrimary] bit NOT NULL,
        [DisplayOrder] int NOT NULL,
        CONSTRAINT [PK_CarImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarImages_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [BookingDriverInfos] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [CitizenIdNumber] nvarchar(30) NOT NULL,
        [CitizenIdFrontImageUrl] nvarchar(500) NULL,
        [CitizenIdBackImageUrl] nvarchar(500) NULL,
        [DriverLicenseNumber] nvarchar(30) NOT NULL,
        [DriverLicenseImageUrl] nvarchar(500) NULL,
        CONSTRAINT [PK_BookingDriverInfos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingDriverInfos_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [BookingStatusHistories] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [OldStatus] nvarchar(32) NOT NULL,
        [NewStatus] nvarchar(32) NOT NULL,
        [ChangedByUserId] int NULL,
        [Note] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_BookingStatusHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingStatusHistories_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookingStatusHistories_Users_ChangedByUserId] FOREIGN KEY ([ChangedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [BookingVouchers] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [VoucherId] int NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [AppliedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_BookingVouchers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BookingVouchers_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookingVouchers_Vouchers_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [Vouchers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [CarAvailabilityBlocks] (
        [Id] int NOT NULL IDENTITY,
        [CarId] int NOT NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [Reason] nvarchar(300) NOT NULL,
        [BookingId] int NULL,
        CONSTRAINT [PK_CarAvailabilityBlocks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarAvailabilityBlocks_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CarAvailabilityBlocks_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [IncidentReports] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [ReporterId] int NOT NULL,
        [Description] nvarchar(2000) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [ResolvedAt] datetime2 NULL,
        CONSTRAINT [PK_IncidentReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_IncidentReports_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_IncidentReports_Users_ReporterId] FOREIGN KEY ([ReporterId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [NotificationLogs] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NULL,
        [RecipientUserId] int NOT NULL,
        [Channel] nvarchar(32) NOT NULL,
        [Subject] nvarchar(200) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_NotificationLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationLogs_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_NotificationLogs_Users_RecipientUserId] FOREIGN KEY ([RecipientUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [PaymentTransactions] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [PaymentProvider] nvarchar(32) NOT NULL,
        [TransactionCode] nvarchar(100) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [PaymentUrl] nvarchar(1000) NULL,
        [PaidAt] datetime2 NULL,
        [RawRequest] nvarchar(max) NULL,
        [RawResponse] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_PaymentTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PaymentTransactions_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [RentalContracts] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [ContractNumber] nvarchar(50) NOT NULL,
        [PdfUrl] nvarchar(500) NOT NULL,
        [GeneratedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_RentalContracts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RentalContracts_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] int NOT NULL IDENTITY,
        [BookingId] int NOT NULL,
        [CustomerId] int NOT NULL,
        [CarId] int NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Reviews_Rating] CHECK ([Rating] BETWEEN 1 AND 5),
        CONSTRAINT [FK_Reviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'AvatarUrl', N'CreatedAt', N'DateOfBirth', N'Email', N'FullName', N'PasswordHash', N'PhoneNumber', N'Role', N'Status', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] ON;
    EXEC(N'INSERT INTO [Users] ([Id], [Address], [AvatarUrl], [CreatedAt], [DateOfBirth], [Email], [FullName], [PasswordHash], [PhoneNumber], [Role], [Status], [UpdatedAt])
    VALUES (1, N''Ho Chi Minh City'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1990-01-01'', N''admin@vivucar.local'', N''System Admin'', N''SeedPasswordHash_Admin123'', N''0900000001'', N''Admin'', N''Active'', NULL),
    (2, N''District 1, Ho Chi Minh City'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1998-03-12'', N''customer01@vivucar.local'', N''Nguyen Van An'', N''SeedPasswordHash_Customer123'', N''0900000002'', N''Customer'', N''Active'', NULL),
    (3, N''Thu Duc City, Ho Chi Minh City'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1997-07-21'', N''customer02@vivucar.local'', N''Tran Thi Binh'', N''SeedPasswordHash_Customer123'', N''0900000003'', N''Customer'', N''Active'', NULL),
    (4, N''Da Nang'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1995-11-05'', N''customer03@vivucar.local'', N''Le Minh Chau'', N''SeedPasswordHash_Customer123'', N''0900000004'', N''Customer'', N''Active'', NULL),
    (5, N''Can Tho'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1996-09-18'', N''customer04@vivucar.local'', N''Pham Gia Huy'', N''SeedPasswordHash_Customer123'', N''0900000005'', N''Customer'', N''Locked'', NULL),
    (6, N''District 7, Ho Chi Minh City'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1988-04-09'', N''owner01@vivucar.local'', N''Vo Quoc Khanh'', N''SeedPasswordHash_Owner123'', N''0900000006'', N''CarOwner'', N''Active'', NULL),
    (7, N''Binh Thanh, Ho Chi Minh City'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1985-12-02'', N''owner02@vivucar.local'', N''Dang Hoang Long'', N''SeedPasswordHash_Owner123'', N''0900000007'', N''CarOwner'', N''Active'', NULL),
    (8, N''Nha Trang'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1992-06-30'', N''owner03@vivucar.local'', N''Hoang Bao Tram'', N''SeedPasswordHash_Owner123'', N''0900000008'', N''CarOwner'', N''Active'', NULL),
    (9, N''Ha Noi'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1987-08-14'', N''owner04@vivucar.local'', N''Bui Thanh Son'', N''SeedPasswordHash_Owner123'', N''0900000009'', N''CarOwner'', N''Locked'', NULL),
    (10, N''Ho Chi Minh City'', NULL, ''2026-01-01T00:00:00.0000000Z'', ''1991-10-25'', N''admin02@vivucar.local'', N''Support Admin'', N''SeedPasswordHash_Admin123'', N''0900000010'', N''Admin'', N''Active'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'AvatarUrl', N'CreatedAt', N'DateOfBirth', N'Email', N'FullName', N'PasswordHash', N'PhoneNumber', N'Role', N'Status', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BookingDriverInfos_BookingId] ON [BookingDriverInfos] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bookings_BookingCode] ON [Bookings] ([BookingCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Bookings_CarId] ON [Bookings] ([CarId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Bookings_CarId_StartDateTime_EndDateTime] ON [Bookings] ([CarId], [StartDateTime], [EndDateTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Bookings_CustomerId] ON [Bookings] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Bookings_Status] ON [Bookings] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_BookingStatusHistories_BookingId] ON [BookingStatusHistories] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_BookingStatusHistories_ChangedByUserId] ON [BookingStatusHistories] ([ChangedByUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BookingVouchers_BookingId] ON [BookingVouchers] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_BookingVouchers_VoucherId] ON [BookingVouchers] ([VoucherId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_CarAvailabilityBlocks_BookingId] ON [CarAvailabilityBlocks] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_CarAvailabilityBlocks_CarId_StartDateTime_EndDateTime] ON [CarAvailabilityBlocks] ([CarId], [StartDateTime], [EndDateTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CarBrands_Name] ON [CarBrands] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_CarImages_CarId_DisplayOrder] ON [CarImages] ([CarId], [DisplayOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CarModels_CarBrandId_Name] ON [CarModels] ([CarBrandId], [Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Cars_CarBrandId] ON [Cars] ([CarBrandId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Cars_CarModelId] ON [Cars] ([CarModelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cars_LicensePlate] ON [Cars] ([LicensePlate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Cars_OwnerId] ON [Cars] ([OwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Cars_Status] ON [Cars] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DriverDocuments_CitizenIdNumber] ON [DriverDocuments] ([CitizenIdNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DriverDocuments_DriverLicenseNumber] ON [DriverDocuments] ([DriverLicenseNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DriverDocuments_UserId] ON [DriverDocuments] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_IncidentReports_BookingId] ON [IncidentReports] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_IncidentReports_ReporterId] ON [IncidentReports] ([ReporterId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_NotificationLogs_BookingId] ON [NotificationLogs] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_NotificationLogs_RecipientUserId] ON [NotificationLogs] ([RecipientUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_PaymentTransactions_BookingId] ON [PaymentTransactions] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PaymentTransactions_TransactionCode] ON [PaymentTransactions] ([TransactionCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RentalContracts_BookingId] ON [RentalContracts] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RentalContracts_ContractNumber] ON [RentalContracts] ([ContractNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reviews_BookingId] ON [Reviews] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Reviews_CarId] ON [Reviews] ([CarId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE INDEX [IX_Reviews_CustomerId] ON [Reviews] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vouchers_Code] ON [Vouchers] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605035311_InitialVivuCarSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605035311_InitialVivuCarSchema', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606071922_UseRuntimeUserSeeding'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606071922_UseRuntimeUserSeeding', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606073455_AddUserTokenVersion'
)
BEGIN
    ALTER TABLE [Users] ADD [TokenVersion] int NOT NULL DEFAULT 1;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606073455_AddUserTokenVersion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606073455_AddUserTokenVersion', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606074458_AddRefreshTokens'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(128) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        [RevokedAt] datetime2 NULL,
        [ReplacedByTokenHash] nvarchar(128) NULL,
        [CreatedByIp] nvarchar(64) NULL,
        [RevokedByIp] nvarchar(64) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606074458_AddRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_ExpiresAt] ON [RefreshTokens] ([ExpiresAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606074458_AddRefreshTokens'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606074458_AddRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606074458_AddRefreshTokens'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606074458_AddRefreshTokens', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613024017_AddCarAvailabilityBlockRowVersion'
)
BEGIN
    ALTER TABLE [CarAvailabilityBlocks] ADD [RowVersion] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260613024017_AddCarAvailabilityBlockRowVersion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260613024017_AddCarAvailabilityBlockRowVersion', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616082433_SeedBookingAndPaymentTestData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[CarBrands]'))
        SET IDENTITY_INSERT [CarBrands] ON;
    EXEC(N'INSERT INTO [CarBrands] ([Id], [IsActive], [Name])
    VALUES (1, CAST(1 AS bit), N''Toyota'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[CarBrands]'))
        SET IDENTITY_INSERT [CarBrands] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616082433_SeedBookingAndPaymentTestData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Description', N'DiscountType', N'DiscountValue', N'EndDateTime', N'IsActive', N'MaxDiscountAmount', N'MinOrderAmount', N'StartDateTime', N'UsageLimit', N'UsedCount') AND [object_id] = OBJECT_ID(N'[Vouchers]'))
        SET IDENTITY_INSERT [Vouchers] ON;
    EXEC(N'INSERT INTO [Vouchers] ([Id], [Code], [Description], [DiscountType], [DiscountValue], [EndDateTime], [IsActive], [MaxDiscountAmount], [MinOrderAmount], [StartDateTime], [UsageLimit], [UsedCount])
    VALUES (1, N''VIVUCAR10'', N''Giảm giá 10% tổng hóa đơn'', N''Percentage'', 10.0, ''2026-07-01T00:00:00.0000000Z'', CAST(1 AS bit), NULL, 500000.0, ''2026-06-01T00:00:00.0000000Z'', 100, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Description', N'DiscountType', N'DiscountValue', N'EndDateTime', N'IsActive', N'MaxDiscountAmount', N'MinOrderAmount', N'StartDateTime', N'UsageLimit', N'UsedCount') AND [object_id] = OBJECT_ID(N'[Vouchers]'))
        SET IDENTITY_INSERT [Vouchers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616082433_SeedBookingAndPaymentTestData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CarBrandId', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[CarModels]'))
        SET IDENTITY_INSERT [CarModels] ON;
    EXEC(N'INSERT INTO [CarModels] ([Id], [CarBrandId], [IsActive], [Name])
    VALUES (1, 1, CAST(1 AS bit), N''Vios'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CarBrandId', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[CarModels]'))
        SET IDENTITY_INSERT [CarModels] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616082433_SeedBookingAndPaymentTestData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CarBrandId', N'CarModelId', N'CreatedAt', N'DailyPrice', N'DeliveryFee', N'DepositAmount', N'Description', N'FuelType', N'InsuranceFeePerDay', N'LicensePlate', N'Location', N'Name', N'OwnerId', N'SeatCount', N'Status', N'TransmissionType', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Cars]'))
        SET IDENTITY_INSERT [Cars] ON;
    EXEC(N'INSERT INTO [Cars] ([Id], [CarBrandId], [CarModelId], [CreatedAt], [DailyPrice], [DeliveryFee], [DepositAmount], [Description], [FuelType], [InsuranceFeePerDay], [LicensePlate], [Location], [Name], [OwnerId], [SeatCount], [Status], [TransmissionType], [UpdatedAt])
    VALUES (1, 1, 1, ''2026-06-01T00:00:00.0000000Z'', 600000.0, 10000.0, 1500000.0, N''Xe gia đình 5 chỗ sạch sẽ, vận hành êm ái, tiết kiệm nhiên liệu.'', N''Gasoline'', 50000.0, N''43A-12345'', N''Hải Châu, Đà Nẵng'', N''Toyota Vios 2022'', 6, 5, N''Available'', N''Automatic'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CarBrandId', N'CarModelId', N'CreatedAt', N'DailyPrice', N'DeliveryFee', N'DepositAmount', N'Description', N'FuelType', N'InsuranceFeePerDay', N'LicensePlate', N'Location', N'Name', N'OwnerId', N'SeatCount', N'Status', N'TransmissionType', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Cars]'))
        SET IDENTITY_INSERT [Cars] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616082433_SeedBookingAndPaymentTestData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616082433_SeedBookingAndPaymentTestData', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707134511_AddDriverLicenseBackImage'
)
BEGIN
    EXEC sp_rename N'[DriverDocuments].[DriverLicenseImageUrl]', N'DriverLicenseFrontImageUrl', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707134511_AddDriverLicenseBackImage'
)
BEGIN
    EXEC sp_rename N'[BookingDriverInfos].[DriverLicenseImageUrl]', N'DriverLicenseFrontImageUrl', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707134511_AddDriverLicenseBackImage'
)
BEGIN
    ALTER TABLE [DriverDocuments] ADD [DriverLicenseBackImageUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707134511_AddDriverLicenseBackImage'
)
BEGIN
    ALTER TABLE [BookingDriverInfos] ADD [DriverLicenseBackImageUrl] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260707134511_AddDriverLicenseBackImage'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260707134511_AddDriverLicenseBackImage', N'8.0.27');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [BlockedReason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [CarTypeId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [Color] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [KilometersDriven] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [PreviousStatus] nvarchar(32) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [PricePerHour] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD [Year] smallint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE TABLE [AdminAuditLogs] (
        [Id] bigint NOT NULL IDENTITY,
        [AdminUserId] int NOT NULL,
        [Action] nvarchar(80) NOT NULL,
        [EntityType] nvarchar(80) NOT NULL,
        [EntityId] int NOT NULL,
        [OldValues] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [CreatedAtUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_AdminAuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdminAuditLogs_Users_AdminUserId] FOREIGN KEY ([AdminUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE TABLE [CarTypes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_CarTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[CarTypes]'))
        SET IDENTITY_INSERT [CarTypes] ON;
    EXEC(N'INSERT INTO [CarTypes] ([Id], [IsActive], [Name])
    VALUES (1, CAST(1 AS bit), N''Sedan''),
    (2, CAST(1 AS bit), N''SUV''),
    (3, CAST(1 AS bit), N''Hatchback''),
    (4, CAST(1 AS bit), N''MPV'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[CarTypes]'))
        SET IDENTITY_INSERT [CarTypes] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    EXEC(N'UPDATE [Cars] SET [BlockedReason] = NULL, [CarTypeId] = 1, [Color] = N''White'', [Description] = N''Clean 5-seat family car with stable handling and efficient fuel usage.'', [KilometersDriven] = 28000, [Location] = N''Hai Chau, Da Nang'', [PreviousStatus] = NULL, [PricePerHour] = 90000.0, [Year] = CAST(2022 AS smallint)
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE INDEX [IX_Cars_CarTypeId] ON [Cars] ([CarTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE INDEX [IX_AdminAuditLogs_Action] ON [AdminAuditLogs] ([Action]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE INDEX [IX_AdminAuditLogs_AdminUserId] ON [AdminAuditLogs] ([AdminUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE INDEX [IX_AdminAuditLogs_EntityType_EntityId] ON [AdminAuditLogs] ([EntityType], [EntityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CarTypes_Name] ON [CarTypes] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    ALTER TABLE [Cars] ADD CONSTRAINT [FK_Cars_CarTypes_CarTypeId] FOREIGN KEY ([CarTypeId]) REFERENCES [CarTypes] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710110153_SyncCarSchemaPhase03'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260710110153_SyncCarSchemaPhase03', N'8.0.27');
END;
GO

COMMIT;
GO

