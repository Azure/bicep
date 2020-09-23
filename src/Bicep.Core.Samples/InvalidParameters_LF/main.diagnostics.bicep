/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
wrong
//@[0:5) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |wrong|

param myInt int
param
//@[5:5) [BCP013 (Error)] Expected a parameter identifier at this location. ||

param myBool bool

param missingType
//@[17:17) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: array, bool, int, object, string. ||

param myString2 string = 'string value'

param wrongDefaultValue string = 42
//@[33:35) [BCP027 (Error)] The parameter expects a default value of type string but provided value is of type int. |42|

param myInt2 int = 42
param noValueAfterColon int =   
//@[32:32) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

param myTruth bool = 'not a boolean'
//@[21:36) [BCP027 (Error)] The parameter expects a default value of type bool but provided value is of type 'not a boolean'. |'not a boolean'|
param myFalsehood bool = 'false'
//@[25:32) [BCP027 (Error)] The parameter expects a default value of type bool but provided value is of type 'false'. |'false'|

param wrongAssignmentToken string: 'hello'
//@[33:34) [BCP008 (Error)] Expected the '=' token, a parameter modifier, or a newline at this location. |:|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267) [BCP024 (Error)] The identifier exceeds the limit of 255. Reduce the length of the identifier. |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[36:37) [BCP019 (Error)] Expected a new line character at this location. |s|
//@[45:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'|

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[35:37) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: \$, \', \\, \n, \r, \t. |\s|

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[30:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'what\'s up doc?|

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[41:41) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[41:41) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[43:43) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[43:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}|
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[41:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'up|
//@[44:44) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[46:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[46:46) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
//@[46:46) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[49:49) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[49:50) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}|
//@[50:50) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[51:51) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[51:53) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |}?|

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|
//@[34:38) [BCP019 (Error)] Expected a new line character at this location. |this|
//@[53:54) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. |'|
//@[54:54) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. ||

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[46:49) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. |}!'|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[49:52) [BCP064 (Error)] Found unexpected tokens in interpolated expression. |b c|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[6:15) [BCP028 (Error)] Identifier 'wrongType' is declared multiple times. Remove or rename the duplicates. |wrongType|
//@[16:27) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |fluffyBunny|

// modifier on an invalid type
param someArray arra {
//@[16:20) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: array, bool, int, object, string. |arra|
  minLength: 3
  maxLength: 24
}

// duplicate modifier property
param duplicatedModifierProperty string {
  minLength: 3
//@[2:11) [BCP025 (Error)] The property 'minLength' is declared multiple times in this object. Remove or rename the duplicate properties. |minLength|
  minLength: 24
//@[2:11) [BCP025 (Error)] The property 'minLength' is declared multiple times in this object. Remove or rename the duplicate properties. |minLength|
}

// non-existent modifiers
param secureInt int {
  secure: true
//@[2:8) [BCP038 (Error)] The property 'secure' is not allowed on objects of type ParameterModifier<int>. Permissible properties include 'allowed', 'default', 'maxValue', 'metadata', 'minValue'. |secure|
  minLength: 3
//@[2:11) [BCP038 (Error)] The property 'minLength' is not allowed on objects of type ParameterModifier<int>. Permissible properties include 'allowed', 'default', 'maxValue', 'metadata', 'minValue'. |minLength|
  maxLength: 123
//@[2:11) [BCP038 (Error)] The property 'maxLength' is not allowed on objects of type ParameterModifier<int>. Permissible properties include 'allowed', 'default', 'maxValue', 'metadata', 'minValue'. |maxLength|
}

// wrong modifier value types
param wrongIntModifier int {
  default: true
//@[11:15) [BCP036 (Error)] The property 'default' expected a value of type int but the provided value is of type bool. |true|
  allowed: [
    'test'
//@[4:10) [BCP034 (Error)] The enclosing array expected an item of type int, but the provided item was of type 'test'. |'test'|
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type int, but the provided item was of type bool. |true|
  ]
  minValue: {
//@[12:17) [BCP036 (Error)] The property 'minValue' expected a value of type int but the provided value is of type object. |{\n  }|
  }
  maxValue: [
//@[12:17) [BCP036 (Error)] The property 'maxValue' expected a value of type int but the provided value is of type array. |[\n  ]|
  ]
  metadata: 'wrong'
//@[12:19) [BCP036 (Error)] The property 'metadata' expected a value of type ParameterModifierMetadata but the provided value is of type 'wrong'. |'wrong'|
}

// wrong metadata schema
param wrongMetadataSchema string {
  metadata: {
    description: true
//@[17:21) [BCP036 (Error)] The property 'description' expected a value of type string but the provided value is of type bool. |true|
  }
}

// expression in modifier
param expressionInModifier string {
  default: 2 + 3
//@[11:16) [BCP036 (Error)] The property 'default' expected a value of type string but the provided value is of type int. |2 + 3|
  maxLength: a + 2
//@[13:14) [BCP057 (Error)] The name 'a' does not exist in the current context. |a|
//@[13:18) [BCP032 (Error)] The value must be a compile-time constant. |a + 2|
  minLength: foo()
//@[13:16) [BCP057 (Error)] The name 'foo' does not exist in the current context. |foo|
//@[13:18) [BCP032 (Error)] The value must be a compile-time constant. |foo()|
  allowed: [
    i
//@[4:5) [BCP057 (Error)] The name 'i' does not exist in the current context. |i|
//@[4:5) [BCP032 (Error)] The value must be a compile-time constant. |i|
  ]
}

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[36:56) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramDefaultOneCycle|

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[37:58) [BCP080 (Error)] The expression is involved in a cycle (paramDefaultTwoCycle2 -> paramDefaultTwoCycle1). |paramDefaultTwoCycle2|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[37:58) [BCP080 (Error)] The expression is involved in a cycle (paramDefaultTwoCycle1 -> paramDefaultTwoCycle2). |paramDefaultTwoCycle1|

// 1-cycle in modifier params
param paramModifierOneCycle string {
  default: paramModifierOneCycle
//@[11:32) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramModifierOneCycle|
}

// 1-cycle in modifier with non-default property
param paramModifierSelfCycle string {
  allowed: [
    paramModifierSelfCycle
//@[4:26) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |paramModifierSelfCycle|
//@[4:26) [BCP032 (Error)] The value must be a compile-time constant. |paramModifierSelfCycle|
  ]
}

// 2-cycle in modifier params
param paramModifierTwoCycle1 string {
  default: paramModifierTwoCycle2
//@[11:33) [BCP080 (Error)] The expression is involved in a cycle (paramModifierTwoCycle2 -> paramModifierTwoCycle1). |paramModifierTwoCycle2|
}
param paramModifierTwoCycle2 string {
  default: paramModifierTwoCycle1
//@[11:33) [BCP080 (Error)] The expression is involved in a cycle (paramModifierTwoCycle1 -> paramModifierTwoCycle2). |paramModifierTwoCycle1|
}

// 2-cycle mixed param syntaxes
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[35:54) [BCP080 (Error)] The expression is involved in a cycle (paramMixedTwoCycle2 -> paramMixedTwoCycle1). |paramMixedTwoCycle2|
param paramMixedTwoCycle2 string {
  default: paramMixedTwoCycle1
//@[11:30) [BCP080 (Error)] The expression is involved in a cycle (paramMixedTwoCycle1 -> paramMixedTwoCycle2). |paramMixedTwoCycle1|
}

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
//@[40:49) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleVar|
param paramAccessingVar2 string {
  default: 'foo ${sampleVar} foo'
//@[18:27) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleVar|
}

param paramAccessingResource string = sampleResource
//@[38:52) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleResource|
//@[38:52) [BCP027 (Error)] The parameter expects a default value of type string but provided value is of type Microsoft.Foo/foos@2020-02-02. |sampleResource|
param paramAccessingResource2 string {
  default: base64(sampleResource.properties.foo)
//@[18:32) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |sampleResource|
}

param paramAccessingOutput string = sampleOutput
//@[36:48) [BCP058 (Error)] The name 'sampleOutput' is an output. Outputs cannot be referenced in expressions. |sampleOutput|
param paramAccessingOutput2 string {
  default: sampleOutput
//@[11:23) [BCP058 (Error)] The name 'sampleOutput' is an output. Outputs cannot be referenced in expressions. |sampleOutput|
}

param stringLiteral string {
  allowed: [
    'def'
  ]
}

param stringLiteral2 string {
  allowed: [
    'abc'
    'def'
  ]
  default: stringLiteral
}

param stringLiteral3 string {
  allowed: [
    'abc'
  ]
  default: stringLiteral2
//@[11:25) [BCP036 (Error)] The property 'default' expected a value of type 'abc' but the provided value is of type 'abc' | 'def'. |stringLiteral2|
}

// unterminated multi-line comment
/*    
//@[0:6) [BCP002 (Error)] The multi-line comment at this location is not terminated. Terminate it with the */ character sequence. |/*    |
