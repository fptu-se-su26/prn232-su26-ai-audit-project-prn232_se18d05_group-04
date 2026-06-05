namespace BusinessObjects.Enums;

public enum BookingStatus
{
    PendingApproval = 1,
    Rejected = 2,
    WaitingDeposit = 3,
    WaitingPickup = 4,
    InProgress = 5,
    ReturnRequested = 6,
    Completed = 7,
    Cancelled = 8,
    Expired = 9
}
