// DNS Zone
resource ${1:dnsZone} 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: ${2:'name'}
  location: 'global'
}
