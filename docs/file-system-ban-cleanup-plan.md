# File-System Ban Cleanup Plan

This document is the working playbook for making the `IFileSystem` ban in `Bicep.Core` and `Bicep.Decompiler` take effect, and for closing related gaps where file-system access bypasses the `Bicep.IO` abstractions.

Future cleanup PRs should update this document as work is completed or new constraints are discovered.

## Scope Decision

Keep the Core/Decompiler ban cleanup separate from CLI globbing. `Bicep.Cli` intentionally uses `IFileSystem` for command-line paths and working-directory handling; it should not migrate to `Bicep.IO` merely to satisfy a ban that applies to Core and Decompiler.

WP8 is implemented using an `IFileSystem`-backed globbing matcher adapter. This makes pattern discovery mockable while retaining the matcher's directory pruning and existing symlink/junction traversal policy. It does not introduce a new `Bicep.IO` API.

WP1-WP6 remain deferred pending a separate decision about the abstraction needed by documentation discovery. In particular, adding `IsReparsePoint` to every `Bicep.IO` handle is a proposal, not an agreed API.

Status legend:

- `[x]` Done
- `[~]` In progress
- `[ ]` Not started

## Problem

[`Bicep.Core/BannedSymbols.txt`](../src/Bicep.Core/BannedSymbols.txt) and [`Bicep.Decompiler/BannedSymbols.txt`](../src/Bicep.Decompiler/BannedSymbols.txt) are meant to stop those projects from touching the file system directly, so that all file access goes through `Bicep.IO.Abstraction` (`IFileExplorer`, `IFileHandle`, `IDirectoryHandle`, `IOUri`).

The `IFileSystem` entry has a typo in its namespace (it is missing the trailing `s`):

```text
T:System.IO.Abstraction.IFileSystem      <- banned entry
  System.IO.Abstractions.IFileSystem     <- actual type (TestableIO.System.IO.Abstractions)
```

The banned API analyzer matches entries by exact documentation ID and does not report entries that resolve to nothing, so this entry has never had any effect. `Bicep.Core` builds with `TreatWarningsAsErrors=true` while still using `IFileSystem` in several places (see below), which confirms the ban is inactive. The entry's message text contains the same typo.

Correcting the entry is a one-line change, but it activates the ban and turns every remaining `IFileSystem` usage in `Bicep.Core` into a build error. Those usages must be migrated first.

## Principles

- `Bicep.Core` and `Bicep.Decompiler` must not reference `System.IO` file types or `System.IO.Abstractions.IFileSystem`. Use `Bicep.IO.Abstraction` instead.
- When `Bicep.IO.Abstraction` lacks a capability that Core needs, add it to `Bicep.IO` (with both file-system and in-memory implementations) rather than reaching for `IFileSystem`.
- Prefer making a rule enforceable by the analyzer over relying on review. Avoid `#pragma` suppressions of `RS0030`.
- Activate the corrected ban in the same PR as the last migration, so the build proves nothing is left.

## Current Violations

Usages of `IFileSystem` in `Bicep.Core` that the corrected ban would reject. `Bicep.Decompiler` has none.

| Location | Usage | Migration |
|---|---|---|
| `Registry/Oci/Oras/DockerCredentialProvider` | Reads the Docker `config.json` via `fileSystem.File.Exists`, `File.OpenRead`, and `Path.Combine` | Use `IFileExplorer.GetFile(uri)` with `Exists()`/`OpenRead()`, and build the path as an `IOUri`. No new APIs needed. |
| `Documentation/BicepDocumentationGenerator` | Optional `IFileSystem? fileSystem` constructor parameter, used only by `IsReparsePoint` (`File.GetAttributes(...).HasFlag(FileAttributes.ReparsePoint)`) to avoid following symlinks/junctions during discovery | Needs a reparse-point/symlink check on `Bicep.IO` handles (WP1), then drop the parameter. |
| `BicepCoreServiceCollectionExtensions.AddBicepCore` | Registers `TryAddSingleton<IFileSystem, LocalFileSystem>()` (composition, not file operations, but it names the banned type) | Move file-system registrations into a `Bicep.IO` service-collection extension called by `AddBicepCore` (WP4). |

These files also contain `using System.IO.Abstractions;` with no other apparent use. Namespace imports are not banned, but they should be removed as part of the cleanup (confirm with the build): `BicepCompiler`, `Registry/ExternalArtifactRegistry`, `Registry/OciArtifactRegistry`, `Registry/Providers/TypesV1Archive`, `Registry/LocalModuleRegistry`, `TypeSystem/Providers/ResourceTypeProviderFactory`.

## Progress Tracker

- `[x]` Fix the stale namespace in the ban messages (`Bicep.Core.IO.Abstraction`/`Bicep.Core.IO` -> `Bicep.IO.Abstraction`).
- `[ ]` WP1: Add a reparse-point check to `Bicep.IO` handles.
- `[ ]` WP2: Migrate `DockerCredentialProvider` to `IFileExplorer`.
- `[ ]` WP3: Migrate `BicepDocumentationGenerator` off `IFileSystem`.
- `[ ]` WP4: Move file-system DI registrations into `Bicep.IO`.
- `[ ]` WP5: Remove unused `System.IO.Abstractions` imports from `Bicep.Core`.
- `[ ]` WP6: Correct the banned `IFileSystem` entries.
- `[ ]` WP7: Ban `FileSystemGlobbing` APIs that read the disk.
- `[x]` WP8: Route CLI `--pattern` globbing through its injected `IFileSystem`.

