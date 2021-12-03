
// unknown declaration
bad
//@[0:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |bad|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var
//@[3:3) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) ||
//@[3:3) [no-unused-vars (Warning)] Variable "<missing>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) ||

// missing identifier #completionTest(4) -> empty
var 
//@[4:4) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) ||
//@[4:4) [no-unused-vars (Warning)] Variable "<missing>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) ||

// incomplete keyword
// #completionTest(0,1) -> declarations
v
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |v|
// #completionTest(0,1,2) -> declarations
va
//@[0:2) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |va|

// unassigned variable
var foo
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[4:7) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo|
//@[7:7) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(18,19) -> symbols
var missingValue = 
//@[4:16) [no-unused-vars (Warning)] Variable "missingValue" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingValue|
//@[19:19) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// malformed identifier
var 2 
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) |2|
//@[4:5) [no-unused-vars (Warning)] Variable "<error>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |2|
//@[6:6) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
var $ = 23
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) |$|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "$". (CodeDescription: none) |$|
//@[4:5) [no-unused-vars (Warning)] Variable "<error>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |$|
var # 33 = 43
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) |#|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "#". (CodeDescription: none) |#|
//@[4:8) [no-unused-vars (Warning)] Variable "<error>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |# 33|

// no value assigned
var foo =
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[4:7) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo|
//@[9:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// bad =
var badEquals 2
//@[4:13) [no-unused-vars (Warning)] Variable "badEquals" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badEquals|
//@[14:15) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |2|
//@[15:15) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
var badEquals2 3 true
//@[4:14) [no-unused-vars (Warning)] Variable "badEquals2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |badEquals2|
//@[15:16) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |3|
//@[21:21) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

// malformed identifier but type check should happen regardless
var 2 = x
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) |2|
//@[4:5) [no-unused-vars (Warning)] Variable "<error>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |2|
//@[8:9) [BCP062 (Error)] The referenced declaration with name "x" is not valid. (CodeDescription: none) |x|

// bad token value
var foo = &
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[4:7) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |&|
//@[10:11) [BCP001 (Error)] The following token is not recognized: "&". (CodeDescription: none) |&|

// bad value
var foo = *
//@[4:7) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[4:7) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |foo|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |*|

// expressions
var bar = x
//@[4:7) [BCP028 (Error)] Identifier "bar" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bar|
//@[4:7) [no-unused-vars (Warning)] Variable "bar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bar|
//@[10:11) [BCP062 (Error)] The referenced declaration with name "x" is not valid. (CodeDescription: none) |x|
var bar = foo()
//@[4:7) [BCP028 (Error)] Identifier "bar" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |bar|
//@[4:7) [no-unused-vars (Warning)] Variable "bar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bar|
//@[10:13) [BCP059 (Error)] The name "foo" is not a function. (CodeDescription: none) |foo|
var x = 2 + !3
//@[4:5) [BCP028 (Error)] Identifier "x" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |x|
//@[12:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!3|
var y = false ? true + 1 : !4
//@[4:5) [BCP028 (Error)] Identifier "y" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |y|
//@[4:5) [no-unused-vars (Warning)] Variable "y" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |y|
//@[16:24) [BCP045 (Error)] Cannot apply operator "+" to operands of type "bool" and "int". (CodeDescription: none) |true + 1|
//@[27:29) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|

// test for array item recovery
var x = [
//@[4:5) [BCP028 (Error)] Identifier "x" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |x|
//@[4:5) [no-unused-vars (Warning)] Variable "x" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |x|
  3 + 4
  =
//@[2:3) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |=|
  !null
//@[2:7) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (CodeDescription: none) |!null|
]

