// $1 = sharedImageGallery
// $2 = 'name'
// $3 = location
// $4 = 'description'

param location string

resource sharedImageGallery 'Microsoft.Compute/galleries@2020-09-30' = {
  name: 'name'
  location: location
  properties: {
    description: 'description'
  }
}
// Insert snippet here

