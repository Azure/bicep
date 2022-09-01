/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep'

// parameter assignment to literals
param myString = 'hello world!!'
//@[6:14) AssignedParameter myString. Type: 'hello world!!'. Declaration start char: 0, length: 32
param myInt = 42
//@[6:11) AssignedParameter myInt. Type: int. Declaration start char: 0, length: 16
param myBool = true
//@[6:12) AssignedParameter myBool. Type: bool. Declaration start char: 0, length: 19

// parameter assignment to objects
param password = 'strongPassword'
//@[6:14) AssignedParameter password. Type: 'strongPassword'. Declaration start char: 0, length: 33
param secretObject = {
//@[6:18) AssignedParameter secretObject. Type: object. Declaration start char: 0, length: 65
    name : 'vm2'
    location : 'westus'
}
param storageSku = 'Standard_LRS'
//@[6:16) AssignedParameter storageSku. Type: 'Standard_LRS'. Declaration start char: 0, length: 33
param storageName = 'myStorage'
//@[6:17) AssignedParameter storageName. Type: 'myStorage'. Declaration start char: 0, length: 31
param someArray = [
//@[6:15) AssignedParameter someArray. Type: ('a' | 'b' | 'c' | 'd')[]. Declaration start char: 0, length: 53
    'a'
    'b'
    'c'
    'd'
]
param emptyMetadata = 'empty!'
//@[6:19) AssignedParameter emptyMetadata. Type: 'empty!'. Declaration start char: 0, length: 30
param description = 'descriptive description'
//@[6:17) AssignedParameter description. Type: 'descriptive description'. Declaration start char: 0, length: 45
param description2 = 'also descriptive'
//@[6:18) AssignedParameter description2. Type: 'also descriptive'. Declaration start char: 0, length: 39
param additionalMetadata = 'more metadata'
//@[6:24) AssignedParameter additionalMetadata. Type: 'more metadata'. Declaration start char: 0, length: 42
param someParameter = 'three'
//@[6:19) AssignedParameter someParameter. Type: 'three'. Declaration start char: 0, length: 29
param stringLiteral = 'abc'
//@[6:19) AssignedParameter stringLiteral. Type: 'abc'. Declaration start char: 0, length: 27
param decoratedString = 'Apple'
//@[6:21) AssignedParameter decoratedString. Type: 'Apple'. Declaration start char: 0, length: 31

