# Outputs should not contain secrets

**Code**: outputs-should-not-contain-secrets

**Description**: Don't include any values in an output that could potentially expose secrets. For example, secure parameters of type secureString or secureObject, or list* functions such as listKeys.

The output from a template is stored in the deployment history, so a malicious user could find that information.

The following example fails because it includes a secure parameter in an output value.
```bicep
@secure()
param secureParam string

output badResult string = 'this is the value ${secureParam}'
```

The following example fails because it uses a list* member function in an output.
```bicep
param storageName string
resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: storageName
}

output badResult object = {
  value: stg.listKeys().keys[0].value
}
```

The following example fails because it uses a list* function in an output.
```bicep
param storageName string
resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: storageName
}

output badResult object = {
  value: listKeys(resourceId('Microsoft.Storage/storageAccounts', stg.name), '2021-02-01')
}
```

The following example fails because the output name contains 'password', indicating that it may contains a secret
```bicep
output accountPassword string = '...'
```