/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[06:014) [no-unused-params (Warning)] Parameter "myString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myString|
wrong
//@[00:005) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |wrong|

param myInt int
//@[06:011) [no-unused-params (Warning)] Parameter "myInt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myInt|
param
//@[05:005) [BCP013 (Error)] Expected a parameter identifier at this location. (CodeDescription: none) ||

param 3
//@[06:007) [BCP013 (Error)] Expected a parameter identifier at this location. (CodeDescription: none) |3|
//@[07:007) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||
param % string
//@[06:007) [BCP013 (Error)] Expected a parameter identifier at this location. (CodeDescription: none) |%|
param % string 3 = 's'
//@[06:007) [BCP013 (Error)] Expected a parameter identifier at this location. (CodeDescription: none) |%|
//@[15:016) [BCP008 (Error)] Expected the "=" token, or a newline at this location. (CodeDescription: none) |3|

param myBool bool
//@[06:012) [no-unused-params (Warning)] Parameter "myBool" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myBool|

param missingType
//@[06:017) [no-unused-params (Warning)] Parameter "missingType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |missingType|
//@[17:017) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 
//@[06:031) [no-unused-params (Warning)] Parameter "missingTypeWithSpaceAfter" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |missingTypeWithSpaceAfter|
//@[32:032) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	
//@[06:029) [no-unused-params (Warning)] Parameter "missingTypeWithTabAfter" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |missingTypeWithTabAfter|
//@[30:030) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// #completionTest(20) -> paramTypes
param trailingSpace  
//@[06:019) [no-unused-params (Warning)] Parameter "trailingSpace" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |trailingSpace|
//@[21:021) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str
//@[06:017) [no-unused-params (Warning)] Parameter "partialType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |partialType|
//@[18:021) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |str|

param malformedType 44
//@[06:019) [no-unused-params (Warning)] Parameter "malformedType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |malformedType|
//@[20:022) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |44|

// malformed type but type check should still happen
param malformedType2 44 = f
//@[06:020) [no-unused-params (Warning)] Parameter "malformedType2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |malformedType2|
//@[21:023) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |44|
//@[26:027) [BCP057 (Error)] The name "f" does not exist in the current context. (CodeDescription: none) |f|

// malformed type but type check should still happen
@secure('s')
//@[07:012) [BCP071 (Error)] Expected 0 arguments, but got 1. (CodeDescription: none) |('s')|
param malformedModifier 44
//@[06:023) [no-unused-params (Warning)] Parameter "malformedModifier" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |malformedModifier|
//@[24:026) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |44|

param myString2 string = 'string value'
//@[06:015) [no-unused-params (Warning)] Parameter "myString2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myString2|

param wrongDefaultValue string = 42
//@[06:023) [no-unused-params (Warning)] Parameter "wrongDefaultValue" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongDefaultValue|
//@[33:035) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "int". (CodeDescription: none) |42|

param myInt2 int = 42
//@[06:012) [no-unused-params (Warning)] Parameter "myInt2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myInt2|
param noValueAfterColon int =   
//@[06:023) [no-unused-params (Warning)] Parameter "noValueAfterColon" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |noValueAfterColon|
//@[32:032) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

param myTruth bool = 'not a boolean'
//@[06:013) [no-unused-params (Warning)] Parameter "myTruth" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myTruth|
//@[21:036) [BCP027 (Error)] The parameter expects a default value of type "bool" but provided value is of type "'not a boolean'". (CodeDescription: none) |'not a boolean'|
param myFalsehood bool = 'false'
//@[06:017) [no-unused-params (Warning)] Parameter "myFalsehood" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myFalsehood|
//@[25:032) [BCP027 (Error)] The parameter expects a default value of type "bool" but provided value is of type "'false'". (CodeDescription: none) |'false'|

