resource foo 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: 'foo'
  location: 'sadf'
  // ensure brace matching works correctly with single-line object containing an interpolated string
  sku: { name: 's${23}df' }
  kind: 'asdf'
}
