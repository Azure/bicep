// Create Image Gallery
resource ${1:sharedImageGallery} 'Microsoft.Compute/galleries@2020-09-30' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    description: ${3:'description'}
  }
}
