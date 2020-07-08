# Variables
> **Note**: Not implemented yet

A `variable` declaration is used to assign a value to an identifier in the current scope. They can depend on resources, other variables or inputs. They can be assigned any valid value.

Unlike many imperative languages, variables cannot be reassigned, and must be assigned when declared. A variable cannot have the same name as a [parameter](./parameters.md), [resource](./resources.md), or another variable in the same scope.

Unlike [parameters](./parameters.md) or [outputs](./outputs.md), variables do not require declaring the type. The type is inferred from the value of the variable.

There are no constraints on placement of variable declarations. They be mixed with any other valid declarations in any order.

## Examples
The examples below cover variable declaration using hard-coded and calculated values.

### String variable
```
variable myString = 'my string value'

variable location = resourceGroup().location
```

### Boolean variables
```
variable iAmTrue = true
variable iAmFalse = false

variable hasItems = length(myArray) >= 0
```

### Numeric variables
```
variable meaningOfLifeTheUniVerseAndEverything = 42

variable lengthOfMyArray = length(myArray)
```

### Object variables
```
variable myObject = {
  first: 1
  second: 2
}

variable keys = listKeys(myResource.id, myResource.apiVersion)
```

### Array variables
```
variable myArray = [
  'item 1'
  'item 2'
]

variable myOtherArray = createArray('first', 'second', 'third')
```