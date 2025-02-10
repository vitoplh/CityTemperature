using CityTemperatureApp.ApiEndpoints;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using CityTemperatureApp.Infrastructure.Configuration;
using CityTemperatureApp.Infrastructure.Database;
using CityTemperatureApp.Repository;
using CityTemperatureApp.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.Configure<DataImportServiceOptions>(
    builder.Configuration.GetSection(DataImportServiceOptions.DataImportService));
builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.Authentication));

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("testDb"));

builder.Services.AddScoped<ICityTemperatureRepository, CityTemperatureTemperatureRepository>();
builder.Services.AddSingleton<IDataImportService, DataImportService>();
builder.Services.AddHostedService<StartupService>();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.ResponseBodyLogLimit = 1028;
    logging.CombineLogs = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("City Temperature API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
    //app.UseHttpLogging();
}
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.MapCities();

app.Run();


