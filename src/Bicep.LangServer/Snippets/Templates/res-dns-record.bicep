// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: ${1:'name'}
  location: ${2:'location'}
}

resource ${3:dnsRecord} 'Microsoft.Network/dnsZones/${4|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}@2018-05-01' = {
  parent: dnsZone
  name: ${5:'name'}
  properties: {
    TTL: ${6:TTL}
    '${7|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}': []
  }
}
