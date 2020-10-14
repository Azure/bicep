
// wrong declaration
bad
//@[0:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete
resource 
//@[9:9) [BCP017 (Error)] Expected a resource identifier at this location. ||
//@[9:9) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". ||
resource foo
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[12:12) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". ||
//@[12:12) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". ||
resource fo/o
//@[11:12) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |/|
//@[11:13) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |/o|
//@[13:13) [BCP018 (Error)] Expected the "=" character at this location. ||
resource foo 'ddd'
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |'ddd'|
//@[18:18) [BCP018 (Error)] Expected the "=" character at this location. ||
resource foo 'ddd'=
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |'ddd'|
//@[19:19) [BCP018 (Error)] Expected the "{" character at this location. ||

// wrong resource type
resource foo 'ddd'={
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |'ddd'|
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:58) [BCP047 (Error)] String interpolation is unsupported for specifying the resource type. |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[51:55) [BCP035 (Error)] The specified object is missing the following required properties: "name". |{\r\n}|
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
//@[2:6) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. |name|
  name: true
//@[2:6) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. |name|
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
//@[2:6) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. |name|
  'name': true
//@[2:8) [BCP025 (Error)] The property "name" is declared multiple times in this object. Remove or rename the duplicate properties. |'name'|
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  properties: {
    foo: 'a'
//@[4:7) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
    foo: 'a'
//@[4:7) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  properties: {
    foo: 'a'
//@[4:7) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
    'foo': 'a'
//@[4:9) [BCP025 (Error)] The property "foo" is declared multiple times in this object. Remove or rename the duplicate properties. |'foo'|
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  location: [
//@[12:18) [BCP036 (Error)] The property "location" expected a value of type "string" but the provided value is of type "array". |[\r\n  ]|
  ]
  tags: 'tag are not a string?'
//@[8:31) [BCP036 (Error)] The property "tags" expected a value of type "Tags" but the provided value is of type "'tag are not a string?'". |'tag are not a string?'|
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: true ? 's' : 'a' + 1
//@[21:28) [BCP045 (Error)] Cannot apply operator "+" to operands of type "'a'" and "int". |'a' + 1|
  properties: {
    x: foo()
//@[7:10) [BCP059 (Error)] The name "foo" is not a function. |foo|
    y: true && (null || !4)
//@[24:26) [BCP044 (Error)] Cannot apply operator "!" to operand of type "int". |!4|
    a: [
      a
//@[6:7) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
      !null
//@[6:11) [BCP044 (Error)] Cannot apply operator "!" to operand of type "null". |!null|
      true && true || true + -true * 4
//@[29:34) [BCP044 (Error)] Cannot apply operator "-" to operand of type "bool". |-true|
    ]
  }
}

// unsupported resource ref
var resrefvar = bar.name

param resrefpar string = foo.id
//@[25:28) [BCP072 (Error)] This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |foo|
//@[25:28) [BCP062 (Error)] The referenced declaration with name "foo" is not valid. |foo|

output resrefout bool = bar.id
//@[24:30) [BCP026 (Error)] The output expects a value of type "bool" but the provided value is of type "string". |bar.id|

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  id: 2
//@[2:4) [BCP073 (Error)] The property "id" is read-only. Expressions cannot be assigned to read-only properties. |id|
//@[6:7) [BCP036 (Error)] The property "id" expected a value of type "string" but the provided value is of type "int". |2|
  type: 'hello'
//@[2:6) [BCP073 (Error)] The property "type" is read-only. Expressions cannot be assigned to read-only properties. |type|
//@[8:15) [BCP036 (Error)] The property "type" expected a value of type "'Microsoft.Foo/foos'" but the provided value is of type "'hello'". |'hello'|
  apiVersion: true
//@[2:12) [BCP073 (Error)] The property "apiVersion" is read-only. Expressions cannot be assigned to read-only properties. |apiVersion|
//@[14:18) [BCP036 (Error)] The property "apiVersion" expected a value of type "'2020-02-02-alpha'" but the provided value is of type "bool". |true|
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    baz.id
//@[4:10) [BCP034 (Error)] The enclosing array expected an item of type "resource", but the provided item was of type "string". |baz.id|
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    'hello'
//@[4:11) [BCP034 (Error)] The enclosing array expected an item of type "resource", but the provided item was of type "'hello'". |'hello'|
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type "resource", but the provided item was of type "bool". |true|
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: badDepends3.dependsOn
//@[25:34) [BCP077 (Error)] The property "dependsOn" on type "Microsoft.Foo/foos@2020-02-02-alpha" is write-only. Write-only properties cannot be accessed. |dependsOn|
}

var interpVal = 'abc'
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
//@[2:16) [BCP040 (Error)] String interpolation is not supported for keys on objects of type "Microsoft.Foo/foos@2020-02-02-alpha". Permissible properties include "dependsOn", "eTag", "extendedLocation", "identity", "kind", "location", "managedBy", "managedByExtended", "plan", "properties", "scale", "sku", "tags", "zones". |'${interpVal}'|
  '${undefinedSymbol}': true
//@[2:22) [BCP040 (Error)] String interpolation is not supported for keys on objects of type "Microsoft.Foo/foos@2020-02-02-alpha". Permissible properties include "dependsOn", "eTag", "extendedLocation", "identity", "kind", "location", "managedBy", "managedByExtended", "plan", "properties", "scale", "sku", "tags", "zones". |'${undefinedSymbol}'|
//@[5:20) [BCP057 (Error)] The name "undefinedSymbol" does not exist in the current context. |undefinedSymbol|
}

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[92:151) [BCP035 (Error)] The specified object is missing the following required properties: "name". |{\r\n  // #completionTest(0, 1, 2) -> topLevelProperties\r\n\r\n}|
  // #completionTest(0, 1, 2) -> topLevelProperties

}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: 'v'
  location: 'eastus'
  properties: {
    subnets: [
      {
        // #completionTest(0,1,2,3,4,5,6,7) -> subnetPropertiesMinusProperties
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

resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[86:148) [BCP035 (Error)] The specified object is missing the following required properties: "name". |{\r\n  // #completionTest(0,1,2) -> discriminatorProperty\r\n  \r\n}|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[85:264) [BCP035 (Error)] The specified object is missing the following required properties: "name". |{\r\n  kind: 'AzureCLI'\r\n  // #completionTest(0,1,2) -> deploymentScriptTopLevel\r\n\r\n  properties: {\r\n    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties\r\n    \r\n  }\r\n}|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}

resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[85:270) [BCP035 (Error)] The specified object is missing the following required properties: "name". |{\r\n  kind: 'AzurePowerShell'\r\n  // #completionTest(0,1,2) -> deploymentScriptTopLevel\r\n\r\n  properties: {\r\n    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties\r\n    \r\n  }\r\n}|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
