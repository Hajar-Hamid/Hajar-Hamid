using System.ComponentModel.DataAnnotations;

namespace EcmDocs.Data.Entities;

public class MetadataValue
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid DocumentId { get; set; }
    public Document? Document { get; set; }

    [Required]
    public Guid MetadataDefinitionId { get; set; }
    public MetadataDefinition? MetadataDefinition { get; set; }

    // Store values in separate columns for querying while keeping model simple
    [MaxLength(2000)]
    public string? ValueText { get; set; }
    public DateTime? ValueDate { get; set; }
    public decimal? ValueNumber { get; set; }
}

