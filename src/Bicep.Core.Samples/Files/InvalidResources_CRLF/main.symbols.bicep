
// wrong declaration
bad

// incomplete #completionTest(9) -> empty
resource 
//@[009:009) Resource <missing>. Type: error. Declaration start char: 0, length: 9
resource foo
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 12
resource fo/o
//@[009:011) Resource fo. Type: error. Declaration start char: 0, length: 13
resource foo 'ddd'
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 18

// #completionTest(23) -> resourceTypes
resource trailingSpace  
//@[009:022) Resource trailingSpace. Type: error. Declaration start char: 0, length: 24

// #completionTest(19,20) -> resourceObject
resource foo 'ddd'= 
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 20

// wrong resource type
resource foo 'ddd'={
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 23
}

resource foo 'ddd'=if (1 + 1 == 2) {
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 39
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 64
}

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[009:012) Resource foo. Type: error. Declaration start char: 0, length: 74
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 55
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 77
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 83
}

// simulate typing if condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 54

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 56

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 60

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 61

// missing condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 74
  name: 'foo'
}

// empty condition
// #completionTest(56) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 77
  name: 'foo'
}

// #completionTest(57, 59) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 82
  name: 'foo'
}

// invalid condition type
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 80
  name: 'foo'
}

// runtime functions are no allowed in resource conditions
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 165
  name: 'foo'
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'foo'
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 85
  name: 'foo'
  name: true
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 87
  name: 'foo'
  'name': true
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 121
  name: 'foo'
  properties: {
    foo: 'a'
    foo: 'a'
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 123
  name: 'foo'
  properties: {
    foo: 'a'
    'foo': 'a'
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:012) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 124
  name: 'foo'
  location: [
  ]
  tags: 'tag are not a string?'
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:012) Resource bar. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 231
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

// there should be no completions without the colon
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:034) Resource noCompletionsWithoutColon. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 138
  // #completionTest(7,8) -> empty
  kind  
}

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:033) Resource noCompletionsBeforeColon. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 138
  // #completionTest(7,8) -> empty
  kind  :
}

// unsupported resource ref
var resrefvar = bar.name
//@[004:013) Variable resrefvar. Type: string. Declaration start char: 0, length: 24

param resrefpar string = foo.id
//@[006:015) Parameter resrefpar. Type: string. Declaration start char: 0, length: 31

output resrefout bool = bar.id
//@[007:016) Output resrefout. Type: bool. Declaration start char: 0, length: 30

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:012) Resource baz. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'test'
  id: 2
  type: 'hello'
  apiVersion: true
}

resource readOnlyPropertyAssignment 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[009:035) Resource readOnlyPropertyAssignment. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 352
  name: 'vnet-bicep'
  location: 'westeurope'
  etag: 'assigning-to-read-only-value'
  properties: {
    resourceGuid: 'assigning-to-read-only-value'
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: []
  }
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:019) Resource badDepends. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 113
  name: 'test'
  dependsOn: [
    baz.id
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:020) Resource badDepends2. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 125
  name: 'test'
  dependsOn: [
    'hello'
    true
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:020) Resource badDepends3. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 81
  name: 'test'
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:020) Resource badDepends4. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'test'
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:020) Resource badDepends5. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 117
  name: 'test'
  dependsOn: badDepends3.dependsOn
}

var interpVal = 'abc'
//@[004:013) Variable interpVal. Type: 'abc'. Declaration start char: 0, length: 21
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[009:018) Resource badInterp. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 205
  name: 'test'
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
  '${undefinedSymbol}': true
}

module validModule './module.bicep' = {
//@[007:018) Module validModule. Type: module. Declaration start char: 0, length: 106
  name: 'storageDeploy'
  params: {
    name: 'contoso'
  }
}

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[009:025) Resource runtimeValidRes1. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 174
  name: 'name1'
  location: 'eastus'
  properties: {
    evictionPolicy: 'Deallocate'
  }
}

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:025) Resource runtimeValidRes2. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 329
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes3. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 131
  name: '${runtimeValidRes1.name}_v1'
}

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes4. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 135
  name: concat(validModule['name'], 'v1')
}

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes5. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 126
  name: '${validModule.name}_v1'
}

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes1. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 129
  name: runtimeValidRes1.location
}

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes2. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 132
  name: runtimeValidRes1['location']
}

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:027) Resource runtimeInvalidRes3. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 292
  name: runtimeValidRes1.properties.evictionPolicy
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes4. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 149
  name: runtimeValidRes1['properties'].evictionPolicy
}

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes5. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 152
  name: runtimeValidRes1['properties']['evictionPolicy']
}

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes6. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 149
  name: runtimeValidRes1.properties['evictionPolicy']
}

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes7. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 144
  name: runtimeValidRes2.properties.azCliVersion
}

var magicString1 = 'location'
//@[004:016) Variable magicString1. Type: 'location'. Declaration start char: 0, length: 29
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes8. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 139
  name: runtimeValidRes2['${magicString1}']
}

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
var magicString2 = 'name'
//@[004:016) Variable magicString2. Type: 'name'. Declaration start char: 0, length: 25
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:027) Resource runtimeInvalidRes9. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 139
  name: runtimeValidRes2['${magicString2}']
}

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes10. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 135
  name: '${runtimeValidRes3.location}'
}

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes11. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 131
  name: validModule.params['name']
}

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes12. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 240
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
}

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes13. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 243
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
}

// variable related runtime validation
var runtimefoo1 = runtimeValidRes1['location']
//@[004:015) Variable runtimefoo1. Type: 'eastus'. Declaration start char: 0, length: 46
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[004:015) Variable runtimefoo2. Type: '2.0'. Declaration start char: 0, length: 61
var runtimefoo3 = runtimeValidRes2
//@[004:015) Variable runtimefoo3. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 34
var runtimefoo4 = {
//@[004:015) Variable runtimefoo4. Type: object. Declaration start char: 0, length: 42
  hop: runtimefoo2
}

var runtimeInvalid = {
//@[004:018) Variable runtimeInvalid. Type: object. Declaration start char: 0, length: 119
  foo1: runtimefoo1
  foo2: runtimefoo2
  foo3: runtimefoo3
  foo4: runtimeValidRes1.name
}

