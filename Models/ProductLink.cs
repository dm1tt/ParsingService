namespace ParsingService.Models;
public class ProductLink
{
    public required string Id { get; set; }
    
    public string? Link { get; set; }

    public string? SiteName { get; set; }

    public int? Price { get; set; }

    public bool? InStock { get; set; }

    public int? Count { get; set; }

    public List<Product>? Products { get; set ;}
}