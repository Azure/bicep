// External Load Balancer
resource /*${1:publicIPAddresses}*/publicIPAddresses 'Microsoft.Network/publicIPAddresses@2021-02-01' existing = {
  name: /*${2:'name'}*/'name'
}

resource /*${3:backendAddressPool}*/backendAddressPool 'Microsoft.Network/loadBalancers/backendAddressPools@2021-02-01' existing = {
  name: /*${4:'loadBalancerExternal/loadBalancerBackEndPool'}*/'loadBalancerExternal/loadBalancerBackEndPool'
}

resource /*${5:loadBalancerExternal}*/loadBalancerExternal 'Microsoft.Network/loadBalancers@2020-11-01' = {
  name: /*${6:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    frontendIPConfigurations: [
      {
        name: /*${7:'name'}*/'name'
        properties: {
          publicIPAddress: {
            id: publicIPAddresses.id
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${8:'name'}*/'name'
      }
    ]
    inboundNatRules: [
      {
        name: /*${9:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: /*${10:'id'}*/'id'
          }
          protocol: /*'${11|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${12:50001}*/50001
          backendPort: /*${13:3389}*/3389
          enableFloatingIP: false
        }
      }
    ]
    loadBalancingRules: [
      {
        name: /*${14:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
             id: /*${15:'id'}*/'id'
          }
          backendAddressPool: {
            id: backendAddressPool.id
          }
          protocol: /*'${16|Tcp,Udp,All|}'*/'Tcp'
          frontendPort: /*${17:80}*/80
          backendPort: /*${18:80}*/80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          probe: {
            id: /*${19:'id'}*/'id'
          }
        }
      }
    ]
    probes: [
      {
        name: /*${20:'name'}*/'name'
        properties: {
          protocol: /*'${21|Tcp,Udp,All|}'*/'Tcp'
          port: /*${22:80}*/80
          intervalInSeconds: 5
          numberOfProbes: 2
        }
      }
    ]
  }
}
