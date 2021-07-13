// Internal Load Balancer
resource /*${1:loadBalancerInternal}*/loadBalancerInternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          privateIPAddress: /*${4:'0.0.0.0'}*/'0.0.0.0'
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', /*${5:'virtualNetwork'}*/'virtualNetwork', /*${6:'subnet'}*/'subnet')
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${7:'name'}*/'name'
      }
    ]
    loadBalancingRules: [
      {
        name: /*${8:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', /*${2:'name'}*/'name', /*${3:'name'}*/'name')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', /*${2:'name'}*/'name', /*${7:'name'}*/'name')
          }
          protocol: /*'${9|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${10:80}*/80
          backendPort: /*${11:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', /*${2:'name'}*/'name', /*${12:'name'}*/'name')
          }
        }
      }
    ]
    probes: [
      {
        name: /*${12:'name'}*/'name'
        properties: {
          protocol: /*'${13|Tcp,Udp,All|}'*/'Tcp'
          port: /*${14:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
