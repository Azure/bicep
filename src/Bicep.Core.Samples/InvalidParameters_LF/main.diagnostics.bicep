/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
wrong
//@[0:5) Error This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |wrong|

param myInt int
param
//@[5:5) Error Expected a parameter identifier at this location. ||

param myBool bool

param missingType
//@[17:17) Error Expected a parameter type at this location. Please specify one of the following types: array, bool, int, object, string. ||

param myString2 string = 'string value'

param wrongDefaultValue string = 42
//@[33:35) Error The parameter expects a default value of type 'string' but provided value is of type 'int'. |42|

param myInt2 int = 42
param noValueAfterColon int =   
//@[32:32) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

param myTruth bool = 'not a boolean'
//@[21:36) Error The parameter expects a default value of type 'bool' but provided value is of type 'string'. |'not a boolean'|
param myFalsehood bool = 'false'
//@[25:32) Error The parameter expects a default value of type 'bool' but provided value is of type 'string'. |'false'|

param wrongAssignmentToken string: 'hello'
//@[33:34) Error Expected the '=' token, a parameter modifier, or a newline at this location. |:|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267) Error The identifier exceeds the limit of 255. Reduce the length of the identifier. |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[36:37) Error Expected a new line character at this location. |s|
//@[45:46) Error The string at this location is not terminated due to an unexpected new line character. |'|

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[35:37) Error The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: \$, \', \\, \n, \r, \t. |\s|

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[30:46) Error The string at this location is not terminated due to an unexpected new line character. |'what\'s up doc?|

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[41:41) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[41:41) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[43:43) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[43:44) Error The string at this location is not terminated due to an unexpected new line character. |}|
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[41:44) Error The string at this location is not terminated due to an unexpected new line character. |'up|
//@[44:44) Error The string at this location is not terminated due to an unexpected new line character. ||

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[46:46) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[46:46) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[49:49) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[49:50) Error The string at this location is not terminated due to an unexpected new line character. |}|
//@[50:50) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[51:51) Error The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[51:53) Error The string at this location is not terminated due to an unexpected new line character. |}?|

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[34:38) Error Expected a new line character at this location. |this|
//@[53:54) Error The string at this location is not terminated due to an unexpected new line character. |'|
//@[54:54) Error The string at this location is not terminated due to an unexpected new line character. ||

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[46:49) Error Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |}!'|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[6:15) Error Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|

// modifier on an invalid type
param someArray arra {
//@[16:20) Error The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |arra|
  minLength: 3
  maxLength: 24
}

// duplicate modifier property
param duplicatedModifierProperty string {
  minLength: 3
//@[2:11) Error The property 'minLength' is declared multiple times in this object. Remove or rename the duplicate properties. |minLength|
  minLength: 24
//@[2:11) Error The property 'minLength' is declared multiple times in this object. Remove or rename the duplicate properties. |minLength|
}

// non-existent modifiers
param secureInt int {
  secure: true
//@[2:8) Error The property 'secure' is not allowed on objects of type 'ParameterModifier_int'. |secure|
  minLength: 3
//@[2:11) Error The property 'minLength' is not allowed on objects of type 'ParameterModifier_int'. |minLength|
  maxLength: 123
//@[2:11) Error The property 'maxLength' is not allowed on objects of type 'ParameterModifier_int'. |maxLength|
}

// wrong modifier value types
param wrongIntModifier int {
  default: true
//@[11:15) Error The property 'default' expected a value of type 'int' but the provided value is of type 'bool'. |true|
  allowedValues: [
    'test'
//@[4:10) Error The enclosing array expected an item of type 'int', but the provided item was of type 'string'. |'test'|
    true
//@[4:8) Error The enclosing array expected an item of type 'int', but the provided item was of type 'bool'. |true|
  ]
  minValue: {
//@[12:17) Error The property 'minValue' expected a value of type 'int' but the provided value is of type 'object'. |{\n  }|
  }
  maxValue: [
//@[12:17) Error The property 'maxValue' expected a value of type 'int' but the provided value is of type 'array'. |[\n  ]|
  ]
  metadata: 'wrong'
//@[12:19) Error The property 'metadata' expected a value of type 'ParameterModifierMetadata' but the provided value is of type 'string'. |'wrong'|
}

// wrong metadata schema
param wrongMetadataSchema string {
  metadata: {
    description: true
//@[17:21) Error The property 'description' expected a value of type 'string' but the provided value is of type 'bool'. |true|
  }
}

// expression in modifier
param expressionInModifier string {
  default: 2 + 3
//@[11:16) Error The property 'default' expected a value of type 'string' but the provided value is of type 'int'. |2 + 3|
  maxLength: a + 2
//@[13:14) Error The name 'a' does not exist in the current context. |a|
//@[13:18) Error The value must be a compile-time constant. |a + 2|
  minLength: foo()
//@[13:16) Error The name 'foo' does not exist in the current context. |foo|
//@[13:18) Error The value must be a compile-time constant. |foo()|
  allowedValues: [
    i
//@[4:5) Error The name 'i' does not exist in the current context. |i|
//@[4:5) Error The value must be a compile-time constant. |i|
  ]
}

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[36:56) Error The expression is involved in a cycle. |paramDefaultOneCycle|

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[37:58) Error The expression is involved in a cycle. |paramDefaultTwoCycle2|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[37:58) Error The expression is involved in a cycle. |paramDefaultTwoCycle1|

// 1-cycle in modifier params
param paramModifierOneCycle string {
  default: paramModifierOneCycle
//@[11:32) Error The expression is involved in a cycle. |paramModifierOneCycle|
}

// 1-cycle in modifier with non-default property
param paramModifierSelfCycle string {
  allowedValues: [
    paramModifierSelfCycle
//@[4:26) Error The expression is involved in a cycle. |paramModifierSelfCycle|
//@[4:26) Error The value must be a compile-time constant. |paramModifierSelfCycle|
  ]
}

// 2-cycle in modifier params
param paramModifierTwoCycle1 string {
  default: paramModifierTwoCycle2
//@[11:33) Error The expression is involved in a cycle. |paramModifierTwoCycle2|
}
param paramModifierTwoCycle2 string {
  default: paramModifierTwoCycle1
//@[11:33) Error The expression is involved in a cycle. |paramModifierTwoCycle1|
}

// 2-cycle mixed param syntaxes
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[35:54) Error The expression is involved in a cycle. |paramMixedTwoCycle2|
param paramMixedTwoCycle2 string {
  default: paramMixedTwoCycle1
//@[11:30) Error The expression is involved in a cycle. |paramMixedTwoCycle1|
}

// unterminated multi-line comment
/*    
//@[0:6) Error The multi-line comment at this location is not terminated. Terminate it with the */ character sequence. |/*    |
