# ARM Template syntax and the native Bicep equivalent

In Bicep, we have tried to make some common syntax in ARM templates that is quite verbose and change it to a terser equivalent. This doc is a simple table to look at all of the changes in one place.

Scenario | ARM Template | Bicep
--- | --- | ---
Author an expression | `[func()]` | `func()` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/expressions.md))
Concatenate strings together | `concat('John', ' ', parameters('lastName'))`| `'John ${lastName}'` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/bicep.md#strings))
Return the logical AND of multiple boolean values | `and(parameter('isMonday'), parameter('isNovember'))` | `isMonday && isNovember` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/expressions.md#binary-operators))
Get the resource ID of a resource declared in the template | `resourceId('microsoft.network/virtualNetworks')` | `res.id`
Get a property (`resourceProperty`) from a created resource (assumes `resource` named `res` has been declared in bicep) | `reference(parameters('resourceName')).properties.resourceProperty` | `res.properties.resourceProperty`
Conditionally declare a property value | `if(parameters('isMonday'), 'valueIfTrue', 'valueIfFalse')` | `isMonday ? 'valueIfTrue' : 'valueIfFalse'` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/expressions.md#ternary-operator))
Separate a solution into multiple files | Use [linked templates](https://docs.microsoft.com/azure/azure-resource-manager/templates/linked-templates#linked-template) | Use [modules](https://github.com/Azure/bicep/blob/main/docs/spec/modules.md)
Set the target scope of the deployment to a subscription | `"$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#"` | `targetScope = 'subscription'` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/resource-scopes.md#declaring-the-target-scopes))
Set a dependency between two resources | `"dependsOn": ["[resourceId('Microsoft.Storage/storageAccounts', 'parameters('storageAcountName'))]"`] | Either dependsOn not needed because of [auto-dependency management](https://github.com/Azure/bicep/blob/main/docs/spec/resources.md#implicit-dependency) or manually set dependsOn with `dependsOn: [ stg ]` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/resources.md#resource-dependencies))

## Incorporating new syntax into best practices

* Avoid the `reference()` and `resourceId()` functions like the plague. Anytime the resource you are referencing is declared in the same bicep project, you can pull the equivalent information from the resource identifier in bicep (i.e. `stg.id` or `stg.properties.primaryEndpoints.blob`). This also creates an [**implicit dependency**](https://github.com/Azure/bicep/blob/main/docs/spec/resources.md#implicit-dependency) between resources, which means you can eliminate your usage of the `dependsOn` property. All of this results in cleaner, more maintainable code.
* Use consistent casing for identifiers. When in doubt, use [camel case](https://en.wikipedia.org/wiki/Camel_case) (e.g. `param myCamelCasedParameter string`)
* If you are going to add a `description` to a parameter, ensure the parameter is in fact descriptive. If you have a `location` parameter, having a description of "the resource's location" is not particularly helpful and results in your code being noisier. Sometimes a `//` comment is more appropriate.
