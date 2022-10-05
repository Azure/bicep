
// wrong declaration
bad

// incomplete #completionTest(7) -> empty
output 
//@[07:07) Output <missing>. Type: any. Declaration start char: 0, length: 7

var testSymbol = 42
//@[04:14) Variable testSymbol. Type: 42. Declaration start char: 0, length: 19

// #completionTest(28,29) -> symbols
output missingValueAndType = 
//@[07:26) Output missingValueAndType. Type: any. Declaration start char: 0, length: 29

// #completionTest(28,29) -> symbols
output missingValue string = 
//@[07:19) Output missingValue. Type: string. Declaration start char: 0, length: 29

// #completionTest(31,32) -> arrayPlusSymbols
output arrayCompletions array = 
//@[07:23) Output arrayCompletions. Type: array. Declaration start char: 0, length: 32

// #completionTest(33,34) -> objectPlusSymbols
output objectCompletions object = 
//@[07:24) Output objectCompletions. Type: object. Declaration start char: 0, length: 34

// #completionTest(29,30) -> boolPlusSymbols
output boolCompletions bool = 
//@[07:22) Output boolCompletions. Type: bool. Declaration start char: 0, length: 30

output foo
//@[07:10) Output foo. Type: any. Declaration start char: 0, length: 10

// space after identifier #completionTest(20) -> outputTypes
output spaceAfterId 
//@[07:19) Output spaceAfterId. Type: any. Declaration start char: 0, length: 20

// #completionTest(25) -> outputTypes
output spacesAfterCursor  
//@[07:24) Output spacesAfterCursor. Type: any. Declaration start char: 0, length: 26

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
output partialType obj
//@[07:18) Output partialType. Type: error. Declaration start char: 0, length: 22

// malformed identifier
output 2
//@[07:08) Output <error>. Type: any. Declaration start char: 0, length: 8

// malformed type
output malformedType 3
//@[07:20) Output malformedType. Type: any. Declaration start char: 0, length: 22

// malformed type but type check should still happen
output malformedType2 3 = 2 + null
//@[07:21) Output malformedType2. Type: any. Declaration start char: 0, length: 34

// malformed type assignment
output malformedAssignment 2 = 2
//@[07:26) Output malformedAssignment. Type: any. Declaration start char: 0, length: 32

// malformed type before assignment
output lol 2 = true
//@[07:10) Output lol. Type: any. Declaration start char: 0, length: 19

// wrong type + missing value
output foo fluffy
//@[07:10) Output foo. Type: error. Declaration start char: 0, length: 17

// missing value
output foo string
//@[07:10) Output foo. Type: string. Declaration start char: 0, length: 17

// missing value
output foo string =
//@[07:10) Output foo. Type: string. Declaration start char: 0, length: 19

// wrong string output values
output str string = true
//@[07:10) Output str. Type: string. Declaration start char: 0, length: 24
output str string = false
//@[07:10) Output str. Type: string. Declaration start char: 0, length: 25
output str string = [
//@[07:10) Output str. Type: string. Declaration start char: 0, length: 24
]
output str string = {
//@[07:10) Output str. Type: string. Declaration start char: 0, length: 24
}
output str string = 52
//@[07:10) Output str. Type: string. Declaration start char: 0, length: 22

// wrong int output values
output i int = true
//@[07:08) Output i. Type: int. Declaration start char: 0, length: 19
output i int = false
//@[07:08) Output i. Type: int. Declaration start char: 0, length: 20
output i int = [
//@[07:08) Output i. Type: int. Declaration start char: 0, length: 19
]
output i int = }
//@[07:08) Output i. Type: int. Declaration start char: 0, length: 16
}
output i int = 'test'
//@[07:08) Output i. Type: int. Declaration start char: 0, length: 21

// wrong bool output values
output b bool = [
//@[07:08) Output b. Type: bool. Declaration start char: 0, length: 20
]
output b bool = {
//@[07:08) Output b. Type: bool. Declaration start char: 0, length: 20
}
output b bool = 32
//@[07:08) Output b. Type: bool. Declaration start char: 0, length: 18
output b bool = 'str'
//@[07:08) Output b. Type: bool. Declaration start char: 0, length: 21

