/* 
  This is a block comment.
*/

// templateMetadata with value
templateMetadata myString2 = 'string value'
templateMetadata myInt2 = 42
templateMetadata myTruth = true
templateMetadata myFalsehood = false
templateMetadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'

// object value
templateMetadata foo = {
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {
  }
  array: [
    'string item'
    12
    true
    [
      'inner'
      false
    ]
    {
      a: 'b'
    }
  ]
}

// array value
templateMetadata myArrayParam = [
  'a'
  'b'
  'c'
]

// emtpy object and array
templateMetadata myEmptyObj = { }
templateMetadata myEmptyArray = [ ]
