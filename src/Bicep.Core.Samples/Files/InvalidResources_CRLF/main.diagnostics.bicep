
// wrong declaration
bad
//@[0:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete #completionTest(9) -> empty
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

// #completionTest(19,20) -> object
resource foo 'ddd'= 
//@[9:12) [BCP028 (Error)] Identifier "foo" is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:18) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |'ddd'|
//@[20:20) [BCP018 (Error)] Expected the "{" character at this location. ||

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
//@[9:12) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |foo|
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
  type: 'hello'
//@[2:6) [BCP073 (Error)] The property "type" is read-only. Expressions cannot be assigned to read-only properties. |type|
  apiVersion: true
//@[2:12) [BCP073 (Error)] The property "apiVersion" is read-only. Expressions cannot be assigned to read-only properties. |apiVersion|
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    baz.id
//@[4:10) [BCP034 (Error)] The enclosing array expected an item of type "resource | module", but the provided item was of type "string". |baz.id|
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    'hello'
//@[4:11) [BCP034 (Error)] The enclosing array expected an item of type "resource | module", but the provided item was of type "'hello'". |'hello'|
    true
//@[4:8) [BCP034 (Error)] The enclosing array expected an item of type "resource | module", but the provided item was of type "bool". |true|
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
  '${undefinedSymbol}': true
//@[5:20) [BCP057 (Error)] The name "undefinedSymbol" does not exist in the current context. |undefinedSymbol|
}

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'name1'
  location: 'eastus'
  properties: {
    evictionPolicy: 'Deallocate'
  }
}

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.location
//@[8:33) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1.location|
}

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['location']
//@[8:36) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1['location']|
}

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: runtimeValidRes1.properties.evictionPolicy
//@[8:50) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1.properties.evictionPolicy|
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties'].evictionPolicy
//@[8:53) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1['properties'].evictionPolicy|
}

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[8:56) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1['properties']['evictionPolicy']|
}

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.properties['evictionPolicy']
//@[8:53) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes1.properties['evictionPolicy']|
}

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2.properties.azCliVersion
//@[8:48) [BCP118 (Error)] The property "name" cannot be assigned a runtime value. Accessible resource properties are "id", "name", and "type" and module properties are "name" |runtimeValidRes2.properties.azCliVersion|
}

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:34) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |missingTopLevelProperties|
  // #completionTest(0, 1, 2) -> topLevelProperties

}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65) -> resourceTypes
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
//@[9:32) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |discriminatorKeyMissing|
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
  kind:   
//@[10:10) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[75:76) [BCP053 (Error)] The type "Microsoft.Resources/deploymentScripts@2020-10-01" does not contain property "p". Available properties include "apiVersion", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "tags", "type", "zones". |p|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[76:76) [BCP020 (Error)] Expected a function or property name at this location. ||

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[76:76) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. ||

resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |discriminatorKeySetOne|
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[75:75) [BCP020 (Error)] Expected a function or property name at this location. ||

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[75:75) [BCP117 (Error)] An empty indexer is not allowed. Specify a valid expression. ||

resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |discriminatorKeySetTwo|
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[75:75) [BCP020 (Error)] Expected a function or property name at this location. ||

// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[90:90) [BCP020 (Error)] Expected a function or property name at this location. ||

resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |incorrectPropertiesKey|
  kind: 'AzureCLI'

  propertes: {
//@[2:11) [BCP089 (Error)] The property "propertes" is not allowed on objects of type "Microsoft.Resources/deploymentScripts@2020-10-01". Did you mean "properties"? |propertes|
  }
}

var mock = incorrectPropertiesKey.p
//@[34:35) [BCP053 (Error)] The type "Microsoft.Resources/deploymentScripts@2020-10-01" does not contain property "p". Available properties include "apiVersion", "eTag", "extendedLocation", "id", "identity", "kind", "location", "managedBy", "managedByExtended", "name", "plan", "properties", "scale", "sku", "tags", "type", "zones". |p|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'
  name: 'test'
  location: ''
  properties: {
    azCliVersion: '2'
    retentionInterval: 'PT1H'
    
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
    
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
    cleanupPreference: 
//@[23:23) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

    // #completionTest(25,26) -> arrayPlusSymbols
    supportingScriptUris: 
//@[26:26) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

    // #completionTest(27,28) -> objectPlusSymbols
    storageAccountSettings: 
//@[28:28) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. ||

    environmentVariables: [
      {
        // #completionTest(0,2,4,6,8) -> environmentVariableProperties
        
      }
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbols
      
    ]
  }
}

// #completionTest(21) -> resourceTypes
resource missingType 
//@[21:21) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". ||
//@[21:21) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". ||

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
resource startedTypingTypeWithQuotes 'virma'
//@[37:44) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |'virma'|
//@[44:44) [BCP018 (Error)] Expected the "=" character at this location. ||

// #completionTest(40,41,42,43,44,45) -> resourceTypes
resource startedTypingTypeWithoutQuotes virma
//@[40:45) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |virma|
//@[40:45) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<provider>/<types>@<apiVersion>". |virma|
//@[45:45) [BCP018 (Error)] Expected the "=" character at this location. ||

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[9:30) [BCP035 (Error)] The specified "resource" declaration is missing the following required properties: "name". |dashesInPropertyNames|
}
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[78:78) [BCP020 (Error)] Expected a function or property name at this location. ||

resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}
// #completionTest(90) -> createMode
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
// #completionTest(92) -> createMode
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[92:92) [BCP020 (Error)] Expected a function or property name at this location. ||

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']

resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
// #completionTest(73) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[69:69) [BCP020 (Error)] Expected a function or property name at this location. ||
// #completionTest(72) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[72:72) [BCP020 (Error)] Expected a function or property name at this location. ||

// #completionTest(79) -> defaultCreateModeIndexes
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[78:79) [BCP057 (Error)] The name "a" does not exist in the current context. |a|
