
// wrong declaration
bad
//@[0:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |bad|

// incomplete #completionTest(9) -> empty
resource 
//@[9:9) [BCP017 (Error)] Expected a resource identifier at this location. (CodeDescription: none) ||
//@[9:9) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
resource foo
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[12:12) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
//@[12:12) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
resource fo/o
//@[11:12) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |/|
//@[11:13) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |/o|
//@[13:13) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
resource foo 'ddd'
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
//@[18:18) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(23) -> resourceTypes
resource trailingSpace  
//@[24:24) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
//@[24:24) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||

// #completionTest(19,20) -> resourceObject
resource foo 'ddd'= 
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
//@[20:20) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

// wrong resource type
resource foo 'ddd'={
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
}

resource foo 'ddd'=if (1 + 1 == 2) {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:58) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. (CodeDescription: none) |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:58) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. (CodeDescription: none) |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[9:12) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[9:12) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[56:60) [BCP057 (Error)] The name "name" does not exist in the current context. (CodeDescription: none) |name|
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[9:12) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[58:58) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) ||
//@[63:64) [BCP057 (Error)] The name "b" does not exist in the current context. (CodeDescription: none) |b|
//@[65:65) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) ||
}

// simulate typing if condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[54:54) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[56:56) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[60:60) [BCP018 (Error)] Expected the ")" character at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[61:61) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) ||

// missing condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[55:56) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |{|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// empty condition
// #completionTest(56) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[56:57) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// #completionTest(57, 59) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[61:62) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// invalid condition type
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[55:60) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |(123)|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// runtime functions are no allowed in resource conditions
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[57:66) [BCP066 (Error)] Function "reference" is not valid at this location. It can only be used in resource declarations. (CodeDescription: none) |reference|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[57:65) [BCP066 (Error)] Function "listKeys" is not valid at this location. It can only be used in resource declarations. (CodeDescription: none) |listKeys|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[2:6) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |name|
  name: true
//@[2:6) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |name|
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[2:6) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |name|
  'name': true
//@[2:8) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'name'|
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
  properties: {
    foo: 'a'
//@[4:7) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |foo|
    foo: 'a'
//@[4:7) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |foo|
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
  properties: {
    foo: 'a'
//@[4:7) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |foo|
    'foo': 'a'
//@[4:9) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'foo'|
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[8:13) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
  location: [
//@[12:18) [BCP036 (Error)] The property "location" expected a value of type "string" but the provided value is of type "array". (CodeDescription: none) |[\r\n  ]|
  ]
  tags: 'tag are not a string?'
//@[8:31) [BCP036 (Error)] The property "tags" expected a value of type "Tags" but the provided value is of type "'tag are not a string?'". (CodeDescription: none) |'tag are not a string?'|
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: true ? 's' : 'a' + 1
//@[21:28) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "int". (CodeDescription: none) |'a' + 1|
  properties: {
    x: foo()
//@[7:10) [BCP059 (Error)] The name "foo" is not a function. (CodeDescription: none) |foo|
    y: true && (null || !4)
//@[24:26) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". (CodeDescription: none) |!4|
    a: [
      a
//@[6:7) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|
      !null
//@[6:11) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (CodeDescription: none) |!null|
      true && true || true + -true * 4
//@[29:34) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". (CodeDescription: none) |-true|
    ]
  }
}

// there should be no completions without the colon
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8) -> empty
  kind  
//@[8:8) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
}

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8) -> empty
  kind  :
//@[9:9) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}

// unsupported resource ref
var resrefvar = bar.name
//@[4:13) [no-unused-vars (Warning)] Variable "resrefvar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resrefvar|

param resrefpar string = foo.id
//@[6:15) [no-unused-params (Warning)] Parameter "resrefpar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |resrefpar|
//@[25:28) [BCP062 (Error)] The referenced declaration with name "foo" is not valid. (CodeDescription: none) |foo|
//@[25:28) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. (CodeDescription: none) |foo|

output resrefout bool = bar.id
//@[24:30) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "string". (CodeDescription: none) |bar.id|

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[13:50) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  id: 2
//@[2:4) [BCP073 (Error)] The property "id" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |id|
  type: 'hello'
//@[2:6) [BCP073 (Error)] The property "type" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |type|
  apiVersion: true
//@[2:12) [BCP073 (Error)] The property "apiVersion" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |apiVersion|
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[20:57) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: [
    baz.id
//@[4:10) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "string". (CodeDescription: none) |baz.id|
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[21:58) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: [
    'hello'
//@[4:11) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "'hello'". (CodeDescription: none) |'hello'|
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "bool". (CodeDescription: none) |true|
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[21:58) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[21:58) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[21:58) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: badDepends3.dependsOn
//@[25:34) [BCP077 (Error)] The property "dependsOn" on type "Microsoft.Foo/foos@2020-02-02-alpha" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |dependsOn|
}

var interpVal = 'abc'
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[19:56) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
  '${undefinedSymbol}': true
//@[5:20) [BCP057 (Error)] The name "undefinedSymbol" does not exist in the current context. (CodeDescription: none) |undefinedSymbol|
}

module validModule './module.bicep' = {
  name: 'storageDeploy'
  params: {
//@[2:8) [no-hardcoded-location (Warning)] The 'location' parameter for module 'validModule' should be assigned an explicit value. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |params|
    name: 'contoso'
  }
}

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'name1'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    evictionPolicy: 'Deallocate'
  }
}

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[8:89) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)|
  kind:'AzureCLI'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes1.name}_v1'
}

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(validModule['name'], 'v1')
//@[8:41) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(validModule['name'], 'v1')|
}

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${validModule.name}_v1'
}

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.location
//@[8:33) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
}

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['location']
//@[8:36) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['location']|
}

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: runtimeValidRes1.properties.evictionPolicy
//@[8:35) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "AzureCLI" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.properties|
  kind:'AzureCLI'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties'].evictionPolicy
