/*
  This template requires a nested deployment, which we don't yet support natively
*/

param _artifactsLocation string = 'https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/301-nested-vms-in-virtual-network/'
param _artifactsLocationSasToken string {
  secure: true
  default: ''
}
param location string = resourceGroup().location
param HostPublicIPAddressName string = 'HVHOSTPIP'
param virtualNetworkName string = 'VirtualNetwork'
param virtualNetworkAddressPrefix string = '10.0.0.0/22'
param NATSubnetName string = 'NAT'
param NATSubnetPrefix string = '10.0.0.0/24'
param hyperVSubnetName string = 'Hyper-V-LAN'
param hyperVSubnetPrefix string = '10.0.1.0/24'

param ghostedSubnetName string = 'Ghosted'
param ghostedSubnetPrefix string = '10.0.2.0/24'
param azureVMsSubnetName string = 'Azure-VMs'
param azureVMsSubnetPrefix string = '10.0.3.0/24'
param HostNetworkInterface1Name string = 'HVHOSTNIC1'
param HostNetworkInterface2Name string = 'HVHOSTNIC2'
param HostVirtualMachineName string {
  default: 'HVHOST'
  maxLength: 15
}
param HostVirtualMachineSize string {
  default: 'Standard_D4s_v3'
  allowed: [
    'Standard_D2_v3'
    'Standard_D4_v3'
    'Standard_D8_v3'
    'Standard_D16_v3'
    'Standard_D32_v3'
    'Standard_D2s_v3'
    'Standard_D4s_v3'
    'Standard_D8s_v3'
    'Standard_D16s_v3'
    'Standard_D32s_v3'
    'Standard_D64_v3'
    'Standard_E2_v3'
    'Standard_E4_v3'
    'Standard_E8_v3'
    'Standard_E16_v3'
    'Standard_E32_v3'
    'Standard_E64_v3'
    'Standard_D64s_v3'
    'Standard_E2s_v3'
    'Standard_E4s_v3'
    'Standard_E8s_v3'
    'Standard_E16s_v3'
    'Standard_E32s_v3'
    'Standard_E64s_v3'
  ]
}
param HostAdminUsername string
param HostAdminPassword string

var NATSubnetNSGName = '${NATSubnetName}NSG'
var hyperVSubnetNSGName = '${hyperVSubnetName}NSG'
var ghostedSubnetNSGName = '${ghostedSubnetName}NSG'
var azureVMsSubnetNSGName = '${azureVMsSubnetName}NSG'
var azureVMsSubnetUDRName = '${azureVMsSubnetName}UDR'
var DSCInstallWindowsFeaturesUri = uri(_artifactsLocation, 'dsc/dscinstallwindowsfeatures.zip${_artifactsLocationSasToken}')
var HVHostSetupScriptUri = uri(_artifactsLocation, 'hvhostsetup.ps1${_artifactsLocationSasToken}')

resource publicIp 'Microsoft.Network/publicIpAddresses@2019-04-01' = {
  name: HostPublicIPAddressName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: toLower('${HostVirtualMachineName}-${uniqueString(resourceGroup().id)}')
    }
  }
}

resource natNsg 'Microsoft.Network/networkSecurityGroups@2019-04-01' = {
  name: NATSubnetNSGName
  location: location
  properties: {
  }
}

resource hyperVNsg 'Microsoft.Network/networkSecurityGroups@2019-04-01' = {
  name: hyperVSubnetNSGName
  location: location
  properties: {
  }
}

resource ghostedNsg 'Microsoft.Network/networkSecurityGroups@2019-04-01' = {
  name: ghostedSubnetNSGName
  location: location
  properties: {
  }
}

resource azureVmsSubnet 'Microsoft.Network/networkSecurityGroups@2019-04-01' = {
  name: azureVMsSubnetNSGName
  location: location
  properties: {
  }
}

resource azureVmsUdr 'Microsoft.Network/routeTables@2019-04-01' = {
  name: azureVMsSubnetUDRName
  location: location
  properties: {
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2019-04-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        virtualNetworkAddressPrefix
      ]
    }
    subnets: [
      {
        name: NATSubnetName
        properties: {
          addressPrefix: NATSubnetPrefix
          networkSecurityGroup: {
            id: natNsg.id
          }
        }
      }
      {
        name: hyperVSubnetName
        properties: {
          addressPrefix: hyperVSubnetPrefix
          networkSecurityGroup: {
            id: hyperVNsg.id
          }
        }
      }
      {
        name: hyperVSubnetName
        properties: {
          addressPrefix: hyperVSubnetPrefix
          networkSecurityGroup: {
            id: hyperVNsg.id
          }
        }
      }
      {
        name: ghostedSubnetName
        properties: {
          addressPrefix: ghostedSubnetPrefix
          networkSecurityGroup: {
            id: ghostedNsg.id
          }
        }
      }
      {
        name: azureVMsSubnetName
        properties: {
          addressPrefix: azureVMsSubnetPrefix
          networkSecurityGroup: {
            id: azureVmsSubnet.id
          }
          routeTable: {
            id: azureVmsUdr.id
          }
        }
      }
    ]
  }
}

resource nic1 'Microsoft.Network/networkInterfaces@2019-04-01' = {
  name: HostNetworkInterface1Name
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          primary: true
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: '${vnet}/subnets${NATSubnetName}'
          }
          publicIPAddress: {
            id: publicIp.id
          }
        }
      }
    ]
  }
}

