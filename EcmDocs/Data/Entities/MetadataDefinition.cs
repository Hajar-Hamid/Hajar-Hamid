using System.ComponentModel.DataAnnotations;

namespace EcmDocs.Data.Entities;

public class MetadataDefinition
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Key { get; set; } = string.Empty; // e.g., date_creation, version

    [Required]
    public MetadataDataType DataType { get; set; }

    public bool IsRequired { get; set; } = false;

    public Guid DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }
}

