using Microsoft.AspNetCore.Http;

namespace AntiqueHub.Core.Models;
public class AntiqueForResponseDto
{
    public string? Name { get; set; }
    public Status? Status { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Thumbnail { get; set; }
    public string[]? Images { get; set; }
}

public class AntiqueForCreationDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Status? Status { get; set; }
    public decimal? Price { get; set; }
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