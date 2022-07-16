/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep'

// parameter assignment to literals
param myInt = 42
//@[6:11) AssignedParameter myInt. Type: any. Declaration start char: 0, length: 16
param myStr = 'hello world!!'
//@[6:11) AssignedParameter myStr. Type: any. Declaration start char: 0, length: 29
param myBool = true
//@[6:12) AssignedParameter myBool. Type: any. Declaration start char: 0, length: 19

// parameter assignment to objects
param myObj = {
//@[6:11) AssignedParameter myObj. Type: any. Declaration start char: 0, length: 50
	name: 'vm1'
	location: 'westus'
}
param myComplexObj = {
//@[6:18) AssignedParameter myComplexObj. Type: any. Declaration start char: 0, length: 123
	enabled: true
	name: 'complex object!'
	priority: 3
	data: {
		a: 'b'
		c: [
			'd'
			'e'
		]
	}
}

// parameter assignment to arrays
param myIntArr = [
//@[6:14) AssignedParameter myIntArr. Type: any. Declaration start char: 0, length: 35
	1
	2
	3
	4
	5
]
param myStrArr = [
//@[6:14) AssignedParameter myStrArr. Type: any. Declaration start char: 0, length: 49
	'ant'
	'bear'
	'cat'
	'dog'
]
param myComplexArr = [
//@[6:18) AssignedParameter myComplexArr. Type: any. Declaration start char: 0, length: 75
	'eagle'
	21
	false
	{
		f: [
			'g'
			'h'
		]
	}
]
