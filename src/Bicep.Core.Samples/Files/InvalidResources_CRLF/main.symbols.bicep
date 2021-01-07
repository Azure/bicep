
// wrong declaration
bad

// incomplete #completionTest(9) -> empty
resource 
//@[9:9) Resource <missing>. Type: error. Declaration start char: 0, length: 9
resource foo
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 12
resource fo/o
//@[9:11) Resource fo. Type: error. Declaration start char: 0, length: 13
resource foo 'ddd'
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 18

// #completionTest(23) -> resourceTypes
resource trailingSpace  
//@[9:22) Resource trailingSpace. Type: error. Declaration start char: 0, length: 24

// #completionTest(19,20) -> object
resource foo 'ddd'= 
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 20

// wrong resource type
resource foo 'ddd'={
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 23
}

resource foo 'ddd'=if (1 + 1 == 2) {
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 39
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 64
}

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 74
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 55
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 77
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 83
}

// simulate typing if condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 54

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 56

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 60

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 61

// missing condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 74
  name: 'foo'
}

// empty condition
// #completionTest(56) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 77
  name: 'foo'
}

// #completionTest(57, 59) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 82
  name: 'foo'
}

// invalid condition type
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 80
  name: 'foo'
}

// runtime functions are no allowed in resource conditions
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 165
  name: 'foo'
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'foo'
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 85
  name: 'foo'
  name: true
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 87
  name: 'foo'
  'name': true
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 121
  name: 'foo'
  properties: {
    foo: 'a'
    foo: 'a'
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 123
  name: 'foo'
  properties: {
    foo: 'a'
    'foo': 'a'
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 124
  name: 'foo'
  location: [
  ]
  tags: 'tag are not a string?'
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:12) Resource bar. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 231
  name: true ? 's' : 'a' + 1
  properties: {
    x: foo()
    y: true && (null || !4)
    a: [
      a
      !null
      true && true || true + -true * 4
    ]
  }
}

// unsupported resource ref
var resrefvar = bar.name
//@[4:13) Variable resrefvar. Type: string. Declaration start char: 0, length: 24

param resrefpar string = foo.id
//@[6:15) Parameter resrefpar. Type: string. Declaration start char: 0, length: 31

output resrefout bool = bar.id
//@[7:16) Output resrefout. Type: bool. Declaration start char: 0, length: 30

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:12) Resource baz. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'test'
  id: 2
  type: 'hello'
  apiVersion: true
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:19) Resource badDepends. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 113
  name: 'test'
  dependsOn: [
    baz.id
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends2. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 125
  name: 'test'
  dependsOn: [
    'hello'
    true
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends3. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 81
  name: 'test'
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends4. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'test'
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends5. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 117
  name: 'test'
  dependsOn: badDepends3.dependsOn
}

var interpVal = 'abc'
//@[4:13) Variable interpVal. Type: 'abc'. Declaration start char: 0, length: 21
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:18) Resource badInterp. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 205
  name: 'test'
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
  '${undefinedSymbol}': true
}

module validModule './module.bicep' = {
//@[7:18) Module validModule. Type: module. Declaration start char: 0, length: 106
  name: 'storageDeploy'
  params: {
    name: 'contoso'
  }
}

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[9:25) Resource runtimeValidRes1. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 174
  name: 'name1'
  location: 'eastus'
  properties: {
    evictionPolicy: 'Deallocate'
  }
}

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:25) Resource runtimeValidRes2. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 329
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes3. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 131
  name: '${runtimeValidRes1.name}_v1'
}

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes4. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 135
  name: concat(validModule['name'], 'v1')
}

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes5. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 126
  name: '${validModule.name}_v1'
}

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes1. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 129
  name: runtimeValidRes1.location
}

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes2. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 132
  name: runtimeValidRes1['location']
}

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:27) Resource runtimeInvalidRes3. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 292
  name: runtimeValidRes1.properties.evictionPolicy
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes4. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 149
  name: runtimeValidRes1['properties'].evictionPolicy
}

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes5. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 152
  name: runtimeValidRes1['properties']['evictionPolicy']
}

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes6. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 149
  name: runtimeValidRes1.properties['evictionPolicy']
}

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes7. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 144
  name: runtimeValidRes2.properties.azCliVersion
}

