# Docs Generation Command Proposal

## Summary

Add a new `docs` command group to the Bicep CLI, with two initial subcommands:

```sh
bicep docs generate
bicep docs output
```

The first implementation should generate Markdown documentation for Bicep modules, defaulting to an AVM-compatible `README.md` layout and allowing callers to supply a custom template. `bicep docs generate` should be the file-oriented command, while `bicep docs output` should render the same content to stdout as a string-oriented command and should not support recursive operation. The implementation should be deterministic, fast for bulk generation, and reusable through the existing JSON-RPC surface so that many modules can be processed in a single session without repeated process startup overhead. The core documentation generation engine should also be packaged as a separate NuGet package so module developers can consume the same functionality directly from .NET tooling without shelling out to the CLI.

## Goals

1. Add a first-class CLI experience for module documentation generation under a dedicated command group.
2. Default to Markdown output, with a built-in template that matches the section ordering and general formatting used by the AVM `Set-ModuleReadMe.ps1` script.
3. Allow callers to provide a custom template using a familiar, well-documented templating syntax.
4. Support generating docs for one module or many modules in one invocation, including glob-based bulk discovery via `--pattern` (matching the existing `bicep build`, `bicep format`, `bicep lint`, `bicep restore`, and `bicep build-params` convention).
5. Make bulk generation efficient by reusing compilation state and exposing the same capability over JSON-RPC for persistent sessions.
6. Add golden-file tests based on representative example repositories so that generated Markdown can be compared directly with checked-in README files.
7. Package the reusable documentation generation engine as a separate NuGet package that can be consumed directly by module developers and other .NET tooling.
8. Keep the implementation aligned with existing Bicep quality standards: clear command/help text, deterministic output, localized CLI messages, nullable-safe C#, and minimal duplication of existing compilation logic.

## Non-goals

1. Supporting non-Markdown output formats in the initial release.
2. Reproducing every AVM pipeline concern that depends on remote calls, repository-wide caching, or external release metadata.
3. Designing a generic site generator or static documentation platform.
4. Changing the existing `build`, `lint`, `format`, or `jsonrpc` behavior beyond what is required to add the new docs surface.

## Why A Command Group

The current CLI is single-verb (`bicep build`, `bicep lint`, `bicep snapshot`, etc.). Documentation generation is likely to grow beyond one operation, so it should start as a grouped verb:

```sh
bicep docs generate
```

This leaves room for later commands such as:

```sh
bicep docs validate
bicep docs init-template
```

This does require a small evolution of the command parser and help text because `docs` becomes the first grouped verb in the CLI.

## Proposed CLI Shape

### File-oriented syntax

```sh
bicep docs generate [<path>] [options]
```

`<path>` may be:

1. A `.bicep` file.
2. A directory containing a module entrypoint.

If `<path>` is omitted, the current directory is used.

### String-oriented syntax

```sh
bicep docs output [<path>] [options]
```

`<path>` may be:

1. A `.bicep` file.
2. A directory containing a module entrypoint.

If `<path>` is omitted, the current directory is used.

`bicep docs output` should resolve exactly one module and write the rendered documentation to stdout. It should not support `--pattern`.

### `generate` options

| Option | Description |
| :-- | :-- |
| `--preset <name>` | Built-in template preset selector. Defaults to `markdown`. The only supported preset in v1 is `markdown`. When `--template-file` is provided, rendering is determined by that template and `--preset` is validation/default-selection metadata only. |
| `--template-file <path>` | Path to a custom template file. If omitted, the built-in default Markdown template is used. |
| `--template-root <path>` | Optional override for include resolution root. By default, include paths are resolved relative to the module being processed. |
| `--set <key=value>` | Supplies a custom string value that is exposed to the template model for use by custom Scriban templates. May be specified multiple times. |
| `--output-file <name>` | Output file name to write in each module directory. Defaults to `README.md` when `--preset markdown` is used. |
| `--pattern <glob>` | Generates docs for all files matching the specified glob pattern (for example `./modules/**/main.bicep`). Mutually exclusive with the positional `<path>` argument. Matches the convention used by `bicep build --pattern`. |
| `--no-restore` | Skips restore when compiling modules for docs generation. |
| `--diagnostics-format <format>` | Reuses existing diagnostics formatting behavior for emitted compiler diagnostics. |

