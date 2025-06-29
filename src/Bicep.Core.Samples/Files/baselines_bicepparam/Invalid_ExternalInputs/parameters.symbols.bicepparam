using none

var myVar = 1 + 2
//@[4:09) Variable myVar. Type: 3. Declaration start char: 0, length: 17
param p = externalInput('sys.envVar', myVar)
//@[6:07) ParameterAssignment p. Type: any. Declaration start char: 0, length: 44

var x = 42
//@[4:05) Variable x. Type: 42. Declaration start char: 0, length: 10
var myVar2 = 'abcd-${x}'
//@[4:10) Variable myVar2. Type: 'abcd-42'. Declaration start char: 0, length: 24
param p2 = externalInput('sys.envVar', myVar2)
//@[6:08) ParameterAssignment p2. Type: any. Declaration start char: 0, length: 46

var myVar3 = 'test'
//@[4:10) Variable myVar3. Type: 'test'. Declaration start char: 0, length: 19
param p3 = externalInput(myVar3, myVar3)
//@[6:08) ParameterAssignment p3. Type: any. Declaration start char: 0, length: 40

var myVar4 = {
//@[4:10) Variable myVar4. Type: object. Declaration start char: 0, length: 33
  name: 'test'
}
param p4 = externalInput('sys.cli', myVar4)
//@[6:08) ParameterAssignment p4. Type: any. Declaration start char: 0, length: 43

var test = 'test'
//@[4:08) Variable test. Type: 'test'. Declaration start char: 0, length: 17
var myVar5 = {
//@[4:10) Variable myVar5. Type: object. Declaration start char: 0, length: 31
  name: test
}
param p5 = externalInput('sys.cli', {
//@[6:08) ParameterAssignment p5. Type: any. Declaration start char: 0, length: 57
  name: myVar5
})

param p6 = externalInput('custom', 'test')
//@[6:08) ParameterAssignment p6. Type: any. Declaration start char: 0, length: 42
param p7 = externalInput(p6)
//@[6:08) ParameterAssignment p7. Type: any. Declaration start char: 0, length: 28

param p8 = externalInput('custom', externalInput('custom', 'foo'))
//@[6:08) ParameterAssignment p8. Type: any. Declaration start char: 0, length: 66

