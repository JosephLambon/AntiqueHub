using AntiqueHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiqueHub.Tests.Mocks
{
    public static class AntiqueDbContextMock
    {
        // Generic helper to mock a DbSet<T> from a List<T>
        public static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            // Tell Moq that when LINQ queries are run, they should use the underlying data's query provider.
            // This makes methods like .Where(), .OrderBy(), etc. work on the mock set.
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            // Link the LINQ expression tree from the mock to the real IQueryable data.
            // This allows query expressions to be parsed and executed against the data.
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            // Define the type of objects that the IQueryable will return (in this case, T).
            // EF Core uses this metadata to know what entity type is being queried.
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            // Hook up the enumerator so that foreach loops and iteration work correctly.
            // Without this, iterating over the DbSet<T> would fail.
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        // Factory for creating a mock AntiqueDbContext
        public static Mock<AntiqueDbContext> CreateMockAntiqueDbContext()
        {
            var mockAntiques = GetMockAntiques().AsQueryable();
            var mockSet = CreateMockDbSet(mockAntiques);

            var mockContext = new Mock<AntiqueDbContext>(
                new DbContextOptionsBuilder<AntiqueDbContext>().Options
            );

            mockContext.Setup(c => c.Antiques).Returns(mockSet.Object);

            return mockContext;
        }

        private static List<Antique> GetMockAntiques()
        {
            return new List<Antique>
            {
                new Antique
                {
                    Id = 1,
                    Name = "Victorian Chair",
                    Description = "A 19th century Victorian-era wooden chair.",
                    Status = Status.Available,
                    Price = 120.50m,
                    Thumbnail = $"{Guid.NewGuid()}.jpg",
                    Images = new [] { $"{Guid.NewGuid()}.jpg", $"{Guid.NewGuid()}.png" },
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
                    UpdatedAt = DateTimeOffset.UtcNow.AddDays(-2),
                    Version = 1
                },
                new Antique
                {
                    Id = 2,
                    Name = "Art Deco Lamp",
                    Description = "Classic 1930s Art Deco style lamp.",
                    Status = Status.Sold,
                    Price = 340.00m,
                    Thumbnail = $"{Guid.NewGuid()}.png",
                    Images = new [] { $"{Guid.NewGuid()}.png" },
                    CreatedAt = DateTimeOffset.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                    Version = 2
                },
                new Antique
                {
                    Id = 3,
                    Name = "Renaissance Painting",
                    Description = "Oil painting attributed to the late Renaissance period.",
                    Status = Status.Archived,
                    Price = 12500.00m,
                    Thumbnail = $"{Guid.NewGuid()}.jpg",
                    Images = new [] { $"{Guid.NewGuid()}.jpg", $"{Guid.NewGuid()}.jpg", $"{Guid.NewGuid()}.png" },
                    CreatedAt = DateTimeOffset.UtcNow.AddYears(-2),
                    UpdatedAt = DateTimeOffset.UtcNow.AddMonths(-6),
                    Version = 5
                }
            };
        }
    }
}
