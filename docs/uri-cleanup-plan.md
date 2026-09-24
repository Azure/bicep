# URI Cleanup Plan

This document is the working playbook for moving shared test infrastructure into `Bicep.Testing`, migrating tests away from file-oriented `System.Uri` and incidental direct `MockFileSystem` usage, and tightening the `DocumentUri`/`IOUri` boundary between LangServer and `Bicep.Core`.

Future cleanup PRs should update this document as work is completed or new migration constraints are discovered.

## How To Use This Plan

1. Pick exactly one work package from the work queue.
2. Keep the PR scoped to one test area or one helper boundary.
3. Prefer the conversion recipes below before inventing new helper APIs.
4. Do not migrate intentionally file-system-focused tests.
5. Run the narrow validation listed for the package.
6. Update the progress tracker and work queue before finishing the PR.

Status legend:

- `[x]` Done
- `[~]` In progress
- `[ ]` Not started

## Migration Principles

- Use `IOUri` for compiler, source graph, and test-file identifiers.
- Use `DocumentUri` only at LangServer/LSP boundaries.
- Convert `DocumentUri` to `IOUri` only when calling into `Bicep.Core`.
- Convert `IOUri` to `DocumentUri` only when returning Core file identifiers to LangServer/LSP-facing APIs.
- Use `Bicep.Testing.IO` and `TestCompiler` for synthetic test files.
- Keep broadly reusable test helpers in `Bicep.Testing`, not `Bicep.Core.UnitTests.Utils`.
- Do not add new references from any test project to `Bicep.Core.UnitTests`.
- Remove existing `Bicep.Core.UnitTests` project references from non-Core-unit-test projects over time. They should reference `Bicep.Testing` for shared helpers instead.
- Keep `MockFileSystem` where the test validates `System.IO.Abstractions`, path resolution, globbing, current directory, file writes, or `FileSystemFileExplorer` behavior.

## Conversion Recipes

### Single-File Compiler Tests

Use this for tests that only need a synthetic `main.bicep` and do not need restore:

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .WithEmptyAzResources()
    .CompileWithoutRestore(bicepText);

var compilation = result.Compilation;
```

Use `result.Template` and `result.Diagnostics` when needed. They are lazy, so params-file tests can safely use the same result type without forcing template emission.

`CompileWithoutRestore(...)` is synchronous and uses `BicepCompiler.CreateCompilationWithoutRestore(...)`. Use the asynchronous `Compile(...)` overloads only when artifact restore is required.

### Params-File Entry Points

Use `CompileWithoutRestore(entryPointPath, files)` and then inspect `result.Compilation`:

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .WithEmptyAzResources()
    .CompileWithoutRestore(
        "main.bicepparam",
        ("main.bicep", mainText),
        ("main.bicepparam", paramsText));

var compilation = result.Compilation;
```

Keep `IOUri` constants only when diagnostics assertions need stable file identifiers:

```csharp
private static readonly IOUri ParamsUri = TestFileUri.FromInMemoryPath("main.bicepparam");
```

### Static Configuration Or Custom Resource Types

Use fluent `TestCompiler` extensions:

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .WithConfiguration(BicepTestConstants.BuiltInConfigurationWithStableAnalyzers)
    .WithEmptyAzResources()
    .CompileWithoutRestore(bicepText);
```

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .WithAzResources(BuiltInTestTypes.Types)
    .CompileWithoutRestore(bicepText);
```

The `WithXxx` helpers should replace existing singleton registrations with `TestServices.RemoveAll<T>()` before adding replacements.

### Multi-File Synthetic Tests

Prefer `TestCompiler` or `InMemoryTestFileSet`:

```csharp
var fileSet = InMemoryTestFileSet.Create(
    ("main.bicep", mainText),
    ("module.bicep", moduleText));
```

Use `fileSet.GetUri(path)` for expected diagnostic locations.

### LangServer URI Boundary

