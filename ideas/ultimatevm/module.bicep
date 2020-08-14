module virtualMachine {
  input string adminUsername
  input securestring adminPasswordOrKey
  input string location: resourceGroup().location // TODO defaults
  input string vmName: 'linux-vm'
  input string authenticationType: 'password'
  input string vmSize: 'Standard_A2_v2'
  input string storageNewOrExisting: 'new'
  input string storageAccountName: 'storage${uniqueString(vmName, resourceGroup().id)}' // TODO string interpolation and colorization for it
  input string storageAccountType: 'Standard_LRS'
  input string storageAccountResourceGroupName: resourceGroup().name
  input string subnetId
  input string publicIpNewOrExisting: 'new'
  input string publicIpName: '${vmName}-ip'
  input string publicIpDns: '${vmName}${uniqueString(resourceGroup().id)}'
  input string publicIpResourceGroupName: resourceGroup().name
  input string publicIpAllocationMethod: 'Static'
  input string publicIpSku: 'Standard'
  input string operatingSystem: 'UbuntuServer_18.04-LTS'
  input array dataDiskSizes: [128]
  input string zones: ''
  input string availabilitySetName: ''
  input string managedIdentity: 'None'
  input string userAssignedIdentity: ''
  input array managedIdentityRoleDefinitionIds: [{
    scope: resourceGroup().id
    role: 'b24988ac-6180-42a0-ab88-20f7382dd24c' //contributor
  }]
  input bool validateManagedIdentity: (managedIdentity == 'UserAssigned' && userAssignedIdentity == '') ? false : true // TODO ternary operator
  input bool validateAvailabilitySet: (zones != '' && availabilitySetName != '') ? false : true
  input bool validateZones: (zones != '' && toLower(publicIpSku) != 'standard') // TODO case insensitive comparison operator? e.g. '=~'

  variable isWindowsOS: contains(toLower(imageReference[operatingSystem].offer), 'windows') // TODO should we use : syntax for variable assignment for consistency?
  variable imageReference: {
    'UbuntuServer_16.04-LTS': {
      publisher: 'Canonical'
      offer: 'UbuntuServer'
      sku: '16.04-LTS'
      version: 'latest'
    }
    'UbuntuServer_18.04-LTS': { // TODO string-valued property names (to handle special chars)
      publisher: 'Canonical'
      offer: 'UbuntuServer'
      sku: '18.04-LTS'
      version: 'latest'
    }
    'WindowsServer_2016-DataCenter': {
      publisher: 'MicrosoftWindowsServer'
      offer: 'WindowsServer'
      sku: '2016-Datacenter'
      version: 'latest'
    }
  }
  variable windowsConfiguration: {
    provisionVmAgent: true
  }
  variable enableBootDiags: storageNewOrExisting != 'none'
  variable storageAccountType: resourceId(storageAccountResourceGroupName, 'Microsoft.Storage/storageAccounts', storageAccountName) // TODO see if we can replace this with a resource ref
  variable nicName: '${vmName}-nic'
  variable linuxConfiguration: {
    disablePasswordAuthentication: true
    ssh: {
      publicKeys: [
        {
          path: '/home/${adminUsername}/.ssh/authorized_keys'
          keyData: adminPasswordOrKey
        }
      ]
    }
  }
  variable availabilitySet: resourceId('Microsoft.Compute/availabilitySets', availabilitySetName) // TODO see if we can replace this with a resource ref
  variable userAssignedIdentities: {
    '${userAssignedIdentity}': {} // TODO string interpolation in object key
  }

  if (availabilitySetName != '') {
    resource azrm 'compute/availabilitySets@2019-07-01' availabilitySet {
      name: availabilitySetName
      location: location
      sku: {
        name: 'Aligned'
      }
      properties: {
        PlatformUpdateDomainCount: 2
        PlatformFaultDomainCount: 2
      }
    }
  }

  if (storageNewOrExisting == 'new') {
    resource azrm 'storage/storageAccounts@2019-06-01' storageAccount {
      name: storageAccountName
      location: location
      kind: 'Storage'
      sku: {
        name: storageAccountType
      }
    }
  }

  if (publicIpNewOrExisting == 'new') {
    resource azrm 'network/publicIPAddresses@2019-06-01' publicIp {
      name: publicIpName
      location: location
      kind: 'Storage'
      sku: {
        name: publicIpSku
      }
      properties: {
        publicIPAllocationMethod: publicIPAllocationMethod
        dnsSettings: {
          domainNameLabel: publicIpDns
        }
      }
    }
  }

  resource azrm 'network/networkInterfaces@2019-09-01' nic {
    name: nicName
    location: location
    properties: {
      ipConfigurations: [{
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: subnetId
          }
          publicIPAddress: (publicIpNewOrExisting != 'none') ? publicIp : null // TODO need a mechanism to pass in a publicIp resource by ref
        }
      }]
    }
  }

  resource azrm 'compute/virtualMachines@2019-07-01' vm {
    name: vmName
    location: location
    identity: {
      type: managedIdentity
      userAssignedIdentities: (managedIdentity == 'UserAssigned') ? userAssignedIdentities : null
    }
    zones: (zones == '') ? null : [zones] // TODO not sure if this is the actual intention of the original template
    properties: {
      availabilitySet: availabilitySet // TODO can we reference this variable that is declared inside an 'if' scope - and treat as null if missing?
      hardwareProfile: {
        vmSize: vmSize
      }
      osProfile: {
        computerName: vmName
        adminUsername: adminUsername
        adminPassword: adminPasswordOrKey
        linuxConfiguration: isWindowsOS ? null : linuxConfiguration
        windowsConfiguration: isWindowsOS ? windowsConfiguration : null
      }
      storageProfile: {
        imageReference[operatingSystem] // TODO dynamic property access
        osDisk: {
          caching: 'ReadWrite'
          createOption: 'FromImage'
        }
        dataDisks: (map i in range(0, dataDiskSizes) {
          diskSizeGB: dataDiskSizes[i]
          lun: i
          createOption: 'Empty'
        }) // TODO this syntax needs some work! (kind of analagous to a ternary condition vs if condition on a for loop)
      }
      networkProfile: {
        networkInterfaces: [
          nic.id()
        ]
      }
      diagnosticsProfile: {
        bootDiagnostics: {
          enabled: enableBootDiags
          storageUri: enableBootDiags ? reference(storageAccountId, '2019-06-01').primaryEndpoints.blob : null
        }
      }
    }
  }

  if (managedIdentity == 'SystemAssigned') {
    for identity, i in managedIdentityRoleDefinitionIds { // TODO index syntax on for-loop
      resource azrm 'authorization/roleAssignments@2019-04-01-preview' roleAssignment {
        extends: vm // TODO extension resource syntax?
        name: guid(vm.id(), i)
        location: location
        properties: {
          principalId: vm.identity.principalId
          roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', identity.role)
          scope: identity.scope
          principalType: 'ServicePrincipal'
        }
      }
    }
  }

  output sshCommand: publicIpNewOrExisting == 'none' ? 
    'no public ip' :
    publicIp.properties.dnsSettings.fqdn
  output debug: {
    validateAvailabilitySet: validateAvailabilitySet
    validateZones: validateZones
    validateManagedIdentity: validateManagedIdentity
  }
}