// test for object property recovery
var y = {
//@[4:5) [BCP028 (Error)] Identifier "y" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |y|
//@[4:5) [no-unused-vars (Warning)] Variable "y" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |y|
  =
//@[2:3) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |=|
//@[3:3) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
  foo: !2
//@[7:9) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!2|
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[4:8) [no-unused-vars (Warning)] Variable "test" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test|
//@[11:17) [BCP065 (Error)] Function "utcNow" is not valid at this location. It can only be used as a parameter default value. (CodeDescription: none) |utcNow|
var test2 = newGuid()
//@[4:9) [no-unused-vars (Warning)] Variable "test2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test2|
//@[12:19) [BCP065 (Error)] Function "newGuid" is not valid at this location. It can only be used as a parameter default value. (CodeDescription: none) |newGuid|

// bad string escape sequence in object key
var test3 = {
//@[4:9) [no-unused-vars (Warning)] Variable "test3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |test3|
  'bad\escape': true
//@[2:14) [BCP022 (Error)] Expected a property name at this location. (CodeDescription: none) |'bad\escape'|
//@[6:8) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". (CodeDescription: none) |\e|
}

// duplicate properties
var testDupe = {
//@[4:12) [no-unused-vars (Warning)] Variable "testDupe" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |testDupe|
  'duplicate': true
//@[2:13) [BCP025 (Error)] The property "duplicate" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'duplicate'|
  duplicate: true
//@[2:11) [BCP025 (Error)] The property "duplicate" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |duplicate|
}

// interpolation with type errors in key
var objWithInterp = {
//@[4:17) [no-unused-vars (Warning)] Variable "objWithInterp" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objWithInterp|
  'ab${nonExistentIdentifier}cd': true
//@[7:28) [BCP057 (Error)] The name "nonExistentIdentifier" does not exist in the current context. (CodeDescription: none) |nonExistentIdentifier|
}

// invalid fully qualified function access
var mySum = az.add(1,2)
//@[4:9) [no-unused-vars (Warning)] Variable "mySum" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mySum|
//@[15:18) [BCP107 (Error)] The function "add" does not exist in namespace "az". (CodeDescription: none) |add|
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[4:12) [no-unused-vars (Warning)] Variable "myConcat" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |myConcat|
//@[34:40) [BCP107 (Error)] The function "concat" does not exist in namespace "az". (CodeDescription: none) |concat|

// invalid string using double quotes
var doubleString = "bad string"
//@[4:16) [no-unused-vars (Warning)] Variable "doubleString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |doubleString|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |"|
//@[19:20) [BCP103 (Error)] The following token is not recognized: """. Strings are defined using single quotes in bicep. (CodeDescription: none) |"|
//@[30:31) [BCP103 (Error)] The following token is not recognized: """. Strings are defined using single quotes in bicep. (CodeDescription: none) |"|

var resourceGroup = ''
//@[4:17) [no-unused-vars (Warning)] Variable "resourceGroup" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resourceGroup|
var rgName = resourceGroup().name
//@[4:10) [no-unused-vars (Warning)] Variable "rgName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |rgName|
//@[13:26) [BCP059 (Error)] The name "resourceGroup" is not a function. (CodeDescription: none) |resourceGroup|

// this does not work at the resource group scope
var invalidLocationVar = deployment().location
//@[4:22) [no-unused-vars (Warning)] Variable "invalidLocationVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidLocationVar|
//@[25:46) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'deployment().location'. 'deployment().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |deployment().location|
//@[38:46) [BCP053 (Error)] The type "deployment" does not contain property "location". Available properties include "name", "properties". (CodeDescription: none) |location|

var invalidEnvironmentVar = environment().aosdufhsad
//@[4:25) [no-unused-vars (Warning)] Variable "invalidEnvironmentVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidEnvironmentVar|
//@[42:52) [BCP053 (Error)] The type "environment" does not contain property "aosdufhsad". Available properties include "activeDirectoryDataLake", "authentication", "batch", "gallery", "graph", "graphAudience", "media", "name", "portal", "resourceManager", "sqlManagement", "suffixes", "vmImageAliasDoc". (CodeDescription: none) |aosdufhsad|
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[4:21) [no-unused-vars (Warning)] Variable "invalidEnvAuthVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidEnvAuthVar|
//@[53:61) [BCP053 (Error)] The type "authentication" does not contain property "asdgdsag". Available properties include "audiences", "identityProvider", "loginEndpoint", "tenant". (CodeDescription: none) |asdgdsag|

// invalid use of reserved namespace
var az = 1
//@[4:6) [BCP084 (Error)] The symbolic name "az" is reserved. Please use a different symbolic name. Reserved namespaces are "az", "sys". (CodeDescription: none) |az|
//@[4:6) [no-unused-vars (Warning)] Variable "az" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |az|

// cannot assign a variable to a namespace
var invalidNamespaceAssignment = az
//@[4:30) [no-unused-vars (Warning)] Variable "invalidNamespaceAssignment" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |invalidNamespaceAssignment|
//@[33:35) [BCP041 (Error)] Values of type "az" cannot be assigned to a variable. (CodeDescription: none) |az|

var objectLiteralType = {
  first: true
  second: false
  third: 42
  fourth: 'test'
  fifth: [
    {
      one: true
    }
    {
      one: false
    }
  ]
  sixth: [
    {
      two: 44
    }
  ]
}

// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions = objectLiteralType.f
//@[4:32) [no-unused-vars (Warning)] Variable "objectVarTopLevelCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectVarTopLevelCompletions|
//@[53:54) [BCP053 (Error)] The type "object" does not contain property "f". Available properties include "fifth", "first", "fourth", "second", "sixth", "third". (CodeDescription: none) |f|
// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[4:33) [no-unused-vars (Warning)] Variable "objectVarTopLevelCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectVarTopLevelCompletions2|
//@[54:54) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// this does not produce any completions because mixed array items are of type "any"
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[4:29) [no-unused-vars (Warning)] Variable "mixedArrayTypeCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mixedArrayTypeCompletions|
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[4:30) [no-unused-vars (Warning)] Variable "mixedArrayTypeCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mixedArrayTypeCompletions2|
//@[60:60) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[4:27) [no-unused-vars (Warning)] Variable "oneArrayItemCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |oneArrayItemCompletions|
//@[57:58) [BCP053 (Error)] The type "object" does not contain property "t". Available properties include "two". (CodeDescription: none) |t|
// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[4:28) [no-unused-vars (Warning)] Variable "oneArrayItemCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |oneArrayItemCompletions2|
//@[58:58) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(65) -> objectVarTopLevelIndexes
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[4:42) [no-unused-vars (Warning)] Variable "objectVarTopLevelArrayIndexCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |objectVarTopLevelArrayIndexCompletions|
//@[63:64) [BCP057 (Error)] The name "f" does not exist in the current context. (CodeDescription: none) |f|

// #completionTest(58) -> twoIndexPlusSymbols
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[4:28) [no-unused-vars (Warning)] Variable "oneArrayIndexCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |oneArrayIndexCompletions|
//@[58:58) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

// Issue 486
var myFloat = 3.14
//@[4:11) [no-unused-vars (Warning)] Variable "myFloat" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |myFloat|
//@[16:16) [BCP055 (Error)] Cannot access properties of type "int". An "object" type is required. (CodeDescription: none) ||
//@[16:16) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
//@[16:18) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |14|

// secure cannot be used as a variable decorator
@sys.secure()
//@[5:11) [BCP126 (Error)] Function "secure" cannot be used as a variable decorator. (CodeDescription: none) |secure|
var something = 1
//@[4:13) [no-unused-vars (Warning)] Variable "something" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |something|

// #completionTest(1) -> sysAndDescription
@
//@[1:1) [BCP123 (Error)] Expected a namespace or decorator name at this location. (CodeDescription: none) ||
// #completionTest(5) -> description
@sys.
//@[5:5) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
var anotherThing = true
//@[4:16) [no-unused-vars (Warning)] Variable "anotherThing" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |anotherThing|

// invalid identifier character classes
var ☕ = true
//@[4:5) [BCP015 (Error)] Expected a variable identifier at this location. (CodeDescription: none) |☕|
//@[4:5) [BCP001 (Error)] The following token is not recognized: "☕". (CodeDescription: none) |☕|
//@[4:5) [no-unused-vars (Warning)] Variable "<error>" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |☕|
var a☕ = true
//@[4:5) [no-unused-vars (Warning)] Variable "a" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |a|
//@[5:6) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |☕|
//@[5:6) [BCP001 (Error)] The following token is not recognized: "☕". (CodeDescription: none) |☕|
//@[13:13) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

var missingArrayVariable = [for thing in stuff: 4]
//@[4:24) [no-unused-vars (Warning)] Variable "missingArrayVariable" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |missingArrayVariable|
//@[41:46) [BCP057 (Error)] The name "stuff" does not exist in the current context. (CodeDescription: none) |stuff|

// loops are only allowed at the top level
var nonTopLevelLoop = {
//@[4:19) [no-unused-vars (Warning)] Variable "nonTopLevelLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nonTopLevelLoop|
  notOkHere: [for thing in stuff: 4]
//@[14:17) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
//@[27:32) [BCP057 (Error)] The name "stuff" does not exist in the current context. (CodeDescription: none) |stuff|
}

// loops with conditions won't even parse
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[4:30) [no-unused-vars (Warning)] Variable "noFilteredLoopsInVariables" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |noFilteredLoopsInVariables|
//@[47:52) [BCP057 (Error)] The name "stuff" does not exist in the current context. (CodeDescription: none) |stuff|
//@[54:56) [BCP100 (Error)] The "if" function is not supported. Use the ternary conditional operator instead. (CodeDescription: none) |if|

// nested loops are also not allowed
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[4:31) [no-unused-vars (Warning)] Variable "noNestedVariableLoopsEither" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |noNestedVariableLoopsEither|
//@[48:53) [BCP057 (Error)] The name "stuff" does not exist in the current context. (CodeDescription: none) |stuff|
  hello: [for thing in []: 4]
//@[10:13) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
}]

