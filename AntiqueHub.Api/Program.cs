using Microsoft.Extensions.FileProviders;
using AntiqueHub.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext(builder.Configuration, builder.Environment)
    .RegisterServices()
    .AddBlobStorageService(builder.Configuration, builder.Environment);

var app = builder.Build();

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