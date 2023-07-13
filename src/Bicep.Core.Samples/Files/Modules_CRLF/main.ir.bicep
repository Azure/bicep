
//@[000:8881) ProgramExpression
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ModuleReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ModuleReferenceExpression [UNPARENTED]
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
@sys.description('this is deployTimeSuffix param')
//@[000:0093) ├─DeclaredParameterExpression { Name = deployTimeSuffix }
param deployTimeSuffix string = newGuid()
//@[023:0029) | ├─AmbientTypeReferenceExpression { Name = string }
//@[032:0041) | └─FunctionCallExpression { Name = newGuid }

@sys.description('this module a')
//@[000:0252) ├─DeclaredModuleExpression
module modATest './modulea.bicep' = {
//@[036:0217) | ├─ObjectExpression
  name: 'modATest'
//@[002:0018) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0018) | |   └─StringLiteralExpression { Value = modATest }
  params: {
//@[010:0155) | └─ObjectExpression
    stringParamB: 'hello!'
//@[004:0026) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0026) |   | └─StringLiteralExpression { Value = hello! }
    objParam: {
//@[004:0036) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0036) |   | └─ObjectExpression
      a: 'b'
//@[006:0012) |   |   └─ObjectPropertyExpression
//@[006:0007) |   |     ├─StringLiteralExpression { Value = a }
//@[009:0012) |   |     └─StringLiteralExpression { Value = b }
    }
    arrayParam: [
//@[004:0071) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0071) |     └─ArrayExpression
      {
//@[006:0032) |       ├─ObjectExpression
        a: 'b'
//@[008:0014) |       | └─ObjectPropertyExpression
//@[008:0009) |       |   ├─StringLiteralExpression { Value = a }
//@[011:0014) |       |   └─StringLiteralExpression { Value = b }
      }
      'abc'
//@[006:0011) |       └─StringLiteralExpression { Value = abc }
    ]
  }
}


@sys.description('this module b')
//@[000:0136) ├─DeclaredModuleExpression
module modB './child/moduleb.bicep' = {
//@[038:0101) | ├─ObjectExpression
  name: 'modB'
//@[002:0014) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0014) | |   └─StringLiteralExpression { Value = modB }
  params: {
//@[010:0041) | └─ObjectExpression
    location: 'West US'
//@[004:0023) |   └─ObjectPropertyExpression
//@[004:0012) |     ├─StringLiteralExpression { Value = location }
//@[014:0023) |     └─StringLiteralExpression { Value = West US }
  }
}

@sys.description('this is just module b with a condition')
//@[000:0203) ├─DeclaredModuleExpression
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[055:0065) | ├─ConditionExpression
//@[055:0065) | | ├─BinaryExpression { Operator = Equals }
//@[055:0060) | | | ├─BinaryExpression { Operator = Add }
//@[055:0056) | | | | ├─IntegerLiteralExpression { Value = 1 }
//@[059:0060) | | | | └─IntegerLiteralExpression { Value = 1 }
//@[064:0065) | | | └─IntegerLiteralExpression { Value = 2 }
//@[067:0143) | | └─ObjectExpression
  name: 'modBWithCondition'
//@[002:0027) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0027) | |     └─StringLiteralExpression { Value = modBWithCondition }
  params: {
//@[010:0041) | └─ObjectExpression
    location: 'East US'
//@[004:0023) |   └─ObjectPropertyExpression
//@[004:0012) |     ├─StringLiteralExpression { Value = location }
//@[014:0023) |     └─StringLiteralExpression { Value = East US }
  }
}

