using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse1.Models;

[Table("ExternalTableColumns", Schema = "ext")]
[Index("ExternalTableId", Name = "IX_ExtTableColumns_ExtTableId")]
public partial class ExternalTableColumn1
{
    [Key]
    public int Id { get; set; }

    public int ExternalTableId { get; set; }

    [StringLength(256)]
    public string ColumnName { get; set; } = null!;

    [StringLength(256)]
    public string? DisplayName { get; set; }

    public bool IsNameColumn { get; set; }

    public int? OrdinalPosition { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ExternalTableId")]
    [InverseProperty("ExternalTableColumn1s")]
    public virtual ExternalTable1 ExternalTable { get; set; } = null!;
}
