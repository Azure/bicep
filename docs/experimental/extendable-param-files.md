# Using the Extendable Bicep Parameters Feature

## What is it?

Extendable Bicep Parameter Files (enabled with the `extend` keyword) is a feature that allows you to reuse parameters from one `.bicepparam` file in another `.bicepparam` file. This enables better re-use across multiple deployments and helps to keep your code [DRY](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself).

When using extendable bicep parameter files, you will have a main `.bicepparam` file that can be consumed by multiple extended `.bicepparam` files.

> NOTE: This feature is experimental. Enable it in `bicepconfig.json` under `experimentalFeaturesEnabled.extendableParamFiles` before use.
>
>```json
>{
>  "experimentalFeaturesEnabled": {
>    "extendableParamFiles": true
>  }
>}
>```

## Example Usage

`main.bicep` This is your main bicep file, which will define your parameters for deployment.

```bicep
param namePrefix string
param location string
param tags object
```

`root.bicepparam` This is your main bicepparam file, which can be reused by multiple extended .bicepparam files and in multiple deployments.

```bicep
using none
// Notice that the first line of this .bicepparam file declares `using none` which tells the compiler not to validate this against any particular .bicep file.

param namePrefix = 'Prod'
param location = 'westus'
param tags = {
  environment: 'dev'
  owner: 'platform'
}
```

`leaf.bicepparam` This is your extended bicepparam file, which will refer to one main.bicep file and one main .bicepparam file. Any parameter value in this file will override all previous values.

```bicep
using 'main.bicep'

extends 'root.bicepparam'

param namePrefix = 'Dev'
param tags = {
  ...base.tags        // inherit the object from the base file
  environment: 'prod' // override a single property
  region: 'westus2'   // add new data
}
```

Resolved Values

| Param | Value |
| -- | -- |
| namePrefix | `'Dev'` |
| location | `'westus'` |
| tags | `{ environment: 'prod', owner: 'platform', region: 'westus2' }` |

**Note**: Any parameter values in the **`leaf.bicepparam`** file will override the values of the parameter in **both** the `main.bicep` and `root.bicepparam` files. In the example, `namePrefix` is defined in both the `root.bicepparam` and `leaf.bicepparam` files and resolves to `Dev` from the leaf file. The `tags` object is merged using `base.tags`, preserving `owner` while overriding `environment` and adding `region`.

### Accessing parent parameters with `base`

When a parameters file declares an `extends` clause, the `base` identifier becomes available and exposes the parent file's parameters as properties. Use it to merge or extend values instead of retyping them.

```bicep
// shared.bicepparam
using none

param app = {
  name: 'demo'
  tags: {
    owner: 'platform'
    environment: 'dev'
  }
}
param locations = ['westus', 'eastus']

// child.bicepparam
using './main.bicep'
extends './shared.bicepparam'

// Merge objects
param app = {
  ...base.app
  tags: {
    ...base.app.tags
    environment: 'prod'
    costCenter: '1234'
  }
}

// Merge arrays
param locations = [...base.locations, 'centralus']

// Use base in expressions or variables
var suffix = '-api'
param fullName = '${base.app.name}${suffix}'
```

**Rules and tips**

- `base` is only available when the file has a single `extends` clause. Using it without `extends` results in an error.
- `base` is reserved; you cannot redeclare it as a parameter or variable.
- Object and array spreads work with `base` for merging. Spreading a non-object/non-array value fails during compilation.
- Types must still align with the target parameter type; incompatible overrides produce type errors.

### Nested Parameter Files

We also support nested parameter files, so that `a.bicepparam` extends `b.bicepparam` extends `c.bicepparam`. In this case, you will use `using none` for file `a.bicepparam` and `b.bicepparam`.

## Limitations

We do not have support for:

* variables: we will not have variable support in the `root.bicepparam` file to be able to read and override values in other files
  ```bicep
  var namePrefix = 'share'
  ```
* import function: you will not be able to import a parameter from another file
  ```bicep
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
