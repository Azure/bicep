# Expressions
Other content TBD. This section will be expanded as we design the expression syntax and semantics.

# Operators
The bicep language supports the following operators in expressions:

## Precedence and Associativity
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
parameter replicateGlobally bool

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

## Property Accessors
Property accessors are used to access properties of an object. They are constructed using the `.` operator. Consider the following:
```
variable x = {
  y: {
    z: 'Hello`
    a: true
  }
  q: 42
}
```

Given the above declaration, the expression `x.y.z` would evaluate to the literal string `'Hello'`. Similarly, the expression `x.q` would evaluate to the integer literal `42`.

## Array Indexers


### Indexers containing expressions
When referencing an object property using a parameter value, you will need to use the `[]` syntax. Let's say we have an `environment` parameter and an object declaration like the following:

```
parameter environment string = 'prod'

variable environmentSettings = {
  dev: {
    name: 'dev'
  }
  prod: {
    name: 'prod'
  }
}
```

To reference the `prod` or `dev` object properties, in this case in the context of a resource declaration, use the following syntax:

```
resource site 'microsoft.web/sites@2018-11-01' = {
  name: environmentSettings[environment].name
  location: location
  ...
}
```