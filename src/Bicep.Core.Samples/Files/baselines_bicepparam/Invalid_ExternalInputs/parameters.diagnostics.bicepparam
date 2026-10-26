using none

import { exportedVariable, helperFunction } from 'main.bicep'
//@[09:25) [BCP360 (Error)] The 'exportedVariable' symbol was not found in (or was not exported by) the imported template. (bicep https://aka.ms/bicep/core-diagnostics#BCP360) |exportedVariable|
//@[27:41) [BCP360 (Error)] The 'helperFunction' symbol was not found in (or was not exported by) the imported template. (bicep https://aka.ms/bicep/core-diagnostics#BCP360) |helperFunction|

param p12 = '${exportedVariable}-${externalInput('custom', helperFunction())}'
//@[15:31) [BCP063 (Error)] The name "exportedVariable" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |exportedVariable|
//@[59:73) [BCP059 (Error)] The name "helperFunction" is not a function. (bicep https://aka.ms/bicep/core-diagnostics#BCP059) |helperFunction|

param p13 = '${exportedVariable}-${externalInput('custom', exportedVariable)}'
//@[15:31) [BCP063 (Error)] The name "exportedVariable" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |exportedVariable|
//@[59:75) [BCP063 (Error)] The name "exportedVariable" is not a parameter, variable, resource or module. (bicep https://aka.ms/bicep/core-diagnostics#BCP063) |exportedVariable|

var myVar = 1 + 2
param p = externalInput('sys.envVar', myVar)
//@[38:43) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |myVar|

var x = 42
var myVar2 = 'abcd-${x}'
param p2 = externalInput('sys.envVar', myVar2)
//@[39:45) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |myVar2|

var myVar3 = 'test'
param p3 = externalInput(myVar3, myVar3)
//@[25:31) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |myVar3|
//@[33:39) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |myVar3|

var myVar4 = {
  name: 'test'
}
param p4 = externalInput('sys.cli', myVar4)
//@[36:42) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |myVar4|

var test = 'test'
var myVar5 = {
  name: test
}
param p5 = externalInput('sys.cli', {
  name: myVar5
//@[08:14) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |myVar5|
})

param p6 = externalInput('custom', 'test')
param p7 = externalInput(p6)
//@[25:27) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |p6|

param p8 = externalInput('custom', externalInput('custom', 'foo'))
//@[35:65) [BCP032 (Error)] The value must be a compile-time constant. (bicep https://aka.ms/bicep/core-diagnostics#BCP032) |externalInput('custom', 'foo')|

param p9 = externalInput('custom',)
//@[34:35) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |)|
param p10 = externalInput(, 'test')
//@[26:27) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |,|
param p11 = externalInput('custom',foo')
//@[25:40) [BCP071 (Error)] Expected 1 to 2 arguments, but got 3. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |('custom',foo')|
//@[35:38) [BCP057 (Error)] The name "foo" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |foo|
//@[38:38) [BCP236 (Error)] Expected a new line or comma character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP236) ||
//@[38:40) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (bicep https://aka.ms/bicep/core-diagnostics#BCP004) |')|
//@[40:40) [BCP018 (Error)] Expected the ")" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

