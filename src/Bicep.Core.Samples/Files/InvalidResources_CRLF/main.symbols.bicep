
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

// #completionTest(19,20) -> resourceObject
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

// there should be no completions without the colon
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:34) Resource noCompletionsWithoutColon. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 138
  // #completionTest(7,8) -> empty
  kind  
}

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:33) Resource noCompletionsBeforeColon. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 138
  // #completionTest(7,8) -> empty
  kind  :
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
//@[4:15) Variable runtimefoo2. Type: string. Declaration start char: 0, length: 61
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


resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[76:81) Local thing. Type: any. Declaration start char: 76, length: 5
//@[9:28) Resource loopForRuntimeCheck. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 130
  name: 'test'
  location: 'test'
}]

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
//@[4:19) Variable runtimeCheckVar. Type: 'Private' | 'Public'. Declaration start char: 0, length: 64
var runtimeCheckVar2 = runtimeCheckVar
//@[4:20) Variable runtimeCheckVar2. Type: 'Private' | 'Public'. Declaration start char: 0, length: 38

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[9:38) Resource singleResourceForRuntimeCheck. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 131
  name: runtimeCheckVar2
  location: 'test'
}

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[77:82) Local thing. Type: any. Declaration start char: 77, length: 5
//@[9:29) Resource loopForRuntimeCheck2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 141
  name: runtimeCheckVar2
  location: 'test'
}]

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[77:87) Local otherThing. Type: any. Declaration start char: 77, length: 10
//@[9:29) Resource loopForRuntimeCheck3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 172
  name: loopForRuntimeCheck[0].properties.zoneType
  location: 'test'
}]

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
//@[4:24) Variable varForRuntimeCheck4a. Type: 'Private' | 'Public'. Declaration start char: 0, length: 69
var varForRuntimeCheck4b = varForRuntimeCheck4a
//@[4:24) Variable varForRuntimeCheck4b. Type: 'Private' | 'Public'. Declaration start char: 0, length: 47
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[77:87) Local otherThing. Type: any. Declaration start char: 77, length: 10
//@[9:29) Resource loopForRuntimeCheck4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 150
  name: varForRuntimeCheck4b
  location: 'test'
}]

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:34) Resource missingTopLevelProperties. Type: Microsoft.Storage/storageAccounts@2020-08-01-preview. Declaration start char: 0, length: 153
  // #completionTest(0, 1, 2) -> topLevelProperties
  
}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[9:44) Resource missingTopLevelPropertiesExceptName. Type: Microsoft.Storage/storageAccounts@2020-08-01-preview. Declaration start char: 0, length: 305
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[9:23) Resource unfinishedVnet. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 531
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
//@[9:32) Resource discriminatorKeyMissing. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 148
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (conditional)
*/
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[9:35) Resource discriminatorKeyMissing_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 160
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (loop)
*/
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[95:100) Local thing. Type: any. Declaration start char: 95, length: 5
//@[9:36) Resource discriminatorKeyMissing_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 171
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key missing (filtered loop)
*/
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[98:103) Local thing. Type: any. Declaration start char: 98, length: 5
//@[9:39) Resource discriminatorKeyMissing_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 183
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key value missing with property access
*/
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:37) Resource discriminatorKeyValueMissing. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 175
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
  kind:   
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[4:43) Variable discriminatorKeyValueMissingCompletions. Type: any. Declaration start char: 0, length: 76
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[4:44) Variable discriminatorKeyValueMissingCompletions2. Type: any. Declaration start char: 0, length: 76

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[4:44) Variable discriminatorKeyValueMissingCompletions3. Type: error. Declaration start char: 0, length: 77

/*
Discriminator key value missing with property access (conditional)
*/
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
//@[9:40) Resource discriminatorKeyValueMissing_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 191
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
  kind:   
}
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[4:46) Variable discriminatorKeyValueMissingCompletions_if. Type: any. Declaration start char: 0, length: 82
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[4:47) Variable discriminatorKeyValueMissingCompletions2_if. Type: any. Declaration start char: 0, length: 82

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[4:47) Variable discriminatorKeyValueMissingCompletions3_if. Type: error. Declaration start char: 0, length: 83

/*
Discriminator key value missing with property access (loops)
*/
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[100:105) Local thing. Type: any. Declaration start char: 100, length: 5
//@[9:41) Resource discriminatorKeyValueMissing_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 202
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
  kind:   
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[4:35) Variable resourceListIsNotSingleResource. Type: error. Declaration start char: 0, length: 75

// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[4:47) Variable discriminatorKeyValueMissingCompletions_for. Type: any. Declaration start char: 0, length: 87
// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[4:48) Variable discriminatorKeyValueMissingCompletions2_for. Type: any. Declaration start char: 0, length: 87

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[4:48) Variable discriminatorKeyValueMissingCompletions3_for. Type: error. Declaration start char: 0, length: 88

