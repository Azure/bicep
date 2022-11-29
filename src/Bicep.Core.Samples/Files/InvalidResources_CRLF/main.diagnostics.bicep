
// wrong declaration
bad
//@[000:003) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |bad|

// incomplete #completionTest(9) -> empty
resource 
//@[009:009) [BCP017 (Error)] Expected a resource identifier at this location. (CodeDescription: none) ||
//@[009:009) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
resource foo
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[012:012) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
//@[012:012) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
resource fo/o
//@[011:012) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |/|
//@[011:013) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |/o|
//@[013:013) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
resource foo 'ddd'
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:018) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
//@[018:018) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(23) -> resourceTypes
resource trailingSpace  
//@[024:024) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
//@[024:024) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||

// #completionTest(19,20) -> resourceObject
resource foo 'ddd'= 
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:018) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
//@[020:020) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

// wrong resource type
resource foo 'ddd'={
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:018) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
}

resource foo 'ddd'=if (1 + 1 == 2) {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:018) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'ddd'|
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:058) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. (CodeDescription: none) |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:058) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. (CodeDescription: none) |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[009:012) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[009:012) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[056:060) [BCP057 (Error)] The name "name" does not exist in the current context. (CodeDescription: none) |name|
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[009:012) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[058:061) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'a'|
//@[063:064) [BCP057 (Error)] The name "b" does not exist in the current context. (CodeDescription: none) |b|
}

// simulate typing if condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[054:054) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[056:056) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[060:060) [BCP018 (Error)] Expected the ")" character at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[061:061) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) ||

// missing condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[055:056) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |{|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// empty condition
// #completionTest(56) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[056:056) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// #completionTest(57, 59) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[056:056) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// invalid condition type
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[055:060) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |(123)|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// runtime functions are no allowed in resource conditions
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[057:066) [BCP066 (Error)] Function "reference" is not valid at this location. It can only be used in resource declarations. (CodeDescription: none) |reference|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[057:065) [BCP066 (Error)] Function "listKeys" is not valid at this location. It can only be used in resource declarations. (CodeDescription: none) |listKeys|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[002:006) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |name|
  name: true
//@[002:006) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |name|
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[002:006) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |name|
  'name': true
//@[002:008) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'name'|
//@[002:008) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'name'|
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
  properties: {
    foo: 'a'
//@[004:007) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |foo|
    foo: 'a'
//@[004:007) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |foo|
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
  properties: {
    foo: 'a'
//@[004:007) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |foo|
    'foo': 'a'
//@[004:009) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'foo'|
//@[004:009) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. (CodeDescription: none) |'foo'|
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'foo'
//@[008:013) [BCP121 (Error)] Resources: "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo", "foo" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'foo'|
  location: [
//@[012:018) [BCP036 (Warning)] The property "location" expected a value of type "string" but the provided value is of type "[]". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |[\r\n  ]|
  ]
  tags: 'tag are not a string?'
//@[008:031) [BCP036 (Warning)] The property "tags" expected a value of type "Tags" but the provided value is of type "'tag are not a string?'". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |'tag are not a string?'|
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: true ? 's' : 'a' + 1
//@[021:028) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "1". (CodeDescription: none) |'a' + 1|
  properties: {
    x: foo()
//@[007:010) [BCP059 (Error)] The name "foo" is not a function. (CodeDescription: none) |foo|
    y: true && (null || !4)
//@[024:026) [BCP044 (Error)] Cannot apply operator "!" to operand of type "4". (CodeDescription: none) |!4|
    a: [
      a
//@[006:007) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|
      !null
//@[006:011) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". (CodeDescription: none) |!null|
      true && true || true + -true * 4
//@[029:034) [BCP044 (Error)] Cannot apply operator "-" to operand of type "true". (CodeDescription: none) |-true|
    ]
  }
}

// there should be no completions without the colon
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8) -> empty
  kind  
//@[008:008) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) ||
}

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8) -> empty
  kind  :
//@[009:009) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}

// unsupported resource ref
var resrefvar = bar.name
//@[004:013) [no-unused-vars (Warning)] Variable "resrefvar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resrefvar|

param resrefpar string = foo.id
//@[006:015) [no-unused-params (Warning)] Parameter "resrefpar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |resrefpar|
//@[025:028) [BCP062 (Error)] The referenced declaration with name "foo" is not valid. (CodeDescription: none) |foo|
//@[025:028) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. (CodeDescription: none) |foo|

output resrefout bool = bar.id
//@[024:030) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "string". (CodeDescription: none) |bar.id|

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[013:050) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  id: 2
//@[002:004) [BCP073 (Error)] The property "id" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |id|
  type: 'hello'
//@[002:006) [BCP073 (Error)] The property "type" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |type|
  apiVersion: true
//@[002:012) [BCP073 (Error)] The property "apiVersion" is read-only. Expressions cannot be assigned to read-only properties. (CodeDescription: none) |apiVersion|
}

resource readOnlyPropertyAssignment 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'vnet-bicep'
  location: 'westeurope'
//@[012:024) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  etag: 'assigning-to-read-only-value'
//@[002:006) [BCP073 (Warning)] The property "etag" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |etag|
  properties: {
    resourceGuid: 'assigning-to-read-only-value'
//@[004:016) [BCP073 (Warning)] The property "resourceGuid" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |resourceGuid|
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: []
  }
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[020:057) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: [
    baz.id
//@[004:010) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "string". (CodeDescription: none) |baz.id|
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[021:058) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: [
    'hello'
//@[004:011) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "'hello'". (CodeDescription: none) |'hello'|
    true
//@[004:008) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "true". (CodeDescription: none) |true|
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[021:058) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[021:058) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[021:058) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  dependsOn: badDepends3.dependsOn
//@[025:034) [BCP077 (Error)] The property "dependsOn" on type "Microsoft.Foo/foos@2020-02-02-alpha" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |dependsOn|
}

var interpVal = 'abc'
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[019:056) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "baz", "badDepends", "badDepends2", "badDepends3", "badDepends4", "badDepends5", "badInterp" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
  '${undefinedSymbol}': true
//@[005:020) [BCP057 (Error)] The name "undefinedSymbol" does not exist in the current context. (CodeDescription: none) |undefinedSymbol|
}

