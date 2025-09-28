using EcmDocs.Data;
using EcmDocs.Data.Entities;
using EcmDocs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcmDocs.Services.Implementations;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly AppDbContext _db;

    public DocumentTypeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DocumentType>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.DocumentTypes.Include(t => t.MetadataDefinitions).AsNoTracking().ToListAsync(ct);
    }

    public async Task<DocumentType?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.DocumentTypes.Include(t => t.MetadataDefinitions).FirstOrDefaultAsync(t => t.Id == id, ct);
    }
}

