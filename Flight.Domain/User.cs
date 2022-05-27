using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flight.Domain;

[Table("users")]
[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    [MaxLength(256)]
    public string UserName { get; set; }
    [MaxLength(256)]
    public string Password { get; set; }
    public int RoleId { get; set; }
}