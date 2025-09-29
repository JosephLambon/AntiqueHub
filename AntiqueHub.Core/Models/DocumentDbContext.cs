using Microsoft.EntityFrameworkCore;

namespace DocumentMiddleware.Core.Models;

public class DocumentDbContext(DbContextOptions<DocumentDbContext> options) : DbContext(options)
{
    public DbSet<Antique> Antiques { get; set; }
}