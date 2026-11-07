# REP 0022 – Artifact aliases and redirects: implementation plan

Source: Azure/bicep-reps `active/0022-artifact-aliases-and-redirects.md`. Tracking issue: #19883.

The work has two parts:

- **Phase 0** refactors how Bicep resolves, restores, stores and loads artifacts. It changes no behavior, except for the few bug fixes noted below, so that the REP's features can be added without rework.
- **Feature PRs 1–10** implement the REP itself: `oci:`, `file:`, `<alias>::`, the `artifacts` configuration section, redirects, and the `artifact-redirect` rule.

---

## 1. Status

| Phase | Summary | PR | Status |
|---|---|---|---|
| 0.1 + 0.3a | Resolution characterization tests; `ArtifactAddress` parser and `ArtifactReferenceResolver` | #20498 | Merged |
| 0.2 | Compiler and language server read resolved references from the source graph | #20496 | Merged |
| 0.3b | Alias expansion in the resolver; `moduleAliasesMock` isolated | #20502 | Merged |
| 0.4 | `ModuleDispatcher` split into resolver, restorer and publisher | #20515 | Merged |
| 0.5 | Artifact service APIs cleaned up; compiler-only redirect layer | #20518 | Merged |
| 0.6 | One `LocalArtifactReference`; local references are never restored | #20519 | Merged |
| 0.7 | External artifact context on remote references | #20523 | Merged |
| 0.8a | Registry, publish and cache cuts (includes the former 0.8c) | #20528 | Merged |
| 0.8b | Vocabulary and folders: renames and moves only | #20530 | In review |
| 0.9a | OCI payload and codecs | – | Next |
| 0.9b | Cache entries store packages | – | Planned |
| 0.9c | The compiler gets artifacts, not files | – | Planned |
| 1–10 | REP 0022 features (section 5) | – | Planned |

Restructuring the source file grouping (`ArtifactResolutionInfo`) is a separate effort (`source-graph-ownership.md`, draft).

---

## 2. Vocabulary

**Rule:** unqualified "artifact" always means a **Bicep artifact**: a module or an extension, the thing an `ArtifactReference` names and an `ArtifactType` classifies. Every other type that relates to artifacts says what role it plays with a suffix: `[Location][Kind]Artifact[Role]`. A prefix such as `Oci` or `Local` says where the artifact lives, not that it's a different kind of artifact. Generic types that only handle remote artifacts (interfaces, shared base classes, services) start with `Remote`. Concrete types start with their location (`Oci`, `TemplateSpec`, `Local`), which already says whether they're remote or local. Types that handle both have no prefix (`ArtifactCacheEntry`, `ArtifactException`, `IArtifactReferenceResolver`).

| Term | Means | Types (after 0.9) |
|---|---|---|
| **Artifact** | A module or extension, as the compiler, language server and local deploy use it | `Artifact`, `ModuleArtifact`, `ExtensionArtifact` |
| **Artifact reference** | Which artifact, and where it lives. Identity only, no I/O | `ArtifactReference`, `RemoteArtifactReference`, `OciArtifactReference`, `TemplateSpecArtifactReference`, `LocalArtifactReference` |
| **Artifact package** | An artifact's content in memory, independent of any container | `ModuleArtifactPackage`, `ExtensionArtifactPackage` |
| **Artifact codec** | Converts a package to and from one container (an OCI payload, a `.tgz`), both ways; tested by round trip | `OciModuleArtifactCodec`, `OciExtensionArtifactCodec`, `ExtensionArtifactArchiveCodec` |
| **Artifact payload** | An OCI manifest and the blobs it references, as pushed or pulled | `OciArtifactPayload` |
| **Artifact cache entry** | Where an artifact is stored on this machine; owns the on-disk layout | `ArtifactCacheEntry`, `OciModuleArtifactCacheEntry`, `OciExtensionArtifactCacheEntry`, `TemplateSpecArtifactCacheEntry`, `LocalExtensionArtifactCacheEntry` |
| **Remote artifact registry** | Restores remote artifacts; the OCI registry also publishes | `IRemoteArtifactRegistry`, `RemoteArtifactRegistry<TReference>`, `OciArtifactRegistry`, `TemplateSpecArtifactRegistry` |
| **Remote artifact restorer** | Restores remote artifacts across registries and remembers recent failures | `IRemoteArtifactRestorer`, `RemoteArtifactRestorer`, `RemoteArtifactRestoreState` |

The rule is a review convention, not an architecture test: a test strict enough to catch a new `FooArtifact` would also reject legitimate names such as `ArtifactAddress`, `RedirectingArtifactResolver` and `RemoteArtifactContextFactory`. 0.8b applied it to the types that survive 0.9; the types 0.9 deletes keep their names until then.

The user-facing term "artifact" stays: it appears in diagnostics (BCP190–BCP192 and others), in the docs, and in REP 0022 itself (`artifacts`, `artifact-redirect`). A Bicep module stored in OCI is also an OCI artifact (`artifactType` = `application/vnd.ms.bicep.module.artifact`), so the two meanings agree.

---

## 3. Target architecture

### 3.1 Resolution pipeline

Every reference the compiler loads goes through this pipeline: declarations, values in the `extensions` configuration, implicit extensions, and data-file loads. The language server reads the results from the source graph instead of resolving them again (0.2).

```
reference text
  -> ArtifactAddress.Parse
       Path | SchemeQualified(scheme, body) | SchemeAndAliasQualified(scheme, alias, body) | AliasQualified(alias, body)
  -> (1) redirect: exact match on the source spelling
  -> (2) redirect: capture match on the source spelling (most literal characters wins; a tie is an error)
  -> (3) br/<alias> with a moduleAliasesMock.br entry -> ModuleAliasMockRule -> local file (stop)
  -> (4) AliasQualified -> expand through artifacts.aliases and the predefined mar/avm (no I/O)
  ->     canonicalize through the scheme's parser (OCI, Template Spec, local) -> canonical identity
  -> (5) redirect: exact match on the canonical identity
  -> (6) redirect: capture match on the canonical identity
  -> (7) no redirect -> ArtifactReference (OciArtifactReference, TemplateSpecArtifactReference, LocalArtifactReference)
```

The pipeline is split in two:

- **`IArtifactReferenceResolver`** parses, expands aliases and builds the reference (steps 4 and 7).
- **`RedirectingArtifactResolver`**, used only by the compiler, wraps it and applies redirects and the legacy mock (steps 1–3, 5 and 6).

Publish targets go straight to the resolver, so redirects can never apply to them. That holds by construction, with no flag. Otherwise `publish-extension --target oci:…` with a matching redirect would overwrite the local file. The one exception is the legacy mock: `bicep publish` applies `ModuleAliasMockRule` to its target only so that a mocked alias stays an invalid target, as before. Stored ext-src references are fully qualified and are parsed, not resolved.

PR 1 makes `RedirectingArtifactResolver` return an `ArtifactResolution`: `{ SourceText, Address, CanonicalIdentity?, Reference, Redirect? }`, where `Redirect = { MatchedKey, Phase (Source | Canonical), DeclaringConfigUri, TargetUri }`.

