using AutoMapper;
using DocumentMiddleware.Api.Services;
using DocumentMiddleware.Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocumentMiddleware.Api.EndpointsHandlers;
public static class AntiqueHandlers
{
    public static async Task<Ok<IEnumerable<AntiqueForResponseDto>>> GetAntiquesAsync(
        DocumentDbContext documentDbContext,
        IMapper mapper,
        ILogger<Antique> logger,
        bool includeAvailable = true,
        bool includeSold = false,
        bool includeArchived = false
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
                await documentDbContext.Antiques
                    .AsNoTracking()
                    .Where(a => includedStatuses.Contains(a.Status))
                    .ToListAsync())
        );
    }
    
    
    
    public static async Task<Results<Ok<AntiqueForResponseDto>,BadRequest<string>, UnprocessableEntity<string>,StatusCodeHttpResult>> CreateAntiqueAsync(
        DocumentDbContext documentDbContext,
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
            foreach(IFormFile image in antiqueToCreate.ImageFiles)
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
            
            var antiqueEntity = mapper.Map<Antique>(antiqueToCreate, opt =>
            {
                opt.Items["FileNames"] = uploadedFiles;
            });
            antiqueEntity.Thumbnail = uploadedFiles.FirstOrDefault();
            antiqueEntity.CreatedAt = antiqueEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
            antiqueEntity.Version = 1;
    
            var antiqueToReturn = mapper.Map<AntiqueForResponseDto>(antiqueEntity);
        
            documentDbContext.Antiques.Add(antiqueEntity);
            await documentDbContext.SaveChangesAsync();
    
            // Need to add a valid routeName and routeValues
            //return TypedResults.CreatedAtRoute(
            //    antiqueToReturn,
            //    null,
            //    null
            //);
            
            logger.LogInformation("Antique creation succeeded.");
            
            return TypedResults.Ok(antiqueToReturn); // TEMPORARY
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            // .NET 9.0 introduces TypedResults.InternalServerError (could return ex.Message)
            // ^ Scope for future improvement
            return TypedResults.StatusCode(500);
        }
    }

    
    // public static async Task UpdateAntiqueAsync(bool)
    // {
    //     DocumentDbContext documentDbContext,
    //         IMapper mapper,
    //     [FromForm] AntiqueForCreationDto antiqueToCreate,
    //     ILogger<Antique> logger,
    // }
}