param wrongAssignmentToken string: 'hello'
//@[06:026) [no-unused-params (Warning)] Parameter "wrongAssignmentToken" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongAssignmentToken|
//@[33:034) [BCP008 (Error)] Expected the "=" token, or a newline at this location. (CodeDescription: none) |:|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[06:267) [no-unused-params (Warning)] Parameter "WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[06:267) [BCP024 (Error)] The identifier exceeds the limit of 255. Reduce the length of the identifier. (CodeDescription: none) |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|

// #completionTest(28,29) -> boolPlusSymbols
param boolCompletions bool = 
//@[06:021) [no-unused-params (Warning)] Parameter "boolCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |boolCompletions|
//@[29:029) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// #completionTest(30,31) -> arrayPlusSymbols
param arrayCompletions array = 
//@[06:022) [no-unused-params (Warning)] Parameter "arrayCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |arrayCompletions|
//@[31:031) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// #completionTest(32,33) -> objectPlusSymbols
param objectCompletions object = 
//@[06:023) [no-unused-params (Warning)] Parameter "objectCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |objectCompletions|
//@[33:033) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[36:037) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |s|
//@[45:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |'|

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[35:037) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". (CodeDescription: none) |\s|

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[30:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |'what\'s up doc?|

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[41:041) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[41:041) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${up
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[43:043) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${up}
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[43:044) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |}|
param wrongType fluffyBunny = 'what\'s ${'up
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[41:044) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |'up|
//@[44:044) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[46:046) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[46:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${'up${
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[46:046) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
//@[46:046) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[49:049) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[49:050) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |}|
//@[50:050) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[51:051) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[51:053) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |}?|

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|
//@[33:034) [BCP087 (Error)] Array and object literals are not allowed here. (CodeDescription: none) |{|
//@[53:054) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) |'|
//@[54:054) [BCP004 (Error)] The string at this location is not terminated due to an unexpected new line character. (CodeDescription: none) ||

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[06:027) [no-unused-params (Warning)] Parameter "badInterpolatedString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |badInterpolatedString|
//@[46:049) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |}!'|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[06:028) [no-unused-params (Warning)] Parameter "badInterpolatedString2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |badInterpolatedString2|
//@[49:052) [BCP064 (Error)] Found unexpected tokens in interpolated expression. (CodeDescription: none) |b c|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[06:015) [BCP028 (Error)] Identifier "wrongType" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |wrongType|
//@[06:015) [no-unused-params (Warning)] Parameter "wrongType" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongType|
//@[16:027) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |fluffyBunny|

// modifier on an invalid type
@minLength(3)
@maxLength(24)
param someArray arra
//@[06:015) [no-unused-params (Warning)] Parameter "someArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |someArray|
//@[16:020) [BCP031 (Error)] The parameter type is not valid. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) |arra|

@secure()
//@[00:009) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". (CodeDescription: none) |@secure()|
@minLength(3)
//@[00:013) [BCP124 (Error)] The decorator "minLength" can only be attached to targets of type "array | string", but the target has type "int". (CodeDescription: none) |@minLength(3)|
@maxLength(123)
//@[00:015) [BCP124 (Error)] The decorator "maxLength" can only be attached to targets of type "array | string", but the target has type "int". (CodeDescription: none) |@maxLength(123)|
param secureInt int
//@[06:015) [no-unused-params (Warning)] Parameter "secureInt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |secureInt|

// wrong modifier value types
@allowed([
  'test'
//@[02:008) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'test'". (CodeDescription: none) |'test'|
  true
//@[02:006) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". (CodeDescription: none) |true|
])
@minValue({
//@[10:013) [BCP070 (Error)] Argument of type "object" is not assignable to parameter of type "int". (CodeDescription: none) |{\n}|
})
@maxValue([
//@[10:013) [BCP070 (Error)] Argument of type "array" is not assignable to parameter of type "int". (CodeDescription: none) |[\n]|
])
@metadata('wrong')
//@[10:017) [BCP070 (Error)] Argument of type "'wrong'" is not assignable to parameter of type "object". (CodeDescription: none) |'wrong'|
param wrongIntModifier int = true
//@[06:022) [no-unused-params (Warning)] Parameter "wrongIntModifier" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongIntModifier|
//@[29:033) [BCP027 (Error)] The parameter expects a default value of type "int" but provided value is of type "bool". (CodeDescription: none) |true|

@metadata(any([]))
//@[10:017) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |any([])|
@allowed(any(2))
//@[09:015) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |any(2)|
param fatalErrorInIssue1713
//@[06:027) [no-unused-params (Warning)] Parameter "fatalErrorInIssue1713" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |fatalErrorInIssue1713|
//@[27:027) [BCP014 (Error)] Expected a parameter type at this location. Please specify one of the following types: "array", "bool", "int", "object", "string". (CodeDescription: none) ||

// wrong metadata schema
@metadata({
  description: true
//@[15:019) [BCP036 (Error)] The property "description" expected a value of type "string" but the provided value is of type "bool". (CodeDescription: none) |true|
})
param wrongMetadataSchema string
//@[06:025) [no-unused-params (Warning)] Parameter "wrongMetadataSchema" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |wrongMetadataSchema|

// expression in modifier
@maxLength(a + 2)
//@[11:012) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|
@minLength(foo())
//@[11:014) [BCP057 (Error)] The name "foo" does not exist in the current context. (CodeDescription: none) |foo|
@allowed([
  i
//@[02:003) [BCP057 (Error)] The name "i" does not exist in the current context. (CodeDescription: none) |i|
])
param expressionInModifier string = 2 + 3
//@[06:026) [no-unused-params (Warning)] Parameter "expressionInModifier" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |expressionInModifier|
//@[36:041) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "int". (CodeDescription: none) |2 + 3|

@maxLength(2 + 3)
//@[11:016) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |2 + 3|
@minLength(length([]))
//@[11:021) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |length([])|
@allowed([
  resourceGroup().id
//@[02:020) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |resourceGroup().id|
])
param nonCompileTimeConstant string
//@[06:028) [no-unused-params (Warning)] Parameter "nonCompileTimeConstant" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |nonCompileTimeConstant|


@allowed([])
//@[09:011) [BCP099 (Error)] The "allowed" array must contain one or more items. (CodeDescription: none) |[]|
param emptyAllowedString string
//@[06:024) [no-unused-params (Warning)] Parameter "emptyAllowedString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |emptyAllowedString|

@allowed([])
//@[09:011) [BCP099 (Error)] The "allowed" array must contain one or more items. (CodeDescription: none) |[]|
param emptyAllowedInt int
//@[06:021) [no-unused-params (Warning)] Parameter "emptyAllowedInt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |emptyAllowedInt|

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[36:056) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |paramDefaultOneCycle|

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[37:058) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle2" -> "paramDefaultTwoCycle1"). (CodeDescription: none) |paramDefaultTwoCycle2|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[37:058) [BCP080 (Error)] The expression is involved in a cycle ("paramDefaultTwoCycle1" -> "paramDefaultTwoCycle2"). (CodeDescription: none) |paramDefaultTwoCycle1|

@allowed([
  paramModifierSelfCycle
//@[02:024) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |paramModifierSelfCycle|
])
param paramModifierSelfCycle string

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[24:055) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02'|
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
//@[33:055) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(sampleVar, 's')|
//@[40:049) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. (CodeDescription: none) |sampleVar|

param paramAccessingResource string = sampleResource
//@[06:028) [no-unused-params (Warning)] Parameter "paramAccessingResource" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramAccessingResource|
//@[38:052) [BCP027 (Error)] The parameter expects a default value of type "string" but provided value is of type "Microsoft.Foo/foos@2020-02-02". (CodeDescription: none) |sampleResource|
//@[38:052) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. (CodeDescription: none) |sampleResource|

param paramAccessingOutput string = sampleOutput
//@[06:026) [no-unused-params (Warning)] Parameter "paramAccessingOutput" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |paramAccessingOutput|
//@[36:048) [BCP058 (Error)] The name "sampleOutput" is an output. Outputs cannot be referenced in expressions. (CodeDescription: none) |sampleOutput|

// #completionTest(6) -> empty
param 
//@[06:006) [BCP013 (Error)] Expected a parameter identifier at this location. (CodeDescription: none) ||

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 
//@[06:037) [no-unused-params (Warning)] Parameter "defaultValueOneLinerCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |defaultValueOneLinerCompletions|
//@[47:047) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// invalid comma separator (array)
@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
//@[08:008) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
  'def'
])
param commaOne string
//@[06:014) [no-unused-params (Warning)] Parameter "commaOne" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |commaOne|

