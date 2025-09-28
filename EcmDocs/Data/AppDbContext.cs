using EcmDocs.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcmDocs.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<MetadataDefinition> MetadataDefinitions => Set<MetadataDefinition>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<MetadataValue> MetadataValues => Set<MetadataValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Folder>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.Children)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DocumentType>()
            .HasIndex(dt => dt.Code)
            .IsUnique();

        modelBuilder.Entity<MetadataDefinition>()
            .HasIndex(md => new { md.DocumentTypeId, md.Key })
            .IsUnique();

        modelBuilder.Entity<MetadataValue>()
            .HasIndex(mv => new { mv.DocumentId, mv.MetadataDefinitionId })
            .IsUnique();
    }
}