module validModule './module.bicep' = {
  name: 'storageDeploy'
  params: {
//@[002:037) [explicit-values-for-loc-params (Warning)] Parameter 'location' of module 'validModule' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    name: 'contoso'\r\n  }|
    name: 'contoso'
  }
}

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'name1'
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    evictionPolicy: 'Deallocate'
  }
}

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[008:089) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)|
  kind:'AzureCLI'
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
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
//@[008:041) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(validModule['name'], 'v1')|
//@[026:034) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['name']|
}

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${validModule.name}_v1'
}

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.location
//@[008:033) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
}

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['location']
//@[008:036) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['location']|
//@[024:036) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['location']|
}

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: runtimeValidRes1.properties.evictionPolicy
//@[008:035) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "AzureCLI" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.properties|
  kind:'AzureCLI'
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties'].evictionPolicy
//@[008:038) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['properties']|
//@[024:038) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
}

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[008:038) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1['properties']|
//@[024:038) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[038:056) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['evictionPolicy']|
}

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.properties['evictionPolicy']
//@[008:035) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.properties|
//@[035:053) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['evictionPolicy']|
}

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2.properties.azCliVersion
//@[008:035) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2.properties|
}

var magicString1 = 'location'
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2['${magicString1}']
//@[008:043) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['${magicString1}']|
}

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
var magicString2 = 'name'
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2['${magicString2}']
//@[008:043) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['${magicString2}']|
}

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes3.location}'
//@[008:038) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-interpolation)) |'${runtimeValidRes3.location}'|
//@[011:036) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes3 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes3.location|
//@[028:036) [BCP187 (Warning)] The property "location" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |location|
}

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: validModule.params['name']
//@[008:026) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of validModule which can be calculated at the start include "name". (CodeDescription: none) |validModule.params|
//@[020:026) [BCP077 (Error)] The property "params" on type "module" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |params|
//@[026:034) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['name']|
}

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
//@[008:143) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)|
//@[015:040) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
//@[042:070) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['location']|
//@[058:070) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['location']|
//@[072:104) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeInvalidRes3 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalidRes3['properties']|
//@[090:104) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[119:137) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of validModule which can be calculated at the start include "name". (CodeDescription: none) |validModule.params|
//@[131:137) [BCP077 (Error)] The property "params" on type "module" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |params|
}

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
//@[011:036) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
//@[039:067) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['location']|
//@[055:067) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['location']|
//@[070:099) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeInvalidRes3 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalidRes3.properties|
//@[099:115) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['azCliVersion']|
//@[118:139) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of validModule which can be calculated at the start include "name". (CodeDescription: none) |validModule['params']|
//@[129:139) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['params']|
//@[130:138) [BCP077 (Error)] The property "params" on type "module" is write-only. Write-only properties cannot be accessed. (CodeDescription: none) |'params'|
}

// variable related runtime validation
var runtimefoo1 = runtimeValidRes1['location']
//@[034:046) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['location']|
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[034:048) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
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
//@[008:022) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo2
//@[008:022) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo3.properties.azCliVersion
//@[008:022) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo4
//@[008:022) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
}

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
//@[008:129) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)|
//@[015:029) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeInvalid" -> "runtimefoo1" -> "runtimeValidRes1"). Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeInvalid|
//@[036:066) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes2 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes2['properties']|
//@[052:066) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[084:109) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Advisor/recommendations/suppressions" type, which requires a value that can be calculated at the start of the deployment. Properties of runtimeValidRes1 which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeValidRes1.location|
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
//@[009:028) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck|
  name: 'test'
  location: 'test'
//@[012:018) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
var runtimeCheckVar2 = runtimeCheckVar

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: runtimeCheckVar2
//@[008:024) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeCheckVar2" -> "runtimeCheckVar" -> "loopForRuntimeCheck"). Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeCheckVar2|
  location: 'test'
//@[012:018) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[009:029) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck2|
  name: runtimeCheckVar2
//@[008:024) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("runtimeCheckVar2" -> "runtimeCheckVar" -> "loopForRuntimeCheck"). Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |runtimeCheckVar2|
  location: 'test'
//@[012:018) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[009:029) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "otherThing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck3|
  name: loopForRuntimeCheck[0].properties.zoneType
//@[008:041) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |loopForRuntimeCheck[0].properties|
  location: 'test'
//@[012:018) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
var varForRuntimeCheck4b = varForRuntimeCheck4a
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[009:029) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "otherThing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |loopForRuntimeCheck4|
  name: varForRuntimeCheck4b
//@[008:028) [BCP120 (Error)] This expression is being used in an assignment to the "name" property of the "Microsoft.Network/dnsZones" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("varForRuntimeCheck4b" -> "varForRuntimeCheck4a" -> "loopForRuntimeCheck"). Properties of loopForRuntimeCheck which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |varForRuntimeCheck4b|
  location: 'test'
//@[012:018) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
}]

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[009:034) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |missingTopLevelProperties|
  // #completionTest(0, 1, 2) -> topLevelProperties
  
}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[009:044) [BCP035 (Warning)] The specified "resource" declaration is missing the following required properties: "kind", "location", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |missingTopLevelPropertiesExceptName|
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'v'
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
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
//@[086:148) [BCP078 (Error)] The property "kind" requires a value of type "'AzureCLI' | 'AzurePowerShell'", but none was supplied. (CodeDescription: none) |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (conditional)
*/
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[098:160) [BCP078 (Error)] The property "kind" requires a value of type "'AzureCLI' | 'AzurePowerShell'", but none was supplied. (CodeDescription: none) |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
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
//@[010:010) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[004:043) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[004:044) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2|
//@[076:076) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[004:044) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3|
//@[076:076) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator key value missing with property access (conditional)
*/
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
  kind:   
//@[010:010) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[004:046) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions_if|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[004:047) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2_if|
//@[082:082) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[004:047) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3_if|
//@[082:082) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator key value missing with property access (loops)
*/
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
  kind:   
//@[010:010) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[004:035) [no-unused-vars (Warning)] Variable "resourceListIsNotSingleResource" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resourceListIsNotSingleResource|
//@[038:070) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |discriminatorKeyValueMissing_for|
//@[071:075) [BCP055 (Error)] Cannot access properties of type "Microsoft.Resources/deploymentScripts@2020-10-01[]". An "object" type is required. (CodeDescription: none) |kind|

// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[004:047) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions_for|
// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[004:048) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2_for|
//@[087:087) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[004:048) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3_for|
//@[087:087) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator key value missing with property access (filtered loops)
*/
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
  kind:   
//@[010:010) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[004:038) [no-unused-vars (Warning)] Variable "resourceListIsNotSingleResource_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |resourceListIsNotSingleResource_if|
//@[041:076) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |discriminatorKeyValueMissing_for_if|
//@[077:081) [BCP055 (Error)] Cannot access properties of type "Microsoft.Resources/deploymentScripts@2020-10-01[]". An "object" type is required. (CodeDescription: none) |kind|

// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[004:050) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions_for_if|
// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[004:051) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions2_for_if|
//@[093:093) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[004:051) [no-unused-vars (Warning)] Variable "discriminatorKeyValueMissingCompletions3_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeyValueMissingCompletions3_for_if|
//@[093:093) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1
*/
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetOne|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[004:037) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions|
//@[074:075) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[004:038) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2|
//@[075:075) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[004:038) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3|
//@[075:075) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1 (conditional)
*/
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[009:034) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetOne_if|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[004:040) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions_if|
//@[080:081) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[004:041) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2_if|
//@[081:081) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[004:041) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3_if|
//@[081:081) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1 (loop)
*/
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[009:035) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetOne_for|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(86) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[004:041) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions_for|
//@[085:086) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(94) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[004:042) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2_for|
//@[094:094) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[004:042) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3_for|
//@[086:086) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||

/*
Discriminator value set 1 (filtered loop)
*/
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[009:038) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetOne_for_if|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azCliVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(92) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[004:044) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions_for_if|
//@[091:092) [BCP053 (Warning)] The type "AzureCliScriptProperties" does not contain property "a". Available properties include "arguments", "azCliVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(100) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[004:045) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions2_for_if|
//@[100:100) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[004:045) [no-unused-vars (Warning)] Variable "discriminatorKeySetOneCompletions3_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetOneCompletions3_for_if|
//@[092:092) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. (CodeDescription: none) ||


/*
Discriminator value set 2
*/
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetTwo|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[004:037) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions|
//@[074:075) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[004:038) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2|
//@[075:075) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[004:049) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer|
//@[074:088) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[089:090) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[004:050) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2|
//@[075:089) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[090:090) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

/*
Discriminator value set 2 (conditional)
*/
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:034) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetTwo_if|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[004:040) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions_if|
//@[080:081) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[004:041) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2_if|
//@[081:081) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[004:052) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer_if|
//@[080:094) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[095:096) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[004:053) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2_if|
//@[081:095) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[096:096) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||


/*
Discriminator value set 2 (loops)
*/
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[009:035) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetTwo_for|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[004:041) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions_for|
//@[085:086) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[004:042) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2_for|
//@[086:086) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[004:053) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer_for|
//@[085:099) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[100:101) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[004:054) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2_for|
//@[086:100) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[101:101) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||


/*
Discriminator value set 2 (filtered loops)
*/
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[009:038) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |discriminatorKeySetTwo_for_if|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "azPowerShellVersion", "retentionInterval". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[004:044) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions_for_if|
//@[091:092) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[004:045) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletions2_for_if|
//@[092:092) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[004:056) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer_for_if|
//@[091:105) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[106:107) [BCP053 (Warning)] The type "AzurePowerShellScriptProperties" does not contain property "a". Available properties include "arguments", "azPowerShellVersion", "cleanupPreference", "containerSettings", "environmentVariables", "forceUpdateTag", "outputs", "primaryScriptUri", "provisioningState", "retentionInterval", "scriptContent", "status", "storageAccountSettings", "supportingScriptUris", "timeout". (CodeDescription: none) |a|
// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[004:057) [no-unused-vars (Warning)] Variable "discriminatorKeySetTwoCompletionsArrayIndexer2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |discriminatorKeySetTwoCompletionsArrayIndexer2_for_if|
//@[092:106) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[107:107) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |incorrectPropertiesKey|
  kind: 'AzureCLI'

  propertes: {
//@[002:011) [BCP089 (Error)] The property "propertes" is not allowed on objects of type "AzureCLI". Did you mean "properties"? (CodeDescription: none) |propertes|
  }
}

var mock = incorrectPropertiesKey.p
//@[004:008) [no-unused-vars (Warning)] Variable "mock" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |mock|
//@[034:035) [BCP053 (Error)] The type "AzureCLI" does not contain property "p". Available properties include "apiVersion", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "systemData", "tags", "type", "zones". (CodeDescription: none) |p|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'
  name: 'test'
  location: ''
//@[012:014) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: '' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |''|
  properties: {
    azCliVersion: '2'
    retentionInterval: 'PT1H'
    
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
    
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
    cleanupPreference: 
//@[023:023) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

    // #completionTest(25,26) -> arrayPlusSymbols
    supportingScriptUris: 
//@[026:026) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

    // #completionTest(27,28) -> objectPlusSymbols
    storageAccountSettings: 
//@[028:028) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

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
//@[021:021) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||
//@[021:021) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) ||

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
resource startedTypingTypeWithQuotes 'virma'
//@[037:044) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'virma'|
//@[044:044) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(40,41,42,43,44,45) -> resourceTypes
resource startedTypingTypeWithoutQuotes virma
//@[040:045) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |virma|
//@[040:045) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |virma|
//@[045:045) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[009:030) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "location", "name". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |dashesInPropertyNames|
}
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[004:023) [no-unused-vars (Warning)] Variable "letsAccessTheDashes" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |letsAccessTheDashes|
//@[077:078) [BCP053 (Warning)] The type "ManagedClusterPropertiesAutoScalerProfile" does not contain property "s". Available properties include "balance-similar-node-groups", "expander", "max-empty-bulk-delete", "max-graceful-termination-sec", "max-total-unready-percentage", "new-pod-scale-up-delay", "ok-total-unready-count", "scale-down-delay-after-add", "scale-down-delay-after-delete", "scale-down-delay-after-failure", "scale-down-unneeded-time", "scale-down-unready-time", "scale-down-utilization-threshold", "scan-interval", "skip-nodes-with-local-storage", "skip-nodes-with-system-pods". (CodeDescription: none) |s|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[004:024) [no-unused-vars (Warning)] Variable "letsAccessTheDashes2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |letsAccessTheDashes2|
//@[078:078) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

/* 
Nested discriminator missing key
*/
resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[014:051) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}
// #completionTest(90) -> createMode
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[004:044) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions|
// #completionTest(92) -> createMode
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[004:045) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2|
//@[077:091) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[092:092) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[004:049) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions|

/* 
Nested discriminator missing key (conditional)
*/
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[014:051) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}
// #completionTest(96) -> createMode
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[004:047) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions_if|
// #completionTest(98) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[004:048) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2_if|
//@[083:097) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[098:098) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(100) -> createModeIndexPlusSymbols_if
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[004:052) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions_if|

/* 
Nested discriminator missing key (loop)
*/
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[009:042) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminatorMissingKey_for|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[014:051) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}]
// #completionTest(101) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[004:048) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions_for|
// #completionTest(103) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[004:049) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2_for|
//@[088:102) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[103:103) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(105) -> createModeIndexPlusSymbols_for
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[004:053) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions_for|