//@[8:38) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['properties']|
}

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[8:38) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['properties']|
}

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.properties['evictionPolicy']
//@[8:35) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.properties|
}

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2.properties.azCliVersion
//@[8:35) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2.properties|
}

var magicString1 = 'location'
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2['${magicString1}']
//@[8:43) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['${magicString1}']|
}

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
var magicString2 = 'name'
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2['${magicString2}']
//@[8:43) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['${magicString2}']|
}

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes3.location}'
//@[11:36) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes3 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes3.location|
//@[28:36) [BCP187 (Warning)] The property "location" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |location|
}

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: validModule.params['name']
//@[8:26) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of validModule which can be calculated at the start include "name". (CodeDescription: none) |validModule.params|
//@[20:26) [BCP077 (Error)] The property "params" on type "module" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |params|
}

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
//@[8:143) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)|
//@[15:40) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
//@[42:70) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['location']|
//@[72:104) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeInvalidRes3 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalidRes3['properties']|
//@[119:137) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of validModule which can be calculated at the start include "name". (CodeDescription: none) |validModule.params|
//@[131:137) [BCP077 (Error)] The property "params" on type "module" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |params|
}

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
//@[11:36) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
//@[39:67) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['location']|
//@[70:99) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeInvalidRes3 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalidRes3.properties|
//@[118:139) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of validModule which can be calculated at the start include "name". (CodeDescription: none) |validModule['params']|
//@[130:138) [BCP077 (Error)] The property "params" on type "module" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |'params'|
}

// variable related runtime validation
var runtimefoo1 = runtimeValidRes1['location']
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
var runtimefoo3 = runtimeValidRes2
var runtimefoo4 = {
  hop: runtimefoo2
}

var runtimeInvalid = {
  foo1: runtimefoo1
  foo2: runtimefoo2
  foo3: runtimefoo3
  foo4: runtimeValidRes1.name
}

var runtimeValid = {
  foo1: runtimeValidRes1.name
  foo2: runtimeValidRes1.apiVersion
  foo3: runtimeValidRes2.type
  foo4: runtimeValidRes2.id
}

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo1
//@[8:22) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo2
//@[8:22) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo3.properties.azCliVersion
//@[8:22) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo4
//@[8:22) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
//@[8:129) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)|
//@[15:29) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
//@[36:66) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['properties']|
//@[84:109) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
//@[113:124) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimefoo4" -> "runtimefoo2" -> "runtimeValidRes2"). Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimefoo4|
}

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValid.foo1
}

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValid.foo2
}

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValid.foo3
}

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValid.foo4
}


resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[9:28) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck|
  name: 'test'
  location: 'test'
//@[12:18) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
var runtimeCheckVar2 = runtimeCheckVar

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: runtimeCheckVar2
//@[8:24) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeCheckVar2" -> "runtimeCheckVar" -> "loopForRuntimeCheck"). Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeCheckVar2|
  location: 'test'
//@[12:18) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[9:29) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck2|
  name: runtimeCheckVar2
//@[8:24) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeCheckVar2" -> "runtimeCheckVar" -> "loopForRuntimeCheck"). Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeCheckVar2|
  location: 'test'
//@[12:18) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[9:29) [BCP179 (Warning)] The loop item variable "otherThing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck3|
  name: loopForRuntimeCheck[0].properties.zoneType
//@[8:41) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |loopForRuntimeCheck[0].properties|
  location: 'test'
//@[12:18) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
var varForRuntimeCheck4b = varForRuntimeCheck4a
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[9:29) [BCP179 (Warning)] The loop item variable "otherThing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck4|
  name: varForRuntimeCheck4b
//@[8:28) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("varForRuntimeCheck4b" -> "varForRuntimeCheck4a" -> "loopForRuntimeCheck"). Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |varForRuntimeCheck4b|
  location: 'test'
//@[12:18) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". (CodeDescription: none) |missingTopLevelProperties|
  // #completionTest(0, 1, 2) -> topLevelProperties
  
}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:44) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "sku". (CodeDescription: none) |missingTopLevelPropertiesExceptName|
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'v'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    subnets: [
      {
        // #completionTest(0,1,2,3,4,5,6,7) -> subnetPropertiesMinusProperties
       
        // #completionTest(0,1,2,3,4,5,6,7) -> empty
        properties: {
          delegations: [
            {
              // #completionTest(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14) -> delegationProperties
              
            }
          ]
        }
      }
    ]
  }
}

/*
Discriminator key missing
*/
resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[86:148) [BCP078 (Error)] The property "kind" requires a value of type "'AzureCLI' | 'AzurePowerShell'", but none was supplied. (CodeDescription: none) |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (conditional)
*/
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[98:160) [BCP078 (Error)] The property "kind" requires a value of type "'AzureCLI' | 'AzurePowerShell'", but none was supplied. (CodeDescription: none) |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (loop)
*/
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[108:170) [BCP078 (Error)] The property "kind" requires a value of type "'AzureCLI' | 'AzurePowerShell'", but none was supplied. (CodeDescription: none) |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key missing (filtered loop)
*/
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[120:182) [BCP078 (Error)] The property "kind" requires a value of type "'AzureCLI' | 'AzurePowerShell'", but none was supplied. (CodeDescription: none) |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key value missing with property access
*/
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
  kind:   
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[4:43) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[4:44) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2|
//@[76:76) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[4:44) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3|
//@[76:76) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator key value missing with property access (conditional)
*/
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
  kind:   
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[4:46) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions_if|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[4:47) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2_if|
//@[82:82) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[4:47) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3_if|
//@[82:82) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator key value missing with property access (loops)
*/
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
  kind:   
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[4:35) [no-unused-vars (Warning)] Variable "resourceListIsNotSingleResource" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resourceListIsNotSingleResource|
//@[38:70) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |discriminatorKeyValueMissing_for|
//@[71:75) [BCP055 (Error)] Cannot access properties of type "Microsoft.Resources/deploymentScripts@2020-10-01[]". An "object" type is required. (CodeDescription: none) |kind|

// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[4:47) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions_for|
// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[4:48) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2_for|
//@[87:87) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[4:48) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3_for|
//@[87:87) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator key value missing with property access (filtered loops)
*/
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
  kind:   
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[4:38) [no-unused-vars (Warning)] Variable "resourceListIsNotSingleResource_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resourceListIsNotSingleResource_if|
//@[41:76) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |discriminatorKeyValueMissing_for_if|
//@[77:81) [BCP055 (Error)] Cannot access properties of type "Microsoft.Resources/deploymentScripts@2020-10-01[]". An "object" type is required. (CodeDescription: none) |kind|

// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[4:50) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions_for_if|
// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[4:51) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2_for_if|
//@[93:93) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[4:51) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3_for_if|
//@[93:93) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1
*/
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetOne|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[4:37) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions|
//@[74:75) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[4:38) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2|
//@[75:75) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[4:38) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3|
//@[75:75) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1 (conditional)
*/
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetOne_if|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[4:40) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions_if|
//@[80:81) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[4:41) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2_if|
//@[81:81) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[4:41) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3_if|
//@[81:81) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1 (loop)
*/
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[9:35) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetOne_for|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(86) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[4:41) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions_for|
//@[85:86) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(94) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[4:42) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2_for|
//@[94:94) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[4:42) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3_for|
//@[86:86) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1 (filtered loop)
*/
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[9:38) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetOne_for_if|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(92) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[4:44) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions_for_if|
//@[91:92) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(100) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[4:45) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2_for_if|
//@[100:100) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[4:45) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3_for_if|
//@[92:92) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||


/*
Discriminator value set 2
*/
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetTwo|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[4:37) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions|
//@[74:75) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[4:38) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2|
//@[75:75) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[4:49) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer|
//@[89:90) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[4:50) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2|
//@[90:90) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

/*
Discriminator value set 2 (conditional)
*/
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetTwo_if|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[4:40) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions_if|
//@[80:81) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[4:41) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2_if|
//@[81:81) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[4:52) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer_if|
//@[95:96) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[4:53) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2_if|
//@[96:96) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||


/*
Discriminator value set 2 (loops)
*/
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[9:35) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetTwo_for|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[4:41) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions_for|
//@[85:86) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[4:42) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2_for|
//@[86:86) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[4:53) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer_for|
//@[100:101) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[4:54) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2_for|
//@[101:101) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||


/*
Discriminator value set 2 (filtered loops)
*/
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[9:38) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |discriminatorKeySetTwo_for_if|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". (CodeDescription: none) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[4:44) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions_for_if|
//@[91:92) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[4:45) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2_for_if|
//@[92:92) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[4:56) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer_for_if|
//@[106:107) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[4:57) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2_for_if|
//@[107:107) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name", "properties". (CodeDescription: none) |incorrectPropertiesKey|
  kind: 'AzureCLI'

  propertes: {
//@[2:11) [BCP089 (Error)] The property "propertes" is not allowed on objects of type "AzureCLI". Did you mean "properties"? (CodeDescription: none) |propertes|
  }
}

var mock = incorrectPropertiesKey.p
//@[4:8) [no-unused-vars (Warning)] Variable "mock" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mock|
//@[34:35) [BCP053 (Error)] The type "AzureCLI" does not contain property "p". Available properties include "apiVersion", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "systemData", "tags", "type", "zones". (CodeDescription: none) |p|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'
  name: 'test'
  location: ''
//@[12:14) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found '' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |''|
  properties: {
    azCliVersion: '2'
    retentionInterval: 'PT1H'
    
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
    
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
    cleanupPreference: 
//@[23:23) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

    // #completionTest(25,26) -> arrayPlusSymbols
    supportingScriptUris: 
//@[26:26) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

    // #completionTest(27,28) -> objectPlusSymbols
    storageAccountSettings: 
//@[28:28) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

    environmentVariables: [
      {
        // #completionTest(0,2,4,6,8) -> environmentVariableProperties
        
      }
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbolsWithRequiredProperties
      
    ]
  }
}

// #completionTest(21) -> resourceTypes
resource missingType 
//@[21:21) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
//@[21:21) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
resource startedTypingTypeWithQuotes 'virma'
//@[37:44) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'virma'|
//@[44:44) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(40,41,42,43,44,45) -> resourceTypes
resource startedTypingTypeWithoutQuotes virma
//@[40:45) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |virma|
//@[40:45) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |virma|
//@[45:45) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[9:30) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". (CodeDescription: none) |dashesInPropertyNames|
}
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[4:23) [no-unused-vars (Warning)] Variable "letsAccessTheDashes" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |letsAccessTheDashes|
//@[77:78) [BCP053 (Warning)] The type "ManagedClusterPropertiesAutoScalerProfile" does not contain property "s". Available properties include "balance-similar-node-groups", "expander", "max-empty-bulk-delete", "max-graceful-termination-sec", "max-total-unready-percentage", "new-pod-scale-up-delay", "ok-total-unready-count", "scale-down-delay-after-add", "scale-down-delay-after-delete", "scale-down-delay-after-failure", "scale-down-unneeded-time", "scale-down-unready-time", "scale-down-utilization-threshold", "scan-interval", "skip-nodes-with-local-storage", "skip-nodes-with-system-pods". (CodeDescription: none) |s|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[4:24) [no-unused-vars (Warning)] Variable "letsAccessTheDashes2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |letsAccessTheDashes2|
//@[78:78) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

/* 
Nested discriminator missing key
*/
resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[14:51) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}
// #completionTest(90) -> createMode
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[4:44) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions|
// #completionTest(92) -> createMode
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[4:45) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2|
//@[92:92) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[4:49) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions|

/* 
Nested discriminator missing key (conditional)
*/
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[14:51) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}
// #completionTest(96) -> createMode
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[4:47) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions_if|
// #completionTest(98) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[4:48) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2_if|
//@[98:98) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(100) -> createModeIndexPlusSymbols_if
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[4:52) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions_if|

