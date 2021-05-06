resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'location'
}

resource dnsRecord 'Microsoft.Network/dnsZones/A@2018-05-01' = {
  parent: dnsZone
  name: 'name'
  properties: {
    TTL: 3600
    'ARecords': []
  }
}

