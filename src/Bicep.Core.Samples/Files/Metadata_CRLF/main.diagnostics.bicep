/* 
  This is a block comment.
*/

// metadata with value
meta myString2 = 'string value'
meta myInt2 = 42
meta myTruth = true
meta myFalsehood = false
meta myEscapedString = 'First line\r\nSecond\ttabbed\tline'
meta myMultiLineString = '''
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''

// object value
meta foo = {
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
meta myArrayMetadata = [
  'a'
  'b'
  'c'
]

// emtpy object and array
meta myEmptyObj = { }
meta myEmptyArray = [ ]

// param with same name as metadata is permitted
param foo string
//@[6:9) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |foo|

