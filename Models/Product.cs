namespace ParsingService.Models;

public class Product
{
    public required string ProductId { get; set; }
    
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }
    
    public string? ProductType { get; set; } 
}