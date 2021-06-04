// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
}

resource ${2:dnsRecord} 'Microsoft.Network/dnsZones/${3|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}@2018-05-01' = {
  parent: dnsZone
  name: ${4:'name'}
  properties: {
    TTL: 3600
    '${5|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}': []
  }
}
