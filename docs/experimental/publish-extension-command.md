# Using the `publish-extension` command (Experimental!)

## What is it?
The `publish-extension` command allows you to **publish** extensions to a registry of your choice. Simply specify a file and the registry extension reference.

## Using
`bicep publish-extension [<file>] --target <ref>`

### Arguments
`<file>` Path to the input file (index JSON file). This is optional for [local deploy extensions](./local-deploy.md).\
`<ref>` The extension reference

### Options
`force` Overwrite existing published extension

### Examples
`bicep publish-extension` ./index.json --target br:example.azurecr.io/hello/world:v1\
`bicep publish-extension` ./index.json --target br:example.azurecr.io/hello/world:v1 --force

## Raising bugs or feature requests
Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
