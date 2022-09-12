/* 
  This is a block comment.
*/

// metadata with value
metadata myString2 = 'string value'
metadata myInt2 = 42
metadata myTruth = true
metadata myFalsehood = false
metadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
metadata myMultiLineString = '''
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''

// object value
metadata foo = {
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
metadata myArrayMetadata = [
  'a'
  'b'
  'c'
]

// emtpy object and array
metadata myEmptyObj = { }
metadata myEmptyArray = [ ]

// param with same name as metadata is permitted
param foo string
//@[6:9) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |foo|

