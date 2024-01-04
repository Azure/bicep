type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@    "foo": {
//@      "type": "object",
//@      "metadata": {
//@        "__bicep_resource_derived_type!": "Microsoft.Storage/storageAccounts@2023-01-01"
//@      }
//@    }

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

