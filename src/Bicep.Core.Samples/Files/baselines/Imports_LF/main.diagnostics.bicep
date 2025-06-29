import {foo, fizz, pop, greet} from 'modules/mod.bicep'
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
//@[36:57) [no-unused-imports (Warning)] Import "withInvalidIdentifier" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-imports) |withInvalidIdentifier|
  refersToCopyVariable
} from 'modules/mod.json'

var aliasedFoo = foo
//@[04:14) [no-unused-vars (Warning)] Variable "aliasedFoo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |aliasedFoo|
var aliasedBar = mod2.foo
//@[04:14) [no-unused-vars (Warning)] Variable "aliasedBar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |aliasedBar|

type fizzes = fizz[]

param fizzParam mod2.fizz
//@[06:15) [no-unused-params (Warning)] Parameter "fizzParam" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |fizzParam|
output magicWord pop = refersToCopyVariable[3].value

output greeting string = greet('friend')

