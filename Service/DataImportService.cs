using System.Globalization;
using System.Text;
using CityTemperatureApp.Infrastructure.Configuration;
using CityTemperatureApp.Models;
using CityTemperatureApp.Repository;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using CityTemperatureApp.Mapping;

namespace CityTemperatureApp.Service;

public class DataImportService(
    IOptions<DataImportServiceOptions> options,
    ILogger<DataImportService> logger,
    IServiceProvider serviceProvider)
    : IDataImportService
{
    private DataImportServiceState _state = DataImportServiceState.Uninitialized;
    private readonly string _filePath = options.Value.AbsoluteFilePath;

    public DataImportServiceResult RefreshData()
    {
        if (_state == DataImportServiceState.Processing)
        {
            return DataImportServiceResult.Unavailable;
        }
        Task.Run(ProcessFile);
        return DataImportServiceResult.Accepted;
    }

    private async Task ProcessFile()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var cityRepository = scope.ServiceProvider.GetService<ICityTemperatureRepository>();

            if (cityRepository == null)
            {
                throw new NullReferenceException("City temperature repository is null");
            }
            
            _state = DataImportServiceState.Processing;
            logger.LogInformation("Processing data file: {filePath}", _filePath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            };

            using var reader = new StreamReader(_filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            var records = csv.GetRecords<Measurement>();
            var listOfCities = new Dictionary<string, CityTemperature>();
            
            foreach (var record in records)
            {
                var cityFound = listOfCities.TryGetValue(record.City, out var city);
    
                if (!cityFound)
                {
                    listOfCities.Add(record.City, new CityTemperature(record.City, record.Temperature));
                }
                else
                {
                    city!.AddMeasurement(record.Temperature);
                }
            }
            var dtoList = listOfCities.Select(kvp => kvp.Value.ToDto()).ToList();
            var rowsStored = await cityRepository.UpdateCities(dtoList);
            logger.LogInformation("Processing data file completed in {Time} - {Rows} records have been stored", watch.Elapsed, rowsStored);
            _state = DataImportServiceState.Available;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception message {msg}", ex.Message);
        }
    }
}