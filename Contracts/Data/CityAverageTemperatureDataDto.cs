namespace CityTemperatureApp.Contracts.Data;

public class CityAverageTemperatureDataDto(string name, float averageTemperature)
{
    public string CityName { get; set; } = name;
    public float AverageTemperature { get; set; } = averageTemperature;
}