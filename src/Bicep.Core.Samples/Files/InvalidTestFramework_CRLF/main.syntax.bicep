test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
//@[00:1852) ProgramSyntax
//@[00:0091) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0041) | ├─IdentifierSyntax
//@[05:0041) | | └─Token(Identifier) |testShouldIgnoreAdditionalProperties|
//@[42:0062) | ├─StringSyntax
//@[42:0062) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[63:0064) | ├─Token(Assignment) |=|
//@[65:0091) | └─ObjectSyntax
//@[65:0066) |   ├─Token(LeftBrace) |{|
//@[66:0068) |   ├─Token(NewLine) |\r\n|
  additionalProp: {}
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0016) |   | ├─IdentifierSyntax
//@[02:0016) |   | | └─Token(Identifier) |additionalProp|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0020) |   | └─ObjectSyntax
//@[18:0019) |   |   ├─Token(LeftBrace) |{|
//@[19:0020) |   |   └─Token(RightBrace) |}|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testShouldIgnoreAdditionalProperties2 'samples/main.bicep' = {
//@[00:0142) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0042) | ├─IdentifierSyntax
//@[05:0042) | | └─Token(Identifier) |testShouldIgnoreAdditionalProperties2|
//@[43:0063) | ├─StringSyntax
//@[43:0063) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[64:0065) | ├─Token(Assignment) |=|
//@[66:0142) | └─ObjectSyntax
//@[66:0067) |   ├─Token(LeftBrace) |{|
//@[67:0069) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:0048) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0048) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0013) |   |   ├─Token(NewLine) |\r\n|
    env: 'dev'
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0014) |   |   | └─StringSyntax
//@[09:0014) |   |   |   └─Token(StringComplete) |'dev'|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |suffix|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0014) |   |   | └─IntegerLiteralSyntax
//@[12:0014) |   |   |   └─Token(Integer) |10|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
  additionalProp: {}
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0016) |   | ├─IdentifierSyntax
//@[02:0016) |   | | └─Token(Identifier) |additionalProp|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0020) |   | └─ObjectSyntax
//@[18:0019) |   |   ├─Token(LeftBrace) |{|
//@[19:0020) |   |   └─Token(RightBrace) |}|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// Skipped tests
//@[16:0018) ├─Token(NewLine) |\r\n|
test testNoParams 'samples/main.bicep' ={
//@[00:0057) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0017) | ├─IdentifierSyntax
//@[05:0017) | | └─Token(Identifier) |testNoParams|
//@[18:0038) | ├─StringSyntax
//@[18:0038) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[39:0040) | ├─Token(Assignment) |=|
//@[40:0057) | └─ObjectSyntax
//@[40:0041) |   ├─Token(LeftBrace) |{|
//@[41:0043) |   ├─Token(NewLine) |\r\n|
  params:{}
//@[02:0011) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[09:0011) |   | └─ObjectSyntax
//@[09:0010) |   |   ├─Token(LeftBrace) |{|
//@[10:0011) |   |   └─Token(RightBrace) |}|
//@[11:0013) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testMissingParams 'samples/main.bicep' ={
//@[00:0086) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0022) | ├─IdentifierSyntax
//@[05:0022) | | └─Token(Identifier) |testMissingParams|
//@[23:0043) | ├─StringSyntax
//@[23:0043) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[44:0045) | ├─Token(Assignment) |=|
//@[45:0086) | └─ObjectSyntax
//@[45:0046) |   ├─Token(LeftBrace) |{|
//@[46:0048) |   ├─Token(NewLine) |\r\n|
  params:{
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[09:0035) |   | └─ObjectSyntax
//@[09:0010) |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   ├─Token(NewLine) |\r\n|
    env: 'NotMain'
//@[04:0018) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0018) |   |   | └─StringSyntax
//@[09:0018) |   |   |   └─Token(StringComplete) |'NotMain'|
//@[18:0020) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testWrongParamsType 'samples/main.bicep' ={
//@[00:0096) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0024) | ├─IdentifierSyntax
//@[05:0024) | | └─Token(Identifier) |testWrongParamsType|
//@[25:0045) | ├─StringSyntax
//@[25:0045) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[46:0047) | ├─Token(Assignment) |=|
//@[47:0096) | └─ObjectSyntax
//@[47:0048) |   ├─Token(LeftBrace) |{|
//@[48:0050) |   ├─Token(NewLine) |\r\n|
  params:{
//@[02:0043) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[09:0043) |   | └─ObjectSyntax
//@[09:0010) |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   ├─Token(NewLine) |\r\n|
    env: 1
//@[04:0010) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0010) |   |   | └─IntegerLiteralSyntax
//@[09:0010) |   |   |   └─Token(Integer) |1|
//@[10:0012) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |suffix|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0014) |   |   | └─IntegerLiteralSyntax
//@[12:0014) |   |   |   └─Token(Integer) |10|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testWrongParamsType2 'samples/main.bicep' ={
//@[00:0103) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0025) | ├─IdentifierSyntax
//@[05:0025) | | └─Token(Identifier) |testWrongParamsType2|
//@[26:0046) | ├─StringSyntax
//@[26:0046) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[47:0048) | ├─Token(Assignment) |=|
//@[48:0103) | └─ObjectSyntax
//@[48:0049) |   ├─Token(LeftBrace) |{|
//@[49:0051) |   ├─Token(NewLine) |\r\n|
  params:{
//@[02:0049) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[09:0049) |   | └─ObjectSyntax
//@[09:0010) |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   ├─Token(NewLine) |\r\n|
    env: 'dev'
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0014) |   |   | └─StringSyntax
//@[09:0014) |   |   |   └─Token(StringComplete) |'dev'|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
    suffix: '10'
//@[04:0016) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |suffix|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0016) |   |   | └─StringSyntax
//@[12:0016) |   |   |   └─Token(StringComplete) |'10'|
//@[16:0018) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testWrongParamsType3 'samples/main.bicep' ={
//@[00:0126) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0025) | ├─IdentifierSyntax
//@[05:0025) | | └─Token(Identifier) |testWrongParamsType3|
//@[26:0046) | ├─StringSyntax
//@[26:0046) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[47:0048) | ├─Token(Assignment) |=|
//@[48:0126) | └─ObjectSyntax
//@[48:0049) |   ├─Token(LeftBrace) |{|
//@[49:0051) |   ├─Token(NewLine) |\r\n|
  params:{
//@[02:0072) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[09:0072) |   | └─ObjectSyntax
//@[09:0010) |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   ├─Token(NewLine) |\r\n|
    env: 'dev'
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0014) |   |   | └─StringSyntax
//@[09:0014) |   |   |   └─Token(StringComplete) |'dev'|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |suffix|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0014) |   |   | └─IntegerLiteralSyntax
//@[12:0014) |   |   |   └─Token(Integer) |10|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus2'
//@[04:0023) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |location|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0023) |   |   | └─StringSyntax
//@[14:0023) |   |   |   └─Token(StringComplete) |'westus2'|
//@[23:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testInexitentParam 'samples/main.bicep' ={
//@[00:0116) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0023) | ├─IdentifierSyntax
//@[05:0023) | | └─Token(Identifier) |testInexitentParam|
//@[24:0044) | ├─StringSyntax
//@[24:0044) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[45:0046) | ├─Token(Assignment) |=|
//@[46:0116) | └─ObjectSyntax
//@[46:0047) |   ├─Token(LeftBrace) |{|
//@[47:0049) |   ├─Token(NewLine) |\r\n|
  params:{
//@[02:0064) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[09:0064) |   | └─ObjectSyntax
//@[09:0010) |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   ├─Token(NewLine) |\r\n|
    env: 'dev'
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0014) |   |   | └─StringSyntax
//@[09:0014) |   |   |   └─Token(StringComplete) |'dev'|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:0014) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |suffix|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0014) |   |   | └─IntegerLiteralSyntax
//@[12:0014) |   |   |   └─Token(Integer) |10|
//@[14:0016) |   |   ├─Token(NewLine) |\r\n|
    location: 1
//@[04:0015) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |location|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0015) |   |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   |   └─Token(Integer) |1|
//@[15:0017) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testEmptyBody 'samples/main.bicep' = {}
//@[00:0044) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0018) | ├─IdentifierSyntax
//@[05:0018) | | └─Token(Identifier) |testEmptyBody|
//@[19:0039) | ├─StringSyntax
//@[19:0039) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[40:0041) | ├─Token(Assignment) |=|
//@[42:0044) | └─ObjectSyntax
//@[42:0043) |   ├─Token(LeftBrace) |{|
//@[43:0044) |   └─Token(RightBrace) |}|
//@[44:0048) ├─Token(NewLine) |\r\n\r\n|

// Test inexistent file
//@[23:0027) ├─Token(NewLine) |\r\n\r\n|

test testInexistentFile 'samples/inexistent.bicep' = {}
//@[00:0055) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0023) | ├─IdentifierSyntax
//@[05:0023) | | └─Token(Identifier) |testInexistentFile|
//@[24:0050) | ├─StringSyntax
//@[24:0050) | | └─Token(StringComplete) |'samples/inexistent.bicep'|
//@[51:0052) | ├─Token(Assignment) |=|
//@[53:0055) | └─ObjectSyntax
//@[53:0054) |   ├─Token(LeftBrace) |{|
//@[54:0055) |   └─Token(RightBrace) |}|
//@[55:0061) ├─Token(NewLine) |\r\n\r\n\r\n|


test sample 'samples/sample1.bicep' = {
//@[00:0174) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0035) | ├─StringSyntax
//@[12:0035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0174) | └─ObjectSyntax
//@[38:0039) |   ├─Token(LeftBrace) |{|
//@[39:0041) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0040) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0013) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus'
//@[04:0022) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |location|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0022) |   |   | └─StringSyntax
//@[14:0022) |   |   |   └─Token(StringComplete) |'westus'|
//@[22:0024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0007) |   ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep'{
//@[00:0036) |   ├─ObjectPropertySyntax
//@[00:0004) |   | ├─IdentifierSyntax
//@[00:0004) |   | | └─Token(Identifier) |test|
//@[05:0036) |   | ├─SkippedTriviaSyntax
//@[05:0011) |   | | ├─Token(Identifier) |sample|
//@[12:0035) |   | | ├─Token(StringComplete) |'samples/sample1.bicep'|
//@[35:0036) |   | | └─Token(LeftBrace) |{|
//@[36:0036) |   | └─SkippedTriviaSyntax
//@[36:0038) |   ├─Token(NewLine) |\r\n|
    params: {
//@[04:0046) |   ├─ObjectPropertySyntax
//@[04:0010) |   | ├─IdentifierSyntax
//@[04:0010) |   | | └─Token(Identifier) |params|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0046) |   | └─ObjectSyntax
//@[12:0013) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
      location: 'westus'
//@[06:0024) |   |   ├─ObjectPropertySyntax
//@[06:0014) |   |   | ├─IdentifierSyntax
//@[06:0014) |   |   | | └─Token(Identifier) |location|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0024) |   |   | └─StringSyntax
//@[16:0024) |   |   |   └─Token(StringComplete) |'westus'|
//@[24:0026) |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   └─Token(RightBrace) |}|
//@[05:0007) |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test sample ={
//@[00:0014) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0014) | ├─SkippedTriviaSyntax
//@[12:0013) | | ├─Token(Assignment) |=|
//@[13:0014) | | └─Token(LeftBrace) |{|
//@[14:0014) | ├─SkippedTriviaSyntax
//@[14:0014) | └─SkippedTriviaSyntax
//@[14:0016) ├─Token(NewLine) |\r\n|
    params: {
//@[04:0013) ├─SkippedTriviaSyntax
//@[04:0010) | ├─Token(Identifier) |params|
//@[10:0011) | ├─Token(Colon) |:|
//@[12:0013) | └─Token(LeftBrace) |{|
//@[13:0015) ├─Token(NewLine) |\r\n|
      location: 'westus'
//@[06:0024) ├─SkippedTriviaSyntax
//@[06:0014) | ├─Token(Identifier) |location|
//@[14:0015) | ├─Token(Colon) |:|
//@[16:0024) | └─Token(StringComplete) |'westus'|
//@[24:0026) ├─Token(NewLine) |\r\n|
    }
//@[04:0005) ├─SkippedTriviaSyntax
//@[04:0005) | └─Token(RightBrace) |}|
//@[05:0007) ├─Token(NewLine) |\r\n|
  }
//@[02:0003) ├─SkippedTriviaSyntax
//@[02:0003) | └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep'{
//@[00:0090) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0035) | ├─StringSyntax
//@[12:0035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[35:0035) | ├─SkippedTriviaSyntax
//@[35:0090) | └─ObjectSyntax
//@[35:0036) |   ├─Token(LeftBrace) |{|
//@[36:0038) |   ├─Token(NewLine) |\r\n|
    params: {
//@[04:0047) |   ├─ObjectPropertySyntax
//@[04:0010) |   | ├─IdentifierSyntax
//@[04:0010) |   | | └─Token(Identifier) |params|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0047) |   | └─ObjectSyntax
//@[12:0013) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:0024) |   |   ├─ObjectPropertySyntax
//@[06:0014) |   |   | ├─IdentifierSyntax
//@[06:0014) |   |   | | └─Token(Identifier) |location|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0024) |   |   | └─StringSyntax
//@[16:0024) |   |   |   └─Token(StringComplete) |'westus'|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[25:0025) |   |   ├─SkippedTriviaSyntax
//@[25:0027) |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   └─Token(RightBrace) |}|
//@[05:0007) |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test sample{
//@[00:0012) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[11:0012) | ├─SkippedTriviaSyntax
//@[11:0012) | | └─Token(LeftBrace) |{|
//@[12:0012) | ├─SkippedTriviaSyntax
//@[12:0012) | └─SkippedTriviaSyntax
//@[12:0014) ├─Token(NewLine) |\r\n|
    params: {
//@[04:0013) ├─SkippedTriviaSyntax
//@[04:0010) | ├─Token(Identifier) |params|
//@[10:0011) | ├─Token(Colon) |:|
//@[12:0013) | └─Token(LeftBrace) |{|
//@[13:0015) ├─Token(NewLine) |\r\n|
      location: 'westus'
//@[06:0024) ├─SkippedTriviaSyntax
//@[06:0014) | ├─Token(Identifier) |location|
//@[14:0015) | ├─Token(Colon) |:|
//@[16:0024) | └─Token(StringComplete) |'westus'|
//@[24:0026) ├─Token(NewLine) |\r\n|
    }
//@[04:0005) ├─SkippedTriviaSyntax
//@[04:0005) | └─Token(RightBrace) |}|
//@[05:0007) ├─Token(NewLine) |\r\n|
  }
//@[02:0003) ├─SkippedTriviaSyntax
//@[02:0003) | └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test sample{
//@[00:0012) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[11:0012) | ├─SkippedTriviaSyntax
//@[11:0012) | | └─Token(LeftBrace) |{|
//@[12:0012) | ├─SkippedTriviaSyntax
//@[12:0012) | └─SkippedTriviaSyntax
//@[12:0014) ├─Token(NewLine) |\r\n|
    params: {
//@[04:0013) ├─SkippedTriviaSyntax
//@[04:0010) | ├─Token(Identifier) |params|
//@[10:0011) | ├─Token(Colon) |:|
//@[12:0013) | └─Token(LeftBrace) |{|
//@[13:0015) ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:0025) ├─SkippedTriviaSyntax
//@[06:0014) | ├─Token(Identifier) |location|
//@[14:0015) | ├─Token(Colon) |:|
//@[16:0024) | ├─Token(StringComplete) |'westus'|
//@[24:0025) | └─Token(Comma) |,|
//@[25:0027) ├─Token(NewLine) |\r\n|
    },
//@[04:0006) ├─SkippedTriviaSyntax
//@[04:0005) | ├─Token(RightBrace) |}|
//@[05:0006) | └─Token(Comma) |,|
//@[06:0008) ├─Token(NewLine) |\r\n|
  }
//@[02:0003) ├─SkippedTriviaSyntax
//@[02:0003) | └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test sample{
//@[00:0012) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[11:0012) | ├─SkippedTriviaSyntax
//@[11:0012) | | └─Token(LeftBrace) |{|
//@[12:0012) | ├─SkippedTriviaSyntax
//@[12:0012) | └─SkippedTriviaSyntax
//@[12:0014) ├─Token(NewLine) |\r\n|
    params: {
//@[04:0013) ├─SkippedTriviaSyntax
//@[04:0010) | ├─Token(Identifier) |params|
//@[10:0011) | ├─Token(Colon) |:|
//@[12:0013) | └─Token(LeftBrace) |{|
//@[13:0015) ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:0025) ├─SkippedTriviaSyntax
//@[06:0014) | ├─Token(Identifier) |location|
//@[14:0015) | ├─Token(Colon) |:|
//@[16:0024) | ├─Token(StringComplete) |'westus'|
//@[24:0025) | └─Token(Comma) |,|
//@[25:0027) ├─Token(NewLine) |\r\n|
      env:'prod'
//@[06:0016) ├─SkippedTriviaSyntax
//@[06:0009) | ├─Token(Identifier) |env|
//@[09:0010) | ├─Token(Colon) |:|
//@[10:0016) | └─Token(StringComplete) |'prod'|
//@[16:0018) ├─Token(NewLine) |\r\n|
    },
//@[04:0006) ├─SkippedTriviaSyntax
//@[04:0005) | ├─Token(RightBrace) |}|
//@[05:0006) | └─Token(Comma) |,|
//@[06:0008) ├─Token(NewLine) |\r\n|
  }
//@[02:0003) ├─SkippedTriviaSyntax
//@[02:0003) | └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test 'samples/sample1.bicep'{
//@[00:0102) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0005) | ├─IdentifierSyntax
//@[05:0005) | | └─SkippedTriviaSyntax
//@[05:0028) | ├─StringSyntax
//@[05:0028) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[28:0028) | ├─SkippedTriviaSyntax
//@[28:0102) | └─ObjectSyntax
//@[28:0029) |   ├─Token(LeftBrace) |{|
//@[29:0031) |   ├─Token(NewLine) |\r\n|
    params: {
//@[04:0065) |   ├─ObjectPropertySyntax
//@[04:0010) |   | ├─IdentifierSyntax
//@[04:0010) |   | | └─Token(Identifier) |params|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0065) |   | └─ObjectSyntax
//@[12:0013) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:0024) |   |   ├─ObjectPropertySyntax
//@[06:0014) |   |   | ├─IdentifierSyntax
//@[06:0014) |   |   | | └─Token(Identifier) |location|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0024) |   |   | └─StringSyntax
//@[16:0024) |   |   |   └─Token(StringComplete) |'westus'|
//@[24:0025) |   |   ├─Token(Comma) |,|
//@[25:0025) |   |   ├─SkippedTriviaSyntax
//@[25:0027) |   |   ├─Token(NewLine) |\r\n|
      env:'prod'
//@[06:0016) |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   | | └─Token(Identifier) |env|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[10:0016) |   |   | └─StringSyntax
//@[10:0016) |   |   |   └─Token(StringComplete) |'prod'|
//@[16:0018) |   |   ├─Token(NewLine) |\r\n|
    },
//@[04:0005) |   |   └─Token(RightBrace) |}|
//@[05:0006) |   ├─Token(Comma) |,|
//@[06:0006) |   ├─SkippedTriviaSyntax
//@[06:0008) |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   └─Token(RightBrace) |}|
//@[03:0007) ├─Token(NewLine) |\r\n\r\n|

test
//@[00:0004) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[04:0004) | ├─IdentifierSyntax
//@[04:0004) | | └─SkippedTriviaSyntax
//@[04:0004) | ├─SkippedTriviaSyntax
//@[04:0004) | ├─SkippedTriviaSyntax
//@[04:0004) | └─SkippedTriviaSyntax
//@[04:0008) ├─Token(NewLine) |\r\n\r\n|

test sample
//@[00:0011) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[11:0011) | ├─SkippedTriviaSyntax
//@[11:0011) | ├─SkippedTriviaSyntax
//@[11:0011) | └─SkippedTriviaSyntax
//@[11:0015) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep'
//@[00:0035) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0035) | ├─StringSyntax
//@[12:0035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[35:0035) | ├─SkippedTriviaSyntax
//@[35:0035) | └─SkippedTriviaSyntax
//@[35:0039) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep' = 
//@[00:0038) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0035) | ├─StringSyntax
//@[12:0035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0038) | └─SkippedTriviaSyntax
//@[38:0042) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep' = {
//@[00:0067) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0035) | ├─StringSyntax
//@[12:0035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0067) | └─SkippedTriviaSyntax
//@[38:0039) |   ├─Token(LeftBrace) |{|
//@[39:0043) |   ├─Token(NewLine) |\r\n\r\n|

test sample '' = {
//@[00:0004) |   ├─Token(Identifier) |test|
//@[05:0011) |   ├─Token(Identifier) |sample|
//@[12:0014) |   ├─Token(StringComplete) |''|
//@[15:0016) |   ├─Token(Assignment) |=|
//@[17:0018) |   ├─Token(LeftBrace) |{|
//@[18:0024) |   └─Token(NewLine) |\r\n\r\n\r\n|



//@[00:0000) └─Token(EndOfFile) ||