// loops in inner properties of a variable are also not supported
var innerPropertyLoop = {
//@[4:21) [no-unused-vars (Warning)] Variable "innerPropertyLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |innerPropertyLoop|
  a: [for i in range(0,10): i]
//@[6:9) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
}
var innerPropertyLoop2 = {
//@[4:22) [no-unused-vars (Warning)] Variable "innerPropertyLoop2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |innerPropertyLoop2|
  b: {
    a: [for i in range(0,10): i]
//@[8:11) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
  }
}

// loops using expressions with a runtime dependency are also not allowed
var keys = listKeys('fake','fake')
var indirection = keys

var runtimeLoop = [for (item, index) in []: indirection]
//@[4:15) [no-unused-vars (Warning)] Variable "runtimeLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |runtimeLoop|
//@[44:55) [BCP182 (Error)] This expression is being used in the for-body of the variable "runtimeLoop", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("indirection" -> "keys" -> "listKeys"). (CodeDescription: none) |indirection|
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[4:16) [no-unused-vars (Warning)] Variable "runtimeLoop2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |runtimeLoop2|
//@[41:52) [BCP178 (Error)] This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("indirection" -> "keys" -> "listKeys"). (CodeDescription: none) |indirection|

var zoneInput = []
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
  name: zone
  location: az.resourceGroup().location
