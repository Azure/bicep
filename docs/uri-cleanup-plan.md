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
var result = await TestCompiler
    .ForInMemoryCompilation()
    .WithEmptyAzResources()
    .CompileWithoutRestore(bicepText);

var compilation = result.Compilation;
```

Use `result.Template` and `result.Diagnostics` when needed. They are lazy, so params-file tests can safely use the same result type without forcing template emission.

### Params-File Entry Points

Use `CompileWithoutRestore(entryPointPath, files)` and then inspect `result.Compilation`:

```csharp
var result = await TestCompiler
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
var result = await TestCompiler
    .ForInMemoryCompilation()
    .WithConfiguration(BicepTestConstants.BuiltInConfigurationWithStableAnalyzers)
    .WithEmptyAzResources()
    .CompileWithoutRestore(bicepText);
```

```csharp
var result = await TestCompiler
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

- `[x]` LangServer refresh APIs use `DocumentUri` instead of round-tripping through `System.Uri`.
  - Commit: `9f6498394 Use DocumentUri for LangServer refreshes`
  - Files covered:
    - `src/Bicep.LangServer/CompilationManager/ICompilationManager.cs`
    - `src/Bicep.LangServer/BicepCompilationManager.cs`
    - `src/Bicep.LangServer/Handlers/BicepDefinitionHandler.cs`
    - `src/Bicep.LangServer/Handlers/BicepForceModulesRestoreCommandHandler.cs`
- `[x]` `TestCompiler` consistently returns `TestCompilationResult`, with lazy `Template` and `Diagnostics`.
  - Files covered:
    - `src/Bicep.Testing/Utils/TestCompiler.cs`
    - `src/Bicep.Testing/Utils/TestCompilerExtensions.cs`
    - `src/Bicep.Testing/Utils/TestServices.cs`
    - `src/Bicep.Testing/Utils/TestCompilationResult.cs`
- `[x]` Initial Core unit-test migration to `TestCompiler.ForInMemoryCompilation()`.
  - Files covered:
    - `src/Bicep.Core.UnitTests/Semantics/BaseParametersSymbolTests.cs`
    - `src/Bicep.Core.UnitTests/Diagnostics/LinterRuleTests/SecureParamsInParametersFileRuleTests.cs`
    - `src/Bicep.Core.UnitTests/Emit/ExpressionConverterTests.cs`
    - `src/Bicep.Core.UnitTests/Emit/InlineDependencyVisitorTests.cs`
    - `src/Bicep.Core.UnitTests/Emit/PositionTrackingJsonTextWriterTests.cs`
    - `src/Bicep.Core.UnitTests/Semantics/SymbolContextTests.cs`
    - `src/Bicep.Core.UnitTests/SourceGraph/BicepFileTests.cs`
    - `src/Bicep.Core.UnitTests/TypeSystem/Az/AzResourceTypeProviderTests.cs`
- `[x]` Direct workspace helpers accept `IOUri` without `IOUri -> Uri -> IOUri` round-trips.
  - Files covered:
    - `src/Bicep.Core.UnitTests/Utils/CompilationHelper.cs`
    - `src/Bicep.Core.UnitTests/Utils/ServiceBuilderExtensions.cs`
- `[ ]` Remaining Core unit-test call sites.
- `[ ]` Move shared assertions, constants, mocks, and helper APIs out of `Bicep.Core.UnitTests` into `Bicep.Testing`.
- `[ ]` Remove non-Core-unit-test project references to `Bicep.Core.UnitTests`.
- `[ ]` Core integration test URI dictionaries.
- `[ ]` Incidental `MockFileSystem` usage that only creates compiler-visible files.
- `[ ]` `DocumentUriExtensions.ToUriEncoded` deprecation and removal.

## Current PR Baseline

Current PR scope:

- Add lazy `TestCompilationResult` properties.
- Add `TestCompiler.Compile(...)` and `CompileWithoutRestore(...)` source-text overloads.
- Add fluent `TestCompiler` service customization extensions.
- Replace first Core unit-test slice with `TestCompiler.ForInMemoryCompilation()`.
- Keep restore-based, params-emission, linter-infrastructure, CLI, and file-system behavior tests for later PRs.

Validation already run:

```powershell
dotnet test .\src\Bicep.Core.UnitTests\Bicep.Core.UnitTests.csproj --no-restore -- --filter "FullyQualifiedName~BaseParametersSymbolTests|FullyQualifiedName~SecureParamsInParametersFileRuleTests|FullyQualifiedName~PositionTrackingJsonTextWriterTests|FullyQualifiedName~SymbolContextTests|FullyQualifiedName~BicepFileTests|FullyQualifiedName~ExpressionConverterTests|FullyQualifiedName~InlineDependencyVisitorTests"
```

