/* 
  This is a block comment.
*/

// metadata with value
metadata myString2 = 'string value'
//@    "myString2": "string value",
metadata myInt2 = 42
//@    "myInt2": 42,
metadata myTruth = true
//@    "myTruth": true,
metadata myFalsehood = false
//@    "myFalsehood": false,
metadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@    "myEscapedString": "First line\r\nSecond\ttabbed\tline",
metadata myMultiLineString = '''
//@    "myMultiLineString": "  This is a multi line string // with comments,\r\n  blocked ${interpolation},\r\n  and a /* newline.\r\n  */\r\n",
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''

// object value
metadata foo = {
//@    "foo": {
//@    },
  enabled: true
//@      "enabled": true,
  name: 'this is my object'
//@      "name": "this is my object",
  priority: 3
//@      "priority": 3,
  info: {
//@      "info": {
//@      },
    a: 'b'
//@        "a": "b"
  }
  empty: {
//@      "empty": {},
  }
  array: [
//@      "array": [
//@      ]
    'string item'
//@        "string item",
    12
//@        12,
    true
//@        true,
    [
//@        [
//@        ],
      'inner'
//@          "inner",
      false
//@          false
    ]
    {
//@        {
//@        }
      a: 'b'
//@          "a": "b"
    }
  ]
}

// array value
metadata myArrayMetadata = [
//@    "myArrayMetadata": [
//@    ],
  'a'
//@      "a",
  'b'
//@      "b",
  'c'
//@      "c"
]

// emtpy object and array
metadata myEmptyObj = { }
//@    "myEmptyObj": {},
metadata myEmptyArray = [ ]
//@    "myEmptyArray": []

// param with same name as metadata is permitted
param foo string
//@    "foo": {
//@      "type": "string"
//@    }

