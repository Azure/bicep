param workspaceName string

@allowed([
  'standard'
  'premium'
])
param pricingTier string = 'premium'

param location string = resourceGroup().location

var managedResourceGroupName = 'databricks-rg-${workspaceName}-${uniqueString(workspaceName, resourceGroup().id)}'

resource ws 'Microsoft.Databricks/workspaces@2018-04-01' = {
  name: workspaceName
  location: location
  sku: {
    name: pricingTier
  }
  properties: {
    // TODO: improve once we have scoping functions
    managedResourceGroupId: '${subscription().id}/resourceGroups/${managedResourceGroupName}'
  }
}

output workspace object = ws
