[Jonny]
For extracting a parameter, where does the type come from? I think there are at least a few scenarios that might need to get treated differently:

Extracted value is in a var statement and has no declared type: the type will be based on the value. You might get recursive types or unions if the value contains a reference to a parameter, but you can pull the type clause from the parameter declaration.
var blah1 = [{ foo: 'bar' }, { foo: 'baz' }]

Extracted value is in a param statement (or something else with an explicit type declaration): you may be able to use the declared type syntax of the enclosing statement rather than working from the type backwards to a declaration.
param p1 { intVal: int }
param p2 object = p1
param newParameter {} = p2
var v1 = newParameter

Extracted value is in a resource body: definite possibility of complex structures, recursion, and a few type constructs that aren't fully expressible in Bicep syntax (e.g., "open" enums like 'foo' | 'bar' | string). Resource-derived types might be a good solution here, but they're still behind a feature flag

param p1 { intVal: int } = { intVal: 123 }

type TFoo = {
  property: TFoo?
}
param pfoo TFoo

// EXPECTED: param newParameter { property: TFoo? } = pfoo
// ACTUAL: param newParameter { property: TFoo? } = pfoo

// [Anthony]
// If pulling from a var - do we have any existing logic or heuristic to do this? There may be different expectations - for example, it wouldn't be particularly useful to convert:
//
// var foo = { intVal: 2 }
// to:
// param foo { intVal: 2}

// more likely the user would instead expect:
// param foo { intVal: int }

var foo = { intVal: 2 }

// var blah = [{foo: 'bar'}, {foo: 'baz'}]
// I would expect the user to more likely want:
// param blah {foo: string }[]
// rather than:
// param blah {foo: 'bar' | 'baz'}[]
// or:
// param blah ({foo: 'bar'}|{foo: 'baz'})[]

var blah = [{ foo: 'bar' }, { foo: 'baz' }]

// EXPECTED:
// param newParameter2 [{foo: string}, {foo: string}] = [{foo: 'bar'}, {foo: 'baz'}]
// var blah = newParameter2

// Entire properties
//   - any
//   - Subresource arrays
param properties {
  autoUpgradeMinorVersion: bool
  forceUpdateTag: string
  instanceView: {
    name: string
    statuses: InstanceViewStatus[]
    substatuses: InstanceViewStatus[]
    type: string
    typeHandlerVersion: string
  }
  protectedSettings: any
  provisioningState: string
  publisher: string
  settings: any
  type: string
  typeHandlerVersion: string
} = {
  // Entire properties object selected
  publisher: 'Microsoft.Compute'
  type: 'CustomScriptExtension'
  typeHandlerVersion: '1.8'
  autoUpgradeMinorVersion: true
  settings: {
    fileUris: [
      uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
    ]
    commandToExecute: 'commandToExecute'
  }
}
resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = {
  name: 'cse/windows'
  location: 'location'
  properties: properties
}

type superComplexType = {
  p: string
  i: 123 | 456
}



param p { *: superComplexType } = {
  a: { p: 'mystring', i: 123 } // <-- want to extract this value as param
}


param super superComplexType
var v = super

param pp { *: superComplexType } = {
  a: { p: 'mystring', i: 123 }
}
