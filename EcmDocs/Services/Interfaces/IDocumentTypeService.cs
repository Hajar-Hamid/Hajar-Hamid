using EcmDocs.Data.Entities;

namespace EcmDocs.Services.Interfaces;

public interface IDocumentTypeService
{
    Task<IReadOnlyList<DocumentType>> GetAllAsync(CancellationToken ct = default);
    Task<DocumentType?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DocumentType> CreateAsync(string code, string name, IEnumerable<(string key, MetadataDataType type, bool required)> metadata, CancellationToken ct = default);
    Task<MetadataDefinition> AddMetadataAsync(Guid documentTypeId, string key, MetadataDataType type, bool required, CancellationToken ct = default);
}

