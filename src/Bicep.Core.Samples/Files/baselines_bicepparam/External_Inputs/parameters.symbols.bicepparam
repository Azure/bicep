using none

// single parameter
param singleParam = externalInput('sys.cli', 'foo')
//@[06:17) ParameterAssignment singleParam. Type: any. Declaration start char: 0, length: 51

// single parameter with casting expression
param singleParamCast = bool(externalInput('sys.cli', 'foo'))
//@[06:21) ParameterAssignment singleParamCast. Type: bool. Declaration start char: 0, length: 61

// multiple parameters with different syntax
param foo = externalInput('sys.cli', 'foo')
//@[06:09) ParameterAssignment foo. Type: any. Declaration start char: 0, length: 43
param bar = externalInput('sys.envVar', 'bar')
//@[06:09) ParameterAssignment bar. Type: any. Declaration start char: 0, length: 46
param baz = externalInput('custom.binding', '__BINDING__')
//@[06:09) ParameterAssignment baz. Type: any. Declaration start char: 0, length: 58

// single param with variable reference
var myVar = bool(externalInput('sys.cli', 'myVar'))
//@[04:09) Variable myVar. Type: bool. Declaration start char: 0, length: 51
param varRef = myVar
//@[06:12) ParameterAssignment varRef. Type: bool. Declaration start char: 0, length: 20

// object config
param objectConfig = externalInput('custom.tool', {
//@[06:18) ParameterAssignment objectConfig. Type: any. Declaration start char: 0, length: 98
  path: '/path/to/file'
  isSecure: true
})

// variable reference chain
var a = externalInput('sys.cli', 'a')
//@[04:05) Variable a. Type: any. Declaration start char: 0, length: 37
var b = a
//@[04:05) Variable b. Type: any. Declaration start char: 0, length: 9
param c = b
//@[06:07) ParameterAssignment c. Type: any. Declaration start char: 0, length: 11

// param reference chain
param a1 = externalInput('sys.cli', 'a')
//@[06:08) ParameterAssignment a1. Type: any. Declaration start char: 0, length: 40
param b1 = a1
//@[06:08) ParameterAssignment b1. Type: any. Declaration start char: 0, length: 13
param c1 = b1
//@[06:08) ParameterAssignment c1. Type: any. Declaration start char: 0, length: 13

// string interpolation
param first = int(externalInput('custom.binding', '__BINDING__'))
//@[06:11) ParameterAssignment first. Type: int. Declaration start char: 0, length: 65
param second = externalInput('custom.binding', {
//@[06:12) ParameterAssignment second. Type: any. Declaration start char: 0, length: 99
    path: '/path/to/file'
    isSecure: true
})
param result = '${first} combined with ${second}'
//@[06:12) ParameterAssignment result. Type: string. Declaration start char: 0, length: 49

// instance function call
param myParam = sys.externalInput('sys.cli', 'myParam')
//@[06:13) ParameterAssignment myParam. Type: any. Declaration start char: 0, length: 55

// check sanitized externaInputDefinition
param coolParam = externalInput('sys&sons.cool#param provider')
//@[06:15) ParameterAssignment coolParam. Type: any. Declaration start char: 0, length: 63

param objectBody = {
//@[06:16) ParameterAssignment objectBody. Type: object. Declaration start char: 0, length: 132
  foo: externalInput('custom.binding', 'foo')
  bar: externalInput('custom.binding', 'bar')
  baz: 'blah'
}

var poodle = 'toy'
//@[04:10) Variable poodle. Type: 'toy'. Declaration start char: 0, length: 18
param retriever = 'golden'
//@[06:15) ParameterAssignment retriever. Type: 'golden'. Declaration start char: 0, length: 26
param concat = '${poodle}-${retriever}-${externalInput('sys.cli', 'foo')}'
//@[06:12) ParameterAssignment concat. Type: string. Declaration start char: 0, length: 74

import * as main2 from 'main2.bicep'
//@[12:17) ImportedNamespace main2. Type: main2. Declaration start char: 7, length: 10
import { person, getPerson, getDefaultPerson } from 'main.bicep'
//@[09:15) Variable person. Type: object. Declaration start char: 9, length: 6
//@[17:26) Function getPerson. Type: (string, int) => { name: string, age: int }. Declaration start char: 17, length: 9
//@[28:44) Function getDefaultPerson. Type: () => { name: string, age: int }. Declaration start char: 28, length: 16

param principalIds = externalInput('sys.cli', 'principalIds')
//@[06:18) ParameterAssignment principalIds. Type: any. Declaration start char: 0, length: 61

var anotherPerson = {
//@[04:17) Variable anotherPerson. Type: object. Declaration start char: 0, length: 51
  name: 'John'
  age: 21
}
param varPeople = [
//@[06:15) ParameterAssignment varPeople. Type: array. Declaration start char: 0, length: 193
  ...map(principalIds, id => {
//@[23:25) Local id. Type: any. Declaration start char: 23, length: 2
    objectId: id
  })
  person
  anotherPerson
  getPerson('Bob', 30)
  getDefaultPerson()
  externalInput('custom.binding', 'foo')
]

var infra main2.InfraConfig = {
//@[04:09) Variable infra. Type: { storage: StorageConfig, vm: VmConfig, tag: string }. Declaration start char: 0, length: 135
  storage: main2.storageConfig
  vm: main2.vmConfig
  tag: externalInput('custom.binding', 'bar')
}

param infraParam = infra
//@[06:16) ParameterAssignment infraParam. Type: { storage: StorageConfig, vm: VmConfig, tag: string }. Declaration start char: 0, length: 24