/* 
Nested discriminator missing key (filtered loop)
*/
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[009:045) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminatorMissingKey_for_if|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[014:051) [BCP078 (Warning)] The property "createMode" requires a value of type "'Default' | 'Restore'", but none was supplied. (CodeDescription: none) |{\r\n    //createMode: 'Default'\r\n\r\n  }|
    //createMode: 'Default'

  }
}]
// #completionTest(107) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[004:051) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions_for_if|
// #completionTest(109) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[004:052) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyCompletions2_for_if|
//@[094:108) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[109:109) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[004:056) [no-unused-vars (Warning)] Variable "nestedDiscriminatorMissingKeyIndexCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorMissingKeyIndexCompletions_for_if|


/*
Nested discriminator
*/
resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    createMode: 'Default'

  }
}
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[004:034) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions|
//@[068:069) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(73) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[004:035) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2|
//@[057:071) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[072:073) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[004:035) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3|
//@[069:069) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(72) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[004:035) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4|
//@[057:071) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[072:072) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(79) -> defaultCreateModeIndexes
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[004:044) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions|
//@[078:079) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|

/*
Nested discriminator (conditional)
*/
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    createMode: 'Default'

  }
}
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[004:037) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions_if|
//@[074:075) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(79) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[004:038) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2_if|
//@[063:077) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[078:079) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[004:038) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3_if|
//@[075:075) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(78) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[004:038) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4_if|
//@[063:077) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[078:078) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(85) -> defaultCreateModeIndexes_if
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[004:047) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions_if|
//@[084:085) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|


/*
Nested discriminator (loop)
*/
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[009:032) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminator_for|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    createMode: 'Default'

  }
}]
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[004:038) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions_for|
//@[079:080) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(84) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[004:039) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2_for|
//@[068:082) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[083:084) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[004:039) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3_for|
//@[080:080) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(83) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[004:039) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4_for|
//@[068:082) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[083:083) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(90) -> defaultCreateModeIndexes_for
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[004:048) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions_for" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions_for|
//@[089:090) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|


/*
Nested discriminator (filtered loop)
*/
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[009:035) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nestedDiscriminator_for_if|
  name: 'test'
//@[008:014) [BCP121 (Error)] Resources: "nestedDiscriminatorMissingKey", "nestedDiscriminatorMissingKey_if", "nestedDiscriminatorMissingKey_for", "nestedDiscriminatorMissingKey_for_if", "nestedDiscriminator", "nestedDiscriminator_if", "nestedDiscriminator_for", "nestedDiscriminator_for_if" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'test'|
  location: 'l'
//@[012:015) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'l' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'l'|
  properties: {
//@[002:012) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "databaseAccountOfferType", "locations". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |properties|
    createMode: 'Default'

  }
}]
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[004:041) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions_for_if|
//@[085:086) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(90) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[004:042) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions2_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions2_for_if|
//@[074:088) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[089:090) [BCP053 (Warning)] The type "Default" does not contain property "a". Available properties include "apiProperties", "backupPolicy", "capabilities", "connectorOffer", "consistencyPolicy", "cors", "createMode", "databaseAccountOfferType", "disableKeyBasedMetadataWriteAccess", "documentEndpoint", "enableAnalyticalStorage", "enableAutomaticFailover", "enableCassandraConnector", "enableFreeTier", "enableMultipleWriteLocations", "failoverPolicies", "instanceId", "ipRules", "isVirtualNetworkFilterEnabled", "keyVaultKeyUri", "locations", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "readLocations", "restoreParameters", "virtualNetworkRules", "writeLocations". (CodeDescription: none) |a|
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[004:042) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions3_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions3_for_if|
//@[086:086) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||
// #completionTest(89) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[004:042) [no-unused-vars (Warning)] Variable "nestedDiscriminatorCompletions4_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorCompletions4_for_if|
//@[074:088) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[089:089) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

// #completionTest(96) -> defaultCreateModeIndexes_for_if
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[004:051) [no-unused-vars (Warning)] Variable "nestedDiscriminatorArrayIndexCompletions_for_if" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedDiscriminatorArrayIndexCompletions_for_if|
//@[095:096) [BCP057 (Error)] The name "a" does not exist in the current context. (CodeDescription: none) |a|



// sample resource to validate completions on the next declarations
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
  location: 'test'
//@[012:018) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
  name: 'test'
  properties: {
    additionalCapabilities: {
      
    }
  }
}
// this validates that we can get nested property access completions on a conditional resource
//#completionTest(56) -> vmProperties
var sigh = nestedPropertyAccessOnConditional.properties.
//@[004:008) [no-unused-vars (Warning)] Variable "sigh" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |sigh|
//@[056:056) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: none) ||

/*
  boolean property value completions
*/ 
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  properties: {
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
    autoUpgradeMinorVersion: t
//@[029:030) [BCP057 (Error)] The name "t" does not exist in the current context. (CodeDescription: none) |t|
  }
}

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[019:050) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'selfScope'
  scope: selfScope
//@[009:018) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |selfScope|
}

var notAResource = {
  im: 'not'
  a: 'resource!'
}
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[022:053) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'invalidScope'
  scope: notAResource
//@[009:021) [BCP036 (Error)] The property "scope" expected a value of type "resource | tenant" but the provided value is of type "object". (CodeDescription: none) |notAResource|
}

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[023:054) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'invalidScope2'
  scope: resourceGroup()
}

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[023:054) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/mockResource@2020-12-01'|
  name: 'invalidScope3'
  scope: subscription()
//@[009:023) [BCP139 (Error)] A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope. (CodeDescription: none) |subscription()|
}

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[031:064) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'invalidDuplicateName'
//@[008:030) [BCP121 (Error)] Resources: "invalidDuplicateName1", "invalidDuplicateName2", "invalidDuplicateName3" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidDuplicateName'|
}
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[031:064) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'invalidDuplicateName'
//@[008:030) [BCP121 (Error)] Resources: "invalidDuplicateName1", "invalidDuplicateName2", "invalidDuplicateName3" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidDuplicateName'|
}
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[031:064) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2019-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2019-01-01'|
  name: 'invalidDuplicateName'
//@[008:030) [BCP121 (Error)] Resources: "invalidDuplicateName1", "invalidDuplicateName2", "invalidDuplicateName3" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidDuplicateName'|
}

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[063:096) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
}

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[048:084) [BCP081 (Warning)] Resource type "Mock.Rp/mockExtResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExtResource@2020-01-01'|
  name: 'invalidExtensionResourceDuplicateName'
//@[008:047) [BCP121 (Error)] Resources: "invalidExtensionResourceDuplicateName1", "invalidExtensionResourceDuplicateName2" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidExtensionResourceDuplicateName'|
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[048:084) [BCP081 (Warning)] Resource type "Mock.Rp/mockExtResource@2019-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExtResource@2019-01-01'|
  name: 'invalidExtensionResourceDuplicateName'