/*
Discriminator key value missing with property access (filtered loops)
*/
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[103:108) Local thing. Type: any. Declaration start char: 103, length: 5
//@[9:44) Resource discriminatorKeyValueMissing_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 217
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
  kind:   
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[4:38) Variable resourceListIsNotSingleResource_if. Type: error. Declaration start char: 0, length: 81

// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[4:50) Variable discriminatorKeyValueMissingCompletions_for_if. Type: any. Declaration start char: 0, length: 93
// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[4:51) Variable discriminatorKeyValueMissingCompletions2_for_if. Type: any. Declaration start char: 0, length: 93

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[4:51) Variable discriminatorKeyValueMissingCompletions3_for_if. Type: error. Declaration start char: 0, length: 94

/*
Discriminator value set 1
*/
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) Resource discriminatorKeySetOne. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 266
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

/*
Discriminator value set 1 (conditional)
*/
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[9:34) Resource discriminatorKeySetOne_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 278
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[4:40) Variable discriminatorKeySetOneCompletions_if. Type: any. Declaration start char: 0, length: 81
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[4:41) Variable discriminatorKeySetOneCompletions2_if. Type: error. Declaration start char: 0, length: 81

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[4:41) Variable discriminatorKeySetOneCompletions3_if. Type: error. Declaration start char: 0, length: 82

/*
Discriminator value set 1 (loop)
*/
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[95:100) Local thing. Type: any. Declaration start char: 95, length: 5
//@[9:35) Resource discriminatorKeySetOne_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 290
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(86) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[4:41) Variable discriminatorKeySetOneCompletions_for. Type: any. Declaration start char: 0, length: 86
// #completionTest(94) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[4:42) Variable discriminatorKeySetOneCompletions2_for. Type: error. Declaration start char: 0, length: 94

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[4:42) Variable discriminatorKeySetOneCompletions3_for. Type: error. Declaration start char: 0, length: 87

/*
Discriminator value set 1 (filtered loop)
*/
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[98:103) Local thing. Type: any. Declaration start char: 98, length: 5
//@[9:38) Resource discriminatorKeySetOne_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 302
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(92) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[4:44) Variable discriminatorKeySetOneCompletions_for_if. Type: any. Declaration start char: 0, length: 92
// #completionTest(100) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[4:45) Variable discriminatorKeySetOneCompletions2_for_if. Type: error. Declaration start char: 0, length: 100

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[4:45) Variable discriminatorKeySetOneCompletions3_for_if. Type: error. Declaration start char: 0, length: 93


/*
Discriminator value set 2
*/
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) Resource discriminatorKeySetTwo. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 272
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

/*
Discriminator value set 2 (conditional)
*/
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:34) Resource discriminatorKeySetTwo_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 275
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[4:40) Variable discriminatorKeySetTwoCompletions_if. Type: any. Declaration start char: 0, length: 81
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[4:41) Variable discriminatorKeySetTwoCompletions2_if. Type: error. Declaration start char: 0, length: 81

// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[4:52) Variable discriminatorKeySetTwoCompletionsArrayIndexer_if. Type: any. Declaration start char: 0, length: 96
// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[4:53) Variable discriminatorKeySetTwoCompletionsArrayIndexer2_if. Type: error. Declaration start char: 0, length: 96


/*
Discriminator value set 2 (loops)
*/
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[94:99) Local thing. Type: any. Declaration start char: 94, length: 5
//@[9:35) Resource discriminatorKeySetTwo_for. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 295
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[4:41) Variable discriminatorKeySetTwoCompletions_for. Type: any. Declaration start char: 0, length: 86
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[4:42) Variable discriminatorKeySetTwoCompletions2_for. Type: error. Declaration start char: 0, length: 86

// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[4:53) Variable discriminatorKeySetTwoCompletionsArrayIndexer_for. Type: any. Declaration start char: 0, length: 101
// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[4:54) Variable discriminatorKeySetTwoCompletionsArrayIndexer2_for. Type: error. Declaration start char: 0, length: 101


/*
Discriminator value set 2 (filtered loops)
*/
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[97:102) Local thing. Type: any. Declaration start char: 97, length: 5
//@[9:38) Resource discriminatorKeySetTwo_for_if. Type: Microsoft.Resources/deploymentScripts@2020-10-01[]. Declaration start char: 0, length: 307
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[4:44) Variable discriminatorKeySetTwoCompletions_for_if. Type: any. Declaration start char: 0, length: 92
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[4:45) Variable discriminatorKeySetTwoCompletions2_for_if. Type: error. Declaration start char: 0, length: 92

// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[4:56) Variable discriminatorKeySetTwoCompletionsArrayIndexer_for_if. Type: any. Declaration start char: 0, length: 107
// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[4:57) Variable discriminatorKeySetTwoCompletionsArrayIndexer2_for_if. Type: error. Declaration start char: 0, length: 107



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:31) Resource incorrectPropertiesKey. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 132
  kind: 'AzureCLI'

  propertes: {
  }
}

