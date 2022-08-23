/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using'./main.bicep'

// parameter assignment to literals
parammyString='hello world!!'
parammyInt=42
parammyBool=true

// parameter assignment to objects
parampassword='strongPassword'
paramsecretObject={
  name: 'vm2'
  location: 'westus'
}
paramstorageSku='Standard_LRS'
paramstorageName='myStorage'
paramsomeArray=[
  'a'
  'b'
  'c'
  'd'
]
paramemptyMetadata='empty!'
paramdescription='descriptive description'
paramdescription2='also descriptive'
paramadditionalMetadata='more metadata'
paramsomeParameter='three'
paramstringLiteral='abc'
paramdecoratedString='Apple'
