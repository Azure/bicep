# File-System Ban Cleanup Plan

This document is the working playbook for making the `IFileSystem` ban in `Bicep.Core` and `Bicep.Decompiler` take effect, and for closing related gaps where file-system access bypasses the `Bicep.IO` abstractions.

Future cleanup PRs should update this document as work is completed or new constraints are discovered.

## Scope Decision

Keep the Core/Decompiler ban cleanup separate from CLI globbing. `Bicep.Cli` intentionally uses `IFileSystem` for command-line paths and working-directory handling; it should not migrate to `Bicep.IO` merely to satisfy a ban that applies to Core and Decompiler.

WP8 is implemented using an `IFileSystem`-backed globbing matcher adapter. This makes pattern discovery mockable while retaining the matcher's directory pruning and existing symlink/junction traversal policy. It does not introduce a new `Bicep.IO` API.

WP1-WP7 add `IIOHandle.IsReparsePoint()` to `Bicep.IO`, migrate the remaining Core usages, and activate the corrected bans. The analyzer now rejects any new `IFileSystem` usage in `Bicep.Core` and `Bicep.Decompiler`.

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

## Original Violations (Resolved)

Usages of `IFileSystem` in `Bicep.Core` that the corrected ban would reject. `Bicep.Decompiler` had none. All have been migrated.

| Location | Usage | Migration |
|---|---|---|
| `Registry/Oci/Oras/DockerCredentialProvider` | Reads the Docker `config.json` via `fileSystem.File.Exists`, `File.OpenRead`, and `Path.Combine` | Use `IFileExplorer.GetFile(uri)` with `Exists()`/`OpenRead()`, and build the path as an `IOUri`. No new APIs needed. |
| `Documentation/BicepDocumentationGenerator` | Optional `IFileSystem? fileSystem` constructor parameter, used only by `IsReparsePoint` (`File.GetAttributes(...).HasFlag(FileAttributes.ReparsePoint)`) to avoid following symlinks/junctions during discovery | Needs a reparse-point/symlink check on `Bicep.IO` handles (WP1), then drop the parameter. |
| `BicepCoreServiceCollectionExtensions.AddBicepCore` | Registers `TryAddSingleton<IFileSystem, LocalFileSystem>()` (composition, not file operations, but it names the banned type) | Move file-system registrations into a `Bicep.IO` service-collection extension called by `AddBicepCore` (WP4). |

These files also contain `using System.IO.Abstractions;` with no other apparent use. Namespace imports are not banned, but they should be removed as part of the cleanup (confirm with the build): `BicepCompiler`, `Registry/ExternalArtifactRegistry`, `Registry/OciArtifactRegistry`, `Registry/Providers/TypesV1Archive`, `Registry/LocalModuleRegistry`, `TypeSystem/Providers/ResourceTypeProviderFactory`.

## Progress Tracker

- `[x]` Fix the stale namespace in the ban messages (`Bicep.Core.IO.Abstraction`/`Bicep.Core.IO` -> `Bicep.IO.Abstraction`).
- `[x]` WP1: Add a reparse-point check to `Bicep.IO` handles.
- `[x]` WP2: Migrate `DockerCredentialProvider` to `IFileExplorer`.
- `[x]` WP3: Migrate `BicepDocumentationGenerator` off `IFileSystem`.
- `[x]` WP4: Move file-system DI registrations into `Bicep.IO`.
- `[x]` WP5: Remove unused `System.IO.Abstractions` imports from `Bicep.Core`.
- `[x]` WP6: Correct the banned `IFileSystem` entries.
- `[x]` WP7: Ban `FileSystemGlobbing` APIs that read the disk.
- `[x]` WP8: Route CLI `--pattern` globbing through its injected `IFileSystem`.

Commits were ordered WP8, then WP1 -> WP7, so the WP6 commit shows the analyzer enforcing the rule with nothing left to migrate.

## Work Packages

### WP1: Reparse-Point Check in `Bicep.IO` (Completed)

