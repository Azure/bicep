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

resource forecastReq 'request@v1' = {
  uri: 'https://api.weather.gov/gridpoints/${gridpoints.gridId}/${gridpoints.gridX},${gridpoints.gridY}/forecast'
  format: 'raw'
}
