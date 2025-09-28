using EcmDocs.Data;
using EcmDocs.Data.Entities;
using EcmDocs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcmDocs.Services.Implementations;

public class DocumentService : IDocumentService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    private const string StorageRootFolderName = "storage";

    public DocumentService(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<Document> CreateAsync(
        Guid folderId,
        Guid documentTypeId,
        string fileName,
        string contentType,
        Stream fileStream,
        long fileSizeBytes,
        IDictionary<Guid, object?> metadataValues,
        CancellationToken ct = default)
    {
        var folder = await _db.Folders.FirstOrDefaultAsync(f => f.Id == folderId, ct)
                     ?? throw new InvalidOperationException("Folder not found");
        var docType = await _db.DocumentTypes.Include(t => t.MetadataDefinitions)
            .FirstOrDefaultAsync(t => t.Id == documentTypeId, ct)
            ?? throw new InvalidOperationException("Document type not found");

        // Validate metadata values
        foreach (var def in docType.MetadataDefinitions)
        {
            metadataValues.TryGetValue(def.Id, out var provided);
            if (def.IsRequired && (provided is null || (provided is string s && string.IsNullOrWhiteSpace(s))))
            {
                throw new InvalidOperationException($"Metadata '{def.Key}' is required");
            }
        }

        var storageRoot = Path.Combine(_env.ContentRootPath, StorageRootFolderName);
        Directory.CreateDirectory(storageRoot);

        // Create folder path hierarchy by Ids to avoid invalid chars
        var folderPath = Path.Combine(storageRoot, folder.Id.ToString());
        Directory.CreateDirectory(folderPath);

        var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var physicalPath = Path.Combine(folderPath, uniqueName);
        await using (var fs = File.Create(physicalPath))
        {
            await fileStream.CopyToAsync(fs, ct);
        }

        var relativePath = Path.GetRelativePath(_env.ContentRootPath, physicalPath);

        var document = new Document
        {
            FolderId = folder.Id,
            DocumentTypeId = docType.Id,
            FileName = fileName,
            StoragePath = relativePath.Replace('\\', '/'),
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Documents.Add(document);
        await _db.SaveChangesAsync(ct);

        // Persist metadata values
        foreach (var def in docType.MetadataDefinitions)
        {
            metadataValues.TryGetValue(def.Id, out var value);
            var mv = new MetadataValue
            {
                DocumentId = document.Id,
                MetadataDefinitionId = def.Id
            };
            switch (def.DataType)
            {
                case MetadataDataType.Text:
                    mv.ValueText = value?.ToString();
                    break;
                case MetadataDataType.Date:
                    if (value is DateTime dt) mv.ValueDate = dt;
                    else if (DateTime.TryParse(value?.ToString(), out var parsed)) mv.ValueDate = parsed;
                    break;
                case MetadataDataType.Number:
                    if (value is decimal dec) mv.ValueNumber = dec;
                    else if (decimal.TryParse(value?.ToString(), out var parsedDec)) mv.ValueNumber = parsedDec;
                    break;
            }
            _db.MetadataValues.Add(mv);
        }

        await _db.SaveChangesAsync(ct);
        return document;
    }

    public async Task<(IReadOnlyList<Document> items, int totalCount)> SearchAsync(
        Guid? folderId,
        Guid? documentTypeId,
        string? textQuery,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _db.Documents.AsQueryable();
        if (folderId.HasValue) query = query.Where(d => d.FolderId == folderId.Value);
        if (documentTypeId.HasValue) query = query.Where(d => d.DocumentTypeId == documentTypeId.Value);
        if (!string.IsNullOrWhiteSpace(textQuery))
        {
            var t = textQuery.Trim();
            query = query.Where(d => d.FileName.Contains(t));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(d => d.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
        return (items, total);
    }
}

