# Proposal - Functions

## Goals
* Avoid code repetition both in-file and across files

## Spec
The `function` keyword provides a very simple mechanism for defining a pure function:
```
function <identifier>(<<arg> ,>*) {
    variable <name>: <value>
    ...

    return <output>
}
```

A `function` creates a new scope, and cannot reference any identifiers from an outer scope; the only identifiers initially available are those defined as arguments. The function body can consist of `variable` definitions to break things up, and the function must terminate with a `return` value. Other keywords such as `resource`, `input` and `output` are not supported inside a function.

A `function` body may access another user-defined function, or a built-in function, but no form of recursion is allowed.

Functions are not annotated with types, but types should be inferred where possible when the function is called.

## Example
```
// defining the function
function getConnectionString(storageAccount) {
    variable primaryKey: storageAccount.listKeys().primary

    return 'DefaultEndpointsProtocol=http;AccountName=${storageAccount.name};AccountKey=${primaryKey};'
}

// calling the function
variable myConnectionString: getConnectionString(myAccount)

// this should throw a type error because the boolean type does not support `listKeys()`:
variable myConnectionString: getConnectionString(false)
```