var runtimeValid = {
//@[004:016) Variable runtimeValid. Type: object. Declaration start char: 0, length: 151
  foo1: runtimeValidRes1.name
  foo2: runtimeValidRes1.apiVersion
  foo3: runtimeValidRes2.type
  foo4: runtimeValidRes2.id
}

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes14. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 124
  name: runtimeInvalid.foo1
}

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes15. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 124
  name: runtimeInvalid.foo2
}

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes16. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 148
  name: runtimeInvalid.foo3.properties.azCliVersion
}

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes17. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 124
  name: runtimeInvalid.foo4
}

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:028) Resource runtimeInvalidRes18. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 226
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
}

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes6. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo1
}

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes7. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo2
}

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes8. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo3
}

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[009:025) Resource runtimeValidRes9. Type: Microsoft.Advisor/recommendations/suppressions@2020-01-01. Declaration start char: 0, length: 119
  name: runtimeValid.foo4
}


resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[076:081) Local thing. Type: never. Declaration start char: 76, length: 5
//@[009:028) Resource loopForRuntimeCheck. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 130
  name: 'test'
  location: 'test'
}]

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
//@[004:019) Variable runtimeCheckVar. Type: 'Private' | 'Public'. Declaration start char: 0, length: 64
var runtimeCheckVar2 = runtimeCheckVar
//@[004:020) Variable runtimeCheckVar2. Type: 'Private' | 'Public'. Declaration start char: 0, length: 38

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[009:038) Resource singleResourceForRuntimeCheck. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 131
  name: runtimeCheckVar2
  location: 'test'
}

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[077:082) Local thing. Type: never. Declaration start char: 77, length: 5
//@[009:029) Resource loopForRuntimeCheck2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 141
  name: runtimeCheckVar2
  location: 'test'
}]

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[077:087) Local otherThing. Type: never. Declaration start char: 77, length: 10
//@[009:029) Resource loopForRuntimeCheck3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 172
  name: loopForRuntimeCheck[0].properties.zoneType
  location: 'test'
}]

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
//@[004:024) Variable varForRuntimeCheck4a. Type: 'Private' | 'Public'. Declaration start char: 0, length: 69
var varForRuntimeCheck4b = varForRuntimeCheck4a
//@[004:024) Variable varForRuntimeCheck4b. Type: 'Private' | 'Public'. Declaration start char: 0, length: 47
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[077:087) Local otherThing. Type: never. Declaration start char: 77, length: 10
//@[009:029) Resource loopForRuntimeCheck4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 150
  name: varForRuntimeCheck4b
  location: 'test'
}]

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[009:034) Resource missingTopLevelProperties. Type: Microsoft.Storage/storageAccounts@2020-08-01-preview. Declaration start char: 0, length: 153
  // #completionTest(0, 1, 2) -> topLevelProperties
  
}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[009:044) Resource missingTopLevelPropertiesExceptName. Type: Microsoft.Storage/storageAccounts@2020-08-01-preview. Declaration start char: 0, length: 305
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[009:023) Resource unfinishedVnet. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 531
  name: 'v'
  location: 'eastus'
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
//@[009:032) Resource discriminatorKeyMissing. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 148
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (conditional)
*/
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[009:035) Resource discriminatorKeyMissing_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 160
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (loop)
*/
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[095:100) Local thing. Type: never. Declaration start char: 95, length: 5
//@[009:036) Resource discriminatorKeyMissing_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 171
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key missing (filtered loop)
*/
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[098:103) Local thing. Type: never. Declaration start char: 98, length: 5
//@[009:039) Resource discriminatorKeyMissing_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 183
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key value missing with property access
*/
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:037) Resource discriminatorKeyValueMissing. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 175
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
  kind:   
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[004:043) Variable discriminatorKeyValueMissingCompletions. Type: any. Declaration start char: 0, length: 76
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[004:044) Variable discriminatorKeyValueMissingCompletions2. Type: any. Declaration start char: 0, length: 76

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[004:044) Variable discriminatorKeyValueMissingCompletions3. Type: error. Declaration start char: 0, length: 77

/*
Discriminator key value missing with property access (conditional)
*/
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
//@[009:040) Resource discriminatorKeyValueMissing_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 191
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
  kind:   
}
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[004:046) Variable discriminatorKeyValueMissingCompletions_if. Type: any. Declaration start char: 0, length: 82
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[004:047) Variable discriminatorKeyValueMissingCompletions2_if. Type: any. Declaration start char: 0, length: 82

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[004:047) Variable discriminatorKeyValueMissingCompletions3_if. Type: error. Declaration start char: 0, length: 83

/*
Discriminator key value missing with property access (loops)
*/
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[100:105) Local thing. Type: never. Declaration start char: 100, length: 5
//@[009:041) Resource discriminatorKeyValueMissing_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 202
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
  kind:   
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[004:035) Variable resourceListIsNotSingleResource. Type: error. Declaration start char: 0, length: 75

// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[004:047) Variable discriminatorKeyValueMissingCompletions_for. Type: any. Declaration start char: 0, length: 87
// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[004:048) Variable discriminatorKeyValueMissingCompletions2_for. Type: any. Declaration start char: 0, length: 87

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[004:048) Variable discriminatorKeyValueMissingCompletions3_for. Type: error. Declaration start char: 0, length: 88

/*
Discriminator key value missing with property access (filtered loops)
*/
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[103:108) Local thing. Type: never. Declaration start char: 103, length: 5
//@[009:044) Resource discriminatorKeyValueMissing_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 217
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
  kind:   
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[004:038) Variable resourceListIsNotSingleResource_if. Type: error. Declaration start char: 0, length: 81

// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[004:050) Variable discriminatorKeyValueMissingCompletions_for_if. Type: any. Declaration start char: 0, length: 93
// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[004:051) Variable discriminatorKeyValueMissingCompletions2_for_if. Type: any. Declaration start char: 0, length: 93

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[004:051) Variable discriminatorKeyValueMissingCompletions3_for_if. Type: error. Declaration start char: 0, length: 94

/*
Discriminator value set 1
*/
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) Resource discriminatorKeySetOne. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 266
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[004:037) Variable discriminatorKeySetOneCompletions. Type: any. Declaration start char: 0, length: 75
// #completionTest(75) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[004:038) Variable discriminatorKeySetOneCompletions2. Type: error. Declaration start char: 0, length: 75

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[004:038) Variable discriminatorKeySetOneCompletions3. Type: error. Declaration start char: 0, length: 76

