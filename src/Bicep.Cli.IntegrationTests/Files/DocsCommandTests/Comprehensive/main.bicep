targetScope = 'subscription'

metadata name = 'Comprehensive Module'
metadata description = '''
Exercises every documentation feature | with multiline details.
Second line.
'''

@description('Deployment location.')
param location string = deployment().location

@description('''
Resource group | name.
Second line.
''')
@minLength(3)
@maxLength(90)
param resourceGroupName string

@description('Deployment tier.')
@allowed([
  'Standard'
  'Premium'
])
param tier string = 'Standard'

@description('Retention period in days.')
@minValue(1)
@maxValue(365)
param retentionInDays int = 30

@description('Names assigned to the deployment.')
@minLength(1)
@maxLength(5)
param names string[] = [
  'default'
]

@description('Secret used by the child module.')
@secure()
param secret string

@description('Network access configuration.')
param networkAccess networkAccessType = {
  kind: 'public'
}

@description('Nested settings.')
param settings settingsType = {
  enabled: true
  labels: {
    environment: 'test'
  }
}

@description('Enables anonymous usage telemetry.')
param enableTelemetry bool = false

@export()
@description('Nested module settings.')
type settingsType = {
  @description('Whether the feature is enabled.')
  enabled: bool
  @description('Labels applied to resources.')
  labels: {
    @description('Environment label.')
    environment: string
  }
}

@export()
type publicAccessType = {
  kind: 'public'
}

@export()
type restrictedAccessType = {
  kind: 'restricted'
  @description('Allowed CIDR ranges.')
  allowedCidrs: string[]
}

@export()
@discriminator('kind')
type networkAccessType = publicAccessType | restrictedAccessType

resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
  tags: {
    tier: tier
    retentionInDays: string(retentionInDays)
    primaryName: names[0]
    settingsEnabled: string(settings.enabled)
    networkAccessKind: networkAccess.kind
  }
}

resource existingResourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' existing = {
  name: 'existing-resource-group'
}

module logging 'modules/logging.bicep' = {
  name: 'logging'
  scope: resourceGroup
  params: {
    secret: secret
  }
}

@export()
@description('Builds a display name.')
func buildDisplayName(prefix string) string => '${prefix}-deployment'

@description('The deployed resource group ID.')
output resourceGroupId string = resourceGroup.id

@description('The existing resource group ID.')
output existingResourceGroupId string = existingResourceGroup.id

@secure()
@description('A secure output used to exercise output metadata.')
output secureValue string = secret
