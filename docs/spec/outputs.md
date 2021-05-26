# Outputs
Output declarations will be compiled into template outputs and are used to return information back out from the template deployment. See [Template Parameter Syntax Reference](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-syntax#outputs) for more information. The value of an output is any value literal or any [expression](./expressions.md).

There are no constraints on placement of output declarations. They can be mixed with any other valid declarations in any order.

## String output
```bicep
output myEndpoint string = myResource.properties.endpoint
```

## Integer output
The following declares a hard-coded integer output and sets the value to `42`.
```bicep
output myHardcodedOutput int = 42
```

## Object output
The following declares an object output that returns information about the resourceGroup:
```bicep
output myResourceGroup object = resourceGroup()
```

## Boolean output
The following declares a boolean output value and sets the value using an expression.
```bicep
output isInputParamEmpty bool = length(myParam) == 0
```

## Array output
The following declares an array output and computes the value using a [loop](./loops.md).
```bicep
output myLoopyOutput array = [for myItem in myArray: {
  myProperty: myItem.myOtherProperty
}]
```