/*
Discriminator value set 1 (conditional)
*/
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[009:034) Resource discriminatorKeySetOne_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 278
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[004:040) Variable discriminatorKeySetOneCompletions_if. Type: any. Declaration start char: 0, length: 81
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[004:041) Variable discriminatorKeySetOneCompletions2_if. Type: error. Declaration start char: 0, length: 81

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[004:041) Variable discriminatorKeySetOneCompletions3_if. Type: error. Declaration start char: 0, length: 82

/*
Discriminator value set 1 (loop)
*/
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[095:100) Local thing. Type: never. Declaration start char: 95, length: 5
//@[009:035) Resource discriminatorKeySetOne_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 290
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(86) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[004:041) Variable discriminatorKeySetOneCompletions_for. Type: any. Declaration start char: 0, length: 86
// #completionTest(94) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[004:042) Variable discriminatorKeySetOneCompletions2_for. Type: error. Declaration start char: 0, length: 94

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[004:042) Variable discriminatorKeySetOneCompletions3_for. Type: error. Declaration start char: 0, length: 87

/*
Discriminator value set 1 (filtered loop)
*/
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[098:103) Local thing. Type: never. Declaration start char: 98, length: 5
//@[009:038) Resource discriminatorKeySetOne_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 302
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(92) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[004:044) Variable discriminatorKeySetOneCompletions_for_if. Type: any. Declaration start char: 0, length: 92
// #completionTest(100) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[004:045) Variable discriminatorKeySetOneCompletions2_for_if. Type: error. Declaration start char: 0, length: 100

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[004:045) Variable discriminatorKeySetOneCompletions3_for_if. Type: error. Declaration start char: 0, length: 93


/*
Discriminator value set 2
*/
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) Resource discriminatorKeySetTwo. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 272
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[004:037) Variable discriminatorKeySetTwoCompletions. Type: any. Declaration start char: 0, length: 75
// #completionTest(75) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[004:038) Variable discriminatorKeySetTwoCompletions2. Type: error. Declaration start char: 0, length: 75

// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[004:049) Variable discriminatorKeySetTwoCompletionsArrayIndexer. Type: any. Declaration start char: 0, length: 90
// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[004:050) Variable discriminatorKeySetTwoCompletionsArrayIndexer2. Type: error. Declaration start char: 0, length: 90

/*
Discriminator value set 2 (conditional)
*/
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:034) Resource discriminatorKeySetTwo_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 275
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[004:040) Variable discriminatorKeySetTwoCompletions_if. Type: any. Declaration start char: 0, length: 81
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[004:041) Variable discriminatorKeySetTwoCompletions2_if. Type: error. Declaration start char: 0, length: 81

// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[004:052) Variable discriminatorKeySetTwoCompletionsArrayIndexer_if. Type: any. Declaration start char: 0, length: 96
// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[004:053) Variable discriminatorKeySetTwoCompletionsArrayIndexer2_if. Type: error. Declaration start char: 0, length: 96


/*
Discriminator value set 2 (loops)
*/
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[094:099) Local thing. Type: never. Declaration start char: 94, length: 5
//@[009:035) Resource discriminatorKeySetTwo_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 295
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[004:041) Variable discriminatorKeySetTwoCompletions_for. Type: any. Declaration start char: 0, length: 86
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[004:042) Variable discriminatorKeySetTwoCompletions2_for. Type: error. Declaration start char: 0, length: 86

// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[004:053) Variable discriminatorKeySetTwoCompletionsArrayIndexer_for. Type: any. Declaration start char: 0, length: 101
// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[004:054) Variable discriminatorKeySetTwoCompletionsArrayIndexer2_for. Type: error. Declaration start char: 0, length: 101


/*
Discriminator value set 2 (filtered loops)
*/
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[097:102) Local thing. Type: never. Declaration start char: 97, length: 5
//@[009:038) Resource discriminatorKeySetTwo_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 307
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[004:044) Variable discriminatorKeySetTwoCompletions_for_if. Type: any. Declaration start char: 0, length: 92
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[004:045) Variable discriminatorKeySetTwoCompletions2_for_if. Type: error. Declaration start char: 0, length: 92

// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[004:056) Variable discriminatorKeySetTwoCompletionsArrayIndexer_for_if. Type: any. Declaration start char: 0, length: 107
// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[004:057) Variable discriminatorKeySetTwoCompletionsArrayIndexer2_for_if. Type: error. Declaration start char: 0, length: 107



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) Resource incorrectPropertiesKey. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 132
  kind: 'AzureCLI'

  propertes: {
  }
}

var mock = incorrectPropertiesKey.p
//@[004:008) Variable mock. Type: error. Declaration start char: 0, length: 35

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:032) Resource incorrectPropertiesKey2. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 796
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
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbolsWithRequiredProperties
      
    ]
  }
}

// #completionTest(21) -> resourceTypes
resource missingType 
//@[009:020) Resource missingType. Type: error. Declaration start char: 0, length: 21

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
resource startedTypingTypeWithQuotes 'virma'
//@[009:036) Resource startedTypingTypeWithQuotes. Type: error. Declaration start char: 0, length: 44

// #completionTest(40,41,42,43,44,45) -> resourceTypes
resource startedTypingTypeWithoutQuotes virma
//@[009:039) Resource startedTypingTypeWithoutQuotes. Type: error. Declaration start char: 0, length: 45

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[009:030) Resource dashesInPropertyNames. Type: Microsoft.ContainerService/managedClusters@2020-09-01. Declaration start char: 0, length: 93
}
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[004:023) Variable letsAccessTheDashes. Type: any. Declaration start char: 0, length: 78
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[004:024) Variable letsAccessTheDashes2. Type: error. Declaration start char: 0, length: 78

/* 
Nested discriminator missing key
*/
resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[009:038) Resource nestedDiscriminatorMissingKey. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 190
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}
// #completionTest(90) -> createMode
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[004:044) Variable nestedDiscriminatorMissingKeyCompletions. Type: any. Declaration start char: 0, length: 90
// #completionTest(92) -> createMode
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[004:045) Variable nestedDiscriminatorMissingKeyCompletions2. Type: any. Declaration start char: 0, length: 92

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[004:049) Variable nestedDiscriminatorMissingKeyIndexCompletions. Type: any. Declaration start char: 0, length: 96

/* 
Nested discriminator missing key (conditional)
*/
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
//@[009:041) Resource nestedDiscriminatorMissingKey_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 205
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}
// #completionTest(96) -> createMode
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[004:047) Variable nestedDiscriminatorMissingKeyCompletions_if. Type: any. Declaration start char: 0, length: 96
// #completionTest(98) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[004:048) Variable nestedDiscriminatorMissingKeyCompletions2_if. Type: any. Declaration start char: 0, length: 98

