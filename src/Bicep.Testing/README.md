# Bicep.Testing

`Bicep.Testing` contains shared infrastructure for Bicep tests. Use it instead of referencing `Bicep.Core.UnitTests` for compiler setup, synthetic files, assertions, fakes, mocks, or test data.

Typical imports are:

```csharp
using Bicep.Testing;
using Bicep.Testing.Assertions;
```

## Choose The Right API

| Need | API |
| --- | --- |
| Compile synthetic Bicep or params files | `TestCompiler.ForInMemoryCompilation()` |
| Validate `IFileSystem` behavior | `TestCompiler.ForMockFileSystemCompilation()` |
| Compile without artifact restore | `CompileWithoutRestore(...)` |
| Restore modules or extensions before compiling | `Compile(...)` |
| Publish fake registry artifacts | `TestExternalArtifactManager` |
| Use a standard configuration | `TestConfigurations` |
| Build a custom configuration | `TestConfigurationBuilder` |
| Customize compiler services | `TestCompiler.ConfigureServices(...)` or a focused `With...` extension |
| Assert a stable synthetic file identifier | `TestFileUri` |
| Control environment variables and current directory | `Bicep.Testing.Fakes.FakeEnvironment` |
| Parse syntax without compiling | `TestParser` |
| Decompile templates or parameters | `TestDecompiler` |

## Compiler Recipes

### Single file without restore

`CompileWithoutRestore(...)` is synchronous.

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .WithEmptyAzResources()
    .CompileWithoutRestore("""
        param name string
        output name string = name
        """);

result.Should().NotHaveAnyDiagnostics();
var compilation = result.Compilation;
```

### Multiple files

The default entry point is `main.bicep`.

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .CompileWithoutRestore(
        ("main.bicep", """
            module child 'child.bicep' = {}
            """),
        ("child.bicep", """
            output value string = 'test'
            """));
```

Pass an explicit entry point for params files or a differently named main file.

```csharp
var result = TestCompiler
    .ForInMemoryCompilation()
    .CompileWithoutRestore(
        "main.bicepparam",
        ("main.bicep", "param count int"),
        ("main.bicepparam", """
            using 'main.bicep'
            param count = 3
            """));

result.Parameters.Should().NotBeNull();
```

### Restore registry artifacts

```csharp
var compiler = TestCompiler.ForInMemoryCompilation();
var artifacts = new TestExternalArtifactManager(compiler);
await artifacts.PublishExtension(extensionData);

var result = await compiler.Compile(
    "main.bicep",
    ("main.bicep", "extension 'br:example.azurecr.io/extensions/foo:1.0.0'"));
```

Use `Compile(...)` only when restore is required.

## Configuration

Use a named preset when one matches the test:

```csharp
var compiler = TestCompiler
    .ForInMemoryCompilation()
    .WithConfiguration(TestConfigurations.BuiltInWithStableAnalyzers);
```

Use the builder for one-off mutations:

```csharp
var configuration = TestConfigurationBuilder
    .Create(TestConfigurations.BuiltInWithStableAnalyzers)
    .WithAnalyzer("no-unused-params", DiagnosticLevel.Off)
    .WithExperimentalFeaturesEnabled(features)
    .Build();
```

Do not add test-only `With...Configuration` extension methods to `Bicep.Core`.

## Results And Assertions

`TestCompilationResult` exposes:

- `Compilation`
- `EntryPointFile`
- `Diagnostics`
- `Template` for Bicep entry points
- `Parameters` for params entry points

Template, parameters, and diagnostics are lazy. Prefer assertions from `Bicep.Testing` and `Bicep.Testing.Assertions` so failures include source annotations.

## Service Customization

Prefer focused `TestCompiler` extensions such as `WithConfiguration`, `WithAzResources`, and `WithAzResourceTypeLoader`. For a test-specific service replacement:

```csharp
var compiler = TestCompiler
    .ForInMemoryCompilation()
    .ConfigureServices(services => services.ReplaceSingleton<IMyService>(fakeService));
```

Use `ReplaceSingleton` when overriding an existing registration. Use `AddSingleton` only for a service that is not already registered.

## Namespace And Naming Conventions

- Public `Test*` toolkit types live in the `Bicep.Testing` root namespace.
- `Fake*`, `Mock*`, and `Dummy*` implementations live under `Fakes`, `Mocks`, and `Dummies`.
- Assertion infrastructure that is not itself a public `Test*` type lives under `Assertions`.
- In-memory and mock-file-system implementations that back `TestFileSet` live under `IO`.
- Do not create a catch-all `Bicep.Testing.Utils` namespace.

## Migration Rules

When migrating existing tests:

- Replace `CompilationHelper.Compile(...)` with `TestCompiler.CompileWithoutRestore(...)`.
- Replace `CompilationHelper.RestoreAndCompile(...)` with `await TestCompiler.Compile(...)`.
- Replace `CompilationHelper.InputFile` with `(string FilePath, TestFileData FileData)` tuples.
- Replace `CompilationHelper.CompilationResult` and `ParamsCompilationResult` with `TestCompilationResult`.
- Replace compilation-oriented `ServiceBuilder` setup with `TestCompiler` configuration.
- Keep direct file-system setup only when the test intentionally validates file-system behavior.
- Do not introduce compatibility wrappers that preserve legacy helper shapes.

Validate the touched test classes and build every changed consuming project. At shared API or deletion milestones, build the full solution.
