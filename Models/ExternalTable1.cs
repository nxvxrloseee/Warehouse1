using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Warehouse1.Models;

[Table("ExternalTables", Schema = "ext")]
[Index("SchemaName", "TableName", Name = "UX_ExtTables_Schema_Table", IsUnique = true)]
public partial class ExternalTable1
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

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("ExternalTable1s")]
    public virtual User? CreatedByUser { get; set; }

    [InverseProperty("ExternalTable")]
    public virtual ICollection<ExternalTableColumn1> ExternalTableColumn1s { get; set; } = new List<ExternalTableColumn1>();

    [InverseProperty("ExternalTable")]
    public virtual ICollection<Import> Imports { get; set; } = new List<Import>();

    [InverseProperty("ExternalTable")]
    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
}