/* 
Nested discriminator missing key (loop)
*/
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[9:42) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminatorMissingKey_for|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[14:51) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}]
// #completionTest(101) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[4:48) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions_for|
// #completionTest(103) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[4:49) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2_for|
//@[103:103) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(105) -> createModeIndexPlusSymbols_for
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[4:53) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions_for|


/* 
Nested discriminator missing key (filtered loop)
*/
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[9:45) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminatorMissingKey_for_if|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[14:51) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}]
// #completionTest(107) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[4:51) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions_for_if|
// #completionTest(109) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[4:52) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2_for_if|
//@[109:109) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[4:56) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions_for_if|


/*
Nested discriminator
*/
resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". (CodeDescription: none) |properties|
    createMode: 'Default'

  }
}
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[4:34) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions|
//@[68:69) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(73) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[4:35) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2|
//@[72:73) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[4:35) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3|
//@[69:69) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(72) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[4:35) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4|
//@[72:72) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(79) -> defaultCreateModeIndexes
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[4:44) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions|
//@[78:79) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|

/*
Nested discriminator (conditional)
*/
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". (CodeDescription: none) |properties|
    createMode: 'Default'

  }
}
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[4:37) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions_if|
//@[74:75) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(79) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[4:38) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2_if|
//@[78:79) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[4:38) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3_if|
//@[75:75) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(78) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[4:38) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4_if|
//@[78:78) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(85) -> defaultCreateModeIndexes_if
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[4:47) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions_if|
//@[84:85) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|


/*
Nested discriminator (loop)
*/
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[9:32) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminator_for|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". (CodeDescription: none) |properties|
    createMode: 'Default'

  }
}]
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[4:38) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions_for|
//@[79:80) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(84) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[4:39) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2_for|
//@[83:84) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[4:39) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3_for|
//@[80:80) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(83) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[4:39) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4_for|
//@[83:83) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(90) -> defaultCreateModeIndexes_for
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[4:48) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions_for|
//@[89:90) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|


/*
Nested discriminator (filtered loop)
*/
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[9:35) [BCP179 (Warning)] The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminator_for_if|
  name: 'test'
//@[8:14) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[12:15) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[2:12) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". (CodeDescription: none) |properties|
    createMode: 'Default'

  }
}]
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[4:41) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions_for_if|
//@[85:86) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(90) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[4:42) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2_for_if|
//@[89:90) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[4:42) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3_for_if|
//@[86:86) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(89) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[4:42) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4_for_if|
//@[89:89) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(96) -> defaultCreateModeIndexes_for_if
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[4:51) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions_for_if|
//@[95:96) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|



// sample resource to validate completions on the next declarations
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
  location: 'test'
//@[12:18) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
  name: 'test'
  properties: {
    additionalCapabilities: {
      
    }
  }
}
// this validates that we can get nested property access completions on a conditional resource
//#completionTest(56) -> vmProperties
var sigh = nestedPropertyAccessOnConditional.properties.
//@[4:8) [no-unused-vars (Warning)] Variable "sigh" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sigh|
//@[56:56) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

/*
  boolean property value completions
*/ 
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  properties: {
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
    autoUpgradeMinorVersion: t
//@[29:30) [BCP057 (Error)] The name "t" does not exist in the current context. (CodeDescription: none) |t|
  }
}

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[19:50) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'selfScope'
  scope: selfScope
//@[9:18) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |selfScope|
}

var notAResource = {
  im: 'not'
  a: 'resource!'
}
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[22:53) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'invalidScope'
  scope: notAResource
//@[9:21) [BCP036 (Error)] The property "scope" expected a value of type "resource | tenant" but the provided value is of type "object". (CodeDescription: none) |notAResource|
}

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[23:54) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'invalidScope2'
  scope: resourceGroup()
}

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[23:54) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'invalidScope3'
  scope: subscription()
//@[9:23) [BCP139 (Error)] The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module. (CodeDescription: none) |subscription()|
}

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[31:64) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'invalidDuplicateName'
//@[8:30) [BCP121 (Error)] Resources: "invalidDuplicateName1", "invalidDuplicateName2", "invalidDuplicateName3" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidDuplicateName'|
}
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[31:64) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'invalidDuplicateName'
//@[8:30) [BCP121 (Error)] Resources: "invalidDuplicateName1", "invalidDuplicateName2", "invalidDuplicateName3" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidDuplicateName'|
}
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[31:64) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2019-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2019-01-01'|
  name: 'invalidDuplicateName'
//@[8:30) [BCP121 (Error)] Resources: "invalidDuplicateName1", "invalidDuplicateName2", "invalidDuplicateName3" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidDuplicateName'|
}

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[63:96) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
}

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[48:84) [BCP081 (Warning)] Resource type "Mock.Rp/mockExtResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExtResource@2020-01-01'|
  name: 'invalidExtensionResourceDuplicateName'
//@[8:47) [BCP121 (Error)] Resources: "invalidExtensionResourceDuplicateName1", "invalidExtensionResourceDuplicateName2" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidExtensionResourceDuplicateName'|
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[48:84) [BCP081 (Warning)] Resource type "Mock.Rp/mockExtResource@2019-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExtResource@2019-01-01'|
  name: 'invalidExtensionResourceDuplicateName'
//@[8:47) [BCP121 (Error)] Resources: "invalidExtensionResourceDuplicateName1", "invalidExtensionResourceDuplicateName2" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidExtensionResourceDuplicateName'|
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

@concat('foo', 'bar')
//@[1:7) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (CodeDescription: none) |concat|
@secure()
//@[1:7) [BCP127 (Error)] Function "secure" cannot be used as a resource decorator. (CodeDescription: none) |secure|
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[26:63) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'invalidDecorator'
}

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[19:60) [BCP081 (Warning)] Resource type "Mock.Rp/mockExistingResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExistingResource@2020-01-01'|
  name: 'cyclicRes'
  scope: cyclicRes
