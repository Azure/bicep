targetScope = 'subscription'
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:28) StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

param location string = 'eastus'
//@[00:05) Identifier |param|
//@[06:14) Identifier |location|
//@[15:21) Identifier |string|
//@[22:23) Assignment |=|
//@[24:32) StringComplete |'eastus'|
//@[32:34) NewLine |\n\n|

// REP 0015: with the 'formalizedScope' experimental feature enabled, this module's cross-scope
//@[95:96) NewLine |\n|
// targeting is emitted as a single duck-typed "@scope" object instead of the legacy
//@[84:85) NewLine |\n|
// "subscriptionId" / "resourceGroup" properties.
//@[49:50) NewLine |\n|
module storageMod 'modules/mod.bicep' = {
//@[00:06) Identifier |module|
//@[07:17) Identifier |storageMod|
//@[18:37) StringComplete |'modules/mod.bicep'|
//@[38:39) Assignment |=|
//@[40:41) LeftBrace |{|
//@[41:42) NewLine |\n|
  name: 'storageMod'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:20) StringComplete |'storageMod'|
//@[20:21) NewLine |\n|
  scope: resourceGroup('my-rg')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:30) StringComplete |'my-rg'|
//@[30:31) RightParen |)|
//@[31:32) NewLine |\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    location: location
//@[04:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) Identifier |location|
//@[22:23) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

module storageMod2 'modules/mod.bicep' = {
//@[00:06) Identifier |module|
//@[07:18) Identifier |storageMod2|
//@[19:38) StringComplete |'modules/mod.bicep'|
//@[39:40) Assignment |=|
//@[41:42) LeftBrace |{|
//@[42:43) NewLine |\n|
  name: 'storageMod2'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:21) StringComplete |'storageMod2'|
//@[21:22) NewLine |\n|
  scope: location != 'eastus' ? resourceGroup() : resourceGroup('my-rg')
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:17) Identifier |location|
//@[18:20) NotEquals |!=|
//@[21:29) StringComplete |'eastus'|
//@[30:31) Question |?|
//@[32:45) Identifier |resourceGroup|
//@[45:46) LeftParen |(|
//@[46:47) RightParen |)|
//@[48:49) Colon |:|
//@[50:63) Identifier |resourceGroup|
//@[63:64) LeftParen |(|
//@[64:71) StringComplete |'my-rg'|
//@[71:72) RightParen |)|
//@[72:73) NewLine |\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    location: location
//@[04:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) Identifier |location|
//@[22:23) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

output loc string = storageMod.outputs.loc
//@[00:06) Identifier |output|
//@[07:10) Identifier |loc|
//@[11:17) Identifier |string|
//@[18:19) Assignment |=|
//@[20:30) Identifier |storageMod|
//@[30:31) Dot |.|
//@[31:38) Identifier |outputs|
//@[38:39) Dot |.|
//@[39:42) Identifier |loc|
//@[42:43) NewLine |\n|

//@[00:00) EndOfFile ||
