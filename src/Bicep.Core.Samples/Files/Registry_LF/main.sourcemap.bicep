targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[33:38]       "type": "Microsoft.Resources/resourceGroups",
  name: 'adotfrank-rg'
  location: deployment().location
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[39:99]       "type": "Microsoft.Resources/deployments",
  name: 'planDeploy'
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[100:160]       "type": "Microsoft.Resources/deployments",
  name: 'planDeploy2'
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

var websites = [
//@[11:20]     "websites": [
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
//@[161:261]       "[string('copy')]": {
  name: '${site.name}siteDeploy'
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[262:362]       "[string('copy')]": {
  name: '${site.name}siteDeploy2'
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[363:385]       "type": "Microsoft.Resources/deployments",
  name: 'storageDeploy'
  scope: rg
  params: {
    location: 'eastus'
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[386:408]       "type": "Microsoft.Resources/deployments",
  name: 'storageDeploy2'
  scope: rg
  params: {
    location: 'eastus'
  }
}

var vnets = [
//@[21:30]     "vnets": [
  {
    name: 'vnet1'
    subnetName: 'subnet1.1'
  }
  {
    name: 'vnet2'
    subnetName: 'subnet2.1'
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[409:438]       "[string('copy')]": {
  name: '${vnet.name}Deploy'
  scope: rg
  params: {
    vnetName: vnet.name
    subnetName: vnet.subnetName
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[656:662]     "siteUrls": {

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[439:481]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'port'
  params: {
    port: 'test'
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[482:524]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv4'
  params: {
    ipv4: 'test'
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[525:567]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv4port'
  params: {
    ipv4port: 'test'
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[568:610]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv6'
  params: {
    ipv6: 'test'
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[611:653]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv6port'
  params: {
    ipv6port: 'test'
  }
}
