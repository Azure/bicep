# No unnecessary dependsOn

**Code**: no-unnecessary-dependson

**Description**: To reduce confusion in your template, delete any dependsOn entries which are not necessary.  Bicep automatically infers most resource dependencies as long as you use expressions that reference other resources instead of hard-coded ids and names.

The following example fails this test because the dependsOn entry `appServicePlan` is already implied by the expression `appServicePlan.id` in the `serverFarmId` property's value.

```bicep
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    serverFarmId: appServicePlan.id
  }
  dependsOn: [
    appServicePlan
  ]
}
```

The following example passes this test.

```bicep
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

resource webApplication 'Microsoft.Web/sites@2018-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    serverFarmId: appServicePlan.id
  }
}
```
