using System.ComponentModel;

namespace CityTemperatureApp.Contracts.Api;

public record CityAverageTemperatureDto(
        [Description("Name of the city.")]
        string CityName, 
        [Description("Calculated average temperature in the city.")]
        float AverageTemperature
    );
