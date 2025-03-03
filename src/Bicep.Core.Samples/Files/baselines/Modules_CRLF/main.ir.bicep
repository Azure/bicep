
//@[000:10080) ProgramExpression
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ModuleReferenceExpression [UNPARENTED]
@sys.description('this is deployTimeSuffix param')
//@[000:00093) ├─DeclaredParameterExpression { Name = deployTimeSuffix }
//@[017:00049) | ├─StringLiteralExpression { Value = this is deployTimeSuffix param }
param deployTimeSuffix string = newGuid()
//@[023:00029) | ├─AmbientTypeReferenceExpression { Name = string }
//@[032:00041) | └─FunctionCallExpression { Name = newGuid }

@sys.description('this module a')
//@[000:00252) ├─DeclaredModuleExpression
//@[017:00032) | ├─StringLiteralExpression { Value = this module a }
module modATest './modulea.bicep' = {
//@[036:00217) | ├─ObjectExpression
  name: 'modATest'
//@[002:00018) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00018) | |   └─StringLiteralExpression { Value = modATest }
  params: {
//@[010:00155) | └─ObjectExpression
    stringParamB: 'hello!'
//@[004:00026) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00026) |   | └─StringLiteralExpression { Value = hello! }
    objParam: {
//@[004:00036) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00036) |   | └─ObjectExpression
      a: 'b'
//@[006:00012) |   |   └─ObjectPropertyExpression
//@[006:00007) |   |     ├─StringLiteralExpression { Value = a }
//@[009:00012) |   |     └─StringLiteralExpression { Value = b }
    }
    arrayParam: [
//@[004:00071) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00071) |     └─ArrayExpression
      {
//@[006:00032) |       ├─ObjectExpression
        a: 'b'
//@[008:00014) |       | └─ObjectPropertyExpression
//@[008:00009) |       |   ├─StringLiteralExpression { Value = a }
//@[011:00014) |       |   └─StringLiteralExpression { Value = b }
      }
      'abc'
//@[006:00011) |       └─StringLiteralExpression { Value = abc }
    ]
  }
}


@sys.description('this module b')
//@[000:00136) ├─DeclaredModuleExpression
//@[017:00032) | ├─StringLiteralExpression { Value = this module b }
module modB './child/moduleb.bicep' = {
//@[038:00101) | ├─ObjectExpression
  name: 'modB'
//@[002:00014) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00014) | |   └─StringLiteralExpression { Value = modB }
  params: {
//@[010:00041) | └─ObjectExpression
    location: 'West US'
//@[004:00023) |   └─ObjectPropertyExpression
//@[004:00012) |     ├─StringLiteralExpression { Value = location }
//@[014:00023) |     └─StringLiteralExpression { Value = West US }
  }
}

@sys.description('this is just module b with a condition')
//@[000:00203) ├─DeclaredModuleExpression
//@[017:00057) | ├─StringLiteralExpression { Value = this is just module b with a condition }
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[055:00065) | ├─ConditionExpression
//@[055:00065) | | ├─BinaryExpression { Operator = Equals }
//@[055:00060) | | | ├─BinaryExpression { Operator = Add }
//@[055:00056) | | | | ├─IntegerLiteralExpression { Value = 1 }
//@[059:00060) | | | | └─IntegerLiteralExpression { Value = 1 }
//@[064:00065) | | | └─IntegerLiteralExpression { Value = 2 }
//@[067:00143) | | └─ObjectExpression
  name: 'modBWithCondition'
//@[002:00027) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00027) | |     └─StringLiteralExpression { Value = modBWithCondition }
  params: {
//@[010:00041) | └─ObjectExpression
    location: 'East US'
//@[004:00023) |   └─ObjectPropertyExpression
//@[004:00012) |     ├─StringLiteralExpression { Value = location }
//@[014:00023) |     └─StringLiteralExpression { Value = East US }
  }
}

module modBWithCondition2 './child/moduleb.bicep' =
//@[000:00166) ├─DeclaredModuleExpression
// awkward comment
if (1 + 1 == 2) {
//@[004:00014) | ├─ConditionExpression
//@[004:00014) | | ├─BinaryExpression { Operator = Equals }
//@[004:00009) | | | ├─BinaryExpression { Operator = Add }
//@[004:00005) | | | | ├─IntegerLiteralExpression { Value = 1 }
//@[008:00009) | | | | └─IntegerLiteralExpression { Value = 1 }
//@[013:00014) | | | └─IntegerLiteralExpression { Value = 2 }
//@[016:00093) | | └─ObjectExpression
  name: 'modBWithCondition2'
//@[002:00028) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00028) | |     └─StringLiteralExpression { Value = modBWithCondition2 }
  params: {
//@[010:00041) | └─ObjectExpression
    location: 'East US'
//@[004:00023) |   └─ObjectPropertyExpression
//@[004:00012) |     ├─StringLiteralExpression { Value = location }
//@[014:00023) |     └─StringLiteralExpression { Value = East US }
  }
}

