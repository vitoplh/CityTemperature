using CityTemperatureApp.ApiEndpoints;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using CityTemperatureApp.Infrastructure.Configuration;
using CityTemperatureApp.Infrastructure.Database;
using CityTemperatureApp.Repository;
using CityTemperatureApp.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DataImportServiceOptions>(
    builder.Configuration.GetSection(DataImportServiceOptions.DataImportService));
builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.Authentication));

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("testDb"));

builder.Services.AddScoped<ICityTemperatureRepository, CityTemperatureRepository>();
builder.Services.AddSingleton<IDataImportService, DataImportService>();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

var dataImportService = app.Services.GetRequiredService<IDataImportService>();
if (dataImportService.RefreshData() != DataImportServiceResponse.Accepted)
{
    throw new Exception("Failed to start city initialization service");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("City Temperature API")
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.MapCities();

app.Run();


