# Expressions
The bicep language supports the following operators in expressions:

## Operator precedence and associativity
The operators below are listed in descending order of precedence (the higher the position the higher the precedence). Operators listed at the same level have equal precedence.

| Symbol | Type of Operation | Associativity |
|:-|:-|:-|
| `(` `)` `[` `]` `.` | Parentheses, property accessors and array indexers | Left to right |
| `!` `-` | Unary | Right to left |
| `%` `*` `/` | Multiplicative | Left to right |
| `+` `-` | Additive | Left to right |
| `<=` `<` `>` `>=` | Relational | Left to right |
| `==` `!=` `=~` `!~` | Equality | Left to right |
| `&&` | Logical AND | Left to right |
| `\|\|` | Logical OR | Left to right |
| `?` `:` | Conditional expression (ternary) | Right to left

## Unary operators
Unary operators operate on a single operand. Bicep supports the following unary operators:

| Symbol | Operator Name | Operand Types | Return Type | Template Equivalent | Description | 
|:-|:-|:-|:-|:-|:-|
| `!` | Unary NOT | `bool` | `bool` | `[not(<value>)]` | Negates the specified value |
| `-` | Unary minus | `int` | `int` | `-<value>` or `[sub(0, <value>)]` | Multiplies number by -1 |

## Binary operators
Binary operators operate on a pair of operands. Bicep supports the following binary operators:

| Symbol | Operator Name | Operand Types | Return Type | Template Equivalent | Description | 
|:-|:-|:-|:-|:-|:-|
| `%` | Modulo | `int` `int` | `int` | `[mod(<value1>,<value2>)]` | Calculates the remainder from integer division |
| `*` | Multiply | `int` `int` | `int` | `[mul(<value1>,<value2>)]` | Multiplies two integers |
| `/` | Divide | `int` `int` | `int` | `[div(<value1>,<value2>)]` | Divides two integers |
| `+` | Add | `int` `int` | `int` | `[add(<value1>,<value2>)]` | Adds two integers |
| `-` | Subtract | `int` `int` | `int` | `[sub(<value1>,<value2>)]` | Subtracts two integers |
| `>=` | Greater than or equal | `string` `string` | `bool` | `[greaterOrEquals(<value1>,<value2>)]` | Greater than or equal |
| `>=` | Greater than or equal | `number` `number` | `bool` | `[greaterOrEquals(<value1>,<value2>)]` | Greater than or equal |
| `>` | Greater than | `string` `string` | `bool` | `[greater(<value1>,<value2>)]` | Greater than |
| `>` | Greater than | `number` `number` | `bool` | `[greater(<value1>,<value2>)]` | Greater than |
| `<=` | Less than or equal | `string` `string` | `bool` | `[lessOrEquals(<value1>,<value2>)]` | Less than or equal |
| `<=` | Less than or equal | `number` `number` | `bool` | `[lessOrEquals(<value1>,<value2>)]` | Less than or equal |
| `<` | Less than | `string` `string` | `bool` | `[less(<value1>,<value2>)]` | Less than |
| `<` | Less than | `number` `number` | `bool` | `[less(<value1>,<value2>)]` | Less than |
| `==` | Equals | `any` `any` | `bool` | `[equals(<value1>,<value2>)]` | Less than |
| `!=` | Not Equal | `any` `any` | `bool` | `[not(equals(<value1>,<value2>))]` | Less than |
| `=~` | Equals (case-insensitive) | `string` `string` | `bool` | `[equals(toLower(<value1>),toLower(<value2>))]` | Less than |
| `!~` | Equals (case-insensitive) | `string` `string` | `bool` | `[not(equals(toLower(<value1>),toLower(<value2>)))]` | Less than |
| `&&` | Logical AND | `bool` *n | `bool` | `[and(<value1>, ...)]` | Returns true if all values are true |
| `\|\|` | Logical OR | `bool` *n | `bool` | `[or(<value1>, ...)]` | Returns true if any value is true |

## Ternary operator
Ternary operators operate on 3 operands. Bicep supports only one such operator.

| Symbol | Operator Name | Operand Types | Return Type | Template Equivalent | Description | 
|:-|:-|:-|:-|:-|:-|
| `?` `:` | Conditional expression | `bool` `any` `any` | ` <true type> \| <false type> ` | `[if(<condition>, <true value>, <false value>)]` | Returns a value based on whether the condition is true or false |

Example usage:
```
param replicateGlobally bool

resource myStorageAccount `Microsoft.Storage/storageAccounts@2017-10-01` = {
  name: storageAccountName
  location: resourceGroup().location
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: StorageV2
  sku: {
    name: replicateGlobally ? 'Standard_GRS' : 'Standard_LRS'
  }
}
```

## Parentheses
Enclosing an expression between `(` and `)` allows you to override the default bicep operator precedence. For example, the expression `x + y / z` would evaluate the division first and then the addition. However, the expression `(x + y) / z` would evaluate the addition first and division second.

## Property accessors
Property accessors are used to access properties of an object. They are constructed using the `.` operator. Consider the following:
```
var x = {
  y: {
    z: 'Hello`
    a: true
  }
  q: 42
}
```

Given the above declaration, the expression `x.y.z` would evaluate to the literal string `'Hello'`. Similarly, the expression `x.q` would evaluate to the integer literal `42`.

Property accessors can be used with any object. This includes parameters and variables of object types and object literals. Using a property accessor on an expression of non-object type is an error.

## Array indexers
Array indexers serve two purposes. Most commonly, they are used to access items in an array. However, they can also be used to access properties of objects via expressions or string literals.

Consider the following:
```
var index = 1

var myArray = [
  1
  2
  3
]
```

Arrays in bicep are 0-based. In other words, the first item in an array is at index 0. As such, the expression `myArray[0]` will evaluate to `1` and `myArray[2]` will evaluate to `3`. The index of the indexer may itself be another expression. In the above example, `myArray[index]` would evaluate to `2`. Integer indexers are only allowed on expression of array types. Usage of integer indexers on other types is an error.

String-based indexers are also allowed in bicep. Consider the following:
```
param environment string = 'prod'

var environmentSettings = {
  dev: {
    name: 'dev'
  }
  prod: {
    name: 'prod'
  }
}
```

Given the above, the expression `environmentSettings['dev']` would evaluate to this object:
```
{
  name: 'dev'
}
```

Just like with integer indexers, the string indexer can also be an expression. Given the above example, the expression `environmentSettings[environment].name` would evaluate to `'dev'`, `'prod'`, or a runtime error depending on the value of the `environment` parameter. String-based indexers can only be used with expression of object type. Usage on expressions of other types is an error.

In general, expressions are allowed anywhere where a value is specified in bicep. For example you could use the expressions above in a resource declaration as follows:
```
resource site 'microsoft.web/sites@2018-11-01' = {
  name: environmentSettings[environment].name
  location: location
  ...
}
```
