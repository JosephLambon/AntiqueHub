using AntiqueHub.Core.Models;
using AntiqueHub.Api.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddDbContext(builder.Configuration, builder.Environment)
    .RegisterServices()
    .AddBlobStorageService(builder.Configuration, builder.Environment);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AntiqueDbContext>();
     // Apply migrations on app startup
    db.Database.Migrate();
}

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuth() lines go before anti forgery

app.UseAntiforgery();
app.RegisterAntiqueEndpoints();

app.Run();

// Enable Integration tests to access
public partial class Program { }