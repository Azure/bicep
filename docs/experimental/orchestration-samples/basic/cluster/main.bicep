@description('The name of the managed environment')
param managedEnvironmentName string

@description('Whether to enable zone redundancy for the managed environment')
param zoneRedundant bool

module managedEnvironment 'br/public:avm/res/app/managed-environment:0.11.3' = {
  params: {
    name: managedEnvironmentName
    publicNetworkAccess: 'Enabled'
    infrastructureResourceGroupName: '${managedEnvironmentName}-infra-rg'
    zoneRedundant: zoneRedundant
  }
}
