resource sharedImageGallery 'Microsoft.Compute/galleries@2020-09-30' = {
  name: 'sharedImageGallery'
  location: resourceGroup().location
  properties: {
    description: 'sharedImageGalleryDescription'
  }
}