// #completionTest(100) -> createModeIndexPlusSymbols_if
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[004:052) Variable nestedDiscriminatorMissingKeyIndexCompletions_if. Type: any. Declaration start char: 0, length: 102

/* 
Nested discriminator missing key (loop)
*/
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[109:114) Local thing. Type: never. Declaration start char: 109, length: 5
//@[009:042) Resource nestedDiscriminatorMissingKey_for. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 213
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}]
// #completionTest(101) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[004:048) Variable nestedDiscriminatorMissingKeyCompletions_for. Type: any. Declaration start char: 0, length: 101
// #completionTest(103) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[004:049) Variable nestedDiscriminatorMissingKeyCompletions2_for. Type: any. Declaration start char: 0, length: 103

// #completionTest(105) -> createModeIndexPlusSymbols_for
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[004:053) Variable nestedDiscriminatorMissingKeyIndexCompletions_for. Type: any. Declaration start char: 0, length: 107


/* 
Nested discriminator missing key (filtered loop)
*/
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[112:117) Local thing. Type: never. Declaration start char: 112, length: 5
//@[009:045) Resource nestedDiscriminatorMissingKey_for_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 225
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}]
// #completionTest(107) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[004:051) Variable nestedDiscriminatorMissingKeyCompletions_for_if. Type: any. Declaration start char: 0, length: 107
// #completionTest(109) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[004:052) Variable nestedDiscriminatorMissingKeyCompletions2_for_if. Type: any. Declaration start char: 0, length: 109

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[004:056) Variable nestedDiscriminatorMissingKeyIndexCompletions_for_if. Type: any. Declaration start char: 0, length: 113


/*
Nested discriminator
*/
resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[009:028) Resource nestedDiscriminator. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 178
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[004:034) Variable nestedDiscriminatorCompletions. Type: any. Declaration start char: 0, length: 69
// #completionTest(73) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[004:035) Variable nestedDiscriminatorCompletions2. Type: any. Declaration start char: 0, length: 73
// #completionTest(69) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[004:035) Variable nestedDiscriminatorCompletions3. Type: error. Declaration start char: 0, length: 69
// #completionTest(72) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[004:035) Variable nestedDiscriminatorCompletions4. Type: error. Declaration start char: 0, length: 72

// #completionTest(79) -> defaultCreateModeIndexes
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[004:044) Variable nestedDiscriminatorArrayIndexCompletions. Type: error. Declaration start char: 0, length: 80

/*
Nested discriminator (conditional)
*/
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
//@[009:031) Resource nestedDiscriminator_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 190
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[004:037) Variable nestedDiscriminatorCompletions_if. Type: any. Declaration start char: 0, length: 75
// #completionTest(79) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[004:038) Variable nestedDiscriminatorCompletions2_if. Type: any. Declaration start char: 0, length: 79
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[004:038) Variable nestedDiscriminatorCompletions3_if. Type: error. Declaration start char: 0, length: 75
// #completionTest(78) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[004:038) Variable nestedDiscriminatorCompletions4_if. Type: error. Declaration start char: 0, length: 78

// #completionTest(85) -> defaultCreateModeIndexes_if
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[004:047) Variable nestedDiscriminatorArrayIndexCompletions_if. Type: error. Declaration start char: 0, length: 86


/*
Nested discriminator (loop)
*/
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[099:104) Local thing. Type: never. Declaration start char: 99, length: 5
//@[009:032) Resource nestedDiscriminator_for. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 201
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}]
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[004:038) Variable nestedDiscriminatorCompletions_for. Type: any. Declaration start char: 0, length: 80
// #completionTest(84) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[004:039) Variable nestedDiscriminatorCompletions2_for. Type: any. Declaration start char: 0, length: 84
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[004:039) Variable nestedDiscriminatorCompletions3_for. Type: error. Declaration start char: 0, length: 80
// #completionTest(83) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[004:039) Variable nestedDiscriminatorCompletions4_for. Type: error. Declaration start char: 0, length: 83

// #completionTest(90) -> defaultCreateModeIndexes_for
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[004:048) Variable nestedDiscriminatorArrayIndexCompletions_for. Type: error. Declaration start char: 0, length: 91


/*
Nested discriminator (filtered loop)
*/
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[102:107) Local thing. Type: never. Declaration start char: 102, length: 5
//@[009:035) Resource nestedDiscriminator_for_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 213
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}]
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[004:041) Variable nestedDiscriminatorCompletions_for_if. Type: any. Declaration start char: 0, length: 86
// #completionTest(90) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[004:042) Variable nestedDiscriminatorCompletions2_for_if. Type: any. Declaration start char: 0, length: 90
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[004:042) Variable nestedDiscriminatorCompletions3_for_if. Type: error. Declaration start char: 0, length: 86
// #completionTest(89) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[004:042) Variable nestedDiscriminatorCompletions4_for_if. Type: error. Declaration start char: 0, length: 89

// #completionTest(96) -> defaultCreateModeIndexes_for_if
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[004:051) Variable nestedDiscriminatorArrayIndexCompletions_for_if. Type: error. Declaration start char: 0, length: 97



// sample resource to validate completions on the next declarations
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
//@[009:042) Resource nestedPropertyAccessOnConditional. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 209
  location: 'test'
  name: 'test'
  properties: {
    additionalCapabilities: {
      
    }
  }
}
// this validates that we can get nested property access completions on a conditional resource
//#completionTest(56) -> vmProperties
var sigh = nestedPropertyAccessOnConditional.properties.
//@[004:008) Variable sigh. Type: error. Declaration start char: 0, length: 56

/*
  boolean property value completions
*/ 
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[009:036) Resource booleanPropertyPartialValue. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 222
  properties: {
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
    autoUpgradeMinorVersion: t
  }
}

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[009:018) Resource selfScope. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 98
  name: 'selfScope'
  scope: selfScope
}

