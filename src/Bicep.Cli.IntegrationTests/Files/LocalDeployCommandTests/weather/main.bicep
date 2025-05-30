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

resource forecastReq 'request@v1' = {
  uri: 'https://api.weather.gov/gridpoints/${gridpoints.gridId}/${gridpoints.gridX},${gridpoints.gridY}/forecast'
  format: 'raw'
}

var forecast = json(forecastReq.body).properties

type forecastType = {
  name: string
  temperature: int
}

var val = 'Name'

func getForecast() string => 'Forecast: ${val}'

output forecast forecastType[] = map(forecast.periods, p => {
  name: p.name
  temperature: p.temperature
})

output forecastString string = getForecast()