//@[9:18) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |cyclicRes|
}

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[27:68) [BCP081 (Warning)] Resource type "Mock.Rp/mockExistingResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExistingResource@2020-01-01'|
  name: 'cyclicExistingRes'
  scope: cyclicExistingRes
//@[9:26) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |cyclicExistingRes|
}

// loop parsing cases
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[78:79) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |]|

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[79:80) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |f|

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[78:78) [BCP162 (Error)] Expected a loop item variable identifier or "(" at this location. (CodeDescription: none) ||

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[82:83) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[84:85) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |b|
//@[85:86) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[91:92) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[82:83) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[83:84) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[85:86) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[87:88) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// loop index parsing cases
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[78:79) [BCP136 (Error)] Expected a loop item variable identifier at this location. (CodeDescription: none) |)|

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[79:79) [BCP136 (Error)] Expected a loop item variable identifier at this location. (CodeDescription: none) ||

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[72:73) [BCP018 (Error)] Expected the "," character at this location. (CodeDescription: none) |)|

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[82:83) [BCP163 (Error)] Expected a loop index variable identifier at this location. (CodeDescription: none) |)|

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[81:82) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[82:83) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |z|
//@[83:84) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[91:92) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[81:82) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[82:83) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[84:85) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[86:87) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// loop filter parsing cases
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[96:97) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[101:102) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[95:96) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[100:101) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[103:104) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[109:110) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
//@[110:111) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[102:103) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[108:109) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |)|
//@[109:110) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|

// wrong body type
var emptyArray = []
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[97:98) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[103:104) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|

// duplicate variable in the same scope
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[9:29) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |itemAndIndexSameName|
//@[80:84) [BCP028 (Error)] Identifier "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|
//@[86:90) [BCP028 (Error)] Identifier "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|
}]

// errors in the array expression
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[106:107) [BCP070 (Error)] Argument of type "int" is not assignable to parameter of type "array". (CodeDescription: none) |2|
}]
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[111:112) [BCP070 (Error)] Argument of type "int" is not assignable to parameter of type "array". (CodeDescription: none) |2|
}]

// wrong array type
var notAnArray = true
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[89:99) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "bool". (CodeDescription: none) |notAnArray|
}]
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[94:104) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "bool". (CodeDescription: none) |notAnArray|
}]

// wrong filter expression type
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". (CodeDescription: none) |wrongFilterExpressionType|
//@[114:117) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |(4)|
}]
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[9:35) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". (CodeDescription: none) |wrongFilterExpressionType2|
//@[119:132) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |(concat('s'))|
}]

// missing required properties
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". (CodeDescription: none) |missingRequiredProperties|
}]
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[9:35) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". (CodeDescription: none) |missingRequiredProperties2|
}]

// fewer missing required properties and a wrong property
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[9:39) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "sku". (CodeDescription: none) |missingFewerRequiredProperties|
  name: account
  location: 'eastus42'
//@[12:22) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus42' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus42'|
  properties: {
    wrong: 'test'
//@[4:9) [BCP037 (Warning)] The property "wrong" is not allowed on objects of type "StorageAccountPropertiesCreateParameters". Permissible properties include "accessTier", "allowBlobPublicAccess", "allowSharedKeyAccess", "azureFilesIdentityBasedAuthentication", "customDomain", "encryption", "isHnsEnabled", "largeFileSharesState", "minimumTlsVersion", "networkAcls", "routingPreference", "supportsHttpsTrafficOnly". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |wrong|
  }
}]

// wrong property inside the nested property loop
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      doesNotExist: 'test'
//@[6:18) [BCP037 (Warning)] The property "doesNotExist" is not allowed on objects of type "Subnet". Permissible properties include "id", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |doesNotExist|
      name: 'subnet-${i}-${j}'
    }]
  }
}]
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      doesNotExist: 'test'
//@[6:18) [BCP037 (Warning)] The property "doesNotExist" is not allowed on objects of type "Subnet". Permissible properties include "id", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |doesNotExist|
      name: 'subnet-${i}-${j}-${k}'
    }]
  }
}]

// nonexistent arrays and loop variables
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[9:26) [BCP179 (Warning)] The loop item variable "i" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nonexistentArrays|
//@[86:95) [BCP057 (Error)] The name "notAThing" does not exist in the current context. (CodeDescription: none) |notAThing|
  name: 'vnet-${justPlainWrong}'
//@[16:30) [BCP057 (Error)] The name "justPlainWrong" does not exist in the current context. (CodeDescription: none) |justPlainWrong|
  properties: {
    subnets: [for j in alsoNotAThing: {
//@[23:36) [BCP057 (Error)] The name "alsoNotAThing" does not exist in the current context. (CodeDescription: none) |alsoNotAThing|
      doesNotExist: 'test'
      name: 'subnet-${fake}-${totallyFake}'
//@[22:26) [BCP057 (Error)] The name "fake" does not exist in the current context. (CodeDescription: none) |fake|
//@[30:41) [BCP057 (Error)] The name "totallyFake" does not exist in the current context. (CodeDescription: none) |totallyFake|
    }]
  }
}]

// property loops cannot be nested
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[98:113) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {

    networkAcls: {
      virtualNetworkRules: [for rule in []: {
        id: '${account.name}-${account.location}'
        state: [for lol in []: 4]
//@[16:19) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
      }]
    }
  }
}]
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[9:33) [BCP028 (Error)] Identifier "propertyLoopsCannotNest2" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |propertyLoopsCannotNest2|
//@[103:118) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {

    networkAcls: {
      virtualNetworkRules: [for (rule,j) in []: {
        id: '${account.name}-${account.location}'
        state: [for (lol,k) in []: 4]
//@[16:19) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
      }]
    }
  }
}]

// property loops cannot be nested (even more nesting)
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[9:33) [BCP028 (Error)] Identifier "propertyLoopsCannotNest2" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |propertyLoopsCannotNest2|
//@[99:114) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls:  {
      virtualNetworkRules: [for rule in []: {
        // #completionTest(15,31) -> symbolsPlusRule
        id: '${account.name}-${account.location}'
        state: [for state in []: {
//@[16:19) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
          fake: [for something in []: true]
//@[17:20) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
        }]
      }]
    }
  }
}]

