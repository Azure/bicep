/* 
  This is a block comment.
*/
//@[2:6) NewLine |\r\n\r\n|

// templateMetadata with value
//@[30:32) NewLine |\r\n|
templateMetadata myString2 = 'string value'
//@[0:16) Identifier |templateMetadata|
//@[17:26) Identifier |myString2|
//@[27:28) Assignment |=|
//@[29:43) StringComplete |'string value'|
//@[43:45) NewLine |\r\n|
templateMetadata myInt2 = 42
//@[0:16) Identifier |templateMetadata|
//@[17:23) Identifier |myInt2|
//@[24:25) Assignment |=|
//@[26:28) Integer |42|
//@[28:30) NewLine |\r\n|
templateMetadata myTruth = true
//@[0:16) Identifier |templateMetadata|
//@[17:24) Identifier |myTruth|
//@[25:26) Assignment |=|
//@[27:31) TrueKeyword |true|
//@[31:33) NewLine |\r\n|
templateMetadata myFalsehood = false
//@[0:16) Identifier |templateMetadata|
//@[17:28) Identifier |myFalsehood|
//@[29:30) Assignment |=|
//@[31:36) FalseKeyword |false|
//@[36:38) NewLine |\r\n|
templateMetadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[0:16) Identifier |templateMetadata|
//@[17:32) Identifier |myEscapedString|
//@[33:34) Assignment |=|
//@[35:71) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[71:75) NewLine |\r\n\r\n|

// object value
//@[15:17) NewLine |\r\n|
templateMetadata foo = {
//@[0:16) Identifier |templateMetadata|
//@[17:20) Identifier |foo|
//@[21:22) Assignment |=|
//@[23:24) LeftBrace |{|
//@[24:26) NewLine |\r\n|
  enabled: true
//@[2:9) Identifier |enabled|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:17) NewLine |\r\n|
  name: 'this is my object'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'this is my object'|
//@[27:29) NewLine |\r\n|
  priority: 3
//@[2:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Integer |3|
//@[13:15) NewLine |\r\n|
  info: {
//@[2:6) Identifier |info|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:11) NewLine |\r\n|
    a: 'b'
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'b'|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  empty: {
//@[2:7) Identifier |empty|
//@[7:8) Colon |:|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  array: [
//@[2:7) Identifier |array|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
    'string item'
//@[4:17) StringComplete |'string item'|
//@[17:19) NewLine |\r\n|
    12
//@[4:6) Integer |12|
//@[6:8) NewLine |\r\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:10) NewLine |\r\n|
    [
//@[4:5) LeftSquare |[|
//@[5:7) NewLine |\r\n|
      'inner'
//@[6:13) StringComplete |'inner'|
//@[13:15) NewLine |\r\n|
      false
//@[6:11) FalseKeyword |false|
//@[11:13) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
    {
//@[4:5) LeftBrace |{|
//@[5:7) NewLine |\r\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:14) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// array value
//@[14:16) NewLine |\r\n|
templateMetadata myArrayParam = [
//@[0:16) Identifier |templateMetadata|
//@[17:29) Identifier |myArrayParam|
//@[30:31) Assignment |=|
//@[32:33) LeftSquare |[|
//@[33:35) NewLine |\r\n|
  'a'
//@[2:5) StringComplete |'a'|
//@[5:7) NewLine |\r\n|
  'b'
//@[2:5) StringComplete |'b'|
//@[5:7) NewLine |\r\n|
  'c'
//@[2:5) StringComplete |'c'|
//@[5:7) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

// emtpy object and array
//@[25:27) NewLine |\r\n|
templateMetadata myEmptyObj = { }
//@[0:16) Identifier |templateMetadata|
//@[17:27) Identifier |myEmptyObj|
//@[28:29) Assignment |=|
//@[30:31) LeftBrace |{|
//@[32:33) RightBrace |}|
//@[33:35) NewLine |\r\n|
templateMetadata myEmptyArray = [ ]
//@[0:16) Identifier |templateMetadata|
//@[17:29) Identifier |myEmptyArray|
//@[30:31) Assignment |=|
//@[32:33) LeftSquare |[|
//@[34:35) RightSquare |]|
//@[35:37) NewLine |\r\n|

//@[0:0) EndOfFile ||