- Keep request/notification APIs as `DocumentUri`.
- Use `DocumentUriExtensions.ToIOUri()` only when entering `Bicep.Core`.
- Use `IOUriExtensions.ToDocumentUri()` only when returning Core file identifiers to LangServer/LSP APIs.
- Remove intermediate `System.Uri` conversions where the target API can take `DocumentUri` directly.

## Do Not Migrate Blindly

Leave these categories alone unless a PR explicitly targets their owning behavior:

- HTTP, registry, documentation, or other non-file `System.Uri` values.
- LSP-facing `DocumentUri` values.
- CLI tests that validate path resolution, current directory, globbing, stdout/outfile behavior, or file writes.
- `Bicep.IO.UnitTests` tests for `FileSystemFileExplorer`, `FileSystemFileHandle`, and `FileSystemDirectoryHandle`.
- Docker credential and registry tests where the product API requires `IFileSystem`.

## Progress Tracker

- `[x]` LangServer refresh APIs use `DocumentUri` without `System.Uri` round-trips (`9f6498394`).
- `[x]` `TestCompiler` provides in-memory and mock-file-system compilation, synchronous no-restore compilation, restore-capable compilation, and lazy template/parameters/diagnostics results.
- `[x]` Initial Core unit-test slices use `TestCompiler`, including params emission and linter/extension scenarios.
- `[x]` All linter rule tests use `TestCompiler`, `TestFileData`, and test-owned configuration APIs.
- `[x]` `TestConfigurations` and `TestConfigurationBuilder` own shared test configuration presets and mutations.
- `[x]` Public `Test*` toolkit types live in the `Bicep.Testing` root namespace; `FakeEnvironment` lives under `Bicep.Testing.Fakes` and the `Utils` namespace is removed.
- `[x]` Simple Core unit-test callers use `TestCompiler` directly.
- `[x]` No synchronous `CompilationHelper.Compile(...)` call sites remain in `Bicep.Core.UnitTests`.
- `[x]` `Bicep.Decompiler.UnitTests` references `Bicep.Testing` instead of `Bicep.Core.UnitTests`.
- `[x]` `Bicep.Local.Extension.UnitTests` and `Bicep.RegistryModuleTool.TestFixtures` no longer reference `Bicep.Core.UnitTests`.
- `[x]` Reusable JToken assertions have one canonical implementation in `Bicep.Testing.Assertions.Json`; Core retains only baseline-update behavior.
- `[ ]` Replace all `CompilationHelper` call sites with `TestCompiler` and delete `CompilationHelper` and its result types.
- `[ ]` Replace `ServiceBuilder`, `ServiceBuilderExtensions`, and related Core-unit-test DI extensions with `TestCompiler`, `TestServices`, or focused fixtures, then delete them.
- `[ ]` Consolidate reusable assertions, feature fixtures, mocks, and data builders in `Bicep.Testing`; rewrite or remove obsolete helpers instead of moving them wholesale.
- `[ ]` Remove all non-Core-unit-test project references to `Bicep.Core.UnitTests`.
- `[ ]` Core integration test URI dictionaries.
- `[ ]` Incidental `MockFileSystem` usage that only creates compiler-visible files.
- `[ ]` `DocumentUriExtensions.ToUriEncoded` deprecation and removal.

## Work Queue

### Workstream B: Retire Legacy Test Infrastructure

Goal: remove the shared test infrastructure owned by `Bicep.Core.UnitTests`. `CompilationHelper`, `ServiceBuilder`, and their forwarding extensions are migration sources, not APIs to move into `Bicep.Testing`.

This is an umbrella workstream, not one PR. Pick one package below and keep each PR scoped to a coherent test area or helper boundary.

#### [x] Simple Core Unit-Test Callers

Replace direct `CompilationHelper.Compile(...)` calls in:

