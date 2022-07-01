//me
using './main.bicep'

param location = 'westus'
param storageAccountName = 'my-account'

//cathy
para myint = [
  1
  2
  3
]

param myComplexObj = {
  enabled: true
  name: 'complex object!'
  priority: 3
  data: {
      a: 'b'
      c: [
          'd'
          'e'
      ]
  }
}
//single line comment

/*
multi line comment
*/