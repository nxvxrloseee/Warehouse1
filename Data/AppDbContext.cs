using Microsoft.EntityFrameworkCore;
using Warehouse1.Models;

namespace Warehouse1.Data;

public partial class AppDbContext : DbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // 1. Настройка составного ключа для UserRoles
        // В сгенерированном коде я не увидел явного HasKey для UserRole, 
        // а он критически важен для таблицы связки (Многие-ко-Многим)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
        });

        // 2. Настройка вычисляемого поля (Хотя в вашем авто-коде оно уже есть, 
        // лучше продублировать или убедиться, что тут нет конфликтов)
        /* modelBuilder.Entity<PriceHistory>(entity =>
        {
             entity.Property(e => e.ProductNameHash)
                   .HasComputedColumnSql("CONVERT(varbinary(32), HASHBYTES('SHA2_256', CONVERT(nvarchar(max), COALESCE(ProductName, N''))))", stored: true);
        });
        */
    }
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExternalTable> ExternalTables { get; set; }

    public virtual DbSet<ExternalTable1> ExternalTables1 { get; set; }

    public virtual DbSet<ExternalTableColumn> ExternalTableColumns { get; set; }

    public virtual DbSet<ExternalTableColumn1> ExternalTableColumns1 { get; set; }

    public virtual DbSet<Import> Imports { get; set; }

    public virtual DbSet<PlatformLink> PlatformLinks { get; set; }

    public virtual DbSet<PlatformsLink> PlatformsLinks { get; set; }

    public virtual DbSet<PriceHistory> PriceHistories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-JMVB6MO\\SQLEXPRESS;Initial Catalog=WarehouseDb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__External__3214EC07D09AF6C3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.SchemaName).HasDefaultValue("dbo");
        });

        modelBuilder.Entity<ExternalTable1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__External__3214EC077D3421E6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.SchemaName).HasDefaultValue("dbo");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ExternalTable1s).HasConstraintName("FK_ExtTables_CreatedByUser");
        });

        modelBuilder.Entity<ExternalTableColumn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__External__3214EC07E8CF93CC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ExternalTable).WithMany(p => p.ExternalTableColumns).HasConstraintName("FK__ExternalT__Exter__3C69FB99");
        });

        modelBuilder.Entity<ExternalTableColumn1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__External__3214EC0729932B26");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ExternalTable).WithMany(p => p.ExternalTableColumn1s).HasConstraintName("FK_ExtTableColumns_ExtTable");
        });

        modelBuilder.Entity<Import>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Imports__3214EC07508AA8B4");

            entity.Property(e => e.ImportedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ExternalTable).WithMany(p => p.Imports).HasConstraintName("FK_Imports_ExtTable");

            entity.HasOne(d => d.ImportedByUser).WithMany(p => p.Imports).HasConstraintName("FK_Imports_User");
        });

        modelBuilder.Entity<PlatformLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Platform__3214EC07F0C811F2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<PlatformsLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Platform__3214EC0713162FC8");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PriceHis__3214EC07823B0C66");

            entity.Property(e => e.Currency).HasDefaultValue("RUB");
            entity.Property(e => e.ProductNameHash).HasComputedColumnSql("(CONVERT([varbinary](32),hashbytes('SHA2_256',CONVERT([nvarchar](max),coalesce([ProductName],N'')))))", true);
            entity.Property(e => e.RecordedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ExternalTable).WithMany(p => p.PriceHistories).HasConstraintName("FK_PriceHistory_ExternalTable");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07EFE480C7");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0769AC10E1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
