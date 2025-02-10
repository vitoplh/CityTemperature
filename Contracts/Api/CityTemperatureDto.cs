using System.ComponentModel;

namespace CityTemperatureApp.Contracts.Api;

public record CityTemperatureDto(
    [Description("Name of the city.")]
    string CityName, 
    [Description("Lowest measured average temperature in the city.")]
    float MinimumTemperature,
    [Description("Highest measured temperature in the city.")]
    float MaximumTemperature,
    [Description("Calculated average temperature in the city.")]
    float AverageTemperature
    );