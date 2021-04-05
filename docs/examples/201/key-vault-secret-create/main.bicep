// 
// v0.3 Bicep Known issue & work-around related to "secretsObject"
// not possible to use this file (as is) on the `az cli` passing in secretsObject
// https://github.com/Azure/bicep/issues/2135
// https://github.com/Azure/bicep/issues/1754

// Also: 
// * KeyVaults secrets CANNOT BE EXPORTED into AMR/Bicep format. 
// * documentation: 
//   https://docs.microsoft.com/en-us/azure/templates/microsoft.keyvault/vaults/secrets?tabs=json
// * 201/key-vault-secret=create in pre-decompiled AMR format
//   https://github.com/Azure/azure-quickstart-templates/tree/master/201-key-vault-secret-create

@description('Specifies the name of the key vault.')
param keyVaultName string

@description('Specifies the Azure location where the key vault should be created.')
param location string = resourceGroup().location

@description('Specifies whether Azure Virtual Machines are permitted to retrieve certificates stored as secrets from the key vault.')
param enabledForDeployment bool = false

@description('Specifies whether Azure Disk Encryption is permitted to retrieve secrets from the vault and unwrap keys.')
param enabledForDiskEncryption bool = false

@description('Specifies whether Azure Resource Manager is permitted to retrieve secrets from the key vault.')
param enabledForTemplateDeployment bool = false

@description('Specifies the Azure Active Directory tenant ID that should be used for authenticating requests to the key vault. Get it by using Get-AzSubscription cmdlet.')
param tenantId string = subscription().tenantId

@description('Specifies the object ID of a user, service principal or security group in the Azure Active Directory tenant for the vault. The object ID must be unique for the list of access policies. Get it by using Get-AzADUser or Get-AzADServicePrincipal cmdlets.')
param objectId string

@description('Specifies the permissions to keys in the vault. Valid values are: all, encrypt, decrypt, wrapKey, unwrapKey, sign, verify, get, list, create, update, import, delete, backup, restore, recover, and purge.')
param keysPermissions array = [
  'list'
]

@description('Specifies the permissions to secrets in the vault. Valid values are: all, get, list, set, delete, backup, restore, recover, and purge.')
param secretsPermissions array = [
  'list'
]

@allowed([
  'standard'
  'premium'
])
@description('Specifies whether the key vault is a standard vault or a premium vault.')
param skuName string = 'standard'

@secure()
@description('Specifies all secrets {"secretName":"","secretValue":""} wrapped in a secure object.')
param secretsObject object = {
  secrets: [
  // either edit this section with your secrets, or add them as parameters you can define. 
  //  {
  //    secretName: 'yourSecret'
  //    secretValue: 'yourValue'
  //  }
  ]
}

resource vault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultName
  location: location
  tags: {
    displayName: 'KeyVault'
  }
  properties: {
    enabledForDeployment: enabledForDeployment
    enabledForTemplateDeployment: enabledForTemplateDeployment
    enabledForDiskEncryption: enabledForDiskEncryption
    tenantId: tenantId
    accessPolicies: [
      {
        objectId: objectId
        tenantId: tenantId
        permissions: {
          keys: keysPermissions
          secrets: secretsPermissions
        }
      }
    ]
    sku: {
      name: skuName
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

resource secrets 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = [for secret in secretsObject.secrets: {
  name: '${vault.name}/${secret.secretName}'
  properties: {
    value: secret.secretValue
  }
}]
