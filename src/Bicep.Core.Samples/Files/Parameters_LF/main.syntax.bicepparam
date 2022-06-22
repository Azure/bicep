/* 
//@[00:706) ProgramSyntax
  This is a block comment.
*/

// parameters without default value
//@[02:006) ├─Token(NewLine) |\r\n\r\n|
@sys.description('''
//@[02:006) ├─Token(NewLine) |\r\n\r\n|
this is my multi line 
description for my myString
//@[06:008) ├─Token(NewLine) |\r\n|
//@[08:029) ├─UsingDeclarationSyntax
//@[08:013) | ├─Token(Identifier) |using|
//@[14:029) | └─StringSyntax
//@[14:029) | | └─Token(StringComplete) |'./main.bicep/'|
''')
//@[01:005) ├─Token(NewLine) |\r\n\r\n|
param myString string
param myInt int
//@[13:015) ├─Token(NewLine) |\r\n|
//@[15:031) ├─ParameterAssignmentSyntax
//@[15:020) | ├─Token(Identifier) |param|
param myBool bool
//@[05:010) | ├─IdentifierSyntax
//@[05:010) | | └─Token(Identifier) |myInt|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:015) | └─IntegerLiteralSyntax
//@[13:015) | | └─Token(Integer) |42|
//@[15:017) ├─Token(NewLine) |\r\n|
//@[17:045) ├─ParameterAssignmentSyntax
//@[17:022) | ├─Token(Identifier) |param|

