// Log Analytics Solution
resource /*${1:operationalInsightsWorkspace}*/operationalInsightsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: /*${2:'name'}*/'name'
}

resource /*${3:logAnalyticsSolution}*/logAnalyticsSolution 'Microsoft.OperationsManagement/solutions@2015-11-01-preview' = {
  name: /*${4:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    workspaceResourceId: operationalInsightsWorkspace.id
    containedResources: [
      /*${5:'id'}*/'id'
    ]
  }
  plan: {
    name: /*${6:'name'}*/'name'
    product: /*${7:'product'}*/'product'
    publisher: /*${8:'publisher'}*/'publisher'
    promotionCode: /*${9:'promotionCode'}*/'promotionCode'
  }
}
