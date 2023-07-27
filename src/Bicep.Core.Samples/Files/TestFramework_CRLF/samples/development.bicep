param location string 

var obj = {
  prop: 'juan'
  nested: {
    nestedProp: 'nestedPropValue'
    nestedArray: [1, 2, 3]
    loc: location
  }
}



assert accessObj = obj.prop == 'juan'
assert accessNestedProp = obj.nested.nestedProp == 'nestedPropValue'
assert accessNestedPropArray = obj.nested.nestedArray == [1, 2, 4]