// loops cannot be used inside of expressions
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[81:96) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      virtualNetworkRules: concat([for lol in []: {
//@[35:38) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
        id: '${account.name}-${account.location}'
      }])
    }
  }
}]

// using the same loop variable in a new language scope should be allowed
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[90:105) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  // #completionTest(7,8) -> symbolsPlusAccount2
  name: account.name
  location: account.location
  sku: {
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
    name: 
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  }
  kind: 'StorageV2'
}]

var directRefViaVar = premiumStorages
//@[4:19) [no-unused-vars (Warning)] Variable "directRefViaVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |directRefViaVar|
//@[22:37) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[40:55) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
//@[57:63) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |stuffs|

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone2'
  location: 'global'
  properties: {
    registrationVirtualNetworks: premiumStorages
//@[33:48) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
  }
}

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
  name: 'myZone3'
  location: 'global'
  properties: {
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
//@[40:55) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
//@[57:63) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |stuffs|
  }
}

@batchSize()
//@[10:12) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
//@[13:28) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
  }
}]

@batchSize(0)
//@[11:12) [BCP154 (Error)] Expected a batch size of at least 1 but the specified value was "0". (CodeDescription: none) |0|
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
//@[13:28) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
    dependsOn: [
//@[4:13) [BCP037 (Warning)] The property "dependsOn" is not allowed on objects of type "VirtualNetworkPropertiesFormat". Permissible properties include "addressSpace", "bgpCommunities", "ddosProtectionPlan", "dhcpOptions", "enableDdosProtection", "enableVmProtection", "ipAllocations", "virtualNetworkPeerings". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |dependsOn|
      premiumStorages
//@[6:21) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
    ]
  }
  dependsOn: [
    
  ]
}]

var expressionInPropertyLoopVar = true
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'hello'
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[4:61) [BCP143 (Error)] For-expressions cannot be used with properties whose names are also expressions. (CodeDescription: none) |'resolutionVirtualNetworks${expressionInPropertyLoopVar}'|
//@[4:61) [BCP040 (Warning)] String interpolation is not supported for keys on objects of type "ZoneProperties". Permissible properties include "registrationVirtualNetworks", "resolutionVirtualNetworks", "zoneType". (CodeDescription: none) |'resolutionVirtualNetworks${expressionInPropertyLoopVar}'|
  }
}

// resource loop body that isn't an object
@batchSize(-1)
//@[11:13) [BCP154 (Error)] Expected a batch size of at least 1 but the specified value was "-1". (CodeDescription: none) |-1|
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[95:101) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'test'|
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[96:107) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |environment|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[9:35) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody3" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody3|
//@[100:106) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'test'|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[9:35) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody4" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody4|
//@[100:111) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |environment|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[9:35) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody3" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody3|
//@[109:115) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |'test'|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[9:35) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody4" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody4|
//@[109:120) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |environment|

// #completionTest(54,55) -> objectPlusFor
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[55:55) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
  properties: {
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
    registrationVirtualNetworks: 
//@[33:33) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
    resolutionVirtualNetworks: [for lol in []: {
      
    }]
  }
}]

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  properties: {
    virtualNetworkPeerings: [for item in []: {
        properties: {
          remoteAddressSpace: {
            // #completionTest(28,29) -> symbolsPlusArrayWithoutFor
            addressPrefixes: 
//@[29:29) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
          }
        }
    }]
  }
}

// parent property with 'existing' resource at different scope
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  scope: subscription()
  name: 'res1'
}

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[19:62) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[65:106) [BCP165 (Error)] Cannot deploy a resource with ancestor under a different scope. Resource "p1_res1" has the "scope" property set. (CodeDescription: none) |{\r\n  parent: p1_res1\r\n  name: 'child1'\r\n}|
  parent: p1_res1
  name: 'child1'
}

// parent property with scope on child resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
//@[8:14) [BCP121 (Error)] Resources: "p2_res1", "p5_res1", "p7_res1" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'res1'|
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2@2020-06-01'|
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[22:65) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
  scope: p2_res1
//@[9:16) [BCP164 (Error)] The "scope" property is unsupported for a resource with a parent resource. This resource has "p2_res2" declared as its parent. (CodeDescription: none) |p2_res1|
  parent: p2_res2
  name: 'child2'
}

// parent property self-cycle
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  parent: p3_vmExt
//@[10:18) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |p3_vmExt|
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}

// parent property 2-cycle
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  parent: p4_vmExt
//@[10:18) [BCP080 (Error)] The expression is involved in a cycle ("p4_vmExt" -> "p4_vm"). (CodeDescription: none) |p4_vmExt|
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  parent: p4_vm
//@[10:15) [BCP080 (Error)] The expression is involved in a cycle ("p4_vm" -> "p4_vmExt"). (CodeDescription: none) |p4_vm|
  location: 'eastus'
//@[12:20) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}

// parent property with invalid child
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
//@[8:14) [BCP121 (Error)] Resources: "p2_res1", "p5_res1", "p7_res1" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'res1'|
}

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[17:60) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
  parent: p5_res1
//@[10:17) [BCP036 (Error)] The property "parent" expected a value of type "Microsoft.Rp2/resource2" but the provided value is of type "Microsoft.Rp1/resource1@2020-06-01". (CodeDescription: none) |p5_res1|
  name: 'res2'
}

// parent property with invalid parent
resource p6_res1 '${true}' = {
//@[17:26) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. (CodeDescription: none) |'${true}'|
  name: 'res1'
}

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[17:60) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
  parent: p6_res1
//@[10:17) [BCP062 (Error)] The referenced declaration with name "p6_res1" is not valid. (CodeDescription: none) |p6_res1|
  name: 'res2'
}

// parent property with incorrectly-formatted name
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
//@[8:14) [BCP121 (Error)] Resources: "p2_res1", "p5_res1", "p7_res1" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'res1'|
}

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[17:60) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
  parent: p7_res1
  name: 'res1/res2'
