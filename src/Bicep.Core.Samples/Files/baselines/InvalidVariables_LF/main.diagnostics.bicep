
// unknown declaration
bad
//@[00:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |bad|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
var
//@[03:03) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) ||

// missing identifier #completionTest(4) -> empty
var 
//@[04:04) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) ||

// incomplete keyword
// #completionTest(0,1) -> declarations
v
//@[00:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |v|
// #completionTest(0,1,2) -> declarations
va
//@[00:02) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |va|

// unassigned variable
var foo
//@[04:07) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |foo|
//@[07:07) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// #completionTest(18,19) -> symbols
var missingValue = 
//@[04:16) [no-unused-vars (Warning)] Variable "missingValue" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingValue|
//@[19:19) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// malformed identifier
var 2 
//@[04:05) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) |2|
//@[06:06) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
var $ = 23
//@[04:05) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) |$|
//@[04:05) [BCP001 (Error)] The following token is not recognized: "$". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |$|
//@[10:10) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
var # 33 = 43
//@[04:05) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) |#|
//@[04:05) [BCP001 (Error)] The following token is not recognized: "#". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |#|
//@[13:13) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// no value assigned
var foo =
//@[04:07) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |foo|
//@[04:07) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |foo|
//@[09:09) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// bad =
var badEquals 2
//@[04:13) [no-unused-vars (Warning)] Variable "badEquals" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badEquals|
//@[15:15) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
var badEquals2 3 true
//@[04:14) [no-unused-vars (Warning)] Variable "badEquals2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |badEquals2|
//@[17:21) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) |true|
//@[21:21) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) ||

// malformed identifier but type check should happen regardless
var 2 = x
//@[04:05) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) |2|
//@[08:09) [BCP287 (Error)] 'x' refers to a value but is being used as a type here. (bicep https://aka.ms/bicep/core-diagnostics#BCP287) |x|
//@[09:09) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||

// bad token value
var foo = &
//@[04:07) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |foo|
//@[04:07) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |foo|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |&|
//@[10:11) [BCP001 (Error)] The following token is not recognized: "&". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |&|

// bad value
var foo = *
//@[04:07) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |foo|
//@[04:07) [no-unused-vars (Warning)] Variable "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |foo|
//@[10:11) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |*|

// expressions
var bar = x
//@[04:07) [BCP028 (Error)] Identifier "bar" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bar|
//@[04:07) [no-unused-vars (Warning)] Variable "bar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bar|
//@[10:11) [BCP062 (Error)] The referenced declaration with name "x" is not valid. (bicep https://aka.ms/bicep/core-diagnostics#BCP062) |x|
var bar = foo()
//@[04:07) [BCP028 (Error)] Identifier "bar" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |bar|
//@[04:07) [no-unused-vars (Warning)] Variable "bar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bar|
//@[10:13) [BCP059 (Error)] The name "foo" is not a function. (bicep https://aka.ms/bicep/core-diagnostics#BCP059) |foo|
var x = 2 + !3
//@[04:05) [BCP028 (Error)] Identifier "x" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |x|
//@[12:14) [BCP044 (Error)] Cannot apply operator "!" to operand of type "3". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!3|
var y = false ? true + 1 : !4
//@[04:05) [BCP028 (Error)] Identifier "y" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |y|
//@[04:05) [no-unused-vars (Warning)] Variable "y" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |y|
//@[16:24) [BCP045 (Error)] Cannot apply operator "+" to operands of type "true" and "1". (bicep https://aka.ms/bicep/core-diagnostics#BCP045) |true + 1|
//@[27:29) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!4|

// test for array item recovery
var x = [
//@[04:05) [BCP028 (Error)] Identifier "x" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |x|
//@[04:05) [no-unused-vars (Warning)] Variable "x" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |x|
  3 + 4
  =
//@[02:03) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |=|
  !null
//@[02:07) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!null|
]

// test for object property recovery
var y = {
//@[04:05) [BCP028 (Error)] Identifier "y" is declared multiple times. Remove or rename the duplicates. (bicep https://aka.ms/bicep/core-diagnostics#BCP028) |y|
//@[04:05) [no-unused-vars (Warning)] Variable "y" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |y|
  =
//@[02:03) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |=|
//@[03:03) [BCP018 (Error)] Expected the ":" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
  foo: !2
//@[07:09) [BCP044 (Error)] Cannot apply operator "!" to operand of type "2". (bicep https://aka.ms/bicep/core-diagnostics#BCP044) |!2|
}