module customScript {
  input string location
  input string extensionNamePrefix: 'cse'
  input resource virtualMachine // TODO input resource reference?
  input array fileUris
  input string commandToExecute
  input bool isWindowsOS

  variable extensionNameWindows: '${extensionNamePrefix}-windows'
  variable extensionNameLinux: '${extensionNamePrefix}-linux'

  if (isWindowsOS) {
    resource 'compute/virtualMachines/extensions@2019-03-01' cse {
      parent: virtualMachine // TODO mechanism for deploying child resource?
      name: extensionNameWindows
      location: location
      properties: {
        publisher: 'Microsoft.Compute'
        type: 'CustomScriptExtension'
        typeHandlerVersion: '1.8'
        autoUpgradeMinorVersion: true
        settings: {
          fileUris: fileUris
        }
        protectedSettings: {
          commandToExecute: commandToExecute
        }
      }
    }

    output instanceView: cse.properties.instanceView // TODO conditional outputs?
  } else {
    resource 'compute/virtualMachines/extensions@2019-03-01' cse {
      parent: virtualMachine // TODO mechanism for deploying child resource?
      name: extensionNameLinux
      location: location
      properties: {
        publisher: 'Microsoft.Azure.Extensions'
        type: 'CustomScript'
        typeHandlerVersion: '2.0'
        autoUpgradeMinorVersion: true
        settings: {
          fileUris: fileUris
        }
        protectedSettings: {
          commandToExecute: commandToExecute
        }
      }
    }
  }

  output instanceView: cse.properties.instanceView
}