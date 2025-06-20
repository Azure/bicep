var value = {
  obj: {
    property: 'value'
    deeply: {
      nested: {
        object: {
          property: 'value'
        }
        arrayOfObjects: [
          {
            property: 'value'
          }
          {}
        ]
      }
    }
  }
  array: [
    'foo'
    'bar'
  ]
}
var standardPropertyAccess = [
//@[4:26) [no-unused-vars (Warning)] Variable "standardPropertyAccess" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |standardPropertyAccess|
  value.obj.property
  value.array[1]
  value.obj.deeply.nested.object.property
  value.obj.deeply.nested.arrayOfObjects[0].property
]
var safeDereferences = [
//@[4:20) [no-unused-vars (Warning)] Variable "safeDereferences" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |safeDereferences|
  value.obj.?property
  value.?obj.property
  value.array[?1]
  value.?array[1]
  value.obj.deeply.?nested.arrayOfObjects[0].property
]
var fromEnd = [
//@[4:11) [no-unused-vars (Warning)] Variable "fromEnd" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |fromEnd|
  value.array[^1]
  value.array[?^1]
  value.?array[^1]
  value.obj.deeply.?nested.arrayOfObjects[^2].property
  (value.obj.deeply.?nested.arrayOfObjects[^2]).property
]

