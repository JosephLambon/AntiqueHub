namespace DocumentMiddleware.Core.Models;

public enum Status
{
    Available,
    Sold,
    Archived
}

public class Antique
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Status Status { get; set; } = Models.Status.Available;
    public decimal Price { get; set; }
    public string? Thumbnail { get; set; }
    public string[]? Images { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}