using AntiqueHub.Core.Interfaces;
using AntiqueHub.Core.Models;
using AntiqueHub.Core.Constants;
using Moq;

namespace AntiqueHub.Api.Tests.Mocks;

public static class AntiqueRepositoryMock
{
    public static Mock<IAntiqueRepository> GetAntiqueRepository()
    {
        var antiques = new List<Antique>
        {
            new Antique
                {
                Id = 1,
                Name = "Victorian Chair",
                Description = "A 19th century Victorian-style chair made of oak.",
                Status = Status.Available,
                Price = 250.00m,
                Thumbnail = "3f8b1e1c-6c4f-4a4d-9e1f-8c3d82a1f5e4.jpg",
                Images = new[]
                {
                    "3f8b1e1c-6c4f-4a4d-9e1f-8c3d82a1f5e4.jpg",
                    "9c6c1c92-8c88-41d9-b13d-672e9a114c5d.png"
                },
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-3),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-10),
                Version = 1
            },
            new Antique
            {
                Id = 2,
                Name = "Art Deco Lamp",
                Description = "Classic 1930s Art Deco lamp with chrome finish.",
                Status = Status.Sold,
                Price = 180.00m,
                Thumbnail = "27a73a5f-4b41-4e2b-bb0e-d6f364ef29b9.png",
                Images = new[]
                {
                    "27a73a5f-4b41-4e2b-bb0e-d6f364ef29b9.png"
                },
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-5),
                Version = 2
            },
            new Antique
            {
                Id = 3,
                Name = "Georgian Desk",
                Description = "Solid mahogany desk from the Georgian period.",
                Status = Status.Archived,
                Price = 950.00m,
                Thumbnail = "b5f7c7c2-7fdd-48b1-ae5a-fb9d93c7a62f.jpg",
                Images = new[]
                {
                    "b5f7c7c2-7fdd-48b1-ae5a-fb9d93c7a62f.jpg",
                    "d1c4f627-8a7c-46df-a3dd-74670dbbd912.png"
                },
                CreatedAt = DateTimeOffset.UtcNow.AddYears(-1),
                UpdatedAt = DateTimeOffset.UtcNow.AddMonths(-6),
                Version = 3
            }
        };

        var mockRepo = new Mock<IAntiqueRepository>();
        
        mockRepo.Setup(r => r.GetAntiquesAsync(It.IsAny<IEnumerable<Status>>()))
            .ReturnsAsync((IEnumerable<Status> statuses) =>
                antiques.Where(a => statuses.Contains(a.Status)));
        
        mockRepo.Setup(r => r.GetAntiqueByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => antiques.FirstOrDefault(a => a.Id == id));
        
        mockRepo.Setup(r => r.AddAntiqueAsync(It.IsAny<Antique>()))
            .Callback<Antique>(antique =>
            {
                antique.Id = 123;
            })
            .Returns(Task.CompletedTask)
            .Verifiable();

        mockRepo.Setup(r => r.UpdateAntiqueAsync(It.IsAny<Antique>()))
            .Callback<Antique>(antique =>
            {
                antique.UpdatedAt = DateTimeOffset.UtcNow;
                antique.Version += 1;
            })
            .Returns(Task.CompletedTask)
            .Verifiable();

        return mockRepo;
    }
}