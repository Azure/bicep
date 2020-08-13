# Working with a basic bicep file

In the previous step, we compiled the most basic bicep file -- a blank template. Now let's add a `resource` to our `main.bicep` bicep file:

## Add a resource

```
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: 'uniquestorage001' // must be globally unique
    location: 'eastus'
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}
```

The resource declaration has four components:

* `resource` keyword
* **symbolic name** (`stg`) - this is an identifier for referencing the resource throughout your bicep file. It is *not* what the name of the resource will be when it's deployed.
* **resource type** (`Microsoft.Storage/storageAccounts@2019-06-01`) - composed of the resource provider (`Microsoft.Storage`), resource type (`storageAccounts`), and apiVersion (`2019-06-01`). These properties should be familiar if you've ever deployed ARM Templates before.
* **resource properties** (everything inside `= {...}`) - these are the specific properties you would like to specify for the given resource type. These are *exactly* the same properties available to you in an ARM Template.

When we compile the template with `bicep build main.bicep`, we see the following JSON:

```json
// todo - waiting for above to compile
```

At this point, I can deploy it like any other ARM template using the standard command line tools (`az deployment group create ...` or `New-AzResourceGroupDeployment ...`).

```bash
az deployment group create -f ./main.json -g my-rg
```

## Add parameters

In most cases, I want to expose a part of the resource name and the resource location via parameters, so I can add the following:

```
parameter location string = 'eastus'
parameter name string = 'uniquestorage001' // must be globally unique

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: name
    location: location
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}
```

Notice the `parameters` can be referenced directly via their name in bicep, compared to requiring `[parameters('location')]` in ARM template JSON.

The end of the parameter declaration (`= 'eastus'`) is only the default value and can be optionally overridden at deployment time.

Let's compile with `bicep build main.bicep` and look at the output:

```json
//todo
```

## Add variables and outputs

I can also add `variables` for storing values or complex expressions, and emit `outputs` to be passed to a script or another template:

```
parameter location string = 'eastus'
parameter name string = 'uniquestorage001' // must be globally unique

variable storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: storageSku // reference variable
    }
}

output storageId string = stg.id // output resourceId of storage account
```

Notice I can easily reference the resourceId from the symbolic name of the storage account (`stg.id`) which we will translate to the `resourceId(...)` function in the compiled template. When compiled, you should see the following ARM Template JSON:

```json
// todo
```

## Next steps

In the next tutorial, we will start working with more advanced bicep expressions:

[2 - Using 'advanced' expressions](./02-using-expressions.md)
