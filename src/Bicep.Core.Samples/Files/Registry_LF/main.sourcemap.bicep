targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[33:38]       "type": "Microsoft.Resources/resourceGroups",
  name: 'adotfrank-rg'
  location: deployment().location
//@[37:37]       "location": "[deployment().location]"
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[39:99]       "type": "Microsoft.Resources/deployments",
  name: 'planDeploy'
//@[42:42]       "name": "planDeploy",
  scope: rg
  params: {
    namePrefix: 'hello'
//@[51:51]             "value": "hello"
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[100:160]       "type": "Microsoft.Resources/deployments",
  name: 'planDeploy2'
//@[103:103]       "name": "planDeploy2",
  scope: rg
  params: {
    namePrefix: 'hello'
//@[112:112]             "value": "hello"
  }
}

var websites = [
//@[11:20]     "websites": [
  {
    name: 'fancy'
//@[13:13]         "name": "fancy",
    tag: 'latest'
//@[14:14]         "tag": "latest"
  }
  {
    name: 'plain'
//@[17:17]         "name": "plain",
    tag: 'plain-text'
//@[18:18]         "tag": "plain-text"
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[161:261]       "copy": {
  name: '${site.name}siteDeploy'
//@[168:168]       "name": "[format('{0}siteDeploy', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
//@[177:177]             "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2020-10-01').outputs.planId.value]"
    namePrefix: site.name
//@[180:180]             "value": "[variables('websites')[copyIndex()].name]"
    dockerImage: 'nginxdemos/hello'
//@[183:183]             "value": "nginxdemos/hello"
    dockerImageTag: site.tag
//@[186:186]             "value": "[variables('websites')[copyIndex()].tag]"
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[262:362]       "copy": {
  name: '${site.name}siteDeploy2'
//@[269:269]       "name": "[format('{0}siteDeploy2', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
//@[278:278]             "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2020-10-01').outputs.planId.value]"
    namePrefix: site.name
//@[281:281]             "value": "[variables('websites')[copyIndex()].name]"
    dockerImage: 'nginxdemos/hello'
//@[284:284]             "value": "nginxdemos/hello"
    dockerImageTag: site.tag
//@[287:287]             "value": "[variables('websites')[copyIndex()].tag]"
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[363:385]       "type": "Microsoft.Resources/deployments",
  name: 'storageDeploy'
//@[366:366]       "name": "storageDeploy",
  scope: rg
  params: {
    location: 'eastus'
//@[375:375]             "value": "eastus"
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[386:408]       "type": "Microsoft.Resources/deployments",
  name: 'storageDeploy2'
//@[389:389]       "name": "storageDeploy2",
  scope: rg
  params: {
    location: 'eastus'
//@[398:398]             "value": "eastus"
  }
}

var vnets = [
//@[21:30]     "vnets": [
  {
    name: 'vnet1'
//@[23:23]         "name": "vnet1",
    subnetName: 'subnet1.1'
//@[24:24]         "subnetName": "subnet1.1"
  }
  {
    name: 'vnet2'
//@[27:27]         "name": "vnet2",
    subnetName: 'subnet2.1'
//@[28:28]         "subnetName": "subnet2.1"
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[409:438]       "copy": {
  name: '${vnet.name}Deploy'
//@[416:416]       "name": "[format('{0}Deploy', variables('vnets')[copyIndex()].name)]",
  scope: rg
  params: {
    vnetName: vnet.name
//@[425:425]             "value": "[variables('vnets')[copyIndex()].name]"
    subnetName: vnet.subnetName
//@[428:428]             "value": "[variables('vnets')[copyIndex()].subnetName]"
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[656:662]     "siteUrls": {

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[439:481]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'port'
//@[442:442]       "name": "port",
  params: {
    port: 'test'
//@[451:451]             "value": "test"
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[482:524]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv4'
//@[485:485]       "name": "ipv4",
  params: {
    ipv4: 'test'
//@[494:494]             "value": "test"
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[525:567]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv4port'
//@[528:528]       "name": "ipv4port",
  params: {
    ipv4port: 'test'
//@[537:537]             "value": "test"
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[568:610]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv6'
//@[571:571]       "name": "ipv6",
  params: {
    ipv6: 'test'
//@[580:580]             "value": "test"
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[611:653]       "type": "Microsoft.Resources/deployments",
  scope: rg
  name: 'ipv6port'
//@[614:614]       "name": "ipv6port",
  params: {
    ipv6port: 'test'
//@[623:623]             "value": "test"
  }
}
