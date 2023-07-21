/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep'

// parameter assignment to literals
param myString = 'hello world!!'
//@[6:14) ParameterAssignment myString. Type: 'hello world!!'. Declaration start char: 0, length: 32
param myInt = 42
//@[6:11) ParameterAssignment myInt. Type: 42. Declaration start char: 0, length: 16
param myBool = false
//@[6:12) ParameterAssignment myBool. Type: false. Declaration start char: 0, length: 20

param numberOfVMs = 1
//@[6:17) ParameterAssignment numberOfVMs. Type: 1. Declaration start char: 0, length: 21

// parameter assignment to objects
param password = 'strongPassword'
//@[6:14) ParameterAssignment password. Type: 'strongPassword'. Declaration start char: 0, length: 33
param secretObject = {
//@[6:18) ParameterAssignment secretObject. Type: object. Declaration start char: 0, length: 61
  name : 'vm2'
  location : 'westus'
}
param storageSku = 'Standard_LRS'
//@[6:16) ParameterAssignment storageSku. Type: 'Standard_LRS'. Declaration start char: 0, length: 33
param storageName = 'myStorage'
//@[6:17) ParameterAssignment storageName. Type: 'myStorage'. Declaration start char: 0, length: 31
param someArray = [
//@[6:15) ParameterAssignment someArray. Type: ['a', 'b', 'c', 'd']. Declaration start char: 0, length: 45
  'a'
  'b'
  'c'
  'd'
]
param emptyMetadata = 'empty!'
//@[6:19) ParameterAssignment emptyMetadata. Type: 'empty!'. Declaration start char: 0, length: 30
param description = 'descriptive description'
//@[6:17) ParameterAssignment description. Type: 'descriptive description'. Declaration start char: 0, length: 45
param description2 = 'also descriptive'
//@[6:18) ParameterAssignment description2. Type: 'also descriptive'. Declaration start char: 0, length: 39
param additionalMetadata = 'more metadata'
//@[6:24) ParameterAssignment additionalMetadata. Type: 'more metadata'. Declaration start char: 0, length: 42
param someParameter = 'three'
//@[6:19) ParameterAssignment someParameter. Type: 'three'. Declaration start char: 0, length: 29
param stringLiteral = 'abc'
//@[6:19) ParameterAssignment stringLiteral. Type: 'abc'. Declaration start char: 0, length: 27
param decoratedString = 'Apple'
//@[6:21) ParameterAssignment decoratedString. Type: 'Apple'. Declaration start char: 0, length: 31
param stringfromEnvironmentVariables = readEnvironmentVariable('stringEnvVariableName')
//@[6:36) ParameterAssignment stringfromEnvironmentVariables. Type: 'test'. Declaration start char: 0, length: 87
param intfromEnvironmentVariables = int(readEnvironmentVariable('intEnvVariableName'))
//@[6:33) ParameterAssignment intfromEnvironmentVariables. Type: int. Declaration start char: 0, length: 86
param boolfromEnvironmentVariables = bool(readEnvironmentVariable('boolEnvironmentVariable'))
//@[6:34) ParameterAssignment boolfromEnvironmentVariables. Type: bool. Declaration start char: 0, length: 93
param intfromEnvironmentVariablesDefault = int(readEnvironmentVariable('intDefaultEnvVariableName','12'))
//@[6:40) ParameterAssignment intfromEnvironmentVariablesDefault. Type: int. Declaration start char: 0, length: 105

