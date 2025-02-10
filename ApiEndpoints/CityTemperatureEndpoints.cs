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
            .WithSummary("Get cities with an average temperature within the specified range.")
            .WithDescription("This endpoint returns cities filtered by average temperature within the specified range.")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        group.MapPut("/", RefreshData)
            .WithSummary("Refresh data")
            .WithDescription("This endpoint triggers a data refresh by re-importing the data file.");
            
        return group;
    }
    
    static async Task<Results<Ok<List<CityTemperatureDto>>, NotFound>> GetCities(ICityTemperatureRepository cityTemperatureRepository)
    {
        var cities = await cityTemperatureRepository.GetCitiesAsync();
        var citiesResponse = cities.Select(c => c.ToDto()).ToList();
        
        return cities.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(citiesResponse);
    }
    
    static async Task<Results<Ok<CityTemperatureDto>, NotFound>> GetCityByName(string cityName, ICityTemperatureRepository cityTemperatureRepository)
    {
        var city = await cityTemperatureRepository.GetCity(cityName);
        
        return city is null ? TypedResults.NotFound() : TypedResults.Ok(city.ToDto());
    }
    
    static async Task<Results<Ok<List<CityAverageTemperatureDto>>, NotFound, BadRequest<ProblemDetails>>> 
        GetCityFilterByAverageTemperature(float? greaterThan, float? lowerThan, ICityTemperatureRepository cityTemperatureRepository)
    {
        if (greaterThan is null && lowerThan is null)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "No arguments provided",
            });
        }

        if (greaterThan is not null && lowerThan is not null && greaterThan < lowerThan)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = "Greater value cannot be lower than lower value.",
            });
        }
        
        var cities = await cityTemperatureRepository.GetCityFilterByAverageTemperature(greaterThan, lowerThan);
        var citiesResponse = cities.Select(c => c.ToFilteredDto()).ToList();
        
        return cities.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(citiesResponse);
    }

    static Results<Ok, ProblemHttpResult> RefreshData(IDataImportService dataImportService, ICityTemperatureRepository cityTemperatureRepository)
    {
        var result = dataImportService.RefreshData();
        return result switch
        {
            DataImportServiceResponse.Accepted => TypedResults.Ok(),
            DataImportServiceResponse.Unavailable => TypedResults.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Data Import Service Unavailable",
                Detail = "Data import service is currently processing a request."
            }),
            _ => throw new NotSupportedException($"Unknown data import service result type: {result.GetType().Name}")
        };
    }
}