module modC './child/modulec.json' = {
//@[000:00100) ├─DeclaredModuleExpression
//@[037:00100) | ├─ObjectExpression
  name: 'modC'
//@[002:00014) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00014) | |   └─StringLiteralExpression { Value = modC }
  params: {
//@[010:00041) | └─ObjectExpression
    location: 'West US'
//@[004:00023) |   └─ObjectPropertyExpression
//@[004:00012) |     ├─StringLiteralExpression { Value = location }
//@[014:00023) |     └─StringLiteralExpression { Value = West US }
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[000:00142) ├─DeclaredModuleExpression
//@[054:00064) | ├─ConditionExpression
//@[054:00064) | | ├─BinaryExpression { Operator = Equals }
//@[054:00059) | | | ├─BinaryExpression { Operator = Subtract }
//@[054:00055) | | | | ├─IntegerLiteralExpression { Value = 2 }
//@[058:00059) | | | | └─IntegerLiteralExpression { Value = 1 }
//@[063:00064) | | | └─IntegerLiteralExpression { Value = 1 }
//@[066:00142) | | └─ObjectExpression
  name: 'modCWithCondition'
//@[002:00027) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00027) | |     └─StringLiteralExpression { Value = modCWithCondition }
  params: {
//@[010:00041) | └─ObjectExpression
    location: 'East US'
//@[004:00023) |   └─ObjectPropertyExpression
//@[004:00012) |     ├─StringLiteralExpression { Value = location }
//@[014:00023) |     └─StringLiteralExpression { Value = East US }
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[000:00098) ├─DeclaredModuleExpression
//@[061:00098) | └─ObjectExpression
  name: 'optionalWithNoParams1'
//@[002:00031) |   └─ObjectPropertyExpression
//@[002:00006) |     ├─StringLiteralExpression { Value = name }
//@[008:00031) |     └─StringLiteralExpression { Value = optionalWithNoParams1 }
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[000:00116) ├─DeclaredModuleExpression
//@[061:00116) | ├─ObjectExpression
  name: 'optionalWithNoParams2'
//@[002:00031) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00031) | |   └─StringLiteralExpression { Value = optionalWithNoParams2 }
  params: {
//@[010:00016) | └─ObjectExpression
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[000:00210) ├─DeclaredModuleExpression
//@[061:00210) | ├─ObjectExpression
  name: 'optionalWithNoParams3'
//@[002:00031) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00031) | |   └─StringLiteralExpression { Value = optionalWithNoParams3 }
  params: {
//@[010:00110) | └─ObjectExpression
    optionalString: 'abc'
//@[004:00025) |   ├─ObjectPropertyExpression
//@[004:00018) |   | ├─StringLiteralExpression { Value = optionalString }
//@[020:00025) |   | └─StringLiteralExpression { Value = abc }
    optionalInt: 42
//@[004:00019) |   ├─ObjectPropertyExpression
//@[004:00015) |   | ├─StringLiteralExpression { Value = optionalInt }
//@[017:00019) |   | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00015) |   | ├─StringLiteralExpression { Value = optionalObj }
//@[017:00020) |   | └─ObjectExpression
    optionalArray: [ ]
//@[004:00022) |   └─ObjectPropertyExpression
//@[004:00017) |     ├─StringLiteralExpression { Value = optionalArray }
//@[019:00022) |     └─ArrayExpression
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00233) ├─DeclaredResourceExpression
//@[065:00233) | ├─ObjectExpression
  name: 'harry'
  properties: {
//@[002:00145) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00145) | |   └─ObjectExpression
    modADep: modATest.outputs.stringOutputA
//@[004:00043) | |     ├─ObjectPropertyExpression
//@[004:00011) | |     | ├─StringLiteralExpression { Value = modADep }
//@[013:00043) | |     | └─ModuleOutputPropertyAccessExpression { PropertyName = stringOutputA }
//@[013:00029) | |     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[013:00021) | |     |     └─ModuleReferenceExpression
    modBDep: modB.outputs.myResourceId
//@[004:00038) | |     ├─ObjectPropertyExpression
//@[004:00011) | |     | ├─StringLiteralExpression { Value = modBDep }
//@[013:00038) | |     | └─ModuleOutputPropertyAccessExpression { PropertyName = myResourceId }
//@[013:00025) | |     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[013:00017) | |     |     └─ModuleReferenceExpression
    modCDep: modC.outputs.myResourceId
//@[004:00038) | |     └─ObjectPropertyExpression
//@[004:00011) | |       ├─StringLiteralExpression { Value = modCDep }
//@[013:00038) | |       └─ModuleOutputPropertyAccessExpression { PropertyName = myResourceId }
//@[013:00025) | |         └─PropertyAccessExpression { PropertyName = outputs }
//@[013:00017) | |           └─ModuleReferenceExpression
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[000:00321) ├─DeclaredModuleExpression
//@[080:00321) | ├─ObjectExpression
  name: 'optionalWithAllParamsAndManualDependency'
//@[002:00050) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00050) | |   └─StringLiteralExpression { Value = optionalWithAllParamsAndManualDependency }
  params: {
//@[010:00110) | ├─ObjectExpression
    optionalString: 'abc'
//@[004:00025) | | ├─ObjectPropertyExpression
//@[004:00018) | | | ├─StringLiteralExpression { Value = optionalString }
//@[020:00025) | | | └─StringLiteralExpression { Value = abc }
    optionalInt: 42
//@[004:00019) | | ├─ObjectPropertyExpression
//@[004:00015) | | | ├─StringLiteralExpression { Value = optionalInt }
//@[017:00019) | | | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:00020) | | ├─ObjectPropertyExpression
//@[004:00015) | | | ├─StringLiteralExpression { Value = optionalObj }
//@[017:00020) | | | └─ObjectExpression
    optionalArray: [ ]
//@[004:00022) | | └─ObjectPropertyExpression
//@[004:00017) | |   ├─StringLiteralExpression { Value = optionalArray }
//@[019:00022) | |   └─ArrayExpression
  }
  dependsOn: [
    resWithDependencies
    optionalWithAllParams
  ]
}

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[000:00300) ├─DeclaredModuleExpression
//@[070:00300) | ├─ObjectExpression
  name: 'optionalWithImplicitDependency'
//@[002:00040) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00040) | |   └─StringLiteralExpression { Value = optionalWithImplicitDependency }
  params: {
//@[010:00182) | ├─ObjectExpression
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:00097) | | ├─ObjectPropertyExpression
//@[004:00018) | | | ├─StringLiteralExpression { Value = optionalString }
//@[020:00097) | | | └─FunctionCallExpression { Name = concat }
//@[027:00049) | | |   ├─PropertyAccessExpression { PropertyName = id }
//@[027:00046) | | |   | └─ResourceReferenceExpression
//@[051:00096) | | |   └─PropertyAccessExpression { PropertyName = name }
//@[051:00091) | | |     └─ModuleReferenceExpression
    optionalInt: 42
//@[004:00019) | | ├─ObjectPropertyExpression
//@[004:00015) | | | ├─StringLiteralExpression { Value = optionalInt }
//@[017:00019) | | | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:00020) | | ├─ObjectPropertyExpression
//@[004:00015) | | | ├─StringLiteralExpression { Value = optionalObj }
//@[017:00020) | | | └─ObjectExpression
    optionalArray: [ ]
//@[004:00022) | | └─ObjectPropertyExpression
//@[004:00017) | |   ├─StringLiteralExpression { Value = optionalArray }
//@[019:00022) | |   └─ArrayExpression
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[000:00331) ├─DeclaredModuleExpression
//@[064:00331) | ├─ObjectExpression
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:00077) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00077) | |   └─InterpolatedStringExpression
//@[011:00056) | |     ├─PropertyAccessExpression { PropertyName = name }
//@[011:00051) | |     | └─ModuleReferenceExpression
//@[059:00075) | |     └─ParametersReferenceExpression { Parameter = deployTimeSuffix }
  params: {
//@[010:00182) | ├─ObjectExpression
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:00097) | | ├─ObjectPropertyExpression
//@[004:00018) | | | ├─StringLiteralExpression { Value = optionalString }
//@[020:00097) | | | └─FunctionCallExpression { Name = concat }
//@[027:00049) | | |   ├─PropertyAccessExpression { PropertyName = id }
//@[027:00046) | | |   | └─ResourceReferenceExpression
//@[051:00096) | | |   └─PropertyAccessExpression { PropertyName = name }
//@[051:00091) | | |     └─ModuleReferenceExpression
    optionalInt: 42
//@[004:00019) | | ├─ObjectPropertyExpression
//@[004:00015) | | | ├─StringLiteralExpression { Value = optionalInt }
//@[017:00019) | | | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:00020) | | ├─ObjectPropertyExpression
//@[004:00015) | | | ├─StringLiteralExpression { Value = optionalObj }
//@[017:00020) | | | └─ObjectExpression
    optionalArray: [ ]
//@[004:00022) | | └─ObjectPropertyExpression
//@[004:00017) | |   ├─StringLiteralExpression { Value = optionalArray }
//@[019:00022) | |   └─ArrayExpression
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00241) ├─DeclaredResourceExpression
//@[079:00241) | ├─ObjectExpression
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@[002:00077) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00077) | |   └─ObjectExpression
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[004:00055) | |     └─ObjectPropertyExpression
//@[004:00011) | |       ├─StringLiteralExpression { Value = modADep }
//@[013:00055) | |       └─ModuleOutputPropertyAccessExpression { PropertyName = outputObj }
//@[013:00045) | |         └─PropertyAccessExpression { PropertyName = outputs }
//@[013:00037) | |           └─ModuleReferenceExpression
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@[000:00060) ├─DeclaredOutputExpression { Name = stringOutputA }
//@[021:00027) | ├─AmbientTypeReferenceExpression { Name = string }
//@[030:00060) | └─ModuleOutputPropertyAccessExpression { PropertyName = stringOutputA }
//@[030:00046) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[030:00038) |     └─ModuleReferenceExpression
output stringOutputB string = modATest.outputs.stringOutputB
//@[000:00060) ├─DeclaredOutputExpression { Name = stringOutputB }
//@[021:00027) | ├─AmbientTypeReferenceExpression { Name = string }
//@[030:00060) | └─ModuleOutputPropertyAccessExpression { PropertyName = stringOutputB }
//@[030:00046) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[030:00038) |     └─ModuleReferenceExpression
output objOutput object = modATest.outputs.objOutput
//@[000:00052) ├─DeclaredOutputExpression { Name = objOutput }
//@[017:00023) | ├─AmbientTypeReferenceExpression { Name = object }
//@[026:00052) | └─ModuleOutputPropertyAccessExpression { PropertyName = objOutput }
//@[026:00042) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[026:00034) |     └─ModuleReferenceExpression
output arrayOutput array = modATest.outputs.arrayOutput
//@[000:00055) ├─DeclaredOutputExpression { Name = arrayOutput }
//@[019:00024) | ├─AmbientTypeReferenceExpression { Name = array }
//@[027:00055) | └─ModuleOutputPropertyAccessExpression { PropertyName = arrayOutput }
//@[027:00043) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[027:00035) |     └─ModuleReferenceExpression
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[000:00082) ├─DeclaredOutputExpression { Name = modCalculatedNameOutput }
//@[031:00037) | ├─AmbientTypeReferenceExpression { Name = object }
//@[040:00082) | └─ModuleOutputPropertyAccessExpression { PropertyName = outputObj }
//@[040:00072) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[040:00064) |     └─ModuleReferenceExpression

