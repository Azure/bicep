# Language Spec

## Introduction
This document puts forward a proposal for ARMLang (name TBD). ARMLang is a declarative configuration language for Azure Resource Manager Template Deployments.

## Structure
An individual `.arm` file at its most basic, consists of a set of runtime inputs, resources which will be deployed, and runtime outputs.

### Scoping
Each file is given a new scope. Any `identifier` declared inside this file is added to this scope, and each `identifier` must be globally unique within this scope.

### Ordering
The order which the individual components are listed inside the file does not determine the order in which the resources are deployed. Since each component consists of a set of input values and defines an output `identifier`, the compiler is able to infer the dependency graph by analyzing where `identifier` values are produced, and where they are consumed. Resources will be deployed according to this dependency graph.

As long as the compiler is able to produce an acyclic dependency graph, the order is considered valid. Components may be ordered arbitrarily within a given scope if desired.

* [TODO] comment on overriding dependency order

### Whitespace
Whitespace is ignored when parsing.

## Components

### Inputs
An `input` declaration is used to declare a named runtime variable that can be supplied during template deployment and accessed inside the template.

Inputs can optionally be assigned a default, which can either be a constant `value`, or a `value` built up of functions on other `input` values. Inputs cannot depend on each other in a cyclic fashion.

The `input` statement defines an `identifier` on the current `scope` with the assigned `value`.

#### Structure
```
input <type> <identifier> [: <default_value>]
```

#### Examples
```
input string myInput
```

```
input bool myInputWithDefault: true
```

```
input string generatedGuid: guid(myInput)
```

### Outputs
An `output` declaration is used to declare a named runtime output that can be accessed once a template deployment has completed.

