using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.API.Extensions;
using SmartCollegeComplaintPortal.Business;
using SmartCollegeComplaintPortal.Business.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog();
builder.Services.AddControllers();

builder.Services.AddBusinessLayer(builder.Configuration);
builder.Services.AddRateLimiting(builder.Environment);

builder.Services.Configure<AzureBlobStorageOptions>(
    builder.Configuration.GetSection(
        AzureBlobStorageOptions.SectionName));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage)
        .ToList();

        var response = new ErrorResponse(false, 400, null, errors);
        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerPolicy(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsProduction())
{

    app.ApplyMigrations();
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.UseCustomMiddlewares();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwaggerGate();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
