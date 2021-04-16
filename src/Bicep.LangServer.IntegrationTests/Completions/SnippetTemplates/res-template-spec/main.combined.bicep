resource templatespec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: 'myTemplateSpecName'
  location: resourceGroup().location
  properties: {
    description: 'myTemplateSpecFriendlyName'
    displayName: 'myTemplateSpecDisplayName'
  }
}
