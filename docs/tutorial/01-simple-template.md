# Working with a basic bicep file


In the previous step, we compiled the most basic bicep file -- a blank template. Now let's add a resource to our `main.arm` bicep file:

## Add a resource

```
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: 'stg-${uniqueString(resourceGroup().id)}'
    location: 'eastus'
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}
```

The resource declaration has four components:
* `resource` keyword
* **resource identifier** (`storage`) - this is a symbolic name for referencing the resource. It is *not* what the name of the resource will be when it's deployed.
* **resource type** (`Microsoft.Storage/storageAccounts@2019-06-01`) - composed of the resource provider (`Microsoft.Storage`), resource type (`storageAccounts`), and apiVersion (`2019-06-01`). These properties should be familiar if you've ever deployed ARM Templates before.
* **resource properties** (everything inside `= {...}`) - these are the specific properties you would like to specify for the given resource type. These are *exactly* the same properities available to you in an ARM Template.

When we compile the template with `bicep build main.arm`, we see the following JSON. Notice the string interpolation in bicep gets translated to the `concat()` function in the ARM Template JSON:

```json
// todo
```

At this point, I can deploy it like any other ARM template using the standard command line tools (`az deployment group create ...` or `New-AzResourceGroupDeployment ...`).

## Add parameters

In most cases, I want to expose a part of the resource name and the resource location via parameters, so I can add the following:

```
parameter location string = 'eastus'
parameter namePrefix string = 'stg'

resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: '${namePrefix}-${uniqueString(resourceGroup().id)}'
    location: location
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}
```

Notice the `parameters` can be referenced directly in bicep, compared to requiring `parameters('location')` in ARM template JSON. Let's compile with `bicep build main.arm` and look at the output:

```json
//todo
```

## Add variables and outputs

I can also add `variables` for storing repeated values or complex expressions, and emit `outputs`:

```
parameter location string = 'eastus'
parameter namePrefix string = 'stg'

variable storageAccountName = '${namePrefix}-${uniqueString(resourceGroup().id)}'

resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}

output storageId string = storage.id
```

Notice I can easily reference the resourceId from the symbolic name of the storage account (`storage.id`) which we will translate to the `resourceId()` function in the compiled template. When compiled, you should see the following ARM Template JSON:

```json
// todo
```