//@[8:19) [BCP170 (Error)] Expected resource name to not contain any "/" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name. (CodeDescription: none) |'res1/res2'|
}

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[17:60) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
  parent: p7_res1
  name: '${p7_res1.name}/res2'
//@[8:30) [BCP170 (Error)] Expected resource name to not contain any "/" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name. (CodeDescription: none) |'${p7_res1.name}/res2'|
}

// top-level resource with too many '/' characters
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[17:53) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1/res2'
//@[8:19) [BCP169 (Error)] Expected resource name to contain 0 "/" character(s). The number of name segments must match the number of segments in the resource type. (CodeDescription: none) |'res1/res2'|
}

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
  name: 'existingResProperty'
  location: 'westeurope'
//@[2:10) [BCP173 (Error)] The property "location" cannot be used in an existing resource declaration. (CodeDescription: none) |location|
//@[12:24) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  properties: {}
//@[2:12) [BCP173 (Error)] The property "properties" cannot be used in an existing resource declaration. (CodeDescription: none) |properties|
}

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
    parent: existingResProperty
    name: 'myExt'
    location: existingResProperty.location
//@[14:42) [BCP120 (Error)] This expression is being used in an assignment to the "location" property of the "Microsoft.Compute/virtualMachines/extensions" type, which requires a value that can be calculated at the start of the deployment. Properties of existingResProperty which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingResProperty.location|
}

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'anyTypeInDependsOn'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
  dependsOn: [
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
//@[4:70) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)|
    's'
//@[4:7) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "'s'". (CodeDescription: none) |'s'|
    any(true)
//@[4:13) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(true)|
  ]
}

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[9:24) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInParent|
  parent: any(true)
//@[10:19) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(true)|
}

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[9:28) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInParentLoop|
  parent: any(true)
//@[10:19) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(true)|
}]

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[9:23) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name", "properties". (CodeDescription: none) |anyTypeInScope|
  scope: any(invalidExistingLocationRef)
//@[9:40) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(invalidExistingLocationRef)|
}

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name", "properties". (CodeDescription: none) |anyTypeInScopeConditional|
  scope: any(invalidExistingLocationRef)
//@[9:40) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(invalidExistingLocationRef)|
}

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInExistingScope|
  parent: any('')
//@[10:17) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('')|
  scope: any(false)
//@[9:19) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(false)|
}

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[9:35) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInExistingScopeLoop|
  parent: any('')
//@[10:17) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('')|
  scope: any(false)
//@[9:19) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(false)|
}]

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[89:131) [BCP135 (Error)] Scope "resourceGroup" is not valid for this resource type. Permitted scopes: "tenant". (CodeDescription: none) |{\r\n  name: 'tenantLevelResourceBlocked'\r\n}|
  name: 'tenantLevelResourceBlocked'
}

// #completionTest(15,36,37) -> resourceTypes
resource comp1 'Microsoft.Resources/'
//@[15:37) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/'|
//@[37:37) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(15,16,17) -> resourceTypes
resource comp2 ''
//@[15:17) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |''|
//@[17:17) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(38) -> resourceTypes
resource comp3 'Microsoft.Resources/t'
//@[15:38) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/t'|
//@[38:38) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(40) -> resourceTypes
resource comp4 'Microsoft.Resources/t/v'
//@[15:40) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/t/v'|
//@[40:40) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(49) -> resourceTypes
resource comp5 'Microsoft.Storage/storageAccounts'
//@[15:50) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts'|
//@[50:50) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(50) -> storageAccountsResourceTypes
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[15:51) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts@'|
//@[51:51) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(52) -> templateSpecsResourceTypes
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[15:53) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/templateSpecs@20'|
//@[53:53) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(60,61) -> virtualNetworksResourceTypes
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[61:61) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||


// issue #3000
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'issue3000LogicApp1'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
  properties: {
    state: 'Enabled'
    definition: ''
  }
  identity: {
    type: 'SystemAssigned'
  }
  extendedLocation: {}
//@[2:18) [BCP187 (Warning)] The property "extendedLocation" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |extendedLocation|
  sku: {}
//@[2:5) [BCP187 (Warning)] The property "sku" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |sku|
  kind: 'V1'
//@[2:6) [BCP187 (Warning)] The property "kind" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |kind|
  managedBy: 'string'
//@[2:11) [BCP187 (Warning)] The property "managedBy" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedBy|
  mangedByExtended: [
//@[2:18) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "Microsoft.Logic/workflows". Permissible properties include "dependsOn", "tags". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |mangedByExtended|
   'str1'
   'str2'
  ]
  zones: [
//@[2:7) [BCP187 (Warning)] The property "zones" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |zones|
   'str1'
   'str2'
  ]
  plan: {}
//@[2:6) [BCP187 (Warning)] The property "plan" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |plan|
  eTag: ''
//@[2:6) [BCP187 (Warning)] The property "eTag" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |eTag|
  scale: {}  
//@[2:7) [BCP187 (Warning)] The property "scale" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |scale|
}

resource issue3000LogicApp2 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'issue3000LogicApp2'
  location: resourceGroup().location
//@[12:36) [no-hardcoded-location (Warning)] Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |resourceGroup().location|
  properties: {
    state: 'Enabled'
    definition: ''
  }
  identity: 'SystemAssigned'
//@[12:28) [BCP036 (Error)] The property "identity" expected a value of type "ManagedServiceIdentity | null" but the provided value is of type "'SystemAssigned'". (CodeDescription: none) |'SystemAssigned'|
  extendedLocation: 'eastus'
//@[2:18) [BCP187 (Warning)] The property "extendedLocation" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |extendedLocation|
//@[20:28) [BCP036 (Error)] The property "extendedLocation" expected a value of type "object" but the provided value is of type "'eastus'". (CodeDescription: none) |'eastus'|
  sku: 'Basic'