### `output` options

| Option | Description |
| :-- | :-- |
| `--preset <name>` | Built-in template preset selector. Defaults to `markdown`. The only supported preset in v1 is `markdown`. When `--template-file` is provided, rendering is determined by that template and `--preset` is validation/default-selection metadata only. |
| `--template-file <path>` | Path to a custom template file. If omitted, the built-in default Markdown template is used. |
| `--template-root <path>` | Optional override for include resolution root. By default, include paths are resolved relative to the module being processed. |
| `--set <key=value>` | Supplies a custom string value that is exposed to the template model for use by custom Scriban templates. May be specified multiple times. |
| `--no-restore` | Skips restore when compiling modules for docs generation. |
| `--diagnostics-format <format>` | Reuses existing diagnostics formatting behavior for emitted compiler diagnostics. |

### Examples

```sh
# Generate README.md for the module in the current directory
bicep docs generate

# Generate README.md for a specific module
bicep docs generate ./modules/storage/main.bicep

# Generate README.md for all modules discovered by a glob
bicep docs generate --pattern './modules/**/main.bicep'

# Use a custom template
bicep docs generate ./modules/storage/main.bicep --template-file ./templates/readme.md

# Pass custom strings into the template
bicep docs generate ./modules/storage/main.bicep --template-file ./templates/readme.md --set ownerDisplayName="Platform Team" --set supportUrl="https://contoso.example/support"

# Write a different file name in each module directory
bicep docs generate --pattern './modules/**/main.bicep' --output-file MODULE.md

# Print a single generated document to stdout
bicep docs output ./modules/storage/main.bicep

# Print the current directory module docs using a custom template
bicep docs output --template-file ./templates/readme.md
```

## Discovery Rules

To keep v1 simple, predictable, and aligned with the existing `bicep build`, `bicep format`, `bicep lint`, `bicep restore`, and `bicep build-params` commands:

1. If the positional `<path>` is a file, process that file.
2. If the positional `<path>` is a directory, look for `<dir>/main.bicep`.
3. If `--pattern` is supplied, `bicep docs generate` enumerates all files matching the glob (for example `./modules/**/main.bicep`) and processes each as a module entrypoint. The positional `<path>` argument and `--pattern` are mutually exclusive.
4. `bicep docs output` does not support `--pattern` and must resolve exactly one module.
5. `--output-file` is treated as a file name, not a destination directory. When `--pattern` is used, the file is written next to each matched module entrypoint.

This intentionally avoids introducing a separate `--recursive` flag; recursion is expressed by the glob itself, consistent with the rest of the Bicep CLI surface.

### Why `main.bicep` instead of `*.bicep`

The recommended pattern in examples is `**/main.bicep` rather than `**/*.bicep` because documentation generation is module-entrypoint-oriented, not file-oriented. This differs from `bicep build`, where every `.bicep` file is a valid compilation target.

By Bicep and AVM convention, the entrypoint of a consumable module is `main.bicep` at the module root. Other `.bicep` files in the tree are typically implementation details that should not produce their own `README.md`:

1. Nested or helper modules referenced by a parent `main.bicep`.
2. Test files such as `tests/e2e/defaults/main.test.bicep`.
3. Re-usable type or function files imported by the entrypoint.

Generating docs for those files would write meaningless `README.md` documents into test and helper folders and overwrite anything already there. Matching only `main.bicep` produces docs exactly for the directories that are intended to be consumed as modules, and lines up with the directory-default rule above and with the AVM `Set-ModuleReadMe.ps1` behavior.