@secure
//@[01:007) [BCP063 (Error)] The name "secure" is not a parameter, variable, resource or module. (CodeDescription: none) |secure|
@
//@[01:001) [BCP123 (Error)] Expected a namespace or decorator name at this location. (CodeDescription: none) ||
@&& xxx
//@[01:003) [BCP123 (Error)] Expected a namespace or decorator name at this location. (CodeDescription: none) |&&|
@sys
//@[01:004) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. (CodeDescription: none) |sys|
@paramAccessingVar
//@[01:018) [BCP141 (Error)] The expression cannot be used as a decorator as it is not callable. (CodeDescription: none) |paramAccessingVar|
param incompleteDecorators string
//@[06:026) [no-unused-params (Warning)] Parameter "incompleteDecorators" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |incompleteDecorators|

@concat(1, 2)
//@[01:007) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (CodeDescription: none) |concat|
@sys.concat('a', 'b')
//@[05:011) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (CodeDescription: none) |concat|
@secure()
// wrong target type
@minValue(20)
//@[00:013) [BCP124 (Error)] The decorator "minValue" can only be attached to targets of type "int", but the target has type "string". (CodeDescription: none) |@minValue(20)|
param someString string
//@[06:016) [no-unused-params (Warning)] Parameter "someString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |someString|

