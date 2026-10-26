using none

// single parameter
param singleParam = externalInput('sys.cli', 'foo')

// single parameter with casting expression
param singleParamCast = bool(externalInput('sys.cli', 'foo'))

// multiple parameters with different syntax
param foo = externalInput('sys.cli', 'foo')
param bar = externalInput('sys.envVar', 'bar')
param baz = externalInput('custom.binding', '__BINDING__')

// single param with variable reference
var myVar = bool(externalInput('sys.cli', 'myVar'))
param varRef = myVar

// object config
param objectConfig = externalInput('custom.tool', {
  path: '/path/to/file'
  isSecure: true
})

// variable reference chain
var a = externalInput('sys.cli', 'a')
var b = a
param c = b

// param reference chain
param a1 = externalInput('sys.cli', 'a')
param b1 = a1
param c1 = b1

// string interpolation
param first = int(externalInput('custom.binding', '__BINDING__'))
param second = externalInput('custom.binding', {
  path: '/path/to/file'
  isSecure: true
})
param result = '${first} combined with ${second}'

// instance function call
param myParam = sys.externalInput('sys.cli', 'myParam')

// check sanitized externaInputDefinition
param coolParam = externalInput('sys&sons.cool#param provider')

param objectBody = {
  foo: externalInput('custom.binding', 'foo')
  bar: externalInput('custom.binding', 'bar')
  baz: 'blah'
}

var poodle = 'toy'
param retriever = 'golden'
param concat = '${poodle}-${retriever}-${externalInput('sys.cli', 'foo')}'

import * as main2 from 'main2.bicep'
import { person, getPerson, getDefaultPerson } from 'main.bicep'

param principalIds = externalInput('sys.cli', 'principalIds')

var anotherPerson = {
  name: 'John'
  age: 21
}
param varPeople = [
  ...map(principalIds, id => {
    objectId: id
  })
  person
  anotherPerson
  getPerson('Bob', 30)
  getDefaultPerson()
  externalInput('custom.binding', 'foo')
]

var infra main2.InfraConfig = {
  storage: main2.storageConfig
  vm: main2.vmConfig
  tag: externalInput('custom.binding', 'bar')
}

param infraParam = infra
