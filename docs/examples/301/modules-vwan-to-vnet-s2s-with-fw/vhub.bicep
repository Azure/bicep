param location string {
  default: resourceGroup().location
  metadata: {
    description: 'Specifies the Azure location where the resource should be created.'
  }
}
param hubname string {
  metadata: {
    description: 'Specifies the name to use for the Virtual Hub resources.'
  }
}
param hubaddressprefix string {
  default: '10.10.0.0/24'
  metadata: {
    description: 'Specifies the Virtual Hub Address Prefix.'
  }
}
param wanid string {
  metadata: {
    description: 'Virtual WAN ID'
  }
}

resource hub 'Microsoft.Network/virtualHubs@2020-06-01' = {
  name: hubname
  location: location
  properties: {
    addressPrefix: hubaddressprefix
    virtualWan: {
      id: wanid
    }
  }
}

output id string = hub.id
output name string = hub.name