@allowed([
    true
//@[04:008) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "bool". (CodeDescription: none) |true|
    10
    'foo'
//@[04:009) [BCP034 (Error)] The enclosing array expected an item of type "int", but the provided item was of type "'foo'". (CodeDescription: none) |'foo'|
])
@secure()
//@[00:009) [BCP124 (Error)] The decorator "secure" can only be attached to targets of type "object | string", but the target has type "int". (CodeDescription: none) |@secure()|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
@  
//@[03:003) [BCP123 (Error)] Expected a namespace or decorator name at this location. (CodeDescription: none) ||
// #completionTest(5, 6) -> intParameterDecorators
@sys.   
//@[08:008) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
param someInteger int = 20
//@[06:017) [no-unused-params (Warning)] Parameter "someInteger" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |someInteger|
//@[22:026) [secure-parameter-default (Warning)] Secure parameters should not have hardcoded defaults (except for empty or newGuid()). (CodeDescription: bicep core(https://aka.ms/bicep/linter/secure-parameter-default)) |= 20|

@allowed([], [], 2)
//@[08:019) [BCP071 (Error)] Expected 1 argument, but got 3. (CodeDescription: none) |([], [], 2)|
// #completionTest(4) -> empty
@az.
//@[04:004) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
param tooManyArguments1 int = 20
//@[06:023) [no-unused-params (Warning)] Parameter "tooManyArguments1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |tooManyArguments1|