Added `IIOHandle.IsReparsePoint()`, which returns true for a symlink, junction, or other reparse point, and false if the entry does not exist.

- File-system implementation: reads `FileAttributes.ReparsePoint` through the handle's `IFileSystem`. It trims the trailing separator from directory paths so the link itself is inspected rather than its target.
- In-memory implementation: returns `false`.
- Tests cover mock attributes, missing entries, in-memory handles, and real file and directory symlinks (validated on Windows; Linux pending CI).

### WP2: `DockerCredentialProvider` (Completed)

Replaced the `IFileSystem` constructor dependency with `IFileExplorer`. The config directory (`DOCKER_CONFIG`, or `.docker` under `HOME`/`USERPROFILE`) is resolved to a directory handle; relative paths resolve against `IEnvironment.CurrentDirectory`, as `File.Exists` did before. Paths that cannot be parsed are treated as a missing config rather than throwing, which preserves the old `File.Exists` behavior.

- Unit tests use `MockFileSystemTestFileSet`; other test call sites use `InMemoryFileExplorer`.
- New tests cover each config location and invalid paths.

### WP3: `BicepDocumentationGenerator` (Completed)

Removed the optional `IFileSystem? fileSystem` parameter, and the `shouldSkip` predicate from `BicepDocumentationExampleDiscovery`. Discovery now always skips linked files and subdirectories using `IsReparsePoint()` (in-memory handles report `false`). As before, the configured example root itself is not checked.

### WP4: DI Registrations (Completed)

Added `AddBicepFileSystem()` in `Bicep.IO`, which registers `IFileSystem` and `IFileExplorer` with `TryAdd`; `AddBicepCore` calls it. Behavior is unchanged for hosts and for `BicepCompiler.Create()`.

- Cost: `Bicep.IO` now references `Microsoft.Extensions.DependencyInjection.Abstractions`.
- Rejected alternative: keep the registration in Core under a `#pragma warning disable RS0030`. It is cheaper, but it leaves a permanent exception to the rule.

### WP5: Unused Imports (Completed)

Removed `using System.IO.Abstractions;` from the files listed under [Original Violations](#original-violations-resolved).

### WP6: Correct the Ban (Completed)

In both `BannedSymbols.txt` files, changed `T:System.IO.Abstraction.IFileSystem` to `T:System.IO.Abstractions.IFileSystem` and fixed the namespace in the message text. The solution builds with no suppressions.

A probe showed that the banned-API analyzer reports member access through a banned type, but not a bare declaration or `new` of an unbanned concrete type. Without another entry, `new System.IO.Abstractions.FileSystem().File.Exists(...)` would compile, so both files also ban `T:System.IO.Abstractions.FileSystem`.

### WP7: Close the Globbing Gap (Completed)

`Microsoft.Extensions.FileSystemGlobbing` can read the disk without naming any banned type: `MatcherExtensions.GetResultsInFullPath(string)` takes a path string and walks the directory with `System.IO.DirectoryInfo` internally. A call in `Bicep.Core` would bypass the ban unnoticed. The safe pattern, already used by `BicepDocumentationExampleDiscovery`, is to enumerate through `Bicep.IO` handles and call `Matcher.Match(relativePath)`, which only matches strings.

Added to `Bicep.Core/BannedSymbols.txt` (both confirmed with a probe to produce RS0030):

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

## Validation (WP1-WP7, Windows)

- `dotnet build Bicep.slnx`: passed with zero warnings and errors, with the corrected bans active and no RS0030 suppressions.
- `Bicep.IO.UnitTests`: all 157 passed, including real file and directory symlinks.
- `Bicep.Core.UnitTests`: all 7,363 passed.
- `Bicep.Cli.UnitTests`: all 85 passed.
- `Bicep.Cli.IntegrationTests`: all 1,165 passed.
- `Bicep.Decompiler.UnitTests`: all 52 passed.
- Linux validation (attribute-based symlink detection in particular) is left to CI.