//@[12:39) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |az.resourceGroup().location|
}]
var inlinedVariable = zones[0].properties.zoneType

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[4:16) [no-unused-vars (Warning)] Variable "runtimeLoop3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |runtimeLoop3|
  a: inlinedVariable
//@[5:20) [BCP182 (Error)] This expression is being used in the for-body of the variable "runtimeLoop3", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("inlinedVariable" -> "zones"). Properties of zones which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |inlinedVariable|
}]

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[4:16) [no-unused-vars (Warning)] Variable "runtimeLoop4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |runtimeLoop4|
//@[37:56) [BCP178 (Error)] This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of zones which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |zones[0].properties|
  a: 0
}]

var notRuntime = concat('a','b')
//@[17:32) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat('a','b')|
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[26:69) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(notRuntime, string(moreIndirection))|
var moreIndirection = reference('s','s', 'Full')

var myRef = [
  evenMoreIndirection
]
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[4:16) [no-unused-vars (Warning)] Variable "runtimeLoop5" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |runtimeLoop5|
//@[41:46) [BCP178 (Error)] This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("myRef" -> "evenMoreIndirection" -> "moreIndirection" -> "reference"). (CodeDescription: none) |myRef|

// cannot use loops in expressions
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[4:18) [no-unused-vars (Warning)] Variable "loopExpression" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |loopExpression|
//@[28:31) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
//@[41:46) [BCP057 (Error)] The name "stuff" does not exist in the current context. (CodeDescription: none) |stuff|
//@[53:56) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
//@[66:71) [BCP057 (Error)] The name "stuff" does not exist in the current context. (CodeDescription: none) |stuff|

@batchSize(1)
//@[1:10) [BCP126 (Error)] Function "batchSize" cannot be used as a variable decorator. (CodeDescription: none) |batchSize|
var batchSizeMakesNoSenseHere = false
//@[4:29) [no-unused-vars (Warning)] Variable "batchSizeMakesNoSenseHere" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |batchSizeMakesNoSenseHere|


//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[4:21) [no-unused-vars (Warning)] Variable "keyVaultSecretVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |keyVaultSecretVar|
//@[24:48) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[4:33) [no-unused-vars (Warning)] Variable "keyVaultSecretInterpolatedVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |keyVaultSecretInterpolatedVar|
//@[39:63) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
var keyVaultSecretObjectVar = {
//@[4:27) [no-unused-vars (Warning)] Variable "keyVaultSecretObjectVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |keyVaultSecretObjectVar|
  secret: kv.getSecret('mySecret')
//@[10:34) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
}
var keyVaultSecretArrayVar = [
//@[4:26) [no-unused-vars (Warning)] Variable "keyVaultSecretArrayVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |keyVaultSecretArrayVar|
  kv.getSecret('mySecret')
//@[2:26) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
]
var keyVaultSecretArrayInterpolatedVar = [
//@[4:38) [no-unused-vars (Warning)] Variable "keyVaultSecretArrayInterpolatedVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |keyVaultSecretArrayInterpolatedVar|
  '${kv.getSecret('mySecret')}'
//@[5:29) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (CodeDescription: none) |kv.getSecret('mySecret')|
]