//@[008:047) [BCP121 (Error)] Resources: "invalidExtensionResourceDuplicateName1", "invalidExtensionResourceDuplicateName2" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'invalidExtensionResourceDuplicateName'|
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

@concat('foo', 'bar')
//@[001:007) [BCP152 (Error)] Function "concat" cannot be used as a decorator. (CodeDescription: none) |concat|
@secure()
//@[001:007) [BCP127 (Error)] Function "secure" cannot be used as a resource decorator. (CodeDescription: none) |secure|
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[026:063) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2020-02-02-alpha" does not have types available. (CodeDescription: none) |'Microsoft.Foo/foos@2020-02-02-alpha'|
  name: 'invalidDecorator'
}

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[019:060) [BCP081 (Warning)] Resource type "Mock.Rp/mockExistingResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExistingResource@2020-01-01'|
  name: 'cyclicRes'
  scope: cyclicRes
//@[009:018) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |cyclicRes|
}

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[027:068) [BCP081 (Warning)] Resource type "Mock.Rp/mockExistingResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockExistingResource@2020-01-01'|
  name: 'cyclicExistingRes'
  scope: cyclicExistingRes
//@[009:026) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |cyclicExistingRes|
}

// loop parsing cases
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[078:079) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |]|

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[079:080) [BCP012 (Error)] Expected the "for" keyword at this location. (CodeDescription: none) |f|

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[078:078) [BCP162 (Error)] Expected a loop item variable identifier or "(" at this location. (CodeDescription: none) ||

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[082:083) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[084:085) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |b|
//@[085:086) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[091:092) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[082:083) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[083:084) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[085:086) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[087:088) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// loop index parsing cases
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[077:079) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 0. (CodeDescription: none) |()|

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[079:079) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[070:073) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 1. (CodeDescription: none) |(x)|

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[078:083) [BCP249 (Error)] Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found 1. (CodeDescription: none) |(x, )|

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[081:082) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |]|

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[082:083) [BCP012 (Error)] Expected the "in" keyword at this location. (CodeDescription: none) |z|
//@[083:084) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[091:092) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |]|

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[081:082) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[082:083) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |]|

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[084:085) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[086:087) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |]|

// loop filter parsing cases
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[096:097) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[101:102) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[095:096) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[100:101) [BCP018 (Error)] Expected the "(" character at this location. (CodeDescription: none) |]|

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[103:104) [BCP057 (Error)] The name "y" does not exist in the current context. (CodeDescription: none) |y|
//@[109:109) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||
//@[110:111) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[102:103) [BCP057 (Error)] The name "z" does not exist in the current context. (CodeDescription: none) |z|
//@[108:108) [BCP243 (Error)] Parentheses must contain exactly one expression. (CodeDescription: none) ||
//@[109:110) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |]|

// wrong body type
var emptyArray = []
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[097:098) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[103:104) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |4|

// duplicate variable in the same scope
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[009:029) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |itemAndIndexSameName|
//@[080:084) [BCP028 (Error)] Identifier "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|
//@[086:090) [BCP028 (Error)] Identifier "same" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |same|
}]

// errors in the array expression
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[106:107) [BCP070 (Error)] Argument of type "2" is not assignable to parameter of type "array". (CodeDescription: none) |2|
}]
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[111:112) [BCP070 (Error)] Argument of type "2" is not assignable to parameter of type "array". (CodeDescription: none) |2|
}]

// wrong array type
var notAnArray = true
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[089:099) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "true". (CodeDescription: none) |notAnArray|
}]
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[094:104) [BCP137 (Error)] Loop expected an expression of type "array" but the provided value is of type "true". (CodeDescription: none) |notAnArray|
}]

// wrong filter expression type
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[009:034) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |wrongFilterExpressionType|
//@[114:117) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |(4)|
}]
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[009:035) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |wrongFilterExpressionType2|
//@[119:132) [BCP046 (Error)] Expected a value of type "bool". (CodeDescription: none) |(concat('s'))|
}]

// missing required properties
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[009:034) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |missingRequiredProperties|
}]
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[009:035) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "kind", "location", "name", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |missingRequiredProperties2|
}]

// fewer missing required properties and a wrong property
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[009:039) [BCP035 (Warning)] The specified "resource" declaration is missing the following required properties: "kind", "sku". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |missingFewerRequiredProperties|
  name: account
  location: 'eastus42'
//@[012:022) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus42' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus42'|
  properties: {
    wrong: 'test'
//@[004:009) [BCP037 (Warning)] The property "wrong" is not allowed on objects of type "StorageAccountPropertiesCreateParametersOrStorageAccountProperties". Permissible properties include "accessTier", "allowBlobPublicAccess", "allowSharedKeyAccess", "azureFilesIdentityBasedAuthentication", "customDomain", "encryption", "isHnsEnabled", "largeFileSharesState", "minimumTlsVersion", "networkAcls", "routingPreference", "supportsHttpsTrafficOnly". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |wrong|
  }
}]

// wrong property inside the nested property loop
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      doesNotExist: 'test'
//@[006:018) [BCP037 (Warning)] The property "doesNotExist" is not allowed on objects of type "Subnet". Permissible properties include "id", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |doesNotExist|
      name: 'subnet-${i}-${j}'
    }]
  }
}]
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      doesNotExist: 'test'
//@[006:018) [BCP037 (Warning)] The property "doesNotExist" is not allowed on objects of type "Subnet". Permissible properties include "id", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |doesNotExist|
      name: 'subnet-${i}-${j}-${k}'
    }]
  }
}]

// nonexistent arrays and loop variables
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[009:026) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "i" must be referenced in at least one of the value expressions of the following properties: "name" (CodeDescription: none) |nonexistentArrays|
//@[086:095) [BCP057 (Error)] The name "notAThing" does not exist in the current context. (CodeDescription: none) |notAThing|
  name: 'vnet-${justPlainWrong}'
//@[016:030) [BCP057 (Error)] The name "justPlainWrong" does not exist in the current context. (CodeDescription: none) |justPlainWrong|
  properties: {
    subnets: [for j in alsoNotAThing: {
//@[023:036) [BCP057 (Error)] The name "alsoNotAThing" does not exist in the current context. (CodeDescription: none) |alsoNotAThing|
      doesNotExist: 'test'
      name: 'subnet-${fake}-${totallyFake}'
//@[022:026) [BCP057 (Error)] The name "fake" does not exist in the current context. (CodeDescription: none) |fake|
//@[030:041) [BCP057 (Error)] The name "totallyFake" does not exist in the current context. (CodeDescription: none) |totallyFake|
    }]
  }
}]

