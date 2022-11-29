using './main.bicep'

param string = 123
//@[0:18) [BCP260 (Error)] The parameter "string" expects a value of type "string" but the provided value is of type "123". (CodeDescription: none) |param string = 123|

param bool = 'hello'
//@[0:20) [BCP260 (Error)] The parameter "bool" expects a value of type "bool" but the provided value is of type "'hello'". (CodeDescription: none) |param bool = 'hello'|

param int = false
//@[0:17) [BCP260 (Error)] The parameter "int" expects a value of type "int" but the provided value is of type "false". (CodeDescription: none) |param int = false|

param object = ['abc', 'def']
//@[0:29) [BCP260 (Error)] The parameter "object" expects a value of type "object" but the provided value is of type "['abc', 'def']". (CodeDescription: none) |param object = ['abc', 'def']|

param array = {
//@[0:38) [BCP260 (Error)] The parameter "array" expects a value of type "array" but the provided value is of type "object". (CodeDescription: none) |param array = {\n  isThis: 'correct?'\n}|
  isThis: 'correct?'
}

param stringAllowed = 'notTheAllowedValue'
//@[0:42) [BCP260 (Error)] The parameter "stringAllowed" expects a value of type "'bar' | 'foo'" but the provided value is of type "'notTheAllowedValue'". (CodeDescription: none) |param stringAllowed = 'notTheAllowedValue'|