module modC './child/modulec.json' = {
//@[000:0100) ├─DeclaredModuleExpression
//@[037:0100) | ├─ObjectExpression
  name: 'modC'
//@[002:0014) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0014) | |   └─StringLiteralExpression { Value = modC }
  params: {
//@[010:0041) | └─ObjectExpression
    location: 'West US'
//@[004:0023) |   └─ObjectPropertyExpression
//@[004:0012) |     ├─StringLiteralExpression { Value = location }
//@[014:0023) |     └─StringLiteralExpression { Value = West US }
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[000:0142) ├─DeclaredModuleExpression
//@[054:0064) | ├─ConditionExpression
//@[054:0064) | | ├─BinaryExpression { Operator = Equals }
//@[054:0059) | | | ├─BinaryExpression { Operator = Subtract }
//@[054:0055) | | | | ├─IntegerLiteralExpression { Value = 2 }
//@[058:0059) | | | | └─IntegerLiteralExpression { Value = 1 }
//@[063:0064) | | | └─IntegerLiteralExpression { Value = 1 }
//@[066:0142) | | └─ObjectExpression
  name: 'modCWithCondition'
//@[002:0027) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0027) | |     └─StringLiteralExpression { Value = modCWithCondition }
  params: {
//@[010:0041) | └─ObjectExpression
    location: 'East US'
//@[004:0023) |   └─ObjectPropertyExpression
//@[004:0012) |     ├─StringLiteralExpression { Value = location }
//@[014:0023) |     └─StringLiteralExpression { Value = East US }
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[000:0098) ├─DeclaredModuleExpression
//@[061:0098) | └─ObjectExpression
  name: 'optionalWithNoParams1'
//@[002:0031) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0031) |     └─StringLiteralExpression { Value = optionalWithNoParams1 }
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[000:0116) ├─DeclaredModuleExpression
//@[061:0116) | ├─ObjectExpression
  name: 'optionalWithNoParams2'
//@[002:0031) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0031) | |   └─StringLiteralExpression { Value = optionalWithNoParams2 }
  params: {
//@[010:0016) | └─ObjectExpression
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[000:0210) ├─DeclaredModuleExpression
//@[061:0210) | ├─ObjectExpression
  name: 'optionalWithNoParams3'
//@[002:0031) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0031) | |   └─StringLiteralExpression { Value = optionalWithNoParams3 }
  params: {
//@[010:0110) | └─ObjectExpression
    optionalString: 'abc'
//@[004:0025) |   ├─ObjectPropertyExpression
//@[004:0018) |   | ├─StringLiteralExpression { Value = optionalString }
//@[020:0025) |   | └─StringLiteralExpression { Value = abc }
    optionalInt: 42
//@[004:0019) |   ├─ObjectPropertyExpression
//@[004:0015) |   | ├─StringLiteralExpression { Value = optionalInt }
//@[017:0019) |   | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0015) |   | ├─StringLiteralExpression { Value = optionalObj }
//@[017:0020) |   | └─ObjectExpression
    optionalArray: [ ]
//@[004:0022) |   └─ObjectPropertyExpression
//@[004:0017) |     ├─StringLiteralExpression { Value = optionalArray }
//@[019:0022) |     └─ArrayExpression
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:0233) ├─DeclaredResourceExpression
//@[065:0233) | ├─ObjectExpression
  name: 'harry'
  properties: {
//@[002:0145) | | └─ObjectPropertyExpression
//@[002:0012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:0145) | |   └─ObjectExpression
    modADep: modATest.outputs.stringOutputA
//@[004:0043) | |     ├─ObjectPropertyExpression
//@[004:0011) | |     | ├─StringLiteralExpression { Value = modADep }
//@[013:0043) | |     | └─ModuleOutputPropertyAccessExpression { PropertyName = stringOutputA }
//@[013:0029) | |     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[013:0021) | |     |     └─ModuleReferenceExpression
    modBDep: modB.outputs.myResourceId
//@[004:0038) | |     ├─ObjectPropertyExpression
//@[004:0011) | |     | ├─StringLiteralExpression { Value = modBDep }
//@[013:0038) | |     | └─ModuleOutputPropertyAccessExpression { PropertyName = myResourceId }
//@[013:0025) | |     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[013:0017) | |     |     └─ModuleReferenceExpression
    modCDep: modC.outputs.myResourceId
//@[004:0038) | |     └─ObjectPropertyExpression
//@[004:0011) | |       ├─StringLiteralExpression { Value = modCDep }
//@[013:0038) | |       └─ModuleOutputPropertyAccessExpression { PropertyName = myResourceId }
//@[013:0025) | |         └─PropertyAccessExpression { PropertyName = outputs }
//@[013:0017) | |           └─ModuleReferenceExpression
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[000:0321) ├─DeclaredModuleExpression
//@[080:0321) | ├─ObjectExpression
  name: 'optionalWithAllParamsAndManualDependency'
//@[002:0050) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0050) | |   └─StringLiteralExpression { Value = optionalWithAllParamsAndManualDependency }
  params: {
//@[010:0110) | ├─ObjectExpression
    optionalString: 'abc'
//@[004:0025) | | ├─ObjectPropertyExpression
//@[004:0018) | | | ├─StringLiteralExpression { Value = optionalString }
//@[020:0025) | | | └─StringLiteralExpression { Value = abc }
    optionalInt: 42
//@[004:0019) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = optionalInt }
//@[017:0019) | | | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:0020) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = optionalObj }
//@[017:0020) | | | └─ObjectExpression
    optionalArray: [ ]
//@[004:0022) | | └─ObjectPropertyExpression
//@[004:0017) | |   ├─StringLiteralExpression { Value = optionalArray }
//@[019:0022) | |   └─ArrayExpression
  }
  dependsOn: [
    resWithDependencies
    optionalWithAllParams
  ]
}

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[000:0300) ├─DeclaredModuleExpression
//@[070:0300) | ├─ObjectExpression
  name: 'optionalWithImplicitDependency'
//@[002:0040) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0040) | |   └─StringLiteralExpression { Value = optionalWithImplicitDependency }
  params: {
//@[010:0182) | ├─ObjectExpression
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:0097) | | ├─ObjectPropertyExpression
//@[004:0018) | | | ├─StringLiteralExpression { Value = optionalString }
//@[020:0097) | | | └─FunctionCallExpression { Name = concat }
//@[027:0049) | | |   ├─PropertyAccessExpression { PropertyName = id }
//@[027:0046) | | |   | └─ResourceReferenceExpression
//@[051:0096) | | |   └─PropertyAccessExpression { PropertyName = name }
//@[051:0091) | | |     └─ModuleReferenceExpression
    optionalInt: 42
//@[004:0019) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = optionalInt }
//@[017:0019) | | | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:0020) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = optionalObj }
//@[017:0020) | | | └─ObjectExpression
    optionalArray: [ ]
//@[004:0022) | | └─ObjectPropertyExpression
//@[004:0017) | |   ├─StringLiteralExpression { Value = optionalArray }
//@[019:0022) | |   └─ArrayExpression
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[000:0331) ├─DeclaredModuleExpression
//@[064:0331) | ├─ObjectExpression
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:0077) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0077) | |   └─InterpolatedStringExpression
//@[011:0056) | |     ├─PropertyAccessExpression { PropertyName = name }
//@[011:0051) | |     | └─ModuleReferenceExpression
//@[059:0075) | |     └─ParametersReferenceExpression { Parameter = deployTimeSuffix }
  params: {
//@[010:0182) | ├─ObjectExpression
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:0097) | | ├─ObjectPropertyExpression
//@[004:0018) | | | ├─StringLiteralExpression { Value = optionalString }
//@[020:0097) | | | └─FunctionCallExpression { Name = concat }
//@[027:0049) | | |   ├─PropertyAccessExpression { PropertyName = id }
//@[027:0046) | | |   | └─ResourceReferenceExpression
//@[051:0096) | | |   └─PropertyAccessExpression { PropertyName = name }
//@[051:0091) | | |     └─ModuleReferenceExpression
    optionalInt: 42
//@[004:0019) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = optionalInt }
//@[017:0019) | | | └─IntegerLiteralExpression { Value = 42 }
    optionalObj: { }
//@[004:0020) | | ├─ObjectPropertyExpression
//@[004:0015) | | | ├─StringLiteralExpression { Value = optionalObj }
//@[017:0020) | | | └─ObjectExpression
    optionalArray: [ ]
//@[004:0022) | | └─ObjectPropertyExpression
//@[004:0017) | |   ├─StringLiteralExpression { Value = optionalArray }
//@[019:0022) | |   └─ArrayExpression
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:0241) ├─DeclaredResourceExpression
//@[079:0241) | ├─ObjectExpression
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@[002:0077) | | └─ObjectPropertyExpression
//@[002:0012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:0077) | |   └─ObjectExpression
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[004:0055) | |     └─ObjectPropertyExpression
//@[004:0011) | |       ├─StringLiteralExpression { Value = modADep }
//@[013:0055) | |       └─ModuleOutputPropertyAccessExpression { PropertyName = outputObj }
//@[013:0045) | |         └─PropertyAccessExpression { PropertyName = outputs }
//@[013:0037) | |           └─ModuleReferenceExpression
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@[000:0060) ├─DeclaredOutputExpression { Name = stringOutputA }
//@[021:0027) | ├─AmbientTypeReferenceExpression { Name = string }
//@[030:0060) | └─ModuleOutputPropertyAccessExpression { PropertyName = stringOutputA }
//@[030:0046) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[030:0038) |     └─ModuleReferenceExpression
output stringOutputB string = modATest.outputs.stringOutputB
//@[000:0060) ├─DeclaredOutputExpression { Name = stringOutputB }
//@[021:0027) | ├─AmbientTypeReferenceExpression { Name = string }
//@[030:0060) | └─ModuleOutputPropertyAccessExpression { PropertyName = stringOutputB }
//@[030:0046) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[030:0038) |     └─ModuleReferenceExpression
output objOutput object = modATest.outputs.objOutput
//@[000:0052) ├─DeclaredOutputExpression { Name = objOutput }
//@[017:0023) | ├─AmbientTypeReferenceExpression { Name = object }
//@[026:0052) | └─ModuleOutputPropertyAccessExpression { PropertyName = objOutput }
//@[026:0042) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[026:0034) |     └─ModuleReferenceExpression
output arrayOutput array = modATest.outputs.arrayOutput
//@[000:0055) ├─DeclaredOutputExpression { Name = arrayOutput }
//@[019:0024) | ├─AmbientTypeReferenceExpression { Name = array }
//@[027:0055) | └─ModuleOutputPropertyAccessExpression { PropertyName = arrayOutput }
//@[027:0043) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[027:0035) |     └─ModuleReferenceExpression
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[000:0082) └─DeclaredOutputExpression { Name = modCalculatedNameOutput }
//@[031:0037)   ├─AmbientTypeReferenceExpression { Name = object }
//@[040:0082)   └─ModuleOutputPropertyAccessExpression { PropertyName = outputObj }
//@[040:0072)     └─PropertyAccessExpression { PropertyName = outputs }
//@[040:0064)       └─ModuleReferenceExpression

/*
  valid loop cases
*/

@sys.description('this is myModules')
//@[000:0162) ├─DeclaredVariableExpression { Name = myModules }
var myModules = [
//@[016:0123) | └─ArrayExpression
  {
//@[002:0050) |   ├─ObjectExpression
    name: 'one'
//@[004:0015) |   | ├─ObjectPropertyExpression
//@[004:0008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:0015) |   | | └─StringLiteralExpression { Value = one }
    location: 'eastus2'
//@[004:0023) |   | └─ObjectPropertyExpression
//@[004:0012) |   |   ├─StringLiteralExpression { Value = location }
//@[014:0023) |   |   └─StringLiteralExpression { Value = eastus2 }
  }
  {
//@[002:0049) |   └─ObjectExpression
    name: 'two'
//@[004:0015) |     ├─ObjectPropertyExpression
//@[004:0008) |     | ├─StringLiteralExpression { Value = name }
//@[010:0015) |     | └─StringLiteralExpression { Value = two }
    location: 'westus'
//@[004:0022) |     └─ObjectPropertyExpression
//@[004:0012) |       ├─StringLiteralExpression { Value = location }
//@[014:0022) |       └─StringLiteralExpression { Value = westus }
  }
]

var emptyArray = []
//@[000:0019) ├─DeclaredVariableExpression { Name = emptyArray }
//@[017:0019) | └─ArrayExpression

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[000:0189) ├─DeclaredModuleExpression
//@[042:0189) | ├─ForLoopExpression
//@[057:0066) | | ├─VariableReferenceExpression { Variable = myModules }
//@[068:0188) | | └─ObjectExpression
//@[057:0066) | |         └─VariableReferenceExpression { Variable = myModules }
//@[057:0066) |   |   └─VariableReferenceExpression { Variable = myModules }
//@[057:0066) |         └─VariableReferenceExpression { Variable = myModules }
  name: module.name
//@[002:0019) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0019) | |     └─PropertyAccessExpression { PropertyName = name }
//@[008:0014) | |       └─ArrayAccessExpression
//@[008:0014) | |         ├─CopyIndexExpression
  params: {
//@[010:0093) | └─ObjectExpression
    arrayParam: []
//@[004:0018) |   ├─ObjectPropertyExpression
//@[004:0014) |   | ├─StringLiteralExpression { Value = arrayParam }
//@[016:0018) |   | └─ArrayExpression
    objParam: module
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0020) |   | └─ArrayAccessExpression
//@[014:0020) |   |   ├─CopyIndexExpression
    stringParamB: module.location
//@[004:0033) |   └─ObjectPropertyExpression
//@[004:0016) |     ├─StringLiteralExpression { Value = stringParamB }
//@[018:0033) |     └─PropertyAccessExpression { PropertyName = location }
//@[018:0024) |       └─ArrayAccessExpression
//@[018:0024) |         ├─CopyIndexExpression
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[000:0256) ├─DeclaredModuleExpression
//@[051:0256) | ├─ForLoopExpression
//@[071:0080) | | ├─VariableReferenceExpression { Variable = myModules }
//@[082:0255) | | └─ObjectExpression
//@[071:0080) | |         └─VariableReferenceExpression { Variable = myModules }
//@[071:0080) |   |   └─VariableReferenceExpression { Variable = myModules }
//@[071:0080) |   |     └─VariableReferenceExpression { Variable = myModules }
  name: module.name
//@[002:0019) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0019) | |     └─PropertyAccessExpression { PropertyName = name }
//@[008:0014) | |       └─ArrayAccessExpression
//@[008:0014) | |         ├─CopyIndexExpression
  params: {
//@[010:0146) | └─ObjectExpression
    arrayParam: [
//@[004:0037) |   ├─ObjectPropertyExpression
//@[004:0014) |   | ├─StringLiteralExpression { Value = arrayParam }
//@[016:0037) |   | └─ArrayExpression
      i + 1
//@[006:0011) |   |   └─BinaryExpression { Operator = Add }
//@[006:0007) |   |     ├─CopyIndexExpression
//@[010:0011) |   |     └─IntegerLiteralExpression { Value = 1 }
    ]
    objParam: module
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0020) |   | └─ArrayAccessExpression
//@[014:0020) |   |   ├─CopyIndexExpression
    stringParamB: module.location
//@[004:0033) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0033) |   | └─PropertyAccessExpression { PropertyName = location }
//@[018:0024) |   |   └─ArrayAccessExpression
//@[018:0024) |   |     ├─CopyIndexExpression
    stringParamA: concat('a', i)
//@[004:0032) |   └─ObjectPropertyExpression
//@[004:0016) |     ├─StringLiteralExpression { Value = stringParamA }
//@[018:0032) |     └─FunctionCallExpression { Name = concat }
//@[025:0028) |       ├─StringLiteralExpression { Value = a }
//@[030:0031) |       └─CopyIndexExpression
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[000:0246) ├─DeclaredModuleExpression
//@[042:0246) | ├─ForLoopExpression
//@[057:0066) | | ├─VariableReferenceExpression { Variable = myModules }
//@[068:0245) | | └─ObjectExpression
//@[057:0066) | |         └─VariableReferenceExpression { Variable = myModules }
//@[057:0066) |   |         └─VariableReferenceExpression { Variable = myModules }
//@[057:0066) |   |   └─VariableReferenceExpression { Variable = myModules }
//@[057:0066) |         └─VariableReferenceExpression { Variable = myModules }
  name: module.name
//@[002:0019) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0019) | |     └─PropertyAccessExpression { PropertyName = name }
//@[008:0014) | |       └─ArrayAccessExpression
//@[008:0014) | |         ├─CopyIndexExpression
  params: {
//@[010:0150) | └─ObjectExpression
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[004:0075) |   ├─ObjectPropertyExpression
//@[004:0014) |   | ├─StringLiteralExpression { Value = arrayParam }
//@[016:0075) |   | └─ForLoopExpression
//@[026:0036) |   |   ├─FunctionCallExpression { Name = range }
//@[032:0033) |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[034:0035) |   |   | └─IntegerLiteralExpression { Value = 3 }
//@[038:0074) |   |   └─FunctionCallExpression { Name = concat }
//@[045:0052) |   |     ├─StringLiteralExpression { Value = test- }
//@[054:0055) |   |     ├─ArrayAccessExpression
//@[054:0055) |   |     | ├─CopyIndexExpression
//@[026:0036) |   |     | └─FunctionCallExpression { Name = range }
//@[032:0033) |   |     |   ├─IntegerLiteralExpression { Value = 0 }
//@[034:0035) |   |     |   └─IntegerLiteralExpression { Value = 3 }
//@[057:0060) |   |     ├─StringLiteralExpression { Value = - }
//@[062:0073) |   |     └─PropertyAccessExpression { PropertyName = name }
//@[062:0068) |   |       └─ArrayAccessExpression
//@[062:0068) |   |         ├─CopyIndexExpression
    objParam: module
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0020) |   | └─ArrayAccessExpression
//@[014:0020) |   |   ├─CopyIndexExpression
    stringParamB: module.location
//@[004:0033) |   └─ObjectPropertyExpression
//@[004:0016) |     ├─StringLiteralExpression { Value = stringParamB }
//@[018:0033) |     └─PropertyAccessExpression { PropertyName = location }
//@[018:0024) |       └─ArrayAccessExpression
//@[018:0024) |         ├─CopyIndexExpression
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:0234) ├─DeclaredModuleExpression
//@[056:0234) | ├─ForLoopExpression
//@[066:0076) | | ├─VariableReferenceExpression { Variable = emptyArray }
//@[077:0233) | | └─ObjectExpression
//@[066:0076) | |         └─VariableReferenceExpression { Variable = emptyArray }
  name: 'hello-${x}'