Callers who follow a different convention can still supply any glob they like (for example `**/module.bicep` or `**/*.bicep`); the examples reflect the recommended default rather than a hard constraint.

## Default Output Behavior

The initial default preset is Markdown.

When `--preset` is omitted:

1. Both subcommands use the built-in Markdown template.
2. `bicep docs generate` writes `README.md` by default.
3. `bicep docs output` writes the rendered document to stdout and performs no filesystem writes.
4. Existing files are overwritten only after successful generation for that module.

When `--template-file` is provided:

1. The custom Scriban template defines rendering behavior.
2. `--preset` does not alter template rendering in v1 and is treated as validation/default-selection metadata.

## AVM Compatibility Scope

The built-in template should aim for compatibility with the structure produced by the AVM `Set-ModuleReadMe.ps1` script, while staying deterministic and local-first.

The v1 built-in template should support these sections when data is available:

1. Title and description.
2. Optional module reference block.
3. Navigation.
4. Resource types.
5. Usage examples.
6. Parameters.
7. Exported functions.
8. Outputs.
9. Cross-referenced modules.
10. Data collection note when applicable.

The generated Markdown does not need to replicate every AVM implementation detail exactly. In particular, v1 should avoid live URL validation and other network-dependent logic. Where the AVM script validates links dynamically, the CLI should generate stable URLs from known patterns instead.

## Templating

### Proposed syntax and package

