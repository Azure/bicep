// Log Analytics Solution
resource ${1:logAnalyticsSolution} 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    workspaceResourceId: resourceId('Microsoft.OperationalInsights/workspaces', ${3:'logAnalyticsWorkspace'})
    containedResources: [
      resourceId('Microsoft.OperationalInsights/workspaces/views', ${3:'logAnalyticsWorkspace'}, ${4:'logAnalyticsSolution'})
    ]
  }
  plan: {
    name: ${5:'name'}
    product: ${6:'product'}
    publisher: ${7:'publisher'}
    promotionCode: ${8:'promotionCode'}
  }
}