//@[002:0020) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0020) | |     └─InterpolatedStringExpression
//@[017:0018) | |       └─ArrayAccessExpression
//@[017:0018) | |         ├─CopyIndexExpression
  params: {
//@[010:0128) | └─ObjectExpression
    objParam: {}
//@[004:0016) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0016) |   | └─ObjectExpression
    stringParamA: 'test'
//@[004:0024) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamA }
//@[018:0024) |   | └─StringLiteralExpression { Value = test }
    stringParamB: 'test'
//@[004:0024) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0024) |   | └─StringLiteralExpression { Value = test }
    arrayParam: [for x in emptyArray: x]
//@[004:0040) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0040) |     └─ForLoopExpression
//@[026:0036) |       ├─VariableReferenceExpression { Variable = emptyArray }
//@[038:0039) |       └─ArrayAccessExpression
//@[038:0039) |         ├─CopyIndexExpression
//@[026:0036) |         └─VariableReferenceExpression { Variable = emptyArray }
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[000:0035) ├─DeclaredVariableExpression { Name = duplicateAcrossScopes }
//@[028:0035) | └─StringLiteralExpression { Value = hello }
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[000:0264) ├─DeclaredModuleExpression
//@[053:0264) | ├─ForLoopExpression
//@[083:0085) | | ├─ArrayExpression
//@[087:0263) | | └─ObjectExpression
//@[083:0085) | |         └─ArrayExpression
  name: 'hello-${duplicateAcrossScopes}'
//@[002:0040) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0040) | |     └─InterpolatedStringExpression
//@[017:0038) | |       └─ArrayAccessExpression
//@[017:0038) | |         ├─CopyIndexExpression
  params: {
//@[010:0128) | └─ObjectExpression
    objParam: {}
//@[004:0016) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0016) |   | └─ObjectExpression
    stringParamA: 'test'
//@[004:0024) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamA }
//@[018:0024) |   | └─StringLiteralExpression { Value = test }
    stringParamB: 'test'
//@[004:0024) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0024) |   | └─StringLiteralExpression { Value = test }
    arrayParam: [for x in emptyArray: x]
//@[004:0040) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0040) |     └─ForLoopExpression
//@[026:0036) |       ├─VariableReferenceExpression { Variable = emptyArray }
//@[038:0039) |       └─ArrayAccessExpression
//@[038:0039) |         ├─CopyIndexExpression
//@[026:0036) |         └─VariableReferenceExpression { Variable = emptyArray }
  }
}]

