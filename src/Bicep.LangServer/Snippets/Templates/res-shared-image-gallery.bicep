// Create Image Gallery
resource /*${1:sharedImageGallery}*/sharedImageGallery 'Microsoft.Compute/galleries@2020-09-30' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    description: /*${3:'description'}*/'description'
  }
}
