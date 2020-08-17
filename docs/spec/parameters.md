# Parameters
> **Note**: Parameter modifiers are not implemented.

param declarations will be compiled into ARM template parameters. See [Template Parameter Syntax Reference](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-syntax#parameters) for more information.

There are no constraints on placement of parameter declarations. They can be mixed with any other valid declarations in any order and they do not have to appear at the top of the file.

## Minimal Declaration
These are smallest possible parameter declarations:
```
param myString string
param myInt int
param myBool bool
param myObject object
param myArray array
```

A parameter cannot have the same name as a [variable](./variables.md), [resource](./resources.md), or another parameter in the same scope.

## Secure parameters
If you are familiar with ARM template parameters, you will notice a conspicuous absense of `secureString` and `secureObject` types. In this language, these types are implemented as modifiers.

The following declarations will compile into a `secureString` and `secureObject` parameters, respectively.
```
param myPassword string { 
  secure: true
}

param mySuperSecretObject object { 
  secure: true
}
```
## Allowed Values
You can constrain which values are allowed using the `allowedValues` modifier:
```
param myEnum string {
  allowedValues: [
    'one'
    'two'
  ]
}
```

The constraint will be evaluated at deployment time of the compiled template.

## Default value
Default values can be declared as follows:
```
param myParam string = 'my default value'
```

If you need to combine a default value with other identifiers, you may also use the following syntax to achieve the same:
```
param myParam string {
  defaultValue: 'my default value'
}
```

You may use [expressions](./expressions.md) with the `defaultValue` modifier. (All other modifiers require a constant literal.) Here is an example of a location parameter whose value defaults to the location of the current resource group if the parameter is not specified during the deployment:
```
param myParam string {
  defaultValue: resourceGroup().location
}
```

## String and array length constraint
Parameters of type `string` and `array` can have length constraints. The following declares a storage account name parameter of type strings whose length can only be between 3-24 characters (inclusive).
```
param storageAccountName string {
  minLength: 3
  maxLength: 24
}
```

The length constraint is evaluated at compiled template deployment time.

## Integer value constraint
Integer parameters can also have a value constraint. These are expressed as follows:
```
param month int {
  minValue: 1
  maxValue: 12
}
```

The value constraint is evaluated at compiled template deployment time.

## Description
Parameters of any type can have a description associated with them. This looks like the following:
```
param myObject object {
  metadata: {
    description: "There are many like this, but this object is mine."
  }
}
```

## Combined modifiers
If applicable to the parameter type, multiple modifiers can be combined together. The following is an example of this:
```
param storageAccountName string {
  minLength: 3
  maxLength: 24
  defaultValue: concat(uniqueString(resourceGroup().id), 'sa')
  metadata: {
    description: "Name of the storage account"
  }
}
```