// Resource with defaults
resource ${1:Identifier} 'Microsoft.${2:Provider/Type@Version}' = {
  name: $3
  location: $4
  properties: {
    $0
  }
}