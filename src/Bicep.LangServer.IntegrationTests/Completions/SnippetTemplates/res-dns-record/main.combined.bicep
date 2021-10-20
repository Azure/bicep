// $1 = 'name'
// $2 = dnsRecord
// $3 = A
// $4 = 'name'
// $5 = ARecords

param location string

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: location
}

resource dnsRecord 'Microsoft.Network/dnsZones/A@2018-05-01' = {
  parent: dnsZone
  name: 'name'
  properties: {
    TTL: 3600
    'ARecords': []
  }
}
// Insert snippet here

