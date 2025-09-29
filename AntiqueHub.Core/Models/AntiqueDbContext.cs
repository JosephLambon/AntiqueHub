using Microsoft.EntityFrameworkCore;

namespace AntiqueHub.Core.Models;

public class AntiqueDbContext(DbContextOptions<AntiqueDbContext> options) : DbContext(options)
{
    public DbSet<Antique> Antiques { get; set; }
}