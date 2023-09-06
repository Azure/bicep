param optionalString string = 'abc'
param optionalInt int = 42
param optionalObj object = {
  a: 'b'
}
param optionalArray array = [
  1
  2
  3
]

output outputObj object = {
  optionalString: optionalString
  optionalInt: optionalInt
  optionalObj: optionalObj
  optionalArray: optionalArray
}