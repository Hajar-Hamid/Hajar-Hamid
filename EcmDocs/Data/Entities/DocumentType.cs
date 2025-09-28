using System.ComponentModel.DataAnnotations;

namespace EcmDocs.Data.Entities;

public class DocumentType
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Code { get; set; } = string.Empty; // e.g., ECM_ModelQuestionnaire

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = string.Empty; // human label

    public ICollection<MetadataDefinition> MetadataDefinitions { get; set; } = new List<MetadataDefinition>();
}

