# Generate module documentation

The experimental `docs` command group renders documentation from compiled Bicep modules. The semantic model supplies resource types, parameters, exported types, exported variables, exported functions, outputs, and referenced modules. Local files can supply usage examples. A [Scriban](https://github.com/scriban/scriban) template turns that model into text.

> [!WARNING]
> `docs` is experimental. Command options, configuration, the template model, and built-in Markdown may change without a breaking-change notice.

- [Experimental status](#experimental-status)
- [Command](#command)
- [Input and output resolution](#input-and-output-resolution)
- [Options](#options)
- [Configuration](#configuration)
- [Usage example discovery](#usage-example-discovery)
- [Custom templates and values](#custom-templates-and-values)
- [Diagnostics and failures](#diagnostics-and-failures)
- [Template model](#template-model)
- [JSON-RPC](#json-rpc)

## Experimental status

The `docs` command group does not use a feature flag. It is available whenever the installed Bicep CLI contains it:

```powershell
bicep docs --help
```

A successful non-SARIF invocation writes the standard experimental warning to stderr. Bulk generation writes it at most once. SARIF mode suppresses the plain-text warning so stderr remains one valid SARIF document.

The `experimentalFeaturesWarning` setting controls warnings for experimental language features used by a Bicep file. It does not enable, disable, or suppress the `docs` command group warning.

## Command

```text
bicep docs generate [<inputFile>] [options]
```

The command requires either one positional `.bicep` input file or `--pattern`.

Write `README.md` beside one module:

```powershell
bicep docs generate .\main.bicep
```

Write documentation for every matched module:

```powershell
bicep docs generate --pattern '.\modules\**\main.bicep'
```

Render one module to stdout without writing a file:

```powershell
bicep docs generate .\main.bicep --stdout
```

Redirect stdout:

```powershell
bicep docs generate .\main.bicep --stdout > .\docs\module.md
```

`--stdout` is single-module only. It cannot be combined with `--pattern`, `--outdir`, or `--outfile`.

## Input and output resolution

### Input

The positional input must resolve to a `.bicep` file. Directory inputs are not supported.

`--pattern` selects `.bicep` files directly. The longest literal directory prefix before the first wildcard is the pattern root. This is also the root used when preserving relative directories under `--outdir`.

Supplying both a positional input and `--pattern` fails. Omitting both fails with:

```text
Either the input file path or the --pattern parameter must be specified
```

### Output

For every successfully rendered module, `docs generate` resolves its output as follows:

| Situation | Destination |
| :-- | :-- |
| No output option | `documentation.output.file` beside the input module. Default: `README.md`. |
| `--outfile <path>` | Exactly that path. Only valid for a single input. |
| `--outdir <dir>` with one input | `documentation.output.file` inside `<dir>`. |
| `--pattern` with no `--outdir` | `documentation.output.file` beside each matched module. |
| `--pattern` with `--outdir <dir>` | Recreates each matched relative directory under `<dir>`, then writes `documentation.output.file`. |
| `--stdout` | Writes the rendered document to stdout and creates no file. |

Output paths are validated before writes:

- Documentation cannot overwrite its input Bicep file.
- Documentation cannot use a `.bicep` or `.bicepparam` extension.
- Multiple inputs cannot resolve to the same output path.

Compilation and rendering complete before any output is written for that module. A compile or render failure therefore does not overwrite an existing output.

## Options

| Option | Argument | Description |
| :-- | :-- | :-- |
| `--stdout` | flag | Print one rendered document to stdout. |
| `--pattern` | glob | Generate documentation for every matched Bicep file. |
| `--outdir` | directory | Write generated documentation beneath this directory. |
| `--outfile` | path | Write one generated document to this exact path. |
| `--no-restore` | flag | Skip restoring external modules before compilation. |
| `--diagnostics-format` | `default` or `sarif` | Select diagnostic output format. |

Mutually exclusive combinations use the same validation as other Bicep commands:

| Combination | Result |
| :-- | :-- |
| positional input and `--pattern` | Error |
| `--stdout` and `--pattern` | Error |
| `--stdout` and `--outdir` | Error |
| `--stdout` and `--outfile` | Error |
| `--outdir` and `--outfile` | Error |
| `--outfile` and `--pattern` | Error |

## Configuration

Documentation settings live under `documentation` in `bicepconfig.json`.

```json
{
  "documentation": {
    "output": {
      "file": "README.md"
    },
    "template": {
      "file": "docs/templates/readme.scriban",
      "includeRoot": "docs/templates",
      "values": {
        "owner": "Platform Team"
      }
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
      ],
      "reassignments": []
    }
  }
}
```

The existing `bicepconfig.json` schema provides validation and editor completion.

### Resolution behavior

Configuration is resolved independently for each input Bicep file using the standard Bicep configuration lookup.

The lookup starts in the source file's directory and walks toward the filesystem root. The first `bicepconfig.json` found is merged with built-in defaults and used for that module.

> [!IMPORTANT]
> `bicepconfig.json` is nearest-file-wins and does not merge ancestor configuration files. If a module has its own `bicepconfig.json`, it does not inherit repository-level `documentation` settings. Omitted settings fall back to built-in defaults.

For repositories that require identical documentation settings across every module, enforce a policy that prevents nested `bicepconfig.json` files. Configuration inheritance is designed in [REP 0023](https://github.com/Azure/bicep-reps/blob/main/active/0023-bicep-configuration-inheritance.md), which adds an `extends` property to `bicepconfig.json`; once implemented, a nested configuration will be able to inherit repository-level `documentation` settings explicitly.

### Precedence

Documentation content settings are read only from `bicepconfig.json`. Output location options override the configured output file without changing the rendered content.

| Setting | Built-in default | Configuration | Output override |
| :-- | :-- | :-- | :-- |
| Output file name | `README.md` | `documentation.output.file` | `--outfile` or `--outdir` |
| Template | Built-in Markdown | `documentation.template.file` | None |
| Include root | Module directory | `documentation.template.includeRoot` | None |
| Custom values | None | `documentation.template.values` | None |
| Example sources | `examples` and `tests` | `documentation.examples.sources` | None |
| Example reassignments | None | `documentation.examples.reassignments` | None |

### Configuration properties

| JSON path | Type | Default | Description |
| :-- | :-- | :-- | :-- |
| `documentation.output.file` | string | `README.md` | A portable file name without directory separators. |
| `documentation.template.file` | string | Built-in template | Custom Scriban template path. |
| `documentation.template.includeRoot` | string | Module directory | Root for Scriban includes. Must exist. |
| `documentation.template.values` | object of string | `{}` | Baseline custom template values. |
| `documentation.examples.sources` | array | See below | Ordered example sources. Supplied values replace the defaults. |
| `documentation.examples.sources[].path` | string | Required | Directory relative to each module root. `.` selects the module root. |
| `documentation.examples.sources[].include` | array of string | `[]` | Case-insensitive include globs relative to the source path. |
| `documentation.examples.sources[].exclude` | array of string | `[]` | Case-insensitive exclude globs relative to the source path. |
| `documentation.examples.reassignments` | array | `[]` | Ordered parent-to-child reassignment rules. |
| `documentation.examples.reassignments[].from.include` | array of string | Required | Paths selected from the parent examples. |
| `documentation.examples.reassignments[].from.exclude` | array of string | `[]` | Parent example paths excluded from the rule. |
| `documentation.examples.reassignments[].to` | string | Required | One direct child directory name. |

An omitted `documentation.examples.sources` uses the built-in sources. An explicit empty array disables usage-example discovery.

The built-in sources are:

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

### Path anchoring

| Value | Anchor |
| :-- | :-- |
| `documentation.template.file` | Directory containing the resolved `bicepconfig.json`. |
| `documentation.template.includeRoot` | Directory containing the resolved `bicepconfig.json`. |
| `documentation.examples.sources[].path` | Each module's own directory. |
| `documentation.examples.reassignments[].to` | Parent module directory. |

Rooted template paths are used as-is. A relative configured template path requires a resolved user `bicepconfig.json`; built-in configuration has no filesystem directory to use as an anchor.

### AVM-style example reassignment

Some repositories keep scope-specific examples beside a parent module while documenting them on child modules:

```json
{
  "documentation": {
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
          "from": {
            "include": ["**/rg-scope.*/**"]
          },
          "to": "rg-scope"
        },
        {
          "from": {
            "include": ["**/sub-scope.*/**"]
          },
          "to": "sub-scope"
        },
        {
          "from": {
            "include": ["**/mg-scope.*/**"]
          },
          "to": "mg-scope"
        }
      ]
    }
  }
}
```

When the parent is documented, matching examples are removed if the named child exists. When that child is documented, matching parent examples are added with paths relative to the child. If the child does not exist, the rule is a no-op.

## Usage example discovery

Discovery runs once per compiled module.

1. Each source is processed in declaration order.
2. `<moduleRoot>/<source.path>` is resolved. Missing directories are skipped.
3. Files are traversed recursively. Reparse points are skipped and traversal is bounded to 100 directory levels.
4. Paths relative to the source directory are matched case-insensitively against `include` and `exclude`.
5. Resolved files are deduplicated. The first source that matches a file wins.
6. Results are sorted by path relative to the module root.

Example names are selected in this order:

1. Literal `metadata name`.
2. The containing directory name when nested.
3. The file name without `.bicep`.

Descriptions come from literal `metadata description`, then leading contiguous `//` comments, then `null`.

## Custom templates and values

Custom templates use Scriban:

```scriban
# {{ module.name }}

{{ module.description }}

{{ for parameter in module.parameters }}
- `{{ parameter.name }}`: {{ parameter.description }}
{{ end }}
```

Use includes for reusable fragments:

```scriban
{{ include "_header.md" }}
```

Includes resolve from the module directory unless `documentation.template.includeRoot` is configured.

Rendered output uses `\n` line endings and exactly one trailing newline. Template loops are limited to 100,000 iterations.

### Custom values

Custom values are configured under `documentation.template.values` and are available as `custom.<key>` and `module.custom.<key>`:

```json
{
  "documentation": {
    "template": {
      "values": {
        "owner": "Platform Team",
        "supportUrl": "https://contoso.example/support"
      }
    }
  }
}
```

## Diagnostics and failures

Modules are compiled before rendering. `--no-restore` skips external module restoration.

Compilation failures use normal Bicep diagnostics. Input, configuration, rendering, and write failures use the same CLI error handling as other Bicep commands rather than introducing documentation-specific diagnostic codes.

Any failure returns exit code `1`. Pattern generation continues after compilation diagnostics so valid modules can still be rendered. Setup, rendering, and write failures stop the command.

With `--diagnostics-format sarif`, each compiled input emits its normal SARIF diagnostics on stderr. Errors that are not Bicep diagnostics remain plain CLI errors. `--stdout` writes nothing on failure.

## Template model

The root Scriban object contains `module` and `custom`.

| Field | Type | Description |
| :-- | :-- | :-- |
| `module.name` | string | Literal `metadata name`, or the module directory/file fallback. |
| `module.description` | string or null | Module description. |
| `module.path` | string | Entrypoint file path. |
| `module.targetScope` | string | Bicep target scope. |
| `module.custom` | object | Effective custom values. |
| `module.resourceTypes` | array | Declared resource types. |
| `module.parameters` | array | Parameters and nested type information. |
| `module.exportedTypes` | array | Named exported types and nested type information. |
| `module.exportedVariables` | array | Exported variables and inferred type information. |
| `module.exportedFunctions` | array | Exported functions. |
| `module.outputs` | array | Module outputs. |
| `module.references` | array | Referenced local modules. |
| `module.usageExamples` | array | Discovered usage examples. |
| `custom` | object | Effective custom values. |

Collections are deterministic and sorted by name, except usage examples, which are sorted by relative path.

### Resource types

Each resource type contains:

| Field | Type |
| :-- | :-- |
| `type` | string |
| `existing` | bool |

### Parameters

Each parameter and nested property contains:

| Field | Type | Description |
| :-- | :-- | :-- |
| `name` | string | Parameter or property name. |
| `type` | string | Normalized Bicep type. |
| `required` | bool | Whether a value is required. |
| `secure` | bool | Whether the type is secure. |
| `description` | string or null | Description metadata. |
| `defaultValue` | string or null | Bicep source for the default. |
| `defaultValueFence` | string or null | Markdown-safe code fence. |
| `allowedValues` | array | Literal allowed values. |
| `minValue`, `maxValue` | integer or null | Numeric bounds. |
| `minLength`, `maxLength` | integer or null | Length bounds. |
| `pattern` | string or null | String validation pattern. |
| `truncated` | bool | Whether bounded expansion omitted nested details. |
| `properties` | array | Nested properties. |
| `discriminator` | object or null | Discriminator property and cases. |

Type expansion is bounded to 20 levels and 10,000 expanded nodes, and detects recursive cycles.

### Exported types and variables

Each exported type or variable contains:

| Field | Type |
| :-- | :-- |
| `name` | string |
| `type` | string |
| `secure` | bool |
| `description` | string or null |
| `allowedValues` | array |
| `minValue`, `maxValue` | integer or null |
| `minLength`, `maxLength` | integer or null |
| `pattern` | string or null |
| `truncated` | bool |
| `properties` | array |
| `discriminator` | object or null |

Exported types unwrap the compiler's type-value wrapper so templates receive the documented type itself.

### Exported functions

Each exported function contains `name`, `parameters`, `returnType`, and `description`. Function parameters contain `name`, `type`, and `description`.

### Outputs

Each output contains `name`, `type`, `secure`, and `description`.

### References

Each reference contains `symbolicName`, `path`, and `description`.

### Usage examples

Each usage example contains `name`, `path`, `description`, `contents`, and `fence`. The fence is longer than any backtick run in the contents.

## JSON-RPC

Long-lived clients can use:

- `bicep/generateDocs` to render documentation for a module.

The method never writes files. It returns the rendered content to the caller, and the client is
responsible for writing any output. This matches `bicep/compile`, which returns the compiled template
rather than writing `main.json`. Note that the configured output file name (`documentation.output.file`)
is not returned, so a client that wants to reproduce `bicep docs generate` file naming must choose
its own.

The method supports the experimental `bicep docs` command group, but its JSON-RPC contract follows
the same cross-version compatibility requirements as the rest of the interface.

The same model builder and renderer are available directly from `Bicep.Core` through
`IBicepDocumentationGenerator`.

### `bicep/generateDocs`

Request parameters:

| Field | Type | Description |
| :-- | :-- | :-- |
| `path` | string | Bicep file path to render. |

Template and example settings are resolved from the Bicep file's `bicepconfig.json`.

The response contains `success`, `diagnostics`, and `contents`. `contents` is `null` when compilation fails.

Request:

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "bicep/generateDocs",
  "params": {
    "path": "/repo/modules/storage/main.bicep"
  }
}
```

Response:

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "success": true,
    "diagnostics": [],
    "contents": "# Storage Account\n\nDeploys a storage account.\n"
  }
}
```

### Calling from C#

The `Azure.Bicep.RpcClient` package wraps this method. It requires Bicep CLI 0.47.0 or later.

```csharp
using var client = await factory.Initialize(new BicepClientConfiguration(), cancellationToken);

var rendered = await client.GenerateDocs(
    new GenerateDocsRequest("./modules/storage/main.bicep"),
    cancellationToken);

if (rendered.Success)
{
    // The client owns the filesystem - nothing is written by the RPC server.
    await File.WriteAllTextAsync("./modules/storage/README.md", rendered.Contents, cancellationToken);
}
```
