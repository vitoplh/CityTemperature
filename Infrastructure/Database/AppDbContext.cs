using CityTemperatureApp.Contracts;
using CityTemperatureApp.Contracts.Data;
using Microsoft.EntityFrameworkCore;

namespace CityTemperatureApp.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CityTemperatureDataDto> Cities { get; set; }
}