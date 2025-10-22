# Using the `console` command (Experimental!)

## What is it?
The `console` command provides an interactive Read-Eval-Print Loop (REPL) environment for Bicep expressions. It allows you to experiment with Bicep functions and expressions in an interactive console session.

## Features
- **Interactive Expression Evaluation**: Enter Bicep expressions and see their evaluated results immediately
- **Variable Declarations**: Define variables using `var name = expression` syntax and reuse them in subsequent expressions
- **Multi-line Input**: Support for complex multi-line expressions with automatic structural completion detection
- **Syntax Highlighting**: Real-time syntax highlighting for input and output

## Usage
```sh
bicep console
```

## Examples

### Simple Expressions
```bicep
> 1 + 2
3

> 'Hello, ' + 'World!'
'Hello, World!'

> length(['a', 'b', 'c'])
3
```

### Variable Declarations
```bicep
> var myName = 'John'
> var greeting = 'Hello, ${myName}!'
> greeting
'Hello, John!'
```

### Multi-line Expressions
The console automatically detects when expressions are structurally complete:

```bicep
> var config = {
  name: 'myApp'
  version: '1.0.0'
  settings: {
    debug: true
    timeout: 30
  }
}
> config.settings.debug
true
```

### Complex Expressions
```bicep
> var users = [
  { name: 'Alice', age: 30 }
  { name: 'Bob', age: 25 }
]
> map(users, user => user.name)
['Alice', 'Bob']

> filter(users, user => user.age > 26)
[
  {
    age: 30
    name: 'Alice'
  }
]
```

## Limitations
- No support for expressions requiring Azure context, e.g. `resourceGroup()`
- No file system access or external dependencies
- Limited to expression evaluation and variable declarations
- No support for for-loop expressions, e.g. `[for i in range(0, x): i]`
- No persistent state between console sessions
- No completions support

## Raising bugs or feature requests
Please raise bug reports or feature requests under [Bicep REPL issue](https://github.com/Azure/bicep/issues/11963).