// property loops cannot be nested
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[098:113) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
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
//@[008:010) [use-resource-id-functions (Warning)] If property "id" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-id-functions)) |id|
        state: [for lol in []: 4]
//@[016:019) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
      }]
    }
  }
}]
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[009:033) [BCP028 (Error)] Identifier "propertyLoopsCannotNest2" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |propertyLoopsCannotNest2|
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
//@[008:010) [use-resource-id-functions (Warning)] If property "id" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-id-functions)) |id|
        state: [for (lol,k) in []: 4]
//@[016:019) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
      }]
    }
  }
}]

// property loops cannot be nested (even more nesting)
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[009:033) [BCP028 (Error)] Identifier "propertyLoopsCannotNest2" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |propertyLoopsCannotNest2|
//@[099:114) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
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
//@[008:010) [use-resource-id-functions (Warning)] If property "id" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-id-functions)) |id|
        state: [for state in []: {
//@[016:019) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
          fake: [for something in []: true]
//@[017:020) [BCP142 (Error)] Property value for-expressions cannot be nested. (CodeDescription: none) |for|
        }]
      }]
    }
  }
}]

// loops cannot be used inside of expressions
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[081:096) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      virtualNetworkRules: concat([for lol in []: {
//@[035:038) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
        id: '${account.name}-${account.location}'
//@[008:010) [use-resource-id-functions (Warning)] If property "id" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-id-functions)) |id|
      }])
    }
  }
}]

// using the same loop variable in a new language scope should be allowed
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[090:105) [BCP057 (Error)] The name "storageAccounts" does not exist in the current context. (CodeDescription: none) |storageAccounts|
  // #completionTest(7) -> symbolsPlusAccount1
  name: account.name
  // #completionTest(12) -> symbolsPlusAccount2
  location: account.location
  sku: {
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
    name: 
//@[010:010) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
  }
  kind: 'StorageV2'
}]

var directRefViaVar = premiumStorages
//@[004:019) [no-unused-vars (Warning)] Variable "directRefViaVar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |directRefViaVar|
//@[022:037) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[040:055) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
//@[057:063) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |stuffs|

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone2'
  location: 'global'
  properties: {
    registrationVirtualNetworks: premiumStorages
//@[033:048) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
  }
}

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
  name: 'myZone3'
  location: 'global'
  properties: {
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
//@[040:055) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
//@[057:063) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |stuffs|
  }
}

@batchSize()
//@[010:012) [BCP071 (Error)] Expected 1 argument, but got 0. (CodeDescription: none) |()|
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
//@[013:028) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
  }
}]

@batchSize(0)
//@[011:012) [BCP154 (Error)] Expected a batch size of at least 1 but the specified value was "0". (CodeDescription: none) |0|
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
//@[013:028) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
    dependsOn: [
//@[004:013) [BCP037 (Warning)] The property "dependsOn" is not allowed on objects of type "VirtualNetworkPropertiesFormat". Permissible properties include "addressSpace", "bgpCommunities", "ddosProtectionPlan", "dhcpOptions", "enableDdosProtection", "enableVmProtection", "ipAllocations", "virtualNetworkPeerings". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |dependsOn|
      premiumStorages
//@[006:021) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |premiumStorages|
    ]
  }
  dependsOn: [
    
  ]
}]

var expressionInPropertyLoopVar = true
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'hello'
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
  properties: {
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[004:061) [BCP143 (Error)] For-expressions cannot be used with properties whose names are also expressions. (CodeDescription: none) |'resolutionVirtualNetworks${expressionInPropertyLoopVar}'|
//@[004:061) [BCP040 (Warning)] String interpolation is not supported for keys on objects of type "ZoneProperties". Permissible properties include "registrationVirtualNetworks", "resolutionVirtualNetworks", "zoneType". (CodeDescription: none) |'resolutionVirtualNetworks${expressionInPropertyLoopVar}'|
  }
}

// resource loop body that isn't an object
@batchSize(-1)
//@[011:013) [BCP154 (Error)] Expected a batch size of at least 1 but the specified value was "-1". (CodeDescription: none) |-1|
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[095:101) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'test'|
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[096:107) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |environment|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[009:035) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody3" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody3|
//@[100:106) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |'test'|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[009:035) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody4" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody4|
//@[100:111) [BCP167 (Error)] Expected the "{" character or the "if" keyword at this location. (CodeDescription: none) |environment|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[009:035) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody3" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody3|
//@[109:115) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |'test'|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[009:035) [BCP028 (Error)] Identifier "nonObjectResourceLoopBody4" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |nonObjectResourceLoopBody4|
//@[109:120) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) |environment|

// #completionTest(54,55) -> objectPlusFor
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
//@[055:055) [BCP118 (Error)] Expected the "{" character, the "[" character, or the "if" keyword at this location. (CodeDescription: none) ||

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[009:012) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. (CodeDescription: none) |foo|
  properties: {
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
    registrationVirtualNetworks: 
//@[033:033) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
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
//@[029:029) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
          }
        }
    }]
  }
}

// parent property with 'existing' resource at different scope
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[017:053) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  scope: subscription()
  name: 'res1'
}

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[019:062) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[065:106) [BCP165 (Error)] A resource's computed scope must match that of the Bicep file for it to be deployable. This resource's scope is computed from the "scope" property value assigned to ancestor resource "p1_res1". You must use modules to deploy resources to a different scope. (CodeDescription: none) |{\r\n  parent: p1_res1\r\n  name: 'child1'\r\n}|
  parent: p1_res1
  name: 'child1'
}

// parent property with scope on child resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[017:053) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
//@[008:014) [BCP121 (Error)] Resources: "p2_res1", "p5_res1", "p7_res1" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'res1'|
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[017:053) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2@2020-06-01'|
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[022:065) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
  scope: p2_res1
//@[009:016) [BCP164 (Error)] A child resource's scope is computed based on the scope of its ancestor resource. This means that using the "scope" property on a child resource is unsupported. (CodeDescription: none) |p2_res1|
  parent: p2_res2
  name: 'child2'
}

// parent property self-cycle
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  parent: p3_vmExt
//@[010:018) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: none) |p3_vmExt|
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}

// parent property 2-cycle
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  parent: p4_vmExt
//@[010:018) [BCP080 (Error)] The expression is involved in a cycle ("p4_vmExt" -> "p4_vm"). (CodeDescription: none) |p4_vmExt|
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  parent: p4_vm
//@[010:015) [BCP080 (Error)] The expression is involved in a cycle ("p4_vm" -> "p4_vmExt"). (CodeDescription: none) |p4_vm|
  location: 'eastus'
//@[012:020) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'eastus' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'eastus'|
}

