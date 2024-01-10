type invalid1 = resource
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid1|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:26) NewLine |\n\n|

type invalid2 = resource<>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid2|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:26) RightChevron |>|
//@[26:28) NewLine |\n\n|

type invalid3 = resource<'abc', 'def'>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid3|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:30) StringComplete |'abc'|
//@[30:31) Comma |,|
//@[32:37) StringComplete |'def'|
//@[37:38) RightChevron |>|
//@[38:39) NewLine |\n|
type invalid4 = resource<hello>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid4|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:30) Identifier |hello|
//@[30:31) RightChevron |>|
//@[31:32) NewLine |\n|
type invalid5 = resource<'Microsoft.Storage/storageAccounts'>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid5|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:60) StringComplete |'Microsoft.Storage/storageAccounts'|
//@[60:61) RightChevron |>|
//@[61:62) NewLine |\n|
type invalid6 = resource<'Microsoft.Storage/storageAccounts@'>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid6|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:61) StringComplete |'Microsoft.Storage/storageAccounts@'|
//@[61:62) RightChevron |>|
//@[62:63) NewLine |\n|
type invalid7 = resource<'Microsoft.Storage/storageAccounts@hello'>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid7|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:66) StringComplete |'Microsoft.Storage/storageAccounts@hello'|
//@[66:67) RightChevron |>|
//@[67:68) NewLine |\n|
type invalid8 = resource<'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid8|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:89) StringComplete |'notARealNamespace:Microsoft.Storage/storageAccounts@2022-09-01'|
//@[89:90) RightChevron |>|
//@[90:91) NewLine |\n|
type invalid9 = resource<':Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:04) Identifier |type|
//@[05:13) Identifier |invalid9|
//@[14:15) Assignment |=|
//@[16:24) Identifier |resource|
//@[24:25) LeftChevron |<|
//@[25:72) StringComplete |':Microsoft.Storage/storageAccounts@2022-09-01'|
//@[72:73) RightChevron |>|
//@[73:74) NewLine |\n|
type invalid10 = resource<'abc' 'def'>
//@[00:04) Identifier |type|
//@[05:14) Identifier |invalid10|
//@[15:16) Assignment |=|
//@[17:25) Identifier |resource|
//@[25:26) LeftChevron |<|
//@[26:31) StringComplete |'abc'|
//@[32:37) StringComplete |'def'|
//@[37:38) RightChevron |>|
//@[38:39) NewLine |\n|
type invalid11 = resource<123>
//@[00:04) Identifier |type|
//@[05:14) Identifier |invalid11|
//@[15:16) Assignment |=|
//@[17:25) Identifier |resource|
//@[25:26) LeftChevron |<|
//@[26:29) Integer |123|
//@[29:30) RightChevron |>|
//@[30:31) NewLine |\n|
type invalid12 = resource<resourceGroup()>
//@[00:04) Identifier |type|
//@[05:14) Identifier |invalid12|
//@[15:16) Assignment |=|
//@[17:25) Identifier |resource|
//@[25:26) LeftChevron |<|
//@[26:39) Identifier |resourceGroup|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:42) RightChevron |>|
//@[42:44) NewLine |\n\n|

type thisIsWeird = resource</*
//@[00:04) Identifier |type|
//@[05:16) Identifier |thisIsWeird|
//@[17:18) Assignment |=|
//@[19:27) Identifier |resource|
//@[27:28) LeftChevron |<|
*/'Astronomer.Astro/organizations@2023-08-01-preview'
//@[02:53) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[53:54) NewLine |\n|
///  >
//@[06:07) NewLine |\n|
>
//@[00:01) RightChevron |>|
//@[01:03) NewLine |\n\n|

type interpolated = resource<'Microsoft.${'Storage'}/storageAccounts@2022-09-01'>
//@[00:04) Identifier |type|
//@[05:17) Identifier |interpolated|
//@[18:19) Assignment |=|
//@[20:28) Identifier |resource|
//@[28:29) LeftChevron |<|
//@[29:42) StringLeftPiece |'Microsoft.${|
//@[42:51) StringComplete |'Storage'|
//@[51:80) StringRightPiece |}/storageAccounts@2022-09-01'|
//@[80:81) RightChevron |>|
//@[81:83) NewLine |\n\n|

@sealed()
//@[00:01) At |@|
//@[01:07) Identifier |sealed|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
type shouldNotBeSealable = resource<'Microsoft.Storage/storageAccounts@2022-09-01'>
//@[00:04) Identifier |type|
//@[05:24) Identifier |shouldNotBeSealable|
//@[25:26) Assignment |=|
//@[27:35) Identifier |resource|
//@[35:36) LeftChevron |<|
//@[36:82) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[82:83) RightChevron |>|
//@[83:85) NewLine |\n\n|

type hello = {
//@[00:04) Identifier |type|
//@[05:10) Identifier |hello|
//@[11:12) Assignment |=|
//@[13:14) LeftBrace |{|
//@[14:15) NewLine |\n|
  @discriminator('hi')
//@[02:03) At |@|
//@[03:16) Identifier |discriminator|
//@[16:17) LeftParen |(|
//@[17:21) StringComplete |'hi'|
//@[21:22) RightParen |)|
//@[22:23) NewLine |\n|
  bar: resource<'Astronomer.Astro/organizations@2023-08-01-preview'>
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:15) Identifier |resource|
//@[15:16) LeftChevron |<|
//@[16:67) StringComplete |'Astronomer.Astro/organizations@2023-08-01-preview'|
//@[67:68) RightChevron |>|
//@[68:69) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
