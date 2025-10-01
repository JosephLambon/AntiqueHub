using AntiqueHub.Core.Models;

namespace AntiqueHub.Core.Interfaces;

public interface IAntiqueRepository
{
    Task<IEnumerable<Antique>> GetAntiquesAsync(IEnumerable<Status> statuses);
    Task<Antique?> GetAntiqueByIdAsync(int id);
    Task AddAntiqueAsync(Antique antique);
    Task UpdateAntiqueAsync(Antique antique);
    Task SaveChangesAsync();
}