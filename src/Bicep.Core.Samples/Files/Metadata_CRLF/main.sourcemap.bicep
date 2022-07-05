/* 
  This is a block comment.
*/

// metadata with value
meta myString2 = 'string value'
meta myInt2 = 42
meta myTruth = true
meta myFalsehood = false
meta myEscapedString = 'First line\r\nSecond\ttabbed\tline'

// object value
meta foo = {
  enabled: true
//@[15:15]       "enabled": true,
  name: 'this is my object'
//@[16:16]       "name": "this is my object",
  priority: 3
//@[17:17]       "priority": 3,
  info: {
//@[18:20]       "info": {
    a: 'b'
//@[19:19]         "a": "b"
  }
  empty: {
//@[21:21]       "empty": {},
  }
  array: [
//@[22:33]       "array": [
    'string item'
//@[23:23]         "string item",
    12
//@[24:24]         12,
    true
//@[25:25]         true,
    [
      'inner'
//@[27:27]           "inner",
      false
//@[28:28]           false
    ]
    {
      a: 'b'
//@[31:31]           "a": "b"
    }
  ]
}

// array value
meta myArrayMetadata = [
  'a'
//@[36:36]       "a",
  'b'
//@[37:37]       "b",
  'c'
//@[38:38]       "c"
]

// emtpy object and array
meta myEmptyObj = { }
meta myEmptyArray = [ ]

