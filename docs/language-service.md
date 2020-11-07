# Key features of the Bicep language service

The bicep language service (implemented in the [bicep VS Code extension](./installing.md#install-the-bicep-vs-code-extension)) is capable of many of the features you would expect out of other language tooling. Here's all of the features that are currently implemented.

## Validation and type checking

The bicep compiler has a type system that ensure your code is authored correctly. We will check the basic types that are built into the bicep language, as well as types of returned functions, and types of resources. Depending on the type of validation, you will see either a warning in yellow which will successfully compile with `bicep build` or you will see an error in red which will fail to compile.

## Intellisense

* code completions
  * dot-property access
    * works on any `param` or `var` object, `resource` or `module`
  * resource property names & property values
    * works for every `resource` and `module` (e.g. `myModule.outputs.`)
  * resource types

### Snippets

## Code navigation

### Go to definition

### Find all references

## Refactoring

### Rename symbol

### Formatting

* `ctrl` + `shift` + `f` (windows)
* supports `.editorconfig`
* sets its own setting in the `.vscode` directory

### Quick fixes

* use suggestion on spelling/casing issues
