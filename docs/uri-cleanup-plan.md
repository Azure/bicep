# URI Cleanup Plan

This document tracks the migration away from file-oriented `System.Uri` and direct `MockFileSystem` usage in tests, plus the related LangServer URI-boundary cleanup.

Status legend:

- `[x]` Done
- `[~]` In progress
- `[ ]` Not started

## Goals

- Prefer `IOUri` over `System.Uri` for compiler, source graph, and test-file identifiers.
- Prefer `Bicep.Testing.IO` and `TestCompiler` for synthetic test files.
- Keep `DocumentUri` at LangServer/LSP boundaries and `IOUri` at `Bicep.Core` boundaries. Convert only when crossing those layers.
- Move broadly shared test helpers into `Bicep.Testing` over time instead of growing `Bicep.Core.UnitTests.Utils`.
- Keep each PR small enough to review and validate independently.

## Non-Goals

- Do not replace `System.Uri` values that represent HTTP, registry, documentation, or other non-file URIs.
- Do not replace LSP `DocumentUri` values inside LangServer/LSP-facing behavior.
- Do not migrate tests that intentionally exercise `System.IO.Abstractions` behavior, such as current directory handling, globbing, file writes, or the `FileSystemFileExplorer` implementation.
- Do not move large shared helpers wholesale. Split reusable compiler/test-service behavior from Core-unit-test-specific assertion and mock helpers.

## Progress

- `[x]` LangServer refresh APIs use `DocumentUri` instead of round-tripping through `System.Uri`.
  - Commit: `9f6498394 Use DocumentUri for LangServer refreshes`
  - Files covered:
    - `src/Bicep.LangServer/CompilationManager/ICompilationManager.cs`
    - `src/Bicep.LangServer/BicepCompilationManager.cs`
    - `src/Bicep.LangServer/Handlers/BicepDefinitionHandler.cs`
    - `src/Bicep.LangServer/Handlers/BicepForceModulesRestoreCommandHandler.cs`
- `[~]` Add `TestCompiler` APIs for compiler tests that need raw `Compilation` objects.
  - Current work:
    - `src/Bicep.Testing/Utils/TestCompiler.cs`
    - `src/Bicep.Testing/Utils/TestCompilerExtensions.cs`
    - `src/Bicep.Testing/Utils/TestServices.cs`
- `[~]` Migrate first Core unit tests from `System.Uri`/`ServiceBuilder` setup to `TestCompiler.ForInMemoryCompilation()`.
  - Current work:
    - `src/Bicep.Core.UnitTests/Semantics/BaseParametersSymbolTests.cs`
    - `src/Bicep.Core.UnitTests/Diagnostics/LinterRuleTests/SecureParamsInParametersFileRuleTests.cs`
- `[~]` Make direct workspace helpers accept `IOUri` without `IOUri -> Uri -> IOUri` round-trips.
  - Current work:
    - `src/Bicep.Core.UnitTests/Utils/CompilationHelper.cs`
    - `src/Bicep.Core.UnitTests/Utils/ServiceBuilderExtensions.cs`
- `[ ]` Migrate remaining easy Core unit test call sites.
- `[ ]` Migrate Core integration test URI dictionaries.
- `[ ]` Migrate incidental `MockFileSystem` usage that only creates compiler-visible files.
- `[ ]` Deprecate and remove `DocumentUriExtensions.ToUriEncoded` call sites.

## Current PR Scope

The current PR should stay focused on the first test migration slice and the minimum shared helper support needed for that slice.

Included:

- Add lower-level `TestCompiler.CreateCompilation(...)` and `CreateCompilationWithoutRestore(...)` APIs for tests that need `Compilation` directly or use params-file entry points.
- Add fluent `TestCompiler` extension methods for common service customization:
  - `WithConfiguration(...)`
  - `WithAzResources(...)`
  - `WithEmptyAzResources()`
- Use `TestServices.RemoveAll<T>()` before adding replacement singleton registrations in override-style helpers.
- Migrate `BaseParametersSymbolTests` to `TestCompiler.ForInMemoryCompilation().WithEmptyAzResources()`.
- Migrate `SecureParamsInParametersFileRuleTests` to `TestCompiler.ForInMemoryCompilation().WithConfiguration(...).WithEmptyAzResources()`.

Excluded:

- Moving `CompilationHelper` or `ServiceBuilder` wholesale into `Bicep.Testing`.
- Migrating restore-based helper call sites such as `BuildCompilationWithRestore`.
- Migrating CLI tests where `MockFileSystem` is part of the behavior under test.
- Migrating `Bicep.IO.UnitTests` file-system tests.

Validation for this PR:

