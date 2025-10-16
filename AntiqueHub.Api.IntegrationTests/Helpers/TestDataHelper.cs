using AntiqueHub.Core.Entities;
using AntiqueHub.Core.Models;

namespace AntiqueHub.Api.IntegrationTests;

public static class TestDataHelper
{
    public static void Seed(AntiqueDbContext db)
    {
        if (db.Antiques.Any())
            return;

        var antiques = new List<Antique>
        {
            new()
            {
                Id = 1,
                Name = "First antique",
                Status = Status.Available,
                Description = "A lovely old thing with a description.",
                Price = 500m,
                Thumbnail = "fa453816-bb2b-45d4-83ce-3c5e99ccba25.jpg",
                Images = new[]
                {
                    "fa453816-bb2b-45d4-83ce-3c5e99ccba25.jpg"
                },
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                Version = 1
            },
            new()
            {
                Id = 2,
                Name = "Second antique - A frog",
                Status = Status.Available,
                Description = "A lovely old thing with a ribbet.",
                Price = 50m,
                Thumbnail = "627f94d7-410a-48f6-9a5e-2ed29fa6a95f.jpg",
                Images = new[]
                {
                    "627f94d7-410a-48f6-9a5e-2ed29fa6a95f.jpg",
                    "a974ae0e-bb3e-4322-9157-744cd9eb6fc0.png",
                    "b06aa0ef-86d4-4a9d-b2d5-1621fb13a9c8.png"
                },
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                Version = 1
            },
            new()
            {
                Id = 3,
                Name = "Vintage clock",
                Status = Status.Sold,
                Description = "This thing ticks.",
                Price = 1200m,
                Thumbnail = "22e6805f-a05a-490b-a5fb-7d9de71acdf2.jpg",
                Images = new[]
                {
                    "22e6805f-a05a-490b-a5fb-7d9de71acdf2.jpg",
                    "c114052a-a157-47fa-a855-a53aca941066.png",
                    "761d6b3b-eafe-4f36-b017-45d88c4a25ee.png"
                },
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                Version = 1
            }
        };

        db.Antiques.AddRange(antiques);
        db.SaveChanges();
    }
}
