# Using the Extendable Bicep Parameters Feature (Experimental!)

## What is it?

Extendable Bicep Parameter Files is a feature that allows you to extend `.bicepparam` files from another `.bicepparam` file in order to reuse parameters across multiple deployments.

When using extendable bicep parameter files, you will have a `base.bicepparam` file that can be used by multiple `.bicep` and `.bicepparam` files.

The `base.bicepparam` file will link to a `shared.bicepparam` file, which will refer to a `main.bicep` file and your `base.bicepparam file`. In this case, you will be able to reuse and refer to this `base.bicepparam` file multiple times.

## Using

`main.bicep` This is your main bicep file, which will define your parameters for deployment.

```bicep
param foo string
param foo string
```

`base.bicepparam` This is your base bicepparam file, which can be reused by multiple shared.bicepparam files and in multiple deployments.

```bicep
using none

param foo = 'foo'
param bar = 'bar'
```

`shared.bicepparam` This is your shared bicepparam file, which will refer to one main.bicep file and one base.bicepparam file. Any parameter definition in this file will override all previous definitions.

```bicep
using 'main.bicep'

extends 'base.bicepparam'

param foo = 'bar'
param baz = foo
```

Compiled json output

```bicep
{
  foo: 'bar'
  bar: 'bar'
  baz: 'bar'
}
```

**Note**: As `foo` is defined in both `base.bicepparam` and `shared.bicepparam` files, any parameter definitions in the **`shared.bicepparam`** file will override the definitions of the parameter in **both** the `main.bicep` and `base.bicepparam` files.

## Limitations

We do not have support for:

* variables
* import/export function
* Multiple extends statements
* Ability to merge (union) parameters of type object and array

## Raising bugs or feature requests

Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.
