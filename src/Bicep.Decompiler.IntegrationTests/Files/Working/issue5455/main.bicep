param name string
param location string
param defaultDataLakeStorageAccountName string
param defaultDataLakeStorageFilesystemName string
param sqlAdministratorLogin string

@secure()
param sqlAdministratorLoginPassword string = ''
param setWorkspaceIdentityRbacOnStorageAccount bool
param createManagedPrivateEndpoint bool
param defaultAdlsGen2AccountResourceId string = ''
param allowAllConnections bool = true

@allowed([
  'default'
  ''
])
param managedVirtualNetwork string
param tagValues object = {
}

@allowed([
  'Enabled'
  'Disabled'
])
param publicNetworkAccess string
param storageSubscriptionID string = subscription().subscriptionId
param storageResourceGroupName string = resourceGroup().name
param storageLocation string = resourceGroup().location
param storageRoleUniqueId string = newGuid()
param isNewStorageAccount bool = false
param isNewFileSystemOnly bool = false
param adlaResourceId string = ''
//@[06:020) [no-unused-params (Warning)] Parameter "adlaResourceId" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |adlaResourceId|
param managedResourceGroupName string = ''
param storageAccessTier string
param storageAccountType string
param storageSupportsHttpsTrafficOnly bool
param storageKind string
param minimumTlsVersion string
param storageIsHnsEnabled bool
param userObjectId string = ''
param setSbdcRbacOnStorageAccount bool = false
param setWorkspaceMsiByPassOnStorageAccount bool = false
param workspaceStorageAccountProperties object = {
}
param managedVirtualNetworkSettings object

var storageBlobDataContributorRoleID = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
var defaultDataLakeStorageAccountUrl = 'https://${defaultDataLakeStorageAccountName}.dfs.core.windows.net'
//@[89:105) [no-hardcoded-env-urls (Warning)] Environment URLs should not be hardcoded. Use the environment() function to ensure compatibility across clouds. Found this disallowed host: "core.windows.net" (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-env-urls)) |core.windows.net|

resource name_resource 'Microsoft.Synapse/workspaces@2021-06-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    defaultDataLakeStorage: {
      accountUrl: defaultDataLakeStorageAccountUrl
      filesystem: defaultDataLakeStorageFilesystemName
      resourceId: defaultAdlsGen2AccountResourceId
      createManagedPrivateEndpoint: createManagedPrivateEndpoint
    }
    managedVirtualNetwork: managedVirtualNetwork
    managedResourceGroupName: managedResourceGroupName
    publicNetworkAccess: publicNetworkAccess
    managedVirtualNetworkSettings: managedVirtualNetworkSettings
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorLoginPassword: sqlAdministratorLoginPassword
  }
  tags: tagValues
  dependsOn: [
    defaultDataLakeStorageAccount
    defaultDataLakeStorageFilesystem
  ]
}

resource name_allowAll 'Microsoft.Synapse/workspaces/firewallrules@2021-06-01' = if (allowAllConnections) {
  parent: name_resource
  location: location
//@[02:010) [BCP187 (Warning)] The property "location" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |location|
  name: 'allowAll'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

module StorageRoleDeploymentResource './nested_StorageRoleDeploymentResource.bicep' = if (setWorkspaceIdentityRbacOnStorageAccount) {
//@[37:083) [BCP104 (Error)] The referenced module has errors. (CodeDescription: none) |'./nested_StorageRoleDeploymentResource.bicep'|
  name: 'StorageRoleDeploymentResource'
  scope: resourceGroup(storageSubscriptionID, storageResourceGroupName)
  params: {
    reference_concat_Microsoft_Synapse_workspaces_parameters_name_2021_06_01_Full_identity_principalId: reference('Microsoft.Synapse/workspaces/${name}', '2021-06-01', 'Full')
    resourceId_Microsoft_Authorization_roleDefinitions_variables_storageBlobDataContributorRoleID: resourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataContributorRoleID)
    variables_storageBlobDataContributorRoleID: storageBlobDataContributorRoleID
    defaultDataLakeStorageAccountName: defaultDataLakeStorageAccountName
    name: name
    storageRoleUniqueId: storageRoleUniqueId
    storageLocation: storageLocation
    setSbdcRbacOnStorageAccount: setSbdcRbacOnStorageAccount
    userObjectId: userObjectId
  }
  dependsOn: [
    name_resource
  ]
}

module UpdateStorageAccountNetworkingAcls './nested_UpdateStorageAccountNetworkingAcls.bicep' = if (setWorkspaceMsiByPassOnStorageAccount) {
  name: 'UpdateStorageAccountNetworkingAcls'
  scope: resourceGroup(storageSubscriptionID, storageResourceGroupName)
  params: {
    storageLocation: storageLocation
    defaultDataLakeStorageAccountName: defaultDataLakeStorageAccountName
    workspaceStorageAccountProperties: workspaceStorageAccountProperties
  }
  dependsOn: [
    name_resource
  ]
}

resource defaultDataLakeStorageAccount 'Microsoft.Storage/storageAccounts@2021-01-01' = if (isNewStorageAccount) {
  name: defaultDataLakeStorageAccountName
  location: storageLocation
  properties: {
    accessTier: storageAccessTier
    supportsHttpsTrafficOnly: storageSupportsHttpsTrafficOnly
    isHnsEnabled: storageIsHnsEnabled
    minimumTlsVersion: minimumTlsVersion
  }
  sku: {
    name: storageAccountType
  }
  kind: storageKind
  tags: {
  }
}

resource defaultDataLakeStorageAccountName_default_defaultDataLakeStorageFilesystem 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-01-01' = if (isNewStorageAccount) {
  name: '${defaultDataLakeStorageAccountName}/default/${defaultDataLakeStorageFilesystemName}'
  properties: {
    publicAccess: 'None'
  }
  dependsOn: [
    defaultDataLakeStorageAccount
  ]
}

module defaultDataLakeStorageFilesystem './nested_defaultDataLakeStorageFilesystem.bicep' = if (isNewFileSystemOnly) {
  name: defaultDataLakeStorageFilesystemName
  scope: resourceGroup(storageSubscriptionID, storageResourceGroupName)
  params: {
    defaultDataLakeStorageAccountName: defaultDataLakeStorageAccountName
    defaultDataLakeStorageFilesystemName: defaultDataLakeStorageFilesystemName
  }
}
