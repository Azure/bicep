test sample 'samples/sample1.bicep' = {
//@[00:845) ProgramSyntax
//@[00:174) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:035) | ├─StringSyntax
//@[12:035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:037) | ├─Token(Assignment) |=|
//@[38:174) | └─ObjectSyntax
//@[38:039) |   ├─Token(LeftBrace) |{|
//@[39:041) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:040) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:040) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[11:013) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus'
//@[04:022) |   |   ├─ObjectPropertySyntax
//@[04:012) |   |   | ├─IdentifierSyntax
//@[04:012) |   |   | | └─Token(Identifier) |location|
//@[12:013) |   |   | ├─Token(Colon) |:|
//@[14:022) |   |   | └─StringSyntax
//@[14:022) |   |   |   └─Token(StringComplete) |'westus'|
//@[22:024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:007) |   ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep'{
//@[00:036) |   ├─ObjectPropertySyntax
//@[00:004) |   | ├─IdentifierSyntax
//@[00:004) |   | | └─Token(Identifier) |test|
//@[05:036) |   | ├─SkippedTriviaSyntax
//@[05:011) |   | | ├─Token(Identifier) |sample|
//@[12:035) |   | | ├─Token(StringComplete) |'samples/sample1.bicep'|
//@[35:036) |   | | └─Token(LeftBrace) |{|
//@[36:036) |   | └─SkippedTriviaSyntax
//@[36:038) |   ├─Token(NewLine) |\r\n|
    params: {
//@[04:046) |   ├─ObjectPropertySyntax
//@[04:010) |   | ├─IdentifierSyntax
//@[04:010) |   | | └─Token(Identifier) |params|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:046) |   | └─ObjectSyntax
//@[12:013) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
      location: 'westus'
//@[06:024) |   |   ├─ObjectPropertySyntax
//@[06:014) |   |   | ├─IdentifierSyntax
//@[06:014) |   |   | | └─Token(Identifier) |location|
//@[14:015) |   |   | ├─Token(Colon) |:|
//@[16:024) |   |   | └─StringSyntax
//@[16:024) |   |   |   └─Token(StringComplete) |'westus'|
//@[24:026) |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:005) |   |   └─Token(RightBrace) |}|
//@[05:007) |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test sample ={
//@[00:014) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:014) | ├─SkippedTriviaSyntax
//@[12:013) | | ├─Token(Assignment) |=|
//@[13:014) | | └─Token(LeftBrace) |{|
//@[14:014) | ├─SkippedTriviaSyntax
//@[14:014) | └─SkippedTriviaSyntax
//@[14:016) ├─Token(NewLine) |\r\n|
    params: {
//@[04:013) ├─SkippedTriviaSyntax
//@[04:010) | ├─Token(Identifier) |params|
//@[10:011) | ├─Token(Colon) |:|
//@[12:013) | └─Token(LeftBrace) |{|
//@[13:015) ├─Token(NewLine) |\r\n|
      location: 'westus'
//@[06:024) ├─SkippedTriviaSyntax
//@[06:014) | ├─Token(Identifier) |location|
//@[14:015) | ├─Token(Colon) |:|
//@[16:024) | └─Token(StringComplete) |'westus'|
//@[24:026) ├─Token(NewLine) |\r\n|
    }
//@[04:005) ├─SkippedTriviaSyntax
//@[04:005) | └─Token(RightBrace) |}|
//@[05:007) ├─Token(NewLine) |\r\n|
  }
//@[02:003) ├─SkippedTriviaSyntax
//@[02:003) | └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep'{
//@[00:090) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:035) | ├─StringSyntax
//@[12:035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[35:035) | ├─SkippedTriviaSyntax
//@[35:090) | └─ObjectSyntax
//@[35:036) |   ├─Token(LeftBrace) |{|
//@[36:038) |   ├─Token(NewLine) |\r\n|
    params: {
//@[04:047) |   ├─ObjectPropertySyntax
//@[04:010) |   | ├─IdentifierSyntax
//@[04:010) |   | | └─Token(Identifier) |params|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:047) |   | └─ObjectSyntax
//@[12:013) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:024) |   |   ├─ObjectPropertySyntax
//@[06:014) |   |   | ├─IdentifierSyntax
//@[06:014) |   |   | | └─Token(Identifier) |location|
//@[14:015) |   |   | ├─Token(Colon) |:|
//@[16:024) |   |   | └─StringSyntax
//@[16:024) |   |   |   └─Token(StringComplete) |'westus'|
//@[24:025) |   |   ├─Token(Comma) |,|
//@[25:025) |   |   ├─SkippedTriviaSyntax
//@[25:027) |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:005) |   |   └─Token(RightBrace) |}|
//@[05:007) |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test sample{
//@[00:012) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[11:012) | ├─SkippedTriviaSyntax
//@[11:012) | | └─Token(LeftBrace) |{|
//@[12:012) | ├─SkippedTriviaSyntax
//@[12:012) | └─SkippedTriviaSyntax
//@[12:014) ├─Token(NewLine) |\r\n|
    params: {
//@[04:013) ├─SkippedTriviaSyntax
//@[04:010) | ├─Token(Identifier) |params|
//@[10:011) | ├─Token(Colon) |:|
//@[12:013) | └─Token(LeftBrace) |{|
//@[13:015) ├─Token(NewLine) |\r\n|
      location: 'westus'
//@[06:024) ├─SkippedTriviaSyntax
//@[06:014) | ├─Token(Identifier) |location|
//@[14:015) | ├─Token(Colon) |:|
//@[16:024) | └─Token(StringComplete) |'westus'|
//@[24:026) ├─Token(NewLine) |\r\n|
    }
//@[04:005) ├─SkippedTriviaSyntax
//@[04:005) | └─Token(RightBrace) |}|
//@[05:007) ├─Token(NewLine) |\r\n|
  }
//@[02:003) ├─SkippedTriviaSyntax
//@[02:003) | └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test sample{
//@[00:012) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[11:012) | ├─SkippedTriviaSyntax
//@[11:012) | | └─Token(LeftBrace) |{|
//@[12:012) | ├─SkippedTriviaSyntax
//@[12:012) | └─SkippedTriviaSyntax
//@[12:014) ├─Token(NewLine) |\r\n|
    params: {
//@[04:013) ├─SkippedTriviaSyntax
//@[04:010) | ├─Token(Identifier) |params|
//@[10:011) | ├─Token(Colon) |:|
//@[12:013) | └─Token(LeftBrace) |{|
//@[13:015) ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:025) ├─SkippedTriviaSyntax
//@[06:014) | ├─Token(Identifier) |location|
//@[14:015) | ├─Token(Colon) |:|
//@[16:024) | ├─Token(StringComplete) |'westus'|
//@[24:025) | └─Token(Comma) |,|
//@[25:027) ├─Token(NewLine) |\r\n|
    },
//@[04:006) ├─SkippedTriviaSyntax
//@[04:005) | ├─Token(RightBrace) |}|
//@[05:006) | └─Token(Comma) |,|
//@[06:008) ├─Token(NewLine) |\r\n|
  }
