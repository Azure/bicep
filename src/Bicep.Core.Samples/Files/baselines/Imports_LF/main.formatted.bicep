import { foo, fizz, pop, greet } from 'modules/mod.bicep'
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

import { fooFunc, fooVar, fooType } from 'modules/deprecations.bicep'

var aliasedFoo = foo
var aliasedBar = mod2.foo

type fizzes = fizz[]

param fizzParam mod2.fizz
output magicWord pop = refersToCopyVariable[3].value

output greeting string = greet('friend')
