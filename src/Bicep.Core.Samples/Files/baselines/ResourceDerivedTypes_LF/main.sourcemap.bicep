type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@    "foo": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01"
//@      }
//@    },

type test = {
//@    "test": {
//@      "type": "object",
//@      "properties": {
//@        "resA": {
//@        },
//@        "resB": {
//@        },
//@        "resD": {
//@        }
//@      }
//@    },
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@          "type": "object",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01"
//@          }
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@          "type": "object",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2022-09-01"
//@          }
  resC: sys.array
//@        "resC": {
//@          "type": "array"
//@        },
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>
//@          "type": "object",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2022-09-01"
//@          }
}

type strangeFormattings = {
//@    "strangeFormattings": {
//@      "type": "object",
//@      "properties": {
//@        "test": {
//@        },
//@        "test2": {
//@        },
//@        "test3": {
//@        }
//@      }
//@    },
  test: resource<
//@          "type": "object",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Astronomer.Astro/organizations@2023-08-01-preview"
//@          }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>
//@          "type": "object",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01"
//@          }
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>
//@          "type": "object",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01"
//@          }
}

@description('I love space(s)')
//@        "description": "I love space(s)"
type test2 = resource<
//@    "test2": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Astronomer.Astro/organizations@2023-08-01-preview",
//@      }
//@    }

     'Astronomer.Astro/organizations@2023-08-01-preview'

>

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@    "bar": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Resources/tags@2022-09-01"
//@      },
//@      "defaultValue": {
//@      }
//@    }
  name: 'default'
//@        "name": "default",
  properties: {
//@        "properties": {
//@        }
    tags: {
//@          "tags": {
//@          }
      fizz: 'buzz'
//@            "fizz": "buzz",
      snap: 'crackle'
//@            "snap": "crackle"
    }
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@    "baz": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31"
//@      },
//@      "value": {
//@      }
//@    }
  name: 'myId'
//@        "name": "myId",
  location: 'eastus'
//@        "location": "eastus"
}

