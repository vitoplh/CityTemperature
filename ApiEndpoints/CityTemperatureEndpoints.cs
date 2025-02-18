using CityTemperatureApp.ApiEndpoints.ValidationFilters;
using CityTemperatureApp.Contracts.Api;
using CityTemperatureApp.Infrastructure.Authentication;
using CityTemperatureApp.Mapping;
using CityTemperatureApp.Repository;
using CityTemperatureApp.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CityTemperatureApp.ApiEndpoints;

internal static class CityTemperatureEndpoints
{
    public static IEndpointRouteBuilder MapCities(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cities")
            .ProducesProblem(statusCode: 500)
            .AddEndpointFilter<ApiKeyFilter>();
        
        group.MapGet("/", GetCities)
            .WithSummary("Get all cities and their min/max/avg temperatures.")
            .WithDescription("This endpoint returns all the cities and their min/max/avg temperatures.")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        group.MapGet("/{cityName}", GetCityByName)
            .WithSummary("Get a city and their min/max/avg temperatures by name.")
            .WithDescription("This endpoint returns the named city and its min/max/avg temperatures.")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        group.MapGet("/averageTemperature", GetCityFilterByAverageTemperature)
            .AddEndpointFilter<TemperatureParameterValidation>()
            .WithSummary("Get cities with an average temperature within the specified range.")
            .WithDescription("This endpoint returns cities filtered by average temperature within the specified range.")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        group.MapPut("/", RefreshData)
            .WithSummary("Refresh data")
            .WithDescription("This endpoint triggers a data refresh by re-importing the data file.");
        group.MapGet("/importStatus", GetImportStatus)
            .WithSummary("Status of the data refresh")
            .WithDescription("This endpoint triggers checks the state of the import service");
            
        return group;
    }
    
    static async Task<Ok<IEnumerable<CityTemperatureDto>>> GetCities(ICityTemperatureRepository cityTemperatureRepository)
    {
        var cities = await cityTemperatureRepository.GetCities();
        var citiesResponse = cities.Select(c => c.ToDto()).ToList();
        
        return TypedResults.Ok<IEnumerable<CityTemperatureDto>>(citiesResponse);
    }
    
    static async Task<Results<Ok<CityTemperatureDto>, NotFound>> GetCityByName(string cityName, ICityTemperatureRepository cityTemperatureRepository)
    {
        var city = await cityTemperatureRepository.GetCity(cityName);
        return city is null ? TypedResults.NotFound() : TypedResults.Ok(city.ToDto());
    }
    
    static async Task<Results<Ok<List<CityAverageTemperatureDto>>, NotFound, BadRequest<ProblemDetails>>> 
        GetCityFilterByAverageTemperature(float? greaterThan, float? lowerThan, ICityTemperatureRepository cityTemperatureRepository)
    {
        var cities = await cityTemperatureRepository.GetCityFilterByAverageTemperature(greaterThan, lowerThan);
        var citiesResponse = cities.Select(c => c.ToFilteredDto()).ToList();
        
        return cities.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(citiesResponse);
    }

    static Results<Accepted, ProblemHttpResult> RefreshData(IDataImportService dataImportService)
    {
        var result = dataImportService.RefreshData();
        return result switch
        {
            DataImportServiceResponse.Accepted => TypedResults.Accepted("/importStatus"),
            DataImportServiceResponse.Unavailable => TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Data Import Service Unavailable",
                Detail = "Data import service is currently processing a file."
            }),
            _ => throw new NotSupportedException($"Unknown data import service result type: {result.GetType().Name}")
        };
    }

    static Results<Ok, ProblemHttpResult> GetImportStatus(IDataImportService dataImportService)
    {
        var state = dataImportService.State;
        return state switch
        {
            DataImportServiceState.Available => TypedResults.Ok(),
            DataImportServiceState.Processing => TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "File currently being processed.",
                Detail = "Data import service is currently processing a file."
            }),
            _ => throw new NotSupportedException($"Unknown data import service state type: {state.GetType().Name}")
        };
    }
}