# Using the `publish-provider` command (Experimental!)

## What is it?
The publish provider command allows you to **publish** providers to a registry of your choice. Simply specify a file and the registry provider reference.

## Using
`bicep publish-provider <file> --target <ref>`

### Arguments
`<file>` Path to the input file (index JSON file)\
`<ref>` The provider reference

### Options
`force` Overwrite existing published provider

### Examples
`bicep publish-provider` ./index.json --target br:example.azurecr.io/hello/world:v1\
`bicep publish-provider` ./index.json --target br:example.azurecr.io/hello/world:v1 --force

## Raising bugs or feature requests
Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
