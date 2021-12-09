
@sys.description('string output description')
output myStr string = 'hello'

@sys.description('int output description')
output myInt int = 7
output myOtherInt int = 20 / 13 + 80 % -4

@sys.description('bool output description')
output myBool bool = !false
output myOtherBool bool = true

@sys.description('object array description')
output suchEmpty array = [
]

output suchEmpty2 object = {
}

@sys.description('object output description')
output obj object = {
  a: 'a'
  b: 12
  c: true
  d: null
  list: [
    1
    2
    3
    null
    {
    }
  ]
  obj: {
    nested: [
      'hello'
    ]
  }
}

output myArr array = [
  'pirates'
  'say'
   false ? 'arr2' : 'arr'
]

output rgLocation string = resourceGroup().location
//@[27:51) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[23:47) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|

output expressionBasedIndexer string = {
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo
//@[2:26) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[27:86) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: function 'listKeys' (CodeDescription: bicep core(https://aka.ms/bicep/linter/outputs-should-not-contain-secrets)) |listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01')|
output secondaryKey string = secondaryKeyIntermediateVar

var varWithOverlappingOutput = 'hello'
param paramWithOverlappingOutput string

output varWithOverlappingOutput string = varWithOverlappingOutput
output paramWithOverlappingOutput string = paramWithOverlappingOutput

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]

