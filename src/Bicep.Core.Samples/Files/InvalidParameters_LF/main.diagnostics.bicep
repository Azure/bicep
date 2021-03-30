/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
wrong
//@[0:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |wrong|

param myInt int
param
//@[5:5) [BCP013 (Error)] Expected a parameter identifier at this location. ||

param 3
//@[6:7) [BCP013 (Error)] Expected a parameter identifier at this location. |3|
//@[7:7) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||
param % string
//@[6:7) [BCP013 (Error)] Expected a parameter identifier at this location. |%|
param % string 3 = 's'
//@[6:7) [BCP013 (Error)] Expected a parameter identifier at this location. |%|
//@[15:16) [BCP008 (Error)] Expected the "=" token, a parameter modifier, or a newline at this location. |3|

param myBool bool

param missingType
//@[17:17) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 
//@[32:32) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	
//@[30:30) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// #completionTest(20) -> paramTypes
param trailingSpace  
//@[21:21) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str
//@[18:21) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |str|

param malformedType 44
//@[20:22) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |44|

// malformed type but type check should still happen
param malformedType2 44 = f
//@[21:23) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |44|
//@[26:27) [BCP057 (Error)] The name "f" does not exist in the current context. |f|

// malformed type but type check should still happen
param malformedModifier 44 {
//@[24:26) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". |44|
//@[27:44) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: 's'\n}|
  secure: 's'
//@[10:13) [BCP036 (Error)] The property "secure" expected a value of type "bool" but the provided value is of type "'s'". |'s'|
}

param myString2 string = 'string value'

param wrongDefaultValue string = 42
//@[33:35) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "int". |42|

param myInt2 int = 42
param noValueAfterColon int =   
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

param myTruth bool = 'not a boolean'
//@[21:36) [BCP027 (Error)] The parameter expects a default value of type "bool" but provided value is of type "'not a boolean'". |'not a boolean'|
param myFalsehood bool = 'false'
//@[25:32) [BCP027 (Error)] The parameter expects a default value of type "bool" but provided value is of type "'false'". |'false'|

