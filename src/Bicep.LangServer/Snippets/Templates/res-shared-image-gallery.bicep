// Create Image Gallery
resource sharedImageGallery 'Microsoft.Compute/galleries@2020-09-30' = {
  name: ${1:name}
  location: resourceGroup().location
  properties: {
    description: ${2:description}
  }
}
