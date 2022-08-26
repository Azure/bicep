/* 
  This is a block comment.
*/
//@[02:006) NewLine |\r\n\r\n|

// metadata with value
//@[22:024) NewLine |\r\n|
metadata myString2 = 'string value'
//@[00:008) Identifier |metadata|
//@[09:018) Identifier |myString2|
//@[19:020) Assignment |=|
//@[21:035) StringComplete |'string value'|
//@[35:037) NewLine |\r\n|
metadata myInt2 = 42
//@[00:008) Identifier |metadata|
//@[09:015) Identifier |myInt2|
//@[16:017) Assignment |=|
//@[18:020) Integer |42|
//@[20:022) NewLine |\r\n|
metadata myTruth = true
//@[00:008) Identifier |metadata|
//@[09:016) Identifier |myTruth|
//@[17:018) Assignment |=|
//@[19:023) TrueKeyword |true|
//@[23:025) NewLine |\r\n|
metadata myFalsehood = false
//@[00:008) Identifier |metadata|
//@[09:020) Identifier |myFalsehood|
//@[21:022) Assignment |=|
//@[23:028) FalseKeyword |false|
//@[28:030) NewLine |\r\n|
metadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[00:008) Identifier |metadata|
//@[09:024) Identifier |myEscapedString|
//@[25:026) Assignment |=|
//@[27:063) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[63:065) NewLine |\r\n|
metadata myMultiLineString = '''
//@[00:008) Identifier |metadata|
//@[09:026) Identifier |myMultiLineString|
//@[27:028) Assignment |=|
//@[29:142) MultilineString |'''\r\n  This is a multi line string // with comments,\r\n  blocked ${interpolation},\r\n  and a /* newline.\r\n  */\r\n'''|
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''
//@[03:007) NewLine |\r\n\r\n|

// object value
//@[15:017) NewLine |\r\n|
metadata foo = {
//@[00:008) Identifier |metadata|
//@[09:012) Identifier |foo|
//@[13:014) Assignment |=|
//@[15:016) LeftBrace |{|
//@[16:018) NewLine |\r\n|
  enabled: true
//@[02:009) Identifier |enabled|
//@[09:010) Colon |:|
//@[11:015) TrueKeyword |true|
//@[15:017) NewLine |\r\n|
  name: 'this is my object'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:027) StringComplete |'this is my object'|
//@[27:029) NewLine |\r\n|
  priority: 3
//@[02:010) Identifier |priority|
//@[10:011) Colon |:|
//@[12:013) Integer |3|
//@[13:015) NewLine |\r\n|
  info: {
//@[02:006) Identifier |info|
//@[06:007) Colon |:|
//@[08:009) LeftBrace |{|
//@[09:011) NewLine |\r\n|
    a: 'b'
//@[04:005) Identifier |a|
//@[05:006) Colon |:|
//@[07:010) StringComplete |'b'|
//@[10:012) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
  empty: {
//@[02:007) Identifier |empty|
//@[07:008) Colon |:|
//@[09:010) LeftBrace |{|
//@[10:012) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
  array: [
//@[02:007) Identifier |array|
//@[07:008) Colon |:|
//@[09:010) LeftSquare |[|
//@[10:012) NewLine |\r\n|
    'string item'
//@[04:017) StringComplete |'string item'|
//@[17:019) NewLine |\r\n|
    12
//@[04:006) Integer |12|
//@[06:008) NewLine |\r\n|
    true
//@[04:008) TrueKeyword |true|
//@[08:010) NewLine |\r\n|
    [
//@[04:005) LeftSquare |[|
//@[05:007) NewLine |\r\n|
      'inner'
//@[06:013) StringComplete |'inner'|
//@[13:015) NewLine |\r\n|
      false
//@[06:011) FalseKeyword |false|
//@[11:013) NewLine |\r\n|
    ]
//@[04:005) RightSquare |]|
//@[05:007) NewLine |\r\n|
    {
//@[04:005) LeftBrace |{|
//@[05:007) NewLine |\r\n|
      a: 'b'
//@[06:007) Identifier |a|
//@[07:008) Colon |:|
//@[09:012) StringComplete |'b'|
//@[12:014) NewLine |\r\n|
    }
//@[04:005) RightBrace |}|
//@[05:007) NewLine |\r\n|
  ]
//@[02:003) RightSquare |]|
//@[03:005) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

// array value
//@[14:016) NewLine |\r\n|
metadata myArrayMetadata = [
//@[00:008) Identifier |metadata|
//@[09:024) Identifier |myArrayMetadata|
//@[25:026) Assignment |=|
//@[27:028) LeftSquare |[|
//@[28:030) NewLine |\r\n|
  'a'
//@[02:005) StringComplete |'a'|
//@[05:007) NewLine |\r\n|
  'b'
//@[02:005) StringComplete |'b'|
//@[05:007) NewLine |\r\n|
  'c'
//@[02:005) StringComplete |'c'|
//@[05:007) NewLine |\r\n|
]
//@[00:001) RightSquare |]|
//@[01:005) NewLine |\r\n\r\n|

// emtpy object and array
//@[25:027) NewLine |\r\n|
metadata myEmptyObj = { }
//@[00:008) Identifier |metadata|
//@[09:019) Identifier |myEmptyObj|
//@[20:021) Assignment |=|
//@[22:023) LeftBrace |{|
//@[24:025) RightBrace |}|
//@[25:027) NewLine |\r\n|
metadata myEmptyArray = [ ]
//@[00:008) Identifier |metadata|
//@[09:021) Identifier |myEmptyArray|
//@[22:023) Assignment |=|
//@[24:025) LeftSquare |[|
//@[26:027) RightSquare |]|
//@[27:031) NewLine |\r\n\r\n|

// param with same name as metadata is permitted
//@[48:050) NewLine |\r\n|
param foo string
//@[00:005) Identifier |param|
//@[06:009) Identifier |foo|
//@[10:016) Identifier |string|
//@[16:018) NewLine |\r\n|

//@[00:000) EndOfFile ||
