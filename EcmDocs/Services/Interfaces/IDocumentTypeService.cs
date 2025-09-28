using EcmDocs.Data.Entities;

namespace EcmDocs.Services.Interfaces;

public interface IDocumentTypeService
{
    Task<IReadOnlyList<DocumentType>> GetAllAsync(CancellationToken ct = default);
    Task<DocumentType?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

