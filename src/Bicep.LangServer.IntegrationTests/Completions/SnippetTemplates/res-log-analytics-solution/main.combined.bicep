// $1 = operationalInsightsWorkspace
// $2 = 'name'
// $3 = logAnalyticsSolution
// $4 = 'name'
// $5 = 'id'
// $6 = 'name'
// $7 = 'product'
// $8 = 'publisher'
// $9 = 'promotionCode'

resource operationalInsightsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: 'name'
}

resource logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    workspaceResourceId: operationalInsightsWorkspace.id
    containedResources: [
      'id'
    ]
  }
  plan: {
    name: 'name'
    product: 'product'
    publisher: 'publisher'
    promotionCode: 'promotionCode'
  }
}
// Insert snippet here
