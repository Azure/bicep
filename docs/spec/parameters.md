# Parameters
Parameter declarations will be compiled into ARM template parameters. See [Template Parameter Syntax Reference](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-syntax#parameters) for more information.

## Minimal Declaration
These are smallest possible parameter declarations:
```
parameter myString string
parameter myInt int
parameter myBool bool
parameter myObject object
parameter myArray array
```

## Secure parameters
If you are familiar with ARM template parameters, you will notice a conspicuous absense of `secureString` and `secureObject` types. In this language, these types are implemented as modifiers.

The following declarations will compile into a `secureString` and `secureObject` parameters, respectively.
```
parameter myPassword string { 
  secure: true
}

parameter mySuperSecretObject object { 
  secure: true
}
```
## Allowed Values
You can constrain which values are allowed using the `allowedValues` modifier:
```
parameter myEnum string {
  allowedValues: [
    'one'
    'two'
  ]
}
```

The constraint will be evaluated at deployment time of the compiled template.

## Default value
Default values will be declared as follows:

```
parameter myParam string {
  defaultValue: 'my default value'
}
```

You may use expressions with the `defaultValue` modifier. (All other modifiers require a constant literal.) Here is an example of a location parameter whose value defaults to the location of the current resource group if the parameter is not specified during the deployment:
```
parameter myParam string {
  defaultValue: resourceGroup().location
}
```

## String and array length constraint
Parameters of type `string` and `array` can have length constraints. The following declares a storage account name parameter of type strings whose length can only be between 3-24 characters (inclusive).
```
parameter storageAccountName string {
  minLength: 3
  maxLength: 24
}
```

The length constraint is evaluated at compiled template deployment time.

## Integer value constraint
Integer parameters can also have a value constraint. These are expressed as follows:
```
parameter month int {
  minValue: 1
  maxValue: 12
}
```

The value constraint is evaluated at compiled template deployment time.

## Description
Parameters of any type can have a description associated with them. This looks like the following:
```
parameter myObject object {
  metadata: {
    description: "There are many like this, but this object is mine."
  }
}
```

## Combined modifiers
If applicable to the parameter type, multiple modifiers can be combined together. The following is an example of this:
```
parameter storageAccountName string {
  minLength: 3
  maxLength: 24,
  defaultValue: concat(uniqueString(resourceGroup().id), 'sa'),
  metadata: {
    description: "Name of the storage account"
  }
}
```