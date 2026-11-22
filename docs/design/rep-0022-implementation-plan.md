# REP 0022 – Artifact aliases and redirects: implementation plan

Source: Azure/bicep-reps `active/0022-artifact-aliases-and-redirects.md`. Tracking issue: #19883.

Phase 0 (PRs #20496–#20575) refactored how Bicep resolves, restores, stores and loads artifacts, so that the REP's features can be added without rework. What remains are **feature PRs 1–6**: `oci:`, `file:`, `<alias>::`, the `artifacts` configuration section, redirects, and the `artifact-redirect` rule.

---

## 1. Status

| Phase | Summary | PR | Status |
|---|---|---|---|
| 0 | Resolution, restore, cache and artifact-model refactoring | #20496–#20575 | Done |
| 1 | `oci:`, `file:` and `<alias>::` parsing | #20580 | Done |
| 2 | `artifacts.aliases` | – | In progress |
| 3–6 | Remaining REP 0022 features (section 5.7) | – | Planned |

---

## 2. Vocabulary

Unqualified "artifact" always means a **Bicep artifact**: a module or an extension, the thing an `ArtifactReference` names and an `ArtifactType` classifies. Other artifact-related types name their role with a suffix: `[Location][Kind]Artifact[Role]`. A location prefix (`Oci`, `TemplateSpec`, `Local`) says where the artifact lives; generic remote-only types start with `Remote`; types that handle both have no prefix. This is a review convention, not an architecture test.

| Term | Means | Types |
|---|---|---|
| **Artifact** | A module or extension, as the compiler, language server and local deploy use it | `Artifact`, `ModuleArtifact`, `ExtensionArtifact` |
| **Artifact reference** | Which artifact, and where it lives. Identity only, no I/O | `ArtifactReference`, `RemoteArtifactReference`, `OciArtifactReference`, `TemplateSpecArtifactReference`, `LocalArtifactReference` |
| **Artifact package** | An artifact's content in memory, independent of any container | `ModuleArtifactPackage`, `ExtensionArtifactPackage` |
| **Artifact codec** | Converts a package to and from one container (OCI payload, `.tgz`) | `OciModuleArtifactCodec`, `OciExtensionArtifactCodec`, `LocalExtensionArtifactCodec` |
| **Artifact payload** | An OCI manifest and the blobs it references | `OciArtifactPayload` |
| **Artifact cache entry** | Where an artifact is stored on this machine; owns the on-disk layout | `ArtifactCacheEntry`, `OciModuleArtifactCacheEntry`, `OciExtensionArtifactCacheEntry`, `TemplateSpecArtifactCacheEntry`, `LocalExtensionArtifactCacheEntry`, `ExtensionArtifactCacheLayout` |
| **Remote artifact registry** | Restores remote artifacts; the OCI registry also publishes | `IRemoteArtifactRegistry`, `RemoteArtifactRegistry<TReference>`, `OciArtifactRegistry`, `TemplateSpecArtifactRegistry` |
| **Remote artifact restorer** | Restores across registries and remembers recent failures | `IRemoteArtifactRestorer`, `RemoteArtifactRestorer`, `RemoteArtifactRestoreState` |

The user-facing term "artifact" (diagnostics, docs, `artifacts`, `artifact-redirect`) agrees with this meaning.

---

## 3. Architecture

### 3.1 Resolution pipeline

Every reference the compiler loads goes through this pipeline: declarations, `extensions` configuration values, implicit extensions, and (from PR 3) data-file loads. The language server reads the results from the source graph (`SourceFileGrouping`) instead of resolving again.

```
reference text
  -> ArtifactAddress.Parse
       Path | SchemeQualified(scheme, body) | SchemeAndAliasQualified(scheme, alias, body) | AliasQualified(alias, body)
  -> (1) redirect: exact match on the source spelling
  -> (2) redirect: capture match on the source spelling (most literal characters wins; a tie is an error)
  -> (3) br/<alias> with a moduleAliasesMock.br entry -> ModuleAliasMockRule -> local file (stop)
  -> (4) AliasQualified -> expand through artifacts.aliases and the predefined mar/avm (no I/O)
  ->     resolve through the scheme's parser (OCI, Template Spec) -> remote reference
  -> (5) redirect: exact match on the reference's normalized text
  -> (6) redirect: capture match on the reference's normalized text
  -> (7) no redirect -> ArtifactReference (OciArtifactReference, TemplateSpecArtifactReference, LocalArtifactReference)
```

- **`IArtifactReferenceResolver`** parses, expands aliases and builds the reference (steps 4 and 7). One method: `TryResolve(IOUri referencingUri, ArtifactType type, string text)`.
- **`RedirectingArtifactResolver`** (compiler only) wraps it and applies redirects and the legacy mock (steps 1–3, 5, 6).

Publish targets go straight to the resolver, so redirects can never apply to them. The one exception: `bicep publish` applies `ModuleAliasMockRule` to its target only so that a mocked alias stays an invalid target. Stored ext-src references are fully qualified and are parsed, not resolved.

Steps 5 and 6 match the REP's *canonical identity*: `oci:` or `ts:` text that the redirect engine (PR 4) builds privately from the reference's parts. It isn't a property of `ArtifactReference`. Local references have none; `file:` and file-alias spellings are matched at steps 1 and 2.

**Redirect provenance (PR 4).** The artifact-redirect rule, `--verbose` and hover need to know which redirect selected a local file. Until PR 4, `RedirectingArtifactResolver.TryResolve` returns an `ArtifactReference`. PR 4 changes it to return an `ArtifactResolution`:

```csharp
public enum RedirectPhase { Source, Canonical }

public sealed record ArtifactRedirect(
    string MatchedKey,           // the key in artifacts.redirects
    string MatchedText,          // the text it matched: the source spelling or the canonical identity
    RedirectPhase Phase,
    IOUri DeclaringConfigUri);   // the bicepconfig.json that declared the redirect

public sealed record ArtifactResolution(ArtifactReference Reference, ArtifactRedirect? Redirect);
```

- The target isn't a field: a redirect always produces a `LocalArtifactReference`, whose `File.Uri` is the target.
- A `moduleAliasesMock` hit isn't a redirect and has no `ArtifactRedirect`, so the rule never reports it.
- **Modules and extensions:** `ArtifactDependency` gets `ArtifactRedirect? Redirect` on its base type, because both `Loaded` and `Failed` (missing or invalid target, cycle) can be redirected; `NotRestored` never is. `SourceFileGroupingBuilder` passes `resolution.Redirect` into the dependency, and `ReportFailuresForCycles` carries it over when it replaces a dependency with `Failed`. Implicit extensions get it through their `ArtifactDependency`.
- **Data-file loads** aren't in the source graph: `SystemNamespaceType` resolves them during type checking. Their redirects are recorded per compilation in `SemanticModel.DataFileRedirects` (`FunctionCallSyntaxBase -> ArtifactRedirect`), written by the load result builders. Not on `BicepSourceFile`: a redirect depends on configuration, and a source file can outlive a configuration change.
- **Readers:** the `artifact-redirect` rule reads `ArtifactLookup`, `ImplicitExtensions` and `DataFileRedirects` for its file (forcing the load calls to be type-checked first, since type checking is lazy). Hover reads `ArtifactLookup[syntax].Redirect`. `--verbose` reads every redirected dependency in the grouping.

**Parsing rules** for `ArtifactAddress.Parse`, which is total (it never fails), checked in order:

1. `^(?<alias>[A-Za-z0-9_-]+)::(?<body>.*)$` gives `AliasQualified`. Checked first, so `br::x` is the alias `br`, not the scheme `br`; the resolver then rejects it because scheme names are reserved (question 8).
2. `^(?<scheme>br|ts)/(?<alias>[^:]+):(?<body>.*)$` gives `SchemeAndAliasQualified`, resolved only through `moduleAliases`. Any other `<scheme>/<x>:` form (`oci/…`, `file/…`) gets a new diagnostic: "scheme-qualified aliases are not supported for this scheme; use `<alias>::`".
3. `^(?<scheme>[a-z-]+):(?<body>.*)$` gives `SchemeQualified` (`oci`, `br`, `ts`, `file`). `br-mock` stays rejected (BCP449).
4. Anything else gives `Path`.

An empty body is allowed only for an OCI alias whose body starts with `:` or `@` (the `storage:::0.32.1` form), validated after expansion.

**Reference types**

| Spelling | Reference type | Restored? | Canonical identity for redirects (PR 4) |
|---|---|---|---|
| `oci:`, `br:`, `br/alias:`, OCI alias | `OciArtifactReference` | Yes, into `~/.bicep/br/…` | `oci:<registry>/<repo>(:<tag>|@<digest>)`, registry lowercase |
| `ts:`, `ts/alias:`, Template Spec alias | `TemplateSpecArtifactReference` | Yes | `ts:<sub>/<rg>/<name>:<version>`, lowercase |
| Relative path, `file:`, file alias, redirect target, mock hit | `LocalArtifactReference` | Never | – |

- Every local file uses one reference type, however it's spelled; redirects are provenance on `ArtifactDependency` (above), not reference types. `LocalArtifactReference` equality is `(File, Type)`.
- `OciArtifactReference.FullyQualifiedReference` stays `br:…` (persisted in source archives, ext-src URIs and cache paths that older Bicep versions read). PR 1 makes the resolver parse both `br:` and `oci:` into `OciArtifactReference`.
- Diagnostics that print a reference prefer the user's spelling (`LocalArtifactReference.SourceText`, or the text from `ArtifactDependency.Syntax`).
- A `file:` module must have an extension the compiler recognizes: `.bicep`, `.bicepparam` (where `using`/`extends` allows it) or an ARM template (`.json`, `.jsonc`, `.arm`). Bare paths keep today's behavior (no extension check). Extensions are valid if they decode as an extension archive.

### 3.2 Services

| Question | Owner | Callers |
|---|---|---|
| What artifact does this text name? | `IArtifactReferenceResolver` | The redirect layer, CLI publish commands, test helpers |
| What should the compiler load for this dependency? | `RedirectingArtifactResolver` | `SourceFileGroupingBuilder`; later the `load*()` functions (PR 3) |
| Is this remote artifact cached, and can we download it? | `IRemoteArtifactRestorer` over `IEnumerable<IRemoteArtifactRegistry>` | Compiler, source graph, language server, MCP |
| Can we push it? | `OciArtifactRegistry` (`CheckArtifactExists`, `PublishModule`, `PublishExtension`) | CLI publish commands, test helpers |
| What's in the cache, and where? | Artifact cache entries | Registries, `LocalArtifactLoader` |

- Only `RemoteArtifactReference`s are restored; local references go through `LocalArtifactLoader` (a local extension `.tgz` is extracted into the cache on first use).
- `IRemoteArtifactRegistry` has `Scheme` (`Schemes` from PR 1), `CanHandle` (routes by reference type), `TryOpen` and `RestoreAsync(references, RestoreMode)`.
- The source graph stores each dependency as an `ArtifactDependency`: `Loaded(Artifact)`, `NotRestored(RemoteArtifactReference)` or `Failed(ArtifactReference?, Error)`. It picks the parser from `ModuleArtifact.Kind`.
- Redirect rules are subclasses of one type; the legacy mock is one of them, so deleting the mock means deleting one class.

### 3.3 Artifact model

```
publish:          ModuleArtifactPackage -> OciModuleArtifactCodec.Encode -> OciArtifactPayload -> push
restore:          pull -> OciArtifactPayload -> Decode -> package -> cache entry WriteAsync(package, payload) -> files
compile:          files -> cache entry Open -> ModuleArtifact / ExtensionArtifact
local module:     LocalArtifactReference -> ModuleArtifact(file, SourceFile)
local extension:  .tgz -> LocalExtensionArtifactCodec.Decode -> package -> LocalExtensionArtifactCacheEntry.Write -> Open -> ExtensionArtifact
```

Template Specs have no package or codec; their cache entry opens a `ModuleArtifact` with `Kind = TemplateSpec`. The cache layout is pinned by `ArtifactCacheLayoutTests` and must not change.

### 3.4 File structure

- **`Bicep.Core/Artifacts/`**: the artifact model and services (`References`, `Resolution`, `Redirects`, `LegacyModuleAliasMocks`, `Restore`, `Registries`, `Packages`, `Codecs`, `Cache`, `Catalog`).
- **`Bicep.Core/RegistryClients/`**: OCI distribution protocol (`Oci`, `Oci/AzureSdk`, `Oci/Oras`) and the Template Spec client. Knows nothing about Bicep artifacts, except a few remaining exceptions (`OciArtifactReferenceFacts` grammar, `BicepMediaTypes`, client exceptions deriving from `ArtifactException`).
- New feature code goes in: `Configuration/ArtifactsConfiguration.cs` (PR 2), `Artifacts/Redirects/PatternRedirectRule.cs` and `Analyzers/Linter/Rules/ArtifactRedirectRule.cs` (PR 4).

Tests mirror the product folders under `Bicep.Core.UnitTests/Artifacts/` and `Bicep.Core.UnitTests/RegistryClients/`.

### 3.5 Permanent and deletable reference formats

Every reference syntax customers have written is supported permanently. Only `moduleAliasesMock` is deletable.

| Format or feature | Status |
|---|---|
| `br:<registry>/<repo>:<tag>` | Permanent; another spelling of `oci:`. |
| `ts:<sub>/<rg>/<name>:<version>` | Permanent. |
| `br/public:`, `br/<alias>:`, `ts/<alias>:` with `moduleAliases` | Permanent: scheme-qualified aliases. |
| `oci:`, `file:`, `<alias>::`, `artifacts.*` | Permanent: new in REP 0022. |
| Relative paths; `extensions` values | Permanent. |
| `moduleAliasesMock` | Deletable; superseded by `artifacts.redirects`. |
| `br-mock:` | Removed; still rejected (BCP449). |

The permanent forms are never called "legacy"; the `Legacy` prefix marks only the mock's code (`LegacyModuleAliasMocks`). The two alias namespaces stay independent: scheme-qualified aliases resolve through `moduleAliases`; artifact aliases (`<alias>::`) through `artifacts.aliases` and the predefined `mar`/`avm`. The resolver never falls back from one to the other.

`ModuleAliasMockRule` owns all mock behavior, and `ModuleAliasMockIsolationTests` enforces where it's referenced. The deletion checklist is in a comment on `ModuleAliasMockRule`.

---

## 4. Conventions and notes

- Existing baselines stay byte-identical through PRs 1–4. Diagnostic text isn't frozen: when touching an error, make it clear to users and list the reworded messages in the PR.
- `Azure.Bicep.Core` is published but unsupported, so public API renames are fine. Bicep.Core types stay `public`.
- Full validation: `dotnet build Bicep.slnx --no-restore`, then `dotnet test --solution Bicep.slnx --no-build` (~16,500 tests). Don't pass `--blame-hang-timeout` or `--output Minimal` (zero tests run). Integration tests leave ~18 GB in `bin/Debug/net10.0/TestOutput`; delete those folders if the disk fills.
- Wasm: `AddBicepDecompiler` calls `AddBicepCore` again, so Wasm removes the default registries after both calls.

---

## 5. REP 0022 features

The resolver, the parser, URI-anchored local references and routing by reference type already exist (Phase 0).

### 5.1 Configuration model

New `Configuration/ArtifactsConfiguration.cs`, bound from the top-level `artifacts` key:

```csharp
abstract record ArtifactAlias(string Name);
record OciArtifactAlias(string Name, string Registry, string? RepositoryPrefix);
record TemplateSpecArtifactAlias(string Name, string SubscriptionId, string ResourceGroup);
record FileArtifactAlias(string Name, RelativePath Path, IOUri DeclaringConfigUri);   // Path is relative to DeclaringConfigUri
```

`IBicepArtifactsConfiguration.TryGetAlias(name)` returns an alias or a diagnostic. PR 4 adds `artifacts.redirects` to the same section.

- **Binding never throws.** A bad alias would otherwise invalidate the whole configuration through `InvalidBicepConfigFile`. The section keeps the merged JSON and checks an alias when it's used, as `moduleAliases` errors are reported today. That includes an unknown `type` and an `artifacts` or `aliases` value that isn't an object.
- **Nothing is reported for an alias that isn't used.** Configuration diagnostics are copied into every Bicep file that uses the configuration, so a load-time warning would reach files the alias can't affect, and fail warnings-as-errors builds across a monorepo. The schema flags bad definitions while the configuration is edited. Revisit if configuration plan PRs 6 and 7 add warnings that are shown only on the configuration file.
- **Alias names** use the `ModuleAliasesConfiguration.ModuleAliasNameRegex()` grammar and are case-sensitive.
- **The predefined `mar`/`avm` are defined in code**, not in the built-in `bicepconfig.json`. The configuration chain deep-merges JSON, so a user override would silently inherit the built-in `repositoryPrefix`; defaults defined in code are replaced as a whole entry. Overriding them with `type != "oci"` is an error. The defaults don't depend on the cloud profile.
- **Declaring URIs:** relative paths in file aliases and redirect targets resolve against the `bicepconfig.json` that declares the entry, and alias errors name that file (a missing alias names the leaf, where it would be added). `BicepConfigurationManager.AnnotateDeclaringFiles` gives the section the chain's `ConfigurationProvenance` (`ArtifactsConfiguration.WithProvenance`); configuration plan PR 3 moves this into binding with the other paths.
- **Serialization:** `IBicepConfiguration.Artifacts` and `BicepConfigurationExtensions.With`/`ToUtf8Json` are updated. `artifacts` is written only when a file declares it, so existing configuration dumps are unchanged.

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

- In the `SystemNamespaceType` load helpers, a literal containing `:` resolves through a data-file entry point on `RedirectingArtifactResolver`, so redirects apply (from PR 4). Otherwise the existing relative-path behavior is unchanged. This applies to `loadTextContent`, `loadJsonContent`, `loadYamlContent` and `loadFileAsBase64`, but not `loadDirectoryFileInfo`. A `:` can't appear in a relative path, so this isn't a breaking change.
- The entry point is separate because data files have no `ArtifactType`: `TryResolveDataFile(IOUri referencingUri, string text)` returns the file's URI (and, from PR 4, the `ArtifactRedirect`), supporting only `file:`, file aliases and redirects.
- Allowed sources are `file:`, file aliases, and redirects on those spellings. An `oci:`, `ts:`, OCI alias or Template Spec alias in a data load gets a new "artifact reference not supported for file loading" error (question 7).
- Add a `BicepSourceFile.TryLoadAuxiliaryFile(IOUri)` overload that shares the cache and the tracking of referenced auxiliary files, so language server invalidation keeps working. The resolver is plumbed through `BicepSourceFileFactory`.

### 5.4 `artifact-redirect` linter rule

- `Analyzers/Linter/Rules/ArtifactRedirectRule.cs`, code `artifact-redirect`, default level `Warning`.
- **Sources:** the source graph's `ArtifactDependency.Redirect` for the model's file (modules, `using`, `extends`, imports, explicit, config-named and implicit extensions), plus `SemanticModel.DataFileRedirects`.
- **Span:** the reference string, or the identifier for `extension foo`. **Message:** `"<source>" was redirected to local file "<path>" by "<config>"`.
- **Emitted once per (file, `Redirect.MatchedText`)**, at the first occurrence (question 6). **Not** emitted for `moduleAliasesMock` hits.
- **`UseRecentModuleVersionsRule`** skips redirected references and recognizes `avm::`/`mar::`; its code fix keeps the source spelling.

### 5.5 CLI

- Add `--verbose` (or reuse logging) to `build`, `build-params`, `lint`, `restore`, `publish` and `generate-params`. It prints one stderr line per applied redirect, whatever the analyzer level (question 2).
- `publish` and `publish-extension` accept `oci:` and `alias::` targets and reject `file:` for modules. Targets go through `IArtifactReferenceResolver`, so redirects can't apply. Update `publish-extension`'s `br:`/`ts:` prefix check.
- `restore`: redirected references are local, so there's nothing to restore.
- The MCP extension tools accept `oci:` as well as `br:` once the experimental flag is removed (PR 6): the MCP server has no `bicepconfig.json` to read the flag from. `Bicep.Wasm/Interop.cs` already skips every scheme-qualified module path, so it needs no change.

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

Each PR is complete when it ships: new syntax lands together with its resolution path. All of it is gated behind the `artifactAliasesAndRedirects` experimental feature, so users can try it and give feedback before it becomes the default (question 1). With the flag off, behavior and messages match the release before REP 0022; PR 6 removes the flag.

| # | PR | Main files | Tests |
|---|---|---|---|
| 1 | **Configuration-free spellings: `oci:`, `file:` and `<alias>::` parsing.** `oci:` resolves to `OciArtifactReference` (equal to the `br:` spelling) and is accepted as a publish target; `IRemoteArtifactRegistry.Scheme` becomes `Schemes`. `file:` resolves to `LocalArtifactReference` relative to the referencing file, for modules, `using`, `extends`, imports and extensions; in the `extensions` map, relative to the configuration, like bare paths (question 3). Absolute `file:` paths are rejected (question 4). A module must be a file whose extension the compiler recognizes: `.bicep`, `.bicepparam`, or an ARM template (`.json`, `.jsonc`, `.arm`) (BCP461). `oci/x:` and `file/x:` get "the scheme does not support aliases" (BCP460). `ArtifactAddress.AliasQualified` is parsed but rejected until PR 2. OCI parse errors show the scheme as written. BCP189 lists the new schemes. Adds the `artifactAliasesAndRedirects` experimental feature, which gates `oci:`, `file:` and BCP460. | `ArtifactSchemes`, `ArtifactAddress`, `ArtifactReferenceResolver`, `ArtifactReferenceText`, `IRemoteArtifactRegistry`, `PublishExtensionCommand`, feature flag plumbing | Parser tests for `oci:localhost:5000/x:v1`, `br::x`, `storage:::1.0`; `oci:` and `br:` are equal and deduplicated in restore; publish to `oci:`; source archives still write `br:`; `file:` for each declaration kind and the `extensions` map; absolute and wrong-extension `file:` errors; every new spelling fails as before with the flag off. |
| 2 | **`artifacts.aliases`** (5.1): OCI, Template Spec and file aliases, the predefined `mar`/`avm`, the empty-path OCI form, and a "use `<alias>::`" hint added to BCP460. Independent of `moduleAliases`. The schema gets `artifacts.aliases` only. | `ArtifactsConfiguration`, `BicepConfiguration*`, resolver, `DiagnosticBuilder`, schema | Binding; inheritance (an alias declared in a base configuration resolves from its directory); predefined-override type error; whole-entry replacement; expansion matrix; `br/x:` with only `artifacts.aliases.x` still gives the unknown-alias error; the same name in both maps; the REP's monorepo scenario; transitive references use each file's own configuration. |
| 3 | **Data-file loads** (5.3) through `file:` and file aliases. | `SystemNamespaceType`, `BicepSourceFile`, `BicepSourceFileFactory`, resolver | Loads through aliases; size and encoding limits unchanged; language server invalidation; remote spellings rejected. |
| 4 | **Redirects with disclosure** (5.2, 5.4): `artifacts.redirects` and its schema, the redirect engine for modules, extensions, Template Specs and data files, redirect provenance (3.1), and the `artifact-redirect` rule. Shipped together so a redirect is never applied without the diagnostic. | `PatternRedirectRule`, `RedirectingArtifactResolver`, `ArtifactDependency`, `SourceFileGroupingBuilder`, `SemanticModel`, `ArtifactRedirectRule`, schema | Precedence matrix including mock vs. redirect; tags, digests and nested paths; `{...path}`; rejecting a captured `..`; missing target; path normalization on Windows and Linux; publish ignores redirects; provenance kept on `Failed` and cycle dependencies; rule levels (an error blocks emit); suppression; no diagnostic for mocks. |
| 5 | **`--verbose` and language server UX** (5.5, 5.6). `ModuleReferenceCompletionProvider` builds its items from `ArtifactSchemes` instead of `"br:"`/`"ts:"` literals. | CLI, completion, hover and definition handlers | Verbose output even when the rule is `off`; language server unit and integration tests. |
| 6 | **Docs, samples and removing the flag**: drop `artifactAliasesAndRedirects` (its schema entry and docs), accept `oci:` in the MCP extension tools, baseline datasets, Learn docs, linter rule docs, changelog. | `Bicep.Core.Samples/Files/baselines/ArtifactAliases_*`, docs | Baselines for alias, redirect and file scenarios. |

Each PR depends on the one before it, except PR 5, which needs only PRs 2 and 4.
### 5.8 Diagnostics to add (next free BCP codes; BCP465 is currently the highest)

- Unknown artifact alias (BCP462, PR 2).
- Artifact alias name reserved for a scheme (BCP465, PR 2; question 8).
- Invalid alias definition: missing or unknown `type`, a missing or empty required property, a non-relative file `path`, or a predefined `mar`/`avm` overridden with a type other than `oci` (BCP463, PR 2). The message names the invalid property, such as `artifacts.aliases.contoso`.
- Scheme-qualified alias form (`oci/x:`) not supported (BCP460, PR 1; PR 2 adds the "write `<alias>::`" hint).
- Empty artifact path after an alias, or an OCI tag or digest after an alias without `repositoryPrefix` (BCP464, PR 2).
- A module `file:` reference or file alias must be a Bicep file or an ARM template (BCP461, PR 1; PR 2 covers file aliases).
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
- All existing baselines are byte-identical after PRs 1–4.

---

## 6. Open questions

1. **A temporary experimental flag** *(decided)*: yes, `artifactAliasesAndRedirects`, so the feature can collect feedback before it becomes the default (requested in review of #20580). The REP's rollout plan says "no flag"; update it when the flag is removed.
2. **`--verbose` doesn't exist.** Add a global option, or use a log level?
3. **`file:` values in the `extensions` map** *(decided)*: relative to the `bicepconfig.json` that declares the entry, the same as bare paths. Not fully implemented yet, for bare paths and `file:` alike: `extension foo` resolves them against the leaf configuration rather than the declaring one (bug B2 in `configuration-improvement-plan.md`), and an entry used through `implicitExtensions` resolves them against the referencing file (question 11). Configuration plan PR 3 fixes both together; `file:` values aren't fixed separately. A file alias (`"foo": "exts::foo.tgz"`) already resolves against the configuration that declares the alias in both cases.
4. **Absolute `file:` paths** *(decided)*: rejected, with the same error as other absolute paths.
5. **Ambiguous redirect detection:** an error at resolution time, or static configuration validation?
6. **Rule granularity:** once per redirected artifact per compilation, or per occurrence (for `#disable-next-line`)?
7. **Redirecting remote identities for data loads** (an `oci:` key in `loadTextContent`): out of scope, or an error?
8. **Alias names equal to scheme names** *(decided)*: reserved. `br`, `oci`, `ts`, `file` and `br-mock` can't be artifact alias names, compared case-insensitively, because `br::x` is almost certainly a mistyped `br:x`. Using one gives BCP465, even if `artifacts.aliases` defines it, and the schema rejects defining one.
9. **Diagnostic display:** in existing restore-failure messages for `oci:` and alias references, show the source spelling or the fully qualified `br:` reference?
10. **REP wording:** the REP's "Future deprecation of `moduleAliases`" section could state that `br/<alias>:`, `ts/<alias>:` and `br/public:` are permanent and only `moduleAliasesMock` may be deprecated.
11. **Bare paths for implicit extensions** *(out of scope)*: an `extensions` entry used through `implicitExtensions` resolves its bare path relative to the referencing file instead of `bicepconfig.json` (pinned in `ArtifactReferenceResolutionTests`). It's a configuration bug, fixed separately from REP 0022.
