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

- `--template-file <path>`
- `--template-root <path>`
- repeatable `--custom-template-value key=value`
- `--no-restore`
- `--diagnostics-format default|sarif`

`docs generate` also accepts `--pattern` and `--output-file`. The output value must be a file name without a directory or Bicep source extension.

When a directory is supplied, the command uses its `main.bicep`. The default template root is the module directory.

## Usage examples

The built-in Markdown template discovers Bicep files below `examples` and `tests`. Files named `dependencies.bicep` are excluded.

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