WP1-WP6 can ship as one PR after the handle API design is settled; order the commits WP1 -> WP5 -> WP6 so the final commit shows the analyzer enforcing the rule. WP7 and WP8 are independent.

## Work Packages

### WP1: Reparse-Point Check in `Bicep.IO`

Deferred proposal: add a way to ask whether a file or directory handle is a reparse point (symlink or junction), for example on `IIOHandle`. First decide whether this platform-specific query belongs on the shared abstraction or whether discovery needs a different capability.

- File-system implementation: check `FileAttributes.ReparsePoint` through the handle's `IFileSystem`.
- In-memory implementation: return `false`.
- Validate on Windows and Linux (symlinks), since the existing check is attribute-based.

### WP2: `DockerCredentialProvider`

Replace the `IFileSystem` constructor dependency with `IFileExplorer`. Resolve the Docker config directory (`DOCKER_CONFIG` or `~/.docker`) to an `IOUri`, then use `GetFile(uri).Exists()` and `OpenRead()`.

- Update its unit tests from a raw `MockFileSystem` to `MockFileSystemTestFileSet` (or `InMemoryTestFileSet`).
- Validation: `Bicep.Core.UnitTests` tests for `DockerCredentialProvider`, and the registry tests that depend on credential resolution.

### WP3: `BicepDocumentationGenerator`

Remove the optional `IFileSystem? fileSystem` parameter and use the WP1 check on the handles that discovery already enumerates. The check is currently skipped when no `IFileSystem` is supplied; after the change it should always apply (in-memory handles report `false`).

- Validation: `DocsCommandTests` in `Bicep.Cli.IntegrationTests` and the documentation tests in `Bicep.Core.UnitTests`.

### WP4: DI Registrations

Add a service-collection extension in `Bicep.IO` (for example `AddBicepFileSystem()`) that registers `IFileSystem` and `IFileExplorer` with `TryAdd`, and call it from `AddBicepCore`. Behavior is unchanged for hosts and for `BicepCompiler.Create()`.

- Cost: `Bicep.IO` gains a reference to `Microsoft.Extensions.DependencyInjection.Abstractions`.
- Rejected alternative: keep the registration in Core under a `#pragma warning disable RS0030`. It is cheaper, but it leaves a permanent exception to the rule.

### WP5: Unused Imports

Remove `using System.IO.Abstractions;` from the files listed under [Current Violations](#current-violations) once they no longer need it.

### WP6: Correct the Ban

In both `BannedSymbols.txt` files, change `T:System.IO.Abstraction.IFileSystem` to `T:System.IO.Abstractions.IFileSystem` and fix the namespace in the message text. The build must pass with no suppressions.

- Validation: `dotnet build Bicep.slnx` (the analyzer runs as part of the build), then the full test suites of the projects touched by WP1-WP4.

### WP7: Close the Globbing Gap

`Microsoft.Extensions.FileSystemGlobbing` can read the disk without naming any banned type: `MatcherExtensions.GetResultsInFullPath(string)` takes a path string and walks the directory with `System.IO.DirectoryInfo` internally. A call in `Bicep.Core` would bypass the ban unnoticed. The safe pattern, already used by `BicepDocumentationExampleDiscovery`, is to enumerate through `Bicep.IO` handles and call `Matcher.Match(relativePath)`, which only matches strings.

Add to `Bicep.Core/BannedSymbols.txt`:

- `MatcherExtensions.GetResultsInFullPath`
- `Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper`

### WP8: CLI `--pattern` Globbing (Completed)

`InputOutputArgumentsResolver.ResolveFilePattern` previously called `matcher.GetResultsInFullPath(rootPath)`, so `--pattern` read the real disk even when tests injected a mock `IFileSystem`.

- Keep the existing `IFileSystem` constructor dependency.
- Adapt `IDirectoryInfo` and `IFileInfo` to the matcher's abstractions in [`FileSystemGlobbingDirectory`](../src/Bicep.Cli/Helpers/FileSystemGlobbingDirectory.cs), and call `Matcher.Execute`. All directory access goes through the injected file system.
- Preserve matcher-controlled directory pruning and symlink/junction behavior rather than recursively enumerating everything or adding a new link-skipping policy.
- Sort matched relative paths with ordinal comparison for deterministic output.
- Keep working-directory resolution, root splitting, and `--outdir` mapping unchanged and CLI-only.
- Use mock-only fixtures in `FormatCommandTests` and `ConfigOptionTests`. Cover recursive and non-recursive matching, literal and missing paths, deterministic ordering, directory pruning, reparse-point attributes, and relative output mapping in `InputOutputArgumentsResolverTests`.

Validation on Windows:

- `dotnet build Bicep.slnx --no-restore -v quiet`: passed with zero warnings and errors.
- `dotnet exec src\Bicep.Cli.UnitTests\bin\Debug\net10.0\Bicep.Cli.UnitTests.dll --filter FullyQualifiedName~InputOutputArgumentsResolverTests`: 41 passed, including physical directory-symlink comparisons against the previous globbing API.
- `dotnet exec src\Bicep.Cli.IntegrationTests\bin\Debug\net10.0\Bicep.Cli.IntegrationTests.dll`: all 1,165 passed.
- Linux validation was not run locally.
