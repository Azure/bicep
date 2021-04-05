@description('Specifies the location of AKS cluster.')
param location string = resourceGroup().location

@description('Specifies the name of the AKS cluster.')
param aksClusterName string = 'aks-${uniqueString(resourceGroup().id)}'

@description('Specifies the DNS prefix specified when creating the managed cluster.')
param aksClusterDnsPrefix string = aksClusterName

@description('Specifies the tags of the AKS cluster.')
param aksClusterTags object = {
  resourceType: 'AKS Cluster'
  createdBy: 'ARM Template'
}

@allowed([
  'azure'
  'kubenet'
])
@description('Specifies the network plugin used for building Kubernetes network. - azure or kubenet.')
param aksClusterNetworkPlugin string = 'azure'

@allowed([
  'azure'
  'calico'
])
@description('Specifies the network policy used for building Kubernetes network. - calico or azure')
param aksClusterNetworkPolicy string = 'azure'

@description('Specifies the CIDR notation IP range from which to assign pod IPs when kubenet is used.')
param aksClusterPodCidr string = '10.244.0.0/16'

@description('A CIDR notation IP range from which to assign service cluster IPs. It must not overlap with any Subnet IP ranges.')
param aksClusterServiceCidr string = '10.2.0.0/16'

@description('Specifies the IP address assigned to the Kubernetes DNS service. It must be within the Kubernetes service address range specified in serviceCidr.')
param aksClusterDnsServiceIP string = '10.2.0.10'

@description('Specifies the CIDR notation IP range assigned to the Docker bridge network. It must not overlap with any Subnet IP ranges or the Kubernetes service address range.')
param aksClusterDockerBridgeCidr string = '172.17.0.1/16'

@allowed([
  'basic'
  'standard'
])
@description('Specifies the sku of the load balancer used by the virtual machine scale sets used by nodepools.')
param aksClusterLoadBalancerSku string = 'standard'

@allowed([
  'Paid'
  'Free'
])
@description('Specifies the tier of a managed cluster SKU: Paid or Free')
param aksClusterSkuTier string = 'Paid'

@description('Specifies the version of Kubernetes specified when creating the managed cluster.')
param aksClusterKubernetesVersion string = '1.19.7'

@description('Specifies the administrator username of Linux virtual machines.')
param aksClusterAdminUsername string

@description('Specifies the SSH RSA public key string for the Linux nodes.')
param aksClusterSshPublicKey string

@description('Specifies whether enabling AAD integration.')
param aadEnabled bool = false

@description('Specifies the tenant id of the Azure Active Directory used by the AKS cluster for authentication.')
param aadProfileTenantId string = subscription().tenantId

@description('Specifies the AAD group object IDs that will have admin role of the cluster.')
param aadProfileAdminGroupObjectIDs array = []

@description('Specifies whether to create the cluster as a private cluster or not.')
param aksClusterEnablePrivateCluster bool = true

@description('Specifies whether to enable managed AAD integration.')
param aadProfileManaged bool = false

@description('Specifies whether to  to enable Azure RBAC for Kubernetes authorization.')
param aadProfileEnableAzureRBAC bool = false

@description('Specifies the unique name of the node pool profile in the context of the subscription and resource group.')
param nodePoolName string = 'nodepool1'

@description('Specifies the vm size of nodes in the node pool.')
param nodePoolVmSize string = 'Standard_DS3_v2'

@description('Specifies the OS Disk Size in GB to be used to specify the disk size for every machine in this master/agent pool. If you specify 0, it will apply the default osDisk size according to the vmSize specified..')
param nodePoolOsDiskSizeGB int = 100

@description('Specifies the number of agents (VMs) to host docker containers. Allowed values must be in the range of 1 to 100 (inclusive). The default value is 1.')
param nodePoolCount int = 3

@allowed([
  'Linux'
  'Windows'
])
@description('Specifies the OS type for the vms in the node pool. Choose from Linux and Windows. Default to Linux.')
param nodePoolOsType string = 'Linux'

@description('Specifies the maximum number of pods that can run on a node. The maximum number of pods per node in an AKS cluster is 250. The default maximum number of pods per node varies between kubenet and Azure CNI networking, and the method of cluster deployment.')
param nodePoolMaxPods int = 30