// utcNow and newGuid used outside a param default value
var test = utcNow('u')
//@[04:08) [no-unused-vars (Warning)] Variable "test" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test|
//@[11:17) [BCP065 (Error)] Function "utcNow" is not valid at this location. It can only be used as a parameter default value. (bicep https://aka.ms/bicep/core-diagnostics#BCP065) |utcNow|
var test2 = newGuid()
//@[04:09) [no-unused-vars (Warning)] Variable "test2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test2|
//@[12:19) [BCP065 (Error)] Function "newGuid" is not valid at this location. It can only be used as a parameter default value. (bicep https://aka.ms/bicep/core-diagnostics#BCP065) |newGuid|

// bad string escape sequence in object key
var test3 = {
//@[04:09) [no-unused-vars (Warning)] Variable "test3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |test3|
  'bad\escape': true
//@[02:14) [BCP022 (Error)] Expected a property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP022) |'bad\escape'|
//@[06:08) [BCP006 (Error)] The specified escape sequence is not recognized. Only the following escape sequences are allowed: "\$", "\'", "\\", "\n", "\r", "\t", "\u{...}". (bicep https://aka.ms/bicep/core-diagnostics#BCP006) |\e|
}

// duplicate properties
var testDupe = {
//@[04:12) [no-unused-vars (Warning)] Variable "testDupe" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |testDupe|
  'duplicate': true
//@[02:13) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-unquoted-property-names) |'duplicate'|
//@[02:13) [BCP025 (Error)] The property "duplicate" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |'duplicate'|
  duplicate: true
//@[02:11) [BCP025 (Error)] The property "duplicate" is declared multiple times in this object. Remove or rename the duplicate properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP025) |duplicate|
}

// interpolation with type errors in key
var objWithInterp = {
//@[04:17) [no-unused-vars (Warning)] Variable "objWithInterp" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objWithInterp|
  'ab${nonExistentIdentifier}cd': true
//@[07:28) [BCP057 (Error)] The name "nonExistentIdentifier" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |nonExistentIdentifier|
}

// invalid fully qualified function access
var mySum = az.add(1,2)
//@[04:09) [no-unused-vars (Warning)] Variable "mySum" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mySum|
//@[15:18) [BCP067 (Error)] Cannot call functions on type "1". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP067) |add|
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[04:12) [no-unused-vars (Warning)] Variable "myConcat" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |myConcat|
//@[34:40) [BCP067 (Error)] Cannot call functions on type "1". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP067) |concat|

// invalid string using double quotes
var doubleString = "bad string"
//@[04:16) [no-unused-vars (Warning)] Variable "doubleString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |doubleString|
//@[19:20) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP009) |"|
//@[19:20) [BCP103 (Error)] The following token is not recognized: """. Strings are defined using single quotes in bicep. (bicep https://aka.ms/bicep/core-diagnostics#BCP103) |"|
//@[30:31) [BCP103 (Error)] The following token is not recognized: """. Strings are defined using single quotes in bicep. (bicep https://aka.ms/bicep/core-diagnostics#BCP103) |"|

// invalid index on array literal
var nonExistentIndex1 = [][0]
//@[04:21) [no-unused-vars (Warning)] Variable "nonExistentIndex1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nonExistentIndex1|
//@[27:28) [BCP311 (Error)] The provided index value of "0" is not valid for type "<empty array>". (bicep https://aka.ms/bicep/core-diagnostics#BCP311) |0|
var nonExistentIndex2 = ['foo'][1]
//@[04:21) [no-unused-vars (Warning)] Variable "nonExistentIndex2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nonExistentIndex2|
//@[32:33) [BCP311 (Error)] The provided index value of "1" is not valid for type "['foo']". Indexes for this type must be between 0 and 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP311) |1|
var nonExistentIndex3 = ['foo', 'bar'][-1]
//@[04:21) [no-unused-vars (Warning)] Variable "nonExistentIndex3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nonExistentIndex3|
//@[39:41) [BCP311 (Error)] The provided index value of "-1" is not valid for type "['foo', 'bar']". Indexes for this type must be between 0 and 1. (bicep https://aka.ms/bicep/core-diagnostics#BCP311) |-1|

var resourceGroup = ''
var rgName = resourceGroup().name
//@[04:10) [no-unused-vars (Warning)] Variable "rgName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |rgName|
//@[13:26) [BCP265 (Error)] The name "resourceGroup" is not a function. Did you mean "az.resourceGroup"? (bicep https://aka.ms/bicep/core-diagnostics#BCP265) |resourceGroup|

