using './main.bicep'

param string = 123
//@[6:12) AssignedParameter string. Type: int. Declaration start char: 0, length: 18

param bool = 'hello'
//@[6:10) AssignedParameter bool. Type: 'hello'. Declaration start char: 0, length: 20

param int = false
//@[6:09) AssignedParameter int. Type: bool. Declaration start char: 0, length: 17

param object = ['abc', 'def']
//@[6:12) AssignedParameter object. Type: ('abc' | 'def')[]. Declaration start char: 0, length: 29

param array = {
//@[6:11) AssignedParameter array. Type: object. Declaration start char: 0, length: 38
  isThis: 'correct?'
}

param stringAllowed = 'notTheAllowedValue'
//@[6:19) AssignedParameter stringAllowed. Type: 'notTheAllowedValue'. Declaration start char: 0, length: 42