var someDuplicate = true
//@[000:0024) ├─DeclaredVariableExpression { Name = someDuplicate }
//@[020:0024) | └─BooleanLiteralExpression { Value = True }
var otherDuplicate = false
//@[000:0026) ├─DeclaredVariableExpression { Name = otherDuplicate }
//@[021:0026) | └─BooleanLiteralExpression { Value = False }
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[000:0263) ├─DeclaredModuleExpression
//@[046:0263) | ├─ForLoopExpression
//@[068:0070) | | ├─ArrayExpression
//@[072:0262) | | └─ObjectExpression
//@[068:0070) | |         └─ArrayExpression
//@[068:0070) |         | └─ArrayExpression
  name: 'hello-${someDuplicate}'
//@[002:0032) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0032) | |     └─InterpolatedStringExpression
//@[017:0030) | |       └─ArrayAccessExpression
//@[017:0030) | |         ├─CopyIndexExpression
  params: {
//@[010:0150) | └─ObjectExpression
    objParam: {}
//@[004:0016) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0016) |   | └─ObjectExpression
    stringParamB: 'test'
//@[004:0024) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0024) |   | └─StringLiteralExpression { Value = test }
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[004:0088) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0088) |     └─ForLoopExpression
//@[039:0049) |       ├─VariableReferenceExpression { Variable = emptyArray }
//@[051:0087) |       └─InterpolatedStringExpression
//@[054:0067) |         ├─ArrayAccessExpression
//@[054:0067) |         | ├─CopyIndexExpression
//@[071:0085) |         └─ArrayAccessExpression
//@[071:0085) |           ├─CopyIndexExpression
//@[039:0049) |           └─VariableReferenceExpression { Variable = emptyArray }
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[000:0438) ├─DeclaredModuleExpression
//@[058:0438) | ├─ObjectExpression
  name: 'propertyLoopInsideParameterValue'