var notAResource = {
//@[004:016) Variable notAResource. Type: object. Declaration start char: 0, length: 54
  im: 'not'
  a: 'resource!'
}
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[009:021) Resource invalidScope. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 107
  name: 'invalidScope'
  scope: notAResource
}

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[009:022) Resource invalidScope2. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 112
  name: 'invalidScope2'
  scope: resourceGroup()
}

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[009:022) Resource invalidScope3. Type: My.Rp/mockResource@2020-12-01. Declaration start char: 0, length: 111
  name: 'invalidScope3'
  scope: subscription()
}

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[009:030) Resource invalidDuplicateName1. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 103
  name: 'invalidDuplicateName'
}
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[009:030) Resource invalidDuplicateName2. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 103
  name: 'invalidDuplicateName'
}
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[009:030) Resource invalidDuplicateName3. Type: Mock.Rp/mockResource@2019-01-01. Declaration start char: 0, length: 103
  name: 'invalidDuplicateName'
}

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[009:062) Resource validResourceForInvalidExtensionResourceDuplicateName. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 168
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
}

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[009:047) Resource invalidExtensionResourceDuplicateName1. Type: Mock.Rp/mockExtResource@2020-01-01. Declaration start char: 0, length: 204
  name: 'invalidExtensionResourceDuplicateName'
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[009:047) Resource invalidExtensionResourceDuplicateName2. Type: Mock.Rp/mockExtResource@2019-01-01. Declaration start char: 0, length: 204
  name: 'invalidExtensionResourceDuplicateName'
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

@concat('foo', 'bar')
@secure()
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[009:025) Resource invalidDecorator. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 131
  name: 'invalidDecorator'
}

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[009:018) Resource cyclicRes. Type: Mock.Rp/mockExistingResource@2020-01-01. Declaration start char: 0, length: 108
  name: 'cyclicRes'
  scope: cyclicRes
}

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[009:026) Resource cyclicExistingRes. Type: Mock.Rp/mockExistingResource@2020-01-01. Declaration start char: 0, length: 141
  name: 'cyclicExistingRes'
  scope: cyclicExistingRes
}

// loop parsing cases
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[009:027) Resource expectedForKeyword. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 79

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[009:028) Resource expectedForKeyword2. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 81

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[009:024) Resource expectedLoopVar. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 79

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[081:082) Local x. Type: any. Declaration start char: 81, length: 1
//@[009:026) Resource expectedInKeyword. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 83

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[082:083) Local x. Type: any. Declaration start char: 82, length: 1
//@[009:027) Resource expectedInKeyword2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 86

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[087:088) Local x. Type: any. Declaration start char: 87, length: 1
//@[009:032) Resource expectedArrayExpression. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 92

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[077:078) Local x. Type: any. Declaration start char: 77, length: 1
//@[009:022) Resource expectedColon. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 84

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[080:081) Local x. Type: any. Declaration start char: 80, length: 1
//@[009:025) Resource expectedLoopBody. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 88

// loop index parsing cases
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[009:029) Resource expectedLoopItemName. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 80

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[009:030) Resource expectedLoopItemName2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 79

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[009:022) Resource expectedComma. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 74

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[009:030) Resource expectedLoopIndexName. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 84

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[076:077) Local x. Type: any. Declaration start char: 76, length: 1
//@[079:080) Local y. Type: int. Declaration start char: 79, length: 1
//@[009:027) Resource expectedInKeyword3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 82

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[076:077) Local x. Type: any. Declaration start char: 76, length: 1
//@[079:080) Local y. Type: int. Declaration start char: 79, length: 1
//@[009:027) Resource expectedInKeyword4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 84

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[082:083) Local x. Type: any. Declaration start char: 82, length: 1
//@[085:086) Local y. Type: int. Declaration start char: 85, length: 1
//@[009:033) Resource expectedArrayExpression2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 92

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[072:073) Local x. Type: any. Declaration start char: 72, length: 1
//@[075:076) Local y. Type: int. Declaration start char: 75, length: 1
//@[009:023) Resource expectedColon2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 83

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[075:076) Local x. Type: any. Declaration start char: 75, length: 1
//@[078:079) Local y. Type: int. Declaration start char: 78, length: 1
//@[009:026) Resource expectedLoopBody2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 87

// loop filter parsing cases
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[091:092) Local x. Type: any. Declaration start char: 91, length: 1
//@[009:036) Resource expectedLoopFilterOpenParen. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 102
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[086:087) Local x. Type: any. Declaration start char: 86, length: 1
//@[089:090) Local y. Type: int. Declaration start char: 89, length: 1
//@[009:037) Resource expectedLoopFilterOpenParen2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 101

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[098:099) Local x. Type: any. Declaration start char: 98, length: 1
//@[009:043) Resource expectedLoopFilterPredicateAndBody. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 111
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[093:094) Local x. Type: any. Declaration start char: 93, length: 1
//@[096:097) Local y. Type: int. Declaration start char: 96, length: 1
//@[009:044) Resource expectedLoopFilterPredicateAndBody2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 110

// wrong body type
var emptyArray = []
//@[004:014) Variable emptyArray. Type: []. Declaration start char: 0, length: 19
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[081:082) Local x. Type: never. Declaration start char: 81, length: 1
//@[009:026) Resource wrongLoopBodyType. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 99
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[083:084) Local x. Type: never. Declaration start char: 83, length: 1
//@[086:087) Local i. Type: int. Declaration start char: 86, length: 1
//@[009:027) Resource wrongLoopBodyType2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 105

// duplicate variable in the same scope
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[080:084) Local same. Type: never. Declaration start char: 80, length: 4
//@[086:090) Local same. Type: int. Declaration start char: 86, length: 4
//@[009:029) Resource itemAndIndexSameName. Type: Microsoft.AAD/domainServices@2020-01-01[]. Declaration start char: 0, length: 112
}]

// errors in the array expression
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[085:092) Local account. Type: any. Declaration start char: 85, length: 7
//@[009:030) Resource arrayExpressionErrors. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 115
}]
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[087:094) Local account. Type: any. Declaration start char: 87, length: 7
//@[095:096) Local k. Type: int. Declaration start char: 95, length: 1
//@[009:031) Resource arrayExpressionErrors2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 120
}]

// wrong array type
var notAnArray = true
//@[004:014) Variable notAnArray. Type: true. Declaration start char: 0, length: 21
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[078:085) Local account. Type: any. Declaration start char: 78, length: 7
//@[009:023) Resource wrongArrayType. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 106
}]
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[080:087) Local account. Type: any. Declaration start char: 80, length: 7
//@[088:089) Local i. Type: int. Declaration start char: 88, length: 1
//@[009:024) Resource wrongArrayType2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 111
}]

// wrong filter expression type
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[089:096) Local account. Type: never. Declaration start char: 89, length: 7
//@[009:034) Resource wrongFilterExpressionType. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 123
}]
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[091:098) Local account. Type: never. Declaration start char: 91, length: 7
//@[099:100) Local i. Type: int. Declaration start char: 99, length: 1
//@[009:035) Resource wrongFilterExpressionType2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 137
}]