//@[02:003) ├─SkippedTriviaSyntax
//@[02:003) | └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test sample{
//@[00:012) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[11:012) | ├─SkippedTriviaSyntax
//@[11:012) | | └─Token(LeftBrace) |{|
//@[12:012) | ├─SkippedTriviaSyntax
//@[12:012) | └─SkippedTriviaSyntax
//@[12:014) ├─Token(NewLine) |\r\n|
    params: {
//@[04:013) ├─SkippedTriviaSyntax
//@[04:010) | ├─Token(Identifier) |params|
//@[10:011) | ├─Token(Colon) |:|
//@[12:013) | └─Token(LeftBrace) |{|
//@[13:015) ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:025) ├─SkippedTriviaSyntax
//@[06:014) | ├─Token(Identifier) |location|
//@[14:015) | ├─Token(Colon) |:|
//@[16:024) | ├─Token(StringComplete) |'westus'|
//@[24:025) | └─Token(Comma) |,|
//@[25:027) ├─Token(NewLine) |\r\n|
      env:'prod'
//@[06:016) ├─SkippedTriviaSyntax
//@[06:009) | ├─Token(Identifier) |env|
//@[09:010) | ├─Token(Colon) |:|
//@[10:016) | └─Token(StringComplete) |'prod'|
//@[16:018) ├─Token(NewLine) |\r\n|
    },