@description('Specifies the maximum number of nodes for auto-scaling for the node pool.')
param nodePoolMaxCount int = 3

@description('Specifies the minimum number of nodes for auto-scaling for the node pool.')
param nodePoolMinCount int = 3

@description('Specifies whether to enable auto-scaling for the node pool.')
param nodePoolEnableAutoScaling bool = true

@allowed([
  'Spot'
  'Regular'
])
@description('Specifies the virtual machine scale set priority: Spot or Regular.')
param nodePoolScaleSetPriority string = 'Regular'

@description('Specifies the Agent pool node labels to be persisted across all nodes in agent pool.')
param nodePoolNodeLabels object = {}

@description('Specifies the taints added to new nodes during node pool create and scale. For example, key=value:NoSchedule. - string')
param nodePoolNodeTaints array = []

@allowed([
  'System'
  'User'
])
@description('Specifies the mode of an agent pool: System or User')
param nodePoolMode string = 'System'

@allowed([
  'VirtualMachineScaleSets'
  'AvailabilitySet'
])
@description('Specifies the type of a node pool: VirtualMachineScaleSets or AvailabilitySet')
param nodePoolType string = 'VirtualMachineScaleSets'

@description('Specifies the availability zones for nodes. Requirese the use of VirtualMachineScaleSets as node pool type.')
param nodePoolAvailabilityZones array = []

@description('Specifies the name of the virtual network.')
param virtualNetworkName string = '${aksClusterName}Vnet'

@description('Specifies the address prefixes of the virtual network.')
param virtualNetworkAddressPrefixes string = '10.0.0.0/8'

@description('Specifies the name of the default subnet hosting the AKS cluster.')
param aksSubnetName string = 'AksSubnet'

@description('Specifies the address prefix of the subnet hosting the AKS cluster.')
param aksSubnetAddressPrefix string = '10.0.0.0/16'

@description('Specifies the name of the Log Analytics Workspace.')
param logAnalyticsWorkspaceName string

@allowed([
  'Free'
  'Standalone'
  'PerNode'
  'PerGB2018'
])
@description('Specifies the service tier of the workspace: Free, Standalone, PerNode, Per-GB.')
param logAnalyticsSku string = 'PerGB2018'

@description('Specifies the workspace data retention in days. -1 means Unlimited retention for the Unlimited Sku. 730 days is the maximum allowed for all other Skus.')
param logAnalyticsRetentionInDays int = 60

@description('Specifies the name of the subnet which contains the virtual machine.')
param vmSubnetName string = 'VmSubnet'

@description('Specifies the address prefix of the subnet which contains the virtual machine.')
param vmSubnetAddressPrefix string = '10.1.0.0/24'

@description('Specifies the name of the virtual machine.')
param vmName string = 'TestVm'

@description('Specifies the size of the virtual machine.')
param vmSize string = 'Standard_DS3_v2'

@description('Specifies the image publisher of the disk image used to create the virtual machine.')
param imagePublisher string = 'Canonical'

@description('Specifies the offer of the platform image or marketplace image used to create the virtual machine.')
param imageOffer string = 'UbuntuServer'

@description('Specifies the Ubuntu version for the VM. This will pick a fully patched image of this given Ubuntu version.')
param imageSku string = '18.04-LTS'

@allowed([
  'sshPublicKey'
  'password'
])
@description('Specifies the type of authentication when accessing the Virtual Machine. SSH key is recommended.')
param authenticationType string = 'password'

@description('Specifies the name of the administrator account of the virtual machine.')
param vmAdminUsername string

@description('Specifies the SSH Key or password for the virtual machine. SSH key is recommended.')
@secure()
param vmAdminPasswordOrKey string

@allowed([
  'Premium_LRS'
  'StandardSSD_LRS'
  'Standard_LRS'
  'UltraSSD_LRS'
])
@description('Specifies the storage account type for OS and data disk.')
param diskStorageAccounType string = 'Premium_LRS'

@minValue(0)
@maxValue(64)
@description('Specifies the number of data disks of the virtual machine.')
param numDataDisks int = 1

