import {foo, fizz} from 'modules/mod.bicep'
//@[00:247) ProgramExpression
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

var aliasedFoo = foo
//@[00:020) └─DeclaredVariableExpression { Name = aliasedFoo }
//@[17:020)   └─ImportedVariableReferenceExpression { Variable = foo }

type fizzes = fizz[]
//@[00:020) ├─DeclaredTypeExpression { Name = fizzes }
//@[14:020) | └─ArrayTypeExpression { Name = { property: pop? }[][] }
//@[14:018) |   └─ImportedTypeReferenceExpression { Name = fizz }

