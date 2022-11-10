// Log Analytics Solution
resource /*${1:logAnalyticsSolution}*/logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    workspaceResourceId: /*${4:'operationalInsightsWorkspace.id'}*/'operationalInsightsWorkspace.id'
    containedResources: [
      /*${5:'view.id'}*/'view.id'
    ]
  }
  plan: {
    name: /*${6:'name'}*/'name'
    product: /*${7:'product'}*/'product'
    publisher: /*${8:'publisher'}*/'publisher'
    promotionCode: /*${9:'promotionCode'}*/'promotionCode'
  }
}
