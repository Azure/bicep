# Bicep Language Specification

## Structure
An individual `.bicep` file is composed of the following declarations, in any order. Each declaration must be separated from the next by at minimum a single new line.
* [Parameters](./parameters.md)
* [Variables](./variables.md) 
* [Resources](./resources.md)
* [Modules](./modules.md)
* [Outputs](./outputs.md)

## Whitespace
Spaces and tabs are ignored when authoring Bicep files. New lines however have semantic meaning, for example in [object](#objects) and [array](#arrays) declarations.

## Comments
### Single line
Single line comments begin with `//`.
#### Examples
```bicep
var myVar = 'hi!' // this is a comment
```

### Multi line
Multi-line comments are declared between `/*` and `*/` characters.
#### Examples
```bicep
/* this 
is a 
multi line comment */

var myVar = 'hi!' /* so is
this */
```

## Literals
The sections below cover common types of literals you will encounter in Bicep. See [Bicep Data Types](./types.md) for additional information about the type system and validation rules.

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
```bicep
// myVar evaluates to "hello!"
var myVar = 'hello!'

// myVar evaluates to "what's up?"
var myVar2 = 'what\'s up?'

// myVar evaluates to "hello steve!"
var name = 'steve'
var myVar3 = 'hello ${name}!'
```

### Multi-line strings
> Requires Bicep CLI v0.3 or later

Multi-line strings are defined by a sequence of characters between 3 or more single quote (`'`) characters. Characters that are entered between the opening and closing sequence are read verbatim, and no escaping is necessary or possible.

Bicep uses the following set of rules to interpret a multi-line string:
1. If a sequence of 3 or more `'` quotes are seen, start processing the string.
1. If the opening set of quotes is immediately followed by a newline, skip over this newline.
1. Start reading the string, until another set of `'` quotes are found, matching the number of opening quotes.

Note that:
* Because the Bicep parser reads all characters as-is, depending on the line endings of your Bicep file, newlines will either be interpreted as `\r\n` or `\n`.
* Interpolation is not currently supported in multi-line strings.
* If you wish to include `'''` in your multi-line string, you may choose to use a different number of opening quotes to avoid `'''` being interpreted as the end of your string.

#### Examples
```bicep
// evaluates to "hello!"
var myVar = '''hello!'''

// also evaluates to "hello!" as the first newline is skipped
var myVar2 = '''
hello!'''

// evaluates to "hello!\n" as the final newline is included
var myVar3 = '''
hello!
'''

// evaluates to "  this\n    is\n      indented\n"
var myVar4 = '''
  this
    is
      indented
'''

// evaluates to "comments // are included\n/* because everything is read as-is */\n"
var myVar5 = '''
comments // are included
/* because everything is read as-is */
'''

// evaluates to "var nestedBicep = '''\nhello!\n'''\n"
// we are using a different number of opening/closing quotes to allow ''' to be interpreted as part of the string
var myVar6 = ''''
var nestedBicep = '''
hello!
'''
''''

// evaluates to "interpolation\nis ${blocked}"
// note ${blocked} is part of the string, and is not evaluated as an expression
myVar6 = '''interpolation
is ${blocked}'''
```

### Numbers
Number literals are formatted as integers - a list of digits optionally preceeded by a `-`. Floating point, decimal or binary formats are not currently supported.

#### Examples
```bicep
var myVar = 123
var myVar2 = -454
var mvVar3 = 0
```

### Booleans
Boolean literals consist of the keywords `true` and `false`

#### Examples
```bicep
var myVar = false
var myVar2 = true
```

### Arrays
Arrays are declared using the open and close square bracket characters (`[` and `]`), and consist of a set of values. Each value pair **must** be on a new line, and there **must** be a new line after the opening bracket and before the closing bracket. The values may consist of either a [literal](#literals) or an [expression](./expressions.md).

Note that commas are not required or permitted between array values.

#### Examples
```bicep
var myArray = [
  5983
  3923
  -241
]

var myEmptyArray = [
]

var myMixedArray = [
  myVariable
  'hello!'
  true
  1255
]
```

### Objects
Objects are declared using the open and close brace characters (`{` and `}`), and consist of a set of keys and values. Each key/value pair **must** be on a new line, and there **must** be a new line after the opening brace and before the closing brace. Keys and values are separated with the colon (`:`) character.

Keys must either be a sequence of alphanumeric characters, or a string in cases where special characters or in-line expression interpolation is needed. Values may consist of any [literal](#literals) or an [expression](./expressions.md).

Note that commas are not required or permitted between object property pairs.
#### Examples
```bicep
var myObj = {
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

var myObjWithSpecialChars = {
  '$special\tchars!': true
  normalKey: 'val'
}

var name = 'Steve'
var objWithInterpolatedKeys = {
  'Hello ${name}!': 'val'
  '${name}': 'val'
  '${name == 'Steve' ? 'wibble' : 'wobble'}': 'val'
  '${guid(name)}': 'val'
}

var myEmptyObj = {}
```

### Null
Null may be declared using the keyword `null`

#### Examples
```bicep
var myVar = null
```
