// FYI - there is a known bug for referencing properties of a deployments resource across scopes:
// https://github.com/Azure/bicep/issues/1828

param templateSpecSub string = subscription().subscriptionId
param templateSpecRg string = resourceGroup().name
param templateSpecName string
param templateSpecVersion string

// reference to existing template spec
resource ts 'Microsoft.Resources/templateSpecs@2019-06-01-preview' existing = {
  scope: resourceGroup(templateSpecSub, templateSpecRg)
  name: templateSpecName

  // reference to existing version
  resource tsVersion 'versions' existing = {
    name: templateSpecVersion
  }
}

// deploy template spec
resource deployTs 'Microsoft.Resources/deployments@2021-01-01' = {
  name: 'deployTs'
  // subscriptionId: '' // only needed for cross-sub deployments
  // resourceGroup: '' // only needed for cross-rg deployments
  properties: {
    mode: 'Incremental'
    templateLink: {
      id: ts::tsVersion.id
    }
    parameters: {
      // parameters go here
    }
  }
}
