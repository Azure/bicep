# Parameters Required

**Code**: params-required

**Description**: Your template should have a parameters element. Parameters are essential for making your templates reusable in different environments. Add parameters to your template for values that change when deploying to different environments.

The following example fails this test because parameters are not used.

```bicep
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: 'mygloballyuniquestorage'
    location: resourceGroup().location
    ...
}
```

The following example passes this test.

```bicep
@description('Primary location for all resources')
param location string = resourceGroup().location
@description('Prefix for storage account name')
param namePrefix string = 'stg'

var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  ...
}
```
