namespace AntiqueHub.Core.Entities;

public enum Status
{
    Available,
    Sold,
    Archived
}

public class Antique
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Status Status { get; set; } = Status.Available;
    public required decimal Price { get; set; }
    public string? Thumbnail { get; set; }
    public string[]? Images { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Version { get; set; }
}