/* 
  This is a block comment.
*/
//@[02:06) NewLine |\r\n\r\n|

// metadata with value
//@[22:24) NewLine |\r\n|
meta myString2 = 'string value'
//@[00:04) Identifier |meta|
//@[05:14) Identifier |myString2|
//@[15:16) Assignment |=|
//@[17:31) StringComplete |'string value'|
//@[31:33) NewLine |\r\n|
meta myInt2 = 42
//@[00:04) Identifier |meta|
//@[05:11) Identifier |myInt2|
//@[12:13) Assignment |=|
//@[14:16) Integer |42|
//@[16:18) NewLine |\r\n|
meta myTruth = true
//@[00:04) Identifier |meta|
//@[05:12) Identifier |myTruth|
//@[13:14) Assignment |=|
//@[15:19) TrueKeyword |true|
//@[19:21) NewLine |\r\n|
meta myFalsehood = false
//@[00:04) Identifier |meta|
//@[05:16) Identifier |myFalsehood|
//@[17:18) Assignment |=|
//@[19:24) FalseKeyword |false|
//@[24:26) NewLine |\r\n|
meta myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[00:04) Identifier |meta|
//@[05:20) Identifier |myEscapedString|
//@[21:22) Assignment |=|
//@[23:59) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[59:63) NewLine |\r\n\r\n|

// object value
//@[15:17) NewLine |\r\n|
meta foo = {
//@[00:04) Identifier |meta|
//@[05:08) Identifier |foo|
//@[09:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[12:14) NewLine |\r\n|
  enabled: true
//@[02:09) Identifier |enabled|
//@[09:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:17) NewLine |\r\n|
  name: 'this is my object'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:27) StringComplete |'this is my object'|
//@[27:29) NewLine |\r\n|
  priority: 3
//@[02:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Integer |3|
//@[13:15) NewLine |\r\n|
  info: {
//@[02:06) Identifier |info|
//@[06:07) Colon |:|
//@[08:09) LeftBrace |{|
//@[09:11) NewLine |\r\n|
    a: 'b'
//@[04:05) Identifier |a|
//@[05:06) Colon |:|
//@[07:10) StringComplete |'b'|
//@[10:12) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  empty: {
//@[02:07) Identifier |empty|
//@[07:08) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  array: [
//@[02:07) Identifier |array|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
    'string item'
//@[04:17) StringComplete |'string item'|
//@[17:19) NewLine |\r\n|
    12
//@[04:06) Integer |12|
//@[06:08) NewLine |\r\n|
    true
//@[04:08) TrueKeyword |true|
//@[08:10) NewLine |\r\n|
    [
//@[04:05) LeftSquare |[|
//@[05:07) NewLine |\r\n|
      'inner'
//@[06:13) StringComplete |'inner'|
//@[13:15) NewLine |\r\n|
      false
//@[06:11) FalseKeyword |false|
//@[11:13) NewLine |\r\n|
    ]
//@[04:05) RightSquare |]|
//@[05:07) NewLine |\r\n|
    {
//@[04:05) LeftBrace |{|
//@[05:07) NewLine |\r\n|
      a: 'b'
//@[06:07) Identifier |a|
//@[07:08) Colon |:|
//@[09:12) StringComplete |'b'|
//@[12:14) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  ]
//@[02:03) RightSquare |]|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// array value
//@[14:16) NewLine |\r\n|
meta myArrayMetadata = [
//@[00:04) Identifier |meta|
//@[05:20) Identifier |myArrayMetadata|
//@[21:22) Assignment |=|
//@[23:24) LeftSquare |[|
//@[24:26) NewLine |\r\n|
  'a'
//@[02:05) StringComplete |'a'|
//@[05:07) NewLine |\r\n|
  'b'
//@[02:05) StringComplete |'b'|
//@[05:07) NewLine |\r\n|
  'c'
//@[02:05) StringComplete |'c'|
//@[05:07) NewLine |\r\n|
]
//@[00:01) RightSquare |]|
//@[01:05) NewLine |\r\n\r\n|

// emtpy object and array
//@[25:27) NewLine |\r\n|
meta myEmptyObj = { }
//@[00:04) Identifier |meta|
//@[05:15) Identifier |myEmptyObj|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[20:21) RightBrace |}|
//@[21:23) NewLine |\r\n|
meta myEmptyArray = [ ]
//@[00:04) Identifier |meta|
//@[05:17) Identifier |myEmptyArray|
//@[18:19) Assignment |=|
//@[20:21) LeftSquare |[|
//@[22:23) RightSquare |]|
//@[23:25) NewLine |\r\n|

//@[00:00) EndOfFile ||
