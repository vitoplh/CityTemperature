using CityTemperatureApp.Contracts.Data;

namespace CityTemperatureApp.Repository;

public interface ICityTemperatureRepository
{
    Task<List<CityTemperatureDataDto>> GetCitiesAsync();
    
    Task<CityTemperatureDataDto?> GetCity(string name);

    Task<List<CityAverageTemperatureDataDto>> GetCityFilterByAverageTemperature(float? greaterThan, float? lowerThan);
    
    Task<int> UpdateCities(List<CityTemperatureDataDto> cities);
}