var subscription = ''
var subName = subscription().name
//@[04:11) [no-unused-vars (Warning)] Variable "subName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |subName|
//@[14:26) [BCP265 (Error)] The name "subscription" is not a function. Did you mean "az.subscription"? (bicep https://aka.ms/bicep/core-diagnostics#BCP265) |subscription|

// this does not work at the resource group scope
var invalidLocationVar = deployment().location
//@[04:22) [no-unused-vars (Warning)] Variable "invalidLocationVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidLocationVar|
//@[38:46) [BCP053 (Error)] The type "deployment" does not contain property "location". Available properties include "name", "properties". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |location|

var invalidEnvironmentVar = environment().aosdufhsad
//@[04:25) [no-unused-vars (Warning)] Variable "invalidEnvironmentVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidEnvironmentVar|
//@[42:52) [BCP053 (Error)] The type "environment" does not contain property "aosdufhsad". Available properties include "activeDirectoryDataLake", "authentication", "batch", "gallery", "graph", "graphAudience", "media", "name", "portal", "resourceManager", "sqlManagement", "suffixes", "vmImageAliasDoc". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |aosdufhsad|
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[04:21) [no-unused-vars (Warning)] Variable "invalidEnvAuthVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidEnvAuthVar|
//@[53:61) [BCP053 (Error)] The type "authenticationProperties" does not contain property "asdgdsag". Available properties include "audiences", "identityProvider", "loginEndpoint", "tenant". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |asdgdsag|

// invalid use of reserved namespace
var az = 1
//@[04:06) [BCP084 (Error)] The symbolic name "az" is reserved. Please use a different symbolic name. Reserved namespaces are "az", "sys". (bicep https://aka.ms/bicep/core-diagnostics#BCP084) |az|

// cannot assign a variable to a namespace
var invalidNamespaceAssignment = az
//@[04:30) [no-unused-vars (Warning)] Variable "invalidNamespaceAssignment" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |invalidNamespaceAssignment|

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
//@[04:32) [no-unused-vars (Warning)] Variable "objectVarTopLevelCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectVarTopLevelCompletions|
//@[53:54) [BCP053 (Error)] The type "object" does not contain property "f". Available properties include "fifth", "first", "fourth", "second", "sixth", "third". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |f|
// #completionTest(54) -> objectVarTopLevel
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[04:33) [no-unused-vars (Warning)] Variable "objectVarTopLevelCompletions2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectVarTopLevelCompletions2|
//@[54:54) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// this does not produce any completions because mixed array items are of type "any"
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[04:29) [no-unused-vars (Warning)] Variable "mixedArrayTypeCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mixedArrayTypeCompletions|
//@[59:60) [BCP053 (Error)] The type "object" does not contain property "o". Available properties include "one". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |o|
// #completionTest(60) -> mixedArrayProperties
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[04:30) [no-unused-vars (Warning)] Variable "mixedArrayTypeCompletions2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |mixedArrayTypeCompletions2|
//@[60:60) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[04:27) [no-unused-vars (Warning)] Variable "oneArrayItemCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |oneArrayItemCompletions|
//@[57:58) [BCP053 (Error)] The type "object" does not contain property "t". Available properties include "two". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |t|
// #completionTest(58) -> oneArrayItemProperties
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[04:28) [no-unused-vars (Warning)] Variable "oneArrayItemCompletions2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |oneArrayItemCompletions2|
//@[58:58) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||

// #completionTest(65) -> objectVarTopLevelIndexes
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[04:42) [no-unused-vars (Warning)] Variable "objectVarTopLevelArrayIndexCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |objectVarTopLevelArrayIndexCompletions|
//@[63:64) [BCP057 (Error)] The name "f" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |f|

// #completionTest(58) -> twoIndexPlusSymbols
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[04:28) [no-unused-vars (Warning)] Variable "oneArrayIndexCompletions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |oneArrayIndexCompletions|
//@[58:58) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (bicep https://aka.ms/bicep/core-diagnostics#BCP117) ||

// Issue 486
var myFloat = 3.14
//@[04:11) [no-unused-vars (Warning)] Variable "myFloat" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |myFloat|
//@[16:16) [BCP055 (Error)] Cannot access properties of type "3". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP055) ||
//@[16:16) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
//@[16:18) [BCP019 (Error)] Expected a new line character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP019) |14|

