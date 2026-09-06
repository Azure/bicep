targetScope = 'subscription'

metadata description = 'Deploys the module with its default settings.'

module example '../../main.bicep' = {
  name: 'example'
  params: {
    resourceGroupName: 'example-rg'
    secret: 'example'
  }
}
