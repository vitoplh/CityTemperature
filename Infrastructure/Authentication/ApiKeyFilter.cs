using CityTemperatureApp.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CityTemperatureApp.Infrastructure.Authentication;

public class ApiKeyFilter(IOptions<AuthenticationOptions> options) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext, 
        EndpointFilterDelegate next)
    {
        if (!efiContext.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
        {
            return TypedResults.Unauthorized();
        }
        
        if (apiKey != options.Value.ApiKey)
        {
            return TypedResults.Unauthorized();
        }
        
        efiContext.HttpContext.Request.Headers.Remove("X-API-KEY");
        
        return await next(efiContext);
    }
}