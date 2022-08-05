// $1 = ttl
// $2 = int
// $3 = 1

param ttl int = 1// Insert snippet here

resource dnsRecord 'Microsoft.Network/dnsZones/A@2018-05-01' = {
  name: 'zone/A'
  properties: {
    TTL: ttl
  }
}

