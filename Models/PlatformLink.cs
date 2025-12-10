using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse1.Models;

[Table("PlatformLinks", Schema = "app")]
[Index("Name", Name = "UQ__Platform__737584F60EDB5861", IsUnique = true)]
public partial class PlatformLink
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(2000)]
    public string UrlTemplate { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
