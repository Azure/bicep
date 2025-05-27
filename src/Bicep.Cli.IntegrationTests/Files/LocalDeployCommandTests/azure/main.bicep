targetScope = 'local'

extension http

param coords {
  latitude: string
  longitude: string
}

resource gridpointsReq 'request@v1' = {
  uri: 'https://api.weather.gov/points/${coords.latitude},${coords.longitude}'
  format: 'raw'
}

var gridpoints = json(gridpointsReq.body).properties

module gridCoords 'module.bicep' = {
  name: 'gridCoords'
  scope: resourceGroup('f33917e6-545f-452a-af87-e976c97e056d', 'mockRg')
  params: {
    gridpoints: gridpoints
  }
}

output gridId string = gridCoords.outputs.gridId
