using System.ComponentModel.DataAnnotations;

namespace CityTemperatureApp.Models;

public class CityTemperature(string cityName, float temperature, int counter = 1)
{
    private int _counter = counter;
    public string CityName { get; private set;  } = cityName;

    public float MinimumTemperature { get; private set; } = temperature;
    public float MaximumTemperature { get; private set; } = temperature;
    public float AverageTemperature { get; private set; } = temperature;

    public void AddMeasurement(float temperature)
    {
        MinimumTemperature = MinimumTemperature > temperature ? temperature : MinimumTemperature;
        MaximumTemperature = MaximumTemperature < temperature ? temperature : MaximumTemperature;
        _counter++;
        AverageTemperature += (temperature - AverageTemperature) / _counter;
    }
}