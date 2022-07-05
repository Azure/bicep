resource /*${1:lock}*/lock 'Microsoft.Authorization/locks@2017-04-01' = {
  name: /*${2:'name'}*/'name'
  scope: /*${3:scopeResource}*/scopeResource
  properties: {
    level: /*${4|'NotSpecified','CanNotDelete','ReadOnly'|}*/'NotSpecified'
  }
}
