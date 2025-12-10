using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse1.Models;

[Index("SchemaName", "TableName", Name = "UX_ExternalTables_Schema_Table", IsUnique = true)]
public partial class ExternalTable
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string SchemaName { get; set; } = null!;

    [StringLength(256)]
    public string TableName { get; set; } = null!;

    [StringLength(256)]
    public string? DisplayName { get; set; }

    public bool HasHeaderRow { get; set; }

    [StringLength(256)]
    public string? NameColumn { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Notes { get; set; }

    [InverseProperty("ExternalTable")]
    public virtual ICollection<ExternalTableColumn> ExternalTableColumns { get; set; } = new List<ExternalTableColumn>();
}
