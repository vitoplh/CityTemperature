using CityTemperatureApp.Contracts.Api;
using CityTemperatureApp.Contracts.Data;

namespace CityTemperatureApp.Mapping;

public static class DataDtoToApiDtoMapper
{
    public static CityTemperatureDto ToDto(this CityTemperatureDataDto cityData)
    {
        return new CityTemperatureDto(
            cityData.CityName,
            cityData.MinimumTemperature,
            cityData.MaximumTemperature,
            cityData.AverageTemperature);
    }
    
    public static CityAverageTemperatureDto ToFilteredDto(this CityAverageTemperatureDataDto cityData)
    {
        return new CityAverageTemperatureDto(
            cityData.CityName,
            cityData.AverageTemperature);
    }
}