**Parsing rules** for `ArtifactAddress.Parse`, which is total (it never fails). The checks run in order on the raw string:

1. `^(?<alias>[A-Za-z0-9_-]+)::(?<body>.*)$` gives `AliasQualified`. This is checked first, so `br::x` is the alias `br`, not the scheme `br`.
2. `^(?<scheme>br|ts)/(?<alias>[^:]+):(?<body>.*)$` gives `SchemeAndAliasQualified`. This form is permanent and resolves only through `moduleAliases`. Any other `<scheme>/<x>:` form (`oci/…`, `file/…`) gets a new diagnostic: "scheme-qualified aliases are not supported for this scheme; use `<alias>::`".
3. `^(?<scheme>[a-z-]+):(?<body>.*)$` gives `SchemeQualified` (`oci`, `br`, `ts`, `file`). `br-mock` stays rejected (BCP449).
4. Anything else gives `Path`.

An empty body is allowed only for an OCI alias whose body starts with `:` or `@` (the `storage:::0.32.1` form). That's validated after expansion; other alias types require a non-empty path.

**Reference types and identity**

| Spelling | Reference type | Restored? | Canonical identity |
|---|---|---|---|
| `oci:`, `br:`, `br/alias:`, OCI alias | `OciArtifactReference` | Yes, into `~/.bicep/br/…` | `oci:<registry>/<repo>(:<tag>|@<digest>)` |
| `ts:`, `ts/alias:`, Template Spec alias | `TemplateSpecArtifactReference` | Yes | `ts:<sub>/<rg>/<name>:<version>` |
| Relative path, `file:`, file alias, redirect target, mock hit | `LocalArtifactReference` | Never | Normalized absolute URI |

- Every local file uses one reference type, however it's spelled. The spellings differ only in what the path is relative to (the referencing file, a file alias root, or the declaring configuration file), and resolving that is the resolver's job. Redirects and mocks are recorded as provenance on `ArtifactResolution`, not as reference types.
- `OciArtifactReference.FullyQualifiedReference` stays `br:…`. It's persisted in source archives, ext-src URIs and cache paths, which older Bicep versions read. `ArtifactReference.CanonicalIdentity` is added instead (PR 1), and the resolver parses both `br:` and `oci:` into `OciArtifactReference`.
- Diagnostics that print a reference prefer the user's spelling (`LocalArtifactReference.SourceText`, `ArtifactResolution.SourceText`). Existing diagnostic text stays byte-identical.
- `LocalArtifactReference` equality is `(File, Type)`. So `./a.bicep` and `file:./a.bicep` are the same artifact, while `./a.bicep` written in two different folders are different artifacts.
- Allowed file types: modules must be `.bicep` or `.json` (and `.bicepparam` only where `using`/`extends` already allows it). Extensions have no file-type requirement; a file is valid if it decodes as an extension archive.

### 3.2 Services: one question, one owner

| Question | Owner | Callers | State |
|---|---|---|---|
| What artifact does this text name? | `IArtifactReferenceResolver` | The redirect layer, CLI publish commands, test helpers | None |
| What should the compiler load for this dependency? | `RedirectingArtifactResolver` (compiler only) | `SourceFileGroupingBuilder`; later the `load*()` functions (PR 6) | None |
| Is this remote artifact cached, and can we download it? | `IRemoteArtifactRestorer` over `IEnumerable<IRemoteArtifactRegistry>` | Compiler, source graph, language server, MCP | Recent restore failures (30 minutes) |
| Can we push it? | `OciArtifactRegistry` (`CheckArtifactExists`, `PublishModule`, `PublishExtension`) | CLI publish commands, test helpers | None |
| What's in the cache, and where? | Artifact cache entries | Registries, local-extension loading | None (the files on disk) |

Design rules:

- **Resolution has one method:** `TryResolve(IOUri referencingUri, ArtifactType type, string text)`. It doesn't read Bicep syntax; the source graph extracts the text. It doesn't read stored `br:` references; those are parsed directly. And it doesn't apply redirects or mocks; those live in the compiler-only layer.
- **Acquiring and loading are separate.** Only `RemoteArtifactReference`s are ever restored, and the restorer's signatures enforce it. Local references never reach the restorer; a local extension `.tgz` is extracted into the cache on first use.
- **Restore and publish are separate operations.** Publishing only targets OCI, so it's three `OciArtifactRegistry` members with no publisher service. `publish-extension --target <path>` writes the archive itself.
- **Objects carry their own facts.** Every reference knows its `ArtifactType`, and whether it's local or remote is its type, not a flag. Module/extension method pairs exist only where the payload differs (publish).
- **Variation is modeled with types:** restore states, the module and extension families, and redirect rules. The legacy mock and redirect patterns are subclasses of one redirect-rule type, so deleting the mock means deleting one class.

`IRemoteArtifactRegistry` (`IArtifactRegistry` until 0.8b) has `Scheme` (`Schemes` from PR 2), `CanHandle`, `TryGetEntryPoint` (`TryOpen` after 0.9c) and `RestoreAsync(references, RestoreMode)`. Each has one job: `Schemes` lists the text spellings the registry supports (for BCP189); `CanHandle` routes references by type, so the cast in `RemoteArtifactRegistry<TReference>` can't fail. Routing by scheme was removed on purpose in 0.6, and after PR 2 `oci:` and `br:` share one reference type whose persisted scheme stays `br`. A forced restore deletes the cached copies first. The restorer starts the public module catalog download alongside a restore, so that linter rules read cached metadata. Hover reads OCI metadata from `OciArtifactRegistry` and gets other modules' descriptions from their semantic models.

### 3.3 Artifact model (Phase 0.9)

**Problem.** An artifact's content has no single model. Each place it lives has its own shape, and the conversions between them are written by hand:

| Where the content is | Shape today | Conversion today |
|---|---|---|
| In memory, to publish | `ModulePackage` / `ExtensionPackage` | – |
| In OCI | `OciArtifactResult` + subclasses, built inside the transports by `OciArtifactResultFactory` | Push encoding in `OciArtifactRegistry.PublishModule`/`PublishExtension`; pull validation in the result classes |
| In a local `.tgz` | Extension archive | `ExtensionV1Archive.Build` / `Read` (the only real round trip) |
| In the cache | Cache entry classes | `WriteLayers(OciArtifactResult)` and `LocalExtensionArtifact.WriteContent(ExtensionPackage)`, which pick this machine's binary in two places |
| What the compiler gets | `ArtifactResolutionInfo.Result` is a bare `IFileHandle` | Each consumer works out what kind of file it is |

Because the compiler gets only a file, consumers reach back for the rest:

- The source graph checks `reference is TemplateSpecModuleReference` to choose a parser.
- Local deploy goes back through `NamespaceType.ExtensionArtifactReference.ResolveExtensionArtifact()`.
- The language server reads `source.tgz` through `TryLoadSourceArchive()`, skipping the restore state.
- `LocalArtifactReference` extracts extensions into the cache inside a `Lazy`. That's I/O in a value object used as a dictionary key.

