using none

var myVar = 1 + 2
param p = externalInput('sys.envVar', myVar)

var x = 42
var myVar2 = 'abcd-${x}'
param p2 = externalInput('sys.envVar', myVar2)

var myVar3 = 'test'
param p3 = externalInput(myVar3, myVar3)

var myVar4 = {
  name: 'test'
}
param p4 = externalInput('sys.cli', myVar4)

var test = 'test'
var myVar5 = {
  name: test
}
param p5 = externalInput('sys.cli', {
  name: myVar5
})

param p6 = externalInput('custom', 'test')
param p7 = externalInput(p6)

param p8 = externalInput('custom', externalInput('custom', 'foo'))
