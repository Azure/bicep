using './main.bicep'

param para1 = 'value
//@[6:11) ParameterAssignment para1. Type: error. Declaration start char: 0, length: 20

para

para2

param expr = 1 + 2
//@[6:10) ParameterAssignment expr. Type: 3. Declaration start char: 0, length: 18

param interp = 'abc${123}def'
//@[6:12) ParameterAssignment interp. Type: 'abc123def'. Declaration start char: 0, length: 29

param doubleinterp = 'abc${interp + 'blah'}def'
//@[6:18) ParameterAssignment doubleinterp. Type: error. Declaration start char: 0, length: 47

param objWithExpressions = {
//@[6:24) ParameterAssignment objWithExpressions. Type: object. Declaration start char: 0, length: 91
  foo: 1 + 2
  bar: {
    baz: concat('abc', 'def')
  }
}

param arrayWithExpressions = [1 + 1, 'ok']
//@[6:26) ParameterAssignment arrayWithExpressions. Type: [2, 'ok']. Declaration start char: 0, length: 42