**Design.**

- **Packages** are the content, used both ways.
- **Codecs** encode a package into one container (an OCI payload, a `.tgz`) and decode it back, and are tested by round trip. As in Netty or gRPC codecs, encoding means framing: the parts are not transformed, only the container around them changes.
- **Transports** move an `OciArtifactPayload` that knows nothing about Bicep.
- **Cache entries** write a package and open an artifact.
- **The compiler** gets the artifact.

```csharp
// Content. Immutable, independent of any container.
public sealed record ModuleArtifactPackage(BinaryData Template, BinaryData? Sources, string? DocumentationUri, string? Description = null)
{
    // Publishing derives the description from the template, as PublishModule does today.
    public static ModuleArtifactPackage FromTemplate(BinaryData template, BinaryData? sources, string? documentationUri) => …;
}

// LocalDeployEnabled is dropped: it was always Binaries.Any().
public sealed record ExtensionArtifactPackage(BinaryData Types, ImmutableArray<ExtensionBinary> Binaries);

// One model for both directions, shaped like ORAS: a manifest plus the content it references, keyed by digest.
// The manifest bytes are the truth. A pulled payload keeps the exact bytes (digest checks; the cache stores them
// as-is); a pushed payload serializes once.
public sealed record OciArtifactPayload(BinaryData ManifestData, ImmutableDictionary<string, BinaryData> BlobsByDigest)
{
    public OciManifest Manifest { get; }           // parsed from ManifestData
    public string ManifestDigest { get; }          // sha256 of ManifestData
    public BinaryData? Config { get; }
    public BinaryData? TryGetLayer(string mediaType);
    public static OciArtifactPayload Create(string artifactType, … config, … layers, … annotations);
}

public interface IRegistrySession : IAsyncDisposable   // the Azure SDK and ORAS transports: no Bicep media types
{
    Task PushAsync(IOciArtifactReference target, OciArtifactPayload payload, CancellationToken cancellationToken);
    Task<OciArtifactPayload> PullAsync(IOciArtifactReference reference, CancellationToken cancellationToken);
    Task<(string Digest, OciManifest Manifest)> ResolveAsync(IOciArtifactReference reference, CancellationToken cancellationToken);
}

// Codecs: the only code that knows Bicep's OCI media types. Error messages move here verbatim.
internal static class OciModuleArtifactCodec
{
    public static OciArtifactPayload Encode(ModuleArtifactPackage package);    // config, template layer, optional sources layer, annotations
    public static ModuleArtifactPackage Decode(OciArtifactPayload payload);    // today's OciModuleArtifactResult validation
}

internal static class OciExtensionArtifactCodec
{
    public static OciArtifactPayload Encode(ExtensionArtifactPackage package); // config, types layer, one layer per binary
    public static ExtensionArtifactPackage Decode(OciArtifactPayload payload); // keeps every architecture's binary
}

// ExtensionArtifactArchiveCodec (today's ExtensionV1Archive) encodes and decodes the local .tgz.

// Cache entries: write a package once, open it as an artifact. OCI entries also store the original manifest
// bytes and the metadata file, which older Bicep versions and hover read.
public sealed class OciModuleArtifactCacheEntry : OciArtifactCacheEntry
{
    public Task WriteAsync(ModuleArtifactPackage package, OciArtifactPayload pulled);
    public ModuleArtifact Open(OciArtifactReference reference);
}

// One writer for the extension files shared by OCI and local extension cache entries:
// picks this machine's binary (today's two error messages), then writes types.tgz last.
internal static class ExtensionArtifactCacheFiles
{
    public static void Write(IDirectoryHandle directory, ExtensionArtifactPackage package, string referenceForErrors);
}

// What the compiler, language server and local deploy get. Backed by files and read lazily
// (extension binaries can be large).
public abstract class Artifact(ArtifactReference reference) { public ArtifactReference Reference { get; } = reference; }

public enum ModuleTemplateKind { SourceFile, ArmTemplate, TemplateSpec }

public sealed class ModuleArtifact : Artifact
{
    public IFileHandle EntryPoint { get; }
    public ModuleTemplateKind Kind { get; }
    public ResultWithException<SourceArchive>? TryGetSources();     // null if published without sources
}

public sealed class ExtensionArtifact : Artifact
{
    public IFileHandle Types { get; }
    public IFileHandle? Binary { get; }                             // this machine's binary, if any
}
```

The flows:

```
publish:          ModuleArtifactPackage -> OciModuleArtifactCodec.Encode -> OciArtifactPayload -> push
restore:          pull -> OciArtifactPayload -> Decode -> package -> cache entry WriteAsync(package, payload) -> files
compile:          files -> cache entry Open -> ModuleArtifact / ExtensionArtifact
local module:     LocalArtifactReference -> ModuleArtifact(file, SourceFile)
local extension:  .tgz -> ExtensionArtifactArchiveCodec.Decode -> package -> LocalExtensionArtifactCacheEntry.Write -> Open -> ExtensionArtifact
```

Consumers after 0.9c:

- **Restore state:** `RemoteArtifactRestoreState.Restored(Artifact)`.
- **Source graph:** `ArtifactResolutionInfo.Result` is a `ResultWithDiagnosticBuilder<Artifact>`, and it chooses the parser from `ModuleArtifact.Kind`.
- **Extensions:** `NamespaceProvider` reads `ExtensionArtifact.Types`.
- **Local deploy:** `NamespaceType.ExtensionArtifact?.Binary` gives the binary.
- **Language server:** it calls `ModuleArtifact.TryGetSources()`, after asking the restorer when it has no compilation.
- **Local extensions:** extracting them into the cache moves into a small `LocalArtifactLoader`, leaving `LocalArtifactReference` a pure value.

Deliberately not done:

- **No eager content loading.** Reading from the cache stays file-backed and lazy.
- **No merging the artifact and package types.** They model content on disk and content in memory.
- **No package or codec for Template Specs.** They're never published, and their cached `main.json` (the Template Spec resource JSON) is read by older Bicep versions. A Template Spec cache entry writes the string and opens a `ModuleArtifact` with `Kind = TemplateSpec`.
- **No cache layout change.** `ArtifactCacheLayoutTests` pins it.
- **No generic kind framework.** There are exactly two kinds, so there's one `switch` on `reference.Type` per registry.
- **No ORAS types in our model (yet).** The Azure SDK transport is still the default, and ORAS's `Descriptor`/`Manifest` are mutable. Because `OciArtifactPayload` is shaped like ORAS, the sessions could later become `CopyAsync` between a `MemoryStore` and the repository.

### 3.4 Target file structure

There are two top-level folders, with a one-way dependency between them:

- **`Bicep.Core/Artifacts/`** holds the Bicep artifact model and its services: naming, resolving, restoring, publishing, storing and loading artifacts.
- **`Bicep.Core/Registry/`** holds the code that talks to remote registries: transports, sessions, clients, OCI wire types, the Template Spec repository and the module catalog. It contains no Bicep artifact model.

