resource /*${1:resourceGroupLock}*/resourceGroupLock 'Microsoft.Authorization/locks@2020-05-01' = {
  name: /*${2:'name'}*/'name'
  properties: {
    level: /*${3|'NotSpecified','CanNotDelete','ReadOnly'|}*/'NotSpecified'
  }
}