@description('Specifies the size in GB of the OS disk of the VM.')
param osDiskSize int = 50

@description('Specifies the size in GB of the OS disk of the virtual machine.')
param dataDiskSize int = 50

@description('Specifies the caching requirements for the data disks.')
param dataDiskCaching string = 'ReadWrite'

@description('Specifies the globally unique name for the storage account used to store the boot diagnostics logs of the virtual machine.')
param blobStorageAccountName string = 'blob${uniqueString(resourceGroup().id)}'

@description('Specifies the name of the private link to the boot diagnostics storage account.')
param blobStorageAccountPrivateEndpointName string = 'BlobStorageAccountPrivateEndpoint'

@description('Specifies the Bastion subnet IP prefix. This prefix must be within vnet IP prefix address space.')
param bastionSubnetAddressPrefix string = '10.1.1.0/26'

@description('Specifies the name of the Azure Bastion resource.')
param bastionHostName string = '${aksClusterName}Bastion'

var vmSubnetNsgName = '${vmSubnetName}Nsg'
//var vmSubnetNsgId = vmSubnetNsgName.id
var bastionSubnetNsgName = '${bastionHostName}Nsg'
//var bastionSubnetNsgId = bastionSubnetNsg.id
//var vnetId = virtualNetworkName_resource.id
var vmSubnetId = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetwork.name, vmSubnetName)
var aksSubnetId = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetwork.name, aksSubnetName)
var vmNicName = '${vmName}Nic'
var vmNicId = vmNic.id
var blobStorageAccountId = blobStorageAccount.id
var blobPublicDNSZoneForwarder = '.blob.${environment().suffixes.storage}'
var blobPrivateDnsZoneName = 'privatelink${blobPublicDNSZoneForwarder}'
//var blobPrivateDnsZoneId = blobPrivateDnsZoneName.id
var blobStorageAccountPrivateEndpointGroupName = 'blob'
var blobPrivateDnsZoneGroupName = '${blobStorageAccountPrivateEndpoint.name}/${blobStorageAccountPrivateEndpointGroupName}PrivateDnsZoneGroup'
//var blobStorageAccountPrivateEndpointId = blobStorageAccountPrivateEndpointName_resource.id
//var vmId = vmName_resource.id
var omsAgentForLinuxName = 'LogAnalytics'
//var omsAgentForLinuxId = omsAgentForLinux.id
var omsDependencyAgentForLinuxName = 'DependencyAgent'
var linuxConfiguration = {
  disablePasswordAuthentication: true
  ssh: {
    publicKeys: [
      {
        path: '/home/${vmAdminUsername}/.ssh/authorized_keys'
        keyData: vmAdminPasswordOrKey
      }
    ]
  }
  provisionVMAgent: true
}
var bastionPublicIpAddressName = '${bastionHostName}PublicIp'
//var bastionPublicIpAddressId = bastionPublicIpAddress.id
var bastionSubnetName = 'AzureBastionSubnet'
var bastionSubnetId = resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetwork.name, bastionSubnetName)
//var workspaceId = logAnalyticsWorkspaceName_resource.id
var aadProfileConfiguration = {
  managed: aadProfileManaged
  enableAzureRBAC: aadProfileEnableAzureRBAC
  adminGroupObjectIDs: aadProfileAdminGroupObjectIDs
  tenantID: aadProfileTenantId
}

resource bastionPublicIpAddress 'Microsoft.Network/publicIPAddresses@2020-08-01' = {
  name: bastionPublicIpAddressName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
  }
}