/*
  valid loop cases
*/

@sys.description('this is myModules')
//@[000:00162) ├─DeclaredVariableExpression { Name = myModules }
//@[017:00036) | ├─StringLiteralExpression { Value = this is myModules }
var myModules = [
//@[016:00123) | └─ArrayExpression
  {
//@[002:00050) |   ├─ObjectExpression
    name: 'one'
//@[004:00015) |   | ├─ObjectPropertyExpression
//@[004:00008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:00015) |   | | └─StringLiteralExpression { Value = one }
    location: 'eastus2'
//@[004:00023) |   | └─ObjectPropertyExpression
//@[004:00012) |   |   ├─StringLiteralExpression { Value = location }
//@[014:00023) |   |   └─StringLiteralExpression { Value = eastus2 }
  }
  {
//@[002:00049) |   └─ObjectExpression
    name: 'two'
//@[004:00015) |     ├─ObjectPropertyExpression
//@[004:00008) |     | ├─StringLiteralExpression { Value = name }
//@[010:00015) |     | └─StringLiteralExpression { Value = two }
    location: 'westus'
//@[004:00022) |     └─ObjectPropertyExpression
//@[004:00012) |       ├─StringLiteralExpression { Value = location }
//@[014:00022) |       └─StringLiteralExpression { Value = westus }
  }
]

var emptyArray = []
//@[000:00019) ├─DeclaredVariableExpression { Name = emptyArray }
//@[017:00019) | └─ArrayExpression

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[000:00189) ├─DeclaredModuleExpression
//@[042:00189) | ├─ForLoopExpression
//@[057:00066) | | ├─VariableReferenceExpression { Variable = myModules }
//@[068:00188) | | └─ObjectExpression
//@[057:00066) | |         └─VariableReferenceExpression { Variable = myModules }
//@[057:00066) |   |   └─VariableReferenceExpression { Variable = myModules }
//@[057:00066) |         └─VariableReferenceExpression { Variable = myModules }
  name: module.name
//@[002:00019) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00019) | |     └─PropertyAccessExpression { PropertyName = name }
//@[008:00014) | |       └─ArrayAccessExpression
//@[008:00014) | |         ├─CopyIndexExpression
  params: {
//@[010:00093) | └─ObjectExpression
    arrayParam: []
//@[004:00018) |   ├─ObjectPropertyExpression
//@[004:00014) |   | ├─StringLiteralExpression { Value = arrayParam }
//@[016:00018) |   | └─ArrayExpression
    objParam: module
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00020) |   | └─ArrayAccessExpression
//@[014:00020) |   |   ├─CopyIndexExpression
    stringParamB: module.location
//@[004:00033) |   └─ObjectPropertyExpression
//@[004:00016) |     ├─StringLiteralExpression { Value = stringParamB }
//@[018:00033) |     └─PropertyAccessExpression { PropertyName = location }
//@[018:00024) |       └─ArrayAccessExpression
//@[018:00024) |         ├─CopyIndexExpression
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[000:00256) ├─DeclaredModuleExpression
//@[051:00256) | ├─ForLoopExpression
//@[071:00080) | | ├─VariableReferenceExpression { Variable = myModules }
//@[082:00255) | | └─ObjectExpression
//@[071:00080) | |         └─VariableReferenceExpression { Variable = myModules }
//@[071:00080) |   |   └─VariableReferenceExpression { Variable = myModules }
//@[071:00080) |   |     └─VariableReferenceExpression { Variable = myModules }
  name: module.name
//@[002:00019) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00019) | |     └─PropertyAccessExpression { PropertyName = name }
//@[008:00014) | |       └─ArrayAccessExpression
//@[008:00014) | |         ├─CopyIndexExpression
  params: {
//@[010:00146) | └─ObjectExpression
    arrayParam: [
//@[004:00037) |   ├─ObjectPropertyExpression
//@[004:00014) |   | ├─StringLiteralExpression { Value = arrayParam }
//@[016:00037) |   | └─ArrayExpression
      i + 1
//@[006:00011) |   |   └─BinaryExpression { Operator = Add }
//@[006:00007) |   |     ├─CopyIndexExpression
//@[010:00011) |   |     └─IntegerLiteralExpression { Value = 1 }
    ]
    objParam: module
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00020) |   | └─ArrayAccessExpression
//@[014:00020) |   |   ├─CopyIndexExpression
    stringParamB: module.location
//@[004:00033) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00033) |   | └─PropertyAccessExpression { PropertyName = location }
//@[018:00024) |   |   └─ArrayAccessExpression
//@[018:00024) |   |     ├─CopyIndexExpression
    stringParamA: concat('a', i)
//@[004:00032) |   └─ObjectPropertyExpression
//@[004:00016) |     ├─StringLiteralExpression { Value = stringParamA }
//@[018:00032) |     └─FunctionCallExpression { Name = concat }
//@[025:00028) |       ├─StringLiteralExpression { Value = a }
//@[030:00031) |       └─CopyIndexExpression
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[000:00246) ├─DeclaredModuleExpression
//@[042:00246) | ├─ForLoopExpression
//@[057:00066) | | ├─VariableReferenceExpression { Variable = myModules }
//@[068:00245) | | └─ObjectExpression
//@[057:00066) | |         └─VariableReferenceExpression { Variable = myModules }
//@[057:00066) |   |         └─VariableReferenceExpression { Variable = myModules }
//@[057:00066) |   |   └─VariableReferenceExpression { Variable = myModules }
//@[057:00066) |         └─VariableReferenceExpression { Variable = myModules }
  name: module.name
//@[002:00019) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00019) | |     └─PropertyAccessExpression { PropertyName = name }
//@[008:00014) | |       └─ArrayAccessExpression
//@[008:00014) | |         ├─CopyIndexExpression
  params: {
//@[010:00150) | └─ObjectExpression
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[004:00075) |   ├─ObjectPropertyExpression
//@[004:00014) |   | ├─StringLiteralExpression { Value = arrayParam }
//@[016:00075) |   | └─ForLoopExpression
//@[026:00036) |   |   ├─FunctionCallExpression { Name = range }
//@[032:00033) |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[034:00035) |   |   | └─IntegerLiteralExpression { Value = 3 }
//@[038:00074) |   |   └─FunctionCallExpression { Name = concat }
//@[045:00052) |   |     ├─StringLiteralExpression { Value = test- }
//@[054:00055) |   |     ├─ArrayAccessExpression
//@[054:00055) |   |     | ├─CopyIndexExpression
//@[026:00036) |   |     | └─FunctionCallExpression { Name = range }
//@[032:00033) |   |     |   ├─IntegerLiteralExpression { Value = 0 }
//@[034:00035) |   |     |   └─IntegerLiteralExpression { Value = 3 }
//@[057:00060) |   |     ├─StringLiteralExpression { Value = - }
//@[062:00073) |   |     └─PropertyAccessExpression { PropertyName = name }
//@[062:00068) |   |       └─ArrayAccessExpression
//@[062:00068) |   |         ├─CopyIndexExpression
    objParam: module
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00020) |   | └─ArrayAccessExpression
//@[014:00020) |   |   ├─CopyIndexExpression
    stringParamB: module.location
