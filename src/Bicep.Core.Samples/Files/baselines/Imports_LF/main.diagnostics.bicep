import {foo, fizz, pop, greet} from 'modules/mod.bicep'
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

import { fooFunc, fooVar, fooType } from 'modules/deprecations.bicep'
//@[09:16) [no-deprecated-dependencies (Warning)] Symbol 'fooFunc' has been marked as deprecated, and should not be used. Reason: 'deprecated func'. (bicep core linter https://aka.ms/bicep/linter/no-deprecated-dependencies) |fooFunc|
//@[18:24) [no-deprecated-dependencies (Warning)] Symbol 'fooVar' has been marked as deprecated, and should not be used. Reason: 'deprecated var'. (bicep core linter https://aka.ms/bicep/linter/no-deprecated-dependencies) |fooVar|
//@[26:33) [no-deprecated-dependencies (Warning)] Symbol 'fooType' has been marked as deprecated, and should not be used. Reason: 'deprecated type'. (bicep core linter https://aka.ms/bicep/linter/no-deprecated-dependencies) |fooType|

var aliasedFoo = foo
//@[04:14) [no-unused-vars (Warning)] Variable "aliasedFoo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter/no-unused-vars) |aliasedFoo|
var aliasedBar = mod2.foo
//@[04:14) [no-unused-vars (Warning)] Variable "aliasedBar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter/no-unused-vars) |aliasedBar|

type fizzes = fizz[]

param fizzParam mod2.fizz
//@[06:15) [no-unused-params (Warning)] Parameter "fizzParam" is declared but never used. (bicep core linter https://aka.ms/bicep/linter/no-unused-params) |fizzParam|
output magicWord pop = refersToCopyVariable[3].value

output greeting string = greet('friend')