- `Diagnostics/ErrorBuilderTests`
- `Diagnostics/Linter/Common/FindPossibleSecretsVisitorTests`
- `Diagnostics/LinterAnalyzerTests`
- `Highlighting/SemanticTokenVisitorTests`
- `Semantics/Namespaces/RoleDefinitionFunctionTests`
- `Semantics/ResourceDeclarationDiagnosticTests`
- `TestTests/PrintHelperTests`
- `TypeSystem/FunctionResolverTests`
- `TypeSystem/TypeValidatorAssignabilityTests`

Validation:

- Run all touched classes by `FullyQualifiedName` filter and build `Bicep.Core.UnitTests`.
- Require zero direct `CompilationHelper` references in the migrated files.

Completed validation: all 420 focused tests passed, and migrated-file searches for `CompilationHelper` return no results.

#### [x] Remaining Synchronous Core Unit-Test Callers

- Migrated the dependency, read-only property, and type-casing rewriter tests.
- Migrated the shared Bicep-formatting string assertion.
- Migrated `ThisNamespaceTests`, including feature overrides and restore-capable extension archive tests.
- Kept mock-file-system compilation for binary extension archives; ordinary synthetic files use in-memory compilation.

Validation: all 10 rewriter tests and all 26 `ThisNamespaceTests` passed. A zero-result search confirms that `Bicep.Core.UnitTests` has no remaining `CompilationHelper.Compile(...)` calls.

#### B1: Close `Bicep.Testing` API Gaps

Add only the capabilities required to remove callers of the legacy helpers:

- Map `CompilationHelper.Compile(...)` and `CompileParams(...)` to synchronous `TestCompiler.CompileWithoutRestore(...)` calls.
- Map `RestoreAndCompile(...)` and `RestoreAndCompileParams(...)` to asynchronous `TestCompiler.Compile(...)` calls.
- Use explicit entry-point paths and `TestFileSet`/`TestFileData` instead of `InputFile`, `CreateFileDictionary`, or workspace-building helpers.
- Extend `TestCompilationResult` and its assertions when callers need template, parameters, diagnostics, source-file, or filtered-diagnostic behavior. Do not retain parallel `CompilationResult` and `ParamsCompilationResult` types.
- Add focused `TestCompiler` or `TestServices` customization for configuration patches, feature overrides, analyzers, namespaces, resource types, environment variables, and artifact managers as migration needs arise.
- Use `TestServices.ReplaceSingleton(...)` when overriding an existing service registration; do not stack another singleton registration.

Do not add a `Bicep.Testing.CompilationHelper`, a generic compatibility facade, or forwarding overloads that preserve the old API shape.

#### B2: Replace `CompilationHelper` Callers

Migrate callers by test area:

- Single-file and multi-file synthetic compilation should use `TestCompiler.ForInMemoryCompilation()`.
- Tests that intentionally validate `IFileSystem` behavior should use `TestCompiler.ForMockFileSystemCompilation()` or their existing file-system fixture.
- Restore and registry tests should use `TestExternalArtifactManager` and restore-capable `Compile(...)` overloads.
- Params tests should compile the `.bicepparam` entry point and consume `TestCompilationResult.Parameters`.
- Replace tuple deconstruction and legacy result assertions with `TestCompilationResult` assertions.
- Replace `CompilationHelper.InputFile` with `(string FilePath, TestFileData FileData)` or a local scenario type when additional metadata is genuinely needed.

Delete these when the last caller is gone:

- `src/Bicep.Core.UnitTests/Utils/CompilationHelper.cs`
- `CompilationHelper.ICompilationResult`
- `CompilationHelper.CompilationResult`
- `CompilationHelper.ParamsCompilationResult`
- `CompilationHelper.InputFile`
- `src/Bicep.Core.UnitTests/Assertions/CompilationResultAssertions.cs`
- `src/Bicep.Core.UnitTests/Assertions/ParamsCompilationResultAssertions.cs`
- extensions that exist only for the legacy result types