//@[004:00033) |   └─ObjectPropertyExpression
//@[004:00016) |     ├─StringLiteralExpression { Value = stringParamB }
//@[018:00033) |     └─PropertyAccessExpression { PropertyName = location }
//@[018:00024) |       └─ArrayAccessExpression
//@[018:00024) |         ├─CopyIndexExpression
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:00234) ├─DeclaredModuleExpression
//@[056:00234) | ├─ForLoopExpression
//@[066:00076) | | ├─VariableReferenceExpression { Variable = emptyArray }
//@[077:00233) | | └─ObjectExpression
//@[066:00076) | |         └─VariableReferenceExpression { Variable = emptyArray }
  name: 'hello-${x}'
//@[002:00020) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00020) | |     └─InterpolatedStringExpression
//@[017:00018) | |       └─ArrayAccessExpression
//@[017:00018) | |         ├─CopyIndexExpression
  params: {
//@[010:00128) | └─ObjectExpression
    objParam: {}
//@[004:00016) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00016) |   | └─ObjectExpression
    stringParamA: 'test'
//@[004:00024) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamA }
//@[018:00024) |   | └─StringLiteralExpression { Value = test }
    stringParamB: 'test'
//@[004:00024) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00024) |   | └─StringLiteralExpression { Value = test }
    arrayParam: [for x in emptyArray: x]
//@[004:00040) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00040) |     └─ForLoopExpression
//@[026:00036) |       ├─VariableReferenceExpression { Variable = emptyArray }
//@[038:00039) |       └─ArrayAccessExpression
//@[038:00039) |         ├─CopyIndexExpression
//@[026:00036) |         └─VariableReferenceExpression { Variable = emptyArray }
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[000:00035) ├─DeclaredVariableExpression { Name = duplicateAcrossScopes }
//@[028:00035) | └─StringLiteralExpression { Value = hello }
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[000:00264) ├─DeclaredModuleExpression
//@[053:00264) | ├─ForLoopExpression
//@[083:00085) | | ├─ArrayExpression
//@[087:00263) | | └─ObjectExpression
//@[083:00085) | |         └─ArrayExpression
  name: 'hello-${duplicateAcrossScopes}'
//@[002:00040) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00040) | |     └─InterpolatedStringExpression
//@[017:00038) | |       └─ArrayAccessExpression
//@[017:00038) | |         ├─CopyIndexExpression
  params: {
//@[010:00128) | └─ObjectExpression
    objParam: {}
//@[004:00016) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00016) |   | └─ObjectExpression
    stringParamA: 'test'
//@[004:00024) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamA }
//@[018:00024) |   | └─StringLiteralExpression { Value = test }
    stringParamB: 'test'
//@[004:00024) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00024) |   | └─StringLiteralExpression { Value = test }
    arrayParam: [for x in emptyArray: x]
//@[004:00040) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00040) |     └─ForLoopExpression
//@[026:00036) |       ├─VariableReferenceExpression { Variable = emptyArray }
//@[038:00039) |       └─ArrayAccessExpression
//@[038:00039) |         ├─CopyIndexExpression
//@[026:00036) |         └─VariableReferenceExpression { Variable = emptyArray }
  }
}]

var someDuplicate = true
//@[000:00024) ├─DeclaredVariableExpression { Name = someDuplicate }
//@[020:00024) | └─BooleanLiteralExpression { Value = True }
var otherDuplicate = false
//@[000:00026) ├─DeclaredVariableExpression { Name = otherDuplicate }
//@[021:00026) | └─BooleanLiteralExpression { Value = False }
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[000:00263) ├─DeclaredModuleExpression
//@[046:00263) | ├─ForLoopExpression
//@[068:00070) | | ├─ArrayExpression
//@[072:00262) | | └─ObjectExpression
//@[068:00070) | |         └─ArrayExpression
//@[068:00070) |         | └─ArrayExpression
  name: 'hello-${someDuplicate}'
//@[002:00032) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00032) | |     └─InterpolatedStringExpression
//@[017:00030) | |       └─ArrayAccessExpression
//@[017:00030) | |         ├─CopyIndexExpression
  params: {
//@[010:00150) | └─ObjectExpression
    objParam: {}
//@[004:00016) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00016) |   | └─ObjectExpression
    stringParamB: 'test'
//@[004:00024) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00024) |   | └─StringLiteralExpression { Value = test }
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[004:00088) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00088) |     └─ForLoopExpression
//@[039:00049) |       ├─VariableReferenceExpression { Variable = emptyArray }
//@[051:00087) |       └─InterpolatedStringExpression
//@[054:00067) |         ├─ArrayAccessExpression
//@[054:00067) |         | ├─CopyIndexExpression
//@[071:00085) |         └─ArrayAccessExpression
//@[071:00085) |           ├─CopyIndexExpression
//@[039:00049) |           └─VariableReferenceExpression { Variable = emptyArray }
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[000:00438) ├─DeclaredModuleExpression
//@[058:00438) | ├─ObjectExpression
  name: 'propertyLoopInsideParameterValue'
