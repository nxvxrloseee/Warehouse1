using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Warehouse1.Models;

[Table("PriceHistory", Schema = "app")]
[Index("ProductNameHash", "RecordedAt", Name = "IX_PriceHistory_ProductNameHash_RecordedAt")]
[Index("RecordedAt", Name = "IX_PriceHistory_RecordedAt")]
public partial class PriceHistory
{
    [Key]
    public long Id { get; set; }

    public int? ExternalTableId { get; set; }

    [StringLength(1000)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal Price { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = null!;

    [StringLength(200)]
    public string? Source { get; set; }

    public DateTime RecordedAt { get; set; }

    [MaxLength(32)]
    public byte[]? ProductNameHash { get; set; }

    [ForeignKey("ExternalTableId")]
    [InverseProperty("PriceHistories")]
    public virtual ExternalTable1? ExternalTable { get; set; }
}
