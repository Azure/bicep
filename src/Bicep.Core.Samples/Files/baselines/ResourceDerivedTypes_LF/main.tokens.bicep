type foo = resource<'Microsoft.Storage/storageAccounts@2023-01-01'>
//@[00:04) Identifier |type|
//@[05:08) Identifier |foo|
//@[09:10) Assignment |=|
//@[11:19) Identifier |resource|
//@[19:20) LeftChevron |<|
//@[20:66) StringComplete |'Microsoft.Storage/storageAccounts@2023-01-01'|
//@[66:67) RightChevron |>|
//@[67:69) NewLine |\n\n|

param bar resource<'Microsoft.Resources/tags@2022-09-01'> = {
//@[00:05) Identifier |param|
//@[06:09) Identifier |bar|
//@[10:18) Identifier |resource|
//@[18:19) LeftChevron |<|
//@[19:56) StringComplete |'Microsoft.Resources/tags@2022-09-01'|
//@[56:57) RightChevron |>|
//@[58:59) Assignment |=|
//@[60:61) LeftBrace |{|
//@[61:62) NewLine |\n|
  name: 'default'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:17) StringComplete |'default'|
//@[17:18) NewLine |\n|
  properties: {
//@[02:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    tags: {
//@[04:08) Identifier |tags|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
      fizz: 'buzz'
//@[06:10) Identifier |fizz|
//@[10:11) Colon |:|
//@[12:18) StringComplete |'buzz'|
//@[18:19) NewLine |\n|
      snap: 'crackle'
//@[06:10) Identifier |snap|
//@[10:11) Colon |:|
//@[12:21) StringComplete |'crackle'|
//@[21:22) NewLine |\n|
    }
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

output baz resource<'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'> = {
//@[00:06) Identifier |output|
//@[07:10) Identifier |baz|
//@[11:19) Identifier |resource|
//@[19:20) LeftChevron |<|
//@[20:81) StringComplete |'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31'|
//@[81:82) RightChevron |>|
//@[83:84) Assignment |=|
//@[85:86) LeftBrace |{|
//@[86:87) NewLine |\n|
  name: 'myId'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:14) StringComplete |'myId'|
//@[14:15) NewLine |\n|
  location: 'eastus'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:21) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
