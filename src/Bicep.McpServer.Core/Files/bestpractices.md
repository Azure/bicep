# Bicep best-practices
This list of best-practices builds on top of information available at https://learn.microsoft.com/azure/azure-resource-manager/bicep. It provides a more opinionated and up-to-date set of rules for generating high-quality Bicep code. You should aim to follow these rules whenever generating or modifying Bicep code.

## Rules
### General
1. Avoid setting the `name` field for `module` statements - it is no longer required.
1. If you need to input or output a set of logically-grouped values, generate a single `param` or `output` statement with a User-defined type instead of emitting a `param` or `output` statement for each value.
1. If generating parameters, default to generating Bicep parameters files (`*.bicepparam`), instead of ARM parameters files (`*.json`).

### Resources
1. Do not add references from child resources to parent resources by using `/` characters in the child resource `name` property. Instead, use the `parent` property with a symbolic reference to the parent resource.
1. If you are generating a child resource type, sometimes this may require you to add an `existing` resource for the parent if the parent is not already present in the file.
1. If you see diagnostic codes `BCP036`, `BCP037` or `BCP081`, this may indicate you have hallucinated resource types or resource type properties. You may need to double-check against available resource type schema to tune your output.
1. Avoid using multiple `resourceId()` functions and `reference()` function where possible. Instead use symbolic names to refer to ids or properties, creating `existing` resources if needed. For example, write `foo.id` or `foo.properties.id`, instead of `resourceId('...')` or `reference('...').id`.

### Types
1. Avoid using open types such as `array` or `object` when referencing types where possible (e.g. in `output` or `param` statements). Instead, use User-defined types to define a more precise type.
1. Use typed variables instead of untyped variables when exporting values with the `@export()` decorator. For example, use `var foo string = 'blah'` instead of `var foo = bar`.
1. When using User-defined types, aim to avoid repetition, and comment properties with `@description()` where the context is unclear.
1. If you are passing data directly to or from a resource body via a `param` or `output` statement, try to use existing Resource-derived types (`resourceInput<'type@version'>` and `resourceOutput<'type@version'>`) instead of writing User-defined types.

### Security
1. When generating `param` or `output` statements, ALWAYS use the `@secure()` decorator if sensitive data is present.

### Syntax
1. If you hit warnings or errors with null properties, prefer solving them with the safe-dereference (`.?`) operator, in conjunction with the coalesce (`??`) operator. For example, `a.?b ?? c` is better than `a!.b` which may cause runtime errors, or `a != null ? a.b : c` which is unnecessarily verbose.

### Extensions
1. When creating Bicep files that use extension resources (e.g., Microsoft Graph), two things are required:
   - An `extension` statement in the `.bicep` file (e.g., `extension microsoftGraphV1_0`). The identifier must match the key in `bicepconfig.json`.
   - A `bicepconfig.json` file in the current directory or an ancestor directory that maps the extension identifier to its OCI artifact reference.
   If no `bicepconfig.json` exists, create one. For example, for Microsoft Graph v1.0:
   ```json
   {
     "extensions": {
       "microsoftGraphV1_0": "br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:1.0.0"
     }
   }
   ```
1. Use the `list_well_known_extensions` MCP tool to discover available extensions and their versions before configuring `bicepconfig.json`.

### Formatting
1. Only include comments if they provide additional context. Use simple single line comments for smaller blocks of text, e.g. `// this is a comment`, and avoid adding unnecessary demarcation - e.g. `// ====`.

## Glossary
* Child resource: an Azure resource type with type name consisting of more than 1 `/` characters. For example, `Microsoft.Network/virtualNetworks/subnets` is a child resource. `Microsoft.Network/virtualNetworks` is not.
