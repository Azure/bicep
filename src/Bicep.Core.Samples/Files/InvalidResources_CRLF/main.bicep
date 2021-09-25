
// wrong declaration
bad

// incomplete #completionTest(9) -> empty
resource 
resource foo
resource fo/o
resource foo 'ddd'

// #completionTest(23) -> resourceTypes
resource trailingSpace  

// #completionTest(19,20) -> resourceObject
resource foo 'ddd'= 

// wrong resource type
resource foo 'ddd'={
}

resource foo 'ddd'=if (1 + 1 == 2) {
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
}

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
}

// simulate typing if condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)

// missing condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
  name: 'foo'
}

// empty condition
// #completionTest(56) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
  name: 'foo'
}

// #completionTest(57, 59) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
  name: 'foo'
}

// invalid condition type
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
  name: 'foo'
}

// runtime functions are no allowed in resource conditions
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
  name: 'foo'
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
  name: 'foo'
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
  name: 'foo'
  name: true
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
  name: 'foo'
  'name': true
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
  name: 'foo'
  properties: {
    foo: 'a'
    foo: 'a'
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
  name: 'foo'
  properties: {
    foo: 'a'
    'foo': 'a'
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
  name: 'foo'
  location: [
  ]
  tags: 'tag are not a string?'
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
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
  // #completionTest(7,8) -> empty
  kind  
}

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8) -> empty
  kind  :
}

// unsupported resource ref
var resrefvar = bar.name

param resrefpar string = foo.id

output resrefout bool = bar.id

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  id: 2
  type: 'hello'
  apiVersion: true
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    baz.id
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    'hello'
    true
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
}

var interpVal = 'abc'
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
  '${undefinedSymbol}': true
}

module validModule './module.bicep' = {
  name: 'storageDeploy'
  params: {
    name: 'contoso'
  }
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

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes1.name}_v1'
}

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(validModule['name'], 'v1')
}

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${validModule.name}_v1'
}

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.location
}

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['location']
}

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: runtimeValidRes1.properties.evictionPolicy
  kind:'AzureCLI'
  location: 'eastus'
  properties: {
    azCliVersion: '2.0'
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
  }
}

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties'].evictionPolicy
}

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1['properties']['evictionPolicy']
}

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes1.properties['evictionPolicy']
}

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2.properties.azCliVersion
}

var magicString1 = 'location'
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2['${magicString1}']
}

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
var magicString2 = 'name'
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeValidRes2['${magicString2}']
}

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes3.location}'
}

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: validModule.params['name']
}

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
}

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
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
}

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo2
}

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo3.properties.azCliVersion
}

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: runtimeInvalid.foo4
}

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
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
  name: 'test'
  location: 'test'
}]

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
var runtimeCheckVar2 = runtimeCheckVar

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: runtimeCheckVar2
  location: 'test'
}

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
  name: runtimeCheckVar2
  location: 'test'
}]

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
  name: loopForRuntimeCheck[0].properties.zoneType
  location: 'test'
}]

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
var varForRuntimeCheck4b = varForRuntimeCheck4a
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
  name: varForRuntimeCheck4b
  location: 'test'
}]

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  // #completionTest(0, 1, 2) -> topLevelProperties
  
}

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
  name: 'me'
  // do not remove whitespace before the closing curly
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
  
}

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
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
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (conditional)
*/
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

/*
Discriminator key missing (loop)
*/
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key missing (filtered loop)
*/
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
  // #completionTest(0,1,2) -> discriminatorProperty
  
}]

/*
Discriminator key value missing with property access
*/
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
  kind:   
}
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
// #completionTest(76) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]

/*
Discriminator key value missing with property access (conditional)
*/
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
  kind:   
}
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
// #completionTest(82) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]

/*
Discriminator key value missing with property access (loops)
*/
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
  kind:   
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind

// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
// #completionTest(87) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]

/*
Discriminator key value missing with property access (filtered loops)
*/
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
  kind:   
}]

// cannot . access properties of a resource loop
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind

// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
// #completionTest(93) -> missingDiscriminatorPropertyAccess
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]

/*
Discriminator value set 1
*/
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
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

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]

/*
Discriminator value set 1 (conditional)
*/
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
// #completionTest(81) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]

/*
Discriminator value set 1 (loop)
*/
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(86) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
// #completionTest(94) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]

