import { foo, fizz } from 'modules/mod.bicep'
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

var aliasedFoo = foo

type fizzes = fizz[]
