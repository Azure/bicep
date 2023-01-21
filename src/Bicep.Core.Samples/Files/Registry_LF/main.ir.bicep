targetScope = 'subscription'
//@[000:2463) ProgramExpression
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[000:0122) ├─DeclaredResourceExpression
//@[062:0122) | └─ObjectExpression
  name: 'adotfrank-rg'
  location: deployment().location
//@[002:0033) |   └─ObjectPropertyExpression
//@[002:0010) |     ├─StringLiteralExpression { Value = location }
//@[012:0033) |     └─PropertyAccessExpression { PropertyName = location }
//@[012:0024) |       └─FunctionCallExpression { Name = deployment }
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[000:0143) ├─DeclaredModuleExpression
//@[067:0143) | ├─ObjectExpression
  name: 'planDeploy'
//@[002:0020) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0020) | |   └─StringLiteralExpression { Value = planDeploy }
  scope: rg
  params: {
//@[010:0039) | ├─ObjectExpression
    namePrefix: 'hello'
//@[004:0023) | | └─ObjectPropertyExpression
//@[004:0014) | |   ├─StringLiteralExpression { Value = namePrefix }
//@[016:0023) | |   └─StringLiteralExpression { Value = hello }
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[000:0137) ├─DeclaredModuleExpression
//@[060:0137) | ├─ObjectExpression
  name: 'planDeploy2'
//@[002:0021) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0021) | |   └─StringLiteralExpression { Value = planDeploy2 }
  scope: rg
  params: {
//@[010:0039) | ├─ObjectExpression
    namePrefix: 'hello'
//@[004:0023) | | └─ObjectPropertyExpression
//@[004:0014) | |   ├─StringLiteralExpression { Value = namePrefix }
//@[016:0023) | |   └─StringLiteralExpression { Value = hello }
  }
}

