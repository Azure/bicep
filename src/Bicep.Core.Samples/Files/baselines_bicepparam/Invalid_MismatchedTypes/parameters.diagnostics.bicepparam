using './main.bicep'

param string = 123
//@[15:18) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "123". (CodeDescription: none) |123|

param bool = 'hello'
//@[13:20) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "'hello'". (CodeDescription: none) |'hello'|

param int = false
//@[12:17) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "false". (CodeDescription: none) |false|

param object = ['abc', 'def']
//@[15:29) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "['abc', 'def']". (CodeDescription: none) |['abc', 'def']|

param array = {
//@[14:38) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "object". (CodeDescription: none) |{\n  isThis: 'correct?'\n}|
  isThis: 'correct?'
}

param stringAllowed = 'notTheAllowedValue'
//@[22:42) [BCP033 (Error)] Expected a value of type "'bar' | 'foo'" but the provided value is of type "'notTheAllowedValue'". (CodeDescription: none) |'notTheAllowedValue'|

