import {foo, fizz, pop, greet} from 'modules/mod.bicep'
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

var aliasedFoo = foo
//@[4:14) [no-unused-vars (Warning)] Variable "aliasedFoo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |aliasedFoo|
var aliasedBar = mod2.foo
//@[4:14) [no-unused-vars (Warning)] Variable "aliasedBar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |aliasedBar|

type fizzes = fizz[]

param fizzParam mod2.fizz
//@[6:15) [no-unused-params (Warning)] Parameter "fizzParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |fizzParam|
output magicWord pop = refersToCopyVariable[3].value

output greeting string = greet('friend')

