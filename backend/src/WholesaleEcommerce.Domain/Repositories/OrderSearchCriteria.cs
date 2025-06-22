namespace WholesaleEcommerce.Domain.Repositories;

public class OrderSearchCriteria
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public Guid? UserId { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
} 