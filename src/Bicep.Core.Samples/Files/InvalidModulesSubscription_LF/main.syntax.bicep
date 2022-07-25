targetScope = 'subscription'
//@[00:1524) ProgramSyntax
//@[00:0028) ├─TargetScopeSyntax
//@[00:0011) | ├─Token(Identifier) |targetScope|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0028) | └─StringSyntax
//@[14:0028) |   └─Token(StringComplete) |'subscription'|
//@[28:0030) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[00:0178) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName1|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0178) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
//@[02:0061) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0061) |   | └─FunctionCallSyntax
//@[09:0021) |   |   ├─IdentifierSyntax
//@[09:0021) |   |   | └─Token(Identifier) |subscription|
//@[21:0022) |   |   ├─Token(LeftParen) |(|
//@[22:0060) |   |   ├─FunctionArgumentSyntax
//@[22:0060) |   |   | └─StringSyntax
//@[22:0060) |   |   |   └─Token(StringComplete) |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
//@[60:0061) |   |   └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[00:0178) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName2|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0178) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
//@[02:0061) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0061) |   | └─FunctionCallSyntax
//@[09:0021) |   |   ├─IdentifierSyntax
//@[09:0021) |   |   | └─Token(Identifier) |subscription|
//@[21:0022) |   |   ├─Token(LeftParen) |(|
//@[22:0060) |   |   ├─FunctionArgumentSyntax
//@[22:0060) |   |   | └─StringSyntax
//@[22:0060) |   |   |   └─Token(StringComplete) |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
//@[60:0061) |   |   └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
//@[00:0140) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName3|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0140) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: subscription()
//@[02:0023) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0023) |   | └─FunctionCallSyntax
//@[09:0021) |   |   ├─IdentifierSyntax
//@[09:0021) |   |   | └─Token(Identifier) |subscription|
//@[21:0022) |   |   ├─Token(LeftParen) |(|
//@[22:0023) |   |   └─Token(RightParen) |)|
//@[23:0024) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
//@[00:0140) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName4|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0140) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: subscription()
//@[02:0023) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0023) |   | └─FunctionCallSyntax
//@[09:0021) |   |   ├─IdentifierSyntax
//@[09:0021) |   |   | └─Token(Identifier) |subscription|
//@[21:0022) |   |   ├─Token(LeftParen) |(|
//@[22:0023) |   |   └─Token(RightParen) |)|
//@[23:0024) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
//@[00:0116) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName5|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0116) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
//@[00:0148) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0040) | ├─IdentifierSyntax
//@[07:0040) | | └─Token(Identifier) |resourceGroupModuleDuplicateName1|
//@[41:0070) | ├─StringSyntax
//@[41:0070) | | └─Token(StringComplete) |'modules/resourceGroup.bicep'|
//@[71:0072) | ├─Token(Assignment) |=|
//@[73:0148) | └─ObjectSyntax
//@[73:0074) |   ├─Token(LeftBrace) |{|
//@[74:0075) |   ├─Token(NewLine) |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[02:0042) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0042) |   | └─StringSyntax
//@[08:0042) |   |   └─Token(StringComplete) |'resourceGroupModuleDuplicateName'|
//@[42:0043) |   ├─Token(NewLine) |\n|
  scope: resourceGroup('RG')
//@[02:0028) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0028) |   | └─FunctionCallSyntax
//@[09:0022) |   |   ├─IdentifierSyntax
//@[09:0022) |   |   | └─Token(Identifier) |resourceGroup|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0027) |   |   ├─FunctionArgumentSyntax
//@[23:0027) |   |   | └─StringSyntax
//@[23:0027) |   |   |   └─Token(StringComplete) |'RG'|
//@[27:0028) |   |   └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
//@[00:0148) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0040) | ├─IdentifierSyntax
//@[07:0040) | | └─Token(Identifier) |resourceGroupModuleDuplicateName2|
//@[41:0070) | ├─StringSyntax
//@[41:0070) | | └─Token(StringComplete) |'modules/resourceGroup.bicep'|
//@[71:0072) | ├─Token(Assignment) |=|
//@[73:0148) | └─ObjectSyntax
//@[73:0074) |   ├─Token(LeftBrace) |{|
//@[74:0075) |   ├─Token(NewLine) |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[02:0042) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0042) |   | └─StringSyntax
//@[08:0042) |   |   └─Token(StringComplete) |'resourceGroupModuleDuplicateName'|
//@[42:0043) |   ├─Token(NewLine) |\n|
  scope: resourceGroup('RG')