// secure cannot be used as a variable decorator
@sys.secure()
//@[05:11) [BCP126 (Error)] Function "secure" cannot be used as a variable decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP126) |secure|
var something = 1
//@[04:13) [no-unused-vars (Warning)] Variable "something" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |something|

// #completionTest(1) -> sysAndDescription
@
//@[01:01) [BCP123 (Error)] Expected a namespace or decorator name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP123) ||
// #completionTest(5) -> description
@sys.
//@[05:05) [BCP020 (Error)] Expected a function or property name at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP020) ||
var anotherThing = true
//@[04:16) [no-unused-vars (Warning)] Variable "anotherThing" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |anotherThing|

// invalid identifier character classes
var ☕ = true
//@[04:05) [BCP015 (Error)] Expected a variable identifier at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP015) |☕|
//@[04:05) [BCP001 (Error)] The following token is not recognized: "☕". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |☕|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP018) ||
var a☕ = true
//@[04:05) [no-unused-vars (Warning)] Variable "a" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |a|
//@[05:06) [BCP279 (Error)] Expected a type at this location. Please specify a valid type expression or one of the following types: "any", "array", "bool", "int", "object", "string". (bicep https://aka.ms/bicep/core-diagnostics#BCP279) |☕|
//@[05:06) [BCP001 (Error)] The following token is not recognized: "☕". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |☕|

var missingArrayVariable = [for thing in stuff: 4]
//@[04:24) [no-unused-vars (Warning)] Variable "missingArrayVariable" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |missingArrayVariable|
//@[41:46) [BCP057 (Error)] The name "stuff" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |stuff|

// loops are only allowed at the top level
var nonTopLevelLoop = {
//@[04:19) [no-unused-vars (Warning)] Variable "nonTopLevelLoop" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |nonTopLevelLoop|
  notOkHere: [for thing in stuff: 4]
//@[14:17) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
//@[27:32) [BCP057 (Error)] The name "stuff" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |stuff|
}

// loops with conditions won't even parse
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[04:30) [no-unused-vars (Warning)] Variable "noFilteredLoopsInVariables" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |noFilteredLoopsInVariables|
//@[47:52) [BCP057 (Error)] The name "stuff" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |stuff|
//@[54:56) [BCP100 (Error)] The function "if" is not supported. Use the "?:" (ternary conditional) operator instead, e.g. condition ? ValueIfTrue : ValueIfFalse (bicep https://aka.ms/bicep/core-diagnostics#BCP100) |if|

// nested loops are also not allowed
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[04:31) [no-unused-vars (Warning)] Variable "noNestedVariableLoopsEither" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |noNestedVariableLoopsEither|
//@[48:53) [BCP057 (Error)] The name "stuff" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |stuff|
  hello: [for thing in []: 4]
//@[10:13) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
}]

// loops in inner properties of a variable are also not supported
var innerPropertyLoop = {
//@[04:21) [no-unused-vars (Warning)] Variable "innerPropertyLoop" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |innerPropertyLoop|
  a: [for i in range(0,10): i]
//@[06:09) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
}
var innerPropertyLoop2 = {
//@[04:22) [no-unused-vars (Warning)] Variable "innerPropertyLoop2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |innerPropertyLoop2|
  b: {
    a: [for i in range(0,10): i]
//@[08:11) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
  }
}

// loops using expressions with a runtime dependency are also not allowed
var keys = listKeys('fake','fake')
var indirection = keys

var runtimeLoop = [for (item, index) in []: indirection]
//@[04:15) [no-unused-vars (Warning)] Variable "runtimeLoop" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |runtimeLoop|
//@[44:55) [BCP182 (Error)] This expression is being used in the for-body of the variable "runtimeLoop", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("indirection" -> "keys" -> "listKeys"). (bicep https://aka.ms/bicep/core-diagnostics#BCP182) |indirection|
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[04:16) [no-unused-vars (Warning)] Variable "runtimeLoop2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |runtimeLoop2|
//@[41:52) [BCP178 (Error)] This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("indirection" -> "keys" -> "listKeys"). (bicep https://aka.ms/bicep/core-diagnostics#BCP178) |indirection|

var zoneInput = []
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
  name: zone
  location: az.resourceGroup().location
