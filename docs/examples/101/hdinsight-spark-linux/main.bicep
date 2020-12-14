param clusterName string
param clusterLoginUserName string
param clusterLoginPassword string {
  minLength: 10
  secure: true
}
param sshUserName string
param sshPassword string {
  minLength: 6
  maxLength: 72
  secure: true
}
param location string = resourceGroup().location
param headNodeVirtualMachineSize string {
  allowed: [
    'Standard_A4_v2'
    'Standard_A8_v2'
    'Standard_D3_v2'
    'Standard_D4_v2'
    'Standard_D5_v2'
    'Standard_D12_v2'
    'Standard_D13_v2'
  ]
  default: 'Standard_D3_v2'
}
param workerNodeVirtualMachineSize string {
  allowed: [
    'Standard_A4_v2'
    'Standard_A8_v2'
    'Standard_D1_v2'
    'Standard_D2_v2'
    'Standard_D3_v2'
    'Standard_D4_v2'
    'Standard_D5_v2'
    'Standard_D12_v2'
    'Standard_D13_v2'
  ]
  default: 'Standard_D3_v2'
}
resource defaultStorageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'storage${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource cluster 'Microsoft.HDInsight/clusters@2018-06-01-preview' = {
  name: clusterName
  location: location
  properties: {
    clusterVersion: '3.6'
    osType: 'Linux'
    tier: 'Standard'
    clusterDefinition: {
      kind: 'spark'
      configurations: {
        gateway: {
          'restAuthCredential.isEnabled': true
          'restAuthCredential.username': clusterLoginUserName
          'restAuthCredential.password': clusterLoginPassword
        }
      }
    }
    storageProfile: {
      storageaccounts: [
        {
          name: replace(replace(defaultStorageAccount.properties.primaryEndpoints.blob, 'https://', ''), '/', '')
          isDefault: true
          container: clusterName
          key: listKeys(defaultStorageAccount.id, '2019-06-01').keys[0].value
        }
      ]
    }
    computeProfile: {
      roles: [
        {
          name: 'headnode'
          targetInstanceCount: 2
          hardwareProfile: {
            vmSize: headNodeVirtualMachineSize
          }
          osProfile: {
            linuxOperatingSystemProfile: {
              username: sshUserName
              password: sshPassword
            }
          }
        }
        {
          name: 'workernode'
          targetInstanceCount: 2
          hardwareProfile: {
            vmSize: workerNodeVirtualMachineSize
          }
          osProfile: {
            linuxOperatingSystemProfile: {
              username: sshUserName
              password: sshPassword
            }
          }
        }
      ]
    }
  }
}

output storageOutput object = defaultStorageAccount.properties
output clusterOutput object = cluster.properties
