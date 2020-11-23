# Key features of the Bicep VS Code extension

The [bicep VS Code extension](/docs/installing.md#install-the-bicep-vs-code-extension) is capable of many of the features you would expect out of other language tooling. Here is a comprehensive list of the features that are currently implemented.

## Validation

The bicep compiler validates that your code is authored correctly. We always validate the syntax of each file and whenever possible also validate the return types of all expressions (functions, resource bodies, parameters, outputs, etc.). Depending on the type of validation, you will see either a warning in yellow which will successfully compile with `bicep build` or you will see an error in red which will fail to compile either. Bicep is more restrictive than ARM Templates, so certain behaviors in ARM Templates that you have used may not be supported and result in an error in bicep. For example, we no longer allow math functions like `add()` because we support the `+` operator.

See [Bicep Type System](/docs/spec/types.md) for more information about Bicep data types and the type validation rules.

## Intellisense

Bicep provides intellisense for the core language and extends to support type definitions for all resource types in Azure.

### Dot-property access

Type `.` for any object to view and autocomplete its properties. Works on any `param` or `var` of type `object`, and any `resource` or `module` properties (e.g. `myModule.outputs.`).

![intellisense being displayed for property access of a bicep resource](/docs/images/resource-dot-property-intellisense.gif)

### Resource property names & property values

Bicep knows the allowed properties and values for any `resource` or `module` declaration.

![intellisense being displayed for available property names and property values where applicable of a bicep resource](/docs/images/resource-property-names-and-values.gif)

### List all available resource types

Easily explore all available resource types and api versions for a given type. You can type partial fragments of the type and bicep will narrow the list down accordingly.

![intellisense being displayed for all available types for a resource](/docs/images/list-types-intellisense.gif)

### Other intellisense and completions

* Keyword declarations. On an empty line you will get completions for all keywords (`param`, `var`, `resource`, etc.)
* `param` and `output` types (i.e. `param myParam `)
* target scopes (i.e. `targetScope = `)

### Snippets

Bicep has a small set of snippets for core language keywords (`param`, `var`, `resource`, `module`, `output`). The snippets are contextual, so they should only show up in the places they are valid. We plan to convert all of the ARM Template resource snippets that are used by the ARM Tools VS Code extension into bicep snippets.

![snippets for top level keywords](/docs/images/snippets.gif)

## Code navigation

The bicep language service supports document symbols, which help power a broad set of code navigation features.

### Go to definition, peek definition

![navigating from parameter used as a property value to the parameter declaration](/docs/images/go-to-def.gif)

### Find all references, peek references

![showing all times a particular symbol is referenced](/docs/images/show-all-references.gif)

### Outline view and breadcrumb view

![screenshot of vs code with the Outline panel and breadcrumb highlighted](/docs/images/outline-and-breadcrumb.PNG)

### Highlights

When your cursor is on or in a particular symbol, bicep will highlight other uses of that symbol. The color of the highlight is different for declarations of a symbol as opposed to accessing a symbol.

![showing the cursor on a symbol and the other references that are automatically highlighted](/docs/images/highlights.gif)

### Hovers

![showing the mouse hovering over a symbol, which shows type information on a pop-up window](/docs/images/hovers.gif)

## Refactoring

### Rename symbol

You can rename any symbol such as a `param` or `resource` and bicep will intelligently rename all the uses of that symbol

![rename a symbol called 'pip' into a symbol named 'publicIp' with the 'Rename symbol' feature](/docs/images/rename-symbol.gif)

### Formatting

* Default keybinding is `alt` + `shift` + `f` on Windows, `option` + `shift` + `f` on macOS
  * You can also format via the VS Code UI. `View` -> `Command palette...` then type `format document`

![formatting a bicep file that is disorganized](/docs/images/format.gif)

* Bicep will set the following default settings for `.bicep` files when installed:

```json
  "[bicep]": {
    "editor.tabSize": 2,
    "editor.insertSpaces": true
  }
```

You can change the default settings in the following places (sorted by precedence in ascending order):

* VSCode global user settings
* VSCode workspace settings
* `.editorconfig` files (requires EditorConfig for VSCode extension to be installed)

### Quick fixes

For small issues like misspelled symbols or incorrect casing, bicep will offer a "Quick fix" to fix it for you.

![correcting a spelling error with "quick fix"](/docs/images/quick-fix.gif)
