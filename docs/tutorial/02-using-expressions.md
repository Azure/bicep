# Using "advanced" expressions

In the previous tutorial, we declared a basic storage account resource and augmented that declaration with parameters, variables, and outputs.

While the bicep language is still in its infancy, there are some more advanced expressions you can already take advantage of in your files.

## Using a bicep function

Any valid [ARM Template function](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions) is also a valid bicep function. Resource functions do not yet understand new concepts like the symbolic resource name, so they still require a resourceId.

The following are all valid function calls in bicep:

```
param serverFarmId string = resourceId('Microsoft.Web/sites', 'myWebsite')

var location = resourceGroup().location

output makeCapital string = toUpper('all lowercase')
```

In our `main.bicep` file, instead of forcing users to guess a unique storage account name, let's use the `uniqueString()` and `resourceGroup()` functions to calculate a unique name:

```
param location string = 'eastus'

var storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: uniqueString(resourceGroup().id) // generates unique name based on resource group ID
    location: location
    kind: storageSku
    sku: {
        name: storageSku // assign variable
    }
}

output storageId string = stg.id
```

## Using string interpolation

The `concat()` function is one of the most commonly used ARM Template functions and can add a lot of verbosity to a template. To simplify this, we now support a [string interpolation](https://en.wikipedia.org/wiki/String_interpolation#:~:text=In%20computer%20programming%2C%20string%20interpolation,replaced%20with%20their%20corresponding%20values.) syntax. Let's add a `namePrefix` parameter and concatenate that with our `uniqueString()` using string interpolation. We'll also use the variable `storageAccountName` to store this expression, so that our resource declaration is a bit cleaner: 

```
param location string = 'eastus'
param namePrefix string = 'stg'

var storageSku = 'Standard_LRS' // declare variable and assign value
var storageAccountName = '${namePrefix}-${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: storageSku
    }
}

output storageId string = stg.id
```

If you compile with `bicep build`, you will notice we are compiling this into the `format()` function, not the `concat()` function. Functionally, the end result is the same, but the `format()` function is a bit more stable in the IL.

## Using the ternary operator

You can conditionally provide a value for a variable, resource, or output using the [ternary operator](https://en.wikipedia.org/wiki/%3F:), which is the equivalent of the `if()` function in ARM Templates. Instead of using a variable for our storage sku, let's conditionally choose a redundancy setting for our storage account by adding a new parameter `globalRedundancy` and combining it with the ternary operator:

```
param location string = 'eastus'
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}-${uniqueString(resourceGroup().id)}'

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

## Referencing the symbolic name of a resource

With the resource's symbolic name, it is much easier to retrieve properties of a resource. Instead of using the `reference()` or `resourceId()` function, you can simply use the identifier and retrieve the relevant property. We've already done this with our output `stg.id`.

Let's add another output to retrieve the `primaryEndpoint` of our storage account:

```
param location string = 'eastus'
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}-${uniqueString(resourceGroup().id)}'

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
