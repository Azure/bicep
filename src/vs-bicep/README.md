## This extension requires Visual Studio 17.10 or above.

Previous versions (before v0.29) of the Bicep for Visual Studio extension supported VS 17.3 through 17.9 and can be downloaded here: https://github.com/Azure/bicep/releases/tag/v0.28.1.  Under Assets, download and double-click on vs-bicep.vsix to install. It's safe to install a different version of the Bicep extension in VS than the version of Bicep you have otherwise installed on the machine, but the extension inside VS will not understand more recent additions to the Bicep language, so upgrading to VS 17.10 or above is recommended.

## Key features of the Bicep Visual Studio extension
The Bicep Visual Studio extension is capable of many of the features you would expect out of other language tooling. Here is a comprehensive list of the features that are currently implemented.

### Validation
The Bicep compiler validates that your code is authored correctly. We always validate the syntax of each file and whenever possible also validate the return types of all expressions (functions, resource bodies, parameters, outputs, etc.). Bicep is more restrictive than ARM Templates, so certain behaviors in ARM Templates that you have used may not be supported and result in an error in Bicep. For example, we no longer allow math functions like add() because we support the + operator.
See Bicep Type System for more information about Bicep data types and the type validation rules.

## IntelliSense
Bicep provides IntelliSense for the core language and extends to support type definitions for all resource types in Azure.

### Dot-property access
Type . for any object to view and autocomplete its properties. Works on any param or var of type object, and any resource or module properties (e.g. myModule.outputs.).

![1_DotPropertyAccess.gif](1_DotPropertyAccess.gif)

### Resource property names & property values

Bicep knows the allowed properties and values for any resource or module declaration.

![2_ResourcePropertyNamesAndValues.gif](2_ResourcePropertyNamesAndValues.gif)

### List all available resource types

Easily explore all available resource types and api versions for a given type. You can type partial fragments of the type and Bicep will narrow the list down accordingly.

![3_AllAvailableResourceTypes.gif](3_AllAvailableResourceTypes.gif)

### Other IntelliSense and completions

        • Keyword declarations. On an empty line you will get completions for all keywords (param, var, resource, etc.)
        • param and output types (i.e. param myParam)
        • target scopes (i.e. targetScope =)

### Snippets

Bicep has a small set of snippets for core language keywords (param, var, resource, module, output). The snippets are contextual, so they should only show up in the places they are valid. All of the ARM Template resource snippets available in the ARM Tools VS Code extension are available as Bicep resource snippets.

![4_Snippets.gif](4_Snippets.gif)

## Code navigation
The Bicep language service supports document symbols, which help power a broad set of code navigation features.

### Go to definition, peek definition

![5_GotoDefinition.gif](5_GotoDefinition.gif)

### Find all references, peek references

![6_FindAllReferences.gif](6_FindAllReferences.gif)

### Highlights

When your cursor is on or in a particular symbol, Bicep will highlight other uses of that symbol. The color of the highlight is different for declarations of a symbol as opposed to accessing a symbol.

![7_Highlights.gif](7_Highlights.gif)

### Hovers

![8_Hover.gif](8_Hover.gif)

## Refactoring
### Rename symbol

You can rename any symbol such as a param or resource and Bicep will intelligently rename all the uses of that symbol.

![9_Rename.gif](9_Rename.gif)

### Formatting
We support the Format Document command and we insert spaces with a default tab size of 2.

![10_Formatting.gif](10_Formatting.gif)

You can change the default settings via Tools -> Options.

### Quick fixes
For small issues like misspelled symbols or incorrect casing, Bicep will offer a "Quick fix" option.

![11_QuickFix.gif](11_QuickFix.gif)

