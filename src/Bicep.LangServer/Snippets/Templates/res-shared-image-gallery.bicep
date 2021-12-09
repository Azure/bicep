// Create Image Gallery
resource /*${1:sharedImageGallery}*/sharedImageGallery 'Microsoft.Compute/galleries@2020-09-30' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    description: /*${4:'description'}*/'description'
  }
}
