# Generate module documentation

The experimental `docs` command group renders documentation from a compiled Bicep module.

## Commands

Generate `README.md` next to one module:

```powershell
bicep docs generate .\main.bicep
```

Generate documentation for multiple module entrypoints:

```powershell
bicep docs generate --pattern '.\modules\**\main.bicep'
```

Render one module to stdout without writing a file:

```powershell
bicep docs output .\main.bicep
```

Both commands accept:

- `--config-file-path <path>`
- `--template-file <path>`
- `--template-root <path>`
- repeatable `--custom-template-value key=value`
- repeatable `--custom-template-value-file-path <path>`
- `--no-restore`
- `--diagnostics-format default|sarif`

`docs generate` also accepts `--pattern`, `--outdir`, and `--outfile`. Without an output option, it writes `README.md` beside each input module. With `--pattern`, `--outdir` preserves the matched relative directory structure and writes `README.md` in each destination directory.

Inputs may be `.bicep` files or module directories. Directory inputs use the configured `entryPoint`, which defaults to `main.bicep`. The default template root is the input file's directory.

## Docs configuration

`--config-file-path` loads optional JSON configuration. Explicit CLI options override configuration values, and configuration values override built-in defaults.

```json
{
  "entryPoint": "main.bicep",
  "output": {
    "file": "README.md"
  },
  "template": {
    "file": "templates/readme.scriban",
    "includeRoot": ".",
    "values": {
      "owner": "Platform Team"
    }
  },
  "examples": {
    "sources": [
      {
        "path": "examples",
        "include": [
          "*.bicep",
          "**/main.bicep"
        ],
        "exclude": [
          "**/dependencies*.bicep"
        ]
      },
      {
        "path": "tests",
        "include": [
          "**/*.test.bicep"
        ],
        "exclude": [
          "**/dependencies*.bicep"
        ]
      }
    ],
    "reassignments": [
      {
        "from": {
          "include": [
            "**/rg-scope.*/**"
          ]
        },
        "to": "rg-scope"
      }
    ]
  }
}
```

Configuration sections are optional:

- `entryPoint` is used only when the input is a directory.
- `output.file` sets the generated file name. `--outfile` and `--outdir` still take precedence.
- `template.file` and `template.includeRoot` are resolved relative to the configuration file.
- `template.values` supplies baseline string values. Repeatable CLI value files and inline values are applied afterward in command-line order.
- `examples.sources` replaces the default sources when present. An empty array disables discovery.
- Source paths are relative to each module root. Include and exclude values are case-insensitive glob patterns.
- `examples.reassignments[].from` matches discovered example paths relative to their source.
- `examples.reassignments[].to` identifies one direct child module directory. Matching examples are removed from the parent and added to that child. A reassignment with no matching structure is a no-op.

## Usage examples

By default, the built-in Markdown template discovers direct `.bicep` files and nested `main.bicep` files below `examples`, plus `*.test.bicep` files below `tests`. Files whose names start with `dependencies` are excluded. These defaults can be replaced through `examples.sources`.

An example name is selected in this order:

1. Literal `metadata name`.
2. The file's containing folder.
3. The file name when it is directly below `examples` or `tests`.

Literal `metadata description` provides the description. Otherwise, leading `//` comments are used. The built-in template numbers headings so repeated display names remain unambiguous.

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

Includes resolve from the module directory unless `--template-root` is supplied. Relative traversal such as `../shared/notes.md` is supported.

Values supplied with `--custom-template-value owner=Platform` are available as both `custom.owner` and `module.custom.owner`.

Value files must contain a JSON object whose values are strings:

```json
{
  "owner": "Platform Team",
  "supportUrl": "https://contoso.example/support"
}
```

Files and individual values may be repeated and interleaved. They are applied from left to right, so the last occurrence of a key wins:

```powershell
bicep docs output .\main.bicep `
  --template-file .\readme.scriban `
  --custom-template-value-file-path .\defaults.json `
  --custom-template-value owner="Application Team" `
  --custom-template-value-file-path .\environment.json
```

In this example, `environment.json` has the final precedence for keys it contains.

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

Secure object schemas are expanded so templates can document property names, types, and descriptions. Runtime parameter values are never part of the documentation model.

### Outputs

Each `module.outputs` item contains `name`, `type`, `secure`, and `description`.

### Exported functions

Each `module.exportedFunctions` item contains `name`, `parameters`, `returnType`, and `description`. Function parameters contain `name`, `type`, and `description`.

### References

Each `module.references` item contains `symbolicName`, `path`, and `description`.

### Usage examples

Each `module.usageExamples` item contains `name`, `path`, `description`, `contents`, and a Markdown-safe `fence`.

## JSON-RPC

Long-lived clients can use:

- `bicep/generateDocs` for one or more file-oriented results.
- `bicep/outputDocs` for one stdout-oriented result.

Each result contains the input path, optional output path, success state, diagnostics, and rendered contents. Documentation-specific errors use:

| Code | Meaning |
| :-- | :-- |
| `DOCS001` | Invalid input, option, feature configuration, or compilation setup. |
| `DOCS002` | Output write failure. |
| `DOCS003` | Documentation model or template rendering failure. |
