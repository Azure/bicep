// Log Analytics Solution
resource /*${1:logAnalyticsSolution}*/logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    workspaceResourceId: resourceId('Microsoft.OperationalInsights/workspaces', /*${3:'logAnalyticsWorkspace'}*/'logAnalyticsWorkspace')
    containedResources: [
      resourceId('Microsoft.OperationalInsights/workspaces/views', /*${3:'logAnalyticsWorkspace'}*/'logAnalyticsWorkspace', /*${4:'logAnalyticsSolution'}*/'logAnalyticsSolution')
    ]
  }
  plan: {
    name: /*${5:'name'}*/'name'
    product: /*${6:'product'}*/'product'
    publisher: /*${7:'publisher'}*/'publisher'
    promotionCode: /*${8:'promotionCode'}*/'promotionCode'
  }
}
