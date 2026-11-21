# Bicep configuration: implementation plan

Scope: everything under `src/Bicep.Core/Configuration`, plus the code that loads, caches and reads `bicepconfig.json`: the language server's config lifecycle, `BicepSourceFile.LoadConfiguration`, `FeatureProvider`, linter rules, and artifact and extension resolution.

The work is 12 PRs, plus one in a later release. Anything the [source graph redesign](https://github.com/Azure/bicep/blob/shenglol/source-graph-redesign/docs/design/source-graph-redesign.md) covers is left to it (see the end of section 1):

- **Fixes (PR 1)**: crashes and cache invalidation. Merged (#20561).
- **Loading (PRs 2–4)**: a loader pipeline that records which file set each value, so errors and relative paths use that file; the cache root moves to an environment variable.
- **Cache freshness (PRs 13–14)**: caches record the files they read and check them themselves, so every host (language server, `bicep jsonrpc`, MCP server) sees file changes; hosts start a new file change epoch when files may have changed, and caches check their entries once per epoch.
- **Models and validation (PRs 5–9)**: plain records, typed sections, warnings for unknown and ignored settings, per-section fallback.
- **Access**: one configuration snapshot per file (PR 10) and the explicit config path passed in instead of ambient state (PR 11) are both handed off to the source graph redesign.

PRs that change user-visible behavior are marked **(behavior change)**. Everything else keeps behavior.

---

## 1. Status

| PR | Summary | Files (product + test) | After | Status |
|---|---|---|---|---|
| 1 | Config errors become diagnostics; correct cache invalidation | ~9 + ~11 | – | Merged (#20561) |
| 2 | Loader pipeline: parse cache, one merge that records the declaring file, per-file diagnostics | ~13 + ~7 | 1 | Merged (#20582) |
| 3 | The declaring file is used everywhere: `bicep.version`, aliases, mocks, relative paths | ~12 + ~8 | 2 | Planned |
| 4 | `BICEP_ARTIFACT_CACHE_DIR`; `cacheRootDirectory` deprecated | ~5 + ~5 | 2 | Planned |
| 5 | Sections and `BicepConfiguration` become records; one source of defaults | ~18 + ~6 | 3 | Planned |
| 6 | Typed analyzer settings, with warnings for unknown rules and ignored values | ~9 + ~10 | 5 | Planned |
| 7 | Experimental features as a set; warnings for unknown settings and `null` | ~12 + ~8 | 5 | Planned |
| 8 | Typed extension sources and cloud profiles | ~13 + ~7 | 5 | Planned |
| 9 | Per-section fallback | ~5 + ~3 | 5 | Planned |
| 10 | One configuration snapshot per file | – | – | Handed off to the source graph redesign |
| 11 | Explicit config path passed in; `ExplicitConfigurationContext` removed | – | – | Handed off to the source graph redesign |
| 12 | *(later release)* Remove the `cacheRootDirectory` property | ~3 + ~3 | 4 | Later |
| 13 | Configuration cache checks its own dependencies once per file change epoch | ~12 + ~10 | 2 | In progress |
| 14 | Auxiliary file cache (`load*()` files) uses the same mechanism | ~5 + ~4 | 13 | Waiting on the source graph redesign |

File counts are estimates from today's call sites. PR 5 is larger, but most of its edits are the same change repeated (noted in the PR). PRs 13 and 14 were added after PR 1 showed that the CLI JSON-RPC and MCP servers never see file changes (B6).

```
1 -> 2 ─┬─> 3 -> 5 ─┬─> 6
        │           ├─> 7
        │           ├─> 8
        │           └─> 9
        ├─> 4 ··· (one release later) ··· 12
        └─> 13 -> 14
```

After PR 2, three tracks can run side by side: PRs 3, 5 and 6–9 in order, PR 4, and PRs 13–14. PRs 6, 7, 8 and 9 only depend on PR 5.

**Related work: the [source graph redesign](https://github.com/Azure/bicep/blob/shenglol/source-graph-redesign/docs/design/source-graph-redesign.md)** (draft, not scheduled). What the redesign does as part of its own design is left to it; this plan doesn't build an interim version.

Handed off to the redesign:

- **PR 10, one configuration snapshot per file.** The redesign's `SourceNode` captures `Configuration` and `Features` when the node is built, replacing `BicepSourceFile.LoadConfiguration()` and `LoadFeatures()`. Its immutable nodes are recreated on configuration changes, which removes the reason a snapshot couldn't live with the file (3.6). Until then, the call sites keep querying the manager; in the language server a purge in the middle of a compile can still give one compilation two configs, as it can today.
- **PR 11, the explicit config path passed in.** `ExplicitConfigurationContext` is read by the singleton configuration manager, which is injected into every service on the compile path (`SourceFileFactory`, `FeatureProviderFactory`, `ArtifactReferenceResolver`, `RemoteArtifactContextFactory`, `ModuleAliasMockRule`, local-deploy extensions). Passing the path in today means either threading it through all of them or building a service provider per config path. In the redesign, the explicit config file is a graph-build input: nodes capture it when they're built, and the builder passes each node's configuration to the services it calls (redesign step 2).
- **How the language server reacts to configuration changes.** In the redesign, configuration files are graph nodes and each source node records its configuration inputs (chain and searched lookup paths), so after redesign step 6 the language server pushes watched-file URIs through `SourceGraph.WithChangedFiles` and rebuilds only the affected nodes: no epoch and no instance comparison. `bicep jsonrpc` and the MCP server have no watcher, so they keep pulling, through `SourceGraph.Revalidate()` with PR 13's `FileDependencies`. PR 13 refreshes the affected files in `BicepCompilationManager` until then.
- **A shared "diagnostics by file" type.** The redesign's `FileDiagnostics` replaces `GetAllDiagnosticsByBicepFile()`'s dictionary; the configuration chain follows it later (see the follow-ups).

Coordinate with the redesign:

| PR | What to agree on |
|---|---|
| 8 | Redesign step 4 takes built-in implicit extensions out of the graph, so `NamespaceProvider` reads them from configuration. Both edit `SourceFileGroupingBuilder` and `NamespaceProvider`; `ExtensionSource.BuiltIn` is the check step 4 needs. Whichever lands second adapts. |
| 13 | How a configuration change reaches the graph. PR 13 has no change event: after starting a new `FileChangeEpoch`, the language server asks the configuration manager again for each file, and a different instance means the configuration changed (3.2). The redesign replaces that with push (`WithChangedFiles(uris)` over recorded configuration inputs) in the language server, and keeps pull (`Revalidate()` with `FileDependencies`) for hosts without a watcher. |
| 14 | Waits on the redesign's open question about data-file loads. If they become graph edges, their freshness is the redesign's and PR 14 is dropped; otherwise it goes ahead as written. |
---

## 2. Problems

Each item was confirmed with the Debug CLI built from `main`, or by reading the code where noted.

### 2.1 Bugs

| # | Problem | Evidence | Fixed in |
|---|---|---|---|
| B1 | Invalid values crash the compiler instead of producing a diagnostic (regression) | `{"experimentalFeaturesWarning": "yes"}`, `{"bicep": "1.0"}` and `{"extends": 5}` each make `bicep build` exit with an unhandled exception. `BuildChain` only catches `ConfigurationException`; before `d6d06cb66`, `ConfigurationManager` also caught `JsonException` and `Exception`. | PR 1 |
| B2 | Relative `extensions` and documentation paths resolve against the wrong file | (a) Inherited: a base config with `"extensions": {"foo": "./ext/foo.tgz"}` resolves the path from `child\ext\foo.tgz`. The same is true for documentation paths (`DocsGenerationOptionsResolver`). Only `moduleAliasesMock` uses the file that declared the value. (b) Implicit extensions, even with a single config: with `"implicitExtensions": ["foo"]`, `apps\web\main.bicep` reports BCP091 for `apps\web\ext\foo.tgz`. `SourceFileGroupingBuilder.GetImplicitExtension` passes the raw `extensions` value to the resolver, which resolves it against the referencing file; only `extension foo` goes through `ArtifactReferenceText`, which uses the config file. | PR 3 |
| B3 | Errors are blamed on the wrong file | An invalid `currentProfile` in a base config reports BCP273 against the child's `bicepconfig.json`. Module alias errors (`ModuleAliasesConfiguration.configFileUri`) also always name the leaf. | PRs 2, 3 |
| B4 | Per-file config diagnostics are always empty (code reading) | `BicepConfiguration.Bind` passes `diagnostics: null`, so every layer has none. `EnumerateDiagnosticsPerFile` only ever returns empty arrays. The doc comments on `IBicepConfiguration.GetDiagnostics` and `BicepConfigurationChain.GetEffectiveConfiguration` describe aggregation that doesn't happen. | PR 2 |
| B5 | A failed chain stays cached after the file is fixed (code reading) | `LoadChain` records a chain's files only when loading succeeds. After a parse error or a missing base file, `PurgeCacheForAffectedChains(base)` finds nothing to purge. In the language server this is hidden because the change event purges everything. | PR 1 |
| B6 | Long-running hosts other than the language server never see file changes (code reading) | `bicep jsonrpc` and the MCP server use the same singleton `BicepConfigurationManager` and `AuxiliaryFileCache`, and nothing in `Bicep.Cli` or `Bicep.McpServer*` ever invalidates or trims them. An edited `bicepconfig.json`, or a file read by a `load*()` function, isn't picked up until the process restarts, and `AuxiliaryFileCache` grows without limit. | PRs 13, 14 |
| B7 | Config changes the language server missed | The watched-files handler only reacted to files named exactly `bicepconfig.json`, so editing an extended base config such as `bicepconfig.base.json` never refreshed anything. Clients may also report only the folder when a folder is deleted, which exact URI matching misses. | PR 1 |

### 2.2 Silent misconfiguration

| Input | Result | Fixed in |
|---|---|---|
| `"experimentalFeaturesEnabled": {"symbolicNameCodeGen": true}` (wrong casing) | Ignored | PR 7 |
| A misspelled top-level key such as `"analyzerz"` | Ignored | PR 7 |
| A rule with `"level": "eror"` | The rule's default level is used | PR 6 |
| A rule with `"maxAllowedAgeInDays": "abc"` | The default of 730 is used | PR 6 |
| A child with `"bicep": {"version": null}` | Silently ignored; the base's version still applies | PR 7 |

### 2.3 Design debt

- **Loader**
  - `LoadChain` and `BuildChain` read files, follow `extends`, merge, bind, check trust and record where each value came from, all at once.
  - Each step reports errors differently: some return diagnostics, some throw, some swallow the error.
  - A shared base config is re-parsed for every leaf that extends it. (PR 13 keeps this: the cost is measured as marginal; see 3.2.)
  - `JsonElement.Merge` re-serializes the whole document for every layer.
  - Every layer is also merged with the built-in config and bound again, and the result is thrown away (B4).
- **"Which file set this?"** is handled three different ways: a per-alias `DeclaringConfigUri` for mocks, `CompilerConfiguration.WithDeclaringUri`, and the leaf URI for everything else.
- **Caches**
  - `PurgeCacheForAffectedChains` clears the entire file lookup cache.
  - `PurgeChainCache`, `PurgeCache` and `PurgeLookupCache` aren't used in product code.
  - `directoryHandleCache` grows without limit and caches a trivial `GetParent()`.
  - `HandleBicepConfigChangeEvent` purges everything and then does a targeted purge.
- **Models**
  - `ConfigurationSection<T>` plus nine `IBicep*Configuration` interfaces that each have exactly one implementation. Code keeps casting them back to the concrete classes: `ToUtf8Json`, `ModuleReferenceCompletionProvider`, `BuildChain` and the test helpers.
  - `BicepConfiguration` has a 14-parameter constructor plus a `With(...)` with 14 nullable parameters, so a value can't be set to `null`.
  - Only `Cloud` uses source-generated JSON; the other sections use reflection with trimming warnings (IL2026) suppressed.
  - `AnalyzersConfiguration` keeps the raw `JsonElement` and is looked up by path string on every call.
  - `ExperimentalFeaturesEnabled` is a record with 16 positional booleans, so adding a feature means editing four places.
  - `ExtensionConfigEntry` is a plain string checked against `"builtin:"`.
- **Defaults** are kept in four places: the built-in `bicepconfig.json`, C# property initializers (documentation example sources are written out in both), fallbacks hand-written in `Bind`, and the VS Code schema.
- **How code gets configuration**
  - `BicepSourceFile.LoadConfiguration()` goes through the manager on every call (about 10 call sites). In the language server, a purge in the middle of a compile can give one compilation two different configs.
  - `ExplicitConfigurationContext` is ambient `AsyncLocal` state.
- **Error messages**: BCP273 says "Failed to parse the contents" even when the JSON parsed but a value is invalid. The language server publishes every config diagnostic at `Range(0,0,0,0)`.

---

## 3. Target architecture

### 3.1 Loader pipeline

```
source file URI
  -> (1) Locate      explicit --config file, or nearest bicepconfig.json     -> leaf IOUri | none
  -> (2) Read        per file, cached by URI                                -> ConfigurationFile | diagnostic
  -> (3) Chain       follow "extends" (relative only, no cycles, depth <= 64) -> ImmutableArray<ConfigurationFile>
  -> (4) Merge       built-in + layers, recording where each value came from  -> MergedConfiguration
  -> (5) Bind        typed records, values checked, paths made absolute      -> BicepConfiguration + diagnostics
  -> (6) Policy      cloud trust and per-section fallback                    -> BicepConfigurationChain
```

Rules:

- **Every step returns a value or diagnostics. No step throws for user input.** An exception that reaches the manager is a Bicep bug. The manager reports it as a BCP272 diagnostic as a last resort, and a unit test enforces that no known kind of bad input gets that far.
- **A diagnostic names the file, and the property, that caused it.** Each config diagnostic is stored with the file it belongs to. A binding error carries the failing property as a `ConfigurationPropertyPath` (the list of property names), which is used to find the file that declared it; messages show the property as a dotted path such as `bicep.version`.
- **Steps don't share mutable state.** The manager only coordinates them and owns the caches.

Types:

| Type | Role |
|---|---|
| `ConfigurationFile` | `Uri` and the parsed `JsonElement` root (always an object), plus the raw `extends` value. Immutable. |
| `ConfigurationFileReader` | Reads and parses one file into `ResultWithDiagnostic<ConfigurationFile>`. Comments are allowed and trailing commas aren't, as today. Never throws. |
| `ConfigurationChainResolver` | Follows `extends` from a leaf, reading through a callback so the manager owns caching and dependency tracking. Owns the existence, cycle, depth, absolute-path and type checks. Reports a cycle in chain order. |
| `ConfigurationMerger` | Deep-merges the built-in config and the layers in one pass over `JsonNode`, producing `MergedConfiguration(JsonElement Root, ConfigurationProvenance Provenance)`. |
| `ConfigurationProvenance` | Maps a `ConfigurationPropertyPath` to the URI of the file that declared it (`null` means built-in). Lookup uses the longest matching prefix. Replaces the three existing mechanisms. |
| `ConfigurationBinder` *(PR 5)* | `MergedConfiguration` -> `BicepConfiguration` plus diagnostics. Each section has a binder; each binder gets the provenance so it can resolve paths and report against the right file. Until then, `BicepConfiguration.Bind` throws `ConfigurationException` with the failing property's path and the manager turns it into a diagnostic. |
| `ConfigurationDiagnostic` | `(IOUri? File, IDiagnostic Diagnostic)`: a diagnostic and the file it belongs to. `File` is `null` when no single file caused it, such as an invalid `BICEP_TRUSTED_CLOUDS` value. |

### 3.2 Caches and freshness

| Cache | Key | Value |
|---|---|---|
| Located leaf | directory | leaf `IOUri?` |
| Chain | leaf `IOUri` | `BicepConfigurationChain` and its dependencies (one entry, so they can't get out of step); the dependencies become `FileDependencies` in PR 13 |

PR 2 also cached parsed files, so a base shared by several leaves was parsed once. PR 13 removes that cache: chains are cached, so a file is only reread when a chain is loaded, and a Release measurement with 200 leaves sharing a 40-rule base showed the extra reads cost about 0.27 ms each against about 1.3 ms per chain load (mostly merging and binding), once per leaf. It didn't justify a third cache with its own versioning.

**After PR 1 (interim).** The configuration manager records which files each chain read, including failed chains (fixes B5). The language server forwards every watched-file event to `InvalidateFile(uri)`, which drops each chain that read the file or anything under it (a folder event covers its contents). An event on any `bicepconfig.json` also resets discovery, because clients don't reliably say whether a file was created or changed. This works for the language server only; other hosts never call it (B6).

**Target (PRs 13–14).** Each cache owns its freshness. Callers don't tell it what to invalidate; hosts only say when files may have changed.

1. **Recorded dependencies.** A shared `FileDependencies` type in `Bicep.IO` records, for one cache entry, every file it read with a version stamp (last-write time and size), and every path it checked that didn't exist. For configuration, that's each file in the chain plus every directory searched for a `bicepconfig.json` without finding one. That second list is what catches a config file created closer to the source file. A lookup only records whether each `bicepconfig.json` exists, so editing the leaf reloads its chain but doesn't search for it again.
2. **Version checks.** `FileDependencies.IsCurrent()` compares the recorded stamps with the file system: a file changed or was deleted, or a missing path now exists. `IFileHandle` gets a version API for this; the in-memory file system returns a counter that increases on every write.
3. **Checks are pulled, once per epoch.** `FileChangeEpoch` in `Bicep.IO` marks periods during which files are assumed not to change. Each cache entry records the epoch in which it was last found current, and is checked again the first time it's read in a later epoch; a stale entry is reloaded. Nothing is pushed: caches don't subscribe to file events and don't raise events of their own.
4. **Hosts decide when an epoch starts.**
   - **Language server:** for each batch of `didChangeWatchedFiles` events, and when a config file is saved (clients may report the save before the watched-file event). The compilation manager then asks the configuration manager again for every Bicep file in its open compilations. The manager returns the same configuration instance until it's reloaded, so a different instance from the one the file's semantic model captured means that file's configuration changed. Those files are refreshed like modified files, which also rebuilds their dependents. The language server never recompiles for events that didn't change any configuration (some watchers report a folder as "changed" when files inside it change), and it refreshes modules whose configuration changed, not only entry points.
   - **`bicep jsonrpc` and the MCP server:** at the start of each request (each JSON-RPC handler, and the MCP call-tool filter), because they don't watch files. Each entry is checked once per request: a few file-status calls each.
   - **One-shot CLI commands** never start one, so they make no checks; nothing they read changes during the command.
   - **No epoch at all** (library use through `BicepCompiler.Create`, the playground, tests that don't need freshness): entries are checked when they're loaded and then trusted, like a one-shot command and like `main`. Checking on every read instead is correct but slow: `BicepSourceFile.LoadConfiguration()` runs many times per file per compile, and that mode made a 401-file, 7-folder-deep `bicep build` 15% slower (median 6.1 s to 7.1 s). With an epoch it's unchanged (6,037 ms vs 6,041 ms). Once the source graph redesign captures configuration per node, every mode is about one check per file per graph build.
5. **Edge cases.**
   - **Symbolic links.** A link's own timestamp and size don't change when its target is edited, but reading the link reads the target, so `TryGetVersion()` resolves the final target and versions that.
   - **Failed reads.** A file that couldn't be read (for example, locked by an editor or a virus scanner while it saves) is recorded as a failed read, which is never current. A lock doesn't change the file's version, so without this the error would stay cached until the next edit. The chain is loaded again in the next epoch.
   - **Concurrent loads.** When two callers load the same entry at the same time, only one result is cached, and both return it. Callers compare instances, so returning two would trigger a spurious rebuild.

With this, `InvalidateFile`, `InvalidateAll` and the invalidation part of `BicepConfigLifecycleManager` go away; the lifecycle manager only publishes diagnostics for open config files. Nothing in product code called `InvalidateAll`, and `BicepCompilationManager.RefreshAllActiveCompilations` loses its only caller.

An earlier draft pushed instead: a notifier published changed URIs, caches matched them against their entries, and the configuration manager raised a `ConfigurationChanged` event listing what it dropped. That needed URI matching with folder rules, two kinds of report (specific files from the language server, "anything may have changed" from JSON-RPC and MCP), a race check for loads that overlapped a report, and an event payload that only tests read. Pulling needs none of it.

```mermaid
flowchart LR
    subgraph Hosts
        LSP["Language server<br/>(per watched-file batch, on config save)"]
        RPC["bicep jsonrpc / MCP<br/>(per request)"]
    end

    E["FileChangeEpoch"]

    subgraph Core["Bicep.Core / Bicep.IO"]
        BCM["BicepConfigurationManager"]
        AUX["AuxiliaryFileCache (PR 14)"]
        FD["FileDependencies<br/>(files read + version stamps,<br/>paths checked for existence)"]
    end

    FS[("File system")]

    LSP -- "StartNew()" --> E
    RPC -- "StartNew()" --> E
    BCM -- "reads Current" --> E
    AUX -- "reads Current" --> E
    BCM -- "one per entry" --> FD
    AUX -- "one per entry" --> FD
    FD -- "IsCurrent(): file-status checks,<br/>once per entry per epoch" --> FS
    LSP -- "same configuration instance?" --> BCM
```

How each host sees an edit to a base config:

```mermaid
sequenceDiagram
    autonumber
    participant Client as Editor / caller
    participant Host as Host
    participant BCM as BicepConfigurationManager
    participant FS as File system

    rect rgb(235, 245, 255)
    Note over Client,FS: Language server
    Client->>Host: didChangeWatchedFiles(base/bicepconfig.base.json)
    Host->>Host: FileChangeEpoch.StartNew()
    loop each Bicep file in the open compilations
        Host->>BCM: GetConfigurationChain(file)
        BCM->>FS: IsCurrent(), once per entry in this epoch
        BCM-->>Host: same instance, or a reloaded one
    end
    Host->>Host: refresh files whose configuration instance changed, and their dependents
    end

    rect rgb(245, 245, 235)
    Note over Client,FS: bicep jsonrpc / MCP
    Client->>Host: compile request
    Host->>Host: FileChangeEpoch.StartNew()
    Host->>BCM: GetConfigurationChain(main.bicep)
    BCM->>FS: IsCurrent(), once per entry in this epoch
    alt something changed
        BCM->>BCM: reload chain
    end
    BCM-->>Host: current chain
    end
```
**Why not a file watcher for `bicep jsonrpc` and the MCP server?** The interface allows one later, but it isn't needed now:

- These servers have no workspace root. Each request brings arbitrary paths, so a watcher implementation would have to start and stop directory watchers for every file read, count references, and dispose of them when cache entries are dropped.
- `FileSystemWatcher` is least reliable in the places that matter:
  - Editors often save by writing a temp file and renaming it, so a save shows up as a rename.
  - Its buffer can overflow and drop events; the only signal is its `Error` event, which then has to clear everything.
  - Linux limits how many watches a user can have.
  - Network shares, WSL and container mounts are unreliable.
- These servers compile once per request, so checking a few file versions per request costs microseconds and is deterministic.

### 3.3 Merge rules

- Objects merge deeply, property by property; the more specific (child) file wins.
- Arrays and plain values are replaced wholesale.
- **`null` is not allowed anywhere.** A `null` value gives an "ignored setting" warning (3.4) that names the file and property, and the value is ignored, so the inherited value stays. This is what already happens today, minus the warning, and it matches the schema, which allows `null` nowhere. A child changes an inherited value by setting a different one; nothing can be cleared or removed.
- `extends` is removed before merging, and `$schema` is ignored.
- These rules are documented on `ConfigurationMerger` and covered by tests. User-facing documentation lives in the separate docs repository.

### 3.4 Diagnostics

One code per category, with the message saying what's wrong and where:

| Category | Level | Code | Covers | Result |
|---|---|---|---|---|
| Invalid configuration | Error | BCP273 (reworded) | A value that prevents a section from binding: wrong type for a section, invalid cloud profile, invalid `bicep.version` range, invalid docs path | The section falls back to the built-in default (PR 9) |
| Unknown setting | Warning | new | An unknown top-level property, experimental feature name, analyzer namespace or rule name | Ignored |
| Ignored setting | Warning | new | `null`, an invalid rule `level`, a rule option of the wrong JSON type | Ignored; the inherited or default value stays |
| Deprecated setting | Warning | new | `cacheRootDirectory`, and experimental features that have graduated or been removed | Still honored until removed (a removed feature does nothing) |
| Chain errors | Error | BCP271, BCP272, BCP453–BCP455 (existing) | Unreadable or unparsable file, absolute `extends`, cycle, depth; plus a non-string `extends` (new message under BCP453) | The whole built-in config is used |

BCP273's new wording applies to values as well as JSON parsing: `"The Bicep configuration file "{uri}" is invalid: {message}."`

Every message includes the JSON path (for example `analyzers.core.rules.no-unused-params.level`) so the setting is easy to find. Warnings can fail CI pipelines that treat warnings as errors, so the release notes list the new warnings.

### 3.5 Models

```csharp
public sealed record BicepConfiguration(
    CloudConfiguration Cloud,
    ModuleAliasesConfiguration ModuleAliases,
    ModuleAliasesMockConfiguration ModuleAliasesMock,
    ExtensionsConfiguration Extensions,
    ImmutableArray<string> ImplicitExtensions,
    AnalyzersConfiguration Analyzers,
    FormattingConfiguration Formatting,
    DocumentationConfiguration Documentation,
    CompilerConfiguration Compiler,
    ExperimentalFeatures ExperimentalFeatures,
    string? CacheRootDirectory,          // deprecated (PR 4), removed in PR 12
    bool ExperimentalFeaturesWarning,
    IOUri? ConfigFileUri,
    ImmutableArray<IDiagnostic> Diagnostics);
```

- Every section is a `sealed record` that holds data plus the methods that belong with it (alias lookups, `TryGetExtensionSource`). `ConfigurationSection<T>` and `IWritableConfigurationSection` are deleted. Tests change settings with `with`.
- Sections keep their current class names, so callers that already use a concrete class don't change.
- Interfaces: the section interfaces with few users are deleted. `IBicepCloudConfiguration` (51 files) and `IBicepConfiguration` (58 files) stay. `CloudConfiguration` and `BicepConfiguration` implement them, and the record properties use the concrete types, so the casts go away without touching every caller. Removing those two interfaces would be a separate rename-only PR, only worth doing if they cause confusion.
- `FormattingConfiguration` keeps exposing `PrettyPrinterV2Options`: about 19 files read it, and a separate config-owned type would only add a mapping layer.
- Paths are resolved when binding, so a record holds absolute `IOUri`s, never strings relative to some config file.
- Serialization to JSON (`ToUtf8Json`, which only tests use: `BicepConfigurationAssertions`, `BicepConfigSchemaTests`) goes through a source-generated `BicepConfigurationJsonContext`.
- Typed sections:
  - `AnalyzersConfiguration`: `Enabled`, `Verbose`, and `ImmutableDictionary<string, RuleConfiguration>`, where `RuleConfiguration(DiagnosticLevel? Level, JsonElement Options)` (PR 6).
  - `ExperimentalFeatures`: a set of enabled feature names, checked against `FeatureProvider` metadata, with typed properties for the features in use (PR 7).
  - `ExtensionsConfiguration`: `ImmutableDictionary<string, ExtensionSource>`, where `ExtensionSource` is `BuiltIn`, `LocalFile(IOUri)` or `Reference(string)` (`br:`, `ts:`, ...) (PR 8).
  - `CloudConfiguration`: profiles parsed to `Uri`, equality over every field (PR 8).

### 3.6 How code gets configuration

- **One configuration per file is handed off to the source graph redesign** (section 1). Today `BicepSourceFile.LoadConfiguration()` asks the manager on every call (about 10 call sites). A snapshot can't simply be stored on `BicepSourceFile`, because the language server reuses source file instances across config changes (`RefreshAllActiveCompilations` and the copy constructor keep the same file). The redesign's immutable `SourceNode` captures it when the node is built and is recreated when the configuration changes.
- **Replacing `ExplicitConfigurationContext` with an explicit config path is handed off to the source graph redesign** (section 1, PR 11): the explicit config file becomes a graph-build input, and nodes capture it.

---

## 4. PRs

Verify with `dotnet test --solution Bicep.slnx --no-build` (full suite). To update baselines, run `-- --filter "TestCategory=Baseline" --test-parameter SetBaseLine=true`, then run the tests again, and review every baseline diff.

### PR 1 – Config errors become diagnostics; correct cache invalidation **(behavior change: crashes become errors)**

Fixes B1, B5 and B7. Two independent fixes in the same files (`BicepConfigurationManager` and its tests), so they're reviewed together.

Errors:

1. `BuildChain`: wrap binding of the merged config in `catch (ConfigurationException)`, `catch (JsonException)` and `catch (InvalidOperationException)`, all mapped to BCP273. Add a last-resort `catch (Exception)` mapped to BCP272, matching the handling before `d6d06cb66`.
2. `LoadChain`: check that `extends` is a string. Any other non-null value reports an error: `"The "extends" value in the Bicep configuration file "{uri}" must be a string."`
3. `FindCompilerVersionDeclaringUri`: check that `bicep` is an object before reading from it.
4. Remove the per-layer binding in `BuildChain`: it never produces diagnostics (B4) and it swallows exceptions. Keep layer entries that only carry each file's URI, so the language server keeps clearing old squiggles until PR 2.
5. Build the cycle message from the ordered list of layers, not by enumerating the `HashSet`.
6. Reword BCP273 (3.4). Fix the doc comments on `IBicepConfiguration.GetDiagnostics` and `BicepConfigurationChain.GetEffectiveConfiguration`.

Caching:

7. Record the chain's files as soon as each file is read, before any early return, so failed chains are tracked too.
8. `PurgeCacheForAffectedChains` stops clearing the lookup cache on every match; only events on a `bicepconfig.json` reset discovery.
9. Remove `directoryHandleCache`: building a directory handle is in-memory string work, and handles compare by URI, so the lookup cache still hits.
10. Replace `PurgeCacheForAffectedChains`, `PurgeAllCaches` and `PurgeChainCache` with `InvalidateFile(uri)` (returns whether anything was dropped) and `InvalidateAll()`. Delete `PurgeCache`, `PurgeLookupCache`, `GetDependenciesForLeaf` and the unused `BicepConfigurationChain.GetAllDiagnostics`. Update `PatchingConfigurationManager` and the `ConstantBicepConfigurationManager` test fake.
11. The language server passes **every** watched-file event to `InvalidateFile` and refreshes compilations only if something was dropped. Before, only files named exactly `bicepconfig.json` were considered, so editing an extended base config such as `bicepconfig.base.json` never refreshed anything. Unsaved editor changes no longer purge anything, because configuration is read from disk.
12. A URI passed to `InvalidateFile` also covers everything under it, because clients may report only the folder when a folder is deleted. It uses `IOUri.IsBaseOf`, which compares whole path segments, so `base` covers `base/x.json` but not `base.json`. Cached lookups for directories under the folder are also dropped, so a folder recreated without its `bicepconfig.json` finds its parent's config. That cleanup doesn't count as an invalidation, so a folder reported as "changed" doesn't refresh every compilation.

Tests:

- A data-driven test over the inputs that crash today, plus a wrong-type value in each top-level section. Each asserts a diagnostic and no exception.
- The cycle message lists files in order.
- Fixing a broken base file, or creating a missing one, and calling `InvalidateFile(base)` reloads the chain.
- Editing one leaf leaves other chains cached; a file no chain read returns `false`.
- Creating a `bicepconfig.json` closer to a source file changes which leaf it finds.
- Deleting a folder that holds a base config, or recreating a folder without its config, when only the folder is reported; a path that only shares a prefix (`base` vs `base.json`) doesn't match.
- LangServer: editing an extended base config with another name refreshes the compilation (fails without the fix).
- Run `BicepConfigChangeHandlerTests` and the LangServer `BicepConfigTests`.

### PR 2 – Loader pipeline with declaring-file tracking **(behavior change: errors name the right file)**

Fixes B3 (for binding errors) and B4.

1. Add `ConfigurationFile`, `ConfigurationFileReader`, `ConfigurationChainResolver` and the per-file parse cache (3.1, 3.2), and move the `LoadChain` logic into them. A base config shared by several leaves is parsed once. (PR 13 removes the parse cache; see 3.2.)
2. Add `ConfigurationMerger` and `ConfigurationProvenance`: one deep merge over `JsonNode` that replaces the repeated `JsonElement.Merge` calls and `StripExtendsProperty`. `null` is ignored everywhere (before, a `null` with nothing to inherit was kept); PR 7 adds the warning.
3. Binding errors carry the failing property as a `ConfigurationPropertyPath`, and the diagnostic goes to the file that declared it. `CloudConfiguration` errors go to the file that declared `cloud.currentProfile` or the profile. `JsonException.Path` is converted with `JsonPath.Net`, so names containing dots work.
   - Other errors: a read or parse error goes to that file; a bad `extends` value or a missing `extends` target goes to the file with the `extends`; a cycle or a chain that's too deep goes to the leaf, because the message describes the chain from there.
4. `BicepConfigurationChain` keeps a diagnostics list for each file, so `EnumerateDiagnosticsPerFile` returns real results and the language server stops publishing everything to the leaf. Diagnostics without a file, such as an invalid `BICEP_TRUSTED_CLOUDS` value, are only shown in Bicep files, because no config file caused them.
5. `BicepConfigurationManager` only coordinates: locate -> read -> chain -> merge -> bind -> policy.

Tests:

- The reader (comments, trailing commas rejected, non-object, unreadable file) and the chain resolver (which file each error is reported on).
- A counting `IFileExplorer` shows a shared base is read once for two leaves (removed with the parse cache in PR 13).
- A merge table: objects, arrays, `null`, `extends` removed, `$schema` ignored, and the declaring file recorded for each.
- An invalid `currentProfile` in a base config is reported against the base file, including in the language server; so is an invalid profile whose name contains a dot.

### PR 3 – The declaring file is used everywhere **(behavior change: relative paths resolve against the file that declares them)**

Fixes B2 and the rest of B3.

1. `bicep.version` (BCP456 in `SemanticModel`), mock paths (`ModuleAliasMockRule`) and module alias errors read the declaring file from the provenance. Delete `FindDeclaringUrisByKey`, `FindCompilerVersionDeclaringUri`, `CompilerConfiguration.WithDeclaringUri`, `ModuleAliasesMockConfiguration.WithDeclaringUris` and `OciArtifactModuleAliasMock.DeclaringConfigUri`.
2. Alias errors name the file that declared the alias. A missing alias still names the leaf, because that's where it would be added.
3. Relative paths are resolved when binding, against the file that declared them:
   - `extensions` local paths
   - `documentation.template.file` and `includeRoot`
   - `moduleAliasesMock.br.*.mapToFilePath`
4. `ArtifactReferenceText`, `SourceFileGroupingBuilder.GetImplicitExtension` and `DocsGenerationOptionsResolver` use those absolute URIs and stop using `ConfigFileUri`. `extension foo` and `implicitExtensions: ["foo"]` share one helper, so they can't disagree again.

Tests:

- An extension and a docs template declared in a base config, with relative paths, used from a child folder.
- An implicit local extension used from a subfolder of the folder that holds `bicepconfig.json`, with and without `extends`. Flip `ArtifactReferenceResolutionTests.Implicit_extension_with_a_bare_path_is_resolved_relative_to_the_referencing_file`, which pins today's behavior.
- Regression tests for mock paths and `bicep.version`, which already use the declaring file.
- A CLI test in `ConfigurationInheritanceTests`.

This is the only PR where a relative path can resolve to a different file than before: an inherited one, or an `extensions` entry used through `implicitExtensions` from a subfolder. Mention it in the release notes. For anyone who copied files into the child folder to work around the bug, the declaring config's path now wins.

### PR 4 – `BICEP_ARTIFACT_CACHE_DIR` **(behavior change: deprecation warning)**

The cache root is a machine-wide choice, not a per-project one. Keeping it in `bicepconfig.json` means two projects on the same machine can silently use different caches, and a relative value depends on the working directory, which is unpredictable for the language server.

1. Add `BicepEnvironmentVariables.ArtifactCacheDir = "BICEP_ARTIFACT_CACHE_DIR"`. The name follows the "artifact cache" vocabulary in `rep-0022-implementation-plan.md`; `~/.bicep` holds only artifact cache entries (OCI, Template Spec and local extension).
2. Accepted values: an absolute path, or a path starting with `~`, expanded to the user profile. A relative value is ignored with a warning (like BCP459 for `BICEP_TRUSTED_CLOUDS`), and the default is used.
3. `FeatureProvider.CacheRootDirectory` precedence: the environment variable, then the deprecated `cacheRootDirectory` config value, then `~/.bicep`.
4. A `cacheRootDirectory` value in any config file gives a "deprecated setting" warning (3.4) that names the file and points to the variable. The schema marks the property `deprecated`. Relative config values keep today's behavior until PR 12 removes the property.
5. The `~` expansion moves out of the `BicepConfiguration` constructor into a helper shared with the variable.

Tests: precedence; `~` expansion; relative values rejected; the deprecation warning names the declaring file.

### PR 5 – Sections and `BicepConfiguration` become records; one source of defaults

No behavior change. Larger than the others, but most of the files outside `Configuration/` only change a type name.

1. Turn each section into a `sealed record` that keeps today's methods (3.5). Delete `ConfigurationSection<T>`, `IWritableConfigurationSection`, and the section interfaces except `IBicepCloudConfiguration`.
2. Make `BicepConfiguration` a `sealed record`. Move `With(...)` into `Bicep.Testing` as a thin wrapper over `with`, so the 25 test files that call it don't change.
3. Add `BicepConfigurationJsonContext` (source-generated) and replace `ToNonNullObject` and the IL2026 suppressions in configuration code. `ToUtf8Json` serializes through it.
4. Remove the casts in `ModuleReferenceCompletionProvider`, `CloudConfigurationTrustPolicy`, `BicepConfigurationManager` and the test helpers.
5. Defaults: the built-in `bicepconfig.json` is the only source. Remove the defaults written into C# records (documentation example sources, `DocumentationOutput.File`) and the `TryGetProperty` fallbacks in `Bind`; binding always runs over a merge that includes the built-in config.
6. Extend `BicepConfigSchemaTests`: every default in the built-in config equals the schema's `default` for that property, and every bound property appears in the schema.

Tests: existing tests; `BicepConfiguration.BuiltIn.ToUtf8Json()` gives the same output before and after.

### PR 6 – Typed analyzer settings **(behavior change: new warnings)**

1. `AnalyzersConfiguration` binds to `RuleConfiguration` for each rule (3.5).
2. `LinterRuleBase.GetDiagnosticLevel` reads `RuleConfiguration.Level`. `GetConfigurationValue<T>` deserializes from `RuleConfiguration.Options` once per rule per configuration, cached on the record. The rules with options (`no-hardcoded-env-urls`, `use-recent-api-versions`, `use-recent-az-powershell-version`) keep their call sites.
3. Warnings (3.4):
   - "Unknown setting" for an unknown analyzer namespace (only `core` exists today) and for an unknown rule name in any namespace.
   - "Ignored setting" for an invalid `level` and for a rule option of the wrong JSON type. The rule then uses its default, as today.

Tests: existing linter tests; each warning, naming the declaring file and the property.

### PR 7 – Experimental features as a set; unknown settings and `null` **(behavior change: new warnings)**

1. Replace the 16-boolean `ExperimentalFeaturesEnabled` with `ExperimentalFeatures` (3.5), backed by `FeatureProvider` metadata. `FeatureProvider` reads from it. Adding a feature means adding one metadata entry and one property.
2. Warnings (3.4):
   - "Unknown setting" for an unknown top-level property (`$schema` and `extends` are allowed) and for an unknown feature name.
   - "Deprecated setting" for a feature that has graduated or been removed, using a `RetiredFeatures` list.
   - "Ignored setting" for any `null` value (3.3).

Tests: existing `FeatureProvider` and `BicepConfigSchemaTests`; each warning; a `null` in a child config is ignored and the base value stays.

### PR 8 – Typed extension sources and cloud profiles

No behavior change.

1. Parse extension entries into `ExtensionSource` when binding. `IsSysOrBuiltIn`, `TryGetExtensionSource` and `ArtifactReferenceText` switch on the variant instead of checking strings.
2. Parse cloud profile URIs when binding. `CloudConfigurationTrustPolicy.CreateBuiltInPairs` reads the `Uri`s. `CloudConfiguration` equality covers every field.

Tests: unit tests for each `ExtensionSource` kind and for cloud parsing; existing extension and trust-policy tests.

### PR 9 – Per-section fallback **(behavior change)**

When one section is invalid, keep the valid sections and fall back to the built-in value only for that section, with an error that names the file and property. A bad `documentation` section no longer throws away the user's analyzer rules and module aliases.

1. Each section's binder returns `(section, diagnostics)`. When a section has errors, the built-in section is used and the errors are kept.
2. Cloud stays strict. If `cloud` is invalid or untrusted, compilation reports the error, and restoring artifacts doesn't run while it's present, so tokens are never sent to the built-in cloud by accident. Check the restore path (`RemoteArtifactRestorer`) explicitly.
3. Chain errors (cycle, depth, a missing or unparsable file) still use the whole built-in config: there's no merged document to take sections from.

Tests: an invalid section next to a valid one keeps the valid one; an invalid cloud blocks restore; chain errors still fall back completely.

### PR 10 – One configuration snapshot per file *(handed off)*

Done by the source graph redesign: `SourceNode` captures `Configuration` and `Features` when it's built, and `BicepSourceFile.LoadConfiguration()` and `LoadFeatures()` are deleted, so a missed call site fails to compile. Carry over this test: in the language server, changing the config during a compile still gives each compilation one config.
### PR 11 – Explicit config path passed in *(handed off)*

Done by the source graph redesign (section 1): the explicit config file is a graph-build input, nodes capture it, and the builder passes each node's configuration to the services it calls, so `ExplicitConfigurationContext` is deleted (redesign step 2). Carry over the existing `--config` CLI and JSON-RPC tests, including the one that runs two JSON-RPC requests with different config paths at the same time.

### PR 12 – *(later release)* Remove `cacheRootDirectory`

At least one release after PR 4. Remove the property from `BicepConfiguration`, the binder and the schema. A leftover key then gets the "unknown setting" warning.

### PR 13 – Configuration cache checks its own dependencies **(behavior change: `bicep jsonrpc` and the MCP server see config edits)**

Fixes B6 for configuration and replaces PR 1's interim `InvalidateFile` (3.2).

1. Add `IFileHandle.TryGetVersion()`, returning `FileVersion(Timestamp, Length)` (last-write time in UTC ticks and size, of a symbolic link's final target), or `null` if the file doesn't exist. The in-memory file system returns a counter shared by all files that increases on every write, so rewriting a file with the same content, or deleting and recreating it, gives a new version.
2. Add `FileDependencies` to `Bicep.IO`: `IsCurrent()`, and a `Builder` that records files read, with their versions (recorded before reading), files checked for existence only, and failed reads.
3. Add `FileChangeEpoch` to `Bicep.IO`: `Current` and `StartNew()`.
4. `BicepConfigurationManager` records `FileDependencies` for each chain and for each located-leaf lookup (whether `bicepconfig.json` exists in every directory searched). Each entry is checked the first time it's read in an epoch; without an epoch, it's never checked again. A failed read is never current, so it's retried in the next epoch, and concurrent loads of the same entry publish and return one result. The manager returns the same configuration instance until it reloads it. The parsed-file cache from PR 2 is removed (3.2).
5. Hosts:
   - The language server registers a `FileChangeEpoch` and starts a new one in the watched-files handler and on `didSave` of a config file. `BicepCompilationManager.HandleFileChanges` then refreshes every Bicep file whose semantic model captured a different configuration instance than the manager now returns (and the files that depend on it). After redesign step 6, `SourceGraph.WithChangedFiles` replaces this.
   - The CLI registers one: `bicep jsonrpc` starts a new epoch at the start of each request, and one-shot commands never do.
   - The MCP server starts one in its call-tool filter.
6. Delete `InvalidateFile`, `InvalidateAll`, `RefreshAllActiveCompilations` and the invalidation in `BicepConfigLifecycleManager`, which keeps only diagnostics publishing.

Tests:

- `FileDependencies`: changed, deleted and created files; a file checked for existence only isn't stale when its content changes.
- In a new epoch: editing a base config, creating a closer `bicepconfig.json`, and deleting the leaf are all seen on the next `GetConfigurationChain`. A locked config is retried after it's unlocked; two concurrent loads return the same instance; editing a symbolic link's target changes the link's version.
- With an epoch: within an epoch, cached entries are trusted; in a new epoch, they're checked once; a new epoch with nothing changed returns the same configuration instance.
- A CLI JSON-RPC test: compile, edit `bicepconfig.json`, compile again, and see the new setting.
- The language server: a config file created after the compilation opened is used after the next watched-file batch; a batch that changed no configuration doesn't recompile. The LangServer `BicepConfigTests` keep passing. One changes: a `bicepconfig.json` created above the one a file already uses no longer refreshes the compilation, because nothing it uses changed (before, any `bicepconfig.json` event refreshed everything).
Measured with the `bicep-perf-profiling` skill on a generated 401-file repo, 7 folders deep, with a two-file config chain (3.2, item 4): checking on every read cost 15%, almost all in `FileDependencies.IsCurrent()`; with an epoch the CLI build time matches `main`.

### PR 14 – Auxiliary file cache uses the same mechanism **(behavior change: `bicep jsonrpc` and the MCP server see `load*()` edits)**

Fixes B6 for files read by `load*()` functions. Waits on the source graph redesign's open question about data-file loads (section 1): if they become graph edges, the redesign handles their freshness and this PR is dropped.

1. `AuxiliaryFileCache` entries record `FileDependencies` (one file each).
2. It checks an entry's version the first time the entry is read in a `FileChangeEpoch` (or on every read without one), and drops entries that changed or were deleted.
3. `BicepCompilationManager` stops trimming the cache by hand on file events (keeping the trim for files no open compilation uses), and finds files to refresh the same way it does for configuration (3.2).
4. Long-running hosts without a file watcher trim entries that haven't been read for a while, so the cache stops growing without limit.

Tests: a CLI JSON-RPC test that edits a `loadTextContent` file between two requests; the existing LangServer tests for files read by `load*()` functions.

### Optional follow-ups

- **Diagnostics at the property's position.** Today every config diagnostic is at offset 0, so the language server publishes it at `Range(0,0,0,0)`.
  - **Positions: a positioned JSON tree.** `JsonElement` and `JsonNode` have no position API. Instead of a second `Utf8JsonReader` pass next to the parse, `ConfigurationFileReader` reads each file once with `Utf8JsonReader` into a small tree of our own in which every value records its file and byte offset (`TokenStartIndex`). Line starts and character positions are computed from the file text only when a diagnostic is published.
    - `ConfigurationMerger` merges these trees, so every merged value still knows where it came from. `ConfigurationProvenance` and its longest-prefix lookup go away, along with the descendant-path scan on every replaced value.
    - Binding is unchanged: the merged tree is written to JSON once and bound with source-generated deserialization; a binding error's `JsonException.Path` leads to the value in the tree, giving the declaring file and position.
    - Keep numbers as their raw text, decode strings with `GetString()`, keep today's reader options (comments skipped, trailing commas rejected, default depth limit), and parse the built-in config into a tree once.
    - Expected cost is about today's: one read pass per file (cached), and the merge replaces today's copy into `JsonNode`. Measure a chain load before and after (`bicep-perf-profiling` skill).
  - **One diagnostic per destination.** A config error is shown in two places: on the config file in the editor, and on every `.bicep` file that uses it. The `.bicep` copy is the only one the CLI, JSON-RPC and MCP report (it fails the build), and the only one the editor shows when the config file isn't open. A diagnostic has a single span, so the two can't share one object: the config file gets the positioned diagnostic, and each `.bicep` file gets one at its start that names the config file and property (as BCP273 already does). `ConfigurationDiagnostic` carries the config-file position next to `File`.
  - **Diagnostics by file.** With line starts on both sides, source files and config files can share one small "URI + line starts" abstraction. The source graph redesign's `FileDiagnostics` (an ordered list of per-file records) replaces `GetAllDiagnosticsByBicepFile()`'s dictionary, because no caller looks diagnostics up by file and the dictionary enumerates in hash order. Replace `DiagnosticsPerFile` with the same kind of list.
- **Remove `IBicepCloudConfiguration` and `IBicepConfiguration`.** Rename-only changes over 51 and 58 files. Only worth doing if the extra names cause confusion.

---

## 5. Decisions

1. **`null`**: not allowed anywhere; it gives an "ignored setting" warning and the inherited value stays (3.3).
2. **Fallback**: per section, with cloud kept strict (PR 9).
3. **Cache root**: moves to `BICEP_ARTIFACT_CACHE_DIR`, which accepts an absolute path or `~`. The config property is deprecated with a warning (PR 4) and removed a release later (PR 12).
4. **Diagnostic codes**: one code per category (3.4).
5. **Unknown rule names**: checked in every analyzer namespace, and unknown namespaces are reported too (PR 6).
6. **Cache freshness**: caches record the files they read and check them themselves (`FileDependencies`), once per `FileChangeEpoch`. Hosts start a new epoch when files may have changed (the language server per watched-file batch, `bicep jsonrpc` and the MCP server per request); one-shot CLI commands never do, and without an epoch entries are never checked again, as on `main`. Nothing is pushed: the language server finds changed configurations by comparing instances. A `FileSystemWatcher`-based approach is deferred (3.2). PR 1's `InvalidateFile` was an interim API that PR 13 removes, together with the unused `InvalidateAll`.

### Still open

1. **Should `bicep.version` be inherited?** Today it is: a base config's constraint applies to every child that extends it (`GetConfigurationChain_VersionInBaseOnly_DeclaringConfigUriIsBase`), and PR 3 keeps that. With `null` disallowed, a child can replace an inherited constraint but can't remove it. If `bicep.version` should be allowed only in the leaf file, that's a separate change: an "invalid configuration" error when a base config sets it.

---

## 6. Risks

| Risk | Mitigation |
|---|---|
| PR 3 changes which file a relative path points to | Release notes; a test for each kind of path value; only inherited values and implicit extensions used from subfolders change, and both currently point to the wrong file |
| New warnings (PRs 4, 6, 7) break CI pipelines that treat warnings as errors | Warnings only; JSON paths in messages; release notes |
| Per-section fallback (PR 9) could restore artifacts with the built-in cloud when the user's cloud config is invalid | Cloud stays strict: an invalid or untrusted cloud is an error, and restore doesn't run while it's present |
| Removing `cacheRootDirectory` (PR 12) moves caches for users who set it | At least one release with a deprecation warning naming the variable; the variable takes precedence as soon as PR 4 ships |
| PRs 5 and 10 are large | Most files only swap a type name or a call; `With(...)` kept in `Bicep.Testing` so test files don't churn; baselines and `ToUtf8Json` output checked |
| Version checks miss an edit (PR 13) | Compare size as well as last-write time, and version a symbolic link's target. An edit is still missed if it keeps the size and the timestamp, which on NTFS, ext4 or APFS takes a tool that restores the old timestamp (`touch -d`, `cp -p`), or a write within the same tick on file systems that record times only to the second (FAT, HFS+, some network shares). The next edit is seen. Config files are small enough to hash if this turns out to matter |
| Version checks cost too much in CLI commands or large repos (PR 13) | Measured: checking on every read cost 15% on a 401-file build, so one-shot commands make no checks and `bicep jsonrpc` and the MCP server check each entry once per request (3.2). Caches without a `FileChangeEpoch` (library use, tests) never check again, as on `main`. Once the redesign captures configuration per node, about one check per source file per graph build |
| A file changes between being read and its version being recorded (PR 13) | Record the version before reading the file, so a change in between shows up as stale on the next check |

---

## 7. Tests

- **Unit** (`Bicep.Core.UnitTests/Configuration`): reader, chain resolver, merger and provenance, each section's binder, cache invalidation, and the "no exception for bad input" test.
- **Integration** (`Bicep.Core.IntegrationTests/ConfigurationInheritanceTests`): an inheritance scenario for each section that has paths or errors (PRs 2–3).
- **Language server** (`Bicep.LangServer.IntegrationTests/BicepConfigTests`, `Bicep.LangServer.UnitTests/Configuration`): per-file squiggles and invalidation on file events. (One configuration per compilation is tested by the source graph redesign.)
- **CLI JSON-RPC** (`Bicep.Cli.IntegrationTests`): edits to `bicepconfig.json` and to files read by `load*()` functions are seen between requests (PRs 13–14).
- **Schema** (`BicepConfigSchemaTests`): defaults and properties match the schema (PR 5).
- **Baselines**: PRs 1, 2, 6 and 7 change diagnostic text. Update the baselines and review every diff.
