using none

import { exportedVariable, helperFunction } from 'main.bicep'
//@[09:25) Error exportedVariable. Type: error. Declaration start char: 9, length: 16
//@[27:41) Error helperFunction. Type: error. Declaration start char: 27, length: 14

param p12 = '${exportedVariable}-${externalInput('custom', helperFunction())}'
//@[06:09) ParameterAssignment p12. Type: error. Declaration start char: 0, length: 78

param p13 = '${exportedVariable}-${externalInput('custom', exportedVariable)}'
//@[06:09) ParameterAssignment p13. Type: error. Declaration start char: 0, length: 78

var myVar = 1 + 2
//@[04:09) Variable myVar. Type: 3. Declaration start char: 0, length: 17
param p = externalInput('sys.envVar', myVar)
//@[06:07) ParameterAssignment p. Type: any. Declaration start char: 0, length: 44

var x = 42
//@[04:05) Variable x. Type: 42. Declaration start char: 0, length: 10
var myVar2 = 'abcd-${x}'
//@[04:10) Variable myVar2. Type: 'abcd-42'. Declaration start char: 0, length: 24
param p2 = externalInput('sys.envVar', myVar2)
//@[06:08) ParameterAssignment p2. Type: any. Declaration start char: 0, length: 46

var myVar3 = 'test'
//@[04:10) Variable myVar3. Type: 'test'. Declaration start char: 0, length: 19
param p3 = externalInput(myVar3, myVar3)
//@[06:08) ParameterAssignment p3. Type: any. Declaration start char: 0, length: 40

var myVar4 = {
//@[04:10) Variable myVar4. Type: object. Declaration start char: 0, length: 33
  name: 'test'
}
param p4 = externalInput('sys.cli', myVar4)
//@[06:08) ParameterAssignment p4. Type: any. Declaration start char: 0, length: 43

var test = 'test'
//@[04:08) Variable test. Type: 'test'. Declaration start char: 0, length: 17
var myVar5 = {
//@[04:10) Variable myVar5. Type: object. Declaration start char: 0, length: 31
  name: test
}
param p5 = externalInput('sys.cli', {
//@[06:08) ParameterAssignment p5. Type: any. Declaration start char: 0, length: 57
  name: myVar5
})

param p6 = externalInput('custom', 'test')
//@[06:08) ParameterAssignment p6. Type: any. Declaration start char: 0, length: 42
param p7 = externalInput(p6)
//@[06:08) ParameterAssignment p7. Type: any. Declaration start char: 0, length: 28

param p8 = externalInput('custom', externalInput('custom', 'foo'))
//@[06:08) ParameterAssignment p8. Type: any. Declaration start char: 0, length: 66

param p9 = externalInput('custom',)
//@[06:08) ParameterAssignment p9. Type: any. Declaration start char: 0, length: 35
param p10 = externalInput(, 'test')
//@[06:09) ParameterAssignment p10. Type: any. Declaration start char: 0, length: 35
param p11 = externalInput('custom',foo')
//@[06:09) ParameterAssignment p11. Type: error. Declaration start char: 0, length: 40

