using EcmDocs.Data.Entities;

namespace EcmDocs.Services.Interfaces;

public interface IFolderService
{
    Task<Folder> CreateAsync(string name, Guid? parentFolderId, CancellationToken ct = default);
    Task<IReadOnlyList<Folder>> GetTreeAsync(CancellationToken ct = default);
}