/*
Discriminator value set 1 (filtered loop)
*/
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}]
// #completionTest(92) -> cliPropertyAccess
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
// #completionTest(100) -> cliPropertyAccess
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]


/*
Discriminator value set 2
*/
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
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

// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
// #completionTest(90) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].

/*
Discriminator value set 2 (conditional)
*/
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
// #completionTest(81) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.

// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
// #completionTest(96) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].


/*
Discriminator value set 2 (loops)
*/
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
// #completionTest(86) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.

// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
// #completionTest(101) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].


/*
Discriminator value set 2 (filtered loops)
*/
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
  
  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}]
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
// #completionTest(92) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.

// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
// #completionTest(107) -> powershellPropertyAccess
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'

  propertes: {
  }
}

var mock = incorrectPropertiesKey.p

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

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
resource startedTypingTypeWithQuotes 'virma'

// #completionTest(40,41,42,43,44,45) -> resourceTypes
resource startedTypingTypeWithoutQuotes virma

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
}
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.

/* 
Nested discriminator missing key
*/
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

// #completionTest(94) -> createModeIndexPlusSymbols
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']

/* 
Nested discriminator missing key (conditional)
*/
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}
// #completionTest(96) -> createMode
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
// #completionTest(98) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].

// #completionTest(100) -> createModeIndexPlusSymbols_if
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']

/* 
Nested discriminator missing key (loop)
*/
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}]
// #completionTest(101) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
// #completionTest(103) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].

// #completionTest(105) -> createModeIndexPlusSymbols_for
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']


/* 
Nested discriminator missing key (filtered loop)
*/
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
  name: 'test'
  location: 'l'
  properties: {
    //createMode: 'Default'

  }
}]
// #completionTest(107) -> createMode
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
// #completionTest(109) -> createMode
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']


/*
Nested discriminator
*/
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
// #completionTest(72) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].

// #completionTest(79) -> defaultCreateModeIndexes
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]

/*
Nested discriminator (conditional)
*/
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
// #completionTest(79) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
// #completionTest(75) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
// #completionTest(78) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].

// #completionTest(85) -> defaultCreateModeIndexes_if
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]


/*
Nested discriminator (loop)
*/
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}]
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
// #completionTest(84) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
// #completionTest(80) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
// #completionTest(83) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].

// #completionTest(90) -> defaultCreateModeIndexes_for
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]


/*
Nested discriminator (filtered loop)
*/
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
  name: 'test'
  location: 'l'
  properties: {
    createMode: 'Default'

  }
}]
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
// #completionTest(90) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
// #completionTest(86) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
// #completionTest(89) -> defaultCreateModeProperties
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].

// #completionTest(96) -> defaultCreateModeIndexes_for_if
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]



// sample resource to validate completions on the next declarations
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
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

/*
  boolean property value completions
*/ 
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  properties: {
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
    autoUpgradeMinorVersion: t
  }
}

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
  name: 'selfScope'
  scope: selfScope
}

var notAResource = {
  im: 'not'
  a: 'resource!'
}
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
  name: 'invalidScope'
  scope: notAResource
}

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
  name: 'invalidScope2'
  scope: resourceGroup()
}

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
  name: 'invalidScope3'
  scope: subscription()
}

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'invalidDuplicateName'
}
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'invalidDuplicateName'
}
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
  name: 'invalidDuplicateName'
}

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
}

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
  name: 'invalidExtensionResourceDuplicateName'
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
  name: 'invalidExtensionResourceDuplicateName'
  scope: validResourceForInvalidExtensionResourceDuplicateName
}

@concat('foo', 'bar')
@secure()
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
  name: 'invalidDecorator'
}

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
  name: 'cyclicRes'
  scope: cyclicRes
}

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
  name: 'cyclicExistingRes'
  scope: cyclicExistingRes
}

// loop parsing cases
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]

// loop index parsing cases
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]

// loop filter parsing cases
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]

// wrong body type
var emptyArray = []
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]

// duplicate variable in the same scope
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
}]

// errors in the array expression
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
}]
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
}]

// wrong array type
var notAnArray = true
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
}]
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
}]

// wrong filter expression type
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
}]
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
}]

// missing required properties
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
}]
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
}]

// fewer missing required properties and a wrong property
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
  name: account
  location: 'eastus42'
  properties: {
    wrong: 'test'
  }
}]

