# Generate module documentation

The experimental `docs` command group renders documentation from a compiled Bicep module. The compiled semantic model supplies the module name, target scope, resource types, parameters, outputs, exported functions, and cross-referenced modules. Discovered usage examples supply worked samples. A [Scriban](https://github.com/scriban/scriban) template turns that model into text, which is written beside the module or streamed to stdout.

> [!WARNING]
> `docs` is an experimental command group. Command names, options, the configuration schema, the template data model, and the built-in Markdown output may change at any time without a breaking-change notice.

- [Enablement and experimental status](#enablement-and-experimental-status)
- [Commands](#commands)
- [Input resolution](#input-resolution)
- [Output resolution](#output-resolution)
- [Options](#options)
- [Precedence and mutual exclusions](#precedence-and-mutual-exclusions)
- [Configuration file](#configuration-file)
- [Usage example discovery](#usage-example-discovery)
- [Worked configuration examples](#worked-configuration-examples)
- [Custom templates](#custom-templates)
- [Custom values](#custom-values)
- [Diagnostics, failures, and exit codes](#diagnostics-failures-and-exit-codes)
- [Template model](#template-model)
- [JSON-RPC](#json-rpc)

## Enablement and experimental status

The `docs` command group does **not** use a `bicepconfig.json` feature flag. Do not add `docsGeneration` or another docs setting under `experimentalFeaturesEnabled`.

The command is available whenever the installed Bicep CLI version contains it:

```powershell
bicep docs --help
```

Experimental command groups are exposed directly rather than gated by module configuration. A successful non-SARIF docs operation writes the standard experimental disclaimer to stderr. Bulk generation writes the disclaimer at most once per invocation. SARIF output suppresses the plain-text disclaimer so stderr remains one valid SARIF document.

The `experimentalFeaturesWarning` setting in `bicepconfig.json` controls warnings for experimental **language features used by a compiled Bicep file**. It does not enable, disable, or suppress the `docs` command group.

## Commands

### `docs generate`

```text
bicep docs generate [<inputFile>] [options]
```

Renders documentation and writes it to one or more files. The input path is optional. When it is omitted, the current directory is the target folder and module files are selected by `input.include` and `input.exclude`.

Generate `README.md` next to one module:

```powershell
bicep docs generate .\main.bicep
```

Generate documentation for a module directory:

```powershell
bicep docs generate .\modules\storage
```

Generate documentation from the current directory:

```powershell
bicep docs generate
```

Generate documentation for multiple module entrypoints:

```powershell
bicep docs generate --pattern '.\modules\**\main.bicep'
```

With `--pattern`, every matched file is processed in turn. A failure for one module is reported and the run continues with the next module; the process exit code is `1` if any module failed.

### `docs output`

```text
bicep docs output [<inputFile>] [options]
```

Renders exactly one selected module and writes the result to stdout. The input path is optional. No file is created, and the output options (`--pattern`, `--outdir`, `--outfile`) are not available. Directory or omitted input must select exactly one module.

```powershell
bicep docs output .\main.bicep
```

```powershell
bicep docs output .\main.bicep > .\docs\storage.md
```

Both commands reject unmatched tokens, so an unrecognized option is an error rather than being forwarded as a positional value.

## Input resolution

The command first determines one **target folder**. Configuration discovery probes only this folder.

| Invocation | Target folder |
| :-- | :-- |
| No positional input | Current directory |
| Directory input | That directory |
| `.bicep` file input | The file's containing directory |
| `--pattern` | The pattern root: the longest path prefix before the first wildcard |

Files are then selected in this precedence order:

1. An explicit `.bicep` file is used directly. `input.include` and `input.exclude` are ignored.
2. An explicit `--pattern` selects matching files directly. Configured input patterns are ignored.
3. A directory or omitted input uses `input.include` and `input.exclude`, relative to the target folder. The built-in selection is `include: ["main.bicep"]` and `exclude: []`.

Configured input matching is recursive when the pattern is recursive and is case-insensitive. `docs generate` processes every selected file. `docs output` fails unless exactly one file is selected. Selecting no files is an error for both commands.

## Output resolution

`docs output` always writes to stdout.

`docs generate` resolves the output path per module:

| Situation | Destination |
| :-- | :-- |
| No output option | `output.file` in the module's own directory. Default `README.md`. |
| `--outfile <path>` | Exactly that path. Only valid for a single input. |
| `--outdir <dir>` | `output.file` inside `<dir>`. |
| `--pattern` with no `--outdir` | `output.file` beside each matched module. |
| `--pattern` with `--outdir <dir>` | The matched relative directory structure is recreated under `<dir>`, and `output.file` is written in each destination directory. |

Output paths are validated before anything is written:

- The output file must not be the input Bicep file (`cannot overwrite the input Bicep file`).
- The output file must not use a `.bicep` or `.bicepparam` extension (`cannot use a Bicep source file extension`).
- Two inputs in the same run must not resolve to the same output file (`Multiple input files resolve to the output file "..."`).

## Options

`docs generate` and `docs output` share the following options.

| Option | Argument | Description |
| :-- | :-- | :-- |
| `--config-file-path` | path | Explicit docs configuration JSON file. Resolved relative to the current directory and takes precedence over automatic discovery. |
| `--template-file` | path | Scriban template used instead of the built-in Markdown template. Resolved relative to the current directory. |
| `--template-root` | path | Root directory used to resolve template includes. Resolved relative to the current directory. Must exist. |
| `--custom-template-value` | `key=value` | Repeatable inline custom value. |
| `--custom-template-value-file-path` | path | Repeatable JSON file of custom values. |
| `--no-restore` | flag | Skip restoring external modules before compiling. |
| `--diagnostics-format` | `default` or `sarif` | Diagnostic output format. |

`docs generate` adds:

| Option | Argument | Description |
| :-- | :-- | :-- |
| `--pattern` | glob | Generate documentation for every matched module entrypoint. |
| `--outdir` | directory | Directory to write generated documentation into. |
| `--outfile` | path | Exact file to write generated documentation to. |

`docs output` does not accept `--pattern`, `--outdir`, or `--outfile`.

## Precedence and mutual exclusions

### Mutual exclusions

| Combination | Result |
| :-- | :-- |
| positional input **and** `--pattern` | `The input path and --pattern parameter cannot both be specified.` |
| `--outdir` **and** `--outfile` | `The --outdir and --outfile parameters cannot both be used` |
| `--outfile` **and** `--pattern` | `The --outfile parameter cannot be used with the --pattern parameter` |

### Configuration versus CLI

Explicit CLI options override configuration values, and configuration values override built-in defaults.

| Setting | Built-in default | Configuration | CLI override |
| :-- | :-- | :-- | :-- |
| Input selection | `include: ["main.bicep"]`, `exclude: []` | `input.include`, `input.exclude` | explicit `.bicep` input or `--pattern` |
| Generated file name | `README.md` | `output.file` | `--outfile` (whole path) or `--outdir` (directory only) |
| Template | built-in Markdown template | `template.file` | `--template-file` |
| Include root | the module's own directory | `template.includeRoot` | `--template-root` |
| Custom values | none | `template.values` | `--custom-template-value`, `--custom-template-value-file-path` |
| Usage example sources | `examples` and `tests` sources | `examples.sources` | none |

`--template-file` and `--template-root` are resolved relative to the current working directory and completely replace the corresponding configuration values, including their configuration-relative anchoring.

### Custom value precedence

Custom values are merged in this order, with later entries overwriting earlier entries key by key:

1. `template.values` from the configuration file (the baseline).
2. `--custom-template-value` and `--custom-template-value-file-path` arguments, applied strictly in command-line order.

Inline values and value files may be interleaved and repeated; the last occurrence of a key wins. Keys supplied only by `template.values` are retained.

## Configuration file

The conventional file name is `bicepdocsconfig.json`. Configuration is resolved once per invocation in this order:

1. An explicit `--config-file-path`, resolved relative to the current working directory.
2. `bicepdocsconfig.json` in the target folder.
3. Built-in defaults.

Automatic discovery checks the target folder only. It never walks up to ancestor directories and never probes the folders of individual modules selected by `--pattern` or configured input patterns. Every selected module therefore uses the same invocation-scoped settings.

The three missing or invalid file cases are intentionally different:

| Situation | Behaviour |
| :-- | :-- |
| Explicit `--config-file-path` does not exist | Hard error naming the requested file. |
| No conventional file exists in the target folder | Silently use built-in defaults. |
| A discovered conventional file exists but is malformed or invalid | Hard error naming the discovered file. |

Docs settings use a dedicated file rather than `bicepconfig.json` by design. The file holds static settings that must be consistent across every module in a repository; dynamic choices are supplied per invocation on the command line. `bicepconfig.json` uses nearest-file-wins resolution with no ancestor merging, so a module with its own `bicepconfig.json` would silently lose repository-level docs settings and fall back to defaults.

Use the published schema for validation and editor completion:

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json"
}
```

VS Code associates this schema automatically with files named `bicepdocsconfig.json`. Keep the `$schema` property when using another file name through `--config-file-path`.

### Syntax and strictness

- The document is JSON with `//` and `/* */` comments and trailing commas allowed.
- Property names are camelCase.
- The root must be a JSON object.
- Unknown properties are rejected.
- Duplicate property names are rejected, including in nested objects.
- `null` is rejected everywhere — as a property value and as an array element.

Representative failures:

| Input | Message fragment |
| :-- | :-- |
| Missing file | `The docs configuration file "..." does not exist.` |
| Unreadable file | `Unable to read docs configuration file "...": ...` |
| Malformed JSON or unknown property | `The docs configuration file "..." is invalid: ...` |
| Non-object root | `The docs configuration file "..." must contain a JSON object.` |
| Duplicate property | `The docs configuration contains the duplicate property "$.template.file".` |
| `null` value | `The docs configuration property "$.template.file" cannot be null.` |

### Schema reference

| JSON path | Type | Required | Default | Notes |
| :-- | :-- | :-- | :-- | :-- |
| `$schema` | string | no | — | JSON schema URI used by editors. |
| `input.include` | array of string | no | `["main.bicep"]` | Case-insensitive glob patterns that select module files relative to the target folder. Patterns must be nonempty and relative and must not traverse with `..`. |
| `input.exclude` | array of string | no | `[]` | Case-insensitive glob patterns removed from the included files. Uses the same target-folder anchor and validation as `input.include`. |
| `output.file` | string | no | `README.md` | Generated file name only. Must be a single portable file name: no directory separators, no invalid or reserved characters, no trailing dot or space, and not a reserved device name. |
| `template.file` | string | no | built-in template | Scriban template path. Must be nonempty. Relative paths are anchored to the configuration file's directory. |
| `template.includeRoot` | string | no | module directory | Root for template includes. Must be nonempty and must exist. Relative paths are anchored to the configuration file's directory. |
| `template.values` | object of string | no | `{}` | Baseline custom values. Keys must be nonempty. Values must be strings. |
| `examples.sources` | array of object | no | see below | Replaces the default sources entirely when present. |
| `examples.sources[].path` | string | **yes** | — | Directory relative to each module root. `.` selects the module root itself. Must be relative and must not traverse with `..`. |
| `examples.sources[].include` | array of string | no | `[]` | Case-insensitive glob patterns, relative to the source path. Each pattern must be nonempty and valid. |
| `examples.sources[].exclude` | array of string | no | `[]` | Case-insensitive glob patterns, relative to the source path. |
| `examples.reassignments` | array of object | no | `[]` | Conditional parent-to-child example moves. |
| `examples.reassignments[].from.include` | array of string | **yes** | — | Must contain at least one pattern. Matched against example paths relative to their discovery source. |
| `examples.reassignments[].from.exclude` | array of string | no | `[]` | Matched against example paths relative to their discovery source. |
| `examples.reassignments[].to` | string | **yes** | — | Exactly one direct child directory segment. Must not contain separators or traverse with `..`. |

`input.include` and `input.exclude` apply only to directory or omitted input. An empty `input.include` selects no files and causes an error. Exclusions are applied after all inclusions. For example:

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json",
  "input": {
    "include": ["modules/**/*.bicep"],
    "exclude": ["**/*.test.bicep", "**/dependencies/**"]
  }
}
```

`docs generate .` processes every selected module. `docs output .` reports an error if this configuration selects more than one module. Passing `.\modules\storage\main.bicep` or `--pattern` bypasses these configured input patterns.

The default `examples.sources` value is:

```json
[
  {
    "path": "examples",
    "include": ["*.bicep", "**/main.bicep"],
    "exclude": ["**/dependencies*.bicep"]
  },
  {
    "path": "tests",
    "include": ["**/*.test.bicep"],
    "exclude": ["**/dependencies*.bicep"]
  }
]
```

### Omitted versus empty `sources`

| Value | Behaviour |
| :-- | :-- |
| `examples` omitted, or `examples.sources` omitted | The two default sources are used. |
| `"sources": []` | Usage example discovery is disabled. `module.usageExamples` is empty. |
| `"sources": [ ... ]` | The supplied sources fully replace the defaults. Defaults are not merged in. |

Within a supplied source, an omitted `include` defaults to `[]`, which matches nothing. Supply at least one include pattern for every source that should contribute examples.

### Path anchoring

| Value | Anchor |
| :-- | :-- |
| `--config-file-path` | Current working directory. |
| Discovered `bicepdocsconfig.json` | The invocation's target folder. |
| `input.include`, `input.exclude` | The target folder. |
| `template.file`, `template.includeRoot` | The configuration file's directory. Rooted paths are used as-is. |
| `--template-file`, `--template-root` | Current working directory. These bypass configuration anchoring entirely. |
| `examples.sources[].path` | Each module's own root directory. |
| `examples.reassignments[].to` | The parent module's root directory. |

This means one configuration file can be shared by many modules: input selection is evaluated once from the target folder, template settings stay pinned to the configuration file, and example sources are re-resolved per module.

### Replaceable defaults

`main.bicep`, `README.md`, `examples`, `tests`, and the `dependencies*` exclusion are **defaults, not hardcoded requirements**. Every one of them is replaceable:

| Default | Replace with |
| :-- | :-- |
| `main.bicep` input | `input.include` and `input.exclude` |
| `README.md` output name | `output.file`, `--outfile` |
| `examples` and `tests` directories | `examples.sources[].path` |
| `*.bicep`, `**/main.bicep`, `**/*.test.bicep` | `examples.sources[].include` |
| `**/dependencies*.bicep` exclusion | `examples.sources[].exclude` |
| Built-in Markdown template | `template.file`, `--template-file` |

## Usage example discovery

Discovery runs once per module, using the module's own root directory.

1. Each source in `examples.sources` is processed in declaration order.
2. `<moduleRoot>/<source.path>` is resolved. Missing directories are skipped silently.
3. Files are enumerated recursively. Reparse points (symbolic links and junctions) are skipped, and directory recursion is limited to 100 levels.
4. Each file's path relative to the source directory is matched against `include` and then `exclude`. Glob matching is **case-insensitive**, so `**/*.test.bicep` also matches `Main.Test.Bicep`.
5. Duplicates are removed by resolved file path. The first source that matches a file wins, so source order determines which source claims a shared file.
6. The final list is sorted by the example path relative to the module root.

An invalid glob pattern fails the run with `Invalid usage-example glob: ...`.

### Example names and descriptions

An example name is selected in this order:

1. Literal `metadata name` in the example file.
2. The example file's containing folder, when the file is nested below the source directory.
3. The file name without its `.bicep` extension, when the file sits directly in the source directory.

The description is taken from literal `metadata description`, otherwise from the contiguous `//` comments at the top of the file, otherwise it is `null`. Example contents have trailing whitespace trimmed.

The built-in Markdown template numbers usage-example headings (`### Example 1: _name_`), so repeated display names remain unambiguous.

### Reassignments

A reassignment conditionally moves examples from a parent module to one direct child module directory. It is evaluated from both sides:

- **Parent side.** When `<moduleRoot>/<to>` exists, discovered examples whose source-relative paths match `from` are removed from the parent's example list.
- **Child side.** When the module being documented is `<parent>/<to>`, the parent directory is scanned with the same sources and matching examples are added to the child's list. Their `path` values are relative to the child, for example `../tests/e2e/mg-scope.defaults/main.test.bicep`.
- **No-op.** If the `to` directory does not exist and the module is not the named child, the reassignment changes nothing.

`to` must identify a single child directory; otherwise the run fails with `Usage-example reassignments must identify one child module directory.`

## Worked configuration examples

### 1. Defaults-equivalent configuration

Save this as `bicepdocsconfig.json` in the target folder. It is behaviourally identical to using built-in defaults.

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json",
  "input": {
    "include": ["main.bicep"],
    "exclude": []
  },
  "output": {
    "file": "README.md"
  },
  "examples": {
    "sources": [
      {
        "path": "examples",
        "include": ["*.bicep", "**/main.bicep"],
        "exclude": ["**/dependencies*.bicep"]
      },
      {
        "path": "tests",
        "include": ["**/*.test.bicep"],
        "exclude": ["**/dependencies*.bicep"]
      }
    ]
  }
}
```

```powershell
bicep docs generate .
```

### 2. Nonstandard folders, module files, and extensions

A repository that keeps module files at `deploy/module.bicep`, samples under `samples`, and integration tests under `verify`, and that publishes `DOCUMENTATION.md`.

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json",
  "input": {
    "include": ["deploy/module.bicep"],
    "exclude": []
  },
  "output": {
    "file": "DOCUMENTATION.md"
  },
  "examples": {
    "sources": [
      {
        "path": "samples",
        "include": ["**/*.sample.bicep", "**/deploy.bicep"],
        "exclude": ["**/_shared/**"]
      },
      {
        "path": "verify",
        "include": ["**/*.integration.bicep"],
        "exclude": ["**/fixtures/**"]
      }
    ]
  }
}
```

```powershell
bicep docs generate .\services\payments
```

With `bicepdocsconfig.json` in `.\services\payments`, the directory input selects `.\services\payments\deploy\module.bicep` and writes `.\services\payments\deploy\DOCUMENTATION.md`. Use `--outdir` or `--outfile` when documentation should be written somewhere else.

### 3. Disabling usage examples

An explicitly empty `sources` array turns discovery off. This is different from omitting `sources`, which applies the defaults.

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json",
  "examples": {
    "sources": []
  }
}
```

```powershell
bicep docs output .\main.bicep `
  --config-file-path .\docs\no-examples.json
```

### 4. Template baseline values with CLI overrides

The configuration file supplies stable values; the pipeline overrides the volatile ones. Template paths are anchored to the configuration file's directory, so `templates/readme.scriban` below means `.\docs\templates\readme.scriban`.

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json",
  "template": {
    "file": "templates/readme.scriban",
    "includeRoot": "templates",
    "values": {
      "owner": "Platform Team",
      "supportUrl": "https://contoso.example/support",
      "buildNumber": "local"
    }
  }
}
```

```powershell
bicep docs generate .\main.bicep `
  --config-file-path .\docs\bicep-docs.json `
  --custom-template-value buildNumber=$env:BUILD_BUILDNUMBER `
  --custom-template-value-file-path .\docs\release-values.json
```

Effective values: `owner` and `supportUrl` come from the configuration file, `buildNumber` is replaced by the inline CLI value, and any keys in `release-values.json` override everything before them because that argument appears last.

### 5. AVM-style multi-scope reassignment

An Azure Verified Modules pattern module keeps scope-specific tests beside the parent module, but documents them on the corresponding child modules.

```json
{
  "$schema": "https://raw.githubusercontent.com/Azure/bicep/main/src/vscode-bicep/schemas/bicepdocsconfig.schema.json",
  "input": {
    "include": ["**/main.bicep"],
    "exclude": []
  },
  "output": {
    "file": "README.md"
  },
  "examples": {
    "sources": [
      {
        "path": "tests",
        "include": ["**/*.test.bicep"],
        "exclude": ["**/dependencies*.bicep"]
      }
    ],
    "reassignments": [
      {
        "from": { "include": ["**/rg-scope.*/**"] },
        "to": "rg-scope"
      },
      {
        "from": { "include": ["**/sub-scope.*/**"] },
        "to": "sub-scope"
      },
      {
        "from": {
          "include": ["**/mg-scope.*/**"],
          "exclude": ["**/mg-scope.legacy/**"]
        },
        "to": "mg-scope"
      }
    ]
  }
}
```

```powershell
bicep docs generate .\avm\res\authorization\policy-assignment `
  --config-file-path .\docs\avm-docs.json
```

The parent module's documentation omits the scope-specific tests, and `rg-scope`, `sub-scope`, and `mg-scope` each document only their own tests. `mg-scope.legacy` tests stay with the parent because they are excluded from the reassignment.

## Custom templates

Custom templates use [Scriban](https://github.com/scriban/scriban) syntax:

```scriban
# {{ module.name }}

{{ module.description }}

{{ for parameter in module.parameters }}
- `{{ parameter.name }}`: {{ parameter.description }}
{{ end }}
```

Use includes for authored Markdown:

```scriban
{{ include "_header.md" }}
```

Includes resolve from the module directory unless `template.includeRoot` or `--template-root` is supplied. Relative traversal such as `../shared/notes.md` is supported, and the include root must exist.

Rendered output is normalized to `\n` line endings with exactly one trailing newline. Template loops are capped at 100,000 iterations. A template that fails to parse or render fails the module with `DOCS003`.

The built-in Markdown template emits `Navigation`, `Resource Types`, `Usage Examples`, `Parameters`, `Exported Functions`, `Outputs`, and `Cross-referenced Modules` sections, omitting any section with no content.

## Custom values

Custom values are available to templates as both `custom.<key>` and `module.custom.<key>`.

There are two CLI mechanisms, plus the `template.values` configuration baseline.

Inline values use `key=value`, and the separator must not be the first character:

```powershell
bicep docs output .\main.bicep --custom-template-value owner="Platform Team"
```

Value files must contain a JSON object whose values are strings:

```json
{
  "owner": "Platform Team",
  "supportUrl": "https://contoso.example/support"
}
```

Value files are rejected when they do not exist, are not valid JSON, are not an object, contain an empty key, contain a duplicate key, or contain a non-string value.

Files and individual values may be repeated and interleaved. They are applied from left to right, so the last occurrence of a key wins:

```powershell
bicep docs output .\main.bicep `
  --template-file .\readme.scriban `
  --custom-template-value-file-path .\defaults.json `
  --custom-template-value owner="Application Team" `
  --custom-template-value-file-path .\environment.json
```

In this example, `environment.json` has the final precedence for keys it contains, and any key it omits keeps the value set by the earlier arguments or by `template.values`.

## Diagnostics, failures, and exit codes

### Compilation

Modules are compiled before rendering. `--no-restore` skips restoring external modules, which is appropriate when a restore has already been performed and the cache is warm; unrestored references then surface as compilation errors.

Compilation errors are reported as normal Bicep diagnostics, the module fails, and no documentation is produced. No `DOCS` diagnostic is added, because the underlying diagnostics already describe the problem.

### Failure semantics

| Stage | Failure | Code |
| :-- | :-- | :-- |
| Input, option, or compilation setup | Invalid path, invalid option value, unreadable configuration | `DOCS001` |
| Output write | The generated file could not be written or replaced | `DOCS002` |
| Model or template | Template parse failure, render failure, invalid glob, depth limit | `DOCS003` |

Any failure sets the process exit code to `1`. For multi-module generation through `--pattern` or configured input patterns, the run continues with the remaining modules and reports the failures collectively.

### Existing output is preserved

`docs generate` writes atomically. Content is written to a temporary file in the destination directory and then moved over the target, and the temporary file is removed afterwards. Consequently:

- A compilation failure, template failure, or write failure never truncates or corrupts an existing output file.
- Readers never observe a partially written file.

### Diagnostics format

With `--diagnostics-format default`, diagnostics are written to stderr per module and the experimental disclaimer warning is logged at most once per run.

With `--diagnostics-format sarif`, all modules in the run are aggregated into exactly one SARIF log containing a single `runs` entry, written to stderr. The experimental warning is suppressed so the SARIF payload stays machine-readable. `docs output` writes nothing to stdout when rendering fails in SARIF mode.

## Template model

The root object contains `module` and `custom`.

| Field | Type | Description |
| :-- | :-- | :-- |
| `module.name` | string | Module name from literal `metadata name`, or the module directory name. |
| `module.description` | string or null | Module description. |
| `module.path` | string | Module entrypoint path. |
| `module.targetScope` | string | Bicep target scope. |
| `module.custom` | object | Values supplied with `--custom-template-value`. |
| `module.resourceTypes` | array | Declared Azure resource types. |
| `module.parameters` | array | Module parameters and nested properties. |
| `module.outputs` | array | Module outputs. |
| `module.exportedFunctions` | array | Exported functions. |
| `module.references` | array | Referenced local modules. |
| `module.usageExamples` | array | Discovered local examples and tests. |
| `custom` | object | Values supplied with `--custom-template-value`. |

Collections are sorted by name, except `module.usageExamples`, which is sorted by path relative to the module root.

### Resource types

Each `module.resourceTypes` item contains:

| Field | Type |
| :-- | :-- |
| `type` | string |
| `existing` | bool |

### Parameters and nested properties

Each `module.parameters` item and nested `properties` item contains:

| Field | Type | Description |
| :-- | :-- | :-- |
| `name` | string | Parameter or property name. |
| `type` | string | Normalized Bicep type. |
| `required` | bool | Whether a value is required. |
| `secure` | bool | Whether the type is secure. |
| `description` | string or null | Description metadata. |
| `defaultValue` | string or null | Bicep source representation of the default. |
| `defaultValueFence` | string or null | Markdown code fence sized for the default value. |
| `allowedValues` | array | Literal allowed values. |
| `minValue` | integer or null | Minimum integer value. |
| `maxValue` | integer or null | Maximum integer value. |
| `minLength` | integer or null | Minimum string or array length. |
| `maxLength` | integer or null | Maximum string or array length. |
| `pattern` | string or null | String validation pattern. |
| `truncated` | bool | Whether recursive or deeply nested properties were omitted. |
| `properties` | array | Nested object or array-item properties. |
| `discriminator` | object or null | Discriminated object details. |

A discriminator contains `propertyName` and `cases`. Each case contains `value` and `properties`.

Object types that permit additional properties expose a synthetic property named `>Any_other_property<` describing the additional-property type.

Secure object schemas are expanded so templates can document property names, types, and descriptions. Runtime parameter values are never part of the documentation model.

Type expansion is bounded. Nesting deeper than 20 levels, a total expanded node count above 10,000, and recursive type cycles all stop expansion and set `truncated` to `true` on the affected node.

### Outputs

Each `module.outputs` item contains `name`, `type`, `secure`, and `description`.

### Exported functions

Each `module.exportedFunctions` item contains `name`, `parameters`, `returnType`, and `description`. Function parameters contain `name`, `type`, and `description`.

### References

Each `module.references` item contains `symbolicName`, `path`, and `description`.

### Usage examples

Each `module.usageExamples` item contains `name`, `path`, `description`, `contents`, and a Markdown-safe `fence`. The fence is at least three backticks and always longer than the longest backtick run in `contents`, so example content never breaks the surrounding code block.

## JSON-RPC

Long-lived clients can use:

- `bicep/generateDocs` for one or more file-oriented results.
- `bicep/outputDocs` for one string-oriented result.

Both requests accept the same customization as the CLI: an optional template file, template root, custom values, configuration file path, and a no-restore flag. `bicep/generateDocs` takes a list of paths and an optional output file; `bicep/outputDocs` takes a single path.

JSON-RPC does not auto-discover `bicepdocsconfig.json`. Tooling clients must pass the configuration file path explicitly. This difference is intentional because a `bicep/generateDocs` request can contain unrelated paths with no unambiguous shared target folder.

Each JSON-RPC path must resolve to exactly one module. An explicit `.bicep` path always identifies that module. A directory path uses `input.include` and `input.exclude` from the explicitly supplied configuration, or the built-in `main.bicep` selection when no configuration path is supplied. A directory that selects zero or multiple files returns a structured `DOCS001` failure. CLI `docs generate` differs here because one CLI target folder can intentionally select multiple modules.

Each result contains the input path, optional output path, success state, diagnostics, and rendered contents. The same generation pipeline is available directly from `Bicep.Core` for hosts that embed the compiler.

Documentation-specific errors use:

| Code | Meaning |
| :-- | :-- |
| `DOCS001` | Invalid input, option, docs configuration, or compilation setup. |
| `DOCS002` | Output write failure. |
| `DOCS003` | Documentation model or template rendering failure. |