var mock = incorrectPropertiesKey.p
//@[4:8) Variable mock. Type: error. Declaration start char: 0, length: 35

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[9:32) Resource incorrectPropertiesKey2. Type: Microsoft.Resources/deploymentScripts@2020-10-01. Declaration start char: 0, length: 796
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
//@[4:24) Variable letsAccessTheDashes2. Type: error. Declaration start char: 0, length: 78

/* 
Nested discriminator missing key
*/
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
//@[4:45) Variable nestedDiscriminatorMissingKeyCompletions2. Type: any. Declaration start char: 0, length: 92

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[4:49) Variable nestedDiscriminatorMissingKeyIndexCompletions. Type: any. Declaration start char: 0, length: 96

/* 
Nested discriminator missing key (conditional)
*/
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
//@[9:41) Resource nestedDiscriminatorMissingKey_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 205
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}
// #completionTest(96) -> createMode
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[4:47) Variable nestedDiscriminatorMissingKeyCompletions_if. Type: any. Declaration start char: 0, length: 96
// #completionTest(98) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[4:48) Variable nestedDiscriminatorMissingKeyCompletions2_if. Type: any. Declaration start char: 0, length: 98

// #completionTest(100) -> createModeIndexPlusSymbols_if
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[4:52) Variable nestedDiscriminatorMissingKeyIndexCompletions_if. Type: any. Declaration start char: 0, length: 102

/* 
Nested discriminator missing key (loop)
*/
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[109:114) Local thing. Type: any. Declaration start char: 109, length: 5
//@[9:42) Resource nestedDiscriminatorMissingKey_for. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 213
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}]
// #completionTest(101) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[4:48) Variable nestedDiscriminatorMissingKeyCompletions_for. Type: any. Declaration start char: 0, length: 101
// #completionTest(103) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[4:49) Variable nestedDiscriminatorMissingKeyCompletions2_for. Type: any. Declaration start char: 0, length: 103

// #completionTest(105) -> createModeIndexPlusSymbols_for
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[4:53) Variable nestedDiscriminatorMissingKeyIndexCompletions_for. Type: any. Declaration start char: 0, length: 107


/* 
Nested discriminator missing key (filtered loop)
*/
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[112:117) Local thing. Type: any. Declaration start char: 112, length: 5
//@[9:45) Resource nestedDiscriminatorMissingKey_for_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 225
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}]
// #completionTest(107) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[4:51) Variable nestedDiscriminatorMissingKeyCompletions_for_if. Type: any. Declaration start char: 0, length: 107
// #completionTest(109) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[4:52) Variable nestedDiscriminatorMissingKeyCompletions2_for_if. Type: any. Declaration start char: 0, length: 109

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[4:56) Variable nestedDiscriminatorMissingKeyIndexCompletions_for_if. Type: any. Declaration start char: 0, length: 113


/*
Nested discriminator
*/
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

/*
Nested discriminator (conditional)
*/
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
//@[9:31) Resource nestedDiscriminator_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview. Declaration start char: 0, length: 190
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[4:37) Variable nestedDiscriminatorCompletions_if. Type: any. Declaration start char: 0, length: 75
// #completionTest(79) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[4:38) Variable nestedDiscriminatorCompletions2_if. Type: any. Declaration start char: 0, length: 79
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[4:38) Variable nestedDiscriminatorCompletions3_if. Type: error. Declaration start char: 0, length: 75
// #completionTest(78) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[4:38) Variable nestedDiscriminatorCompletions4_if. Type: error. Declaration start char: 0, length: 78

// #completionTest(85) -> defaultCreateModeIndexes_if
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[4:47) Variable nestedDiscriminatorArrayIndexCompletions_if. Type: error. Declaration start char: 0, length: 86


/*
Nested discriminator (loop)
*/
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[99:104) Local thing. Type: any. Declaration start char: 99, length: 5
//@[9:32) Resource nestedDiscriminator_for. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 201
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}]
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[4:38) Variable nestedDiscriminatorCompletions_for. Type: any. Declaration start char: 0, length: 80
// #completionTest(84) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[4:39) Variable nestedDiscriminatorCompletions2_for. Type: any. Declaration start char: 0, length: 84
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[4:39) Variable nestedDiscriminatorCompletions3_for. Type: error. Declaration start char: 0, length: 80
// #completionTest(83) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[4:39) Variable nestedDiscriminatorCompletions4_for. Type: error. Declaration start char: 0, length: 83

// #completionTest(90) -> defaultCreateModeIndexes_for
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[4:48) Variable nestedDiscriminatorArrayIndexCompletions_for. Type: error. Declaration start char: 0, length: 91


/*
Nested discriminator (filtered loop)
*/
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[102:107) Local thing. Type: any. Declaration start char: 102, length: 5
//@[9:35) Resource nestedDiscriminator_for_if. Type: Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview[]. Declaration start char: 0, length: 213
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}]
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[4:41) Variable nestedDiscriminatorCompletions_for_if. Type: any. Declaration start char: 0, length: 86
// #completionTest(90) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[4:42) Variable nestedDiscriminatorCompletions2_for_if. Type: any. Declaration start char: 0, length: 90
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[4:42) Variable nestedDiscriminatorCompletions3_for_if. Type: error. Declaration start char: 0, length: 86
// #completionTest(89) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[4:42) Variable nestedDiscriminatorCompletions4_for_if. Type: error. Declaration start char: 0, length: 89

