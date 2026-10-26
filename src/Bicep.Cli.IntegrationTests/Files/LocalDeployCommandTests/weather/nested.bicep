targetScope = 'local'

param coords {
  latitude: string
  longitude: string
}

module main 'main.bicep' = {
  params: {
    coords: coords
  }
}
