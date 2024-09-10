using 'main.bicep'

import * as foo from 'foo.bicep'
//@[16:32) [BCP104 (Error)] The referenced module has errors. (bicep https://aka.ms/bicep/core-diagnostics#BCP104) |from 'foo.bicep'|
import { bar } from 'foo.bicep'
//@[15:31) [BCP104 (Error)] The referenced module has errors. (bicep https://aka.ms/bicep/core-diagnostics#BCP104) |from 'foo.bicep'|

