# Bicep Project Guidelines

## Architecture

Bicep is a DSL for Azure ARM templates. The compiler pipeline is: **Lexer → Parser → Binder → Type Checking / Linting → Emitter** (ARM JSON).

| Project | Role |
|---------|------|
| `Bicep.Core` | Compiler library (parser, binder, type system, emitter, linter) |
| `Bicep.Cli` | CLI (`bicep build`, `bicep decompile`, etc.) |
| `Bicep.LangServer` | OmniSharp-based LSP server |
| `Bicep.McpServer` | Model Context Protocol server for AI tooling |
| `Bicep.Decompiler` | ARM JSON → Bicep decompilation |
| `vscode-bicep` | VS Code extension (TypeScript) |

Key classes: `Compilation` → `SemanticModel` → `TypeManager`, `Binder`, `TemplateEmitter`. All use lazy evaluation and caching—the language server recreates state per keystroke, so only compute what's requested.

## Code Style

- **C#**: 4-space indent, braces always required, nullable enabled, `TreatWarningsAsErrors=true`
- **File header required**: `// Copyright (c) Microsoft Corporation.` + `// Licensed under the MIT License.`
- **Null-forgiving `!` operator is banned** in product code (allowed in tests)
- **`Console.Write`/`Console.WriteLine` are banned**—use proper logging
- Syntax trees and data structures are **immutable by default**
- Prefer **collection expressions** (`[a, b]`) over `.ToList()` / `new List<T>{}`
- Line endings: `LF` (configured in `.editorconfig`)
- TypeScript/JSON: 2-space indent

## Build and Test

```bash
# Build
dotnet build Bicep.sln

# Run all tests
dotnet test

# Build VS Code extension (requires Node.js 20+)
cd src/vscode-bicep && npm ci && npm run build

# Build vscode-bicep-ui
cd src/vscode-bicep-ui && npm ci && npm run build
```

.NET SDK version is pinned in `global.json` (currently .NET 10). Use VS Code tasks (defined in `.vscode/tasks.json`) for common operations.

## Test Conventions

- **Framework**: MSTest (`[TestClass]`, `[TestMethod]`), FluentAssertions for assertions, Moq for mocking
- **Naming**: `Bicep.Foo.UnitTests`, `Bicep.Foo.IntegrationTests`, `Bicep.Foo.E2eTests`
- **DI in tests**: Use `ServiceBuilder` for composable test setup: `new ServiceBuilder().WithRegistration(...).Build()`
- **Jest** for VS Code extension tests
- **Vitest** for vscode-bicep-ui tests

### Baseline Tests (Critical Pattern)

Integration test datasets live in `src/Bicep.Core.Samples/Files/baselines/`. Each dataset has `main.bicep` plus assertion files (`.diagnostics`, `.formatted`, `.symbols`, `.syntax`, `.tokens`, `.ir`, `main.json`, etc.).

- **Update baselines**: run VS Code task "Update Baselines" or `dotnet test -- --filter TestCategory=Baseline --test-parameter SetBaseLine=true`
- **Always inspect baseline diffs** before committing—unexplained changes block PRs
- Folders prefixed with `Invalid` expect compilation failure

## Key Patterns

### Diagnostics

Diagnostics are defined centrally in `DiagnosticBuilder.DiagnosticBuilderInternal`. Pattern:
```csharp
DiagnosticBuilder.ForPosition(span).SomeError(args)
```

### Visitor Pattern

Layered hierarchy: `SyntaxVisitor` → `CstVisitor` (tokens/trivia) → `AstVisitor` (semantic nodes) → `SyntaxRewriteVisitor` (immutable transforms). Extend `AstVisitor` for most analysis tasks.

### Linter Rules

Extend `LinterRuleBase` in `Bicep.Core/Analyzers/Linter/Rules/`. Rules are configurable via `bicepconfig.json`.

### Dependency Injection

Core services registered via `AddBicepCore()` extension method. MCP server uses `AddBicepMcpServer()`. MCP tools use `[McpServerToolType]`/`[McpServerTool]` attributes.

## Project Conventions

- Link issues in PRs (`Fixes #NNN`)
- Design proposals required for significant changes
- Snippet templates live in `src/Bicep.LangServer/Files/SnippetTemplates/`
- Central package management enabled (`src/Directory.Packages.props`)—add package versions there, not in individual `.csproj` files
