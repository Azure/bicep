param skuName string
param cdnProfileName string
param afdEndpointName string
param enableAfdEndpoint bool
param cdnProfileTags object

@description('Create CDN Profile')
resource cdnProfile 'Microsoft.Cdn/profiles@2021-06-01' = {
  name: cdnProfileName
  location: 'Global'
  tags: cdnProfileTags
  sku: {
    name: skuName
  }
  properties: {
    originResponseTimeoutSeconds: 60
  }
}

@description('Create AFD Endpoint')
resource cdnProfileName_afdEndpoint 'Microsoft.Cdn/profiles/afdEndpoints@2021-06-01' = {
  parent: cdnProfile
  name: '${afdEndpointName}'
//@[8:28) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#simplify-interpolation) |'${afdEndpointName}'|
  location: 'Global'
  properties: {
    enabledState: (enableAfdEndpoint ? 'Enabled' : 'Disabled')
  }
}

