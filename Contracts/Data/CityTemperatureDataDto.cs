using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityTemperatureApp.Contracts.Data;

public class CityTemperatureDataDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required string CityName { get; set; }
    public float MinimumTemperature { get; set; }
    public float MaximumTemperature { get; set; }
    public float AverageTemperature { get; set; }
}