using './main.bicep'

param string = 123
//@[15:18) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "123". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |123|

param bool = 'hello'
//@[13:20) [BCP033 (Error)] Expected a value of type "bool" but the provided value is of type "'hello'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'hello'|

param int = false
//@[12:17) [BCP033 (Error)] Expected a value of type "int" but the provided value is of type "false". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |false|

param object = ['abc', 'def']
//@[15:29) [BCP033 (Error)] Expected a value of type "object" but the provided value is of type "['abc', 'def']". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |['abc', 'def']|

param array = {
//@[14:38) [BCP033 (Error)] Expected a value of type "array" but the provided value is of type "object". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |{\n  isThis: 'correct?'\n}|
  isThis: 'correct?'
}

param stringAllowed = 'notTheAllowedValue'
//@[22:42) [BCP033 (Error)] Expected a value of type "'bar' | 'foo'" but the provided value is of type "'notTheAllowedValue'". (bicep https://aka.ms/bicep/core-diagnostics#BCP033) |'notTheAllowedValue'|