resource nic2 'Microsoft.Network/networkInterfaces@2019-04-01' = {
  name: HostNetworkInterface2Name
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          primary: true
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: '${vnet}/subnets${hyperVSubnetName}'
          }
        }
      }
    ]
    enableIPForwarding: true
  }
}

resource hostVm 'Microsoft.Compute/virtualMachines@2019-03-01' = {
  name: HostVirtualMachineName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: HostVirtualMachineSize
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2016-Datacenter'
        version: 'latest'
      }
      osDisk: {
        name: '${HostVirtualMachineName}OsDisk'
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'Premium_LRS'
        }
        caching: 'ReadWrite'
      }
      dataDisks: [
        {
          lun: 0
          name: '${HostVirtualMachineName}DataDisk1'
          createOption: 'Empty'
          diskSizeGB: 1024
          caching: 'ReadOnly'
          managedDisk: {
            storageAccountType: 'Premium_LRS'
          }
        }
      ]
    }
    osProfile: {
      computerName: HostVirtualMachineName
      adminUsername: HostAdminUsername
      adminPassword: HostAdminPassword
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: nic1.id
          properties: {
            primary: true
          }
        }
        {
          id: nic2.id
          properties: {
            primary: false
          }
        }
      ]
    }
  }
}

resource vmExtension 'Microsoft.Compute/virtualMachines/extensions@2019-03-01' = {
  name: '${hostVm.id}/InstallWindowsFeatures'
  location: location
  properties: {
    publisher: 'Microsoft.Powershell'
    type: 'DSC'
    typeHandlerVersion: '2.77'
    autoUpgradeMinorVersion: true
    settings: {
      wmfVersion: 'latest'
      configuration: {
        url: DSCInstallWindowsFeaturesUri
        script: 'DSCInstallWindowsFeatures.ps1'
        function: 'InstallWindowsFeatures'
      }
    }
  }
}

resource hostVmSetupExtension 'Microsoft.Compute/virtualMachines/extensions@2019-03-01' = {
  name: '${hostVm.id}/HVHOSTSetup'
  location: location
  properties: {
    publisher: 'Microsoft.Compute'
    type: 'CustomScriptExtension'
    typeHandlerVersion: '1.9'
    autoUpgradeMinorVersion: true
    settings: {
      fileUris: [
        HVHostSetupScriptUri
      ]
      commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File HVHostSetup.ps1 -NIC1IPAddress ${nic1.properties.ipConfigurations[0].properties.privateIPAddress} -NIC2IPAddress ${nic2.properties.ipConfigurations[0].properties.privateIPAddress} -GhostedSubnetPrefix ${ghostedSubnetPrefix} -VirtualNetworkPrefix ${virtualNetworkAddressPrefix}'
    }
  }
}

/*
TODO:

      {
        "type": "Microsoft.Resources/deployments",
        "apiVersion": "2019-05-01",
        "name": "UpdateNetworking",
        "dependsOn": [
          "[parameters('HostNetworkInterface1Name')]",
          "[parameters('HostNetworkInterface2Name')]"
        ],
        "properties": {
          "mode": "Incremental",
          "template": {
            "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
            "contentVersion": "1.0.0.0",
            "resources": [
              {
                "type": "Microsoft.Network/routeTables",
                "apiVersion": "2019-04-01",
                "name": "[variables('azureVMsSubnetUDRName')]",
                "location": "[parameters('location')]",
                "properties": {
                  "routes": [
                    {
                      "name": "Nested-VMs",
                      "properties": {
                        "addressPrefix": "[parameters('ghostedSubnetPrefix')]",
                        "nextHopType": "VirtualAppliance",
                        "nextHopIPAddress": "[reference(parameters('HostNetworkInterface2Name')).ipconfigurations[0].properties.privateIPAddress]"
                      }
                    }
                  ]
                }
              },
              {
                "type": "Microsoft.Network/networkInterfaces",
                "apiVersion": "2019-04-01",
                "name": "[parameters('HostNetworkInterface1Name')]",
                "location": "[parameters('location')]",
                "properties": {
                  "ipConfigurations": [
                    {
                      "name": "ipconfig",
                      "properties": {
                        "primary": "true",
                        "privateIPAllocationMethod": "Static",
                        "privateIPAddress": "[reference(parameters('HostNetworkInterface1Name')).ipconfigurations[0].properties.privateIPAddress]",
                        "subnet": {
                          "id": "[resourceId('Microsoft.Network/virtualNetworks/subnets', parameters('virtualNetworkName'), parameters('NATSubnetName'))]"
                        },
                        "publicIPAddress": {
                          "id": "[resourceId('Microsoft.Network/publicIPAddresses', parameters('HostPublicIPAddressName'))]"
                        }
                      }
                    }
                  ]
                }
              },
              {
                "type": "Microsoft.Network/networkInterfaces",
                "apiVersion": "2019-04-01",
                "name": "[parameters('HostNetworkInterface2Name')]",
                "location": "[parameters('location')]",
                "properties": {
                  "ipConfigurations": [
                    {
                      "name": "ipconfig",
                      "properties": {
                        "primary": "true",
                        "privateIPAllocationMethod": "Static",
                        "privateIPAddress": "[reference(parameters('HostNetworkInterface2Name')).ipconfigurations[0].properties.privateIPAddress]",
                        "subnet": {
                          "id": "[resourceId('Microsoft.Network/virtualNetworks/subnets', parameters('virtualNetworkName'), parameters('hyperVSubnetName'))]"
                        }
                      }
                    }
                  ],
                  "enableIPForwarding": true
                }
              }
            ]
          }
        }
      },
*/