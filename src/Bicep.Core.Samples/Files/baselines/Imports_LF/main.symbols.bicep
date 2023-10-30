import {foo, fizz, pop, greet} from 'modules/mod.bicep'
//@[13:17) TypeAlias fizz. Type: Type<{ property: pop? }[]>. Declaration start char: 13, length: 4
//@[19:22) TypeAlias pop. Type: Type<string>. Declaration start char: 19, length: 3
//@[08:11) Variable foo. Type: 'quux'. Declaration start char: 8, length: 3
//@[24:29) Function greet. Type: string => string. Declaration start char: 24, length: 5
import * as mod2 from 'modules/mod2.bicep'
//@[12:16) ImportedNamespace mod2. Type: mod2. Declaration start char: 7, length: 9
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
//@[36:57) Variable withInvalidIdentifier. Type: 'value'. Declaration start char: 2, length: 55
  refersToCopyVariable
//@[02:22) Variable refersToCopyVariable. Type: (object | object | object | object | object | object | object | object | object | object)[]. Declaration start char: 2, length: 20
} from 'modules/mod.json'

var aliasedFoo = foo
//@[04:14) Variable aliasedFoo. Type: 'quux'. Declaration start char: 0, length: 20
var aliasedBar = mod2.foo
//@[04:14) Variable aliasedBar. Type: 'bar'. Declaration start char: 0, length: 25

type fizzes = fizz[]
//@[05:11) TypeAlias fizzes. Type: Type<{ property: pop? }[][]>. Declaration start char: 0, length: 20

param fizzParam mod2.fizz
//@[06:15) Parameter fizzParam. Type: 'buzz'. Declaration start char: 0, length: 25
output magicWord pop = refersToCopyVariable[3].value
//@[07:16) Output magicWord. Type: string. Declaration start char: 0, length: 52

output greeting string = greet('friend')
//@[07:15) Output greeting. Type: string. Declaration start char: 0, length: 40

