using Microsoft.EntityFrameworkCore;
using AntiqueHub.Core.Entities;

namespace AntiqueHub.Core.Models;

public class AntiqueDbContext(DbContextOptions<AntiqueDbContext> options) : DbContext(options)
{
    public DbSet<Antique> Antiques { get; set; }
}