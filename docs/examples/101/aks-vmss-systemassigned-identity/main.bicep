// params
param kubernetesVersion string = '1.19.0'
param dnsPrefix string = 'cl01'
param clusterName string = 'aks101'
param location string = resourceGroup().location
param agentCount int {
  default: 1
  minValue: 1
  maxValue: 50
}
param agentVMSize string = 'Standard_D2_v3'

// vars
var subnetRef = '${vn.id}/subnets/${subnetName}'
var addressPrefix =  '20.0.0.0/16'
var subnetName = 'Subnet01'
var subnetPrefix = '20.0.0.0/24'
var virtualNetworkName =  'MyVNET01'
var nodeResourceGroup = 'rg-${dnsPrefix}-${clusterName}'
var tags = {
  environment: 'production'
  projectCode: 'xyz'
}
var agentPoolName = 'agentpool01'

// Azure virtual network
resource vn 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        addressPrefix
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetPrefix
        }
      }
    ] 
  }
}

// Azure kubernetes service
resource aks 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
  name: clusterName
  location: location
  tags: tags
  identity: {
      type: 'SystemAssigned'
    }
  properties: {
    kubernetesVersion: kubernetesVersion
    enableRBAC: true
    dnsPrefix: dnsPrefix
    agentPoolProfiles: [
      {
        name: agentPoolName
        count: agentCount
        mode: 'System'
        vmSize: agentVMSize
        type: 'VirtualMachineScaleSets'
        osType: 'Linux'
        enableAutoScaling: false
        vnetSubnetID: subnetRef
      }
    ]
    servicePrincipalProfile: {
        clientId: 'msi'
      }
    nodeResourceGroup: nodeResourceGroup
    networkProfile: {
        networkPlugin: 'azure'
        loadBalancerSku: 'standard'
      }
  }
}

output id string = aks.id
output apiServerAddress string = aks.properties.fqdn