//@[002:00042) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00042) | |   └─StringLiteralExpression { Value = propertyLoopInsideParameterValue }
  params: {
//@[010:00330) | └─ObjectExpression
    objParam: {
//@[004:00209) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00209) |   | └─ObjectExpression
      a: [for i in range(0,10): i]
//@[006:00034) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = a }
//@[009:00034) |   |   | └─ForLoopExpression
//@[019:00030) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[027:00029) |   |   |   | └─IntegerLiteralExpression { Value = 10 }
//@[032:00033) |   |   |   └─ArrayAccessExpression
//@[032:00033) |   |   |     ├─CopyIndexExpression
//@[019:00030) |   |   |     └─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |       ├─IntegerLiteralExpression { Value = 0 }
//@[027:00029) |   |   |       └─IntegerLiteralExpression { Value = 10 }
      b: [for i in range(1,2): i]
//@[006:00033) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = b }
//@[009:00033) |   |   | └─ForLoopExpression
//@[019:00029) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |   | ├─IntegerLiteralExpression { Value = 1 }
//@[027:00028) |   |   |   | └─IntegerLiteralExpression { Value = 2 }
//@[031:00032) |   |   |   └─ArrayAccessExpression
//@[031:00032) |   |   |     ├─CopyIndexExpression
//@[019:00029) |   |   |     └─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |       ├─IntegerLiteralExpression { Value = 1 }
//@[027:00028) |   |   |       └─IntegerLiteralExpression { Value = 2 }
      c: {
//@[006:00056) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = c }
//@[009:00056) |   |   | └─ObjectExpression
        d: [for j in range(2,3): j]
//@[008:00035) |   |   |   └─ObjectPropertyExpression
//@[008:00009) |   |   |     ├─StringLiteralExpression { Value = d }
//@[011:00035) |   |   |     └─ForLoopExpression
//@[021:00031) |   |   |       ├─FunctionCallExpression { Name = range }
//@[027:00028) |   |   |       | ├─IntegerLiteralExpression { Value = 2 }
//@[029:00030) |   |   |       | └─IntegerLiteralExpression { Value = 3 }
//@[033:00034) |   |   |       └─ArrayAccessExpression
//@[033:00034) |   |   |         ├─CopyIndexExpression
//@[021:00031) |   |   |         └─FunctionCallExpression { Name = range }
//@[027:00028) |   |   |           ├─IntegerLiteralExpression { Value = 2 }
//@[029:00030) |   |   |           └─IntegerLiteralExpression { Value = 3 }
      }
      e: [for k in range(4,4): {
//@[006:00056) |   |   └─ObjectPropertyExpression
//@[006:00007) |   |     ├─StringLiteralExpression { Value = e }
//@[009:00056) |   |     └─ForLoopExpression
//@[019:00029) |   |       ├─FunctionCallExpression { Name = range }
//@[025:00026) |   |       | ├─IntegerLiteralExpression { Value = 4 }
//@[027:00028) |   |       | └─IntegerLiteralExpression { Value = 4 }
//@[031:00055) |   |       └─ObjectExpression
//@[019:00029) |   |             └─FunctionCallExpression { Name = range }
//@[025:00026) |   |               ├─IntegerLiteralExpression { Value = 4 }
//@[027:00028) |   |               └─IntegerLiteralExpression { Value = 4 }
        f: k
//@[008:00012) |   |         └─ObjectPropertyExpression
//@[008:00009) |   |           ├─StringLiteralExpression { Value = f }
//@[011:00012) |   |           └─ArrayAccessExpression
//@[011:00012) |   |             ├─CopyIndexExpression
      }]
    }
    stringParamB: ''
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00020) |   | └─StringLiteralExpression { Value =  }
    arrayParam: [
//@[004:00079) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00079) |     └─ArrayExpression
      {
//@[006:00053) |       └─ObjectExpression
        e: [for j in range(7,7): j]
//@[008:00035) |         └─ObjectPropertyExpression
//@[008:00009) |           ├─StringLiteralExpression { Value = e }
//@[011:00035) |           └─ForLoopExpression
//@[021:00031) |             ├─FunctionCallExpression { Name = range }
//@[027:00028) |             | ├─IntegerLiteralExpression { Value = 7 }
//@[029:00030) |             | └─IntegerLiteralExpression { Value = 7 }
//@[033:00034) |             └─ArrayAccessExpression
//@[033:00034) |               ├─CopyIndexExpression
//@[021:00031) |               └─FunctionCallExpression { Name = range }
//@[027:00028) |                 ├─IntegerLiteralExpression { Value = 7 }
//@[029:00030) |                 └─IntegerLiteralExpression { Value = 7 }
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[000:00514) ├─DeclaredModuleExpression
//@[069:00514) | ├─ObjectExpression
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[002:00053) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00053) | |   └─StringLiteralExpression { Value = propertyLoopInsideParameterValueWithIndexes }
  params: {
//@[010:00384) | └─ObjectExpression
    objParam: {
//@[004:00263) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00263) |   | └─ObjectExpression
      a: [for (i, i2) in range(0,10): i + i2]
//@[006:00045) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = a }
//@[009:00045) |   |   | └─ForLoopExpression
//@[025:00036) |   |   |   ├─FunctionCallExpression { Name = range }
//@[031:00032) |   |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[033:00035) |   |   |   | └─IntegerLiteralExpression { Value = 10 }
//@[038:00044) |   |   |   └─BinaryExpression { Operator = Add }
//@[038:00039) |   |   |     ├─ArrayAccessExpression
//@[038:00039) |   |   |     | ├─CopyIndexExpression
//@[025:00036) |   |   |     | └─FunctionCallExpression { Name = range }
//@[031:00032) |   |   |     |   ├─IntegerLiteralExpression { Value = 0 }
//@[033:00035) |   |   |     |   └─IntegerLiteralExpression { Value = 10 }
//@[042:00044) |   |   |     └─CopyIndexExpression
      b: [for (i, i2) in range(1,2): i / i2]
//@[006:00044) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = b }
//@[009:00044) |   |   | └─ForLoopExpression
//@[025:00035) |   |   |   ├─FunctionCallExpression { Name = range }
//@[031:00032) |   |   |   | ├─IntegerLiteralExpression { Value = 1 }
//@[033:00034) |   |   |   | └─IntegerLiteralExpression { Value = 2 }
//@[037:00043) |   |   |   └─BinaryExpression { Operator = Divide }
//@[037:00038) |   |   |     ├─ArrayAccessExpression
//@[037:00038) |   |   |     | ├─CopyIndexExpression
//@[025:00035) |   |   |     | └─FunctionCallExpression { Name = range }
//@[031:00032) |   |   |     |   ├─IntegerLiteralExpression { Value = 1 }
//@[033:00034) |   |   |     |   └─IntegerLiteralExpression { Value = 2 }
//@[041:00043) |   |   |     └─CopyIndexExpression
      c: {
//@[006:00067) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = c }
//@[009:00067) |   |   | └─ObjectExpression
        d: [for (j, j2) in range(2,3): j * j2]
//@[008:00046) |   |   |   └─ObjectPropertyExpression
//@[008:00009) |   |   |     ├─StringLiteralExpression { Value = d }
//@[011:00046) |   |   |     └─ForLoopExpression
//@[027:00037) |   |   |       ├─FunctionCallExpression { Name = range }
//@[033:00034) |   |   |       | ├─IntegerLiteralExpression { Value = 2 }
//@[035:00036) |   |   |       | └─IntegerLiteralExpression { Value = 3 }
//@[039:00045) |   |   |       └─BinaryExpression { Operator = Multiply }
//@[039:00040) |   |   |         ├─ArrayAccessExpression
//@[039:00040) |   |   |         | ├─CopyIndexExpression
//@[027:00037) |   |   |         | └─FunctionCallExpression { Name = range }
//@[033:00034) |   |   |         |   ├─IntegerLiteralExpression { Value = 2 }
//@[035:00036) |   |   |         |   └─IntegerLiteralExpression { Value = 3 }
//@[043:00045) |   |   |         └─CopyIndexExpression
      }
      e: [for (k, k2) in range(4,4): {
//@[006:00077) |   |   └─ObjectPropertyExpression
//@[006:00007) |   |     ├─StringLiteralExpression { Value = e }
//@[009:00077) |   |     └─ForLoopExpression
//@[025:00035) |   |       ├─FunctionCallExpression { Name = range }
//@[031:00032) |   |       | ├─IntegerLiteralExpression { Value = 4 }
//@[033:00034) |   |       | └─IntegerLiteralExpression { Value = 4 }
//@[037:00076) |   |       └─ObjectExpression
//@[025:00035) |   |         |   └─FunctionCallExpression { Name = range }
//@[031:00032) |   |         |     ├─IntegerLiteralExpression { Value = 4 }
//@[033:00034) |   |         |     └─IntegerLiteralExpression { Value = 4 }
        f: k
//@[008:00012) |   |         ├─ObjectPropertyExpression
//@[008:00009) |   |         | ├─StringLiteralExpression { Value = f }
//@[011:00012) |   |         | └─ArrayAccessExpression
//@[011:00012) |   |         |   ├─CopyIndexExpression
        g: k2
//@[008:00013) |   |         └─ObjectPropertyExpression
//@[008:00009) |   |           ├─StringLiteralExpression { Value = g }
//@[011:00013) |   |           └─CopyIndexExpression
      }]
    }
    stringParamB: ''
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00020) |   | └─StringLiteralExpression { Value =  }
    arrayParam: [
//@[004:00079) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00079) |     └─ArrayExpression
      {
//@[006:00053) |       └─ObjectExpression
        e: [for j in range(7,7): j]
//@[008:00035) |         └─ObjectPropertyExpression
//@[008:00009) |           ├─StringLiteralExpression { Value = e }
//@[011:00035) |           └─ForLoopExpression
//@[021:00031) |             ├─FunctionCallExpression { Name = range }
//@[027:00028) |             | ├─IntegerLiteralExpression { Value = 7 }
//@[029:00030) |             | └─IntegerLiteralExpression { Value = 7 }
//@[033:00034) |             └─ArrayAccessExpression
//@[033:00034) |               ├─CopyIndexExpression
//@[021:00031) |               └─FunctionCallExpression { Name = range }
//@[027:00028) |                 ├─IntegerLiteralExpression { Value = 7 }
//@[029:00030) |                 └─IntegerLiteralExpression { Value = 7 }
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[000:00535) ├─DeclaredModuleExpression
//@[074:00535) | ├─ForLoopExpression
//@[088:00098) | | ├─FunctionCallExpression { Name = range }
//@[094:00095) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[096:00097) | | | └─IntegerLiteralExpression { Value = 1 }
//@[100:00534) | | └─ObjectExpression
//@[088:00098) |   |   |       └─FunctionCallExpression { Name = range }
//@[094:00095) |   |   |         ├─IntegerLiteralExpression { Value = 0 }
//@[096:00097) |   |   |         └─IntegerLiteralExpression { Value = 1 }
//@[088:00098) |   |   |       └─FunctionCallExpression { Name = range }
//@[094:00095) |   |   |         ├─IntegerLiteralExpression { Value = 0 }
//@[096:00097) |   |   |         └─IntegerLiteralExpression { Value = 1 }
//@[088:00098) |   |               └─FunctionCallExpression { Name = range }
//@[094:00095) |   |                 ├─IntegerLiteralExpression { Value = 0 }
//@[096:00097) |   |                 └─IntegerLiteralExpression { Value = 1 }
//@[088:00098) |                 | └─FunctionCallExpression { Name = range }
//@[094:00095) |                 |   ├─IntegerLiteralExpression { Value = 0 }
//@[096:00097) |                 |   └─IntegerLiteralExpression { Value = 1 }
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[002:00058) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00058) | |     └─StringLiteralExpression { Value = propertyLoopInsideParameterValueInsideModuleLoop }
  params: {
//@[010:00368) | └─ObjectExpression
    objParam: {
//@[004:00233) |   ├─ObjectPropertyExpression
//@[004:00012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:00233) |   | └─ObjectExpression
      a: [for i in range(0,10): i + thing]
//@[006:00042) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = a }
//@[009:00042) |   |   | └─ForLoopExpression
//@[019:00030) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[027:00029) |   |   |   | └─IntegerLiteralExpression { Value = 10 }
//@[032:00041) |   |   |   └─BinaryExpression { Operator = Add }
//@[032:00033) |   |   |     ├─ArrayAccessExpression
//@[032:00033) |   |   |     | ├─CopyIndexExpression
//@[019:00030) |   |   |     | └─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |     |   ├─IntegerLiteralExpression { Value = 0 }
//@[027:00029) |   |   |     |   └─IntegerLiteralExpression { Value = 10 }
//@[036:00041) |   |   |     └─ArrayAccessExpression
//@[036:00041) |   |   |       ├─CopyIndexExpression
      b: [for i in range(1,2): i * thing]
//@[006:00041) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = b }
//@[009:00041) |   |   | └─ForLoopExpression
//@[019:00029) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |   | ├─IntegerLiteralExpression { Value = 1 }
//@[027:00028) |   |   |   | └─IntegerLiteralExpression { Value = 2 }
//@[031:00040) |   |   |   └─BinaryExpression { Operator = Multiply }
//@[031:00032) |   |   |     ├─ArrayAccessExpression
//@[031:00032) |   |   |     | ├─CopyIndexExpression
//@[019:00029) |   |   |     | └─FunctionCallExpression { Name = range }
//@[025:00026) |   |   |     |   ├─IntegerLiteralExpression { Value = 1 }
//@[027:00028) |   |   |     |   └─IntegerLiteralExpression { Value = 2 }
//@[035:00040) |   |   |     └─ArrayAccessExpression
//@[035:00040) |   |   |       ├─CopyIndexExpression
      c: {
//@[006:00056) |   |   ├─ObjectPropertyExpression
//@[006:00007) |   |   | ├─StringLiteralExpression { Value = c }
//@[009:00056) |   |   | └─ObjectExpression
        d: [for j in range(2,3): j]
//@[008:00035) |   |   |   └─ObjectPropertyExpression
//@[008:00009) |   |   |     ├─StringLiteralExpression { Value = d }
//@[011:00035) |   |   |     └─ForLoopExpression
//@[021:00031) |   |   |       ├─FunctionCallExpression { Name = range }
//@[027:00028) |   |   |       | ├─IntegerLiteralExpression { Value = 2 }
//@[029:00030) |   |   |       | └─IntegerLiteralExpression { Value = 3 }
//@[033:00034) |   |   |       └─ArrayAccessExpression
//@[033:00034) |   |   |         ├─CopyIndexExpression
//@[021:00031) |   |   |         └─FunctionCallExpression { Name = range }
//@[027:00028) |   |   |           ├─IntegerLiteralExpression { Value = 2 }
//@[029:00030) |   |   |           └─IntegerLiteralExpression { Value = 3 }
      }
      e: [for k in range(4,4): {
//@[006:00064) |   |   └─ObjectPropertyExpression
//@[006:00007) |   |     ├─StringLiteralExpression { Value = e }
//@[009:00064) |   |     └─ForLoopExpression
//@[019:00029) |   |       ├─FunctionCallExpression { Name = range }
//@[025:00026) |   |       | ├─IntegerLiteralExpression { Value = 4 }
//@[027:00028) |   |       | └─IntegerLiteralExpression { Value = 4 }
//@[031:00063) |   |       └─ObjectExpression
//@[019:00029) |   |             | └─FunctionCallExpression { Name = range }
//@[025:00026) |   |             |   ├─IntegerLiteralExpression { Value = 4 }
//@[027:00028) |   |             |   └─IntegerLiteralExpression { Value = 4 }
        f: k - thing
//@[008:00020) |   |         └─ObjectPropertyExpression
//@[008:00009) |   |           ├─StringLiteralExpression { Value = f }
//@[011:00020) |   |           └─BinaryExpression { Operator = Subtract }
//@[011:00012) |   |             ├─ArrayAccessExpression
//@[011:00012) |   |             | ├─CopyIndexExpression
//@[015:00020) |   |             └─ArrayAccessExpression
//@[015:00020) |   |               ├─CopyIndexExpression
      }]
    }
    stringParamB: ''
//@[004:00020) |   ├─ObjectPropertyExpression
//@[004:00016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:00020) |   | └─StringLiteralExpression { Value =  }
    arrayParam: [
//@[004:00093) |   └─ObjectPropertyExpression
//@[004:00014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:00093) |     └─ArrayExpression
      {
//@[006:00067) |       └─ObjectExpression
        e: [for j in range(7,7): j % (thing + 1)]
//@[008:00049) |         └─ObjectPropertyExpression
//@[008:00009) |           ├─StringLiteralExpression { Value = e }
//@[011:00049) |           └─ForLoopExpression
//@[021:00031) |             ├─FunctionCallExpression { Name = range }
//@[027:00028) |             | ├─IntegerLiteralExpression { Value = 7 }
//@[029:00030) |             | └─IntegerLiteralExpression { Value = 7 }
//@[033:00048) |             └─BinaryExpression { Operator = Modulo }
//@[033:00034) |               ├─ArrayAccessExpression
//@[033:00034) |               | ├─CopyIndexExpression
//@[021:00031) |               | └─FunctionCallExpression { Name = range }
//@[027:00028) |               |   ├─IntegerLiteralExpression { Value = 7 }
//@[029:00030) |               |   └─IntegerLiteralExpression { Value = 7 }
//@[038:00047) |               └─BinaryExpression { Operator = Add }
//@[038:00043) |                 ├─ArrayAccessExpression
//@[038:00043) |                 | ├─CopyIndexExpression
//@[046:00047) |                 └─IntegerLiteralExpression { Value = 1 }
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:00090) ├─DeclaredResourceExpression
//@[062:00090) | └─ObjectExpression
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@[000:00213) ├─DeclaredModuleExpression
//@[050:00213) | ├─ObjectExpression
  name: 'secureModule1'
//@[002:00023) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00023) | |   └─StringLiteralExpression { Value = secureModule1 }
  params: {
//@[010:00132) | └─ObjectExpression
    secureStringParam1: kv.getSecret('mySecret')
//@[004:00048) |   ├─ObjectPropertyExpression
//@[004:00022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:00048) |   | └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:00026) |   |   ├─ResourceReferenceExpression
//@[037:00047) |   |   └─StringLiteralExpression { Value = mySecret }
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
//@[004:00064) |   └─ObjectPropertyExpression
//@[004:00022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:00064) |     └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:00026) |       ├─ResourceReferenceExpression
//@[037:00047) |       ├─StringLiteralExpression { Value = mySecret }
//@[048:00063) |       └─StringLiteralExpression { Value = secretVersion }
  }
}

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:00134) ├─DeclaredResourceExpression
//@[068:00134) | └─ObjectExpression
  name: 'testkeyvault'
  scope: resourceGroup('otherGroup')
}

module secureModule2 'child/secureParams.bicep' = {
//@[000:00225) ├─DeclaredModuleExpression
//@[050:00225) | ├─ObjectExpression
  name: 'secureModule2'
//@[002:00023) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00023) | |   └─StringLiteralExpression { Value = secureModule2 }
  params: {
//@[010:00144) | └─ObjectExpression
    secureStringParam1: scopedKv.getSecret('mySecret')
//@[004:00054) |   ├─ObjectPropertyExpression
//@[004:00022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:00054) |   | └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:00032) |   |   ├─ResourceReferenceExpression
//@[043:00053) |   |   └─StringLiteralExpression { Value = mySecret }
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
//@[004:00070) |   └─ObjectPropertyExpression
//@[004:00022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:00070) |     └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:00032) |       ├─ResourceReferenceExpression
//@[043:00053) |       ├─StringLiteralExpression { Value = mySecret }
//@[054:00069) |       └─StringLiteralExpression { Value = secretVersion }
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
//@[000:00200) ├─DeclaredVariableExpression { Name = vaults }
//@[013:00200) | └─ArrayExpression
  {
//@[002:00089) |   ├─ObjectExpression
    vaultName: 'test-1-kv'
//@[004:00026) |   | ├─ObjectPropertyExpression
//@[004:00013) |   | | ├─StringLiteralExpression { Value = vaultName }
//@[015:00026) |   | | └─StringLiteralExpression { Value = test-1-kv }
    vaultRG: 'test-1-rg'
//@[004:00024) |   | ├─ObjectPropertyExpression
//@[004:00011) |   | | ├─StringLiteralExpression { Value = vaultRG }
//@[013:00024) |   | | └─StringLiteralExpression { Value = test-1-rg }
    vaultSub: 'abcd-efgh'
//@[004:00025) |   | └─ObjectPropertyExpression
//@[004:00012) |   |   ├─StringLiteralExpression { Value = vaultSub }
//@[014:00025) |   |   └─StringLiteralExpression { Value = abcd-efgh }
  }
  {
//@[002:00090) |   └─ObjectExpression
    vaultName: 'test-2-kv'
//@[004:00026) |     ├─ObjectPropertyExpression
//@[004:00013) |     | ├─StringLiteralExpression { Value = vaultName }
//@[015:00026) |     | └─StringLiteralExpression { Value = test-2-kv }
    vaultRG: 'test-2-rg'
//@[004:00024) |     ├─ObjectPropertyExpression
//@[004:00011) |     | ├─StringLiteralExpression { Value = vaultRG }
//@[013:00024) |     | └─StringLiteralExpression { Value = test-2-rg }
    vaultSub: 'ijkl-1adg1'
//@[004:00026) |     └─ObjectPropertyExpression
//@[004:00012) |       ├─StringLiteralExpression { Value = vaultSub }
//@[014:00026) |       └─StringLiteralExpression { Value = ijkl-1adg1 }
  }
]
var secrets = [
//@[000:00132) ├─DeclaredVariableExpression { Name = secrets }
//@[014:00132) | └─ArrayExpression
  {
//@[002:00055) |   ├─ObjectExpression
    name: 'secret01'
//@[004:00020) |   | ├─ObjectPropertyExpression
//@[004:00008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:00020) |   | | └─StringLiteralExpression { Value = secret01 }
    version: 'versionA'
//@[004:00023) |   | └─ObjectPropertyExpression
//@[004:00011) |   |   ├─StringLiteralExpression { Value = version }
//@[013:00023) |   |   └─StringLiteralExpression { Value = versionA }
  }
  {
//@[002:00055) |   └─ObjectExpression
    name: 'secret02'
//@[004:00020) |     ├─ObjectPropertyExpression
//@[004:00008) |     | ├─StringLiteralExpression { Value = name }
//@[010:00020) |     | └─StringLiteralExpression { Value = secret02 }
    version: 'versionB'
//@[004:00023) |     └─ObjectPropertyExpression
//@[004:00011) |       ├─StringLiteralExpression { Value = version }
//@[013:00023) |       └─StringLiteralExpression { Value = versionB }
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
//@[000:00175) ├─DeclaredResourceExpression
//@[068:00175) | └─ForLoopExpression
//@[082:00088) |   ├─VariableReferenceExpression { Variable = vaults }
//@[090:00174) |   └─ObjectExpression
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[000:00278) ├─DeclaredModuleExpression
//@[055:00278) | ├─ForLoopExpression
//@[075:00082) | | ├─VariableReferenceExpression { Variable = secrets }
//@[084:00277) | | └─ObjectExpression
//@[075:00082) |   |       └─VariableReferenceExpression { Variable = secrets }
//@[075:00082) |       |   └─VariableReferenceExpression { Variable = secrets }
//@[075:00082) |           └─VariableReferenceExpression { Variable = secrets }
  name: 'secureModuleLooped-${i}'
//@[002:00033) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00033) | |     └─InterpolatedStringExpression
//@[030:00031) | |       └─CopyIndexExpression
  params: {
//@[010:00152) | └─ObjectExpression
    secureStringParam1: loopedKv[i].getSecret(secret.name)
//@[004:00058) |   ├─ObjectPropertyExpression
//@[004:00022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:00058) |   | └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:00035) |   |   ├─ResourceReferenceExpression
//@[046:00057) |   |   └─PropertyAccessExpression { PropertyName = name }
//@[046:00052) |   |     └─ArrayAccessExpression
//@[046:00052) |   |       ├─CopyIndexExpression
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
//@[004:00074) |   └─ObjectPropertyExpression
//@[004:00022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:00074) |     └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:00035) |       ├─ResourceReferenceExpression
//@[046:00057) |       ├─PropertyAccessExpression { PropertyName = name }
//@[046:00052) |       | └─ArrayAccessExpression
//@[046:00052) |       |   ├─CopyIndexExpression
//@[059:00073) |       └─PropertyAccessExpression { PropertyName = version }
//@[059:00065) |         └─ArrayAccessExpression
//@[059:00065) |           ├─CopyIndexExpression
  }
}]

module secureModuleCondition 'child/secureParams.bicep' = {
//@[000:00285) ├─DeclaredModuleExpression
//@[058:00285) | ├─ObjectExpression
  name: 'secureModuleCondition'
//@[002:00031) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00031) | |   └─StringLiteralExpression { Value = secureModuleCondition }
  params: {
//@[010:00188) | └─ObjectExpression
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
//@[004:00067) |   ├─ObjectPropertyExpression
//@[004:00022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:00067) |   | └─TernaryExpression
//@[024:00028) |   |   ├─BooleanLiteralExpression { Value = True }
//@[031:00055) |   |   ├─ResourceFunctionCallExpression { Name = getSecret }
//@[031:00033) |   |   | ├─ResourceReferenceExpression
//@[044:00054) |   |   | └─StringLiteralExpression { Value = mySecret }
//@[058:00067) |   |   └─StringLiteralExpression { Value = notTrue }
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
//@[004:00101) |   └─ObjectPropertyExpression
//@[004:00022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:00101) |     └─TernaryExpression
//@[024:00028) |       ├─BooleanLiteralExpression { Value = True }
//@[031:00089) |       ├─TernaryExpression
//@[031:00036) |       | ├─BooleanLiteralExpression { Value = False }
//@[039:00046) |       | ├─StringLiteralExpression { Value = false }
//@[049:00089) |       | └─ResourceFunctionCallExpression { Name = getSecret }
//@[049:00051) |       |   ├─ResourceReferenceExpression
//@[062:00072) |       |   ├─StringLiteralExpression { Value = mySecret }
//@[073:00088) |       |   └─StringLiteralExpression { Value = secretVersion }
//@[092:00101) |       └─StringLiteralExpression { Value = notTrue }
  }
}

// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@[000:00070) ├─DeclaredModuleExpression
//@[045:00070) | └─ObjectExpression
  name: 'withSpace'
//@[002:00019) |   └─ObjectPropertyExpression
//@[002:00006) |     ├─StringLiteralExpression { Value = name }
//@[008:00019) |     └─StringLiteralExpression { Value = withSpace }
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[000:00104) ├─DeclaredModuleExpression
//@[074:00104) | └─ObjectExpression
  name: 'childWithSpace'
//@[002:00024) |   └─ObjectPropertyExpression
//@[002:00006) |     ├─StringLiteralExpression { Value = name }
//@[008:00024) |     └─StringLiteralExpression { Value = childWithSpace }
}

// nameof

var nameofModule = nameof(folderWithSpace)
//@[000:00042) ├─DeclaredVariableExpression { Name = nameofModule }
//@[026:00041) | └─StringLiteralExpression { Value = folderWithSpace }
var nameofModuleParam = nameof(secureModuleCondition.outputs.exposedSecureString)
//@[000:00081) ├─DeclaredVariableExpression { Name = nameofModuleParam }
//@[031:00080) | └─StringLiteralExpression { Value = exposedSecureString }

module moduleWithNameof 'modulea.bicep' = {
//@[000:00358) ├─DeclaredModuleExpression
//@[042:00358) | ├─ObjectExpression
  name: 'nameofModule'
//@[002:00022) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00022) | |   └─StringLiteralExpression { Value = nameofModule }
  scope: resourceGroup(nameof(nameofModuleParam))
  params:{
//@[009:00235) | ├─ObjectExpression
    stringParamA: nameof(withSpace)
//@[004:00035) | | ├─ObjectPropertyExpression
//@[004:00016) | | | ├─StringLiteralExpression { Value = stringParamA }
//@[025:00034) | | | └─StringLiteralExpression { Value = withSpace }
    stringParamB: nameof(folderWithSpace)
//@[004:00041) | | ├─ObjectPropertyExpression
//@[004:00016) | | | ├─StringLiteralExpression { Value = stringParamB }
//@[025:00040) | | | └─StringLiteralExpression { Value = folderWithSpace }
    objParam: {
//@[004:00090) | | ├─ObjectPropertyExpression
//@[004:00012) | | | ├─StringLiteralExpression { Value = objParam }
//@[014:00090) | | | └─ObjectExpression
      a: nameof(secureModuleCondition.outputs.exposedSecureString)
//@[006:00066) | | |   └─ObjectPropertyExpression
//@[006:00007) | | |     ├─StringLiteralExpression { Value = a }
//@[016:00065) | | |     └─StringLiteralExpression { Value = exposedSecureString }
    }
    arrayParam: [
//@[004:00046) | | └─ObjectPropertyExpression
//@[004:00014) | |   ├─StringLiteralExpression { Value = arrayParam }
//@[016:00046) | |   └─ArrayExpression
      nameof(vaults)
//@[013:00019) | |     └─StringLiteralExpression { Value = vaults }
    ]
  }
}

module moduleWithNullableOutputs 'child/nullableOutputs.bicep' = {
//@[000:00096) ├─DeclaredModuleExpression
//@[065:00096) | └─ObjectExpression
  name: 'nullableOutputs'
//@[002:00025) |   └─ObjectPropertyExpression
//@[002:00006) |     ├─StringLiteralExpression { Value = name }
//@[008:00025) |     └─StringLiteralExpression { Value = nullableOutputs }
}

output nullableString string? = moduleWithNullableOutputs.outputs.?nullableString
//@[000:00081) ├─DeclaredOutputExpression { Name = nullableString }
//@[022:00029) | ├─NullableTypeExpression { Name = null | string }
//@[022:00028) | | └─AmbientTypeReferenceExpression { Name = string }
//@[032:00081) | └─ModuleOutputPropertyAccessExpression { PropertyName = nullableString }
//@[032:00065) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[032:00057) |     └─ModuleReferenceExpression
output deeplyNestedProperty string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.property
//@[000:00107) ├─DeclaredOutputExpression { Name = deeplyNestedProperty }
//@[028:00035) | ├─NullableTypeExpression { Name = null | string }
//@[028:00034) | | └─AmbientTypeReferenceExpression { Name = string }
//@[038:00107) | └─PropertyAccessExpression { PropertyName = property }
//@[038:00098) |   └─PropertyAccessExpression { PropertyName = nested }
//@[038:00091) |     └─PropertyAccessExpression { PropertyName = deeply }
//@[038:00084) |       └─ModuleOutputPropertyAccessExpression { PropertyName = nullableObj }
//@[038:00071) |         └─PropertyAccessExpression { PropertyName = outputs }
//@[038:00063) |           └─ModuleReferenceExpression
output deeplyNestedArrayItem string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[0]
//@[000:00108) ├─DeclaredOutputExpression { Name = deeplyNestedArrayItem }
//@[029:00036) | ├─NullableTypeExpression { Name = null | string }
//@[029:00035) | | └─AmbientTypeReferenceExpression { Name = string }
//@[039:00108) | └─ArrayAccessExpression
//@[106:00107) |   ├─IntegerLiteralExpression { Value = 0 }
//@[039:00105) |   └─PropertyAccessExpression { PropertyName = array }
//@[039:00099) |     └─PropertyAccessExpression { PropertyName = nested }
//@[039:00092) |       └─PropertyAccessExpression { PropertyName = deeply }
//@[039:00085) |         └─ModuleOutputPropertyAccessExpression { PropertyName = nullableObj }
//@[039:00072) |           └─PropertyAccessExpression { PropertyName = outputs }
//@[039:00064) |             └─ModuleReferenceExpression
output deeplyNestedArrayItemFromEnd string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[^1]
//@[000:00116) ├─DeclaredOutputExpression { Name = deeplyNestedArrayItemFromEnd }
//@[036:00043) | ├─NullableTypeExpression { Name = null | string }
//@[036:00042) | | └─AmbientTypeReferenceExpression { Name = string }
//@[046:00116) | └─ArrayAccessExpression
//@[114:00115) |   ├─IntegerLiteralExpression { Value = 1 }
//@[046:00112) |   └─PropertyAccessExpression { PropertyName = array }
//@[046:00106) |     └─PropertyAccessExpression { PropertyName = nested }
//@[046:00099) |       └─PropertyAccessExpression { PropertyName = deeply }
//@[046:00092) |         └─ModuleOutputPropertyAccessExpression { PropertyName = nullableObj }
//@[046:00079) |           └─PropertyAccessExpression { PropertyName = outputs }
//@[046:00071) |             └─ModuleReferenceExpression
output deeplyNestedArrayItemFromEndAttempt string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[?^1]
//@[000:00124) └─DeclaredOutputExpression { Name = deeplyNestedArrayItemFromEndAttempt }
//@[043:00050)   ├─NullableTypeExpression { Name = null | string }
//@[043:00049)   | └─AmbientTypeReferenceExpression { Name = string }
//@[053:00124)   └─ArrayAccessExpression
//@[122:00123)     ├─IntegerLiteralExpression { Value = 1 }
//@[053:00119)     └─PropertyAccessExpression { PropertyName = array }
//@[053:00113)       └─PropertyAccessExpression { PropertyName = nested }
//@[053:00106)         └─PropertyAccessExpression { PropertyName = deeply }
//@[053:00099)           └─ModuleOutputPropertyAccessExpression { PropertyName = nullableObj }
//@[053:00086)             └─PropertyAccessExpression { PropertyName = outputs }
//@[053:00078)               └─ModuleReferenceExpression

