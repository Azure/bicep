/* 
  This is a block comment.
*/

// metadata with value
metadata myString2 = 'string value'
//@[9:18) Metadata myString2. Type: 'string value'. Declaration start char: 0, length: 35
metadata myInt2 = 42
//@[9:15) Metadata myInt2. Type: 42. Declaration start char: 0, length: 20
metadata myTruth = true
//@[9:16) Metadata myTruth. Type: true. Declaration start char: 0, length: 23
metadata myFalsehood = false
//@[9:20) Metadata myFalsehood. Type: false. Declaration start char: 0, length: 28
metadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[9:24) Metadata myEscapedString. Type: 'First line\r\nSecond\ttabbed\tline'. Declaration start char: 0, length: 63
metadata myMultiLineString = '''
//@[9:26) Metadata myMultiLineString. Type: '  This is a multi line string // with comments,\r\n  blocked \${interpolation},\r\n  and a /* newline.\r\n  */\r\n'. Declaration start char: 0, length: 142
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''

// object value
metadata foo = {
//@[9:12) Metadata foo. Type: object. Declaration start char: 0, length: 249
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
//@[9:24) Metadata myArrayMetadata. Type: ['a', 'b', 'c']. Declaration start char: 0, length: 52
  'a'
  'b'
  'c'
]

// emtpy object and array
metadata myEmptyObj = { }
//@[9:19) Metadata myEmptyObj. Type: object. Declaration start char: 0, length: 25
metadata myEmptyArray = [ ]
//@[9:21) Metadata myEmptyArray. Type: <empty array>. Declaration start char: 0, length: 27

// param with same name as metadata is permitted
param foo string
//@[6:09) Parameter foo. Type: string. Declaration start char: 0, length: 16

