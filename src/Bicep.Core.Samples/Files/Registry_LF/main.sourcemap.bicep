targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[34:39]       "type": "Microsoft.Resources/resourceGroups",
  name: 'adotfrank-rg'
  location: deployment().location
//@[38:38]       "location": "[deployment().location]"
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[40:100]       "type": "Microsoft.Resources/deployments",
  name: 'planDeploy'
//@[43:43]       "name": "planDeploy",
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[101:161]       "type": "Microsoft.Resources/deployments",
  name: 'planDeploy2'
//@[104:104]       "name": "planDeploy2",
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

var websites = [
//@[12:21]     "websites": [
  {
    name: 'fancy'
//@[14:14]         "name": "fancy",
    tag: 'latest'
//@[15:15]         "tag": "latest"
  }
  {
    name: 'plain'
//@[18:18]         "name": "plain",
    tag: 'plain-text'
//@[19:19]         "tag": "plain-text"
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[162:262]       "copy": {
  name: '${site.name}siteDeploy'
//@[169:169]       "name": "[format('{0}siteDeploy', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[263:363]       "copy": {
  name: '${site.name}siteDeploy2'
//@[270:270]       "name": "[format('{0}siteDeploy2', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[364:386]       "type": "Microsoft.Resources/deployments",
  name: 'storageDeploy'
//@[367:367]       "name": "storageDeploy",
  scope: rg
  params: {
    location: 'eastus'
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[387:409]       "type": "Microsoft.Resources/deployments",
  name: 'storageDeploy2'
//@[390:390]       "name": "storageDeploy2",
  scope: rg
  params: {
    location: 'eastus'
  }
}

var vnets = [
//@[22:31]     "vnets": [
  {
    name: 'vnet1'
//@[24:24]         "name": "vnet1",
    subnetName: 'subnet1.1'
//@[25:25]         "subnetName": "subnet1.1"
  }
  {
    name: 'vnet2'
//@[28:28]         "name": "vnet2",
    subnetName: 'subnet2.1'
//@[29:29]         "subnetName": "subnet2.1"
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[410:439]       "copy": {
  name: '${vnet.name}Deploy'
//@[417:417]       "name": "[format('{0}Deploy', variables('vnets')[copyIndex()].name)]",
  scope: rg
  params: {
    vnetName: vnet.name
    subnetName: vnet.subnetName
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[657:663]     "siteUrls": {

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[440:482]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'port'
//@[443:443]       "name": "port",
  params: {
    port: 'test'
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[483:525]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv4'
//@[486:486]       "name": "ipv4",
  params: {
    ipv4: 'test'
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[526:568]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv4port'
//@[529:529]       "name": "ipv4port",
  params: {
    ipv4port: 'test'
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[569:611]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv6'
//@[572:572]       "name": "ipv6",
  params: {
    ipv6: 'test'
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[612:654]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv6port'
//@[615:615]       "name": "ipv6port",
  params: {
    ipv6port: 'test'
  }
}