// #completionTest(96) -> defaultCreateModeIndexes_for_if
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[4:51) Variable nestedDiscriminatorArrayIndexCompletions_for_if. Type: error. Declaration start char: 0, length: 97



// sample resource to validate completions on the next declarations
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
//@[9:42) Resource nestedPropertyAccessOnConditional. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 209
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
//@[4:8) Variable sigh. Type: error. Declaration start char: 0, length: 56

/*
  boolean property value completions
*/ 
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[9:36) Resource booleanPropertyPartialValue. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 222
  properties: {
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
    autoUpgradeMinorVersion: t
  }
}

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

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[9:30) Resource invalidDuplicateName1. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 103
  name: 'invalidDuplicateName'
}
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[9:30) Resource invalidDuplicateName2. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 103
  name: 'invalidDuplicateName'
}
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[9:30) Resource invalidDuplicateName3. Type: Mock.Rp/mockResource@2019-01-01. Declaration start char: 0, length: 103
  name: 'invalidDuplicateName'
}

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[9:62) Resource validResourceForInvalidExtensionResourceDuplicateName. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 168
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
}

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[9:47) Resource invalidExtensionResourceDuplicateName1. Type: Mock.Rp/mockExtResource@2020-01-01. Declaration start char: 0, length: 204
  name: 'invalidExtensionResourceDuplicateName'
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[9:47) Resource invalidExtensionResourceDuplicateName2. Type: Mock.Rp/mockExtResource@2019-01-01. Declaration start char: 0, length: 204
  name: 'invalidExtensionResourceDuplicateName'
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

@concat('foo', 'bar')
@secure()
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:25) Resource invalidDecorator. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 131
  name: 'invalidDecorator'
}

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[9:18) Resource cyclicRes. Type: Mock.Rp/mockExistingResource@2020-01-01. Declaration start char: 0, length: 108
  name: 'cyclicRes'
  scope: cyclicRes
}

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[9:26) Resource cyclicExistingRes. Type: Mock.Rp/mockExistingResource@2020-01-01. Declaration start char: 0, length: 141
  name: 'cyclicExistingRes'
  scope: cyclicExistingRes
}

// loop parsing cases
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[9:27) Resource expectedForKeyword. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 79

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[9:28) Resource expectedForKeyword2. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 81

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[9:24) Resource expectedLoopVar. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 79

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[81:82) Local x. Type: any. Declaration start char: 81, length: 1
//@[9:26) Resource expectedInKeyword. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 83

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[82:83) Local x. Type: any. Declaration start char: 82, length: 1
//@[9:27) Resource expectedInKeyword2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 86

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[87:88) Local x. Type: any. Declaration start char: 87, length: 1
//@[9:32) Resource expectedArrayExpression. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 92

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[77:78) Local x. Type: any. Declaration start char: 77, length: 1
//@[9:22) Resource expectedColon. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 84

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[80:81) Local x. Type: any. Declaration start char: 80, length: 1
//@[9:25) Resource expectedLoopBody. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 88

// loop index parsing cases
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[78:78) Local <missing>. Type: any. Declaration start char: 78, length: 0
//@[78:78) Local <missing>. Type: int. Declaration start char: 78, length: 0
//@[9:29) Resource expectedLoopItemName. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 80

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[79:79) Local <missing>. Type: any. Declaration start char: 79, length: 0
//@[79:79) Local <missing>. Type: int. Declaration start char: 79, length: 0
//@[9:30) Resource expectedLoopItemName2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 79

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[71:72) Local x. Type: any. Declaration start char: 71, length: 1
//@[72:72) Local <missing>. Type: int. Declaration start char: 72, length: 0
//@[9:22) Resource expectedComma. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 74

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[79:80) Local x. Type: any. Declaration start char: 79, length: 1
//@[82:82) Local <missing>. Type: int. Declaration start char: 82, length: 0
//@[9:30) Resource expectedLoopIndexName. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 84

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[76:77) Local x. Type: any. Declaration start char: 76, length: 1
//@[79:80) Local y. Type: int. Declaration start char: 79, length: 1
//@[9:27) Resource expectedInKeyword3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 82

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[76:77) Local x. Type: any. Declaration start char: 76, length: 1
//@[79:80) Local y. Type: int. Declaration start char: 79, length: 1
//@[9:27) Resource expectedInKeyword4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 84

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[82:83) Local x. Type: any. Declaration start char: 82, length: 1
//@[85:86) Local y. Type: int. Declaration start char: 85, length: 1
//@[9:33) Resource expectedArrayExpression2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 92

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[72:73) Local x. Type: any. Declaration start char: 72, length: 1
//@[75:76) Local y. Type: int. Declaration start char: 75, length: 1
//@[9:23) Resource expectedColon2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 83

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[75:76) Local x. Type: any. Declaration start char: 75, length: 1
//@[78:79) Local y. Type: int. Declaration start char: 78, length: 1
//@[9:26) Resource expectedLoopBody2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 87

