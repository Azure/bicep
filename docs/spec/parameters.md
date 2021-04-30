# Parameters
`param` declarations will be compiled into ARM template parameters. See [Template Parameter Syntax Reference](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-syntax#parameters) for more information.

There are no constraints on placement of parameter declarations. They can be mixed with any other valid declarations in any order and they do not have to appear at the top of the file.

## Minimal Declaration
These are smallest possible parameter declarations:
```bicep
param myString string
param myInt int
param myBool bool
param myObject object
param myArray array
```

## Default value
Default values can be declared as follows:
```bicep
param myParam string = 'my default value'
```

You may use [expressions](./expressions.md) with the `default` modifier.  Here is an example of a location parameter whose value defaults to the location of the current resource group if the parameter is not specified during the deployment:
```bicep
param myParam string = resourceGroup().location
```

A parameter cannot have the same name as a [variable](./variables.md), [resource](./resources.md), [output](./outputs.md) or another parameter in the same scope.

## Declaration with decorators
Decorators provide a way to attach constrains and metadata to a parameter. Decorators are placed above the parameter declaration to be decorated. They use the form @expression, where expression must be a function call:

```bicep
@expression
param myParam string
```

### Secure parameters
If you are familiar with ARM template parameters, you will notice a conspicuous absence of `secureString` and `secureObject` types. In this language, these types are annotated with the `@secure` decorator.

The following declarations will compile into a `secureString` and `secureObject` parameters, respectively.
```bicep
@secure()
param myPassword string

@secure()
param mySuperSecretObject object
```

### Allowed Values
You can constrain which values are allowed using the `@allowed` decorator:
```bicep
@allowed([
  'one'
  'two'
])
param myEnum string
```

The constraint will be evaluated at deployment time of the compiled template.

### String and array length constraint
Parameters of type `string` and `array` can have length constraints. The following declares a storage account name parameter of type strings whose length can only be between 3-24 characters (inclusive).
```bicep
@minLength(3)
@maxLength(24)
param storageAccountName string
```

The length constraint is evaluated at compiled template deployment time.

### Integer value constraint
Integer parameters can also have a value constraint. These are expressed as follows:
```bicep
@minValue(1)
@maxValue(12)
param month int
```

The value constraint is evaluated at compiled template deployment time.

### Metadata
Parameters of any type can have metadata. The following example shows how to attach a metadata object to a parameter:
```bicep
@metadata({
  author: 'Example Name'
})
param myParam string
```

### Description
Parameters of any type can have a description associated with them. This looks like the following:
```bicep
@description('There are many like this, but this object is mine.')
param myObject object
```

The `@metadata` decorator can be used to achieve the same goal, but it is a bit more verbose:
```bicep
@metadata({
  description: 'There are many like this, but this object is mine.'
})
param myObject object
```

### Combined decorators
If applicable to the parameter type, multiple decorators can be combined together. The following is an example of this:
```bicep
@minLength(3)
@maxLength(24)
@description('Name of the storage account')
param storageAccountName string = concat(uniqueString(resourceGroup().id), 'sa')
```
