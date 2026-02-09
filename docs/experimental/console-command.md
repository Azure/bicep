# Using the `console` command (Experimental!)

## What is it?
The `console` command provides an interactive Read-Eval-Print Loop (REPL) environment for Bicep expressions. It allows you to experiment with Bicep functions and expressions in an interactive console session.

## Features
- **Interactive Expression Evaluation**: Enter Bicep expressions and see their evaluated results immediately
- **Variable Declarations**: Define variables using `var name = expression` syntax and reuse them in subsequent expressions
- **Multi-line Input**: Support for complex multi-line expressions with automatic structural completion detection
- **Syntax Highlighting**: Real-time syntax highlighting for input and output

## Usage
To launch the REPL, run the following Bicep command:

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
#### Lambdas
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
#### User-defined types and functions
```bicep
> type PersonType = {
  name: string
  age: int
}
> func sayHi(person PersonType) string => 'Hello ${person.name}, you are ${person.age} years old!'
> var alice = {
  name: 'Alice'
  age: 30
}
> [ sayHi(alice), sayHi({ name: 'Bob', age: 25 })]
[
  'Hello Alice, you are 30 years old!'
  'Hello Bob, you are 25 years old!'
]
```

### Loading content from files
- Bicep console also supports the [`load*()` functions](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-functions-files). Note: The directory from which the `bicep console` command is run is used as the _current directory_ when evaluating the `load*()` functions


### Piping and standard input/output redirection
The console command supports evaluating expressions provided through piping or redirected standard input, i.e.:

**Powershell**:
```pwsh
# piped input
"parseCidr('10.144.0.0/20')" | bicep console
```

**Bash**:
```sh
# piped input
echo "parseCidr('10.144.0.0/20')" | bicep console
# stdin redirection from file content
bicep console < test.txt
```

Multi-line input is also supported, i.e:
```pwsh
"{
> foo: 'bar'
> }.foo" | bicep console
# Output: bar
```

Output redirection is also supported:
```sh
"toObject([{name:'Evie', age:4},{name:'Casper', age:3}], x => x.name)" | bicep console > output.json
```

## Limitations
- No support for expressions requiring Azure context, e.g. `resourceGroup()`
- No support for for-loop expressions, e.g. `[for i in range(0, x): i]`
- No persistent state between console sessions
- No completions support

## Raising bugs or feature requests

Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