// loop filter parsing cases
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[91:92) Local x. Type: any. Declaration start char: 91, length: 1
//@[9:36) Resource expectedLoopFilterOpenParen. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 102
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[86:87) Local x. Type: any. Declaration start char: 86, length: 1
//@[89:90) Local y. Type: int. Declaration start char: 89, length: 1
//@[9:37) Resource expectedLoopFilterOpenParen2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 101

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[98:99) Local x. Type: any. Declaration start char: 98, length: 1
//@[9:43) Resource expectedLoopFilterPredicateAndBody. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 111
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[93:94) Local x. Type: any. Declaration start char: 93, length: 1
//@[96:97) Local y. Type: int. Declaration start char: 96, length: 1
//@[9:44) Resource expectedLoopFilterPredicateAndBody2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 110

// wrong body type
var emptyArray = []
//@[4:14) Variable emptyArray. Type: array. Declaration start char: 0, length: 19
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[81:82) Local x. Type: any. Declaration start char: 81, length: 1
//@[9:26) Resource wrongLoopBodyType. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 99
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[83:84) Local x. Type: any. Declaration start char: 83, length: 1
//@[86:87) Local i. Type: int. Declaration start char: 86, length: 1
//@[9:27) Resource wrongLoopBodyType2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 105

// duplicate variable in the same scope
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[80:84) Local same. Type: any. Declaration start char: 80, length: 4
//@[86:90) Local same. Type: int. Declaration start char: 86, length: 4
//@[9:29) Resource itemAndIndexSameName. Type: Microsoft.AAD/domainServices@2020-01-01[]. Declaration start char: 0, length: 112
}]

// errors in the array expression
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[85:92) Local account. Type: any. Declaration start char: 85, length: 7
//@[9:30) Resource arrayExpressionErrors. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 115
}]
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[87:94) Local account. Type: any. Declaration start char: 87, length: 7
//@[95:96) Local k. Type: int. Declaration start char: 95, length: 1
//@[9:31) Resource arrayExpressionErrors2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 120
}]

// wrong array type
var notAnArray = true
//@[4:14) Variable notAnArray. Type: bool. Declaration start char: 0, length: 21
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[78:85) Local account. Type: any. Declaration start char: 78, length: 7
//@[9:23) Resource wrongArrayType. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 106
}]
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[80:87) Local account. Type: any. Declaration start char: 80, length: 7
//@[88:89) Local i. Type: int. Declaration start char: 88, length: 1
//@[9:24) Resource wrongArrayType2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 111
}]

// wrong filter expression type
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[89:96) Local account. Type: any. Declaration start char: 89, length: 7
//@[9:34) Resource wrongFilterExpressionType. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 123
}]
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[91:98) Local account. Type: any. Declaration start char: 91, length: 7
//@[99:100) Local i. Type: int. Declaration start char: 99, length: 1
//@[9:35) Resource wrongFilterExpressionType2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 137
}]

// missing required properties
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[89:96) Local account. Type: any. Declaration start char: 89, length: 7
//@[9:34) Resource missingRequiredProperties. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 109
}]
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[91:98) Local account. Type: any. Declaration start char: 91, length: 7
//@[99:100) Local j. Type: int. Declaration start char: 99, length: 1
//@[9:35) Resource missingRequiredProperties2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 114
}]

// fewer missing required properties and a wrong property
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[94:101) Local account. Type: any. Declaration start char: 94, length: 7
//@[9:39) Resource missingFewerRequiredProperties. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 196
  name: account
  location: 'eastus42'
  properties: {
    wrong: 'test'
  }
}]

// wrong property inside the nested property loop
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[89:90) Local i. Type: int. Declaration start char: 89, length: 1
//@[9:34) Resource wrongPropertyInNestedLoop. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 262
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
//@[18:19) Local j. Type: int. Declaration start char: 18, length: 1
      doesNotExist: 'test'
      name: 'subnet-${i}-${j}'
    }]
  }
}]
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
//@[91:92) Local i. Type: int. Declaration start char: 91, length: 1
//@[93:94) Local k. Type: int. Declaration start char: 93, length: 1
//@[9:35) Resource wrongPropertyInNestedLoop2. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 272
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
//@[18:19) Local j. Type: int. Declaration start char: 18, length: 1
      doesNotExist: 'test'
      name: 'subnet-${i}-${j}-${k}'
    }]
  }
}]

// nonexistent arrays and loop variables
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[81:82) Local i. Type: any. Declaration start char: 81, length: 1
//@[9:26) Resource nonexistentArrays. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 280
  name: 'vnet-${justPlainWrong}'
  properties: {
    subnets: [for j in alsoNotAThing: {
//@[18:19) Local j. Type: any. Declaration start char: 18, length: 1
      doesNotExist: 'test'
      name: 'subnet-${fake}-${totallyFake}'
    }]
  }
}]