//@[04:006) ├─SkippedTriviaSyntax
//@[04:005) | ├─Token(RightBrace) |}|
//@[05:006) | └─Token(Comma) |,|
//@[06:008) ├─Token(NewLine) |\r\n|
  }
//@[02:003) ├─SkippedTriviaSyntax
//@[02:003) | └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test 'samples/sample1.bicep'{
//@[00:102) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:005) | ├─IdentifierSyntax
//@[05:005) | | └─SkippedTriviaSyntax
//@[05:028) | ├─StringSyntax
//@[05:028) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[28:028) | ├─SkippedTriviaSyntax
//@[28:102) | └─ObjectSyntax
//@[28:029) |   ├─Token(LeftBrace) |{|
//@[29:031) |   ├─Token(NewLine) |\r\n|
    params: {
//@[04:065) |   ├─ObjectPropertySyntax
//@[04:010) |   | ├─IdentifierSyntax
//@[04:010) |   | | └─Token(Identifier) |params|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:065) |   | └─ObjectSyntax
//@[12:013) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
      location: 'westus',
//@[06:024) |   |   ├─ObjectPropertySyntax
//@[06:014) |   |   | ├─IdentifierSyntax
//@[06:014) |   |   | | └─Token(Identifier) |location|
//@[14:015) |   |   | ├─Token(Colon) |:|
//@[16:024) |   |   | └─StringSyntax
//@[16:024) |   |   |   └─Token(StringComplete) |'westus'|
//@[24:025) |   |   ├─Token(Comma) |,|
//@[25:025) |   |   ├─SkippedTriviaSyntax
//@[25:027) |   |   ├─Token(NewLine) |\r\n|
      env:'prod'
//@[06:016) |   |   ├─ObjectPropertySyntax
//@[06:009) |   |   | ├─IdentifierSyntax
//@[06:009) |   |   | | └─Token(Identifier) |env|
//@[09:010) |   |   | ├─Token(Colon) |:|
//@[10:016) |   |   | └─StringSyntax
//@[10:016) |   |   |   └─Token(StringComplete) |'prod'|
//@[16:018) |   |   ├─Token(NewLine) |\r\n|
    },
//@[04:005) |   |   └─Token(RightBrace) |}|
//@[05:006) |   ├─Token(Comma) |,|
//@[06:006) |   ├─SkippedTriviaSyntax
//@[06:008) |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   └─Token(RightBrace) |}|
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

test
//@[00:004) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[04:004) | ├─IdentifierSyntax
//@[04:004) | | └─SkippedTriviaSyntax
//@[04:004) | ├─SkippedTriviaSyntax
//@[04:004) | ├─SkippedTriviaSyntax
//@[04:004) | └─SkippedTriviaSyntax
//@[04:008) ├─Token(NewLine) |\r\n\r\n|

test sample
//@[00:011) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[11:011) | ├─SkippedTriviaSyntax
//@[11:011) | ├─SkippedTriviaSyntax
//@[11:011) | └─SkippedTriviaSyntax
//@[11:015) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep'
//@[00:035) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:035) | ├─StringSyntax
//@[12:035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[35:035) | ├─SkippedTriviaSyntax
//@[35:035) | └─SkippedTriviaSyntax
//@[35:039) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep' = 
//@[00:038) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:035) | ├─StringSyntax
//@[12:035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:037) | ├─Token(Assignment) |=|
//@[38:038) | └─SkippedTriviaSyntax
//@[38:042) ├─Token(NewLine) |\r\n\r\n|

test sample 'samples/sample1.bicep' = {
//@[00:063) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:035) | ├─StringSyntax
//@[12:035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:037) | ├─Token(Assignment) |=|
//@[38:063) | └─SkippedTriviaSyntax
//@[38:039) |   ├─Token(LeftBrace) |{|
//@[39:043) |   ├─Token(NewLine) |\r\n\r\n|

test sample '' = {
//@[00:004) |   ├─Token(Identifier) |test|
//@[05:011) |   ├─Token(Identifier) |sample|
//@[12:014) |   ├─Token(StringComplete) |''|
//@[15:016) |   ├─Token(Assignment) |=|
//@[17:018) |   ├─Token(LeftBrace) |{|
//@[18:020) |   └─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
