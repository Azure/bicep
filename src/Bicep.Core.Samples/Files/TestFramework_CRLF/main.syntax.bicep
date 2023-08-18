test sample 'samples/sample1.bicep' = {
//@[00:896) ProgramSyntax
//@[00:084) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |sample|
//@[12:035) | ├─StringSyntax
//@[12:035) | | └─Token(StringComplete) |'samples/sample1.bicep'|
//@[36:037) | ├─Token(Assignment) |=|
//@[38:084) | └─ObjectSyntax
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
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// Test the main file
//@[21:023) ├─Token(NewLine) |\r\n|
test testMain 'samples/main.bicep' = {  
//@[00:095) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:013) | ├─IdentifierSyntax
//@[05:013) | | └─Token(Identifier) |testMain|
//@[14:034) | ├─StringSyntax
//@[14:034) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[35:036) | ├─Token(Assignment) |=|
//@[37:095) | └─ObjectSyntax
//@[37:038) |   ├─Token(LeftBrace) |{|
//@[40:042) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:050) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:050) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
    env: 'prod'
//@[04:015) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |env|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:015) |   |   | └─StringSyntax
//@[09:015) |   |   |   └─Token(StringComplete) |'prod'|
//@[15:017) |   |   ├─Token(NewLine) |\r\n|
    suffix: 1
//@[04:013) |   |   ├─ObjectPropertySyntax
//@[04:010) |   |   | ├─IdentifierSyntax
//@[04:010) |   |   | | └─Token(Identifier) |suffix|
//@[10:011) |   |   | ├─Token(Colon) |:|
//@[12:013) |   |   | └─IntegerLiteralSyntax
//@[12:013) |   |   |   └─Token(Integer) |1|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

test testMain2 'samples/main.bicep' = {  
//@[00:096) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |testMain2|
//@[15:035) | ├─StringSyntax
//@[15:035) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[36:037) | ├─Token(Assignment) |=|
//@[38:096) | └─ObjectSyntax
//@[38:039) |   ├─Token(LeftBrace) |{|
//@[41:043) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:050) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:050) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
    env: 'dev'
//@[04:014) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |env|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:014) |   |   | └─StringSyntax
//@[09:014) |   |   |   └─Token(StringComplete) |'dev'|
//@[14:016) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:014) |   |   ├─ObjectPropertySyntax
//@[04:010) |   |   | ├─IdentifierSyntax
//@[04:010) |   |   | | └─Token(Identifier) |suffix|
//@[10:011) |   |   | ├─Token(Colon) |:|
//@[12:014) |   |   | └─IntegerLiteralSyntax
//@[12:014) |   |   |   └─Token(Integer) |10|
//@[14:016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

test testMain21 'samples/main.bicep' = {  
//@[00:098) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:015) | ├─IdentifierSyntax
//@[05:015) | | └─Token(Identifier) |testMain21|
//@[16:036) | ├─StringSyntax
//@[16:036) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[37:038) | ├─Token(Assignment) |=|
//@[39:098) | └─ObjectSyntax
//@[39:040) |   ├─Token(LeftBrace) |{|
//@[42:044) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:051) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:051) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
    env: 'main'
//@[04:015) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |env|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:015) |   |   | └─StringSyntax
//@[09:015) |   |   |   └─Token(StringComplete) |'main'|
//@[15:017) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:014) |   |   ├─ObjectPropertySyntax
//@[04:010) |   |   | ├─IdentifierSyntax
//@[04:010) |   |   | | └─Token(Identifier) |suffix|
//@[10:011) |   |   | ├─Token(Colon) |:|
//@[12:014) |   |   | └─IntegerLiteralSyntax
//@[12:014) |   |   |   └─Token(Integer) |10|
//@[14:016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

test testMain3 'samples/main.bicep' = {  
//@[00:100) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |testMain3|
//@[15:035) | ├─StringSyntax
//@[15:035) | | └─Token(StringComplete) |'samples/main.bicep'|
//@[36:037) | ├─Token(Assignment) |=|
//@[38:100) | └─ObjectSyntax
//@[38:039) |   ├─Token(LeftBrace) |{|
//@[41:043) |   ├─Token(NewLine) |\r\n|
  params: {  
//@[02:054) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:054) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[13:015) |   |   ├─Token(NewLine) |\r\n|
    env: 'NotMain'
//@[04:018) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |env|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:018) |   |   | └─StringSyntax
//@[09:018) |   |   |   └─Token(StringComplete) |'NotMain'|
//@[18:020) |   |   ├─Token(NewLine) |\r\n|
    suffix: 10
//@[04:014) |   |   ├─ObjectPropertySyntax
//@[04:010) |   |   | ├─IdentifierSyntax
//@[04:010) |   |   | | └─Token(Identifier) |suffix|
//@[10:011) |   |   | ├─Token(Colon) |:|
//@[12:014) |   |   | └─IntegerLiteralSyntax
//@[12:014) |   |   |   └─Token(Integer) |10|
//@[14:016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// Test the development file
//@[28:030) ├─Token(NewLine) |\r\n|
test testDev 'samples/development.bicep' = {
//@[00:090) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:012) | ├─IdentifierSyntax
//@[05:012) | | └─Token(Identifier) |testDev|
//@[13:040) | ├─StringSyntax
//@[13:040) | | └─Token(StringComplete) |'samples/development.bicep'|
//@[41:042) | ├─Token(Assignment) |=|
//@[43:090) | └─ObjectSyntax
//@[43:044) |   ├─Token(LeftBrace) |{|
//@[44:046) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:041) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:041) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[11:013) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus3'
//@[04:023) |   |   ├─ObjectPropertySyntax
//@[04:012) |   |   | ├─IdentifierSyntax
//@[04:012) |   |   | | └─Token(Identifier) |location|
//@[12:013) |   |   | ├─Token(Colon) |:|
//@[14:023) |   |   | └─StringSyntax
//@[14:023) |   |   |   └─Token(StringComplete) |'westus3'|
//@[23:025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// Test the file trying to access a resource
//@[44:048) ├─Token(NewLine) |\r\n\r\n|

test testResource2 'samples/AccessResources.bicep' = {
//@[00:100) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:018) | ├─IdentifierSyntax
//@[05:018) | | └─Token(Identifier) |testResource2|
//@[19:050) | ├─StringSyntax
//@[19:050) | | └─Token(StringComplete) |'samples/AccessResources.bicep'|
//@[51:052) | ├─Token(Assignment) |=|
//@[53:100) | └─ObjectSyntax
//@[53:054) |   ├─Token(LeftBrace) |{|
//@[54:056) |   ├─Token(NewLine) |\r\n|
  params: {
//@[02:041) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:041) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[11:013) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus2'
//@[04:023) |   |   ├─ObjectPropertySyntax
//@[04:012) |   |   | ├─IdentifierSyntax
//@[04:012) |   |   | | └─Token(Identifier) |location|
//@[12:013) |   |   | ├─Token(Colon) |:|
//@[14:023) |   |   | └─StringSyntax
//@[14:023) |   |   |   └─Token(StringComplete) |'westus2'|
//@[23:025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:007) ├─Token(NewLine) |\r\n\r\n\r\n|


// Test the file trying to access runtime functions
//@[51:053) ├─Token(NewLine) |\r\n|
test testRuntime 'samples/runtime.bicep' = {}
//@[00:045) ├─TestDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |test|
//@[05:016) | ├─IdentifierSyntax
//@[05:016) | | └─Token(Identifier) |testRuntime|
//@[17:040) | ├─StringSyntax
//@[17:040) | | └─Token(StringComplete) |'samples/runtime.bicep'|
//@[41:042) | ├─Token(Assignment) |=|
//@[43:045) | └─ObjectSyntax
//@[43:044) |   ├─Token(LeftBrace) |{|
//@[44:045) |   └─Token(RightBrace) |}|
//@[45:049) ├─Token(NewLine) |\r\n\r\n|


//@[00:000) └─Token(EndOfFile) ||
