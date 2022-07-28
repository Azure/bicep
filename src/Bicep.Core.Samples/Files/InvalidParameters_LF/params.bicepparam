/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep'

// parameter assignment to literals
param myStr = 'hello world!!'
param myInt = 42
param myBool = true

param myString2 = 'overwritten value!'
param myTruth = false

// parameter assignment to objects
param foo = {
	name: 'vm1'
	location: 'westus'
}

param password = 'strongPassword'
param secretObject = {
    name: 'vm2'
    location: 'westus'
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
param negativeValues = -5