// property loops cannot be nested
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[87:94) Local account. Type: any. Declaration start char: 87, length: 7
//@[9:32) Resource propertyLoopsCannotNest. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 428
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {

    networkAcls: {
      virtualNetworkRules: [for rule in []: {
//@[32:36) Local rule. Type: any. Declaration start char: 32, length: 4
        id: '${account.name}-${account.location}'
        state: [for lol in []: 4]
//@[20:23) Local lol. Type: any. Declaration start char: 20, length: 3
      }]
    }
  }
}]
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[89:96) Local account. Type: any. Declaration start char: 89, length: 7
//@[97:98) Local i. Type: int. Declaration start char: 97, length: 1
//@[9:33) Resource propertyLoopsCannotNest2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 441
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {

    networkAcls: {
      virtualNetworkRules: [for (rule,j) in []: {
//@[33:37) Local rule. Type: any. Declaration start char: 33, length: 4
//@[38:39) Local j. Type: int. Declaration start char: 38, length: 1
        id: '${account.name}-${account.location}'
        state: [for (lol,k) in []: 4]
//@[21:24) Local lol. Type: any. Declaration start char: 21, length: 3
//@[25:26) Local k. Type: int. Declaration start char: 25, length: 1
      }]
    }
  }
}]

// property loops cannot be nested (even more nesting)
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[88:95) Local account. Type: any. Declaration start char: 88, length: 7
//@[9:33) Resource propertyLoopsCannotNest2. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 634
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls:  {
      virtualNetworkRules: [for rule in []: {
//@[32:36) Local rule. Type: any. Declaration start char: 32, length: 4
        // #completionTest(15,31) -> symbolsPlusRule
        id: '${account.name}-${account.location}'
        state: [for state in []: {
//@[20:25) Local state. Type: any. Declaration start char: 20, length: 5
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
          fake: [for something in []: true]
//@[21:30) Local something. Type: any. Declaration start char: 21, length: 9
        }]
      }]
    }
  }
}]

// loops cannot be used inside of expressions
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[70:77) Local account. Type: any. Declaration start char: 70, length: 7
//@[9:15) Resource stuffs. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 381
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      virtualNetworkRules: concat([for lol in []: {
//@[39:42) Local lol. Type: any. Declaration start char: 39, length: 3
        id: '${account.name}-${account.location}'
      }])
    }
  }
}]

// using the same loop variable in a new language scope should be allowed
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[79:86) Local account. Type: any. Declaration start char: 79, length: 7
//@[9:24) Resource premiumStorages. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 321
  // #completionTest(7,8) -> symbolsPlusAccount2
  name: account.name
  location: account.location
  sku: {
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
    name: 
  }
  kind: 'StorageV2'
}]

var directRefViaVar = premiumStorages
//@[4:19) Variable directRefViaVar. Type: Microsoft.Storage/storageAccounts@2019-06-01[]. Declaration start char: 0, length: 37
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[7:25) Output directRefViaOutput. Type: array. Declaration start char: 0, length: 64

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
//@[9:39) Resource directRefViaSingleResourceBody. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 199
  name: 'myZone2'
  location: 'global'
  properties: {
    registrationVirtualNetworks: premiumStorages
  }
}

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
//@[9:50) Resource directRefViaSingleConditionalResourceBody. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 235
  name: 'myZone3'
  location: 'global'
  properties: {
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
  }
}

@batchSize()
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[98:99) Local i. Type: int. Declaration start char: 98, length: 1
//@[9:43) Resource directRefViaSingleLoopResourceBody. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 208
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
  }
}]

@batchSize(0)
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[116:117) Local i. Type: int. Declaration start char: 116, length: 1
//@[9:61) Resource directRefViaSingleLoopResourceBodyWithExtraDependsOn. Type: Microsoft.Network/virtualNetworks@2020-06-01[]. Declaration start char: 0, length: 302
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
//@[4:31) Variable expressionInPropertyLoopVar. Type: bool. Declaration start char: 0, length: 38
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[9:38) Resource expressionsInPropertyLoopName. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 232
  name: 'hello'
  location: 'eastus'
  properties: {
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[68:73) Local thing. Type: any. Declaration start char: 68, length: 5
  }
}

// resource loop body that isn't an object
@batchSize(-1)
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[82:87) Local thing. Type: any. Declaration start char: 82, length: 5
//@[9:34) Resource nonObjectResourceLoopBody. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 118
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[83:88) Local thing. Type: any. Declaration start char: 83, length: 5
//@[9:35) Resource nonObjectResourceLoopBody2. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 110
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[84:89) Local thing. Type: any. Declaration start char: 84, length: 5
//@[90:91) Local i. Type: int. Declaration start char: 90, length: 1
//@[9:35) Resource nonObjectResourceLoopBody3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 107
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[84:89) Local thing. Type: any. Declaration start char: 84, length: 5
//@[90:91) Local i. Type: int. Declaration start char: 90, length: 1
//@[9:35) Resource nonObjectResourceLoopBody4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 114
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[84:89) Local thing. Type: any. Declaration start char: 84, length: 5
//@[90:91) Local i. Type: int. Declaration start char: 90, length: 1
//@[9:35) Resource nonObjectResourceLoopBody3. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 116
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[84:89) Local thing. Type: any. Declaration start char: 84, length: 5
//@[90:91) Local i. Type: int. Declaration start char: 90, length: 1
//@[9:35) Resource nonObjectResourceLoopBody4. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 123

