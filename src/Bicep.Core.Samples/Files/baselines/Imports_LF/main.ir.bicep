import {foo, fizz, pop, greet} from 'modules/mod.bicep'
//@[00:407) ProgramExpression
import * as mod2 from 'modules/mod2.bicep'
import {
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
  refersToCopyVariable
} from 'modules/mod.json'

var aliasedFoo = foo
//@[00:020) ├─DeclaredVariableExpression { Name = aliasedFoo }
//@[17:020) | └─ImportedVariableReferenceExpression { Variable = foo }
var aliasedBar = mod2.foo
//@[00:025) ├─DeclaredVariableExpression { Name = aliasedBar }
//@[17:025) | └─WildcardImportVariablePropertyReferenceExpression { Variable = mod2.foo }

type fizzes = fizz[]
//@[00:020) ├─DeclaredTypeExpression { Name = fizzes }
//@[14:020) | └─ArrayTypeExpression { Name = { property: pop? }[][] }
//@[14:018) |   └─ImportedTypeReferenceExpression { Name = fizz }

param fizzParam mod2.fizz
//@[00:025) ├─DeclaredParameterExpression { Name = fizzParam }
//@[16:025) | └─WildcardImportTypePropertyReferenceExpression { Name = mod2.fizz }
output magicWord pop = refersToCopyVariable[3].value
//@[00:052) ├─DeclaredOutputExpression { Name = magicWord }
//@[17:020) | ├─ImportedTypeReferenceExpression { Name = pop }
//@[23:052) | └─PropertyAccessExpression { PropertyName = value }
//@[23:046) |   └─ArrayAccessExpression
//@[44:045) |     ├─IntegerLiteralExpression { Value = 3 }
//@[23:043) |     └─ImportedVariableReferenceExpression { Variable = refersToCopyVariable }

output greeting string = greet('friend')
//@[00:040) └─DeclaredOutputExpression { Name = greeting }
//@[16:022)   ├─AmbientTypeReferenceExpression { Name = string }
//@[25:040)   └─ImportedUserDefinedFunctionCallExpression { Name = greet }
//@[31:039)     └─StringLiteralExpression { Value = friend }

