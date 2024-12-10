type foo = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@    "foo": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@        }
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
  resA: resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": {
//@              "source": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@            }
//@          }
  resB: sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": {
//@              "source": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
//@            }
//@          }
  resC: sys.array
//@        "resC": {
//@          "type": "array"
//@        },
  resD: sys.resourceInput<'az:Microsoft.Storage/storageAccounts@2022-09-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": {
//@              "source": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
//@            }
//@          }
}

type strangeFormatting = {
//@    "strangeFormatting": {
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
  test: resourceInput<
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": {
//@              "source": "Astronomer.Astro/organizations@2023-08-01-preview#properties/name"
//@            }
//@          }

  'Astronomer.Astro/organizations@2023-08-01-preview'

>.name
  test2: resourceInput    <'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": {
//@              "source": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@            }
//@          }
  test3: resourceInput</*    */'Microsoft.Storage/storageAccounts@2023-01-01'/*     */>.name
//@          "type": "string",
//@          "metadata": {
//@            "__bicep_resource_derived_type!": {
//@              "source": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@            }
//@          }
}

@description('I love space(s)')
//@        "description": "I love space(s)"
type test2 = resourceInput<
//@    "test2": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Astronomer.Astro/organizations@2023-08-01-preview#properties/name"
//@        },
//@      }
//@    },

     'Astronomer.Astro/organizations@2023-08-01-preview'

>.name

param bar resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties = {
//@    "bar": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Microsoft.Resources/tags@2022-09-01#properties/properties"
//@        }
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

output baz resourceInput<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'>.name = 'myId'
//@    "baz": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31#properties/name"
//@        }
//@      },
//@      "value": "myId"
//@    }

type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2023-01-01'>.name
//@    "storageAccountName": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Microsoft.Storage/storageAccounts@2023-01-01#properties/name"
//@        }
//@      }
//@    },
type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
//@    "accessPolicy": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Microsoft.KeyVault/vaults@2022-07-01#properties/properties/properties/accessPolicies/items"
//@        }
//@      }
//@    },
type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
//@    "tag": {
//@      "type": "string",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": {
//@          "source": "Microsoft.Resources/tags@2022-09-01#properties/properties/properties/tags/additionalProperties"
//@        }
//@      }
//@    }

