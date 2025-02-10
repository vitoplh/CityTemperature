using CsvHelper.Configuration.Attributes;
using static System.String;

namespace CityTemperatureApp.Models;

public class Measurement
{
    [Index(0)]
    public string City { get; set; } = Empty;
    [Index(1)]
    public float Temperature { get; set; }
}