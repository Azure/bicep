// Child Resource with defaults
resource ${1:Identifier} 'Microsoft.${2:Provider/ParentType/ChildType@Version}' = {
  name: $3
  properties: {
    $0
  }
}