The result of the output can be set to any computable [`value`](#values).

The `output` statement does not define an `identifier` on the current `scope`.

#### Structure
```
output <identifier>: <value>
```

#### Examples
```
output myOutput: 'this is a string output'
```

```
output myVariableOutput: myVariable
```

```
output resourceIdOutput: myResource.id()
```

### Variables
A `variable` declaration is used to assign a value to an identifier in the current scope. They can depend on resources, other variables or inputs. They can be assigned any valid `value`.

Unlike many imperative languages, variables cannot be reassigned, and must be assigned when declared.

#### Structure
```
variable <identifier>: <value>
```

#### Examples
```
variable myString: 'this is a string'
```

```
variable myObject: {
  prop1: myResource.id()
  prop2: myString
}
```

### Resources
A `resource` declaration defines a resource which will be either created or updated at deployment time, along with its intended state.

To allow for an pluggable provider model, the resource declaration consists of a 'provider' as well as a 'type'. This model is discussed in more detail in the [extensibility](#extensibility) section.
* [TODO] write the extensibility section!

The resource state can be assigned any valid [`value`](#values), but compile-time validations should ensure the structure of the value matches that expected by the resoruce type.

#### Structure
```
resource <provider> <type> <identifier>: <value>
```

#### Examples
```
resource azrm 'Microsoft.Network/networkInterfaces@2019-10-01' myNic: {
  name: 'myNic'
  properties: {
    ...
  }
}
```

### Comments
Single line comments begin with `//`.

Multi-line comments are declared between `/*` and `*/` characters.

## Values
### Strings
Literal strings are defined between single quote `'` characters, and all strings support interpolation by default.

The backslash `\` is used as an escape character, and the following characters must be escaped: `$`, `\r`, `\n`, `\`, `'`, `\t`.

* [TODO] mention unicode escaping
* [TODO] mention interpolation
* [TODO] mention multiline support (`'''`?)

#### Examples
```
'this is a normal string'
```

```
'interpolated ${resource.id()} string'
```

```
'string \' with \r\nescaped\tcharacters\\ \$'
```

### Numbers
* [TODO] copy JSON/JS number literals

### Booleans
Boolean literals are either `true` or `false`.

### Objects
Objects are defined between `{` and `}` characters as a set of key and value pairs. Note that commas are not used to separate pairs.

Unquoted object keys may be defined as long as they are alphanumeric, with the first character being a non-digit. Otherwise they may be quoted using the [`string`](#strings) syntax.

#### Structure
```
{
  <key1>: <value1>
  <key2>: <value2>
  ...
}
```

#### Examples
```
{} // empty object
```

```
{
  propertyA: 'value'
  propertyB: resource.id()
  propertyC: {
    nested: 'value'
  }
  'property;\$': 'value'
}
```

### Arrays
Arrays are defined between `[` and `]` characters as a set of values. Note that commas are not used to separate values.

#### Structure
```
[
  <value1>
  <value2>
  ...
]
```

#### Examples
```
[] //empty array
```

```
[
  'myString'
  true
  computedValue()
]
```

## Conditionals/Looping

### Conditionals

* [TODO] info here

### Looping
Loops may be used iterate over a collection to declare multiple copies of one or more resources. The input collection may be either an array, in which case iteration will occur over the elements, or an object, in which case iteration will occur over the keys. It is also possible to access an optional index.

A new scope is initialized inside the loop body. Identifiers declared in the outer scope may be accessed inside the inner scope, but identifiers declared in the inner scope will not be added to the outer scope. As well as resources, variables may be declared in the inner scope and referenced locally.

Loops may be nested inside each other.

#### Structure
```
for <iterator_identifier> [, <index>] in <collection> [as <loop_identifier>] {
  <declaration1>
  <declaration2>
  ...
}
```

#### Examples
```
variable myObject {
  keyA: 'valA'
  keyB: 'valB'
}

// iterating over an object
for key in myObject {
  resource azrm 'Microsoft.Network/networkInterfaces@2019-10-01' myNic: {
    name: 'myNic-${key}'
    properties: {
      value: myObject[key]
    }
  }
}

// iterating over an array and referencing outside the loop
for val in ['steve' 'bob'] as loopResources {
  resource azrm 'Microsoft.Network/networkInterfaces@2019-10-01' myNic: {
    name: val
    properties: {}
  }
}

output steve: loopResources[0].myNic.name
output bob: loopResources[1].myNic.name
```

## Expressions

### Parentheses
Parentheses `(` & `)` may be optionally specified for logical grouping. For a statement `(<val>)`, the parentheses will be ignored, and the evaluated value will return `<val>`.

### Unary Operators
`!<val>` - evaluates `<val>`, and returns `true` if `<val>` is `false`, otherwise `false`.

### Binary Operators

#### Math Operators
`<a> + <b>` - evaluates `<a>` and `<b>`, and sums the result. Both `<a>` and `<b>` must be of type `number`, and the output is of type `number`.

`<a> - <b>` - evaluates `<a>` and `<b>`, and subtracts `<b>` from `<a>`. Both `<a>` and `<b>` must be of type `number`, and the output is of type `number`.

`<a> * <b>` - evaluates `<a>` and `<b>`, and multiplies them together. Both `<a>` and `<b>` must be of type `number`, and the output is of type `number`.

#### Equality Operators
`<a> == <b>` - evaluates `<a>` and `<b>`, and returns `true` if they are equal, `false` otherwise.

`<a> != <b>` - evaluates `<a>` and `<b>`, and returns `true` if they are not equal, `false` otherwise.

`<a> =~ <b>` - evaluates `<a>` and `<b>`, and returns `true` if they are case-insensitive equal, `false` otherwise. Both `<a>` and `<b>` must be of type `string`.

`<a> !~ <b>` - evaluates `<a>` and `<b>`, and returns `true` if they are not case-insensitive equal, `false` otherwise. Both `<a>` and `<b>` must be of type `string`.

#### Logical Operators
`<a> && <b>` - evaluates `<a>` and `<b>`, and returns `true` if they are both `true`, `false` otherwise. Both `<a>` and `<b>` must be of type `boolean`.

`<a> && <b>` - evaluates `<a>` and `<b>`, and returns `true` if either is `true`, `false` otherwise. Both `<a>` and `<b>` must be of type `boolean`.

### Ternary Operator
The ternary operator evaluates a `<condition>` on the left side of the `?`, and
 either evaluates and returns the left-hand `<valueA>` or right-hand `<valueB>` of the expressions separated by `:` on the right side of the `?`.

#### Structure
```
<condition> ? <valueA> : <valueB>
```

#### Examples
```
myCondition ? 'true!' : 'false!'
otherCondition ? { value: true } : null
```

* [TODO] anything else?

### Built-in Functions

`range(n: number)` - returns an array of `n` integers from `0` to `n`.

* [TODO] override types