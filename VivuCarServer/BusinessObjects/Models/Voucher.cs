using BusinessObjects.Enums;
namespace BusinessObjects.Models;
public class Voucher
{
    public int Id{get;set;} public string Name{get;set;}=string.Empty; public string Code{get;set;}=string.Empty; public DiscountType DiscountType{get;set;} public decimal DiscountValue{get;set;} public decimal MinOrderAmount{get;set;} public decimal MaxDiscount{get;set;} public int Quantity{get;set;} public DateTime? ExpiresAt{get;set;} public DateTime CreatedAt{get;set;} public ICollection<BookingVoucher> BookingVouchers{get;set;}=[];
}