Latest result: 76 passed.

## Work Queue

### PR A: Remaining Core Unit Test Compiler Setup

Goal: migrate remaining unit tests that still use `ServiceBuilder.BuildCompilation` or `CompilationHelper` only for synthetic compiler setup.

Candidates:

- `src/Bicep.Core.UnitTests/Diagnostics/LinterRuleTests/StacksExtensibilityCompatibilityRuleTests.cs`
- `src/Bicep.Core.UnitTests/Diagnostics/LinterRuleTests/NoHardcodedLocationRuleTests.cs`
- `src/Bicep.Core.UnitTests/Diagnostics/LinterRuleTests/NoLocationExprOutsideParamsRuleTests.cs`
- `src/Bicep.Core.UnitTests/Emit/ParameterAssignmentEvaluatorTests.cs`

Validation:

- Run touched classes by `FullyQualifiedName` filter.
- If linter helper infrastructure changes, run the affected linter rule test class and one neighboring linter rule class.

Notes:

- Treat params-emission tests as their own small slice if they require new `TestCompilationResult` support.
- Treat linter-helper refactors as their own slice if they affect shared linter assertion behavior.

### PR B: Shared Helper Ownership

Goal: move shared helper ownership from `Bicep.Core.UnitTests` to `Bicep.Testing` so other test projects stop depending on `Bicep.Core.UnitTests`.

Candidates:

- `src/Bicep.Core.UnitTests/Utils/CompilationHelper.cs`
- `src/Bicep.Core.UnitTests/Utils/ServiceBuilderExtensions.cs`
- `src/Bicep.Core.UnitTests/BicepTestConstants.cs`
- `src/Bicep.Core.UnitTests/Assertions/*`
- `src/Bicep.Core.UnitTests/Features/*`
- `src/Bicep.Core.UnitTests/Mock/*`
- `src/Bicep.Core.UnitTests/Utils/FileHelper.cs`
- `src/Bicep.Core.UnitTests/Utils/RegistryHelper.cs`
- `src/Bicep.Core.UnitTests/Utils/TestTypeHelper.cs`

Guidance:

- Move reusable compiler/test-file behavior, assertions, constants, mocks, and test data builders into `Bicep.Testing`.
- Leave only tests and Core-unit-test-only fixtures in `Bicep.Core.UnitTests`.
- Prefer namespace moves that make consumers use `Bicep.Testing.*` directly.
- Keep temporary compatibility wrappers only when needed to keep a PR small, and track their removal in this document.

Validation:

- Run the Core unit tests touched in PR A plus representative integration tests that still call `CompilationHelper`.

### PR C: Remove Project References To Bicep.Core.UnitTests

Goal: no test project except `Bicep.Core.UnitTests` should reference `Bicep.Core.UnitTests` for shared helpers.

Current known project-reference cleanup candidates:

- `src/Bicep.Cli.IntegrationTests/Bicep.Cli.IntegrationTests.csproj`
- `src/Bicep.Cli.UnitTests/Bicep.Cli.UnitTests.csproj`
- `src/Bicep.Core.IntegrationTests/Bicep.Core.IntegrationTests.csproj`
- `src/Bicep.Core.Samples/Bicep.Core.Samples.csproj`

Likely import cleanup areas:

- `using Bicep.Core.UnitTests;`
- `using Bicep.Core.UnitTests.Assertions;`
- `using Bicep.Core.UnitTests.Features;`
- `using Bicep.Core.UnitTests.Mock;`
- `using Bicep.Core.UnitTests.Utils;`

Guidance:

- Do this after the helpers those projects need have moved to `Bicep.Testing`.
- Remove one consuming project reference per PR when the usage is broad.
- Validate the consuming test project, not just `Bicep.Core.UnitTests`.

### PR D: Core Integration URI Dictionaries

Goal: migrate `Uri`-keyed synthetic file dictionaries in Core integration tests to `IOUri`, `TestCompiler`, and `Bicep.Testing.IO`.

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

- Shared compiler/test-file helper APIs live in `Bicep.Testing`.
- Shared assertions, constants, mocks, and helper APIs live in `Bicep.Testing`.
- No non-Core-unit-test project references `Bicep.Core.UnitTests`.
- `Bicep.Core.UnitTests.Utils` no longer contains broadly shared IO or compilation helpers.
- Remaining file-oriented `System.Uri` usage in tests is intentional and sits at a compatibility boundary.
- Remaining `DocumentUriExtensions.ToUriEncoded` usage is removed or isolated behind an explicitly named compatibility API.
- Remaining direct `MockFileSystem` usage is intentional and covers `IFileSystem` behavior.
