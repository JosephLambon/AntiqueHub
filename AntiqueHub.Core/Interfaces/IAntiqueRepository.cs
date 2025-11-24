using AntiqueHub.Core.Entities;

namespace AntiqueHub.Core.Interfaces;

public interface IAntiqueRepository
{
    Task<IEnumerable<Antique>> GetAntiquesAsync(IEnumerable<Status> statuses);
    Task<Antique?> GetAntiqueByIdAsync(int id);
    Task AddAntiqueAsync(Antique antique);
    void UpdateAntiqueAsync(Antique antique);
    Task SaveChangesAsync();
}