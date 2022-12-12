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
//@[line19->line16]       "enabled": true,
  name: 'this is my object'
//@[line20->line17]       "name": "this is my object",
  priority: 3
//@[line21->line18]       "priority": 3,
  info: {
//@[line22->line19]       "info": {
//@[line22->line21]       },
    a: 'b'
//@[line23->line20]         "a": "b"
  }
  empty: {
//@[line25->line22]       "empty": {},
  }
  array: [
//@[line27->line23]       "array": [
//@[line27->line34]       ]
    'string item'
//@[line28->line24]         "string item",
    12
//@[line29->line25]         12,
    true
//@[line30->line26]         true,
    [
//@[line31->line27]         [
//@[line31->line30]         ],
      'inner'
//@[line32->line28]           "inner",
      false
//@[line33->line29]           false
    ]
    {
//@[line35->line31]         {
//@[line35->line33]         }
      a: 'b'
//@[line36->line32]           "a": "b"
    }
  ]
}

// array value
metadata myArrayMetadata = [
  'a'
//@[line43->line37]       "a",
  'b'
//@[line44->line38]       "b",
  'c'
//@[line45->line39]       "c"
]

// emtpy object and array
metadata myEmptyObj = { }
metadata myEmptyArray = [ ]

// param with same name as metadata is permitted
param foo string
//@[line53->line45]     "foo": {
//@[line53->line46]       "type": "string"
//@[line53->line47]     }

