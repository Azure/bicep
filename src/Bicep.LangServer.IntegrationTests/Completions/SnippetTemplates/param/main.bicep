// $1 = ttl
// $2 = int

// Insert snippet here

resource dnsRecord 'Microsoft.Network/dnsZones/A@2018-05-01' = {
  name: 'zone/A'
  properties: {
    TTL: ttl
  }
}