//@[02:0028) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0028) |   | └─FunctionCallSyntax
//@[09:0022) |   |   ├─IdentifierSyntax
//@[09:0022) |   |   | └─Token(Identifier) |resourceGroup|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0027) |   |   ├─FunctionArgumentSyntax
//@[23:0027) |   |   | └─StringSyntax
//@[23:0027) |   |   |   └─Token(StringComplete) |'RG'|
//@[27:0028) |   |   └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module unsupportedScopeManagementGroup 'modules/managementGroup.bicep' = {
//@[00:0149) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0038) | ├─IdentifierSyntax
//@[07:0038) | | └─Token(Identifier) |unsupportedScopeManagementGroup|
//@[39:0070) | ├─StringSyntax
//@[39:0070) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[71:0072) | ├─Token(Assignment) |=|
//@[73:0149) | └─ObjectSyntax
//@[73:0074) |   ├─Token(LeftBrace) |{|
//@[74:0075) |   ├─Token(NewLine) |\n|
  name: 'unsupportedScopeManagementGroup'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'unsupportedScopeManagementGroup'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: managementGroup('MG')
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0030) |   | └─FunctionCallSyntax
//@[09:0024) |   |   ├─IdentifierSyntax
//@[09:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0029) |   |   ├─FunctionArgumentSyntax
//@[25:0029) |   |   | └─StringSyntax
//@[25:0029) |   |   |   └─Token(StringComplete) |'MG'|
//@[29:0030) |   |   └─Token(RightParen) |)|
//@[30:0031) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module singleRgModule 'modules/passthrough.bicep' = {
//@[00:0143) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0021) | ├─IdentifierSyntax
//@[07:0021) | | └─Token(Identifier) |singleRgModule|
//@[22:0049) | ├─StringSyntax
//@[22:0049) | | └─Token(StringComplete) |'modules/passthrough.bicep'|
//@[50:0051) | ├─Token(Assignment) |=|
//@[52:0143) | └─ObjectSyntax
//@[52:0053) |   ├─Token(LeftBrace) |{|
//@[53:0054) |   ├─Token(NewLine) |\n|
  name: 'single-rg'
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0019) |   | └─StringSyntax
//@[08:0019) |   |   └─Token(StringComplete) |'single-rg'|
//@[19:0020) |   ├─Token(NewLine) |\n|
  params: {
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0036) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0012) |   |   ├─Token(NewLine) |\n|
    myInput: 'stuff'
//@[04:0020) |   |   ├─ObjectPropertySyntax
//@[04:0011) |   |   | ├─IdentifierSyntax
//@[04:0011) |   |   | | └─Token(Identifier) |myInput|
//@[11:0012) |   |   | ├─Token(Colon) |:|
//@[13:0020) |   |   | └─StringSyntax
//@[13:0020) |   |   |   └─Token(StringComplete) |'stuff'|
//@[20:0021) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
  scope: resourceGroup('test')
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0030) |   | └─FunctionCallSyntax
//@[09:0022) |   |   ├─IdentifierSyntax
//@[09:0022) |   |   | └─Token(Identifier) |resourceGroup|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0029) |   |   ├─FunctionArgumentSyntax
//@[23:0029) |   |   | └─StringSyntax
//@[23:0029) |   |   |   └─Token(StringComplete) |'test'|
//@[29:0030) |   |   └─Token(RightParen) |)|
//@[30:0031) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module singleRgModule2 'modules/passthrough.bicep' = {
//@[00:0138) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0022) | ├─IdentifierSyntax
//@[07:0022) | | └─Token(Identifier) |singleRgModule2|
//@[23:0050) | ├─StringSyntax
//@[23:0050) | | └─Token(StringComplete) |'modules/passthrough.bicep'|
//@[51:0052) | ├─Token(Assignment) |=|
//@[53:0138) | └─ObjectSyntax
//@[53:0054) |   ├─Token(LeftBrace) |{|
//@[54:0055) |   ├─Token(NewLine) |\n|
  name: 'single-rg2'
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0020) |   | └─StringSyntax
//@[08:0020) |   |   └─Token(StringComplete) |'single-rg2'|
//@[20:0021) |   ├─Token(NewLine) |\n|
  params: {
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |params|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0036) |   | └─ObjectSyntax
//@[10:0011) |   |   ├─Token(LeftBrace) |{|
//@[11:0012) |   |   ├─Token(NewLine) |\n|
    myInput: 'stuff'
//@[04:0020) |   |   ├─ObjectPropertySyntax
//@[04:0011) |   |   | ├─IdentifierSyntax
//@[04:0011) |   |   | | └─Token(Identifier) |myInput|
//@[11:0012) |   |   | ├─Token(Colon) |:|
//@[13:0020) |   |   | └─StringSyntax
//@[13:0020) |   |   |   └─Token(StringComplete) |'stuff'|
//@[20:0021) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
  scope: singleRgModule
//@[02:0023) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0023) |   | └─VariableAccessSyntax
//@[09:0023) |   |   └─IdentifierSyntax
//@[09:0023) |   |     └─Token(Identifier) |singleRgModule|
//@[23:0024) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
