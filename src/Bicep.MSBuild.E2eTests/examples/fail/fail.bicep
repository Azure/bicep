targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  wrong: 'wrong'
}

wrong

resource nonExistent 'Wrong/wrong@2021-03-18' = {
  name: 'wrong'
}