resource bastionSubnetNsg 'Microsoft.Network/networkSecurityGroups@2020-08-01' = {
  name: bastionSubnetNsgName
  location: location
  properties: {
    securityRules: [
      {
        name: 'bastionInAllow'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: 'Internet'
          destinationPortRange: '443'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 100
          direction: 'Inbound'
        }
      }
      {
        name: 'bastionControlInAllow'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: 'GatewayManager'
          destinationPortRanges: [
            '443'
            '4443'
          ]
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 120
          direction: 'Inbound'
        }
      }
      {
        name: 'AllowLoadBalancerInBound'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: 'AzureLoadBalancer'
          destinationPortRange: '443'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 130
          direction: 'Inbound'
        }
      }
      {
        name: 'AllowBastionHostCommunicationInBound'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationPortRanges: [
            '8080'
            '5701'
          ]
          destinationAddressPrefix: 'VirtualNetwork'
          access: 'Allow'
          priority: 140
          direction: 'Inbound'
        }
      }
      {
        name: 'bastionInDeny'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Deny'
          priority: 900
          direction: 'Inbound'
        }
      }
      {
        name: 'bastionVnetOutAllow'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: '*'
          destinationPortRanges: [
            '22'
            '3389'
          ]
          destinationAddressPrefix: 'VirtualNetwork'
          access: 'Allow'
          priority: 100
          direction: 'Outbound'
        }
      }
      {
        name: 'bastionAzureOutAllow'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: '*'
          destinationPortRange: '443'
          destinationAddressPrefix: 'AzureCloud'
          access: 'Allow'
          priority: 120
          direction: 'Outbound'
        }
      }
    ]
  }
}

resource bastionHost 'Microsoft.Network/bastionHosts@2020-08-01' = {
  name: bastionHostName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'IpConf'
        properties: {
          subnet: {
            id: bastionSubnetId
          }
          publicIPAddress: {
            id: bastionPublicIpAddress.id
          }
        }
      }
    ]
  }
}

resource blobStorageAccount 'Microsoft.Storage/storageAccounts@2021-01-01' = {
  name: blobStorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource vmNic 'Microsoft.Network/networkInterfaces@2020-08-01' = {
  name: vmNicName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: vmSubnetId
          }
        }
      }
    ]
  }
}

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: vmName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: vmName
      adminUsername: vmAdminUsername
      adminPassword: vmAdminPasswordOrKey
      linuxConfiguration: ((authenticationType == 'password') ? json('null') : linuxConfiguration)
    }
    storageProfile: {
      imageReference: {
        publisher: imagePublisher
        offer: imageOffer
        sku: imageSku
        version: 'latest'
      }
      osDisk: {
        name: '${vmName}_OSDisk'
        caching: 'ReadWrite'
        createOption: 'FromImage'
        diskSizeGB: osDiskSize
        managedDisk: {
          storageAccountType: diskStorageAccounType
        }
      }
      dataDisks: [for j in range(0, numDataDisks): {
        caching: dataDiskCaching
        diskSizeGB: dataDiskSize
        lun: j
        name: '${vmName}-DataDisk${j}'
        createOption: 'Empty'
        managedDisk: {
          storageAccountType: diskStorageAccounType
        }
      }]
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: vmNic.id
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: reference(blobStorageAccountId).primaryEndpoints.blob
      }
    }
  }
}

resource omsAgentForLinux 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
  name: '${vm.name}/${omsAgentForLinuxName}'
  location: location
  properties: {
    publisher: 'Microsoft.EnterpriseCloud.Monitoring'
    type: 'OmsAgentForLinux'
    typeHandlerVersion: '1.12'
    settings: {
      workspaceId: reference(logAnalyticsWorkspace.id, '2020-03-01-preview').customerId
      stopOnMultipleConnections: false
    }
    protectedSettings: {
      workspaceKey: listKeys(logAnalyticsWorkspace.id, '2020-03-01-preview').primarySharedKey
    }
  }
}

resource omsDependencyAgentForLinux 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
  name: '${vm.name}/${omsDependencyAgentForLinuxName}'
  location: location
  properties: {
    publisher: 'Microsoft.Azure.Monitoring.DependencyAgent'
    type: 'DependencyAgentLinux'
    typeHandlerVersion: '9.10'
    autoUpgradeMinorVersion: true
  }
}

