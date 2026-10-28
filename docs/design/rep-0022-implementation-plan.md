# REP 0022 – Artifact aliases and redirects: implementation plan

Source: Azure/bicep-reps `active/0022-artifact-aliases-and-redirects.md` (tracking issue #19883).
Baseline: `main` @ 31ea91a8a.

---

## 1. Current state (what exists today)

| Concern | Where | Notes |
|---|---|---|
| Reference string → scheme/alias split | `src/Bicep.Core/Registry/ModuleDispatcher.cs` `TryGetArtifactReference(...)` | Splits on the **first** `:`; a `/` in the scheme part means a scheme-qualified alias (`br/alias:`). `avm::x` would currently parse as scheme `avm` + body `:x` → `UnknownModuleReferenceScheme`. |
| Extension name → reference | `ModuleDispatcher.TryGetArtifactAddress` + `SourceFileGroupingBuilder.GetImplicitExtension` | Values without `:` are resolved relative to `config.ConfigFileUri` (the **leaf** config, not the declaring one). |
| Registries | `DefaultArtifactRegistryProvider`: `LocalModuleRegistry` (`""`), `OciArtifactRegistry` (`br`), `OciArtifactMockedRegistry` (`br-mock`), `TemplateSpecModuleRegistry` (`ts`) | `IArtifactRegistryProvider.GetRegistry(scheme)` is keyed on `ArtifactReference.Scheme`. |
| OCI reference | `Registry/Oci/OciArtifactReference.cs`, `OciArtifactAddressComponents.cs`, `OciArtifactReferenceFacts.cs` (`Scheme = "br"`) | `FullyQualifiedReference` = `br:<registry>/<repo>:<tag>`. This string is **persisted**: source archives (`SourceLink/ArchivedSourceFileMetadata.cs`, `SourceArchiveMetadata.cs`), ext-src URIs (`LangServer/.../ExternalSourceReference.cs`, `vscode-bicep/src/features/external-source/external-source-uri.ts`), restore-failure keys. |
| Scheme-qualified module aliases (permanent) | `Configuration/ModuleAliasesConfiguration.cs` (`br`, `ts` maps; alias regex `^[a-zA-Z0-9-_]+$`) | `br/public` lives in built-in `Configuration/bicepconfig.json`. |
| Alias mock | `OciArtifactRegistry.TryParseArtifactReference` → `OciArtifactMockedReference.TryParse` | Uses `DeclaringConfigUri` populated by `BicepConfigurationManager.BuildAliasDeclaringUriMap` (walks the `extends` chain leaf-first). |
| Local file reference | `Modules/LocalModuleReference.cs` | Anchored on referencing file. **`Equals` compares only the relative path string** (not the referencing file) – will be wrong once the same text can resolve to different files. Local extensions are cached content-addressed under `~/.bicep/local/sha256_<digest>`. |
| Source graph | `SourceGraph/SourceFileGroupingBuilder.cs` | `forceRestore` special-cases `Scheme == Local`; cycle detection in `ReportFailuresForCycles`. |
| Data-file loads | `Semantics/Namespaces/SystemNamespaceType.cs` (`TryLoadTextContentFromFile`, `TryLoadTextContentAsBase64`, JSON/YAML builders) → `BicepSourceFile.TryLoadAuxiliaryFile(RelativePath)` | `:` is a forbidden relative-path char (`Bicep.IO/Abstraction/FilePathFacts.cs`), so treating `:`-containing args as artifact references is **non-breaking**. |
| Config chain | `Configuration/BicepConfigurationManager.cs` `BuildChain` | JSON deep-merge of layers; declaring-URI annotation only for `moduleAliasesMock` and `bicep.version`. |
| LS consumers of the dispatcher | `Hover/BicepHoverHandler.cs`, `Definition/BicepDefinitionHandler.cs`, `DocumentLink/BicepExternalSourceRequestHandler.cs`, `Completion/ModuleReferenceCompletionProvider.cs`, `CodeLens/ExternalSourceCodeLensProvider.cs` | Completions hard-code `br:`, `br/`, `ts/`. |
| Linter | `Analyzers/Linter/Rules/*Rule.cs`; `UseRecentModuleVersionsRule` keys off `IOciArtifactReference` + MCR registry | |
| Other `br:` string checks | `Bicep.Cli/Commands/PublishExtensionCommand.cs:136`, `Bicep.Wasm/Interop.cs:242`, `Bicep.McpServer.Core/BicepTools.cs`, `.../WellKnownExtension.cs` | |
| Schema | `src/vscode-bicep/resources/configuration/bicepconfig.schema.json` (`moduleAliases` L240, `moduleAliasesMock` L268, `extensions` L287, linter rules ~L843) | |
| CLI verbosity | No `--verbose` option exists in `Bicep.Cli`. | REP requires redirects be visible in "verbose CLI output". |

---

## 2. Target architecture

### 2.1 One resolver pipeline

Introduce `Bicep.Core/Registry/Resolution/` and route **every** reference through it (dispatcher, extensions-config values, implicit extensions, data-file loads, LS):

```
ArtifactReferenceText (string)
  └─ ArtifactReferenceSyntaxParser.Parse            → ParsedArtifactReference
       Relative(path) | Qualified(scheme, body) | SchemeQualifiedAlias(scheme, alias, body) | ArtifactAlias(alias, body)   (see 2.11)
  ├─ (1) redirect: exact match on source spelling
  ├─ (2) redirect: capture match on source spelling (most literal chars; tie → error)
  ├─ (3) br/<alias> + moduleAliasesMock.br hit → ModuleAliasMockResolver → local file (STOP)   [2.11.3]
  ├─ (4) Alias → expand via artifacts.aliases ∪ predefined {mar, avm}  (no I/O)
  ├─     canonicalize through scheme handler (OCI/TS/File parsers)    → CanonicalIdentity
  ├─ (5) redirect: exact match on canonical identity
  ├─ (6) redirect: capture match on canonical identity
  └─ (7) no redirect → existing ArtifactReference (Oci/TemplateSpec/File/Local)
```

Outputs an `ArtifactResolution` record: `{ SourceText, Parsed, CanonicalIdentity?, Reference, Redirect? }` where `Redirect = { MatchedKey, Phase(Source|Canonical), DeclaringConfigUri, TargetUri }`.

After Phase 0 this pipeline lives in `IArtifactReferenceResolver` (which replaces the resolution half of `ModuleDispatcher`). Redirects **must not apply to publish targets or ext-src requests** (otherwise `publish-extension --target oci:…` with a matching redirect would write into the local file via the local registry's publish) – enforced by `TryResolvePublishTarget` and an ext-src resolution path that skip redirects.

### 2.2 Parsing rules (`ArtifactReferenceSyntaxParser`)

Ordered checks on the raw string:
1. `^(?<alias>[A-Za-z0-9_-]+)::(?<body>.*)$` → `Alias`. (Checked first so `br::x` is alias `br`, not scheme `br`.)
2. `^(?<scheme>br|ts)/(?<alias>[^:]+):(?<body>.*)$` → `SchemeQualifiedAlias` (permanent; resolved only via `moduleAliases`). Any other `<scheme>/<x>:` form (`oci/…`, `file/…`) → new diagnostic "scheme-qualified aliases are not supported for this scheme; use `<alias>::`".
3. `^(?<scheme>[a-z-]+):(?<body>.*)$` → `FullyQualified` (`oci`, `br`, `ts`, `file`; `br-mock` stays rejected).
4. Otherwise → `Relative` (existing `LocalModuleReference`).

Body rules: empty body allowed only for OCI alias when body starts with `:`/`@` (the `storage:::0.32.1` form) – validated after expansion; other alias types require non-empty path.

### 2.3 Reference types and identity

| Kind | Type | `Scheme` (registry key) | `CanonicalIdentity` |
|---|---|---|---|
| `oci:` / `br:` / `br/alias:` / OCI alias | existing `OciArtifactReference` | keep `"br"` internally | `oci:<registry>/<repo>(:<tag>|@<digest>)` |
| `ts:` / `ts/alias:` / TS alias | existing `TemplateSpecModuleReference` | `ts` | `ts:<sub>/<rg>/<name>:<ver>` |
| `file:` / file alias / redirect target | **new** `FileArtifactReference(IOUri target, ArtifactType, string sourceText)` | **new** `"file"` | normalized absolute URI |
| relative path | existing `LocalModuleReference` | `""` | normalized absolute URI |
| module alias mock | existing `OciArtifactMockedReference` → `LocalModuleReference` (provenance `ModuleAliasMock`) after Phase 0.5 | `br-mock` → removed | n/a |

Decisions:
- **Do not change `OciArtifactReferenceFacts.Scheme` / `FullyQualifiedReference`** (`br:`) – it is persisted in source archives, ext-src URIs and cache paths read by older Bicep versions. Add `ArtifactReference.CanonicalIdentity` (virtual) instead; `OciArtifactRegistry` registers for both `br` and `oci` *parse* schemes.
- Diagnostics that print a reference should prefer the user's source spelling where available (pass `SourceText` through `ArtifactResolution`).
- `FileArtifactReference.Equals` = `(TargetUri, ArtifactType)`. Also fix `LocalModuleReference.Equals` to compare resolved URI (latent bug that becomes reachable once `RestoreArtifacts` dedups mixed local references).
- New `FileArtifactRegistry` (scheme `file`): modules are not restorable; extensions reuse the content-addressed `local/sha256_<digest>` cache (extract shared code from `LocalModuleRegistry` into a helper). Content-addressing satisfies "never stored under the source's remote identity". Publish to `file:` modules → not supported; `publish-extension --target file:…` → write the file (parity with relative targets).
- Module file extension check: `.bicep` / `.json` (and `.bicepparam` only where `using`/`extends` already allows it). Extensions: no extension requirement; validity from `ExtensionV1Archive.Read`.

### 2.4 Configuration model

New `Configuration/ArtifactsConfiguration.cs` bound from top-level `artifacts` (key constant in `BicepConfiguration`):

```csharp
record ArtifactsConfigData {
  ImmutableSortedDictionary<string, ArtifactAlias> Aliases;     // polymorphic on "type"
  ImmutableDictionary<string, string> Redirects;                 // key pattern -> relative target
}
abstract record ArtifactAlias { IOUri? DeclaringConfigUri }      // [JsonIgnore], set by chain builder
record OciArtifactAlias(string Registry, string? RepositoryPrefix);
record TemplateSpecArtifactAlias(string SubscriptionId, string ResourceGroup);
record FileArtifactAlias(string Path);
```

- Binding must **not throw** on semantic errors (a bad alias would otherwise invalidate the whole config via `InvalidBicepConfigFile`). Store per-entry `DiagnosticBuilderDelegate` errors; surface them when an entry is used (matches how `ModuleAliasesConfiguration` errors surface today). Unknown `type` → per-entry error.
- Alias name grammar: reuse `ModuleAliasesConfiguration.ModuleAliasNameRegex()`; case-sensitive (`StringComparer.Ordinal`).
- **Predefined `mar`/`avm` are defined in code** (`PredefinedArtifactAliases`), *not* in built-in `bicepconfig.json`. Reason: `BuildChain` deep-merges JSON, so a user override `{type:"oci", registry:"contoso.azurecr.io"}` would silently inherit the built-in `repositoryPrefix`. Code-defined defaults give whole-entry replacement. Override with `type != "oci"` → error. Defaults are cloud-profile-independent.
- **Declaring URIs:** generalize `BicepConfigurationManager.BuildAliasDeclaringUriMap` into a per-section helper and annotate `artifacts.aliases.*` (file aliases) and `artifacts.redirects.*` with the leaf-most declaring layer. Relative paths resolve against that file’s directory, not the leaf.
- `IBicepConfiguration.Artifacts`, `BicepConfigurationExtensions.With(...)`/`WriteTo` updated so `bicep` config dumping/round-tripping still works.

### 2.5 Redirect engine (`ArtifactRedirectTable`)

- Compile each key once (lazily, cached on the config object) into: literal-char count, capture list, and an internally generated regex (keys are templates, never user regex):
  - `{name}` → `(?<name>[^/:@]+)` (one segment; also must not be `.`/`..`)
  - `{...name}` → `(?<name>[^/:@]+(?:/[^/:@]+)*)`
  - Literal text `Regex.Escape`d. Treat `::` as a delimiter for both capture kinds (excluded since `:` already is).
- Key validation: capture names `^[A-Za-z_][A-Za-z0-9_]*$`, unique, ≤1 multi-segment capture, no `*`/`?`/`**`/`[`; key must itself parse via `ArtifactReferenceSyntaxParser` (with captures replaced by a placeholder) as non-relative.
- Target validation: starts with `./` or `../`; no absolute path; not an artifact reference; every `{x}` declared in the key; after substitution validate with `RelativePath`-equivalent rules (allowing leading `../`), and each captured segment ≠ `.`/`..`, no `\`, no forbidden chars. Resolve against `DeclaringConfigUri`; must be an existing file (not directory) – else "redirect target not found" (no fallback search).
- Matching per phase: exact keys first (dictionary lookup), then all capture keys; pick max literal count; if >1 tie → error diagnostic "ambiguous artifact redirect" listing keys. (REP says "configuration validation fails"; static overlap detection of templates is non-trivial – implement as a resolution-time error on the reference; optionally add a best-effort static check for identical-shape keys. Confirm with REP author.)
- Applied at most once per reference; references inside the target file are resolved fresh (its own closest config), cycles caught by existing `CycleDetector`.

### 2.6 Data-file loads

- In `SystemNamespaceType` load helpers: if the literal contains `:` → `resolver.ResolveDataFile(sourceFile, text)`; else unchanged `RelativePath` path. Applies to `loadTextContent`, `loadJsonContent`, `loadYamlContent`, `loadFileAsBase64` (not `loadDirectoryFileInfo`).
- Allowed sources: `file:` and file aliases; redirects on those spellings. `oci:`/`ts:`/OCI/TS alias in a data load → new "artifact reference not supported for file loading" error (REP is silent on redirecting remote identities for data loads – see open questions).
- Add `BicepSourceFile.TryLoadAuxiliaryFile(IOUri)` overload sharing the cache + `referencedAuxiliaryFileUris` tracking (so LS invalidation and the `LoadContentAsBase64ResultBuilder` display path keep working). `BicepSourceFile` needs the resolver/`IFileExplorer` – plumb through `BicepSourceFileFactory`.

### 2.7 `artifact-redirect` analyzer rule

- `Analyzers/Linter/Rules/ArtifactRedirectRule.cs`, `LinterRuleBase`, code `artifact-redirect`, default `Warning`.
- Sources of redirects: `SourceFileGrouping` artifact resolution infos for the model’s file (modules, `using`, `extends`, imports, explicit extensions, config-named extensions, implicit extensions) + load-function literals (re-resolve via the cached resolver).
- Span: the reference string literal (or the identifier for `extension foo`). Message: `"<source>" was redirected to local file "<path>" by "<config>"`.
- Emit once per (file, canonical identity) at first occurrence (REP: "once per redirected artifact during a compilation"; confirm intended granularity vs per-occurrence `#disable-next-line`).
- **Not** emitted for `moduleAliasesMock` hits.
- `UseRecentModuleVersionsRule`: skip redirected references; recognize `avm::`/`mar::` (it already works off the resolved `IOciArtifactReference`, so mostly free) and ensure its code fix preserves the source spelling.

### 2.8 CLI

- Add `--verbose` (or reuse logging) to `build`, `build-params`, `lint`, `restore`, `publish`, `generate-params`; print one stderr line per applied redirect regardless of analyzer level.
- Publish/publish-extension: accept `oci:` and `alias::` targets; reject `file:` for modules; **resolve targets via `IArtifactReferenceResolver.TryResolvePublishTarget` (no redirects)**. Update the `br:`/`ts:` prefix check in `PublishExtensionCommand.cs:136`.
- `restore`: redirected refs are no longer external → nothing to restore (natural).
- `Bicep.Wasm/Interop.cs:242` and MCP `BicepTools.cs` filters: include `oci:`/alias forms.

### 2.9 Language server / VS Code

- Completion (`ModuleReferenceCompletionProvider`): top-level items for `oci:`, `file:`, and each `artifacts.aliases` + `mar::`/`avm::`; reuse existing registry/module/version completion logic by expanding the alias to its OCI location; file-path completion after `file:` and `<fileAlias>::` (reuse the existing local path completion in `BicepCompletionProvider`, re-rooted).
- Same for extension declarations and load-function string args.
- Definition/hover/document links: `FileArtifactReference` → open target file; redirected OCI → open local file and show "redirected from …" in hover. ext-src `br:` URIs unchanged.
- File watching: redirect targets and alias roots are ordinary source/aux files → existing invalidation; `bicepconfig.json` change already triggers recompilation (verify `artifacts` section is included in the config chain dependency set – it is, as it’s the same file).
- Schema (`bicepconfig.schema.json`): `artifacts.aliases` (`propertyNames` pattern, `oneOf` on `type`: oci/ts/file with required props; `if name in [mar, avm] then type const oci`), `artifacts.redirects` (`additionalProperties: { pattern: "^\\.\\.?/" }`), `analyzers.core.rules.artifact-redirect`.

---

## 2.10 Phase 0 – Refactor artifact references, resolution and dispatching (no behavior change)

Goal: make the REP work additive. Every Phase 0 PR must leave **all baselines and test expectations unchanged** (except tests that only exercise renamed types). `Azure.Bicep.Core` is published to nuget.org but explicitly unsupported ("we reserve the right to push breaking changes"), so renames are fine; call them out in release notes.

### 2.10.1 Problems found (evidence)

| # | Problem | Evidence | Why it blocks REP 0022 |
|---|---|---|---|
| P1 | **`IModuleDispatcher` is a god object with a misleading name.** It is a reference factory (`IArtifactReferenceFactory`), a registry router, a restore orchestrator with a 30-min failure cache, a cache-path lookup (`TryGetLocalArtifactEntryPointFileHandle`), and a publisher. It handles extensions, Template Specs, `using`/`extends`/`import`, not only modules. Interface declared in `IArtifactDispatcher.cs`, class `ModuleDispatcher`, tests in `ArtifactDispatcherTests.cs`. | `Registry/IArtifactDispatcher.cs:22`, `Registry/ModuleDispatcher.cs` | Resolution gains alias/redirect logic; mixing it with restore/publish makes "don't redirect publish targets" an option flag instead of a type boundary. |
| P2 | **References are resolved many times instead of once.** `SourceFileGrouping.ArtifactLookup` already stores the `ArtifactReference` per syntax, yet `Compilation`/`SemanticModel`/`SymbolContext` carry an `IArtifactReferenceFactory` so `ImportedSymbol`/`WildcardImportSymbol` re-resolve (`ImportClosureInfo.cs:255,265`), and the LS re-resolves in `BicepDefinitionHandler.cs:108,119,132,212,472` and `BicepHoverHandler.cs:231` — passing **`SourceFileGrouping.EntryPoint`** as the referencing file (wrong config/anchor for nested files). | grep `ArtifactReferenceFactory`, `moduleDispatcher.TryGetArtifactReference` | With aliases/redirects, re-resolution can diverge from what the compiler used (different referencing file → different `bicepconfig.json`). REP requires "the compiler and language server use the same resolver so build, diagnostics, completion, and navigation do not disagree". |
| P3 | **Registries parse.** `IArtifactRegistry.TryParseArtifactReference(referencingFile, artifactType, aliasName, text)`; alias expansion lives in `OciArtifactReference.TryParseComponents` / `TemplateSpecModuleReference.TryParse`; mock lives in `OciArtifactRegistry.TryParseArtifactReference`. Splitting text is in `ModuleDispatcher.TryGetArtifactReference` (first `:` then `/`), and the extension-config path has its own rules (`TryGetArtifactAddress`). | `Registry/IArtifactRegistry.cs:34`, `OciArtifactRegistry.cs:65-110`, `ModuleDispatcher.cs:39-139` | The REP's ordered pipeline (source redirect → mock → alias → canonicalize → canonical redirect) must cut across all schemes. |
| P4 | **Scheme string is overloaded.** `ArtifactReference.Scheme` is the registry routing key, the display prefix and (for OCI) the persisted prefix (`br:` in source archives / ext-src URIs / cache path `~/.bicep/br/...`). `br-mock` exists only so the dispatcher can route a mock reference to `OciArtifactMockedRegistry`, whose parse method just errors. `ArtifactReferenceSchemes` lives in `Modules/ModuleReferenceSchemes.cs`. | `Modules/ModuleReferenceSchemes.cs`, `OciArtifactMockRegistry.cs`, `BicepRegistryArtifact.cs:52` | Adding `oci`/`file` and canonical identity on top of an overloaded string is error-prone. |
| P5 | **`LocalModuleReference` is overloaded and has wrong identity.** Covers modules *and* extensions (TODO L21); for extensions its "entry point" is the cached `types.tgz` (TODO L86); anchored on `referencingFile`; `Equals` compares only the relative string, so `./a.bicep` from two different folders are "equal" (matters for `RestoreArtifacts` `Distinct()` and restore-failure keys). `OciArtifactMockedReference` is a second local-file reference type. | `Modules/LocalModuleReference.cs` | `file:`, file aliases and redirect targets are local files anchored elsewhere (config dir) – needs a URI-anchored type. |
| P6 | **"Artifact" means two things.** `SourceGraph/Artifacts/*` (`ExternalArtifact`, `BicepRegistryArtifact`, `LocalExtensionArtifact`, `TemplateSpecModuleArtifact`) are actually **cache-directory layouts**, not artifacts. `ExternalArtifactRegistry` really means "registry backed by the local cache". | `SourceGraph/Artifacts/*.cs`, `Registry/ExternalArtifactRegistry.cs` | Confusing vocabulary once "artifact" also means alias/redirect target. |
| P7 | **Registry interface mixes concerns.** `IArtifactRegistry` has parse, restore, invalidate, publish, exists, capabilities, `GetDocumentationUri`, and `TryGetModuleDescription(ModuleSymbol, …)` (semantic type leaking into the registry; used only by hover). | `Registry/IArtifactRegistry.cs` | Not blocking, but every new reference kind must implement all of it. |
| P8 | **References carry ambient services.** `ArtifactReference` holds `IFeatureProvider` + `IBicepConfiguration`; registries read `reference.Configuration.Cloud` (auth session, restore-failure key), `reference.FeatureProvider.CacheRootDirectory`, `.OciEnabled`. | `ArtifactReference.cs`, `OciArtifactRegistry.cs:312,557,609`, `ModuleDispatcher.cs:164-258` | Muddies "identity"; but each reference is legitimately per-file-config. Narrow, don't remove (see 0.7). |
| P9 | **Scattered folders/namespaces.** `Bicep.Core.Modules` (Local/TemplateSpec refs, schemes, `OciModuleArtifactResult`), `Bicep.Core.Registry` (+`Oci`, `Providers`, `Catalog`), `SourceGraph/Artifacts`, `SourceGraph/ArtifactReferences` (one interface). | directory listing | Discoverability. |

### 2.10.2 Naming: replace `ModuleDispatcher`

"Dispatcher" describes the internal routing-by-scheme mechanism, not what callers want; "Module" is simply wrong (extensions, TS, using/extends/import). **Decision: split it into two services along the boundary that matters for REP 0022 – text → reference (where aliases/redirects apply) vs. operations on an already-resolved reference (registry/cache I/O):**

| New service | Members (from today's `IModuleDispatcher`) | Production consumers |
|---|---|---|
| **`IArtifactReferenceResolver`** (absorbs `IArtifactReferenceFactory`) | `TryResolve(BicepSourceFile, IArtifactReferenceSyntax)`, `TryResolve(BicepSourceFile, ArtifactType, string)`, `TryResolvePublishTarget(ArtifactType, string)` (never applies redirects); later: alias expansion + redirects | `SourceFileGroupingBuilder`, `PublishCommand`, `PublishExtensionCommand`, `BicepExternalSourceRequestHandler`, `TestExternalArtifactManager`, `DataSetsExtensions` (LS hover/definition and semantic layer stop calling it – see 0.2) |
| **`IArtifactDistributionManager`** | Restore side: `RestoreArtifacts`, `GetRestoreStatus` (was `GetArtifactRestoreStatus`), `TryGetEntryPoint` (was `TryGetLocalArtifactEntryPointFileHandle`), `PruneRestoreStatuses`. Publish side: `PublishModule`, `PublishExtension`, `ModuleExists`, `ExtensionExists`, `CanPublish(ArtifactType, ref)` (was `GetRegistryCapabilities(...).HasFlag(Publish)` – only used for publish). | Restore: `BicepCompiler`, `SourceFileGroupingBuilder`, `ModuleRestoreScheduler`, `BicepForceModulesRestoreCommandHandler`, `BicepExternalSourceDocumentLinkHandler`, `BicepExternalSourceRequestHandler`, MCP `ExtensionTypeLoaderProvider`. Publish: `PublishCommand`, `PublishExtensionCommand`, `TestExternalArtifactManager`. |

"Distribution" follows OCI vocabulary (the OCI Distribution Spec covers pull/push/exists) and also fits Template Specs; restore and publish share registry routing (`IArtifactRegistryProvider`), sessions and cloud/auth context, so they live in one class.

Guardrails so `IArtifactDistributionManager` does not regrow into a god object:
- Accepts only `ArtifactReference` – never strings or syntax (no parsing).
- Doc comment: "operations on resolved artifact references that touch a registry or the local artifact cache."
- Restore-failure tracking is an internal detail.
- Hover metadata (`GetDocumentationUri`, `TryGetModuleDescription`) goes to `IArtifactMetadataProvider`, not here.

Benefits: publish targets are resolved via `TryResolvePublishTarget`, which has no redirects by construction; "resolver" matches REP vocabulary ("The compiler and language server use the same resolver"); registry routing stays an internal detail of the distribution manager.

Alternatives considered: single rename to `ArtifactManager`/`ArtifactService` (keeps the god object); three-way split resolver/restorer/publisher (publisher has only 3 consumers and shares all routing with restore); `ArtifactLifecycleManager`, `ArtifactCacheManager` (undersells publish), `ArtifactRegistryManager` (clashes with `AzureContainerRegistryManager`), `ArtifactDistributionClient`/`Coordinator`. Rename in one PR (35 files, mostly mechanical); add a temporary `[Obsolete] IModuleDispatcher : IArtifactReferenceResolver, IArtifactDistributionManager` only if a staged migration is needed.

Related renames (same PR set):
- `IArtifactReferenceFactory` → folded into `IArtifactReferenceResolver`.
- `SourceGraph/Artifacts/ExternalArtifact` & subclasses → `Artifacts/Cache/*CacheEntry` (`OciCacheEntry`, `OciModuleCacheEntry`, `OciExtensionCacheEntry`, `TemplateSpecCacheEntry`, `LocalExtensionCacheEntry`); `IExtensionArtifact` → `IExtensionCacheEntry`.
- `ExternalArtifactRegistry<,>` → `CachedArtifactRegistry<,>`.
- `ArtifactReferenceSchemes` → `ArtifactSchemes` in its own file; becomes **text-only** constants (what users type).
- `ArtifactResolutionInfo` → keep name (now accurate) but add `Resolution` details (see below).

### 2.10.3 Target model after Phase 0

```
Bicep.Core/Artifacts/
  ArtifactType.cs, ArtifactSchemes.cs
  References/   ArtifactReference (abstract), OciArtifactReference, TemplateSpecArtifactReference,
                LocalModuleReference, LocalExtensionReference, ArtifactReferenceKind (enum: Local, Oci, TemplateSpec)
  Resolution/   ArtifactAddress (+ parser), IArtifactReferenceResolver, ArtifactReferenceResolver,
                ArtifactResolution (SourceText, Address, Reference, Provenance)
  Registries/   IArtifactRegistry, IArtifactRegistryProvider, OciArtifactRegistry, TemplateSpecArtifactRegistry,
                LocalArtifactRegistry, CachedArtifactRegistry<,>
  Cache/        *CacheEntry
  Distribution/ IArtifactDistributionManager, ArtifactDistributionManager (restore + publish; failure cache lives here)
  Oci/ …        (existing OCI transport/manifest code moves under Artifacts/Oci or stays in Registry/Oci – decide in 0.6)
```

Key contracts:

```csharp
// pure, no I/O, no config
abstract record ArtifactAddress(string Text) {
  record Relative(RelativePath Path);
  record Qualified(string Scheme, string Body);           // br:, ts:  (later oci:, file:)
  record SchemeQualifiedAlias(string Scheme, string Alias, string Body); // br/x:, ts/x: (permanent, moduleAliases)
  // later: record Alias(string Name, string Body);       // x::
  static ResultWithDiagnosticBuilder<ArtifactAddress> TryParse(string text);
}

abstract class ArtifactReference {               // value-ish
  ArtifactReferenceKind Kind; ArtifactType Type;
  string FullyQualifiedReference;                // persisted/display form – unchanged ("br:…", "ts:…", relative path)
  bool IsExternal;
  // Phase 1 adds: string CanonicalIdentity; ArtifactProvenance Provenance (Direct | ModuleAliasMock | Redirect)
}

interface IArtifactRegistry {                     // no parsing
  ArtifactReferenceKind Kind;
  bool IsRestoreRequired(ArtifactReference); Task<…> Restore(…); Task<…> InvalidateCache(…);
  ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPoint(ArtifactReference);
  bool CanPublish(ArtifactType, ArtifactReference); Task Publish…; Task<bool> Exists(…);
}
```

- Registry routing keyed by `ArtifactReferenceKind` instead of scheme strings → deletes the `br-mock` scheme and `OciArtifactMockedRegistry`. A mock becomes a `LocalModuleReference` (URI-anchored) produced by `ModuleAliasMockResolver` (provenance `ModuleAliasMock`), with display reference still `br:…` for diagnostics. Keep the `br-mock:` rejection diagnostic (`ModuleReferenceSchemeBrFsNotSupported`) as a parser rule so the text behavior is unchanged.
- Hover-only metadata (`GetDocumentationUri`, `TryGetModuleDescription(ModuleSymbol, …)`) moves off `IArtifactRegistry` into an `IArtifactMetadataProvider` used by hover (OCI impl reads the cached manifest; local impl reads the semantic model).

### 2.10.4 Phase 0 PRs

| # | PR | Details | Verification |
|---|---|---|---|
| 0.1 | **Characterization tests** | Table tests pinning today's parse/resolve results & diagnostics for: relative paths (incl. forbidden chars, `\`, absolute), `br:` (tags, digests, ports, uppercase), `br/public:`, `br/<alias>:` (missing, invalid name, null registry), `ts:` + `ts/<alias>:`, `br-mock:`, unknown scheme, empty, moduleAliasesMock (relative/absolute/inherited, extension → error), extension config map (builtin, bare path relative to config, `br:` value, unknown name, MS Graph retired). Restore-dedup test showing the `LocalModuleReference.Equals` bug (marked as expected-to-change in 0.5). LS definition/hover on a nested file with its own `bicepconfig.json`. | New tests green on `main`. |
| 0.2 | **Single source of truth for resolved references** | Remove `IArtifactReferenceFactory` from `Compilation`, `SemanticModel`, `ISymbolContext`/`SymbolContext`; `ImportedSymbol`/`WildcardImportSymbol.TryGetArtifactReference()` read `SourceFileGrouping.ArtifactLookup[EnclosingDeclaration].Reference`; LS `BicepDefinitionHandler`/`BicepHoverHandler` read the lookup instead of re-resolving with `EntryPoint`. Expose a helper `IArtifactFileLookup.TryGetArtifactReference(IArtifactReferenceSyntax)`. | Baselines unchanged; LS nested-file test from 0.1 now uses the correct config (only intended change – fix of P2). |
| 0.3 | **`ArtifactAddress` parser + `ArtifactReferenceResolver` + mock isolation** | Pure core parser (including the permanent `SchemeQualifiedAlias` form) replaces the split logic in `ModuleDispatcher.TryGetArtifactReference`. Scheme-qualified alias expansion moves out of `OciArtifactReference.TryParseComponents`/`TemplateSpecModuleReference.TryParse` into the resolver (these now only parse a fully qualified body, with a neutral `aliasContext` for diagnostics). The mock moves out of `OciArtifactRegistry` into `internal` `Artifacts/LegacyModuleAliasMocks/ModuleAliasMockResolver`, called once at step 3. Generalize declaring-URI annotation. Add the mock architecture test (2.11.3). The resolver also owns the extension-config-map path (`TryGetArtifactAddress`, `GetImplicitExtension`). Remove `TryParseArtifactReference` from `IArtifactRegistry`. | 0.1 tests unchanged (diagnostic text byte-identical); architecture test green. |
| 0.4 | **Split dispatcher → `IArtifactReferenceResolver` + `IArtifactDistributionManager`** | As in 2.10.2. DI in `BicepCoreServiceCollectionExtensions.cs:47`; Wasm provider; test helpers (`RegistryHelper`, `ServiceBuilder` extensions). Rename `ArtifactDispatcherTests` accordingly. Publish commands resolve via a publish-target path. | Full test suite; CLI publish integration tests. |
| 0.5 | **Local references** | `LocalModuleReference`/`LocalExtensionReference` hold a resolved `IOUri` (+ original `RelativePath` for display); `Equals` by URI. Local extension caching moves to `LocalArtifactRegistry` (content-addressed `local/sha256_…` unchanged). Delete `OciArtifactMockedReference`; `ModuleAliasMockResolver` returns a `LocalModuleReference`. Delete `OciArtifactMockedRegistry` and the `br-mock` scheme; registry routing by `ArtifactReferenceKind`. `SourceFileGroupingBuilder` `forceRestore` check uses `!IsExternal`/kind instead of `Scheme == ""`. | Mock tests unchanged; restore-dedup test flipped to correct behavior. |
| 0.6 | **Vocabulary + folders** | Move to `Bicep.Core/Artifacts/*` namespaces; mock config types to `Configuration/LegacyModuleAliasMocks/`; cache-entry renames; `IArtifactMetadataProvider` for hover metadata; split `ModuleReferenceCompletionProvider` into prefix parsing + catalog completion, with alias prefixes (`br/x:`, `ts/x:`, later `x::`) supplied by `IArtifactAliasCatalog`. Mostly mechanical; land last to minimize rebase pain. | Build + full tests; LS completion tests unchanged. |
| 0.7 | *(optional, time-boxed)* **Narrow ambient state** | Replace `IFeatureProvider` + `IBicepConfiguration` on references with a small `ArtifactRegistryContext(CloudConfiguration Cloud, IDirectoryHandle CacheRoot, bool OciEnabled)` captured at resolve time. Restore-failure key = (Cloud, Reference). Skip if it grows beyond a mechanical change. | Full tests. |

Dependencies: 0.1 → (0.2 ∥ 0.3) → 0.4 → 0.5 → 0.6 → 0.7.

Risks / mitigations:
- *Persisted strings*: `FullyQualifiedReference` for OCI stays `br:…`; cache paths (`~/.bicep/br/...`, `local/sha256_…`, TS layout) unchanged – add a test asserting cache directory paths.
- *LS performance*: 0.2 reduces work (no re-resolution per hover/definition).
- *Wasm/MCP*: both construct their own registry providers – include in 0.4/0.5 builds.
- *Scope creep*: 0.7 and the `Registry/Oci` move are explicitly optional.

---

## 2.11 Permanent vs. deletable reference formats

**Decision:** only `moduleAliasesMock` is deletable. Every source-reference syntax customers have written is supported forever, because a migration can't realistically be required. So:
- Permanent syntaxes are **first-class core forms**. They are not plugins or "legacy" code.
- Only the mock is isolated, so it can be removed without touching shared logic.

### 2.11.1 Classification

| Format / feature | Status |
|---|---|
| `br:<registry>/<repo>:<tag>` | **Permanent**. Persisted in source archives, ext-src URIs and the `~/.bicep/br` cache. Another spelling of `oci:`. |
| `ts:<sub>/<rg>/<name>:<ver>` | **Permanent**. Core scheme. |
| `br/public:` | **Permanent**. Predefined scheme-qualified alias. |
| `br/<alias>:` with `moduleAliases.br` | **Permanent**. Scheme-qualified alias. |
| `ts/<alias>:` with `moduleAliases.ts` | **Permanent**. Scheme-qualified alias. |
| `oci:`, `file:`, `<alias>::`, `artifacts.*` | **Permanent**. New in REP 0022. |
| Relative paths; `extensions` map values (with a scheme or a bare path) | **Permanent**. |
| `moduleAliasesMock` (`mapToFilePath`, absolute roots, `OciArtifactMockedReference`) | **Deletable**. Superseded by `artifacts.redirects`. |
| `br-mock:` scheme | Internal artifact, removed in Phase 0.5. |

Terminology for code and docs: avoid calling the permanent forms "legacy". The `Legacy` prefix is used **only** for the internal code location of the deletable mock (`LegacyModuleAliasMocks` folders and namespaces), never in user-facing text (config key, schema, diagnostics, docs); class names inside it are not prefixed (e.g. `LegacyModuleAliasMocks.ModuleAliasMockResolver`).
- **Scheme-qualified aliases** (`br/<alias>:`, `ts/<alias>:`, backed by `moduleAliases`).
- **Artifact aliases** (`<alias>::`, backed by `artifacts.aliases` and the predefined `mar`/`avm`).

The two namespaces stay independent, as the REP requires.

### 2.11.2 Core address model (both alias forms first-class)

```csharp
abstract record ArtifactAddress(string Text)
{
    record Relative(RelativePath Path);
    record Qualified(string Scheme, string Body);                       // br:, oci:, ts:, file:
    record SchemeQualifiedAlias(string Scheme, string Alias, string Body); // br/<alias>:, ts/<alias>:   (moduleAliases)
    record ArtifactAlias(string Alias, string Body);                    // <alias>::                  (artifacts.aliases)
}
```

- The parser recognizes `br/<alias>:` and `ts/<alias>:` directly. Other `<scheme>/<x>:` forms (`oci/…`, `file/…`) get the "scheme-qualified aliases are not supported for this scheme; use `<alias>::`" diagnostic.
- The resolver's alias-expansion step has two **separate** lookups, one per address form:
  - `SchemeQualifiedAlias` → `IBicepConfiguration.ModuleAliases`
  - `ArtifactAlias` → `IBicepConfiguration.Artifacts.Aliases` plus predefined aliases

  Each produces `(scheme, body, aliasContext)` for the core OCI/TS/File body parsers, so there is no fallback between the maps. The OCI and Template Spec reference types no longer read config (alias lookup moves out of `OciArtifactReference.TryParseComponents` and `TemplateSpecModuleReference.TryParse`).
- `aliasContext` feeds the existing "…after resolving alias "X" is not valid" diagnostics for both alias forms.
- Language server completion uses one core `IArtifactAliasCatalog` that lists both kinds of prefix (`br/x:`, `ts/x:`, `x::`) with their expanded locations. The registry/module/version catalog completion is shared.

### 2.11.3 Isolating `moduleAliasesMock`

Today the mock is spread through shared code:
- `OciArtifactRegistry.TryParseArtifactReference`: mock lookup and `OciArtifactMockedReference` creation inside the core OCI registry.
- `OciArtifactMockedReference`, `OciArtifactMockedRegistry`, and the `br-mock` scheme in `ArtifactReferenceSchemes`.
- `BicepConfigurationManager.BuildAliasDeclaringUriMap`: declaring-URI annotation hard-coded for `moduleAliasesMock.br`.

Target layout:
- **`Bicep.Core/Artifacts/LegacyModuleAliasMocks/ModuleAliasMockResolver`** (`internal`) owns all mock behavior. It takes a `SchemeQualifiedAlias` with scheme `br` and returns one of:
  - `null`: no mock configured for this alias
  - a failure: today's diagnostics, including "modules only"
  - a local file URI plus a display reference `br:<modulePath>`

  It keeps today's rules unchanged: absolute and relative roots, the `.bicep` suffix, tag/digest ignored, and paths anchored to the declaring config file.
- **One call site:** the resolver calls it at REP step 3, after source-spelling redirects and before alias expansion. A hit is terminal: it produces a `LocalModuleReference` with provenance `ModuleAliasMock`, and canonical redirects are skipped. The `artifact-redirect` rule ignores this provenance.
- **Config:** `ModuleAliasesMockConfiguration` and related types move to `Configuration/LegacyModuleAliasMocks/`. The generalized declaring-URI annotation (also used by `artifacts.aliases` and `artifacts.redirects`) registers `moduleAliasesMock.br` as one entry.
- **Diagnostics** stay in `DiagnosticBuilder` as they are.
- **Tests:** mock tests are grouped in `Bicep.Core.UnitTests/Artifacts/LegacyModuleAliasMocks/` and `Bicep.Core.IntegrationTests/LegacyModuleAliasMocks/`. This includes the existing `OciArtifactMockedReferenceTests` and the mock cases in `ConfigurationInheritanceTests`.
- **Guardrail:** an architecture test checks that only the resolver's step-3 call site, the config binding (`BicepConfiguration` and `BicepConfigurationExtensions`), and the mock folder reference `Bicep.Core.Artifacts.LegacyModuleAliasMocks` / `ModuleAliasesMock`.

Deletion checklist (to put in a code comment on `ModuleAliasMockResolver`):
- the `LegacyModuleAliasMocks` folders in product code and tests
- the step-3 call in the resolver
- the `ModuleAliasesMock` property and its bind/`With`/`WriteTo` code, plus its declaring-URI registration
- the `moduleAliasesMock` placeholder in built-in `bicepconfig.json` and in the schema
- now-unused diagnostic methods (BCP codes stay reserved)
- optionally, a "`moduleAliasesMock` is no longer supported; use `artifacts.redirects`" config diagnostic

Deprecation itself (warnings, a timeline) is a product decision outside REP 0022, which requires mocks to keep working without new warnings.

---

## 3. Work breakdown (PR sequence)

> After Phase 0, feature PR 1 below shrinks to "add `Alias` address form and canonical identity/provenance plumbing"; the resolver, parser, URI-anchored local references and kind-based routing already exist.

Each PR is independently shippable; user-visible syntax lands only when its resolution path is complete. If releases are cut mid-sequence, gate new syntax behind a temporary (hidden) experimental flag removed in PR 10 – the REP says no flag at release (decision needed).

| # | PR | Main files | Tests |
|---|---|---|---|
| 1 | **Canonical identity + provenance plumbing** (on top of Phase 0). Add `ArtifactReference.CanonicalIdentity`, `ArtifactProvenance`, and the `Alias` address form (parsed but rejected until PR 4). | `Artifacts/References/*`, `Artifacts/Resolution/*` | Parser tests for `oci:localhost:5000/x:v1`, `br::x`, `storage:::1.0`, `oci/mar:x` error; baselines unchanged. |
| 2 | **`oci:` scheme.** Register as parse alias of OCI registry; publish target support; Wasm/MCP filters. | `ModuleReferenceSchemes.cs`, `DefaultArtifactRegistryProvider.cs`, `PublishExtensionCommand.cs`, `Interop.cs` | `oci:` ≡ `br:` equality/restore-dedup; publish to `oci:`; source-archive still writes `br:`. |
| 3 | **`artifacts` config section** (aliases + redirects data model, non-throwing validation, declaring-URI annotation across `extends`, code-defined `mar`/`avm`). | `ArtifactsConfiguration.cs`, `BicepConfiguration*.cs`, `BicepConfigurationManager.cs`, `IBicepConfiguration.cs`, schema | Binding, inheritance (alias/redirect declared in base config resolves from base dir), predefined override type error, merge-replacement semantics. |
| 4 | **Alias references `<alias>::`** for OCI and TS (incl. `mar`, `avm`, empty-path OCI form). Core-only; independent of `moduleAliases` by construction (separate address form and lookup). | core resolver/parser, `ArtifactsConfiguration.cs`, `DiagnosticBuilder.cs` | Expansion matrix; `br/x:` with only `artifacts.aliases.x` → existing unknown-alias error; same name in both maps with different targets; architecture test still green. |
| 5 | **`file:` scheme + file aliases** for modules, `using`/`extends`/imports, and extensions (direct + `extensions` map). `FileArtifactRegistry`, shared local-extension cache helper; `SourceFileGroupingBuilder` uses `!IsExternal` instead of `Scheme == Local`. | `FileArtifactReference.cs`, `FileArtifactRegistry.cs`, `LocalModuleRegistry.cs`, `SourceFileGroupingBuilder.cs`, `ModuleDispatcher.TryGetArtifactAddress` | Monorepo scenario from REP (nested solutions); transitive references use each file’s own config; cycles through file aliases; invalid/unsafe `.tgz`. |
| 6 | **Data-file loads** via `file:`/file aliases. | `SystemNamespaceType.cs`, `BicepSourceFile.cs`, `BicepSourceFileFactory` | load* with aliases; size/encoding limits unchanged; LS aux-file invalidation. |
| 7 | **Redirect engine** (exact + capture, 7-step precedence, ambiguity, target validation) applied to modules, extensions, TS, data files; publish targets (`TryResolvePublishTarget`) and ext-src requests skip redirects. | `ArtifactRedirectTable.cs`, resolver, CLI publish commands, `BicepExternalSourceRequestHandler.cs` | Precedence matrix incl. mock-vs-redirect; tags/digests/nested paths; `{...path}`; capture `..` rejection; missing target; Windows/Linux path normalization; publish ignores redirects. |
| 8 | **`artifact-redirect` rule + `--verbose` CLI disclosure**; `UseRecentModuleVersionsRule` adjustments. | `ArtifactRedirectRule.cs`, CLI args/commands, schema | Rule levels (error blocks emit), suppression, once-per-artifact; verbose output still shown when rule is `off`; mock produces no diagnostic. |
| 9 | **Language server UX**: completions, hover, definition, document links for new forms. | `ModuleReferenceCompletionProvider.cs`, `BicepCompletionProvider.cs`, `BicepHoverHandler.cs`, `BicepDefinitionHandler.cs` | LangServer unit + integration tests. |
| 10 | **Docs, samples, cleanup**: remove temp flag (if used), baseline dataset, Learn docs PR, linter-rule doc, changelog. | `src/Bicep.Core.Samples/Files/baselines/ArtifactAliases_*`, docs | Baseline (`.diagnostics`, `.symbols`, `main.json`) for alias/redirect/file scenarios. |

PRs 2, 3 can proceed in parallel after 1; 4 depends on 3; 5 depends on 1 (+3 for file aliases); 6 depends on 5; 7 depends on 3–6; 8 on 7; 9 on 4–7.

---

## 4. Diagnostics to add (`DiagnosticBuilder`, next free BCP codes – BCP459 is currently the highest)

- Unknown artifact alias `<alias>`.
- Invalid alias definition (missing/unknown `type`, missing required property per type).
- Predefined alias `mar`/`avm` must have type `oci`.
- Scheme-qualified alias form (`oci/x:`) not supported.
- Empty artifact path not allowed for alias type `<type>` / OCI empty-path requires a full repository.
- Module `file:` reference must end in `.bicep`/`.json`.
- Artifact reference type not supported for data-file loading.
- Invalid redirect key (capture syntax, duplicate/invalid capture names, multiple `{...}`, glob chars).
- Invalid redirect target (not `./`/`../`, absolute, artifact reference, undeclared placeholder, captured `.`/`..`).
- Ambiguous redirect (equally specific patterns).
- Redirect target not found / is a directory.
- Linter: `artifact-redirect`.

---

## 5. Backward-compatibility checklist (must have explicit tests)

- `br/public:`, `br:`, `br/<alias>:`, `ts/<alias>:` unchanged; no new warnings.
- `moduleAliases` / `moduleAliasesMock` unchanged incl. absolute `mapToFilePath`, digest refs, inherited relative roots, existing diagnostics.
- `artifacts.aliases` never used for `br/`/`ts/` forms and vice-versa.
- Persisted `br:` strings (source archives, ext-src URIs, cache layout) unchanged.
- All existing baselines byte-identical after PRs 1–7.

---

## 6. Open questions / decisions to confirm

1. **Temporary experimental flag** during the multi-PR rollout vs. REP’s "no flag" (recommend hidden flag, removed in final PR).
2. **`--verbose` CLI option** doesn’t exist – add a new global option or use log level (`--log-level`)? 
3. **`file:` values in the `extensions` config map** – relative to the declaring `bicepconfig.json` (consistent with today’s bare-path behavior) rather than the referencing `.bicep` file? REP says `file:` is relative to "the file containing the reference"; recommend treating the config file as that file. Also: today’s bare-path extension entries use the *leaf* config URI, not the declaring layer – fix alongside?
4. **Absolute `file:` paths** (`file:C:/x.bicep`, `file:/x.bicep`) – reject (consistent with relative-path rules)? REP implies relative only.
5. **Ambiguous redirect detection** – resolution-time error vs. static config validation.
6. **Rule granularity** – "once per redirected artifact per compilation" vs. per-occurrence `#disable-next-line`.
7. **Redirecting remote identities for data loads** (e.g. `oci:` key used in `loadTextContent`) – out of scope / error?
8. **Alias names equal to scheme names** (`br::`, `oci::`, `file::`) – allowed (unambiguous) or reserved?
9. **Diagnostic display** – show source spelling vs. `br:` FQ reference in existing restore-failure messages for `oci:`/alias references.
10. **REP wording** – REP 0022 has a "Future deprecation of `moduleAliases`" section; consider updating it to state that `br/<alias>:`, `ts/<alias>:` and `br/public:` are supported permanently and only `moduleAliasesMock` may be deprecated.
