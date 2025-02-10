namespace CityTemperatureApp.Infrastructure.Configuration;

public class DataImportServiceOptions
{
    public const string DataImportService = "DataImportService";
    
    public string AbsoluteFilePath { get; set; } = String.Empty;
}