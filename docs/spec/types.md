
# Bicep Type System
Because programs written in Bicep transpile primarily into JSON, it was clear from the beginning that Bicep has to support all data types that are available in JSON. We also wanted to provide stronger typing and validation so the users' errors are caught early in the development process and more failed deployments are avoided. These goals are very similar to the goals of the [TypeScript](https://www.typescriptlang.org/) language, which adds strong typing to JavaScript and compiles into the same.

As a result, the Bicep type system implements a small subset of the capabilities present in the TypeScript type system that are needed to support Bicep scenarios. If you are familiar with TypeScript and JSON, many of these concepts will feel familiar.

## Data Types
Bicep has a [Structural type system](https://en.wikipedia.org/wiki/Structural_type_system). In other words, the assignability of types in Bicep is determined by the definition (or structure) of the type rather than its name. All types in Bicep do have names, but these are used as shorthand type identifiers in error messages and hover texts.

The Bicep type system automatically infers the types of expressions but does not perform any automatic type conversion or coercion. Any such operations must be declared explicitly.

All values in Bicep have one of the following types:

### Simple Types
| Name | Description |
|:-|:-|
| `any` | The value can be anything or allows anything. |
| `error` | The expression has an error. |
| `string` | Represents arbitrary text. Equivalent to JSON strings. |
| `number` | **Not yet implemented [#486](https://github.com/Azure/bicep/issues/486)** Represents a floating point number or a big integer. Equivalent to the JSON 
| `int` | Represents a 32-bit integer. Is a subtype of `number`. There is no equivalent type in JSON. |
| `bool` | Represents a boolean `true`/`false` value. Equivalent to booleans in JSON. |
| `null` | Equivalent to `null` values in JSON. |

### Objects
Objects in Bicep are equivalent to JSON objects. The `object` type represents "any object" or an object that allows any property of any type. A narrower type than `object` may be inferred for object literals.

### Arrays
Arrays in Bicep are equivalent to JSON arrays. The `array` type represents an array of items where each item is of the `any` type. More strongly typed arrays may arise in various expressions. For example, an array of strings would be denoted by `string[]`. A narrower type than `array` may be inferred for array literals.

### Union Types
A union type is a type that represent a value that can one of several types. For example the type of a value that could either a string or an integer can be represented by the type `string | int`. Union types most often arise in expressions containing the ternary conditional operator, due to `allowedValues` constraint in a parameter modifier, or enum properties in resource declarations.

### Literal Types
As the name suggests, literal types are literal values that are treated as a type. In Bicep, we only support string literal types. This allows a constant string value such as `'Hello!'` to be its own type. Literal types are most commonly used in conjunction with union types to construct enum types with a limited set of allowed values. For example the type `'One' | 'Two' | 'Three'` is a subtype of `string` that allows three values: `'One'`, `'Two'`, and `'Three'`.

## Declaration Types
In Bicep, new data types cannot be directly created. Parameters, resources, modules, and outputs declare one of the existing types in their own declaration. (Variables do not have a declared type and instead take on the type of the assigned expression.) 

### Resource Types
Resource types, as the name suggests, represent the schema or shape of resources deployed by Bicep. Common resource types include the following:
- `Microsoft.Compute/virtualMachineScaleSets@2018-10-01`
- `Microsoft.Network/virtualNetworks/subnets@2018-11-01`
- `Microsoft.Authorization/roleAssignments@2018-09-01-preview`
- many others...

All of such types are considered named resource types, which are all subtypes of the `resource` type. The `resource` type is a subtype of `object`.

### Module Types
Module types are very similar to resource types except they involve module declarations. The `module` type is the supertype of all named module types. The `module` type is a subtype of `object`.

### Resource Scopes
Resource and module scopes in Bicep are represented using the `scope` type. The `scope` type is a subtype of `object`. Values of type `scope` are assignable to properties of type `scope` provided the two scopes are compatible.

## Type Assignability
When an expression is assigned to a declaration, the type of the expression is compared against a declared type based on the assignability rules between types. For complex types such as objects and arrays, this check is performed recursively. If the types are not compatible, the type checker emits one or more warnings or errors. (While we try to return as many errors as possible, we try to avoid errors and warnings that were caused by other errors and warnings.)

## Assignability Matrix
Type assignability determines whether a value of one type (source type) can be assigned to another type (target type). In the following matrix source types are listed horizontally and target types are listed vertically:
| Types | `any` | `error` | `string` | `number` | `int` | `bool` | `null` | `object` | `array` | named resource | named module | `scope` |
|-|-|-|-|-|-|-|-|-|-|-|-|-|
| `any`          |X| |X|X|X|X|X|X|X|X|X|X|
| `error`        | | | | | | | | | | | | |
| `string`       |X| |X| | | | | | | | | |
| `number`       |X| | |X|X| | | | | | | |
| `int`          |X| | | |X| | | | | | | |
| `bool`         |X| | | | |X| | | | | | |
| `null`         |X| | | | | |X| | | | | |
| `object`       |X| | | | | | |X| | | | |
| `array`        |X| | | | | | | |X| | | |
| `resource`     |X| | | | | | | | |X| | |
| `module`       |X| | | | | | | | | |X| |
| `scope`        | | | | | | | | | | | |?|
| named resource |X| | | | | | |?| |?| | |
| named module   |X| | | | | | |?| | |?| |

### Legend
| Mark | Source type assignable to target type? |
|:-|:-|
| X | Yes |
|   | No |
| ? | Yes if types are compatible. Otherwise, no. |