//@[002:0042) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0042) | |   └─StringLiteralExpression { Value = propertyLoopInsideParameterValue }
  params: {
//@[010:0330) | └─ObjectExpression
    objParam: {
//@[004:0209) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0209) |   | └─ObjectExpression
      a: [for i in range(0,10): i]
//@[006:0034) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = a }
//@[009:0034) |   |   | └─ForLoopExpression
//@[019:0030) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[027:0029) |   |   |   | └─IntegerLiteralExpression { Value = 10 }
//@[032:0033) |   |   |   └─ArrayAccessExpression
//@[032:0033) |   |   |     ├─CopyIndexExpression
//@[019:0030) |   |   |     └─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |       ├─IntegerLiteralExpression { Value = 0 }
//@[027:0029) |   |   |       └─IntegerLiteralExpression { Value = 10 }
      b: [for i in range(1,2): i]
//@[006:0033) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = b }
//@[009:0033) |   |   | └─ForLoopExpression
//@[019:0029) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |   | ├─IntegerLiteralExpression { Value = 1 }
//@[027:0028) |   |   |   | └─IntegerLiteralExpression { Value = 2 }
//@[031:0032) |   |   |   └─ArrayAccessExpression
//@[031:0032) |   |   |     ├─CopyIndexExpression
//@[019:0029) |   |   |     └─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |       ├─IntegerLiteralExpression { Value = 1 }
//@[027:0028) |   |   |       └─IntegerLiteralExpression { Value = 2 }
      c: {
//@[006:0056) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = c }
//@[009:0056) |   |   | └─ObjectExpression
        d: [for j in range(2,3): j]
//@[008:0035) |   |   |   └─ObjectPropertyExpression
//@[008:0009) |   |   |     ├─StringLiteralExpression { Value = d }
//@[011:0035) |   |   |     └─ForLoopExpression
//@[021:0031) |   |   |       ├─FunctionCallExpression { Name = range }
//@[027:0028) |   |   |       | ├─IntegerLiteralExpression { Value = 2 }
//@[029:0030) |   |   |       | └─IntegerLiteralExpression { Value = 3 }
//@[033:0034) |   |   |       └─ArrayAccessExpression
//@[033:0034) |   |   |         ├─CopyIndexExpression
//@[021:0031) |   |   |         └─FunctionCallExpression { Name = range }
//@[027:0028) |   |   |           ├─IntegerLiteralExpression { Value = 2 }
//@[029:0030) |   |   |           └─IntegerLiteralExpression { Value = 3 }
      }
      e: [for k in range(4,4): {
//@[006:0056) |   |   └─ObjectPropertyExpression
//@[006:0007) |   |     ├─StringLiteralExpression { Value = e }
//@[009:0056) |   |     └─ForLoopExpression
//@[019:0029) |   |       ├─FunctionCallExpression { Name = range }
//@[025:0026) |   |       | ├─IntegerLiteralExpression { Value = 4 }
//@[027:0028) |   |       | └─IntegerLiteralExpression { Value = 4 }
//@[031:0055) |   |       └─ObjectExpression
//@[019:0029) |   |             └─FunctionCallExpression { Name = range }
//@[025:0026) |   |               ├─IntegerLiteralExpression { Value = 4 }
//@[027:0028) |   |               └─IntegerLiteralExpression { Value = 4 }
        f: k
//@[008:0012) |   |         └─ObjectPropertyExpression
//@[008:0009) |   |           ├─StringLiteralExpression { Value = f }
//@[011:0012) |   |           └─ArrayAccessExpression
//@[011:0012) |   |             ├─CopyIndexExpression
      }]
    }
    stringParamB: ''
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0020) |   | └─StringLiteralExpression { Value =  }
    arrayParam: [
//@[004:0079) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0079) |     └─ArrayExpression
      {
//@[006:0053) |       └─ObjectExpression
        e: [for j in range(7,7): j]
//@[008:0035) |         └─ObjectPropertyExpression
//@[008:0009) |           ├─StringLiteralExpression { Value = e }
//@[011:0035) |           └─ForLoopExpression
//@[021:0031) |             ├─FunctionCallExpression { Name = range }
//@[027:0028) |             | ├─IntegerLiteralExpression { Value = 7 }
//@[029:0030) |             | └─IntegerLiteralExpression { Value = 7 }
//@[033:0034) |             └─ArrayAccessExpression
//@[033:0034) |               ├─CopyIndexExpression
//@[021:0031) |               └─FunctionCallExpression { Name = range }
//@[027:0028) |                 ├─IntegerLiteralExpression { Value = 7 }
//@[029:0030) |                 └─IntegerLiteralExpression { Value = 7 }
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[000:0514) ├─DeclaredModuleExpression
//@[069:0514) | ├─ObjectExpression
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[002:0053) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0053) | |   └─StringLiteralExpression { Value = propertyLoopInsideParameterValueWithIndexes }
  params: {
//@[010:0384) | └─ObjectExpression
    objParam: {
//@[004:0263) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0263) |   | └─ObjectExpression
      a: [for (i, i2) in range(0,10): i + i2]
//@[006:0045) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = a }
//@[009:0045) |   |   | └─ForLoopExpression
//@[025:0036) |   |   |   ├─FunctionCallExpression { Name = range }
//@[031:0032) |   |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[033:0035) |   |   |   | └─IntegerLiteralExpression { Value = 10 }
//@[038:0044) |   |   |   └─BinaryExpression { Operator = Add }
//@[038:0039) |   |   |     ├─ArrayAccessExpression
//@[038:0039) |   |   |     | ├─CopyIndexExpression
//@[025:0036) |   |   |     | └─FunctionCallExpression { Name = range }
//@[031:0032) |   |   |     |   ├─IntegerLiteralExpression { Value = 0 }
//@[033:0035) |   |   |     |   └─IntegerLiteralExpression { Value = 10 }
//@[042:0044) |   |   |     └─CopyIndexExpression
      b: [for (i, i2) in range(1,2): i / i2]
//@[006:0044) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = b }
//@[009:0044) |   |   | └─ForLoopExpression
//@[025:0035) |   |   |   ├─FunctionCallExpression { Name = range }
//@[031:0032) |   |   |   | ├─IntegerLiteralExpression { Value = 1 }
//@[033:0034) |   |   |   | └─IntegerLiteralExpression { Value = 2 }
//@[037:0043) |   |   |   └─BinaryExpression { Operator = Divide }
//@[037:0038) |   |   |     ├─ArrayAccessExpression
//@[037:0038) |   |   |     | ├─CopyIndexExpression
//@[025:0035) |   |   |     | └─FunctionCallExpression { Name = range }
//@[031:0032) |   |   |     |   ├─IntegerLiteralExpression { Value = 1 }
//@[033:0034) |   |   |     |   └─IntegerLiteralExpression { Value = 2 }
//@[041:0043) |   |   |     └─CopyIndexExpression
      c: {
//@[006:0067) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = c }
//@[009:0067) |   |   | └─ObjectExpression
        d: [for (j, j2) in range(2,3): j * j2]
//@[008:0046) |   |   |   └─ObjectPropertyExpression
//@[008:0009) |   |   |     ├─StringLiteralExpression { Value = d }
//@[011:0046) |   |   |     └─ForLoopExpression
//@[027:0037) |   |   |       ├─FunctionCallExpression { Name = range }
//@[033:0034) |   |   |       | ├─IntegerLiteralExpression { Value = 2 }
//@[035:0036) |   |   |       | └─IntegerLiteralExpression { Value = 3 }
//@[039:0045) |   |   |       └─BinaryExpression { Operator = Multiply }
//@[039:0040) |   |   |         ├─ArrayAccessExpression
//@[039:0040) |   |   |         | ├─CopyIndexExpression
//@[027:0037) |   |   |         | └─FunctionCallExpression { Name = range }
//@[033:0034) |   |   |         |   ├─IntegerLiteralExpression { Value = 2 }
//@[035:0036) |   |   |         |   └─IntegerLiteralExpression { Value = 3 }
//@[043:0045) |   |   |         └─CopyIndexExpression
      }
      e: [for (k, k2) in range(4,4): {
//@[006:0077) |   |   └─ObjectPropertyExpression
//@[006:0007) |   |     ├─StringLiteralExpression { Value = e }
//@[009:0077) |   |     └─ForLoopExpression
//@[025:0035) |   |       ├─FunctionCallExpression { Name = range }
//@[031:0032) |   |       | ├─IntegerLiteralExpression { Value = 4 }
//@[033:0034) |   |       | └─IntegerLiteralExpression { Value = 4 }
//@[037:0076) |   |       └─ObjectExpression
//@[025:0035) |   |         |   └─FunctionCallExpression { Name = range }
//@[031:0032) |   |         |     ├─IntegerLiteralExpression { Value = 4 }
//@[033:0034) |   |         |     └─IntegerLiteralExpression { Value = 4 }
        f: k
//@[008:0012) |   |         ├─ObjectPropertyExpression
//@[008:0009) |   |         | ├─StringLiteralExpression { Value = f }
//@[011:0012) |   |         | └─ArrayAccessExpression
//@[011:0012) |   |         |   ├─CopyIndexExpression
        g: k2
//@[008:0013) |   |         └─ObjectPropertyExpression
//@[008:0009) |   |           ├─StringLiteralExpression { Value = g }
//@[011:0013) |   |           └─CopyIndexExpression
      }]
    }
    stringParamB: ''
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0020) |   | └─StringLiteralExpression { Value =  }
    arrayParam: [
//@[004:0079) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0079) |     └─ArrayExpression
      {
//@[006:0053) |       └─ObjectExpression
        e: [for j in range(7,7): j]
//@[008:0035) |         └─ObjectPropertyExpression
//@[008:0009) |           ├─StringLiteralExpression { Value = e }
//@[011:0035) |           └─ForLoopExpression
//@[021:0031) |             ├─FunctionCallExpression { Name = range }
//@[027:0028) |             | ├─IntegerLiteralExpression { Value = 7 }
//@[029:0030) |             | └─IntegerLiteralExpression { Value = 7 }
//@[033:0034) |             └─ArrayAccessExpression
//@[033:0034) |               ├─CopyIndexExpression
//@[021:0031) |               └─FunctionCallExpression { Name = range }
//@[027:0028) |                 ├─IntegerLiteralExpression { Value = 7 }
//@[029:0030) |                 └─IntegerLiteralExpression { Value = 7 }
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[000:0529) ├─DeclaredModuleExpression
//@[074:0529) | ├─ForLoopExpression
//@[088:0098) | | ├─FunctionCallExpression { Name = range }
//@[094:0095) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[096:0097) | | | └─IntegerLiteralExpression { Value = 1 }
//@[100:0528) | | └─ObjectExpression
//@[088:0098) |   |   |       └─FunctionCallExpression { Name = range }
//@[094:0095) |   |   |         ├─IntegerLiteralExpression { Value = 0 }
//@[096:0097) |   |   |         └─IntegerLiteralExpression { Value = 1 }
//@[088:0098) |   |   |       └─FunctionCallExpression { Name = range }
//@[094:0095) |   |   |         ├─IntegerLiteralExpression { Value = 0 }
//@[096:0097) |   |   |         └─IntegerLiteralExpression { Value = 1 }
//@[088:0098) |   |               └─FunctionCallExpression { Name = range }
//@[094:0095) |   |                 ├─IntegerLiteralExpression { Value = 0 }
//@[096:0097) |   |                 └─IntegerLiteralExpression { Value = 1 }
//@[088:0098) |                 └─FunctionCallExpression { Name = range }
//@[094:0095) |                   ├─IntegerLiteralExpression { Value = 0 }
//@[096:0097) |                   └─IntegerLiteralExpression { Value = 1 }
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[002:0058) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0058) | |     └─StringLiteralExpression { Value = propertyLoopInsideParameterValueInsideModuleLoop }
  params: {
//@[010:0362) | └─ObjectExpression
    objParam: {
//@[004:0233) |   ├─ObjectPropertyExpression
//@[004:0012) |   | ├─StringLiteralExpression { Value = objParam }
//@[014:0233) |   | └─ObjectExpression
      a: [for i in range(0,10): i + thing]
//@[006:0042) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = a }
//@[009:0042) |   |   | └─ForLoopExpression
//@[019:0030) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |   | ├─IntegerLiteralExpression { Value = 0 }
//@[027:0029) |   |   |   | └─IntegerLiteralExpression { Value = 10 }
//@[032:0041) |   |   |   └─BinaryExpression { Operator = Add }
//@[032:0033) |   |   |     ├─ArrayAccessExpression
//@[032:0033) |   |   |     | ├─CopyIndexExpression
//@[019:0030) |   |   |     | └─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |     |   ├─IntegerLiteralExpression { Value = 0 }
//@[027:0029) |   |   |     |   └─IntegerLiteralExpression { Value = 10 }
//@[036:0041) |   |   |     └─ArrayAccessExpression
//@[036:0041) |   |   |       ├─CopyIndexExpression
      b: [for i in range(1,2): i * thing]
//@[006:0041) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = b }
//@[009:0041) |   |   | └─ForLoopExpression
//@[019:0029) |   |   |   ├─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |   | ├─IntegerLiteralExpression { Value = 1 }
//@[027:0028) |   |   |   | └─IntegerLiteralExpression { Value = 2 }
//@[031:0040) |   |   |   └─BinaryExpression { Operator = Multiply }
//@[031:0032) |   |   |     ├─ArrayAccessExpression
//@[031:0032) |   |   |     | ├─CopyIndexExpression
//@[019:0029) |   |   |     | └─FunctionCallExpression { Name = range }
//@[025:0026) |   |   |     |   ├─IntegerLiteralExpression { Value = 1 }
//@[027:0028) |   |   |     |   └─IntegerLiteralExpression { Value = 2 }
//@[035:0040) |   |   |     └─ArrayAccessExpression
//@[035:0040) |   |   |       ├─CopyIndexExpression
      c: {
//@[006:0056) |   |   ├─ObjectPropertyExpression
//@[006:0007) |   |   | ├─StringLiteralExpression { Value = c }
//@[009:0056) |   |   | └─ObjectExpression
        d: [for j in range(2,3): j]
//@[008:0035) |   |   |   └─ObjectPropertyExpression
//@[008:0009) |   |   |     ├─StringLiteralExpression { Value = d }
//@[011:0035) |   |   |     └─ForLoopExpression
//@[021:0031) |   |   |       ├─FunctionCallExpression { Name = range }
//@[027:0028) |   |   |       | ├─IntegerLiteralExpression { Value = 2 }
//@[029:0030) |   |   |       | └─IntegerLiteralExpression { Value = 3 }
//@[033:0034) |   |   |       └─ArrayAccessExpression
//@[033:0034) |   |   |         ├─CopyIndexExpression
//@[021:0031) |   |   |         └─FunctionCallExpression { Name = range }
//@[027:0028) |   |   |           ├─IntegerLiteralExpression { Value = 2 }
//@[029:0030) |   |   |           └─IntegerLiteralExpression { Value = 3 }
      }
      e: [for k in range(4,4): {
//@[006:0064) |   |   └─ObjectPropertyExpression
//@[006:0007) |   |     ├─StringLiteralExpression { Value = e }
//@[009:0064) |   |     └─ForLoopExpression
//@[019:0029) |   |       ├─FunctionCallExpression { Name = range }
//@[025:0026) |   |       | ├─IntegerLiteralExpression { Value = 4 }
//@[027:0028) |   |       | └─IntegerLiteralExpression { Value = 4 }
//@[031:0063) |   |       └─ObjectExpression
//@[019:0029) |   |             | └─FunctionCallExpression { Name = range }
//@[025:0026) |   |             |   ├─IntegerLiteralExpression { Value = 4 }
//@[027:0028) |   |             |   └─IntegerLiteralExpression { Value = 4 }
        f: k - thing
//@[008:0020) |   |         └─ObjectPropertyExpression
//@[008:0009) |   |           ├─StringLiteralExpression { Value = f }
//@[011:0020) |   |           └─BinaryExpression { Operator = Subtract }
//@[011:0012) |   |             ├─ArrayAccessExpression
//@[011:0012) |   |             | ├─CopyIndexExpression
//@[015:0020) |   |             └─ArrayAccessExpression
//@[015:0020) |   |               ├─CopyIndexExpression
      }]
    }
    stringParamB: ''
//@[004:0020) |   ├─ObjectPropertyExpression
//@[004:0016) |   | ├─StringLiteralExpression { Value = stringParamB }
//@[018:0020) |   | └─StringLiteralExpression { Value =  }
    arrayParam: [
//@[004:0087) |   └─ObjectPropertyExpression
//@[004:0014) |     ├─StringLiteralExpression { Value = arrayParam }
//@[016:0087) |     └─ArrayExpression
      {
//@[006:0061) |       └─ObjectExpression
        e: [for j in range(7,7): j % thing]
//@[008:0043) |         └─ObjectPropertyExpression
//@[008:0009) |           ├─StringLiteralExpression { Value = e }
//@[011:0043) |           └─ForLoopExpression
//@[021:0031) |             ├─FunctionCallExpression { Name = range }
//@[027:0028) |             | ├─IntegerLiteralExpression { Value = 7 }
//@[029:0030) |             | └─IntegerLiteralExpression { Value = 7 }
//@[033:0042) |             └─BinaryExpression { Operator = Modulo }
//@[033:0034) |               ├─ArrayAccessExpression
//@[033:0034) |               | ├─CopyIndexExpression
//@[021:0031) |               | └─FunctionCallExpression { Name = range }
//@[027:0028) |               |   ├─IntegerLiteralExpression { Value = 7 }
//@[029:0030) |               |   └─IntegerLiteralExpression { Value = 7 }
//@[037:0042) |               └─ArrayAccessExpression
//@[037:0042) |                 ├─CopyIndexExpression
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0090) ├─DeclaredResourceExpression
//@[062:0090) | └─ObjectExpression
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@[000:0213) ├─DeclaredModuleExpression
//@[050:0213) | ├─ObjectExpression
  name: 'secureModule1'
//@[002:0023) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0023) | |   └─StringLiteralExpression { Value = secureModule1 }
  params: {
//@[010:0132) | └─ObjectExpression
    secureStringParam1: kv.getSecret('mySecret')
//@[004:0048) |   ├─ObjectPropertyExpression
//@[004:0022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:0048) |   | └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:0026) |   |   ├─ResourceReferenceExpression
//@[037:0047) |   |   └─StringLiteralExpression { Value = mySecret }
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
//@[004:0064) |   └─ObjectPropertyExpression
//@[004:0022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:0064) |     └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:0026) |       ├─ResourceReferenceExpression
//@[037:0047) |       ├─StringLiteralExpression { Value = mySecret }
//@[048:0063) |       └─StringLiteralExpression { Value = secretVersion }
  }
}

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0134) ├─DeclaredResourceExpression
//@[068:0134) | └─ObjectExpression
  name: 'testkeyvault'
  scope: resourceGroup('otherGroup')
}

module secureModule2 'child/secureParams.bicep' = {
//@[000:0225) ├─DeclaredModuleExpression
//@[050:0225) | ├─ObjectExpression
  name: 'secureModule2'
//@[002:0023) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0023) | |   └─StringLiteralExpression { Value = secureModule2 }
  params: {
//@[010:0144) | └─ObjectExpression
    secureStringParam1: scopedKv.getSecret('mySecret')
//@[004:0054) |   ├─ObjectPropertyExpression
//@[004:0022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:0054) |   | └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:0032) |   |   ├─ResourceReferenceExpression
//@[043:0053) |   |   └─StringLiteralExpression { Value = mySecret }
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
//@[004:0070) |   └─ObjectPropertyExpression
//@[004:0022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:0070) |     └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:0032) |       ├─ResourceReferenceExpression
//@[043:0053) |       ├─StringLiteralExpression { Value = mySecret }
//@[054:0069) |       └─StringLiteralExpression { Value = secretVersion }
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
//@[000:0200) ├─DeclaredVariableExpression { Name = vaults }
//@[013:0200) | └─ArrayExpression
  {
//@[002:0089) |   ├─ObjectExpression
    vaultName: 'test-1-kv'
//@[004:0026) |   | ├─ObjectPropertyExpression
//@[004:0013) |   | | ├─StringLiteralExpression { Value = vaultName }
//@[015:0026) |   | | └─StringLiteralExpression { Value = test-1-kv }
    vaultRG: 'test-1-rg'
//@[004:0024) |   | ├─ObjectPropertyExpression
//@[004:0011) |   | | ├─StringLiteralExpression { Value = vaultRG }
//@[013:0024) |   | | └─StringLiteralExpression { Value = test-1-rg }
    vaultSub: 'abcd-efgh'
//@[004:0025) |   | └─ObjectPropertyExpression
//@[004:0012) |   |   ├─StringLiteralExpression { Value = vaultSub }
//@[014:0025) |   |   └─StringLiteralExpression { Value = abcd-efgh }
  }
  {
//@[002:0090) |   └─ObjectExpression
    vaultName: 'test-2-kv'
//@[004:0026) |     ├─ObjectPropertyExpression
//@[004:0013) |     | ├─StringLiteralExpression { Value = vaultName }
//@[015:0026) |     | └─StringLiteralExpression { Value = test-2-kv }
    vaultRG: 'test-2-rg'
//@[004:0024) |     ├─ObjectPropertyExpression
//@[004:0011) |     | ├─StringLiteralExpression { Value = vaultRG }
//@[013:0024) |     | └─StringLiteralExpression { Value = test-2-rg }
    vaultSub: 'ijkl-1adg1'
//@[004:0026) |     └─ObjectPropertyExpression
//@[004:0012) |       ├─StringLiteralExpression { Value = vaultSub }
//@[014:0026) |       └─StringLiteralExpression { Value = ijkl-1adg1 }
  }
]
var secrets = [
//@[000:0132) ├─DeclaredVariableExpression { Name = secrets }
//@[014:0132) | └─ArrayExpression
  {
//@[002:0055) |   ├─ObjectExpression
    name: 'secret01'
//@[004:0020) |   | ├─ObjectPropertyExpression
//@[004:0008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:0020) |   | | └─StringLiteralExpression { Value = secret01 }
    version: 'versionA'
//@[004:0023) |   | └─ObjectPropertyExpression
//@[004:0011) |   |   ├─StringLiteralExpression { Value = version }
//@[013:0023) |   |   └─StringLiteralExpression { Value = versionA }
  }
  {
//@[002:0055) |   └─ObjectExpression
    name: 'secret02'
//@[004:0020) |     ├─ObjectPropertyExpression
//@[004:0008) |     | ├─StringLiteralExpression { Value = name }
//@[010:0020) |     | └─StringLiteralExpression { Value = secret02 }
    version: 'versionB'
//@[004:0023) |     └─ObjectPropertyExpression
//@[004:0011) |       ├─StringLiteralExpression { Value = version }
//@[013:0023) |       └─StringLiteralExpression { Value = versionB }
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
//@[000:0175) ├─DeclaredResourceExpression
//@[068:0175) | └─ForLoopExpression
//@[082:0088) |   ├─VariableReferenceExpression { Variable = vaults }
//@[090:0174) |   └─ObjectExpression
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[000:0278) ├─DeclaredModuleExpression
//@[055:0278) | ├─ForLoopExpression
//@[075:0082) | | ├─VariableReferenceExpression { Variable = secrets }
//@[084:0277) | | └─ObjectExpression
//@[075:0082) |   |       └─VariableReferenceExpression { Variable = secrets }
//@[075:0082) |       |   └─VariableReferenceExpression { Variable = secrets }
//@[075:0082) |           └─VariableReferenceExpression { Variable = secrets }
  name: 'secureModuleLooped-${i}'
//@[002:0033) | |   └─ObjectPropertyExpression
//@[002:0006) | |     ├─StringLiteralExpression { Value = name }
//@[008:0033) | |     └─InterpolatedStringExpression
//@[030:0031) | |       └─CopyIndexExpression
  params: {
//@[010:0152) | └─ObjectExpression
    secureStringParam1: loopedKv[i].getSecret(secret.name)
//@[004:0058) |   ├─ObjectPropertyExpression
//@[004:0022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:0058) |   | └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:0035) |   |   ├─ResourceReferenceExpression
//@[046:0057) |   |   └─PropertyAccessExpression { PropertyName = name }
//@[046:0052) |   |     └─ArrayAccessExpression
//@[046:0052) |   |       ├─CopyIndexExpression
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
//@[004:0074) |   └─ObjectPropertyExpression
//@[004:0022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:0074) |     └─ResourceFunctionCallExpression { Name = getSecret }
//@[024:0035) |       ├─ResourceReferenceExpression
//@[046:0057) |       ├─PropertyAccessExpression { PropertyName = name }
//@[046:0052) |       | └─ArrayAccessExpression
//@[046:0052) |       |   ├─CopyIndexExpression
//@[059:0073) |       └─PropertyAccessExpression { PropertyName = version }
//@[059:0065) |         └─ArrayAccessExpression
//@[059:0065) |           ├─CopyIndexExpression
  }
}]

module secureModuleCondition 'child/secureParams.bicep' = {
//@[000:0285) ├─DeclaredModuleExpression
//@[058:0285) | ├─ObjectExpression
  name: 'secureModuleCondition'
//@[002:0031) | | └─ObjectPropertyExpression
//@[002:0006) | |   ├─StringLiteralExpression { Value = name }
//@[008:0031) | |   └─StringLiteralExpression { Value = secureModuleCondition }
  params: {
//@[010:0188) | └─ObjectExpression
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
//@[004:0067) |   ├─ObjectPropertyExpression
//@[004:0022) |   | ├─StringLiteralExpression { Value = secureStringParam1 }
//@[024:0067) |   | └─TernaryExpression
//@[024:0028) |   |   ├─BooleanLiteralExpression { Value = True }
//@[031:0055) |   |   ├─ResourceFunctionCallExpression { Name = getSecret }
//@[031:0033) |   |   | ├─ResourceReferenceExpression
//@[044:0054) |   |   | └─StringLiteralExpression { Value = mySecret }
//@[058:0067) |   |   └─StringLiteralExpression { Value = notTrue }
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
//@[004:0101) |   └─ObjectPropertyExpression
//@[004:0022) |     ├─StringLiteralExpression { Value = secureStringParam2 }
//@[024:0101) |     └─TernaryExpression
//@[024:0028) |       ├─BooleanLiteralExpression { Value = True }
//@[031:0089) |       ├─TernaryExpression
//@[031:0036) |       | ├─BooleanLiteralExpression { Value = False }
//@[039:0046) |       | ├─StringLiteralExpression { Value = false }
//@[049:0089) |       | └─ResourceFunctionCallExpression { Name = getSecret }
//@[049:0051) |       |   ├─ResourceReferenceExpression
//@[062:0072) |       |   ├─StringLiteralExpression { Value = mySecret }
//@[073:0088) |       |   └─StringLiteralExpression { Value = secretVersion }
//@[092:0101) |       └─StringLiteralExpression { Value = notTrue }
  }
}

// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@[000:0070) ├─DeclaredModuleExpression
//@[045:0070) | └─ObjectExpression
  name: 'withSpace'
//@[002:0019) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0019) |     └─StringLiteralExpression { Value = withSpace }
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[000:0104) ├─DeclaredModuleExpression
//@[074:0104) | └─ObjectExpression
  name: 'childWithSpace'
//@[002:0024) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0024) |     └─StringLiteralExpression { Value = childWithSpace }
}

module withSeparateConfig './child/folder with separate config/moduleWithAzImport.bicep' = {
//@[000:0125) ├─DeclaredModuleExpression
//@[091:0125) | └─ObjectExpression
  name: 'withSeparateConfig'
//@[002:0028) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = name }
//@[008:0028) |     └─StringLiteralExpression { Value = withSeparateConfig }
}

