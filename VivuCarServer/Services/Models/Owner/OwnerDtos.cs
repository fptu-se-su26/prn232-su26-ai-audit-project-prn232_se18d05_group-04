namespace Services.Models.Owner;

// ─── Car List ──────────────────────────────────────────────────────────────

public class OwnerCarListQuery
{
    public string? Status { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class OwnerCarResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public short? Year { get; set; }
    public decimal DailyPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? PrimaryImageUrl { get; set; }
    public int TotalBookings { get; set; }
    public double AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OwnerCarDetailResponse : OwnerCarResponse
{
    public string? Color { get; set; }
    public int KilometersDriven { get; set; }
    public string? Description { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal InsuranceFeePerDay { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DepositAmount { get; set; }
    public int SeatCount { get; set; }
    public string TransmissionType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public int? CarTypeId { get; set; }
    public string? CarType { get; set; }
    public int CarBrandId { get; set; }
    public int CarModelId { get; set; }
    public IReadOnlyList<OwnerCarImageResponse> Images { get; set; } = [];
}

public class OwnerCarImageResponse
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

// ─── Car Upsert ────────────────────────────────────────────────────────────

public class OwnerCarUpsertRequest
{
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int? CarTypeId { get; set; }
    public short? Year { get; set; }
    public string? Color { get; set; }
    public int KilometersDriven { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal InsuranceFeePerDay { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DepositAmount { get; set; }
    public int SeatCount { get; set; }
    public string TransmissionType { get; set; } = "Automatic";
    public string FuelType { get; set; } = "Gasoline";
}

public class OwnerCarImageCreateRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

// ─── Car Activity ─────────────────────────────────────────────────────────

public class OwnerCarActivityResponse
{
    public int CarId { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedTrips { get; set; }
    public int TotalRentalDays { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
    public IReadOnlyList<OwnerBookingResponse> RecentBookings { get; set; } = [];
}

// ─── Booking ───────────────────────────────────────────────────────────────

public class OwnerBookingListFilter
{
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class OwnerBookingResponse
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CarName { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string? CarImageUrl { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class OwnerBookingDetailResponse : OwnerBookingResponse
{
    public string PickupLocation { get; set; } = string.Empty;
    public string ReturnLocation { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? CancellationReason { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? CitizenIdNumber { get; set; }
    public string? ContractNumber { get; set; }
    public HandoverInfoDto? PreRentalHandover { get; set; }
    public HandoverInfoDto? PostRentalHandover { get; set; }
}

public class HandoverInfoDto
{
    public int OdometerKm { get; set; }
    public string? FuelLevel { get; set; }
    public string? ExteriorStatus { get; set; }
    public string? InteriorStatus { get; set; }
    public string? DamageNotes { get; set; }
    public string? Note { get; set; }
    public decimal ExtraFee { get; set; }
    public DateTime ConfirmedAt { get; set; }
}

// ─── Handover ─────────────────────────────────────────────────────────────

public class HandoverRequest
{
    public int OdometerKm { get; set; }
    public string FuelLevel { get; set; } = "Full";  // UI-only field. Not in DB schema.
    public string ExteriorStatus { get; set; } = "Good";  // UI-only field. Not in DB schema.
    public string InteriorStatus { get; set; } = "Good";  // UI-only field. Not in DB schema.
    public string? DamageNotes { get; set; }
    public string? Note { get; set; }
}

public class ReturnInspectionRequest
{
    public int OdometerKm { get; set; }
    public string FuelLevel { get; set; } = "Full";  // UI-only field. Not in DB schema.
    public string ExteriorStatus { get; set; } = "Good";  // UI-only field. Not in DB schema.
    public string InteriorStatus { get; set; } = "Good";  // UI-only field. Not in DB schema.
    public string? DamageNotes { get; set; }
    public string? Note { get; set; }
    public decimal ExtraFee { get; set; }
    public string NextCarStatus { get; set; } = "Available";
    
    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    public List<string> ImageUrls { get; set; } = [];
}

// ─── Dashboard ────────────────────────────────────────────────────────────

public class OwnerDashboardStats
{
    public int TotalCars { get; set; }
    public int AvailableCars { get; set; }
    public int RentedCars { get; set; }
    public int PendingBookings { get; set; }
    public int ActiveBookings { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public IReadOnlyList<OwnerBookingResponse> RecentBookings { get; set; } = [];
}

// ─── Incidents ────────────────────────────────────────────────────────────

public class CreateIncidentRequest
{
    public string Description { get; set; } = string.Empty;
    public string? Title { get; set; }
}

public class OwnerIncidentFilter
{
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class OwnerIncidentImageResponse
{
    public string ImageUrl { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
}

public class OwnerIncidentResponse
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportedByName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? PenaltyAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // UI-only field. Not present in current DB schema. Requires migration before backend integration.
    public List<OwnerIncidentImageResponse> Images { get; set; } = [];
}

// ─── Shared Types ─────────────────────────────────────────────────────────

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}

public class CarTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CarModelResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
}
