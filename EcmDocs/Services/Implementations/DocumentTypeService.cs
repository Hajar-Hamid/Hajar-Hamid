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

    public async Task<DocumentType> CreateAsync(string code, string name, IEnumerable<(string key, MetadataDataType type, bool required)> metadata, CancellationToken ct = default)
    {
        code = code.Trim();
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Code et nom sont requis");

        if (await _db.DocumentTypes.AnyAsync(t => t.Code == code, ct))
            throw new InvalidOperationException("Ce code existe déjà");

        var type = new DocumentType { Code = code, Name = name };
        _db.DocumentTypes.Add(type);
        await _db.SaveChangesAsync(ct);

        foreach (var (key, dt, req) in metadata)
        {
            await AddMetadataAsync(type.Id, key, dt, req, ct);
        }

        return (await GetByIdAsync(type.Id, ct))!;
    }

    public async Task<MetadataDefinition> AddMetadataAsync(Guid documentTypeId, string key, MetadataDataType type, bool required, CancellationToken ct = default)
    {
        key = key.Trim();
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Clé requise");

        var exists = await _db.MetadataDefinitions.AnyAsync(m => m.DocumentTypeId == documentTypeId && m.Key == key, ct);
        if (exists) throw new InvalidOperationException("Cette clé de métadonnée existe déjà pour ce type");

        var md = new MetadataDefinition
        {
            DocumentTypeId = documentTypeId,
            Key = key,
            DataType = type,
            IsRequired = required
        };
        _db.MetadataDefinitions.Add(md);
        await _db.SaveChangesAsync(ct);
        return md;
    }
}

