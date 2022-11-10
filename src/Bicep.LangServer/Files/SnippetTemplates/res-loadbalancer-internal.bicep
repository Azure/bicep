// Internal Load Balancer
resource /*${1:loadBalancerInternal}*/loadBalancerInternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${4:'name'}*/'name'
        properties: {
          privateIPAddress: /*${5:'0.0.0.0'}*/'0.0.0.0'
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: /*${6:'subnet.id'}*/'subnet.id'
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
            id: /*${9:'frontendIPConfiguration.id'}*/'frontendIPConfiguration.id'
          }
          backendAddressPool: {
            id: /*${10:'backendAddressPool.id'}*/'backendAddressPool.id'
          }
          protocol: /*'${11|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${12:80}*/80
          backendPort: /*${13:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: /*${14:'probe.id'}*/'probe.id'
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