Use `Scriban` as the templating engine. See the [Appendix](#appendix) for a comparison of considered options and a more detailed rationale for the choice.

Reasons:

1. It is a well-known .NET templating package.
2. Its syntax is familiar to users of Liquid-like templating systems and simple enough for README generation.
3. It supports loops, conditionals, includes, whitespace control, and strict parsing.
4. It avoids building a custom templating language.

Example:

```scriban
# {{ module.name }}

{{ module.description }}

## Parameters
{{ for parameter in module.parameters }}
- `{{ parameter.name }}`: {{ parameter.description }}
{{ end }}
```

### Built-in template

The CLI should embed a built-in default template so that `bicep docs generate` works without any template file. The built-in template does not need to support add-in files or complex composition. It can remain intentionally simple.

### Custom templates

If `--template-file` is supplied:

1. The file is parsed as Scriban.
2. Template parse errors fail the command with file, line, and column information.
3. The same strongly shaped data model used by the built-in template is exposed to the custom template.

### Rendering responsibility split

To keep behavior predictable and maintainable, the implementation should explicitly separate semantic shaping from markdown rendering:

1. Template responsibility: markdown presentation concerns such as section order, headings, tables, code fences, list styles, and optional include composition.
2. Engine/model responsibility: semantic transformations such as parameter categorization, stable sorting, recursive type expansion, example normalization, and other logic that interprets compiled Bicep meaning.
3. Validation and policy responsibility: non-rendering checks should stay in engine code, not template files.

In short, markdown-specific transforms should be handled in the template, while semantics-specific transforms should be computed into the model before rendering.

### Custom string values

Custom templates should be able to consume caller-supplied string values passed on the CLI.

This is intended for values that are not part of the compiled module metadata but still need to appear in the rendered document, such as:

1. Display names for owning teams.
2. Support links.
3. Internal compliance identifiers.
4. Environment or product labels.

The recommended CLI shape is a repeatable key/value option:

```sh
--set key=value
```

Example:

```sh
bicep docs output ./main.bicep --template-file ./templates/readme.md --set ownerDisplayName="Platform Team" --set supportUrl="https://contoso.example/support"
```

Those values should be exposed to the template model through a dedicated object such as `custom`.

Example Scriban usage:

```scriban
{{ if custom.ownerDisplayName }}
**Owner:** {{ custom.ownerDisplayName }}
{{ end }}

{{ if custom.supportUrl }}
**Support:** {{ custom.supportUrl }}
{{ end }}
```

Only string values are required for v1. If richer typed values are needed later, they can be added as a separate extension to the templating model.

### Template add-in files

Custom templates should support optional add-in files so module developers can inject authored Markdown content without embedding everything directly in the main template.

Examples:

1. `_header.md`
2. `_footer.md`
3. Other convention-based Markdown fragments that a custom template chooses to include.

The primary use case is repository-authored content such as notes, prerequisites, support statements, or other hand-maintained sections that should be inserted into the rendered document in predictable locations.

The recommended model is:

1. `--template-file` selects the main Scriban template.
2. Include paths are resolved relative to the module being processed by default.
3. `--template-root` may be supplied for edge cases where includes should resolve from a different root.
4. The renderer makes add-in files available through simple include syntax, without requiring the built-in template to participate.

The exact mechanism can be finalized during implementation, but the template system should make it easy for a custom template to consume files such as `_header.md` and `_footer.md`.

Include paths should support relative paths. By default, relative paths are resolved from the module root. This means an include may legally reference content outside the module folder via normal relative traversal (for example `../shared/compliance.md`).

The preferred authoring experience is a direct include-style syntax where the main template references the file name it wants to embed.

Example main template:

```scriban
{{ include "_header.md" }}
{{ include "../shared/compliance.md" }}

# {{ module.name }}

{{ module.description }}

{{ if module.parameters.size > 0 }}
## Parameters
{{ for parameter in module.parameters }}
- `{{ parameter.name }}`: {{ parameter.description }}
{{ end }}
{{ end }}

{{ include "_footer.md" }}
```

In this example, `../shared/compliance.md` is resolved relative to the module root and may resolve outside the module root.

Example `_header.md` add-in file:

```md
> This module is maintained by the platform team.

Review the prerequisites in the internal onboarding guide before deployment.
```

Example `_footer.md` add-in file:

```md
## Notes

- This module is intended for internal consumption.
- Support is provided through the shared infrastructure channel.
```

This approach keeps the built-in template simple while giving custom templates a straightforward and readable way to compose authored Markdown fragments into the final generated document.

## Documentation Data Model

The renderer should consume a typed model instead of concatenating strings throughout the command.

At a minimum, the model should expose:

```text
module
  name
  description
  path
  targetScope
  custom{}
  resourceTypes[]
  parameters[]
  outputs[]
  exportedFunctions[]
  references[]
  usageExamples[]
  dataCollection
```

Each parameter entry should include enough information to support the AVM-style tables and detail sections:

1. Name.
2. Type.
3. Required flag.
4. Description.
5. Default value.
6. Allowed values.
7. Min/max constraints where applicable.
8. Nested object and discriminator metadata where applicable.

## Usage Example Generation

The AVM format derives much of its README value from tested examples. The CLI should preserve that direction.

For v1:

1. Usage examples should be generated from local example/test files that can be discovered relative to the module folder.
2. The rules should be deterministic and repository-local.
3. The implementation should prefer compiled information over string-only parsing when extracting parameter metadata.

If a module has no discoverable examples, the built-in template should omit the section rather than generating placeholder noise.

## Existing README Preservation

`bicep docs generate` should always treat the output as a full generated artifact.

For v1:

1. `bicep docs generate` always overwrites the entire output file.
2. It never reads the existing output file to preserve sections or merge content.
3. Custom templates are also treated as full-document renders.
4. For authored content scenarios, templates should consume explicit add-in files such as `_header.md` and `_footer.md` rather than depending on implicit preservation of content already present in the output file.

This keeps the generation model deterministic and easy to reason about: the output file is always a pure function of the input module, template, add-in files, and command options.

## Packaging And Reuse

The documentation generation implementation should not live only as CLI command code. The core engine should be factored into a reusable library and published as a separate NuGet package.

### Why a separate package?

1. Module developers should be able to generate or inspect documentation from their own .NET tooling without invoking the Bicep CLI as an external process.
2. The CLI, JSON-RPC layer, and third-party tooling should all use the same core implementation.
3. Packaging the engine separately reduces pressure to expose the CLI process as the only automation surface.
4. It provides a cleaner layering model: reusable library first, CLI and RPC adapters second.

### Proposed shape

The reusable package should contain:

1. The typed documentation model.
2. The model-building logic that projects compiled Bicep information into that model.
3. The template rendering engine and built-in templates.
4. Support for template add-in file resolution for custom templates.
5. Any repository-local example discovery logic that is needed by both CLI and library consumers.

The package should not depend on CLI-only abstractions such as command-line parsing, console I/O, or localized command help resources.

### Consumer model

The expected architecture is:

1. A reusable NuGet package exposes the core docs generation APIs.
2. `bicep docs generate` and `bicep docs output` call into that package.
3. The JSON-RPC endpoints call into that same package.
4. External .NET tools used by module developers can reference the same package directly.

The exact package name can be decided during implementation, but it should be clearly scoped and consistent with existing repository naming. A name along the lines of `Bicep.Docs` or `Bicep.Documentation` would fit the current structure.

## Performance And JSON-RPC

Bulk generation should be efficient. Process startup and repeated compilation work become expensive when walking many module folders.

### CLI behavior

`bicep docs generate --pattern <glob>` should:

1. Discover all target modules first.
2. Reuse the same dependency injection container and compiler services for the full invocation.
3. Avoid recompiling the same dependency graph more than necessary.
4. Build the documentation model once per module and render from that model.

### JSON-RPC additions

Add a JSON-RPC method for documentation generation so a long-lived client can process many modules in a single session.

Proposed method for file-oriented generation and bulk rendering:

```text
bicep/generateDocs
```

Proposed method for string-oriented single-module rendering:

```text
bicep/outputDocs
```

Representative request shape:

```json
{
  "paths": [
    "./modules/storage/main.bicep",
    "./modules/network/main.bicep"
  ],
  "preset": "markdown",
  "templateFile": null,
  "custom": {
    "ownerDisplayName": "Platform Team"
  },
  "outputFile": "README.md",
  "noRestore": false
}
```

Representative response shape:

```json
{
  "results": [
    {
      "path": "./modules/storage/main.bicep",
      "success": true,
      "diagnostics": [],
      "contents": "# Storage..."
    }
  ]
}
```

The CLI command itself should call the shared generation service directly in-process. The JSON-RPC endpoints should reuse that same service, not a second implementation. That shared service should live in the reusable docs NuGet package rather than inside the CLI assembly.

## Implementation Shape

Suggested high-level structure:

1. A reusable docs library project, packaged as a separate NuGet package for module developers.
2. `DocsGenerateArguments` and `DocsOutputArguments` for grouped command parsing.
3. `DocsGenerateCommand` to orchestrate discovery, generation, and file writes.
4. `DocsOutputCommand` to orchestrate single-module rendering to stdout.
5. `DocsModuleScanner` to resolve one or many module entrypoints.
6. `DocsModelBuilder` to produce the typed documentation model from compiled Bicep data.
7. `DocsTemplateRenderer` backed by Scriban.
8. `DocsFileWriter` for deterministic whole-file overwrite behavior.
9. A binding layer that maps CLI `--set` values into the template model.
10. Shared service registration so both CLI and JSON-RPC paths use the same implementation.

The design should favor small, testable services over a single command class with mixed parsing, compilation, templating, and file I/O logic.

## Diagnostics And Exit Codes

1. If a module fails to compile with errors, no output file should be written for that module.
2. Warnings may be surfaced but should not block generation unless they prevent the docs model from being built.
3. When `--pattern` is used, generation should continue processing remaining modules after a failure and return a non-zero exit code if any module fails.
4. Template parsing or rendering failures should be reported with actionable diagnostics.

## Testing Strategy

The initial implementation should include unit tests with example repository fixtures.

### Fixture layout

Add representative fixture repositories under a CLI test project, for example:

```text
src/Bicep.Cli.UnitTests/Files/DocsGeneration/
  AvmLikeStorage/
    main.bicep
    README.md
    tests/
      e2e/
        defaults/
          main.test.bicep
  AvmLikeNetwork/
    main.bicep
    README.md
```

### Required tests

1. Generate docs for a single fixture and verify the rendered Markdown matches the checked-in README exactly.
2. Generate docs for multiple fixtures using `--pattern` and verify all outputs match.
3. Verify `bicep docs output` renders the same content as `bicep docs generate` would write for a single module.
4. Verify `bicep docs output` rejects `--pattern`.
5. Verify `bicep docs generate` rejects supplying both a positional `<path>` and `--pattern`.
5. Verify `--output-file` changes the file name but not the rendered content.
6. Verify `--template-file` can render a custom Scriban template.
7. Verify a custom template can consume add-in files such as `_header.md` and `_footer.md`.
8. Verify a custom template can consume caller-supplied `--set key=value` strings.
9. Verify `bicep docs generate` overwrites the entire output file and does not preserve existing content already present in that file.
10. Verify compiler errors prevent output writes and produce the expected non-zero exit code.
11. Verify JSON-RPC generation returns the same content as the CLI services.
12. Verify the reusable docs library can be invoked directly without going through CLI-specific types.

Golden-file comparison should normalize only platform-specific newline handling if required. Beyond that, the tests should compare exact content so that formatting regressions are caught early.

## Quality Bar

The implementation should follow existing Bicep standards:

1. Reuse existing abstractions for compilation, diagnostics, and file I/O where possible.
2. Keep command parsing explicit and localized resource strings consistent with the rest of the CLI.
3. Avoid null-forgiving operators in product code.
4. Keep output deterministic by sorting collections consistently.
5. Prefer well-known packages like Scriban over custom template engines.
6. Keep the reusable docs engine cleanly separated from CLI-specific concerns so it can ship as a stable NuGet package.
7. Add focused unit tests rather than relying only on manual validation.

## Open Questions

The following decisions should be treated as the v1 scope unless implementation work uncovers a concrete blocker:

1. v1 should expose `bicep docs generate` and `bicep docs output`. A future `bicep docs validate` command is still reasonable for CI scenarios, but it should be deferred until generation semantics, output stability, and template behavior are proven.
2. The initial built-in template should include the core AVM-aligned sections that can be generated locally and deterministically: title/description, navigation, resource types, usage examples when present, parameters, exported functions, outputs, and data collection notes when applicable. Cross-referenced modules may be included only if they can be derived from local compilation inputs without introducing network-dependent or repository-wide cache complexity. The built-in template does not need to support add-in files. That capability should be available for custom templates.
3. The initial JSON-RPC contracts should return rendered content plus diagnostics. The typed intermediate docs model may remain internal to the public RPC contract in v1, but it should be public within the reusable NuGet package so .NET consumers can use it directly if needed.

## Recommended V1 Scope

To make implementation sequencing clear, the first milestone should deliver the following:

1. `bicep docs generate` and `bicep docs output` under a new `bicep docs` command group.
2. Markdown as the only supported built-in preset, selected by default.
3. `README.md` as the default output file name for `generate`, and stdout-only behavior for `output`.
4. A built-in Scriban-backed Markdown template that is AVM-aligned in structure.
5. Support for `--template-file` to allow caller-supplied templates.
6. Support for template add-in files for custom templates, for example `_header.md` and `_footer.md`.
7. Support for caller-supplied custom string values via repeatable `--set key=value` arguments for use by custom templates.
8. Support for single-file generation and glob-based bulk generation via `--pattern` for `generate`, aligned with the existing `bicep build --pattern` convention.
9. Support for single-module string rendering with no `--pattern` support for `output`.
10. A reusable docs engine packaged as a separate NuGet package for module developers and other .NET tooling.
11. Reuse of in-process compiler services, with a shared generation service that can also be invoked through JSON-RPC.
12. Golden-file unit tests based on example repositories with checked-in expected README outputs.

Anything outside that list should be treated as a follow-up unless it is required to complete one of the items above cleanly.

## Appendix

### Templating engine comparison

The following options were evaluated for the docs feature. The evaluation criteria are: safety for user-authored templates, syntax familiarity for the IaC/AVM audience, whitespace control for Markdown output, include/partial support, performance for bulk generation, ecosystem maintenance, and license compatibility with Bicep.

| Option | Pros | Cons |
| :-- | :-- | :-- |
| Scriban | Sandboxed by default; Liquid-compatible mode plus richer native syntax; strong Markdown-friendly whitespace control; native include support; fast AST-based execution; Apache-2.0 license; precedent in Microsoft-adjacent tooling. | Native syntax is less universally known than Liquid; Liquid compatibility mode is a subset of the full Liquid spec. |
| Fluid | Pure Liquid syntax (broadly known); sandboxed by default; async-first; active maintenance; MIT license; strong .NET integration via OrchardCore lineage. | Weaker whitespace control for Markdown than Scriban; stricter Liquid-only feature set limits more advanced template patterns. |
| Handlebars.Net | Familiar Mustache/Handlebars syntax for many web developers; logic-less design reduces template complexity; MIT license; active maintenance. | Logic-less philosophy forces helper functions for non-trivial README composition; weaker conditional/table ergonomics; whitespace handling is less precise for Markdown. |
| RazorLight / RazorEngineCore | Full Razor (C#) inside templates; very expressive; familiar to .NET developers. | Templates execute arbitrary C# code, which is unsafe for user-authored templates; heavy Roslyn dependency; significantly higher runtime cost; overkill for Markdown rendering. |
| DotLiquid | Familiar Liquid syntax; long history in .NET; permissive license. | Maintenance has slowed and the .NET community has largely moved to Fluid for new work; fewer modern conveniences. |
| Stubble / Morestachio / Mustachio (Mustache variants) | Lightweight; familiar Mustache syntax. | Logic-less model is too constrained for README composition with conditional sections and tables; whitespace handling is not Markdown-friendly. |
| T4 templates | Built into the .NET tooling ecosystem; well understood for code generation. | Designed for compile-time/MSBuild scenarios; not suitable for runtime user-supplied templates. |
| Cottle | Fast and sandboxed; small footprint. | Smaller ecosystem and audience familiarity; less momentum than Scriban or Fluid. |
| String interpolation / Roslyn scripting | Maximum flexibility for internal use cases. | Executes arbitrary code; poor authoring ergonomics; not viable for user-supplied templates. |

### Chosen package and rationale

The chosen templating engine for v1 is `Scriban`.

The decision is based on the following:

1. Safety. Scriban evaluates templates in a sandbox and does not execute arbitrary code, which is appropriate for templates supplied by module developers.
2. Markdown ergonomics. Scriban's whitespace control and include semantics map naturally onto README-style output, which is the primary target format in v1.
3. Familiar syntax with room to grow. Scriban offers a Liquid-compatibility mode for users who already know Liquid, while still exposing a richer native syntax for more advanced templates.
4. Performance. Scriban is fast and AST-based, which suits bulk generation across many modules and reuse from JSON-RPC.
5. Ecosystem fit. Scriban is widely used in Microsoft-adjacent .NET tooling, is actively maintained, and ships under the Apache-2.0 license, which is compatible with Bicep.
6. Reasonable alternative. Fluid is the closest alternative and remains a sensible fallback if a Scriban-specific concern emerges during implementation.
