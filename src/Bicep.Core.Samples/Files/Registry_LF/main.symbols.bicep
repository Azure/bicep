targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[9:11) Resource rg. Type: Microsoft.Resources/resourceGroups@2020-06-01. Declaration start char: 0, length: 122
  name: 'adotfrank-rg'
  location: deployment().location
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[7:20) Module appPlanDeploy. Type: module. Declaration start char: 0, length: 143
  name: 'planDeploy'
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

var websites = [
//@[4:12) Variable websites. Type: array. Declaration start char: 0, length: 110
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
//@[69:73) Local site. Type: any. Declaration start char: 69, length: 4
//@[7:17) Module siteDeploy. Type: module[]. Declaration start char: 0, length: 287
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
//@[7:20) Module storageDeploy. Type: module. Declaration start char: 0, length: 168
  name: 'storageDeploy'
  scope: rg
  params: {
    location: 'eastus'
  }
}

var vnets = [
//@[4:9) Variable vnets. Type: array. Declaration start char: 0, length: 123
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
//@[109:113) Local vnet. Type: any. Declaration start char: 109, length: 4
//@[7:17) Module vnetDeploy. Type: module[]. Declaration start char: 0, length: 241
  name: '${vnet.name}Deploy'
  scope: rg
  params: {
    vnetName: vnet.name
    subnetName: vnet.subnetName
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[30:34) Local site. Type: any. Declaration start char: 30, length: 4
//@[36:37) Local i. Type: int. Declaration start char: 36, length: 1
//@[7:15) Output siteUrls. Type: array. Declaration start char: 0, length: 82

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[7:22) Module passthroughPort. Type: module. Declaration start char: 0, length: 128
  scope: rg
  name: 'port'
  params: {
    port: 'test'
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[7:11) Module ipv4. Type: module. Declaration start char: 0, length: 112
  scope: rg
  name: 'ipv4'
  params: {
    ipv4: 'test'
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[7:15) Module ipv4port. Type: module. Declaration start char: 0, length: 133
  scope: rg
  name: 'ipv4port'
  params: {
    ipv4port: 'test'
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[7:11) Module ipv6. Type: module. Declaration start char: 0, length: 108
  scope: rg
  name: 'ipv6'
  params: {
    ipv6: 'test'
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[7:15) Module ipv6port. Type: module. Declaration start char: 0, length: 129
  scope: rg
  name: 'ipv6port'
  params: {
    ipv6port: 'test'
  }
}