`Artifacts` depends on `Registry`, never the other way around. Today there is one exception: `IOciRegistryTransportFactory.CreateSession(OciArtifactReference)`. 0.9a changes it to take the address plus the cloud and ORAS settings.

Annotations: `<-` gives where a file came from (moved in 0.8b unless marked); `(0.9x)` marks a new file. Namespaces follow the folders (`Bicep.Core.Artifacts.References` and so on).

```
Bicep.Core/
  Artifacts/
    Artifact.cs                                (0.9c) Artifact
    ModuleArtifact.cs                          (0.9c) ModuleArtifact, ModuleTemplateKind
    ExtensionArtifact.cs                       (0.9c)
    ArtifactType.cs                            <- Registry/
    ArtifactSchemes.cs                         <- Modules/ModuleReferenceSchemes.cs (ArtifactReferenceSchemes)
    ArtifactException.cs                       <- Registry/ExternalArtifactException.cs
    InvalidArtifactException.cs                <- Registry/
    References/
      ArtifactReference.cs                     <- Registry/
      RemoteArtifactReference.cs               <- Registry/ExternalArtifactReference.cs
      RemoteArtifactContext.cs                 <- Registry/ExternalArtifactContext.cs
      LocalArtifactReference.cs                <- Registry/   (0.9b: a pure value, no I/O)
      OciArtifactReference.cs                  <- Registry/Oci/
      OciArtifactContext.cs                    <- Registry/Oci/
      OciArtifactReferenceFacts.cs             <- Registry/Oci/
      TemplateSpecArtifactReference.cs         <- Modules/TemplateSpecModuleReference.cs
      TemplateSpecArtifactContext.cs           <- Registry/
    Resolution/
      ArtifactAddress.cs                       <- Registry/
      IArtifactReferenceResolver.cs            <- Registry/
      ArtifactReferenceResolver.cs             <- Registry/
      IRemoteArtifactContextFactory.cs         <- Registry/IExternalArtifactContextFactory.cs
      RemoteArtifactContextFactory.cs          <- Registry/ExternalArtifactContextFactory.cs
      LocalArtifactLoader.cs                   (0.9b) opens local modules; decodes local extension archives, extracts them into the cache and opens them
    Redirects/                                 unchanged: ArtifactRedirectRule, RedirectingArtifactResolver (+ PatternRedirectRule in PR 7)
    LegacyModuleAliasMocks/                    unchanged: ModuleAliasMockRule
    Restore/
      IRemoteArtifactRestorer.cs               <- Registry/IArtifactRestorer.cs
      RemoteArtifactRestorer.cs                <- Registry/ArtifactRestorer.cs
      RemoteArtifactRestoreState.cs            <- Registry/ArtifactRestoreState.cs
      RestoreMode.cs                           <- Registry/
      RestoreFailureCache.cs                   <- Registry/
      RestoreExtensions.cs                     <- Registry/ArtifactHelper.cs (GetRemoteArtifactReferences)
    Registries/
      IRemoteArtifactRegistry.cs               <- Registry/IArtifactRegistry.cs
      RemoteArtifactRegistry.cs                <- Registry/ArtifactRegistry.cs   RemoteArtifactRegistry<TReference>
      OciArtifactRegistry.cs                   <- Registry/
      OciArtifactRegistryException.cs          <- Registry/
      TemplateSpecArtifactRegistry.cs          <- Registry/TemplateSpecModuleRegistry.cs
    Packages/
      ModuleArtifactPackage.cs                 <- Registry/ModulePackage.cs
      ExtensionArtifactPackage.cs              <- Registry/ExtensionPackage.cs (with ExtensionBinary)
    Codecs/
      ExtensionArtifactArchiveCodec.cs         <- Registry/Providers/ExtensionV1Archive.cs
      TypesV1Archive.cs                        <- Registry/Providers/ (packs a types index into types.tgz)
      OciExtensionV1Config.cs                  <- Registry/Oci/
      OciModuleArtifactCodec.cs                (0.9a)
      OciExtensionArtifactCodec.cs             (0.9a)
    Cache/
      ArtifactCacheEntry.cs                    <- SourceGraph/Artifacts/ExternalArtifact.cs
      OciArtifactCacheEntry.cs                 <- SourceGraph/Artifacts/BicepRegistryArtifact.cs
      OciModuleArtifactCacheEntry.cs           <- SourceGraph/Artifacts/BicepRegistryModuleArtifact.cs
      OciExtensionArtifactCacheEntry.cs        <- SourceGraph/Artifacts/BicepRegistryExtensionArtifact.cs
      TemplateSpecArtifactCacheEntry.cs        <- SourceGraph/Artifacts/TemplateSpecModuleArtifact.cs
      LocalExtensionArtifactCacheEntry.cs      <- SourceGraph/Artifacts/LocalExtensionArtifact.cs
      ExtensionArtifactCacheFiles.cs           (0.9b) the one writer of types.tgz and extension.bin
      ModuleMetadata.cs                        <- Registry/Oci/ (the cache's "metadata" file)
      TagEncoder.cs                            <- Registry/Oci/ (the cache directory name of a tag)
  Registry/
    AzureContainerRegistryManager.cs           unchanged (Azure SDK transport)
    ContainerRegistryClientFactory.cs          unchanged
    IContainerRegistryClientFactory.cs         unchanged
    RegistryConfiguration.cs                   unchanged (trusted registries)
    ITemplateSpecRepository.cs                 unchanged
    TemplateSpecRepository.cs                  unchanged
    ITemplateSpecRepositoryFactory.cs          unchanged
    TemplateSpecRepositoryFactory.cs           unchanged
    TemplateSpecException.cs                   unchanged
    TemplateSpecEntity.cs                      <- Registry/TemplateSpecModuleRegistry.cs (now in its own file)
    Azure/                                     unchanged: AcrRegistrySession
    Catalog/                                   unchanged: module catalog for completion and the linter
    Oci/
      OciArtifactPayload.cs                    (0.9a) a manifest and its blobs, pushed or pulled
      BicepMediaTypes.cs                       unchanged
      OciAnnotationKeys.cs                     unchanged
      OciDescriptor.cs                         unchanged (0.9a: Data removed)
      OciManifest.cs                           unchanged
      OciManifestAnnotationsBuilder.cs         unchanged
      OciSerialization.cs                      unchanged
      IOciArtifactAddressComponents.cs         unchanged (the wire address: registry, repository, tag or digest)
      OciArtifactAddressComponents.cs          unchanged
      IOciArtifactReference.cs                 unchanged
      IOciRegistryTransport.cs                 unchanged
      IOciRegistryTransportFactory.cs          unchanged (0.9a: no OciArtifactReference parameter)
      OciRegistryTransportFactory.cs           unchanged
      Oras/                                    unchanged
    Sessions/                                  unchanged: IRegistrySession (0.9a: pushes and pulls OciArtifactPayload)
  Configuration/
    LegacyModuleAliasMocks/                    <- Configuration/IBicepModuleAliasesMockConfiguration.cs, ModuleAliasesMockConfiguration.cs
  SourceGraph/
    ArtifactReferenceText.cs                   unchanged (syntax -> reference text belongs to the source graph)
```

