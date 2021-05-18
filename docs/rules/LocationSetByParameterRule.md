# Location set by parameter

Your templates should have a parameter named location. Use this parameter for setting the location of resources in your template. In the main template, this parameter can default to the resource group location.

Users of your template may have limited regions available to them. When you hard code the resource location, users may be blocked from creating a resource in that region. Users could be blocked even if you set the resource location to "[resourceGroup().location]". The resource group may have been created in a region that other users can't access. Those users are blocked from using the template.

By providing a location parameter that defaults to the resource group location, users can use the default value when convenient but also specify a different location.

The following example fails this test because location on the resource is set to resourceGroup().location.

```bicep
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: resourceGroup().location
    ...
}
```

The next example uses a location parameter but fails this test because the location parameter defaults to a hardcoded location.

```bicep
param location string = 'westus'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    ...
}
```

Instead, create a parameter that defaults to the resource group location but allows users to provide a different value. The following example passes this test when the template is used as the main template.

```bicep
param location string = resourceGroup().location

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    ...
}
```
