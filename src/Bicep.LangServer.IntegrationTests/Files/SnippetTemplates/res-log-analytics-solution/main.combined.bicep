// $1 = logAnalyticsSolution
// $2 = 'name'
// $3 = location
// $4 = 'operationalInsightsWorkspace.id'
// $5 = 'view.id'
// $6 = 'name'
// $7 = 'product'
// $8 = 'publisher'
// $9 = 'promotionCode'

param location string

resource logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: 'name'
  location: location
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

