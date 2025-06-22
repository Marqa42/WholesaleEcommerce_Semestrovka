namespace WholesaleEcommerce.Domain.Repositories;

public class UserSearchCriteria
{
    public string? Search { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
} 