#### B3: Replace `ServiceBuilder` And DI Extensions

`ServiceBuilder` and its extensions should disappear rather than move:

- Compilation-oriented setup should become fluent `TestCompiler` configuration.
- Tests that only construct services should use `TestServices.Get<T>()` or a focused fixture in `Bicep.Testing`; do not recreate a generic builder facade.
- Move reusable service-registration behavior into focused `TestServices` methods or `TestCompiler` extensions.
- Delete forwarding methods that only wrap `IServiceCollection` registration.
- Replace `BuildCompilation*` extensions with `TestCompiler.CompileWithoutRestore(...)` or `Compile(...)`.
- Keep direct `IServiceCollection` setup local only when a test explicitly validates DI registration behavior.

Deletion targets include:

- `src/Bicep.Core.UnitTests/ServiceBuilder.cs`
- `src/Bicep.Core.UnitTests/Utils/ServiceBuilderExtensions.cs`
- `src/Bicep.Core.UnitTests/IServiceCollectionExtensions.cs` after its reusable registrations have focused replacements
- `IDependencyHelper` and `IDependencyHelperExtensions`

#### B4: Consolidate Assertions

Do not move the whole assertions directory unchanged. Classify each assertion by its subject and consumers:

- Merge duplicate implementations already present in `Bicep.Testing`, such as diagnostic assertion infrastructure.
- Keep focused reusable assertions in `Bicep.Testing.Assertions`; do not move baseline-management or Core-specific assertion APIs with them.
- Use `TestPrinter.Print(...)` only for printing and `BeValidBicepText()` for string syntax validation; keep source annotation rendering separate.
- Expand `TestCompilationResultAssertions` to replace legacy compilation-result assertions, including template/parameters emission and diagnostic filtering where still useful.
- Move broadly reusable assertions such as diagnostics, syntax, JSON tokens, strings, code fixes, and configuration assertions when non-Core projects consume them.
- Keep Core-unit-test-only assertions local when no other project needs them.
- Remove obsolete assertion APIs, misspelled methods, duplicate FluentAssertions entry points, and assertions tied only to deleted wrappers.
- Preserve rich source annotations and assertion scopes during consolidation.

#### B5: Consolidate Features, Mocks, Constants, And Helpers

Treat each family separately:

- Keep the public `Test*` toolkit types in the `Bicep.Testing` root namespace. Keep `Fake*`, `Mock*`, and `Dummy*` implementations in their corresponding domain namespaces; for example, use `Bicep.Testing.Fakes.FakeEnvironment` rather than `TestEnvironment`.
- Do not recreate a catch-all `Bicep.Testing.Utils` namespace.
- Move feature overrides and their provider factory to `Bicep.Testing` if they remain the shared way to configure compiler features. This should allow `TestCompiler.WithFeatureOverrides(...)` to become non-generic.
- Delete the Core `StrictMock` duplicate and use `Bicep.Testing.Mocks.StrictMock`.
- Prefer `TestExternalArtifactManager` and `Bicep.Testing.Fakes.ContainerRegistry` over moving registry mocks and `RegistryHelper` wholesale.
- Split `BicepTestConstants` by responsibility. Move only shared configuration, feature, registry, and test-type fixtures; keep Core-specific constants local or inline them.
- Use `Bicep.Testing.TestConfigurations` for canonical `RootConfiguration` variants; migrate and remove the corresponding `BicepTestConstants` fields as consumers move.
- Migrate the remaining test consumers of `AnalyzersConfigurationExtensions`, `ExtensionsConfigurationExtensions`, and `ExperimentalFeaturesExtensions` to `TestConfigurationBuilder`, then delete those test-only extension files from `Bicep.Core`.
- Replace compiler-file uses of `FileHelper` with `TestFileSet`. Move only reusable temporary-directory, output-file, or cache-root behavior that intentionally uses the real file system.
- Move reusable type builders from `TestTypeHelper` and `FakeResourceTypes` into `Bicep.Testing.Fakes.TypeSystem`; remove overlapping implementations.
- Rewrite or delete helpers whose only purpose was adapting `ServiceBuilder`, `CompilationHelper`, `System.Uri`, or `MockFileSystem`.