// parent property with invalid child
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[017:053) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
//@[008:014) [BCP121 (Error)] Resources: "p2_res1", "p5_res1", "p7_res1" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'res1'|
}

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[017:060) [BCP081 (Warning)] Resource type "Microsoft.Rp2/resource2/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
  parent: p5_res1
//@[010:017) [BCP036 (Error)] The property "parent" expected a value of type "Microsoft.Rp2/resource2" but the provided value is of type "Microsoft.Rp1/resource1@2020-06-01". (CodeDescription: none) |p5_res1|
  name: 'res2'
}

// parent property with invalid parent
resource p6_res1 '${true}' = {
//@[017:026) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. (CodeDescription: none) |'${true}'|
  name: 'res1'
}

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[017:060) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
  parent: p6_res1
//@[010:017) [BCP062 (Error)] The referenced declaration with name "p6_res1" is not valid. (CodeDescription: none) |p6_res1|
  name: 'res2'
}

// parent property with incorrectly-formatted name
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[017:053) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1'
//@[008:014) [BCP121 (Error)] Resources: "p2_res1", "p5_res1", "p7_res1" are defined with this same name in a file. Rename them or split into different modules. (CodeDescription: none) |'res1'|
}

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[017:060) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
  parent: p7_res1
  name: 'res1/res2'
//@[008:019) [BCP170 (Error)] Expected resource name to not contain any "/" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name. (CodeDescription: none) |'res1/res2'|
}

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[017:060) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1/child2@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
  parent: p7_res1
  name: '${p7_res1.name}/res2'
//@[008:030) [BCP170 (Error)] Expected resource name to not contain any "/" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name. (CodeDescription: none) |'${p7_res1.name}/res2'|
}

// top-level resource with too many '/' characters
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[017:053) [BCP081 (Warning)] Resource type "Microsoft.Rp1/resource1@2020-06-01" does not have types available. (CodeDescription: none) |'Microsoft.Rp1/resource1@2020-06-01'|
  name: 'res1/res2'
//@[008:019) [BCP169 (Error)] Expected resource name to contain 0 "/" character(s). The number of name segments must match the number of segments in the resource type. (CodeDescription: none) |'res1/res2'|
}

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
  name: 'existingResProperty'
  location: 'westeurope'
//@[002:010) [BCP173 (Error)] The property "location" cannot be used in an existing resource declaration. (CodeDescription: none) |location|
//@[012:024) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  properties: {}
//@[002:012) [BCP173 (Error)] The property "properties" cannot be used in an existing resource declaration. (CodeDescription: none) |properties|
}

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
    parent: existingResProperty
    name: 'myExt'
    location: existingResProperty.location
//@[014:042) [BCP120 (Error)] This expression is being used in an assignment to the "location" property of the "Microsoft.Compute/virtualMachines/extensions" type, which requires a value that can be calculated at the start of the deployment. Properties of existingResProperty which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingResProperty.location|
}

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'anyTypeInDependsOn'
  location: resourceGroup().location
//@[012:036) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  dependsOn: [
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
//@[004:070) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)|
    's'
//@[004:007) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "'s'". (CodeDescription: none) |'s'|
    any(true)
//@[004:013) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(true)|
  ]
}

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[009:024) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInParent|
  parent: any(true)
//@[010:019) [BCP240 (Error)] The "parent" property only permits direct references to resources. Expressions are not supported. (CodeDescription: none) |any(true)|
//@[010:019) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(true)|
}

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[009:028) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInParentLoop|
  parent: any(true)
//@[010:019) [BCP240 (Error)] The "parent" property only permits direct references to resources. Expressions are not supported. (CodeDescription: none) |any(true)|
//@[010:019) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(true)|
}]

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[009:023) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |anyTypeInScope|
  scope: any(invalidExistingLocationRef)
//@[009:040) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(invalidExistingLocationRef)|
}

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[009:034) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name", "properties". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |anyTypeInScopeConditional|
  scope: any(invalidExistingLocationRef)
//@[009:040) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(invalidExistingLocationRef)|
}

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[009:031) [no-unused-existing-resources (Warning)] Existing resource "anyTypeInExistingScope" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-existing-resources)) |anyTypeInExistingScope|
//@[009:031) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInExistingScope|
  parent: any('')
//@[010:017) [BCP240 (Error)] The "parent" property only permits direct references to resources. Expressions are not supported. (CodeDescription: none) |any('')|
//@[010:017) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('')|
  scope: any(false)
//@[009:019) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(false)|
}

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[009:035) [no-unused-existing-resources (Warning)] Existing resource "anyTypeInExistingScopeLoop" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-existing-resources)) |anyTypeInExistingScopeLoop|
//@[009:035) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". (CodeDescription: none) |anyTypeInExistingScopeLoop|
  parent: any('')
//@[010:017) [BCP240 (Error)] The "parent" property only permits direct references to resources. Expressions are not supported. (CodeDescription: none) |any('')|
//@[010:017) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any('')|
  scope: any(false)
//@[009:019) [BCP176 (Error)] Values of the "any" type are not allowed here. (CodeDescription: none) |any(false)|
}]

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[089:131) [BCP135 (Error)] Scope "resourceGroup" is not valid for this resource type. Permitted scopes: "tenant". (CodeDescription: none) |{\r\n  name: 'tenantLevelResourceBlocked'\r\n}|
  name: 'tenantLevelResourceBlocked'
}

// #completionTest(15,36,37) -> resourceTypes
resource comp1 'Microsoft.Resources/'
//@[015:037) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/'|
//@[037:037) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(15,16,17) -> resourceTypes
resource comp2 ''
//@[015:017) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |''|
//@[017:017) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(38) -> resourceTypes
resource comp3 'Microsoft.Resources/t'
//@[015:038) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/t'|
//@[038:038) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(40) -> resourceTypes
resource comp4 'Microsoft.Resources/t/v'
//@[015:040) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/t/v'|
//@[040:040) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(49) -> resourceTypes
resource comp5 'Microsoft.Storage/storageAccounts'
//@[015:050) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts'|
//@[050:050) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(50) -> storageAccountsResourceTypes
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[015:051) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Storage/storageAccounts@'|
//@[051:051) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(52) -> templateSpecsResourceTypes
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[015:053) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |'Microsoft.Resources/templateSpecs@20'|
//@[053:053) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

// #completionTest(60,61) -> virtualNetworksResourceTypes
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[061:061) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||


// issue #3000
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'issue3000LogicApp1'
  location: resourceGroup().location
//@[012:036) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  properties: {
    state: 'Enabled'
    definition: ''
  }
  identity: {
    type: 'SystemAssigned'
  }
  extendedLocation: {}