Files and folders that disappear:

| Removed | When | Where it goes |
|---|---|---|
| `Modules/` (the whole folder) | 0.8b + 0.9a | Moved to `Artifacts/`; `OciModuleArtifactResult.cs` deleted in 0.9a |
| `Registry/Providers/` | 0.8b | Both archives move to `Artifacts/Codecs/` |
| `Registry/ArtifactHelper.cs` | 0.8b | Its one method became the `GetRemoteArtifactReferences()` extension method in `Artifacts/Restore/RestoreExtensions.cs` |
| `Registry/Oci/OciArtifactResult.cs`, `OciExtensionArtifactResult.cs`, `OciArtifactResultFactory.cs` | 0.9a | `OciArtifactPayload` and the codecs |
| `SourceGraph/Artifacts/IExtensionArtifact.cs` | 0.9c | `ExtensionArtifact` (after 0.8b it is the only file left in that folder) |
| `SourceGraph/ArtifactReferences/` | 0.9c | `IExtensionArtifactReference` replaced by `NamespaceType.ExtensionArtifact` |
| `SourceLink/OciModuleReferenceExtensions.cs` | 0.9c | `ModuleArtifact.TryGetSources()` |

Tests mirror the product folders under `Bicep.Core.UnitTests/Artifacts/` (`References`, `Resolution`, `Restore`, `Registries`, `Codecs`, `Cache`, `LegacyModuleAliasMocks`). `ArtifactCacheLayoutTests` stays an integration test.

### 3.5 Permanent and deletable reference formats

Every reference syntax customers have written is supported permanently, because a migration can't realistically be required. Only `moduleAliasesMock` is deletable.

| Format or feature | Status |
|---|---|
| `br:<registry>/<repo>:<tag>` | Permanent. Persisted in source archives, ext-src URIs and the `~/.bicep/br` cache; another spelling of `oci:`. |
| `ts:<sub>/<rg>/<name>:<version>` | Permanent. |
| `br/public:`, `br/<alias>:` and `ts/<alias>:` with `moduleAliases` | Permanent: scheme-qualified aliases. |
| `oci:`, `file:`, `<alias>::`, `artifacts.*` | Permanent: new in REP 0022. |
| Relative paths; `extensions` values (with a scheme or a bare path) | Permanent. |
| `moduleAliasesMock` | Deletable; superseded by `artifacts.redirects`. |
| `br-mock:` | Removed in 0.6; the text is still rejected (BCP449). |

Terminology: the permanent forms are never called "legacy". The `Legacy` prefix is used only for the code location of the deletable mock (`LegacyModuleAliasMocks` folders and namespaces). The two alias namespaces stay independent, as the REP requires:

- **Scheme-qualified aliases** (`br/<alias>:`, `ts/<alias>:`) resolve through `moduleAliases`.
- **Artifact aliases** (`<alias>::`) resolve through `artifacts.aliases` and the predefined `mar`/`avm`.

The resolver has two separate lookups and never falls back from one to the other. Both feed the same "…after resolving alias "X" is not valid" diagnostics through `aliasContext`.

**Mock isolation (done in 0.3b, 0.5 and 0.8a).** `ModuleAliasMockRule` owns all mock behavior. It is the only rule in `RedirectingArtifactResolver` until PR 7, and it is evaluated at step 3. A hit is terminal and produces a `LocalArtifactReference` whose display text is `br:<modulePath>`. `ModuleAliasMockIsolationTests` checks that only these places mention the mock:

- the `LegacyModuleAliasMocks` folder
- its registration in `RedirectingArtifactResolver`
- the CLI `publish` command
- the configuration binding and its declaring-URI annotation

Since 0.8b, the mock configuration types live in `Configuration/LegacyModuleAliasMocks/`.

To delete the mock (this checklist is also in a comment on `ModuleAliasMockRule`), remove:

- the `LegacyModuleAliasMocks` folders in product code and tests
- its registration in `RedirectingArtifactResolver`
- the call in the CLI `publish` command
- the `ModuleAliasesMock` configuration property, with its bind, `With` and `WriteTo` code and its declaring-URI registration
- the `moduleAliasesMock` placeholder in the built-in `bicepconfig.json` and in the schema
- diagnostic methods that become unused (their BCP codes stay reserved)
- optionally, add a "`moduleAliasesMock` is no longer supported; use `artifacts.redirects`" configuration diagnostic

Deprecating the mock (warnings, a timeline) is a product decision outside REP 0022, which requires mocks to keep working without new warnings.

---

## 4. Phase 0: refactoring

Every Phase 0 PR leaves all baselines unchanged and changes no diagnostic text. `Azure.Bicep.Core` is published to nuget.org but explicitly unsupported, so public API renames are fine. Bicep.Core types stay `public`, by convention.

### 4.1 Completed phases

- **0.1: characterization tests.** `ArtifactReferenceResolutionTests` pins end-to-end resolution through the real stack. It covers relative paths, `br:`, `br/public:`, `br/<alias>:`, `moduleAliasesMock`, `br-mock:`, `ts:`, `ts/<alias>:`, extension text, the `extensions` map, implicit extensions, and how the REP 0022 spellings (`oci:`, `file:`, `avm::`, `br::`, `oci/x:`) fail today.
- **0.2: one source of truth.** The compiler and the language server read resolved references from `SourceFileGrouping.ArtifactLookup` instead of resolving them again. `IArtifactReferenceFactory` left `Compilation`, `SemanticModel` and `SymbolContext`.
- **0.3a: parser and resolver.** The total `ArtifactAddress.Parse` replaced the string splitting in `ModuleDispatcher`, and `ArtifactReferenceResolver` took over turning text into references.
- **0.3b: alias expansion and mock isolation.**
  - The resolver expands `moduleAliases` and builds the references itself, so registries stop parsing.
  - The OCI and Template Spec body parsers take an `aliasContext` used only in diagnostics.
  - The mock moved into `Artifacts/LegacyModuleAliasMocks`, guarded by an architecture test.
  - The declaring-URI annotation was generalized.
- **0.4: split `ModuleDispatcher`.** It became `IArtifactReferenceResolver`, `IArtifactRestorer` (which owns the restore-failure cache) and `IArtifactPublisher`.
- **0.5: cleaner service APIs.**
  - `ArtifactReference.Type` moved to the base class, and an `ExternalArtifactReference` base was added.
  - The resolver has one `TryResolve` method, and the source graph extracts the reference text from syntax.
  - The compiler-only `RedirectingArtifactResolver` was added, with `ModuleAliasMockRule`.
  - The restorer returns an `ArtifactRestoreState` and has `RestoreAsync(references, RestoreMode)`, backed by a `RestoreFailureCache`.
  - `bicep publish` applies the mock rule only to keep rejecting mocked targets.