//@[15:28) [BCP067 (Error)] Cannot call functions on type "1". An "object" type is required. (bicep https://aka.ms/bicep/core-diagnostics#BCP067) |resourceGroup|
}]
var inlinedVariable = zones[0].properties.zoneType

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[04:16) [no-unused-vars (Warning)] Variable "runtimeLoop3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |runtimeLoop3|
  a: inlinedVariable
//@[05:20) [BCP182 (Error)] This expression is being used in the for-body of the variable "runtimeLoop3", which requires values that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("inlinedVariable" -> "zones"). The property "properties" of zones cannot be calculated at the start. Properties of zones which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP182) |inlinedVariable|
}]

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[04:16) [no-unused-vars (Warning)] Variable "runtimeLoop4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |runtimeLoop4|
//@[37:56) [BCP178 (Error)] This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of zones which can be calculated at the start include "apiVersion", "id", "name", "type". (bicep https://aka.ms/bicep/core-diagnostics#BCP178) |zones[0].properties|
  a: 0
}]

var notRuntime = concat('a','b')
//@[17:32) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('a','b')|
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[26:69) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(notRuntime, string(moreIndirection))|
var moreIndirection = reference('s','s', 'Full')

var myRef = [
  evenMoreIndirection
]
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[04:16) [no-unused-vars (Warning)] Variable "runtimeLoop5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |runtimeLoop5|
//@[41:46) [BCP178 (Error)] This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("myRef" -> "evenMoreIndirection" -> "moreIndirection" -> "reference"). (bicep https://aka.ms/bicep/core-diagnostics#BCP178) |myRef|

// cannot use loops in expressions
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[04:18) [no-unused-vars (Warning)] Variable "loopExpression" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |loopExpression|
//@[28:31) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
//@[41:46) [BCP057 (Error)] The name "stuff" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |stuff|
//@[53:56) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (bicep https://aka.ms/bicep/core-diagnostics#BCP138) |for|
//@[66:71) [BCP057 (Error)] The name "stuff" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |stuff|

@batchSize(1)
//@[01:10) [BCP126 (Error)] Function "batchSize" cannot be used as a variable decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP126) |batchSize|
var batchSizeMakesNoSenseHere = false
//@[04:29) [no-unused-vars (Warning)] Variable "batchSizeMakesNoSenseHere" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |batchSizeMakesNoSenseHere|


//KeyVault Secret Reference
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[04:21) [no-unused-vars (Warning)] Variable "keyVaultSecretVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |keyVaultSecretVar|
//@[24:48) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[04:33) [no-unused-vars (Warning)] Variable "keyVaultSecretInterpolatedVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |keyVaultSecretInterpolatedVar|
//@[36:65) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (bicep core linter https://aka.ms/bicep/linter-diagnostics#simplify-interpolation) |'${kv.getSecret('mySecret')}'|
//@[39:63) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
var keyVaultSecretObjectVar = {
//@[04:27) [no-unused-vars (Warning)] Variable "keyVaultSecretObjectVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |keyVaultSecretObjectVar|
  secret: kv.getSecret('mySecret')
//@[10:34) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
}
var keyVaultSecretArrayVar = [
//@[04:26) [no-unused-vars (Warning)] Variable "keyVaultSecretArrayVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |keyVaultSecretArrayVar|
  kv.getSecret('mySecret')
//@[02:26) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
]
var keyVaultSecretArrayInterpolatedVar = [
//@[04:38) [no-unused-vars (Warning)] Variable "keyVaultSecretArrayInterpolatedVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |keyVaultSecretArrayInterpolatedVar|
  '${kv.getSecret('mySecret')}'
//@[05:29) [BCP180 (Error)] Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator. (bicep https://aka.ms/bicep/core-diagnostics#BCP180) |kv.getSecret('mySecret')|
]

var listSecrets= ''
var listSecretsVar = listSecrets()
//@[04:18) [no-unused-vars (Warning)] Variable "listSecretsVar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |listSecretsVar|
//@[21:32) [BCP265 (Error)] The name "listSecrets" is not a function. Did you mean "az.listSecrets"? (bicep https://aka.ms/bicep/core-diagnostics#BCP265) |listSecrets|

var copy = [
//@[04:08) [BCP239 (Error)] Identifier "copy" is a reserved Bicep symbol name and cannot be used in this context. (bicep https://aka.ms/bicep/core-diagnostics#BCP239) |copy|
//@[04:08) [no-unused-vars (Warning)] Variable "copy" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |copy|
  {
    name: 'one'
    count: '[notAFunction()]'
    input: {}
  }
]

