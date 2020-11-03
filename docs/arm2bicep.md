# ARM Template syntax and the native Bicep equivalent

In Bicep, we have tried to make some common syntax in ARM templates that is quite verbose and change it to a terser equivalent. This doc is a simple table to look at all of the changes in one place.

Scenario | ARM Template | Bicep
--- | --- | ---
Author an expression | `[func()]` | `func()` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/expressions.md))
Concatenate strings together | `concat('John', ' ', parameters('lastName'))`| `'John ${lastName}'` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/bicep.md#strings))
Return the logical AND of multiple boolean values | `and(parameter('isMonday'), parameter('isNovember'))` | `isMonday && isNovember` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/expressions.md#binary-operators))
Get the resource ID of a resource declared in the template | `resourceId('microsoft.network/virtualNetworks')` | `res.id`
Get a property (`resourceProperty`) from a created resource (assumes `resource` named `res` has been declared in bicep) | `reference(parameters('resourceName')).properties.resourceProperty` | `res.properties.resourceProperty`
Conditionally declare a property value | `if(parameters('isMonday'), 'valueIfTrue', 'valueIfFalse')` | `isMonday ? 'valueIfTrue' | 'valueIfFalse'` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/expressions.md#ternary-operator))
Separate a solution into multiple files | Use [linked templates](https://docs.microsoft.com/azure/azure-resource-manager/templates/linked-templates#linked-template) | Use [modules](https://github.com/Azure/bicep/blob/main/docs/spec/modules.md)
Set the target scope of the deployment to a subscription | `"$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#"` | `targetScope = 'subscription'` ([spec](https://github.com/Azure/bicep/blob/main/docs/spec/resource-scopes.md#declaring-the-target-scopes))