//@[002:018) [BCP187 (Warning)] The property "extendedLocation" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |extendedLocation|
//@[002:018) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "type". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |extendedLocation|
  sku: {}
//@[002:005) [BCP187 (Warning)] The property "sku" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |sku|
  kind: 'V1'
//@[002:006) [BCP187 (Warning)] The property "kind" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |kind|
  managedBy: 'string'
//@[002:011) [BCP187 (Warning)] The property "managedBy" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedBy|
  mangedByExtended: [
//@[002:018) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "Microsoft.Logic/workflows". Permissible properties include "dependsOn", "tags". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |mangedByExtended|
   'str1'
   'str2'
  ]
  zones: [
//@[002:007) [BCP187 (Warning)] The property "zones" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |zones|
   'str1'
   'str2'
  ]
  plan: {}
//@[002:006) [BCP187 (Warning)] The property "plan" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |plan|
  eTag: ''
//@[002:006) [BCP187 (Warning)] The property "eTag" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |eTag|
  scale: {}  
//@[002:007) [BCP187 (Warning)] The property "scale" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |scale|
//@[002:007) [BCP035 (Warning)] The specified "object" declaration is missing the following required properties: "capacity". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |scale|
}

resource issue3000LogicApp2 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'issue3000LogicApp2'
  location: resourceGroup().location
//@[012:036) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  properties: {
    state: 'Enabled'
    definition: ''
  }
  identity: 'SystemAssigned'
//@[012:028) [BCP036 (Warning)] The property "identity" expected a value of type "ManagedServiceIdentity | null" but the provided value is of type "'SystemAssigned'". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |'SystemAssigned'|
  extendedLocation: 'eastus'
//@[002:018) [BCP187 (Warning)] The property "extendedLocation" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |extendedLocation|
//@[020:028) [BCP036 (Warning)] The property "extendedLocation" expected a value of type "extendedLocation" but the provided value is of type "'eastus'". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |'eastus'|
  sku: 'Basic'
//@[002:005) [BCP187 (Warning)] The property "sku" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |sku|
//@[007:014) [BCP036 (Warning)] The property "sku" expected a value of type "sku" but the provided value is of type "'Basic'". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |'Basic'|
  kind: {
//@[002:006) [BCP187 (Warning)] The property "kind" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |kind|
//@[008:030) [BCP036 (Warning)] The property "kind" expected a value of type "string" but the provided value is of type "object". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |{\r\n    name: 'V1'\r\n  }|
    name: 'V1'
  }
  managedBy: {}
//@[002:011) [BCP187 (Warning)] The property "managedBy" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedBy|
//@[013:015) [BCP036 (Warning)] The property "managedBy" expected a value of type "string" but the provided value is of type "object". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |{}|
  mangedByExtended: [
//@[002:018) [BCP037 (Error)] The property "mangedByExtended" is not allowed on objects of type "Microsoft.Logic/workflows". Permissible properties include "dependsOn", "tags". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |mangedByExtended|
   {}
   {}
  ]
  zones: [
//@[002:007) [BCP187 (Warning)] The property "zones" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |zones|
   {}
//@[003:005) [BCP034 (Error)] The enclosing array expected an item of type "string", but the provided item was of type "object". (CodeDescription: none) |{}|
   {}
//@[003:005) [BCP034 (Error)] The enclosing array expected an item of type "string", but the provided item was of type "object". (CodeDescription: none) |{}|
  ]
  plan: ''
//@[002:006) [BCP187 (Warning)] The property "plan" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |plan|
//@[008:010) [BCP036 (Warning)] The property "plan" expected a value of type "object" but the provided value is of type "''". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |''|
  eTag: {}
//@[002:006) [BCP187 (Warning)] The property "eTag" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |eTag|
//@[008:010) [BCP036 (Warning)] The property "eTag" expected a value of type "string" but the provided value is of type "object". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |{}|
  scale: [
//@[002:007) [BCP187 (Warning)] The property "scale" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |scale|
//@[009:021) [BCP036 (Warning)] The property "scale" expected a value of type "scale" but the provided value is of type "[object]". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |[\r\n  {}\r\n  ]|
  {}
  ]  
}

resource issue3000stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'issue3000stg'
  kind: 'StorageV2'
  location: 'West US'
//@[012:021) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'West US' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'West US'|
  sku: {
    name: 'Premium_LRS'    
  }
  madeUpProperty: {}
//@[002:016) [BCP037 (Error)] The property "madeUpProperty" is not allowed on objects of type "Microsoft.Storage/storageAccounts". Permissible properties include "dependsOn", "extendedLocation", "identity", "properties", "tags". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |madeUpProperty|
  managedByExtended: []
//@[002:019) [BCP187 (Warning)] The property "managedByExtended" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedByExtended|
}

var issue3000stgMadeUpProperty = issue3000stg.madeUpProperty
//@[004:030) [no-unused-vars (Warning)] Variable "issue3000stgMadeUpProperty" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |issue3000stgMadeUpProperty|
//@[046:060) [BCP053 (Error)] The type "Microsoft.Storage/storageAccounts" does not contain property "madeUpProperty". Available properties include "apiVersion", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "tags", "type", "zones". (CodeDescription: none) |madeUpProperty|
var issue3000stgManagedBy = issue3000stg.managedBy
//@[004:025) [no-unused-vars (Warning)] Variable "issue3000stgManagedBy" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |issue3000stgManagedBy|
//@[041:050) [BCP187 (Warning)] The property "managedBy" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedBy|
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[004:033) [no-unused-vars (Warning)] Variable "issue3000stgManagedByExtended" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |issue3000stgManagedByExtended|
//@[049:066) [BCP187 (Warning)] The property "managedByExtended" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |managedByExtended|

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
//@[021:024) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
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
//@[018:067) [BCP036 (Warning)] The property "destinations" expected a value of type "DataCollectionRuleDestinations | null" but the provided value is of type "object[] | object[]". If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |empty([]) ? [for x in []: {}] : [for x in []: {}]|
//@[031:034) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
//@[051:054) [BCP138 (Error)] For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties. (CodeDescription: none) |for|
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
//@[012:024) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westeurope' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'westeurope'|
  kind: issue4668_kind
//@[008:022) [BCP225 (Warning)] The discriminator property "kind" value cannot be determined at compilation time. Type checking for this object is disabled. (CodeDescription: none) |issue4668_kind|
  identity: issue4668_identity
  properties: issue4668_properties
}

// https://github.com/Azure/bicep/issues/8516
resource storage 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  resource blobServices 'blobServices' existing = {
    name: $account
//@[010:011) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) |$|
//@[010:011) [BCP001 (Error)] The following token is not recognized: "$". (CodeDescription: none) |$|
  }
}

