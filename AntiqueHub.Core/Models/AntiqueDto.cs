using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace AntiqueHub.Core.Models;
public class AntiqueForResponseDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public Status? Status { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Thumbnail { get; set; }
    public string[]? Images { get; set; }
}

public class AntiqueForCreationDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Status Status { get; set; }
    public required decimal Price { get; set; }
    public IFormFileCollection? ImageFiles { get; set; }
}

public class AntiqueForUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Status? Status { get; set; }
    public decimal? Price { get; set; }
    public string? ThumbnailFile { get; set; }
}