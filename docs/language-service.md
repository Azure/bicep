# Key features of the Bicep language service

The bicep language service (implemented in the [bicep VS Code extension](./installing.md#install-the-bicep-vs-code-extension)) is capable of many of the features you would expect out of other language tooling. Here is a comprehensive list of the features that are currently implemented.

## Validation and type checking

The bicep compiler has a type system that ensures your code is authored correctly. We will check the basic types that are built into the bicep language (`string`, `int`, `object`), as well as types of returned functions, and types of resources. Depending on the type of validation, you will see either a warning in yellow, which will successfully compile with `bicep build` or you will see an error in red, which will block compilation because the compiled JSON would produce an invalid ARM Template.

## Intellisense

Bicep provides intellisense for the core language and extends to support type definitions for all resource types in Azure.

* dot-property access
  * works on any `param` or `var` of type `object` as well as `resource` and `module` properties (e.g. `myModule.outputs.`)
<!-- TODO: CREATE GIF HERE -->
* resource property names & property values
  * works for every `resource` and `module`
<!-- TODO: CREATE GIF HERE -->
* list resource types
<!-- TODO: CREATE GIF HERE -->
* keyword declarations
* `param` and `output` types
* target scopes

### Snippets

Bicep has a small set of snippets for core language keywords (`param`, `var`, `resource`, `module`, `output`). The snippets are contextual, so they should only show up in the places they are valid. We plan to convert all of the ARM Template resource snippets that are used by the ARM Tools VS Code extension into bicep snippets.
<!-- TODO: CREATE GIF HERE -->

## Code navigation

The bicep language service supports document symbols, which help power a broad set of code navigation features.

### Go to definition, peek definition
<!-- TODO: CREATE GIF HERE -->

### Find all references, peek references
<!-- TODO: CREATE GIF HERE -->

### Outline view and breadcrumb view
<!-- TODO: CREATE IMG HERE -->

### Highlights
<!-- TODO: CREATE GIF HERE -->

### Hovers
<!-- TODO: CREATE GIF HERE -->

## Refactoring

### Rename symbol
<!-- TODO: CREATE GIF HERE -->

### Formatting

* `alt` + `shift` + `f` on Windows, `option` + `shift` + `f` on macOS
<!-- TODO: CREATE GIF HERE -->
* Bicep will set the following default settings for `.bicep` files when installed:

```json
  "[bicep]": {
    "editor.tabSize": 2,
    "editor.insertSpaces": true
  }
```

You can change the default settings in teh following places (sorted by precedence in ascending order):

* VSCode global user settings
* VSCode workspace settings
* `.editorconfig` files (requires EditorConfig for VSCode extension to be installed)

### Quick fixes

* use suggestion on spelling/casing issues
<!-- TODO: CREATE GIF HERE -->
