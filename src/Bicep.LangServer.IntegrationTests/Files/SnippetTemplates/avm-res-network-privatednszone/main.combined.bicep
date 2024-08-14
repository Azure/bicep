// $1 = privateDnsZone
// $2 = 'br/public:avm/res/network/private-dns-zone:0.2.5'
// $3 = 'deploymentName'
// $4 = 'name'
// $5 = location

param location string = 'global'

module privateDnsZone 'br/public:avm/res/network/private-dns-zone:0.2.5' = {
  name: 'deploymentName'
  params: {
    name: 'name'
    location: location
  }
}


