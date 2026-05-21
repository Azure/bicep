# Extendable Bicep parameter files

Extendable parameter files let one `.bicepparam` file inherit parameter values from another `.bicepparam` file by using `extends`. The child file can keep inherited values, read parent values with `base`, and override any parameter it declares.

No feature flag is required.

## Basic use

`main.bicep`

```bicep
param namePrefix string
param location string
param tags object
```

`shared.bicepparam`

```bicep
using none

param namePrefix = 'prod'
param location = 'westus'
param tags = {
  environment: 'dev'
  owner: 'platform'
}
```

`prod.bicepparam`

```bicep
using './main.bicep'
extends './shared.bicepparam'

param namePrefix = 'app'
param tags = {
  ...base.tags
  environment: 'prod'
  region: 'westus2'
}
```

The final values are:

| Parameter | Value |
| --- | --- |
| `namePrefix` | `'app'` |
| `location` | `'westus'` |
| `tags` | `{ environment: 'prod', owner: 'platform', region: 'westus2' }` |

## Inheritance and overrides

Inheritance starts at the oldest parent file and moves down to the child file that you build or deploy.

If more than one file sets the same parameter, the nearest child wins. In the example above, `prod.bicepparam` overrides `namePrefix` and `tags`, while `location` is inherited from `shared.bicepparam`.

## The `base` identifier

When a parameter file declares `extends`, the `base` identifier is available in that file. It exposes the resolved parameter values from the direct parent file.

Use `base` to build on parent values:

```bicep
using './main.bicep'
extends './shared.bicepparam'

param tags = {
  ...base.tags
  environment: 'prod'
}

param locations = [...base.locations, 'centralus']

var suffix = '-api'
param fullName = '${base.namePrefix}${suffix}'
```

Rules for `base`:

- `base` is only available in files that declare `extends`.
- `base` is reserved and cannot be declared as a parameter or variable.
- `base` exposes parent parameters. Parent variables are not exposed, but a parent file can use its own variables to set parameter values.
- Object spread works with object values. Array spread works with array values.

## Nested extends

Parameter files can extend other parameter files in a chain.

```bicep
// base.bicepparam
using none

param tags = {
  owner: 'platform'
}
```

```bicep
// env.bicepparam
using none
extends './base.bicepparam'

param tags = {
  ...base.tags
  environment: 'prod'
}
```

```bicep
// app.bicepparam
using './main.bicep'
extends './env.bicepparam'

param tags = {
  ...base.tags
  app: 'frontend'
}
```

In `app.bicepparam`, `base.tags` contains the resolved value from `env.bicepparam`, including values that `env.bicepparam` inherited from `base.bicepparam`.

## `using none`

Use `using none` for reusable parent parameter files that are not tied to one `.bicep` file.

A file with `using none` can declare parameters and variables, and it can be extended by another `.bicepparam` file. It is not validated against a target `.bicep` file by itself. Type validation happens when a child parameter file is built with a real `using './main.bicep'` target.

## Diagnostics and limitations

The compiler reports errors for these cases:

- More than one `extends` declaration in the same file.
- A parent path that is missing, unreadable, or not a `.bicepparam` file.
- A file that extends itself or creates an extends cycle.
- Use of `base` in a file without `extends`.
- Redeclaring `base` as a parameter or variable.
- Parent or child values that do not match the target parameter type.
- Spreading a non-object value into an object or a non-array value into an array.

Variables are supported in parameter files. They are scoped to the file where they are declared. To pass a computed value to a child file, assign it to a parameter in the parent file and read that parameter through `base`.

## Migration note

Older configs may contain this setting:

```json
{
  "experimentalFeaturesEnabled": {
    "extendableParamFiles": true
  }
}
```

The setting is now ignored. You can remove it from `bicepconfig.json`.