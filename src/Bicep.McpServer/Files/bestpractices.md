This best-practices doc builds on top of information available at https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep.

## Rules
Here are some general-purpose best-practices for authoring high-quality Bicep code.
1. Avoid using open types such as `array` or `object` when referencing types where possible (e.g. in `output` or `param` statements). Instead, use User-Defined Types to define a more precise type.
1. Do not add references from child resources to parent resources by using `/` charaters in the child resource `name` property. Instead, use the `parent` property with a symbolic reference to the parent resource.
1. If you are generating a child resource type, Sometimes this may require you to add an `existing` resource for the parent if the parent is not already present in the file.
1. Avoid setting the `name` field for `module` statements - it is no longer required.
1. When generating `param` or `output` statements, ALWAYS use the `@secure()` decorator if sensitive data is present.

## Glossary
* Child resource: an Azure resource type with type name consisting of more than 1 `/` characters. For example, `Microsoft.Network/virtualNetworks/subnets` is a child resource. `Microsoft.Network/virtualNetworks` is not.