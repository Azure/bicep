resource lock 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'exampleLock'
  properties: {
    level: 'ReadOnly'
    notes: 'A read only lock'
  }
}
