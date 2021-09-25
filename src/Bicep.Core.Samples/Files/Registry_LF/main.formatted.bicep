targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'adotfrank-rg'
  location: deployment().location
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
  name: 'planDeploy'
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

var websites = [
  {
    name: 'fancy'
    tag: 'latest'
  }
  {
    name: 'plain'
    tag: 'plain-text'
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
  name: '${site.name}siteDeploy'
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
  name: 'storageDeploy'
  scope: rg
  params: {
    location: 'eastus'
  }
}

var vnets = [
  {
    name: 'vnet1'
    subnetName: 'subnet1.1'
  }
  {
    name: 'vnet2'
    subnetName: 'subnet2.1'
  }
]

module vnetDeploy 'ts:management.azure.com/11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
  name: '${vnet.name}Deploy'
  scope: rg
  params: {
    vnetName: vnet.name
    subnetName: vnet.subnetName
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
  scope: rg
  name: 'port'
  params: {
    port: 'test'
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
  scope: rg
  name: 'ipv4'
  params: {
    ipv4: 'test'
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
  scope: rg
  name: 'ipv4port'
  params: {
    ipv4port: 'test'
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
  scope: rg
  name: 'ipv6'
  params: {
    ipv6: 'test'
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
  scope: rg
  name: 'ipv6port'
  params: {
    ipv6port: 'test'
  }
}
