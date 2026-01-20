// Bicep deployment for Bicep Extension Host on Azure Kubernetes Service (AKS)
// This creates an AKS cluster. The application deployment is done separately via kubectl.
// NOTE: AKS provides full control over process spawning and is ideal for this workload.

targetScope = 'resourceGroup'

// ============================================================================
// Parameters
// ============================================================================

@description('The location for all resources')
param location string = resourceGroup().location

@description('The name prefix for all resources')
@minLength(3)
@maxLength(11)
param namePrefix string = 'bicepext'

@description('The VM size for the AKS nodes')
param nodeVmSize string = 'Standard_B2s'

@description('The number of nodes in the AKS cluster')
@minValue(1)
@maxValue(10)
param nodeCount int = 1

@description('The Kubernetes version')
param kubernetesVersion string = '1.34'

@description('Tags to apply to all resources')
param tags object = {}

// ============================================================================
// Variables
// ============================================================================

var resourceToken = toLower(uniqueString(subscription().id, resourceGroup().id, location))
var abbrs = loadJsonContent('../abbreviations.json')

var aksClusterName = '${abbrs.containerServiceManagedClusters}${namePrefix}-${resourceToken}'
var logAnalyticsName = '${abbrs.operationalInsightsWorkspaces}${namePrefix}-${resourceToken}'

// ============================================================================
// Resources
// ============================================================================

// Log Analytics Workspace for AKS monitoring
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// AKS Cluster
resource aksCluster 'Microsoft.ContainerService/managedClusters@2024-09-01' = {
  name: aksClusterName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: '${namePrefix}-${resourceToken}'
    kubernetesVersion: kubernetesVersion
    agentPoolProfiles: [
      {
        name: 'default'
        count: nodeCount
        vmSize: nodeVmSize
        mode: 'System'
        osType: 'Linux'
        osSKU: 'AzureLinux'
      }
    ]
    addonProfiles: {
      omsagent: {
        enabled: true
        config: {
          logAnalyticsWorkspaceResourceID: logAnalytics.id
        }
      }
    }
    ingressProfile: {
      webAppRouting: {
        enabled: true
      }
    }
    networkProfile: {
      networkPlugin: 'azure'
      loadBalancerSku: 'standard'
    }
  }
}

// ============================================================================
// Outputs
// ============================================================================

@description('The name of the AKS cluster')
output aksClusterName string = aksCluster.name

@description('The FQDN of the AKS cluster')
output aksClusterFqdn string = aksCluster.properties.fqdn

@description('The name of the Log Analytics Workspace')
output logAnalyticsWorkspaceName string = logAnalytics.name

@description('Command to get AKS credentials')
output getCredentialsCommand string = 'az aks get-credentials --resource-group ${resourceGroup().name} --name ${aksCluster.name}'
