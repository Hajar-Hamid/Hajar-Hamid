using EcmDocs.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcmDocs.Data.Seed;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.DocumentTypes.AnyAsync())
        {
            var types = new List<DocumentType>
            {
                new() { Code = "ECM_ModelQuestionnaire", Name = "Fichier modèle de questionnaire" },
                new() { Code = "ECM_DICT", Name = "Fichier de dictionnaire des données" },
                new() { Code = "ECM_BonnePratiques", Name = "Liste des recommandations" },
                new() { Code = "ECM_Donnees", Name = "Fichier type SQL (oracle) - enquête trimestrielle" },
                new() { Code = "CPS", Name = "CPS Cahier" },
                new() { Code = "CCAG", Name = "Cahier des clauses admin" },
                new() { Code = "CCAP", Name = "Cahier des clauses" },
                new() { Code = "FAX", Name = "Acte d'engagement (FAX)" },
                new() { Code = "LivrableZip", Name = "Livrable (.zip)" },
            };
            db.DocumentTypes.AddRange(types);
            await db.SaveChangesAsync();

            var defMap = new Dictionary<string, (string key, MetadataDataType type, bool req)[]>
            {
                ["ECM_ModelQuestionnaire"] = new []
                {
                    ("date_creation", MetadataDataType.Date, true),
                    ("date_modification", MetadataDataType.Date, false),
                    ("version", MetadataDataType.Text, true),
                    ("auteurs", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                },
                ["ECM_DICT"] = new []
                {
                    ("date_creation", MetadataDataType.Date, true),
                    ("date_modification", MetadataDataType.Date, false),
                    ("version", MetadataDataType.Text, true),
                    ("auteurs", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                },
                ["ECM_BonnePratiques"] = new []
                {
                    ("date_creation", MetadataDataType.Date, true),
                    ("date_modification", MetadataDataType.Date, false),
                    ("version", MetadataDataType.Text, true),
                    ("auteurs", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                },
                ["ECM_Donnees"] = new []
                {
                    ("date_creation", MetadataDataType.Date, true),
                    ("date_modification", MetadataDataType.Date, false),
                    ("version", MetadataDataType.Text, true),
                    ("trimestre", MetadataDataType.Text, true),
                    ("region", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                },
                ["CPS"] = new []
                {
                    ("date", MetadataDataType.Date, true),
                    ("version", MetadataDataType.Text, true),
                    ("auteurs", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                    ("objet_marche", MetadataDataType.Text, true),
                    ("numero", MetadataDataType.Text, false),
                    ("mots_cles", MetadataDataType.Text, false),
                },
                ["CCAG"] = new []
                {
                    ("date_creation", MetadataDataType.Date, true),
                    ("version", MetadataDataType.Text, true),
                    ("auteurs", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                    ("objet_marche", MetadataDataType.Text, true),
                },
                ["CCAP"] = new []
                {
                    ("date_creation", MetadataDataType.Date, true),
                    ("version", MetadataDataType.Text, true),
                    ("auteurs", MetadataDataType.Text, false),
                    ("etat", MetadataDataType.Text, false),
                    ("objet_marche", MetadataDataType.Text, true),
                },
                ["FAX"] = new []
                {
                    ("emetteur", MetadataDataType.Text, true),
                    ("recepteur", MetadataDataType.Text, true),
                    ("date_reception", MetadataDataType.Date, true),
                    ("objet", MetadataDataType.Text, true),
                },
                ["LivrableZip"] = new []
                {
                    ("date", MetadataDataType.Date, true),
                    ("version", MetadataDataType.Text, true),
                    ("fournisseur", MetadataDataType.Text, true),
                    ("etat", MetadataDataType.Text, false),
                    ("objet", MetadataDataType.Text, true),
                    ("mots_cles", MetadataDataType.Text, false),
                },
            };

            var typesByCode = await db.DocumentTypes.ToDictionaryAsync(t => t.Code, t => t.Id);
            foreach (var kvp in defMap)
            {
                var typeId = typesByCode[kvp.Key];
                foreach (var def in kvp.Value)
                {
                    db.MetadataDefinitions.Add(new MetadataDefinition
                    {
                        DocumentTypeId = typeId,
                        Key = def.key,
                        DataType = def.type,
                        IsRequired = def.req
                    });
                }
            }
            await db.SaveChangesAsync();
        }

        if (!await db.Folders.AnyAsync())
        {
            db.Folders.Add(new Folder { Name = "Racine" });
            await db.SaveChangesAsync();
        }
    }
}

