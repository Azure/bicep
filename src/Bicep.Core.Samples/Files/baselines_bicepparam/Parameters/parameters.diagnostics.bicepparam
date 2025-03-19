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
//@[17:33) [BCP418 (Warning)] The assignment target is expecting sensitive data but has been provided a non-sensitive value. Consider supplying the value as a secure parameter instead to prevent unauthorized disclosure to users who can view the template (via the portal, the CLI, or in source code). (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |'strongPassword'|
param secretObject = {
//@[21:61) [BCP418 (Warning)] The assignment target is expecting sensitive data but has been provided a non-sensitive value. Consider supplying the value as a secure parameter instead to prevent unauthorized disclosure to users who can view the template (via the portal, the CLI, or in source code). (bicep https://aka.ms/bicep/core-diagnostics#BCP418) |{\n  name : 'vm2'\n  location : 'westus'\n}|
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
param intfromEnvironmentVariablesDefault = int(readEnvironmentVariable('intDefaultEnvVariableName','12'))