- **0.6: local references.**
  - One `LocalArtifactReference(File, Type, SourceText)`, equal by `(File, Type)`, replaced `LocalModuleReference` and `OciArtifactMockedReference`.
  - The resolver takes an `IOUri`, and registries are routed by reference type.
  - `LocalModuleRegistry`, `OciArtifactMockedRegistry` and the `br-mock` scheme were deleted.
  - Behavior changes:
    - Local extensions are extracted into the cache on first use, so `--no-restore` builds and the language server load them.
    - The language server's force-restore summary lists only remote artifacts.
    - References from different folders are no longer equal just because the text matches.
- **0.7: external artifact context.**
  - `OciArtifactContext` (cloud, cache root, `UseOrasSdk`) and `TemplateSpecArtifactContext` replaced the configuration and features carried by references. The context is part of a reference's equality.
  - The dummy referencing files are gone.
  - `ArtifactRestoreState.Restored` carries the entry point.
  - Bug fix: an artifact reached with two different clouds is restored once per cloud.
- **0.8a: registry, publish and cache cuts** (includes the former 0.8c).
  - `IEnumerable<IArtifactRegistry>` replaced the registry-provider layer; Wasm registers only OCI.
  - `IArtifactRegistry` keeps only restore operations.
  - `IArtifactPublisher`, `ArtifactPublisher` and `ArtifactPackage` were removed; the CLI calls `OciArtifactRegistry` directly, and `publish-extension` writes local archives itself.
  - Hover reads OCI metadata from `OciArtifactRegistry`.
  - `ArtifactRedirectTable` was folded into `RedirectingArtifactResolver`.
  - `OnRestoreArtifacts` was removed: the restorer now starts the public module catalog download itself.
  - Cache invalidation is part of a forced `RestoreAsync`.
  - Cache entries own every cache path, the completeness check, writes, deletes and the single lock loop. `ArtifactCacheLayoutTests` pins the layout; it passed before and after the move.
  - The one on-disk difference: a pulled artifact of the wrong type no longer leaves a directory containing only its manifest.
- **0.8b: vocabulary and folders** (renames and moves only).
  - The artifact model and services moved to `Artifacts/{References,Resolution,Restore,Registries,Packages,Codecs,Cache}` (section 3.4), with namespaces that follow the folders. The mock configuration types moved to `Configuration/LegacyModuleAliasMocks/`, and `TemplateSpecEntity` got its own file. Unit tests moved to the matching `Bicep.Core.UnitTests/Artifacts/` folders.
  - Types that survive 0.9 took the section 2 names: the cache entries (`ArtifactCacheEntry`, `Oci*ArtifactCacheEntry`, `TemplateSpecArtifactCacheEntry`, `LocalExtensionArtifactCacheEntry`), `*ArtifactPackage`, `ExtensionArtifactArchiveCodec` (`Encode`/`Decode`), `TemplateSpecArtifact{Reference,Registry}`, `RemoteArtifact{Reference,Context,ContextFactory}`, `(I)RemoteArtifactRegistry`, `(I)RemoteArtifactRestorer`, `RemoteArtifactRestoreState`, `ArtifactException` and `ArtifactSchemes`. `ArtifactHelper` became `RestoreExtensions.GetRemoteArtifactReferences()`.
  - The types that 0.9 deletes kept their names and folders. No naming architecture test was added (see section 2).
  - Two commits (moves, then renames), so that git shows both as renames.

### 4.2 Remaining phases