// wrong property inside the nested property loop
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      doesNotExist: 'test'
      name: 'subnet-${i}-${j}'
    }]
  }
}]
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: [for j in range(0, 4): {
      doesNotExist: 'test'
      name: 'subnet-${i}-${j}-${k}'
    }]
  }
}]

// nonexistent arrays and loop variables
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
  name: 'vnet-${justPlainWrong}'
  properties: {
    subnets: [for j in alsoNotAThing: {
      doesNotExist: 'test'
      name: 'subnet-${fake}-${totallyFake}'
    }]
  }
}]

// property loops cannot be nested
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
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
      }]
    }
  }
}]
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
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
      }]
    }
  }
}]

// property loops cannot be nested (even more nesting)
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
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
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
          fake: [for something in []: true]
        }]
      }]
    }
  }
}]

// loops cannot be used inside of expressions
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
  name: account.name
  location: account.location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    networkAcls: {
      virtualNetworkRules: concat([for lol in []: {
        id: '${account.name}-${account.location}'
      }])
    }
  }
}]

// using the same loop variable in a new language scope should be allowed
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
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
output directRefViaOutput array = union(premiumStorages, stuffs)

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone2'
  location: 'global'
  properties: {
    registrationVirtualNetworks: premiumStorages
  }
}

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
  name: 'myZone3'
  location: 'global'
  properties: {
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
  }
}

@batchSize()
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
  name: 'vnet-${i}'
  properties: {
    subnets: premiumStorages
  }
}]

@batchSize(0)
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
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
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'hello'
  location: 'eastus'
  properties: {
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
  }
}

// resource loop body that isn't an object
@batchSize(-1)
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]

// #completionTest(54,55) -> objectPlusFor
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
  properties: {
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
    registrationVirtualNetworks: 
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
          }
        }
    }]
  }
}

// parent property with 'existing' resource at different scope
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: subscription()
  name: 'res1'
}

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
  parent: p1_res1
  name: 'child1'
}

// parent property with scope on child resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
  scope: p2_res1
  parent: p2_res2
  name: 'child2'
}

// parent property self-cycle
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  parent: p3_vmExt
  location: 'eastus'
}

// parent property 2-cycle
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  parent: p4_vmExt
  location: 'eastus'
}

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
  parent: p4_vm
  location: 'eastus'
}

// parent property with invalid child
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'
}

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
  parent: p5_res1
  name: 'res2'
}

// parent property with invalid parent
resource p6_res1 '${true}' = {
  name: 'res1'
}

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
  parent: p6_res1
  name: 'res2'
}

// parent property with incorrectly-formatted name
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1'
}

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
  parent: p7_res1
  name: 'res1/res2'
}

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
  parent: p7_res1
  name: '${p7_res1.name}/res2'
}

// top-level resource with too many '/' characters
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
  name: 'res1/res2'
}

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
  name: 'existingResProperty'
  location: 'westeurope'
  properties: {}
}

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
    parent: existingResProperty
    name: 'myExt'
    location: existingResProperty.location
}

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'anyTypeInDependsOn'
  location: resourceGroup().location
  dependsOn: [
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
    's'
    any(true)
  ]
}

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
  parent: any(true)
}

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
  parent: any(true)
}]

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
  scope: any(invalidExistingLocationRef)
}

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
  scope: any(invalidExistingLocationRef)
}

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
  parent: any('')
  scope: any(false)
}

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
  parent: any('')
  scope: any(false)
}]

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
  name: 'tenantLevelResourceBlocked'
}

// #completionTest(15,36,37) -> resourceTypes
resource comp1 'Microsoft.Resources/'

// #completionTest(15,16,17) -> resourceTypes
resource comp2 ''

// #completionTest(38) -> resourceTypes
resource comp3 'Microsoft.Resources/t'

// #completionTest(40) -> resourceTypes
resource comp4 'Microsoft.Resources/t/v'

// #completionTest(49) -> resourceTypes
resource comp5 'Microsoft.Storage/storageAccounts'

// #completionTest(50) -> storageAccountsResourceTypes
resource comp6 'Microsoft.Storage/storageAccounts@'

// #completionTest(52) -> templateSpecsResourceTypes
resource comp7 'Microsoft.Resources/templateSpecs@20'

// #completionTest(60,61) -> virtualNetworksResourceTypes
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'


// issue #3000
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
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
var issue3000stgManagedBy = issue3000stg.managedBy
var issue3000stgManagedByExtended = issue3000stg.managedByExtended

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
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}
