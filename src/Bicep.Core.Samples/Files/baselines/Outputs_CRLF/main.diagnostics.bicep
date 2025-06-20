
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
//@[17:22) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
]

output suchEmpty2 object = {
//@[18:24) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
}

@sys.description('object output description')
output obj object = {
//@[11:17) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
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
//@[13:18) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
  'pirates'
  'say'
   false ? 'arr2' : 'arr'
]

output rgLocation string = resourceGroup().location

output isWestUs bool = resourceGroup().location != 'westus' ? false : true

output expressionBasedIndexer string = {
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[27:86) [outputs-should-not-contain-secrets (Warning)] Outputs should not contain secrets. Found possible secret: function 'listKeys' (bicep core linter https://aka.ms/bicep/linter-diagnostics#outputs-should-not-contain-secrets) |listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01')|
output secondaryKey string = secondaryKeyIntermediateVar

var varWithOverlappingOutput = 'hello'
param paramWithOverlappingOutput string

output varWithOverlappingOutput string = varWithOverlappingOutput
output paramWithOverlappingOutput string = paramWithOverlappingOutput

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@[22:27) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

