using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcmDocs.Data.Entities;

public class Folder
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid? ParentFolderId { get; set; }
    public Folder? ParentFolder { get; set; }

    public ICollection<Folder> Children { get; set; } = new List<Folder>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