| # | Phase | Details | Verification |
|---|---|---|---|
| 0.9a | **OCI payload and codecs** | `OciArtifactPayload` replaces `OciArtifactResult` and its subclasses, `OciArtifactResultFactory`, `OciArtifactLayer` and `OciDescriptor.Data`. `IRegistrySession` pushes and pulls payloads in both transports (Azure SDK and ORAS), with no Bicep media types. `OciModuleArtifactCodec` and `OciExtensionArtifactCodec` take over the encoding for push (from `OciArtifactRegistry`) and the pull validation (from the result classes), keeping error messages verbatim. `ModuleArtifactPackage` gains `Description` and `FromTemplate`; `ExtensionArtifactPackage` drops `LocalDeployEnabled`. `PrivateAcrModuleMetadataProvider` reads the payload's manifest. | A round-trip test per codec, with and without sources, documentation and binaries; publish, restore and BCP192 message tests unchanged. |
| 0.9b | **Cache entries store packages** | OCI cache entries `WriteAsync(package, payload)`; `ExtensionArtifactCacheFiles` is the one writer of extension files; local extensions are decoded, written and opened through the same path (`LocalExtensionArtifactCacheEntry.Unpack` becomes the cache entry's `Write(package)`). Unpacking moves out of `LocalArtifactReference` into `LocalArtifactLoader`; references stop exposing cache entries, and registries create entries from references. `LocalArtifactReference` loses its cache root, so `ArtifactReferenceResolver` and `ModuleAliasMockRule` no longer need `IFeatureProviderFactory`; `IArtifactReferenceResolver` itself is unchanged. | `ArtifactCacheLayoutTests` unchanged; local-extension and concurrent-extraction tests. |
| 0.9c | **The compiler gets artifacts** | Add `Artifact`, `ModuleArtifact` and `ExtensionArtifact`. `IRemoteArtifactRegistry.TryGetEntryPoint` becomes `TryOpen`; `RemoteArtifactRestoreState.Restored(Artifact)`; `ArtifactResolutionInfo.Result` becomes `ResultWithDiagnosticBuilder<Artifact>`. The source graph uses `ModuleArtifact.Kind`; `NamespaceProvider` uses `ExtensionArtifact.Types`; `NamespaceType` carries `ExtensionArtifact` for local deploy; the language server uses `ModuleArtifact.TryGetSources()`. Delete `IExtensionArtifact`, `IExtensionArtifactReference`, `ResolveExtensionArtifact` and `TryLoadSourceArchive`. | Baselines unchanged; language server ext-src, visualizer and code-lens tests; local deploy tests. |

Phase 0 ends with 0.9c.

### 4.3 Why rename (0.8b) before simplifying (0.9)

- **Review.** A PR that mixes moves and logic changes is hard to review: git can lose the rename, and every changed line looks new. With the moves done first, 0.9's diffs contain only logic changes.
- **Writing the new code.** 0.9's new code is written directly in its final folders and namespaces, under its final names.
- **No double renaming.** Only types that survive 0.9 are renamed. The types 0.9 deletes stay where they are until they're deleted, so nothing is renamed twice.
- **Mechanical is mechanical.** 0.8b keeps only renames and moves. Anything with logic (the completion split, `CancellationToken`) moves to the PRs that need it.

### 4.4 Validation notes

- **Last full run (0.8b):** `dotnet build Bicep.slnx --no-restore` with 0 warnings; `dotnet test --solution Bicep.slnx --no-build` with 16,108 passed and 1 skipped; no baseline changes. Each earlier phase ran the full suite; its results are in the PR description.
- **Test runner flags:** these test runners use Microsoft.Testing.Platform and don't accept `--blame-hang-timeout` or `--output Minimal`. A run with either flag executes zero tests.
- **Wasm DI:** `AddBicepDecompiler` calls `AddBicepCore` again, so Wasm removes the default registries after both calls.
- **Mock test case:** `RemoteArtifactRestorerTests`' mock reference includes its context in equality, as real references do.

---

## 5. REP 0022 features

After Phase 0, PR 1 shrinks to adding the `AliasQualified` address form, the canonical identity and provenance. The resolver, the parser, URI-anchored local references and routing by reference type already exist.

### 5.1 Configuration model

New `Configuration/ArtifactsConfiguration.cs`, bound from the top-level `artifacts` key:

```csharp
record ArtifactsConfigData {
  ImmutableSortedDictionary<string, ArtifactAlias> Aliases;     // polymorphic on "type"
  ImmutableDictionary<string, string> Redirects;                 // key pattern -> relative target
}
abstract record ArtifactAlias { IOUri? DeclaringConfigUri }      // [JsonIgnore], set by the chain builder
record OciArtifactAlias(string Registry, string? RepositoryPrefix);
record TemplateSpecArtifactAlias(string SubscriptionId, string ResourceGroup);
record FileArtifactAlias(string Path);
```

- **Binding never throws on semantic errors.** A bad alias would otherwise invalidate the whole configuration through `InvalidBicepConfigFile`. Errors are stored per entry and reported when the entry is used, as `moduleAliases` errors are today. An unknown `type` is a per-entry error.
- **Alias names** use the `ModuleAliasesConfiguration.ModuleAliasNameRegex()` grammar and are case-sensitive.
- **The predefined `mar`/`avm` are defined in code**, not in the built-in `bicepconfig.json`. The configuration chain deep-merges JSON, so a user override would silently inherit the built-in `repositoryPrefix`; defaults defined in code are replaced as a whole entry. Overriding them with `type != "oci"` is an error. The defaults don't depend on the cloud profile.
- **Declaring URIs:** `artifacts.aliases.*` (file aliases) and `artifacts.redirects.*` are annotated with the leaf-most layer that declares them. Relative paths resolve against that file's directory.
- **Serialization:** `IBicepConfiguration.Artifacts` and `BicepConfigurationExtensions.With`/`WriteTo` are updated, so configuration dumping still round-trips.

### 5.2 Redirect engine (`PatternRedirectRule`s in `RedirectingArtifactResolver`)

- **Compiling keys.** Each key is compiled once (lazily, cached on the configuration object) into a literal-character count, a list of captures, and a generated regex. Keys are templates, never user regexes:
  - `{name}` matches `(?<name>[^/:@]+)`: one segment, which also must not be `.` or `..`.
  - `{...name}` matches `(?<name>[^/:@]+(?:/[^/:@]+)*)`.
  - Literal text is escaped.
- **Validating keys.** Capture names match `^[A-Za-z_][A-Za-z0-9_]*$` and are unique; there is at most one multi-segment capture; `*`, `?`, `**` and `[` aren't allowed. The key itself must parse, with its captures replaced by placeholders, as a non-relative reference.
- **Validating targets.**
  - A target starts with `./` or `../`. It isn't an absolute path or an artifact reference, and every `{x}` it uses is declared in the key.
  - After substitution, it's checked with the same rules as relative paths (allowing a leading `../`). Each captured segment must not be `.` or `..`, and must contain no `\` and no forbidden characters.
  - It resolves against `DeclaringConfigUri` and must be an existing file, not a directory. Otherwise it's "redirect target not found"; there's no fallback search.
- **Matching** happens per phase: exact keys first, then capture keys, where the most literal characters wins. A tie gives an "ambiguous artifact redirect" error listing the keys. The REP says "configuration validation fails"; static overlap detection is non-trivial, so this is a resolution-time error (to confirm with the REP author, question 5).
- **At most one redirect per reference.** References inside the target file are resolved fresh, using that file's own configuration, and cycles are caught by the existing cycle detection.

### 5.3 Data-file loads

- In the `SystemNamespaceType` load helpers, a literal containing `:` resolves through `RedirectingArtifactResolver`, so redirects apply. Otherwise the existing relative-path behavior is unchanged. This applies to `loadTextContent`, `loadJsonContent`, `loadYamlContent` and `loadFileAsBase64`, but not `loadDirectoryFileInfo`. A `:` can't appear in a relative path, so this isn't a breaking change.
- Allowed sources are `file:`, file aliases, and redirects on those spellings. An `oci:`, `ts:`, OCI alias or Template Spec alias in a data load gets a new "artifact reference not supported for file loading" error (question 7).
- Add a `BicepSourceFile.TryLoadAuxiliaryFile(IOUri)` overload that shares the cache and the tracking of referenced auxiliary files, so language server invalidation keeps working. The resolver is plumbed through `BicepSourceFileFactory`.

### 5.4 `artifact-redirect` linter rule

- `Analyzers/Linter/Rules/ArtifactRedirectRule.cs`, code `artifact-redirect`, default level `Warning`.
- **Sources:** the source graph's resolution results for the model's file (modules, `using`, `extends`, imports, explicit, config-named and implicit extensions), plus load-function literals.
- **Span:** the reference string, or the identifier for `extension foo`. **Message:** `"<source>" was redirected to local file "<path>" by "<config>"`.
- **Emitted once per (file, canonical identity)**, at the first occurrence (question 6). **Not** emitted for `moduleAliasesMock` hits.
- **`UseRecentModuleVersionsRule`** skips redirected references and recognizes `avm::`/`mar::`; its code fix keeps the source spelling.

### 5.5 CLI

- Add `--verbose` (or reuse logging) to `build`, `build-params`, `lint`, `restore`, `publish` and `generate-params`. It prints one stderr line per applied redirect, whatever the analyzer level (question 2).
- `publish` and `publish-extension` accept `oci:` and `alias::` targets and reject `file:` for modules. Targets go through `IArtifactReferenceResolver`, so redirects can't apply. Update `publish-extension`'s `br:`/`ts:` prefix check.
- `restore`: redirected references are local, so there's nothing to restore.
- The filters in `Bicep.Wasm/Interop.cs` and the MCP `BicepTools.cs` also accept `oci:` and the alias forms.

### 5.6 Language server and VS Code

- **Completion:**
  - Top-level items for `oci:`, `file:`, each `artifacts.aliases` entry, and `mar::`/`avm::`.
  - Registry, module and version completion works through expanded aliases, and file-path completion after `file:` and `<fileAlias>::`.
  - The same applies to extension declarations and load-function arguments.
  - This includes the completion split and `IArtifactAliasCatalog`, moved here from 0.8b: one catalog lists both kinds of alias prefix (`br/x:`, `ts/x:`, `x::`) with their expanded locations.
- **Definition, hover and document links:** local references open the file; a redirected OCI reference opens the local file and shows "redirected from …" in hover. Ext-src `br:` URIs are unchanged.
- **File watching:** redirect targets and alias roots are ordinary source or auxiliary files, and a `bicepconfig.json` change already triggers recompilation.
- **Schema:**
  - `artifacts.aliases` gets a name pattern and a `oneOf` on `type` (oci/ts/file, each with its required properties); `mar`/`avm` must be `oci`.
  - `artifacts.redirects` values must match `^\.\.?/`.
  - Add `analyzers.core.rules.artifact-redirect`.

### 5.7 PR sequence

Each PR can ship on its own; user-visible syntax lands only once its resolution path is complete. If releases are cut mid-sequence, new syntax is gated behind a temporary hidden flag that PR 10 removes (question 1).

| # | PR | Main files | Tests |
|---|---|---|---|
| 1 | **Canonical identity and provenance.** `ArtifactReference.CanonicalIdentity`; `ArtifactResolution` with provenance, returned by `RedirectingArtifactResolver`; the `AliasQualified` form, parsed but rejected until PR 4. | `Artifacts/References`, `Artifacts/Resolution` | Parser tests for `oci:localhost:5000/x:v1`, `br::x`, `storage:::1.0` and the `oci/mar:x` error; baselines unchanged. |
| 2 | **`oci:` scheme.** Resolution, publish targets, Wasm and MCP filters. `IRemoteArtifactRegistry.Scheme` becomes `Schemes` (`br` and `oci` for OCI), because the resolver derives the supported schemes from the registries. | `ArtifactSchemes`, resolver, `PublishExtensionCommand`, `Interop` | `oci:` and `br:` are equal and deduplicated in restore; publish to `oci:`; source archives still write `br:`. |
| 3 | **`artifacts` configuration section** (5.1). | `ArtifactsConfiguration`, `BicepConfiguration*`, `BicepConfigurationManager`, schema | Binding; inheritance (an alias declared in a base configuration resolves from its directory); predefined-override type error; whole-entry replacement. |
| 4 | **`<alias>::` references** for OCI and Template Specs, including `mar`, `avm` and the empty-path OCI form. Independent of `moduleAliases`. | Resolver, parser, `ArtifactsConfiguration`, `DiagnosticBuilder` | Expansion matrix; `br/x:` with only `artifacts.aliases.x` still gives the unknown-alias error; the same name in both maps with different targets. |
| 5 | **`file:` scheme and file aliases** for modules, `using`, `extends`, imports and extensions (direct and the `extensions` map). All of them resolve to `LocalArtifactReference`. | Resolver, `ArtifactsConfiguration`, `SourceFileGroupingBuilder` | The REP's monorepo scenario; transitive references use each file's own configuration; cycles through file aliases; invalid or unsafe `.tgz`. |
| 6 | **Data-file loads** (5.3). | `SystemNamespaceType`, `BicepSourceFile`, `BicepSourceFileFactory` | Loads through aliases; size and encoding limits unchanged; language server invalidation. |
| 7 | **Redirect engine** (5.2), for modules, extensions, Template Specs and data files. | `PatternRedirectRule`, `RedirectingArtifactResolver` | Precedence matrix including mock vs. redirect; tags, digests and nested paths; `{...path}`; rejecting a captured `..`; missing target; path normalization on Windows and Linux; publish ignores redirects. |
| 8 | **`artifact-redirect` rule and `--verbose`** (5.4, 5.5). | `ArtifactRedirectRule`, CLI, schema | Rule levels (an error blocks emit); suppression; once per artifact; verbose output even when the rule is `off`; no diagnostic for mocks. |
| 9 | **Language server UX** (5.6). | Completion, hover and definition handlers | Language server unit and integration tests. |
| 10 | **Docs, samples, cleanup**: remove the temporary flag (if used), baseline datasets, Learn docs, linter rule docs, changelog. | `Bicep.Core.Samples/Files/baselines/ArtifactAliases_*`, docs | Baselines for alias, redirect and file scenarios. |

PRs 2 and 3 can proceed in parallel after PR 1. PR 4 depends on 3; PR 5 depends on 1 (and 3 for file aliases); PR 6 on 5; PR 7 on 3 to 6; PR 8 on 7; PR 9 on 4 to 7.

### 5.8 Diagnostics to add (next free BCP codes; BCP459 is currently the highest)

- Unknown artifact alias.
- Invalid alias definition (missing or unknown `type`, missing required property).
- Predefined alias `mar`/`avm` must have type `oci`.
- Scheme-qualified alias form (`oci/x:`) not supported.
- Empty artifact path not allowed for the alias type, or an OCI empty path requires a full repository.
- A module `file:` reference must end in `.bicep` or `.json`.
- Artifact reference type not supported for data-file loading.
- Invalid redirect key (capture syntax, duplicate or invalid capture names, more than one `{...}`, glob characters).
- Invalid redirect target (not `./` or `../`, absolute, an artifact reference, an undeclared placeholder, a captured `.` or `..`).
- Ambiguous redirect (equally specific patterns).
- Redirect target not found, or is a directory.
- Linter rule `artifact-redirect`.

### 5.9 Backward-compatibility checklist (each needs an explicit test)

- `br/public:`, `br:`, `br/<alias>:` and `ts/<alias>:` are unchanged and get no new warnings.
- `moduleAliases` and `moduleAliasesMock` are unchanged, including absolute `mapToFilePath`, digest references, inherited relative roots and existing diagnostics.
- `artifacts.aliases` is never used for the `br/`/`ts/` forms, and vice versa.
- Persisted `br:` strings and the cache layout are unchanged (`ArtifactCacheLayoutTests`).
- All existing baselines are byte-identical after PRs 1–7.

---

## 6. Open questions

1. **A temporary experimental flag** during the rollout, vs. the REP's "no flag". Recommendation: a hidden flag, removed in PR 10.
2. **`--verbose` doesn't exist.** Add a global option, or use a log level?
3. **`file:` values in the `extensions` map:** relative to the declaring `bicepconfig.json`, consistent with today's bare paths? The REP says "relative to the file containing the reference"; the recommendation is to treat the configuration file as that file. Also: bare-path extension entries use the *leaf* configuration URI, not the declaring layer. Fix that at the same time?
4. **Absolute `file:` paths** (`file:C:/x.bicep`, `file:/x.bicep`): reject them, consistent with the relative-path rules? The REP implies relative paths only.
5. **Ambiguous redirect detection:** an error at resolution time, or static configuration validation?
6. **Rule granularity:** once per redirected artifact per compilation, or per occurrence (for `#disable-next-line`)?
7. **Redirecting remote identities for data loads** (an `oci:` key in `loadTextContent`): out of scope, or an error?
8. **Alias names equal to scheme names** (`br::`, `oci::`, `file::`): allowed (they're unambiguous) or reserved?
9. **Diagnostic display:** in existing restore-failure messages for `oci:` and alias references, show the source spelling or the fully qualified `br:` reference?
10. **REP wording:** the REP's "Future deprecation of `moduleAliases`" section could state that `br/<alias>:`, `ts/<alias>:` and `br/public:` are permanent and only `moduleAliasesMock` may be deprecated.
11. **Bare paths for implicit extensions:** `extension <name>` resolves a bare path from the `extensions` map relative to `bicepconfig.json`, but the same entry in `implicitExtensions` resolves relative to the referencing file (pinned in `ArtifactReferenceResolutionTests`). Unify on config-relative (a behavior change for implicit extensions), or keep both?
