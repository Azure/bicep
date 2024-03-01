param coalesceObjectToTest object = {
  null1: null
  null2: null
  string: 'default'
  int: 1
  object: {
    first: 'default'
  }
  array: [
    1
  ]
}
param insensitiveToTest object = {
  left: 'value'
  right: 'value'
  leftInsensitive: 'valuE'
  rightInsensitive: 'Value'
}

output andExampleOutput bool = (bool('true') && bool('false'))
output orExampleOutput bool = (bool('true') || bool('false'))
output notExampleOutput bool = (!bool('true'))
output coalesceStringOutput string = ((coalesceObjectToTest.null1 ?? coalesceObjectToTest.null2) ?? coalesceObjectToTest.string)
output coalesceIntOutput int = ((coalesceObjectToTest.null1 ?? coalesceObjectToTest.null2) ?? coalesceObjectToTest.int)
output coalesceObjectOutput object = ((coalesceObjectToTest.null1 ?? coalesceObjectToTest.null2) ?? coalesceObjectToTest.object)
output coalesceArrayOutput array = ((coalesceObjectToTest.null1 ?? coalesceObjectToTest.null2) ?? coalesceObjectToTest.array)
output coalesceEmptyOutput bool = empty((coalesceObjectToTest.null1 ?? coalesceObjectToTest.null2))
output emptyFunctionsOutput bool = ((null == json('null')) ? true : false)
//@[45:57) [simplify-json-null (Warning)] Simplify json('null') to null (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-json-null)) |json('null')|
output equalsInsensitiveWithLower bool = (insensitiveToTest.leftInsensitive =~ insensitiveToTest.rightInsensitive)
output notEqualsInsensitiveWithLower bool = (insensitiveToTest.leftInsensitive !~ insensitiveToTest.rightInsensitive)
output notEquals bool = (insensitiveToTest.left != insensitiveToTest.right)
output items_1 array = items(insensitiveToTest)

