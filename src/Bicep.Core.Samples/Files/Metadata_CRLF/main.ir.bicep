/* 
//@[00:883) ProgramExpression
  This is a block comment.
*/

// metadata with value
metadata myString2 = 'string value'
//@[00:035) ├─DeclaredMetadataExpression { Name = myString2 }
//@[21:035) | └─StringLiteralExpression { Value = string value }
metadata myInt2 = 42
//@[00:020) ├─DeclaredMetadataExpression { Name = myInt2 }
//@[18:020) | └─IntegerLiteralExpression { Value = 42 }
metadata myTruth = true
//@[00:023) ├─DeclaredMetadataExpression { Name = myTruth }
//@[19:023) | └─BooleanLiteralExpression { Value = True }
metadata myFalsehood = false
//@[00:028) ├─DeclaredMetadataExpression { Name = myFalsehood }
//@[23:028) | └─BooleanLiteralExpression { Value = False }
metadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[00:063) ├─DeclaredMetadataExpression { Name = myEscapedString }
//@[27:063) | └─StringLiteralExpression { Value = First line\r\nSecond\ttabbed\tline }
metadata myMultiLineString = '''
//@[00:142) ├─DeclaredMetadataExpression { Name = myMultiLineString }
//@[29:142) | └─StringLiteralExpression { Value =   This is a multi line string // with comments,\r\n  blocked ${interpolation},\r\n  and a /* newline.\r\n  */\r\n }
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''

// object value
metadata foo = {
//@[00:249) ├─DeclaredMetadataExpression { Name = foo }
//@[15:249) | └─ObjectExpression
  enabled: true
//@[02:015) |   ├─ObjectPropertyExpression
//@[02:009) |   | ├─StringLiteralExpression { Value = enabled }
//@[11:015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[02:027) |   ├─ObjectPropertyExpression
//@[02:006) |   | ├─StringLiteralExpression { Value = name }
//@[08:027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[02:013) |   ├─ObjectPropertyExpression
//@[02:010) |   | ├─StringLiteralExpression { Value = priority }
//@[12:013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[02:026) |   ├─ObjectPropertyExpression
//@[02:006) |   | ├─StringLiteralExpression { Value = info }
//@[08:026) |   | └─ObjectExpression
    a: 'b'
//@[04:010) |   |   └─ObjectPropertyExpression
//@[04:005) |   |     ├─StringLiteralExpression { Value = a }
//@[07:010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[02:015) |   ├─ObjectPropertyExpression
//@[02:007) |   | ├─StringLiteralExpression { Value = empty }
//@[09:015) |   | └─ObjectExpression
  }
  array: [
//@[02:122) |   └─ObjectPropertyExpression
//@[02:007) |     ├─StringLiteralExpression { Value = array }
//@[09:122) |     └─ArrayExpression
    'string item'
//@[04:017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[04:006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[04:008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[04:040) |       ├─ArrayExpression
      'inner'
//@[06:013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[06:011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[04:026) |       └─ObjectExpression
      a: 'b'
//@[06:012) |         └─ObjectPropertyExpression
//@[06:007) |           ├─StringLiteralExpression { Value = a }
//@[09:012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

// array value
metadata myArrayMetadata = [
//@[00:052) ├─DeclaredMetadataExpression { Name = myArrayMetadata }
//@[27:052) | └─ArrayExpression
  'a'
//@[02:005) |   ├─StringLiteralExpression { Value = a }
  'b'
//@[02:005) |   ├─StringLiteralExpression { Value = b }
  'c'
//@[02:005) |   └─StringLiteralExpression { Value = c }
]

// emtpy object and array
metadata myEmptyObj = { }
//@[00:025) ├─DeclaredMetadataExpression { Name = myEmptyObj }
//@[22:025) | └─ObjectExpression
metadata myEmptyArray = [ ]
//@[00:027) ├─DeclaredMetadataExpression { Name = myEmptyArray }
//@[24:027) | └─ArrayExpression

// param with same name as metadata is permitted
param foo string
//@[00:016) └─DeclaredParameterExpression { Name = foo }

