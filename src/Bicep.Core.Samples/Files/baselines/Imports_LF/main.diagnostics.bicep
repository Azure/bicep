import {foo, fizz} from 'modules/mod.bicep'
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

var aliasedFoo = foo
//@[4:14) [no-unused-vars (Warning)] Variable "aliasedFoo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |aliasedFoo|

type fizzes = fizz[]

