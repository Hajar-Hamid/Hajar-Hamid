using System.ComponentModel.DataAnnotations;

namespace EcmDocs.Data.Entities;

public class Document
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string StoragePath { get; set; } = string.Empty; // relative path under storage root

    [Required]
    public long FileSizeBytes { get; set; }

    [MaxLength(200)]
    public string? ContentType { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    [Required]
    public Guid FolderId { get; set; }
    public Folder? Folder { get; set; }

    [Required]
    public Guid DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }

    public ICollection<MetadataValue> MetadataValues { get; set; } = new List<MetadataValue>();
}

