using './main.bicep'

param para1 = 'value

para

para2

param expr = 1 + 2

param interp = 'abc${123}def'

param doubleinterp = 'abc${interp + 'blah'}def'

param objWithExpressions = {
  foo: 1 + 2
  bar: {
    baz: concat('abc', 'def')
  }
}

param arrayWithExpressions = [1 + 1, 'ok']