var magicString1 = 'location'
//@[4:16) Variable magicString1. Type: 'location'. Declaration start char: 0, length: 29
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes8. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 139
  name: runtimeValidRes2['${magicString1}']
}

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
var magicString2 = 'name'
//@[4:16) Variable magicString2. Type: 'name'. Declaration start char: 0, length: 25
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:27) Resource runtimeInvalidRes9. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 139
  name: runtimeValidRes2['${magicString2}']
}

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes10. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 135
  name: '${runtimeValidRes3.location}'
}

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes11. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 131
  name: validModule.params['name']
}

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes12. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 240
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
}

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes13. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 243
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
}

// variable related runtime validation
var runtimefoo1 = runtimeValidRes1['location']
//@[4:15) Variable runtimefoo1. Type: string. Declaration start char: 0, length: 46
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[4:15) Variable runtimefoo2. Type: any. Declaration start char: 0, length: 61
var runtimefoo3 = runtimeValidRes2
//@[4:15) Variable runtimefoo3. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 34
var runtimefoo4 = {
//@[4:15) Variable runtimefoo4. Type: object. Declaration start char: 0, length: 42
  hop: runtimefoo2
}

var runtimeInvalid = {
//@[4:18) Variable runtimeInvalid. Type: object. Declaration start char: 0, length: 119
  foo1: runtimefoo1
  foo2: runtimefoo2
  foo3: runtimefoo3
  foo4: runtimeValidRes1.name
}

var runtimeValid = {
//@[4:16) Variable runtimeValid. Type: object. Declaration start char: 0, length: 151
  foo1: runtimeValidRes1.name
  foo2: runtimeValidRes1.apiVersion
  foo3: runtimeValidRes2.type
  foo4: runtimeValidRes2.id
}

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes14. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 124
  name: runtimeInvalid.foo1
}

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes15. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 124
  name: runtimeInvalid.foo2
}

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes16. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 148
  name: runtimeInvalid.foo3.properties.azCliVersion
}

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes17. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 124
  name: runtimeInvalid.foo4
}

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:28) Resource runtimeInvalidRes18. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 226
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
}

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes6. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo1
}

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes7. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo2
}

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes8. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo3
}

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[9:25) Resource runtimeValidRes9. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo4
}

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:34) Resource missingTopLevelProperties. Type: Microsoft.Storage/storageAccounts@2020-08-01-preview. Declaration start char: 0, length: 151
  // #completionTest(0, 1, 2) -> topLevelProperties

}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:44) Resource missingTopLevelPropertiesExceptName. Type: Microsoft.Storage/storageAccounts@2020-08-01-preview. Declaration start char: 0, length: 304
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65) -> resourceTypes
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[9:23) Resource unfinishedVnet. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 468
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
//@[9:32) Resource discriminatorKeyMissing. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 148
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:37) Resource discriminatorKeyValueMissing. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 175
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
  kind:   
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[4:43) Variable discriminatorKeyValueMissingCompletions. Type: error. Declaration start char: 0, length: 76
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[4:44) Variable discriminatorKeyValueMissingCompletions2. Type: error. Declaration start char: 0, length: 76

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[4:44) Variable discriminatorKeyValueMissingCompletions3. Type: error. Declaration start char: 0, length: 77

resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) Resource discriminatorKeySetOne. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 264
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[4:37) Variable discriminatorKeySetOneCompletions. Type: any. Declaration start char: 0, length: 75
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[4:38) Variable discriminatorKeySetOneCompletions2. Type: error. Declaration start char: 0, length: 75

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[4:38) Variable discriminatorKeySetOneCompletions3. Type: error. Declaration start char: 0, length: 76

resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) Resource discriminatorKeySetTwo. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 270
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[4:37) Variable discriminatorKeySetTwoCompletions. Type: any. Declaration start char: 0, length: 75
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[4:38) Variable discriminatorKeySetTwoCompletions2. Type: error. Declaration start char: 0, length: 75

// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[4:49) Variable discriminatorKeySetTwoCompletionsArrayIndexer. Type: any. Declaration start char: 0, length: 90
// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[4:50) Variable discriminatorKeySetTwoCompletionsArrayIndexer2. Type: error. Declaration start char: 0, length: 90

resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) Resource incorrectPropertiesKey. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 132
  kind: 'AzureCLI'

  propertes: {
  }
}

var mock = incorrectPropertiesKey.p
//@[4:8) Variable mock. Type: error. Declaration start char: 0, length: 35

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:32) Resource incorrectPropertiesKey2. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 774
  kind: 'AzureCLI'
  name: 'test'
  location: ''
  properties: {
    azCliVersion: '2'
    retentionInterval: 'PT1H'
    
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
    
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
    cleanupPreference: 

    // #completionTest(25,26) -> arrayPlusSymbols
    supportingScriptUris: 

    // #completionTest(27,28) -> objectPlusSymbols
    storageAccountSettings: 

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
//@[9:20) Resource missingType. Type: error. Declaration start char: 0, length: 21

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
resource startedTypingTypeWithQuotes 'virma'
//@[9:36) Resource startedTypingTypeWithQuotes. Type: error. Declaration start char: 0, length: 44

// #completionTest(40,41,42,43,44,45) -> resourceTypes
resource startedTypingTypeWithoutQuotes virma
//@[9:39) Resource startedTypingTypeWithoutQuotes. Type: error. Declaration start char: 0, length: 45

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[9:30) Resource dashesInPropertyNames. Type: Microsoft.ContainerService/managedClusters@2020-09-01. Declaration start char: 0, length: 93
}
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[4:23) Variable letsAccessTheDashes. Type: any. Declaration start char: 0, length: 78
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[4:24) Variable letsAccessTheDashes2. Type: any. Declaration start char: 0, length: 78

resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[9:38) Resource nestedDiscriminatorMissingKey. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 190
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}
// #completionTest(90) -> createMode
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[4:44) Variable nestedDiscriminatorMissingKeyCompletions. Type: any. Declaration start char: 0, length: 90
// #completionTest(92) -> createMode
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[4:45) Variable nestedDiscriminatorMissingKeyCompletions2. Type: error. Declaration start char: 0, length: 92

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[4:49) Variable nestedDiscriminatorMissingKeyIndexCompletions. Type: any. Declaration start char: 0, length: 96

resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[9:28) Resource nestedDiscriminator. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 178
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[4:34) Variable nestedDiscriminatorCompletions. Type: any. Declaration start char: 0, length: 69
// #completionTest(73) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[4:35) Variable nestedDiscriminatorCompletions2. Type: any. Declaration start char: 0, length: 73
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[4:35) Variable nestedDiscriminatorCompletions3. Type: error. Declaration start char: 0, length: 69
// #completionTest(72) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[4:35) Variable nestedDiscriminatorCompletions4. Type: error. Declaration start char: 0, length: 72

// #completionTest(79) -> defaultCreateModeIndexes
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[4:44) Variable nestedDiscriminatorArrayIndexCompletions. Type: error. Declaration start char: 0, length: 80

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[9:18) Resource selfScope. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 98
  name: 'selfScope'
  scope: selfScope
}

var notAResource = {
//@[4:16) Variable notAResource. Type: object. Declaration start char: 0, length: 54
  im: 'not'
  a: 'resource!'
}
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[9:21) Resource invalidScope. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 107
  name: 'invalidScope'
  scope: notAResource
}

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[9:22) Resource invalidScope2. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 112
  name: 'invalidScope2'
  scope: resourceGroup()
}

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[9:22) Resource invalidScope3. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 111
  name: 'invalidScope3'
  scope: subscription()
}
