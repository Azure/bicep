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
//@[16:16]       "enabled": true,
  name: 'this is my object'
//@[17:17]       "name": "this is my object",
  priority: 3
//@[18:18]       "priority": 3,
  info: {
//@[19:21]       "info": {
    a: 'b'
//@[20:20]         "a": "b"
  }
  empty: {
//@[22:22]       "empty": {},
  }
  array: [
//@[23:34]       "array": [
    'string item'
//@[24:24]         "string item",
    12
//@[25:25]         12,
    true
//@[26:26]         true,
    [
      'inner'
//@[28:28]           "inner",
      false
//@[29:29]           false
    ]
    {
      a: 'b'
//@[32:32]           "a": "b"
    }
  ]
}

// array value
meta myArrayMetadata = [
  'a'
//@[37:37]       "a",
  'b'
//@[38:38]       "b",
  'c'
//@[39:39]       "c"
]

// emtpy object and array
meta myEmptyObj = { }
meta myEmptyArray = [ ]

// param with same name as metadata is permitted
param foo string
//@[45:47]     "foo": {

