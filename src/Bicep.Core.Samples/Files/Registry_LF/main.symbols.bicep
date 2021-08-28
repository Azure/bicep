targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[9:11) Resource rg. Type: Microsoft.Resources/resourceGroups@2020-06-01. Declaration start char: 0, length: 122
  name: 'adotfrank-rg'
  location: deployment().location
}

module appPlanDeploy 'oci:mock-registry-one.invalid/demo/plan:v2' = {
//@[7:20) Module appPlanDeploy. Type: module. Declaration start char: 0, length: 144
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

module siteDeploy 'oci:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[70:74) Local site. Type: any. Declaration start char: 70, length: 4
//@[7:17) Module siteDeploy. Type: module[]. Declaration start char: 0, length: 288
  name: '${site.name}siteDeploy'
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
    namePrefix: site.name
    dockerImage: 'nginxdemos/hello'
    dockerImageTag: site.tag
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[30:34) Local site. Type: any. Declaration start char: 30, length: 4
//@[36:37) Local i. Type: int. Declaration start char: 36, length: 1
//@[7:15) Output siteUrls. Type: array. Declaration start char: 0, length: 82

