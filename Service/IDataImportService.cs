namespace CityTemperatureApp.Service;

public interface IDataImportService
{
    public DataImportServiceState State { get;}
    DataImportServiceResponse RefreshData();
}