// missing required properties
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[089:096) Local account. Type: never. Declaration start char: 89, length: 7
//@[009:034) Resource missingRequiredProperties. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 109
}]
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[091:098) Local account. Type: never. Declaration start char: 91, length: 7
//@[099:100) Local j. Type: int. Declaration start char: 99, length: 1
//@[009:035) Resource missingRequiredProperties2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 114
}]

// fewer missing required properties and a wrong property
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[094:101) Local account. Type: never. Declaration start char: 94, length: 7
//@[009:039) Resource missingFewerRequiredProperties. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 196
  name: account
  location: 'eastus42'
  properties: {
    wrong: 'test'
  }
}]

// wrong property inside the nested property loop
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[089:090) Local i. Type: 0 | 1 | 2. Declaration start char: 89, length: 1
//@[009:034) Resource wrongPropertyInNestedLoop. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 262
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
//@[018:019) Local j. Type: 0 | 1 | 2 | 3. Declaration start char: 18, length: 1
      doesNotExist: 'test'
      name: 'subnet-${i}-${j}'
    }]
  }
}]
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
//@[091:092) Local i. Type: 0 | 1 | 2. Declaration start char: 91, length: 1
//@[093:094) Local k. Type: int. Declaration start char: 93, length: 1
//@[009:035) Resource wrongPropertyInNestedLoop2. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 272
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
//@[018:019) Local j. Type: 0 | 1 | 2 | 3. Declaration start char: 18, length: 1
      doesNotExist: 'test'
      name: 'subnet-${i}-${j}-${k}'
    }]
  }
}]

// nonexistent arrays and loop variables
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[081:082) Local i. Type: any. Declaration start char: 81, length: 1
//@[009:026) Resource nonexistentArrays. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 280
  name: 'vnet-${justPlainWrong}'
  properties: {
    subnets: [for j in alsoNotAThing: {
//@[018:019) Local j. Type: any. Declaration start char: 18, length: 1
      doesNotExist: 'test'
      name: 'subnet-${fake}-${totallyFake}'
    }]
  }
}]

// property loops cannot be nested
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[087:094) Local account. Type: any. Declaration start char: 87, length: 7
//@[009:032) Resource propertyLoopsCannotNest. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 428
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {

    networkAcls: {
      virtualNetworkRules: [for rule in []: {
//@[032:036) Local rule. Type: never. Declaration start char: 32, length: 4
        id: '${account.name}-${account.location}'
        state: [for lol in []: 4]
//@[020:023) Local lol. Type: never. Declaration start char: 20, length: 3
      }]
    }
  }
}]
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[089:096) Local account. Type: any. Declaration start char: 89, length: 7
//@[097:098) Local i. Type: int. Declaration start char: 97, length: 1
//@[009:033) Resource propertyLoopsCannotNest2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 441
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {

    networkAcls: {
      virtualNetworkRules: [for (rule,j) in []: {
//@[033:037) Local rule. Type: never. Declaration start char: 33, length: 4
//@[038:039) Local j. Type: int. Declaration start char: 38, length: 1
        id: '${account.name}-${account.location}'
        state: [for (lol,k) in []: 4]
//@[021:024) Local lol. Type: never. Declaration start char: 21, length: 3
//@[025:026) Local k. Type: int. Declaration start char: 25, length: 1
      }]
    }
  }
}]

// property loops cannot be nested (even more nesting)
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[088:095) Local account. Type: any. Declaration start char: 88, length: 7
//@[009:033) Resource propertyLoopsCannotNest2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 634
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls:  {
      virtualNetworkRules: [for rule in []: {
//@[032:036) Local rule. Type: never. Declaration start char: 32, length: 4
        // #completionTest(15,31) -> symbolsPlusRule
        id: '${account.name}-${account.location}'
        state: [for state in []: {
//@[020:025) Local state. Type: never. Declaration start char: 20, length: 5
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
          fake: [for something in []: true]
//@[021:030) Local something. Type: never. Declaration start char: 21, length: 9
        }]
      }]
    }
  }
}]

// loops cannot be used inside of expressions
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[070:077) Local account. Type: any. Declaration start char: 70, length: 7
//@[009:015) Resource stuffs. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 381
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      virtualNetworkRules: concat([for lol in []: {
//@[039:042) Local lol. Type: never. Declaration start char: 39, length: 3
        id: '${account.name}-${account.location}'
      }])
    }
  }
}]

// using the same loop variable in a new language scope should be allowed
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[079:086) Local account. Type: any. Declaration start char: 79, length: 7
//@[009:024) Resource premiumStorages. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 368
  // #completionTest(7) -> symbolsPlusAccount1
  name: account.name
  // #completionTest(12) -> symbolsPlusAccount2
  location: account.location
  sku: {
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
    name: 
  }
  kind: 'StorageV2'
}]

var directRefViaVar = premiumStorages
//@[004:019) Variable directRefViaVar. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 37
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[007:025) Output directRefViaOutput. Type: array. Declaration start char: 0, length: 64

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
//@[009:039) Resource directRefViaSingleResourceBody. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 199
  name: 'myZone2'
  location: 'global'
  properties: {
    registrationVirtualNetworks: premiumStorages
  }
}

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
//@[009:050) Resource directRefViaSingleConditionalResourceBody. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 235
  name: 'myZone3'
  location: 'global'
  properties: {
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
  }
}

@batchSize()
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[098:099) Local i. Type: 0 | 1 | 2. Declaration start char: 98, length: 1
//@[009:043) Resource directRefViaSingleLoopResourceBody. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 208
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
  }
}]

@batchSize(0)
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[116:117) Local i. Type: 0 | 1 | 2. Declaration start char: 116, length: 1
//@[009:061) Resource directRefViaSingleLoopResourceBodyWithExtraDependsOn. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 302
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
    dependsOn: [
      premiumStorages
    ]
  }
  dependsOn: [
    
  ]
}]

var expressionInPropertyLoopVar = true
//@[004:031) Variable expressionInPropertyLoopVar. Type: true. Declaration start char: 0, length: 38
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[009:038) Resource expressionsInPropertyLoopName. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 232
  name: 'hello'
  location: 'eastus'
  properties: {
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[068:073) Local thing. Type: never. Declaration start char: 68, length: 5
  }
}

