# Using the Extendable Bicep Parameters Feature (Experimental!)

## What is it?

Extendable Bicep Parameter Files is a feature that allows you to extend `.bicepparam` files from another `.bicepparam` file in order to reuse parameters across multiple deployments.

When using extendable bicep parameter files, you will have a main `.bicepparam` file that can be consumed by multiple extended `.bicepparam` files.

## Using

`main.bicep` This is your main bicep file, which will define your parameters for deployment.

```bicep
param namePrefix string
param location string
param symbolicName string
```

`root.bicepparam` This is your main bicepparam file, which can be reused by multiple extended .bicepparam files and in multiple deployments.

```bicep
using none

param namePrefix = 'share'
param location = 'westus'
```

`leaf.bicepparam` This is your extended bicepparam file, which will refer to one main.bicep file and one main .bicepparam file. Any parameter value in this file will override all previous values.

```bicep
using 'main.bicep'

extends 'root.bicepparam'

param namePrefix = 'extend'
param symbolicName = 'test'
```

Resolved Values
| Param | Value |
| -- | -- |
| namePrefix | `'extend'` |
| location | `'westus'` |
| symbolicName | `'test'` |

**Note**: As `foo` is defined in both `root.bicepparam` and `leaf.bicepparam` files, any parameter values in the **`leaf.bicepparam`** file will override the values of the parameter in **both** the `main.bicep` and `root.bicepparam` files.

## Limitations

We do not have support for:

* variables: we will not have variable support in the `root.bicepparam` file to be able to read and override values in other files
  ```bicep
  var namePrefix = 'share'
  ```
* import function: you will not be able to import a parameter from another file
* ```bicep
  import {bar} from 'main.bicep'
  ```
* Multiple extends statements
  ```bicep
  using 'main.bicep'

  extends 'root1.bicepparam'
  extends 'root2.biepparam'
  extends 'root3.bicepparam'
  ```
* Ability to merge (union) parameters of type object and array
  ```bicep
  param loc1 string = [
  'westus'
  'westus2'
  ]
  
  param loc2 string = [
  'eastus'
  'eastus2'
  ]
  
  union(loc1, loc2)
  ```
* user-defined functions and user-defined types
  ```bicep
  type namePrefix = 'extend'
  func outputName(name string) => 'Hi ${name}!'
  ```

## Raising bugs or feature requests

Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
