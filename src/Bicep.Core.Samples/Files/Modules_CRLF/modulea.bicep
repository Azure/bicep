param stringParamA string = 'test'
param stringParamB string
param objParam object
param arrayParam array

resource basicStorage 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'basicblobs'
  location: stringParamA
}

resource dnsZone 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'myZone'
  location: stringParamB
}

output stringOutputA string = stringParamA
output stringOutputB string = stringParamB
output objOutput object = basicStorage.properties
output arrayOutput array = [
  basicStorage.id
  dnsZone.id
]