// wrong array output values
output arr array = 32
//@[07:10) Output arr. Type: array. Declaration start char: 0, length: 21
output arr array = true
//@[07:10) Output arr. Type: array. Declaration start char: 0, length: 23
output arr array = false
//@[07:10) Output arr. Type: array. Declaration start char: 0, length: 24
output arr array = {
//@[07:10) Output arr. Type: array. Declaration start char: 0, length: 23
}
output arr array = 'str'
//@[07:10) Output arr. Type: array. Declaration start char: 0, length: 24

// wrong object output values
output o object = 32
//@[07:08) Output o. Type: object. Declaration start char: 0, length: 20
output o object = true
//@[07:08) Output o. Type: object. Declaration start char: 0, length: 22
output o object = false
//@[07:08) Output o. Type: object. Declaration start char: 0, length: 23
output o object = [
//@[07:08) Output o. Type: object. Declaration start char: 0, length: 22
]
output o object = 'str'
//@[07:08) Output o. Type: object. Declaration start char: 0, length: 23

// a few expression cases
output exp string = 2 + 3
//@[07:10) Output exp. Type: string. Declaration start char: 0, length: 25
output union string = true ? 's' : 1
//@[07:12) Output union. Type: string. Declaration start char: 0, length: 36
output bad int = true && !4
//@[07:10) Output bad. Type: int. Declaration start char: 0, length: 27
output deeper bool = true ? -true : (14 && 's') + 10
//@[07:13) Output deeper. Type: bool. Declaration start char: 0, length: 52

output myOutput string = 'hello'
//@[07:15) Output myOutput. Type: string. Declaration start char: 0, length: 32
var attemptToReferenceAnOutput = myOutput
//@[04:30) Variable attemptToReferenceAnOutput. Type: error. Declaration start char: 0, length: 41

@sys.maxValue(20)
@minValue(10)
output notAttachableDecorators int = 32
//@[07:30) Output notAttachableDecorators. Type: int. Declaration start char: 0, length: 73

// nested loops inside output loops are not supported
output noNestedLoops array = [for thing in things: {
//@[34:39) Local thing. Type: any. Declaration start char: 34, length: 5
//@[07:20) Output noNestedLoops. Type: array. Declaration start char: 0, length: 110
  something: [
    [for thing in things: true]
//@[09:14) Local thing. Type: any. Declaration start char: 9, length: 5
  ]
}]

// loops in inner properties inside outputs are not supported
output noInnerLoopsInOutputs object = {
//@[07:28) Output noInnerLoopsInOutputs. Type: object. Declaration start char: 0, length: 74
  a: [for i in range(0,10): i]
//@[10:11) Local i. Type: int. Declaration start char: 10, length: 1
}
output noInnerLoopsInOutputs2 object = {
//@[07:29) Output noInnerLoopsInOutputs2. Type: object. Declaration start char: 0, length: 116
  a: [for i in range(0,10): {
//@[10:11) Local i. Type: int. Declaration start char: 10, length: 1
    b: [for j in range(0,10): i+j]
//@[12:13) Local j. Type: int. Declaration start char: 12, length: 1
  }]
}

//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:11) Resource kv. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 90
  name: 'testkeyvault'
}

output keyVaultSecretOutput string = kv.getSecret('mySecret')
//@[07:27) Output keyVaultSecretOutput. Type: string. Declaration start char: 0, length: 61
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
//@[07:39) Output keyVaultSecretInterpolatedOutput. Type: string. Declaration start char: 0, length: 78
output keyVaultSecretObjectOutput object = {
//@[07:33) Output keyVaultSecretObjectOutput. Type: object. Declaration start char: 0, length: 83
  secret: kv.getSecret('mySecret')
}
output keyVaultSecretArrayOutput array = [
//@[07:32) Output keyVaultSecretArrayOutput. Type: array. Declaration start char: 0, length: 73
  kv.getSecret('mySecret')
]
output keyVaultSecretArrayInterpolatedOutput array = [
//@[07:44) Output keyVaultSecretArrayInterpolatedOutput. Type: array. Declaration start char: 0, length: 90
  '${kv.getSecret('mySecret')}'
]

// WARNING!!!!! dangling decorators

// #completionTest(1) -> decoratorsPlusNamespace
@
// #completionTest(5) -> decorators
@sys.

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
