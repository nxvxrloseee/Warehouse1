using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse1.Models;

[Table("Users", Schema = "auth")]
[Index("Username", Name = "UQ__Users__536C85E4654450D1", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(200)]
    public string? FullName { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<ExternalTable1> ExternalTable1s { get; set; } = new List<ExternalTable1>();

    [InverseProperty("ImportedByUser")]
    public virtual ICollection<Import> Imports { get; set; } = new List<Import>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