@metadata({}, {}, true)
//@[09:023) [BCP071 (Error)] Expected 1 argument, but got 3. (CodeDescription: none) |({}, {}, true)|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
//@[01:002) [BCP057 (Error)] The name "m" does not exist in the current context. (CodeDescription: none) |m|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
//@[04:004) [BCP123 (Error)] Expected a namespace or decorator name at this location. (CodeDescription: none) ||
// #completionTest(5) -> stringParameterDecorators
@sys.
//@[05:005) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
param tooManyArguments2 string
//@[06:023) [no-unused-params (Warning)] Parameter "tooManyArguments2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |tooManyArguments2|

@description(sys.concat(2))
//@[13:026) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |sys.concat(2)|
@allowed([for thing in []: 's'])
//@[09:031) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |[for thing in []: 's']|
//@[10:013) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
param nonConstantInDecorator string
//@[06:028) [no-unused-params (Warning)] Parameter "nonConstantInDecorator" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |nonConstantInDecorator|

@minValue(-length('s'))
//@[11:022) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |length('s')|
@metadata({
  bool: !true
//@[08:013) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |!true|
//@[08:013) [BCP032 (Error)] The value must be a compile-time constant. (CodeDescription: none) |!true|
})
param unaryMinusOnFunction int
//@[06:026) [no-unused-params (Warning)] Parameter "unaryMinusOnFunction" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |unaryMinusOnFunction|

@minLength(1)
//@[00:013) [BCP166 (Error)] Duplicate "minLength" decorator. (CodeDescription: none) |@minLength(1)|
@minLength(2)
//@[00:013) [BCP166 (Error)] Duplicate "minLength" decorator. (CodeDescription: none) |@minLength(2)|
@secure()
@maxLength(3)
//@[00:013) [BCP166 (Error)] Duplicate "maxLength" decorator. (CodeDescription: none) |@maxLength(3)|
@maxLength(4)
//@[00:013) [BCP166 (Error)] Duplicate "maxLength" decorator. (CodeDescription: none) |@maxLength(4)|
param duplicateDecorators string
//@[06:025) [no-unused-params (Warning)] Parameter "duplicateDecorators" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |duplicateDecorators|

@minLength(-1)
//@[11:013) [BCP168 (Error)] Length must not be a negative value. (CodeDescription: none) |-1|
@maxLength(-100)
//@[11:015) [BCP168 (Error)] Length must not be a negative value. (CodeDescription: none) |-100|
param invalidLength string
//@[06:019) [no-unused-params (Warning)] Parameter "invalidLength" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |invalidLength|

@allowed([
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
	'Microsoft.Network/applicationGateways'
	'Microsoft.Automation/automationAccounts'
	'Microsoft.ContainerInstance/containerGroups'
	'Microsoft.ContainerRegistry/registries'
	'Microsoft.ContainerService/managedClusters'
])
param invalidPermutation array = [
//@[06:024) [no-unused-params (Warning)] Parameter "invalidPermutation" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |invalidPermutation|
	'foobar'
//@[01:009) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "'foobar'". (CodeDescription: none) |'foobar'|
	true
//@[01:005) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "bool". (CodeDescription: none) |true|
    100
//@[04:007) [BCP034 (Error)] The enclosing array expected an item of type "'Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways'", but the provided item was of type "int". (CodeDescription: none) |100|
]

@allowed([
	[
		'Microsoft.AnalysisServices/servers'
		'Microsoft.ApiManagement/service'
	]
	[
		'Microsoft.Network/applicationGateways'
		'Microsoft.Automation/automationAccounts'
	]
])
param invalidDefaultWithAllowedArrayDecorator array = true
//@[06:045) [no-unused-params (Warning)] Parameter "invalidDefaultWithAllowedArrayDecorator" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |invalidDefaultWithAllowedArrayDecorator|
//@[54:058) [BCP027 (Error)] The parameter expects a default value of type "array" but provided value is of type "bool". (CodeDescription: none) |true|

// unterminated multi-line comment
/*    
//@[00:007) [BCP002 (Error)] The multi-line comment at this location is not terminated. Terminate it with the */ character sequence. (CodeDescription: none) |/*    \n|

