/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep'

// parameter assignment to literals
param myString = 'hello world!!'
param myInt = 42
param myBool = false

param numberOfVMs = 1

// parameter assignment to objects
param password = 'strongPassword'
param secretObject = {
  name : 'vm2'
  location : 'westus'
}
param storageSku = 'Standard_LRS'
param storageName = 'myStorage'
param someArray = [
  'a'
  'b'
  'c'
  'd'
]
param emptyMetadata = 'empty!'
param description = 'descriptive description'
param description2 = 'also descriptive'
param additionalMetadata = 'more metadata'
param someParameter = 'three'
param stringLiteral = 'abc'
param decoratedString = 'Apple'
param stringfromEnvironmentVariables = readEnvironmentVariable('stringEnvVariableName')
param intfromEnvironmentVariables = int(readEnvironmentVariable('intEnvVariableName'))
param boolfromEnvironmentVariables = bool(readEnvironmentVariable('boolEnvironmentVariable'))

