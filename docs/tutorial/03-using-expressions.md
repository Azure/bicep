# Using Bicep expressions

In the first tutorial, we declared a basic storage account resource and augmented that declaration with references to declared parameters and variables. These references are forms of **expressions**.

Even though the bicep language is still in its infancy, you can already write some powerful expressions to take advantage of in your bicep files.

## Using a bicep function

Any valid [ARM Template function](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions) is also a valid bicep function. Resource functions do not yet understand new concepts like the symbolic resource name, so they still require a resourceId.

The following are all valid function calls in bicep:

```
param serverFarmId string = resourceId('Microsoft.Web/sites', 'myWebsite')

var location = resourceGroup().location

output makeCapital string = toUpper('all lowercase')
```

In our `main.bicep` file, instead of forcing users to guess a unique storage account name, let's get rid of our `name` parameter and use the `uniqueString()` and `resourceGroup()` functions to calculate a unique name. We'll also use the `resourceGroup().location` property instead of hardcoding a default location.

```
param location string = resourceGroup().location

var storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: uniqueString(resourceGroup().id) // generates unique name based on resource group ID
    location: location
    kind: 'Storage'
    sku: {
        name: storageSku // assign variable
    }
}

output storageId string = stg.id
```

## Using string interpolation

The `concat()` function is one of the most commonly used ARM Template functions and can add a lot of verbosity to a template. To simplify this, we now support a [string interpolation](https://en.wikipedia.org/wiki/String_interpolation#) syntax.

For example, I could combine a `namePrefix` parameter with a hardcoded suffix:

```
param namePrefix string = 'unique'

var storageAccountName = '${namePrefix}storage001'
```

Which is equivalent to the following ARM Template `concat()` function:

```json
{
  "variables": {
    "storageAccountName": "[concat(parameters('namePrefix'), 'storage001')]"
  }
}
```

Let's use a `namePrefix` parameter and concatenate that with our `uniqueString()` using string interpolation. We'll also use the variable `storageAccountName` to store this expression, so that our resource declaration is a bit cleaner:

```
param location string = resourceGroup().location
param namePrefix string = 'stg'

var storageSku = 'Standard_LRS' // declare variable and assign value
var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

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

You can conditionally provide a value for a variable, resource, or output using the [ternary operator](https://en.wikipedia.org/wiki/%3F:), which is the equivalent of the `if()` function in ARM Templates. Instead of using a variable for our storage sku, let's conditionally choose a redundancy setting for our storage account by adding a new parameter `globalRedundancy` and combining it with the ternary operator. As part of that, we can get rid of our `storageSku` variable.

```
param location string = resourceGroup().location
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

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

## Next steps

In the next tutorial, we will learn how use the resource's **symbolic name** for simple property references and calculating dependencies:

[4 - Using the symbolic resource name](./04-using-symbolic-resource-name.md)
