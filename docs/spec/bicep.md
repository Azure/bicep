# Bicep Language Specification

TODO: intro

## Fundamentals

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
// represents "hello!"
'hello!'

// represents "what's up?"
'what\'s up?'

// represents "hello steve!"
variable name = 'steve'
'hello ${name}!'
```

TODO: other fundamental 'types'

TODO: comments

TODO: link to other spec pages