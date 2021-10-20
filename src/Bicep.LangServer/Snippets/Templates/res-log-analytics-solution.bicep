// Log Analytics Solution
resource /*${1:logAnalyticsSolution}*/logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    workspaceResourceId: /*${3:'operationalInsightsWorkspace.id'}*/'operationalInsightsWorkspace.id'
    containedResources: [
      /*${4:'view.id'}*/'view.id'
    ]
  }
  plan: {
    name: /*${5:'name'}*/'name'
    product: /*${6:'product'}*/'product'
    publisher: /*${7:'publisher'}*/'publisher'
    promotionCode: /*${8:'promotionCode'}*/'promotionCode'
  }
}
