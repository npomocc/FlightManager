using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flight.Domain;

[Table("flights")]
public class AirFlight
{
    [Key]
    public int Id { get; set; }
    [MaxLength(256)]
    public string Origin { get; set; }
    [MaxLength(256)]
    public string Destination { get; set; }
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public Status Status { get; set; }
}