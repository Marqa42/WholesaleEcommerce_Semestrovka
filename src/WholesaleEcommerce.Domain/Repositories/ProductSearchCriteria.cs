namespace WholesaleEcommerce.Domain.Repositories;

public class ProductSearchCriteria
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Vendor { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public string? Status { get; set; }
    public List<string>? Tags { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
} 