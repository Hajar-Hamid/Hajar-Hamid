using EcmDocs.Data;
using EcmDocs.Data.Entities;
using EcmDocs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcmDocs.Services.Implementations;

public class FolderService : IFolderService
{
    private readonly AppDbContext _db;

    public FolderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Folder> CreateAsync(string name, Guid? parentFolderId, CancellationToken ct = default)
    {
        var folder = new Folder { Name = name, ParentFolderId = parentFolderId };
        _db.Folders.Add(folder);
        await _db.SaveChangesAsync(ct);
        return folder;
    }

    public async Task<IReadOnlyList<Folder>> GetTreeAsync(CancellationToken ct = default)
    {
        var all = await _db.Folders
            .Include(f => f.Children)
            .AsNoTracking()
            .ToListAsync(ct);
        return all;
    }
}

