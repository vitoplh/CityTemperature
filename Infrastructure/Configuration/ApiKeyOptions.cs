namespace CityTemperatureApp.Infrastructure.Configuration;

public class AuthenticationOptions
{
    public const string Authentication = "Authentication";
    
    public string ApiKey { get; set; } = String.Empty;
}