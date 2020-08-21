param name string = 'bicepKeyVaultTutorial'
param location string = 'westuS'
param sku string = 'Standard'
param tenant string = '72f988bf-86f1-41af-91ab-2d7cd011db47'
param accessPolicies array = [
    {
        tenantId: tenant
        objectId: '414d10da-615f-49a7-90a0-a7008fb31cd3'
        permissions: {
            keys: [ 
                'Get'
                'List'
                'Update'
                'Create'
                'Import'
                'Delete'
                'Recover'
                'Backup'
                'Restore'
            ]
            secrets: [
                'Get'
                'List'
                'Set'
                'Delete'
                'Recover'
                'Backup'
                'Restore'
                ]
            certificates: [
                'Get'
                'List'
                'Update'
                'Create'
                'Import'
                'Delete'
                'Recover'
                'Backup'
                'Restore'
                'ManageContacts'
                'ManageIssuers'
                'GetIssuers'
                'ListIssuers'
                'SetIssuers'
                'DeleteIssuers'
            ]
        }
    }
]

param enabledForDeployment bool = true
param enabledForTemplateDeployment bool = true
param enabledForDiskEncryption bool = true 
param enableRbacAuthorization bool = false
param enableSoftDelete bool = true 
param softDeleteRetentionInDays int = 90

param networkAcls object = {
    bypass: 'AzureServices'
    defaultAction: 'allow'
    ipRules: [
    ]
    virtualNetworkRules: [
    ]
}

resource keyvault 'Microsoft.KeyVault/vaults@2019-09-01' = {    
    name: name       
    location: location 
    tags: {

    }
    properties:{
        tenantId: tenant
        sku: {
            family: 'A'
            name: sku
        }
        accessPolicies: accessPolicies
        enabledForDeployment: enabledForDeployment
        enabledForDiskEncryption: enabledForDiskEncryption
        enabledForTemplateDeployment: enabledForTemplateDeployment
        enableSoftDelete: enableSoftDelete
        softDeleteRetentionInDays: softDeleteRetentionInDays
        enableRbacAuthorization: enableRbacAuthorization
        networkAcls: networkAcls
    }
}