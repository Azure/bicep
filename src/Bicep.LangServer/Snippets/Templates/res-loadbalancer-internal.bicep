// Internal Load Balancer
resource /*${1:loadBalancerInternal}*/loadBalancerInternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          privateIPAddress: /*${4:'0.0.0.0'}*/'0.0.0.0'
          privateIPAllocationMethod: 'Static'
          subnet: {
            id: /*${5:'subnet.id'}*/'subnet.id'
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${6:'name'}*/'name'
      }
    ]
    loadBalancingRules: [
      {
        name: /*${7:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: /*${8:'frontendIPConfiguration.id'}*/'frontendIPConfiguration.id'
          }
          backendAddressPool: {
            id: /*${9:'backendAddressPool.id'}*/'backendAddressPool.id'
          }
          protocol: /*'${10|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${11:80}*/80
          backendPort: /*${12:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: /*${13:'probe.id'}*/'probe.id'
          }
        }
      }
    ]
    probes: [
      {
        name: /*${14:'name'}*/'name'
        properties: {
          protocol: /*'${15|Tcp,Udp,All|}'*/'Tcp'
          port: /*${16:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
