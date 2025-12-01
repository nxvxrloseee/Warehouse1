using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Warehouse1.Models;

[Index("ExternalTableId", Name = "IX_ExtTblCols_ExternalTableId")]
public partial class ExternalTableColumn
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
    [InverseProperty("ExternalTableColumns")]
    public virtual ExternalTable ExternalTable { get; set; } = null!;
}