#### B6: Deletion Gates

Before declaring this workstream complete:

- No references to `CompilationHelper`, `ServiceBuilder`, `ServiceBuilderExtensions`, or their legacy result types remain.
- No shared helper in `Bicep.Core.UnitTests` has consumers outside that project.
- `Bicep.Testing` contains one canonical implementation for each shared assertion, mock, feature fixture, and test-data builder.
- The full solution builds after each legacy type or file is deleted.

Validation for every package:

- Run touched classes by `FullyQualifiedName` filter.
- Build and test every consuming project changed by the package.
- Run `dotnet build .\Bicep.slnx --no-restore` at deletion milestones.
- Use zero-result searches for the retired symbols as completion checks.

### PR C: Remove Project References To Bicep.Core.UnitTests

Goal: no test project except `Bicep.Core.UnitTests` should reference `Bicep.Core.UnitTests` for shared helpers.

Use project references as migration gates:

1. Inventory the exact `Bicep.Core.UnitTests` symbols consumed by one project.
2. Replace local compilation and service setup with `TestCompiler` or `TestServices`.
3. Move only focused, broadly reusable capabilities into `Bicep.Testing`; keep one-off behavior local.
4. Remove the project reference and require a zero-result namespace search.
5. Build and test the consuming project before selecting the next project.

Prioritize by dependency shape:

- Start with low-fanout projects whose remaining dependencies are compilation, printing, or focused assertions.
- Treat `CompilationHelper` and `ServiceBuilder` in Core integration tests as one high-leverage compilation-fixture workstream.
- Decompose `RegistryHelper` and `ExtensionTestHelper` by capability: artifact publishing, fake registry transport, extension packages, and source archives.
- Split `BicepTestConstants`, `FileHelper`, `TestTypeHelper`, and assertions according to their actual consumers rather than moving their files wholesale.

Completed:

- `[x]` `Bicep.Decompiler.UnitTests`: migrated to `TestCompiler`, `TestPrinter`, and focused string/JSON assertions; all 52 tests passed.
- `[x]` `Bicep.Local.Extension.UnitTests`: migrated to `Bicep.Testing.Mocks.StrictMock` and focused JSON assertions; all 99 tests passed.
- `[x]` `Bicep.RegistryModuleTool.TestFixtures`: replaced default Core feature overrides with a focused assembly-version decorator; all 18 consuming integration tests passed.

Validation: the full solution builds after all three project-reference removals and assertion consolidation.

Current project-reference cleanup candidates:

- `src/Bicep.Cli.IntegrationTests/Bicep.Cli.IntegrationTests.csproj`
- `src/Bicep.Cli.UnitTests/Bicep.Cli.UnitTests.csproj`
- `src/Bicep.Core.IntegrationTests/Bicep.Core.IntegrationTests.csproj`
- `src/Bicep.Core.Samples/Bicep.Core.Samples.csproj`
- `src/Bicep.Decompiler.IntegrationTests/Bicep.Decompiler.IntegrationTests.csproj`
- `src/Bicep.LangServer.IntegrationTests/Bicep.LangServer.IntegrationTests.csproj`
- `src/Bicep.Local.Deploy.IntegrationTests/Bicep.Local.Deploy.IntegrationTests.csproj`
- `src/Bicep.McpServer.UnitTests/Bicep.McpServer.UnitTests.csproj`
- `src/Bicep.RpcClient.Tests/Bicep.RpcClient.Tests.csproj`
- `src/Bicep.Wasm.UnitTests/Bicep.Wasm.UnitTests.csproj`

