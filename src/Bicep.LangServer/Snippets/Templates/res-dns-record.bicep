// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
}

resource /*${3:dnsRecord}*/dnsRecord /*'Microsoft.Network/dnsZones/${4|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}@2018-05-01'*/'Microsoft.Network/dnsZones/A@2018-05-01' = {
  parent: dnsZone
  name: /*${5:'name'}*/'name'
  properties: {
    TTL: 3600
    /*'${6|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}'*/'ARecords': []
  }
}
