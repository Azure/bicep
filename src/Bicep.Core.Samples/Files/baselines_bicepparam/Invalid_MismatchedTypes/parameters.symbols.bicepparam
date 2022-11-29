using './main.bicep'

param string = 123
//@[6:12) ParameterAssignment string. Type: 123. Declaration start char: 0, length: 18

param bool = 'hello'
//@[6:10) ParameterAssignment bool. Type: 'hello'. Declaration start char: 0, length: 20

param int = false
//@[6:09) ParameterAssignment int. Type: false. Declaration start char: 0, length: 17

param object = ['abc', 'def']
//@[6:12) ParameterAssignment object. Type: ['abc', 'def']. Declaration start char: 0, length: 29

param array = {
//@[6:11) ParameterAssignment array. Type: object. Declaration start char: 0, length: 38
  isThis: 'correct?'
}

param stringAllowed = 'notTheAllowedValue'
//@[6:19) ParameterAssignment stringAllowed. Type: 'notTheAllowedValue'. Declaration start char: 0, length: 42