Likely import cleanup areas:

- `using Bicep.Core.UnitTests;`
- `using Bicep.Core.UnitTests.Assertions;`
- `using Bicep.Core.UnitTests.Features;`
- `using Bicep.Core.UnitTests.Mock;`
- `using Bicep.Core.UnitTests.Utils;`

Guidance:

- Do this after each project's required helpers have been replaced, moved, consolidated, or deleted under Workstream B.
- Remove one consuming project reference per PR when the usage is broad.
- Validate the consuming test project, not just `Bicep.Core.UnitTests`.

### PR D: Core Integration URI Dictionaries

Goal: migrate `Uri`-keyed synthetic file dictionaries in Core integration tests to `IOUri`, `TestCompiler`, and `Bicep.Testing.IO`.

This is a focused execution package within Workstream B and should remove the affected `CompilationHelper`/`ServiceBuilder` usage rather than introduce adapters.

Candidates:

- `src/Bicep.Core.IntegrationTests/ModuleTests.cs`
- `src/Bicep.Core.IntegrationTests/ExtensibilityTests.cs`
- `src/Bicep.Core.IntegrationTests/Scenarios/TopLevelResourcePropertiesTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/DecoratorTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/OnlyIfNotExistsDecoratorTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/RetryOnDecoratorTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/WaitUntilDecoratorTests.cs`

Validation:

- Run touched classes by `FullyQualifiedName` filter.
- Inspect expected diagnostic file paths carefully.

### PR E: Incidental MockFileSystem Cleanup

Goal: remove direct `MockFileSystem` where it is only used to obtain an `IFileExplorer` or create compiler-visible files.

This is a focused execution package within Workstream B. Prefer existing `Bicep.Testing` file sets and fakes; do not move incidental `MockFileSystem` setup into a new shared helper.

Candidates:

- `src/Bicep.Core.UnitTests/Registry/OciArtifactMockedReferenceTests.cs`
- Incidental registry helper setup in `src/Bicep.Core.UnitTests/Utils/RegistryHelper.cs`
- Incidental file explorer setup in `src/Bicep.Core.UnitTests/Registry/OciModuleRegistryTests.cs`

Validation:

- Run touched registry tests by `FullyQualifiedName` filter.

Do not include:

- Docker credential tests.
- `Bicep.IO.UnitTests` file-system tests.
- CLI command tests that validate real file-system semantics through `System.IO.Abstractions`.

### PR F: LangServer Boundary Follow-Up

Goal: remove ambiguous `System.Uri` conversions at the LangServer/Core boundary.

Tasks:

- Mark `DocumentUriExtensions.ToUriEncoded` obsolete after direct call sites have a replacement path.
- Replace remaining intermediate `System.Uri` conversions where the target API can take `DocumentUri` directly.
- Keep `DocumentUri` in LSP-facing APIs.
- Keep `IOUri` in Core-facing APIs.

Validation:

- Run touched LangServer unit or integration test classes by `FullyQualifiedName` filter.

## Completion Criteria

- `CompilationHelper`, `ServiceBuilder`, their forwarding extensions, and their legacy result types are deleted.
- `TestCompiler`, `TestServices`, test file sets, and focused fixtures provide the remaining shared compiler and service setup.
- Every reusable assertion, feature fixture, mock, constant, and test-data builder has one canonical implementation in `Bicep.Testing`.
- Helpers left in `Bicep.Core.UnitTests` are Core-unit-test-specific and have no consumers outside that project.
- No non-Core-unit-test project references `Bicep.Core.UnitTests`.
- Remaining file-oriented `System.Uri` usage in tests is intentional and sits at a compatibility boundary.
- Remaining `DocumentUriExtensions.ToUriEncoded` usage is removed or isolated behind an explicitly named compatibility API.
- Remaining direct `MockFileSystem` usage is intentional and covers `IFileSystem` behavior.
