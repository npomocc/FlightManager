using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flight.Domain;

[Table("roles")]
[Index(nameof(Code), IsUnique = true)]
public class Role
{
    [Key]
    public int Id { get; set; }
    [MaxLength(256)]
    public string Code { get; set; }
}