param stringParamA string = 'test'
param stringParamB string
param objParam object
param arrayParam array

resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'basicblobs'
  location: stringParamA
}

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
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