// resource loop body that isn't an object
@batchSize(-1)
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[082:087) Local thing. Type: never. Declaration start char: 82, length: 5
//@[009:034) Resource nonObjectResourceLoopBody. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 118
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[083:088) Local thing. Type: never. Declaration start char: 83, length: 5
//@[009:035) Resource nonObjectResourceLoopBody2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 110
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[084:089) Local thing. Type: never. Declaration start char: 84, length: 5
//@[090:091) Local i. Type: int. Declaration start char: 90, length: 1
//@[009:035) Resource nonObjectResourceLoopBody3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 107
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[084:089) Local thing. Type: never. Declaration start char: 84, length: 5
//@[090:091) Local i. Type: int. Declaration start char: 90, length: 1
//@[009:035) Resource nonObjectResourceLoopBody4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 114
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[084:089) Local thing. Type: never. Declaration start char: 84, length: 5
//@[090:091) Local i. Type: int. Declaration start char: 90, length: 1
//@[009:035) Resource nonObjectResourceLoopBody3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 116
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[084:089) Local thing. Type: never. Declaration start char: 84, length: 5
//@[090:091) Local i. Type: int. Declaration start char: 90, length: 1
//@[009:035) Resource nonObjectResourceLoopBody4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 123

// #completionTest(54,55) -> objectPlusFor
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[009:012) Resource foo. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 55

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[060:064) Local item. Type: never. Declaration start char: 60, length: 4
//@[009:012) Resource foo. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 257
  properties: {
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
    registrationVirtualNetworks: 
    resolutionVirtualNetworks: [for lol in []: {
//@[036:039) Local lol. Type: never. Declaration start char: 36, length: 3
      
    }]
  }
}]

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[009:013) Resource vnet. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 325
  properties: {
    virtualNetworkPeerings: [for item in []: {
//@[033:037) Local item. Type: never. Declaration start char: 33, length: 4
        properties: {
          remoteAddressSpace: {
            // #completionTest(28,29) -> symbolsPlusArrayWithoutFor
            addressPrefixes: 
          }
        }
    }]
  }
}

// parent property with 'existing' resource at different scope
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[009:016) Resource p1_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 110
  scope: subscription()
  name: 'res1'
}

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[009:018) Resource p1_child1. Type: Microsoft.Rp1/resource1/child1@2020-06-01. Declaration start char: 0, length: 106
  parent: p1_res1
  name: 'child1'
}

// parent property with scope on child resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[009:016) Resource p2_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[009:016) Resource p2_res2. Type: Microsoft.Rp2/resource2@2020-06-01. Declaration start char: 0, length: 76
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[009:021) Resource p2_res2child. Type: Microsoft.Rp2/resource2/child2@2020-06-01. Declaration start char: 0, length: 127
  scope: p2_res1
  parent: p2_res2
  name: 'child2'
}

// parent property self-cycle
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[009:017) Resource p3_vmExt. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 124
  parent: p3_vmExt
  location: 'eastus'
}

// parent property 2-cycle
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[009:014) Resource p4_vm. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 110
  parent: p4_vmExt
  location: 'eastus'
}

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[009:017) Resource p4_vmExt. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 121
  parent: p4_vm
  location: 'eastus'
}

// parent property with invalid child
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[009:016) Resource p5_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[009:016) Resource p5_res2. Type: Microsoft.Rp2/resource2/child2@2020-06-01. Declaration start char: 0, length: 102
  parent: p5_res1
  name: 'res2'
}

// parent property with invalid parent
resource p6_res1 '${true}' = {
//@[009:016) Resource p6_res1. Type: error. Declaration start char: 0, length: 49
  name: 'res1'
}

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[009:016) Resource p6_res2. Type: Microsoft.Rp1/resource1/child2@2020-06-01. Declaration start char: 0, length: 102
  parent: p6_res1
  name: 'res2'
}

// parent property with incorrectly-formatted name
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[009:016) Resource p7_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[009:016) Resource p7_res2. Type: Microsoft.Rp1/resource1/child2@2020-06-01. Declaration start char: 0, length: 107
  parent: p7_res1
  name: 'res1/res2'
}

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[009:016) Resource p7_res3. Type: Microsoft.Rp1/resource1/child2@2020-06-01. Declaration start char: 0, length: 118
  parent: p7_res1
  name: '${p7_res1.name}/res2'
}

// top-level resource with too many '/' characters
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[009:016) Resource p8_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 81
  name: 'res1/res2'
}

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
//@[009:028) Resource existingResProperty. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 166
  name: 'existingResProperty'
  location: 'westeurope'
  properties: {}
}

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[009:035) Resource invalidExistingLocationRef. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 196
    parent: existingResProperty
    name: 'myExt'
    location: existingResProperty.location
}

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[009:027) Resource anyTypeInDependsOn. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 259
  name: 'anyTypeInDependsOn'
  location: resourceGroup().location
  dependsOn: [
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
    's'
    any(true)
  ]
}

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[009:024) Resource anyTypeInParent. Type: Microsoft.Network/dnsZones/CNAME@2018-05-01. Declaration start char: 0, length: 98
  parent: any(true)
}

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[082:087) Local thing. Type: never. Declaration start char: 82, length: 5
//@[009:028) Resource anyTypeInParentLoop. Type: Microsoft.Network/dnsZones/CNAME@2018-05-01[]. Declaration start char: 0, length: 121
  parent: any(true)
}]

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[009:023) Resource anyTypeInScope. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 115
  scope: any(invalidExistingLocationRef)
}

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[009:034) Resource anyTypeInScopeConditional. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 135
  scope: any(invalidExistingLocationRef)
}

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[009:031) Resource anyTypeInExistingScope. Type: Microsoft.Network/dnsZones/AAAA@2018-05-01. Declaration start char: 0, length: 132
  parent: any('')
  scope: any(false)
}

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[097:102) Local thing. Type: never. Declaration start char: 97, length: 5
//@[009:035) Resource anyTypeInExistingScopeLoop. Type: Microsoft.Network/dnsZones/AAAA@2018-05-01[]. Declaration start char: 0, length: 155
  parent: any('')
  scope: any(false)
}]

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[009:035) Resource tenantLevelResourceBlocked. Type: Microsoft.Management/managementGroups@2020-05-01. Declaration start char: 0, length: 131
  name: 'tenantLevelResourceBlocked'
}

// #completionTest(15,36,37) -> resourceTypes
resource comp1 'Microsoft.Resources/'
//@[009:014) Resource comp1. Type: error. Declaration start char: 0, length: 37