// parameters with default value
//@[04:009) | ├─IdentifierSyntax
//@[04:009) | | └─Token(Identifier) |myStr|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:026) | └─SkippedTriviaSyntax
//@[12:013) | | ├─Token(Unrecognized) |"|
//@[13:018) | | ├─Token(Identifier) |hello|
//@[19:024) | | ├─Token(Identifier) |world|
//@[24:025) | | ├─Token(Exclamation) |!|
//@[25:026) | | └─Token(Unrecognized) |"|
//@[26:028) ├─Token(NewLine) |\r\n|
//@[28:047) ├─ParameterAssignmentSyntax
//@[28:033) | ├─Token(Identifier) |param|
@sys.description('this is myString2')
//@[01:007) | ├─IdentifierSyntax
//@[01:007) | | └─Token(Identifier) |myBool|
//@[08:009) | ├─Token(Assignment) |=|
//@[10:014) | └─BooleanLiteralSyntax
//@[10:014) | | └─Token(TrueKeyword) |true|
//@[14:018) ├─Token(NewLine) |\r\n\r\n|
@metadata({
  description: 'overwrite but still valid'
//@[02:004) ├─Token(NewLine) |\r\n|
//@[04:057) ├─ParameterAssignmentSyntax
//@[04:009) | ├─Token(Identifier) |param|
//@[10:015) | ├─IdentifierSyntax
//@[10:015) | | └─Token(Identifier) |myObj|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:057) | └─ObjectSyntax
//@[18:019) | | ├─Token(LeftBrace) |{|
//@[19:021) | | ├─Token(NewLine) |\r\n|
//@[22:033) | | ├─ObjectPropertySyntax
//@[22:026) | | | ├─IdentifierSyntax
//@[22:026) | | | | └─Token(Identifier) |name|
//@[26:027) | | | ├─Token(Colon) |:|
//@[28:033) | | | └─StringSyntax
//@[28:033) | | | | └─Token(StringComplete) |'vm1'|
//@[33:035) | | ├─Token(NewLine) |\r\n|
//@[36:054) | | ├─ObjectPropertySyntax
//@[36:044) | | | ├─IdentifierSyntax
//@[36:044) | | | | └─Token(Identifier) |location|
})
//@[01:002) | | | ├─Token(Colon) |:|
param myString2 string = 'string value'
//@[00:008) | | | └─StringSyntax
//@[00:008) | | | | └─Token(StringComplete) |'westus'|
//@[08:010) | | ├─Token(NewLine) |\r\n|
//@[10:011) | | └─Token(RightBrace) |}|
//@[11:013) ├─Token(NewLine) |\r\n|
//@[13:147) ├─ParameterAssignmentSyntax
//@[13:018) | ├─Token(Identifier) |param|
//@[19:031) | ├─IdentifierSyntax
//@[19:031) | | └─Token(Identifier) |myComplexObj|
//@[32:033) | ├─Token(Assignment) |=|
//@[34:147) | └─ObjectSyntax
//@[34:035) | | ├─Token(LeftBrace) |{|
//@[35:037) | | ├─Token(NewLine) |\r\n|
//@[38:051) | | ├─ObjectPropertySyntax
//@[38:045) | | | ├─IdentifierSyntax
//@[38:045) | | | | └─Token(Identifier) |enabled|
param myInt2 int = 42
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:011) | | | └─BooleanLiteralSyntax
//@[07:011) | | | | └─Token(TrueKeyword) |true|
//@[11:013) | | ├─Token(NewLine) |\r\n|
//@[14:037) | | ├─ObjectPropertySyntax
//@[14:018) | | | ├─IdentifierSyntax
//@[14:018) | | | | └─Token(Identifier) |name|
//@[18:019) | | | ├─Token(Colon) |:|
//@[20:037) | | | └─StringSyntax
//@[20:037) | | | | └─Token(StringComplete) |'complex object!'|
param myTruth bool = true
//@[15:017) | | ├─Token(NewLine) |\r\n|
//@[18:029) | | ├─ObjectPropertySyntax
//@[18:026) | | | ├─IdentifierSyntax
//@[18:026) | | | | └─Token(Identifier) |priority|
param myFalsehood bool = false
//@[00:001) | | | ├─Token(Colon) |:|
//@[02:003) | | | └─IntegerLiteralSyntax
//@[02:003) | | | | └─Token(Integer) |3|
//@[03:005) | | ├─Token(NewLine) |\r\n|
//@[06:056) | | ├─ObjectPropertySyntax
//@[06:010) | | | ├─IdentifierSyntax
//@[06:010) | | | | └─Token(Identifier) |data|
//@[10:011) | | | ├─Token(Colon) |:|
//@[12:056) | | | └─ObjectSyntax
//@[12:013) | | | | ├─Token(LeftBrace) |{|
//@[13:015) | | | | ├─Token(NewLine) |\r\n|
//@[17:023) | | | | ├─ObjectPropertySyntax
//@[17:018) | | | | | ├─IdentifierSyntax
//@[17:018) | | | | | | └─Token(Identifier) |a|
//@[18:019) | | | | | ├─Token(Colon) |:|
//@[20:023) | | | | | └─StringSyntax
//@[20:023) | | | | | | └─Token(StringComplete) |'b'|
//@[23:025) | | | | ├─Token(NewLine) |\r\n|
//@[27:052) | | | | ├─ObjectPropertySyntax
//@[27:028) | | | | | ├─IdentifierSyntax
//@[27:028) | | | | | | └─Token(Identifier) |c|
//@[28:029) | | | | | ├─Token(Colon) |:|
//@[30:052) | | | | | └─ArraySyntax
//@[30:031) | | | | | | ├─Token(LeftSquare) |[|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[00:002) | | | | | | ├─Token(NewLine) |\r\n|
//@[05:008) | | | | | | ├─ArrayItemSyntax
//@[05:008) | | | | | | | └─StringSyntax
//@[05:008) | | | | | | | | └─Token(StringComplete) |'d'|
//@[08:010) | | | | | | ├─Token(NewLine) |\r\n|
//@[13:016) | | | | | | ├─ArrayItemSyntax
//@[13:016) | | | | | | | └─StringSyntax
//@[13:016) | | | | | | | | └─Token(StringComplete) |'e'|
//@[16:018) | | | | | | ├─Token(NewLine) |\r\n|
//@[20:021) | | | | | | └─Token(RightSquare) |]|
//@[21:023) | | | | ├─Token(NewLine) |\r\n|
//@[24:025) | | | | └─Token(RightBrace) |}|
//@[25:027) | | ├─Token(NewLine) |\r\n|
//@[27:028) | | └─Token(RightBrace) |}|
//@[28:032) ├─Token(NewLine) |\r\n\r\n|
//@[65:067) ├─Token(NewLine) |\r\n|
//@[67:108) ├─ParameterAssignmentSyntax
//@[67:072) | ├─Token(Identifier) |param|

