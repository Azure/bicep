using none

// single parameter
param singleParam = externalInput('sys.cli', 'foo')
//@[6:17) ParameterAssignment singleParam. Type: any. Declaration start char: 0, length: 51

// single parameter with casting expression
param singleParamCast = bool(externalInput('sys.cli', 'foo'))
//@[6:21) ParameterAssignment singleParamCast. Type: bool. Declaration start char: 0, length: 61

// multiple parameters with different syntax
param foo = externalInput('sys.cli', 'foo')
//@[6:09) ParameterAssignment foo. Type: any. Declaration start char: 0, length: 43
param bar = externalInput('sys.envVar', 'bar')
//@[6:09) ParameterAssignment bar. Type: any. Declaration start char: 0, length: 46
param baz = externalInput('custom.binding', '__BINDING__')
//@[6:09) ParameterAssignment baz. Type: any. Declaration start char: 0, length: 58

// single param with variable reference
var myVar = bool(externalInput('sys.cli', 'myVar'))
//@[4:09) Variable myVar. Type: bool. Declaration start char: 0, length: 51
param varRef = myVar
//@[6:12) ParameterAssignment varRef. Type: bool. Declaration start char: 0, length: 20

// object config
param objectConfig = externalInput('custom.tool', {
//@[6:18) ParameterAssignment objectConfig. Type: any. Declaration start char: 0, length: 98
  path: '/path/to/file'
  isSecure: true
})

// variable reference chain
var a = externalInput('sys.cli', 'a')
//@[4:05) Variable a. Type: any. Declaration start char: 0, length: 37
var b = a
//@[4:05) Variable b. Type: any. Declaration start char: 0, length: 9
param c = b
//@[6:07) ParameterAssignment c. Type: any. Declaration start char: 0, length: 11

// param reference chain
param a1 = externalInput('sys.cli', 'a')
//@[6:08) ParameterAssignment a1. Type: any. Declaration start char: 0, length: 40
param b1 = a1
//@[6:08) ParameterAssignment b1. Type: any. Declaration start char: 0, length: 13
param c1 = b1
//@[6:08) ParameterAssignment c1. Type: any. Declaration start char: 0, length: 13

// string interpolation
param first = int(externalInput('custom.binding', '__BINDING__'))
//@[6:11) ParameterAssignment first. Type: int. Declaration start char: 0, length: 65
param second = externalInput('custom.binding', {
//@[6:12) ParameterAssignment second. Type: any. Declaration start char: 0, length: 99
    path: '/path/to/file'
    isSecure: true
})
param result = '${first} combined with ${second}'
//@[6:12) ParameterAssignment result. Type: string. Declaration start char: 0, length: 49

// instance function call
param myParam = sys.externalInput('sys.cli', 'myParam')
//@[6:13) ParameterAssignment myParam. Type: any. Declaration start char: 0, length: 55

// check sanitized externaInputDefinition
param coolParam = externalInput('sys&sons.cool#param provider')
//@[6:15) ParameterAssignment coolParam. Type: any. Declaration start char: 0, length: 63

param objectBody = {
//@[6:16) ParameterAssignment objectBody. Type: object. Declaration start char: 0, length: 132
  foo: externalInput('custom.binding', 'foo')
  bar: externalInput('custom.binding', 'bar')
  baz: 'blah'
}

