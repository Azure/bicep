targetScope = 'subscription'

param location string = 'uksouth'
param adminSshKey string = 'ssh-rsa APublicSshKey2EAAAADAQABAAACAQCpXKIzGlb9fnLJmwrVPFQyntzOngz8QE4SOMHyXueVb4bO2IgSdHk8cD2LDToX3lDOUQzzWz+AiUO3iIRNJPOzZj/6aN+BJZXpi8+cdGOyaQmDyzk0T2w0mwTxXBk3DkXAw+lw13Q9SlFY+YLKsqHyKF7aXiy7RiJl4O3QUMGLKuGtmsqRFcrarp20pyH1UXQbvXUUoPU92AU4cSmx4AS8coWaDoQxWr7EA6toF0hgXsKFf/8MlkzJty0P7IhZ8KPzJg9lFNfBCUZHFEQWSb7FQBV6mFXxVcus1eCtoLEXkIDSkkYGd+edMO6t/Hc73c66M1vL9Ae6RUx2m39kZGF9bpVmcs8pZ2Hy2QukcGR8r61Jx913a32hRmk5fWpCnEo0NfE9XQJ7ibMNU97XL/QSeNZp3yzAyZqIYBkaYp8bFNjjMnVNyVdaANw2rjmxTY2XJlc0jVgucMWim8zT4YDQgKR8UuzXZBtC5uxlqhgZ8Zj+tRqgq/ZGo1MBacj89gQJjiiyFgf9hewVtdxlAEnDUHo0KI/Ro2geI+f2ylf1m0bUuPpwjO8sybJg/ZkA48LyOMbwj9wLHbiNJnPmoJROGGO/pAdvrzMlDuD/2BLf5Xmn3RkvnseS6/qCXZAfsrwlhb/LtCkWF2j+7e5EWXArhT14fGdWHi+5f09UY7ybAQ== user@Somewhere'
param myIp string = '20.49.199.4'
param zone int = 1

resource rgnet 'Microsoft.Resources/resourceGroups@2019-05-01' = {
  name: 'bicep-network-rg'
  location: location
}

resource rganchor 'Microsoft.Resources/resourceGroups@2019-05-01' = {
  name: 'bicep-anchor-rg'
  location: location
}

resource rgworkload 'Microsoft.Resources/resourceGroups@2019-05-01' = {
  name: 'bicep-workload-rg'
  location: location
}

module network './network.bicep' = {
  name: 'network'
  scope: rgnet
  params: {
    myIp: myIp
  }
}

module ppg './anchored-ppg.bicep' = {
  name: 'anchor'
  scope: rganchor
  params: {
    adminSshKey: adminSshKey
    subnetId: '${network.outputs.vnetId}/subnets/Default'
    vmName: 'anchor'
    zone: zone
  }
}

module vm01 './linux-vm-as.bicep' = {
  name: 'vm01'
  scope: rgworkload
  params: {
    adminSshKey: adminSshKey
    ppgId: ppg.outputs.ppgId
    subnetId: '${network.outputs.vnetId}/subnets/Default'
    vmName: 'vm01'
  }
}

module vm02 './linux-vm-as.bicep' = {
  name: 'vm02'
  scope: rgworkload
  params: {
    adminSshKey: adminSshKey
    ppgId: ppg.outputs.ppgId
    subnetId: '${network.outputs.vnetId}/subnets/Default'
    vmName: 'vm02'
  }
}
