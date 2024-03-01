type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@    "foo": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
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
  resA: resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@          }
  resB: sys.resource<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
//@          }
  resC: sys.array
//@        "resC": {
//@          "type": "array"
//@        },
  resD: sys.resource<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
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
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Astronomer.Astro/organizations@2023-08-01-preview#properties/name"
//@          }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resource    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@          }
  test3: resource</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@          }
}

@description('I love space(s)')
//@        "description": "I love space(s)"
type test2 = resource<
//@    "test2": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Astronomer.Astro/organizations@2023-08-01-preview#properties/name",
//@      }
//@    },

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resource<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@    "bar": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Resources/tags@2022-09-01#properties/properties"
//@      },
//@      "defaultValue": {
//@      }
//@    }
  tags: {
//@        "tags": {
//@        }
    fizz: 'buzz'
//@          "fizz": "buzz",
    snap: 'crackle'
//@          "snap": "crackle"
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@    "baz": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31#properties/name"
//@      },
//@      "value": "myId"
//@    }

type storageAccountName = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@    "storageAccountName": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@      }
//@    },
type accessPolicy = resource<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@    "accessPolicy": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.KeyVault/vaults@2022-07-01#properties/properties/properties/accessPolicies/items"
//@      }
//@    },
type tag = resource<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@    "tag": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Resources/tags@2022-09-01#properties/properties/properties/tags/additionalProperties"
//@      }
//@    }

