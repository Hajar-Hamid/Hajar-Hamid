EcmDocs

Blazor Web App (Server interactivity) pour créer et classer des documents avec métadonnées dynamiques, stockage PostgreSQL et hiérarchie de dossiers.

Prérequis
- .NET 8 SDK
- PostgreSQL (local ou distant)

Configuration
Modifiez `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=ecmdocs;Username=postgres;Password=postgres"
  }
}
```

Démarrage
```bash
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$PATH
cd /workspace/EcmDocs
dotnet build
dotnet run
```

Au premier lancement, la base sera migrée et les types de documents + métadonnées seront injectés, avec un dossier racine.

Fonctionnalités
- Création de dossiers: page `/folders`
- Upload de documents: page `/upload`
  - Sélection du dossier
  - Sélection du type de document
  - Saisie des métadonnées selon le type (texte, date, nombre)
  - Dépôt de n'importe quel fichier; stockage sur disque dans `storage/<folderId>/...` et indexation en base

Structure
- `Data/Entities`: `Folder`, `DocumentType`, `MetadataDefinition`, `Document`, `MetadataValue`, `MetadataDataType`
- `Data/AppDbContext`: EF Core + Npgsql
- `Data/Seed/SeedData`: données initiales (9 types + métadonnées)
- `Services`: `IFolderService`, `IDocumentTypeService`, `IDocumentService` et implémentations
- `Components/Pages`: `DocumentUpload.razor`, `FolderManager.razor`

Notes
- Les migrations nécessitent un PostgreSQL accessible. Pour les créer/appliquer:
```bash
export PATH=$HOME/.dotnet/tools:$PATH
dotnet tool install --global dotnet-ef --version 8.0.8
dotnet ef migrations add InitialCreate
dotnet ef database update
```
- Le stockage fichier utilise le dossier `storage` sous le répertoire de l'application.