//@[2:5) [BCP187 (Warning)] The property "sku" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |sku|
//@[7:14) [BCP036 (Error)] The property "sku" expected a value of type "object" but the provided value is of type "'Basic'". (CodeDescription: none) |'Basic'|
  kind: {
//@[2:6) [BCP187 (Warning)] The property "kind" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |kind|
//@[8:30) [BCP036 (Error)] The property "kind" expected a value of type "string" but the provided value is of type "object". (CodeDescription: none) |{\r\n    name: 'V1'\r\n  }|
    name: 'V1'
  }
  managedBy: {}
//@[2:11) [BCP187 (Warning)] The property "managedBy" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedBy|
//@[13:15) [BCP036 (Error)] The property "managedBy" expected a value of type "string" but the provided value is of type "object". (CodeDescription: none) |{}|
  mangedByExtended: [
//@[2:18) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "Microsoft.Logic/workflows". Permissible properties include "dependsOn", "tags". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |mangedByExtended|
   {}
   {}
  ]
  zones: [
//@[2:7) [BCP187 (Warning)] The property "zones" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |zones|
   {}
//@[3:5) [BCP034 (Error)] The enclosing array expected an item of type "string", but the provided item was of type "object". (CodeDescription: none) |{}|
   {}
//@[3:5) [BCP034 (Error)] The enclosing array expected an item of type "string", but the provided item was of type "object". (CodeDescription: none) |{}|
  ]
  plan: ''
//@[2:6) [BCP187 (Warning)] The property "plan" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |plan|
//@[8:10) [BCP036 (Error)] The property "plan" expected a value of type "object" but the provided value is of type "''". (CodeDescription: none) |''|
  eTag: {}
//@[2:6) [BCP187 (Warning)] The property "eTag" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |eTag|
//@[8:10) [BCP036 (Error)] The property "eTag" expected a value of type "string" but the provided value is of type "object". (CodeDescription: none) |{}|
  scale: [
//@[2:7) [BCP187 (Warning)] The property "scale" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |scale|
//@[9:21) [BCP036 (Error)] The property "scale" expected a value of type "object" but the provided value is of type "object[]". (CodeDescription: none) |[\r\n  {}\r\n  ]|
  {}
  ]  
}

resource issue3000stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'issue3000stg'
  kind: 'StorageV2'
  location: 'West US'
//@[12:21) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'West US' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'West US'|
  sku: {
    name: 'Premium_LRS'    
  }
  madeUpProperty: {}
//@[2:16) [BCP037 (Error)] The property "madeUpProperty" is not allowed on objects of type "Microsoft.Storage/storageAccounts". Permissible properties include "dependsOn", "extendedLocation", "identity", "properties", "tags". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |madeUpProperty|
  managedByExtended: []
//@[2:19) [BCP187 (Warning)] The property "managedByExtended" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedByExtended|
}

var issue3000stgMadeUpProperty = issue3000stg.madeUpProperty
//@[4:30) [no-unused-vars (Warning)] Variable "issue3000stgMadeUpProperty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |issue3000stgMadeUpProperty|
//@[46:60) [BCP053 (Error)] The type "Microsoft.Storage/storageAccounts" does not contain property "madeUpProperty". Available properties include "apiVersion", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "tags", "type", "zones". (CodeDescription: none) |madeUpProperty|
var issue3000stgManagedBy = issue3000stg.managedBy
//@[4:25) [no-unused-vars (Warning)] Variable "issue3000stgManagedBy" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |issue3000stgManagedBy|
//@[41:50) [BCP187 (Warning)] The property "managedBy" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedBy|
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[4:33) [no-unused-vars (Warning)] Variable "issue3000stgManagedByExtended" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |issue3000stgManagedByExtended|
//@[49:66) [BCP187 (Warning)] The property "managedByExtended" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedByExtended|

param dataCollectionRule object
param tags object

var defaultLogAnalyticsWorkspace = {
  subscriptionId: subscription().subscriptionId
}

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
  name: logAnalyticsWorkspace.name
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
}]

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
  name: dataCollectionRule.name
  location: dataCollectionRule.location
  tags: tags
  kind: dataCollectionRule.kind
  properties: {
    description: dataCollectionRule.description
    destinations: union(empty(dataCollectionRule.destinations.azureMonitorMetrics.name) ? {} : {
      azureMonitorMetrics: {
        name: dataCollectionRule.destinations.azureMonitorMetrics.name
      }
    },{
      logAnalytics: [for (logAnalyticsWorkspace, i) in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[21:24) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
        name: logAnalyticsWorkspace.destinationName
        workspaceResourceId: logAnalyticsWorkspaces[i].id
      }]
    })
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}

resource dataCollectionRuleRes2 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
  name: dataCollectionRule.name
  location: dataCollectionRule.location
  tags: tags
  kind: dataCollectionRule.kind
  properties: {
    description: dataCollectionRule.description
    destinations: empty([]) ? [for x in []: {}] : [for x in []: {}]
//@[18:67) [BCP036 (Warning)] The property "destinations" expected a value of type "DataCollectionRuleDestinations | null" but the provided value is of type "object[] | object[]". (CodeDescription: none) |empty([]) ? [for x in []: {}] : [for x in []: {}]|
//@[31:34) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
//@[51:54) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}

@description('The language of the Deployment Script. AzurePowerShell or AzureCLI.')
@allowed([
  'AzureCLI'
  'AzurePowerShell'
])
param issue4668_kind string = 'AzureCLI'
@description('The identity that will be used to execute the Deployment Script.')
param issue4668_identity object
@description('The properties of the Deployment Script.')
param issue4668_properties object
resource issue4668_mainResource 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'testscript'
  location: 'westeurope'
//@[12:24) [no-hardcoded-location (Warning)] A resource location should be either an expression or the string 'global'. Found 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  kind: issue4668_kind
//@[8:22) [BCP225 (Warning)] The discriminator property "kind" value cannot be determined at compilation time. Type checking for this object is disabled. (CodeDescription: none) |issue4668_kind|
  identity: issue4668_identity
  properties: issue4668_properties
}

