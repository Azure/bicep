resource /*${1:lock}*/lock 'Microsoft.Authorization/locks@2017-04-01' = {
  name: /*${2:'name'}*/'name'
  properties: {
    level: /*${3|'NotSpecified','CanNotDelete','ReadOnly'|}*/'NotSpecified'
  }
}
