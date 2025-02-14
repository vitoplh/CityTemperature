using CityTemperatureApp.Contracts;
using CityTemperatureApp.Contracts.Data;
using CityTemperatureApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CityTemperatureApp.Repository;

public class CityTemperatureRepository(AppDbContext context) : ICityTemperatureRepository
{
    public async Task<List<CityTemperatureDataDto>> GetCities()
    {
        return await context.Cities.AsNoTracking().ToListAsync();
    }
    
    public async Task<CityTemperatureDataDto?> GetCity(string name)
    {
        return await context.Cities.AsNoTracking().Where(c => c.CityName == name).SingleOrDefaultAsync();
    }

    public async Task<List<CityAverageTemperatureDataDto>> GetCityFilterByAverageTemperature(float? greaterThan, float? lowerThan)
    {
        if (greaterThan is null && lowerThan is null)
        {
            throw new ArgumentNullException();
        }
        
        var collection = context.Cities.AsNoTracking()
            .Select(c => new CityAverageTemperatureDataDto(c.CityName, c.AverageTemperature));

        if (greaterThan is not null)
        {
            collection = collection.Where(c => c.AverageTemperature > greaterThan);
        }

        if (lowerThan is not null)
        {
            collection = collection.Where(c => c.AverageTemperature < lowerThan);
        }
        
        return await collection.ToListAsync();
    }

    public async Task<int> UpdateCities(List<CityTemperatureDataDto> cities)
    {
        context.Cities.RemoveRange(context.Cities);
        var numberOfRowsDeleted = await context.SaveChangesAsync();
        await context.Cities.AddRangeAsync(cities);
        var numberOfRowsUpdated = await context.SaveChangesAsync();

        return numberOfRowsUpdated;
    }
}