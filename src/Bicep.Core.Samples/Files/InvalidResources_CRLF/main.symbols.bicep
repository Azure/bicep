
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

// #completionTest(19,20) -> object
resource foo 'ddd'= 
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 20

// wrong resource type
resource foo 'ddd'={
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 23
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 64
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 55
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
