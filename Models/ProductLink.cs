using System.ComponentModel.DataAnnotations;

namespace ParsingService.Models;
public class ProductLink
{
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Link { get; set; }

    [Required]
    public string SiteName { get; set; }

    public int Price { get; set; }

    public string ProductId { get; set; }

    public bool IsStock { get; set; }

    public int Count { get; set; }

    public Product Product { get; set; } = null!;
}