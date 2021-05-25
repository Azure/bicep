# Variables
A `variable` declaration is used to assign a value to an identifier in the current scope. They can depend on resources, other variables or inputs. They can be assigned any valid value.

Unlike many imperative languages, variables cannot be reassigned, and must be assigned when declared. A variable cannot have the same name as a [parameter](./parameters.md), [resource](./resources.md), [output](./outputs.md) or another variable in the same scope.

Unlike [parameters](./parameters.md) or [outputs](./outputs.md), variables do not require declaring the type. The type is inferred from the value of the variable.

There are no constraints on placement of variable declarations. They can be mixed with any other valid declarations in any order.

## Examples
The examples below cover variable declaration using hard-coded and calculated values.

### String variable
```bicep
var myString = 'my string value'

var location = resourceGroup().location
```

### Boolean variables
```bicep
var iAmTrue = true
var iAmFalse = false

var hasItems = length(myArray) >= 0
```

### Numeric variables
```bicep
var meaningOfLifeTheUniVerseAndEverything = 42

var lengthOfMyArray = length(myArray)
```

### Object variables
```bicep
var myObject = {
  first: 1
  second: 2
}

var keys = listKeys(myResource.id, myResource.apiVersion)
```

### Array variables
```bicep
var myArray = [
  'item 1'
  'item 2'
]

var myOtherArray = createArray('first', 'second', 'third')
```