// #completionTest(15,16,17) -> resourceTypes
resource comp2 ''
//@[009:014) Resource comp2. Type: error. Declaration start char: 0, length: 17

// #completionTest(38) -> resourceTypes
resource comp3 'Microsoft.Resources/t'
//@[009:014) Resource comp3. Type: error. Declaration start char: 0, length: 38

// #completionTest(40) -> resourceTypes
resource comp4 'Microsoft.Resources/t/v'
//@[009:014) Resource comp4. Type: error. Declaration start char: 0, length: 40

// #completionTest(49) -> resourceTypes
resource comp5 'Microsoft.Storage/storageAccounts'
//@[009:014) Resource comp5. Type: error. Declaration start char: 0, length: 50

// #completionTest(50) -> storageAccountsResourceTypes
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[009:014) Resource comp6. Type: error. Declaration start char: 0, length: 51

// #completionTest(52) -> templateSpecsResourceTypes
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[009:014) Resource comp7. Type: error. Declaration start char: 0, length: 53

// #completionTest(60,61) -> virtualNetworksResourceTypes
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[009:014) Resource comp8. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 61


// issue #3000
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
//@[009:027) Resource issue3000LogicApp1. Type: Microsoft.Logic/workflows@2019-05-01. Declaration start char: 0, length: 453
  name: 'issue3000LogicApp1'
  location: resourceGroup().location
  properties: {
    state: 'Enabled'
    definition: ''
  }
  identity: {
    type: 'SystemAssigned'
  }
  extendedLocation: {}
  sku: {}
  kind: 'V1'
  managedBy: 'string'
  mangedByExtended: [
   'str1'
   'str2'
  ]
  zones: [
   'str1'
   'str2'
  ]
  plan: {}
  eTag: ''
  scale: {}  
}

resource issue3000LogicApp2 'Microsoft.Logic/workflows@2019-05-01' = {
//@[009:027) Resource issue3000LogicApp2. Type: Microsoft.Logic/workflows@2019-05-01. Declaration start char: 0, length: 452
  name: 'issue3000LogicApp2'
  location: resourceGroup().location
  properties: {
    state: 'Enabled'
    definition: ''
  }
  identity: 'SystemAssigned'
  extendedLocation: 'eastus'
  sku: 'Basic'
  kind: {
    name: 'V1'
  }
  managedBy: {}
  mangedByExtended: [
   {}
   {}
  ]
  zones: [
   {}
   {}
  ]
  plan: ''
  eTag: {}
  scale: [
  {}
  ]  
}

resource issue3000stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
//@[009:021) Resource issue3000stg. Type: Microsoft.Storage/storageAccounts@2021-04-01. Declaration start char: 0, length: 234
  name: 'issue3000stg'
  kind: 'StorageV2'
  location: 'West US'
  sku: {
    name: 'Premium_LRS'    
  }
  madeUpProperty: {}
  managedByExtended: []
}

var issue3000stgMadeUpProperty = issue3000stg.madeUpProperty
//@[004:030) Variable issue3000stgMadeUpProperty. Type: error. Declaration start char: 0, length: 60
var issue3000stgManagedBy = issue3000stg.managedBy
//@[004:025) Variable issue3000stgManagedBy. Type: string. Declaration start char: 0, length: 50
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[004:033) Variable issue3000stgManagedByExtended. Type: (never)[]. Declaration start char: 0, length: 66

param dataCollectionRule object
//@[006:024) Parameter dataCollectionRule. Type: object. Declaration start char: 0, length: 31
param tags object
//@[006:010) Parameter tags. Type: object. Declaration start char: 0, length: 17

var defaultLogAnalyticsWorkspace = {
//@[004:032) Variable defaultLogAnalyticsWorkspace. Type: object. Declaration start char: 0, length: 88
  subscriptionId: subscription().subscriptionId
}

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[102:123) Local logAnalyticsWorkspace. Type: any. Declaration start char: 102, length: 21
//@[009:031) Resource logAnalyticsWorkspaces. Type: Microsoft.OperationalInsights/workspaces@2020-10-01[]. Declaration start char: 0, length: 364
  name: logAnalyticsWorkspace.name
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
}]

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[009:030) Resource dataCollectionRuleRes. Type: Microsoft.Insights/dataCollectionRules@2021-04-01. Declaration start char: 0, length: 837
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
//@[026:047) Local logAnalyticsWorkspace. Type: any. Declaration start char: 26, length: 21
//@[049:050) Local i. Type: int. Declaration start char: 49, length: 1
        name: logAnalyticsWorkspace.destinationName
        workspaceResourceId: logAnalyticsWorkspaces[i].id
      }]
    })
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}

resource dataCollectionRuleRes2 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[009:031) Resource dataCollectionRuleRes2. Type: Microsoft.Insights/dataCollectionRules@2021-04-01. Declaration start char: 0, length: 445
  name: dataCollectionRule.name
  location: dataCollectionRule.location
  tags: tags
  kind: dataCollectionRule.kind
  properties: {
    description: dataCollectionRule.description
    destinations: empty([]) ? [for x in []: {}] : [for x in []: {}]
//@[035:036) Local x. Type: never. Declaration start char: 35, length: 1
//@[055:056) Local x. Type: never. Declaration start char: 55, length: 1
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
//@[006:020) Parameter issue4668_kind. Type: 'AzureCLI' | 'AzurePowerShell'. Declaration start char: 0, length: 176
@description('The identity that will be used to execute the Deployment Script.')
param issue4668_identity object
//@[006:024) Parameter issue4668_identity. Type: object. Declaration start char: 0, length: 113
@description('The properties of the Deployment Script.')
param issue4668_properties object
//@[006:026) Parameter issue4668_properties. Type: object. Declaration start char: 0, length: 91
resource issue4668_mainResource 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[009:031) Resource issue4668_mainResource. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 229
  name: 'testscript'
  location: 'westeurope'
  kind: issue4668_kind
  identity: issue4668_identity
  properties: issue4668_properties
}

// https://github.com/Azure/bicep/issues/8516
resource storage 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
//@[009:016) Resource storage. Type: Microsoft.Storage/storageAccounts@2022-05-01. Declaration start char: 0, length: 157
  resource blobServices 'blobServices' existing = {
//@[011:023) Resource blobServices. Type: Microsoft.Storage/storageAccounts/blobServices@2022-05-01. Declaration start char: 2, length: 74
    name: $account
  }
}

