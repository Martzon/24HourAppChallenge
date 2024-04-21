using ChallengeApp.API;
using ChallengeApp.Application;
using ChallengeApp.Infrastructure;
using ChallengeApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
       .ReadFrom.Configuration(builder.Configuration)
       .Enrich.FromLogContext()
       .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration["CorsOrigins"];
    options.AddPolicy(name: "AllowSpecific",
                      policy =>
                      {
                          policy.WithOrigins(origins.Split(","))
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                      });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowSpecific");



app.Run();
