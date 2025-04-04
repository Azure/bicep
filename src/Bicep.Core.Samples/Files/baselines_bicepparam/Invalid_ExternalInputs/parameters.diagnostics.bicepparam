using none

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

