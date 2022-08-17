/* 
  This is a block comment.
*/

// metadata with value
meta myString2 = 'string value'
//@[5:14) Metadata myString2. Type: 'string value'. Declaration start char: 0, length: 31
meta myInt2 = 42
//@[5:11) Metadata myInt2. Type: int. Declaration start char: 0, length: 16
meta myTruth = true
//@[5:12) Metadata myTruth. Type: bool. Declaration start char: 0, length: 19
meta myFalsehood = false
//@[5:16) Metadata myFalsehood. Type: bool. Declaration start char: 0, length: 24
meta myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[5:20) Metadata myEscapedString. Type: 'First line\r\nSecond\ttabbed\tline'. Declaration start char: 0, length: 59

// object value
meta foo = {
//@[5:08) Metadata foo. Type: object. Declaration start char: 0, length: 245
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
//@[5:20) Metadata myArrayMetadata. Type: ('a' | 'b' | 'c')[]. Declaration start char: 0, length: 48
  'a'
  'b'
  'c'
]

// emtpy object and array
meta myEmptyObj = { }
//@[5:15) Metadata myEmptyObj. Type: object. Declaration start char: 0, length: 21
meta myEmptyArray = [ ]
//@[5:17) Metadata myEmptyArray. Type: array. Declaration start char: 0, length: 23

// param with same name as metadata is permitted
param foo string
//@[6:09) Parameter foo. Type: string. Declaration start char: 0, length: 16

