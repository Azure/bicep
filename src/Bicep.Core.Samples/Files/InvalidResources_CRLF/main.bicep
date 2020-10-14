
// wrong declaration
bad

// incomplete
resource 
resource foo
resource fo/o
resource foo 'ddd'
resource foo 'ddd'=

// wrong resource type
resource foo 'ddd'={
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
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

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
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
  // #completionTest(0,1,2) -> discriminatorProperty
  
}

resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzureCLI'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
    
  }
}

resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzurePowerShell'
  // #completionTest(0,1,2) -> deploymentScriptTopLevel

  properties: {
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
    
  }
}