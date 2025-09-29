using AntiqueHub.Api.Services;
using AutoMapper;
using AntiqueHub.Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AntiqueHub.Core.Constants;

namespace AntiqueHub.Api.EndpointsHandlers;
public static class AntiqueHandlers
{
    public static async Task<Ok<IEnumerable<AntiqueForResponseDto>>> GetAntiquesAsync(
        AntiqueDbContext antiqueDbContext,
        IMapper mapper,
        ILogger<Antique> logger,
        [FromQuery] bool includeAvailable = true,
        [FromQuery] bool includeSold = false,
        [FromQuery] bool includeArchived = false
    ){
        logger.LogInformation("GET /antiques request received. Include available={IncludeAvailable}; Include sold={IncludeSold} ; Include archived={IncludeArchived}",
            includeAvailable,
            includeSold,
            includeArchived);

        var includedStatuses = new List<Status>();
        if(includeAvailable)
            includedStatuses.Add(Status.Available);
        if(includeSold)
            includedStatuses.Add(Status.Sold);
        if(includeArchived)
            includedStatuses.Add(Status.Archived);
            
        logger.LogInformation("Getting antiques...");
        return TypedResults.Ok(
            mapper.Map<IEnumerable<AntiqueForResponseDto>>(
                await antiqueDbContext.Antiques
                    .AsNoTracking()
                    .Where(a => includedStatuses.Contains(a.Status))
                    .ToListAsync())
        );
    }
    
    
    
    public static async Task<Results<CreatedAtRoute<AntiqueForResponseDto>,BadRequest<string>, UnprocessableEntity<string>,StatusCodeHttpResult>> CreateAntiqueAsync(
        AntiqueDbContext antiqueDbContext,
        IMapper mapper,
        [FromForm] AntiqueForCreationDto antiqueToCreate,
        ILogger<Antique> logger,
        IFileService fileService
    )
    {   
        logger.LogInformation("Creating antique..."); 
        var uploadedFiles = new List<string>();
        try
        {
            if (antiqueToCreate.ImageFiles == null)
            {
                logger.LogError("No ImageFiles received in request.");
                return TypedResults.BadRequest("ImageFiles must be provided.");
            }

            int count = 1;
            foreach (IFormFile image in antiqueToCreate.ImageFiles)
            {
                logger.LogInformation("Uploading Image {Count} of {TotalImageCount}...",
                    count,
                    antiqueToCreate.ImageFiles.Count);
                if (image.Length > 5 * 1024 * 1024)
                {
                    return TypedResults.UnprocessableEntity("File size should not exceed 5 MB");
                }

                string[] allowedFileExtensions = [".jpg", ".jpeg", ".png"];
                string createdImageName = await fileService.UploadFileAsync(
                    imageFile: image,
                    allowedFileExtensions: allowedFileExtensions);
                uploadedFiles.Add(createdImageName);
                logger.LogInformation("Upload successful.");
                count++;
            }

            var antiqueEntity =
                mapper.Map<Antique>(antiqueToCreate, opt => { opt.Items["FileNames"] = uploadedFiles; });
            antiqueEntity.Thumbnail = uploadedFiles.FirstOrDefault();
            antiqueEntity.CreatedAt = antiqueEntity.UpdatedAt = DateTimeOffset.UtcNow;
            antiqueEntity.Version = 1;

            var antiqueToReturn = mapper.Map<AntiqueForResponseDto>(antiqueEntity);

            antiqueDbContext.Antiques.Add(antiqueEntity);
            await antiqueDbContext.SaveChangesAsync();

            logger.LogInformation("Antique creation succeeded.");

            return TypedResults.CreatedAtRoute(
                antiqueToReturn,
                Routes.GET_ANTIQUE_BY_ID,
                new { antiqueId = antiqueEntity.Id }
        );
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            // .NET 9.0 introduces TypedResults.InternalServerError (could return ex.Message)
            // ^ Scope for future improvement
            return TypedResults.StatusCode(500);
        }
    }

    public static async Task<Results<Ok<AntiqueForResponseDto>,NotFound<string>>> GetAntiqueByIdAsync(
        AntiqueDbContext antiqueDbContext,
        IMapper mapper,
        int antiqueId,
        ILogger<Antique> logger
    )
    {
        logger.LogInformation("GET /antiques/{ID} received.",
            antiqueId
            );
        
        var antiqueEntity = mapper.Map<AntiqueForResponseDto>(
            await antiqueDbContext.Antiques
            .Where(a => a.Id == antiqueId)
            .FirstOrDefaultAsync()
            );

        if (antiqueEntity == null)
            return TypedResults.NotFound<string>(
                String.Concat("Unable to retrieve antique with ID: ",
                antiqueId));
        return TypedResults.Ok<AntiqueForResponseDto>(antiqueEntity);
    }

    public static async Task<Results<Ok<AntiqueForResponseDto>, UnprocessableEntity, NotFound<string>>> UpdateAntiqueAsync(
        AntiqueDbContext antiqueDbContext,
        IMapper mapper,
        int antiqueId,
        AntiqueForUpdateDto updatedAntiqueDto,
        ILogger<Antique> logger
    )
    {
        logger.LogInformation("PATCH /antiques/{ID} request received.",
            antiqueId);
        var existingAntiqueEntity = await antiqueDbContext.Antiques
            .Where(a => a.Id == antiqueId)
            .FirstOrDefaultAsync();
        
        if(existingAntiqueEntity == null)
            return TypedResults.NotFound<string>(
                String.Concat("Unable to retrieve antique with ID: ",
                    antiqueId));

        try
        {
            if (updatedAntiqueDto.Name != null)
                existingAntiqueEntity.Name = updatedAntiqueDto.Name;
            if (updatedAntiqueDto.Description != null)
                existingAntiqueEntity.Description = updatedAntiqueDto.Description;
            if (updatedAntiqueDto.Status != null)
                existingAntiqueEntity.Status = updatedAntiqueDto.Status.Value;
            if (updatedAntiqueDto.Price != null)
                existingAntiqueEntity.Price = updatedAntiqueDto.Price.Value;
            if (updatedAntiqueDto.ThumbnailFile != null)
                existingAntiqueEntity.Thumbnail = updatedAntiqueDto.ThumbnailFile;

            existingAntiqueEntity.UpdatedAt = DateTimeOffset.UtcNow;
            existingAntiqueEntity.Version += 1;
            
            await antiqueDbContext.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            logger.LogError("Update failed. {Message}",
                ex.Message);
            return TypedResults.UnprocessableEntity();
        }

        return TypedResults.Ok<AntiqueForResponseDto>(
            mapper.Map<AntiqueForResponseDto>(existingAntiqueEntity)
            );
    }
}