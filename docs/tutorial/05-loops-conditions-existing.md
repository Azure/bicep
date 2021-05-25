# Advanced resource declarations with loops, conditions, and "existing"

As your Bicep files get more complex, you may find you need more complex logic for how resources are deployed. This tutorial walks you through more advanced resource declaration including:

* Getting a reference to an existing resource
* Creating a set of resources based on a list or count (loops)
* Conditionally deploying a resource


## "existing" keyword

In our previous example, we created both a new storage account and new blob container inside of that storage account. However, we may want to add a blob container to an already existing storage account. In order to do this in bicep, we use the `existing` keyword which is appended to the resource declaration like so:

```Bicep
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: storageAccountName
}
```

Notice all the other properties have been removed because the resource already exists, and we don't want or need to change them. We just need enough information to construct the resource ID of the existing resource we are looking for. This is the equivalent of using the `reference()` function and passing a complete resource ID as the argument. The advantage of the new existing syntax is we have a persistent symbolic name that we can continue to reference, which is not possible today in ARM Templates.

>**Note:** Resources declared with the `existing` keyword support an optional `scope` property if the resource does not exist in the same scope as the target scope of the Bicep file being deployed.

Now we can update our file accordingly:

```bicep
param storageAccountName string // need to be provided since it is existing

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: storageAccountName
}

resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${stg.name}/default/logs'
  // dependsOn will be added when the template is compiled
}

output storageId string = stg.id // replacement for resourceId(...)
output primaryEndpoint string = stg.properties.primaryEndpoints.blob // replacement for reference(...).*
```

We've gotten rid of some `params` that were only needed to create the resource, but notice our symbolic reference to the storage account (`stg`) works exactly the same as before.

## Conditions

Just like we can conditionally set a property with the ternary operator, we can conditionally deploy the entire resource based on a condition with the `if` keyword, which is added after the initial resource declaration (after the `=`). Conditions have the following structure:

```bicep
resource foo 'my.provider/type@2021-03-01' = if(<BOOLEAN>) {...}
```

Let's conditionally deploy our storage blob only if it is the year 2021:

```bicep
param currentYear string = utcNow('yyyy') // format utc time to year only

resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = if(currentYear == '2021') {
  name: '${stg.name}/default/logs'
  // dependsOn will be added when the template is compiled
}
```

## Loops

Finally, if I want to deploy not just one storage container, but rather some arbitrary number of containers, I can add a loop statement using the `for` keyword. Loops follow the structure:

```bicep
resource foo 'my.provider/type@2021-03-01' = [for <ITERATOR_NAME> in <ARRAY> = {...}]
```

`ITERATOR_NAME` is a new symbol that is only available inside the body of the resource declaration. It can be any name you would like and represents the current item in the array.

Let's remove our condition and replace it with a loop based on an input of container names: 

```bicep
param containerNames array = [
  'dogs'
  'cats'
  'fish'
]

resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = [for name in containerNames: {
  name: '${stg.name}/default/${name}'
  // dependsOn will be added when the template is compiled
}]
```

There's also a [loop index variant](../spec/loops.md#use-the-loop-index) which gives us access to the current item's index in the array:
```bicep
resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = [for (name, index) in containerNames: {
  name: '${stg.name}/default/${name}-${index + 1}'
}]
```

This change also implicitly changes the type of the `resource` into an array of resources (will show up as `resources[]` on hover), rather than a single resource. In order to access one of the resources, I use array access syntax just like I do for any array in Bicep. Let's add an output that emits the id of each blob container.

```bicep
resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = [for name in containerNames: {
  name: '${stg.name}/default/${name}'
  // dependsOn will be added when the template is compiled
}]

output containerProps array = [for i in range(0, length(containerNames)): blob[i].id]
```

>**Note:** As of the 3/1/2021 with the 0.3 initial release, loops cannot be combined with conditions. The fix is being tracked with [#1667](https://github.com/Azure/bicep/issues/1667)

## Next steps

In the next tutorial, we will learn how to consume our current Bicep file as a `module`:

[6 - Creating and consuming modules](./06-creating-modules.md)
