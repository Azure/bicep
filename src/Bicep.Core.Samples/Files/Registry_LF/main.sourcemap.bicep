targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[33:38]       "type": "Microsoft.Resources/resourceGroups",\r
  name: 'adotfrank-rg'
  location: deployment().location
//@[37:37]       "location": "[deployment().location]"\r
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[39:99]       "type": "Microsoft.Resources/deployments",\r
  name: 'planDeploy'
//@[42:42]       "name": "planDeploy",\r
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[100:160]       "type": "Microsoft.Resources/deployments",\r
  name: 'planDeploy2'
//@[103:103]       "name": "planDeploy2",\r
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

var websites = [
//@[11:20]     "websites": [\r
  {
    name: 'fancy'
//@[13:13]         "name": "fancy",\r
    tag: 'latest'
//@[14:14]         "tag": "latest"\r
  }
  {
    name: 'plain'
//@[17:17]         "name": "plain",\r
    tag: 'plain-text'
//@[18:18]         "tag": "plain-text"\r
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[161:261]       "copy": {\r
  name: '${site.name}siteDeploy'
//@[168:168]       "name": "[format('{0}siteDeploy', variables('websites')[copyIndex()].name)]",\r
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[262:362]       "copy": {\r
  name: '${site.name}siteDeploy2'
//@[269:269]       "name": "[format('{0}siteDeploy2', variables('websites')[copyIndex()].name)]",\r
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[363:385]       "type": "Microsoft.Resources/deployments",\r
  name: 'storageDeploy'
//@[366:366]       "name": "storageDeploy",\r
  scope: rg
  params: {
    location: 'eastus'
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[386:408]       "type": "Microsoft.Resources/deployments",\r
  name: 'storageDeploy2'
//@[389:389]       "name": "storageDeploy2",\r
  scope: rg
  params: {
    location: 'eastus'
  }
}

var vnets = [
//@[21:30]     "vnets": [\r
  {
    name: 'vnet1'
//@[23:23]         "name": "vnet1",\r
    subnetName: 'subnet1.1'
//@[24:24]         "subnetName": "subnet1.1"\r
  }
  {
    name: 'vnet2'
//@[27:27]         "name": "vnet2",\r
    subnetName: 'subnet2.1'
//@[28:28]         "subnetName": "subnet2.1"\r
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[409:438]       "copy": {\r
  name: '${vnet.name}Deploy'
//@[416:416]       "name": "[format('{0}Deploy', variables('vnets')[copyIndex()].name)]",\r
  scope: rg
  params: {
    vnetName: vnet.name
    subnetName: vnet.subnetName
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[656:662]     "siteUrls": {\r

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[439:481]       "type": "Microsoft.Resources/deployments",\r
  scope: rg
  name: 'port'
//@[442:442]       "name": "port",\r
  params: {
    port: 'test'
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[482:524]       "type": "Microsoft.Resources/deployments",\r
  scope: rg
  name: 'ipv4'
//@[485:485]       "name": "ipv4",\r
  params: {
    ipv4: 'test'
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[525:567]       "type": "Microsoft.Resources/deployments",\r
  scope: rg
  name: 'ipv4port'
//@[528:528]       "name": "ipv4port",\r
  params: {
    ipv4port: 'test'
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[568:610]       "type": "Microsoft.Resources/deployments",\r
  scope: rg
  name: 'ipv6'
//@[571:571]       "name": "ipv6",\r
  params: {
    ipv6: 'test'
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[611:653]       "type": "Microsoft.Resources/deployments",\r
  scope: rg
  name: 'ipv6port'
//@[614:614]       "name": "ipv6port",\r
  params: {
    ipv6port: 'test'
  }
}
