resource sharedImageGallery 'Microsoft.Compute/galleries@2020-09-30' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    description: 'description'
  }
}