param wrongAssignmentToken string: 'hello'
//@[33:34) [BCP008 (Error)] Expected the "=" token, a parameter modifier, or a newline at this location. |:|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267) [BCP024 (Error)] The identifier exceeds the limit of 255. Reduce the length of the identifier. |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[36:37) [BCP019 (Error)] Expected a new line character at this location. |s|
//@[45:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'|

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[35:37) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". |\s|

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[30:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'what\'s up doc?|

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[41:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[41:41) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[43:43) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[43:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}|
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[41:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'up|
//@[44:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[46:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[46:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[49:49) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[49:50) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}|
//@[50:50) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[51:51) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[51:53) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}?|

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|
//@[33:34) [BCP087 (Error)] Array and object literals are not allowed here. |{|
//@[53:54) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'|
//@[54:54) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[46:49) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |}!'|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[49:52) [BCP064 (Error)] Found unexpected tokens in interpolated expression. |b c|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[6:15) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |fluffyBunny|

// modifier on an invalid type
param someArray arra {
//@[16:20) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |arra|
//@[21:55) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator arra
//@[29:33) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". |arra|

// duplicate modifier property
param duplicatedModifierProperty string {
//@[40:74) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minLength: 3\n  minLength: 24\n}|
  minLength: 3
//@[2:11) [BCP025 (Error)] The property "minLength" is declared multiple times in this object. Remove or rename the duplicate properties. |minLength|
  minLength: 24
//@[2:11) [BCP025 (Error)] The property "minLength" is declared multiple times in this object. Remove or rename the duplicate properties. |minLength|
}

// non-existent modifiers
param secureInt int {
//@[20:70) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: true\n  minLength: 3\n  maxLength: 123\n}|
  secure: true
//@[2:8) [BCP038 (Error)] The property "secure" is not allowed on objects of type "ParameterModifier<int>". Permissible properties include "allowed", "default", "maxValue", "metadata", "minValue". |secure|
  minLength: 3
//@[2:11) [BCP038 (Error)] The property "minLength" is not allowed on objects of type "ParameterModifier<int>". Permissible properties include "allowed", "default", "maxValue", "metadata", "minValue". |minLength|
  maxLength: 123
//@[2:11) [BCP038 (Error)] The property "maxLength" is not allowed on objects of type "ParameterModifier<int>". Permissible properties include "allowed", "default", "maxValue", "metadata", "minValue". |maxLength|
}

@secure()
//@[0:9) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". |@secure()|
@minLength(3)
//@[0:13) [BCP124 (Error)] The decorator "minLength" can only be attached to targets of type "array | string", but the target has type "int". |@minLength(3)|
@maxLength(123)
//@[0:15) [BCP124 (Error)] The decorator "maxLength" can only be attached to targets of type "array | string", but the target has type "int". |@maxLength(123)|
param secureIntWithDecorator int

// wrong modifier value types
param wrongIntModifier int {
//@[27:139) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: true\n  allowed: [\n    'test'\n    true\n  ]\n  minValue: {\n  }\n  maxValue: [\n  ]\n  metadata: 'wrong'\n}|
  default: true
//@[11:15) [BCP036 (Error)] The property "default" expected a value of type "int" but the provided value is of type "bool". |true|
  allowed: [
    'test'
//@[4:10) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'test'". |'test'|
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". |true|
  ]
  minValue: {
//@[12:17) [BCP036 (Error)] The property "minValue" expected a value of type "int" but the provided value is of type "object". |{\n  }|
  }
  maxValue: [
//@[12:17) [BCP036 (Error)] The property "maxValue" expected a value of type "int" but the provided value is of type "array". |[\n  ]|
  ]
  metadata: 'wrong'
//@[12:19) [BCP036 (Error)] The property "metadata" expected a value of type "ParameterModifierMetadata" but the provided value is of type "'wrong'". |'wrong'|
}

@allowed([
  'test'
//@[2:8) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'test'". |'test'|
  true
//@[2:6) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". |true|
])
@minValue({
//@[10:13) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "int". |{\n}|
})
@maxValue([
//@[10:13) [BCP070 (Error)] Argument of type "array" is not assignable to parameter of type "int". |[\n]|
])
@metadata('wrong')
//@[10:17) [BCP070 (Error)] Argument of type "'wrong'" is not assignable to parameter of type "object". |'wrong'|
param wrongIntModifierWithDecorator int = true
//@[42:46) [BCP027 (Error)] The parameter expects a default value of type "int" but provided value is of type "bool". |true|

@metadata(any([]))
//@[10:17) [BCP032 (Error)] The value must be a compile-time constant. |any([])|
@allowed(any(2))
//@[9:15) [BCP032 (Error)] The value must be a compile-time constant. |any(2)|
param fatalErrorInIssue1713
//@[27:27) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". ||

// wrong metadata schema
param wrongMetadataSchema string {
//@[33:76) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  metadata: {\n    description: true\n  }\n}|
  metadata: {
    description: true
//@[17:21) [BCP036 (Error)] The property "description" expected a value of type "string" but the provided value is of type "bool". |true|
  }
}

@metadata({
  description: true
//@[15:19) [BCP036 (Error)] The property "description" expected a value of type "string" but the provided value is of type "bool". |true|
})
param wrongMetadataSchemaWithDecorator string

// expression in modifier
param expressionInModifier string {
//@[34:179) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  // #completionTest(10,11) -> symbolsPlusParamDefaultFunctions\n  default: 2 + 3\n  maxLength: a + 2\n  minLength: foo()\n  allowed: [\n    i\n  ]\n}|
  // #completionTest(10,11) -> symbolsPlusParamDefaultFunctions
  default: 2 + 3
  maxLength: a + 2
//@[13:14) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
  minLength: foo()
//@[13:16) [BCP057 (Error)] The name "foo" does not exist in the current context. |foo|
  allowed: [
    i
//@[4:5) [BCP057 (Error)] The name "i" does not exist in the current context. |i|
  ]
}

@maxLength(a + 2)
//@[11:12) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
@minLength(foo())
//@[11:14) [BCP057 (Error)] The name "foo" does not exist in the current context. |foo|
@allowed([
  i
//@[2:3) [BCP057 (Error)] The name "i" does not exist in the current context. |i|
])
param expressionInModifierWithDecorator string = 2 + 3
//@[49:54) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "int". |2 + 3|

param nonCompileTimeConstant string {
//@[36:122) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  maxLength: 2 + 3\n  minLength: length([])\n  allowed: [\n    resourceGroup().id\n  ]\n}|
  maxLength: 2 + 3
//@[13:18) [BCP032 (Error)] The value must be a compile-time constant. |2 + 3|
  minLength: length([])
//@[13:23) [BCP032 (Error)] The value must be a compile-time constant. |length([])|
  allowed: [
    resourceGroup().id
//@[4:22) [BCP032 (Error)] The value must be a compile-time constant. |resourceGroup().id|
  ]
}

@maxLength(2 + 3)
//@[11:16) [BCP032 (Error)] The value must be a compile-time constant. |2 + 3|
@minLength(length([]))
//@[11:21) [BCP032 (Error)] The value must be a compile-time constant. |length([])|
@allowed([
  resourceGroup().id
//@[2:20) [BCP032 (Error)] The value must be a compile-time constant. |resourceGroup().id|
])
param nonCompileTimeConstantWithDecorator string


param emptyAllowedString string {
//@[32:49) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: []\n}|
  allowed: []
//@[11:13) [BCP099 (Error)] The "allowed" array must contain one or more items. |[]|
}

@allowed([])
//@[9:11) [BCP099 (Error)] The "allowed" array must contain one or more items. |[]|
param emptyAllowedStringWithDecorator string

param emptyAllowedInt int {
//@[26:43) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: []\n}|
  allowed: []
//@[11:13) [BCP099 (Error)] The "allowed" array must contain one or more items. |[]|
}

@allowed([])
//@[9:11) [BCP099 (Error)] The "allowed" array must contain one or more items. |[]|
param emptyAllowedIntWithDecorator int

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[36:56) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramDefaultOneCycle|

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[37:58) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle2" -> "paramDefaultTwoCycle1"). |paramDefaultTwoCycle2|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[37:58) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle1" -> "paramDefaultTwoCycle2"). |paramDefaultTwoCycle1|

// 1-cycle in modifier params
param paramModifierOneCycle string {
//@[35:71) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: paramModifierOneCycle\n}|
  default: paramModifierOneCycle
//@[11:32) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramModifierOneCycle|
}

// 1-cycle in modifier with non-default property
param paramModifierSelfCycle string {
//@[36:83) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: [\n    paramModifierSelfCycle\n  ]\n}|
  allowed: [
    paramModifierSelfCycle
//@[4:26) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramModifierSelfCycle|
  ]
}

@allowed([
  paramModifierSelfCycleWithDecorator
//@[2:37) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramModifierSelfCycleWithDecorator|
])
param paramModifierSelfCycleWithDecorator string

// 2-cycle in modifier params
param paramModifierTwoCycle1 string {
//@[36:73) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: paramModifierTwoCycle2\n}|
  default: paramModifierTwoCycle2
//@[11:33) [BCP080 (Error)] The expression is involved in a cycle ("paramModifierTwoCycle2" -> "paramModifierTwoCycle1"). |paramModifierTwoCycle2|
}
param paramModifierTwoCycle2 string {
//@[36:73) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: paramModifierTwoCycle1\n}|
  default: paramModifierTwoCycle1
//@[11:33) [BCP080 (Error)] The expression is involved in a cycle ("paramModifierTwoCycle1" -> "paramModifierTwoCycle2"). |paramModifierTwoCycle1|
}

// 2-cycle mixed param syntaxes
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[35:54) [BCP080 (Error)] The expression is involved in a cycle ("paramMixedTwoCycle2" -> "paramMixedTwoCycle1"). |paramMixedTwoCycle2|
param paramMixedTwoCycle2 string {
//@[33:67) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: paramMixedTwoCycle1\n}|
  default: paramMixedTwoCycle1
//@[11:30) [BCP080 (Error)] The expression is involved in a cycle ("paramMixedTwoCycle1" -> "paramMixedTwoCycle2"). |paramMixedTwoCycle1|
}

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[24:55) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02" does not have types available. |'Microsoft.Foo/foos@2020-02-02'|
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
//@[40:49) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleVar|
param paramAccessingVar2 string {
//@[32:69) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: 'foo ${sampleVar} foo'\n}|
  default: 'foo ${sampleVar} foo'
//@[18:27) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleVar|
}

param paramAccessingResource string = sampleResource
//@[38:52) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleResource|
//@[38:52) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "Microsoft.Foo/foos@2020-02-02". |sampleResource|
param paramAccessingResource2 string {
//@[37:89) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: base64(sampleResource.properties.foo)\n}|
  default: base64(sampleResource.properties.foo)
//@[18:32) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleResource|
}

param paramAccessingOutput string = sampleOutput
//@[36:48) [BCP058 (Error)] The name "sampleOutput" is an output. Outputs cannot be referenced in expressions. |sampleOutput|
param paramAccessingOutput2 string {
//@[35:62) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: sampleOutput\n}|
  default: sampleOutput
//@[11:23) [BCP058 (Error)] The name "sampleOutput" is an output. Outputs cannot be referenced in expressions. |sampleOutput|
}

param stringLiteral string {
//@[27:57) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: [\n    'def'\n  ]\n}|
  allowed: [
    'def'
  ]
}

param stringLiteral2 string {
//@[28:93) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: [\n    'abc'\n    'def'\n  ]\n  default: stringLiteral\n}|
  allowed: [
    'abc'
    'def'
  ]
  default: stringLiteral
}

param stringLiteral3 string {
//@[28:84) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: [\n    'abc'\n  ]\n  default: stringLiteral2\n}|
  allowed: [
    'abc'
  ]
  default: stringLiteral2
//@[11:25) [BCP036 (Error)] The property "default" expected a value of type "'abc'" but the provided value is of type "'abc' | 'def'". |stringLiteral2|
}

// #completionTest(6) -> empty
param 
//@[6:6) [BCP013 (Error)] Expected a parameter identifier at this location. ||

param stringModifierCompletions string {
//@[39:101) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  // #completionTest(0,1,2) -> stringModifierProperties\n  \n}|
  // #completionTest(0,1,2) -> stringModifierProperties
  
}

param intModifierCompletions int {
//@[33:92) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  // #completionTest(0,1,2) -> intModifierProperties\n  \n}|
  // #completionTest(0,1,2) -> intModifierProperties
  
}

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 
//@[47:47) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

param defaultValueCompletions string {
//@[37:396) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: [\n    'one'\n    'two'\n    'three'\n    // #completionTest(0,1,2,3,4) -> oneTwoThree\n    \n  ]\n  // #completionTest(10,11) -> oneTwoThreePlusSymbols\n  default: \n  \n  // #completionTest(9,10) -> booleanValues\n  secure: \n\n  metadata: {\n    // #completionTest(0,1,2,3) -> description\n    \n  }\n  // #completionTest(0,1,2) -> stringLengthConstraints\n  \n}|
  allowed: [
    'one'
    'two'
    'three'
    // #completionTest(0,1,2,3,4) -> oneTwoThree
    
  ]
  // #completionTest(10,11) -> oneTwoThreePlusSymbols
  default: 
//@[11:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
  
  // #completionTest(9,10) -> booleanValues
  secure: 
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

  metadata: {
    // #completionTest(0,1,2,3) -> description
    
  }
  // #completionTest(0,1,2) -> stringLengthConstraints
  
}

// invalid comma separator (array)
param commaOne string {
//@[22:174) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n    metadata: {\n      description: 'Name of Virtual Machine'\n    }\n    secure: true\n    allowed: [\n      'abc',\n      'def'\n    ]\n    default: 'abc'\n}|
    metadata: {
      description: 'Name of Virtual Machine'
    }
    secure: true
    allowed: [
      'abc',
//@[11:12) [BCP106 (Error)] Expected a new line character at this location. Commas are not used as separator delimiters. |,|
      'def'
    ]
    default: 'abc'
}

@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
//@[7:8) [BCP106 (Error)] Expected a new line character at this location. Commas are not used as separator delimiters. |,|
  'def'
])
param commaOneWithDecorator string

// invalid comma separator (object)
param commaTwo string {
//@[22:174) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n    metadata: {\n      description: 'Name of Virtual Machine'\n    },\n    secure: true\n    allowed: [\n      'abc'\n      'def'\n    ]\n    default: 'abc'\n}|
    metadata: {
      description: 'Name of Virtual Machine'
    },
//@[5:6) [BCP106 (Error)] Expected a new line character at this location. Commas are not used as separator delimiters. |,|
    secure: true
    allowed: [
      'abc'
      'def'
    ]
    default: 'abc'
}

@secure
//@[1:7) [BCP063 (Error)] The name "secure" is not a parameter, variable, resource or module. |secure|
@
//@[1:1) [BCP123 (Error)] Expected a namespace or decorator name at this location. ||
@&& xxx
//@[1:3) [BCP123 (Error)] Expected a namespace or decorator name at this location. |&&|
@sys
//@[1:4) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. |sys|
@paramAccessingVar
//@[1:18) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. |paramAccessingVar|
param incompleteDecorators string

@concat(1, 2)
//@[1:7) [BCP152 (Error)] Function "concat" cannot be used as a decorator. |concat|
@sys.concat('a', 'b')
//@[5:11) [BCP108 (Error)] The function "concat" does not exist in namespace "sys". Did you mean "concat"? |concat|
@secure()
// wrong target type
@minValue(20)
//@[0:13) [BCP124 (Error)] The decorator "minValue" can only be attached to targets of type "int", but the target has type "string". |@minValue(20)|
param someString string {
//@[24:25) [BCP131 (Error)] Parameter modifiers and decorators cannot be used together. Please use decorators only. |{|
	// using decorators and modifier at the same time
    secure: true
}

@allowed([
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". |true|
    10
    'foo'
//@[4:9) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'foo'". |'foo'|
])
@secure()
//@[0:9) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". |@secure()|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
@  
//@[3:3) [BCP123 (Error)] Expected a namespace or decorator name at this location. ||
// #completionTest(5, 6) -> intParameterDecorators
@sys.   
//@[8:8) [BCP020 (Error)] Expected a function or property name at this location. ||
param someInteger int = 20

@allowed([], [], 2)
//@[8:19) [BCP071 (Error)] Expected 1 argument, but got 3. |([], [], 2)|
// #completionTest(4) -> empty
@az.
//@[4:4) [BCP020 (Error)] Expected a function or property name at this location. ||
param tooManyArguments1 int = 20

@metadata({}, {}, true)
//@[9:23) [BCP071 (Error)] Expected 1 argument, but got 3. |({}, {}, true)|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
//@[1:2) [BCP057 (Error)] The name "m" does not exist in the current context. |m|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
//@[4:4) [BCP123 (Error)] Expected a namespace or decorator name at this location. ||
// #completionTest(5) -> stringParameterDecorators
@sys.
//@[5:5) [BCP020 (Error)] Expected a function or property name at this location. ||
param tooManyArguments2 string

@description(sys.concat(2))
//@[13:26) [BCP032 (Error)] The value must be a compile-time constant. |sys.concat(2)|
@allowed([for thing in []: 's'])
//@[9:31) [BCP032 (Error)] The value must be a compile-time constant. |[for thing in []: 's']|
//@[10:13) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource and module declarations, values of resource and module properties, or values of outputs. |for|
param nonConstantInDecorator string

@minValue(-length('s'))
//@[11:22) [BCP032 (Error)] The value must be a compile-time constant. |length('s')|
@metadata({
  bool: !true
//@[8:13) [BCP032 (Error)] The value must be a compile-time constant. |!true|
//@[8:13) [BCP032 (Error)] The value must be a compile-time constant. |!true|
})
param unaryMinusOnFunction int

@minLength(1)
//@[0:13) [BCP166 (Error)] Duplicate "minLength" decorator. |@minLength(1)|
@minLength(2)
//@[0:13) [BCP166 (Error)] Duplicate "minLength" decorator. |@minLength(2)|
@secure()
@maxLength(3)
//@[0:13) [BCP166 (Error)] Duplicate "maxLength" decorator. |@maxLength(3)|
@maxLength(4)
//@[0:13) [BCP166 (Error)] Duplicate "maxLength" decorator. |@maxLength(4)|
param duplicateDecorators string

@minLength(-1)
//@[11:13) [BCP168 (Error)] Length must not be a negative value. |-1|
@maxLength(-100)
//@[11:15) [BCP168 (Error)] Length must not be a negative value. |-100|
param invalidLength string

// unterminated multi-line comment
/*    
//@[0:7) [BCP002 (Error)] The multi-line comment at this location is not terminated. Terminate it with the */ character sequence. |/*    \n|

