// $0 = 1
// $1 = ttl

var ttl = 1

resource dnsRecord 'Microsoft.Network/dnsZones/A@2018-05-01' = {
  name: 'zone/A'
  properties: {
    TTL: ttl
  }
}

