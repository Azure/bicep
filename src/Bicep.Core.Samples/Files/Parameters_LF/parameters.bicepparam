/*
This is a
multiline comment!
*/

// This is a single line comment

// using keyword for specifying a Bicep file
using './main.bicep/'

// parameter assignment to literals
param myInt = 42
param myStr = 'hello world!!'
param myBool = true

// parameter assignment to objects
param myObj = {
	name: 'vm1'
	location: 'westus'
}
param myComplexObj = {
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
	1
	2
	3
	4
	5
]
param myStrArr = [
	'ant'
	'bear'
	'cat'
	'dog'
]
param myComplexArr = [
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