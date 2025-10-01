using AntiqueHub.Core.Interfaces;
using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiqueHub.Core.Repositories;

public class AntiqueRepository(AntiqueDbContext context) : IAntiqueRepository
{
    // Primary constructor
    private readonly AntiqueDbContext _context = context;

    public async Task<IEnumerable<Antique>> GetAntiquesAsync(IEnumerable<Status> statuses) =>
        await _context.Antiques
            .AsNoTracking()
            .Where(a => statuses.Contains(a.Status))
            .ToListAsync();
    
    public async Task<Antique?> GetAntiqueByIdAsync(int id) =>
        await _context.Antiques.FirstOrDefaultAsync(a => a.Id == id);
    public async Task AddAntiqueAsync(Antique antique) =>
        await _context.Antiques.AddAsync(antique);
    public async Task UpdateAntiqueAsync(Antique antique) =>
        _context.Antiques.Update(antique);
    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}