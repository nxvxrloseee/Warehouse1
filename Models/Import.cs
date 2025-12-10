using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse1.Models;

[Table("Imports", Schema = "app")]
[Index("ExternalTableId", Name = "IX_Imports_ExternalTableId")]
public partial class Import
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string? FileName { get; set; }

    public int? ExternalTableId { get; set; }

    public bool HasHeader { get; set; }

    public int? ImportedByUserId { get; set; }

    public DateTime ImportedAt { get; set; }

    [StringLength(100)]
    public string? Status { get; set; }

    public string? Details { get; set; }

    [ForeignKey("ExternalTableId")]
    [InverseProperty("Imports")]
    public virtual ExternalTable1? ExternalTable { get; set; }

    [ForeignKey("ImportedByUserId")]
    [InverseProperty("Imports")]
    public virtual User? ImportedByUser { get; set; }
}
