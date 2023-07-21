param azureSubscriptionID string
param sigName string
param sigLocation string
param imagePublisher string
param imageDefinitionName string
param imageOffer string
param imageSKU string
param imageLocation string
param roleNameGalleryImage string
param principalId string
param templateImageResourceGroup string

var templateImageResourceGroupId = '/subscriptions/${azureSubscriptionID}/resourcegroups/${templateImageResourceGroup}'

//Create Shard Image Gallery
resource wvdsig 'Microsoft.Compute/galleries@2020-09-30' = {
  name: sigName
  location: sigLocation
}

//Create Image definition
resource wvdid 'Microsoft.Compute/galleries/images@2020-09-30' = {
  parent: wvdsig
  name: imageDefinitionName
  location: imageLocation
  properties: {
    osState: 'Generalized'
    osType: 'Windows'
    identifier: {
      publisher: imagePublisher
      offer: imageOffer
      sku: imageSKU
    }
  }
}

//create role definition
resource gallerydef 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' = {
  name: guid(roleNameGalleryImage)
  properties: {
    roleName: roleNameGalleryImage
    description: 'Custom role for network read'
    permissions: [
      {
        actions: [
          'Microsoft.Compute/galleries/read'
          'Microsoft.Compute/galleries/images/read'
          'Microsoft.Compute/galleries/images/versions/read'
          'Microsoft.Compute/galleries/images/versions/write'
          'Microsoft.Compute/images/write'
          'Microsoft.Compute/images/read'
          'Microsoft.Compute/images/delete'
        ]
      }
    ]
    assignableScopes: [
      templateImageResourceGroupId
    ]
  }
}

//create role assignment
resource galleryass 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(resourceGroup().id, gallerydef.id, principalId)
  properties: {
    roleDefinitionId: gallerydef.id
    principalId: principalId
  }
}