// #completionTest(54,55) -> objectPlusFor
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[9:12) Resource foo. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 55

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[60:64) Local item. Type: any. Declaration start char: 60, length: 4
//@[9:12) Resource foo. Type: Microsoft.Network/dnsZones@2018-05-01[]. Declaration start char: 0, length: 257
  properties: {
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
    registrationVirtualNetworks: 
    resolutionVirtualNetworks: [for lol in []: {
//@[36:39) Local lol. Type: any. Declaration start char: 36, length: 3
      
    }]
  }
}]

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[9:13) Resource vnet. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 325
  properties: {
    virtualNetworkPeerings: [for item in []: {
//@[33:37) Local item. Type: any. Declaration start char: 33, length: 4
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
//@[9:16) Resource p1_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 110
  scope: subscription()
  name: 'res1'
}

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[9:18) Resource p1_child1. Type: Microsoft.Rp1/resource1/child1@2020-06-01. Declaration start char: 0, length: 106
  parent: p1_res1
  name: 'child1'
}

// parent property with scope on child resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[9:16) Resource p2_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[9:16) Resource p2_res2. Type: Microsoft.Rp2/resource2@2020-06-01. Declaration start char: 0, length: 76
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[9:21) Resource p2_res2child. Type: Microsoft.Rp2/resource2/child2@2020-06-01. Declaration start char: 0, length: 127
  scope: p2_res1
  parent: p2_res2
  name: 'child2'
}

// parent property self-cycle
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[9:17) Resource p3_vmExt. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 124
  parent: p3_vmExt
  location: 'eastus'
}

// parent property 2-cycle
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[9:14) Resource p4_vm. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 110
  parent: p4_vmExt
  location: 'eastus'
}

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[9:17) Resource p4_vmExt. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 121
  parent: p4_vm
  location: 'eastus'
}

// parent property with invalid child
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[9:16) Resource p5_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[9:16) Resource p5_res2. Type: error. Declaration start char: 0, length: 102
  parent: p5_res1
  name: 'res2'
}

// parent property with invalid parent
resource p6_res1 '${true}' = {
//@[9:16) Resource p6_res1. Type: error. Declaration start char: 0, length: 49
  name: 'res1'
}

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[9:16) Resource p6_res2. Type: error. Declaration start char: 0, length: 102
  parent: p6_res1
  name: 'res2'
}

// parent property with incorrectly-formatted name
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[9:16) Resource p7_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 76
  name: 'res1'
}

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[9:16) Resource p7_res2. Type: Microsoft.Rp1/resource1/child2@2020-06-01. Declaration start char: 0, length: 107
  parent: p7_res1
  name: 'res1/res2'
}

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[9:16) Resource p7_res3. Type: Microsoft.Rp1/resource1/child2@2020-06-01. Declaration start char: 0, length: 118
  parent: p7_res1
  name: '${p7_res1.name}/res2'
}

// top-level resource with too many '/' characters
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[9:16) Resource p8_res1. Type: Microsoft.Rp1/resource1@2020-06-01. Declaration start char: 0, length: 81
  name: 'res1/res2'
}

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
//@[9:28) Resource existingResProperty. Type: Microsoft.Compute/virtualMachines@2020-06-01. Declaration start char: 0, length: 166
  name: 'existingResProperty'
  location: 'westeurope'
  properties: {}
}

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[9:35) Resource invalidExistingLocationRef. Type: Microsoft.Compute/virtualMachines/extensions@2020-06-01. Declaration start char: 0, length: 196
    parent: existingResProperty
    name: 'myExt'
    location: existingResProperty.location
}

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[9:27) Resource anyTypeInDependsOn. Type: Microsoft.Network/dnsZones@2018-05-01. Declaration start char: 0, length: 259
  name: 'anyTypeInDependsOn'
  location: resourceGroup().location
  dependsOn: [
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
    's'
    any(true)
  ]
}

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[9:24) Resource anyTypeInParent. Type: Microsoft.Network/dnsZones/CNAME@2018-05-01. Declaration start char: 0, length: 98
  parent: any(true)
}

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[82:87) Local thing. Type: any. Declaration start char: 82, length: 5
//@[9:28) Resource anyTypeInParentLoop. Type: Microsoft.Network/dnsZones/CNAME@2018-05-01[]. Declaration start char: 0, length: 121
  parent: any(true)
}]

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[9:23) Resource anyTypeInScope. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 115
  scope: any(invalidExistingLocationRef)
}

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[9:34) Resource anyTypeInScopeConditional. Type: Microsoft.Authorization/locks@2016-09-01. Declaration start char: 0, length: 135
  scope: any(invalidExistingLocationRef)
}

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[9:31) Resource anyTypeInExistingScope. Type: Microsoft.Network/dnsZones/AAAA@2018-05-01. Declaration start char: 0, length: 132
  parent: any('')
  scope: any(false)
}

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[97:102) Local thing. Type: any. Declaration start char: 97, length: 5
//@[9:35) Resource anyTypeInExistingScopeLoop. Type: Microsoft.Network/dnsZones/AAAA@2018-05-01[]. Declaration start char: 0, length: 155
  parent: any('')
  scope: any(false)
}]

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[9:35) Resource tenantLevelResourceBlocked. Type: Microsoft.Management/managementGroups@2020-05-01. Declaration start char: 0, length: 131
  name: 'tenantLevelResourceBlocked'
}

