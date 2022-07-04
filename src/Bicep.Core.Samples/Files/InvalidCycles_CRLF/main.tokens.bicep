
//@[00:02) NewLine |\r\n|
//self-cycle
//@[12:14) NewLine |\r\n|
var x = x
//@[00:03) Identifier |var|
//@[04:05) Identifier |x|
//@[06:07) Assignment |=|
//@[08:09) Identifier |x|
//@[09:11) NewLine |\r\n|
var q = base64(q, !q)
//@[00:03) Identifier |var|
//@[04:05) Identifier |q|
//@[06:07) Assignment |=|
//@[08:14) Identifier |base64|
//@[14:15) LeftParen |(|
//@[15:16) Identifier |q|
//@[16:17) Comma |,|
//@[18:19) Exclamation |!|
//@[19:20) Identifier |q|
//@[20:21) RightParen |)|
//@[21:25) NewLine |\r\n\r\n|

//2-cycle
//@[09:11) NewLine |\r\n|
var a = b
//@[00:03) Identifier |var|
//@[04:05) Identifier |a|
//@[06:07) Assignment |=|
//@[08:09) Identifier |b|
//@[09:11) NewLine |\r\n|
var b = max(a,1)
//@[00:03) Identifier |var|
//@[04:05) Identifier |b|
//@[06:07) Assignment |=|
//@[08:11) Identifier |max|
//@[11:12) LeftParen |(|
//@[12:13) Identifier |a|
//@[13:14) Comma |,|
//@[14:15) Integer |1|
//@[15:16) RightParen |)|
//@[16:20) NewLine |\r\n\r\n|

//3-cycle
//@[09:11) NewLine |\r\n|
var e = f
//@[00:03) Identifier |var|
//@[04:05) Identifier |e|
//@[06:07) Assignment |=|
//@[08:09) Identifier |f|
//@[09:11) NewLine |\r\n|
var f = g && true
//@[00:03) Identifier |var|
//@[04:05) Identifier |f|
//@[06:07) Assignment |=|
//@[08:09) Identifier |g|
//@[10:12) LogicalAnd |&&|
//@[13:17) TrueKeyword |true|
//@[17:19) NewLine |\r\n|
var g = e ? e : e
//@[00:03) Identifier |var|
//@[04:05) Identifier |g|
//@[06:07) Assignment |=|
//@[08:09) Identifier |e|
//@[10:11) Question |?|
//@[12:13) Identifier |e|
//@[14:15) Colon |:|
//@[16:17) Identifier |e|
//@[17:21) NewLine |\r\n\r\n|

//4-cycle
//@[09:11) NewLine |\r\n|
var aa = {
//@[00:03) Identifier |var|
//@[04:06) Identifier |aa|
//@[07:08) Assignment |=|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  bb: bb
//@[02:04) Identifier |bb|
//@[04:05) Colon |:|
//@[06:08) Identifier |bb|
//@[08:10) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
var bb = {
//@[00:03) Identifier |var|
//@[04:06) Identifier |bb|
//@[07:08) Assignment |=|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  cc: cc
//@[02:04) Identifier |cc|
//@[04:05) Colon |:|
//@[06:08) Identifier |cc|
//@[08:10) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
var cc = {
//@[00:03) Identifier |var|
//@[04:06) Identifier |cc|
//@[07:08) Assignment |=|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  dd: dd
//@[02:04) Identifier |dd|
//@[04:05) Colon |:|
//@[06:08) Identifier |dd|
//@[08:10) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
var dd = {
//@[00:03) Identifier |var|
//@[04:06) Identifier |dd|
//@[07:08) Assignment |=|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  aa: aa
//@[02:04) Identifier |aa|
//@[04:05) Colon |:|
//@[06:08) Identifier |aa|
//@[08:10) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// variable completion cycles
//@[29:31) NewLine |\r\n|
var one = {
//@[00:03) Identifier |var|
//@[04:07) Identifier |one|
//@[08:09) Assignment |=|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
  first: two
//@[02:07) Identifier |first|
//@[07:08) Colon |:|
//@[09:12) Identifier |two|
//@[12:14) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
// #completionTest(15) -> empty
//@[31:33) NewLine |\r\n|
var two = one.f
//@[00:03) Identifier |var|
//@[04:07) Identifier |two|
//@[08:09) Assignment |=|
//@[10:13) Identifier |one|
//@[13:14) Dot |.|
//@[14:15) Identifier |f|
//@[15:17) NewLine |\r\n|
// #completionTest(17) -> empty
//@[31:33) NewLine |\r\n|
var twotwo = one.
//@[00:03) Identifier |var|
//@[04:10) Identifier |twotwo|
//@[11:12) Assignment |=|
//@[13:16) Identifier |one|
//@[16:17) Dot |.|
//@[17:21) NewLine |\r\n\r\n|

// resource completion cycles
//@[29:31) NewLine |\r\n|
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[00:08) Identifier |resource|
//@[09:13) Identifier |res1|
//@[14:60) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  // #completionTest(14) -> empty
//@[33:35) NewLine |\r\n|
  name: res2.n
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:12) Identifier |res2|
//@[12:13) Dot |.|
//@[13:14) Identifier |n|
//@[14:16) NewLine |\r\n|
  location: 'l'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:15) StringComplete |'l'|
//@[15:17) NewLine |\r\n|
  sku: {
//@[02:05) Identifier |sku|
//@[05:06) Colon |:|
//@[07:08) LeftBrace |{|
//@[08:10) NewLine |\r\n|
    name: 'Premium_LRS'
//@[04:08) Identifier |name|
//@[08:09) Colon |:|
//@[10:23) StringComplete |'Premium_LRS'|
//@[23:25) NewLine |\r\n|
    // #completionTest(15) -> empty
//@[35:37) NewLine |\r\n|
    tier: res2.
//@[04:08) Identifier |tier|
//@[08:09) Colon |:|
//@[10:14) Identifier |res2|
//@[14:15) Dot |.|
//@[15:17) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  kind: 'StorageV2'
//@[02:06) Identifier |kind|
//@[06:07) Colon |:|
//@[08:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
resource res2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[00:08) Identifier |resource|
//@[09:13) Identifier |res2|
//@[14:60) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftBrace |{|
//@[64:66) NewLine |\r\n|
  name: res1.name
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:12) Identifier |res1|
//@[12:13) Dot |.|
//@[13:17) Identifier |name|
//@[17:19) NewLine |\r\n|
  location: 'l'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:15) StringComplete |'l'|
//@[15:17) NewLine |\r\n|
  sku: {
//@[02:05) Identifier |sku|
//@[05:06) Colon |:|
//@[07:08) LeftBrace |{|
//@[08:10) NewLine |\r\n|
    name: 'Premium_LRS'
//@[04:08) Identifier |name|
//@[08:09) Colon |:|
//@[10:23) StringComplete |'Premium_LRS'|
//@[23:25) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  properties: {
//@[02:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    // #completionTest(21) -> empty
//@[35:37) NewLine |\r\n|
    accessTier: res1.
//@[04:14) Identifier |accessTier|
//@[14:15) Colon |:|
//@[16:20) Identifier |res1|
//@[20:21) Dot |.|
//@[21:23) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  kind: 'StorageV2'
//@[02:06) Identifier |kind|
//@[06:07) Colon |:|
//@[08:19) StringComplete |'StorageV2'|
//@[19:21) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:01) EndOfFile ||
