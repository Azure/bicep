# Bicep Language Specification

## Structure
An individual `.bicep` file is composed of the following declarations, in any order. Each declaration must be separated from the next by at minimum a single new line.
* [Parameters](./parameters.md)
* [Variables](./variables.md) 
* [Resources](./resources.md)
* [Outputs](./outputs.md)

## Whitespace
Spaces and tabs are ignored when authoring Bicep files. New lines however have semantic meaning, for example in [object](#objects) and [array](#arrays) declarations.

## Comments
### Single line
Single line comments begin with `//`.
#### Examples
```
variable myVar = 'hi!' // this is a comment
```

### Multi line
Multi-line comments are declared between `/*` and `*/` characters.
#### Examples
```
/* this 
is a 
multi line comment */

variable myVar = 'hi!' /* so is
this */
```

## Literals

### Strings
Strings are defined by a sequence of characters between single quote (`'`) characters, and must be declared on a single line.

The following are the set of reserved characters which must be escaped by a backslash (`\`) character:
| Escape Sequence | Represented value | Notes |
|:-|:-|:-|
| `\\` | `\` ||
| `\'` | `'` ||
| `\n` | `line feed (LF)` ||
| `\r` | `carriage return (CR)` ||
| `\t` | `tab character` ||
| `\$` | `$` | Only needs to be escaped if it is followed by `{` |

All strings in Bicep support interpolation, in order to reference expressions in-place. To inject an expression, surround it by `${` and `}`. Expressions that are referenced cannot span multiple lines.

#### Examples
```
// myVar evaluates to "hello!"
variable myVar = 'hello!'

// myVar evaluates to "what's up?"
variable myVar2 = 'what\'s up?'

// myVar evaluates to "hello steve!"
variable name = 'steve'
variable myVar3 = 'hello ${name}!'
```

### Numbers
Number literals are formatted as integers - a list of digits optionally preceeded by a `-`. Floating point, decimal or binary formats are not currently supported.

#### Examples
```
variable myVar = 123
variable myVar2 = -454
variable mvVar3 = 0
```

### Booleans
Boolean literals consist of the keywords `true` and `false`

#### Examples
```
variable myVar = false
variable myVar2 = true
```

### Arrays
Arrays are declared using the open and close square bracket characters (`[` and `]`), and consist of a set of values. Each value pair **must** be on a new line, and there **must** be a new line after the opening bracket and before the closing bracket. The values may consist of either a [literal](#literals) or an [expression](./expressions.md).

Note that commas are not required or permitted between array values.

#### Examples
```
variable myArray = [
  5983
  3923
  -241
]

variable myEmptyArray = [
]

variable myMixedArray = [
  myVariable
  'hello!'
  true
  1255
]
```

### Objects
Objects are declared using the open and close brace characters (`{` and `}`), and consist of a set of keys and values. Each key/value pair **must** be on a new line, and there **must** be a new line after the opening brace and before the closing brace. Keys and values are separated with the colon (`:`) character. The values may consist of either a [literal](#literals) or an [expression](./expressions.md).

Note that commas are not required or permitted between object property pairs.

#### Examples
```
variable myObj = {
  stringProp: 'hello!'
  boolProp: false
  numberProp: -464
  objProp: {
    nestedString: 'hello nested!'
  }
  arrayProp: [
    1234
    3435
  ]
  variableProp: myVariable
}

variable myEmptyObj = {
}
```

### Null
Null may be declared using the keyword `null`

#### Examples
```
variable myVar = null
```
