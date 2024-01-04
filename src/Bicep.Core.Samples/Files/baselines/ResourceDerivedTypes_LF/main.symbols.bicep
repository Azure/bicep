type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[5:08) TypeAlias foo. Type: Type<Microsoft.Storage/storageAccounts>. Declaration start char: 0, length: 67

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[6:09) Parameter bar. Type: Microsoft.Resources/tags. Declaration start char: 0, length: 160
  name: 'default'
  properties: {
    tags: {
      fizz: 'buzz'
      snap: 'crackle'
    }
  }
}

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[7:10) Output baz. Type: Microsoft.ManagedIdentity/userAssignedIdentities. Declaration start char: 0, length: 124
  name: 'myId'
  location: 'eastus'
}

