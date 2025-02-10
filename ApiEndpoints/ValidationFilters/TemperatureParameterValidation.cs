using Microsoft.AspNetCore.Mvc;

namespace CityTemperatureApp.ApiEndpoints.ValidationFilters;

public class TemperatureParameterValidation() : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext, 
        EndpointFilterDelegate next)
    {
        var greaterThan = efiContext.GetArgument<float?>(0);
        var lowerThan = efiContext.GetArgument<float?>(1);
        
        if (greaterThan is null && lowerThan is null)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "No arguments provided",
            });
        }
        
        if (greaterThan is not null && lowerThan is not null && greaterThan > lowerThan)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = "Greater than cannot be lower than value.",
            });
        }
        
        return await next(efiContext);
    }
}