module /*${1:privateDnsZone}*/privateDnsZone /*${2:'br/public:avm/res/network/private-dns-zone:VERSION'}*/'br/public:avm/res/network/private-dns-zone:0.2.5' = {
  scope: /*${3:deploymentScope}*/resourceGroup()
  name: /*${4:'deploymentName'}*/'deploymentName'
  params: {
    name: /*${5:'name'}*/'name'
    location: /*${6:'global'}*/'global'
  }
}
