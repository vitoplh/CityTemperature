namespace CityTemperatureApp.Service;

public class StartupService(IDataImportService dataImportService) : IHostedService
{
    private readonly IDataImportService _dataImportService = dataImportService;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var result = _dataImportService.RefreshData();
        if (result != DataImportServiceResponse.Accepted)
        {
            throw new Exception("Failed to start city initialization service");
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}