// Internal Load Balancer
resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:'subnet'}*/subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
  parent: vnet
  name: /*${3:'name'}*/'name'
}

resource /*${4:backendAddressPool}*/backendAddressPool 'Microsoft.Network/loadBalancers/backendAddressPools@2021-02-01' existing = {
  name: /*${5:'loadBalancerExternal/loadBalancerBackEndPool'}*/'loadBalancerExternal/loadBalancerBackEndPool'
}

resource /*${6:loadBalancerInternal}*/loadBalancerInternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${7:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${8:'name'}*/'name'
        properties: {
          privateIPAddress: /*${9:'0.0.0.0'}*/'0.0.0.0'
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: subnet.id
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${10:'name'}*/'name'
      }
    ]
    loadBalancingRules: [
      {
        name: /*${11:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: 'id'
          }
          backendAddressPool: {
            id: backendAddressPool.id
          }
          protocol: /*'${12|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${13:80}*/80
          backendPort: /*${14:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: 'id'
          }
        }
      }
    ]
    probes: [
      {
        name: /*${15:'name'}*/'name'
        properties: {
          protocol: /*'${16|Tcp,Udp,All|}'*/'Tcp'
          port: /*${17:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