// #completionTest(15,36,37) -> resourceTypes
resource comp1 'Microsoft.Resources/'
//@[9:14) Resource comp1. Type: error. Declaration start char: 0, length: 37

// #completionTest(15,16,17) -> resourceTypes
resource comp2 ''
//@[9:14) Resource comp2. Type: error. Declaration start char: 0, length: 17

// #completionTest(38) -> resourceTypes
resource comp3 'Microsoft.Resources/t'
//@[9:14) Resource comp3. Type: error. Declaration start char: 0, length: 38

// #completionTest(40) -> resourceTypes
resource comp4 'Microsoft.Resources/t/v'
//@[9:14) Resource comp4. Type: error. Declaration start char: 0, length: 40

// #completionTest(49) -> resourceTypes
resource comp5 'Microsoft.Storage/storageAccounts'
//@[9:14) Resource comp5. Type: error. Declaration start char: 0, length: 50

// #completionTest(50) -> storageAccountsResourceTypes
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[9:14) Resource comp6. Type: error. Declaration start char: 0, length: 51

// #completionTest(52) -> templateSpecsResourceTypes
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[9:14) Resource comp7. Type: error. Declaration start char: 0, length: 53

// #completionTest(60,61) -> virtualNetworksResourceTypes
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[9:14) Resource comp8. Type: Microsoft.Network/virtualNetworks@2020-06-01. Declaration start char: 0, length: 61


// issue #3000
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
//@[9:27) Resource issue3000LogicApp1. Type: Microsoft.Logic/workflows@2019-05-01. Declaration start char: 0, length: 453
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
//@[9:27) Resource issue3000LogicApp2. Type: Microsoft.Logic/workflows@2019-05-01. Declaration start char: 0, length: 452
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
//@[9:21) Resource issue3000stg. Type: Microsoft.Storage/storageAccounts@2021-04-01. Declaration start char: 0, length: 234
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
//@[4:30) Variable issue3000stgMadeUpProperty. Type: error. Declaration start char: 0, length: 60
var issue3000stgManagedBy = issue3000stg.managedBy
//@[4:25) Variable issue3000stgManagedBy. Type: string. Declaration start char: 0, length: 50
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[4:33) Variable issue3000stgManagedByExtended. Type: (never)[]. Declaration start char: 0, length: 66

param dataCollectionRule object
//@[6:24) Parameter dataCollectionRule. Type: object. Declaration start char: 0, length: 31
param tags object
//@[6:10) Parameter tags. Type: object. Declaration start char: 0, length: 17

var defaultLogAnalyticsWorkspace = {
//@[4:32) Variable defaultLogAnalyticsWorkspace. Type: object. Declaration start char: 0, length: 88
  subscriptionId: subscription().subscriptionId
}

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[102:123) Local logAnalyticsWorkspace. Type: any. Declaration start char: 102, length: 21
//@[9:31) Resource logAnalyticsWorkspaces. Type: Microsoft.OperationalInsights/workspaces@2020-10-01[]. Declaration start char: 0, length: 364
  name: logAnalyticsWorkspace.name
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
}]

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[9:30) Resource dataCollectionRuleRes. Type: Microsoft.Insights/dataCollectionRules@2021-04-01. Declaration start char: 0, length: 837
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
//@[26:47) Local logAnalyticsWorkspace. Type: any. Declaration start char: 26, length: 21
//@[49:50) Local i. Type: int. Declaration start char: 49, length: 1
        name: logAnalyticsWorkspace.destinationName
        workspaceResourceId: logAnalyticsWorkspaces[i].id
      }]
    })
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}

resource dataCollectionRuleRes2 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[9:31) Resource dataCollectionRuleRes2. Type: Microsoft.Insights/dataCollectionRules@2021-04-01. Declaration start char: 0, length: 445
  name: dataCollectionRule.name
  location: dataCollectionRule.location
  tags: tags
  kind: dataCollectionRule.kind
  properties: {
    description: dataCollectionRule.description
    destinations: empty([]) ? [for x in []: {}] : [for x in []: {}]
//@[35:36) Local x. Type: any. Declaration start char: 35, length: 1
//@[55:56) Local x. Type: any. Declaration start char: 55, length: 1
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}

