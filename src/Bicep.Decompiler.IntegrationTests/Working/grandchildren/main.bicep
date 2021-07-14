param storageAccounts_ll21w7onmmpv65q24_name string = 'll21w7onmmpv65q24'
param storageAccounts_ll22w7onmmpv65q24_name string = 'll22w7onmmpv65q24'

resource storageAccounts_ll21w7onmmpv65q24_name_resource 'Microsoft.Storage/storageAccounts@2021-01-01' = {
  name: storageAccounts_ll21w7onmmpv65q24_name
  location: 'westeurope'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
//@[4:8) [BCP073 (Warning)] The property "tier" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |tier|
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}

resource storageAccounts_ll22w7onmmpv65q24_name_resource 'Microsoft.Storage/storageAccounts@2021-01-01' = {
  name: storageAccounts_ll22w7onmmpv65q24_name
  location: 'westeurope'
  sku: {
    name: 'Standard_LRS'
    tier: 'Standard'
//@[4:8) [BCP073 (Warning)] The property "tier" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |tier|
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      bypass: 'AzureServices'
      virtualNetworkRules: []
      ipRules: []
      defaultAction: 'Allow'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        file: {
          keyType: 'Account'
          enabled: true
        }
        blob: {
          keyType: 'Account'
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}

resource storageAccounts_ll21w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/blobServices@2021-01-01' = {
  parent: storageAccounts_ll21w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

resource storageAccounts_ll22w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/blobServices@2021-01-01' = {
  parent: storageAccounts_ll22w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

resource Microsoft_Storage_storageAccounts_fileServices_storageAccounts_ll21w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/fileServices@2021-01-01' = {
  parent: storageAccounts_ll21w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    protocolSettings: {
      smb: {}
    }
    cors: {
      corsRules: []
    }
    shareDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

resource Microsoft_Storage_storageAccounts_fileServices_storageAccounts_ll22w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/fileServices@2021-01-01' = {
  parent: storageAccounts_ll22w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    protocolSettings: {
      smb: {}
    }
    cors: {
      corsRules: []
    }
    shareDeleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

resource Microsoft_Storage_storageAccounts_queueServices_storageAccounts_ll21w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/queueServices@2021-01-01' = {
  parent: storageAccounts_ll21w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource Microsoft_Storage_storageAccounts_queueServices_storageAccounts_ll22w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/queueServices@2021-01-01' = {
  parent: storageAccounts_ll22w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource Microsoft_Storage_storageAccounts_tableServices_storageAccounts_ll21w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/tableServices@2021-01-01' = {
  parent: storageAccounts_ll21w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource Microsoft_Storage_storageAccounts_tableServices_storageAccounts_ll22w7onmmpv65q24_name_default 'Microsoft.Storage/storageAccounts/tableServices@2021-01-01' = {
  parent: storageAccounts_ll22w7onmmpv65q24_name_resource
  name: 'default'
  properties: {
    cors: {
      corsRules: []
    }
  }
}

resource storageAccounts_ll21w7onmmpv65q24_name_default_blobs11 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-01-01' = {
  parent: storageAccounts_ll21w7onmmpv65q24_name_default
  name: 'blobs11'
  properties: {
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_ll21w7onmmpv65q24_name_resource
  ]
}

resource storageAccounts_ll21w7onmmpv65q24_name_default_blobs12 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-01-01' = {
  parent: storageAccounts_ll21w7onmmpv65q24_name_default
  name: 'blobs12'
  properties: {
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_ll21w7onmmpv65q24_name_resource
  ]
}

resource storageAccounts_ll22w7onmmpv65q24_name_default_blobs21 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-01-01' = {
  parent: storageAccounts_ll22w7onmmpv65q24_name_default
  name: 'blobs21'
  properties: {
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_ll22w7onmmpv65q24_name_resource
  ]
}

resource storageAccounts_ll22w7onmmpv65q24_name_default_blobs22 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-01-01' = {
  parent: storageAccounts_ll22w7onmmpv65q24_name_default
  name: 'blobs22'
  properties: {
    defaultEncryptionScope: '$account-encryption-key'
    denyEncryptionScopeOverride: false
    publicAccess: 'None'
  }
  dependsOn: [
    storageAccounts_ll22w7onmmpv65q24_name_resource
  ]
}