var websites = [
//@[000:0110) ├─DeclaredVariableExpression { Name = websites }
//@[015:0110) | └─ArrayExpression
  {
//@[002:0043) |   ├─ObjectExpression
    name: 'fancy'
//@[004:0017) |   | ├─ObjectPropertyExpression
//@[004:0008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:0017) |   | | └─StringLiteralExpression { Value = fancy }
    tag: 'latest'
//@[004:0017) |   | └─ObjectPropertyExpression
//@[004:0007) |   |   ├─StringLiteralExpression { Value = tag }
//@[009:0017) |   |   └─StringLiteralExpression { Value = latest }
  }
  {
//@[002:0047) |   └─ObjectExpression
    name: 'plain'
//@[004:0017) |     ├─ObjectPropertyExpression
//@[004:0008) |     | ├─StringLiteralExpression { Value = name }
//@[010:0017) |     | └─StringLiteralExpression { Value = plain }
    tag: 'plain-text'
//@[004:0021) |     └─ObjectPropertyExpression
//@[004:0007) |       ├─StringLiteralExpression { Value = tag }
//@[009:0021) |       └─StringLiteralExpression { Value = plain-text }
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[000:0287) ├─DeclaredModuleExpression
//@[064:0287) | ├─ForLoopExpression
//@[077:0085) | | ├─VariableReferenceExpression { Variable = websites }
//@[087:0286) | | └─ObjectExpression
//@[077:0085) | |           └─VariableReferenceExpression { Variable = websites }
//@[077:0085) | | |     └─VariableReferenceExpression { Variable = websites }
//@[077:0085) | |       └─VariableReferenceExpression { Variable = websites }
  name: '${site.name}siteDeploy'
//@[002:0032) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0032) | |     └─InterpolatedStringExpression
//@[011:0020) | |       └─PropertyAccessExpression { PropertyName = name }
//@[011:0015) | |         └─ArrayAccessExpression
//@[011:0015) | |           ├─CopyIndexExpression
  scope: rg
  params: {
//@[010:0150) | ├─ObjectExpression
    appPlanId: appPlanDeploy.outputs.planId
//@[004:0043) | | ├─ObjectPropertyExpression
//@[004:0013) | | | ├─StringLiteralExpression { Value = appPlanId }
//@[015:0043) | | | └─ModuleOutputPropertyAccessExpression { PropertyName = planId }
//@[015:0036) | | |   └─PropertyAccessExpression { PropertyName = outputs }
//@[015:0028) | | |     └─ModuleReferenceExpression
    namePrefix: site.name
//@[004:0025) | | ├─ObjectPropertyExpression
//@[004:0014) | | | ├─StringLiteralExpression { Value = namePrefix }
//@[016:0025) | | | └─PropertyAccessExpression { PropertyName = name }
//@[016:0020) | | |   └─ArrayAccessExpression
//@[016:0020) | | |     ├─CopyIndexExpression
    dockerImage: 'nginxdemos/hello'
//@[004:0035) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = dockerImage }
//@[017:0035) | | | └─StringLiteralExpression { Value = nginxdemos/hello }
    dockerImageTag: site.tag
//@[004:0028) | | └─ObjectPropertyExpression
//@[004:0018) | |   ├─StringLiteralExpression { Value = dockerImageTag }
//@[020:0028) | |   └─PropertyAccessExpression { PropertyName = tag }
//@[020:0024) | |     └─ArrayAccessExpression
//@[020:0024) | |       ├─CopyIndexExpression
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[000:0267) ├─DeclaredModuleExpression
//@[043:0267) | ├─ForLoopExpression
//@[056:0064) | | ├─VariableReferenceExpression { Variable = websites }
//@[066:0266) | | └─ObjectExpression
//@[056:0064) | |           └─VariableReferenceExpression { Variable = websites }
//@[056:0064) | | |     └─VariableReferenceExpression { Variable = websites }
//@[056:0064) | |       └─VariableReferenceExpression { Variable = websites }
  name: '${site.name}siteDeploy2'
//@[002:0033) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0033) | |     └─InterpolatedStringExpression
//@[011:0020) | |       └─PropertyAccessExpression { PropertyName = name }
//@[011:0015) | |         └─ArrayAccessExpression
//@[011:0015) | |           ├─CopyIndexExpression
  scope: rg
  params: {
//@[010:0150) | ├─ObjectExpression
    appPlanId: appPlanDeploy.outputs.planId
//@[004:0043) | | ├─ObjectPropertyExpression
//@[004:0013) | | | ├─StringLiteralExpression { Value = appPlanId }
//@[015:0043) | | | └─ModuleOutputPropertyAccessExpression { PropertyName = planId }
//@[015:0036) | | |   └─PropertyAccessExpression { PropertyName = outputs }
//@[015:0028) | | |     └─ModuleReferenceExpression
    namePrefix: site.name
//@[004:0025) | | ├─ObjectPropertyExpression
//@[004:0014) | | | ├─StringLiteralExpression { Value = namePrefix }
//@[016:0025) | | | └─PropertyAccessExpression { PropertyName = name }
//@[016:0020) | | |   └─ArrayAccessExpression
//@[016:0020) | | |     ├─CopyIndexExpression
    dockerImage: 'nginxdemos/hello'
//@[004:0035) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = dockerImage }
//@[017:0035) | | | └─StringLiteralExpression { Value = nginxdemos/hello }
    dockerImageTag: site.tag
//@[004:0028) | | └─ObjectPropertyExpression
//@[004:0018) | |   ├─StringLiteralExpression { Value = dockerImageTag }
//@[020:0028) | |   └─PropertyAccessExpression { PropertyName = tag }
//@[020:0024) | |     └─ArrayAccessExpression
//@[020:0024) | |       ├─CopyIndexExpression
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[000:0168) ├─DeclaredModuleExpression
//@[090:0168) | ├─ObjectExpression
  name: 'storageDeploy'
//@[002:0023) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0023) | |   └─StringLiteralExpression { Value = storageDeploy }
  scope: rg
  params: {
//@[010:0038) | ├─ObjectExpression
    location: 'eastus'
//@[004:0022) | | └─ObjectPropertyExpression
//@[004:0012) | |   ├─StringLiteralExpression { Value = location }
//@[014:0022) | |   └─StringLiteralExpression { Value = eastus }
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[000:0134) ├─DeclaredModuleExpression
//@[055:0134) | ├─ObjectExpression
  name: 'storageDeploy2'
//@[002:0024) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0024) | |   └─StringLiteralExpression { Value = storageDeploy2 }
  scope: rg
  params: {
//@[010:0038) | ├─ObjectExpression
    location: 'eastus'
//@[004:0022) | | └─ObjectPropertyExpression
//@[004:0012) | |   ├─StringLiteralExpression { Value = location }
//@[014:0022) | |   └─StringLiteralExpression { Value = eastus }
  }
}

var vnets = [
//@[000:0123) ├─DeclaredVariableExpression { Name = vnets }
//@[012:0123) | └─ArrayExpression
  {
//@[002:0053) |   ├─ObjectExpression
    name: 'vnet1'
//@[004:0017) |   | ├─ObjectPropertyExpression
//@[004:0008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:0017) |   | | └─StringLiteralExpression { Value = vnet1 }
    subnetName: 'subnet1.1'
//@[004:0027) |   | └─ObjectPropertyExpression
//@[004:0014) |   |   ├─StringLiteralExpression { Value = subnetName }
//@[016:0027) |   |   └─StringLiteralExpression { Value = subnet1.1 }
  }
  {
//@[002:0053) |   └─ObjectExpression
    name: 'vnet2'
//@[004:0017) |     ├─ObjectPropertyExpression
//@[004:0008) |     | ├─StringLiteralExpression { Value = name }
//@[010:0017) |     | └─StringLiteralExpression { Value = vnet2 }
    subnetName: 'subnet2.1'
//@[004:0027) |     └─ObjectPropertyExpression
//@[004:0014) |       ├─StringLiteralExpression { Value = subnetName }
//@[016:0027) |       └─StringLiteralExpression { Value = subnet2.1 }
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[000:0220) ├─DeclaredModuleExpression
//@[083:0220) | ├─ForLoopExpression
//@[096:0101) | | ├─VariableReferenceExpression { Variable = vnets }
//@[103:0219) | | └─ObjectExpression
//@[096:0101) | |           └─VariableReferenceExpression { Variable = vnets }
//@[096:0101) | | |     └─VariableReferenceExpression { Variable = vnets }
//@[096:0101) | |       └─VariableReferenceExpression { Variable = vnets }
  name: '${vnet.name}Deploy'
//@[002:0028) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0028) | |     └─InterpolatedStringExpression
//@[011:0020) | |       └─PropertyAccessExpression { PropertyName = name }
//@[011:0015) | |         └─ArrayAccessExpression
//@[011:0015) | |           ├─CopyIndexExpression
  scope: rg
  params: {
//@[010:0071) | ├─ObjectExpression
    vnetName: vnet.name
//@[004:0023) | | ├─ObjectPropertyExpression
//@[004:0012) | | | ├─StringLiteralExpression { Value = vnetName }
//@[014:0023) | | | └─PropertyAccessExpression { PropertyName = name }
//@[014:0018) | | |   └─ArrayAccessExpression
//@[014:0018) | | |     ├─CopyIndexExpression
    subnetName: vnet.subnetName
//@[004:0031) | | └─ObjectPropertyExpression
//@[004:0014) | |   ├─StringLiteralExpression { Value = subnetName }
//@[016:0031) | |   └─PropertyAccessExpression { PropertyName = subnetName }
//@[016:0020) | |     └─ArrayAccessExpression
//@[016:0020) | |       ├─CopyIndexExpression
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[000:0082) └─DeclaredOutputExpression { Name = siteUrls }
//@[024:0082)   └─ForLoopExpression
//@[042:0050)     ├─VariableReferenceExpression { Variable = websites }
//@[052:0081)     └─ModuleOutputPropertyAccessExpression { PropertyName = siteUrl }
//@[052:0073)       └─PropertyAccessExpression { PropertyName = outputs }
//@[052:0065)         └─ModuleReferenceExpression

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[000:0128) ├─DeclaredModuleExpression
//@[065:0128) | ├─ObjectExpression
  scope: rg
  name: 'port'
//@[002:0014) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0014) | |   └─StringLiteralExpression { Value = port }
  params: {
//@[010:0032) | ├─ObjectExpression
    port: 'test'
//@[004:0016) | | └─ObjectPropertyExpression
//@[004:0008) | |   ├─StringLiteralExpression { Value = port }
//@[010:0016) | |   └─StringLiteralExpression { Value = test }
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[000:0112) ├─DeclaredModuleExpression
//@[049:0112) | ├─ObjectExpression
  scope: rg
  name: 'ipv4'
//@[002:0014) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0014) | |   └─StringLiteralExpression { Value = ipv4 }
  params: {
//@[010:0032) | ├─ObjectExpression
    ipv4: 'test'
//@[004:0016) | | └─ObjectPropertyExpression
//@[004:0008) | |   ├─StringLiteralExpression { Value = ipv4 }
//@[010:0016) | |   └─StringLiteralExpression { Value = test }
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[000:0133) ├─DeclaredModuleExpression
//@[062:0133) | ├─ObjectExpression
  scope: rg
  name: 'ipv4port'
//@[002:0018) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0018) | |   └─StringLiteralExpression { Value = ipv4port }
  params: {
//@[010:0036) | ├─ObjectExpression
    ipv4port: 'test'
//@[004:0020) | | └─ObjectPropertyExpression
//@[004:0012) | |   ├─StringLiteralExpression { Value = ipv4port }
//@[014:0020) | |   └─StringLiteralExpression { Value = test }
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[000:0108) ├─DeclaredModuleExpression
//@[045:0108) | ├─ObjectExpression
  scope: rg
  name: 'ipv6'
//@[002:0014) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0014) | |   └─StringLiteralExpression { Value = ipv6 }
  params: {
//@[010:0032) | ├─ObjectExpression
    ipv6: 'test'
//@[004:0016) | | └─ObjectPropertyExpression
//@[004:0008) | |   ├─StringLiteralExpression { Value = ipv6 }
//@[010:0016) | |   └─StringLiteralExpression { Value = test }
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[000:0129) ├─DeclaredModuleExpression
//@[058:0129) | ├─ObjectExpression
  scope: rg
  name: 'ipv6port'
//@[002:0018) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0018) | |   └─StringLiteralExpression { Value = ipv6port }
  params: {
//@[010:0036) | ├─ObjectExpression
    ipv6port: 'test'
//@[004:0020) | | └─ObjectPropertyExpression
//@[004:0012) | |   ├─StringLiteralExpression { Value = ipv6port }
//@[014:0020) | |   └─StringLiteralExpression { Value = test }
  }
}
