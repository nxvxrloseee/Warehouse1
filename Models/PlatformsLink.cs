using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Warehouse1.Models;

[Index("Name", Name = "UX_PlatformsLinks_Name", IsUnique = true)]
public partial class PlatformsLink
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string UrlTemplate { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
