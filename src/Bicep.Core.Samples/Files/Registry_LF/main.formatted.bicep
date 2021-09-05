targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'adotfrank-rg'
  location: deployment().location
}

module appPlanDeploy 'oci:mock-registry-one.invalid/demo/plan:v2' = {
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

module siteDeploy 'oci:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
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
