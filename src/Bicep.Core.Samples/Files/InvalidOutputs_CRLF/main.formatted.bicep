// wrong declaration
bad

// incomplete #completionTest(7) -> empty
output 

var testSymbol = 42

// #completionTest(28,29) -> symbols
output missingValueAndType = 

// #completionTest(28,29) -> symbols
output missingValue string = 

// #completionTest(31,32) -> arrayPlusSymbols
output arrayCompletions array = 

// #completionTest(33,34) -> objectPlusSymbols
output objectCompletions object = 

// #completionTest(29,30) -> boolPlusSymbols
output boolCompletions bool = 

output foo

// space after identifier #completionTest(20) -> outputTypes
output spaceAfterId 

// #completionTest(25) -> outputTypes
output spacesAfterCursor  

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
output partialType obj

// malformed identifier
output 2

// malformed type
output malformedType 3

// malformed type but type check should still happen
output malformedType2 3 = 2 + null

// malformed type assignment
output malformedAssignment 2 = 2

// malformed type before assignment
output lol 2 = true

// wrong type + missing value
output foo fluffy

// missing value
output foo string

// missing value
output foo string =

// wrong string output values
output str string = true
output str string = false
output str string = []
output str string = {}
output str string = 52

// wrong int output values
output i int = true
output i int = false
output i int = []
output i int = }
}
output i int = 'test'

// wrong bool output values
output b bool = []
output b bool = {}
output b bool = 32
output b bool = 'str'

// wrong array output values
output arr array = 32
output arr array = true
output arr array = false
output arr array = {}
output arr array = 'str'

// wrong object output values
output o object = 32
output o object = true
output o object = false
output o object = []
output o object = 'str'

// a few expression cases
output exp string = 2 + 3
output union string = true ? 's' : 1
output bad int = true && !4
output deeper bool = true ? -true : (14 && 's') + 10

output myOutput string = 'hello'
var attemptToReferenceAnOutput = myOutput

@sys.maxValue(20)
@minValue(10)
output notAttachableDecorators int = 32

// nested loops inside output loops are not supported
output noNestedLoops array = [for thing in things: {
  something: [
    [for thing in things: true]
  ]
}]

// loops in inner properties inside outputs are not supported
output noInnerLoopsInOutputs object = {
  a: [for i in range(0, 10): i]
}
output noInnerLoopsInOutputs2 object = {
  a: [for i in range(0, 10): {
    b: [for j in range(0, 10): i + j]
  }]
}

//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

output keyVaultSecretOutput string = kv.getSecret('mySecret')
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
output keyVaultSecretObjectOutput object = {
  secret: kv.getSecret('mySecret')
}
output keyVaultSecretArrayOutput array = [
  kv.getSecret('mySecret')
]
output keyVaultSecretArrayInterpolatedOutput array = [
  '${kv.getSecret('mySecret')}'
]

// WARNING!!!!! dangling decorators

// #completionTest(1) -> decoratorsPlusNamespace
@
// #completionTest(5) -> decorators
@sys.

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
