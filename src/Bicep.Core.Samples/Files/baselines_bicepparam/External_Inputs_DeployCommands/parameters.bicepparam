using none

// single parameter
param singleParam = readCliArg('foo')

// single parameter with casting expression
param singleParamCast = bool(readCliArg('foo'))

// multiple parameters with different syntax
param foo = readCliArg('foo')
param bar = readEnvVar('bar')
param baz = readEnvVar('FIRST_ENV_VAR')

// single param with variable reference
var myVar = bool(readCliArg('myVar'))
param varRef = myVar

// variable reference chain
var a = readCliArg('a')
var b = a
param c = b

// param reference chain
param a1 = readCliArg('a')
param b1 = a1
param c1 = b1

// string interpolation
param first = int(readEnvVar('FIRST_ENV_VAR'))
param second = readEnvVar('SECOND_ENV_VAR')
param result = '${first} combined with ${second}'

// instance function call
param myParam = sys.readCliArg('myParam')

// check sanitized externaInputDefinition
param coolParam = readCliArg('sys&sons.cool#param provider')

param objectBody = {
  foo: readEnvVar('foo')
  bar: readEnvVar('bar')
  baz: 'blah'
}
