using EcmDocs.Data.Entities;

namespace EcmDocs.Services.Interfaces;

public interface IDocumentService
{
    Task<Document> CreateAsync(Guid folderId, Guid documentTypeId, string fileName, string contentType, Stream fileStream, long fileSizeBytes, IDictionary<Guid, object?> metadataValues, CancellationToken ct = default);

    Task<(IReadOnlyList<Document> items, int totalCount)> SearchAsync(Guid? folderId, Guid? documentTypeId, string? textQuery, int page, int pageSize, CancellationToken ct = default);
}

