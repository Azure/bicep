// $1 = logAnalyticsSolution
// $2 = 'name'
// $3 = 'operationalInsightsWorkspace.id'
// $4 = 'view.id'
// $5 = 'name'
// $6 = 'product'
// $7 = 'publisher'
// $8 = 'promotionCode'

resource logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    workspaceResourceId: 'operationalInsightsWorkspace.id'
    containedResources: [
      'view.id'
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