resource vmSubnetNsg 'Microsoft.Network/networkSecurityGroups@2020-05-01' = {
  name: vmSubnetNsgName
  location: location
  properties: {
    securityRules: [
      {
        name: 'AllowSshInbound'
        properties: {
          priority: 100
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '22'
          protocol: 'Tcp'
          sourceAddressPrefix: '*'
          sourcePortRange: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-08-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        virtualNetworkAddressPrefixes
      ]
    }
    subnets: [
      {
        name: aksSubnetName
        properties: {
          addressPrefix: aksSubnetAddressPrefix
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
      {
        name: vmSubnetName
        properties: {
          addressPrefix: vmSubnetAddressPrefix
          networkSecurityGroup: {
            id: vmSubnetNsg.id
          }
          privateEndpointNetworkPolicies: 'Disabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
      {
        name: bastionSubnetName
        properties: {
          addressPrefix: bastionSubnetAddressPrefix
          networkSecurityGroup: {
            id: bastionSubnetNsg.id
          }
        }
      }
    ]
    enableDdosProtection: false
    enableVmProtection: false
  }
}

resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-02-01' = {
  name: aksClusterName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  tags: aksClusterTags
  sku: {
    name: 'Basic'
    tier: aksClusterSkuTier
  }
  properties: {
    kubernetesVersion: aksClusterKubernetesVersion
    dnsPrefix: aksClusterDnsPrefix
    agentPoolProfiles: [
      {
        name: toLower(nodePoolName)
        count: nodePoolCount
        vmSize: nodePoolVmSize
        osDiskSizeGB: nodePoolOsDiskSizeGB
        vnetSubnetID: aksSubnetId
        maxPods: nodePoolMaxPods
        osType: nodePoolOsType
        maxCount: nodePoolMaxCount
        minCount: nodePoolMinCount
        scaleSetPriority: nodePoolScaleSetPriority
        enableAutoScaling: nodePoolEnableAutoScaling
        mode: nodePoolMode
        type: nodePoolType
        availabilityZones: nodePoolAvailabilityZones
        nodeLabels: nodePoolNodeLabels
        nodeTaints: nodePoolNodeTaints
      }
    ]
    linuxProfile: {
      adminUsername: aksClusterAdminUsername
      ssh: {
        publicKeys: [
          {
            keyData: aksClusterSshPublicKey
          }
        ]
      }
    }
    addonProfiles: {
      omsagent: {
        enabled: true
        config: {
          logAnalyticsWorkspaceResourceID: logAnalyticsWorkspace.id
        }
      }
    }
    enableRBAC: true
    networkProfile: {
      networkPlugin: aksClusterNetworkPlugin
      networkPolicy: aksClusterNetworkPolicy
      podCidr: aksClusterPodCidr
      serviceCidr: aksClusterServiceCidr
      dnsServiceIP: aksClusterDnsServiceIP
      dockerBridgeCidr: aksClusterDockerBridgeCidr
      loadBalancerSku: aksClusterLoadBalancerSku
    }
    aadProfile: (aadEnabled ? aadProfileConfiguration : json('null'))
    apiServerAccessProfile: {
      enablePrivateCluster: aksClusterEnablePrivateCluster
    }
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: logAnalyticsSku
    }
    retentionInDays: logAnalyticsRetentionInDays
  }
}

resource blobPrivateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: blobPrivateDnsZoneName
  location: 'global'
  properties: {
    //maxNumberOfRecordSets: 25000
    //maxNumberOfVirtualNetworkLinks: 1000
    //maxNumberOfVirtualNetworkLinksWithRegistration: 100
  }
}

resource blobPrivateDnsZone_link_to_virtualNetwork 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  name: '${blobPrivateDnsZone.name}/link_to_${toLower(virtualNetworkName)}'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
}

resource blobStorageAccountPrivateEndpoint 'Microsoft.Network/privateEndpoints@2020-08-01' = {
  name: blobStorageAccountPrivateEndpointName
  location: location
  properties: {
    privateLinkServiceConnections: [
      {
        name: blobStorageAccountPrivateEndpointName
        properties: {
          privateLinkServiceId: blobStorageAccountId
          groupIds: [
            blobStorageAccountPrivateEndpointGroupName
          ]
        }
      }
    ]
    subnet: {
      id: vmSubnetId
    }
    customDnsConfigs: [
      {
        fqdn: concat(blobStorageAccountName, blobPublicDNSZoneForwarder)
      }
    ]
  }
}

resource blobPrivateDnsZoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2020-08-01' = {
  name: blobPrivateDnsZoneGroupName
  //  location: location
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'dnsConfig'
        properties: {
          privateDnsZoneId: blobPrivateDnsZone.id
        }
      }
    ]
  }
}
