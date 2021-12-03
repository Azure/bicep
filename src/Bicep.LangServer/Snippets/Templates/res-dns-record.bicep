// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: /*${1:'name'}*/'name'
  location: location
}

resource /*${2:dnsRecord}*/dnsRecord /*'Microsoft.Network/dnsZones/${3|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}@2018-05-01'*/'Microsoft.Network/dnsZones/A@2018-05-01' = {
  parent: dnsZone
  name: /*${4:'name'}*/'name'
  properties: {
    TTL: 3600
    /*'${5|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}'*/'ARecords': []
  }
}
