
//@[0:2) NewLine |\r\n|
//self-cycle
//@[12:14) NewLine |\r\n|
var x = x
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) Identifier |x|
//@[9:11) NewLine |\r\n|
var q = base64(q, !q)
//@[0:3) Identifier |var|
//@[4:5) Identifier |q|
//@[6:7) Assignment |=|
//@[8:14) Identifier |base64|
//@[14:15) LeftParen |(|
//@[15:16) Identifier |q|
//@[16:17) Comma |,|
//@[18:19) Exclamation |!|
//@[19:20) Identifier |q|
//@[20:21) RightParen |)|
//@[21:25) NewLine |\r\n\r\n|

//2-cycle
//@[9:11) NewLine |\r\n|
var a = b
//@[0:3) Identifier |var|
//@[4:5) Identifier |a|
//@[6:7) Assignment |=|
//@[8:9) Identifier |b|
//@[9:11) NewLine |\r\n|
var b = max(a,1)
//@[0:3) Identifier |var|
//@[4:5) Identifier |b|
//@[6:7) Assignment |=|
//@[8:11) Identifier |max|
//@[11:12) LeftParen |(|
//@[12:13) Identifier |a|
//@[13:14) Comma |,|
//@[14:15) Integer |1|
//@[15:16) RightParen |)|
//@[16:20) NewLine |\r\n\r\n|

//3-cycle
//@[9:11) NewLine |\r\n|
var e = f
//@[0:3) Identifier |var|
//@[4:5) Identifier |e|
//@[6:7) Assignment |=|
//@[8:9) Identifier |f|
//@[9:11) NewLine |\r\n|
var f = g && true
//@[0:3) Identifier |var|
//@[4:5) Identifier |f|
//@[6:7) Assignment |=|
//@[8:9) Identifier |g|
//@[10:12) LogicalAnd |&&|
//@[13:17) TrueKeyword |true|
//@[17:19) NewLine |\r\n|
var g = e ? e : e
//@[0:3) Identifier |var|
//@[4:5) Identifier |g|
//@[6:7) Assignment |=|
//@[8:9) Identifier |e|
//@[10:11) Question |?|
//@[12:13) Identifier |e|
//@[14:15) Colon |:|
//@[16:17) Identifier |e|
//@[17:21) NewLine |\r\n\r\n|

//4-cycle
//@[9:11) NewLine |\r\n|
var aa = {
//@[0:3) Identifier |var|
//@[4:6) Identifier |aa|
//@[7:8) Assignment |=|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  bb: bb
//@[2:4) Identifier |bb|
//@[4:5) Colon |:|
//@[6:8) Identifier |bb|
//@[8:10) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
var bb = {
//@[0:3) Identifier |var|
//@[4:6) Identifier |bb|
//@[7:8) Assignment |=|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  cc: cc
//@[2:4) Identifier |cc|
//@[4:5) Colon |:|
//@[6:8) Identifier |cc|
//@[8:10) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
var cc = {
//@[0:3) Identifier |var|
//@[4:6) Identifier |cc|
//@[7:8) Assignment |=|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  dd: dd
//@[2:4) Identifier |dd|
//@[4:5) Colon |:|
//@[6:8) Identifier |dd|
//@[8:10) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
var dd = {
//@[0:3) Identifier |var|
//@[4:6) Identifier |dd|
//@[7:8) Assignment |=|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  aa: aa
//@[2:4) Identifier |aa|
//@[4:5) Colon |:|
//@[6:8) Identifier |aa|
//@[8:10) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// variable completion cycles
//@[29:31) NewLine |\r\n|
var one = {
//@[0:3) Identifier |var|
//@[4:7) Identifier |one|
//@[8:9) Assignment |=|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
  first: two
//@[2:7) Identifier |first|
//@[7:8) Colon |:|
//@[9:12) Identifier |two|
//@[12:14) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(15) -> empty
//@[31:33) NewLine |\r\n|
var two = one.f
//@[0:3) Identifier |var|
//@[4:7) Identifier |two|
//@[8:9) Assignment |=|
//@[10:13) Identifier |one|
//@[13:14) Dot |.|
//@[14:15) Identifier |f|
//@[15:17) NewLine |\r\n|
// #completionTest(17) -> empty
//@[31:33) NewLine |\r\n|
var twotwo = one.
//@[0:3) Identifier |var|
//@[4:10) Identifier |twotwo|
//@[11:12) Assignment |=|
//@[13:16) Identifier |one|
//@[16:17) Dot |.|
//@[17:21) NewLine |\r\n\r\n|

// resource completion cycles
//@[29:31) NewLine |\r\n|
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8) Identifier |resource|
//@[9:13) Identifier |res1|
//@[14:60) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  // #completionTest(14) -> empty
//@[33:35) NewLine |\r\n|
  name: res2.n
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |res2|
//@[12:13) Dot |.|
//@[13:14) Identifier |n|
//@[14:16) NewLine |\r\n|
  location: 'l'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:15) StringComplete |'l'|
//@[15:17) NewLine |\r\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
    name: 'Premium_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:23) StringComplete |'Premium_LRS'|
//@[23:25) NewLine |\r\n|
    // #completionTest(15) -> empty
//@[35:37) NewLine |\r\n|
    tier: res2.
//@[4:8) Identifier |tier|
//@[8:9) Colon |:|
//@[10:14) Identifier |res2|
//@[14:15) Dot |.|
//@[15:17) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
resource res2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8) Identifier |resource|
//@[9:13) Identifier |res2|
//@[14:60) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  name: res1.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |res1|
//@[12:13) Dot |.|
//@[13:17) Identifier |name|
//@[17:19) NewLine |\r\n|
  location: 'l'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:15) StringComplete |'l'|
//@[15:17) NewLine |\r\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
    name: 'Premium_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:23) StringComplete |'Premium_LRS'|
//@[23:25) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    // #completionTest(21) -> empty
//@[35:37) NewLine |\r\n|
    accessTier: res1.
//@[4:14) Identifier |accessTier|
//@[14:15) Colon |:|
//@[16:20) Identifier |res1|
//@[20:21) Dot |.|
//@[21:23) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:1) EndOfFile ||