// object default value
//@[04:012) | ├─IdentifierSyntax
//@[04:012) | | └─Token(Identifier) |myIntArr|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:039) | └─ArraySyntax
//@[15:016) | | ├─Token(LeftSquare) |[|
//@[16:018) | | ├─Token(NewLine) |\r\n|
//@[19:020) | | ├─ArrayItemSyntax
//@[19:020) | | | └─IntegerLiteralSyntax
//@[19:020) | | | | └─Token(Integer) |1|
//@[20:022) | | ├─Token(NewLine) |\r\n|
//@[23:024) | | ├─ArrayItemSyntax
//@[23:024) | | | └─IntegerLiteralSyntax
//@[23:024) | | | | └─Token(Integer) |2|
@sys.description('this is foo')
//@[00:002) | | ├─Token(NewLine) |\r\n|
//@[03:004) | | ├─ArrayItemSyntax
//@[03:004) | | | └─IntegerLiteralSyntax
//@[03:004) | | | | └─Token(Integer) |3|
//@[04:006) | | ├─Token(NewLine) |\r\n|
//@[07:008) | | ├─ArrayItemSyntax
//@[07:008) | | | └─IntegerLiteralSyntax
//@[07:008) | | | | └─Token(Integer) |4|
//@[08:010) | | ├─Token(NewLine) |\r\n|
//@[11:012) | | ├─ArrayItemSyntax
//@[11:012) | | | └─IntegerLiteralSyntax
//@[11:012) | | | | └─Token(Integer) |5|
//@[12:014) | | ├─Token(NewLine) |\r\n|
//@[14:015) | | └─Token(RightSquare) |]|
//@[15:017) ├─Token(NewLine) |\r\n|
//@[17:071) ├─ParameterAssignmentSyntax
//@[17:022) | ├─Token(Identifier) |param|
//@[23:031) | ├─IdentifierSyntax
//@[23:031) | | └─Token(Identifier) |myStrArr|
@metadata({
//@[00:001) | ├─Token(Assignment) |=|
//@[02:039) | └─ArraySyntax
//@[02:003) | | ├─Token(LeftSquare) |[|
//@[03:005) | | ├─Token(NewLine) |\r\n|
//@[06:011) | | ├─ArrayItemSyntax
//@[06:011) | | | └─StringSyntax
//@[06:011) | | | | └─Token(StringComplete) |'ant'|
//@[11:013) | | ├─Token(NewLine) |\r\n|
  description: 'overwrite but still valid'
//@[02:008) | | ├─ArrayItemSyntax
//@[02:008) | | | └─StringSyntax
//@[02:008) | | | | └─Token(StringComplete) |'bear'|
//@[08:010) | | ├─Token(NewLine) |\r\n|
//@[11:016) | | ├─ArrayItemSyntax
//@[11:016) | | | └─StringSyntax
//@[11:016) | | | | └─Token(StringComplete) |'cat'|
//@[16:018) | | ├─Token(NewLine) |\r\n|
//@[19:024) | | ├─ArrayItemSyntax
//@[19:024) | | | └─StringSyntax
//@[19:024) | | | | └─Token(StringComplete) |'dog'|
//@[24:026) | | ├─Token(NewLine) |\r\n|
//@[26:027) | | └─Token(RightSquare) |]|
//@[27:029) ├─Token(NewLine) |\r\n|
//@[29:114) ├─ParameterAssignmentSyntax
//@[29:034) | ├─Token(Identifier) |param|
//@[35:047) | ├─IdentifierSyntax
//@[35:047) | | └─Token(Identifier) |myComplexArr|
  another: 'just for fun'
//@[05:006) | ├─Token(Assignment) |=|
//@[07:071) | └─ArraySyntax
//@[07:008) | | ├─Token(LeftSquare) |[|
//@[08:010) | | ├─Token(NewLine) |\r\n|
//@[11:018) | | ├─ArrayItemSyntax
//@[11:018) | | | └─StringSyntax
//@[11:018) | | | | └─Token(StringComplete) |'eagle'|
//@[18:020) | | ├─Token(NewLine) |\r\n|
//@[21:023) | | ├─ArrayItemSyntax
//@[21:023) | | | └─IntegerLiteralSyntax
//@[21:023) | | | | └─Token(Integer) |21|
//@[23:025) | | ├─Token(NewLine) |\r\n|
})
//@[00:005) | | ├─ArrayItemSyntax
//@[00:005) | | | └─BooleanLiteralSyntax
//@[00:005) | | | | └─Token(FalseKeyword) |false|
param foo object = {
//@[02:004) | | ├─Token(NewLine) |\r\n|
//@[05:039) | | ├─ArrayItemSyntax
//@[05:039) | | | └─ObjectSyntax
//@[05:006) | | | | ├─Token(LeftBrace) |{|
//@[06:008) | | | | ├─Token(NewLine) |\r\n|
//@[10:035) | | | | ├─ObjectPropertySyntax
//@[10:011) | | | | | ├─IdentifierSyntax
//@[10:011) | | | | | | └─Token(Identifier) |f|
//@[11:012) | | | | | ├─Token(Colon) |:|
//@[13:035) | | | | | └─ArraySyntax
//@[13:014) | | | | | | ├─Token(LeftSquare) |[|
//@[14:016) | | | | | | ├─Token(NewLine) |\r\n|
//@[19:022) | | | | | | ├─ArrayItemSyntax
//@[19:022) | | | | | | | └─StringSyntax
//@[19:022) | | | | | | | | └─Token(StringComplete) |'g'|
  enabled: true
//@[01:003) | | | | | | ├─Token(NewLine) |\r\n|
//@[06:009) | | | | | | ├─ArrayItemSyntax
//@[06:009) | | | | | | | └─StringSyntax
//@[06:009) | | | | | | | | └─Token(StringComplete) |'h'|
//@[09:011) | | | | | | ├─Token(NewLine) |\r\n|
//@[13:014) | | | | | | └─Token(RightSquare) |]|
//@[14:016) | | | | ├─Token(NewLine) |\r\n|
  name: 'this is my object'
//@[01:002) | | | | └─Token(RightBrace) |}|
//@[02:004) | | ├─Token(NewLine) |\r\n|
//@[04:005) | | └─Token(RightSquare) |]|
//@[05:007) ├─Token(NewLine) |\r\n|
//@[07:007) └─Token(EndOfFile) ||
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

// array default value
param myArrayParam array = [
  'a'
  'b'
  'c'
]

// secure string
@secure()
param password string

// secure object
@secure()
param secretObject object

// enum parameter
@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array

// empty metadata
@metadata({})
param emptyMetadata string

// description
@metadata({
  description: 'my description'
})
param description string

@sys.description('my description')
param description2 string

// random extra metadata
@metadata({
  description: 'my description'
  a: 1
  b: true
  c: [
  ]
  d: {
    test: 'abc'
  }
})
param additionalMetadata string

// all modifiers together
@secure()
@minLength(3)
@maxLength(24)
@allowed([
  'one'
  'two'
  'three'
])
@metadata({
  description: 'Name of the storage account'
})
param someParameter string

param defaultExpression bool = 18 != (true || false)

@allowed([
  'abc'
  'def'
])
param stringLiteral string

@allowed([
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string

@minValue(200)
param decoratedInt int = 123

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
    bar: [
        {          }
        true
        123
    ]
})
param decoratedBool bool = (true && false) != true

@secure()
param decoratedObject object = {
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

@sys.metadata({
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
    utcNow()
    newGuid()
]

