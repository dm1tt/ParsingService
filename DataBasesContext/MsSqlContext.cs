using Microsoft.EntityFrameworkCore;
using ParsingService.Models;

namespace ParsingServise.DataBasesContext;

public class MsSqlContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductLink> ProductLinks { get; set; } = null!;

    public MsSqlContext(DbContextOptions<MsSqlContext> options) : base(options)
    {
        
    }
}
