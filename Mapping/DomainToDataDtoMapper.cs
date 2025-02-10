using CityTemperatureApp.Contracts.Data;
using CityTemperatureApp.Models;

namespace CityTemperatureApp.Mapping;

public static class DomainToDataDtoMapper
{
    public static CityTemperatureDataDto ToDto(this CityTemperature cityTemperature)
    {
        var cityDataDto = new CityTemperatureDataDto()
        {
            CityName = cityTemperature.CityName,
            MinimumTemperature = cityTemperature.MinimumTemperature,
            MaximumTemperature = cityTemperature.MaximumTemperature,
            AverageTemperature = cityTemperature.AverageTemperature
        };
        return cityDataDto;
    }
}