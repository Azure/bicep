# Using 'advanced' expressions

In the previous main.arm template, we declared a basic storage account resource and augmented that declaration with parameters, variables, and outputs.

While the bicep language is still in its infancy, there are some more advanced expressions you can already take advantage of in your files.

## Using a bicep function

Any valid [ARM Template function](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions) is also a valid bicep function. Resource functions do not yet understand new concepts like the resource identifier, so they still require a resourceId.

The following are all valid function calls in bicep:

```
parameter currentTime string = utcNow() // only valid as a default value of a parameter

variable location = resourceGroup().location

output makeCapital string = toUpper('all lowercase')
```

In our `main.arm` file, we are already using two functions (`uniqueString()` and `resourceGroup()`).

## Using the ternary operator

You can conditionally provide a value for a variable, resource, or output using the [ternary operator](https://en.wikipedia.org/wiki/%3F:), which is the equivalent of the `if()` function in ARM Templates. Let's conditionally choose a redundancy setting for our storage account by adding a new parameter `globalRedundancy` and combining it with the ternary operator:

```
parameter location string = 'eastus'
parameter namePrefix string = 'stg'

parameter globalRedundancy bool = true // defaults to true, but can be overridden

variable storageAccountName = '${namePrefix}-${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: globalRedundancy ? 'Standard_GRS' : 'Standard_LRS' // if true --> GRS, else --> LRS
    }
}

output storageId string = stg.id
```

## Referencing a resource identifier

With the resource's symbolic name, it is much easier to retrieve properties of a resource. Instead of using the `reference()` or `resourceId()` function, you can simply use the identifier and retrieve the relevant property. We've already done this with our output `stg.id`.

Let's add another output to retrieve the `primaryEndpoint` of our storage account:

```
parameter location string = 'eastus'
parameter namePrefix string = 'stg'

parameter globalRedundancy bool = true // defaults to true, but can be overridden

variable storageAccountName = '${namePrefix}-${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: globalRedundancy ? 'Standard_GRS' : 'Standard_LRS' // if true --> GRS, else --> LRS
    }
}

output storageId string = storage.id // replacement for resourceId(...)
output primaryEndpoint string = storage.primaryEndpoints.blob // replacement for reference(...).*
```

## Next steps

In the final tutorial, we will learn how to convert an arbitrary ARM template into a bicep file:

[3 - Convert any ARM template into a Bicep file](./03-convert-arm-template.md)
