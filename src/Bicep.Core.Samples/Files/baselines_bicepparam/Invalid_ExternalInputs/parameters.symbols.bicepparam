using none

var myVar = 1 + 2
param p = externalInput('sys.envVar', myVar)
//@[6:7) ParameterAssignment p. Type: any. Declaration start char: 0, length: 44

var x = 42
var myVar2 = 'abcd-${x}'
param p2 = externalInput('sys.envVar', myVar2)
//@[6:8) ParameterAssignment p2. Type: any. Declaration start char: 0, length: 46

var myVar3 = 'test'
param p3 = externalInput(myVar3, myVar3)
//@[6:8) ParameterAssignment p3. Type: any. Declaration start char: 0, length: 40

var myVar4 = {
  name: 'test'
}
param p4 = externalInput('sys.cli', myVar4)
//@[6:8) ParameterAssignment p4. Type: any. Declaration start char: 0, length: 43

var test = 'test'
var myVar5 = {
  name: test
}
param p5 = externalInput('sys.cli', {
//@[6:8) ParameterAssignment p5. Type: any. Declaration start char: 0, length: 57
  name: myVar5
})

