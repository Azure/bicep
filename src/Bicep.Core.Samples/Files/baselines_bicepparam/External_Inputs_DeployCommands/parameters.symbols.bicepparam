using none

// single parameter
param singleParam = readCliArg('foo')
//@[6:17) ParameterAssignment singleParam. Type: string. Declaration start char: 0, length: 37

// single parameter with casting expression
param singleParamCast = bool(readCliArg('foo'))
//@[6:21) ParameterAssignment singleParamCast. Type: bool. Declaration start char: 0, length: 47

// multiple parameters with different syntax
param foo = readCliArg('foo')
//@[6:09) ParameterAssignment foo. Type: string. Declaration start char: 0, length: 29
param bar = readEnvVar('bar')
//@[6:09) ParameterAssignment bar. Type: string. Declaration start char: 0, length: 29
param baz = readEnvVar('FIRST_ENV_VAR')
//@[6:09) ParameterAssignment baz. Type: string. Declaration start char: 0, length: 39

// single param with variable reference
var myVar = bool(readCliArg('myVar'))
//@[4:09) Variable myVar. Type: bool. Declaration start char: 0, length: 37
param varRef = myVar
//@[6:12) ParameterAssignment varRef. Type: bool. Declaration start char: 0, length: 20

// variable reference chain
var a = readCliArg('a')
//@[4:05) Variable a. Type: string. Declaration start char: 0, length: 23
var b = a
//@[4:05) Variable b. Type: string. Declaration start char: 0, length: 9
param c = b
//@[6:07) ParameterAssignment c. Type: string. Declaration start char: 0, length: 11

// param reference chain
param a1 = readCliArg('a')
//@[6:08) ParameterAssignment a1. Type: string. Declaration start char: 0, length: 26
param b1 = a1
//@[6:08) ParameterAssignment b1. Type: string. Declaration start char: 0, length: 13
param c1 = b1
//@[6:08) ParameterAssignment c1. Type: string. Declaration start char: 0, length: 13

// string interpolation
param first = int(readEnvVar('FIRST_ENV_VAR'))
//@[6:11) ParameterAssignment first. Type: int. Declaration start char: 0, length: 46
param second = readEnvVar('SECOND_ENV_VAR')
//@[6:12) ParameterAssignment second. Type: string. Declaration start char: 0, length: 43
param result = '${first} combined with ${second}'
//@[6:12) ParameterAssignment result. Type: string. Declaration start char: 0, length: 49

// instance function call
param myParam = sys.readCliArg('myParam')
//@[6:13) ParameterAssignment myParam. Type: string. Declaration start char: 0, length: 41

// check sanitized externaInputDefinition
param coolParam = readCliArg('sys&sons.cool#param provider')
//@[6:15) ParameterAssignment coolParam. Type: string. Declaration start char: 0, length: 60

param objectBody = {
//@[6:16) ParameterAssignment objectBody. Type: object. Declaration start char: 0, length: 90
  foo: readEnvVar('foo')
  bar: readEnvVar('bar')
  baz: 'blah'
}