- `dotnet test .\src\Bicep.Core.UnitTests\Bicep.Core.UnitTests.csproj --no-restore -- --filter "FullyQualifiedName~BaseParametersSymbolTests|FullyQualifiedName~SecureParamsInParametersFileRuleTests|FullyQualifiedName~PositionTrackingJsonTextWriterTests"`

Latest result: 15 passed.

## Next PR Candidates

### Core Unit Tests: Easy Compiler Setup

Use `TestCompiler.ForInMemoryCompilation()` where tests only need synthetic compiler-visible files.

Candidates:

- `src/Bicep.Core.UnitTests/Diagnostics/LinterRuleTests/StacksExtensibilityCompatibilityRuleTests.cs`
- `src/Bicep.Core.UnitTests/Emit/ExpressionConverterTests.cs`
- `src/Bicep.Core.UnitTests/Emit/InlineDependencyVisitorTests.cs`
- `src/Bicep.Core.UnitTests/Emit/PositionTrackingJsonTextWriterTests.cs`
- `src/Bicep.Core.UnitTests/Semantics/SymbolContextTests.cs`
- `src/Bicep.Core.UnitTests/SourceGraph/BicepFileTests.cs`

Guidance:

- Prefer `TestCompiler` over new `ServiceBuilder` overloads.
- Use raw `Compilation` APIs for params-file entry points or tests that should not emit a template.
- Keep `IOUri` constants only where assertions need stable file identifiers.

### Shared Helper Bridge

After more call sites move to `TestCompiler`, bridge or retire Core-unit-test helper APIs that duplicate `Bicep.Testing` behavior.

Candidates:

- `src/Bicep.Core.UnitTests/Utils/CompilationHelper.cs`
- `src/Bicep.Core.UnitTests/Utils/ServiceBuilderExtensions.cs`

Guidance:

- Keep compatibility wrappers while integration and CLI tests still depend on `Bicep.Core.UnitTests.Utils`.
- Move reusable helper behavior into `Bicep.Testing`; leave Core-specific assertions and mocks local.

### Core Integration Tests

Migrate `Uri`-keyed file dictionaries and synthetic file URIs to `IOUri`, `TestCompiler`, and `Bicep.Testing.IO`.

Candidates:

- `src/Bicep.Core.IntegrationTests/ModuleTests.cs`
- `src/Bicep.Core.IntegrationTests/ExtensibilityTests.cs`
- `src/Bicep.Core.IntegrationTests/Scenarios/TopLevelResourcePropertiesTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/DecoratorTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/OnlyIfNotExistsDecoratorTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/RetryOnDecoratorTests.cs`
- `src/Bicep.Core.IntegrationTests/Decorators/WaitUntilDecoratorTests.cs`

Validation:

- Run targeted Core integration filters for touched classes.
- Inspect diagnostics carefully if expected file paths change.

### Incidental MockFileSystem Usage

Replace direct `MockFileSystem` usage only when it is a way to obtain an `IFileExplorer` or create compiler-visible files.

Candidates:

- `src/Bicep.Core.UnitTests/Registry/OciArtifactMockedReferenceTests.cs`
- Incidental registry helper setup in `src/Bicep.Core.UnitTests/Utils/RegistryHelper.cs`
- Incidental file explorer setup in `src/Bicep.Core.UnitTests/Registry/OciModuleRegistryTests.cs`

Keep direct `MockFileSystem` where the product API requires `IFileSystem` or the test validates file-system behavior. Examples include Docker credential tests and `Bicep.IO.UnitTests` file-system tests.

### LangServer Boundary Follow-Up

Keep LangServer-facing APIs typed as `DocumentUri` when they represent LSP documents or file-change notifications.

Follow-up:

- Mark `DocumentUriExtensions.ToUriEncoded` as obsolete once direct call sites have a replacement path.
- Replace remaining intermediate `System.Uri` conversions where the target API can take `DocumentUri` directly.
- Convert `DocumentUri` to `IOUri` only when calling into `Bicep.Core`.
- Convert `IOUri` back to `DocumentUri` only when returning Core file identifiers to LangServer/LSP APIs.

## Completion Criteria

- Shared compiler/test-file helper APIs live in `Bicep.Testing`.
- `Bicep.Core.UnitTests.Utils` no longer contains broadly shared IO or compilation helpers.
- Remaining file-oriented `System.Uri` usage in tests is intentional and sits at a compatibility boundary.
- Remaining `DocumentUriExtensions.ToUriEncoded` usage is removed or isolated behind an explicitly named compatibility API.
- Remaining direct `MockFileSystem` usage is intentional and covers `IFileSystem` behavior.