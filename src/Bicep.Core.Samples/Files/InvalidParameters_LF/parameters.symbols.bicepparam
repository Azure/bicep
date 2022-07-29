/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep'

// parameter assignment to literals
param myInt = 42
//@[6:11) AssignedParameter myInt. Type: int. Declaration start char: 0, length: 16
param myStr = 'hello world!!'
//@[6:11) AssignedParameter myStr. Type: 'hello world!!'. Declaration start char: 0, length: 29
param myBool = true
//@[6:12) AssignedParameter myBool. Type: bool. Declaration start char: 0, length: 19

// parameter assignment to objects
param myObj = {
//@[6:11) AssignedParameter myObj. Type: object. Declaration start char: 0, length: 53
	name: 'vm1'
	location: 'westus'
}
param myComplexObj = {
//@[6:18) AssignedParameter myComplexObj. Type: object. Declaration start char: 0, length: 134
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
//@[6:14) AssignedParameter myIntArr. Type: int[]. Declaration start char: 0, length: 41
	1
	2
	3
	4
	5
]
param myStrArr = [
//@[6:14) AssignedParameter myStrArr. Type: ('ant' | 'bear' | 'cat' | 'dog')[]. Declaration start char: 0, length: 54
	'ant'
	'bear'
	'cat'
	'dog'
]
param myComplexArr = [
//@[6:18) AssignedParameter myComplexArr. Type: array. Declaration start char: 0, length: 85
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
