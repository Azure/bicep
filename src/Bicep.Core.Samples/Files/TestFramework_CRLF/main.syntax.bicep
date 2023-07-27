test sample 'samples/sample1.bicep' = {
//@[00:2011) ProgramSyntax
//@[00:0084) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0011) | ├─IdentifierSyntax
//@[05:0011) | | └─Token(Identifier) |sample|
//@[12:0035) | ├─StringSyntax
//@[12:0035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0084) | └─ObjectSyntax
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
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0007) ├─Token(NewLine) |\r\n\r\n\r\n|


// Test the main file
//@[21:0023) ├─Token(NewLine) |\r\n|
test testMain 'samples/main.bicep' = {  
//@[00:0095) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0013) | ├─IdentifierSyntax
//@[05:0013) | | └─Token(Identifier) |testMain|
//@[14:0034) | ├─StringSyntax
//@[14:0034) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:0095) | └─ObjectSyntax
//@[37:0038) |   ├─Token(LeftBrace) |{|
//@[40:0042) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:0050) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0050) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
    env: 'prod'
//@[04:0015) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0015) |   |   | └─StringSyntax
//@[09:0015) |   |   |   └─Token(StringComplete) |'prod'|
//@[15:0017) |   |   ├─Token(NewLine) |\r\n|
    suffix: 1
//@[04:0013) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |suffix|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0013) |   |   | └─IntegerLiteralSyntax
//@[12:0013) |   |   |   └─Token(Integer) |1|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testMain2 'samples/main.bicep' = {  
//@[00:0096) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |testMain2|
//@[15:0035) | ├─StringSyntax
//@[15:0035) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0096) | └─ObjectSyntax
//@[38:0039) |   ├─Token(LeftBrace) |{|
//@[41:0043) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:0050) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0050) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
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
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

test testMain2 'samples/main.bicep' = {  
//@[00:0097) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |testMain2|
//@[15:0035) | ├─StringSyntax
//@[15:0035) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0097) | └─ObjectSyntax
//@[38:0039) |   ├─Token(LeftBrace) |{|
//@[41:0043) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:0051) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0051) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
    env: 'main'
//@[04:0015) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0015) |   |   | └─StringSyntax
//@[09:0015) |   |   |   └─Token(StringComplete) |'main'|
//@[15:0017) |   |   ├─Token(NewLine) |\r\n|
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

test testMain3 'samples/main.bicep' = {  
//@[00:0100) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0014) | ├─IdentifierSyntax
//@[05:0014) | | └─Token(Identifier) |testMain3|
//@[15:0035) | ├─StringSyntax
//@[15:0035) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[36:0037) | ├─Token(Assignment) |=|
//@[38:0100) | └─ObjectSyntax
//@[38:0039) |   ├─Token(LeftBrace) |{|
//@[41:0043) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:0054) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0054) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[13:0015) |   |   ├─Token(NewLine) |\r\n|
    env: 'NotMain'
//@[04:0018) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |env|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0018) |   |   | └─StringSyntax
//@[09:0018) |   |   |   └─Token(StringComplete) |'NotMain'|
//@[18:0020) |   |   ├─Token(NewLine) |\r\n|
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

test testShouldIgnoreAdditionalProperties 'samples/main.bicep' = {
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

// Test the development file
//@[28:0032) ├─Token(NewLine) |\r\n\r\n|

test testDev 'samples/development.bicep' = {
//@[00:0090) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0012) | ├─IdentifierSyntax
//@[05:0012) | | └─Token(Identifier) |testDev|
//@[13:0040) | ├─StringSyntax
//@[13:0040) | | └─Token(StringComplete) |'samples/development.bicep'|
//@[41:0042) | ├─Token(Assignment) |=|
//@[43:0090) | └─ObjectSyntax
//@[43:0044) |   ├─Token(LeftBrace) |{|
//@[44:0046) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0041) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0013) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus3'
//@[04:0023) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |location|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0023) |   |   | └─StringSyntax
//@[14:0023) |   |   |   └─Token(StringComplete) |'westus3'|
//@[23:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// Test the broken file
//@[23:0027) ├─Token(NewLine) |\r\n\r\n|

test testBroken 'samples/broken.bicep' = {
//@[00:0083) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0015) | ├─IdentifierSyntax
//@[05:0015) | | └─Token(Identifier) |testBroken|
//@[16:0038) | ├─StringSyntax
//@[16:0038) | | └─Token(StringComplete) |'samples/broken.bicep'|
//@[39:0040) | ├─Token(Assignment) |=|
//@[41:0083) | └─ObjectSyntax
//@[41:0042) |   ├─Token(LeftBrace) |{|
//@[42:0044) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0036) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0013) |   |   ├─Token(NewLine) |\r\n|
    location: 'us'
//@[04:0018) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |location|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0018) |   |   | └─StringSyntax
//@[14:0018) |   |   |   └─Token(StringComplete) |'us'|
//@[18:0020) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// Test the file trying to access a resource
//@[44:0048) ├─Token(NewLine) |\r\n\r\n|

test testResource2 'samples/AccessResource.bicep' = {
//@[00:0099) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0018) | ├─IdentifierSyntax
//@[05:0018) | | └─Token(Identifier) |testResource2|
//@[19:0049) | ├─StringSyntax
//@[19:0049) | | └─Token(StringComplete) |'samples/AccessResource.bicep'|
//@[50:0051) | ├─Token(Assignment) |=|
//@[52:0099) | └─ObjectSyntax
//@[52:0053) |   ├─Token(LeftBrace) |{|
//@[53:0055) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0041) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0013) |   |   ├─Token(NewLine) |\r\n|
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
//@[01:0007) ├─Token(NewLine) |\r\n\r\n\r\n|


// Test the file trying to access runtime functions
//@[51:0053) ├─Token(NewLine) |\r\n|
test testRuntime 'samples/runtime.bicep' = {}
//@[00:0045) ├─TestDeclarationSyntax
//@[00:0004) | ├─Token(Identifier) |test|
//@[05:0016) | ├─IdentifierSyntax
//@[05:0016) | | └─Token(Identifier) |testRuntime|
//@[17:0040) | ├─StringSyntax
//@[17:0040) | | └─Token(StringComplete) |'samples/runtime.bicep'|
//@[41:0042) | ├─Token(Assignment) |=|
//@[43:0045) | └─ObjectSyntax
//@[43:0044) |   ├─Token(LeftBrace) |{|
//@[44:0045) |   └─Token(RightBrace) |}|
//@[45:0049) ├─Token(NewLine) |\r\n\r\n|

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
//@[55:0057) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
